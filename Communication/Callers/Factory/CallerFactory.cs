using RabbitMQDemo.Communication.CommunicationService;
using RabbitMQDemo.Library;

namespace RabbitMQDemo.Communication.Callers.Factory
{
	/// <summary>
	/// Factory creating universal callers.
	/// </summary>
	public class CallerFactory : ICallerFactory
	{
		private readonly ICommunicationService _communicationService;

		public CallerFactory(
			ICommunicationService communicationService)
		{
			_communicationService = communicationService;
		}

		public T CreateCaller<T>(string targetQueueName)
		{
			return CallerProxy<T>.Create(_communicationService, targetQueueName);
		}

		public T CreateCaller<T>(Identifier target)
		{
			return CallerProxy<T>.Create(_communicationService, target.RpcName);
		}
	}
}
