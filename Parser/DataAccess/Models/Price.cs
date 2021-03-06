﻿using System;

namespace DataAccess.Models
{
    public class Price
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Value { get; set; }

        public int ParsedCarId { get; set; }

        public virtual ParsedCar ParsedCar { get; set; }
    }
}
