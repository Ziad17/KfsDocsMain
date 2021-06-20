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
                Models.ViewModels.EmployeeRoles.ViewEmployeeRoleModel viewModel = new Models.ViewModels.EmployeeRoles.ViewEmployeeRoleModel()
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



                viewModel.canActive = hasPersonPermission(myEmp.RoleID, PersonPermissions.ACTIVE_EMPLOYEE_ROLE) && isOperationFromEmpTOEmpValid(myEmp,EmpRole);
                viewModel.canDeactive = hasPersonPermission(myEmp.RoleID, PersonPermissions.DEACTIVATE_EMPLOYEE_ROLE) && isOperationFromEmpTOEmpValid(myEmp, EmpRole);
                viewModel.canDelete = hasPersonPermission(myEmp.RoleID, PersonPermissions.DELETE_EMPLOYEE_ROLE) && isOperationFromEmpTOEmpValid(myEmp, EmpRole);
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

        private bool isOperationFromEmpTOEmpValid(EmployeeRole CreatorEmpRole,EmployeeRole affectedEmpRole)
        {
            //if they are in the same institutions same roles can't effect eachother

            //if they are not in the same institutions same roles cant effect eachother depending on the institutionLevel



            if (CreatorEmpRole.InstitutionID == affectedEmpRole.InstitutionID)
            {
                return isRolePriorityValidInTheSameInstitution(CreatorEmpRole.RoleID, affectedEmpRole.RoleID);
            }
            else if (affectedEmpRole.Institution.ParentID == CreatorEmpRole.InstitutionID)
            {
                return isRolePriorityValidInTheChildInstitution(CreatorEmpRole.RoleID, affectedEmpRole.RoleID);

            }
            return false;

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
            if (hasPersonPermission(role.ID, PersonPermissions.DEACTIVATE_EMPLOYEE_ROLE) && isOperationFromEmpTOEmpValid( EmpRole,employeeRole))
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
        public ActionResult getAvailableRolesForInstitution(int id)
        {

            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }
            var institution = db.Institutions.Find(id);

            if (institution == null)
            {
                return getErrorView(HttpStatusCode.NotFound);

            }

            List<RoleJSONmodel> availableRoles;
            if (institution.ID == EmpRole.InstitutionID)
            {

                availableRoles = db.Roles.Where(x => x.PriorityOrder > EmpRole.Role.PriorityOrder && x.ParentID != null).Select(x => new RoleJSONmodel()
                {
                    ID = x.ID,
                    Name = x.ArabicName
                }).ToList();
                return new JsonResult { Data = availableRoles, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


            }
            availableRoles = db.Roles.Where(x => x.PriorityOrder >= EmpRole.Role.PriorityOrder && x.ParentID != null).Select(x => new RoleJSONmodel()
            {
                ID = x.ID,
                Name = x.ArabicName
            }).ToList();
            return new JsonResult { Data = availableRoles, JsonRequestBehavior = JsonRequestBehavior.AllowGet };


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
            if (hasPersonPermission(role.ID, PersonPermissions.ACTIVE_EMPLOYEE_ROLE) && isOperationFromEmpTOEmpValid( EmpRole,employeeRole))
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


                CreateEmployeeRoleModel viewModel = new CreateEmployeeRoleModel();
                viewModel.Employees = db.Employees.OrderBy(x => x.Name).Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.Name }).ToList();

                if (id != null)
                {
                    Employee employee = db.Employees.Find(id);
                    if (employee == null)
                    {
                        return getErrorView(HttpStatusCode.NotFound);
                    }
                    else
                    {

                        viewModel.InstitutionID = EmpRole.InstitutionID;
                        viewModel.EmployeeID = employee.ID;
                    }
                }
          


                viewModel.Institutions = getChildrenInstitutionWithParent(EmpRole.InstitutionID).OrderBy(x => x.ParentID).Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.ArabicName }).ToList();

                viewModel.Roles = db.Roles.Where(x => x.PriorityOrder > EmpRole.Role.PriorityOrder).OrderBy(x => x.ParentID).Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.ArabicName }).ToList();
                return View(viewModel);
            }

            return getErrorView(HttpStatusCode.Unauthorized);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateEmployeeRoleModel viewModel)
        {

            var EmpRole = getPrimaryRole();


            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }
            if (ModelState.IsValid)
            {
                var role = EmpRole.Role;

                var institution = db.Institutions.Find(viewModel.InstitutionID);
                bool isOperationValid = false;
                if (viewModel.InstitutionID == EmpRole.InstitutionID)
                {
                    //   isOperationValid = isRolePriorityValidInTheSameInstitution(EmpRole.ID, viewModel.RoleID);
                    isOperationValid = true;
                }
                else if (institution.ParentID == EmpRole.InstitutionID)
                {

                    isOperationValid = isRolePriorityValidInTheChildInstitution(EmpRole.ID, viewModel.RoleID);

                }
                else {
                    isOperationValid = false;
                }
                if (hasPersonPermission(role.ID, PersonPermissions.ATTACH_ROLE_TO_PERSON) && isOperationValid)
                {
                    EmployeeRole employeeRole = new EmployeeRole()
                    {
                        EmployeeID=viewModel.EmployeeID,
                        RoleID=viewModel.RoleID,
                        InstitutionID=viewModel.InstitutionID,
                        HiringDate=viewModel.HiringDate,
                        ArabicJobDesc=viewModel.ArabicJobDesc
                    };


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

            viewModel.Employees = db.Employees.OrderBy(x => x.Name).Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.Name }).ToList();
                viewModel.Institutions = getChildrenInstitutionWithParent(EmpRole.InstitutionID).OrderBy(x => x.ParentID).Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.ArabicName }).ToList();

            viewModel.Roles = db.Roles.Where(x => x.PriorityOrder > EmpRole.Role.PriorityOrder).OrderBy(x => x.ParentID).Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.ArabicName }).ToList();

            return View(viewModel);
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

        //public bool isEmployeeRoleDeletable()
        //{
            





        //}


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
