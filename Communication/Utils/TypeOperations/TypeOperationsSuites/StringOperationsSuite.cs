using System;

namespace ForeCastle.Communication.Utils.TypeOperations.TypeOperationsSuites
{
	/// <summary>
	/// Operation suite for string.
	/// </summary>
	public class StringOperationsSuite : BaseTypeOperationsSuite<string>
	{
		public override string GenerateTyped()
		{
			return Guid.NewGuid().ToString();
		}

		public override bool AreEqual(string first, string second)
		{
			return first == second;
		}
	}
}