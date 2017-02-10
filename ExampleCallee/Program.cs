using System;
using Autofac;
using RabbitMQDemo.Communication.Autofac;
using RabbitMQDemo.Library;
using RabbitMQDemo.Library.Autofac;

namespace RabbitMQDemo.ExampleCallee
{
	public class Program
	{
		private static IContainer InitAutofacContainer(string name)
		{
			var builder = new ContainerBuilder();

			var id = new ExampleCalleeIdentifier(name);

			builder.RegisterModule(new LibraryModule(id));
			builder.RegisterModule(new CommunicationModule(id.RpcName));
			builder.Register(c => id).As<Identifier>();
			builder.RegisterType<ExampleCallee>();

			return builder.Build();
		}
		
		public static void Main(string[] args)
		{
			IContainer container = InitAutofacContainer(args[0]);

			try
			{
				var consumer = container.Resolve<ExampleCallee>();
				consumer.Start();
			}
			catch (Exception ex)
			{
				var logger = container.Resolve<ILogger>();
				logger?.Fatal("ExampleCallee has failed with fatal error: {0}.", ex);
				Environment.Exit(1);
			}
		}
	}
}
