using LogLib;
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
	public class ApplicationViewModel:DependencyObject,IDisposable
	{

		public static readonly DependencyProperty ClientsProperty = DependencyProperty.Register("Clients", typeof(ObservableCollection<ClientViewModel>), typeof(ApplicationViewModel));
		public ObservableCollection<ClientViewModel> Clients
		{
			get { return (ObservableCollection<ClientViewModel>)GetValue(ClientsProperty); }
			set { SetValue(ClientsProperty, value); }
		}

		public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(ClientViewModel), typeof(ApplicationViewModel));
		public ClientViewModel SelectedItem
		{
			get { return (ClientViewModel)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		private IReceiverModule multicastReceiver;
		private IReceiverModule unicastReceiver;

		public ApplicationViewModel(IReceiverModule MulticastReceiver, IReceiverModule UnicastReceiver)
		{
			Clients = new ObservableCollection<ClientViewModel>();

			this.multicastReceiver = MulticastReceiver;
			this.unicastReceiver = UnicastReceiver;
			MulticastReceiver.LogReceived += Receiver_LogReceived;
			UnicastReceiver.LogReceived += Receiver_LogReceived;

		}

		public void Dispose()
		{
			multicastReceiver.LogReceived -= Receiver_LogReceived;
			unicastReceiver.LogReceived -= Receiver_LogReceived;
			
		}
		private void Receiver_LogReceived(object sender, LogReceivedEventArgs e)
		{
			Dispatcher.Invoke(() => OnLogReceived(e));
		}

		private void OnLogReceived(LogReceivedEventArgs e)
		{
			ClientViewModel clientViewModel;

			clientViewModel = Clients.FirstOrDefault(item => item.Name == e.Client);
			if (clientViewModel==null)
			{
				clientViewModel = new ClientViewModel();
				clientViewModel.Name = e.Client;
				Clients.Add(clientViewModel);
				if (SelectedItem == null) SelectedItem = clientViewModel;
			}

			clientViewModel.Add(e.Log);
		}

	}
}
