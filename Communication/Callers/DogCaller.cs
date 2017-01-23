using System;
using ForeCastle.Communication.CommunicationService;
using ForeCastle.Communication.Definitions;

namespace ForeCastle.Communication.Callers
{
	public class DogCaller : IDog
	{
		private ICommunicationService _communicationService;
		private string _queueName;

		public DogCaller(ICommunicationService communicationService, string queueName)
		{
			_communicationService = communicationService;
			_queueName = queueName;
		}
			
		public string DoHaf(int hafCount)
		{
			var sendPacket = new RpcCommunicationPacket
			{
					RpcFunction = "DoHaf"
			};
			sendPacket.Parameters.Add("hafCount", hafCount.ToString()); 

			var replyPacket = _communicationService.CallRpc(_queueName, sendPacket);

			if (!replyPacket.Parameters.ContainsKey("result"))
			{
				// TODO: rewrite
				throw new CommunicationException("Inform about problems");
			}

			return replyPacket.Parameters["result"];
		}
	}
}
