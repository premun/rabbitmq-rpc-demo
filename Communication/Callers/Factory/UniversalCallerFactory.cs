using ForeCastle.Communication.CommunicationService;

namespace ForeCastle.Communication.Callers.Factory
{
	/// <summary>
	/// Factory creating universal callers.
	/// </summary>
	public class UniversalCallerFactory : ICallerFactory
	{
		private readonly ICommunicationService _communicationService;

		public UniversalCallerFactory(
			ICommunicationService communicationService)
		{
			_communicationService = communicationService;
		}

		public T CreateCaller<T>(string targetQueueName)
		{
			return UniversalCallerProxy<T>.Create(_communicationService, targetQueueName);
		}
	}
}
