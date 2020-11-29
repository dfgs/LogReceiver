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
			List<GeneratorModule> modules;
			GeneratorModule module;
			ILogger logger;
			
			modules = new List<GeneratorModule>();
			for(int clientId=0;clientId< Properties.Settings.Default.ClientCount;clientId++)
			{
				//logger=new MulticastLogger(IPAddress.Parse(Properties.Settings.Default.MulticastIPaddress), Properties.Settings.Default.MulticastPort);
				logger=new UnicastLogger(IPAddress.Parse("127.0.0.1"), Properties.Settings.Default.UnicastPort);
				for (int componentID=0;componentID<Properties.Settings.Default.ComponentCount;componentID++)
				{
					for(int methodID=0;methodID<Properties.Settings.Default.MethodCount;methodID++)
					{
						module = new GeneratorModule(logger, Properties.Settings.Default.TriggerDelay,clientId,componentID,methodID);
						module.Start();
						modules.Add(module);
					}
				}
			}

			

			Console.ReadLine();

			foreach (GeneratorModule m in modules)
			{
				m.Stop();
			}

			
		}
	}
}
