﻿using System.Net;
using System.Net.Mail;

namespace EDUHUNT_BE.Helper
{
    public class SendMail
    {
        public static bool SendEmail(string to, string subject, string body, string attachFile)
        {
            try
            {
                MailMessage msg = new MailMessage(ConstantHelper.emailSender, to, subject, body)
                {
                    IsBodyHtml = true
                };
                using (var client = new SmtpClient(ConstantHelper.hostEmail, ConstantHelper.portEmail))
                {
                    client.EnableSsl = true;
                    if (!string.IsNullOrEmpty(attachFile))
                    {
                        Attachment attachment = new Attachment(attachFile);
                        msg.Attachments.Add(attachment);
                    }
                    NetworkCredential credential = new NetworkCredential(ConstantHelper.emailSender, ConstantHelper.passwordSender);
                    client.UseDefaultCredentials = false;
                    client.Credentials = credential;
                    client.Send(msg);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}