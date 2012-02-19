using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace cf.Mail
{
    public class cfEmail
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public MailAddress To { get; set; }
        public MailAddress From { get; set; }
        public MailAddress ReplyTo { get; set; }

        public cfEmail(string subject, string body, MailAddress to, MailAddress from)
        {
            Subject = subject;
            Body = body;
            To = to;
            From = from;
            ReplyTo = from;
        }

        public cfEmail(string subject, string body, MailAddress to, MailAddress from, MailAddress replyTo)
            : this(subject, body, to, from)
        {
            ReplyTo = replyTo;
        }
    }
}
