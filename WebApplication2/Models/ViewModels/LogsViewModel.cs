using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels
{
    public class LogsViewModel
    {
        public List<PersonActionLog> PersonActionLogs { get; set; }
        public List<InstitutionActionLog> InstitutionActionLogs { get; set; }
        public List<FileActionLog> FileActionLogs { get; set; }

        public EmployeeRole MyRef { get; set; }

    }
}