using System;
using System.Diagnostics;

using Renci.SshNet;

namespace ForeCastle.Library
{
	public static class ProcessManager
	{
		public enum ProcessType
		{
			Core,
			Watchdog,
			Worker,
			Reporter,
			ForecastComputer
		}

		[Serializable]
		public class ServerInfo
		{
			public string Hostname { get; set; }

			public int Port { get; set; }

			public string Username { get; set; }

			public string Password { get; set; }

			public string ProjectPath { get; set; }
		}


		/// <summary>
		/// Start the specified process remotely using SSH.
		/// </summary>
		/// <param name="server">Target server</param>
		/// <param name="process">Process type - one of Worker/Watcher/Core.</param>
		/// <param name="args">Command arguments.</param>
		/// <returns>True, if process was started successfully</returns>
		public static bool StartRemote(ServerInfo server, ProcessType process, string[] args)
		{
			// Start local processes locally
			if (server.Hostname == "127.0.0.1" || server.Hostname == "localhost")
			{
				return StartLocal(process, args);
			}

			// Glue arguments and create command
			var command = "mono "
						  + server.ProjectPath
						  + Config.Get[process + "Bin"];

			if (args != null)
			{
				command += " " + string.Join(" ", args);
			}

			return SshExecute(server, command);
		}


		/// <summary>
		/// Start the specified process locally.
		/// </summary>
		/// <param name="process">Process type - one of Worker/Watcher/Core.</param>
		/// <param name="args">Command arguments.</param>
		/// <returns>True, if the process stayed running</returns>
		public static bool StartLocal(ProcessType process, string[] args)
		{
			// Glue arguments and create command
#if WIN
			var filename = Config.Get["ProjectPath"] + Config.Get[process.ToString() + "Bin"];
			var arguments = string.Empty;
			if (args != null)
			{
				arguments = string.Join(" ", args);
			}
			return Execute(filename, arguments);
#else
			var command = "mono "
			              + Config.Get["ProjectPath"]
			              + Config.Get[process + "Bin"];

			if (args != null)
			{
				command += " " + string.Join(" ", args);
			}

			return Execute(command);
#endif
		}


		/// <summary>
		/// Run specified command remotely using SSH
		/// </summary>
		/// <param name="server">Target server.</param>
		/// <param name="command">Command to be run.</param>
		/// <returns>True, if the process started OK</returns>
		public static bool SshExecute(ServerInfo server, string command)
		{
			try
			{
				// Create SSH connection
				using (var ssh = new SshClient(server.Hostname, server.Port, server.Username, server.Password))
				{
					ssh.Connect();

					command = string.Format(
						"nohup {0} >/dev/null 2>&1 &",
						command
					);

					// Execute the command with no output on standard output/error
					// Then terminate the ssh connection
					// TODO: when logging system exists, do not create log files here
					//		 this can be done with " >/dev/null 2>&1"
					var cmd = ssh.CreateCommand(command);
					cmd.Execute();

					if (cmd.Error.Length > 0)
					{
						return false;
					}

					return cmd.ExitStatus == 0;
				}
			}
			catch
			{
				return false;
			}
		}


#if WIN
        /// <summary>
		/// Run specified command locally
		/// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="arguments">Arguments</param>
        /// <returns>True, if the process stayed running</returns>
		private static bool Execute(string fileName, string arguments)
#else
        /// <summary>
        /// Run specified command locally
        /// </summary>
        /// <param name="command">Command to be executed</param>
		/// <returns>True, if the process was executed</returns>
        private static bool Execute(string command)
#endif
        {
		//	try
		//	{
				// Execute command with nohup
				var proc = new ProcessStartInfo
				{
#if WIN
					FileName = fileName,
					Arguments = arguments,
					UseShellExecute = false,
#else
					UseShellExecute = true,
					FileName = "nohup",
					Arguments = command,
#endif
					WindowStyle = ProcessWindowStyle.Hidden
				};
				var result = Process.Start(proc);

	        return result != null;
	        /*		}
			catch (Exception e)
			{
#if WIN
				Console.Error.WriteLine("Couldn't start process '{1}' locally: {0}", e.Message, fileName);
#else
				Console.Error.WriteLine("Couldn't start process locally: {0}", e.Message);
#endif
				return false;
			}
	 */
        }
	}
}
