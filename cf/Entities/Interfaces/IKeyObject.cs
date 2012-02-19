using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cf.Entities.Interfaces
{    
    /// <summary>
    /// IKeyObject is a critical interface in making our data access pattern work.
    /// It's purpose is to make sure objects have keys that we can user perform selects
    /// (and hence updates and deletes).
    ///
    /// Here T is Defined as : IEquatable<T>, this simply means that the key is of a type
    /// where we can user the .Equals method to compare two different objects to see if they
    /// have the same key. In the SSR T is usually either an Int or Guid
    /// </summary>
    public interface IKeyObject<T> : IOOObject where T : IEquatable<T>
    {
        T ID { get; set; }
    }

    public interface IByteKeyObject : IKeyObject<byte> { }
    public interface IIntKeyObject : IKeyObject<int> { }
    public interface IGuidKeyObject : IKeyObject<Guid> { }
}
