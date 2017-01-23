using RabbitMQDemo.Library;

namespace RabbitMQDemo.ExampleProducer
{
	public class ExampleProducerIdentifier : Identifier
	{
		public ExampleProducerIdentifier()
			: base(null, "ExampleProducer")
		{
		}

		public override string FullName => "ExampleProducer";
	}
}
