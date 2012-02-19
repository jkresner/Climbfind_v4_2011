using NetFrameworkExtensions;

namespace cf.Web.Models
{
    public class Bing7MapWithLocationViewModel : Bing7MapViewModel
    {
        public string ID { get; set; }
        public double LocationOriginalLatitude { get; set; }
        public double LocationOriginalLongitude { get; set; }
        public Bing7MapViewOptionsViewModel ViewOptions { get; set; }
        public override string MapTypeId
        {
            get
            {
                if (ViewOptions == default(Bing7MapViewOptionsViewModel) || string.IsNullOrWhiteSpace(ViewOptions.MapTypeId)) { return base.MapTypeId; }
                else { return ViewOptions.MapTypeId; }
            }
            set
            {
                base.MapTypeId = value;
            }
        } 

        public string InfoBoxImageRelativeUrl { get; set; }

        public Bing7MapWithLocationViewModel() : base ()
        {
            ViewOptions = new Bing7MapViewOptionsViewModel();
        }

        public Bing7MapWithLocationViewModel(string mapId, int width, int height, double latitude, double longitude, string infoBoxImageRelativeUrl)
            : base(mapId, width, height)
        {
            LocationOriginalLatitude = latitude;
            LocationOriginalLongitude = longitude;
            InfoBoxImageRelativeUrl = infoBoxImageRelativeUrl;
        }
    }
}