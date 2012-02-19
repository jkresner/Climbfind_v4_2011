using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Mail;

namespace cf.Mail
{
    internal static class SMTP
    {
        /// <summary>
        /// Constructor
        /// </summary>
        static SMTP() { }

        /// <summary>
        /// Base send method to decide how to handle the mail based on which environment we're in
        /// </summary>
        /// <param name="mailToSend"></param>
        private static void EnvironmentSend(cfEmail mailToSend)
        {
            if (Stgs.IsDevelopmentEnvironment)
            {
                string filePath = string.Format(Stgs.DebugMailDir + "debug.html");
                File.WriteAllText(filePath, mailToSend.Body);
            }
            else
            {
                MailMessage mail = new MailMessage(Stgs.MailMan, mailToSend.To);
                mail.ReplyToList.Add(mailToSend.ReplyTo);
                mail.Subject = mailToSend.Subject;
                mail.Body = mailToSend.Body;
                mail.IsBodyHtml = true;

                Stgs.MailServer.Send(mail);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="email"></param>
        public static void SendAppEvent(string subject, string body, string[] eventSubscribers)
        {
            foreach (var email in eventSubscribers)
            {
                EnvironmentSend(new cfEmail(subject,body,new MailAddress(email), Stgs.MailMan));
            }
        }

        public static void SendAppEvent(string subject, string body, string[] eventSubscribers, string replyTo)
        {
            foreach (var email in eventSubscribers)
            {
                EnvironmentSend(new cfEmail(subject,body,new MailAddress(email),Stgs.MailMan, new MailAddress(replyTo)));
            }
        }

        //--------------------------------------------------------------------------------//
        //- createAndSendMail is for single mail messages that need to 
        //- be sent straight away
        //--------------------------------------------------------------------------------//

        public static void PostSingleMail(cfEmail mailToSend)
        {
            EnvironmentSend(mailToSend);
        }
    }
}
