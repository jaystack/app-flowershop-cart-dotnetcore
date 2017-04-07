using App.Flowershop.Cart.Models;
using System.Collections.Generic;

namespace App.Flowershop.Cart.ViewModels
{
    public class CartViewModel
    {
        public List<string> Items { get; set; }

        public decimal CartValue { get; set; }

        public List<Flower> Flowers { get; set; }

        public CartViewModel()
        {
            Items = new List<string>();
            Flowers = new List<Flower>();
        }
    }
}
