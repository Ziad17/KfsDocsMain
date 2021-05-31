using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels
{
    public class MyProfileModel
    {


        public Employee Employee { get; set; }
        public List<EmployeeRole> Roles { get; set; }


        public List<File> Files { get; set; }


    }
}