using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ModelUNRegister.Models
{
    public class CourseSelectionViewModel
    {
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Guid> SelectedCourses { get; set; }
    }
}