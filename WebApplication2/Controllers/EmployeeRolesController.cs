using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class EmployeeRolesController : BaseController
    {

   
        public ActionResult Index()
        {
            var employeeRoles = db.EmployeeRoles.Include(e => e.Employee).Include(e => e.Institution).Include(e => e.Role);
            return View(employeeRoles.ToList());
        }


        public ActionResult Deactive(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);

            }
            var role = EmpRole.Role;

            EmployeeRole employeeRole = db.EmployeeRoles.Find(id);
            if (employeeRole == null)
            {
                return getErrorView(HttpStatusCode.NotFound);
            }
            if (hasPersonPermission(role.ID, PersonPermissions.DEACTIVATE_PERSON_WITHIN_INSTITUTION) && operationValidInInstitution(EmpRole.ID, employeeRole.InstitutionID) && (employeeRole.Active) && isRolePriorityValid(EmpRole.Role.ID, employeeRole.Role.ID))
            {

                return View(employeeRole);

            }

            return getErrorView(HttpStatusCode.Unauthorized);
        }


        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public ActionResult DeactiveConfirmed(int id)
        {


            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);

            }
            var role = EmpRole.Role;

            EmployeeRole employeeRole = db.EmployeeRoles.Find(id);

            if (employeeRole == null)
            {
                return getErrorView(HttpStatusCode.NotFound);
            }
            if (hasPersonPermission(role.ID, PersonPermissions.DEACTIVATE_PERSON_WITHIN_INSTITUTION) && operationValidInInstitution(EmpRole.ID, employeeRole.InstitutionID) && (employeeRole.Active) && isRolePriorityValid(EmpRole.Role.ID, employeeRole.Role.ID))
            {

                db.EmployeeRoles.Find(employeeRole.ID).Active = false;
                PersonActionLog log = new PersonActionLog();
                log.ActionDate = DateTime.Now;
                log.ConductorEmployeeID = EmpRole.ID;
                log.AffectedEmployeeID = employeeRole.EmployeeID;
                log.PermissionName = PersonPermissions.DEACTIVATE_PERSON_WITHIN_INSTITUTION;

                db.PersonActionLogs.Add(log);

                db.SaveChanges();
                return RedirectToAction("View", new { id = employeeRole.ID });

            }
            return getErrorView(HttpStatusCode.NotFound);

        }

        public ActionResult Active(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);

            }
            var role = EmpRole.Role;

            EmployeeRole employeeRole = db.EmployeeRoles.Find(id);
            if (employeeRole == null)
            {
                return getErrorView(HttpStatusCode.NotFound);
            }
            if (hasPersonPermission(role.ID, PersonPermissions.ACTIVATE_PERSON_WITHIN_INSTITUTION) && operationValidInInstitution(EmpRole.ID, employeeRole.InstitutionID) && (!employeeRole.Active) && isRolePriorityValid(EmpRole.Role.ID, employeeRole.Role.ID))
            {

                return View(employeeRole);

            }

            return getErrorView(HttpStatusCode.Unauthorized);
        }


        [HttpPost, ActionName("Active")]
        [ValidateAntiForgeryToken]
        public ActionResult ActiveConfirmed(int id)
        {


            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);

            }
            var role = EmpRole.Role;

            EmployeeRole employeeRole = db.EmployeeRoles.Find(id);

            if (employeeRole == null)
            {
                return getErrorView(HttpStatusCode.NotFound);
            }
            if (hasPersonPermission(role.ID, PersonPermissions.ACTIVATE_PERSON_WITHIN_INSTITUTION) && operationValidInInstitution(EmpRole.ID, employeeRole.InstitutionID) && (!employeeRole.Active) && isRolePriorityValid(EmpRole.Role.ID, employeeRole.Role.ID))
            {

                db.EmployeeRoles.Find(employeeRole.ID).Active = true;
                PersonActionLog log = new PersonActionLog();
                log.ActionDate = DateTime.Now;
                log.ConductorEmployeeID = EmpRole.ID;
                log.AffectedEmployeeID = employeeRole.EmployeeID;
                log.PermissionName = PersonPermissions.ACTIVATE_PERSON_WITHIN_INSTITUTION;

                db.PersonActionLogs.Add(log);

                db.SaveChanges();
                return RedirectToAction("View", new { id = employeeRole.ID });

            }
            return getErrorView(HttpStatusCode.NotFound);

        }



        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeRole employeeRole = db.EmployeeRoles.Find(id);
            if (employeeRole == null)
            {
                return HttpNotFound();
            }
            return View(employeeRole);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EmployeeRole employeeRole = db.EmployeeRoles.Find(id);
            db.EmployeeRoles.Remove(employeeRole);
            db.SaveChanges();
            return RedirectToAction("Index");
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
