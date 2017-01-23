using System;
using System.Collections.Generic;
using System.IO;

namespace ForeCastle.Library
{
    /// <summary>
    /// Thread safe singleton configuration
    /// </summary>
    public class Config
    {
        private readonly Dictionary<string, string> _dictionary;

        private static readonly Lazy<Config> Lazy =
            new Lazy<Config>(() => new Config());

        public static Config Get => Lazy.Value;

	    public static string ConfigFilesDir { get; set; }

        /// <summary>
        /// Parses properties files and prepares the dictionary
        /// </summary>
        private Config()
        {
            _dictionary = new Dictionary<string, string>();
            var dir = ConfigFilesDir ?? AppDomain.CurrentDomain.BaseDirectory + "/../../../";

            ReadPropertyFile(dir + "RabbitMQDemo.properties");
        }

        /// <summary>
        /// Reads the property file.
        /// </summary>
        /// <param name="fileName">Path to the file.</param>
        public void ReadPropertyFile(string fileName)
        {
            foreach (var line in File.ReadAllLines(fileName))
            {
                if (string.IsNullOrEmpty(line)
                    || line.StartsWith(";")
                    || line.StartsWith("#")
                    || line.StartsWith("'")
                    || !line.Contains("="))
                {
                    continue;
                }

                var index = line.IndexOf('=');
                var key = line.Substring(0, index).Trim();
                var value = line.Substring(index + 1).Trim();

                if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                    (value.StartsWith("'") && value.EndsWith("'")))
                {
                    value = value.Substring(1, value.Length - 2);
                }

                if (_dictionary.ContainsKey(key))
                {
                    _dictionary[key] = value;
                }
                else
                {
                    _dictionary.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Returns loaded value from properties files
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Founded value</returns>
        /// <exception cref="KeyNotFoundException" accessor="get">Missing key <paramref name="key"/></exception>
        public string this[string key]
        {
            get
            {
                if (!_dictionary.ContainsKey(key))
                {
                    throw new KeyNotFoundException("Config doesn't contain key `" + key + "`");
                }

                return _dictionary[key];
            }
        }


        /// <summary>
        /// Prepares a MySQL connection string with given name.
        /// ConnectionString are cached for better performance (encryption time, etc.)
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="name">Name.</param>
        public string ConnectionString(string name)
        {
            return this[name];
        }
    }
}
