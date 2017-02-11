using System;

namespace RabbitMQDemo.Communication.CommunicationService.Exceptions

{
	/// <summary>
	/// Exception is thrown when something's wrong in communication library. 
	/// </summary>
	public class CommunicationException : Exception
	{
		public CommunicationException(string message)
			: base(message)
		{
			/* empty */
		}

		public CommunicationException(string message, Exception innerException)
			: base(message, innerException)
		{
			/* empty */
		}
	}
}
