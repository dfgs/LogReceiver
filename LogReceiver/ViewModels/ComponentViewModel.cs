using LogLib;
using LogReceiver.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModelLib;

namespace LogReceiver.ViewModels
{
	public class ComponentViewModel : DependencyObject, ITailProvider,ILogProvider
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
		public bool Tail
		{
			get { return (bool)GetValue(TailProperty); }
			set { SetValue(TailProperty, value); }
		}
		
		public static readonly DependencyProperty IsPausedProperty = DependencyProperty.Register("IsPaused", typeof(bool), typeof(ComponentViewModel), new FrameworkPropertyMetadata(false));
		public bool IsPaused
		{
			get { return (bool)GetValue(IsPausedProperty); }
			set { SetValue(IsPausedProperty, value); }
		}


		public event TailEventHandler UpdateScroll;
		public event LogEventHandler LogAdded;
		public event LogEventHandler LogRemoved;

		public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register("CloseCommand", typeof(ViewModelCommand), typeof(ComponentViewModel));
		public ViewModelCommand CloseCommand
		{
			get { return (ViewModelCommand)GetValue(CloseCommandProperty); }
			set { SetValue(CloseCommandProperty, value); }
		}

		public event EventHandler Close;

		private int bufferLength;

		public ComponentViewModel(int BufferLength)
		{
			this.bufferLength = BufferLength;
			Items = new ObservableCollection<Log>();
			CloseCommand = new ViewModelCommand((object sender)=>true,OnClose);
		}

		protected virtual void OnClose(object sender)
		{
			if (Close != null) Close(this, EventArgs.Empty);
		}

		public void Add(Log Log)
		{
			if (IsPaused) return;
			Items.Add(Log);
			if (LogAdded != null) LogAdded(this, new LogEventArgs(Log));
			if (Items.Count > bufferLength)
			{
				Items.RemoveAt(0);
				if (LogRemoved!= null) LogRemoved(this, new LogEventArgs(Log));
			}
			if (Tail && (UpdateScroll != null)) UpdateScroll(this, new TailEventArgs(Log));
		}
	}
}
