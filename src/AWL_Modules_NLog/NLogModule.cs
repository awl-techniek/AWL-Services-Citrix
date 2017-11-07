using Autofac;
using Autofac.Core;
using AWL.Services.Core.Logging;
using N = NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AWL.Modules.NLog
{
	public class NLogModule : LogModule
	{
		protected override ILogger CreateLoggerFor(Type type)
		{
			return new LoggerAdapter(N.LogManager.GetLogger(type.FullName));
		}
	}
}
