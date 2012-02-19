using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using cf.Entities.Enum;

namespace cf.Entities
{
    public partial class ConversationView : IGuidKeyObject 
    {
        public bool ShouldShow
        {
            get
            {
                var status = (ConversationStatus)Status;
                
                return status != ConversationStatus.Blocked && 
                       status != ConversationStatus.Spam && 
                       status != ConversationStatus.AllDelete;
            }
        }
    }
}
