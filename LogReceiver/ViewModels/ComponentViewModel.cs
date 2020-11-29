using LogLib;
using LogReceiver.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LogReceiver.ViewModels
{
	public class ComponentViewModel : DependencyObject, ITailProvider
	{
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string),typeof(ComponentViewModel));
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<Log>), typeof(ComponentViewModel));
		public ObservableCollection<Log> Items
		{
			get { return (ObservableCollection<Log>)GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}

		public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(Log), typeof(ComponentViewModel));
		public Log SelectedItem
		{
			get { return (Log)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		public static readonly DependencyProperty TailProperty = DependencyProperty.Register("Tail", typeof(bool), typeof(ComponentViewModel),new FrameworkPropertyMetadata(true));

		public event TailEventHandler UpdateScroll;

		public bool Tail
		{
			get { return (bool)GetValue(TailProperty); }
			set { SetValue(TailProperty, value); }
		}

		public ComponentViewModel()
		{
			Items = new ObservableCollection<Log>();
		}

		public void Add(Log Log)
		{
			Items.Add(Log);
			if (Tail && (UpdateScroll != null)) UpdateScroll(this, new TailEventArgs(Log));
		}
	}
}
