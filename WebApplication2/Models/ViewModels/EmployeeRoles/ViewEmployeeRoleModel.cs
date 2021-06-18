using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels.EmployeeRoles
{
    public class ViewEmployeeRoleModel
    {
        [Display(Name ="الإسم")]
        public string PersonName { get; set; }
        [Display(Name = "الوظيفة")]

        public string  RoleName { get; set; }
        [Display(Name ="المؤسسة")]

        public string InstitutionName { get; set; }
        [Display(Name = "الوصف")]

        public string  Desc{ get; set; }
        [Display(Name = "تاريخ التعيين")]
         [DataType(DataType.Date)]
        public DateTime HiringDate { get; set; }

        public int EmployeeRoleID { get; set; }
        public int EmpID { get; set; }
        public int RoleID { get; set; }
        public int InstitutionID { get; set; }


        public bool canActive { get; set; }
        public bool canDeactive { get; set; }
        public bool isActive { get; set; }
        public bool canDelete { get; set; }
        public bool isDeleteable { get; set; }



        public List<File> FilesPublished { get; set; }

    }
}