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
    public partial class LocationEF
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
    
        public virtual byte CountryID
        {
            get;
            set;
        }
    
        public virtual string Name
        {
            get;
            set;
        }
    
        public virtual string NameShort
        {
            get;
            set;
        }
    
        public virtual string NameUrlPart
        {
            get;
            set;
        }
    
        public virtual string SearchSupportString
        {
            get;
            set;
        }
    
        public virtual string Description
        {
            get;
            set;
        }
    
        public virtual string Avatar
        {
            get;
            set;
        }
    
        public virtual double Latitude
        {
            get;
            set;
        }
    
        public virtual double Longitude
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
    
        public virtual ICollection<Climb> Climbs
        {
            get
            {
                if (_climbs == null)
                {
                    var newCollection = new FixupCollection<Climb>();
                    newCollection.CollectionChanged += FixupClimbs;
                    _climbs = newCollection;
                }
                return _climbs;
            }
            set
            {
                if (!ReferenceEquals(_climbs, value))
                {
                    var previousValue = _climbs as FixupCollection<Climb>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupClimbs;
                    }
                    _climbs = value;
                    var newValue = value as FixupCollection<Climb>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupClimbs;
                    }
                }
            }
        }
        private ICollection<Climb> _climbs;
    
        public virtual ICollection<LocationSection> LocationsSections
        {
            get
            {
                if (_locationsSections == null)
                {
                    var newCollection = new FixupCollection<LocationSection>();
                    newCollection.CollectionChanged += FixupLocationsSections;
                    _locationsSections = newCollection;
                }
                return _locationsSections;
            }
            set
            {
                if (!ReferenceEquals(_locationsSections, value))
                {
                    var previousValue = _locationsSections as FixupCollection<LocationSection>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupLocationsSections;
                    }
                    _locationsSections = value;
                    var newValue = value as FixupCollection<LocationSection>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupLocationsSections;
                    }
                }
            }
        }
        private ICollection<LocationSection> _locationsSections;
    
        public virtual ICollection<LoggedClimb> LoggedClimbs
        {
            get
            {
                if (_loggedClimbs == null)
                {
                    var newCollection = new FixupCollection<LoggedClimb>();
                    newCollection.CollectionChanged += FixupLoggedClimbs;
                    _loggedClimbs = newCollection;
                }
                return _loggedClimbs;
            }
            set
            {
                if (!ReferenceEquals(_loggedClimbs, value))
                {
                    var previousValue = _loggedClimbs as FixupCollection<LoggedClimb>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupLoggedClimbs;
                    }
                    _loggedClimbs = value;
                    var newValue = value as FixupCollection<LoggedClimb>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupLoggedClimbs;
                    }
                }
            }
        }
        private ICollection<LoggedClimb> _loggedClimbs;
    
        public virtual ICollection<CheckIn> CheckIns
        {
            get
            {
                if (_checkIns == null)
                {
                    var newCollection = new FixupCollection<CheckIn>();
                    newCollection.CollectionChanged += FixupCheckIns;
                    _checkIns = newCollection;
                }
                return _checkIns;
            }
            set
            {
                if (!ReferenceEquals(_checkIns, value))
                {
                    var previousValue = _checkIns as FixupCollection<CheckIn>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupCheckIns;
                    }
                    _checkIns = value;
                    var newValue = value as FixupCollection<CheckIn>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupCheckIns;
                    }
                }
            }
        }
        private ICollection<CheckIn> _checkIns;

        #endregion
        #region Association Fixup
    
        private void FixupClimbs(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Climb item in e.NewItems)
                {
                    item.Location = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Climb item in e.OldItems)
                {
                    if (ReferenceEquals(item.Location, this))
                    {
                        item.Location = null;
                    }
                }
            }
        }
    
        private void FixupLocationsSections(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (LocationSection item in e.NewItems)
                {
                    item.Location = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (LocationSection item in e.OldItems)
                {
                    if (ReferenceEquals(item.Location, this))
                    {
                        item.Location = null;
                    }
                }
            }
        }
    
        private void FixupLoggedClimbs(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (LoggedClimb item in e.NewItems)
                {
                    item.Location = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (LoggedClimb item in e.OldItems)
                {
                    if (ReferenceEquals(item.Location, this))
                    {
                        item.Location = null;
                    }
                }
            }
        }
    
        private void FixupCheckIns(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CheckIn item in e.NewItems)
                {
                    item.Location = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (CheckIn item in e.OldItems)
                {
                    if (ReferenceEquals(item.Location, this))
                    {
                        item.Location = null;
                    }
                }
            }
        }

        #endregion
    }
}
