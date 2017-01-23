using System;

namespace ForeCastle.Communication.Utils.TypeOperations.TypeOperationsSuites
{
	/// <summary>
	/// Contains non-generic variation of function from ITypeOperationsSuite\T\.
	/// </summary>
	public interface ITypeOperationsSuite
	{
		/// <summary>
		/// Returns working type of operations of this suit. 
		/// </summary>
		/// <returns>Working type of operations of this suit. </returns>
		Type GetSuiteType();
			
		/// <summary>
		/// Generate a random instance of a given type.
		/// </summary>
		/// <returns>Generated random instance of a given type.</returns>
		object Generate();

		/// <summary>
		/// Return if the given parameters are equals.
		/// </summary>
		/// <returns><c>true</c>, if parameters are equal, <c>false</c> otherwise.</returns>
		/// <param name="first">First compared object.</param>
		/// <param name="second">Second compared object.</param>
		bool AreEqual(object first, object second);
	}

	/// <summary>
	/// Contains operations for a given type. This is useful for work with unknown types at compile time.    
	/// </summary>
	/// <typeparam name="T">Operation works with this type</typeparam> 
	public interface ITypeOperationsSuite<T> : ITypeOperationsSuite
	{
		/// <summary>
		/// Generate a random instance of a given type.
		/// </summary>
		/// <returns>Generated random instance of a given type.</returns>
		T GenerateTyped();

		/// <summary>
		/// Return if the given parameters are equals.
		/// </summary>
		/// <returns><c>true</c>, if parameters are equal, <c>false</c> otherwise.</returns>
		/// <param name="first">First compared object.</param>
		/// <param name="second">Second compared object.</param>
		bool AreEqual(T first, T second);
		
		void RegisterResolver(TypeOperationsService typeOperationsService);
	}
}
