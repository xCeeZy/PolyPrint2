using System;

namespace PolyPrint2.AppData
{
    public class ServiceRequestGridItem
    {
        public int ID_Request { get; set; }
        public string ClientName { get; set; }
        public string EquipmentName { get; set; }
        public DateTime Created_Date { get; set; }
        public string Problem_Description { get; set; }
        public string Status { get; set; }
        public string MasterName { get; set; }
    }
}
