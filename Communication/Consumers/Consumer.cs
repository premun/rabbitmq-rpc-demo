using System.Collections.Generic;
using RabbitMQDemo.Communication.CommunicationService;
using RabbitMQDemo.Communication.CommunicationService.Exceptions;
using RabbitMQDemo.Library;

namespace RabbitMQDemo.Communication.Consumers
{
	/// <summary>
	/// Consume packets of TPacket on the given queue.
	/// </summary>
	/// <typeparam name="TPacket">Type of consuming packet</typeparam>
	public class Consumer<TPacket> : IConsumer<TPacket> where TPacket : class
	{
		private ICommunicationConsumer _communicationConsumer;
		private readonly ICommunicationService _communicationService;
		private readonly Dictionary<TPacket, PublishConsumePacket> _unAckJobs = new Dictionary<TPacket, PublishConsumePacket>();

		public Consumer(ICommunicationService communicationService, string consumeQueueName)
		{
			_communicationService = communicationService;
			ConsumeQueueName = consumeQueueName;
		}

		public string ConsumeQueueName { get; }

		public ushort Prefetch { get; set; } = 1;

		public void StartConsume()
		{
			_communicationConsumer = _communicationService.CreateConsumer(ConsumeQueueName);
			_communicationConsumer.Prefetch = Prefetch;
			_communicationConsumer.StartConsume();
		}

		public bool Dequeue(out TPacket packet, int timeout = -1)
		{
			if (_communicationConsumer == null)
			{
				throw new CommunicationException("Could not process function because the consumer was not initialized or disposed.");
			}

			PublishConsumePacket communicationPacket;
			bool result = _communicationConsumer.Dequeue(timeout, out communicationPacket);
			if (result)
			{
				packet = Serializer.Deserialize<TPacket>(communicationPacket.Body);
				_unAckJobs.Add(packet, communicationPacket);
				return true;
			}

			packet = null;
			return false;
		}

		public IDictionary<string, object> GetPacketHeaders(TPacket packet)
		{
			if (CheckPacket(packet))
			{
				return _unAckJobs[packet].Headers;
			}

			return null;
		}

		public void Ack(TPacket packet)
		{
			if (CheckPacket(packet))
			{
				_communicationConsumer.Ack(_unAckJobs[packet]);
				_unAckJobs.Remove(packet);
			}
		}

		public void Nack(TPacket packet)
		{
			if (CheckPacket(packet))
			{
				_communicationConsumer.Nack(_unAckJobs[packet]);
				_unAckJobs.Remove(packet);
			}
		}

		public void StopConsume()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_communicationConsumer != null)
			{
				_communicationConsumer.Dispose();
				_communicationConsumer = null;
			}
		}

		private bool CheckPacket(TPacket packet)
		{
			if (_communicationConsumer == null)
			{
				throw new CommunicationException("Could not process function because the consumer was not initialized or disposed.");
			}

			if (!_unAckJobs.ContainsKey(packet))
			{
				throw new CommunicationException("This packet does not come from this consumer or was already (n)acked.");
			}

			return true;
		}
	}
}
