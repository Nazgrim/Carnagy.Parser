using System.Collections.Generic;

namespace DataAccess.Models
{
    public class StockCar
    {
        public StockCar()
        {
            Cars = new List<Car>();
            StockCarPrices= new List<StockCarPrice>();
        }

        public int Id { get; set; }
        public string ImageScr { get; set; }

        public int MakeId { get; set; }
        public int ModelId { get; set; }
        public int YearId { get; set; }
        public int BodyTypeId { get; set; }
        public int StyleTrimId { get; set; }
        public int DrivetrainId { get; set; }
        public double Price { get; set; }

        public virtual Year Year { get; set; }
        public virtual Model Model { get; set; }
        public virtual Make Make { get; set; }
        public virtual BodyType BodyType { get; set; }
        public virtual StyleTrim StyleTrim { get; set; }
        public virtual Drivetrain Drivetrain { get; set; }

        public virtual ICollection<Car> Cars { get; set; }
        public virtual ICollection<StockCarPrice> StockCarPrices { get; set; }
    }
}