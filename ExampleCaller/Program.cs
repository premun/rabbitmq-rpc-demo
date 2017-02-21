using System;
using Autofac;
using RabbitMQDemo.Communication.Autofac;
using RabbitMQDemo.Communication.Callers.Factory;
using RabbitMQDemo.Library;
using RabbitMQDemo.Library.Autofac;

namespace RabbitMQDemo.ExampleCaller
{
	public class Program
	{
		public static readonly IContainer Container = InitContainer();

		public static void Main(string[] args)
		{
			new ExampleCaller().Run();
			Environment.Exit(0);
		}

		private static IContainer InitContainer()
		{
			var builder = new ContainerBuilder();

			var id = new ExampleCallerIdentifier();

			builder.RegisterModule(new LibraryModule(null));
			builder.RegisterModule(new CommunicationModule(id.RpcName));
			builder.Register(c => id).As<Identifier>();

			return builder.Build();
		}
	}
}
