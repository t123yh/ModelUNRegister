using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModelUNRegister.Models
{
    public class EmailVerificationViewModel
    {
        public string Name { get; set; }

        public string EmailConfirmationLink { get; set; }

        public List<EnrollQuestion> Questions { get; set; }
    }
}