using System;
using System.Threading;

namespace ForeCastle.Communication.CommunicationService
{
	/// <summary>
	/// Event arguments for listening thread failure.
	/// </summary>
	public class ListeningThreadFailedEventArgs : EventArgs
	{
		public Exception ListenException { get; set; }
	}

	/// <summary>
	/// Listen for RPC packets on the given queue. 
	/// </summary>
	public interface IRpcCommunicationListener : IDisposable
	{
		/// <summary>
		/// Listening queue name
		/// </summary>
		string ListeningQueueName { get; }

		/// <summary>
		/// Function provides the response logic.
		/// Function has two arguments sender queue name and received packet and returns the reply packet.
		/// </summary>
		Func<string, RpcCommunicationPacket, RpcCommunicationPacket> ListeningFunction { get; }

		/// <summary>
		/// Return listeners`s thread which listener use for listening.
		/// </summary>
		Thread ListeningThread { get; }

		/// <summary>
		/// StartListening is raised  whenever the listener starts listen. 
		/// </summary>
		event EventHandler StartListening;

		/// <summary>
		/// ListeningThreadFailed is raised  whenever the listener thread crashed with exception. 
		/// </summary>
		event EventHandler<ListeningThreadFailedEventArgs> ListeningThreadFailed;

		/// <summary>
		/// StopListening is raised  whenever the listener stop listening successfully. 
		/// It is not raised  when the listener thread crashed with exception. 
		/// </summary>
		event EventHandler StopListening;

		/// <summary>
		/// Start listen.
		/// </summary>
		void StartListen();

		/// <summary>
		/// Stop listen and dispose the resources.
		/// </summary>
		void StopListen();
	}
}
