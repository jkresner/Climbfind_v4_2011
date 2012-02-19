using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Enum;
using cf.Entities.Interfaces;
using System.Runtime.Serialization;
using ProtoBuf;
using cf.Content;

namespace cf.Dtos.Cache
{   
    /// <summary>
    /// Used to maintain session variables to speed up climb entry
    /// </summary>
    [ProtoContract]
    public class CachedLocationDetails 
    {
        [ProtoMember(1)] public Guid ID { get; set; }
        [ProtoMember(2)] public string Name { get; set; }
        [ProtoMember(3)] public string NameShort { get; set; }
        [ProtoMember(4)] public string Avatar { get; set; }

        //-- Computed Properties
        public bool HasAvatar { get { return !string.IsNullOrWhiteSpace(Avatar); } }
        public string ShortDisplayName { get { return string.IsNullOrWhiteSpace(NameShort) ? Name : NameShort; } }

        public CachedLocationDetails() { }
        public CachedLocationDetails(ILocation l)
        {
            if (l != null)
            {
                ID = l.ID;
                Name = l.Name;
                NameShort = l.NameShort;
                Avatar = l.Avatar;
            }
            else
            {
                ID = Guid.Empty;
                Name = "Deleted location";
            }
        }


    }
}
