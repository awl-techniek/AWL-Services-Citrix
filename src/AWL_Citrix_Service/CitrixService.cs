using Autofac;
using Autofac.Integration.SignalR;
using AWL.Services.Core.Logging;
using Microsoft.AspNet.SignalR;
using Owin;

namespace AWL.Citrix.Service
{
    public class CitrixService : Services.Core.Services.Service
    {
        public CitrixService(ILifetimeScope scope, ILogger log)
            : base(scope, log)
        {
        }

        protected override void StartUp(IAppBuilder appBuilder)
        {
            appBuilder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            base.StartUp(appBuilder);
            var config = new HubConfiguration()
            {
                EnableDetailedErrors = true,
                Resolver = new AutofacDependencyResolver(scope)
            };
            GlobalHost.DependencyResolver = config.Resolver;

            appBuilder.UseAutofacMiddleware(scope);
            appBuilder.MapSignalR(config);
        }
    }
}
