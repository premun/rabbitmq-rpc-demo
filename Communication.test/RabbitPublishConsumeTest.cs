using System;
using System.Threading;
using NUnit.Framework;
using RabbitMQDemo.Communication.CommunicationService;
using RabbitMQDemo.Communication.CommunicationService.Exceptions;
using RabbitMQDemo.Communication.CommunicationService.Rabbit;

namespace RabbitMQDemo.Communication.test
{
	/// <summary>
	/// Tests rabbit message communication.
	/// </summary>
	[TestFixture]
	public class RabbitPublishConsumeTest : BaseCommunicationTest
	{
		private const string TestQueueName = "RabbitPublishConsumeTest";

		/// <summary>
		/// Tests queue delete.
		/// </summary>
		[TestFixtureTearDown]
		public void TearDown()
		{
			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, TestQueueName))
			{
				communicationService.DeleteQueue(TestQueueName);
			}
		}

		private static PublishConsumePacket DequeuePacket(ICommunicationConsumer consumer)
		{
			PublishConsumePacket recievedPacket;
			bool recieved = consumer.Dequeue(out recievedPacket, 1000);
			if (!recieved)
			{
				Assert.Fail("Consumer dequeue function timeout.");
			}
			consumer.Ack(recievedPacket);
			return recievedPacket;
		}

		/// <summary>
		/// Tests simply send package.
		/// </summary>
		[Test]
		public void SimplePublishTest()
		{
			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, TestQueueName))
			{
				using (ICommunicationConsumer consumer = communicationService.CreateConsumer(TestQueueName))
				{
					consumer.StartConsume();
					Thread.Sleep(100);

					var sendPacket = new PublishConsumePacket
					{
						Body = GenerateRandomByteArray()
					};

					communicationService.Publish(TestQueueName, new[] {sendPacket});

					PublishConsumePacket recievedPacket = DequeuePacket(consumer);

					Assert.AreEqual(sendPacket.Body, recievedPacket.Body);
				}
			}
		}

		/// <summary>
		/// Tests send message into not exists queue.
		/// </summary>
		[Test]
		public void PublishBadTargetQueueTest()
		{
			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, TestQueueName))
			{
				var sendPacket = new PublishConsumePacket
				{
					Body = GenerateRandomByteArray()
				};

				const string queueName = "queueThatShouldNotExists";
				communicationService.Publish(queueName, new[] {sendPacket});
				communicationService.DeleteQueue(queueName);
			}
		}

		/// <summary>
		/// Test un ack send message.
		/// </summary>
		[Test]
		public void UnAckPublishTest()
		{
			var sendPacket = new PublishConsumePacket
			{
				Body = GenerateRandomByteArray()
			};

			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, TestQueueName))
			{
				using (ICommunicationConsumer unAckConsumer = communicationService.CreateConsumer(TestQueueName))
				{
					unAckConsumer.StartConsume();

					communicationService.Publish(TestQueueName, new[] {sendPacket});

					PublishConsumePacket recievedPacket;
					bool recieved = unAckConsumer.Dequeue(out recievedPacket, 1000);
					if (!recieved)
					{
						Assert.Fail("Consumer dequeue function timeout.");
					}
				}

				using (ICommunicationConsumer ackConsumer = communicationService.CreateConsumer(TestQueueName))
				{
					ackConsumer.StartConsume();

					PublishConsumePacket recievedPacket = DequeuePacket(ackConsumer);
					Assert.AreEqual(sendPacket.Body, recievedPacket.Body);
				}

				using (ICommunicationConsumer emptyConsumer = communicationService.CreateConsumer(TestQueueName))
				{
					emptyConsumer.StartConsume();
					PublishConsumePacket recievedPacket;
					bool recieved = emptyConsumer.Dequeue(out recievedPacket, 100);
					if (recieved)
					{
						Assert.Fail("There should not be any other packet in queue.");
					}
				}
			}
		}

		/// <summary>
		/// Tests multiple consumer dispose method.
		/// </summary>
		[Test]
		public void ConsumerDisposeTest()
		{
			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, TestQueueName))
			{
				for (int i = 0; i < 10; i++)
				{
					using (ICommunicationConsumer consumer = communicationService.CreateConsumer(TestQueueName))
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
				ICommunicationConsumer consumer = communicationService.CreateConsumer(TestQueueName);

				PublishConsumePacket recievedPacket = null;

				Assert.Throws<CommunicationException>(() => recievedPacket = DequeuePacket(consumer));
				Assert.Throws<CommunicationException>(() => consumer.Ack(recievedPacket));

				consumer.StartConsume();
				consumer.Dispose();

				Assert.Throws<CommunicationException>(() => recievedPacket = DequeuePacket(consumer));
				Assert.Throws<CommunicationException>(() => consumer.Ack(recievedPacket));
			}
		}

		/// <summary>
		/// Tests double ack for one message.
		/// </summary>
		[Test]
		[Ignore("RabbitMQ implementation changed?")]
		public void DoubleAckTest()
		{
			using (ICommunicationService communicationService = new RabbitCommunicationService(Logger, TestQueueName))
			{
				for (int i = 0; i < 10; i++)
				{
					using (ICommunicationConsumer consumer = communicationService.CreateConsumer(TestQueueName))
					{
						consumer.StartConsume();
						Thread.Sleep(100);


						var sendPacket = new PublishConsumePacket
						{
							Body = GenerateRandomByteArray()
						};

						communicationService.Publish(TestQueueName, new[] {sendPacket});

						PublishConsumePacket recievedPacket;
						bool recieved = consumer.Dequeue(out recievedPacket, 1000);
						if (!recieved)
						{
							Assert.Fail("Consumer dequeue function timeout.");
						}

						// Better do not, after this the connection will became unstable and crash
						consumer.Ack(recievedPacket);

						Assert.Throws<CommunicationException>(() => consumer.Ack(recievedPacket));
					}
				}
			}
		}
	}
}
