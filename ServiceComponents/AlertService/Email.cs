using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

namespace SF.Expand.Switch.ServiceComponents
{
    public static class Email
    {
        /// <summary>
        /// </summary>
        /// <param name="emailTo">email recipients separated by ;</param>
        /// <param name="Subject"></param>
        /// <param name="emailMessage"></param>
        /// <param name="SmtpHost"></param>
        public static void Send(String emailTo, String emailSubject, String emailMessage)
        {
            SmtpClient client = new SmtpClient();

            MailMessage message = new MailMessage();
            message.Subject = emailSubject;
            String[] v = emailTo.Split(';');

            for (int i = 0; i <= v.Length - 1; i++)
            {
                message.To.Add(new MailAddress(v[i].ToString()));
            }

            message.Body = emailMessage;
            message.IsBodyHtml = true;

            client.EnableSsl = false;
            client.Send(message);
        }

    }
}
