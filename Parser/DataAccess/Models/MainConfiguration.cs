using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public class MainConfiguration
    {
        public MainConfiguration()
        {
            ErrorLogs= new List<ErrorLog>();
            Fields = new List<Field>();
            ParssedCars= new List<ParssedCar>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string SiteUrl { get; set; }
        public int HoursPeriond { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? LastTimeUpdate { get; set; }

        public virtual ICollection<ParssedCar> ParssedCars { get; set; }
        public virtual ICollection<Field> Fields { get; set; }
        public virtual ICollection<ErrorLog> ErrorLogs { get; set; }
    }
}
