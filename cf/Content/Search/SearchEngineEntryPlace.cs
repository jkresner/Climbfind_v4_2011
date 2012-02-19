using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Documents;
using cf.Entities.Interfaces;
using cf.Entities;

namespace cf.Content.Search
{
    /// <summary>
    /// Specific fields relevant to search through for places
    /// </summary>
    public class SearchEngineEntryPlace : SearchEngineEntry, ILuceneSearchEngineEntry
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string NameShort { get; set; }
        public string SearchSupportString { get; set; }

        public SearchEngineEntryPlace(Profile p)
        {
            //-- Searchable fields
            this.Key = p.IDstring;
            this.Description = p.DisplayName;
            this.Name = p.FullName;
            this.NameShort = p.NickName;
            this.SearchSupportString = "";

            //-- Result / display fields
            this.CountryID = p.CountryID;
            this.TypeID = (byte)p.Type;
            this.Title = p.DisplayName;
            this.Excerpt = p.FullName;
            this.Url = p.SlugUrl;
        }

        public SearchEngineEntryPlace(IPlaceSearchable place)
        {
            //-- Searchable fields
            this.Key = place.IDstring;
            this.Description = place.Description;
            this.Name = place.Name;
            this.NameShort = place.NameShort;
            this.SearchSupportString = place.SearchSupportString;
            
            //-- Result / display fields
            this.CountryID = place.CountryID;
            this.TypeID = (byte)place.Type;
            this.Title = place.Name;
            this.Excerpt = place.NameShort + place.SearchSupportString;
            this.Url = place.SlugUrl;
        }


        public SearchEngineEntryPlace(Climb climb)
        {
            //-- Searchable fields
            this.Key = climb.IDstring;
            this.Description = climb.Description;
            this.Name = climb.Name;
            this.NameShort = climb.Name;
            this.SearchSupportString = climb.Name;

            //-- Result / display fields
            this.CountryID = climb.CountryID;
            this.TypeID = (byte)climb.Type;
            this.Title = climb.Name;
            this.Excerpt = climb.Name;
            this.Url = climb.SlugUrl;
        }

        public override Lucene.Net.Documents.Document ToDocument()
        {
            var doc = base.ToDocument();

            var nameField = new Field("Name", Name, Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.YES);
            nameField.SetBoost(4f);
            doc.Add(nameField);
            
            //name.SetBoost(_settings.Parameters.TitleBoost);
            if (!string.IsNullOrWhiteSpace(NameShort))
            {
                var nameShort = new Field("NameShort", NameShort, Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.YES);
                doc.Add(nameShort);
            }

            if (!string.IsNullOrWhiteSpace(SearchSupportString))
            {
                var searchSupportString = new Field("SearchSupportString", SearchSupportString, Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.YES);
                doc.Add(searchSupportString);
            }

            if (!string.IsNullOrWhiteSpace(Description))
            {
                var description = new Field("Description", Description, Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.YES);
                doc.Add(description);
            }

            if (TypeID == 9)
            {
                //var stringSc = "bahhh";
            }
    
            var tId = ((byte)TypeID).ToString();
            
            var typeField = new Field("Type", tId, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.NO);
            doc.Add(typeField);

            return doc;
        }
    }
}
