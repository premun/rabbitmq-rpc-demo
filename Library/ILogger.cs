using System;
using System.ComponentModel;

namespace RabbitMQDemo.Library
{
	/// <summary>
	/// Interface of logger. Provide logging functionality of forecastle applications.
	/// </summary>
	public interface ILogger
	{
		string Name { get; }

		#region Debug

		void Debug([Localizable(false)] string message);

		void Debug([Localizable(false)] string message, params object[] args);

		void Debug([Localizable(false)] string message, Exception exception, object[] args);

		#endregion

		#region Info

		void Info([Localizable(false)] string message);

		void Info([Localizable(false)] string message, params object[] args);
		
		void Info([Localizable(false)] string message, Exception exception, object[] args);

		#endregion

		#region Warn

		void Warn([Localizable(false)] string message);
		
		void Warn([Localizable(false)] string message, params object[] args);

		void Warn([Localizable(false)] string message, Exception exception, object[] args);

		#endregion

		#region Error

		void Error([Localizable(false)] string message);

		void Error([Localizable(false)] string message, params object[] args);

		void Error([Localizable(false)] string message, Exception exception, object[] args);

		#endregion

		#region Fatal

		void Fatal([Localizable(false)] string message);

		void Fatal([Localizable(false)] string message, params object[] args);

		void Fatal([Localizable(false)] string message, Exception exception, object[] args);

		#endregion
	}
}