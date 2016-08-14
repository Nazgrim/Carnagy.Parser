using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class FieldValueConfiguration: EntityTypeConfiguration<FieldValue>
    {
        public FieldValueConfiguration()
        {
            HasKey(t => new {t.FieldId, t.ParssedCarId});

            HasRequired(t => t.ParssedCar)
                .WithMany(t => t.FieldValues)
                .HasForeignKey(d => d.ParssedCarId);

            HasRequired(t => t.Field)
                .WithMany(t => t.FieldValues)
                .HasForeignKey(d => d.FieldId);

        }
    }
}
