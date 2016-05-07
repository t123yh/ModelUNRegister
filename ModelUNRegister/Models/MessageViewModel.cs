using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using ModelUNRegister.Utilities;
using System.Reflection;

namespace ModelUNRegister.Models
{
    public enum BootstrapTheme
    {
        [Display(Name = "default")]
        Default,
        [Display(Name = "primary")]
        Primary,
        [Display(Name = "success")]
        Success,
        [Display(Name = "info")]
        Inforamtion,
        [Display(Name = "warning")]
        Warning,
        [Display(Name = "danger")]
        Danger
    }

    public class MessageViewModel
    {

        public string Title { get; set; }

        public string Message { get; set; }

        public BootstrapTheme Theme { get; set; }

        public string ThemeString
        {
            get
            {
                return Theme.GetDisplayName();
            }
        }
    }
}