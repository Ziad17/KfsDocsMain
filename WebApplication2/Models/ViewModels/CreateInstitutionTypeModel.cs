using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels.Institutions
{
    public class CreateInstitutionTypeModel
    {



        [Required(ErrorMessage = "هذا الحقل مطلوب")]
        [Display(Name = "إسم المؤسسة")]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 3,
      ErrorMessage = "هذا الحقل يجب أن يكون بين 3 و 100 حرف")]
        public string Name { get; set; }
        [Display(Name = "الوصف")]
        [StringLength(300,
      ErrorMessage = "هذا الحقل يجب أن لا يتعدى 300 حرف")]

        public string  Desc { get; set; }



    }
}