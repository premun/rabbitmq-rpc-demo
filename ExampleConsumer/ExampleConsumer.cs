using System;
using RabbitMQDemo.Communication.Consumers;
using RabbitMQDemo.CommunicationInterface;
using RabbitMQDemo.Library;

namespace RabbitMQDemo.ExampleConsumer
{
	public class ExampleConsumer : IDisposable
	{
		private readonly ILogger _logger;
		private readonly Identifier _identifier;
		private readonly IConsumer<ExampleMessage> _consumer;

		public ExampleConsumer(
			ILogger logger,
			IConsumer<ExampleMessage> consumer,
			Identifier identifier)
		{
			_logger = logger;
			_identifier = identifier;
			_consumer = consumer;
		}

		public void Start()
		{
			_logger.Info($"ExampleConsumer #{_identifier.Id} started");
			_consumer.StartConsume();
		}

		public void Dispose()
		{
			_consumer?.Dispose();
		}
	}
}
