namespace RabbitMQDemo.Communication.CommunicationService.Exceptions
{
	/// <summary>
	/// Exception thrown at RPC-client side when the RPC-listener crashed. 
	/// Contains a details(exception message and stack trace) about listener-side exception. 
	/// </summary>
	public class CommunicationListenerCrashedException : CommunicationException
	{
		public CommunicationListenerCrashedException(string exceptionMessage, string exceptionStackTrace)
			: base("Calling function caused crash of the listener. Exception message: " + exceptionMessage)
		{
			ExceptionMessage = exceptionMessage;
			ExceptionStackTrace = exceptionStackTrace;
		}

		/// <summary>
		/// Listener side exception message.
		/// </summary>
		public string ExceptionMessage { get; }

		/// <summary>
		/// Listener side exception stack trace.
		/// </summary>
		public string ExceptionStackTrace { get; }
	}
}
