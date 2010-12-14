using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class Location
    {
        public bool IsNull { get; private set; }

        Location(bool n)
        {
            IsNull = n;
        }

        public static readonly Location Null = new Location(true);
    }

    public class LocationsBag
    {
        public void AddLocation(object element, params Location[] locations)
        {
         //   simple_locs.Add(element, locations);
        }
        public void AddStatement(object element, params Location[] locations)
        {
            if (locations.Length == 0)
                throw new ArgumentException("Statement is missing semicolon location");

          //  simple_locs.Add(element, locations);
        }
    }
}
