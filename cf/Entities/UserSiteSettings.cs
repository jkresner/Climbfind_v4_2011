//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace cf.Entities
{
    public partial class UserSiteSettings
    {
        #region Primitive Properties
    
        public virtual System.Guid ID
        {
            get { return _iD; }
            set
            {
                if (_iD != value)
                {
                    if (Profile != null && Profile.ID != value)
                    {
                        Profile = null;
                    }
                    _iD = value;
                }
            }
        }
        private System.Guid _iD;
    
        public virtual System.Guid RssID
        {
            get;
            set;
        }
    
        public virtual bool RssEnabled
        {
            get;
            set;
        }
    
        public virtual bool MessagesEmailRealTime
        {
            get;
            set;
        }
    
        public virtual bool MessagesAlertFeed
        {
            get;
            set;
        }
    
        public virtual bool MessagesMobileRealTime
        {
            get;
            set;
        }
    
        public virtual bool CommentsOnPostsEmailRealTime
        {
            get;
            set;
        }
    
        public virtual bool CommentsOnPostsAlertFeed
        {
            get;
            set;
        }
    
        public virtual bool CommentsOnPostsMobileRealTime
        {
            get;
            set;
        }
    
        public virtual bool CommentsOnMediaEmailRealTime
        {
            get;
            set;
        }
    
        public virtual bool CommentsOnMediaAlertFeed
        {
            get;
            set;
        }
    
        public virtual bool CommentsOnMediaMobileRealTime
        {
            get;
            set;
        }
    
        public virtual string DeviceTypeRegistered
        {
            get;
            set;
        }

        #endregion
        #region Navigation Properties
    
        public virtual Profile Profile
        {
            get { return _profile; }
            set
            {
                if (!ReferenceEquals(_profile, value))
                {
                    var previousValue = _profile;
                    _profile = value;
                    FixupProfile(previousValue);
                }
            }
        }
        private Profile _profile;

        #endregion
        #region Association Fixup
    
        private void FixupProfile(Profile previousValue)
        {
            if (previousValue != null && ReferenceEquals(previousValue.UserSiteSetting, this))
            {
                previousValue.UserSiteSetting = null;
            }
    
            if (Profile != null)
            {
                Profile.UserSiteSetting = this;
                if (ID != Profile.ID)
                {
                    ID = Profile.ID;
                }
            }
        }

        #endregion
    }
}
