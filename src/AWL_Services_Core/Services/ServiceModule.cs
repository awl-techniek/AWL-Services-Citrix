using Autofac;
using AWL.Services.Core.Logging;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac.Integration.WebApi;

namespace AWL.Services.Core.Services
{
	public abstract class ServiceModule<TService> : Module
		where TService : Service
	{
        public string Name { get; set; }
        public int Port { get; set; }

        protected StartOptions OwinStartOptions { get; set; }
        protected HttpConfiguration HttpConfiguration { get; set; }

        public ServiceModule(string name) : this(name, 2000)
        {
        }

        public ServiceModule(string name, int port)
        {
            Port = port;
            Name = name;
        }

        protected override void Load(ContainerBuilder builder)
        {
            OwinStartOptions = new StartOptions()
            {
                Port = Port
            };
            OwinStartOptions.Urls.Add(string.Format("http://*:{0}", Port));
            HttpConfiguration = new HttpConfiguration();
            HttpConfiguration.MapHttpAttributeRoutes();

            builder.RegisterType<TService>()
                    .WithProperty("Name", Name)
                    .WithProperty("OwinStartOptions", OwinStartOptions)
                    .WithProperty("HttpConfiguration", HttpConfiguration)
                    .As<TService, IService>()
                    .SingleInstance();

            builder.RegisterApiControllers(typeof(TService).Assembly);
        }
    }
}
