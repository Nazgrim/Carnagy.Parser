﻿using System.Collections.Generic;

namespace DataAccess.Models
{
    public class Dealer
    {
        public Dealer()
        {
            Cars = new List<Car>();
        }
        public int Id { get; set; }
        public string Location { get; set; }
        public string WebSireUrl { get; set; }
        public string WebSiteName { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Car> Cars { get; set; }
    }
}