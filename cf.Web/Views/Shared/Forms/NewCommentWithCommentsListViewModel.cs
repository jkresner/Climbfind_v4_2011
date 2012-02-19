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
    public class NewCommentWithCommentsListViewModel
    {
        public Guid PostID { get; set; }
        
        public List<PostComment> Comments { get; set; }
                
        [Required]
        public string Comment { get; set; }
    }
}