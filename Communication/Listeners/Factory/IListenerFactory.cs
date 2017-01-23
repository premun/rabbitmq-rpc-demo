namespace ForeCastle.Communication.Listeners.Factory
{
	/// <summary>
	/// Listener factory. Provides creation of listeners.
	/// TODO: comments that have some actual value
	/// </summary>
	public interface IListenerFactory
	{
		/// <summary>
		/// Creates a listener of T.
		/// </summary>
		/// <typeparam name="T">Listener type</typeparam>
		/// <returns>Listener of T</returns>
		IListener<T> CreateListener<T>();
	}
}
