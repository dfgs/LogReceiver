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
		private int bufferLength;

		public ApplicationViewModel(IReceiverModule MulticastReceiver, IReceiverModule UnicastReceiver,int BufferLength)
		{
			this.bufferLength = BufferLength;
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
				clientViewModel = new ClientViewModel(bufferLength);
				clientViewModel.Name = e.Client;
				clientViewModel.Close += ClientViewModel_Close;
				Clients.Add(clientViewModel);
				if (SelectedItem == null) SelectedItem = clientViewModel;
			}

			clientViewModel.Add(e.Log);
		}

		private void ClientViewModel_Close(object sender, EventArgs e)
		{
			ClientViewModel clientViewModel;

			clientViewModel = sender as ClientViewModel;
			if (clientViewModel == null) return;

			clientViewModel.Close-= ClientViewModel_Close;
			Clients.Remove(clientViewModel);
			if (SelectedItem == clientViewModel) SelectedItem = Clients.FirstOrDefault();
		}
		


	}
}
