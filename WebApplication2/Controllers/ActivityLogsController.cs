using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using WebApplication2.Models.ViewModels;

namespace WebApplication2.Controllers
{
    public class ActivityLogsController : BaseController
    {

        public ActionResult Index()
        {
                                                                                                                    
            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }
            var personLogs = db.PersonActionLogs.Where(x => x.ConductorEmployeeID == EmpRole.ID).OrderBy(x=>x.ActionDate).ToList();
            var institutionLogs = db.InstitutionActionLogs.Where(x => x.EmployeeID == EmpRole.ID).OrderBy(x => x.ActionDate).ToList();
            var fileLogs = db.FileActionLogs.Where(x => x.EmployeeID == EmpRole.ID).OrderBy(x => x.ActionDate).ToList();
            LogsViewModel viewModel = new LogsViewModel()
            {
                MyRef=EmpRole,
                PersonActionLogs=personLogs,
                InstitutionActionLogs=institutionLogs,
                FileActionLogs=fileLogs
            };
            return View(viewModel);



        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
