using DataAccess.Models;
using System.Data.Entity.ModelConfiguration;

namespace DataAccess.Configurations
{
    public class MainConfigurationConfiguration : EntityTypeConfiguration<MainConfiguration>
    {
        public MainConfigurationConfiguration()
        {
            HasKey(t => t.Id);
        }
    }
}
