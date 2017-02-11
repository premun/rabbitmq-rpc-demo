using System.Collections.Generic;

namespace RabbitMQDemo.Communication.Publishers
{
	/// <summary>
	/// Publish packets of TPacket on the given queue.
	/// </summary>
	/// <typeparam name="TPacket">Type of publishing packet</typeparam>
	public interface IPublisher<in TPacket> where TPacket : class
	{
		/// <summary>
		/// Publish message.
		/// </summary>
		/// <param name="message">Message to publish</param>
		void Publish(TPacket message);

		/// <summary>
		/// Publish messages.
		/// </summary>
		/// <param name="messages">Messages to publish</param>
		void Publish(IEnumerable<TPacket> messages);
	}
}
