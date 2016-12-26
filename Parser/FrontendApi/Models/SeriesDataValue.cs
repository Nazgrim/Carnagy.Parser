using System.Collections.Generic;

namespace FrontendApi.Models
{
    public class SeriesDataValue
    {
        public SeriesDataValue()
        {
            carsId = new List<int>();
        }
        public List<int> carsId { get; set; }
        public int value { get; set; }
    }
}