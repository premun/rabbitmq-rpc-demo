using System;
using System.Threading;
using RabbitMQDemo.Communication.Definitions;
using RabbitMQDemo.Communication.Listeners;
using RabbitMQDemo.Communication.Listeners.Factory;
using RabbitMQDemo.Library;

namespace RabbitMQDemo.ExampleConsumer
{
	public class ExampleConsumer : IExampleConsumer, IDisposable
	{
		private readonly ILogger _logger;
		private readonly IListener<IExampleConsumer> _listener;

		public ExampleConsumer(
			ILogger logger,
			IListenerFactory listenerFactory,
			Identifier workerIdentifier)
		{
			_logger = logger;
			_listener = listenerFactory.CreateListener<IExampleConsumer>();
			_listener.StartListen(this);

			_logger.Info("ExampleConsumer #" + workerIdentifier.Id + " started");
		}

		public void DisplayMessage(string message)
		{
			_logger.Info("Received message: " + message);
		}

		public int Multiply(int x, int y)
		{
			return x * y;
		}

		public void Dispose()
		{
			_listener?.Dispose();
		}
	}
}
