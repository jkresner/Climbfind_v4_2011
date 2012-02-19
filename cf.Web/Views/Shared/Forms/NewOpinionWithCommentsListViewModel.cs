using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using cf.Entities;
using cf.Entities.Enum;
using cf.Entities.Interfaces;

namespace cf.Web.Models
{
    public class NewOpinionWithCommentsListViewModel
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public Guid TargetID { get; set; }
        public Guid TargetOwnerID { get; set; }

        public List<IOpinion> Opinions { get; set; }
        
        [Required]
        [Range(1, 5, ErrorMessage="Rating required")]
        public byte Rating { get; set; }

        [Required]
        public string Comment { get; set; }


    }
}