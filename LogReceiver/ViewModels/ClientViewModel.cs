using LogLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
		public ClientViewModel()
		{
			Components = new ObservableCollection<ComponentViewModel>();
		}

		public void Add(Log Log)
		{
			ComponentViewModel component;

			component = Components.FirstOrDefault(item => item.Name == Log.ComponentName);
			if (component==null)
			{
				component = new ComponentViewModel();
				component.Name = Log.ComponentName;
				Components.Add(component);
				if (SelectedItem == null) SelectedItem = component;
			}

			component.Add(Log);
		}



	}
}
