using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class FieldValueConfiguration: EntityTypeConfiguration<FieldValue>
    {
        public FieldValueConfiguration()
        {
            HasKey(t => new { t.FieldId, t.ParsedCarId });

            HasRequired(t => t.ParsedCar)
                .WithMany(t => t.FieldValues)
                .HasForeignKey(d => d.ParsedCarId);

            HasRequired(t => t.Field)
                .WithMany(t => t.FieldValues)
                .HasForeignKey(d => d.FieldId);

        }
    }
}
