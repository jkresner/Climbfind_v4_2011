using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cf.Entities.Enum;

namespace cf.Web.Views.Shared.Partials
{
    public interface INewMediaViewModel
    {
        string Title { get; set; }
        MediaType Type { get; set; }
        string Content { get; set; }
        bool ChooseFromExisting { get; set; }
    }
}