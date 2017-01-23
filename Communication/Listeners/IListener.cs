using System;
using System.Threading;
using ForeCastle.Communication.CommunicationService;

namespace ForeCastle.Communication.Listeners
{
	/// <summary>
	/// Listen for RPC calls for interface of T on the given queue. 
	/// All calls are called on the given instance of T. 
	/// </summary>
	/// <typeparam name="T">Type of called interface.</typeparam>
	public interface IListener<in T> : IDisposable
	{
		/// <summary>
		/// Start listen and all calls are calls on the instance.
		/// </summary>
		/// <param name="instance">Instance on which will be called all incoming calls.</param>
		void StartListen(T instance);

		/// <summary>
		/// Stop listen and dispose the resources.
		/// </summary>
		void StopListen();

		/// <summary>
		/// Return listeners`s thread which listener use for listening.
		/// </summary>
		Thread ListeningThread { get; }

		/// <summary>
		/// StartListening is raised whenever the listener starts listen. 
		/// </summary>
		event EventHandler StartListening;

		/// <summary>
		/// ListeningThreadFailed is raised  whenever the listener thread crashed with exception. 
		/// </summary>
		event EventHandler<ListeningThreadFailedEventArgs> ListeningThreadFailed;
	}
}
