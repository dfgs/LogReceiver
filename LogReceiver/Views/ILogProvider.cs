using LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReceiver.Views
{
	public class LogEventArgs:EventArgs
	{
		public Log Item
		{
			get;
			private set;
		}
		public LogEventArgs(Log Item)
		{
			this.Item = Item;
		}
	}

	public delegate void LogEventHandler(object sender, LogEventArgs e);

	public interface ILogProvider
	{
		event LogEventHandler LogAdded;
		event LogEventHandler LogRemoved;
	}
}
