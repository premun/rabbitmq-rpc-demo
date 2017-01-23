using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using RabbitMQDemo.Communication.CommunicationService;
using RabbitMQDemo.Library;

namespace RabbitMQDemo.Communication.Callers
{
	/// <summary>
	/// Creates RPC caller of interface T.
	/// </summary>
	/// <typeparam name="T">Caller interface</typeparam>
	public class CallerProxy<T> : RealProxy
	{
		private readonly string _targetQueueName;
		private readonly ICommunicationService _communicationService;

		private CallerProxy(
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
			return (T)new CallerProxy<T>(communicationService, targetQueueName).GetTransparentProxy();
		}

		public override IMessage Invoke(IMessage msg)
		{
			var methodCall = (IMethodCallMessage)msg;
			var method = (MethodInfo)methodCall.MethodBase;

			var parameters = new object[methodCall.InArgCount];

			for (int i = 0; i < methodCall.InArgCount; i++)
			{
				parameters[i] = methodCall.GetInArg(i);
			}


			var methodCallContext = new MethodCallContext
			{
				MethodName = method.Name,
				Parameters = parameters
			};

			var packet = new RpcCommunicationPacket
			{
				Body = Serializer.Serialize(methodCallContext)
			};

			object methodResult = null;
			if (method.ReturnType == typeof(void))
			{
				_communicationService.CallRpc(_targetQueueName, packet, RpcCallType.DoNotExpectReply);
			}
			else
			{
				RpcCommunicationPacket replyPacket = _communicationService.CallRpc(_targetQueueName, packet);
				var replyMethodContext = Serializer.Deserialize<MethodCallContext>(replyPacket.Body);
				methodResult = replyMethodContext.Result;
			}
				
			return new ReturnMessage(methodResult, null, 0, methodCall.LogicalCallContext, methodCall);
		}
	}
}
