using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Web;

namespace ModelUNRegister.Models
{
    public enum Gender
    {
        [Display(Name = "男")]
        Male,
        [Display(Name = "女")]
        Female,
    }

    public enum Grade
    {
        [Display(Name = "初三")]
        Junior3,
        [Display(Name = "高一")]
        Senior1,
        [Display(Name = "高二")]
        Senior2,
        [Display(Name = "高三")]
        Senior3
    }

    public class EnrollRequest
    {
        [Key]
        public Guid RequestId { get; set; }

        [Required(ErrorMessage = "请填写你的姓名。")]
        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Required(ErrorMessage = "请填写你所在的学校。")]
        [Display(Name = "所在学校")]
        public string School { get; set; }

        //[Required]
        public DateTime SubmissionTime { get; set; }

        [Required(ErrorMessage = "请填写你的电子邮件地址。")]
        [EmailAddress(ErrorMessage = "电子邮件地址格式不正确。")]
        [Display(Name = "电子邮件地址")]
        public string Email { get; set; }

        public bool EmailVerified { get; set; }

        public DateTime? EmailVerificationTime { get; set; }

        [Required(ErrorMessage = "请填写你的手机号码。")]
        [RegularExpression(@"1[3|5|7|8|][0-9]{9}", ErrorMessage = "请填写正确的手机号码。")]
        [Display(Name = "手机号码")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "请填写你的 QQ 号码。")]
        [RegularExpression(@"^[1-9][0-9]{4,9}$", ErrorMessage = "请填写正确的 QQ 号码。")]
        [Display(Name = "QQ")]
        public string QQNumber { get; set; }

        [Required(ErrorMessage = "请选择你的性别。")]
        [Display(Name = "性别")]
        public Gender? Gender { get; set; }

        [Required(ErrorMessage = "请选择你的年级。")]
        [Display(Name = "年级")]
        public Grade? Grade { get; set; }

        //[Required]
        public string IPAddress { get; set; }

    }
}