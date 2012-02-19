using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cf.Entities.Enum;
using cf.Web.Views.Shared.Partials;

namespace cf.Web.Views.Profiles
{
    public class PersonalityMediaNewViewModel : INewMediaViewModel
    {
        public PersonalityCategory Category { get; set; }
        public MediaType Type { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool ChooseFromExisting { get; set; }
    }
}