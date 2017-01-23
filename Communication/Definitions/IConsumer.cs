namespace ForeCastle.Communication.Definitions
{
	public interface IConsumer
	{
		void DisplayMessage(string message);

		void Kill();

		int Multiply(int x, int y);
	}
}