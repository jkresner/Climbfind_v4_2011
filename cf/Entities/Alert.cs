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
    public partial class Alert
    {
        #region Primitive Properties
    
        public virtual System.Guid ID
        {
            get;
            set;
        }
    
        public virtual byte TypeID
        {
            get;
            set;
        }
    
        public virtual System.Guid UserID
        {
            get { return _userID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_userID != value)
                    {
                        if (Profile != null && Profile.ID != value)
                        {
                            Profile = null;
                        }
                        _userID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private System.Guid _userID;
    
        public virtual Nullable<System.Guid> PostID
        {
            get { return _postID; }
            set
            {
                try
                {
                    _settingFK = true;
                    if (_postID != value)
                    {
                        if (Post != null && Post.ID != value)
                        {
                            Post = null;
                        }
                        _postID = value;
                    }
                }
                finally
                {
                    _settingFK = false;
                }
            }
        }
        private Nullable<System.Guid> _postID;
    
        public virtual System.DateTime Utc
        {
            get;
            set;
        }
    
        public virtual string Message
        {
            get;
            set;
        }
    
        public virtual bool Viewed
        {
            get;
            set;
        }
    
        public virtual bool ByRss
        {
            get;
            set;
        }
    
        public virtual bool ByEmail
        {
            get;
            set;
        }
    
        public virtual bool ByFeed
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> ByActivityEmail
        {
            get;
            set;
        }
    
        public virtual bool ByMobile
        {
            get;
            set;
        }

        #endregion
        #region Navigation Properties
    
        public virtual Post Post
        {
            get { return _post; }
            set
            {
                if (!ReferenceEquals(_post, value))
                {
                    var previousValue = _post;
                    _post = value;
                    FixupPost(previousValue);
                }
            }
        }
        private Post _post;
    
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
    
        private bool _settingFK = false;
    
        private void FixupPost(Post previousValue)
        {
            if (previousValue != null && previousValue.Alerts.Contains(this))
            {
                previousValue.Alerts.Remove(this);
            }
    
            if (Post != null)
            {
                if (!Post.Alerts.Contains(this))
                {
                    Post.Alerts.Add(this);
                }
                if (PostID != Post.ID)
                {
                    PostID = Post.ID;
                }
            }
            else if (!_settingFK)
            {
                PostID = null;
            }
        }
    
        private void FixupProfile(Profile previousValue)
        {
            if (previousValue != null && previousValue.Alerts.Contains(this))
            {
                previousValue.Alerts.Remove(this);
            }
    
            if (Profile != null)
            {
                if (!Profile.Alerts.Contains(this))
                {
                    Profile.Alerts.Add(this);
                }
                if (UserID != Profile.ID)
                {
                    UserID = Profile.ID;
                }
            }
        }

        #endregion
    }
}