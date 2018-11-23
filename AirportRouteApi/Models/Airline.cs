using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirportRouteApi.Models
{
    public class Airline
    {
        public string Name { get; set; }

        public string Alias { get; set; }

        public bool Active { get; set; }
    }
}