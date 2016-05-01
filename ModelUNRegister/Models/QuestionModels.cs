﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ModelUNRegister.Models
{
    public class EnrollQuestion
    {
        [Key]
        public Guid QuestionId { get; set; }

        [Required]
        [Display(Name = "问题")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "描述")]
        public string Description { get; set; }
    }

    public class EnrollQuestionAnswer
    {
        [Key]
        public Guid AnswerId { get; set; }

        public virtual EnrollQuestion Question { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "回答内容")]
        public string AnswerContent { get; set; }
    }
}