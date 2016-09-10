using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class DrivetrainConfiguration : EntityTypeConfiguration<Drivetrain>
    {
        public DrivetrainConfiguration()
        {
            HasKey(t => t.Id);
        }
    }
}
