using System;
using Autofac;
using ForeCastle.Communication.Callers.Factory;
using ForeCastle.Communication.Definitions;
using ForeCastle.Library.Identifier;

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
			var consumerId = Console.ReadLine();

			if (string.IsNullOrEmpty(consumerId))
			{
				return;
			}

			var id = new ConsumerIdentifier(consumerId);
			var caller = GetProxy(id);

			string message;
			do
			{
				Console.Write("Please enter message:\n> ");
				message = Console.ReadLine();

				if (message.ToLower().Equals("kill"))
				{
					caller.Kill();
					return;
				}

				caller.DisplayMessage(message);
			} while (!string.IsNullOrEmpty(message));
		}

		private IConsumer GetProxy(Identifier id)
		{
			return _container.Resolve<ICallerFactory>().CreateCaller<IConsumer>(id.RpcName);
		}
	}
}