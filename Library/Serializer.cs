using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RabbitMQDemo.Library
{
	/// <summary>
	/// Provides object serialization (byte or json). 
	/// Is shared between all projects of Forecastle.
	/// </summary>
	public class Serializer
	{
		/// <summary>
		/// Serializes given object to byte array.
		/// </summary>
		/// <typeparam name="T">Type of the object to be serialized</typeparam>
		/// <param name="input">Object to be serialized</param>
		/// <returns>Byte array of serialized data</returns>
		public static byte[] Serialize<T>(T input)
		{
			// Binary serialization
			var memoryStream = new MemoryStream();
			var binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, input);
			memoryStream.Flush();
			memoryStream.Seek(0, SeekOrigin.Begin);

			return memoryStream.GetBuffer();
		}

		/// <summary>
		/// Deserializes object to the given type T.
		/// </summary>
		/// <typeparam name="T">Type of the object to be deserialized</typeparam>
		/// <param name="input">Byte array of serialized data</param>
		/// <returns>Deserialized object</returns>
		public static T Deserialize<T>(byte[] input)
		{
			// Binary deserialization
			var memoryStream = new MemoryStream(input.Length);
			memoryStream.Write(input, 0, input.Length);
			memoryStream.Seek(0, SeekOrigin.Begin);
			var binaryFormatter = new BinaryFormatter();

			return (T)binaryFormatter.Deserialize(memoryStream);
		}
	}
}
