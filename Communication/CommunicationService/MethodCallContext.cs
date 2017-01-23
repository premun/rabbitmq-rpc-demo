using System;

namespace ForeCastle.Communication.CommunicationService
{
	/// <summary>
	/// Contains necessary data about calling method.  
	/// Is used in universal caller and listener for marshaling method call.
	/// </summary>
	[Serializable]
	public class MethodCallContext
	{
		public string MethodName { get; set; }

		public object[] Parameters { get; set; }

		public object Result { get; set; }
	}
}