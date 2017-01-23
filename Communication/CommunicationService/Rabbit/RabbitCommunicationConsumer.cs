using System;
using System.Collections.Generic;
using ForeCastle.Communication.Utils;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace ForeCastle.Communication.CommunicationService.Rabbit
{
	/// <summary>
	/// RabbitMQ implementations of ICommunicationConsumer.
	/// Consume publish/consume packets on the given queue. 
	/// </summary>
	public class RabbitCommunicationConsumer : ICommunicationConsumer
	{
		private readonly IConnection _rabbitConnection;

		private readonly string _consumeQueueName;

		private IModel _channel;

		private QueueingBasicConsumer _consumer;

		private bool _disposed;

		public string ConsumeQueueName
		{
			get { return _consumeQueueName; }
		}

		public ushort Prefetch { get; set; }

		public RabbitCommunicationConsumer(
			IConnection rabbitConnection,
			string consumeQueueName)
		{
			_rabbitConnection = rabbitConnection;
			_consumeQueueName = consumeQueueName;
			Prefetch = 1;
		}

		public void StartConsume(bool hasDeadLetterExchange = false)
		{
			_disposed = false;
			_channel = _rabbitConnection.CreateModel();

			try
			{
				var queueArguments = new Dictionary<string, object>
				{
					{ QueueUtils.PriorityArg, 10 }
				};

				if (hasDeadLetterExchange)
				{
					string dlx = QueueUtils.DeclareDeadLetterExchange(_channel, _consumeQueueName);
					queueArguments.Add(QueueUtils.DlxArg, dlx);
				}

				_channel.QueueDeclare(_consumeQueueName, true, false, false, queueArguments);
				_channel.BasicQos(0, Prefetch, false);
			}
			catch (OperationInterruptedException ex)
			{
				string exceptionMessage = string.Format(
											  "Could not start to consume on queue `{0}`. See inner exception for details.",
											  _consumeQueueName);
				throw new CommunicationException(exceptionMessage, ex);
			}

			_consumer = new QueueingBasicConsumer(_channel);
			_channel.BasicConsume(_consumeQueueName, false, _consumer);
		}



		public bool Dequeue(int timeout, out WorkCommunicationPacket packet)
		{
			if (_consumer == null)
			{
				throw new CommunicationException("Could not process function because the consumer was not initialized or disposed.");
			}
			BasicDeliverEventArgs ea;
			if (_consumer.Queue.Dequeue(timeout, out ea))
			{
				packet = new WorkCommunicationPacket
				{
					Body = ea.Body,
					Priority = ea.BasicProperties.Priority,
					Tag = ea.DeliveryTag,
					Headers = ea.BasicProperties.Headers
				};
				return true;
			}
			packet = null;
			return false;
		}

		public void Ack(WorkCommunicationPacket packet)
		{
			if (_channel == null)
			{
				throw new CommunicationException("Could not process function because the consumer was not initialized or disposed.");
			}
			_channel.BasicAck(packet.Tag, false);
		}

		public void Nack(WorkCommunicationPacket packet)
		{
			if (_channel == null)
			{
				throw new CommunicationException("Could not process function because the consumer was not initialized or disposed.");
			}
			_channel.BasicNack(packet.Tag, false, false);
			// _channel.BasicReject(packet.Tag, false);
		}

		public void StopConsume()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				if (_channel != null)
				{
					_channel.Dispose();
					_channel = null;
				}
				// Always set null
				_consumer = null;
				GC.SuppressFinalize(this);
			}
		}
	}
}
