using System;

namespace ForeCastle.Communication.Utils.TypeOperations.TypeOperationsSuites
{
	/// <summary>
	/// Operation suite for byte.
	/// </summary>
	public class ByteOperationsSuite : BaseTypeOperationsSuite<byte>
	{
		private readonly Random _rnd = new Random();

		public override byte GenerateTyped()
		{
			return (byte) _rnd.Next();
		}

		public override bool AreEqual(byte first, byte second)
		{
			return first == second;
		}
	}
}
