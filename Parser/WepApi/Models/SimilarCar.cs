namespace WepApi.Models
{
    public class SimilarCar
    {
        public int id { get; set; }
        public string name { get; set; }
        public string imgUrl { get; set; }
        public string maker { get; set; }
        public string model { get; set; }
        public bool isEmpty { get; set; }
        public string drivetrain { get; set; }
        public int price { get; set; }
        public ChartSeries chartSeries { get; set; }
    }
}