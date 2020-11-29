﻿using ModuleLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogReceiver.Modules
{
	public interface IMulticastReceiverModule:IThreadModule,IDisposable
	{
		event LogReceivedEventHandler LogReceived;
	}
}
