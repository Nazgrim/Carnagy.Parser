using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public class ParsedCar
    {
        public ParsedCar()
        {
            FieldValues = new List<FieldValue>();
            Prices = new List<Price>();
        }

        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsParsed { get; set; }

        public int MainConfigurationId { get; set; }

        public virtual MainConfiguration MainConfiguration { get; set; }
        public virtual AdvertCar AdvertCar { get; set; }
        public virtual ICollection<FieldValue> FieldValues { get; set; }
        public virtual ICollection<Price> Prices { get; set; }

    }
}
