using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Documents;

namespace cf.Content.Search
{
    public abstract class SearchEngineEntry : ILuceneSearchEngineEntry
    {
        /// <summary>
        /// String representation of a Guid ID field of Areas, Locations, Users (with the assumption that Guids never collide)
        /// </summary>
        /// <remarks>
        /// The point of the ID is to be used as a key so that we can remove / update single entries in the index
        /// </remarks>
        public string Key { get; set; }
        public string Url { get; set; }
        public byte CountryID { get; set; }
        public byte TypeID { get; set; }
        public string Title { get; set; }
        public string Excerpt { get; set; } 

        /// <summary>
        /// Base to document that should be called from child classes which adds search result information to the document
        /// </summary>
        /// <returns></returns>
        public virtual Lucene.Net.Documents.Document ToDocument()
        {
            var doc = new Document();

            var keyField = new Field("Key", Key, Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO);
            doc.Add(keyField);

            var urlField = new Field("Url", Url, Field.Store.YES, Field.Index.NO, Field.TermVector.NO);
            doc.Add(urlField);

            var countryIdField = new Field("Country", CountryID.ToString(), Field.Store.YES, Field.Index.NO, Field.TermVector.NO);
            doc.Add(countryIdField);

            var titleField = new Field("Title", Title, Field.Store.YES, Field.Index.NO, Field.TermVector.NO);
            doc.Add(titleField);

            var excerptField = new Field("Excerpt", Excerpt, Field.Store.YES, Field.Index.NO, Field.TermVector.NO);
            doc.Add(excerptField);

            return doc;
        }
    }
}
