using System;
using System.Collections.Generic;

namespace ForeCastle.Communication.Consumers
{
	/// <summary>
	/// Consume packets of TPacket on the given queue. 
	/// For more details <see href="https://sites.google.com/site/logioforecastle/navody/communication-project/publish-consume"/>
	/// </summary>
	/// <typeparam name="TPacket">Type of consuming packet</typeparam>
	public interface IConsumer<TPacket> : IDisposable
		where TPacket : class
	{
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
		/// Start consume.
		/// </summary>
		void StartConsume();

		/// <summary>
		/// Stop consume and dispose the resources.
		/// </summary>
		void StopConsume();

		/// <summary>
		/// Dequeues packet from queue. Blocks for time in timeout parameter.
		/// </summary>
		/// <param name="timeout">Operation timeout in millisecond.</param>
		/// <param name="packet">Dequeued packet</param>
		/// <returns>Returns true if the packet was dequeued otherwise the operation timeouts.</returns>
		bool Dequeue(int timeout, out TPacket packet);

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
