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
    public partial class ObjectMedia
    {
        #region Primitive Properties
    
        public virtual System.Guid OnOjectID
        {
            get;
            set;
        }
    
        public virtual System.Guid MediaID
        {
            get { return _mediaID; }
            set
            {
                if (_mediaID != value)
                {
                    if (Media != null && Media.ID != value)
                    {
                        Media = null;
                    }
                    _mediaID = value;
                }
            }
        }
        private System.Guid _mediaID;

        #endregion
        #region Navigation Properties
    
        public virtual Media Media
        {
            get { return _media; }
            set
            {
                if (!ReferenceEquals(_media, value))
                {
                    var previousValue = _media;
                    _media = value;
                    FixupMedia(previousValue);
                }
            }
        }
        private Media _media;

        #endregion
        #region Association Fixup
    
        private void FixupMedia(Media previousValue)
        {
            if (previousValue != null && previousValue.ObjectMedias.Contains(this))
            {
                previousValue.ObjectMedias.Remove(this);
            }
    
            if (Media != null)
            {
                if (!Media.ObjectMedias.Contains(this))
                {
                    Media.ObjectMedias.Add(this);
                }
                if (MediaID != Media.ID)
                {
                    MediaID = Media.ID;
                }
            }
        }

        #endregion
    }
}
