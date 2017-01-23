using System;

namespace ForeCastle.Communication.Utils.TypeOperations.TypeOperationsSuites
{
	/// <summary>
	/// Operation suite for boolean.
	/// </summary>
	public class BoolOperationsSuite : BaseTypeOperationsSuite<bool>
	{
		private readonly Random _rnd = new Random();

		public override bool GenerateTyped()
		{
			return _rnd.Next(1) == 0;
		}

		public override bool AreEqual(bool first, bool second)
		{
			return first == second;
		}
	}
}
