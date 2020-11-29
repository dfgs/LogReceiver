using LogLib;
using LogReceiver.Modules;
using LogReceiver.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogReceiver
{
	/// <summary>
	/// Logique d'interaction pour MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ApplicationViewModel applicationViewModel;
		private IMulticastReceiverModule receiverModule;
		private FileLogger logger;

		public MainWindow()
		{
			logger = new FileLogger(new DefaultLogFormatter(), "LogReceiver.log");
			receiverModule = new MulicastReceiverModule(logger, IPAddress.Parse(global::LogReceiver.Properties.Settings.Default.MulticastIPaddress), global::LogReceiver.Properties.Settings.Default.Port);

			InitializeComponent();

			applicationViewModel = new ApplicationViewModel(receiverModule);

			DataContext = applicationViewModel;

			receiverModule.Start();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			receiverModule.Stop();
			logger.Dispose();
		}

		private void StartCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;e.CanExecute = receiverModule.State == ModuleLib.ModuleStates.Stopped;
		}

		private void StartCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			receiverModule.Start();
		}

		private void StopCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = receiverModule.State == ModuleLib.ModuleStates.Started;
		}

		private void StopCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			receiverModule.Stop(); ;
		}

	}
}
