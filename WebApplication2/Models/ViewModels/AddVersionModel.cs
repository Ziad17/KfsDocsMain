using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels
{
    public class AddVersionModel
    {




 
        public string InstitutionName { get; set; }


        public string RoleName { get; set; }




        public int FileID { get; set; }

        [Display(Name = "رقم النسخة")]
        public string Number { get; set; }

        [Display(Name = "الملاحظات")]
        public string Notes { get; set; }

        [Required(ErrorMessage = "لا يمكن أن يكون الملف فارغا")]

        [Display(Name ="أختيار ملف")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase file_content { get; set; }

        [Required(ErrorMessage = "هذا الحقل مطلوب")]

        [Display(Name = "تاريخ الإنشاء")]
        [DataType(DataType.DateTime)]
        public System.DateTime DateCreated { get; set; }

        [Required(ErrorMessage = "برجاء كتابة أسم النسخة")]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 3,
        ErrorMessage = "هذا الحقل يجب أن يكون بين 3 و 50 حرف حرف أو رقم")]
        [Display(Name = "أسم النسخة")]
        public string Name { get; set; }
        public string FileName { get; set; }
        public string AuthorName { get; set; }
    }
}