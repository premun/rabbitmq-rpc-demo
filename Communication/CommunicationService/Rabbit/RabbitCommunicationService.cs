using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMQDemo.Communication.Utils;
using RabbitMQDemo.Library;

namespace RabbitMQDemo.Communication.CommunicationService.Rabbit
{
	/// <summary>
	/// Communication service implementation using RabbitMQ for message interchange.
	/// </summary>
	public class RabbitCommunicationService : ICommunicationService
	{
		private ConnectionFactory _rabbitConnectionFactory;
		private IConnection _rabbitConnection;

		private readonly int _connectionTimeout;
		private readonly ILogger _logger;
		private readonly string _processQueueName;

		public RabbitCommunicationService(ILogger logger, string processQueueName)
		{
			_logger = logger;
			_processQueueName = processQueueName;
			_connectionTimeout = int.Parse(Config.Get["RPC.timeout"]);
			InitRabbit();
		}

		private void InitRabbit()
		{
			_rabbitConnectionFactory = new ConnectionFactory
			{
				HostName = Config.Get["RabbitMQ.Broker.Host"],
				Port = int.Parse(Config.Get["RabbitMQ.Broker.Port"]),
				UserName = Config.Get["RabbitMQ.Broker.Username"],
				Password = Config.Get["RabbitMQ.Broker.Password"]
			};

			try
			{
				_rabbitConnection = _rabbitConnectionFactory.CreateConnection();
				_logger.Info("RabbitMQ connection established");
			}
			catch (BrokerUnreachableException)
			{
				_logger.Error("Couldn't connect to the RabbitMQ broker");
				throw;
			}
		}

		public void DeleteQueue(string queueName)
		{
			using (IModel channel = _rabbitConnection.CreateModel())
			{
				channel.QueueDelete(queueName);
			}
		}

		#region Publish/Consumer part

		public void Publish(string targetQueue, IEnumerable<WorkCommunicationPacket> packets, bool hasDeadLetterExchange = false)
		{
			using (IModel channel = _rabbitConnection.CreateModel())
			{
				var queueOptions = new Dictionary<string, object>
				{
					{ QueueUtils.PriorityArg, 10 }
				};

				try
				{
					if (hasDeadLetterExchange)
					{
						string dlx = QueueUtils.DeclareDeadLetterExchange(channel, targetQueue);
						queueOptions.Add(QueueUtils.DlxArg, dlx);
					}

					channel.QueueDeclare(targetQueue, true, false, false, queueOptions);
				}
				catch (OperationInterruptedException ex)
				{
					throw new CommunicationException("Already existent queue is not compatible with declared queue. See inner exception for details.", ex);
				}

				foreach (var packet in packets)
				{
					var properties = channel.CreateBasicProperties();
					properties.DeliveryMode = 2;
					properties.Priority = packet.Priority;
					properties.Headers = packet.Headers;
					channel.BasicPublish(string.Empty, targetQueue, properties, packet.Body);
				}
			}
		}

		public ICommunicationConsumer CreateConsumer(string consumeQueue)
		{
			return new RabbitCommunicationConsumer(_rabbitConnection, consumeQueue);
		}

		#endregion

		#region RPC part

		public bool ExistsListenerOnQueue(string queueName)
		{
			try
			{
				using (IModel channel = _rabbitConnection.CreateModel())
				{
					// Should throw exception if queue does not exist or exists on another connection.
					QueueDeclareOk queueInfo = channel.QueueDeclarePassive(queueName);

					// This code is execute if RPC call was from the same connection as consumer.
					if (queueInfo.ConsumerCount > 0)
					{
						return true;
					}

					return false;
				}
			}
			catch (OperationInterruptedException ex)
			{
				if (ex.ShutdownReason.ReplyCode == 405)
				{
					// Exclusive lock code, the queue is lock by another connection => there exist listener.
					return true;
				}

				if (ex.ShutdownReason.ReplyCode == 404)
				{
					// Passive declaration exception, queue does not exists
					return false;
				}

				throw new InvalidOperationException("Unknown code of OperationInterruptedException.", ex);
			}
		}

		private RabbitRpcPacket DequeueRpcPacket(QueueingBasicConsumer consumer, string correlationId)
		{
			// Blocking wait for the right response (there might be responses for more calls)
			while (true)
			{
				BasicDeliverEventArgs message;
				bool messageReceived = consumer.Queue.Dequeue(_connectionTimeout, out message);

				// No reply after x seconds
				if (!messageReceived)
				{
					throw new CommunicationException("Reply to RPC Call timed out.");
				}

				// Is it the message we are waiting for?
				// (it will be, but better be sure)
				if (message.BasicProperties.CorrelationId == correlationId)
				{
					return RabbitRpcPacket.DeserializeRpcPacket(message.Body);
				}

				// Correlation identifier mismatch, repeat Dequeue operation
				_logger.Error("Correlation Identifier mismatch!");
			}
		}

		public RpcCommunicationPacket CallRpc(string targetQueue, RpcCommunicationPacket sendPacket, RpcCallType type = RpcCallType.ExpectReply)
		{
			bool expectReply = type == RpcCallType.ExpectReply;
			try
			{
				if (!ExistsListenerOnQueue(targetQueue))
				{
					throw new CommunicationException($"Target queue `{targetQueue}` does not have any listener.");
				}

				RabbitRpcPacket sendRpcPacket = new RabbitRpcPacket
				{
					From = _processQueueName,
					Body = sendPacket
				};
					
				using (IModel channel = _rabbitConnection.CreateModel())
				{
					// Send the call and wait for the answer
					// Declare the response queue
					var replyQueueName = channel.QueueDeclare();
					var consumer = new QueueingBasicConsumer(channel);
					channel.BasicConsume(replyQueueName, true, consumer);

					// Set up rabbit properties (correlation id)
					// Add info inside message so they know where to reply
					var correlationId = Guid.NewGuid().ToString();

					var properties = channel.CreateBasicProperties();

					// If the reply is expected then fill the sender identification
					if (expectReply)
					{
						properties.CorrelationId = correlationId;
						properties.ReplyTo = replyQueueName;
					}

					// Send the call
					channel.BasicPublish(string.Empty, targetQueue, properties, Serializer.Serialize(sendRpcPacket));

					// If the reply is not expected then work is finished
					if (!expectReply)
					{
						return null;
					}

					RabbitRpcPacket recievedRpcPacket = DequeueRpcPacket(consumer, correlationId);

					if (recievedRpcPacket.Error)
					{
						// If listener server crashed with exception throws exception with details about it. 
						throw new CommunicationListenerCrashedException(recievedRpcPacket.ExceptionMessage, recievedRpcPacket.ExceptionStackTrace);
					}
					return recievedRpcPacket.Body;
				}
			}
			catch (CommunicationException)
			{
				// Do not catch exception (re-throw)
				// Target does not exist
				throw;
			}
			catch (Exception e)
			{
				// If something invalid happens throw exception with details.
				throw new CommunicationException("Communication in RabbitCommunicationService failed, see inner exception for details.", e);
			}
		}

		public IRpcCommunicationListener CreateRpcCommunicationListener(Func<string, RpcCommunicationPacket, RpcCommunicationPacket> listeningFunction)
		{
			var rpcCommunicationListener = new RabbitRpcCommunicationListener(_logger, _rabbitConnection, _processQueueName, listeningFunction);
			return rpcCommunicationListener;
		}

		#endregion

		public void Dispose()
		{
			if (_rabbitConnection != null && _rabbitConnection.IsOpen)
			{
				_rabbitConnection.Dispose();
				_rabbitConnection = null;
			}
		}
	}
}
