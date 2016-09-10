using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class BodyTypeConfiguration : EntityTypeConfiguration<BodyType>
    {
        public BodyTypeConfiguration()
        {
            HasKey(t => t.Id);
        }
    }
}
