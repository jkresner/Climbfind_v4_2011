using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities.Interfaces;
using cf.Entities;
using NetFrameworkExtensions;

namespace cf.Dtos.Mobile.V1
{
    public class ConversationDetailDto
    {
        public string ID { get; set; }
        public string WithName { get; set; }
        public string WithAvatar { get; set; }
        public string WithID { get; set; }
        public string MeName { get; set; }
        public string MeAvatar { get; set; }
        public string LastSenderID { get; set; }
        public string LastUtc { get; set; }
        public string LastExcerpt { get; set; }
        public List<ConversationMessageDto> Messages { get; set; }

        public ConversationDetailDto() { }

        public ConversationDetailDto(Conversation c, IUserBasicDetail with, IUserBasicDetail me)
        {
            ID = c.ID.ToString("N");
            LastExcerpt = c.LastExcerpt;
            LastUtc = c.LastActivityUtc.ToEpochTimeString();
            LastSenderID = c.LastSenderID.ToString("N");
            WithName = with.DisplayName;
            WithID = with.ID.ToString("N");
            WithAvatar = with.Avatar;
            MeName = me.DisplayName;
            MeAvatar = me.Avatar;
            Messages = new List<ConversationMessageDto>();
        }
    }

    public class ConversationMessageDto
    {
        public string ID { get; set; }
        public string SenderID { get; set; }
        public string Content { get; set; }
        public string Utc { get; set; }

        public ConversationMessageDto() { }
        public ConversationMessageDto(cf.Entities.Message m) 
        {
            ID = m.ID.ToString("N");
            Content = m.Content;
            SenderID = m.SenderID.ToString("N");
            Utc = m.Utc.ToEpochTimeString();
        }
    }
}
