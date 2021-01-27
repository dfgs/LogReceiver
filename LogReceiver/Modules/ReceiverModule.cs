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
	public abstract class ReceiverModule : ThreadModule, IReceiverModule
	{
		private readonly object locker = new object();
		private List<Tuple<IPEndPoint, Log>> items;

		public ReceiverModule(ILogger Logger) : base(Logger)
		{
			items = new List<Tuple<IPEndPoint, Log>>();
		}

		protected abstract Log ReceiveLog(out IPEndPoint Sender);

		public Tuple<IPEndPoint, Log>[] GetLogs()
		{
			Tuple<IPEndPoint,Log>[] result;

			lock(locker)
			{
				result = items.ToArray();
				items.Clear();
			}

			return result;
		}
		protected override void ThreadLoop()
		{
			Log log;
			IPEndPoint sender;

			LogEnter();
			

			while(State==ModuleStates.Started)
			{
				Log(LogLevels.Information, $"Waiting for data");
				try
				{
					log = ReceiveLog(out sender);
					
					Log(LogLevels.Information, $"Received new log from {sender}");
					lock(locker)
					{
						items.Add(new Tuple<IPEndPoint,Log>(sender,log));
					}

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
