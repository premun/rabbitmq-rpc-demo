using System;
using System.IO;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMQDemo.Communication.CommunicationService.Exceptions;
using RabbitMQDemo.Library;

namespace RabbitMQDemo.Communication.CommunicationService.Rabbit
{
	/// <summary>
	/// Implementation for IRpcCommunicationListener using RabbitMQ.
	/// Listen for RPC packets on the given queue. 
	/// </summary>
	public class RabbitRpcCommunicationListener : IRpcCommunicationListener
	{
		private class StopListenException : Exception
		{
		}

		private readonly ILogger _logger;

		private readonly IConnection _rabbitConnection;

		private readonly string _listeningQueueName;

		private IModel _privateListeningChannel;

		private IModel ListeningChannel
		{
			get
			{
				if (_disposed)
				{
					throw new StopListenException();
				}
				return _privateListeningChannel;
			}

			set
			{ 
				_privateListeningChannel = value; 
			}
		}

		private bool _disposed;

		public string ListeningQueueName => _listeningQueueName;

		public Func<string, RpcPacket, RpcPacket> ListeningFunction { get; }

		public event EventHandler ListeningStarted;

		public event EventHandler<ListeningThreadFailedEventArgs> ListeningThreadFailed;

		public event EventHandler ListeningStopped;

		public Thread ListeningThread { get; private set; }

		public RabbitRpcCommunicationListener(
			ILogger logger, 
			IConnection rabbitConnection,
			string listeningQueueName,
			Func<string, RpcPacket, RpcPacket> listeningFunction)
		{
			_logger = logger;
			_rabbitConnection = rabbitConnection;
			_listeningQueueName = listeningQueueName;
			ListeningFunction = listeningFunction;
		}

		private void OnListeningThreadFailed(ListeningThreadFailedEventArgs e)
		{
			ListeningThreadFailed?.Invoke(this, e);
		}

		private void OnStartListening()
		{
			ListeningStarted?.Invoke(this, EventArgs.Empty);
		}

		private void OnStopListening()
		{
			ListeningStopped?.Invoke(this, EventArgs.Empty);
		}


		public void StartListening()
		{
			_disposed = false;
			ListeningThread = new Thread(Run);
			ListeningThread.Start();
		}

		private void InitRabbitChannel()
		{
			ListeningChannel = _rabbitConnection.CreateModel();

			try
			{
				// Declaring queue will throw an OperationInterruptedException if queue already exists
				ListeningChannel.QueueDeclare(ListeningQueueName, false, true, false, null);
			}
			catch (OperationInterruptedException ex)
			{
				throw new CommunicationException(
					$"Listener could not start listening on queue `{ListeningQueueName}`. See inner exception for details.", ex);
			}

			// RabbitMQ properties
			ListeningChannel.BasicQos(0, 1, false);
		}

		private void Run()
		{
			_logger.Info("Starting listener thread...");

			try
			{
				InitRabbitChannel();

				var consumer = new QueueingBasicConsumer(ListeningChannel);
				ListeningChannel.BasicConsume(ListeningQueueName, false, consumer);

				_logger.Info("Channel started. Awaiting RPC calls on `{0}`...", ListeningQueueName);

				OnStartListening();
			
				while (true)
				{
					BasicDeliverEventArgs messageReceived;

					try
					{
						messageReceived = consumer.Queue.Dequeue();
					}
					catch (EndOfStreamException)
					{
						// This is what happens when channel is closed from outside
						throw new StopListenException();
					}
						
					// Response rabbit parameters + correlation Identifier
					IBasicProperties recievedProperties = messageReceived.BasicProperties;
					IBasicProperties replyProperties = ListeningChannel.CreateBasicProperties();
					replyProperties.CorrelationId = recievedProperties.CorrelationId;

					RabbitRpcPacket recievedRpcPacket;
					
					try
					{
						recievedRpcPacket = Serializer.Deserialize<RabbitRpcPacket>(messageReceived.Body);
					}
					catch (Exception ex)
					{
						// If comes bad packet log it and continue in listening.
						_logger.Error($"Could not dematerialized RabbitRpcPacket. Exception stack trace: {ex}");
						continue;
					}

					var replyRpcPacket = new RabbitRpcPacket
					{
						From = _listeningQueueName,
					};
					
					// Process call
					try
					{
						replyRpcPacket.Body = ListeningFunction(recievedRpcPacket.From, recievedRpcPacket.Body);
					}
					catch (Exception ex)
					{
						// If listening function crashed log it and send back exception details.
						_logger.Error($"Couldn't process RPC call (callee side): {ex}", ex);

						// Create error response
						replyRpcPacket.Error = true; 
						replyRpcPacket.ExceptionMessage = ex.Message;
						replyRpcPacket.ExceptionStackTrace = ex.ToString();
					}
					finally
					{
						// If reply is expected, then send it.
						if (!string.IsNullOrEmpty(recievedProperties.ReplyTo))
						{
							var response = Serializer.Serialize(replyRpcPacket);
							ListeningChannel.BasicPublish(string.Empty, recievedProperties.ReplyTo, replyProperties, response);
						}

						ListeningChannel.BasicAck(messageReceived.DeliveryTag, false);
					}
				}
			}
			catch (StopListenException)
			{
				_logger.Info($"Listening on queue `{ListeningQueueName}` stopped.");
				OnStopListening();
			}
			catch (Exception ex)
			{
				_logger.Error("Listening error: {0}", ex.ToString());

				Dispose();
				OnListeningThreadFailed(new ListeningThreadFailedEventArgs { ListenException = ex });
			}
		}

		public void StopListening()
		{
			Dispose();
		}

		public void Dispose()
		{
			lock (this)
			{
				if (_disposed)
				{
					return;
				}

				_disposed = true;
				_privateListeningChannel?.Dispose();
				_privateListeningChannel = null;

				GC.SuppressFinalize(this);
			}
		}
	}
}
