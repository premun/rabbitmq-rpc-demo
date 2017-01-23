using System;
using System.Collections.Generic;

namespace RabbitMQDemo.Communication.Publishers
{
	/// <summary>
	/// Publish packets of TPacket on the given queue.
	/// </summary>
	/// <typeparam name="TPacket">Type of publishing packet</typeparam>
	public interface IPublisher<TPacket>
		where TPacket : class
	{
		/// <summary>
		/// Publish jobs with priority 0 (lowest).
		/// </summary>
		/// <param name="jobs">Jobs to publish</param>
		void Publish(IEnumerable<TPacket> jobs);

		/// <summary>
		/// Publish jobs with priority given by the prioritySelector to each job.
		/// </summary>
		/// <param name="jobs">Jobs to publish</param>
		/// <param name="prioritySelector">
		/// Selector get jobs and returns the job priority. 
		/// Job priority must be from 0(lowest)-9(highest).  
		/// </param>
		void Publish(IEnumerable<TPacket> jobs, Func<TPacket, byte> prioritySelector);

		/// <summary>
		/// Publish jobs with priority 0 (lowest) and set them given headers
		/// </summary>
		/// <param name="jobs">Jobs to publish</param>
		/// <param name="headers">Headers for the publishing jobs</param>
		void Publish(IEnumerable<TPacket> jobs, IDictionary<string, object> headers);
	}
}
