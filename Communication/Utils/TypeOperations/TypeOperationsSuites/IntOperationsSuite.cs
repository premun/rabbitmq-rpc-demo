using System;

namespace ForeCastle.Communication.Utils.TypeOperations.TypeOperationsSuites
{
	/// <summary>
	/// Operation suite for int.
	/// </summary>
	public class IntOperationsSuite : BaseTypeOperationsSuite<int>
	{
		private readonly Random _rnd = new Random();

		public override int GenerateTyped()
		{
			return _rnd.Next();
		}

		public override bool AreEqual(int first, int second)
		{
			return first == second;
		}
	}
}
