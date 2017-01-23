using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using ForeCastle.Communication.CommunicationService;
using ForeCastle.Library;

namespace ForeCastle.Communication.Callers
{
	/// <summary>
	/// Creates RPC caller of interface T.
	/// For definition of caller <see href="https://sites.google.com/site/logioforecastle/navody/communication-project/rpc"/>
	/// </summary>
	/// <typeparam name="T">Caller interface</typeparam>
	public class UniversalCallerProxy<T> : RealProxy
	{
		private readonly string _targetQueueName;
		private readonly ICommunicationService _communicationService;

		private UniversalCallerProxy(
			ICommunicationService communicationService,
			string targetQueueName)
			: base(typeof(T))
		{
			_communicationService = communicationService;
			_targetQueueName = targetQueueName;
		}

		/// <summary>
		/// Create new instance of caller of interface T.
		/// </summary>
		/// <param name="communicationService">Communication service</param>
		/// <param name="targetQueueName">Caller`s target queue.</param>
		/// <returns>Caller of interface T</returns>
		public static T Create(
			ICommunicationService communicationService,
			string targetQueueName)
		{
			return (T)new UniversalCallerProxy<T>(communicationService, targetQueueName).GetTransparentProxy();
		}

		public override IMessage Invoke(IMessage msg)
		{
			var methodCall = (IMethodCallMessage)msg;
			var method = (MethodInfo)methodCall.MethodBase;

			var methodCallContext = new MethodCallContext()
			{
				MethodName = method.Name
			};
			var parameters = new object[methodCall.InArgCount];

			for (int i = 0; i < methodCall.InArgCount; i++)
			{
				parameters[i] = methodCall.GetInArg(i);
			}

			methodCallContext.Parameters = parameters;
			var sendPacket = new RpcCommunicationPacket
			{
				Body = Serializer.Serialize(methodCallContext)
			};

			object methodResult = null;
			if (method.ReturnType == typeof(void))
			{
				_communicationService.CallRpc(_targetQueueName, sendPacket, RpcCallType.DoNotExpectReply);
			}
			else
			{
				var replyPacket = _communicationService.CallRpc(_targetQueueName, sendPacket);
				var replyMethodContext = Serializer.Deserialize<MethodCallContext>(replyPacket.Body);
				methodResult = replyMethodContext.Result;
			}
				
			return new ReturnMessage(methodResult, null, 0, methodCall.LogicalCallContext, methodCall);
		}
	}
}
