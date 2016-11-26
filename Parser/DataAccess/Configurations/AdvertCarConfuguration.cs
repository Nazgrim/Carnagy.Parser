using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class AdvertCarConfuguration : EntityTypeConfiguration<AdvertCar>
    {
        public AdvertCarConfuguration()
        {
            HasKey(t => t.Id);

            HasRequired(t => t.MainAdvertCar)
                .WithMany(t => t.AdvertCars)
                .HasForeignKey(d => d.MainAdvertCarId);

            HasOptional(t => t.ParsedCar)
                .WithMany(t => t.AdvertCars)
                .HasForeignKey(d => d.ParsedCarId);
        }
    }
}
