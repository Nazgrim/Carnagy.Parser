using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class YearConfiguration : EntityTypeConfiguration<Year>
    {
        public YearConfiguration()
        {
            HasKey(t => t.Id);
        }
    }
}
