using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Reflection;
using System.ComponentModel;
using Microsoft.SqlServer.Types;
using NetFrameworkExtensions.SqlServer.Types;

namespace NetFrameworkExtensions
{
    public static class ObjectExtensions
    {
        public static string DifferenceAsJson<T>(this T original, T updated, List<string> propertyNamesToIgnore) where T : new()
        {
            //-- If it's a delete
            if (updated == null) { return SetValuesAsJson(original, propertyNamesToIgnore); }
            
            //-- If it's a create
            if (original == null) { return SetValuesAsJson(updated, propertyNamesToIgnore); }
            
            //-- Else it's an update
            var sb = new StringBuilder("{");
            int i = 0;
            if (propertyNamesToIgnore == null) { propertyNamesToIgnore = new List<string>(); }
            
            Type objType = typeof(T);
            PropertyInfo[] fields = objType.GetProperties();
            NameValueCollection nvCollection = new NameValueCollection();

            foreach (PropertyInfo property in fields)
            {
                if (!propertyNamesToIgnore.Contains(property.Name))
                {
                    var originalValue = GetSimpleTypeValueAsString(original, property);
                    var updatedValue = GetSimpleTypeValueAsString(updated, property);

                    if (originalValue != updatedValue)
                    {
                        if (i > 0) { sb.Append(","); }
                        var propNameToInclude = (property.Name.Length > 3) ? property.Name.Substring(0, 3) : property.Name;
                        sb.AppendFormat(@"""{0}"" : ""{1}""", propNameToInclude, updatedValue);
                        i++;
                    }
                }
            }
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="propertyNamesToIgnore"></param>
        /// <returns></returns>
        public static string SetValuesAsJson<T>(this T t, List<string> propertyNamesToIgnore)
        {
            var sb = new StringBuilder("{");
            int i = 0;
            if (propertyNamesToIgnore == null) { propertyNamesToIgnore = new List<string>(); }

            Type objType = typeof(T);
            PropertyInfo[] fields = objType.GetProperties();
            NameValueCollection nvCollection = new NameValueCollection();

            foreach (PropertyInfo property in fields)
            {
                if (!propertyNamesToIgnore.Contains(property.Name))
                {
                    string val = GetSimpleTypeValueAsString(t, property);
                    
                    if (val != null)
                    {
                        if (i > 0) { sb.Append(","); }
                        var propNameToInclude = (property.Name.Length > 3) ? property.Name.Substring(0, 3) : property.Name;
                        sb.AppendFormat(@"""{0}"" : ""{1}""", propNameToInclude, val);
                        i++;
                    }
                }
            }
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private static string GetSimpleTypeValueAsString<T>(T t, PropertyInfo property)
        {
            string val = null;

            if (property.PropertyType == typeof(string) || property.PropertyType.BaseType == typeof(ValueType))
            {
                if (property.GetValue(t, null) != null)
                {
                    val = property.GetValue(t, null).ToString();
                }
            }
            else if (property.PropertyType == typeof(SqlGeography))
            {
                object geo = property.GetValue(t, null);
                if (geo != null) { val = (geo as SqlGeography).GetWkt(); }
            }

            return val;
        }

        /// <summary>
        /// MapNameValuesCollectionToObject: Sets properties on objects using the Name / Value pairs in the NameValueCollection
        /// </summary>
        public static object MapNameValuesCollectionToObject(object value, NameValueCollection values)
        {
            Type objType = value.GetType();
            string objName = objType.Name;

            PropertyInfo[] fields = objType.GetProperties();

            foreach (PropertyInfo property in fields)
            {
                if (values[property.Name] != null)
                {
                    TypeConverter conv = TypeDescriptor.GetConverter(property.PropertyType);
                    object thisValue = values[property.Name];

                    if (conv.CanConvertFrom(typeof(string)))
                    {
                        thisValue = conv.ConvertFrom(values[property.Name]);
                        property.SetValue(value, thisValue, null);
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Return a collection of an objects Simple Type (String or ValueType) properties and values.
        /// </summary>
        /// <param name="o"></param>
        /// <returns>NameValueCollection of an objects properties and associated values</returns>
        public static NameValueCollection GetSimpleTypePropertyNamesAndValues(this object o)
        {
            Type objType = o.GetType();
            PropertyInfo[] fields = objType.GetProperties();
            NameValueCollection nvCollection = new NameValueCollection();

            foreach (PropertyInfo property in fields)
            {
                if (property.PropertyType == typeof(string) ||
                    property.PropertyType.BaseType == typeof(ValueType) ||
                    property.PropertyType == typeof(SqlGeography))  
                {
                    if (property.GetValue(o, null) != null)
                    {
                        nvCollection.Add(property.Name, property.GetValue(o, null).ToString());
                    }
                }
            }

            return nvCollection;
        }


        /// <summary>
        /// http://www.codeproject.com/KB/linq/linqrandomsample.aspx
        /// </summary>
        public static List<T> RandomSample<T>(this IEnumerable<T> source, int count)
        {
            return RandomSampleIterator<T>(source, count, -1, false).ToList();
        }

        public static IEnumerable<T> RandomSample<T>(this IEnumerable<T> source, int count, int seed, bool allowDuplicates)
        {
            return RandomSampleIterator<T>(source, count, seed, allowDuplicates);
        }

        static IEnumerable<T> RandomSampleIterator<T>(IEnumerable<T> source,
            int count, int seed, bool allowDuplicates)
        {
            // take a copy of the current list
            List<T> buffer = new List<T>(source);

            // create the "random" generator, time dependent or with 
            // the specified seed
            Random random;
            if (seed < 0)
                random = new Random();
            else
                random = new Random(seed);

            count = Math.Min(count, buffer.Count);

            if (count > 0)
            {
                // iterate count times and "randomly" return one of the 
                // elements
                for (int i = 1; i <= count; i++)
                {
                    // maximum index actually buffer.Count -1 because 
                    // Random.Next will only return values LESS than 
                    // specified.
                    int randomIndex = random.Next(buffer.Count);
                    yield return buffer[randomIndex];
                    if (!allowDuplicates)
                        // remove the element so it can't be selected a 
                        // second time
                        buffer.RemoveAt(randomIndex);
                }
            }
        }


    }
}
