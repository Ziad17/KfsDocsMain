using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels.EmployeeRoles
{
    public class EmployeeRoleIndexModel
    {
        public List<EmployeeRole> EmployeeRoles { get; set; }
        public bool canAdd  { get; set; }
        public bool canView { get; set; }

    }
}