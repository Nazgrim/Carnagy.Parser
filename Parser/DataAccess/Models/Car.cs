using System.Collections.Generic;

namespace DataAccess.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int Price { get; set; }

        public int StockCarId { get; set; }
        public int DealerId { get; set; }

        public virtual StockCar StockCar { get; set; }
        public virtual Dealer Dealer { get; set; }

        public virtual ICollection<AdvertCar> AdvertCars { get; set; }
    }
}
