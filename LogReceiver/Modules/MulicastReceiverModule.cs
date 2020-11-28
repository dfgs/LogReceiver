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
	public class MulicastReceiverModule : Module, IMulticastReceiverModule
	{
		private UdpClient client;
		private readonly object locker = new object();
		private IPAddress multicastIPaddress;
		private IPEndPoint remoteEndPoint;

		public event LogReceivedEventHandler LogReceived;

		public MulicastReceiverModule(ILogger Logger,IPAddress MulticastIPaddress, int Port) : base(Logger)
		{
			IPEndPoint localEndPoint;

			try
			{
				Log(LogLevels.Information, $"Initialize multicast client");

				this.multicastIPaddress = MulticastIPaddress;
				localEndPoint = new IPEndPoint(IPAddress.Any, Port);
				remoteEndPoint = new IPEndPoint(MulticastIPaddress, Port);

				client = new UdpClient(AddressFamily.InterNetwork);

				client.ExclusiveAddressUse = false;
				client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
				client.ExclusiveAddressUse = false;
				client.Client.Bind(localEndPoint);
				client.JoinMulticastGroup(MulticastIPaddress, IPAddress.Any);

				Log(LogLevels.Information, $"BeginReceive");
				client.BeginReceive(ReceivedCallback, null);
			}
			catch(Exception ex)
			{
				Log(ex);
			}
		}

		public override void Dispose()
		{
			LogEnter();
			try
			{
				client.DropMulticastGroup(multicastIPaddress);
				client.Close();
			}
			catch(Exception ex)
			{
				Log(ex);
			}
		}

		private void ReceivedCallback(IAsyncResult ar)
		{
			IPEndPoint sender;
			Byte[] buffer;

			LogEnter();

			try
			{
				sender = new IPEndPoint(0, 0);
				buffer = client.EndReceive(ar, ref sender);
			}
			catch(Exception ex)
			{
				Log(ex);
				return;
			}
			Log(LogLevels.Information, $"Received new log from {sender}");


			if (LogReceived != null) LogReceived(this, new LogReceivedEventArgs(Encoding.Default.GetString(buffer)));

			client.BeginReceive(new AsyncCallback(ReceivedCallback), null);
		}

	}
}
