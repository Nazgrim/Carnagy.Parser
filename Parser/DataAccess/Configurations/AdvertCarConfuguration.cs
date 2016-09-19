using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class AdvertCarConfuguration : EntityTypeConfiguration<AdvertCar>
    {
        public AdvertCarConfuguration()
        {
            HasKey(t => t.ParsedCarId);

            

            HasRequired(t => t.Car)
                .WithMany(t => t.AdvertCars)
                .HasForeignKey(d => d.CarId);
        }
    }
}
