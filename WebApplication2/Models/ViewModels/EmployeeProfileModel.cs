﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels
{
    public class EmployeeProfileModel
    {


        public Employee Employee { get; set; }
        public List<EmployeeRole> Roles { get; set; }
    }
}