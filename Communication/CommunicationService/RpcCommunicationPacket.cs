using System;

namespace ForeCastle.Communication.CommunicationService
{
	/// <summary>
	/// Packet for RPC communication. 
	/// </summary>
	[Serializable]
	public class RpcCommunicationPacket
	{
		public byte[] Body { get; set; }
	}
}
