using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelUNRegister.Models
{
    public class Article
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "检索关键字")]
        public string Keyword { get; set; }

        [Required]
        [Display(Name = "文章标题")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "文章内容")]
        public string Content { get; set; }
    }
}