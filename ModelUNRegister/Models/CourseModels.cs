using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ModelUNRegister.Models
{
    public class Course
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "导师")]
        public string Name { get; set; }

        [Display(Name = "来自")]
        [Required]
        public string From { get; set; }

        [Display(Name = "课程名称")]
        [Required]
        public string CourseName { get; set; }

        [Display(Name = "课程介绍")]
        [Required]
        [AllowHtml]
        public string CourseContent { get; set; }

        [Display(Name = "授课语言")]
        [Required]
        public string Language { get; set; }

        [Display(Name = "选课学生")]
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}