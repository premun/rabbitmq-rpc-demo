using System;

namespace ForeCastle.Communication.Utils.TypeOperations.TypeOperationsSuites
{
	/// <summary>
	/// Operation suite for a specified enum. 
	/// </summary>
	/// <typeparam name="TEnum">Type of enum</typeparam>
	public class EnumOperationsSuite<TEnum> : BaseTypeOperationsSuite<TEnum> where TEnum : struct, IConvertible
	{
		public override bool AreEqual(TEnum first, TEnum second)
		{
			return Equals(first, second);
		}

		public override TEnum GenerateTyped()
		{
			TEnum e = default(TEnum);
			return e;
		}			
	}
}
