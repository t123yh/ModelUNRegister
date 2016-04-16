using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace ModelUNRegister.Utilities
{
    public class MailHelper
    {
        /// <summary>
        /// 异步发送电子邮件。
        /// </summary>
        /// <param name="body">邮件正文。</param>
        /// <param name="subject">邮件主题。</param>
        /// <param name="recipient">邮件接收者。</param>
        /// <param name="smtpServer">SMTP 服务器地址。</param>
        /// <param name="smtpPort">SMTP 服务器端口。</param>
        /// <param name="smtpSsl">指示是否要使用 SSL。</param>
        /// <param name="sourceAddress">发件者邮件地址。</param>
        /// <param name="password">发件者密码。</param>
        public static async void SendAsync(string body,
            string subject,
            string recipient,
            string smtpServer,
            int smtpPort,
            bool smtpSsl,
            string sourceAddress,
            string password)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(recipient));
            message.From = new MailAddress(sourceAddress);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient(smtpServer, smtpPort))
            {
                var credential = new NetworkCredential
                {
                    UserName = sourceAddress,
                    Password = password
                };
                smtp.Credentials = credential;
                smtp.EnableSsl = smtpSsl;

                await smtp.SendMailAsync(message);
            }
        }
    }
}