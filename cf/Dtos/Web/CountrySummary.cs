using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using ProtoBuf;

namespace cf.Dtos
{
    /// <summary>
    /// Dto used to transfer information for the countries page (showing the summary of places, climbs etc)
    /// </summary>
    [ProtoContract]
    public class CountrySummary : IByteKeyObject
    {
        [ProtoMember(1)] public byte ID { get; set; }
        [ProtoMember(2)] public string CountryName { get; set; }
        [ProtoMember(3)] public int ProvinceCount { get; set; }
        [ProtoMember(4)] public int CityCount { get; set; }
        [ProtoMember(5)] public int ClimbingAreaCount { get; set; }
        [ProtoMember(6)] public int IndoorLocationCount { get; set; }
        [ProtoMember(7)] public int OutdoorLocationCount { get; set; }
        [ProtoMember(8)] public int IceLocationCount { get; set; }
        [ProtoMember(9)] public int ClimbsCount { get; set; }
    }
}
