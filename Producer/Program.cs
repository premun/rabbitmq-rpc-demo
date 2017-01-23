using System;
using Autofac;
using ForeCastle.Communication.Autofac;
using ForeCastle.Library.Autofac;
using ForeCastle.Library.Identifier;

namespace ForeCastle.Producer
{
	public class Program
	{
		private static IContainer InitContainer()
		{
			var builder = new ContainerBuilder();

			var id = new ProducerIdentifier();

			builder.RegisterModule(new LibraryModule(null));
			builder.RegisterModule(new CommunicationModule(id.RpcName));
			builder.Register(c => id).As<Identifier>();

			return builder.Build();
		}

		public static void Main(string[] args)
		{
			new Producer(InitContainer()).Run();
			Environment.Exit(0);
		}
	}
}
