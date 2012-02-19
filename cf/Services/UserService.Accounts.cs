using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using cf.DataAccess.Repositories;
using cf.Entities;
using cf.Content.Images;
using cf.Identity;
using cf.Entities.Enum;
using cf.Dtos;
using NetFrameworkExtensions.Net;
using System.Web.Security;
using cf.Instrumentation;
using cf.Mail;

namespace cf.Services
{
    public partial class UserService
    {
        public bool ChangeUserEmail(string oldemail, string newemail)
        {
            var success = false;
            success = new cf.DataAccess.cf3.ClimberProfileDA().UpdateEmail(oldemail, newemail);
            if (success)
            {
                var cf4profile = profileRepo.GetProfileByEmail(oldemail);
                cf4profile.Email = newemail;
                profileRepo.Update(cf4profile);
            }
            return success;
        }

        public bool DeleteUser(string email)
        {
            var usr = profileRepo.GetProfileByEmail(email);

            //-- Delete CF4 Profile
            if (usr != default(Profile))
            {
                if (CfIdentity.UserID != usr.ID && !CfPrincipal.IsGod()) { throw new AccessViolationException("Cannot delete a profile that does not belong to you."); }
                DeleteCfProfileAndRelatedData(usr);
            }

            //-- Delete CF3 Profile
            var cf3Profile = new cf.DataAccess.cf3.ClimberProfileDA().GetClimberProfile(email);
            if (cf3Profile != default(Cf3Profile))
            {
                new cf.DataAccess.cf3.ClimberProfileDA().DeleteUserCompletely(cf3Profile.ID);
            }

            //-- Delete Membership User
            var mUser = Membership.GetUser(email);
            if (mUser != default(MembershipUser))
            {
                Membership.DeleteUser(email);
            }

            return true;
        }
        
        /// <summary>
        /// Called from authenticated facebook client 'accounts.climbfind.com' and the mobile app
        /// </summary>
        /// <param name="email"></param>
        /// <param name="fullName"></param>
        /// <param name="password"></param>
        /// <param name="nationality"></param>
        /// <param name="isMale"></param>
        /// <param name="facebookID"></param>
        /// <param name="facebookToken"></param>
        /// <param name="signUpOrigin"></param>
        /// <returns></returns>
        public Profile CreateUser(string email, string fullName, string password, byte nationality, bool isMale, long? facebookID, 
            string facebookToken, string signUpOrigin)
        {
            try
            {
                bool detailsValid = true; //-- todo, perform some sort of validation on incoming details
                if (detailsValid)
                {
                    MembershipCreateStatus createStatus;
                    var mUser = Membership.CreateUser(email, password, email, null, null, true, null, out createStatus);

                    if (createStatus != MembershipCreateStatus.Success) { throw new MembershipCreateUserException(createStatus); }
                    else
                    {
                        var userID = new Guid(mUser.ProviderUserKey.ToString());

                        var user = CreateProfile(new Profile()
                        {
                            ID = userID,
                            CountryID = nationality,
                            Email = email,
                            FullName = fullName,
                            IsMale = isMale,
                            FacebookID = facebookID,
                            FacebookToken = facebookToken,
                            PrivacyAllowNewConversations = true,
                            PrivacyShowFeed = true,
                            PrivacyShowHistory = true,
                            PrivacyPostsDefaultIsPublic = true,
                            PrivacyShowInSearch = true,
                            PrivacyShowOnPartnerSites = true
                        });

                        var traceMsg = string.Format("{0} created an account via {1}", fullName, signUpOrigin);
                        if (facebookID.HasValue) { traceMsg += " w fbid: " + facebookID.Value.ToString(); }
                        CfTrace.Information(TraceCode.UserCreateAccount, traceMsg);
                        MailMan.SendAppEvent(TraceCode.UserCreateAccount, traceMsg, email, userID, "jkresner@yahoo.com.au", true);

                        try
                        {
                            if (facebookID.HasValue)
                            {                                
                                var originalImgUrl = string.Format("http://graph.facebook.com/{0}/picture?type=large", facebookID.Value);
                                using (Stream imgStream = new ImageDownloader().DownloadImageAsStream(originalImgUrl))
                                {
                                    //-- Note this function automatically updates the user object in the database
                                    SaveProfileAvatarPicFrom3rdPartSource(imgStream, user);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            CfTrace.Error(ex);
                        }

                        return user;
                    }
                }
                else
                {
                    throw new Exception("Sign up detail invalid from origin: "+ signUpOrigin);
                }
            }
            catch (Exception ex) //-- extra logging / safety as we really don't want this code to screw up and if it does be better know about it!
            {
                if (!ex.Message.Contains("form required for an e-mail address") && !ex.Message.Contains("username is already in use"))
                {
                    CfTrace.Error(ex);
                }
                throw ex;
            }
        }


        /// <summary>
        /// Used:
        /// 1) In the case when the facebook ID does not match a profile, but the user is signed in to facebook (we check if we can connect the accounts)
        /// 2) When the user logs in with their CF3 email/password to a client (Accounts Server, PG Site, CF4, Mobile App) for the first time
        /// </summary>
        public Profile GetUserByEmailAndCreateCf4ProfileIfNotExists(string email)
        {
            var profile = profileRepo.GetProfileByEmail(email);

            if (profile == null)
            {
                var cf3Profile = new cf.DataAccess.cf3.ClimberProfileDA().GetClimberProfile(email);
                if (cf3Profile != default(Cf3Profile))
                {
                    var idStr = cf3Profile.ID.ToString();
                    string userName = idStr.Substring(idStr.Length - 9, 8);

                    profile = new Profile()
                    {
                        ID = cf3Profile.ID,
                        CountryID = byte.Parse(cf3Profile.Nationality.ToString()),
                        DisplayNameTypeID = 0,
                        Email = email,
                        FullName = cf3Profile.FullName,
                        IsMale = cf3Profile.IsMale.Value,
                        NickName = cf3Profile.NickName,
                        UserName = userName,
                        ContactNumber = cf3Profile.ContractPhoneNumber,
                        PrivacyAllowNewConversations = true,
                        PrivacyShowFeed = true,
                        PrivacyShowHistory = true,
                        PrivacyPostsDefaultIsPublic = true,
                        PrivacyShowInSearch = true,
                        PrivacyShowOnPartnerSites = true
                    };

                    profileRepo.Create(profile);

                    var traceMsg = string.Format("{0} upgraded cf3 account", cf3Profile.FullName);
                    CfTrace.Information(TraceCode.UserCreateAccount, traceMsg);
                    MailMan.SendAppEvent(TraceCode.UserCreateAccount, traceMsg, email, cf3Profile.ID, "jkresner@yahoo.com.au", true);

                    try
                    {
                        var originalImgUrl = GetCf3ProfilePicFullSizeUrl(cf3Profile.ID, cf3Profile.ProfilePictureFile);
                        if (!string.IsNullOrWhiteSpace(originalImgUrl))
                        {
                            using (Stream imgStream = new ImageDownloader().DownloadImageAsStream(originalImgUrl))
                            {
                                if (imgStream == null) { throw new ArgumentException("Cf3 image stream is null for: " + originalImgUrl); }
                                if (profile == null) { throw new ArgumentException("Profile is null..."); }

                                //-- Note this function automatically updates the user object in the database
                                SaveProfileAvatarPicFrom3rdPartSource(imgStream, profile);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CfTrace.Error(ex);
                    }
                }
            }

            return profile;
        }


        public static string GetCf3ProfilePicFullSizeUrl(Guid objectID, string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || fileName == "Default.jpg") { return null; }
            return string.Format("http://cf3.climbfind.com/images/users/profiles/main/{0}/{1}", objectID.ToString().Substring(0, 3), fileName);
        }
         
        //        else
        //{
        //    var reset = existing.ResetPassword();
        //    existing.ChangePassword(reset, password);

        //    var profile = CheckCf3IdentityByEmailAndUpgradeToCf4(email);
        //    profile.FullName = fullName;
        //    profile.CountryID = nationality;
        //    profile.IsMale = isMale;
        //    profile.FacebookID = facebookID;

        //    profileRepo.Update(profile);

        //    return profile;
        //}

        public Profile GetProfileByFacebookID(long facebookID)
        {
            return profileRepo.GetAll().Where(p => p.FacebookID == facebookID).SingleOrDefault();
        }

        public Profile GetProfileByID(Guid id) { return profileRepo.GetByID(id); }
        public Profile GetProfileBySlugUrlPart(string slugUrlPart) { return profileRepo.GetProfileBySlugUrlPart(slugUrlPart); }
        public IList<Profile> GetProfilesAll() { return profileRepo.GetAll().ToList(); }
        public Profile CreateProfile(Profile obj)
        {
            return profileRepo.Create(obj);
        }
        public Profile UpdateProfile(Profile obj) { return profileRepo.Update(obj); }
        
        /// <summary>
        /// Here we do all the business 
        /// </summary>
        /// <param name="obj"></param>
        public void DeleteCfProfileAndRelatedData(Profile u) 
        { 
            //-- Delete any personality media
            var pMediaRepo = new UserPersonalityMediaRepository();
            var mediaRepo = new MediaRepository();
            var mediaOpinionRepo = new MediaOpinionRepository();
            var opinionRepo = new OpinionRepository();
            var ciRepo = new CheckInRepository();
            var pcRepo = new PartnerCallRepository();
            var postRepo = new PostRepository();
            var clmRepo = new ClimbRepository();

            mediaOpinionRepo.Delete(mediaOpinionRepo.GetAll().Where(mo => mo.UserID == u.ID).Select(mo => mo.ID));
            opinionRepo.Delete(opinionRepo.GetAll().Where(o => o.UserID == u.ID).Select(o => o.ID));          

            postRepo.Delete(postRepo.GetAll().Where(p => p.UserID == u.ID).Select(p => p.ID));          
            //-- Delete personality media
            mediaRepo.Delete(pMediaRepo.GetAll().Where(pm => pm.UserID == u.ID).Select(pm => pm.MediaID));
            
            //-- Get other media, e.g. stuff uploaded for places & climbs 
            //-- Remove all the personal tags
            mediaRepo.RemoveAllMediaTagForObject(u.ID);
            var otherMedia = mediaRepo.GetAll().Where(pm => pm.AddedByUserID == u.ID);
            foreach (var m in otherMedia) { m.AddedByUserID = Stgs.SystemID; } mediaRepo.Update(otherMedia);

            ciRepo.Delete(ciRepo.GetAll().Where(ci=>ci.UserID==u.ID).Select(ci=>ci.ID));
            pcRepo.Delete(pcRepo.GetAll().Where(pc => pc.UserID == u.ID).Select(pc => pc.ID));          

            //-- geo problems , e.g. a user ads a area, location or climb or is listed as a setter

            var usersSetClimbs = clmRepo.GetAll().Where(c => c.SetterID == u.ID);
            foreach (var c in usersSetClimbs) { c.SetterID = null; } clmRepo.Update(usersSetClimbs);

            //-- If everything else has been removed this should work....
            profileRepo.Delete(u.ID); 
        }
    }
}

