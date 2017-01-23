using RabbitMQDemo.Library;

namespace RabbitMQDemo.Communication.Callers.Factory
{
	/// <summary>
	/// Caller factory. Provides creation of callers.
	/// </summary>
	public interface ICallerFactory
	{
		/// <summary>
		/// Creates caller of a given interface T with given target queue.
		/// </summary>
		/// <typeparam name="T">Caller type</typeparam>
		/// <param name="targetQueueName">Target queue name</param>
		/// <returns>Caller of type T</returns>
		T CreateCaller<T>(string targetQueueName);

		/// <summary>
		/// Creates caller of a given interface T with given target queue.
		/// </summary>
		/// <typeparam name="T">Caller type</typeparam>
		/// <param name="target">Target queue name</param>
		/// <returns>Caller of type T</returns>
		T CreateCaller<T>(Identifier target);
	}
}
