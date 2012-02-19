using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;

namespace cf.Entities
{
    public partial class ObjectModMeta : IGuidKeyObject 
    {
        public ObjectModMeta() { }

        public ObjectModMeta(ISearchable obj, Guid modUserID) 
        {
            ID = new Guid(obj.IDstring);
            Name = obj.Name;
            CreatedByUserID = modUserID;
            CreatedUtc = DateTime.UtcNow;
            CQR = 0;
            VerifiedDetails = 0;
            VerifiedAvatar = 0;

            if (modUserID == Stgs.SystemID) { CreatedUtc = new DateTime(2011, 1, 1); }

            //-- JSK 08.29 - not really sure what I was doing here...
            CreatedActionID = new Guid("00000000-0000-0000-0000-000000000001");
        }

        public bool HasBeenVerified { get { return VerifiedLastUtc.HasValue; } }

        private DateTime _lastChangedUtc;
        public DateTime LastChangedUtc { get { if (_lastChangedUtc == default(DateTime)) { SetLastChangedDetails(); } return _lastChangedUtc; } }
        private string _lastChangeType;
        public string LastChangeType { get { if (string.IsNullOrWhiteSpace(_lastChangeType)) { SetLastChangedDetails(); } return _lastChangeType; } }
        private Guid _lastChangedUserID, _lastChangedActionID;
        public Guid LastChangedUserID { get { if (_lastChangedUserID == default(Guid)) { SetLastChangedDetails(); } return _lastChangedUserID; } }
        public Guid LastChangedActionID { get { 
            if (_lastChangedActionID == default(Guid) 
                ) { SetLastChangedDetails(); } return _lastChangedActionID; } }


        private void SetLastChangedDetails()
        {
            _lastChangedUtc = CreatedUtc; _lastChangeType = "Created"; _lastChangedUserID = CreatedByUserID; _lastChangedActionID = CreatedActionID;

            if (DetailsLastChangedUtc.HasValue && DetailsLastChangedUtc > _lastChangedUtc)
            {
                _lastChangedUtc = DetailsLastChangedUtc.Value; _lastChangeType = "Details edited"; _lastChangedUserID = DetailsLastChangedByUserID.Value; _lastChangedActionID = DetailsLastChangedActionID.Value;
            }

            if (AvatarLastChangedUtc.HasValue && AvatarLastChangedUtc > _lastChangedUtc)
            {
                _lastChangedUtc = AvatarLastChangedUtc.Value; _lastChangeType = "Map image changed"; _lastChangedUserID = AvatarLastChangedByUserID.Value; _lastChangedActionID = AvatarLastChangedActionID.Value;
            }

            if (VerifiedLastUtc.HasValue && VerifiedLastUtc > _lastChangedUtc)
            {
                _lastChangedUtc = VerifiedLastUtc.Value; _lastChangeType = "Deleted"; _lastChangedUserID = DeletedByUserID.Value; _lastChangedActionID = DeletedActionID.Value;
            }
        }
    }
}
