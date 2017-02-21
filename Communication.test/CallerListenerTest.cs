using System;
using System.Threading;
using NUnit.Framework;
using RabbitMQDemo.Communication.Callers;
using RabbitMQDemo.Communication.CommunicationService;
using RabbitMQDemo.Communication.CommunicationService.Exceptions;
using RabbitMQDemo.Communication.CommunicationService.Rabbit;
using RabbitMQDemo.Communication.Listeners;

namespace RabbitMQDemo.Communication.test
{
	/// <summary>
	/// Class tested universal communications.
	/// </summary>
	[TestFixture]
	public class UniversalCallListenTest : BaseCommunicationTest
	{
		private const int Argument1Value = 100;
		private const string Argument2Value = "Test";
		private const string TestQueueName = "UniversalCallerListenerTest";

		private interface ITestCommunicationObject
		{
			string TestMethod(int argument1, string argument2);

			void TestVoidMethod(int argument1, string argument2);
		}

		private class TestCommunicationObject : ITestCommunicationObject
		{
			private readonly int _argument1Value;
			private readonly string _argument2Value;

			public TestCommunicationObject(int argument1Value, string argument2Value)
			{
				_argument1Value = argument1Value;
				_argument2Value = argument2Value;
			}

			public string TestMethod(int argument1, string argument2)
			{
				if (_argument1Value != argument1 || _argument2Value != argument2)
				{
					throw new ArgumentException();
				}
				
				return argument1 + argument2;
			}

			public void TestVoidMethod(int argument1, string argument2)
			{
				Assert.AreEqual(_argument1Value, argument1);
				Assert.AreEqual(_argument2Value, argument2);
			}
		}

		private interface ITestKillObject
		{
			void Kill();
		}

		private class TestKillObject : ITestKillObject
		{
			private readonly IListener<ITestKillObject> _listener;

			public TestKillObject(IListener<ITestKillObject> listener)
			{
				_listener = listener;
			}

			public void StartListen()
			{
				_listener.StartListening(this);
			}

			public void Kill()
			{
				_listener.Dispose();
			}
		}

		private void ListeningThreadFailed(object sender, ListeningThreadFailedEventArgs e)
		{
			LogException(e.ListenException);
		}

		/// <summary>
		/// Tests simple communications.
		/// </summary>
		[Test]
		public void SimpleCallsTest()
		{
			ITestCommunicationObject implementation = new TestCommunicationObject(Argument1Value, Argument2Value);
			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, TestQueueName))
			{
				using (var listener = new UniversalListener<ITestCommunicationObject>(communicationService))
				{
					listener.ListeningThreadFailed += ListeningThreadFailed;
					listener.StartListening(implementation);
					// Give time to start listener
					Thread.Sleep(100);

					ITestCommunicationObject caller = CallerProxy<ITestCommunicationObject>.Create(communicationService, TestQueueName);

					// Test method which returns value
					string result = caller.TestMethod(Argument1Value, Argument2Value);
					Assert.AreEqual(Argument1Value + Argument2Value, result);

					// Test void method
					caller.TestMethod(Argument1Value, Argument2Value);
				}
			}

			RaiseExceptionFromOtherThreads();
		}

		/// <summary>
		/// Tests reaction if listener in communication is crashed.
		/// </summary>
		[Test]
		public void CrashedListenerTest()
		{
			var implementation = new TestCommunicationObject(Argument1Value, Argument2Value);
			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, TestQueueName))
			{
				using (var listener = new UniversalListener<ITestCommunicationObject>(communicationService))
				{
					listener.ListeningThreadFailed += ListeningThreadFailed;
					listener.StartListening(implementation);

					// Give time to start listener
					Thread.Sleep(100);

					ITestCommunicationObject caller = CallerProxy<ITestCommunicationObject>.Create(communicationService, TestQueueName);

					// This raises an exception on the listener side
					Assert.Throws<CommunicationListenerCrashedException>(() => caller.TestMethod(Argument1Value + 1, Argument2Value));
				}
			}
		}

		/// <summary>
		/// Tests connection into not exists queue.
		/// </summary>
		[Test]
		public void CallBadTargetQueueTest()
		{
			const string targetQueueName = "nonExistentQueueName";
			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, TestQueueName))
			{
				ITestCommunicationObject caller = CallerProxy<ITestCommunicationObject>.Create(communicationService, targetQueueName);
				Assert.Throws<CommunicationException>(() => caller.TestMethod(Argument1Value, Argument2Value));
			}
		}

		/// <summary>
		/// Tests success of dispose methods.
		/// </summary>
		[Test]
		public void ListenerDisposeTest()
		{
			bool startListeningCalled = false;
			bool stopListeningCalled = false;

			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, TestQueueName))
			{
				using (var listener = new UniversalListener<ITestCommunicationObject>(communicationService))
				{
					var implementation = new TestCommunicationObject(Argument1Value, Argument2Value);
					listener.StartListening(implementation);
					listener.ListeningThreadFailed += ListeningThreadFailed;

					listener.ListeningStarted += (sender, e) =>
					{
						startListeningCalled = true;
					};

					listener.ListeningStopped += (sender, e) =>
					{
						stopListeningCalled = true;
					};

					Thread.Sleep(100);
				}
			}

			Assert.AreEqual(true, startListeningCalled);
			Assert.AreEqual(true, stopListeningCalled);
			RaiseExceptionFromOtherThreads();
		}

		/// <summary>
		/// Tests kill of communication.
		/// </summary>
		[Test]
		public void KillTest()
		{
			bool startListeningCalled = false;
			bool stopListeningCalled = false;

			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, TestQueueName))
			{
				ITestKillObject caller = CallerProxy<ITestKillObject>.Create(communicationService, TestQueueName);
				var listener = new UniversalListener<ITestKillObject>(communicationService);
				listener.ListeningThreadFailed += ListeningThreadFailed;
				listener.ListeningStarted += (sender, e) =>
				{
					startListeningCalled = true;
				};

				listener.ListeningStopped += (sender, e) =>
				{
					stopListeningCalled = true;
				};

				var testKillobject = new TestKillObject(listener);
				testKillobject.StartListen();

				Thread.Sleep(100);
				caller.Kill();
			}

			Assert.AreEqual(true, startListeningCalled);
			Assert.AreEqual(true, stopListeningCalled);
		}

		/// <summary>
		/// Tests two listeners on one queue.
		/// </summary>
		[Test, Timeout(5000)]
		public void TwoListenersOnOneQueueTest()
		{
			using (ICommunicationService communicationService1 = new RabbitCommunicationService(Logger, TestQueueName))
			using (ICommunicationService communicationService2 = new RabbitCommunicationService(Logger, TestQueueName))
			{
				using (var listener1 = new UniversalListener<ITestCommunicationObject>(communicationService1))
				{
					var implementation = new TestCommunicationObject(Argument1Value, Argument2Value);
					listener1.StartListening(implementation);
					listener1.ListeningThreadFailed += ListeningThreadFailed;

					Thread.Sleep(500);
					using (var listener2 = new UniversalListener<ITestCommunicationObject>(communicationService2))
					{
						listener2.ListeningThreadFailed += ListeningThreadFailed;
						listener2.StartListening(implementation);

						listener2.ListeningThread.Join();
						Assert.Throws<CommunicationException>(RaiseExceptionFromOtherThreads);
					}
				}
			}
		}
	}
}
