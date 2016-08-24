using System.Collections.Generic;

namespace WepApi.Models
{
    public class ChartData
    {
        public List<ChartSeriesData> seriesData { get; set; }
        public List<string> xAxisCategories { get; set; }
        public List<XAxisPlotBand> xAxisPlotBands { get; set; }
        public List<XAxisPlotLine> xAxisPlotLines { get; set; }
    }
}