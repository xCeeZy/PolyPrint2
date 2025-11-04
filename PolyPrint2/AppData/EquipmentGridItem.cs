namespace PolyPrint2.AppData
{
    public class EquipmentGridItem
    {
        public int ID_Equipment { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Serial_Number { get; set; }
        public string ClientName { get; set; }
        public string Condition { get; set; }
        public int? ID_Client { get; set; }
    }
}
