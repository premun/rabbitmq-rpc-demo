namespace RabbitMQDemo.Communication.Definitions
{
	public interface IExampleConsumer
	{
		void DisplayMessage(string message);

		void Kill();

		int Multiply(int x, int y);
	}
}