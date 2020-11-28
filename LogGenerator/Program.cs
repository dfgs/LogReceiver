using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LogLib;

namespace LogGenerator
{
	class Program
	{
		static void Main(string[] args)
		{
			MulticastLogger logger;

			logger = new MulticastLogger(new DefaultLogFormatter(), IPAddress.Parse(global::LogGenerator.Properties.Settings.Default.MulticastIPaddress), global::LogGenerator.Properties.Settings.Default.Port);

			while(true)
			{
				logger.Log(1, "toto", "Method2", LogLevels.Debug, "test");
				Thread.Sleep(global::LogGenerator.Properties.Settings.Default.TriggerDelay);
			}

		}
	}
}
