using System.Data.Entity;

namespace AWL.Citrix.Service.Models
{
    public partial class CitrixDBModel : DbContext
    {
        public virtual DbSet<DesktopGroup> DesktopGroups { get; set; }

        public CitrixDBModel() : base()
        {
        }

        public CitrixDBModel(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }
    }
}
