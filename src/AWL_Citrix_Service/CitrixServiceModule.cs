using Autofac;
using Autofac.Core;
using Autofac.Integration.SignalR;
//using AWL.Citrix.Service.Hubs;
using AWL.Citrix.Service.Models;
using AWL.Services.Core.Services;
using Citrix.Monitor.Repository.V2;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Reflection;

namespace AWL.Citrix.Service
{
    public class CitrixServiceModule : ServiceModule<CitrixService>
    {
        public string StoreFrontBaseAddress { get; set; }
        public string StoreName { get; set; }
        public string XenDesktopMonitorEndpoint { get; set; }
        public string CitrixDBConnectionString { get; set; }
        public int NoticationTickerInterval { get; set; }

        public CitrixServiceModule()
            : base("Citrix")
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            StoreFrontAuthenticate.StoreName = StoreName;
            
            //NotificationTicker.NoticationTickerInterval = NoticationTickerInterval; - NOT AVAILABLE FOR EXTERNAL USE

            builder.Register(db => new DatabaseContext(new Uri(XenDesktopMonitorEndpoint))
            {
                Credentials = System.Net.CredentialCache.DefaultCredentials
            })
                .AsSelf()
                .InstancePerRequest();

            if (string.IsNullOrEmpty(CitrixDBConnectionString))
            {
                builder.Register(db => new CitrixDBModel()).AsSelf();
            }
            else
            {
                builder.Register(db => new CitrixDBModel(CitrixDBConnectionString)).AsSelf();
            }

            // CODE NOT AVAILABLE EXTERNALLY
            //builder.RegisterType<NotificationTicker>()
            //    .WithParameter(ResolvedParameter.ForNamed<IHubConnectionContext<dynamic>>("clients"))
            //    .AsSelf()
            //    .SingleInstance();

            //builder.Register(c => GlobalHost.DependencyResolver.Resolve<IConnectionManager>().GetHubContext<NotificationHub>().Clients)
            //    .Named<IHubConnectionContext<dynamic>>("clients");
            //var settings = new JsonSerializerSettings();
            //settings.ContractResolver = new SignalRContractResolver();
            //var serializer = JsonSerializer.Create(settings);
            //builder.RegisterInstance(serializer).As<JsonSerializer>();

            // Register your SignalR hubs.
            //builder.RegisterHubs(Assembly.GetExecutingAssembly());

            base.Load(builder);
        }
    }
}
