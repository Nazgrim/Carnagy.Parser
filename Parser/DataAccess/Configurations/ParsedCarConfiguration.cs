using DataAccess.Models;
using System.Data.Entity.ModelConfiguration;

namespace DataAccess.Configurations
{
    public class ParsedCarConfiguration: EntityTypeConfiguration<ParsedCar>
    {
        public ParsedCarConfiguration()
        {
            HasKey(t => t.Id);

            HasRequired(t => t.MainConfiguration)
                .WithMany(t => t.ParsedCars)
                .HasForeignKey(d => d.MainConfigurationId)
                .WillCascadeOnDelete(false);
        }
    }
}
