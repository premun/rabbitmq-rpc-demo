using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQDemo.Library;

namespace RabbitMQDemo.Communication.Utils
{
	/// <summary>
	/// Contains utility methods for work with queues.
	/// </summary>
	public static class QueueUtils
	{
		public const string PriorityArg = "x-max-priority";
		public const string DlxArg = "x-dead-letter-exchange";
		private const string DlxType = "fanout";

		public static readonly string JobQueueName = Config.Get["Queue.work.name"];
		public static readonly string OutputQueueName = Config.Get["Queue.output.name"];
		public static readonly string ErrorQueueName = Config.Get["Queue.error.name"];

		/// <summary>
		/// Converts origin queue name to the dead letter queue name
		/// </summary>
		/// <param name="queueName">Original 'parent' queue</param>
		/// <returns>DLQ name</returns>
		public static string GetDeadQueueName(string queueName)
		{
			return queueName + "_dead_queue";
		}

		/// <summary>
		/// Converts origin queue name to the dead letter exchange name
		/// </summary>
		/// <param name="queueName">Original 'parent' queue</param>
		/// <returns>DLX name</returns>
		public static string GetDeadExchangeName(string queueName)
		{
			return queueName + "_dead_exchange";
		}

		/// <summary>
		/// Declares new dead letter exchange (DLX) and dead letter queue (DLQ) to the given
		/// working queue.
		/// </summary>
		/// <param name="channel">RabbitMQ channel for creating a DLX and DLQ</param>
		/// <param name="workingQueue">Original 'parent' queue</param>
		/// <returns>DLX name</returns>
		public static string DeclareDeadLetterExchange(IModel channel, string workingQueue)
		{
			string dlx = GetDeadExchangeName(workingQueue);
			string dlxQueue = GetDeadQueueName(workingQueue);
			channel.ExchangeDeclare(dlx, DlxType, true);
			var queueOptions = new Dictionary<string, object>
				{
					{ PriorityArg, 10 }
				};
			channel.QueueDeclare(dlxQueue, true, false, false, queueOptions);
			channel.QueueBind(dlxQueue, dlx, ".*");
			return dlx;
		}
	}
}