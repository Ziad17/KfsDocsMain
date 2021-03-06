using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels.InstitutionsModels
{
    public class EditInstitutionModel
    {
        public int ID { get; set; }
  
        [Required(ErrorMessage = "هذا الحقل مطلوب")]
        [Display(Name = "إسم المؤسسة")]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 3,
        ErrorMessage = "هذا الحقل يجب أن يكون بين 3 و 100 حرف")]
        public string InstitutionName { get; set; }
        [Display(Name = "الفاكس")]
        [DataType(DataType.PhoneNumber)]
        public string Fax { get; set; }

        [Display(Name = "البريد الإلكتروني")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "الهاتف الأساسي")]
        [DataType(DataType.PhoneNumber)]
        public string PrimaryPhone { get; set; }

        [Display(Name = "الهاتف الثانوي")]
        [DataType(DataType.PhoneNumber)]
        public string SecondaryPhone { get; set; }

        [Display(Name = "الموقع الإلكتروني")]
        [DataType(DataType.Url)]
        public string Website { get; set; }


   

    }
}