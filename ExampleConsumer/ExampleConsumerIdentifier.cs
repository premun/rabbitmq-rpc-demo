using System;
using RabbitMQDemo.Library;

namespace RabbitMQDemo.ExampleConsumer
{
	[Serializable]
	public class ExampleConsumerIdentifier : Identifier
	{
		public ExampleConsumerIdentifier()
			: base(null, "ExampleConsumer")
		{
		}
	}
}
