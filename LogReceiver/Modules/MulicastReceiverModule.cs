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
	public class MulicastReceiverModule : ThreadModule, IReceiverModule
	{
		private readonly object locker = new object();
		private IPAddress multicastIPaddress;
		private int port;
		private UdpClient client;

		public event LogReceivedEventHandler LogReceived;

		public MulicastReceiverModule(ILogger Logger,IPAddress MulticastIPaddress, int Port) : base(Logger)
		{
			this.multicastIPaddress = MulticastIPaddress;
			this.port = Port;
		}


		protected override void OnStarting()
		{
			IPEndPoint localEndPoint;

			base.OnStarting();

			Log(LogLevels.Information, $"Initialize multicast client");
			try
			{

				localEndPoint = new IPEndPoint(IPAddress.Any, port);
				//remoteEndPoint = new IPEndPoint(multicastIPaddress, port);

				client = new UdpClient(AddressFamily.InterNetwork);

				client.ExclusiveAddressUse = false;
				client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
				client.ExclusiveAddressUse = false;
				client.Client.Bind(localEndPoint);
				client.JoinMulticastGroup(multicastIPaddress, IPAddress.Any);

			}
			catch (Exception ex)
			{
				Log(ex);
				return;
			}
		}
		protected override void OnStopping()
		{
			base.OnStopping();
			Log(LogLevels.Information, $"Closing client");
			try
			{
				client.DropMulticastGroup(multicastIPaddress);
				client.Close();
			}
			catch (Exception ex)
			{
				Log(ex);
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
					if (LogReceived != null) LogReceived.BeginInvoke(this, new LogReceivedEventArgs(sender.ToString(), log), null, null);
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
