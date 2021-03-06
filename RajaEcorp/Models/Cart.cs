﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RajaEcorp.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string Item { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Totalprice { get; set; }
        public string Buyer { get; set; }
        public string Seller { get; set; }
    }
}
