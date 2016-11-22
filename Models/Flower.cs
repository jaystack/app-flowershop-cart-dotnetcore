﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Flowershop.Cart.Models
{
    public class Flower : ModelBase
    {
        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Category { get; set; }

        public decimal Price { get; set; }

        public int Amount { get; set; }
    }
}
