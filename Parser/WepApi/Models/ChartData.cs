using System.Collections.Generic;

namespace WepApi.Models
{
    public class ChartData
    {
        public List<double> seriesData { get; set; }
        public int min { get; set; }
        public int max { get; set; }
        public int msrpPrice { get; set; }
        public int avrPrice { get; set; }
        public int dealerPrice { get; set; }
    }
}
