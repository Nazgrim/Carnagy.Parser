using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class DealerConfiguration : EntityTypeConfiguration<Dealer>
    {
        public DealerConfiguration()
        {
            HasKey(t => t.Id);
        }
    }
}
