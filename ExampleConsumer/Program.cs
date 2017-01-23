﻿using System;
using Autofac;
using RabbitMQDemo.Communication.Autofac;
using RabbitMQDemo.Library;
using RabbitMQDemo.Library.Autofac;

namespace RabbitMQDemo.ExampleConsumer
{
	public class Program
	{
		private static IContainer InitAutofacContainer(string workerId)
		{
			var builder = new ContainerBuilder();

			var id = new ExampleConsumerIdentifier(workerId);

			builder.RegisterModule(new LibraryModule(id.RpcName));
			builder.RegisterModule(new CommunicationModule(id.RpcName));
			builder.Register(c => id).As<Identifier>();
			builder.RegisterType<ExampleConsumer>();

			return builder.Build();
		}
		
		public static void Main(string[] args)
		{
			ILogger logger = null;

			try
			{
				var container = InitAutofacContainer(args[0]);
				logger = container.Resolve<ILogger>();
				var consumer = container.Resolve<ExampleConsumer>();
			}
			catch (Exception ex)
			{
				logger?.Fatal("ExampleConsumer has failed with fatal error: {0}.", ex);
				Environment.Exit(1);
			}
		}
	}
}
