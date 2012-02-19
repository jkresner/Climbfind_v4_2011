using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Instrumentation
{
	public static class ExceptionFilter
	{
		public static bool ShouldRecord(this Exception ex, string relativeUrl)
		{
			if (System.Web.HttpContext.Current.Request.Browser.Crawler) { return false; }
			
			var url = relativeUrl.ToLower();
			if (url.EndsWith("/")) { url = url.Substring(0, url.Length - 1); }

			if (SpecialUrls.IsInternetBullshit(url)) { return false; }
			if (SpecialUrls.Instance.UrlsPostOnly.Contains(url)) { return false; }
			if (SpecialUrls.HasBeenRemoved(url)) { return false; }
			if (SpecialUrls.Instance.UrlsGone.Contains(url)) { return false; }
			if (SpecialUrls.Instance.PermanentlyMoved.Keys.Contains(url)) { return false; }

			string p = ex.Message;
			
			//-- search engine photo not found
			//if (p.Contains("Photo not found")) { return false; }

			//-- Stop recording 301
			if (p.Contains("Thread was being aborted")) { return false; }
			if (p.Contains("_vti_bin/owssvr.dll")) { return false; } 

			return true;
		}
	}

	public sealed class SpecialUrls
	{
		//-- Singleton Stuff
		private static readonly SpecialUrls instance = new SpecialUrls();
		public static SpecialUrls Instance
		{
			get { return instance; }
		}

		public List<string> UrlsGone;
		public List<string> UrlsPostOnly;
		public Dictionary<string, string> PermanentlyMoved = new Dictionary<string, string>();
		public Dictionary<string, string> PermanentlyMovedPattern = new Dictionary<string, string>();

		public static bool Cf3Redirect(string url)
		{
			if (url.Contains("partner-widget")) { return true; }
			if (url.Contains("/media/usersmedia/a9646cc3-18cb-4a62-8402-5263ba8b3476")) { return true; }
			return false;
		}

		public static bool IsInternetBullshit(string url)
		{
			if (System.Web.HttpContext.Current.Request.HttpMethod == "HEAD" ||
				System.Web.HttpContext.Current.Request.HttpMethod == "OPTIONS") { return true; }

			if (url.Contains(".aspx")) { return true; } //-- old school asp .net
			if (url.Contains(".asp")) { return true; } //-- older school asp
			if (url.Contains("msoffice/cltreq.asp")) { return true; } //-- MS Office toolbar shit
			if (url.Contains("php")) { return true; } //-- php apps
			if (url.Contains("jmx-console")) { return true; } //-- http://community.jboss.org/wiki/JMXConsole
			if (url.Contains("verify-compliance_page")) { return true; } //-- http://www.webmasterworld.com/search_engine_spiders/3859463.htm
			if (url.Contains("verify-notifyuser")) { return true; }
			if (url.Contains("notified-notifyuser")) { return true; }
			if (url.Contains("awstats.pl")) { return true; }
			if (url.Contains("labels.rdf")) { return true; }
			if (url.Contains("readme")) { return true; }
			if (url.Contains("cgi-bin")) { return true; }
			if (url.Contains("apple")) { return true; }
			if (url.Contains("images/hypersphere-2010.png")) { return true; } 
			
			return false;
		}

		public static bool HasBeenRemoved(string url)
		{
			if (Instance.UrlsGone.Contains(url)) { return true; }
			if (url.Contains("ads/record")) { return true; }
			if (url.Contains("all-regular-climbers")) { return true; }
			if (url.Contains("blogfiles")) { return true; }
			if (url.Contains("cffeed")) { return true; }
			if (url.Contains("climbing-around")) { return true; }
			if (url.Contains("climber-profile")) { return true; }
			if (url.Contains("climberprofiles/")) { return true; }
			if (url.Contains("feature-article")) { return true; }
			if (url.Contains("images/ads")) { return true; }
			if (url.Contains("images/site-partners")) { return true; }
			if (url.Contains("login/to")) { return true; } //-- Leave the 'L' off because it some in both small and big caps
			if (url.Contains("login/please")) { return true; } //-- Leave the 'L' off because it some in both small and big caps
			if (url.Contains("media/detail")) { return true; }
			if (url.Contains("media/editoutdoorlocationpictures")) { return true; }
			if (url.Contains("media/editoutdoorcragpictures")) { return true; }
			if (url.Contains("media/addplaceyoutube")) { return true; }
			if (url.Contains("media/usersmedia")) { return true; }
			if (url.Contains("media/addcragyoutube")) { return true; }
			if (url.Contains("moderate/editindoorplace")) { return true; }
			if (url.Contains("moderate/addoutdoorlocation")) { return true; }
			if (url.Contains("moderate/editoutdoorlocation")) { return true; }
			if (url.Contains("moderate/addoutdoorcrag")) { return true; }
			if (url.Contains("moderate/editplacemap")) { return true; }
			if (url.Contains("partnercalls/reply")) { return true; }
			if (url.Contains("partner-widget")) { return true; }
			if (url.Contains("partners-rss")) { return true; }
			if (url.Contains("people-looking-for-climbing-partners")) { return true; }
			if (url.Contains("indoor-rock-climbing-gyms")) { return true; }
			if (url.Contains("places/outdoor-rock-climbing")) { return true; }
			if (url.Contains("little-rock-city-tennessee")) { return true; }
			if (url.Contains("/vam")) { return true; }
			if (url.Contains("rock-climbing-clubs")) { return true; }
			if (url.Contains("thumb.ashx")) { return true; }
			if (url.Contains("write-message")) { return true; }
			
			return false;
		}

		private SpecialUrls()
		{
			UrlsPostOnly = new List<string>() {
				"/search-locations",
				"/search-places",
				"/places/searchareafornewarea?Length=6",
				"/posts/newcomment",
				"/posts/deletecomment",
				"/profiles/savepic",
				"/profiles/savepersonalitymedia",
				"/profiles/updateplacepreferences",
				"/visits/createhistorical",
				"/messages/append",
				"/media/opinionnew",
				"/visits/updatecomment",
			};
			
			//-- Every 3 months clear out these links and start again..?
			UrlsGone = new List<string>()
			{
				"/post-an-ad-for-rock-climbing-partners",
				"/ads",
				"/places",
				"/places/indoor",
				"/places/outdoor",
				"/places/outdoor-rock-climbing",
				"/calendar",
				"/clubs",
				"/clubs/about",
				"/climbers-noticeboard",
				"/climbing-jobs",
				"/competitions",
				"/css/cf.css",
				"/css/cf2.55.css",
				"/css/cf2.56.css",
				"/css/cf3.0a.css",
				"/css/cf3.0b.css",
				"/css/cf3.01.css",
				"/css/cf3.01.css",
				"/css/cf3.02.css",
				"/css/cf3.53.css",
				"/css/cf.boxy.css",
				"/css/reset-fonts.css",
				"/feedback",
				"/groups",
				"/groups/about",
				"/groups/new",
				"/groups/search",
				"/home/cf3betabrief",
				"/home/cfmail",
				"/home/preferences",
				"/home/indexexperimental",
				"/home/climbingfeedrss",
				"/home/inbox",
				"/home/sent",
				"/home/promote",
				"/home/webformerror",
				"/home/forgottenpassword",
				"/posts/communityfeed",
				"/join",
				"/js/cf3b.js",
				"/js/cf3a.js",
				"/js/cf3.5.js",
				"/js/cf3.1home.js",
				"/js/cf3.5home.js",
				"/js/CF3.51Home.js",
				"/js/cf3.52.js",
				"/js/climbfind.js",
				"/js/jquery.boxy.js",
				"/js/jquery-1.2.6.js",
				"/js/jquery-1.3.1.min.js",
				"/js/jquery.cluetip.js",
				"/js/jquery-droppy-menu.js",
				"/js/jquery.dimensions.js",
				"/js/jquery.hoverintent.js",
				"/js/jquery.ajaxqueue.js",
				"/js/jquery.autocomplete.min.js",
				"/news",
				"/news/mainfeed",
				"/news/featurearticles",
				"/news/castle_2008_xmas_party",
				"/news/the_castle_presents_niel_gresham_kitty_wallace_jack_griffiths_2008_12_13",
				"/news/sibl_2008_indoor_bouldering_competition",
				"/news/east_meets_west_bouldering_competition_london_2008_round_1",
				"/messages/create",
				"/media",
				"/media/submitmovie",
				"/moderate/addindoorplace",
				"/partnercalls/create",
				"/partnercalls/mycalls",
				"/partnercalls/search",
				"/partnercalls/notifications",
				"/places/regularnights",
				"/places/allplaceregulars",
				"/post-a-climbing-club-trip-meet",
				"/products",
				"/products/guided_outdoor_rock_climbing",
				"/products/books",
				"/products/dvds",
				"/products/private_climbing_instruction",
				"/products/booktopiaadclick?bookname=advanced%20rock%20climbing%20:%20a%20step-by-step%20guide%20to%20improving%20skills&isbn=1844765369",
				"/products/booktopiaadclick?bookname=bouldering%20colorado&isbn=0762736380",
				"/products/booktopiaadclick?bookname=crack%20climbing!&isbn=0762745916",
				"/products/booktopiaadclick?bookname=climber's%20guide%20to%20devil's%20lake&isbn=0299228541",
				"/products/booktopiaadclick?bookname=everyday%20masculinities%20and%20extreme%20sport%20:%20male%20identity%20and%20rock%20climbing&isbn=184520137x",
				"/products/booktopiaadclick?bookname=first%20ascent&isbn=1844035964",
				"/products/booktopiaadclick?bookname=girl%20on%20the%20rocks&isbn=0762745185",
				"/products/booktopiaadclick?bookname=mountaineering&isbn=0713686928",
				"/products/booktopiaadclick?bookname=the%20complete%20guide%20to%20climbing%20and%20mountaineering&isbn=0715328441",
				"/products/booktopiaadclick?bookname=rock%20climbing%20calendar&isbn=1595437584",
				"/popcalendar2008/css/classic.css",
				"/popcalendar2008/popcalendarfunctionsajaxnet.js",
				"/post-an-add-for-rock-climbing-partners",
				"/roadtrip/sponsors",
				"/trips",
				"/glossary/gradeconverter",
				"/climbing-grade-comparison-chart-converter",
				"/images/rock-climbing.jpg",
				"/images/load.gif",
				"/images/news/roadtripfull.jpg"
			};

			PermanentlyMoved.Add("/blog", "http://heroes.climbfind.com/");
			PermanentlyMoved.Add("/pagenotfound", "/page-not-found.htm");
			PermanentlyMoved.Add("/find-rock-climbing-partners", "/search-for-rock-climbing-partners");
			PermanentlyMoved.Add("/about", "/rock-climbing-social-network");
			PermanentlyMoved.Add("/search", "/rock-climbing-search-engine");
			PermanentlyMoved.Add("/glossary", "/rock-climbing-glossary");
			PermanentlyMoved.Add("/world-climbing-map", "http://cf3.climbfind.com/world-climbing-map");
			PermanentlyMoved.Add("/2008-uk-roadtrip", "http://cf3.climbfind.com/2008-UK-Roadtrip");
			PermanentlyMoved.Add("/2009-climbfind-usa-canada-road-trip", "http://cf3.climbfind.com/2009-Climbfind-USA-Canada-Road-Trip");

			PermanentlyMoved.Add("/images/ui/outpin.bmp", "http://cf3.climbfind.com/images/UI/inpin.bmp");
			PermanentlyMoved.Add("/images/ui/inpin.bmp", "http://cf3.climbfind.com/images/UI/inpin.bmp");
			PermanentlyMoved.Add("/images/climbing-partner.png", "http://cf3.climbfind.com/images/climbing-partner.png");
			PermanentlyMoved.Add("/images/banners/cfbanner486x100.jpg", "http://cf3.climbfind.com/images/banners/CFBanner486x100.jpg");
			PermanentlyMoved.Add("/images/site-partners/climbfind-logo-135x50.png", "http://cf3.climbfind.com/images/site-partners/climbfind-logo-135x50.png");
			PermanentlyMoved.Add("/images/site-partners/climbfind-logo-189x70.png", "http://cf3.climbfind.com/images/site-partners/climbfind-logo-189x70.png");
			PermanentlyMoved.Add("/images/site-partners/climbfind-logo.png", "http://cf3.climbfind.com/images/site-partners/climbfind-logo.png");
			PermanentlyMoved.Add("/images/site-partners/mec-climbfind-400x100.png", "http://cf3.climbfind.com/images/site-partners/MEC-Climbfind-400x100.png");
			PermanentlyMoved.Add("/images/wikicflogo150.png", "http://cf3.climbfind.com/images/WikiCFLogo150.png");

			PermanentlyMoved.Add("/rock-climbing-trinidad-and-tobago/san-juan/laventille", "/rock-climbing-trinidad-and-tobago/san-juan-laventille");
			

			//PermanentlyMoved.Add("/2009-Climbfind-USA-Canada-Road-Trip", "http://cf3.climbfind.com/2009-Climbfind-USA-Canada-Road-Trip");
			//PermanentlyMoved.Add("/pagead/atf.js", "http://googleads.g.doubleclick.net/pagead/atf.js");
			//PermanentlyMoved.Add("/pagead/osd.js", "http://googleads.g.doubleclick.net/pagead/osd.js");
			//PermanentlyMoved.Add("/pagead/render_ads.js", "http://pagead2.googlesyndication.com/pagead/render_ads.js");
			//PermanentlyMoved.Add("/pagead/test_domain.js", "http://googleads.g.doubleclick.net/pagead/test_domain.js");
			//PermanentlyMoved.Add("/pagead/expansion_embed.js", "http://pagead2.googlesyndication.com/pagead/expansion_embed.js");
			//PermanentlyMoved.Add("/__utm.gif", "http://pagead2.googlesyndication.com/pagead/expansion_embed.js");
		}
	}
}
