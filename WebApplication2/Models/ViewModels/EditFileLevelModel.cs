using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Models.ViewModels
{
    public class EditFileLevelModel
    {





        public int ID { get; set; }


        [Required(ErrorMessage = "هذا الحقل مطلوب")]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 3,
      ErrorMessage = "هذا الحقل يجب أن يكون بين1 و 30 حرف")]
        [Display(Name = "إسم المستوى")]
        public string levelName { get; set; }



        [Display(Name = "الوصف")]
        [StringLength(300,
     ErrorMessage = "هذا الحقل يجب أن لا يتعدى 300 حرف")]
        public string Desc { get; set; }

        public List<List<SelectListItem>> Permissions { get; set; }
        public List<int> RolesIds { get; set; }
        public List<string> RolesNames { get; set; }

    }
}