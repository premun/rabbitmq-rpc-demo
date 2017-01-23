using System.Linq;

namespace ForeCastle.Communication.Utils.TypeOperations.TypeOperationsSuites
{
	/// <summary>
	/// Operation suite for array of bytes.
	/// </summary>
	public class ByteArrayOperationSuite : BaseTypeOperationsSuite<byte[]>
	{
		public override byte[] GenerateTyped()
		{
			return new byte[0];
		}

		public override bool AreEqual(byte[] first, byte[] second)
		{
			return first.SequenceEqual(second);
		}
	}
}
