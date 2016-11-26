using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class MainAdvertCarConfiguration : EntityTypeConfiguration<MainAdvertCar>
    {
        public MainAdvertCarConfiguration()
        {
            HasKey(t => t.CarId);
            Property(t => t.CarId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            HasRequired(t => t.Car)
                .WithOptional(d => d.MainAdvertCar);
        }
    }
}
