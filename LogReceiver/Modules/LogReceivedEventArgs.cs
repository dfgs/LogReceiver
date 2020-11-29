using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReceiver
{
	public class LogReceivedEventArgs:EventArgs
	{
		public string Client
		{
			get;
			set;
		}
		public Log Log
		{
			get;
			private set;
		}

		public LogReceivedEventArgs(string Client,Log Log)
		{
			this.Client = Client; this.Log = Log;
		}

	}
}
