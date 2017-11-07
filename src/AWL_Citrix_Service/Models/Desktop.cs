namespace AWL.Citrix.Service.Models
{
    public class Desktop
    {
        public int ID { get; set; }
        public bool UnderMaintenance { get; set; }
        public int SessionSupport { get; set; }
        //public int MachinesCapacity { get; set; }
        //public int MachinesInUse { get; set; }
        public int MachinesAvailable { get; set; }
        public bool HasSession { get; set; }
        public string SessionOpen { get; set; }
        public bool IsDisconnected { get; set; }
    }
}
