using System;

namespace DataAccess.Models
{
    public class Car: IEntites
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public double Price { get; set; }
        public string StockNumber { get; set; }
        public string ImageSrc { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }
        public double MsrpPrice { get; set; }
        public bool IsUsed { get; set; }

        public int StockCarId { get; set; }
        public int DealerId { get; set; }

        public virtual StockCar StockCar { get; set; }
        public virtual Dealer Dealer { get; set; }
        public virtual MainAdvertCar MainAdvertCar { get; set; }        
    }
}
