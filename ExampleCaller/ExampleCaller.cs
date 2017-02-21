using System;
using System.Linq;
using System.Text.RegularExpressions;
using Autofac;
using RabbitMQDemo.Communication.Callers.Factory;
using RabbitMQDemo.CommunicationInterface;
using RabbitMQDemo.ExampleCallee;
using RabbitMQDemo.Library;

namespace RabbitMQDemo.ExampleCaller
{
	public class ExampleCaller
	{
		private static readonly Regex MathRegex = new Regex("^[0-9]+x[0-9]+$");

		private IExampleInterface _caller;

		public void Run()
		{
			Console.Write("Who do you want to talk to?\n> ");

			string consumerId = Console.ReadLine();
			if (string.IsNullOrEmpty(consumerId))
			{
				return;
			}

			var id = new ExampleCalleeIdentifier(consumerId);
			_caller = GetCaller<IExampleInterface>(id);

			bool keepGoing;
			do
			{
				Console.Write("Please enter message:\n> ");
				keepGoing = ProcessInput(Console.ReadLine());
			} while (keepGoing);
		}

		private bool ProcessInput(string message)
		{
			if (string.IsNullOrEmpty(message) || message.Equals("q"))
			{
				return false;
			}

			if (MathRegex.IsMatch(message))
			{
				var numbers = message.Split('x').Select(int.Parse).ToArray();

				// Do RPC
				int result = _caller.Multiply(numbers[0], numbers[1]);

				Console.WriteLine($"{numbers[0]} x {numbers[1]} = {result}\n");
			}
			else
			{
				Console.WriteLine("Sending message..");

				// Do RPC
				_caller.DisplayMessage(message);

				Console.WriteLine("Message sent\n");
			}

			return true;
		}
		
		private static T GetCaller<T>(Identifier id)
		{
			return Program.Container
				.Resolve<ICallerFactory>()
				.CreateCaller<T>(id);
		}
	}
}