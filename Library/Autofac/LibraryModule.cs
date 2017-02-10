using Autofac;

namespace RabbitMQDemo.Library.Autofac
{
	/// <summary>
	/// Autofac module which initialize classes from library project.
	/// </summary>
	public class LibraryModule : Module
	{
		private readonly Identifier _identifier;

		public LibraryModule(Identifier identifier)
		{
			_identifier = identifier;
		}

		protected override void Load(ContainerBuilder builder)
		{
			if (string.IsNullOrEmpty(_identifier?.LogName))
			{
				builder.RegisterType<ZeroLogger>().As<ILogger>().SingleInstance();
			}
			else
			{
				builder.Register(c => new Logger(_identifier.LogName)).As<ILogger>().SingleInstance();
			}
		}
	}
}
