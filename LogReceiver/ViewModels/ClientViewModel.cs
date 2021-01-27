using LogLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ViewModelLib;

namespace LogReceiver.ViewModels
{
	public class ClientViewModel:DependencyObject
	{
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string),typeof(ClientViewModel));
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		public static readonly DependencyProperty ComponentsProperty = DependencyProperty.Register("Components", typeof(ObservableCollection<ComponentViewModel>), typeof(ClientViewModel));
		public ObservableCollection<ComponentViewModel> Components
		{
			get { return (ObservableCollection<ComponentViewModel>)GetValue(ComponentsProperty); }
			set { SetValue(ComponentsProperty, value); }
		}
		
		public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(ComponentViewModel), typeof(ClientViewModel));
		public ComponentViewModel SelectedItem
		{
			get { return (ComponentViewModel)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}


		public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register("CloseCommand", typeof(ViewModelCommand), typeof(ClientViewModel));
		public ViewModelCommand CloseCommand
		{
			get { return (ViewModelCommand)GetValue(CloseCommandProperty); }
			set { SetValue(CloseCommandProperty, value); }
		}

		public event EventHandler Close;

		private int bufferLength;


		public ClientViewModel(int BufferLength)
		{
			this.bufferLength = BufferLength;
			Components = new ObservableCollection<ComponentViewModel>();
			CloseCommand = new ViewModelCommand((object t) => true ,OnClose) ;
		}

		protected virtual void OnClose(object sender)
		{
			if (Close != null) Close(this, EventArgs.Empty);
		}

		public void Add(Log Log)
		{
			ComponentViewModel componentViewModel;

			componentViewModel = Components.FirstOrDefault(item => item.Name == Log.ComponentName);
			if (componentViewModel==null)
			{
				componentViewModel = new ComponentViewModel(bufferLength);
				componentViewModel.Name = Log.ComponentName;
				componentViewModel.Close += ComponentViewModel_Close;
				Components.Add(componentViewModel);
				if (SelectedItem == null) SelectedItem = componentViewModel;
			}

			componentViewModel.Add(Log);
		}

		private void ComponentViewModel_Close(object sender, EventArgs e)
		{
			ComponentViewModel componentViewModel;

			componentViewModel = sender as ComponentViewModel;
			if (componentViewModel == null) return;

			componentViewModel.Close -= ComponentViewModel_Close;
			Components.Remove(componentViewModel);
			if (SelectedItem == componentViewModel) SelectedItem = Components.FirstOrDefault();
		}

	}
}
