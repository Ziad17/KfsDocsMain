using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Models.ViewModels
{
    public class EditRoleModel
    {
        public int ID { get; set; }
        public List<SelectListItem> AvailablePersonPermissions { get; set; }
        public List<SelectListItem> AvailableInstitutionPermissions { get; set; }
        [Required(ErrorMessage = "هذا الحقل مطلوب")]
        [Display(Name = "إسم الوظيفة")]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 3,
        ErrorMessage = "هذا الحقل يجب أن يكون بين 3 و 50 حرف")]
        public string RoleName { get; set; }

    }
}