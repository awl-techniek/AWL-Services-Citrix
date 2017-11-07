using Autofac;
using Autofac.Configuration;
using AWL.Services.Core.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AWL.Services.Console
{
    public class ProcessManager
    {
        //privates
        private Thread workerThread;
        private IContainer container;
        private bool mustStop;

        //Public Properties
        public List<string> Services { get; set; }

        //Init Cleanup Routines
        public ProcessManager()
        {
            Services = new List<string>();
        }
        public int Init()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true);

            ContainerBuilder builder = new ContainerBuilder();
#if (IsBeta)
            config.AddJsonFile("appsettings.Development.json", optional: true);
#else
            config.AddJsonFile("appsettings.Production.json", optional: true);
#endif
            var module = new ConfigurationModule(config.Build());
            builder.RegisterModule(module);
            container = builder.Build();
            return 0;
        }
        public int StartManager()
        {
            workerThread = new Thread(WorkerRoutine);
            workerThread.Start();

            return 0;
        }
        public int StopManager()
        {
            mustStop = true;
            return 0;
        }

        //Work Handling
        private void WorkerRoutine()
        {
            using (var scope = container.BeginLifetimeScope())
            {
                List<IService> services = scope.Resolve<IEnumerable<IService>>().ToList();

                services.ForEach(s => s.Start());

                while (!mustStop)
                {
                    Thread.Sleep(10);
                }

                services.ForEach(s => s.Stop());
            }
        }
    }
}
