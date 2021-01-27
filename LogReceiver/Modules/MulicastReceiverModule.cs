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
	public class MulicastReceiverModule : ReceiverModule
	{
		private IPAddress multicastIPaddress;
		private int port;
		private UdpClient client;


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

		protected override Log ReceiveLog(out IPEndPoint Sender)
		{
			Byte[] buffer;
			Log log;

			LogEnter();

			Sender = new IPEndPoint(0, 0);
			buffer = client.Receive(ref Sender);
			log = LogLib.Log.Deserialize(buffer);

			return log;
		}

		

	}
}
