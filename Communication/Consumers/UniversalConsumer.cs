using System.Collections.Generic;
using ForeCastle.Library;
using RabbitMQDemo.Communication.CommunicationService;

namespace RabbitMQDemo.Communication.Consumers
{
	/// <summary>
	/// Consume packets of TPacket on the given queue. 
	/// For more details <see href="https://sites.google.com/site/logioforecastle/navody/communication-project/publish-consume"/>
	/// </summary>
	/// <typeparam name="TPacket">Type of consuming packet</typeparam>
	public class UniversalConsumer<TPacket> : IConsumer<TPacket>, IDeadLetterConsumer<TPacket> where TPacket : class
	{
		private ICommunicationConsumer _communicationConsumer;
		private ushort _prefetch = 1;
		private readonly string _consumeQueueName;
		private readonly ICommunicationService _communicationService;
		private readonly Dictionary<TPacket, WorkCommunicationPacket> _unAckJobs = new Dictionary<TPacket, WorkCommunicationPacket>();
		private readonly bool _hasDeadLetterExchange;

		public UniversalConsumer(ICommunicationService communicationService, string consumeQueueName)
		{
			_communicationService = communicationService;
			_consumeQueueName = consumeQueueName;
		}

		public UniversalConsumer(ICommunicationService communicationService, string consumeQueueName, bool hasDlx)
			: this(communicationService, consumeQueueName)
		{
			_hasDeadLetterExchange = hasDlx;
		}

		public string ConsumeQueueName
		{
			get { return _consumeQueueName; }
		}

		public ushort Prefetch
		{
			get { return _prefetch; }

			set { _prefetch = value; }
		}

		public void StartConsume()
		{
			_communicationConsumer = _communicationService.CreateConsumer(_consumeQueueName);
			_communicationConsumer.Prefetch = _prefetch;
			_communicationConsumer.StartConsume(_hasDeadLetterExchange);
		}

		public bool Dequeue(int timeout, out TPacket packet)
		{
			if (_communicationConsumer == null)
			{
				throw new CommunicationException("Could not process function because the consumer was not initialized or disposed.");
			}

			WorkCommunicationPacket communicationPacket;
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
