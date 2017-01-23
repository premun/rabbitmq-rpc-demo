using System;
using Autofac;
using ForeCastle.Communication.Autofac;
using ForeCastle.Library;
using ForeCastle.Library.Autofac;
using ForeCastle.Library.Identifier;

namespace ForeCastle.Consumer
{
	public class Program
	{
		private static IContainer InitAutofacContainer(string workerId)
		{
			var builder = new ContainerBuilder();

			var id = new ConsumerIdentifier(workerId);

			builder.RegisterModule(new LibraryModule(id.RpcName));
			builder.RegisterModule(new CommunicationModule(id.RpcName));
			builder.Register(c => id).As<Identifier>();
			builder.RegisterType<Consumer>();

			return builder.Build();
		}
		
		public static void Main(string[] args)
		{
			ILogger logger = null;

			try
			{
				var container = InitAutofacContainer(args[0]);
				logger = container.Resolve<ILogger>();
				var consumer = container.Resolve<Consumer>();
			}
			catch (Exception ex)
			{
				logger?.Fatal("Consumer has failed with fatal error: {0}.", ex);
				Environment.Exit(1);
			}
		}
	}
}
