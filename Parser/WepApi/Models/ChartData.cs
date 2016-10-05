using System.Collections.Generic;

namespace WepApi.Models
{
    public class ChartData
    {
        public IEnumerable<int> seriesData { get; set; }
        public double min { get; set; }
        public double max { get; set; }
        public int msrpPrice { get; set; }
        public double avrPrice { get; set; }
        public int dealerPrice { get; set; }
    }
}
