using System;
using NUnit.Framework;

namespace RabbitMQDemo.Communication.test
{
	/// <summary>
	/// Class for multi threading tests.
	/// </summary>
	public abstract class MultiThreadingTest : BaseTest
	{
		private Exception _notMainThreadException;

		/// <summary>
		/// Initiation.
		/// </summary>
		[SetUp]
		public void Init()
		{
			_notMainThreadException = null;
		}

		/// <summary>
		/// Logs exception from no main threads.
		/// </summary>
		/// <param name="ex">Exception to log.</param>
		protected void LogException(Exception ex)
		{
			_notMainThreadException = ex;
		}

		/// <summary>
		/// If no main exception is logged, throw its.
		/// </summary>
		protected void RaiseExceptionFromOtherThreads()
		{
			if (_notMainThreadException != null)
			{
				throw _notMainThreadException;
			}
		}
	}
}
