using System.Collections.Generic;

namespace RabbitMQDemo.Communication.Utils
{
	/// <summary>
	/// Method call log.
	/// </summary>
    public class MethodCallLog
    {
        /// <summary>
        /// Gets or sets the method name.
        /// </summary>
        /// <value>The method name.</value>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets the method parameters.
        /// </summary>
        /// <value>The method parameters.</value>
        public object[] Parameters { get; set; }

        /// <summary>
        /// Gets or sets the method result.
        /// </summary>
        /// <value>The method result.</value>
        public object Result { get; set; }
    }

    /// <summary>
    /// Provides functionality to log method calls.
    /// </summary>
    public class MethodCallLogger
    {
        private readonly List<MethodCallLog> _logs = new List<MethodCallLog>();

		/// <summary>
		/// Log the specified log.
		/// </summary>
		/// <param name="log">Log.</param>
        public void Log(MethodCallLog log)
        {
            _logs.Add(log);
        }

        /// <summary>
        /// Returns a last log.
        /// </summary>
        /// <returns>The last log.</returns>
        public MethodCallLog GetLastLog()
        {
            int indexOfLast = _logs.Count - 1;
            if (indexOfLast == -1)
            {
                return null;
            }

            return  _logs[indexOfLast];
        }
    }
}
