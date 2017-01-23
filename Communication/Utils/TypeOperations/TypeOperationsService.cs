using System;
using System.Collections.Generic;
using ForeCastle.Communication.Utils.TypeOperations.TypeOperationsSuites;

namespace ForeCastle.Communication.Utils.TypeOperations
{
	/// <summary>
	/// Is raised  if is something in TypeOperationsService went wrong.
	/// </summary>
	public class TypeOperationsServiceException : Exception
	{
		public TypeOperationsServiceException(string message)
			: base(message)
		{
			/* empty */
		}
	}

	/// <summary>
	/// Contains function which can work with any registered type at runtime. 
	/// All functions can be used with unknown types at compile-time.
	/// </summary>
	public class TypeOperationsService
	{
		private readonly Dictionary<Type, ITypeOperationsSuite> _suites = new Dictionary<Type, ITypeOperationsSuite>();

		/// <summary>
		/// Register the specified operation suite for the type T.
		/// </summary>
		/// <param name="suite">Operation suite.</param>
		/// <typeparam name="T">Register type</typeparam>
		public void Register<T>(ITypeOperationsSuite<T> suite)
		{
			_suites.Add(typeof(T), suite);
			suite.RegisterResolver(this);
		}

		/// <summary>
		/// Register the specified operation suite.
		/// </summary>
		/// <param name="suite">Operation suite.</param>
		public void Register(ITypeOperationsSuite suite)
		{
			_suites.Add(suite.GetSuiteType(), suite);
		}

		private ITypeOperationsSuite GetSuite(Type type)
		{
			if (!_suites.ContainsKey(type))
			{
				string exceptionMessage = string.Format("Given type `{0}` has not registered operation suite.", type.Name);
				throw new TypeOperationsServiceException(exceptionMessage);
			}
			return _suites[type];
		}

		/// <summary>
		/// Generate a random instance of a given type.
		/// </summary>
		/// <param name="type">Type of generated instance</param>
		/// <returns>Generated random instance of a given type.</returns>
		public object Generate(Type type)
		{
			return GetSuite(type).Generate();
		}

		/// <summary>
		/// Return if the given parameters are equals.
		/// </summary>
		/// <returns><c>true</c>, if parameters are equal, <c>false</c> otherwise.</returns>
		/// <param name="first">First compared object.</param>
		/// <param name="second">Second compared object.</param>
		public bool AreEqual(object first, object second)
		{
			if (first == null || second == null)
			{
				return first == null && second == null;
			}
				
			var firstType = first.GetType();
			return GetSuite(firstType).AreEqual(first, second);
		}
	}
}
