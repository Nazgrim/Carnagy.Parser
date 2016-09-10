using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class CarConfiguration : EntityTypeConfiguration<Car>
    {
        public CarConfiguration()
        {
            HasKey(t => t.Id);
            HasRequired(t => t.Dealer)
                .WithMany(t => t.Cars)
                .HasForeignKey(d => d.DealerId);

            HasRequired(t => t.StockCar)
                .WithMany(t => t.Cars)
                .HasForeignKey(d => d.StockCarId);
        }
    }
}
