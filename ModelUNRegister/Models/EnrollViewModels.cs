using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ModelUNRegister.Models
{
    public class EnrollViewModel
    {
        [Required]
        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "所在学校")]
        public string School { get; set; }

        [Required]
        [Display(Name = "自我介绍")]
        public string SelfIntroduction { get; set; }
    }
}