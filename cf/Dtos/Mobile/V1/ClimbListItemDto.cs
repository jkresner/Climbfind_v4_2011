using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using cf.Entities;
using ProtoBuf;

namespace cf.Dtos.Mobile.V1
{
    [ProtoContract]
    public class ClimbListDto
    {
        [ProtoMember(1)]
        public List<ClimbSectionDto> Sections { get; set; }

        public ClimbListDto()
        {
            Sections = new List<ClimbSectionDto>();
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    [ProtoContract]
    public class ClimbListItemDto
    {
        [ProtoMember(1)] public string ID { get; set; }
        [ProtoMember(2)] public string Name { get; set; }
        [ProtoMember(3)] public string Alt { get; set; }
        [ProtoMember(4)] public string Avatar { get; set; }
        [ProtoMember(5)] public string Grade { get; set; }
        [ProtoMember(6)] public Nullable<double> Rating { get; set; }
        [ProtoMember(7)] public int RatingCount { get; set; }

        public ClimbListItemDto() { }

        //public ClimbListItemDto(Guid id, string name, string name2, string avatar, string grade, double? rating, int ratingCount)
        //{
        //    ID = id.ToString("N");
        //    Name = name;
        //    Name2 = name;
        //    Grade = grade;
        //    Avatar = avatar;
        //    Rating = rating;
        //    RatingCount = ratingCount;
        //}

        public ClimbListItemDto(Climb c)
        {
            ID = c.ID.ToString("N");
            Name = c.Name;
            if (c.Type == Entities.Enum.CfType.ClimbIndoor) { Alt = DtoHelper.GetPGAltName(c as ClimbIndoor); }
            Grade = c.GradeLocal;
            Avatar = c.Avatar;
            Rating = c.Rating;
            RatingCount = c.RatingCount;
        }
    }

    public static class DtoHelper 
    {
        public static string GetPGAltName(ClimbIndoor ci)
        {
            var alt = ci.MarkingColor;
            if (!string.IsNullOrEmpty(ci.LineNumber)) { alt += " " + ci.LineNumber; }
            return alt;
        }
    }

    [ProtoContract]
    public class ClimbSectionDto
    {
        [ProtoMember(1)] public string ID { get; set; }
        [ProtoMember(2)] public string Name { get; set; }
        [ProtoMember(3)] public string Type { get; set; }
        [ProtoMember(4)] public string Range { get; set; }
        [ProtoMember(5)] public string Avatar { get; set; }
        [ProtoMember(6)] public List<ClimbListItemDto> Climbs { get; set; }
    }
}
