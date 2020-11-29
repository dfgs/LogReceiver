using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReceiver.Views
{
	public class TailEventArgs:EventArgs
	{
		public object Item
		{
			get;
			private set;
		}
		public TailEventArgs(object Item)
		{
			this.Item = Item;
		}
	}

	public delegate void TailEventHandler(object sender, TailEventArgs e);

	public interface ITailProvider
	{
		event TailEventHandler UpdateScroll;
	}
}
