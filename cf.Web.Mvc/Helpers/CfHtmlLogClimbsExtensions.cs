using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cf.Entities;
using cf.Caching;
using cf.Entities.Enum;

namespace cf.Web.Mvc.Helpers
{
    public static class CfHtmlLogClimbsExtensions
    {
        public static MvcHtmlString OutcomeImage(this HtmlHelper helper, byte outcome)
        {
            ClimbOutcome eOutcome = (ClimbOutcome)outcome;
            return new MvcHtmlString(string.Format(@"<img src=""{0}/climbed/{1}.png"" class=""outcome"" />", Stgs.StaticRt, eOutcome));
        }

        public static MvcHtmlString ExperienceImage(this HtmlHelper helper, byte experience)
        {
            ClimbExperience eExperience = (ClimbExperience)experience;
            return new MvcHtmlString(string.Format(@"<img src=""{0}/climbed/{1}.png"" class=""experience"" />", Stgs.StaticRt, eExperience));
        }
    }
}