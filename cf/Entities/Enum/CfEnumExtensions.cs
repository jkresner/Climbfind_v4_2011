using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Entities.Enum
{
    public static class CfEnumExtensions
    {
        private static readonly Dictionary<CfType, PlaceCategory> placeTypeToCategory;
        private static readonly Dictionary<ModActionType, byte> modActionTypeToPoints;

        private static readonly Dictionary<string, byte> gradeToCfRank;

        static CfEnumExtensions()
        {
            gradeToCfRank = new Dictionary<string, byte>();

            gradeToCfRank.Add("Unestablished", 0);
            gradeToCfRank.Add("Don't know", 0);
            
            gradeToCfRank.Add("V0",6);
            gradeToCfRank.Add("V1",10);
            gradeToCfRank.Add("V2",15);
            gradeToCfRank.Add("V3",20);
            gradeToCfRank.Add("V4",25);
            gradeToCfRank.Add("V5",30);
            gradeToCfRank.Add("V6",35);
            gradeToCfRank.Add("V7",40);
            gradeToCfRank.Add("V8",45);
            gradeToCfRank.Add("V9",50);
            gradeToCfRank.Add("V10",55);
            gradeToCfRank.Add("V11",60);
            gradeToCfRank.Add("V12",70);
            gradeToCfRank.Add("V13",75);
            gradeToCfRank.Add("V14",80);
            gradeToCfRank.Add("V15",85);
            gradeToCfRank.Add("V16",90);
            gradeToCfRank.Add("V17",95);
            
            gradeToCfRank.Add("5.0", 1);
            gradeToCfRank.Add("5.1", 2);
            gradeToCfRank.Add("5.2", 3);
            gradeToCfRank.Add("5.3", 4);
            gradeToCfRank.Add("5.4", 5);
            gradeToCfRank.Add("5.5", 6);
            gradeToCfRank.Add("5.6", 9);
            gradeToCfRank.Add("5.7", 11);
            gradeToCfRank.Add("5.8", 16);
            gradeToCfRank.Add("5.9", 18);
            gradeToCfRank.Add("5.10a", 22);
            gradeToCfRank.Add("5.10b", 24);
            gradeToCfRank.Add("5.10", 26);
            gradeToCfRank.Add("5.10c", 27);
            gradeToCfRank.Add("5.10d", 28);
            gradeToCfRank.Add("5.11a", 30);
            gradeToCfRank.Add("5.11b", 34);
            gradeToCfRank.Add("5.11c", 37);
            gradeToCfRank.Add("5.11d", 39);
            gradeToCfRank.Add("5.12a", 42);
            gradeToCfRank.Add("5.12b", 46);
            gradeToCfRank.Add("5.12c", 50);
            gradeToCfRank.Add("5.12d", 54);
            gradeToCfRank.Add("5.13a", 58);
            gradeToCfRank.Add("5.13b", 66);
            gradeToCfRank.Add("5.13c", 70);
            gradeToCfRank.Add("5.13d", 72);
            gradeToCfRank.Add("5.14a", 82);
            gradeToCfRank.Add("5.14b", 84);
            gradeToCfRank.Add("5.14c", 88);
            gradeToCfRank.Add("5.15a", 90);
            gradeToCfRank.Add("5.15b", 92);

            gradeToCfRank.Add("4+", 0);
            gradeToCfRank.Add("5+", 0);
            gradeToCfRank.Add("6a", 3);
            gradeToCfRank.Add("6a+", 6);
            gradeToCfRank.Add("6b-", 8);
            gradeToCfRank.Add("6b", 10);
            gradeToCfRank.Add("6b+", 13);
            gradeToCfRank.Add("6c-", 16);
            gradeToCfRank.Add("6c", 19);
            gradeToCfRank.Add("6c+", 20);
            gradeToCfRank.Add("7a-", 22);
            gradeToCfRank.Add("7a", 25);
            gradeToCfRank.Add("7a+", 27);
            gradeToCfRank.Add("7b-", 30);
            gradeToCfRank.Add("7b", 32);
            gradeToCfRank.Add("7b+", 35);
            gradeToCfRank.Add("7c-", 37);
            gradeToCfRank.Add("7c", 40);
            gradeToCfRank.Add("7c+", 44);
            gradeToCfRank.Add("8a-", 48);
            gradeToCfRank.Add("8a", 52);
            gradeToCfRank.Add("8a+", 62);
            gradeToCfRank.Add("8b-", 68);
            gradeToCfRank.Add("8b", 74);
            gradeToCfRank.Add("8b+", 78);
            gradeToCfRank.Add("8c", 84);
            gradeToCfRank.Add("8c+", 86);
            gradeToCfRank.Add("9a-", 88);
            gradeToCfRank.Add("9a", 90);
            gradeToCfRank.Add("9a+", 92);
            gradeToCfRank.Add("9b", 94);
            gradeToCfRank.Add("9b+", 94);

            gradeToCfRank.Add("fo3", 22);
            gradeToCfRank.Add("fo4", 26);
            gradeToCfRank.Add("fo4+", 30);
            gradeToCfRank.Add("fo5", 32);
            gradeToCfRank.Add("fo5+", 36);
            gradeToCfRank.Add("fo6a", 40);
            gradeToCfRank.Add("fo6b", 44);
            gradeToCfRank.Add("fo6c", 48);
            gradeToCfRank.Add("fo7a", 52);
            gradeToCfRank.Add("fo7a+", 56);
            gradeToCfRank.Add("fo7b", 60);
            gradeToCfRank.Add("fo7b+", 64);
            gradeToCfRank.Add("fo7c", 68);
            gradeToCfRank.Add("fo7c+", 70);
            gradeToCfRank.Add("fo8a", 74);
            gradeToCfRank.Add("fo8a+", 78);
            gradeToCfRank.Add("fo8b", 80);
            gradeToCfRank.Add("fo8b+", 84);

            gradeToCfRank.Add("12", 12);
            gradeToCfRank.Add("13", 18);
            gradeToCfRank.Add("14", 22);
            gradeToCfRank.Add("15", 26);
            gradeToCfRank.Add("16", 30);
            gradeToCfRank.Add("17", 32);
            gradeToCfRank.Add("18", 36);
            gradeToCfRank.Add("19", 40);
            gradeToCfRank.Add("20", 44);
            gradeToCfRank.Add("21", 48);
            gradeToCfRank.Add("22", 52);
            gradeToCfRank.Add("23", 56);
            gradeToCfRank.Add("24", 60);
            gradeToCfRank.Add("25", 64);
            gradeToCfRank.Add("26", 68);
            gradeToCfRank.Add("27", 71);
            gradeToCfRank.Add("28", 72);
            gradeToCfRank.Add("29", 74);
            gradeToCfRank.Add("30", 78);
            gradeToCfRank.Add("31", 80);
            gradeToCfRank.Add("32", 84);
            gradeToCfRank.Add("33", 86);
            gradeToCfRank.Add("34", 90);
            gradeToCfRank.Add("35", 94);

            gradeToCfRank.Add("uiaaII", 0);
            gradeToCfRank.Add("uiaaIII-", 3);
            gradeToCfRank.Add("uiaaIII", 6);
            gradeToCfRank.Add("uiaaIII+", 9);
            gradeToCfRank.Add("uiaaIV-", 12);
            gradeToCfRank.Add("uiaaIV", 15);
            gradeToCfRank.Add("uiaaIV+", 18);
            gradeToCfRank.Add("uiaaV-", 21);
            gradeToCfRank.Add("uiaaV", 24);
            gradeToCfRank.Add("uiaaV+", 27);
            gradeToCfRank.Add("uiaaVI-", 30);
            gradeToCfRank.Add("uiaaVI", 33);
            gradeToCfRank.Add("uiaaVI+", 36);
            gradeToCfRank.Add("uiaaVII-", 39);
            gradeToCfRank.Add("uiaaVII", 42);
            gradeToCfRank.Add("uiaaVII+", 45);
            gradeToCfRank.Add("uiaaVIII-", 48);
            gradeToCfRank.Add("uiaaVIII", 51);
            gradeToCfRank.Add("uiaaVIII+", 54);
            gradeToCfRank.Add("uiaaIX-", 57);
            gradeToCfRank.Add("uiaaIX", 60);
            gradeToCfRank.Add("uiaaIX+", 63);
            gradeToCfRank.Add("uiaaX-", 66);
            gradeToCfRank.Add("uiaaX", 69);
            gradeToCfRank.Add("uiaaX+", 72);
            gradeToCfRank.Add("uiaaXI-", 75);
            gradeToCfRank.Add("uiaaXI", 78);
            gradeToCfRank.Add("uiaaXI+", 81);
            gradeToCfRank.Add("uiaaXII-", 84);
            gradeToCfRank.Add("uiaaXII", 87);
            gradeToCfRank.Add("uiaaXII+", 90);

            gradeToCfRank.Add("ukM", 2);
            gradeToCfRank.Add("ukD", 10);
            gradeToCfRank.Add("ukVD 3a", 18);
            gradeToCfRank.Add("ukVD", 20);
            gradeToCfRank.Add("ukHVD 3b", 24);
            gradeToCfRank.Add("ukHVD", 26);
            gradeToCfRank.Add("ukS 3c", 30);
            gradeToCfRank.Add("ukMS 4a", 32);
            gradeToCfRank.Add("ukS", 34);
            gradeToCfRank.Add("ukHS 4b", 36);
            gradeToCfRank.Add("ukHS", 38);
            gradeToCfRank.Add("ukVS 4b", 40);
            gradeToCfRank.Add("ukHSV 4c", 42);
            gradeToCfRank.Add("ukHSV 5a", 44);
            gradeToCfRank.Add("ukE1 5a", 46);
            gradeToCfRank.Add("ukE1 5b", 48);
            gradeToCfRank.Add("ukE2 5b", 50);
            gradeToCfRank.Add("ukE2 5c", 54);
            gradeToCfRank.Add("ukE3 5c", 56);
            gradeToCfRank.Add("ukE3 6a", 58);
            gradeToCfRank.Add("ukE4 6a", 60);
            gradeToCfRank.Add("ukE4 6b", 62);
            gradeToCfRank.Add("ukE5 6b", 64);
            gradeToCfRank.Add("ukE6 6b", 66);
            gradeToCfRank.Add("ukE6 6c", 68);
            gradeToCfRank.Add("ukE7 6c", 70);
            gradeToCfRank.Add("ukE7 7a", 72);
            gradeToCfRank.Add("ukE8 7a", 74);
            gradeToCfRank.Add("ukE8 7b", 76);
            gradeToCfRank.Add("ukE9 7b", 78);
            gradeToCfRank.Add("ukE10 7b", 80);
            gradeToCfRank.Add("ukE10 7c", 82);
            gradeToCfRank.Add("ukE11 7c", 84);
            gradeToCfRank.Add("ukE11 8a", 86);
            gradeToCfRank.Add("ukE11 8b", 88);
            gradeToCfRank.Add("ukE11 8c", 90);
            
            placeTypeToCategory = new Dictionary<CfType, PlaceCategory>();
            placeTypeToCategory.Add(CfType.Unknown, PlaceCategory.Unknown);
            placeTypeToCategory.Add(CfType.Country, PlaceCategory.Country);
            placeTypeToCategory.Add(CfType.Province, PlaceCategory.Area);
            placeTypeToCategory.Add(CfType.City, PlaceCategory.Area);
            placeTypeToCategory.Add(CfType.ClimbingArea, PlaceCategory.Area);
            placeTypeToCategory.Add(CfType.ClimbIndoor, PlaceCategory.Climb);
            placeTypeToCategory.Add(CfType.ClimbOutdoor, PlaceCategory.Climb);
            placeTypeToCategory.Add(CfType.CommercialIndoorClimbing, PlaceCategory.IndoorClimbing);
            placeTypeToCategory.Add(CfType.SportsCenter, PlaceCategory.IndoorClimbing);
            placeTypeToCategory.Add(CfType.PrivateIndoorClimbing, PlaceCategory.IndoorClimbing);
            placeTypeToCategory.Add(CfType.RockWall, PlaceCategory.OutdoorClimbing);
            placeTypeToCategory.Add(CfType.RockBoulder, PlaceCategory.OutdoorClimbing);
            placeTypeToCategory.Add(CfType.RockWaterSoloing, PlaceCategory.OutdoorClimbing);
            placeTypeToCategory.Add(CfType.AlpineWall, PlaceCategory.OutdoorClimbing);
            placeTypeToCategory.Add(CfType.Summit, PlaceCategory.OutdoorClimbing);
            placeTypeToCategory.Add(CfType.IceWall, PlaceCategory.OutdoorClimbing);
            placeTypeToCategory.Add(CfType.Food, PlaceCategory.Business);
            placeTypeToCategory.Add(CfType.Accommodation, PlaceCategory.Business);
            placeTypeToCategory.Add(CfType.Retailer, PlaceCategory.Business);
            placeTypeToCategory.Add(CfType.Guide, PlaceCategory.Business);
            placeTypeToCategory.Add(CfType.ClimbingCarPark, PlaceCategory.MeetingPoint);
            placeTypeToCategory.Add(CfType.ClimbingApproachStart, PlaceCategory.MeetingPoint);

            modActionTypeToPoints = new Dictionary<ModActionType, byte>();
            
            modActionTypeToPoints.Add(ModActionType.AreaAdd, Stgs.ModRepIncrementAddPlace);
            modActionTypeToPoints.Add(ModActionType.AreaEdit, Stgs.ModRepEdit);
            modActionTypeToPoints.Add(ModActionType.AreaVerifyEdit, Stgs.ModRepVerifyEdit);
            modActionTypeToPoints.Add(ModActionType.AreaEditVerified, Stgs.ModRepEditVerified);            
            modActionTypeToPoints.Add(ModActionType.AreaSetAvatar, Stgs.ModRepSetImage);
            modActionTypeToPoints.Add(ModActionType.AreaSetClimbingImage, Stgs.ModRepSetImage);
            modActionTypeToPoints.Add(ModActionType.AreaVerifyAvatar, Stgs.ModRepVerifyImage);
            modActionTypeToPoints.Add(ModActionType.AreaImageVerified, Stgs.ModRepImageVerified);
            modActionTypeToPoints.Add(ModActionType.AreaDelete, Stgs.ModRepDeletePlace);

            modActionTypeToPoints.Add(ModActionType.LocationIndoorAdd, Stgs.ModRepIncrementAddPlace);
            modActionTypeToPoints.Add(ModActionType.LocationIndoorEdit, Stgs.ModRepEdit);
            modActionTypeToPoints.Add(ModActionType.LocationIndoorVerifyEdit, Stgs.ModRepVerifyEdit);
            modActionTypeToPoints.Add(ModActionType.LocationIndoorEditVerified, Stgs.ModRepEditVerified);
            modActionTypeToPoints.Add(ModActionType.LocationIndoorSetAvatar, Stgs.ModRepSetImage);
            modActionTypeToPoints.Add(ModActionType.LocationIndoorSetLogo, Stgs.ModRepSetImage);
            modActionTypeToPoints.Add(ModActionType.LocationIndoorVerifyImage, Stgs.ModRepVerifyImage);
            modActionTypeToPoints.Add(ModActionType.LocationIndoorImageVerified, Stgs.ModRepImageVerified);
            modActionTypeToPoints.Add(ModActionType.LocationIndoorDelete, Stgs.ModRepDeletePlace);

            modActionTypeToPoints.Add(ModActionType.LocationOutdoorAdd, Stgs.ModRepIncrementAddPlace);
            modActionTypeToPoints.Add(ModActionType.LocationOutdoorEdit, Stgs.ModRepEdit);
            modActionTypeToPoints.Add(ModActionType.LocationOutdoorVerifyEdit, Stgs.ModRepVerifyEdit);
            modActionTypeToPoints.Add(ModActionType.LocationOutdoorEditVerified, Stgs.ModRepEditVerified);
            modActionTypeToPoints.Add(ModActionType.LocationOutdoorSetAvatar, Stgs.ModRepSetImage);
            modActionTypeToPoints.Add(ModActionType.LocationOutdoorVerifyAvatar, Stgs.ModRepVerifyImage);
            modActionTypeToPoints.Add(ModActionType.LocationOutdoorImageVerified, Stgs.ModRepImageVerified);
            modActionTypeToPoints.Add(ModActionType.LocationOutdoorDelete, Stgs.ModRepDeletePlace);

            modActionTypeToPoints.Add(ModActionType.ClimbAdd, Stgs.ModRepIncrementAddClimb);
            modActionTypeToPoints.Add(ModActionType.ClimbEdit, Stgs.ModRepEdit);
            modActionTypeToPoints.Add(ModActionType.ClimbVerifyEdit, Stgs.ModRepVerifyEdit);
            modActionTypeToPoints.Add(ModActionType.ClimbEditVerified, Stgs.ModRepEditVerified);
            modActionTypeToPoints.Add(ModActionType.ClimbSetAvatar, Stgs.ModRepSetImage);
            modActionTypeToPoints.Add(ModActionType.ClimbVerifyAvatar, Stgs.ModRepSetImage);
            modActionTypeToPoints.Add(ModActionType.ClimbDelete, Stgs.ModRepDeletePlace);
        }

        public static PlaceCategory ToPlaceCateogry(this CfType placeType) { return placeTypeToCategory[placeType]; }
        public static byte GetPoints(this ModActionType modActionType) { return modActionTypeToPoints[modActionType]; }
        public static byte GetGradeRank(this string grade) { return gradeToCfRank[grade]; }
        public static List<string> GetGrades() { return gradeToCfRank.Keys.ToList(); }

        public static bool IsLocation(this CfType type)
        {
            var typeID = (byte)type;
            return typeID > 9 && typeID < 60;
        }

        public static string ToFriendlyString(this CfType type) {
            if (type == CfType.City) return "City";
            if (type == CfType.ClimbingArea) return "Area";
            if (type == CfType.CommercialIndoorClimbing) return "Indoor climbing";
            if (type == CfType.SportsCenter) return "Sports center";
            if (type == CfType.PrivateIndoorClimbing) return "Private indoor";
            if (type == CfType.RockBoulder) return "Boulder";
            if (type == CfType.RockWall) return "Rock wall";
            if (type == CfType.Summit) return "Summit";
            return "unknown type"; 
        }

        public static string ToFriendlyVerboseString(this CfType type)
        {
            if (type == CfType.City) return "City";
            if (type == CfType.ClimbingArea) return "Climbing area";
            if (type == CfType.CommercialIndoorClimbing) return "Indoor climbing gym/wall";
            if (type == CfType.SportsCenter) return "Sports center";
            if (type == CfType.PrivateIndoorClimbing) return "Private indoor climbing";
            if (type == CfType.RockBoulder) return "Boulder";
            if (type == CfType.RockWall) return "Outdoor rock wall";
            if (type == CfType.Summit) return "Summit";
            return "unknown type";
        }

        public static string ToFriendlyString(this PostType type)
        {
            if (type == PostType.ContentAdd) return "added content";
            if (type == PostType.Opinion) return "left opinion";
            if (type == PostType.MediaOpinion) return "rated media";
            if (type == PostType.Visit) return "checked in";
            if (type == PostType.PartnerCall) return " partner call";
            if (type == PostType.Talk) return "shared a thought";
            if (type == PostType.Introduction) return "updated introduction";
            if (type == PostType.PersonalityMedia) return "updated personality";
            
            return "unknown type";
        }
    }
}
