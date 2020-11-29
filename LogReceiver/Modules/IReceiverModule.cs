using ModuleLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReceiver.Modules
{
	public interface IReceiverModule:IThreadModule,IDisposable
	{
		event LogReceivedEventHandler LogReceived;
	}
}
