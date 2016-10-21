using System.Collections.Generic;

namespace WepApi.Models
{
    public class ChartData
    {
        public IEnumerable<SeriesDataValue> seriesData { get; set; }
        public double min { get; set; }
        public double max { get; set; }
        public double msrpPrice { get; set; }
        public double avrPrice { get; set; }
        public double dealerPrice { get; set; }
    }

    public class SeriesDataValue
    {
        public SeriesDataValue()
        {
            dealersId = new List<int>();
        }
        public List<int> dealersId { get; set; }
        public int value { get; set; }
    }
}
