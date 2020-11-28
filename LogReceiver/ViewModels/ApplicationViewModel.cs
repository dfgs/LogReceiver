using LogReceiver.Modules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LogReceiver.ViewModels
{
	public class ApplicationViewModel:DependencyObject
	{
		public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<string>), typeof(ApplicationViewModel));
		public ObservableCollection<string> Items
		{
			get { return (ObservableCollection<string>)GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}

		public ApplicationViewModel(IMulticastReceiverModule Receiver)
		{
			Items = new ObservableCollection<string>();
			Receiver.LogReceived += Receiver_LogReceived;
		}

		private void Receiver_LogReceived(object sender, LogReceivedEventArgs e)
		{
			Items.Add(e.Log);
		}


	}
}
