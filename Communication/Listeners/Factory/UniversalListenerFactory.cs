using ForeCastle.Communication.CommunicationService;

namespace ForeCastle.Communication.Listeners.Factory
{
	/// <summary>
	/// Factory creating universal listeners. 
	/// TODO: more detailed comments (every listener factory creates listeners... what are they or why we need factory?)
	/// </summary>
	public class UniversalListenerFactory : IListenerFactory
	{
		private readonly ICommunicationService _communicationService;

		public UniversalListenerFactory(
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
