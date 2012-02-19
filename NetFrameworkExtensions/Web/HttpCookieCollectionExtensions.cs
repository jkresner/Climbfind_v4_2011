using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NetFrameworkExtensions.Web
{
    public static class HttpCookieCollectionExtensions
    {
        public static string ToCookieCollectionKeyValueString(this HttpCookieCollection cookies)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var key in cookies.AllKeys)
            {
                sb.AppendFormat("{0}={1}; ", key, cookies.Get(key).Value);
            }

            var valString = sb.ToString();

            return valString.Substring(0, valString.Length-2);
        }
    }
}
