using System;
using RabbitMQDemo.Library;

namespace RabbitMQDemo.ExampleCallee
{
	[Serializable]
	public class ExampleCalleeIdentifier : Identifier
	{
		public ExampleCalleeIdentifier(string id)
			: base(id, "ExampleCallee")
		{
		}
	}
}
