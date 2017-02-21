using System;
using System.Threading;
using RabbitMQDemo.Communication.Consumers;
using RabbitMQDemo.CommunicationInterface;
using RabbitMQDemo.Library;

namespace RabbitMQDemo.ExampleConsumer
{
	public class ExampleConsumer : IDisposable
	{
		private readonly ILogger _logger;
		private readonly IConsumer<ExampleMessage> _consumer;

		public ExampleConsumer(ILogger logger, IConsumer<ExampleMessage> consumer)
		{
			_logger = logger;
			_consumer = consumer;
		}

		public void Start()
		{
			_logger.Info("ExampleConsumer started");
			_consumer.StartConsume();

			while (true)
			{
				ExampleMessage message;

				if (!_consumer.Dequeue(out message))
				{
					break;
				}

				ProcessMessage(message);
			}

			_logger.Info("ExampleConsumer ended");
		}

		private void ProcessMessage(ExampleMessage message)
		{
			_logger.Info($"Received message: {message}");
			_consumer.Ack(message);
			Thread.Sleep(1000);
		}

		public void Dispose()
		{
			_consumer?.Dispose();
		}
	}
}
