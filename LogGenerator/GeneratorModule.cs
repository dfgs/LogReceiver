using LogLib;
using ModuleLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogGenerator
{
	public class GeneratorModule : ThreadModule
	{
		private int clientID;
		private int componentID;
		private int methodID;
		private int delay;
		private ILogger logger;

		public GeneratorModule(ILogger Logger, int Delay,int ClientID,int ComponentID,int MethodID):base(NullLogger.Instance)
		{
			this.logger = Logger;
			this.delay = Delay;
			this.clientID = ClientID;
			this.componentID = ComponentID;
			this.methodID = MethodID;
		}
		protected override void ThreadLoop()
		{
			LogLevels level;
			Random r;
			int value;
			int index=0;

			r = new Random();

			Thread.Sleep(new Random().Next(0, 10000));
			while(State==ModuleStates.Started)
			{
				value = r.Next(100);
				if (value > 90) level = LogLevels.Fatal;
				else if (value > 70) level = LogLevels.Error;
				else if (value > 50) level = LogLevels.Warning;
				else if (value > 25) level = LogLevels.Information;
				else level = LogLevels.Debug;

				logger.Log(componentID, $"Component{componentID}", $"Method{methodID}",level,$"Message {index} from client {clientID}") ;
				index++;
				WaitHandles(delay, QuitEvent);
			}


		}
	}
}
