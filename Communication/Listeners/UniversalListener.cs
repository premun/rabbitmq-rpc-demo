using System;
using System.Threading;
using ForeCastle.Communication.CommunicationService;
using ForeCastle.Library;

namespace ForeCastle.Communication.Listeners
{
	/// <summary>
	/// Listen for RPC calls for interface of T on the given queue. 
	/// All calls are called on the given instance of T. 
	/// </summary>
	/// <typeparam name="T">Type of called interface.</typeparam>
	public class UniversalListener<T> : IListener<T>
	{
		private readonly IRpcCommunicationListener _listener;
		private T _instance;

        /// <summary>
        /// StartListening is raised whenever the listener starts listen.
        /// </summary>
        public event EventHandler StartListening
		{
			add { _listener.StartListening += value; }
			remove { _listener.StartListening -= value; }
		}

        /// <summary>
        /// ListeningThreadFailed is raised  whenever the listener thread crashed with exception.
        /// </summary>
        public event EventHandler<ListeningThreadFailedEventArgs> ListeningThreadFailed
		{
			add { _listener.ListeningThreadFailed += value; }
			remove { _listener.ListeningThreadFailed -= value; }
		}

        /// <summary>
        /// Occurs when [stop listening].
        /// </summary>
        public event EventHandler StopListening
		{
			add { _listener.StopListening += value; }
			remove { _listener.StopListening -= value; }
		}

        /// <summary>
        /// Return listeners`s thread which listener use for listening.
        /// </summary>
        public Thread ListeningThread
		{
			get
			{ 
				return _listener == null ? null : _listener.ListeningThread;
			}
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="UniversalListener{T}"/> class.
        /// </summary>
        /// <param name="communicationService">The communication service.</param>
        public UniversalListener(
			ICommunicationService communicationService)
		{
			_listener = communicationService.CreateRpcCommunicationListener(ListeningFunc);
		}

		private RpcCommunicationPacket ListeningFunc(string from, RpcCommunicationPacket packet)
		{
			var sendMethodContext = Serializer.Deserialize<MethodCallContext>(packet.Body);

			var instanceType = typeof(T);
			var callingMethod = instanceType.GetMethod(sendMethodContext.MethodName);

			if (callingMethod == null)
			{
				throw new InvalidOperationException("Calling unknown function.");
			}
			var parameters = sendMethodContext.Parameters;
			var result = callingMethod.Invoke(_instance, parameters);
			var replyMethodContext = new MethodCallContext
			{
				MethodName = sendMethodContext.MethodName
			};

			if (callingMethod.ReturnType != typeof(void))
			{
				replyMethodContext.Result = result;
			}

			var replyPacket = new RpcCommunicationPacket
			{
				Body = Serializer.Serialize(replyMethodContext)
			};
			return replyPacket; 
		}

		public void StartListen(T instance)
		{
			_instance = instance;
			_listener.StartListen();
		}

        /// <summary>
        /// Stop listen and dispose the resources.
        /// </summary>
        public void StopListen()
		{
			Dispose();
		}

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
		{
			_listener.Dispose();
		}
	}
}
