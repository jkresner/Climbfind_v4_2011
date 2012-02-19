using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.Repositories;
using cf.Entities;
using cf.Caching;
using cf.Identity;
using cf.Entities.Interfaces;
using cf.Entities.Enum;
using cf.Mail;

namespace cf.Services
{
    /// <summary>
    /// Manages messaging operations between users
    /// </summary>
    public partial class ConversationService : AbstractCfService
    {
        ConversationRepository convoRepo { get { if (_convoRepo == null) { _convoRepo = new ConversationRepository(); } return _convoRepo; } } ConversationRepository _convoRepo;

        public ConversationService() { }

        /// <summary>
        /// Used when needing to add (append) messages to a specific conversation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Conversation GetConversationById(Guid id) 
        { 
            var convo = convoRepo.GetByID(id); 
            if (convo.PartyBID != CfIdentity.UserID && convo.PartyAID != CfIdentity.UserID && !CfPrincipal.IsGod())
            {
                throw new AccessViolationException("Cannot retrieve conversation that you are not part of");
            }
            return convo;
        }

        /// <summary>
        /// Returns all conversations a user is involved in
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public IQueryable<Conversation> GetUsersConversations(Guid userID) 
        {
            return convoRepo.GetUsersConversations(userID); 
        }

        /// <summary>
        /// Used to return a conversation when only to ToUserID is known by the UI layer
        /// </summary>
        /// <param name="partyAID"></param>
        /// <param name="partyBID"></param>
        /// <returns></returns>
        public Conversation GetConversationByPartyIDs(Guid partyAID, Guid partyBID) 
        {
            return convoRepo.GetConversationByPartyIDs(partyAID, partyBID);
        }
        
        /// <summary>
        /// Create a new conversation when it is the first message
        /// </summary>
        /// <param name="toID"></param>
        /// <param name="excerpt"></param>
        /// <returns></returns>
        private Conversation CreateConversation(Guid toID, string excerpt)
        {
            var user = CfPerfCache.GetClimber(toID);

            if (user == null) { throw new ArgumentNullException("Cannot create conversation as no user exists for " + toID); }
            if (toID == CfIdentity.UserID) { throw new ArgumentNullException("Cannot start a conversation with yourself!"); }


            var convo = new Conversation() { ID = Guid.NewGuid(), PartyAID = CfIdentity.UserID, PartyBID = toID, 
                StartedUtc = DateTime.UtcNow, LastActivityUtc = DateTime.UtcNow, LastSenderID = CfIdentity.UserID, LastExcerpt = excerpt };
            
            //-- conversation view allows us to track the "view" from a specific users perspective and (eventually) enforce if one user wishes to block another
            convo.ConversationViews.Add( new ConversationView( ){ ID = Guid.NewGuid(), ConversationID = convo.ID, PartyID = toID, Status = (byte)ConversationStatus.Unread });  
            convo.ConversationViews.Add( new ConversationView( ){ ID = Guid.NewGuid(), ConversationID = convo.ID, PartyID = CfIdentity.UserID, Status = (byte)ConversationStatus.Read });
            
            return convoRepo.Create(convo); 
        }

        /// <summary>
        /// Send a message that basically originated from a partner call post
        /// </summary>
        /// <param name="pc"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public Message PrivatePartnerCallReply(PartnerCall pc, string content)
        {
            var msg = SendMessage(pc.UserID, content);
                    
            //-- Log reply to partner calls! (JSK - hehe breakin architecture rules :p)
            new cf.Services.PartnerCallService().LogPartnerCallMessageReply(pc, msg);
            
            return msg;
        }
        
        /// <summary>
        /// Send a message!
        /// </summary>
        /// <param name="toID"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public Message SendMessage(Guid toID, string content)
        {
            var excerpt = content;
            if (content.Length > 80) { excerpt = content.Substring(0, 80); }
            
            var conversation = GetConversationByPartyIDs(CfIdentity.UserID, toID);
            if (conversation == null) { conversation = CreateConversation(toID, excerpt); }
            else 
            {
                conversation.LastActivityUtc = DateTime.UtcNow;
                conversation.LastSenderID = CfIdentity.UserID;
                conversation.LastExcerpt = excerpt;
                convoRepo.Update(conversation);
            }
            
            var msg = new Message() { ID = Guid.NewGuid(), Content = content, SenderID = CfIdentity.UserID };
            msg.Utc = DateTime.UtcNow;
            msg.ConversationID = conversation.ID;

            msg.MessagePartyStatus.Add(new MessagePartyStatus() { ID = Guid.NewGuid(), PartyID = toID, Status = (byte)MessageStatus.Unread });
            msg.MessagePartyStatus.Add(new MessagePartyStatus() { ID = Guid.NewGuid(), PartyID = CfIdentity.UserID, Status = (byte)MessageStatus.Read });

            new MessageRepository().Create(msg);

            //-- Send off Alerts!!
            new AlertsService().EnqueMessageWorkItem(toID, CfIdentity.UserID, msg.Content);
                        
            return msg;
        }

        public void UserDeleteMessage(Conversation conversation, Message msg)
        {
            throw new NotImplementedException("UserDeleteMessage");
        }
    }
}
