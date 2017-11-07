using Autofac;
using Autofac.Integration.WebApi;
using AWL.Services.Core.Logging;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace AWL.Services.Core.Services
{
	public abstract class Service : IService
	{
        protected readonly ILifetimeScope scope;
        private readonly ILogger log;

        public IDisposable Server { get; set; }
        public ServiceStatus Status { get; set; }
        public string Name { get; set; }
        public StartOptions OwinStartOptions { get; set; }
        public HttpConfiguration HttpConfiguration { get; set; }

        public Service(ILifetimeScope scope, ILogger log)
        {
            this.scope = scope;
            this.log = log;
        }

        protected virtual void StartUp(IAppBuilder appBuilder)
        {
            HttpConfiguration.DependencyResolver = new AutofacWebApiDependencyResolver(scope);

            appBuilder.UseAutofacMiddleware(scope);
            appBuilder.UseAutofacWebApi(HttpConfiguration);
            appBuilder.UseWebApi(HttpConfiguration);
        }

        public virtual void Start()
        {
            log.Info("Starting service {0}", Name);
            log.Debug("Options: {0}", this.ToString());
            Status = ServiceStatus.StartPending;
            Server = WebApp.Start(OwinStartOptions, new Action<IAppBuilder>(StartUp));
            Status = ServiceStatus.Running;
        }

        public virtual void Stop()
        {
            log.Info("Stopping service {0}", Name);
            Status = ServiceStatus.StopPending;
            Server.Dispose();
            Status = ServiceStatus.Stopped;
        }

        public virtual void Restart()
        {
            log.Info("Restarting service {0}", Name);
            Stop();
            Start();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(
               new
               {
                   Name = Name,
                   Owin = OwinStartOptions
               });
        }
    }
}
