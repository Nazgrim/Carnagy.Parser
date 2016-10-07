namespace AddisongmParseAndAnalyze
{
    public class Rootobject
    {
        public Speciallabel speciallabel { get; set; }
        public Disclaimer disclaimer { get; set; }
        public object soldlabel { get; set; }
        public Upgrade[] Upgrades { get; set; }
        public Vehicle[] vehicles { get; set; }
    }

    public class Speciallabel
    {
        public object _new { get; set; }
        public object used { get; set; }
    }

    public class Disclaimer
    {
        public object _new { get; set; }
        public object used { get; set; }
    }

    public class Upgrade
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Vehicle
    {
        public int id { get; set; }
        public string condition { get; set; }
        public string stocknumber { get; set; }
        public string vin { get; set; }
        public string year { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string trim { get; set; }
        public string trimdescription { get; set; }
        public string description { get; set; }
        public string cab { get; set; }
        public string transmission { get; set; }
        public string engine { get; set; }
        public float enginedisplacement { get; set; }
        public string drivetrain { get; set; }
        public string type { get; set; }
        public string subtype { get; set; }
        public string exteriorcolor { get; set; }
        public float mileage { get; set; }
        public int age { get; set; }
        public string picture { get; set; }
        public bool special { get; set; }
        public bool salepending { get; set; }
        public bool onhome { get; set; }
        public bool certified { get; set; }
        public bool demo { get; set; }
        public bool asis { get; set; }
        public bool incoming { get; set; }
        public bool reserved { get; set; }
        public object media { get; set; }
        public float cost { get; set; }
        public float msrp { get; set; }
        public float saleprice { get; set; }
        public float specialprice { get; set; }
        public float price { get; set; }
        public string bodystyle { get; set; }
        public object carprooflink { get; set; }
        public string searchables { get; set; }
        public string location { get; set; }
        public string title { get; set; }
        public int timestamp { get; set; }
        public object[] upgrades { get; set; }
        public bool locked { get; set; }
        public object tags { get; set; }
    }
}
