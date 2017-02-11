using System;
using Autofac;
using RabbitMQDemo.Communication.Autofac;
using RabbitMQDemo.Library.Autofac;

namespace RabbitMQDemo.Communication.test
{
	/// <summary>
	/// Base class for tests.
	/// </summary>
	public abstract class BaseTest
	{
		private readonly Lazy<IContainer> _autofacContainer = new Lazy<IContainer>(() =>
			{
				var builder = new ContainerBuilder();
				builder.RegisterModule(new LibraryModule(null));
				builder.RegisterModule(new CommunicationModule("Test"));
				return builder.Build();
			}
		);

		/// <summary>
		/// Autofac container.
		/// </summary>
		protected IContainer AutofacContainer => _autofacContainer.Value;
	}
}
