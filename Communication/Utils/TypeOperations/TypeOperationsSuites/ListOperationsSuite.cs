using System.Collections.Generic;

namespace ForeCastle.Communication.Utils.TypeOperations.TypeOperationsSuites
{
	/// <summary>
	/// Operation suite for list with specified item type.
	/// </summary>
	/// <typeparam name="T">Type of array item.</typeparam> 
	public class ListOperationsSuite<T> : BaseTypeOperationsSuite<List<T>>
	{
		private readonly ITypeOperationsSuite<T> _innerTypeOperationsSuite;

		public ListOperationsSuite(ITypeOperationsSuite<T> innerTypeOperationsSuite)
		{
			_innerTypeOperationsSuite = innerTypeOperationsSuite;
		}

		private string GetCollectionIndexName(string indexName, int iterationIndex)
		{
			return string.Format("{0}_{1}", indexName, iterationIndex);
		}

		public override List<T> GenerateTyped()
		{
			var generatedCollection = new List<T>();

			for (int i = 0; i < 10; i++)
			{
				generatedCollection.Add(_innerTypeOperationsSuite.GenerateTyped());
			}
			return generatedCollection;
		}

		public override bool AreEqual(List<T> first, List<T> second)
		{
			if (first.Count != second.Count)
			{
				return false;
			}

			for (int i = 0; i < first.Count; i++)
			{
				if (!_innerTypeOperationsSuite.AreEqual(first[i], second[i]))
				{
					return false;
				}
			}

			return true;
		}
	}
}
