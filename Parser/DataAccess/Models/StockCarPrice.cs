using System;

namespace DataAccess.Models
{
    public class StockCarPrice
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int Value { get; set; }

        public int StockCarId { get; set; }

        public virtual StockCar StockCar { get; set; }
    }
}
