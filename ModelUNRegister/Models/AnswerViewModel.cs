using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ModelUNRegister.Models
{
    public class QuestionAnswerViewModel
    {
        [Required]
        [Display(Name = "回答内容")]
        public string AnswerContent { get; set; }

        public Guid? AnswerId { get; set; }

        public EnrollQuestion Question { get; set; }
    }

    public class AnswersViewModel
    {
        public List<QuestionAnswerViewModel> Answers { get; set; }

        public bool IsFullyAnswered
        {
            get
            {
                return Answers.All(ans => !string.IsNullOrWhiteSpace(ans.AnswerContent));
            }
        }
    }
}