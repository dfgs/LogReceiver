using LogLib;
using LogReceiver.Modules;
using LogReceiver.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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

			InitializeComponent();

			logger = new FileLogger(new DefaultLogFormatter(),"LogReceiver.log");
			receiverModule = new MulicastReceiverModule(logger,IPAddress.Parse(global::LogReceiver.Properties.Settings.Default.MulticastIPaddress), global::LogReceiver.Properties.Settings.Default.Port);

			applicationViewModel = new ApplicationViewModel(receiverModule);

			DataContext = applicationViewModel;
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			receiverModule.Dispose();
			logger.Dispose();
		}


	}
}
