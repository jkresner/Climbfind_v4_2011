using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cf.Entities.Interfaces;
using cf.Entities;

namespace cf.Web.Models
{
    public class SideOpinionsListModel
    {
        public IRatableGeo Object { get; set; }
        public List<Opinion> Opinions { get; set; }

        public SideOpinionsListModel(IRatableGeo obj, List<Opinion> opinions)
        {
            Object = obj;
            Opinions = opinions;
        }
    }
}