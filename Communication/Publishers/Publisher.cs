using System.Collections.Generic;
using System.Linq;
using RabbitMQDemo.Communication.CommunicationService;
using RabbitMQDemo.Library;

namespace RabbitMQDemo.Communication.Publishers
{
	/// <summary>
	/// Publish messages of TPacket on the given queue.
	/// </summary>
	/// <typeparam name="TPacket">Type of publishing packet</typeparam>
	public class Publisher<TPacket> : IPublisher<TPacket> where TPacket : class
	{
		private readonly ICommunicationService _communicationService;
		private readonly string _targetQueue;

		public Publisher(ICommunicationService communicationService, string targetQueue)
		{
			_communicationService = communicationService;
			_targetQueue = targetQueue;
		}

		public void Publish(TPacket message)
		{
			Publish(new[] { message });
		}

		public void Publish(IEnumerable<TPacket> messages)
		{
			var communicationPackets = messages
				.Select(message => new PublishConsumePacket
				{
					Body = Serializer.Serialize(message)
				})
				.ToList();

			_communicationService.Publish(_targetQueue, communicationPackets);
		}
	}
}
