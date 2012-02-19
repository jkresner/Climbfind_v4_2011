using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Enum;
using cf.Entities;
using cf.Entities.Interfaces;
using cf.Instrumentation;
using cf.DataAccess.Repositories;
using Microsoft.SqlServer.Types;

namespace cf.Services
{
    /// <summary>
    /// Returns data required for map representations
    /// </summary>
    /// <remarks>
    /// Which is usually converted into json and request by a client side call
    /// </remarks>
    public partial class MappingService
    {
        AreaRepository areaRepo { get { if (_areaRepo == null) { _areaRepo = new AreaRepository(); } return _areaRepo; } } AreaRepository _areaRepo;
        CountryRepository countryRepo { get { if (_countryRepo == null) { _countryRepo = new CountryRepository(); } return _countryRepo; } } CountryRepository _countryRepo;
        LocationRepository locRepo { get { if (_locRepo == null) { _locRepo = new LocationRepository(); } return _locRepo; } } LocationRepository _locRepo;
        LocationIndoorRepository locIndoorRepo { get { if (_locIndoorRepo == null) { _locIndoorRepo = new LocationIndoorRepository(); } return _locIndoorRepo; } } LocationIndoorRepository _locIndoorRepo;
        LocationOutdoorRepository locOutdoorRepo { get { if (_locOutdoorRepo == null) { _locOutdoorRepo = new LocationOutdoorRepository(); } return _locOutdoorRepo; } } LocationOutdoorRepository _locOutdoorRepo;
        LocationBingViewRepository locBingViewRepo { get { if (_locBingViewRepo == null) { _locBingViewRepo = new LocationBingViewRepository(); } return _locBingViewRepo; } } LocationBingViewRepository _locBingViewRepo;

        //-- BingView Stuff
        public PlaceBingMapView GetBingViewByID(Guid id) { return locBingViewRepo.GetByID(id); }

        public PlaceBingMapView CreateBingView(PlaceBingMapView obj)
        {
            if (obj.MapTypeId == "a") { obj.MapTypeId = "aerial"; }
            if (obj.MapTypeId == "be") { obj.MapTypeId = "birdseye"; }
            if (obj.MapTypeId == "r") { obj.MapTypeId = "road"; }
            if (obj.CenterOffset == null) { obj.CenterOffset = string.Empty; }
            return locBingViewRepo.Create(obj);
        }

        public PlaceBingMapView UpdateBingView(PlaceBingMapView obj) {
            if (obj.MapTypeId == "a") { obj.MapTypeId = "aerial"; }
            if (obj.MapTypeId == "be") { obj.MapTypeId = "birdseye"; }
            if (obj.MapTypeId == "r") { obj.MapTypeId = "road"; }
            if (obj.CenterOffset == null) { obj.CenterOffset = string.Empty; }
            return locBingViewRepo.Update(obj); }

        /// <summary>
        /// Map Item Collections
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        private MapItemCollection CreateMapItemCollectionWithReducedParentItem(IArea area)
        {
            MapItemCollection collection = new MapItemCollection();
            SqlGeography reducedGeo = area.Geo;
            if (area.GeoReduceThreshold > 5000) { reducedGeo = area.Geo.STBuffer(4000).Reduce(area.GeoReduceThreshold - 5000); }
            else if (area.GeoReduceThreshold > 0) { reducedGeo = area.Geo.Reduce(area.GeoReduceThreshold); }
            collection.AppendGeographyToGeoMapItemCollection(reducedGeo, area.Name);
            return collection;
        }
        
        public MapItemCollection GetCountryMapItems(Country country)
        {
            MapItemCollection collection = CreateMapItemCollectionWithReducedParentItem(country);

            var areas = areaRepo.GetCitiesAndMajorClimbingAreasOfCountry(country.ID);

            var cities = areas.Where(c => c.Type == CfType.City);
            var climbingAreas = areas.Where(c => c.Type == CfType.ClimbingArea).ToList().RemoveAllChildAreas();

            //-- We actually only want to display cities and climbing areas in the country view
            AddSpecificAreaTypes(collection, climbingAreas, CfType.ClimbingArea, "ca");
            //-- Also we want to do cities last because they should appear on the top
            AddSpecificAreaTypes(collection, cities, CfType.City, "cty");

            return collection;
        }

        public MapItemCollection GetProvinceMapItems(Area area)
        {
            MapItemCollection collection = CreateMapItemCollectionWithReducedParentItem(area);

            var locations = locRepo.GetLocationsOfArea(area.ID);
            var indoorLocations = locations.Where(l => l.Type.ToPlaceCateogry() == PlaceCategory.IndoorClimbing).ToList();

            AddLocationsToCollection(collection, indoorLocations, true, false, false, false);

            var areas = areaRepo.GetIntersectingAreasWithGeoInflate(area.ID);
            var cities = areas.Where(c => c.Type == CfType.City);
            var climbingAreas = areas.Where(c => c.Type == CfType.ClimbingArea).ToList();
            climbingAreas = area.RemoveIntersectingNonChildAreas(climbingAreas).RemoveAllChildAreas();

            //-- We actually only want to display cities and climbing areas in the country view
            AddSpecificAreaTypes(collection, climbingAreas, CfType.ClimbingArea, "ca");
            //-- Also we want to do cities last because they should appear on the top
            AddSpecificAreaTypes(collection, cities, CfType.City, "cty");

            return collection;
        }


        public MapItemCollection GetCityMapItems(Area area)
        {
            MapItemCollection collection = CreateMapItemCollectionWithReducedParentItem(area);

            var locations = locRepo.GetLocationsOfArea(area.ID);
            var indoorLocations = locations.Where(l => l.Type.ToPlaceCateogry() == PlaceCategory.IndoorClimbing).ToList();

            AddLocationsToCollection(collection, indoorLocations, true, false, false, false);

            var areas = areaRepo.GetIntersectingAreasWithGeoInflate(area.ID);
            var climbingAreas = areas.Where(c => c.Type == CfType.ClimbingArea).ToList();
            climbingAreas = area.RemoveIntersectingNonChildAreas(climbingAreas).RemoveAllChildAreas();

            //-- We actually only want to display cities and climbing areas in the country view
            AddSpecificAreaTypes(collection, climbingAreas, CfType.ClimbingArea, "ca");
            
            return collection;
        }


        public MapItemCollection GetAreaMapItems(Area area)
        {
            MapItemCollection collection = CreateMapItemCollectionWithReducedParentItem(area);
            
            var areas = areaRepo.GetIntersectingAreasWithGeoInflate(area.ID);
            
            var climbingAreas = areas.Where(a => a.Type == CfType.ClimbingArea && a.ID != area.ID).ToList();
            climbingAreas = area.RemoveIntersectingNonChildAreas(climbingAreas).RemoveAllChildAreas();

            var locations = locRepo.GetLocationsOfArea(area.ID);
            var indoorLocations = locations.Where(l => l.Type.ToPlaceCateogry() == PlaceCategory.IndoorClimbing).ToList();
            var outdoorLocations = locations.Where(l => l.Type.ToPlaceCateogry() == PlaceCategory.OutdoorClimbing).ToList();
            var outdoorOrphanLocations = outdoorLocations.GetOrphanLocations(climbingAreas);

            AddLocationsToCollection(collection, indoorLocations, true, true, false, false);
            AddLocationsToCollection(collection, outdoorOrphanLocations, true, true, false, false);
  
            //-- We actually only want to display cities and climbing areas in the country view
            AddSpecificAreaTypes(collection, climbingAreas, CfType.ClimbingArea, "ca");
            
            return collection;
        }


        public MapItemCollection GetAreaEditMapItems(Area area)
        {
            //-- Edit is a bit different, we don't want to reduce the polygon so we use the normal constructor
            MapItemCollection collection = new MapItemCollection();
            collection.AppendGeographyToGeoMapItemCollection(area.Geo, area.Name);

            var locations = locRepo.GetLocationsOfArea(area.ID);
            var indoorLocations = locations.Where(l => l.Type.ToPlaceCateogry() == PlaceCategory.IndoorClimbing).ToList();
            var outdoorLocations = locations.Where(l => l.Type.ToPlaceCateogry() == PlaceCategory.OutdoorClimbing).ToList();

            AddLocationsToCollection(collection, indoorLocations, true, true, false, false);
            AddLocationsToCollection(collection, outdoorLocations, true, true, false, false);
            
            return collection;
        }

        public MapItemCollection GetAreaForNewOutdoorClimbingLocationMapItems(Area area)
        {
            MapItemCollection collection = new MapItemCollection();
            collection.AppendGeographyToGeoMapItemCollection(area.Geo, area.Name);
            
            var locations = locRepo.GetLocationsOfArea(area.ID);
            var outdoorLocations = locations.Where(l => l.Type.ToPlaceCateogry() == PlaceCategory.IndoorClimbing).ToList();
            AddLocationsToCollection(collection, outdoorLocations, true, true, false, false);
 
            return collection;
        }

        private void AddRelatedAreasToCollection(MapItemCollection collection, Area area, bool provinces, bool cities, bool outdoorAreas)
        {
            var relatedAreas = areaRepo.GetIntersectingAreasWithGeoInflate(area.ID);

            //if (provinces)
            //{
            //    var intersectingCities = relatedAreas.Where(c => c.GeoType == Entities.Enum.GeographyType.Province);
            //    foreach (var city in intersectingCities)
            //    {
            //        SqlGeography center = city.Geo.EnvelopeCenter();
            //        collection.AddPoint(city.Name, city.SearchSupportString, center.Lat.Value, center.Long.Value, "Cty");
            //    }
            //}

            if (outdoorAreas)
            {
                //-- Intersecting climbing areas
                AddSpecificAreaTypes(collection, relatedAreas, CfType.ClimbingArea, "ca");
            }

            //-- We want to do cities last so that they (the city icons) render on top
            if (cities)
            {
                //-- Intersecting cities
                AddSpecificAreaTypes(collection, relatedAreas, CfType.City, "cty");
            }
        }

        


        private void AddSpecificAreaTypes(MapItemCollection collection, IEnumerable<Area> areas, CfType placeType, string placeTypeString)
        {
            foreach (var a in areas)
            {
                collection.AddPoint(a.Name, a.SearchSupportString, a.SlugUrl, a.Avatar, placeTypeString, a.Latitude, a.Longitude);
            }
        }

        private void AddLocationsToCollection(MapItemCollection collection, IList<Location> locations, bool indoor, bool outdoor, bool business, bool meetingPoints)
        {
            //-- Return Indoor Climbing
            foreach (var l in locations)
            {
                var cat = l.Type.ToPlaceCateogry();

                if (indoor && (cat == PlaceCategory.IndoorClimbing))
                {
                    collection.AddPoint(l.Name, l.Description, l.SlugUrl, l.Avatar, "id", l.Latitude, l.Longitude);
                }
                if (outdoor && (cat == PlaceCategory.OutdoorClimbing))
                {
                    collection.AddPoint(l.Name, l.Description, l.SlugUrl, l.Avatar, "od", l.Latitude, l.Longitude);
                }
            }
        }
        
        private void AddAreasToCollection(MapItemCollection collection, IEnumerable<Area> areas, CfType placeType, string placeTypeString)
        {
            foreach (var a in areas)
            {
                collection.AddPoint(a.Name, a.SearchSupportString, a.SlugUrl, a.Avatar, placeTypeString, a.Latitude, a.Longitude);
            }
        }


        /// <summary>
        /// Eventually this should be done properly by hooking into the existing db infrastructure
        /// </summary>
        /// <param name="country"></param>
        public PlaceBingMapView GetCustomCountryBingMapView(Country country)
        {
            if (country.Name == "United States") { return CustomView(39.888705465871716, -96.36013749999988, 4); }
            else if (country.Name == "New Zealand") { return CustomView(-41.363228079400926, 174.4674375000009, 5); }
            else if (country.Name == "Russian Federation") { return CustomView(63.43054602165467, 88.83722187500018, 3); }
            else if (country.Name == "Fiji") { return CustomView(-17.72193049082709, 179.7639439453136, 7); }
            else if (country.Name == "Kiribati") { return CustomView(1.8515559879188288, -157.60823876951835, 9); }
            else if (country.Name == "France") { return CustomView(47.964916, 1.710205, 5); }
            else if (country.Name == "Canada") { return CustomView(64.623877, -105.117187, 3); }
            return default(PlaceBingMapView);
        }

        private PlaceBingMapView CustomView(double lat, double lon, int zoom)
        {
            return new PlaceBingMapView()
            {
                Bounds = "Custom",
                MapTypeId = "road",
                MapCenterLatitude = lat.ToString(),
                MapCenterLongitude = lon.ToString(),
                Zoom = zoom.ToString()
            };
        }
    }
}
