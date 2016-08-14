using System.Collections.Generic;

namespace DataAccess.Models
{
    public class MainConfiguration
    {
        public MainConfiguration()
        {
            Fields = new List<Field>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string SiteUrl { get; set; }
        public int HoursPeriond { get; set; }

        public virtual ICollection<Field> Fields { get; set; }
    }
}
