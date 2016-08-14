using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class FieldConfiguration : EntityTypeConfiguration<Field>
    {
        public FieldConfiguration()
        {
            HasKey(t => t.Id);
            HasRequired(t => t.MainConfiguration)
                .WithMany(t => t.Fields)
                .HasForeignKey(d => d.MainConfigurationId);
        }
    }
}
