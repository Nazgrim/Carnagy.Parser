using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class StockCarPriceConfiguration : EntityTypeConfiguration<StockCarPrice>
    {
        public StockCarPriceConfiguration()
        {
            HasKey(t => t.Id);

            HasRequired(t => t.StockCar)
                .WithMany(t => t.StockCarPrices)
                .HasForeignKey(d => d.StockCarId);
        }
    }
}
