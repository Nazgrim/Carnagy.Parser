using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    public class ParssedCar
    {
        public ParssedCar()
        {
            FieldValues = new List<FieldValue>();
        }
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsDeleted { get; set; }

        public int MainConfigurationId { get; set; }

        public virtual MainConfiguration MainConfiguration { get; set; }
        public virtual ICollection<FieldValue> FieldValues { get; set; }
    }
}
