using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace cf.SiteMapGenerator
{
    class Program
    {
        static int pagesCount;

        static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder(@"<?xml version=""1.0"" encoding=""utf-8"" ?><urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">");

            WriteUrlNode(sb, "/", DateTime.Now, "daily", "1.0");

            WriteUrlNode(sb, "/rock-climbing-social-network", DateTime.Now, "monthly", "1.0");
            WriteUrlNode(sb, "/search-for-rock-climbing-partners", DateTime.Now, "monthly", "1.0");
            WriteUrlNode(sb, "/world-rock-climbing-database", DateTime.Now, "monthly", "1.0");
            WriteUrlNode(sb, "/rock-climbing-search-engine", DateTime.Now, "weekly", "1.0");
            WriteUrlNode(sb, "/rock-climbing-glossary", DateTime.Now, "monthly", "0.8");

            //-- Countries
            WriteUrlNode(sb, "/rock-climbing-usa", DateTime.Now, "weekly", "1.0");
            WriteUrlNode(sb, "/rock-climbing-canada", DateTime.Now, "weekly", "1.0");
            WriteUrlNode(sb, "/rock-climbing-england", DateTime.Now, "weekly", "1.0");
            WriteUrlNode(sb, "/rock-climbing-australia", DateTime.Now, "weekly", "1.0");
            WriteUrlNode(sb, "/rock-climbing-germany", DateTime.Now, "weekly", "1.0");

            //-- Provinces
            WriteUrlNode(sb, "/rock-climbing-usa/california", DateTime.Now, "weekly", "1.0");
            
            //foreach (AreaTag tag in cf.GetAllAreaTags())
            //{
            //    WriteUrlNode(sb, tag.ClimbfindUrl, DateTime.Now.AddDays(-2), "weekly", "0.5");
            //}

            //List<PartnerCall> AllPartnerCalls = new CFController().GetAllPartnerCalls();
            //List<PartnerCall> UniqueUserPartnerCalls = (from c in AllPartnerCalls orderby c.MeetUpDateTime descending select c).Distinct(new PartnerCallUserComparer()).ToList();
            //List<int> PlaceIDsWithCalls = new CFController().GetPlaceIDsWithCalls();

            sb.Append("</urlset>");

            Console.WriteLine(string.Format("<p>Total pages count: {0}</p>", pagesCount));

            File.WriteAllText("Sitemap.xml", sb.ToString());

            Console.ReadLine();
        }
        
        private static void WriteUrlNode(StringBuilder sb, string location, DateTime lastModDate, string changeFrequency, string priorty)
        {
            if (!location.Contains("&"))
            {
                sb.AppendLine("<url>");
                sb.AppendFormat("<loc>http://www.climbfind.com{0}", location);
                sb.AppendLine("</loc>");
                sb.AppendFormat("<lastmod>{0}", lastModDate.ToString("yyyy-MM-dd"));
                sb.AppendLine("</lastmod>");
                sb.AppendFormat("<changefreq>{0}", changeFrequency);
                sb.AppendLine("</changefreq>");
                sb.AppendFormat("<priority>{0}", priorty);
                sb.AppendLine("</priority>");
                sb.AppendLine("</url>");
                pagesCount++;
            }
        }

    }
}
