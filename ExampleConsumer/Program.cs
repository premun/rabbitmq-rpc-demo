using System;
using Autofac;
using RabbitMQDemo.Communication.Autofac;
using RabbitMQDemo.CommunicationInterface;
using RabbitMQDemo.Library;
using RabbitMQDemo.Library.Autofac;

namespace RabbitMQDemo.ExampleConsumer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			IContainer container = InitContainer();

			try
			{
				using (var consumer = container.Resolve<ExampleConsumer>())
				{
					consumer.Start();
				}
			}
			catch (Exception ex)
			{
				var logger = container.Resolve<ILogger>();
				logger?.Fatal("ExampleConsumer has failed with fatal error: {0}.", ex);
				Environment.Exit(1);
			}
		}
		private static IContainer InitContainer()
		{
			var id = new ExampleConsumerIdentifier();

			var builder = new ContainerBuilder();
			builder.RegisterModule(new LibraryModule(id));
			builder.RegisterModule(new CommunicationModule(nameof(ExampleMessage) + "_queue"));
			builder.RegisterType<ExampleConsumer>();
			builder.Register(c => id).As<Identifier>();

			return builder.Build();
		}
	}
}
