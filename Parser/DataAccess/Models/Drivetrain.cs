using System.Collections.Generic;

namespace DataAccess.Models
{
    public class Drivetrain : IDictionaryEntity
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public virtual ICollection<StockCar> StockCars { get; set; }
    }
}
