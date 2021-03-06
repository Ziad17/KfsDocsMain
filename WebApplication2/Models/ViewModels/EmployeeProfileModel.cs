using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels
{
    public class EmployeeProfileModel
    {


        public Employee Employee { get; set; }
        public List<EmployeeRole> Roles { get; set; }
        public List<File> Files { get; set; }
        public List<File> Mentions { get; set; }

        public bool canDelete { get; set; }
    }
}