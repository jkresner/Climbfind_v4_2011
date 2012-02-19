using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace cf.Web.Models
{
    public class NewMessageViewModel
    {
        /// <summary>
        /// The message pay be direct to a user, or it could be to something like a partner call
        /// </summary>
        [Required]
        public Guid ForID { get; set; }

        [Required]
        public string Content { get; set; }

        public string Controller { get; set; }
        public string Action { get; set; }

        public NewMessageViewModel() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="forID"></param>
        public NewMessageViewModel(Guid forID)
        {
            ForID = forID;
            //-- Default Action/Controller is to a user
            Controller = "Messages";
            Action = "Create";
        }
    }
}