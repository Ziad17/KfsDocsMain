using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Models.ViewModels.EmployeeRoles
{
    public class CreateEmployeeRoleModel
    {
        public List<SelectListItem> Employees { get; set; }
        public List<SelectListItem> Roles { get; set; }
        public List<SelectListItem> Institutions { get; set; }

        public int EmployeeID { get; set; }
        public int RoleID { get; set; }
        public string ArabicJobDesc { get; set; }
        public int InstitutionID { get; set; }
        public DateTime HiringDate { get; set; }

    }
}