using System.Collections.Generic;

namespace FrontendApi.Models
{
    public class ChartSeries
    {
        public int carId { get; set; }
        public string name { get; set; }
        public List<long[]> data { get; set; }
    }
}