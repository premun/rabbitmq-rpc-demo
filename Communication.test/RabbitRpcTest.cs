using System;
using System.Threading;
using NUnit.Framework;
using RabbitMQDemo.Communication.CommunicationService;
using RabbitMQDemo.Communication.CommunicationService.Exceptions;
using RabbitMQDemo.Communication.CommunicationService.Rabbit;

namespace RabbitMQDemo.Communication.test
{
	/// <summary>
	/// Tests Rabbit RPC.
	/// </summary>
	[TestFixture]
	public class RabbitRpcTest : BaseCommunicationTest
	{
		private readonly string _testQueueName = "RabbitRpcTest";

		private void ListeningThreadFailed(object sender, ListeningThreadFailedEventArgs e)
		{
			LogException(e.ListenException);
		}

		private RpcPacket SimpleListeningFunction(string from, RpcPacket paket)
		{
			var replyPacket = new RpcPacket
			{
				Body = paket.Body
			};
			return replyPacket;
		}

		private static RpcPacket CrashListeningFunction(string from, RpcPacket paket)
		{
			throw new Exception();
		}

		private IRpcCommunicationListener StartSimpleListener(ICommunicationService communicationService)
		{
			IRpcCommunicationListener listener = communicationService.CreateRpcCommunicationListener(SimpleListeningFunction);
			listener.ListeningThreadFailed += ListeningThreadFailed;
			listener.StartListen();

			return listener;
		}

		/// <summary>
		/// Tests one listeners on one queue.
		/// </summary>
		[Test, Timeout(5000)]
		public void TwoListenersOnOneQueueTest()
		{
			using (ICommunicationService communicationService1 = new RabbitCommunicationService(Logger, _testQueueName))
			using (ICommunicationService communicationService2 = new RabbitCommunicationService(Logger, _testQueueName))
			{
				// Initializes first listener
				using (IRpcCommunicationListener listener1 = StartSimpleListener(communicationService1))
				{
					Thread.Sleep(500);
					// Initializes second listener should failed
					using (IRpcCommunicationListener listener2 = StartSimpleListener(communicationService2))
					{
						listener2.ListeningThread.Join();
						Assert.Throws<CommunicationException>(RaiseExceptionFromOtherThreads);
					}
				}
			}
		}

		/// <summary>
		/// Tests simple RPC call with reply.
		/// </summary>
		[Test]
		public void SimpleRpcCallExpectReplyTest()
		{
			RpcPacket[] packetsToSend =
			{
				new RpcPacket
				{
					Body = GenerateRandomByteArray()
				},
				new RpcPacket
				{
					Body = null
				},
				new RpcPacket
				{
					Body = new byte[0]
				},
				new RpcPacket
				{
					Body = new byte[1000000]
				}
			};

			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, _testQueueName))
			{
				using (IRpcCommunicationListener listener = StartSimpleListener(communicationService))
				{
					Thread.Sleep(100);

					using (ICommunicationService communicationService2 = new RabbitCommunicationService(Logger, _testQueueName))
					{
						foreach (RpcPacket sendPacket in packetsToSend)
						{
							RpcPacket resultPacket = communicationService2.CallRpc(_testQueueName, sendPacket);
							Assert.AreEqual(sendPacket.Body, resultPacket.Body);
						}
					}
				}
			}
		}

		/// <summary>
		/// Tests simple RPC call without reply.
		/// </summary>
		[Test]
		public void SimpleRpcCallDoNotExpectReplyTest()
		{
			RpcPacket[] packetsToSend =
			{
				new RpcPacket
				{
					Body = GenerateRandomByteArray()
				},
				new RpcPacket
				{
					Body = null
				},
				new RpcPacket
				{
					Body = new byte[0]
				},
				new RpcPacket
				{
					Body = new byte[1000000]
				}
			};

			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, _testQueueName))
			{
				using (var listener = StartSimpleListener(communicationService))
				{
					Thread.Sleep(100);

					using (ICommunicationService communicationService2 = new RabbitCommunicationService(Logger, _testQueueName))
					{
						foreach (var sendPacket in packetsToSend)
						{
							var resultPacket = communicationService2.CallRpc(_testQueueName, sendPacket, RpcCallType.DoNotExpectReply);
							Assert.Null(resultPacket);
						}
					}
				}
			}

			RaiseExceptionFromOtherThreads();
		}

		/// <summary>
		/// Tests RPC into not exists queue.
		/// </summary>
		[Test]
		public void RpcCallBadTargetQueueTest()
		{
			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, _testQueueName))
			{
				var sendPacket = new RpcPacket()
				{
					Body = GenerateRandomByteArray()
				};

				Assert.Throws<CommunicationException>(() => communicationService.CallRpc("queueThatShouldNotExists", sendPacket));
			}
		}

		/// <summary>
		/// Tests crash of listener.
		/// </summary>
		[Test]
		public void CrashedListenerRpcCallTest()
		{
			// Initializes listener
			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, _testQueueName))
			{
				using (
					IRpcCommunicationListener listener = communicationService.CreateRpcCommunicationListener(CrashListeningFunction))
				{
					listener.ListeningThreadFailed += ListeningThreadFailed;
					listener.StartListen();

					Thread.Sleep(100);

					var sendPacket = new RpcPacket
					{
						Body = GenerateRandomByteArray()
					};

					Assert.Throws<CommunicationListenerCrashedException>(() => communicationService.CallRpc(_testQueueName, sendPacket));
				}
			}
		}

		/// <summary>
		/// Tests listener dispose method.
		/// </summary>
		[Test]
		public void RpcCommunicationListenerDisposeTest()
		{
			bool startListeningCalled = false;
			bool stopListeningCalled = false;
			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, _testQueueName))
			{
				using (var listener = communicationService.CreateRpcCommunicationListener(SimpleListeningFunction))
				{
					listener.ListeningThreadFailed += ListeningThreadFailed;
					listener.StartListening += (sender, e) =>
					{
						startListeningCalled = true;
					};

					listener.StopListening += (sender, e) =>
					{
						stopListeningCalled = true;
					};

					listener.StartListen();
					Thread.Sleep(500);
				}
			}
			Thread.Sleep(100);
			Assert.AreEqual(true, startListeningCalled);
			Assert.AreEqual(true, stopListeningCalled);
			RaiseExceptionFromOtherThreads();
		}
	}
}