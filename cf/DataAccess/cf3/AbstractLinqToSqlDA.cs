using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Configuration;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Specialized;
using cf.Entities.Interfaces;

namespace cf.DataAccess.cf3
{
    /// <summary>
    /// The code in this namespace was taken from CF3
    /// </summary>
    /// <typeparam name="OOT">Our Object Orientated type as defined in ClimbFind.Model.Objects</typeparam>
    /// <typeparam name="LT">Linq type as defined in our LinqToSqlMapping .dbml designer file</typeparam>
    /// <typeparam name="KeyType">The type of key, either an 'int' or 'Guid' but could be something else</typeparam>
    internal abstract class AbstractLinqToSqlDA<OOT, LT, KeyType> : IObjectOrientatedDA<OOT, KeyType>
        where OOT : class, IKeyObject<KeyType>, new()
        where LT : class, IKeyObject<KeyType>, new()
        where KeyType : IEquatable<KeyType>
    {
        /// <summary>
        /// Members
        /// </summary>

        protected ClimbfindLinqModelDataContext ctx;
        public Table<LT> EntityTable { get { return ctx.GetTable<LT>(); } }

        /// <summary>
        /// Constructors
        /// </summary>
        public AbstractLinqToSqlDA()
        {
            ctx = new ClimbfindLinqModelDataContext(Stgs.DbConnectionString);
        }

        public AbstractLinqToSqlDA(string connectionString)
        {
            ctx = new ClimbfindLinqModelDataContext(connectionString);
        }

        public AbstractLinqToSqlDA(IDATransactionContext transactionContext)
        {
            if (transactionContext == null) { throw new Exception("Cannot run AbstractBaseDA in transaction mode with a null data context"); }
            ctx = transactionContext as ClimbfindLinqModelDataContext;
        }

        /// <summary>
        /// Base virtual Mapping Methods that we can override in each inheriting DA if necessary 
        /// </summary>

        protected virtual LT MapOOTypeToLinqType(OOT t)
        {
            return MapValues(new LT(), t);
        }

        /// <summary>
        /// ** If required, Add additional Custom property mappings then we override this method ** --//
        /// </summary>
        /// <param name="lt"></param>
        /// <returns></returns>
        protected virtual OOT MapLinqTypeToOOType(LT lt)
        {
            return MapValues(new OOT(), lt);
        }

        /// <summary>
        /// Base Mapping Methods 
        /// </summary>

        protected OOT MapType(LT lt)
        {
            if (lt == null) { return null; }
            else { return MapLinqTypeToOOType(lt); }
        }

        protected LT MapType(OOT t)
        {
            if (t == null) { return null; }
            else { return MapOOTypeToLinqType(t); }
        }

        protected List<OOT> MapList(List<LT> listOfLT)
        {
            return (from lt in listOfLT select MapType(lt)).ToList();
        }

        protected List<LT> MapList(List<OOT> listOfOOT)
        {
            return (from t in listOfOOT select MapType(t)).ToList();
        }

        protected LT MapValues(LT lt, OOT t)
        {
            MapValues(lt, t.GetProperyNameAndValues());

            return lt;
        }

        protected OOT MapValues(OOT t, LT lt)
        {
            MapValues(t, lt.GetProperyNameAndValues());

            return t;
        }

        protected LT MapValues(LT lt, NameValueCollection keysAndValue)
        {
            MapNameValuesCollectionToObject(lt, keysAndValue);

            return lt;
        }

        protected OOT MapValues(OOT t, NameValueCollection keysAndValue)
        {
            MapNameValuesCollectionToObject(t, keysAndValue);

            return t;
        }

        /// <summary>
        /// Get Methods
        /// </summary>
        /// <returns></returns>

        public List<OOT> GetAll()
        {
            return MapList((from c in EntityTable select c).ToList());
        }

        /// <summary>
        /// Insert Methods
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>

        public OOT Insert(OOT t)
        {
            return Insert(MapType(t));
        }

        protected OOT Insert(LT lt)
        {
            EntityTable.InsertOnSubmit(lt);
            CommitChanges();

            return MapType(lt);
        }

        public List<OOT> Insert(List<OOT> listOfT)
        {
            return Insert(MapList(listOfT));
        }

        protected List<OOT> Insert(List<LT> listOfT)
        {
            EntityTable.InsertAllOnSubmit(listOfT);
            CommitChanges();

            return MapList(listOfT);
        }

        protected void InsertReference(ITable table, object entity)
        {
            table.InsertOnSubmit(entity);
            CommitChanges();
        }

        /// <summary>
        /// Base update methods
        /// </summary>
        /// <param name="lt"></param>
        /// <returns></returns>

        public OOT Update(OOT t)
        {
            LT lt = MapValues(GetLinqTypeByID(t.ID), t);
            return Update(lt);
        }

        protected OOT Update(LT lt)
        {
            CommitChanges();
            return MapType(lt);
        }

        /// <summary>
        /// Getter Methods
        /// </summary>

        public OOT GetByID(KeyType id)
        {
            return MapType(GetLinqTypeByID(id));
        }

        /// <summary>
        /// GetLinqTypeByID is defined here as a virtual method as sometimes in the
        /// case of AbstractDeprecableDA we want to add a !Deprecated selection criteria
        /// to the basic GetByID query.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual LT GetLinqTypeByID(KeyType id)
        {
            return (from c in EntityTable where id.Equals(c.ID) select c).SingleOrDefault();
        }

        /// <summary>
        /// Delete methods
        /// </summary>

        public void Delete(KeyType id)
        {
            LT t = (from c in EntityTable where id.Equals(c.ID) select c).SingleOrDefault();
            EntityTable.DeleteOnSubmit(t);
            CommitChanges();
        }

        public void Delete(List<KeyType> idsToDelete)
        {
            List<LT> listOfLT = (from c in EntityTable where idsToDelete.Contains(c.ID) select c).ToList();
            EntityTable.DeleteAllOnSubmit(listOfLT);
            CommitChanges();
        }


        /// <summary>
        /// CommitChanges causes the LinqToSql data context to update the database in a single consistent
        /// fasion throughout our DA
        /// </summary>
        protected void CommitChanges()
        {
            ctx.SubmitChanges(ConflictMode.FailOnFirstConflict);
        }


        /// <summary>
        /// MapNameValuesCollectionToObject: Awesome reflection method, sets properties on objects
        /// using the Name / Value pairs in the NameValueCollection
        /// </summary>
        private static object MapNameValuesCollectionToObject(object value, NameValueCollection values)
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
    }

    /// <summary>
    /// Extension helpers methods for SSRObjects
    /// </summary>
    public static partial class ClimbFindObjectExtensionMethods
    {
        /// <summary>
        /// GetProperyNameAndValues uses reflection to return a collection of
        /// an objects Simple Type (String or ValueType) properties and values.
        /// </summary>
        /// <param name="o"></param>
        /// <returns>NameValueCollection of an objects properties and associated values</returns>
        public static NameValueCollection GetProperyNameAndValues(this IOOObject o)
        {
            Type objType = o.GetType();
            PropertyInfo[] fields = objType.GetProperties();
            NameValueCollection nvCollection = new NameValueCollection();

            foreach (PropertyInfo property in fields)
            {
                if (property.PropertyType == typeof(string) ||
                    property.PropertyType.BaseType == typeof(ValueType))
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
        /// ToFriendlyHTMLString (ONLY TO BE USED FOR DEBUGGING PUPOSES PLEASE)!
        /// </summary>
        /// <param name="o"></param>
        /// <returns>Unordered HTML list of properties and values of an object</returns>
        public static string ToFriendlyHTMLString(this IOOObject o)
        {
            StringBuilder sb = new StringBuilder("<ul>");
            NameValueCollection nvCollection = GetProperyNameAndValues(o);

            foreach (string key in nvCollection.Keys)
            {
                sb.AppendFormat("<li>{0}: {1}</li>", key, nvCollection[key]);
            }

            sb.Append("</ul>");

            return sb.ToString();
        }
    }

    /// <summary>
    /// IDATransactionContext, is used by passing this context into a DA's contructor to 
    /// make that DA transaction aware without passing in a full blown DataContexta and
    /// tightly coupling out DAs implementation with how we run transactions in higher layers
    /// </summary>
    public interface IDATransactionContext { }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectOrientatedDA<T, KeyType>
        where T : class, IKeyObject<KeyType>, new()
        where KeyType : IEquatable<KeyType>
    {
        List<T> GetAll();

        T GetByID(KeyType id);

        T Insert(T t);

        List<T> Insert(List<T> listOfT);

        T Update(T t);

        void Delete(KeyType id);

        void Delete(List<KeyType> idsToDelete);
    }
}
