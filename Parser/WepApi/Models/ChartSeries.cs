using System.Collections.Generic;

namespace WepApi.Models
{
    public class ChartSeries
    {
        public int carId { get; set; }
        public string name { get; set; }
        public List<double> data { get; set; }
    }
}