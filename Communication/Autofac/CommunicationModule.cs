using Autofac;
using ForeCastle.Communication.Callers.Factory;
using ForeCastle.Communication.CommunicationService;
using ForeCastle.Communication.CommunicationService.Rabbit;
using ForeCastle.Communication.Listeners.Factory;
using ForeCastle.Communication.Utils.TypeOperations;
using ForeCastle.Communication.Utils.TypeOperations.TypeOperationsSuites;
using ForeCastle.Library;

namespace ForeCastle.Communication.Autofac
{
	/// <summary>
	/// Autofac module for initialization IoC of Communication project
	/// </summary>
	public class CommunicationModule : Module
	{
		private readonly string _applicationQueueName;

		public CommunicationModule(string applicationQueueName)
		{
			_applicationQueueName = applicationQueueName;
		}

		protected override void Load(ContainerBuilder builder)
		{
			// Initializes TypeOperationsService
			builder.Register(x => InitTypeOperationsService()).As<TypeOperationsService>().SingleInstance();

			builder.Register(c => new RabbitCommunicationService(c.Resolve<ILogger>(), _applicationQueueName))
				.As<ICommunicationService>()
				.SingleInstance();

			builder.RegisterType<UniversalCallerFactory>()
				.As<ICallerFactory>()
				.SingleInstance();

			builder.RegisterType<UniversalListenerFactory>()
				.As<IListenerFactory>()
				.SingleInstance();
		}

		private void RegisterSuite<T>(TypeOperationsService typeOperationsService, ITypeOperationsSuite<T> suite)
		{
			typeOperationsService.Register(suite);
			typeOperationsService.Register(new ListOperationsSuite<T>(suite));
		}

		private TypeOperationsService InitTypeOperationsService()
		{
			var service = new TypeOperationsService();
			RegisterSuite(service, new IntOperationsSuite());
			RegisterSuite(service, new StringOperationsSuite());
			RegisterSuite(service, new ByteOperationsSuite());
			RegisterSuite(service, new ByteArrayOperationSuite());
			RegisterSuite(service, new BoolOperationsSuite());

			return service;
		}
	}
}
