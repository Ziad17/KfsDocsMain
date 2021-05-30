using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels
{
    public class InstitutionProfileModel
    {
        public List<Institution> Children { get; set; }
        public Institution Institution { get; set; }
        public List<EmployeeRole> EmployeeRoles { get; set; }
        public bool canEditInfo { get; set; }
        public bool canDeactive { get; set; }
        public bool canActive { get; set; }
    }
}