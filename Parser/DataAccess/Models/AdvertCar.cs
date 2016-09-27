using System.Collections;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public class AdvertCar
    {
        public AdvertCar()
        {
            AdvertCarPrices = new List<AdvertCarPrice>();
        }

        public int Id { get; set; }
        public bool IsDealer { get; set; }
        public string Url { get; set; }

        public int CarId { get; set; }
        public int? ParsedCarId { get; set; }

        public virtual Car Car { get; set; }
        public virtual ParsedCar ParsedCar { get; set; }

        public virtual ICollection<AdvertCarPrice> AdvertCarPrices { get; set; }
    }
}
