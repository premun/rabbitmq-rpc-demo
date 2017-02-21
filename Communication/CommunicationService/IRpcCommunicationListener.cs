using System;
using System.Threading;

namespace RabbitMQDemo.Communication.CommunicationService
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
		Func<string, RpcPacket, RpcPacket> ListeningFunction { get; }

		/// <summary>
		/// Return listeners`s thread which listener use for listening.
		/// </summary>
		Thread ListeningThread { get; }

		/// <summary>
		/// StartListening is raised  whenever the listener starts listen. 
		/// </summary>
		event EventHandler ListeningStarted;

		/// <summary>
		/// ListeningStopped is raised  whenever the listener stop listening successfully. 
		/// It is not raised  when the listener thread crashed with exception. 
		/// </summary>
		event EventHandler ListeningStopped;

		/// <summary>
		/// ListeningThreadFailed is raised  whenever the listener thread crashed with exception. 
		/// </summary>
		event EventHandler<ListeningThreadFailedEventArgs> ListeningThreadFailed;

		/// <summary>
		/// Start listen.
		/// </summary>
		void StartListening();

		/// <summary>
		/// Stop listen and dispose the resources.
		/// </summary>
		void StopListening();
	}
}
