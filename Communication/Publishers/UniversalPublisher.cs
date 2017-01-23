using System;
using System.Collections.Generic;
using System.Linq;
using ForeCastle.Communication.CommunicationService;
using ForeCastle.Library;

namespace ForeCastle.Communication.Publishers
{
	/// <summary>
	/// Publish packets of TPacket on the given queue. 
	/// For more details <see href="https://sites.google.com/site/logioforecastle/navody/communication-project/publish-consume"/>
	/// </summary>
	/// <typeparam name="TPacket">Type of publishing packet</typeparam>
	public class UniversalPublisher<TPacket> : IPublisher<TPacket>
		where TPacket : class
	{
		private readonly ICommunicationService _communicationService;
		private readonly string _targetQueue;
		private readonly bool _hasDeadLetterExchange;

		public UniversalPublisher(ICommunicationService communicationService, string targetQueue)
		{
			_communicationService = communicationService;
			_targetQueue = targetQueue;
		}

		public UniversalPublisher(ICommunicationService communicationService, string targetQueue, bool hasDlx) : this(communicationService, targetQueue)
		{
			_hasDeadLetterExchange = hasDlx;
		}

		public void Publish(IEnumerable<TPacket> packets)
		{
			var communicationPackets = packets.Select(job => new WorkCommunicationPacket
			{
				Body = Serializer.Serialize(job), Priority = 0
			}).ToList();

			_communicationService.Publish(_targetQueue, communicationPackets, _hasDeadLetterExchange);
		}

		public void Publish(IEnumerable<TPacket> packets, Func<TPacket, byte> prioritySelector)
		{
			var communicationPackets = packets.Select(job => new WorkCommunicationPacket
			{
				Body = Serializer.Serialize(job), Priority = prioritySelector(job)
			}).ToList();

			_communicationService.Publish(_targetQueue, communicationPackets, _hasDeadLetterExchange);
		}

		public void Publish(IEnumerable<TPacket> packets, IDictionary<string, object> headers)
		{
			var communicationPackets = packets.Select(job => new WorkCommunicationPacket
			{
				Body = Serializer.Serialize(job),
				Priority = 0,
				Headers = headers
			}).ToList();

			_communicationService.Publish(_targetQueue, communicationPackets, _hasDeadLetterExchange);
		}
	}
}
