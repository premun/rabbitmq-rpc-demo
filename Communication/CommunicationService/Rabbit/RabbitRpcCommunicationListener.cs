using System;
using System.IO;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMQDemo.Library;

namespace RabbitMQDemo.Communication.CommunicationService.Rabbit
{
	/// <summary>
	/// Implementation for IRpcCommunicationListener using RabbitMQ.
	/// Listen for RPC packets on the given queue. 
	/// </summary>
	public class RabbitRpcCommunicationListener : IRpcCommunicationListener
	{
		private class StopListenException : Exception
		{
		}

		private readonly ILogger _logger;

		private readonly IConnection _rabbitConnection;

		private readonly string _listeningQueueName;

		private readonly Func<string, RpcCommunicationPacket, RpcCommunicationPacket> _listeningFunction;

		private IModel _privateListeningChannel;

		private IModel ListeningChannel
		{
			get
			{
				if (_disposed)
				{
					throw new StopListenException();
				}
				return _privateListeningChannel;
			}

			set
			{ 
				_privateListeningChannel = value; 
			}
		}

		private Thread _listeningThread;

		private bool _disposed;

		public string ListeningQueueName
		{ 
			get { return _listeningQueueName; } 
		}

		public Func<string, RpcCommunicationPacket, RpcCommunicationPacket> ListeningFunction
		{
			get { return _listeningFunction; }
		}

		public event EventHandler StartListening;

		public event EventHandler<ListeningThreadFailedEventArgs> ListeningThreadFailed;

		public event EventHandler StopListening;

		public Thread ListeningThread
		{
			get { return _listeningThread; }
		}

		public RabbitRpcCommunicationListener(
			ILogger logger, 
			IConnection rabbitConnection,
			string listeningQueueName,
			Func<string, RpcCommunicationPacket, RpcCommunicationPacket> listeningFunction)
		{
			_logger = logger;
			_rabbitConnection = rabbitConnection;
			_listeningQueueName = listeningQueueName;
			_listeningFunction = listeningFunction;
		}

		private void OnListeningThreadFailed(ListeningThreadFailedEventArgs e)
		{
			if (ListeningThreadFailed != null)
			{
				ListeningThreadFailed(this, e);
			}
		}

		private void OnStartListening()
		{
			if (StartListening != null)
			{
				StartListening(this, EventArgs.Empty);
			}
		}

		private void OnStopListening()
		{
			if (StopListening != null)
			{
				StopListening(this, EventArgs.Empty);
			}
		}


		public void StartListen()
		{
			_disposed = false;
			_listeningThread = new Thread(Run);
			_listeningThread.Start();
		}

		private void InitRabbitChannel()
		{
			ListeningChannel = _rabbitConnection.CreateModel();

			try
			{
				// Declare queue
				// Will throw an OperationInterruptedException if queue already exists
				ListeningChannel.QueueDeclare(ListeningQueueName, false, true, false, null);
			}
			catch (OperationInterruptedException ex)
			{
				string exceptionMessage = string.Format(
					                          "Listener could not start listening on queue `{0}`. See inner exception for details.", 
					                          ListeningQueueName);
				throw new CommunicationException(exceptionMessage, ex);
			}

			// RabbitMQ properties
			ListeningChannel.BasicQos(0, 1, false);
		}

		private void Run()
		{
			_logger.Info("Starting listener thread...");

			try
			{
				InitRabbitChannel();

				var consumer = new QueueingBasicConsumer(ListeningChannel);
				ListeningChannel.BasicConsume(ListeningQueueName, false, consumer);

				_logger.Info("Channel started. Awaiting RPC calls on `{0}`...", ListeningQueueName);

				OnStartListening();
			
				while (true)
				{
					BasicDeliverEventArgs messageReceived;

					try
					{
						messageReceived = consumer.Queue.Dequeue();
					}
					catch (EndOfStreamException)
					{
						// This is what happens when channel is closed from outside
						throw new StopListenException();
					}
						
					// Response rabbit parameters + correlation Identifier
					var recievedProperties = messageReceived.BasicProperties;
					var replyProperties = ListeningChannel.CreateBasicProperties();
					replyProperties.CorrelationId = recievedProperties.CorrelationId;

					RabbitRpcPacket recievedRpcPacket;
					
					try
					{
						recievedRpcPacket = Serializer.Deserialize<RabbitRpcPacket>(messageReceived.Body);
					}
					catch (Exception ex)
					{
						// If comes bad packet log it and continue in listening.
						_logger.Error(string.Format("Could not dematerialized RabbitRpcPacket. Exception stack trace: {0}", ex));
						continue;
					}

					var replyRpcPacket = new RabbitRpcPacket
					{
						From = _listeningQueueName,
					};
					
					// Process call
					try
					{
						replyRpcPacket.Body = ListeningFunction(recievedRpcPacket.From, recievedRpcPacket.Body);
					}
					catch (Exception ex)
					{
						// If listening function crashed log it and send back exception details.
						_logger.Error(string.Format("Couldn't process RPC call (callee side): {0}", ex), ex);

						// Create error response
						replyRpcPacket.Error = true; 
						replyRpcPacket.ExceptionMessage = ex.Message;
						replyRpcPacket.ExceptionStackTrace = ex.ToString();
					}
					finally
					{
						// If reply is expected, then send it.
						if (!string.IsNullOrEmpty(recievedProperties.ReplyTo))
						{
							var response = Serializer.Serialize(replyRpcPacket);
							ListeningChannel.BasicPublish(string.Empty, recievedProperties.ReplyTo, replyProperties, response);
						}

						ListeningChannel.BasicAck(messageReceived.DeliveryTag, false);
					}
				}
			}
			catch (StopListenException)
			{
				_logger.Info(string.Format("Listening on queue `{0}` stopped.", ListeningQueueName));
				OnStopListening();
			}
			catch (Exception ex)
			{
				_logger.Error("Listening error: {0}", ex.ToString());

				Dispose();
				OnListeningThreadFailed(new ListeningThreadFailedEventArgs { ListenException = ex });
			}
		}

		public void StopListen()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				if (_privateListeningChannel != null)
				{
					_privateListeningChannel.Dispose();
					_privateListeningChannel = null;
				}
				GC.SuppressFinalize(this);
			}
		}
	}
}
