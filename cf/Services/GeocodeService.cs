using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using cf.Entities;
using System.Xml.Linq;
using System.IO;
using cf.Caching;
using cf.Instrumentation;
using Microsoft.SqlServer.Types;
using NetFrameworkExtensions.Web;
using NetFrameworkExtensions.SqlServer.Types;
using GeogExtensions = NetFrameworkExtensions.SqlServer.Types.SqlGeographyExtensions;
using cf.Dtos;


namespace cf.Services
{
    /// <summary>
    /// Leverages Bing and Google to resolve latitude & longitude for text addressees
    /// </summary>
    public partial class GeocodeService : AbstractCfService
    {
        protected const string BingMapsApiKey = "ArIOaOmY-BqIbbf1Ueo_9McVfA9iTm_WdfX9-Boyeyg_ZuSN1dCeNQ5d1bvxsdgt";

        /// <summary>
        /// Returns geocode matches that fall within the specified region
        /// </summary>
        /// <param name="country"></param>
        /// <param name="locality"></param>
        /// <param name="regionResultMustBeIn"></param>
        /// <returns></returns>
        public List<GeocodeResult> Geocode(Country country, string locality, SqlGeography regionResultMustBeIn)
        {
            var filteredResults = new List<GeocodeResult>();
            
            //-- Get all geocode results then only include the ones that fall in the region in our filtered results
            foreach (var r in Geocode(country, locality))
            {
                var point = GeogExtensions.GetGeoPoint(double.Parse(r.Lat), double.Parse(r.Lon));

                if (point.STIntersects(regionResultMustBeIn)) { filteredResults.Add(r); }
            }

            return filteredResults;
        }

        /// <summary>
        /// Return all geocode results that are within the specified country
        /// </summary>
        /// <param name="country"></param>
        /// <param name="locality"></param>
        /// <returns></returns>
        public List<GeocodeResult> Geocode(Country country, string locality)
        {
            List<GeocodeResult> results;
            
            //-- Take out special characters from the locality so when we put it in the url our web request works
            var cleanedLocalityString = locality.Replace("&", "and").Replace("#", "").Trim();

            try
            {
                //- Get the results from bing against the country specific store
                results = BingGeocodeCountryRegionQuery(country, cleanedLocalityString);

                if (results.Count == 0) 
                { 
                    //-- Sometimes if it doesn't work when we make a country specific call if we make a general call to bing we get a result
                    //-- e.g. "The Peak District National Park" in the UK
                    results = BingGeocodeSingleQuery(country, cleanedLocalityString); 
                }
                else
                {
                    //-- Double check that bing hasn't give us a stupid default result representing just the country and not the locality
                    if (ResultIsDefaultCountry(country, results))
                    {
                        results = BingGeocodeSingleQuery(country, cleanedLocalityString);
                        if (ResultIsDefaultCountry(country, results))
                        {
                            //-- We don't want to send back a result of country type
                            results.Clear();
                        }
                    }
                    else
                    {
                        //-- Sometimes the result we get from the country specific call sucks and we want to give 
                        //-- the user access to the second result as well
                        results.AddRange(BingGeocodeSingleQuery(country, cleanedLocalityString));
                    }
                }

                //-- If after all the goes above we still don't have anything from silly Bing, let's use google
                if (results.Count == 0)
                {
                    results = GooleGeocodeSingleQuery(country, cleanedLocalityString);
                }

                //-- Remove duplicates
                if (results.Count > 1)
                {
                    results = results.Distinct(new GeocodeResultEqualityComparer()).ToList();
                }
            }
            catch (Exception ex)
            {
                ex.Data.Add("locality", cleanedLocalityString);
                CfTrace.Error(ex);
                throw;
            }

            return results;
        }


        private bool ResultIsDefaultCountry(Country country, List<GeocodeResult> geocodeResults)
        {
            return geocodeResults.Count == 1 && (geocodeResults.First().Name == "United Kingdom"
                    || geocodeResults.First().Name == country.Name);
        }

        private List<GeocodeResult> BingGeocodeCountryRegionQuery(Country country, string locality)
        {
            string countryRegionISO2 = AppLookups.CountryIso2(country.ID).ToUpper();
            string url = string.Format("http://dev.virtualearth.net/REST/v1/Locations?CountryRegion={0}&locality={1}&key={2}&output=xml",
             countryRegionISO2, locality, BingMapsApiKey);
            string xml = new Uri(url).ExecuteRestCall();
            return ParseBingXmlResponse(xml);
        }


        private List<GeocodeResult> BingGeocodeSingleQuery(Country country, string locality)
        {
            var localityAndCountry = string.Format("{0}, {1}", locality, country.Name);
            string url = string.Format("http://dev.virtualearth.net/REST/v1/Locations/{0}?key={1}&output=xml", localityAndCountry, BingMapsApiKey);
            string xml = new Uri(url).ExecuteRestCall();
            return ParseBingXmlResponse(xml);
        }

        
        private List<GeocodeResult> GooleGeocodeSingleQuery(Country country,string locality)
        {
 	        var localityAndCountry = string.Format("{0}, {1}", locality, country.Name);
            string url = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", localityAndCountry);
            string xml = new Uri(url).ExecuteRestCall();
            return ParseGoogleXmlResponse(xml);
        }

        private List<GeocodeResult> ParseBingXmlResponse(string xmlText)
        {
            List<GeocodeResult> geocodeResults = new List<GeocodeResult>();

            var xml = XElement.Parse(xmlText);
            XNamespace ns = "http://schemas.microsoft.com/search/local/ws/rest/v1";
            var results = (from c in xml.Descendants(ns + "Location") select c);
            foreach (var result in results)
            {
                geocodeResults.Add(new GeocodeResult()
                {
                    Name = (from c in result.Descendants(ns + "Name") select c).First().Value,
                    Lat = (from c in result.Descendants(ns + "Latitude") select c).First().Value,
                    Lon = (from c in result.Descendants(ns + "Longitude") select c).First().Value,
                    Address = (from c in result.Descendants(ns + "FormattedAddress") select c).First().Value,
                    Box = new GeoBoundingBox
                    {
                        EastLon = (from c in result.Descendants(ns + "EastLongitude") select c).First().Value,
                        NorthLat = (from c in result.Descendants(ns + "NorthLatitude") select c).First().Value,
                        SouthLat = (from c in result.Descendants(ns + "SouthLatitude") select c).First().Value,
                        WestLon = (from c in result.Descendants(ns + "WestLongitude") select c).First().Value
                    }
                });
            }

            return geocodeResults;
        }


        private List<GeocodeResult> ParseGoogleXmlResponse(string xmlText)
        {
            List<GeocodeResult> geocodeResults = new List<GeocodeResult>();

            var xml = XElement.Parse(xmlText);
            var results = (from c in xml.Descendants("result") select c);
            foreach (var result in results)
            {
                geocodeResults.Add(new GeocodeResult()
                {
                    Name = (from c in result.Descendants("formatted_address") select c).First().Value,
                    Lat = (from c in result.Descendants("location").Descendants("lat") select c).First().Value,
                    Lon = (from c in result.Descendants("location").Descendants("lng") select c).First().Value,
                    Address = (from c in result.Descendants("formatted_address") select c).First().Value,
                    Box = new GeoBoundingBox
                    {
                        EastLon = (from c in result.Descendants("viewport").Descendants("northeast").Descendants("lng") select c).First().Value,
                        NorthLat = (from c in result.Descendants("viewport").Descendants("northeast").Descendants("lat") select c).First().Value,
                        SouthLat = (from c in result.Descendants("viewport").Descendants("southwest").Descendants("lat") select c).First().Value,
                        WestLon = (from c in result.Descendants("viewport").Descendants("southwest").Descendants("lng") select c).First().Value
                    }
                });
            }

            return geocodeResults;
        }

    }
}
