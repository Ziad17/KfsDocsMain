using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels
{
    public class FirstLoginModel
    {

        [Display(Name ="البريد الألكتروني")]
        [DataType(DataType.EmailAddress,ErrorMessage ="برجاء إدخال بريد إلكتروني صحيح")]
        [Required(ErrorMessage ="البريد الإلكتروني مطلوب")]
        public string Email { get; set; }
        [Required(ErrorMessage ="الرقم الأكاديمي مطلوب")]
        [MinLength(length:16,ErrorMessage ="الرقم الأكاديمي لابد أن يكون 16 رقم")]
        [MaxLength(length: 16, ErrorMessage = "الرقم الأكاديمي لابد أن يكون 16 رقم")]
        public string AcadmicNumber { get; set; }

        [MinLength(length: 8, ErrorMessage = "كلمة السر يجب أن تكون على الأفل 8 أحرف أو أرقام")]
        [Required(ErrorMessage = "كلمة السر مطلوبة")]
        [DataType(DataType.Password)]

        public string Password { get; set; }

        [MinLength(length: 8, ErrorMessage = "كلمة السر يجب أن تكون على الأفل 8 أحرف أو أرقام")]
        [Required(ErrorMessage = "كلمة السر مطلوبة")]
        [DataType(DataType.Password)]
        public string  ConfirmPassword { get; set; }
    }
}