using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AWL.Services.Console
{
	partial class MainService : ServiceBase
	{
		private ProcessManager processMan;

		public MainService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			processMan = new ProcessManager();
			processMan.Init();
			processMan.StartManager();
		}

		protected override void OnStop()
		{
			processMan.StopManager();
		}

	}
}
