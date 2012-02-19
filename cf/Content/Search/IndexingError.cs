using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Content.Search
{
    /// <summary>
    /// 
    /// </summary>
    public class IndexingError
    {
        public ILuceneSearchEngineEntry Entry { get; set; }
        public Exception Exception { get; set; }

        public IndexingError(ILuceneSearchEngineEntry entry, Exception exception)
        {
            Entry = entry;
            Exception = exception;
        }
    }
}
