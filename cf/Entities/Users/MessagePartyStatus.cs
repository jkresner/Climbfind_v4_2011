using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using cf.Entities.Enum;

namespace cf.Entities
{
    public partial class MessagePartyStatus : IGuidKeyObject
    {
        public bool ShouldShow
        {
            get
            {
                var status = (MessageStatus)this.Status;

                return status != MessageStatus.Delete &&
                       status != MessageStatus.Spam;
            }
        }
        
        public MessagePartyStatus() { }

        public MessagePartyStatus(Guid messageID, Guid partyID, MessageStatus status)
        {
            ID = Guid.NewGuid();
            MessageID = messageID;
            PartyID = partyID;
            Status = (byte)status;
        }
    }
}
