using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace cf.Web.Models
{
    public class EditPlacePreferencesViewModel
    {      
        public Guid? PlaceHome { get; set; }
        public string PlaceHomeName { get; set; }
        public Guid? PlaceFavorite1 { get; set; }
        public string PlaceFavorite1Name { get; set; }
        public Guid? PlaceFavorite2 { get; set; }
        public string PlaceFavorite2Name { get; set; }
        public Guid? PlaceFavorite3 { get; set; }
        public string PlaceFavorite3Name { get; set; }
        public Guid? PlaceFavorite4 { get; set; }
        public string PlaceFavorite4Name { get; set; }

        public int PlaceCount { get {
            var count = 0;
            if (PlaceHome.HasValue) { count++; }
            if (PlaceFavorite1.HasValue) { count++; }
            if (PlaceFavorite2.HasValue) { count++; }
            if (PlaceFavorite3.HasValue) { count++; }
            if (PlaceFavorite4.HasValue) { count++; }
            //if (PlaceFavorite5.HasValue) { count++; }

            return count;
        }
        }
    }
}