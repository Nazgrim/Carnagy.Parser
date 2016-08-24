using System.Collections.Generic;

namespace WepApi.Models
{
    public class DealerClassInformation
    {
        public string imgScr { get; set; }
        public string name { get; set; }
        public Dictionary<string,string> parametrs { get; set; }
        public DelalerClassPrice averagePrice { get; set; }
        public DelalerClassPrice dealerPrice { get; set; }
    }
}