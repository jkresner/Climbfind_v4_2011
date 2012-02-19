using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using cf.Caching;
using cf.Entities;
using cf.Entities.Enum;
using cf.Entities.Interfaces;
using NetFrameworkExtensions.Web.Mvc;

namespace cf.Web.Mvc.ViewData
{
    /// <summary>
    /// Utility-like class giving us easy programmatic access to common data that is worthy of caching
    /// </summary>
    public static class SelectLists
    {
        private static SelectList yesNoList = null;
        public static SelectList YesNoList { get {
            if (yesNoList == null)
            {
                Dictionary<string, string> types = new Dictionary<string, string>();
                types.Add("true", "Yes");
                types.Add("false", "No");
                yesNoList = types.ToSelectList(k => k.Key, v => v.Value);
            }
            return  yesNoList;
        } }
        
        private static SelectList countrySelectList = null;
        public static SelectList CountrySelectList { get {
            if (countrySelectList == null)
            {
                countrySelectList = AppLookups.Countries.ToSelectList(c => c.ID, c => c.Name);
            }
            return  countrySelectList;
        } }

        private static SelectList countryWithUKSelectList = null;
        public static SelectList CountryWithUKSelectList
        {
            get
            {
                if (countryWithUKSelectList == null)
                {
                    var countriesWUK = new List<Country>() { new Country() { ID = 83, Name = "United Kingdom" } };
                    countriesWUK.AddRange(AppLookups.Countries); //-- Allow upgrades from cf3 to keep there nationality & update their profile
                    countryWithUKSelectList = countriesWUK.ToSelectList(c => c.ID, c => c.Name);
                }
                return countryWithUKSelectList;
            }
        }

        private static SelectList displayNameTypeSelectList = null;
        public static SelectList DisplayNameTypeSelectList
        {
            get
            {
                if (displayNameTypeSelectList == null)
                {
                    Dictionary<byte, string> items = new Dictionary<byte, string>();
                    items.Add(0, "Full Name");
                    items.Add(2, "Nick Name");
                    items.Add(1, "User Name"); //-- out of order to match edit profile page form order

                    displayNameTypeSelectList = items.ToSelectList(k => k.Key, v => v.Value);
                }
                return displayNameTypeSelectList;
            }
        }


        private static SelectList ratingSelectList = null;
        public static SelectList RatingSelectList
        {
            get
            {
                if (ratingSelectList == null)
                {
                    var options = new Dictionary<byte, string>();
                    options.Add(1, "Very poor");
                    options.Add(2, "Not that bad");
                    options.Add(3, "Average");
                    options.Add(4, "Good");
                    options.Add(5, "Perfect");
                    ratingSelectList = options.ToSelectList(k => k.Key, v => v.Value);
                }
                return ratingSelectList;
            }
        }


        private static SelectList indoorLocationTypeSelectList = null;
        public static SelectList IndoorLocationTypeSelectList
        {
            get
            {
                if (indoorLocationTypeSelectList == null)
                {
                    var types = new Dictionary<string, string>();
                    types.Add(CfType.CommercialIndoorClimbing.ToString(), "Dedicated commercial climbing facility");
                    types.Add(CfType.SportsCenter.ToString(), "Sports center with limited climbing");
                    types.Add(CfType.PrivateIndoorClimbing.ToString(), "Private indoor facility or woodie");
                    indoorLocationTypeSelectList = types.ToSelectList(k => k.Key, v => v.Value);
                }
                return indoorLocationTypeSelectList;
            }
        }

        private static SelectList outdoorLocationTypeSelectList = null;
        public static SelectList OutdoorLocationTypeSelectList
        {
            get
            {
                if (outdoorLocationTypeSelectList == null)
                {
                    Dictionary<string, string> types = new Dictionary<string, string>();
                    types.Add(CfType.RockWall.ToString(), "Rock wall");
                    types.Add(CfType.RockBoulder.ToString(), "Boulder");
                    types.Add(CfType.RockWaterSoloing.ToString(), "Water soloing");
                    types.Add(CfType.Summit.ToString(), "Summit");
                    types.Add(CfType.IceWall.ToString(), "Ice wall");
                    outdoorLocationTypeSelectList = types.ToSelectList(k => k.Key, v => v.Value);
                }
                return outdoorLocationTypeSelectList;
            }
        }


        public static SelectList GetAreaTypeSelectList(bool includeCountry)
        {
            Dictionary<string, string> items = new Dictionary<string, string>();
            if (includeCountry) { items.Add(CfType.Country.ToString(), "Country"); }
            items.Add(CfType.Province.ToString(), "Province / state / territory");
            items.Add(CfType.City.ToString(), "City");
            items.Add(CfType.ClimbingArea.ToString(), "Climbing area");
            return items.ToSelectList(k => k.Key, v => v.Value);
        }


        private static SelectList partnerCallClimbingLevelSelectList = null;
        public static SelectList PartnerCallClimbingLevelSelectList
        {
            get
            {
                if (partnerCallClimbingLevelSelectList == null)
                {
                    Dictionary<string, string> items = new Dictionary<string, string>();
                    items.Add(ClimbingLevelGeneral.Any.ToString(), "Any");
                    items.Add(ClimbingLevelGeneral.Beginner.ToString(), "Beginner");
                    items.Add(ClimbingLevelGeneral.Intermediate.ToString(), "Intermediate");
                    items.Add(ClimbingLevelGeneral.Advanced.ToString(), "Advanced");
                    partnerCallClimbingLevelSelectList = items.ToSelectList(k => k.Key, v => v.Value);
                }
                return partnerCallClimbingLevelSelectList;
            }
        }

        private static SelectList gradeOpinionSelectList = null;
        public static SelectList GradeOpinionSelectList 
        {
            get
            {
                if (gradeOpinionSelectList == null)
                {
                    Dictionary<string, string> items = new Dictionary<string, string>();
                    items.Add(ClimbGradeOpinion.Easy.ToString(), "Easy");
                    items.Add(ClimbGradeOpinion.SpotOn.ToString(), "Spot on");
                    items.Add(ClimbGradeOpinion.Hard.ToString(), "Hard");
                    gradeOpinionSelectList = items.ToSelectList(k => k.Key, v => v.Value);
                }
                return gradeOpinionSelectList;
            }
        }

        private static SelectList gradeSelectList = null;
        public static SelectList GradeSelectList
        {
            get
            {
                if (gradeSelectList == null)
                {
                    Dictionary<string, string> items = new Dictionary<string, string>();
                    foreach (var grade in CfEnumExtensions.GetGrades())
                    {
                        items.Add(grade, grade);
                    }                    
                    gradeSelectList = items.ToSelectList(k => k.Key, v => v.Value);
                }
                return gradeSelectList;
            }
        }

        private static SelectList climbTerrainTypeSelectList = null;
        public static SelectList ClimbTerrainTypeSelectList
        {
            get
            {
                if (climbTerrainTypeSelectList == null)
                {
                    Dictionary<byte, string> items = new Dictionary<byte, string>();
                    items.Add(0, "Unspecified");
                    items.Add(1, "Rock");
                    items.Add(2, "Ice");
                    items.Add(3, "Mixed");
                    items.Add(4, "Plastic");

                    climbTerrainTypeSelectList = items.ToSelectList(k => k.Key, v => v.Value);
                }
                return climbTerrainTypeSelectList;
            }
        }

        private static SelectList indoorClimbTypeSelectList = null;
        public static SelectList IndoorClimbTypeSelectList
        {
            get
            {
                if (indoorClimbTypeSelectList == null)
                {
                    Dictionary<byte, string> items = new Dictionary<byte, string>();
                    items.Add(0, "Unspecified");
                    items.Add(1, "Top rope only");
                    items.Add(2, "Top rope and lead");
                    items.Add(3, "Lead only");
                    items.Add(4, "Boulder");
                    
                    indoorClimbTypeSelectList = items.ToSelectList(k => k.Key, v => v.Value);
                }
                return indoorClimbTypeSelectList;
            }
        }

        private static SelectList indoorClimbMarkingTypeSelectList = null;
        public static SelectList IndoorClimbMarkingTypeSelectList
        {
            get
            {
                if (indoorClimbMarkingTypeSelectList == null)
                {
                    Dictionary<byte, string> items = new Dictionary<byte, string>();
                    items.Add(0, "Unspecified");
                    items.Add(1, "Hold");
                    items.Add(2, "Tape");
                    
                    indoorClimbMarkingTypeSelectList = items.ToSelectList(k => k.Key, v => v.Value);
                }
                return indoorClimbMarkingTypeSelectList;
            }
        }
    }
}

