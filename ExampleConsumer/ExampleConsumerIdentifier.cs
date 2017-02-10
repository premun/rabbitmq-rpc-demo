using System;
using RabbitMQDemo.Library;

namespace RabbitMQDemo.ExampleConsumer
{
	[Serializable]
	public class ExampleConsumerIdentifier : Identifier
	{
		public ExampleConsumerIdentifier(string id)
			: base(id, "ExampleConsumer")
		{
		}
	}
}
