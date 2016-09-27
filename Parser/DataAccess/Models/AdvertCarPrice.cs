using System;

namespace DataAccess.Models
{
    public class AdvertCarPrice
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public DateTime DateTime { get; set; }

        public int AdvertCarId { get; set; }

        public virtual AdvertCar AdvertCar { get; set; }
    }
}
