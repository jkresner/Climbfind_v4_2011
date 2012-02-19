using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Documents;

namespace cf.Content.Search
{
    /// <summary>
    /// Declaration that we can transform the entry into a document and that any search entry in our index must contain 
    /// all the content required to return / transform it on the way out into a search result which we can display
    /// </summary>
    public interface ILuceneSearchEngineEntry : ISearchResultContent
    {
        /// <summary>
        /// Unique key for the item in the index (basically the GUID Id property of the object we're indexing)
        /// </summary>
        string Key { get; set; }
        
        /// <summary>
        /// The mechanism to turn out typed SearchEngineEntry into a lucene document
        /// </summary>
        /// <returns></returns>
        Document ToDocument();        
    }
}
