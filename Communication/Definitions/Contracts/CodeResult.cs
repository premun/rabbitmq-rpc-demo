using System;

namespace ForeCastle.Communication.Definitions.Contracts
{
	/// <summary>
	/// Generic communication contract used as return type of method. 
	/// Contains status code and result from method. 
	/// </summary>
	/// <typeparam name="TCode">Type of status code, usually enum.</typeparam>
	/// <typeparam name="TResult">Type of returned result.</typeparam>
	[Serializable]
	public class CodeResult<TCode, TResult>
	{
		/// <summary>
		/// Gets or sets the status code.
		/// </summary>
		/// <value>The status code.</value>
		public TCode Code { get; set; }

		/// <summary>
		/// Gets or sets the result.
		/// </summary>
		/// <value>The result.</value>
		public TResult Result { get; set; }

		public CodeResult()
		{
			/* empty */
		}

		public CodeResult(TCode code, TResult result)
		{
			Code = code;
			Result = result;
		}
	}
}
