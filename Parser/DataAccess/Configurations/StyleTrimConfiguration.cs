using System.Data.Entity.ModelConfiguration;
using DataAccess.Models;

namespace DataAccess.Configurations
{
    public class StyleTrimConfiguration : EntityTypeConfiguration<StyleTrim>
    {
        public StyleTrimConfiguration()
        {
            HasKey(t => t.Id);
        }
    }
}
