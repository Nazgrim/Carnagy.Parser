using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class StockCarConfiguration : EntityTypeConfiguration<StockCar>
    {
        public StockCarConfiguration()
        {
            HasKey(t => t.Id);
            HasRequired(t => t.Year)
                .WithMany(t => t.StockCars)
                .HasForeignKey(d => d.YearId);

            HasRequired(t => t.Make)
                .WithMany(t => t.StockCars)
                .HasForeignKey(d => d.MakeId);

            HasRequired(t => t.BodyType)
                .WithMany(t => t.StockCars)
                .HasForeignKey(d => d.BodyTypeId);

            HasRequired(t => t.Drivetrain)
                .WithMany(t => t.StockCars)
                .HasForeignKey(d => d.DrivetrainId);

            HasRequired(t => t.Model)
                .WithMany(t => t.StockCars)
                .HasForeignKey(d => d.ModelId);

            HasRequired(t => t.StyleTrim)
                .WithMany(t => t.StockCars)
                .HasForeignKey(d => d.StyleTrimId);
        }
    }
}
