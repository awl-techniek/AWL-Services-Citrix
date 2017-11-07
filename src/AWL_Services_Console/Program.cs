using AWL.Modules.NLog;
using AWL.Services.Core.Logging;
using Mono.Options;
using NLog.Config;
using NLog.Targets;
using System.Collections.Generic;
using System.ServiceProcess;
using N = NLog;

namespace AWL.Services.Console
{
    public class Program
	{
		private static LogLevel verbosity;
		private static ProcessManager processMan;

		public static void Main(string[] args)
		{
			
			if (System.Environment.UserInteractive)
			{
				setUpNLog(true, true);
				RunAsConsole(args);
			}
			else
			{
				setUpNLog(false, true);
				RunAsService();
			}
			
		}

		//Private Routines
		private static void RunAsConsole(string[] args)
		{

			processMan = new ProcessManager();
			bool show_help = false;
			verbosity = LogLevel.Debug;

			var options = new OptionSet
			{
				{ "n|name=", "the {NAME} of the services to start.", v => processMan.Services.Add (v) },
				{ "v", "increase debug message verbosity", v => { if (v != null) verbosity = LogLevel.Debug; } },
				{ "h|help",  "show this message and exit", v => show_help = v != null },
			};

			List<string> extra;
			try
			{
				extra = options.Parse(args);
			}
			catch (OptionException e)
			{
				System.Console.Write("services: ");
				System.Console.WriteLine(e.Message);
				System.Console.WriteLine("Try 'services --help' for more information.");
				return;
			}

			if (show_help)
			{
				showHelp(options);
				return;
			}
						
			processMan.Init();
			processMan.StartManager();
			System.Console.CancelKeyPress += Console_CancelKeyPress;
			while (true)
			{
				System.Threading.Thread.Sleep(10);
			}
		}
		private static void Console_CancelKeyPress(object sender, System.ConsoleCancelEventArgs e)
		{
			processMan.StopManager();
		}

		private static void RunAsService()
		{
			ServiceBase[] ServicesToRun;

			try
			{
				ServicesToRun = new ServiceBase[] { new MainService() };
				ServiceBase.Run(ServicesToRun);
			}
			catch (System.Exception ex)
			{
				throw ex;
			}
		}
		
		//Helper Routines
		static void showHelp(OptionSet options)
		{
			System.Console.WriteLine("Usage: services [OPTIONS]+");
			System.Console.WriteLine();
			System.Console.WriteLine("Options:");
			options.WriteOptionDescriptions(System.Console.Out);
		}
		static void setUpNLog(bool EnableConsoleLog, bool EnableServiceLog)
		{
			var config = new LoggingConfiguration();
			if (EnableConsoleLog)
			{ 			
				var consoleTarget = new ColoredConsoleTarget();
				config.AddTarget("console", consoleTarget);
				consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";

				var rule = new LoggingRule("*", LoggerAdapter.ToNLogLevel(verbosity), consoleTarget);
				config.LoggingRules.Add(rule);
			}
			if (EnableServiceLog)
			{
				var eventTarget = new EventLogTarget();
				config.AddTarget("eventlog", eventTarget);
				eventTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";

				var rule = new LoggingRule("*", LoggerAdapter.ToNLogLevel(verbosity), eventTarget);
				config.LoggingRules.Add(rule);
			}

			N.LogManager.Configuration = config;
		}
	}
}
