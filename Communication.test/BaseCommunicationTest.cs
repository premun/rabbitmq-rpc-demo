using System;
using Autofac;
using NUnit.Framework;

namespace RabbitMQDemo.Communication.test
{
	/// <summary>
	/// Exception class for communication tests.
	/// </summary>
	public class CallMethodException : Exception
	{
		public CallMethodException(string methodName, Exception innerException)
			: base($"Method `{methodName}` failed with exception (see inner exception for details):\n{innerException.Message} ")
		{
			MethodName = methodName;
		}

		/// <summary>
		/// Source test method name.
		/// </summary>
		public string MethodName { get; }
	}

	/// <summary>
	/// Base class for communication tests.
	/// </summary>
	public abstract class BaseCommunicationTest : MultiThreadingTest
	{
		/// <summary>
		/// Logger.
		/// </summary>
		protected Library.ILogger Logger { get; set; }

		/// <summary>
		/// Initialization.
		/// </summary>
		[SetUp]
		public void Initialization()
		{
			Logger = AutofacContainer.Resolve<Library.ILogger>();
		}

		/// <summary>
		/// Generate random byte array.
		/// </summary>
		/// <returns>Generated array.</returns>
		public byte[] GenerateRandomByteArray()
		{
			var rnd = new Random();
			var array = new byte[2048];

			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (byte) rnd.Next();
			}

			return array;
		}
	}
}
