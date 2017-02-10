using RabbitMQDemo.Library;

namespace RabbitMQDemo.ExamplePublisher
{
	public class ExamplePublisherIdentifier : Identifier
	{
		public ExamplePublisherIdentifier()
			: base(null, "ExamplePublisher")
		{
		}

		public override string FullName => "ExamplePublisher";
	}
}
