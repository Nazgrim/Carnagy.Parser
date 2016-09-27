using System.Collections.Generic;

namespace DataAccess.Models
{
    public class Dealer
    {
        public Dealer()
        {
            Cars = new List<Car>();
            Configurations = new List<MainConfiguration>();
        }
        public int Id { get; set; }
        public string Location { get; set; }
        public string WebSireUrl { get; set; }
        public string WebSiteName { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public bool IsCreated { get; set; }

        public virtual ICollection<MainConfiguration> Configurations { get; set; }
        public virtual ICollection<Car> Cars { get; set; }
    }
}
