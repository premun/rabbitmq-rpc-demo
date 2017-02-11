using System;
using System.Threading;
using NUnit.Framework;
using RabbitMQDemo.Communication.CommunicationService;
using RabbitMQDemo.Communication.CommunicationService.Exceptions;
using RabbitMQDemo.Communication.CommunicationService.Rabbit;
using RabbitMQDemo.Communication.Consumers;
using RabbitMQDemo.Communication.Publishers;

namespace RabbitMQDemo.Communication.test
{
	/// <summary>
	/// Tests universal rabbit communication.
	/// </summary>
	[TestFixture]
	public class PublisherConsumerTest : BaseCommunicationTest
	{
		private const string TestQueueName = "RabbitPublishConsumeTest";
		private readonly Random _rnd = new Random();

		private static T DequeuePacket<T>(IConsumer<T> consumer, int timeout = 1000) where T : class
		{
			T recievedPacket;
			bool recieved = consumer.Dequeue(out recievedPacket, timeout);
			if (!recieved)
			{
				Assert.Fail("Consumer dequeue function timeout.");
			}
			consumer.Ack(recievedPacket);
			return recievedPacket;
		}

		/// <summary>
		/// Tests delete queue.
		/// </summary>
		[OneTimeTearDown]
		public void TearDown()
		{
			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, TestQueueName))
			{
				communicationService.DeleteQueue(TestQueueName);
			}
		}

		/// <summary>
		/// Test work packet.
		/// </summary>
		[Serializable]
		public class PublishConsumeTestPacket
		{
			public int Id { get; set; }
		}

		/// <summary>
		/// Tests simple communication.
		/// </summary>
		[Test]
		public void SimplePublishTest()
		{
			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, TestQueueName))
			{
				using (var consumer = new Consumer<PublishConsumeTestPacket>(communicationService, TestQueueName))
				{
					consumer.StartConsume();
					Thread.Sleep(100);

					var sendPacket = new PublishConsumeTestPacket()
					{
						Id = _rnd.Next(),
					};

					var publisher = new Publisher<PublishConsumeTestPacket>(communicationService, TestQueueName);
					publisher.Publish(new[] {sendPacket});
					PublishConsumeTestPacket recievedPacket = DequeuePacket(consumer);

					Assert.AreEqual(sendPacket.Id, recievedPacket.Id);
				}
			}
		}

		private static bool ThrowActionCommunicationException(Action action)
		{
			try
			{
				action();
			}
			catch (CommunicationException)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Tests communication into bad queue.
		/// </summary>
		[Test]
		public void PublishBadTargetQueueTest()
		{
			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, TestQueueName))
			{
				var sendPacket = new PublishConsumeTestPacket()
				{
					Id = _rnd.Next(),
				};

				const string queueName = "queueThatShouldNotExists";
				var publisher = new Publisher<PublishConsumeTestPacket>(communicationService, queueName);
				publisher.Publish(new PublishConsumeTestPacket[] {sendPacket});
				communicationService.DeleteQueue(queueName);
			}
		}

		/// <summary>
		/// Test un ack communication.
		/// </summary>
		[Test]
		public void UnAckPublishTest()
		{
			var sendPacket = new PublishConsumeTestPacket()
			{
				Id = _rnd.Next()
			};

			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, TestQueueName))
			{
				using (var unAckConsumer = new Consumer<PublishConsumeTestPacket>(communicationService, TestQueueName))
				{
					unAckConsumer.StartConsume();
					var publisher = new Publisher<PublishConsumeTestPacket>(communicationService, TestQueueName);
					publisher.Publish(new PublishConsumeTestPacket[] {sendPacket});

					PublishConsumeTestPacket recievedPacket;
					bool recieved = unAckConsumer.Dequeue(out recievedPacket, 1000);
					if (!recieved)
					{
						Assert.Fail("Consumer dequeue function timeout.");
					}
				}

				using (var ackConsumer = new Consumer<PublishConsumeTestPacket>(communicationService, TestQueueName))
				{
					ackConsumer.StartConsume();

					var recievedPacket = DequeuePacket(ackConsumer);
					Assert.AreEqual(sendPacket.Id, recievedPacket.Id);
				}

				using (var emptyConsumer = new Consumer<PublishConsumeTestPacket>(communicationService, TestQueueName))
				{
					emptyConsumer.StartConsume();
					PublishConsumeTestPacket recievedPacket;
					bool recieved = emptyConsumer.Dequeue(out recievedPacket, 100);
					if (recieved)
					{
						Assert.Fail("There should not be any other packet in queue.");
					}
				}
			}
		}

		/// <summary>
		/// Tests multiple consume dispose.
		/// </summary>
		[Test]
		public void ConsumerDisposeTest()
		{
			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, TestQueueName))
			{
				for (int i = 0; i < 10; i++)
				{
					using (var consumer = new Consumer<PublishConsumeTestPacket>(communicationService, TestQueueName))
					{
						consumer.StartConsume();
					}
				}
			}
		}

		/// <summary>
		/// Tests communication without initiation or after dispose.
		/// </summary>
		[Test]
		public void ConsumerDisposedOrNotInitTest()
		{
			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, TestQueueName))
			{
				var consumer = new Consumer<PublishConsumeTestPacket>(communicationService, TestQueueName);

				PublishConsumeTestPacket recievedPacket = null;

				const string assertMessage = "The action should throw communication exception.";

				bool result = ThrowActionCommunicationException(() =>
				{
					DequeuePacket(consumer);
				});
				Assert.AreEqual(true, result, assertMessage);

				result = ThrowActionCommunicationException(() =>
				{
					consumer.Ack(recievedPacket);
				});
				Assert.AreEqual(true, result, assertMessage);

				consumer.StartConsume();
				consumer.Dispose();

				result = ThrowActionCommunicationException(() =>
				{
					DequeuePacket(consumer);
				});
				Assert.AreEqual(true, result, assertMessage);

				result = ThrowActionCommunicationException(() =>
				{
					consumer.Ack(recievedPacket);
				});
				Assert.AreEqual(true, result, assertMessage);
			}
		}

		/// <summary>
		/// Tests double message ack.
		/// </summary>
		[Test]
		public void DoubleAckTest()
		{
			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, TestQueueName))
			{
				for (int i = 0; i < 10; i++)
				{
					using (var consumer = new Consumer<PublishConsumeTestPacket>(communicationService, TestQueueName))
					{
						consumer.StartConsume();
						Thread.Sleep(100);

						var sendPacket = new PublishConsumeTestPacket
						{
							Id = _rnd.Next()
						};

						var publisher = new Publisher<PublishConsumeTestPacket>(communicationService, TestQueueName);
						publisher.Publish(new[] {sendPacket});

						PublishConsumeTestPacket recievedPacket;
						bool recieved = consumer.Dequeue(out recievedPacket, 1000);
						if (!recieved)
						{
							Assert.Fail("Consumer dequeue function timeout.");
						}

						consumer.Ack(recievedPacket);
						Assert.Throws<CommunicationException>(() => consumer.Ack(recievedPacket));
					}
				}
			}
		}
	}
}
