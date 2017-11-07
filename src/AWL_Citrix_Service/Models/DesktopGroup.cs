using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace AWL.Citrix.Service.Models
{
    [Table("chb_Config.DesktopGroups")]
    public partial class DesktopGroup
    {
        [Key]
        public int Uid { get; set; }

        public int WISequenceNumber { get; set; }

        [Required]
        [StringLength(64)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string PublishedName { get; set; }

        public Guid UUID { get; set; }

        public int DesktopKind { get; set; }

        public int DeliveryType { get; set; }

        public int SessionSupport { get; set; }

        public bool IsRemotePC { get; set; }

        public bool TurnOnAddedMachine { get; set; }

        public int MinimumFunctionalLevel { get; set; }

        public bool Enabled { get; set; }

        public bool InMaintenanceMode { get; set; }

        public bool AutomaticPowerOnForAssigned { get; set; }

        public bool AutomaticPowerOnForAssignedDuringPeak { get; set; }

        [Required]
        [StringLength(255)]
        public string ProtocolPriority { get; set; }

        public bool ShutdownDesktopsAfterUse { get; set; }

        public int DeferShutdownAfterSessionEndSecs { get; set; }

        public int IdleTimeBeforeUsageSecs { get; set; }

        public int DefaultIconUid { get; set; }

        public int DefaultColorDepth { get; set; }

        public bool SecureIcaRequired { get; set; }

        [StringLength(512)]
        public string DefaultDescription { get; set; }

        [StringLength(255)]
        public string TimeZoneId { get; set; }

        public string ScopeList { get; set; }
    }
}
