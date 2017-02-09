using System;
using RabbitMQDemo.Communication.Listeners;
using RabbitMQDemo.Communication.Listeners.Factory;
using RabbitMQDemo.CommunicationInterface;
using RabbitMQDemo.Library;

namespace RabbitMQDemo.ExampleCallee
{
	public class ExampleCallee : IExampleInterface, IDisposable
	{
		private readonly ILogger _logger;
		private readonly Identifier _identifier;
		private readonly IListener<IExampleInterface> _listener;

		public ExampleCallee(
			ILogger logger,
			IListenerFactory listenerFactory,
			Identifier identifier)
		{
			_logger = logger;
			_identifier = identifier;
			_listener = listenerFactory.CreateListener<IExampleInterface>();
		}

		public void Start()
		{
			_listener.StartListen(this);
			_logger.Info("ExampleCallee #" + _identifier.Id + " started");
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
