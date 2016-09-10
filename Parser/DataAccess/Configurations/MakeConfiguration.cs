using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class MakeConfiguration : EntityTypeConfiguration<Make>
    {
        public MakeConfiguration()
        {
            HasKey(t => t.Id);
        }
    }
}
