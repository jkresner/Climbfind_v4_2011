using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using cf.Entities;
using NetFrameworkExtensions;

namespace cf.Dtos.Mobile.V1
{
    public class ConversationListDto
    {
        public List<ConversationItemDto> Conversations { get; set; }

        public ConversationListDto()
        {
            Conversations = new List<ConversationItemDto>();
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class ConversationItemDto
    {
        public string ID { get; set; }
        public string WithName { get; set; }
        public string WithAvatar { get; set; }
        public string WithID { get; set; }
        public string LastSenderID { get; set; }
        public string LastUtc { get; set; }
        public string LastExcerpt { get; set; }

        public ConversationItemDto() { }

        public ConversationItemDto(Conversation c, IUserBasicDetail with)
        {
            ID = c.ID.ToString("N");
            LastExcerpt = c.LastExcerpt;
            LastUtc = c.LastActivityUtc.ToEpochTimeString();
            LastSenderID = c.LastSenderID.ToString("N");
            WithName = with.FullName;
            WithID = with.ID.ToString("N");
            WithAvatar = with.Avatar;
        }
    }
}
