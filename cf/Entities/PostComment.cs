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
    public partial class PostComment
    {
        #region Primitive Properties
    
        public virtual System.Guid ID
        {
            get;
            set;
        }
    
        public virtual System.Guid PostID
        {
            get { return _postID; }
            set
            {
                if (_postID != value)
                {
                    if (Post != null && Post.ID != value)
                    {
                        Post = null;
                    }
                    _postID = value;
                }
            }
        }
        private System.Guid _postID;
    
        public virtual System.Guid UserID
        {
            get;
            set;
        }
    
        public virtual string Message
        {
            get;
            set;
        }
    
        public virtual System.DateTime Utc
        {
            get;
            set;
        }
    
        public virtual Nullable<double> Rating
        {
            get;
            set;
        }
    
        public virtual int RatingCount
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

        #endregion
        #region Association Fixup
    
        private void FixupPost(Post previousValue)
        {
            if (previousValue != null && previousValue.PostComments.Contains(this))
            {
                previousValue.PostComments.Remove(this);
            }
    
            if (Post != null)
            {
                if (!Post.PostComments.Contains(this))
                {
                    Post.PostComments.Add(this);
                }
                if (PostID != Post.ID)
                {
                    PostID = Post.ID;
                }
            }
        }

        #endregion
    }
}
