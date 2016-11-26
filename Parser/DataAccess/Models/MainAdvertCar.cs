using System.Collections.Generic;

namespace DataAccess.Models
{
    public class MainAdvertCar
    {
        public MainAdvertCar()
        {
            AdvertCars = new List<AdvertCar>();
        }
        public int CarId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Car Car { get; set; }

        public virtual ICollection<AdvertCar> AdvertCars { get; set; }
    }
}
