using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Models.ViewModels.InstitutionModels
{
    public class CreateInstitutionModel
    {
        public int ParentID { get; set; }
        public string ParentName { get; set; }
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

        public List<SelectListItem> InstitutionTypes { get; set; }

        [Display(Name = "نوع المؤسسة")]
        [Required(ErrorMessage = "هذا الحقل مطلوب")]

        public int InstitutionTypeID { get; set; }

    }
}