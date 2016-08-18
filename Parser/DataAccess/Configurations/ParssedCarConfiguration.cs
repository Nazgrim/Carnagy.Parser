using DataAccess.Models;
using System.Data.Entity.ModelConfiguration;

namespace DataAccess.Configurations
{
    public class ParssedCarConfiguration: EntityTypeConfiguration<ParssedCar>
    {
        public ParssedCarConfiguration()
        {
            HasKey(t => t.Id);
            HasRequired(t => t.MainConfiguration)
                .WithMany(t => t.ParssedCars)
                .HasForeignKey(d => d.MainConfigurationId)
                .WillCascadeOnDelete(false);
        }
    }
}
