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
    public partial class ClimbTag
    {
        #region Primitive Properties
    
        public virtual System.Guid ID
        {
            get;
            set;
        }
    
        public virtual System.Guid ClimbID
        {
            get { return _climbID; }
            set
            {
                if (_climbID != value)
                {
                    if (Climb != null && Climb.ID != value)
                    {
                        Climb = null;
                    }
                    _climbID = value;
                }
            }
        }
        private System.Guid _climbID;
    
        public virtual int Category
        {
            get;
            set;
        }
    
        public virtual Nullable<System.Guid> LocationID
        {
            get;
            set;
        }

        #endregion
        #region Navigation Properties
    
        public virtual Climb Climb
        {
            get { return _climb; }
            set
            {
                if (!ReferenceEquals(_climb, value))
                {
                    var previousValue = _climb;
                    _climb = value;
                    FixupClimb(previousValue);
                }
            }
        }
        private Climb _climb;

        #endregion
        #region Association Fixup
    
        private void FixupClimb(Climb previousValue)
        {
            if (previousValue != null && previousValue.ClimbTags.Contains(this))
            {
                previousValue.ClimbTags.Remove(this);
            }
    
            if (Climb != null)
            {
                if (!Climb.ClimbTags.Contains(this))
                {
                    Climb.ClimbTags.Add(this);
                }
                if (ClimbID != Climb.ID)
                {
                    ClimbID = Climb.ID;
                }
            }
        }

        #endregion
    }
}
