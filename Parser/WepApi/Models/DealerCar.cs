using System.Collections.Generic;

namespace WepApi.Models
{
    public class DealerCar
    {
        public Information information { get; set; }
        public ChartSeries chartSeries { get; set; }
        public List<SimilarCar> similarCars { get; set; }
    }
}