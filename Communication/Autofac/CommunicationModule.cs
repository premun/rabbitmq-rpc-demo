using Autofac;
using RabbitMQDemo.Communication.Callers.Factory;
using RabbitMQDemo.Communication.CommunicationService;
using RabbitMQDemo.Communication.CommunicationService.Rabbit;
using RabbitMQDemo.Communication.Consumers;
using RabbitMQDemo.Communication.Listeners.Factory;
using RabbitMQDemo.Communication.Publishers;
using RabbitMQDemo.CommunicationInterface;
using RabbitMQDemo.Library;

namespace RabbitMQDemo.Communication.Autofac
{
	/// <summary>
	/// Autofac module for initialization IoC of Communication project
	/// </summary>
	public class CommunicationModule : Module
	{
		private readonly string _applicationQueueName;

		public CommunicationModule(string applicationQueueName = "RabbitMQDemoQueue")
		{
			_applicationQueueName = applicationQueueName;
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder.Register(c => new RabbitCommunicationService(c.Resolve<ILogger>(), _applicationQueueName))
				.As<ICommunicationService>()
				.SingleInstance();

			builder.RegisterType<CallerFactory>()
				.As<ICallerFactory>()
				.SingleInstance();

			builder.RegisterType<ListenerFactory>()
				.As<IListenerFactory>()
				.SingleInstance();
			
			builder.Register(c => new Publisher<ExampleMessage>(c.Resolve<ICommunicationService>(), _applicationQueueName))
				.As<IPublisher<ExampleMessage>>()
				.SingleInstance();
			
			builder.Register(c => new Consumer<ExampleMessage>(c.Resolve<ICommunicationService>(), _applicationQueueName))
				.As<IConsumer<ExampleMessage>>();
		}
	}
}
