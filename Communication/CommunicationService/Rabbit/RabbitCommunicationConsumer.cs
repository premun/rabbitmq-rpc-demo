using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMQDemo.Communication.CommunicationService.Exceptions;

namespace RabbitMQDemo.Communication.CommunicationService.Rabbit
{
	/// <summary>
	/// RabbitMQ implementations of ICommunicationConsumer.
	/// Consume publish/consume packets on the given queue. 
	/// </summary>
	public class RabbitCommunicationConsumer : ICommunicationConsumer
	{
		private readonly IConnection _rabbitConnection;

		private IModel _channel;

		private QueueingBasicConsumer _consumer;

		private bool _disposed;

		public string ConsumeQueueName { get; }

		public ushort Prefetch { get; set; }

		public RabbitCommunicationConsumer(
			IConnection rabbitConnection,
			string consumeQueueName)
		{
			_rabbitConnection = rabbitConnection;
			ConsumeQueueName = consumeQueueName;
			Prefetch = 1;
		}

		public void StartConsume()
		{
			_disposed = false;
			_channel = _rabbitConnection.CreateModel();

			try
			{
				var queueArguments = new Dictionary<string, object>
				{
					{ "x-max-priority", 10 }
				};

				_channel.QueueDeclare(ConsumeQueueName, true, false, false, queueArguments);
				_channel.BasicQos(0, Prefetch, false);
			}
			catch (OperationInterruptedException ex)
			{
				string exceptionMessage =
					$"Could not start to consume on queue `{ConsumeQueueName}`. See inner exception for details.";
				throw new CommunicationException(exceptionMessage, ex);
			}

			_consumer = new QueueingBasicConsumer(_channel);
			_channel.BasicConsume(ConsumeQueueName, false, _consumer);
		}



		public bool Dequeue(int timeout, out PublishConsumePacket packet)
		{
			if (_consumer == null)
			{
				throw new CommunicationException("Could not process function because the consumer was not initialized or disposed.");
			}

			BasicDeliverEventArgs ea;
			if (_consumer.Queue.Dequeue(timeout, out ea))
			{
				packet = new PublishConsumePacket
				{
					Body = ea.Body,
					Tag = ea.DeliveryTag,
					Headers = ea.BasicProperties.Headers
				};

				return true;
			}

			packet = null;
			return false;
		}

		public void Ack(PublishConsumePacket packet)
		{
			if (_channel == null)
			{
				throw new CommunicationException("Could not process function because the consumer was not initialized or disposed.");
			}

			_channel.BasicAck(packet.Tag, false);
		}

		public void Nack(PublishConsumePacket packet)
		{
			if (_channel == null)
			{
				throw new CommunicationException("Could not process function because the consumer was not initialized or disposed.");
			}

			_channel.BasicNack(packet.Tag, false, false);
		}

		public void StopConsume()
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

				_channel?.Dispose();
				_channel = null;

				// Always set null
				_consumer = null;
				GC.SuppressFinalize(this);
			}
		}
	}
}
