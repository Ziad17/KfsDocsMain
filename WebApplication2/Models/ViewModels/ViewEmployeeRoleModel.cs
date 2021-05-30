using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels
{
    public class ViewEmployeeRoleModel
    {
        public EmployeeRole EmployeeRole { get; set; }
        public bool canActive { get; set; }
        public bool canDeactive { get; set; }


    }
}