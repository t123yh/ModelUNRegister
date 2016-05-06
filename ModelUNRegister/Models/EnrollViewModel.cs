using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ModelUNRegister.Models
{
    public class EnrollViewModel
    {
        public static EnrollViewModel CreateFromUser(ApplicationUser user)
        {
            EnrollViewModel model = new EnrollViewModel()
            {
                Name = user.ActualName,
                School = user.EnrollRequest.School,
                SubmissionTime = user.EnrollRequest.SubmissionTime,
                Email = user.Email,
                EmailVerificationTime = user.EnrollRequest.EmailVerificationTime,
                PhoneNumber = user.PhoneNumber,
                QQNumber = user.EnrollRequest.QQNumber,
                Gender = user.EnrollRequest.Gender,
                Grade = user.EnrollRequest.Grade
            };
            return model;
        }

        [Required(ErrorMessage = "请填写你的姓名。")]
        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Required(ErrorMessage = "请填写你所在的学校。")]
        [Display(Name = "所在学校")]
        public string School { get; set; }

        [Display(Name = "提交时间")]
        public DateTime SubmissionTime { get; set; }

        [Required(ErrorMessage = "请填写你的电子邮件地址。")]
        [EmailAddress(ErrorMessage = "电子邮件地址格式不正确。")]
        [Display(Name = "电子邮件地址")]
        public string Email { get; set; }

        [Display(Name = "Email 验证时间")]
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
    }
}