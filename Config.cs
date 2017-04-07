using System.Collections.Generic;
using SystemEndpointsDotnetCore.Models;

namespace App.Flowershop.Cart
{
    public class Config
    {
        public string DataApi { get; set; }

        public List<Endpoint> hosts { get; set; } = new List<Endpoint>();
    }
}
