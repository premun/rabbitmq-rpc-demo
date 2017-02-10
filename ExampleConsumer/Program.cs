using System;
using Autofac;
using RabbitMQDemo.Communication.Autofac;
using RabbitMQDemo.Library;
using RabbitMQDemo.Library.Autofac;

namespace RabbitMQDemo.ExampleConsumer
{
	public class Program
	{
		private static IContainer InitAutofacContainer(string name)
		{
			var builder = new ContainerBuilder();

			var id = new ExampleConsumerIdentifier(name);

			builder.RegisterModule(new LibraryModule(id.RpcName));
			builder.RegisterModule(new CommunicationModule(id.RpcName));
			builder.RegisterType<ExampleConsumer>();
			builder.Register(c => id).As<Identifier>();

			return builder.Build();
		}

		public static void Main(string[] args)
		{
			IContainer container = InitAutofacContainer(args[0]);

			try
			{
				var consumer = container.Resolve<ExampleConsumer>();
				consumer.Start();
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
