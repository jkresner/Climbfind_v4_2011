using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Caching
{
    public interface IRemoteCache<T> where T : new()
    {
        T Get(string key);
        bool Add(T entry, string key);
        bool Remove(string key);
        bool Refresh();
    }

    public interface IRemoteCache
    {
        object Get(string key);
        bool Add<T>(T entry, string key);
        bool Add<T>(T entry, string key, TimeSpan validFor);
        bool Remove(string key);
        bool Refresh();
    }
}
