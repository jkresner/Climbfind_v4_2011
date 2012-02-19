using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;

namespace NetFrameworkExtensions.Web
{
    public static class HttpExtensions
    {
        public static string ExecuteRestCall(this Uri url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(url); ;
            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            string xmlResponse = string.Empty;
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                xmlResponse = sr.ReadToEnd();
            }
            return xmlResponse;
        }
        
        public static string GetRequestQueryAndFormNameValuePairString(this HttpRequest request)
        {
            StringBuilder nameValues = new StringBuilder();

            foreach (var k in request.QueryString.AllKeys)
            {
                nameValues.AppendFormat("{0}={1}&",k, request[k]);
            }

            foreach (var k in request.Form.AllKeys)
            {
                nameValues.AppendFormat("{0}={1}&", k, request[k]);
            }

            if (nameValues.Length == 0) { return "None"; }

            return nameValues.ToString();
        }
    }
}
