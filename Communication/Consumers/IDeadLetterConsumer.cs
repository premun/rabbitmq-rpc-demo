namespace RabbitMQDemo.Communication.Consumers
{
	/// <summary>
	/// Consume dead packets of TPacket on the given queue. 
	/// New interface for IoC type detection - for more info see <code>IExampleConsumer</code>
	/// </summary>
	/// <typeparam name="TPacket">Type of consuming packet</typeparam>
	public interface IDeadLetterConsumer<TPacket> : IConsumer<TPacket> where TPacket : class
	{		 
	}
}