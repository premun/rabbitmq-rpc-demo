using System;
using System.Collections.Generic;
using RabbitMQDemo.Communication.Publishers;
using RabbitMQDemo.CommunicationInterface;

namespace RabbitMQDemo.ExamplePublisher
{
	public class ExamplePublisher
	{
		private readonly IPublisher<ExampleMessage> _publisher;

		public ExamplePublisher(IPublisher<ExampleMessage> publisher)
		{
			_publisher = publisher;
		}

		public void Run()
		{
			bool keepGoing;
			do
			{
				Console.Write("How many messages should I publish?\n> ");
				keepGoing = ProcessInput(Console.ReadLine());
			} while (keepGoing);
		}

		private bool ProcessInput(string input)
		{
			if (string.IsNullOrEmpty(input) || input.Equals("q"))
			{
				return false;
			}

			int count;
			if (!int.TryParse(input, out count))
			{
				return false;
			}

			Console.WriteLine($"Publishing {count} messages...");

			var messages = new List<ExampleMessage>(count);
			for (int i = 0; i < count; ++i)
			{
				messages.Add(new ExampleMessage(i + 1, count));
			}

			_publisher.Publish(messages);

			Console.WriteLine("Messages published\n");

			return true;
		}
	}
}