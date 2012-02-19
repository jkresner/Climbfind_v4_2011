using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;

namespace cf.Content.Feed
{
    internal static class PostTemplateLibrary
    {
        public const string V0CheckIn = "V0CheckIn";
        public const string V0PartnerCall = "V0PartnerCall";
        public const string V0Talk = "V0Talk";
        public const string V0ContentAdd = "V0ContentAdd";
        public const string V0Opinion = "V0Opinion";

        public const string V1CheckIn = "V1CheckIn";

        private static readonly Dictionary<string, IPostManager> _library;
        
        static PostTemplateLibrary()
        {
            _library = new Dictionary<string, IPostManager>();
            _library.Add(V0CheckIn, new V0.CheckInPostManager());
            _library.Add(V0PartnerCall, new V0.PartnerCallPostManager());
            _library.Add(V0Talk, new V0.TalkPostManager());
            _library.Add(V0ContentAdd, new V0.ContentAddPostManager());
            _library.Add(V0Opinion, new V0.OpinionPostManager());

            _library.Add(V1CheckIn, new V1.CheckInPostManager());
        }

        internal static IPostManager Get(string templateKey) { return _library[templateKey]; }
    }
}
