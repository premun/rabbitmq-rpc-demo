using System;

namespace RabbitMQDemo.CommunicationInterface
{
	[Serializable]
	public class ExampleMessage
	{
		public int Number { get; }

		public int BatchSize { get; }

		public Guid MessageGuid { get; }

		public ExampleMessage(int number, int batchSize)
		{
			Number = number;
			BatchSize = batchSize;
			MessageGuid = Guid.NewGuid();
		}

		public override string ToString()
		{
			return $"{MessageGuid}" + $"[{Number}/{BatchSize}]".PadLeft(10);
        }
	}
}
