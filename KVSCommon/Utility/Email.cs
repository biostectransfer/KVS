using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Configuration;

namespace KVSCommon.Utility
{
    public class Email
    {
        /// <summary>
        /// Sendet eine Email über den angegebenen SMTP-Server.
        /// </summary>
        /// <param name="from">Absenderadresse.</param>
        /// <param name="to">Liste mit Empfaengeradressen.</param>
        /// <param name="subject">Betreff.</param>
        /// <param name="body">Inhalt der Email (Text oder HTML).</param>
        /// <param name="cc">Liste mit Empfaengeradressen (CC).</param>
        /// <param name="bcc">Liste mit Empfaengeradressen (BCC).</param>
        /// <param name="smtpServer">SMTP-Server für den Emailversand.</param>
        /// <param name="attachments">Liste mit Emailattachments.</param>
        /// <remarks><paramref name="cc"/>, <paramref name="bcc"/>, und <paramref name="attachments"/> duerfen auch null sein.</remarks>
        public static void SendMail(string from, List<string> to, string subject, string body, List<string> cc, List<string> bcc, string smtpServer, List<Attachment> attachments)
        {
            if (to == null || to.Count == 0)
            {
                throw new Exception("Der Empfänger für die Email darf nicht leer sein.");
            }

            if (string.IsNullOrEmpty(subject))
            {
                throw new Exception("Der Betreff der Email darf nicht leer sein.");
            }

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            foreach (string st in to.Distinct())
            {
                mail.To.Add(st);
            }

            if (cc != null)
            {
                foreach (string st in cc.Distinct())
                {
                    mail.CC.Add(st);
                }
            }

            if (bcc != null)
            {
                foreach (string st in bcc.Distinct())
                {
                    mail.Bcc.Add(st);
                }
            }

            if (attachments != null)
            {
                foreach (var att in attachments)
                {
                    mail.Attachments.Add(att);
                }
            }
            SmtpClient mailClient;
            
            if(!String.IsNullOrEmpty(smtpServer))
                 mailClient = new SmtpClient(smtpServer,25);
            
            else
                 mailClient = new SmtpClient();
            
            try
            {
               //mailClient.UseDefaultCredentials = true;
                string username = ConfigurationManager.AppSettings["userName"];
                string password = ConfigurationManager.AppSettings["password"];
                mailClient.Credentials = new System.Net.NetworkCredential(username, password);
               //mailClient.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis; O0J#ucoG, s58#Lek8, x$3G48ZC, %ex6C7u2 (utsch->case mail)
                mailClient.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                mail.Dispose();
                mailClient.Dispose();
            }
        }
    }
}
