using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Enum;
using cf.Entities.Interfaces;
using System.Runtime.Serialization;
using ProtoBuf;

namespace cf.Dtos
{
    /// <summary>
    /// Used to maintain session variables to speed up climb entry
    /// </summary>
    [ProtoContract]
    public class ClimbEntrySettings
    {
        [ProtoMember(1)] public Guid SectionID { get; set; }
        [ProtoMember(2)] public byte ClimbTypeID { get; set; }
    }
}
