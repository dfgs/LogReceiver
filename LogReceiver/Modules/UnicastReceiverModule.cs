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
	public class UnicastReceiverModule : ReceiverModule
	{
		private int port;
		private UdpClient client;


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
