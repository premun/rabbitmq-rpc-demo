using System;
using Autofac;
using RabbitMQDemo.Communication.Autofac;
using RabbitMQDemo.CommunicationInterface;
using RabbitMQDemo.Library;
using RabbitMQDemo.Library.Autofac;

namespace RabbitMQDemo.ExamplePublisher
{
	public class Program
	{
		public static readonly IContainer Container = InitContainer();

		public static void Main(string[] args)
		{
			var publisher = Container.Resolve<ExamplePublisher>();
			publisher.Run();

			Environment.Exit(0);
		}

		private static IContainer InitContainer()
		{
			var id = new ExamplePublisherIdentifier();

			var builder = new ContainerBuilder();
			builder.RegisterModule(new LibraryModule(id));
			builder.RegisterModule(new CommunicationModule(nameof(ExampleMessage) + "_queue"));
			builder.Register(c => id).As<Identifier>();
			builder.RegisterType<ExamplePublisher>();

			return builder.Build();
		}
	}
}
