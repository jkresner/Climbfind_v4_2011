using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cf.Entities;
using NetFrameworkExtensions;
using LinqToSql_ClimberProfile = cf.DataAccess.cf3.ClimberProfile;
using Linq_PCS = cf.DataAccess.cf3.PartnerCallSubscription;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace cf.DataAccess.cf3
{
    internal class ClimberProfileDA : AbstractLinqToSqlDA<Cf3Profile, LinqToSql_ClimberProfile, Guid>
    {
        public ClimberProfileDA() : base() { }
        public ClimberProfileDA(IDATransactionContext transactionContext) : base(transactionContext) { }

        /// <summary>
        /// Execute stored proc
        /// </summary>
        public bool UpdateEmail(string oldemail, string newemail)
        {
            using (SqlConnection dbCon = new SqlConnection(Stgs.DbConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ClimbFind.ChangeUserEmail"))
                {
                    cmd.Parameters.Add("@OrginalEmail", SqlDbType.NVarChar).Value = oldemail;
                    cmd.Parameters.Add("@NewEmail", SqlDbType.NVarChar).Value = newemail;

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = dbCon;
                    dbCon.Open();

                    return cmd.ExecuteNonQuery() > 2;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        protected override Cf3Profile MapLinqTypeToOOType(LinqToSql_ClimberProfile o)
        {
            Cf3Profile o2 = new Cf3Profile();
            MapValues(o2, ClimbFindObjectExtensionMethods.GetProperyNameAndValues(o));

            return (o2);
        }

        /// <summary>
        /// Alternate get method to retrieve a profile by user's email instead of Guid UserID
        /// </summary>
        public Cf3Profile GetClimberProfile(string email)
        {
            return MapType((from c in ctx.ClimberProfiles where c.Email == email select c).SingleOrDefault());
        }

        //public Dictionary<ClimberProfile, int> GetPartnerCallEmailSubscribedUsers(List<int> placeIDs)
        //{
        //    var query = from pcs in ctx.PartnerCallSubscriptions
        //                from cp in ctx.ClimberProfiles
        //                where placeIDs.Contains(pcs.PlaceID) && pcs.Email == true && pcs.UserID == cp.ID
        //                select new KeyValuePair<ClimberProfile, int>(MapType(cp), pcs.PlaceID);

        //    //-- Gives us only one result per user so they don't get multiple email notifications
        //    //-- for one partner call when a call is made for more than one place they are suscribed to
        //    List<Guid> distinctUserIDs = new List<Guid>();
        //    Dictionary<ClimberProfile, int> dic = new Dictionary<ClimberProfile, int>();

        //    foreach (KeyValuePair<ClimberProfile, int> result in query)
        //    {
        //        if (distinctUserIDs.Contains(result.Key.ID)) { }
        //        else
        //        {
        //            dic.Add(result.Key, result.Value);
        //            distinctUserIDs.Add(result.Key.ID);
        //        }
        //    }
        //    return dic;
        //}

        public List<Cf3Profile> GetPartnerEmailSubscribedUsers(int placeID)
        {
            return MapList((from pcs in ctx.PartnerCallSubscriptions
                            from cp in ctx.ClimberProfiles
                            where placeID == pcs.PlaceID && pcs.Email == true && pcs.UserID == cp.ID
                            select cp).ToList());
        }

        /// <summary>
        /// Removed a user and all assocaited data from the database
        /// </summary>
        public void DeleteUserCompletely(Guid userID)
        {
            //DeleteAllRelatedPartnersCallsAndReplies(userID);

            //-- Delete all posts and associated FK comments will go too from Cascade rule.
            //new FeedClimberChannelRequestDA().DeleteAllRequestsForUser(userID);

            //List<FeedPostComment> comments = new FeedPostCommentDA().GetUsersComments(userID);
            //foreach (FeedPostComment c in comments) { new FeedPostCommentDA().Delete(c.ID); }

            //List<FeedClimbingPost> posts = new FeedClimbingPostDA().GetUsersPosts(userID);
            //foreach (FeedClimbingPost p in posts) { new FeedClimbingPostDA().Delete(p.ID); }

            //ctx.Feedbacks.DeleteAllOnSubmit(from c in ctx.Feedbacks where c.UserID == userID select c);

            //-- Will have to build something smart here to delete a group and all it's associated childen...          
            //ctx.PartnerCalls.DeleteAllOnSubmit(from c in ctx.PartnerCalls where c.ClimberProfileID == userID select c);
            //ctx.PartnerCallSubscriptions.DeleteAllOnSubmit(from c in ctx.PartnerCallSubscriptions where c.UserID == userID select c);

            //ctx.PlaceUserClimbs.DeleteAllOnSubmit(from c in ctx.PlaceUserClimbs where c.UserID == userID select c);
            //ctx.UserMessages.DeleteAllOnSubmit(from c in ctx.UserMessages where c.ReceivingUserID == userID || c.SendingUserID == userID select c);

            //ctx.UserSettings.DeleteAllOnSubmit(from c in ctx.UserSettings where c.ID == userID select c);
            //ctx.MessageBoardMessages.DeleteAllOnSubmit(from c in ctx.MessageBoardMessages where c.UserID == userID select c);
            //ctx.ClimberProfileExtendeds.DeleteAllOnSubmit(from c in ctx.ClimberProfileExtendeds where c.ID == userID select c);

            ctx.ClimberProfiles.DeleteAllOnSubmit(from c in ctx.ClimberProfiles where c.ID == userID select c);

            //-- TODO: consider if it is worth deleting a users images?
            //-- Deal with removing the message boards and child messages too:
            //ctx.MessageBoards.DeleteAllOnSubmit(from c in ctx.MessageBoards where c. == userID select c);

            //ctx.SubmitChanges(ConflictMode.FailOnFirstConflict);

            //Membership.DeleteUser(Membership.GetUser(userID).UserName);
        }


        //private void DeleteAllRelatedPartnersCallsAndReplies(Guid userID)
        //{
        //    //-- Delete all the replies made by the user
        //    ctx.PartnerCallReplies.DeleteAllOnSubmit(from c in ctx.PartnerCallReplies where c.ReplyingUserID == userID select c);

        //    //-- Delete all the replies to calls made by the user
        //    List<Guid> partnerCallIDsToDelete = (from c in ctx.PartnerCalls where c.ClimberProfileID == userID select c.ID).ToList();
        //    ctx.PartnerCallReplies.DeleteAllOnSubmit(from c in ctx.PartnerCallReplies where partnerCallIDsToDelete.Contains(c.PartnerCallID) select c);

        //    //-- Delete all calls made by the user
        //    ctx.PartnerCalls.DeleteAllOnSubmit(from c in ctx.PartnerCalls where c.ClimberProfileID == userID select c);
        //}
    }
}
