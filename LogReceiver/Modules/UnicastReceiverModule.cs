using LogLib;
using ModuleLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LogReceiver.Modules
{
	public class UnicastReceiverModule : ThreadModule, IReceiverModule
	{
		private readonly object locker = new object();
		private int port;
		private UdpClient client;

		public event LogReceivedEventHandler LogReceived;

		public UnicastReceiverModule(ILogger Logger, int Port) : base(Logger)
		{
			this.port = Port;

		}

		protected override void OnStopping()
		{
			base.OnStopping();
			Log(LogLevels.Information, $"Closing client");
			try
			{
				client.Close();
			}
			catch (Exception ex)
			{
				Log(ex);
			}
		}

		protected override void OnStarting()
		{
			base.OnStarting();
			IPEndPoint localEndPoint;

			Log(LogLevels.Information, $"Initialize multicast client");
			try
			{
				localEndPoint = new IPEndPoint(IPAddress.Any, port);
				client = new UdpClient(AddressFamily.InterNetwork);
				client.Client.Bind(localEndPoint);
			}
			catch (Exception ex)
			{
				Log(ex);
				return;
			}
		}
		protected override void ThreadLoop()
		{
			IPEndPoint sender;
			Byte[] buffer;
			Log log;

			LogEnter();
			

			while(State==ModuleStates.Started)
			{
				Log(LogLevels.Information, $"Waiting for data");
				try
				{
					sender = new IPEndPoint(0, 0);
					buffer = client.Receive(ref sender);
					log = LogLib.Log.Deserialize(buffer);
					
					Log(LogLevels.Information, $"Received new log from {sender}");
					// Event called async to avoid blocking when app close
					if (LogReceived != null) LogReceived.BeginInvoke(this, new LogReceivedEventArgs(sender.ToString(),log), null, null);
				}
				catch (Exception ex)
				{
					if (State != ModuleStates.Started) return;
					Log(ex);
				}
			}

			


		}
		

	}
}
