﻿using System;
using System.Reflection;
using System.Threading;
using RabbitMQDemo.Communication.CommunicationService;
using RabbitMQDemo.Library;

namespace RabbitMQDemo.Communication.Listeners
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
        /// ListeningStarted is raised whenever the listener starts listen.
        /// </summary>
        public event EventHandler ListeningStarted
		{
			add { _listener.ListeningStarted += value; }
			remove { _listener.ListeningStarted -= value; }
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
        public event EventHandler ListeningStopped
		{
			add { _listener.ListeningStopped += value; }
			remove { _listener.ListeningStopped -= value; }
		}

        /// <summary>
        /// Return listeners`s thread which listener use for listening.
        /// </summary>
        public Thread ListeningThread => _listener?.ListeningThread;

		/// <summary>
        /// Initializes a new instance of the <see cref="UniversalListener{T}"/> class.
        /// </summary>
        /// <param name="communicationService">The communication service.</param>
        public UniversalListener(
			ICommunicationService communicationService)
		{
			_listener = communicationService.CreateRpcCommunicationListener(ListeningFunc);
		}

		private RpcPacket ListeningFunc(string from, RpcPacket packet)
		{
			var sendMethodContext = Serializer.Deserialize<MethodCallContext>(packet.Body);

			Type instanceType = typeof(T);
			MethodInfo callingMethod = instanceType.GetMethod(sendMethodContext.MethodName);

			if (callingMethod == null)
			{
				throw new InvalidOperationException("Calling unknown function.");
			}

			var parameters = sendMethodContext.Parameters;
			object result = callingMethod.Invoke(_instance, parameters);
			var replyMethodContext = new MethodCallContext
			{
				MethodName = sendMethodContext.MethodName
			};

			if (callingMethod.ReturnType != typeof(void))
			{
				replyMethodContext.Result = result;
			}

			var replyPacket = new RpcPacket
			{
				Body = Serializer.Serialize(replyMethodContext)
			};

			return replyPacket; 
		}

		public void StartListening(T instance)
		{
			_instance = instance;
			_listener.StartListening();
		}

        /// <summary>
        /// Stop listen and dispose the resources.
        /// </summary>
        public void StopListening()
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
