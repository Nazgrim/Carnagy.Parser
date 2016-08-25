using System.Collections.Generic;

namespace WepApi.Models
{
    public class PriceTrend
    {
        public List<ChartSeries> chartSeries { get; set; }
        public List<string> xAxisCategories { get; set; }
    }
}