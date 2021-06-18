using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Models.ViewModels
{
    public class EditFileModel
    {
        [Required(ErrorMessage = "هذا الحقل مطلوب")]
        [Display(Name = "إسم الملف")]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 3,
      ErrorMessage = "هذا الحقل يجب أن يكون بين 3 و 50 حرف")]
        public string FileName { get; set; }
        public SelectList Levels { get; set; }
        public int LevelID { get; set; }
        public List<SelectListItem> AvailableEmployees { get; set; }
        public int ID { get; set; }
    }
}