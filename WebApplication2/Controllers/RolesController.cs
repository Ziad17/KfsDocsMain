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
    public class RolesController : BaseController
    {

        // GET: Roles
        public ActionResult Index()
        {
            getEmployeeRef();

            var employeeRoles = db.EmployeeRoles.Include(e => e.Employee).Include(e => e.Institution).Include(e => e.Role);
            return View(employeeRoles.ToList());
        }



        public ActionResult View(int id)
        {
            var myEmpRole = getPrimaryRole();

            if (myEmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }
            var role = myEmpRole.Role;
            if (hasPersonPermission(role.ID, PersonPermissions.VIEW_PERSON_ROLE))
            {
                EmployeeRole empRole = db.EmployeeRoles.Find(id);
                if (empRole==null)
                {
                    return getErrorView(HttpStatusCode.NotFound);
                }
                ViewEmployeeRoleModel viewModel = new ViewEmployeeRoleModel();
                viewModel.EmployeeRole = empRole;
                if (isRolePriorityValid(role.ID, empRole.RoleID))
                {
                    viewModel.canDeactive= hasPersonPermission(role.ID, PersonPermissions.DEACTIVATE_PERSON_WITHIN_INSTITUTION);
                    viewModel.canActive = hasPersonPermission(role.ID, PersonPermissions.ACTIVATE_PERSON_WITHIN_INSTITUTION);

                }
                return View(viewModel);
            }
            return getErrorView(HttpStatusCode.BadRequest);

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
            if (hasPersonPermission(role.ID, PersonPermissions.DEACTIVATE_PERSON_WITHIN_INSTITUTION) && operationValidInInstitution(EmpRole.ID,employeeRole.InstitutionID) && (employeeRole.Active) && isRolePriorityValid(EmpRole.Role.ID,employeeRole.Role.ID))
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
            if (hasPersonPermission(role.ID, PersonPermissions.DEACTIVATE_PERSON_WITHIN_INSTITUTION) && operationValidInInstitution(EmpRole.ID,employeeRole.InstitutionID) && (employeeRole.Active) && isRolePriorityValid(EmpRole.Role.ID, employeeRole.Role.ID))
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
            if (hasPersonPermission(role.ID, PersonPermissions.ACTIVATE_PERSON_WITHIN_INSTITUTION)&& operationValidInInstitution(EmpRole.ID,employeeRole.InstitutionID) && (!employeeRole.Active) && isRolePriorityValid(EmpRole.Role.ID, employeeRole.Role.ID))
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


        [HttpPost ,ActionName("ChooseDefault")]
        [ValidateAntiForgeryToken]
        public ActionResult ChooseDefault(int id)
        {
            var myEmpRef = getEmployeeRef();
            if (myEmpRef == null)
            {
                signOut();
                return RedirectToAction("Login", "Home");
            }
            var Role = db.EmployeeRoles.Where(x=> x.ID==id && x.EmployeeID==myEmpRef.ID).FirstOrDefault();
            if (Role == null)
            { return getErrorView(HttpStatusCode.NotFound); }
            
            db.Employees.Find(myEmpRef.ID).PrimaryRoleID = Role.ID;
           
            db.SaveChanges();
            return ChooseDefault();
        }
        

        public ActionResult ChooseDefault()
        {
            var myEmpRef = getEmployeeRef();
            if (myEmpRef == null)
            { signOut();
                return RedirectToAction("Login", "Home");
            }

            RolesChooseDefaultModel viewModel = new RolesChooseDefaultModel();
            viewModel.Employee = myEmpRef;

            if (myEmpRef.PrimaryRoleID != null && myEmpRef.PrimaryRoleID > 1)
            {
                viewModel.DefaultRole = db.EmployeeRoles.Find(myEmpRef.PrimaryRoleID);

                viewModel.EmployeeRoles = db.EmployeeRoles.Where(x => x.ID != myEmpRef.PrimaryRoleID &&x.EmployeeID==myEmpRef.ID).ToList<EmployeeRole>();

            }
            else {
                viewModel.DefaultRole = null;
                viewModel.EmployeeRoles = db.EmployeeRoles.Where(x => x.EmployeeID == myEmpRef.ID).ToList<EmployeeRole>();

            }


            return View(viewModel);

        }






        public ActionResult Create(int? id)
        {

            var EmpRole = getPrimaryRole();


            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }
            var role = EmpRole.Role;

            

            if (hasPersonPermission(role.ID, PersonPermissions.ATTACH_ROLE_TO_PERSON))
            {


                if (id != null)
                {
                    Employee employee = db.Employees.Find(id);
                    if (employee == null)
                    {
                        return getErrorView(HttpStatusCode.NotFound);
                    }
                    else
                    {
                        ViewBag.EmployeeID = new SelectList(db.Employees, "ID", "Name", employee.ID);
                    }
                }
                else
                {
                    ViewBag.EmployeeID = new SelectList(db.Employees, "ID", "Name");

                }


                var availableInstitutions = getChildrenInstitutionWithParent(EmpRole.InstitutionID).ToList<Institution>();
                
                   ViewBag.InstitutionID = new SelectList(availableInstitutions, "ID", "ArabicName");

                ViewBag.RoleID = new SelectList(db.Roles.Where(x => x.PriorityOrder > EmpRole.Role.PriorityOrder), "ID", "ArabicName");
                return View();
            }

            return getErrorView(HttpStatusCode.Unauthorized);


        }

       

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,RoleID,ArabicJobDesc,InstitutionID,HiringDate")] EmployeeRole employeeRole)
        {

            var EmpRole = getPrimaryRole();


            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }
            if (ModelState.IsValid)
            {
                var role = EmpRole.Role;


                if (hasPersonPermission(role.ID, PersonPermissions.ATTACH_ROLE_TO_PERSON) && operationValidInInstitution(EmpRole.ID,employeeRole.InstitutionID)  && isRolePriorityValid(role.ID,employeeRole.RoleID))
                {
                    employeeRole.Active = true;
                    db.EmployeeRoles.Add(employeeRole);
                    PersonActionLog log = new PersonActionLog()
                    {
                        ConductorEmployeeID = EmpRole.ID,
                        AffectedEmployeeID = employeeRole.EmployeeID,
                        PermissionName = PersonPermissions.ATTACH_ROLE_TO_PERSON,
                        ActionDate = DateTime.Now
                    };
                    db.PersonActionLogs.Add(log);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return getErrorView(HttpStatusCode.Unauthorized);
            }

            ViewBag.EmployeeID = new SelectList(db.Employees, "ID", "Name", employeeRole.EmployeeID);
            ViewBag.InstitutionID = new SelectList(db.Institutions, "ID", "ArabicName", employeeRole.InstitutionID);
            ViewBag.RoleID = new SelectList(db.Roles, "ID", "ArabicName", employeeRole.RoleID);
            return View(employeeRole);
        }

    






        // GET: Roles/Delete/5
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

        // POST: Roles/Delete/5
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
