using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class AdvertCarPriceConfiguration :EntityTypeConfiguration<AdvertCarPrice>
    {
        public AdvertCarPriceConfiguration()
        {
            HasKey(t => t.Id);

            HasRequired(t => t.AdvertCar)
                .WithMany(t => t.AdvertCarPrices)
                .HasForeignKey(d => d.AdvertCarId);
        }
    }
}
