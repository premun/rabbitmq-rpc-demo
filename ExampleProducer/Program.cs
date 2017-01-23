using System;
using Autofac;
using RabbitMQDemo.Communication.Autofac;
using RabbitMQDemo.Library;
using RabbitMQDemo.Library.Autofac;

namespace RabbitMQDemo.ExampleProducer
{
	public class Program
	{
		private static IContainer InitContainer()
		{
			var builder = new ContainerBuilder();

			var id = new ExampleProducerIdentifier();

			builder.RegisterModule(new LibraryModule(null));
			builder.RegisterModule(new CommunicationModule(id.RpcName));
			builder.Register(c => id).As<Identifier>();

			return builder.Build();
		}

		public static void Main(string[] args)
		{
			new ExampleProducer(InitContainer()).Run();
			Environment.Exit(0);
		}
	}
}
