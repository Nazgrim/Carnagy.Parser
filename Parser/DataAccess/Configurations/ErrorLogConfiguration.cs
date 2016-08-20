using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class ErrorLogConfiguration : EntityTypeConfiguration<ErrorLog>
    {
        public ErrorLogConfiguration()
        {
            HasKey(t => t.Id);
            HasRequired(t => t.MainConfiguration)
                .WithMany(t => t.ErrorLogs)
                .HasForeignKey(d => d.MainConfigurationId);
        }
    }
}
