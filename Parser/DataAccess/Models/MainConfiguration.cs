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
            ParsedCars= new List<ParsedCar>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string SiteUrl { get; set; }
        public int HoursPeriond { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? LastTimeUpdate { get; set; }

        public int? DealerId { get; set; }

        public virtual Dealer Dealer { get; set; }

        public virtual ICollection<ParsedCar> ParsedCars { get; set; }
        public virtual ICollection<Field> Fields { get; set; }
        public virtual ICollection<ErrorLog> ErrorLogs { get; set; }
    }
}
