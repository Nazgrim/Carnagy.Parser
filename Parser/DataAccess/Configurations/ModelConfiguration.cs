using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class ModelConfiguration : EntityTypeConfiguration<Model>
    {
        public ModelConfiguration()
        {
            HasKey(t => t.Id);
        }
    }
}
