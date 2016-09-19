using DataAccess.Models;
using System.Data.Entity.ModelConfiguration;

namespace DataAccess.Configurations
{
    public class MainConfigurationConfiguration : EntityTypeConfiguration<MainConfiguration>
    {
        public MainConfigurationConfiguration()
        {
            HasKey(t => t.Id);

            //HasOptional(t => t.Dealer)
            //    .WithMany(t => t.Configurations)
            //    .HasForeignKey(d => d.DealerId);
        }
    }
}
