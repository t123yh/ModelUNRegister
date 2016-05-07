using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ModelUNRegister.Utilities
{
    public static class AppSettings
    {

        public static string MailAccount
        {
            get
            {
                return Setting<string>("MailAccount");
            }
        }

        public static string MailPassword
        {
            get
            {
                return Setting<string>("MailPassword");
            }
        }
        public static string SMTPServer
        {
            get
            {
                return Setting<string>("SMTPServer");
            }
        }
        public static int SMTPPort
        {
            get
            {
                return Setting<int>("SMTPPort");
            }
        }

        public static string InitalAdminAccount
        {
            get
            {
                return Setting<string>("InitalAdminAccount");
            }
        }

        public static string InitalAdminPassword
        {
            get
            {
                return Setting<string>("InitalAdminPassword");
            }
        }

        public static string InitalAdminEmail
        {
            get
            {
                return Setting<string>("InitalAdminEmail");
            }
        }

        public static bool SMTPSSL
        {
            get
            {
                return Setting<bool>("SMTPSSL");
            }
        }

        public static int DuplicateEmailResetTime
        {
            get
            {
                return Setting<int>("DuplicateEmailResetTime");
            }
        }

        private static T Setting<T>(string name)
        {
            string value = ConfigurationManager.AppSettings[name];

            if (value == null)
            {
                throw new Exception(String.Format("Could not find setting '{0}',", name));
            }

            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}