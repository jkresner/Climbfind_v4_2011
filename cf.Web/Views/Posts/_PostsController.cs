using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cf.Entities;
using cf.Entities.Enum;
using cf.DataAccess.Repositories;
using cf.Web.Models;
using cf.Web.Views.Moderate;
using cf.Caching;
using cf.Services;
using cf.Web.Mvc.ActionFilters;
using cf.Identity;
using cf.Dtos;
using cf.Web.Views.Shared;


namespace cf.Web.Controllers
{
    [CfAuthorize]
    public class PostsController : Controller
    {
        PostService postSvc { get { if (_postSvc == null) { _postSvc = new PostService(); } return _postSvc; } } PostService _postSvc;
        
        public ActionResult MyFeed() 
        {
            var email = ControllerContext.HttpContext.User.Identity.Name;
            var user = new UserService().GetProfileByID(CfIdentity.UserID);
            ViewBag.User = user;

            var result = postSvc.GetUsersFeed(user.ID,  PostType.Unknown, ClientAppType.CfWeb);
            
            ViewBag.Posts = result.Posts;
            ViewBag.PlaceFilters = result.Places;

            return View(); 
        }

        public ActionResult PlaceAjaxRefresh(Guid id)
        {
            var posts = new List<PostRendered>();
            var postType = GetPostTypeFromQueryString();

            if (id == Guid.Empty)
            {
                posts = postSvc.GetPostForEverywhere(postType, ClientAppType.CfWeb);
            }
            else if (id == Stgs.MyFeedID)
            {
                posts = postSvc.GetUsersFeed(CfIdentity.UserID, postType, ClientAppType.CfWeb).Posts;
            }
            else
            {
                var place = AppLookups.GetCacheIndexEntry(id);

                if (place.Type.ToPlaceCateogry() == PlaceCategory.Area)
                {
                    posts = postSvc.GetPostForArea(id, postType, ClientAppType.CfWeb);
                }
                else
                {
                    posts = postSvc.GetPostForLocation(id, postType, ClientAppType.CfWeb);
                }
            }

            return PartialView("Partials/FeedPostList", new FeedPostListViewData() { FeedPosts = posts, UserHasDeletePostRights = CfPrincipal.IsGod() });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private PostType GetPostTypeFromQueryString()
        {
            if (Request.QueryString["ptype"] == null) { return PostType.Unknown; }
            else
            {
                try
                {
                    var type = (PostType)Enum.Parse(typeof(PostType), Request.QueryString["ptype"].ToString());
                    return type;
                }
                catch
                {
                    return PostType.Unknown; 
                }
            }
        }
        
        public ActionResult Detail(Guid id)
        {
            var post = postSvc.GetPostByID(id);

            if (post == null) { return View("PageNotFound"); }

            ViewBag.Post = post;

            var type = (PostType)post.TypeID;

            if (type == PostType.Visit) { return RedirectToAction("Detail", "Visits", new { id = id }); }
            if (type == PostType.ContentAdd) { return RedirectToAction("ActionPlaceList", "Moderate", new { id = id }); }
            if (type == PostType.PartnerCall) { return RedirectToAction("Detail", "PartnerCalls", new { id = id }); }
            if (type == PostType.Opinion) { return RedirectToAction("Detail", "Opinions", new { id = id }); }
            else
            {
                throw new Exception("Post detail does not yet support " + type.ToString());
            }
        }

        [HttpPost, CfAuthorize]
        public ActionResult NewComment(NewCommentWithCommentsListViewModel m)
        {
            var post = postSvc.GetPostByID(m.PostID);
            
            if (ModelState.IsValid && post != null)
            {
                try
                {
                    var comment = postSvc.CreateComment(post, m.Comment);
                    return Json(new { Success = true, ID = comment.ID });
                }
                catch (Exception ex) 
                {
                    return Json(new { Success = false, Error = ex.Message });
                }
            }
            else
            {
                return Json(new { Success = false });
            }
        }

        [HttpPost, CfAuthorize]
        public ActionResult DeleteComment(Guid id, Guid commentID)
        {
            postSvc.DeleteComment(id, commentID); 
            return Json(new { Success = false });
        }


        [HttpPost,CfAuthorize]
        public ActionResult NewTalk(TalkViewModel m)
        {
            if (ModelState.IsValid)
            {
                postSvc.CreateTalkPost(m.TalkPlaceID, m.TalkComment);
                return RedirectToAction("TalkList", new { id = m.TalkPlaceID });
            }
            else
            {
                return View();
            }
        }

        public ActionResult TalkList(Guid id)
        {
            ViewBag.Posts = new List<Post>();
            return View();
        }

        public ActionResult Delete(Guid id)
        {
            postSvc.DeletePost(postSvc.GetPostByID(id));

            return RedirectToAction("MyFeed");
        }
    }
}

