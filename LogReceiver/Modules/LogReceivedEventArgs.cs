using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReceiver
{
	public class LogReceivedEventArgs:EventArgs
	{
		public string Log
		{
			get;
			private set;
		}

		public LogReceivedEventArgs(string Log)
		{
			this.Log = Log;
		}

	}
}
