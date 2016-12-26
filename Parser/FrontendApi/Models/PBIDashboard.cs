namespace FrontendApi.Models
{
    public class PBIDashboard
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string embedUrl { get; set; }
        public bool isReadOnly { get; set; }
    }
}