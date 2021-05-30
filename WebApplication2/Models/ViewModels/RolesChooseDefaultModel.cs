using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels
{
    public class RolesChooseDefaultModel
    {
        public List<EmployeeRole> EmployeeRoles { get; set; }
        public Employee Employee { get; set; }
        public EmployeeRole DefaultRole { get; set; }
    }
}