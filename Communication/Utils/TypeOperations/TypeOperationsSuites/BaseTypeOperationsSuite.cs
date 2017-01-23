using System;

namespace ForeCastle.Communication.Utils.TypeOperations.TypeOperationsSuites
{
	/// <summary>
	/// Base generic implementation of ITypeOperationsSuite. Simplify the writing implementation of ITypeOperationsSuite.
	/// </summary>
	/// <typeparam name="T">Operation works with this type</typeparam>
	public abstract class BaseTypeOperationsSuite<T> : ITypeOperationsSuite<T>
	{
		protected TypeOperationsService TypeOperationsService { get; set; }

		public Type GetSuiteType()
		{
			return typeof(T);
		}

		public object Generate()
		{
			return GenerateTyped();
		}

		public bool AreEqual(object first, object second)
		{
			try
			{
				T firstTyped = (T)first;
				T secondTyped = (T)second;
				return AreEqual(firstTyped, secondTyped);
			}
			catch (InvalidCastException)
			{
				return false;
			}
		}

		public abstract bool AreEqual(T first, T second);

		public abstract T GenerateTyped();

		public void RegisterResolver(TypeOperationsService typeOperationsService)
		{
			TypeOperationsService = typeOperationsService;
		}
	}
}
