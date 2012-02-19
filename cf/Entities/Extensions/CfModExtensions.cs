using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using System.Web.Script.Serialization;
using cf.Entities.Enum;
using Omu.ValueInjecter;
using Microsoft.SqlServer.Types;
using NetFrameworkExtensions;
using cf.Identity;

namespace cf.Entities
{
    public static partial class CfModExtensions
    {
        public static void SetCreated(this ObjectModMeta meta, Guid actionID)
        {
            meta.CreatedActionID = actionID;
            meta.CreatedByUserID = CfIdentity.UserID;
            meta.CreatedUtc = DateTime.UtcNow;
        }
        
        public static void SetDeleted(this ObjectModMeta meta, Guid actionID)
        {
            meta.DeletedActionID = actionID;
            meta.DeletedByUserID = CfIdentity.UserID;
            meta.DeletedUtc = DateTime.UtcNow;
        }
        
        public static void SetAvatarChanged(this ObjectModMeta meta, Guid actionID)
        {
            meta.VerifiedAvatar = 0;
            meta.AvatarLastChangedActionID = actionID;
            meta.AvatarLastChangedByUserID = CfIdentity.UserID;
            meta.AvatarLastChangedUtc = DateTime.UtcNow;
            if (meta.CQR > 1) { meta.CQR -= 1; };
        }

        public static void SetDetailsChanged(this ObjectModMeta meta, Guid actionID)
        {
            meta.VerifiedDetails= 0;
            meta.DetailsLastChangedActionID = actionID;
            meta.DetailsLastChangedByUserID = CfIdentity.UserID;
            meta.DetailsLastChangedUtc = DateTime.UtcNow;
            if (meta.CQR > 1) { meta.CQR -= 1; };
        }        
    }
}
