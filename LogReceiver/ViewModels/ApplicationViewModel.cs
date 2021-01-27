using LogLib;
using LogReceiver.Modules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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

		private DispatcherTimer timer;

		public ApplicationViewModel(IReceiverModule MulticastReceiver, IReceiverModule UnicastReceiver,int BufferLength,int RefreshDelay)
		{
			this.bufferLength = BufferLength;
			Clients = new ObservableCollection<ClientViewModel>();

			this.multicastReceiver = MulticastReceiver;
			this.unicastReceiver = UnicastReceiver;

			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(RefreshDelay);
			timer.Tick += Timer_Tick;
			timer.Start();

		}

		

		public void Dispose()
		{
			timer.Stop();
			
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			Tuple<IPEndPoint,Log>[] items;

			items = unicastReceiver.GetLogs();
			foreach(Tuple<IPEndPoint, Log> item in items)
			{
				OnLogReceived(item.Item1.ToString(), item.Item2);
			}
		}
		

		private void OnLogReceived(string Client,Log Log)
		{
			ClientViewModel clientViewModel;

			clientViewModel = Clients.FirstOrDefault(item => item.Name == Client);
			if (clientViewModel==null)
			{
				clientViewModel = new ClientViewModel(bufferLength);
				clientViewModel.Name = Client;
				clientViewModel.Close += ClientViewModel_Close;
				Clients.Add(clientViewModel);
				if (SelectedItem == null) SelectedItem = clientViewModel;
			}

			clientViewModel.Add(Log);
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
		
		public void CloseAll()
		{
			foreach(ClientViewModel clientViewModel in Clients)
			{
				clientViewModel.Close -= ClientViewModel_Close;
			}
			Clients.Clear();
			SelectedItem = null;
		}


	}
}
