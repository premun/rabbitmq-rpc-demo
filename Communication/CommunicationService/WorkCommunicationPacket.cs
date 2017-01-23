﻿using System;
using System.Collections.Generic;

namespace ForeCastle.Communication.CommunicationService
{
	/// <summary>
	/// Packet for publish/consume communication.
	/// </summary>
	[Serializable]
	public class WorkCommunicationPacket
	{
		public byte[] Body { get; set; }

		public byte Priority { get; set; }

		public ulong Tag { get; set; }

		public IDictionary<string, object> Headers { get; set; }
	}
}
