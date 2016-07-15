using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ModelUNRegister.Models
{
    public class Course
    {
        [Key]
        public Guid Id { get; set; }
        [Display(Name = "姓名")]
        public string Name { get; set; }
        [Display(Name = "来自")]
        public string From { get; set; }
        [Display(Name = "课程名称")]
        public string CourseName { get; set; }
        [Display(Name = "课程介绍")]
        public string CourseContent { get; set; }
    }
}