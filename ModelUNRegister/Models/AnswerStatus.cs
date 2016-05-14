using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModelUNRegister.Models
{
    public class AnswerStatus
    {
        public int AnsweredCount { get; set; }
        public int TotalQuestionCount { get; set; }

        public AnswerStatus(int ans, int qs)
        {
            AnsweredCount = ans;
            TotalQuestionCount = qs;
        }
    }
}