using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Models.ViewModels.Files
{
    public class CreateFileWithVersionModel
    {
        [Required(ErrorMessage = "هذا الحقل مطلوب")]
        [Display(Name = "إسم الملف")]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 3,
   ErrorMessage = "هذا الحقل يجب أن يكون بين 3 و 50 حرف")]
        public string Name { get; set; }
        public SelectList Levels { get; set; }
        public int LevelID { get; set; }
        public string InstitutionName { get; set; }
        public string RoleName { get; set; }
        public string AuthorName { get; set; }


        [Required(ErrorMessage = "هذا الحقل مطلوب")]

        [Display(Name = "تاريخ الإنشاء")]
        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }
        public List<SelectListItem> AvailableEmployees { get; set; }

        public int RoleID { get; set; }
        public int InstitutionID { get; set; }








        public int FileID { get; set; }

        [Display(Name = "رقم النسخة")]
        public string Number { get; set; }

        [Display(Name = "الملاحظات")]
        public string Notes { get; set; }

        [Required(ErrorMessage = "لا يمكن أن يكون الملف فارغا")]

        [Display(Name = "أختيار ملف")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase file_content { get; set; }




        [Required(ErrorMessage = "برجاء كتابة أسم النسخة")]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 3,
        ErrorMessage = "هذا الحقل يجب أن يكون بين 3 و 50 حرف حرف أو رقم")]
        [Display(Name = "أسم النسخة")]
        public string VersionName { get; set; }
    }                                                  
}