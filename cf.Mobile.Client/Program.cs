using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Services;
using System.Configuration;
using System.Data.Services.Client;
using cf.Entities;
using System.Net;
using System.Web;
using NetFrameworkExtensions;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.ServiceModel.Description;
using System.ServiceModel;
using Microsoft.IdentityModel.Protocols.WSTrust.Bindings;
using System.IdentityModel.Tokens;
using cf.Dtos.Mobile.V1;
using cf.Content.Search;
using cf.Services;

namespace cf.Mobile.Client
{
    partial class Program
    {
        static void Main(string[] args)
        {
            TestPushAlerts();
            //Initialize();

            //var p = GetResultFromMobileService<List<LoggedClimbDetailDto>>("/get-logs-for-user/a9646cc318cb4a6284025263ba8b3476");
            
            //Console.WriteLine("-- GeoContext post for user -------------------");
            //var r13 = GetResultFromMobileService<List<PostDto>>("/feed/geocontext");
            //var r13 = GetResultFromMobileService<List<PostDto>>("/feed/preference");
            //var r13 = GetResultFromMobileService<List<PostDto>>("/feed/everywhere");
            //foreach (var p in r13) { Console.WriteLine("[{0}:{1}][{2}] {3} {4}",p.Type , p.ID, p.PlaceName, p.Meta, p.Comment); }

            //var p = GetResultFromMobileService<PostDto>("/feed/post/dd6b66484b294ac78d1636e67b6867a1");
            //Console.WriteLine("[{0}:{1}][{2}] {3} {4}", p.Type, p.ID, p.PlaceName, p.Meta, p.Comment); 
            
            //Console.WriteLine("-- Latest post for user -------------------");
            //var r13 = GetResultFromMobileService<List<PostDto>>("/get-latest-feed-posts");
            //foreach (var p in r13) { Console.WriteLine("[{0}] {1} {2}", p.ID, p.PlaceName, p.Content); }
            
            //var results = GetResultFromSearcService<List<SearchEngineResult>>("/term/spot");  
            //var iR = 0; foreach (var l in results) { Console.WriteLine("[{0}] {1}, {2}", iR++, l.Title, l.TypeID); }

            //var results = GetResultFromSearcService<List<SearchEngineResult>>("/term/spot");  
            //var iR = 0; foreach (var l in results) { Console.WriteLine("[{0}] {1}, {2}", iR++, l.Title, l.TypeID); }
            
            //var results = GetResultFromSearcService<List<SearchEngineResult>>("/term/spot");  
            //var iR = 0; foreach (var l in results) { Console.WriteLine("[{0}] {1}, {2}", iR++, l.Title, l.TypeID); }

            //var results2 = GetResultFromMobileService<ClimbIndoorDetailDto>("/update-climb-grade/68da04e615124221b473d540f82b304a/5.10");
            //Console.WriteLine("Grade for [{0}] updated to {1}",  results2.ID, results2.Grade);
         
            //var results = GetResultFromSearcService<List<SearchEngineResult>>("/term/spot");  
            //var iR = 0; foreach (var l in results) { Console.WriteLine("[{0}] {1}, {2}", iR++, l.Title, l.TypeID); }

            //Console.WriteLine("-- Nearest Locations --------------------");
            //var results1 = GetResultFromMobileService<List<LocationResultDto>>("/nearest-locations");
            //var iL1 = 0; foreach (var l in results1) { Console.WriteLine("[{0}] {1}m away, {2} ratecount {3}", iL1++, l.Distance, l.Name, l.RatingCount); }

            //Console.WriteLine("-- San Fran Locations --------------------");
            //var results2 = GetResultFromMobileService<AreaDetailDto>("/get-area/7e0723409d0b4b418f1c053934677ba0");
            //var iL2 = 0; foreach (var l in results2.Locations) { Console.WriteLine("[{0}] {1}m away, {2} ratecount {3}", iL2++, l.Distance, l.Name, l.RatingCount); }
         
            //Console.WriteLine("-- Climb at PG San Fran -------------------");

            //var results3 = GetResultFromMobileService<ClimbListDto>("/current-climbs/be0a7b1e2eb64418b61eac7656cc95ff");
            //var results3 = GetResultFromMobileService<ClimbListDto>("/current-climbs/1af79f00cb1a4a78b32250fb5b18127c");
            //var iL3 = 0; foreach (var l in results3.Sections) { Console.WriteLine("[{0}] {1} {2}", iL3++, l.ID, l.Name); }
            //iL3 = 0; foreach (var l in results3.Climbs) { Console.WriteLine("[{0}] {1} {2}", iL3++, l.Grade, l.Name); }
         
            //Console.WriteLine("-- Climb at PG Sunnyvale -------------------");
            //var results3 = GetResultFromMobileService<ClimbListDto>("/current-climbs/6a06abfab7964ab49b41011e195f2331");
            //var iL3 = 0; foreach (var l in results3.Sections) { Console.WriteLine("[{0}] {1} {2}", iL3++, l.ID, l.Name); }
            //iL3 = 0; foreach (var l in results3.Climbs) { Console.WriteLine("[{0}] {1} {2}", iL3++, l.Grade, l.Name); }   

            //Console.WriteLine("-- An indoor climb ------------------------");
            //var results4 = GetResultFromMobileService<ClimbIndoorDetailDto>("/climb/92d682a18fa04cd38fbca76b47cae923");
            //Console.WriteLine("[{0}] {1} {2} {3}", results4.ID, results4.Grade, results4.Name, results4.Mark); 

            //Console.WriteLine("-- An outdoor climb ------------------------");
            //var r5 = GetResultFromMobileService<ClimbOutdoorDetailDto>("/climb/5ceea6a233d944b197b5ed36903594fb");
            //Console.WriteLine("[{0}] {1} {2} {3}", r5.ID, r5.Grade, r5.Name, r5.DescWhere);

            //Console.WriteLine("-- A visit w climbs & media ----------------");
            //var r6 = GetResultFromMobileService<VisitDto>("/get-visit/ac13d1902d1c4335b5723b40de7dac6e");
            //PrintVisit(r6);

            //Console.WriteLine("-- My history ------------------------------");
            //var r7 = GetResultFromMobileService<List<VisitDto>>("/my-history");
            //foreach (var v in r7) { PrintVisit(v); }

            //Console.WriteLine("-- Recent visits at PG San Fran ------------");
            //var r8 = GetResultFromMobileService<List<VisitDto>>("/recent-visits/1af79f00cb1a4a78b32250fb5b18127c");
            //foreach (var v in r8) { PrintVisit(v); }


            //Console.WriteLine("-- Recent visits at PG San Fran ------------");
            //var r9 = GetResultFromMobileService<List<VisitDto>>("/recent-visits/1af79f00cb1a4a78b32250fb5b18127c");
            //foreach (var v in r9) { PrintVisit(v); }

            //Console.WriteLine("-- Check in to PG San Fran -----------------");
            //check-in/{loc}/{isPrivate}/{getAlerts}?comment={comment
            //var r10 = GetResultFromMobileService<VisitDto>("/check-in/1af79f00cb1a4a78b32250fb5b18127c/n/y?comment=This is a test checking");
            //PrintVisit(r10);

            //Console.WriteLine("-- Check out of check in -------------------");
            //check-in/{loc}/{isPrivate}/{getAlerts}?comment={comment
            //var r11 = GetResultFromMobileService<VisitDto>("/check-out/"+r10.ID);
            //PrintVisit(r11);

            //Console.WriteLine("-- Log Climb -------------------------------");
            //log-climb/{ciID}/{climbID}/{outcome}/{experience}/{gradeOpinion}/{rating}?comment={comment}

            //var r12 = GetResultFromMobileService<LoggedClimbDetailDto>("/log-climb/1320968717521/1a9e65bf5cb449469ea1c14594652f48/17/25/19/4?comment=that was an amazing testing climb from our iphone app");
            //Console.WriteLine("[{0}] {1} {2} {3}", r12.ID, r12.Name, r12.Outcome, r12.Experience);

            //var r12 = GetResultFromMobileService<LoggedClimbDetailDto>("/log-climb-update/ac43c4d632294f06b67c49c7eaf3e7bd/17/25/19/4?comment=that was an amazing updated testing climb from our iphone app");
            //Console.WriteLine("[{0}] {1} {2} {3}", r12.ID, r12.Name, r12.Outcome, r12.Experience);

            //var r12 = GetResultFromMobileService<LoggedClimbDetailDto>("/log-climb/1320964771209/1a9e65bf5cb449469ea1c14594652f48/17/25/19/4?comment=that was an amazing testing climb from our iphone app");
            //Console.WriteLine("[{0}] {1} {2} {3}", r12.ID, r12.Name, r12.Outcome, r12.Experience);
            
            //var r12b = GetResultFromMobileService<VisitLoggedClimbDto>("/log-climb/" + r10.ID + "/729b08319f0d4cd28989f24a7702ed17/17/25/19/4?comment=that was an amazing testing climb from our iphone app");

            //Console.WriteLine("-- Talk to Planet Granite SF ---------------");
            //var r14 = GetResultFromMobileService<PostDto>("/talk/1af79f00cb1a4a78b32250fb5b18127c?comment=Yeah cool");
            //Console.WriteLine("[{0}] {1} {2}", r14.ID, r14.PlaceName, r14.Content);

            //Console.WriteLine("-- Comment on talk post --------------------");
            //var r15 = GetResultFromMobileService<PostCommentDto>("/comment/" + r14.ID + "?comment=Here is a comment");
            //Console.WriteLine("[{0}] '{1}'", r15.By, r15.Msg);

            //Console.WriteLine("--- Best media for PG SF ---------------------");
            //var r16 = GetResultFromMobileService<List<MediaDto>>("/highest-rated-media/1af79f00cb1a4a78b32250fb5b18127c");
            //foreach (var m in r16) { Console.WriteLine("{0} by {1}, {2} {3} {4} {5}", m.Rating, m.RatingCount, m.Title, m.By, m.ByID, m.ByPic); }

            //Console.WriteLine("--- Recent media for PG SF ---------------------");
            //var r17 = GetResultFromMobileService<List<MediaDto>>("/recently-submitted-media/1af79f00cb1a4a78b32250fb5b18127c");
            //foreach (var m in r17) { Console.WriteLine("{0} {1} by {2} {3} {4}", m.Added, m.Title, m.By, m.ByID, m.ByPic); }

            //Console.WriteLine("--- Latest opinions on PG SF -------------------");
            //var r18 = GetResultFromMobileService<List<OpinionDto>>("/latest-opinions/1af79f00cb1a4a78b32250fb5b18127c");
            //foreach (var m in r18) { Console.WriteLine("{0} {1} {2} {3}", m.Utc, m.By, m.Rating, m.Comment); }

            //Console.WriteLine("Choose Index: ");
            //int index = byte.Parse(Console.ReadLine());

            //var selectedLoc = results[index];
            //Console.WriteLine("Selected area: " + selectedLoc.Title);

            //if (selectedLoc.TypeID == 3 || selectedLoc.TypeID == 7)
            //{
            //    var locs = GetResultFromMobileService<List<LocationResult>>("/locations-of-area/" + selectedLoc.ID.ToString());
            //    var iL = 0; foreach (var l in locs) { Console.WriteLine("[{0}] {1}, {2}", iL++, l.Distance, l.Name); }
            //}

            //var data = new cf.Odata.cfEntitiesData(new Uri(odataSvcUrl));
            //string relativeGetClimbsCall = string.Format("/Climbs?$filter=LocationID eq guid'{0}'", selectedLoc.ID);
            //List<Climb> climbs = odataSvc.Execute<Climb>(new Uri(relativeGetClimbsCall, UriKind.Relative)).ToList();
            //var iC = 0; foreach (var c in climbs) { Console.WriteLine("[{0}], {1} {2}", iC++, c.Name, c.Grade); }

            //Console.WriteLine("----------------------");
            //Console.WriteLine("{0} Recent Checkins: ", selectedLoc.Name);
            //var recentCheckIns = GetResultFromMobileService<List<CheckInDto>>("/recent-checkins/" + selectedLoc.ID.ToString());
            //foreach (var c in recentCheckIns) { Console.WriteLine("{0} by {1} {2} {3}", c.DateTime, c.By, c.ByID, c.ByPic); }
          
            //Console.WriteLine("Type ac:add a climb, lp:add location photo, uc:update a climb, pc:upload pic of a climb, mc:add photo media of climb ci: check in");
            //var action = Console.ReadLine();
            //if (action == "ac") { AddClimbs(selectedLoc); }
            //else if (action == "lp") { UploadLocationPhotoMedia(selectedLoc); }
            //else if (action == "uc") { UpdateClimb(selectedLoc, climbs); }
            //else if (action == "pc") { UploadClimbPic(selectedLoc, climbs); }
            //else if (action == "mc") { AddClimbPhotoMedia(selectedLoc, climbs); }
            //else if (action == "ci") { CheckIn(selectedLoc); }
            //else { Console.WriteLine("You didn't type in the right thing... We're all done"); }

            Console.WriteLine("Program finished");
            Console.ReadLine();
        }

        private static void TestPushAlerts()
        {
            string alias = "jkresner@yahoo.com.au";
            string clientTypeID = "iphone-pg";
       
            //var alrSvc = new AlertsService();
            AlertsService.MobilePush_MessageAlert("jkresner", alias, clientTypeID);
            //AlertsService.MobilePush_CommentAlert(new Guid("be929473-f273-4745-b3b8-b203974cb802"), "Charlene", alias, clientTypeID);
            //AlertsService.MobilePush_PartnerCallAlert(new Guid("1af79f00-cb1a-4a78-b322-50fb5b18127c"), "Planet Granite San Francisco", 10, alias, clientTypeID);
        }

        private static void PrintVisit(VisitDto v)
        {
            Console.WriteLine("[{0}] [{1}||{2}] {3} {4}", v.ID, v.Utc, v.UtcOut, v.LocName, v.By);
            foreach (var c in v.Climbs) { Console.WriteLine("[{0}] {1} {2}", c.Name, c.Outcome, c.Experience); }
            foreach (var c in v.Media) { Console.WriteLine("[{0}] {1}", c.ID, c.ThumbUrl); }
        }
    }
}
