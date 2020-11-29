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
		private IReceiverModule multicastReceiverModule;
		private IReceiverModule unicastReceiverModule;
		private FileLogger logger;

		public MainWindow()
		{
			logger = new FileLogger(new DefaultLogFormatter(), "LogReceiver.log");
			multicastReceiverModule = new MulicastReceiverModule(logger, IPAddress.Parse(global::LogReceiver.Properties.Settings.Default.MulticastIPaddress), global::LogReceiver.Properties.Settings.Default.MulticastPort);
			unicastReceiverModule = new UnicastReceiverModule(logger,  global::LogReceiver.Properties.Settings.Default.UnicastPort);

			InitializeComponent();

			applicationViewModel = new ApplicationViewModel(multicastReceiverModule,unicastReceiverModule, global::LogReceiver.Properties.Settings.Default.BufferLength);

			DataContext = applicationViewModel;

			multicastReceiverModule.Start();
			unicastReceiverModule.Start();
		}

		private void Window_Closing(object sender, EventArgs e)
		{
			multicastReceiverModule.Stop();
			unicastReceiverModule.Stop();
			/*if ((multicastReceiverModule.State!=ModuleLib.ModuleStates.Stopped) || (unicastReceiverModule.State != ModuleLib.ModuleStates.Stopped))
			{
				int t = 0;
			}*/
			logger.Dispose();
		}

		private void StartCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true;e.CanExecute = (multicastReceiverModule.State == ModuleLib.ModuleStates.Stopped) && (unicastReceiverModule.State == ModuleLib.ModuleStates.Stopped);
		}

		private void StartCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			multicastReceiverModule.Start();
			unicastReceiverModule.Start();
		}

		private void StopCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.Handled = true; e.CanExecute = (multicastReceiverModule.State == ModuleLib.ModuleStates.Started) && (unicastReceiverModule.State == ModuleLib.ModuleStates.Started);
		}

		private void StopCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			multicastReceiverModule.Stop(); ;
			unicastReceiverModule.Stop();
		}

	}
}
