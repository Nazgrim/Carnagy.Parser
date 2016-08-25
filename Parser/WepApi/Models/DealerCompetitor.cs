using System.Collections.Generic;

namespace WepApi.Models
{
    public class DealerCompetitor
    {
        public string name { get; set; }
        public string url { get; set; }
        public string city { get; set; }
        public string webSiteName { get; set; }
        public List<DealerCompetitorCar> cars { get; set; }
    }
}