using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFO_app
{
    public class SightingData
    {
        public DateTime SightingDate { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Shape { get; set; }
        public string Duration { get; set; }
        public string Comments { get; set; }
        public string DatePosted { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public override string ToString()
        {
            return City;
        }
    }
}
