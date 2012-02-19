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
    public partial class ModAction
    {
        #region Primitive Properties
    
        public virtual System.Guid ID
        {
            get;
            set;
        }
    
        public virtual int TypeID
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
                    if (ModProfile != null && ModProfile.ID != value)
                    {
                        ModProfile = null;
                    }
                    _userID = value;
                }
            }
        }
        private System.Guid _userID;
    
        public virtual System.Guid OnObjectID
        {
            get;
            set;
        }
    
        public virtual byte Points
        {
            get;
            set;
        }
    
        public virtual System.DateTime Utc
        {
            get;
            set;
        }
    
        public virtual string Description
        {
            get;
            set;
        }
    
        public virtual string Data
        {
            get;
            set;
        }
    
        public virtual string Comment
        {
            get;
            set;
        }

        #endregion
        #region Navigation Properties
    
        public virtual ICollection<ObjectModMeta> ObjectModMetas
        {
            get
            {
                if (_objectModMetas == null)
                {
                    var newCollection = new FixupCollection<ObjectModMeta>();
                    newCollection.CollectionChanged += FixupObjectModMetas;
                    _objectModMetas = newCollection;
                }
                return _objectModMetas;
            }
            set
            {
                if (!ReferenceEquals(_objectModMetas, value))
                {
                    var previousValue = _objectModMetas as FixupCollection<ObjectModMeta>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupObjectModMetas;
                    }
                    _objectModMetas = value;
                    var newValue = value as FixupCollection<ObjectModMeta>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupObjectModMetas;
                    }
                }
            }
        }
        private ICollection<ObjectModMeta> _objectModMetas;
    
        public virtual ICollection<ObjectModMeta> ObjectModMetas1
        {
            get
            {
                if (_objectModMetas1 == null)
                {
                    var newCollection = new FixupCollection<ObjectModMeta>();
                    newCollection.CollectionChanged += FixupObjectModMetas1;
                    _objectModMetas1 = newCollection;
                }
                return _objectModMetas1;
            }
            set
            {
                if (!ReferenceEquals(_objectModMetas1, value))
                {
                    var previousValue = _objectModMetas1 as FixupCollection<ObjectModMeta>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupObjectModMetas1;
                    }
                    _objectModMetas1 = value;
                    var newValue = value as FixupCollection<ObjectModMeta>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupObjectModMetas1;
                    }
                }
            }
        }
        private ICollection<ObjectModMeta> _objectModMetas1;
    
        public virtual ICollection<ObjectModMeta> ObjectModMetas2
        {
            get
            {
                if (_objectModMetas2 == null)
                {
                    var newCollection = new FixupCollection<ObjectModMeta>();
                    newCollection.CollectionChanged += FixupObjectModMetas2;
                    _objectModMetas2 = newCollection;
                }
                return _objectModMetas2;
            }
            set
            {
                if (!ReferenceEquals(_objectModMetas2, value))
                {
                    var previousValue = _objectModMetas2 as FixupCollection<ObjectModMeta>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupObjectModMetas2;
                    }
                    _objectModMetas2 = value;
                    var newValue = value as FixupCollection<ObjectModMeta>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupObjectModMetas2;
                    }
                }
            }
        }
        private ICollection<ObjectModMeta> _objectModMetas2;
    
        public virtual ICollection<ObjectModMeta> ObjectModMetas3
        {
            get
            {
                if (_objectModMetas3 == null)
                {
                    var newCollection = new FixupCollection<ObjectModMeta>();
                    newCollection.CollectionChanged += FixupObjectModMetas3;
                    _objectModMetas3 = newCollection;
                }
                return _objectModMetas3;
            }
            set
            {
                if (!ReferenceEquals(_objectModMetas3, value))
                {
                    var previousValue = _objectModMetas3 as FixupCollection<ObjectModMeta>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupObjectModMetas3;
                    }
                    _objectModMetas3 = value;
                    var newValue = value as FixupCollection<ObjectModMeta>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupObjectModMetas3;
                    }
                }
            }
        }
        private ICollection<ObjectModMeta> _objectModMetas3;
    
        public virtual ModProfile ModProfile
        {
            get { return _modProfile; }
            set
            {
                if (!ReferenceEquals(_modProfile, value))
                {
                    var previousValue = _modProfile;
                    _modProfile = value;
                    FixupModProfile(previousValue);
                }
            }
        }
        private ModProfile _modProfile;

        #endregion
        #region Association Fixup
    
        private void FixupModProfile(ModProfile previousValue)
        {
            if (previousValue != null && previousValue.ModActions.Contains(this))
            {
                previousValue.ModActions.Remove(this);
            }
    
            if (ModProfile != null)
            {
                if (!ModProfile.ModActions.Contains(this))
                {
                    ModProfile.ModActions.Add(this);
                }
                if (UserID != ModProfile.ID)
                {
                    UserID = ModProfile.ID;
                }
            }
        }
    
        private void FixupObjectModMetas(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ObjectModMeta item in e.NewItems)
                {
                    item.ModAction = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ObjectModMeta item in e.OldItems)
                {
                    if (ReferenceEquals(item.ModAction, this))
                    {
                        item.ModAction = null;
                    }
                }
            }
        }
    
        private void FixupObjectModMetas1(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ObjectModMeta item in e.NewItems)
                {
                    item.ModAction1 = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ObjectModMeta item in e.OldItems)
                {
                    if (ReferenceEquals(item.ModAction1, this))
                    {
                        item.ModAction1 = null;
                    }
                }
            }
        }
    
        private void FixupObjectModMetas2(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ObjectModMeta item in e.NewItems)
                {
                    item.ModAction2 = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ObjectModMeta item in e.OldItems)
                {
                    if (ReferenceEquals(item.ModAction2, this))
                    {
                        item.ModAction2 = null;
                    }
                }
            }
        }
    
        private void FixupObjectModMetas3(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ObjectModMeta item in e.NewItems)
                {
                    item.ModAction3 = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ObjectModMeta item in e.OldItems)
                {
                    if (ReferenceEquals(item.ModAction3, this))
                    {
                        item.ModAction3 = null;
                    }
                }
            }
        }

        #endregion
    }
}