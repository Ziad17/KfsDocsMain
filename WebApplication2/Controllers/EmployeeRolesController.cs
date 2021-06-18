using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using WebApplication2.Models.ViewModels.EmployeeRoles;

namespace WebApplication2.Controllers
{
    public class EmployeeRolesController : BaseController
    {


        public ActionResult Index()
        {

            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }
            var employeeRoles = db.EmployeeRoles.Where(x => x.EmployeeID != EmpRole.EmployeeID);





            if (hasPersonPermission(EmpRole.RoleID, PersonPermissions.VIEW_ALL_EMPLOYEE_ROLES))
            {
                EmployeeRoleIndexModel viewModel = new EmployeeRoleIndexModel()
                {
                    EmployeeRoles = employeeRoles.ToList()
                };
                viewModel.canAdd = hasPersonPermission(EmpRole.RoleID, PersonPermissions.ATTACH_ROLE_TO_PERSON);
                viewModel.canView = hasPersonPermission(EmpRole.RoleID, PersonPermissions.VIEW_EMPLOYEE_ROLE);

                return View(viewModel);

            }
            return getErrorView(HttpStatusCode.Unauthorized);







        }

        public ActionResult View(int? id)
        {
            if (id == null)
            {
                return getErrorView(HttpStatusCode.BadRequest);
            }
            var myEmp = getPrimaryRole();
            if (myEmp == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);

            }
            var EmpRole = db.EmployeeRoles.Find(id);
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.NotFound);

            }
            if (hasPersonPermission(myEmp.RoleID, PersonPermissions.VIEW_EMPLOYEE_ROLE))
            {
                ViewEmployeeRoleModel viewModel = new ViewEmployeeRoleModel()
                {
                    EmpID = EmpRole.EmployeeID,
                    EmployeeRoleID = EmpRole.ID,
                    InstitutionID = EmpRole.InstitutionID,
                    RoleID = EmpRole.RoleID,
                    PersonName = EmpRole.Employee.Name,
                    RoleName = EmpRole.Role.ArabicName,
                    InstitutionName = EmpRole.Institution.ArabicName,
                    isActive = EmpRole.Active,
                    HiringDate = EmpRole.HiringDate

                };



                viewModel.canActive = hasPersonPermission(myEmp.RoleID, PersonPermissions.ACTIVE_EMPLOYEE_ROLE) && isRolePriorityValid(myEmp.RoleID, EmpRole.RoleID);
                viewModel.canDeactive = hasPersonPermission(myEmp.RoleID, PersonPermissions.DEACTIVATE_EMPLOYEE_ROLE) && isRolePriorityValid(myEmp.RoleID, EmpRole.RoleID);
                viewModel.canDelete = hasPersonPermission(myEmp.RoleID, PersonPermissions.DELETE_EMPLOYEE_ROLE) && isRolePriorityValid(myEmp.RoleID, EmpRole.RoleID);
                viewModel.FilesPublished = getAvaiableFilesForMeByScope().Union(getMyFiles()).Union(getMyMentions()).Where(x => x.AuthorID == EmpRole.ID).ToList();

                if (EmpRole.ID == myEmp.ID)
                {
                    viewModel.canActive = false;
                    viewModel.canDelete = false;
                    viewModel.canDeactive = false;

                }
                return View(viewModel);
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
            if (hasPersonPermission(role.ID, PersonPermissions.DEACTIVATE_EMPLOYEE_ROLE) && operationValidInInstitution(EmpRole.ID, employeeRole.InstitutionID) && (employeeRole.Active) && isRolePriorityValid(EmpRole.Role.ID, employeeRole.Role.ID))
            {

                db.EmployeeRoles.Find(employeeRole.ID).Active = false;
                PersonActionLog log = new PersonActionLog();
                log.ActionDate = DateTime.Now;
                log.ConductorEmployeeID = EmpRole.ID;
                log.AffectedEmployeeID = employeeRole.EmployeeID;
                log.PermissionName = PersonPermissions.DEACTIVATE_EMPLOYEE_ROLE;

                db.PersonActionLogs.Add(log);

                db.SaveChanges();
                return RedirectToAction("View", new { id = employeeRole.ID });

            }
            return getErrorView(HttpStatusCode.NotFound);

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
            if (hasPersonPermission(role.ID, PersonPermissions.ACTIVE_EMPLOYEE_ROLE) && operationValidInInstitution(EmpRole.ID, employeeRole.InstitutionID) && (!employeeRole.Active) && isRolePriorityValid(EmpRole.Role.ID, employeeRole.Role.ID))
            {

                db.EmployeeRoles.Find(employeeRole.ID).Active = true;
                PersonActionLog log = new PersonActionLog();
                log.ActionDate = DateTime.Now;
                log.ConductorEmployeeID = EmpRole.ID;
                log.AffectedEmployeeID = employeeRole.EmployeeID;
                log.PermissionName = PersonPermissions.ACTIVE_EMPLOYEE_ROLE;

                db.PersonActionLogs.Add(log);

                db.SaveChanges();
                return RedirectToAction("View", new { id = employeeRole.ID });

            }
            return getErrorView(HttpStatusCode.NotFound);

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
                ViewBag.InstitutionID = new SelectList(availableInstitutions, "ID", "ArabicName", EmpRole.InstitutionID);

                ViewBag.RoleID = new SelectList(db.Roles.Where(x => x.PriorityOrder > EmpRole.Role.PriorityOrder), "ID", "ArabicName");
                return View();
            }

            return getErrorView(HttpStatusCode.Unauthorized);


        }

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


                if (hasPersonPermission(role.ID, PersonPermissions.ATTACH_ROLE_TO_PERSON) && operationValidInInstitution(EmpRole.ID, employeeRole.InstitutionID) && isRolePriorityValid(role.ID, employeeRole.RoleID))
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
