using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.EntityFramework;
using Conversation = cf.Entities.Conversation;
using cf.DataAccess.Interfaces;

namespace cf.DataAccess.Repositories
{
    /// <summary>
    /// Conversation reader / writer
    /// </summary>
    internal class ConversationRepository : AbstractCfEntitiesEf4DA<Conversation, Guid>,
        IKeyEntityAccessor<Conversation, Guid>, IKeyEntityWriter<Conversation, Guid>
    {
        public ConversationRepository() : base() { }
        public ConversationRepository(string connectionStringKey) : base(connectionStringKey) { }

        public IQueryable<Conversation> GetUsersConversations(Guid userID)
        {
            return Ctx.ConversationViews.Where(cv => cv.PartyID == userID).Select(cv => cv.Conversation);
        }


        public Conversation GetConversationByPartyIDs(Guid partyAID, Guid partyBID) 
        {
            return Ctx.Conversations.Include("Messages").Include("Messages.MessagePartyStatus").Where(
                c => (c.PartyAID == partyAID && c.PartyBID == partyBID) ||
                     (c.PartyAID == partyBID && c.PartyBID == partyAID)).SingleOrDefault();
        }
    }
}
