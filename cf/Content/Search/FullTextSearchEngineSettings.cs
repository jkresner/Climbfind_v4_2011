using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections;
using Lucene.Net.Analysis;
using System.Xml.XPath;
using System.Xml;

namespace cf.Content.Search
{
    public class FullTextSearchEngineSettings
    {
        //public static readonly FullTextSearchEngineSettings Settings = 
        //    (FullTextSearchEngineSettings)ConfigurationManager.GetSection("FullTextSearchEngineSettings");

        private string _stopWordsString;

        public FullTextSearchEngineSettings()
        {
            Language = "English";
            StopWords = StopAnalyzer.ENGLISH_STOP_WORDS_SET;
            //Parameters = new TuningParameters();
            MinimumScore = 0.1f;
            //IndexFolderLocation = "~/App_Data";
            IsEnabled = true;
        }

        public string Language { get; set; }
        //public string IndexFolderLocation { get; set; }
        public bool IsEnabled { get; set; }
        //public TuningParameters Parameters { get; set; }
        public float MinimumScore { get; set; }
        [XmlIgnore] public Hashtable StopWords { get; private set; }
        [XmlElement("StopWords")]
        public string StopWordsString
        {
            set
            {
                _stopWordsString = value;
                String[] stopWords = _stopWordsString.Split(',');

                var stopSet = new CharArraySet(stopWords, false);
                StopWords = CharArraySet.UnmodifiableSet(stopSet);
            }
        }
    }


    public class XmlSerializerSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            XPathNavigator nav = section.CreateNavigator();
            var typename = (string)nav.Evaluate("string(@type)");
            Type t = Type.GetType(typename);
            var ser = new XmlSerializer(t);
            return ser.Deserialize(new XmlNodeReader(section));
        }
    }
}
