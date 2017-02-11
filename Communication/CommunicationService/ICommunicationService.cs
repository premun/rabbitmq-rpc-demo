using System;
using System.Collections.Generic;

namespace RabbitMQDemo.Communication.CommunicationService
{
	/// <summary>
	/// RPC call type in a way of an reply expectation. 
	/// </summary>
	public enum RpcCallType
	{
		ExpectReply,
		DoNotExpectReply
	}

	/// <summary>
	/// Provides low-level communication based on queues. The service is a abstraction of communication layer.
	/// </summary>
	public interface ICommunicationService : IDisposable
	{
		/// <summary>
		/// Publish packets to the target queue.
		/// </summary>
		/// <param name="targetQueue">Target queue</param>
		/// <param name="packets">Packets to send</param>
		void Publish(string targetQueue, IEnumerable<PublishConsumePacket> packets);

		/// <summary>
		/// Creates a consumer which will consume on the consumeQueue.
		/// </summary>
		/// <param name="consumeQueue">Consume queue</param>
		/// <returns>Consumer consuming on the given queue</returns>
		ICommunicationConsumer CreateConsumer(string consumeQueue);

		/// <summary>
		/// Deletes the given queue.
		/// </summary>
		/// <param name="queueName">Name of queue which will be deleted</param>
		void DeleteQueue(string queueName);

		/// <summary>
		/// Send the given RPC packet to the targetQueue. 
		/// </summary>
		/// <param name="targetQueue">Target queue</param>
		/// <param name="sendPacket">Packet to send</param>
		/// <param name="type">Type of RPC communication - expect or not reply</param>
		/// <returns>Received packet from listener in case of expecting result type. 
		/// If the reply is not expected then returns null.</returns>
		RpcPacket CallRpc(string targetQueue, RpcPacket sendPacket, RpcCallType type = RpcCallType.ExpectReply);


		/// <summary>
		/// Creates a RPC listener with the given listening function. Listener will listen on the communicationService target queue.
		/// </summary>
		/// <param name="listeningFunction">Function provides the response logic.
		///  Function has two arguments sender queue name and received packet and returns the reply packet.</param>
		/// <returns>The RPC listener</returns>
		IRpcCommunicationListener CreateRpcCommunicationListener(Func<string, RpcPacket, RpcPacket> listeningFunction);
	}
}
