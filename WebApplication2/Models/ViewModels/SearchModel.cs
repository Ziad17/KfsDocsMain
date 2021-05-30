using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels
{
    public class SearchModel
    {
        public List<Employee> Employees { get; set; }
        public List<Institution> Institutions { get; set; }

    }
}