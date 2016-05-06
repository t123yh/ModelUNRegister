using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ModelUNRegister.Models
{
    public class QuestionViewModel
    {
        public static QuestionViewModel CreateFromQuestion(EnrollQuestion question)
        {
            return new QuestionViewModel()
            {
                Description = question.Description,
                Id = question.Id,
                Title = question.Title
            };
        }

        [Required]
        public Guid Id { get; set; }

        [Display(Name = "问题")]
        public string Title { get; set; }

        [Display(Name = "描述")]
        public string Description { get; set; }
    }

    public class QuestionAnswerViewModel
    {
        [Display(Name = "回答内容")]
        public string AnswerContent { get; set; }

        public Guid? AnswerId { get; set; }

        public QuestionViewModel Question { get; set; }
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