using NetFrameworkExtensions;
using cf.Entities;

namespace cf.Web.Models
{
    public class Bing7EditAreaMapViewModel : Bing7MapViewModel
    {
        public string ID { get; set; }
        public double LocationOriginalLatitude { get; set; }
        public double LocationOriginalLongitude { get; set; }
        public Bing7MapViewOptionsViewModel ViewOptions { get; set; }
                
        public Bing7EditAreaMapViewModel() : base ()
        {
            ViewOptions = new Bing7MapViewOptionsViewModel();
        }

        public Bing7EditAreaMapViewModel(string mapId, int width, int height, PlaceBingMapView mapViewSettings)
            : base (mapId, width, height)
        {
            ViewOptions = new Bing7MapViewOptionsViewModel(mapViewSettings);
        }
    }
}