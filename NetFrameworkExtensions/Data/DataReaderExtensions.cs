using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace NetFrameworkExtensions.Data
{
    public static class DataReaderExtensions
    {
        /// <summary>
        /// Help us deal with nullable database values elegantly in ado .net 2.0 style code
        /// </summary>
        /// <param name="r"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <remarks>This extension is found mosting in our Repository classes</remarks>
        public static string GetPossibleNullString(this SqlDataReader r, int index)
        {
            return (r[index] == DBNull.Value) ? null : r.GetString(index);
        }

        /// <summary>
        /// Help us deal with nullable database values elegantly in ado .net 2.0 style code
        /// </summary>
        /// <param name="r"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <remarks>This extension is found mosting in our Repository classes</remarks>
        public static double? GetPossibleNullDouble(this SqlDataReader r, int index)
        {
            if (r[index] == DBNull.Value) { return null; } else { return r.GetDouble(index); }
        }

        /// <summary>
        /// Help us deal with nullable database values elegantly in ado .net 2.0 style code
        /// </summary>
        /// <param name="r"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <remarks>This extension is found mosting in our Repository classes</remarks>
        public static Guid? GetPossibleNullGuid(this SqlDataReader r, int index)
        {
            if (r[index] == DBNull.Value) { return null; } else { return r.GetGuid(index); }
        }
    }
}
