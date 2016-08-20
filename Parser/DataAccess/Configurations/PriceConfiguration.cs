using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class PriceConfiguration : EntityTypeConfiguration<Price>
    {
        public PriceConfiguration()
        {
            HasKey(t => t.Id);
            HasRequired(t => t.ParssedCar)
                .WithMany(t => t.Prices)
                .HasForeignKey(t => t.ParssedCarId);
        }
    }
}
