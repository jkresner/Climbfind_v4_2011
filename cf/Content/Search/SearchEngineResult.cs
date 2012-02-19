using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;

namespace cf.Content.Search
{
    public class SearchEngineResult : ISearchResultContent
    {
        /// <summary>
        /// id of the object the result represents
        /// </summary>
        public string ID { get; set; }
        
        /// <summary>
        /// Main Text in the search result
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Bits of extra information to be displayed on the search results page or as a snip in the quick search drop down
        /// </summary>
        public string Excerpt { get; set; }

        /// <summary>
        /// Url (SlugUrl) to redirect the user to which is the page that represents the object which the SearchEngineResult refers to
        /// </summary>
        public string Url { get; set; }
        
        /// <summary>
        /// Flag string to show the country the object has
        /// </summary>
        public byte CountryID { get; set; }

        /// <summary>
        /// Flag string to show the country the object has
        /// </summary>
        public string Flag { get; set; }

        /// <summary>
        /// Result score (used to order the result)
        /// </summary>
        public float Score { get; set; }

        /// <summary>
        /// Not sure yet if this is going to be useful
        /// </summary>
        public byte TypeID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SearchEngineResult() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="countryID"></param>
        /// <param name="excerpt"></param>
        /// <param name="url"></param>
        public SearchEngineResult(string title, byte countryID, string excerpt, string url)
        {
            Title = title;
            CountryID = countryID;
            Excerpt = excerpt;
            Url = url;
            Flag = "null.gif";
        }

        public List<SearchEngineResult> AsSingleInList()
        {
            return new List<SearchEngineResult>() { this };
        }
    }
}
