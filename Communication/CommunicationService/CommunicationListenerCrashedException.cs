namespace RabbitMQDemo.Communication.CommunicationService
{
	/// <summary>
	/// Exception thrown at RPC-client side when the RPC-listener crashed. 
	/// Contains a details(exception message and stack trace) about listener-side exception. 
	/// </summary>
	public class CommunicationListenerCrashedException : CommunicationException
	{
		private readonly string _exceptionMessage;
		private readonly string _exceptionStackTrace;

		public CommunicationListenerCrashedException(string exceptionMessage, string exceptionStackTrace)
			: base("Calling function caused crash of the listener. Exception message: " + exceptionMessage)
		{
			_exceptionMessage = exceptionMessage;
			_exceptionStackTrace = exceptionStackTrace;
		}

		/// <summary>
		/// Listener side exception message.
		/// </summary>
		public string ExceptionMessage
		{ 
			get { return _exceptionMessage; }
		}

		/// <summary>
		/// Listener side exception stack trace.
		/// </summary>
		public string ExceptionStackTrace
		{ 
			get { return _exceptionStackTrace; }
		}
	}
}
