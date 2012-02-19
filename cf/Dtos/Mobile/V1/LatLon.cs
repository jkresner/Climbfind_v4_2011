using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cf.Entities.Enum;

namespace cf.Dtos.Mobile.V0
{
    public class AppClient 
    { 
        public int ClientAppID { get; set; }
        public ClientAppType Client { get; set; } 
        
        public bool Inflate()
        {
            var appClientStr = HttpContext.Current.Request.Headers["AppClientID"];
            if (appClientStr == null) { return false; }

            try
            {
                var ClientAppID = (int)(ClientAppType)int.Parse(appClientStr);
               
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}