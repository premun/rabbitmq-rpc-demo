namespace RabbitMQDemo.Communication.Definitions
{
	public interface IExampleConsumer
	{
		void DisplayMessage(string message);

		int Multiply(int x, int y);
	}
}