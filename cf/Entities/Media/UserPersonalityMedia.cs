using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using cf.Entities.Enum;
using System.ComponentModel.DataAnnotations;
using cf.Entities.Validation;
using NetFrameworkExtensions;
using cf.Content;

namespace cf.Entities
{
    public partial class UserPersonalityMedia : IKeyObject<Guid>
    {
        public string IDstring { get { return ID.ToString(); } }
        public PersonalityCategory Category { get { return (PersonalityCategory)CategoryID; } }

        public DateTime AddedUtc { get { return Media.AddedUtc; } }
       
        public UserPersonalityMedia() { }

        public UserPersonalityMedia(UserPersonalityMedia um, Media m)
        {
            this.ID = um.ID;
            this.MediaID = um.MediaID;
            this.UserID = um.UserID;
            this.CategoryID = um.CategoryID;
            this.Media = m;
        }
    }
}
