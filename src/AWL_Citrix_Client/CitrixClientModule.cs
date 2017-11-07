using Autofac;
using AWL.Clients.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWL.Settings.Client
{
    public class CitrixClientModule : Module
    {
        private ClientOptions options;

        public CitrixClientModule(ClientOptions options)
        {
            this.options = options;
        }
    }
}
