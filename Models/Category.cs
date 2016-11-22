using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Flowershop.Cart.Models
{
    public class Category : ModelBase
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Selected { get; set; }
    }
}
