namespace WepApi.Models
{
    public class DealerCompetitorCar
    {
        public int dealerId { get; set; }
        public string year { get; set; }
        public string make { get; set; }
        public string model { get; set; }
        public string bodyType { get; set; }
        public string styleTrim { get; set; }
        public string drivetrain { get; set; }
        public string url { get; set; }
        public string dealerName { get; set; }
        public string dealerLocation { get; set; }
        public CarPrice price { get; set; }
    }
}