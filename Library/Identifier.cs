using System;
using System.IO;
using System.Text.RegularExpressions;

namespace RabbitMQDemo.Library
{
	/// <summary>
	/// Identifiers of the distributed system nodes.
	/// </summary>
	[Serializable]
	public abstract class Identifier
	{
		public static readonly char Delimiter = '_';

		public string Id { get; set; }

		private string Prefix { get; }

		private string Type { get; }

		protected Identifier(string id, string type)
		{
			Type = type;
			Id = id;
			Prefix = Config.Get["Logger.prefix"];
		}

		public override string ToString()
		{
			return Id;
		}

		/// <summary>
		/// The full name
		/// </summary>
		public virtual string FullName => $"{Prefix}{Type}_{Id}";

		/// <summary>
		/// The name of a RPC queue
		/// </summary>
		public virtual string RpcName => $"RPC_{Type}_{Id}";

		/// <summary>
		/// A suitable name for a log file
		/// </summary>
		public string LogName
		{
			get
			{ 
				var logName = FullName;

				// throw out possible invalid characters
				var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
				var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

				// substitute them with underscores
				logName = Regex.Replace(logName, invalidRegStr, "_");

				// replace all __ with single _
				while (logName.Contains("__"))
				{
					logName = logName.Replace("__", "_");
				}

				return logName;
			}
		}
	}
}
