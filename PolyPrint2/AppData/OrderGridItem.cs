using System;

namespace PolyPrint2.AppData
{
    public class OrderGridItem
    {
        public int ID_Order { get; set; }
        public string ClientName { get; set; }
        public DateTime Order_Date { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
    }
}
