﻿using System.Collections.Generic;

namespace WepApi.Models
{
    public class Dealer
    {
        public int id { get; set; }
        public List<Car> cars { get; set; }      
    }
}