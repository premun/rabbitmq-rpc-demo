using System;
using System.Linq;
using System.Text.RegularExpressions;
using Autofac;
using ForeCastle.Library.Identifier;
using RabbitMQDemo.Communication.Callers.Factory;
using RabbitMQDemo.Communication.Definitions;

namespace RabbitMQDemo.ExampleProducer
{
	public class ExampleProducer
	{
		private readonly IContainer _container;

		public ExampleProducer(IContainer container)
		{
			_container = container;
		}

		public void Run()
		{
			Console.Write("Who do you want to talk to?\n> ");

			string consumerId = Console.ReadLine();
			if (string.IsNullOrEmpty(consumerId))
			{
				return;
			}

			var id = new ConsumerIdentifier(consumerId);
			IExampleConsumer caller = GetCallerProxy(id);

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

					Console.WriteLine($"{numbers[0]} x {numbers[1]} = {result}");
				}
				else
				{
					caller.DisplayMessage(message);
				}
			}
		}

		private IExampleConsumer GetCallerProxy(Identifier id)
		{
			return _container.Resolve<ICallerFactory>().CreateCaller<IExampleConsumer>(id);
		}
	}
}