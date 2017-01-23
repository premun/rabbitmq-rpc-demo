using System;
using System.Threading;
using ForeCastle.Communication.Definitions;
using ForeCastle.Communication.Listeners;
using ForeCastle.Communication.Listeners.Factory;
using ForeCastle.Library;
using ForeCastle.Library.Identifier;

namespace ForeCastle.Consumer
{
	public class Consumer : IConsumer
	{
		private readonly ILogger _logger;
		private readonly IListener<IConsumer> _listener;

		public Consumer(
			ILogger logger,
			IListenerFactory listenerFactory,
			Identifier workerIdentifier)
		{
			_logger = logger;
			_listener = listenerFactory.CreateListener<IConsumer>();
			_listener.StartListen(this);

			_logger.Info("Consumer #" + workerIdentifier.Id + " started");
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
				_logger.Info("Consumer killed");
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
