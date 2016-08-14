using DataAccess.Models;
using System.Data.Entity.ModelConfiguration;

namespace DataAccess.Configurations
{
    public class ParssedCarConfiguration: EntityTypeConfiguration<ParssedCar>
    {
        public ParssedCarConfiguration()
        {
            HasKey(t => t.Id);
        }
    }
}
