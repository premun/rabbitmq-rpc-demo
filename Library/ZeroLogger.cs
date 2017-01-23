using System;

namespace ForeCastle.Library
{
	/// <summary>
	/// Logger that has no real output. Can be used for testing purposes.
	/// </summary>
	public class ZeroLogger : ILogger
	{
		public string Name
		{
			get { return "ZeroLogger"; }
		}

		public void Debug(string message)
		{
		}

		public void Debug(string message, params object[] args)
		{
		}

		public void Debug(string message, Exception exception, object[] args)
		{
		}

		public void Info(string message)
		{
		}

		public void Info(string message, params object[] args)
		{
		}

		public void Info(string message, Exception exception, object[] args)
		{
		}

		public void Warn(string message)
		{
		}

		public void Warn(string message, params object[] args)
		{
		}

		public void Warn(string message, Exception exception, object[] args)
		{
		}

		public void Error(string message)
		{
		}

		public void Error(string message, params object[] args)
		{
		}

		public void Error(string message, Exception exceptio, object[] argsn)
		{
		}

		public void Fatal(string message)
		{
		}

		public void Fatal(string message, params object[] args)
		{
		}

		public void Fatal(string message, Exception exception, object[] args)
		{
		}
	}
}
