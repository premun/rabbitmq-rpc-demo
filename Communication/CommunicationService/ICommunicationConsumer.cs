using System;

namespace RabbitMQDemo.Communication.CommunicationService
{
	/// <summary>
	/// Consume publish/consume packets on the given queue. 
	/// </summary>
	public interface ICommunicationConsumer : IDisposable
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
		bool Dequeue(int timeout, out PublishConsumePacket packet);

		/// <summary>
		/// Accepts the given packet.
		/// </summary>
		/// <param name="packet">Packet to accepts.</param>
		void Ack(PublishConsumePacket packet);

		/// <summary>
		/// Non-accepts the given packet.
		/// </summary>
		/// <param name="packet">Packet to non-accepts.</param>
		void Nack(PublishConsumePacket packet);
	}
}
