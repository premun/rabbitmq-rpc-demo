using System;
using Autofac;
using ForeCastle.Library.Identifier;
using RabbitMQDemo.Communication.Callers.Factory;
using RabbitMQDemo.Communication.Definitions;

namespace ForeCastle.Producer
{
	public class Producer
	{
		private readonly IContainer _container;

		public Producer(IContainer container)
		{
			_container = container;
		}

		public void Run()
		{
			Console.Write("Please enter consumer id:\n> ");
			string consumerId = Console.ReadLine();

			if (string.IsNullOrEmpty(consumerId))
			{
				return;
			}

			var id = new ConsumerIdentifier(consumerId);
			IConsumer caller = GetProxy(id);

			Console.WriteLine(caller.Multiply(14, 984));
			while(true)
			{
				Console.Write("Please enter message:\n> ");
				string message = Console.ReadLine();

				if (string.IsNullOrEmpty(message))
				{
					return;
				}

				if (message.ToLower().Equals("kill"))
				{
					caller.Kill();
					return;
				}

				caller.DisplayMessage(message);
			}
		}

		private IConsumer GetProxy(Identifier id)
		{
			return _container.Resolve<ICallerFactory>().CreateCaller<IConsumer>(id.RpcName);
		}
	}
}