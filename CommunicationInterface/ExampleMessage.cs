using System;

namespace RabbitMQDemo.CommunicationInterface
{
	public class ExampleMessage
	{
		public Guid MessageGuid { get; }

		public ExampleMessage()
			: this(Guid.NewGuid())
		{
		}

		public ExampleMessage(Guid messageGuid)
		{
			MessageGuid = messageGuid;
		}

		public override string ToString()
		{
			return MessageGuid.ToString();
		}
	}
}
