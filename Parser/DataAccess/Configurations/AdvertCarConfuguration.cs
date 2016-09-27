using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class AdvertCarConfuguration : EntityTypeConfiguration<AdvertCar>
    {
        public AdvertCarConfuguration()
        {
            HasKey(t => t.Id);
           
            HasRequired(t => t.Car)
                .WithMany(t => t.AdvertCars)
                .HasForeignKey(d => d.CarId);

            HasOptional(t => t.ParsedCar)
                .WithMany(t => t.AdvertCars)
                .HasForeignKey(d => d.ParsedCarId);
        }
    }
}
