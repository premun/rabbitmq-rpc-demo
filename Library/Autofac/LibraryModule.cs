using Autofac;

namespace ForeCastle.Library.Autofac
{
	/// <summary>
	/// Autofac module which initialize classes from library project.
	/// </summary>
	public class LibraryModule : Module
	{
		private readonly string _logName;

		public LibraryModule(string logName)
		{
			_logName = logName;
		}

		protected override void Load(ContainerBuilder builder)
		{
			if (string.IsNullOrEmpty(_logName))
			{
				builder.RegisterType<ZeroLogger>().As<ILogger>().SingleInstance();
			}
			else
			{
				builder.Register(c => new Logger(_logName)).As<ILogger>().SingleInstance();
			}
		}
	}
}
