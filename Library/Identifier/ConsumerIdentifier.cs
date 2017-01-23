using System;

namespace ForeCastle.Library.Identifier
{
	[Serializable]
	public class ConsumerIdentifier : Identifier
	{
		public ConsumerIdentifier(string id)
			: base(id, "Consumer")
		{
		}
	}
}
