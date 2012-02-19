using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace NetFrameworkExtensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/aa226544%28SQL.80%29.aspx
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ODataEncode(this string text)
        {
            return text.Replace("'", "&apos;");
        }

        public static string ODataDecode(this string text)
        {
            return text.Replace("&apos;","'");
        }

        public static string GetEmptyIfNull(this string text)
        {
            return string.IsNullOrWhiteSpace(text) ? String.Empty : text;
        }

        public static string ToDomIdFriendlyString(this string text)
        {
            //-- maybe improve this soon
            return text.ToUrlFriendlyString().Replace("'", "").Replace("?", "");
        }

        public static string ToUrlFriendlyString(this string text)
        {
            string str = text.ToLower();
            str = str.Replace(" / ","-");
            str = str.Replace("/ ", "-");
            str = str.Replace(" /", "-");
            str = str.Replace("/", "-");

            // invalid chars, make into spaces
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces/hyphens into one space       
            str = Regex.Replace(str, @"[\s-]+", " ").Trim();
            // cut and trim it
            //str = str.Substring(0, str.Length <= maxLength ? str.Length : maxLength).Trim();
            // hyphens
            str = Regex.Replace(str, @"\s", "-");

            str = str.Replace(".","");

            return str;

            //return text.Trim().Replace(" ", "-").ToLower();   
        }

        public static string RemoveNonUtf8Characters(this string accentedStr)
        {
            byte[] tempBytes;
            tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(accentedStr);
            string asciiStr = System.Text.Encoding.UTF8.GetString(tempBytes);
            return asciiStr;
        }

        /// <summary>
        /// String helpers
        /// </summary>

        public static string Excerpt(this string value, int length)
        {
            if (value.Length < length) { return value; }
            else { return value.Substring(0, length - 1) + " …"; }
        }

        public static string RemoveNewLines(this string input)
        {
            if (input == "") { return (""); }

            Regex RemoveNewLinesRX = new Regex(@"[\r|\n]", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

            return RemoveNewLinesRX.Replace(input, string.Empty);
        }


        public static string RemoveSpaces(this string input)
        {
            return (input.Replace(" ", ""));
        }


        public static string RemoveSpecialChars(this string input)
        {
            return (input.Replace(" ", "").Replace("_", "").Replace("-", "").Replace("'", "").Replace("&", ""));
        }

        public static string RemoveSpecialCharsExcludingSpaces(this string input)
        {
            return (input.Replace("_", "").Replace("-", "").Replace("'", "").Replace("&", "")).Trim();
        }


        private static string upperCaseMatch(Match match)
        {
            return match.Value.ToUpper();
        }


        public static string CapitalizeWordsInString(this string words)
        {
            Regex CapitalizeWordsRX = new Regex(@"\b\w", RegexOptions.Compiled);

            return (CapitalizeWordsRX.Replace(words, new MatchEvaluator(upperCaseMatch)));
        }

        public static string GetHtmlParagraph(this string text)
        {
            if (text == null) { return (""); }
            if (text == "") { return (""); }

            text = HttpUtility.HtmlEncode(text);

            string replace = "<br />";

            string tempStringOne = ieTextBoxReplace.Replace(text, replace);
            string tempStringTwo = ffTextBoxReplace.Replace(tempStringOne, replace);

            return (tempStringTwo);
        }

        private static readonly Regex ieTextBoxReplace = new Regex(Environment.NewLine, RegexOptions.Compiled);
        private static readonly Regex ffTextBoxReplace = new Regex("\n", RegexOptions.Compiled);




        public class Distance
        {
            /// <summary>
            /// Compute Levenshtein distance
            /// </summary>
            /// <param name="s">String 1</param>
            /// <param name="t">String 2</param>
            /// <returns>Distance between the two strings.
            /// The larger the number, the bigger the difference.
            /// </returns>
            /// <see cref="http://www.merriampark.com/ldcsharp.htm"/>
            public int LD(string s, string t)
            {
                int n = s.Length; //length of s
                int m = t.Length; //length of t

                int[,] d = new int[n + 1, m + 1]; // matrix
                int cost; // cost
                // Step 1
                if (n == 0) return m;
                if (m == 0) return n;
                // Step 2
                for (int i = 0; i <= n; d[i, 0] = i++) ;
                for (int j = 0; j <= m; d[0, j] = j++) ;

                // Step 3
                for (int i = 1; i <= n; i++)
                {
                    //Step 4
                    for (int j = 1; j <= m; j++)
                    {
                        // Step 5
                        cost = (t.Substring(j - 1, 1) == s.Substring(i - 1, 1) ? 0 : 1);
                        
                        // Step 6
                        d[i, j] = System.Math.Min(System.Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                    }

                }
                // Step 7
                return d[n, m];
            }
        }
    }
}
