using System;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace RabbitMQDemo.Library
{
	/// <summary>
	/// NLog iplementation of logger. Provide logging functionality of forecastle applications
	/// </summary>
	public class Logger : ILogger
	{
		private readonly NLog.Logger _logger;

		public static NLog.Logger GetLogger(string logName)
		{
			var cfg = new XmlLoggingConfiguration(Config.Get["ProjectPath"] + Config.Get["NLog.config"]);

			// Redefine targets according to ForeCastle.properties LogPath variable
			var baseTarget = (FileTarget)cfg.FindTargetByName("baselog");
			var errorTarget = (FileTarget)cfg.FindTargetByName("errlog");

			var logPath = Config.Get["LogPath"];
			var archivePath = Config.Get["ArchiveLogPath"];

			baseTarget.FileName = $"{logPath}/{logName}.log";
			baseTarget.ArchiveFileName = $"{archivePath}/{logName}.{{###}}.log";
			errorTarget.FileName = $"{logPath}/{logName}.err.log";
			errorTarget.ArchiveFileName = $"{archivePath}/{logName}.{{###}}.log";

			if (Config.Get["LogInConsole"] == "true")
			{
				// Create the target that writes to console
				var consoleTarget = new ConsoleTarget
				{
					Layout = baseTarget.Layout,
					Name = "consolelog"
				};

				// Add rule to match the target
				var consoleRule = new LoggingRule("*", LogLevel.Debug, consoleTarget);
				cfg.LoggingRules.Add(consoleRule);
			}

			LogManager.Configuration = cfg;
			LogManager.ThrowExceptions = true;

			return LogManager.GetLogger(logName);
		}

		public Logger(string logName)
		{
			_logger = GetLogger(logName);
			Name = logName;
		}

		public string Name { get; }

		public void Debug(string message)
		{
			_logger.Debug(message);
		}

		public void Debug(string message, params object[] args)
		{
			_logger.Debug(message, args);
		}

		public void Debug(string message, Exception exception, object[] args)
		{
			_logger.Debug(message, exception, args);
		}

		public void Info(string message)
		{
			_logger.Info(message);
		}

		public void Info(string message, params object[] args)
		{
			_logger.Info(message, args);
		}

		public void Info(string message, Exception exception, object[] args)
		{
			_logger.Info(message, exception, args);
		}

		public void Warn(string message)
		{
			_logger.Warn(message);
		}

		public void Warn(string message, params object[] args)
		{
			_logger.Warn(message, args);
		}

		public void Warn(string message, Exception exception, object[] args)
		{
			_logger.Warn(message, exception, args);
		}

		public void Error(string message)
		{
			_logger.Error(message);
		}

		public void Error(string message, params object[] args)
		{
			_logger.Error(message, args);
		}

		public void Error(string message, Exception exception, object[] args)
		{
			_logger.Error(message, exception, args);
		}

		public void Fatal(string message)
		{
			_logger.Fatal(message);
		}

		public void Fatal(string message, params object[] args)
		{
			_logger.Fatal(message, args);
		}

		public void Fatal(string message, Exception exception, object[] args)
		{
			_logger.Fatal(message, exception, args);
		}
	}
}
