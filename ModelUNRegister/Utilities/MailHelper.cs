using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ModelUNRegister.Utilities
{
    public class MailHelper
    {
        /// <summary>
        /// 将指定的视图渲染至字符串内。
        /// </summary>
        /// <param name="controller">要渲染视图上下文所在的控制器。</param>
        /// <param name="viewName">视图名称。</param>
        /// <param name="model">要使用的模型。</param>
        /// <returns></returns>
        public static string RenderPartialToString(Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }


        /// <summary>
        /// 异步发送电子邮件。
        /// </summary>
        /// <param name="body">邮件正文。</param>
        /// <param name="subject">邮件主题。</param>
        /// <param name="sourceDisplayName">发件人显示名。</param>
        /// <param name="recipient">邮件接收者。</param>
        /// <param name="smtpServer">SMTP 服务器地址。</param>
        /// <param name="smtpPort">SMTP 服务器端口。</param>
        /// <param name="smtpSsl">指示是否要使用 SSL。</param>
        /// <param name="sourceAddress">发件者邮件地址。</param>
        /// <param name="password">发件者密码。</param>
        public static async Task SendAsync(string body,
            string subject,
            string sourceDisplayName,
            string recipient,
            string smtpServer,
            int smtpPort,
            bool smtpSsl,
            string sourceAddress,
            string password)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(recipient, sourceDisplayName));
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