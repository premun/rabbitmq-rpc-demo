using RabbitMQDemo.Communication.CommunicationService;

namespace RabbitMQDemo.Communication.Listeners.Factory
{
	/// <summary>
	/// Factory creating universal listeners.
	/// </summary>
	public class ListenerFactory : IListenerFactory
	{
		private readonly ICommunicationService _communicationService;

		public ListenerFactory(
			ICommunicationService communicationService)
		{
			_communicationService = communicationService;
		}

		public IListener<T> CreateListener<T>()
		{
			return new UniversalListener<T>(_communicationService);
		}
	}
}
