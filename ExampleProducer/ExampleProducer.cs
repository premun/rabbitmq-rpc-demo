using System;
using System.Linq;
using System.Text.RegularExpressions;
using RabbitMQDemo.Communication.Definitions;
using RabbitMQDemo.ExampleConsumer;

namespace RabbitMQDemo.ExampleProducer
{
	public class ExampleProducer
	{
		public void Run()
		{
			Console.Write("Who do you want to talk to?\n> ");

			string consumerId = Console.ReadLine();
			if (string.IsNullOrEmpty(consumerId))
			{
				return;
			}

			var id = new ExampleConsumerIdentifier(consumerId);
			var caller = Program.GetCaller<IExampleConsumer>(id);

			var mathRegex = new Regex("^[0-9]+x[0-9]+$");

			while(true)
			{
				Console.Write("Please enter message:\n> ");
				string message = Console.ReadLine();

				if (string.IsNullOrEmpty(message))
				{
					return;
				}

				if (mathRegex.IsMatch(message))
				{
					var numbers = message
						.Split('x')
						.Select(int.Parse)
						.ToArray();

					int result = caller.Multiply(numbers[0], numbers[1]);

					Console.WriteLine($"{numbers[0]} x {numbers[1]} = {result}\n");
				}
				else
				{
					Console.WriteLine("Sending message..");
					caller.DisplayMessage(message);
					Console.WriteLine("Message sent\n");
				}
			}
		}
	}
}