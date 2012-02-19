using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using cf.Services;
using cf.Entities;
using cf.Caching;
using cf.Identity;
using cf.DataAccess.Repositories;
using Microsoft.IdentityModel.Claims;

namespace cf.Test
{
    [TestClass]
    public class PartnerCallAlertsServerTests
    {
        //PartnerCallRepository pcRepo { get { if (_pcRepo == null) { _pcRepo = new PartnerCallRepository(); } return _pcRepo; } } PartnerCallRepository _pcRepo;
        //PartnerCallSubscriptionRepository pcsRepo { get { if (_pcsRepo == null) { _pcsRepo = new PartnerCallSubscriptionRepository(); } return _pcsRepo; } } PartnerCallSubscriptionRepository _pcsRepo;
        //pcWorkRepository pcWRepo { get { if (_pcWRepo == null) { _pcWRepo = new pcWorkRepository(); } return _pcWRepo; } } pcWorkRepository _pcWRepo;
        //AlertRepository alertRepo { get { if (_alertRepo == null) { _alertRepo = new AlertRepository(); } return _alertRepo; } } AlertRepository _alertRepo;
        //UserSiteSettingsRepository uStgsRepo { get { if (_uStgsRepo == null) { _uStgsRepo = new UserSiteSettingsRepository(); } return _uStgsRepo; } } UserSiteSettingsRepository _uStgsRepo;

        Guid pgSF = new Guid("1af79f00-cb1a-4a78-b322-50fb5b18127c"), cliffhanger = new Guid("7867a097-d227-4b29-8370-602218f5a668"),
            jsk = new Guid("a9646cc3-18cb-4a62-8402-5263ba8b3476");

        [TestInitialize]
        public void Initiazlize()
        {         
            CfCacheIndex.Initialize();
            CfPerfCache.Initialize();

            var cIdentity = new ClaimsIdentity(new List<Claim>() { new Claim("http://climbfind.com/claims/userid",jsk.ToString()) });
            var ids = new ClaimsIdentityCollection(new List<ClaimsIdentity>() { cIdentity });
            CfIdentity.Inject(new ClaimsPrincipal(ids));
        }

		//[TestMethod]
		//public void PostPartnerCall()
		//{
		//    //var pc = new PartnerCall() { Comment = "This is a testing call", ForIndoor = true, PlaceID = pgSF, PreferredLevel = (byte)cf.Entities.Enum.ClimbingLevelGeneral.Any,
		//     //     UserID = jsk, StartDateTime = DateTime.Now.AddDays(2) };

		//    //new PartnerCallService().CreatePartnerCall(pc);
		//}

        [TestMethod]
        public void PostPartnerCall()
        {
            var pcnWi = new PartnerCallNotificationWorkItem()
            {
                ID = Guid.NewGuid(),
                CreatedUtc = DateTime.UtcNow,
                CountryID = 245,
                OnBehalfOfUserID = jsk,
                PartnerCallID = Guid.NewGuid(),
                PlaceID = cliffhanger
            };

            pcnWi.NotificationsSent = 10;
            pcnWi.ProcessedUtc = DateTime.UtcNow;
            //pcWRepo.Create(pcnWi);
        }

    }
}
