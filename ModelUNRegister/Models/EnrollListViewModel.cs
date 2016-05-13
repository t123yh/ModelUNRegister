using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ModelUNRegister.Models
{
    public class EnrollListItem
    {
        public EnrollRequest Request { get; set; }

        [Display(Name = "回答问题")]
        public int AnswerCount { get; set; }
    }
}