using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;

namespace cf.Content.Search
{
    /// <summary>
    /// Declaration of the bits we need to display a search result
    /// </summary>
    public interface ISearchResultContent : IHasCountry
    {
        /// <summary>
        /// Main Text in the search result
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Bits of extra information to be displayed on the search results page or as a snip in the quick search drop down
        /// </summary>
        string Excerpt { get; set; }

        /// <summary>
        /// Url (SlugUrl) to redirect the user to which is the page that represents the object which the SearchEngineResult refers to
        /// </summary>
        string Url { get; set; }
    }
}
