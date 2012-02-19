using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using Lucene.Net.Store;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;

namespace cf.Content.Search
{
    public class CfLuceneIndexSearcher
    {
        private readonly Directory _directory;
        private readonly Analyzer _analyzer;
        private readonly FullTextSearchEngineSettings _settings;
        
        public CfLuceneIndexSearcher(Directory directory)
        {
            _directory = directory;
            _settings = new FullTextSearchEngineSettings();
            _analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
        }

        public CfLuceneIndexSearcher(Directory directory, Analyzer analyzer, FullTextSearchEngineSettings settings)
        {
            _directory = directory;
            _analyzer = analyzer;
            _settings = settings;
        }

        private IndexSearcher Searcher { get { return new IndexSearcher(_directory, true); 
        //    return DoWriterAction(writer => new IndexSearcher(writer.GetReader())); 
        } }

        private QueryParser BuildQueryParser()
        {
            var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, "Name", _analyzer);
            parser.SetDefaultOperator(QueryParser.Operator.AND);
            return parser;
        }

        public IEnumerable<SearchEngineResult> Search(string queryString, int max)
        {
            var list = new List<SearchEngineResult>();
            if (String.IsNullOrEmpty(queryString)) return list;
            QueryParser parser = BuildQueryParser();
            Query nameQuery = parser.Parse(queryString + "*");

            string queryStringMerged = String.Format("({0}) OR ({1}) OR ({2}) OR ({3})",
                                                     nameQuery,
                                                     nameQuery.ToString().Replace("Name", "NameShort"),
                                                     nameQuery.ToString().Replace("Name", "SearchSupportString"),
                                                     nameQuery.ToString().Replace("Name", "Description"));

            Query query = parser.Parse(queryStringMerged);

            return PerformQuery(list, query, max);
        }

        private IEnumerable<SearchEngineResult> PerformQuery(ICollection<SearchEngineResult> list, Query queryOrig, int max)
        {
            //Query isValidPlaceQuery = new TermQuery(new Term(ItemNotUserDeleted, true.ToString()));

            var query = new BooleanQuery();
            query.Add(queryOrig, BooleanClause.Occur.MUST);
            //query.Add(isPublishedQuery, BooleanClause.Occur.MUST);

            IndexSearcher searcher = Searcher;
            TopDocs hits = searcher.Search(query, max);
            int length = hits.scoreDocs.Length;
            int resultsAdded = 0;
            float minScore = _settings.MinimumScore;
            float scoreNorm = 1.0f / hits.GetMaxScore();
            for (int i = 0; i < length && resultsAdded < max; i++)
            {
                float score = hits.scoreDocs[i].score * scoreNorm;
                SearchEngineResult result = CreateSearchResult(searcher.Doc(hits.scoreDocs[i].doc), score);
                //if (idToFilter != result.EntryId && result.Score > minScore)
                //{
                list.Add(result);
                resultsAdded++;
                //}

            }
            return list;
        }

        protected virtual SearchEngineResult CreateSearchResult(Document doc, float score)
        {
            var result = new SearchEngineResult
            {
                ID = doc.Get("Key"),
                Title = doc.Get("Title"),
                Url = doc.Get("Url"),
                TypeID = byte.Parse(doc.Get("Type")),
                Excerpt = doc.Get("Excerpt"),
                Score = score
            };

            //-- This is out here incase later we accommodate areas that belong to more than one country
            byte countryID = 0;
            if (byte.TryParse(doc.Get("Country"), out countryID))
            {
                result.CountryID = countryID;
                result.Flag = cf.Caching.AppLookups.CountryFlag(result.CountryID);
            }

            return result;
        }

        public void Dispose()
        {
            _directory.Close();
        }
    }
}
