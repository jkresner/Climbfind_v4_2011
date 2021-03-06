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
    public partial class LoggedClimb
    {
        #region Primitive Properties
    
        public virtual System.Guid ID
        {
            get;
            set;
        }
    
        public virtual System.Guid UserID
        {
            get { return _userID; }
            set
            {
                if (_userID != value)
                {
                    if (Profile != null && Profile.ID != value)
                    {
                        Profile = null;
                    }
                    _userID = value;
                }
            }
        }
        private System.Guid _userID;
    
        public virtual System.Guid CheckInID
        {
            get { return _checkInID; }
            set
            {
                if (_checkInID != value)
                {
                    if (CheckIn != null && CheckIn.ID != value)
                    {
                        CheckIn = null;
                    }
                    _checkInID = value;
                }
            }
        }
        private System.Guid _checkInID;
    
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
    
        public virtual string ClimbName
        {
            get;
            set;
        }
    
        public virtual System.DateTime Utc
        {
            get;
            set;
        }
    
        public virtual byte Outcome
        {
            get;
            set;
        }
    
        public virtual byte Experince
        {
            get;
            set;
        }
    
        public virtual Nullable<byte> GradeOpinion
        {
            get;
            set;
        }
    
        public virtual byte Rating
        {
            get;
            set;
        }
    
        public virtual string Comment
        {
            get;
            set;
        }
    
        public virtual System.Guid Denorm_LocationID
        {
            get { return _denorm_LocationID; }
            set
            {
                if (_denorm_LocationID != value)
                {
                    if (Location != null && Location.ID != value)
                    {
                        Location = null;
                    }
                    _denorm_LocationID = value;
                }
            }
        }
        private System.Guid _denorm_LocationID;

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
    
        public virtual LocationEF Location
        {
            get { return _location; }
            set
            {
                if (!ReferenceEquals(_location, value))
                {
                    var previousValue = _location;
                    _location = value;
                    FixupLocation(previousValue);
                }
            }
        }
        private LocationEF _location;
    
        public virtual CheckIn CheckIn
        {
            get { return _checkIn; }
            set
            {
                if (!ReferenceEquals(_checkIn, value))
                {
                    var previousValue = _checkIn;
                    _checkIn = value;
                    FixupCheckIn(previousValue);
                }
            }
        }
        private CheckIn _checkIn;
    
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
    
        private void FixupClimb(Climb previousValue)
        {
            if (previousValue != null && previousValue.LoggedClimbs.Contains(this))
            {
                previousValue.LoggedClimbs.Remove(this);
            }
    
            if (Climb != null)
            {
                if (!Climb.LoggedClimbs.Contains(this))
                {
                    Climb.LoggedClimbs.Add(this);
                }
                if (ClimbID != Climb.ID)
                {
                    ClimbID = Climb.ID;
                }
            }
        }
    
        private void FixupLocation(LocationEF previousValue)
        {
            if (previousValue != null && previousValue.LoggedClimbs.Contains(this))
            {
                previousValue.LoggedClimbs.Remove(this);
            }
    
            if (Location != null)
            {
                if (!Location.LoggedClimbs.Contains(this))
                {
                    Location.LoggedClimbs.Add(this);
                }
                if (Denorm_LocationID != Location.ID)
                {
                    Denorm_LocationID = Location.ID;
                }
            }
        }
    
        private void FixupCheckIn(CheckIn previousValue)
        {
            if (previousValue != null && previousValue.LoggedClimbs.Contains(this))
            {
                previousValue.LoggedClimbs.Remove(this);
            }
    
            if (CheckIn != null)
            {
                if (!CheckIn.LoggedClimbs.Contains(this))
                {
                    CheckIn.LoggedClimbs.Add(this);
                }
                if (CheckInID != CheckIn.ID)
                {
                    CheckInID = CheckIn.ID;
                }
            }
        }
    
        private void FixupProfile(Profile previousValue)
        {
            if (previousValue != null && previousValue.LoggedClimbs.Contains(this))
            {
                previousValue.LoggedClimbs.Remove(this);
            }
    
            if (Profile != null)
            {
                if (!Profile.LoggedClimbs.Contains(this))
                {
                    Profile.LoggedClimbs.Add(this);
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
