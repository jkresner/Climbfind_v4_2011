using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Search;
using Lucene.Net.Index;
using Lucene.Net.Store;
using cf.Instrumentation;
using Lucene.Net.Analysis;

namespace cf.Content.Search
{
    public class CfLuceneIndexBuilder
    {
        private readonly Directory _directory;
        private readonly Analyzer _analyzer;
        private readonly FullTextSearchEngineSettings _settings;
        
        public CfLuceneIndexBuilder(Directory directory)
        {
            _directory = directory;
            _settings = new FullTextSearchEngineSettings();
            _analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
        }

        public CfLuceneIndexBuilder(Directory directory, Analyzer analyzer, FullTextSearchEngineSettings settings)
        {
            _directory = directory;
            _analyzer = analyzer;
            _settings = settings;
        }

        private bool _disposed;
        private static IndexWriter _writer;
        private static readonly Object WriterLock = new Object();
        private void DoWriterAction(Action<IndexWriter> action) { lock (WriterLock) { EnsureIndexWriter(); } action(_writer); }
        private T DoWriterAction<T>(Func<IndexWriter, T> action) { lock (WriterLock) { EnsureIndexWriter(); } return action(_writer); }
        // Method should only be called from within a lock.
        void EnsureIndexWriter()
        {
            if (_writer == null)
            {
                if (IndexWriter.IsLocked(_directory))
                {
                    //Log.Error("Something left a lock in the index folder: deleting it");
                    IndexWriter.Unlock(_directory);
                    //Log.Info("Lock Deleted... can proceed");
                }
                _writer = new IndexWriter(_directory, _analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
                _writer.SetMergePolicy(new LogDocMergePolicy(_writer));
                _writer.SetMergeFactor(5);
            }
        }


        public IEnumerable<IndexingError> AddEntry(ILuceneSearchEngineEntry entry) { return AddEntries(new[] { entry }, false); }
        public IEnumerable<IndexingError> AddEntries(IEnumerable<ILuceneSearchEngineEntry> entries) { return AddEntries(entries, true); }
        public IEnumerable<IndexingError> AddEntries(IEnumerable<ILuceneSearchEngineEntry> entries, bool optimize)
        {
            IList<IndexingError> errors = new List<IndexingError>();
            foreach (var e in entries)
            {
                ExecuteRemoveIndexItem(e.Key);
                try
                {
                    var current = e;
                    DoWriterAction(writer => writer.AddDocument(current.ToDocument()));
                }
                catch (Exception ex)
                {
                    errors.Add(new IndexingError(e, ex));
                }
            }
            DoWriterAction(writer =>
            {
                writer.Commit();
                if (optimize) { writer.Optimize(); }
            });

            return errors;
        }

        public void RemoveIndexItem(string objectID)
        {
            ExecuteRemoveIndexItem(objectID);
            DoWriterAction(writer => writer.Commit());
        }

        private void ExecuteRemoveIndexItem(string objectID)
        {
            Query searchQuery = GetIdSearchQuery(objectID);
            DoWriterAction(writer => writer.DeleteDocuments(searchQuery));
        }

        private static Query GetIdSearchQuery(string objectID) { return new TermQuery(new Term("Key", objectID)); }

        public int GetTotalIndexedEntryCount() { return DoWriterAction(writer => writer.GetReader().NumDocs()); }


        ~CfLuceneIndexBuilder()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock(WriterLock)
            {
                if(!_disposed)
                {
                    //Never checking for disposing = true because there are
                    //no managed resources to dispose

                    var writer = _writer;

                    if(writer != null)
                    {
                        try
                        {
                            writer.Commit();
                            writer.Close();
                            IndexWriter.Unlock(_directory);
                        }
                        catch(ObjectDisposedException e)
                        {
                            CfTrace.Error(e); 
                        }
                        _writer = null;
                    }

                    var analyzer = _analyzer;

                    if (_analyzer != null)
                    {
                        try
                        {
                            analyzer.Close();
                        }
                        catch (ObjectDisposedException e)
                        {
                            CfTrace.Current.Error(e); 
                        }
                    }

                    
                    var directory = _directory;
                    if(directory != null)
                    {
                        try
                        {
                            //var lockFactory = directory.GetLockFactory();
                            //foreach (var name in directory.ListAll()) { lockFactory.ClearLock(name); }
                                                                                    
                            directory.Close();
                        }
                        catch(ObjectDisposedException e)
                        {
                            CfTrace.Current.Error(e); 
                        }
                    }

                    _disposed = true;
                }
            }
            GC.SuppressFinalize(this);
        }
    }
}
