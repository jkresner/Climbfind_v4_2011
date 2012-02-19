using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.DataAccess.EntityFramework;
using Message = cf.Entities.Message;
using cf.DataAccess.Interfaces;

namespace cf.DataAccess.Repositories
{
    /// <summary>
    /// Conversation reader / writer
    /// </summary>
    internal class MessageRepository : AbstractCfEntitiesEf4DA<Message, Guid>,
        IKeyEntityAccessor<Message, Guid>, IKeyEntityWriter<Message, Guid>
    {
        public MessageRepository() : base() { }
        public MessageRepository(string connectionStringKey) : base(connectionStringKey) { }
    }
}
