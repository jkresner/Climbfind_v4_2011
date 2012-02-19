using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cf.Web.Mvc.ActionFilters;
using cf.Services;
using cf.Caching;
using cf.Entities;
using cf.Entities.Enum;
using cf.Identity;
using cf.Web.Views.Shared;
using System.Globalization;
using NetFrameworkExtensions;
using cf.Web.Models;
using cf.Dtos;
using NetFrameworkExtensions.Web.Mvc;

namespace cf.Web.Controllers
{
    [CfAuthorize]
    public class MessagesController : Controller
    {
        ConversationService convoSvc { get { if (_convoSvc == null) { _convoSvc = new ConversationService(); } return _convoSvc; } } ConversationService _convoSvc;

        public ActionResult Index() 
        {
            Guid userID = CfIdentity.UserID;
            var conversations = convoSvc.GetUsersConversations(userID).OrderByDescending(c=>c.LastActivityUtc).ToList();

            bool hasConvosToShow = conversations.SelectMany(c => c.ConversationViews).Where(cv => cv.ShouldShow).Count() > 0;
            if (!hasConvosToShow) { return View("Empty"); }
            ViewBag.Conversations = conversations;
                        
            return View(); 
        }

        public ActionResult New(Guid id)
        {
            var user = new UserService().GetProfileByID(id);
            if (user == null) { return new HttpStatusCodeWithBodyResult("ProfileNotFound", 404); }

            ViewBag.OtherParty = user;
            
            var conversation = convoSvc.GetConversationByPartyIDs(user.ID, CfIdentity.UserID);

            ViewBag.Conversation = conversation;

            return View();
        }

        public ActionResult ConversationAjax(Guid id)
        {
            var conversation = convoSvc.GetConversationById(id);

            return PartialView("MessagesList", conversation.Messages.Take(15));
        }
        
        [HttpPost]
        public ActionResult Create(NewMessageViewModel m)
        {
            if (ModelState.IsValid)
            {
                var conversation = convoSvc.SendMessage(m.ForID, m.Content);
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult Append(NewMessageViewModel m)
        {
            if (ModelState.IsValid)
            {
                var conversation = convoSvc.GetConversationById(m.ForID);
                var otherPartyID = conversation.PartyAID;
                if (CfIdentity.UserID == otherPartyID) { otherPartyID = conversation.PartyBID; }
                convoSvc.SendMessage(otherPartyID, m.Content);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(Guid id)
        {
            throw new NotImplementedException();
            //var pc = pcSvc.GetPartnerCallById(id);
            //pcSvc.DeletePartnerCall(pc);
            //return RedirectToAction("ListUser", new { id = CfIdentity.UserID });
        }
    }
}
