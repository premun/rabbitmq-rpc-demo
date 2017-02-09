using RabbitMQDemo.Library;

namespace RabbitMQDemo.ExampleCaller
{
	public class ExampleCallerIdentifier : Identifier
	{
		public ExampleCallerIdentifier()
			: base(null, "ExampleCaller")
		{
		}

		public override string FullName => "ExampleCaller";
	}
}
