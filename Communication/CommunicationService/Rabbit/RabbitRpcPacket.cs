﻿using System;
using RabbitMQDemo.Communication.CommunicationService.Exceptions;
using RabbitMQDemo.Library;

namespace RabbitMQDemo.Communication.CommunicationService.Rabbit
{
	/// <summary>
	/// RPC packet used for work with RabbitCommunicationService.
	/// </summary>
	[Serializable]
	public class RabbitRpcPacket
	{
		public static RabbitRpcPacket DeserializeRpcPacket(byte[] serializedPacket)
		{
			try
			{
				return Serializer.Deserialize<RabbitRpcPacket>(serializedPacket);
			}
			catch (Exception e)
			{
				throw new CommunicationException("Couldn't deserialize RPC result, see inner exception for details.", e);
			}
		}

		public string From { get; set; }

		public bool Error { get; set; }

		public string ExceptionMessage { get; set; } 

		public string ExceptionStackTrace { get; set; } 

		public RpcPacket Body { get; set; }
	}
}
