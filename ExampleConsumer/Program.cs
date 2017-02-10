using System;
using Autofac;
using RabbitMQDemo.Communication.Autofac;
using RabbitMQDemo.Library;
using RabbitMQDemo.Library.Autofac;

namespace RabbitMQDemo.ExampleConsumer
{
	public class Program
	{
		private static IContainer InitAutofacContainer()
		{
			var builder = new ContainerBuilder();

			var id = new ExampleConsumerIdentifier();

			builder.RegisterModule(new LibraryModule(id));
			builder.RegisterModule(new CommunicationModule());
			builder.RegisterType<ExampleConsumer>();
			builder.Register(c => id).As<Identifier>();

			return builder.Build();
		}

		public static void Main(string[] args)
		{
			IContainer container = InitAutofacContainer();

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
	}
}
