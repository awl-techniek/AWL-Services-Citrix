using AWL.Citrix.Service.Models;
using Citrix.Monitor.Repository.V2;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Http;

namespace AWL.Citrix.Service.Controllers
{
    public class DashboardController : ApiController
    {
        private readonly DatabaseContext odb;
        private readonly CitrixDBModel cdb;

        public DashboardController(DatabaseContext odb, CitrixDBModel cdb)
        {
            this.odb = odb;
            this.cdb = cdb;
        }

        [Route("dashboard")]
        [StoreFrontAuthenticate]
        public IHttpActionResult Get()
        {
            var user = odb.Users.Where(u => u.FullName.Equals(User.Identity.Name)).FirstOrDefault();
            var openSessions = odb.Sessions.Where(s => s.ConnectionState == 5 || s.ConnectionState == 2).ToList();

            var machines = odb.Machines.Where(m => m.LifecycleState != 3).ToList();
            var cDesktopGroups = cdb.DesktopGroups.Where(dg => dg.Enabled).ToList();
            var oDesktopGroups = odb.DesktopGroups.ToList();

            var dashboard = new Dashboard();
            var desktops = new List<Desktop>();

            foreach (var cdg in cDesktopGroups)
            {
                var odg = oDesktopGroups.Where(dg => dg.Id == cdg.UUID).FirstOrDefault();
                if (odg == null) continue;

                var desktopMachineIds = machines.Where(m => m.DesktopGroupId == odg.Id).Select(m => m.Id);
                var machineSessions = openSessions.Where(s => desktopMachineIds.Contains(s.MachineId.Value));

                var desktop = new Desktop()
                {
                    ID = cdg.Uid,
                    UnderMaintenance = cdg.InMaintenanceMode,
                    SessionSupport = odg.SessionSupport,
                };

                var activeSession = machineSessions
                    .Where(s => s.ConnectionState == 5 && s.UserId.Value == user.Id)
                    .FirstOrDefault();

                var disconnectedSession = machineSessions
                    .Where(s => s.ConnectionState == 2 && s.UserId.Value == user.Id)
                    .FirstOrDefault();

                if (activeSession != null || disconnectedSession != null)
                {
                    desktop.HasSession = true;
                    desktop.IsDisconnected = disconnectedSession != null;

                    var session = activeSession ?? disconnectedSession;
                    if (session.StartDate.HasValue)
                    {
                        desktop.SessionOpen = (DateTime.UtcNow - session.StartDate).Value.Humanize();
                    }
                }

                if (odg.SessionSupport == 1)
                {
                    //desktop.MachinesCapacity = desktopMachineIds.Count();
                    //desktop.MachinesInUse = machineSessions.Count();
                    desktop.MachinesAvailable = desktopMachineIds.Count() - machineSessions.Count();
                }

                desktops.Add(desktop);
            }

            dashboard.Desktops = desktops;

            return Ok(dashboard);
        }

        private TEntity GetFromCache<TEntity>(string key, Func<TEntity> valueFactory) where TEntity : class
        {
            return GetFromCache(key, valueFactory, DateTimeOffset.Now.AddHours(1));
        }

        private TEntity GetFromCache<TEntity>(string key, Func<TEntity> valueFactory, DateTimeOffset absoluteExpiration) where TEntity : class
        {
            ObjectCache cache = MemoryCache.Default;
            // the lazy class provides lazy initializtion which will eavaluate the valueFactory expression only if the item does not exist in cache
            var newValue = new Lazy<TEntity>(valueFactory);
            CacheItemPolicy policy = new CacheItemPolicy { AbsoluteExpiration = absoluteExpiration };
            //The line below returns existing item or adds the new value if it doesn't exist
            var value = cache.AddOrGetExisting(key, newValue, policy) as Lazy<TEntity>;
            return (value ?? newValue).Value; // Lazy<T> handles the locking itself
        }
    }
}
