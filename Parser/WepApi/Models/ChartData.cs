using System.Collections.Generic;

namespace WepApi.Models
{
    public class ChartData
    {
        public IEnumerable<int> seriesData { get; set; }
        public double min { get; set; }
        public double max { get; set; }
        public double msrpPrice { get; set; }
        public double avrPrice { get; set; }
        public double dealerPrice { get; set; }
    }
}
