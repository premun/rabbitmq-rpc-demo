using System;
using System.Threading;
using ForeCastle.Library;
using ForeCastle.Library.Identifier;
using RabbitMQDemo.Communication.Definitions;
using RabbitMQDemo.Communication.Listeners;
using RabbitMQDemo.Communication.Listeners.Factory;

namespace RabbitMQDemo.ExampleConsumer
{
	public class ExampleConsumer : IExampleConsumer
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

		public void Kill()
		{
			_logger.Info("Killing worker...");

			var killerThread = new Thread(() =>
			{
				_listener.StopListen();
				Thread.Sleep(1000);
				_logger.Info("ExampleConsumer killed");
				Environment.Exit(0);
			});

			killerThread.Start();
		}

		public int Multiply(int x, int y)
		{
			return x * y;
		}
	}
}
