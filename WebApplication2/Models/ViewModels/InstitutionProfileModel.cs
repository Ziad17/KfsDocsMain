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
        public List<Employee> Employees { get; set;}
        public List<File> Files { get; set; }

        public bool canEditInfo { get; set; }
        public bool canDelete { get; set; }
        public bool isDeletable { get; set; }

    }
}