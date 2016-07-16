using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ModelUNRegister.Models
{
    public class HomePageLink
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [Display(Name = "排序序号")]
        public int Index { get; set; }

        [Required]
        [Display(Name = "名称")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "简介")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "图标")]
        public string Icon { get; set; }

        [Required]
        [Display(Name = "链接")]
        public string Link { get; set; }
    }

    
}