using System;
using System.Collections.Generic;

namespace RabbitMQDemo.Communication.Consumers
{
	/// <summary>
	/// Consume packets of TPacket on the given queue.
	/// </summary>
	/// <typeparam name="TPacket">Type of consuming packet</typeparam>
	public interface IConsumer<TPacket> : IDisposable where TPacket : class {
		/// <summary>
		/// Consume queue name.
		/// </summary>
		string ConsumeQueueName { get; }

		/// <summary>
		/// Determines how many unack packets can be dequeue from consumer. 
		/// Default value is 1
		/// </summary>
		ushort Prefetch { get; set; }

		/// <summary>
		/// Starts consuming messages.
		/// </summary>
		void StartConsume();

		/// <summary>
		/// Stops consuming and disposes the resources.
		/// </summary>
		void StopConsume();

		/// <summary>
		/// Dequeues packet from queue. Blocks for time in timeout parameter.
		/// </summary>
		/// <param name="packet">Dequeued packet</param>
		/// <param name="timeout">Operation timeout in millisecond.</param>
		/// <returns>Returns true if the packet was dequeued otherwise the operation timeouts.</returns>
		bool Dequeue(out TPacket packet, int timeout = -1);

		/// <summary>
		/// Ack the given packet.
		/// </summary>
		/// <param name="packet">Packet to ack.</param>
		void Ack(TPacket packet);

		/// <summary>
		/// Nack the given packet.
		/// </summary>
		/// <param name="packet">Packet to nack.</param>
		void Nack(TPacket packet);

		/// <summary>
		/// Returns headers of the given packet
		/// </summary>
		/// <param name="packet">Packet to found headers</param>
		/// <returns>Founded headers</returns>
		IDictionary<string, object> GetPacketHeaders(TPacket packet);
	}
}
