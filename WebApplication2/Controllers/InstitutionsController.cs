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
    public class InstitutionsController : BaseController
    {

        // GET: Institutions
        public ActionResult Index()
        {
            
            var institutions = db.Institutions.Include(i => i.InstitutionType).Include(i => i.Institution2);
            return View(institutions.ToList());
        }


        // GET: Institutions/Create
        public ActionResult Create()
        {

            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }
            var role = EmpRole.Role;

            if (hasInstitutionPermission(role.ID, InstitutionPermissions.CREATE_INSTITUTION))
            {
                ViewBag.InstitutionTypeID = new SelectList(db.InstitutionTypes, "ID", "ArabicName");
                ViewBag.Parent = EmpRole.Institution.ArabicName;

                return View();
            }
            else 
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }

           
        }














        // POST: Institutions/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ArabicName,InstitutionTypeID,Active,ParentID,Website,ImageURL,InsideCampus,PrimaryPhone,SecondaryPhone,Fax,Email")] Institution institution)
        {
          
            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }
            if (ModelState.IsValid)
            {
                var role = EmpRole.Role;

                if (hasInstitutionPermission(role.ID, InstitutionPermissions.CREATE_INSTITUTION))
                {
                    institution.InsideCampus = true;
                    institution.Active = true;
                    institution.ParentID = EmpRole.InstitutionID;
                    db.Institutions.Add(institution);


                    InstitutionActionLog log = new InstitutionActionLog()
                    { EmployeeID=EmpRole.ID,
                    InstitutionID=institution.ID,
                    ActionDate=DateTime.Now,
                    PermissionName = InstitutionPermissions.CREATE_INSTITUTION
                    };

                    db.InstitutionActionLogs.Add(log);


                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return getErrorView(HttpStatusCode.Unauthorized);

            }

            ViewBag.InstitutionTypeID = new SelectList(db.InstitutionTypes, "ID", "ArabicName", institution.InstitutionTypeID);
            return View(institution);
        }

        // GET: Institutions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return getErrorView(HttpStatusCode.BadRequest);
            }

            Institution institution = db.Institutions.Find(id);
            if (institution == null)
            {
                return getErrorView(HttpStatusCode.NotFound);
            }
            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }
            var role = EmpRole.Role;

            if (hasInstitutionPermission(role.ID, InstitutionPermissions.EDIT_INSTITUTION_INFO) && isPartOfInstitution(EmpRole.ID, institution.ID))
            {
                return View(institution);
            }
            else { return  getErrorView(HttpStatusCode.Unauthorized); }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ArabicName,Website,ImageURL,PrimaryPhone,SecondaryPhone,Fax,Email")] Institution institution)
        {
            if (ModelState.IsValid)
            {
                if (institution == null)
                {
                    return getErrorView(HttpStatusCode.NotFound);
                }
                var EmpRole = getPrimaryRole();
                
                if (EmpRole == null)
                {
                    return getErrorView(HttpStatusCode.Unauthorized);
                }
                var role = EmpRole.Role;

                if (hasInstitutionPermission(role.ID, InstitutionPermissions.EDIT_INSTITUTION_INFO) && isPartOfInstitution(EmpRole.ID, institution.ID))
                {
                    db.Institutions.Find(institution.ID).ArabicName = institution.ArabicName;
                    db.Institutions.Find(institution.ID).Website = institution.Website;
                    db.Institutions.Find(institution.ID).ImageURL = institution.ImageURL;
                    db.Institutions.Find(institution.ID).PrimaryPhone = institution.PrimaryPhone;
                    db.Institutions.Find(institution.ID).SecondaryPhone = institution.SecondaryPhone;
                    db.Institutions.Find(institution.ID).Fax = institution.Fax;
                    db.Institutions.Find(institution.ID).Email = institution.Email;


                    InstitutionActionLog log = new InstitutionActionLog()
                    {
                        InstitutionID = institution.ID,
                        EmployeeID =EmpRole.ID,
                        ActionDate = DateTime.Now,
                        PermissionName=InstitutionPermissions.EDIT_INSTITUTION_INFO
             
                    };
                    db.InstitutionActionLogs.Add(log);

                    db.SaveChanges();
                    return RedirectToAction("InstitutionProfile", new { id = institution.ID });
                }
           
            }
            return View(institution);
        }



        public ActionResult Active(int? id)
        {
            if (id == null)
            {
                return getErrorView(HttpStatusCode.BadRequest);
            }


            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);

            }
            var role = EmpRole.Role;

            Institution institution = db.Institutions.Find(id);
            if (institution == null)
            {
                return getErrorView(HttpStatusCode.NotFound);
            }
            if (hasInstitutionPermission(role.ID, InstitutionPermissions.ACTIVATE_INSTITUTION) && !(institution.Active))
            {
            return View(institution);

            }
         

            return getErrorView(HttpStatusCode.Unauthorized);


        }



        // POST: Institutions/Delete/5
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

            Institution institution = db.Institutions.Find(id);
            if (institution == null)
            {
                return getErrorView(HttpStatusCode.NotFound);
            }
            if (hasInstitutionPermission(role.ID, InstitutionPermissions.ACTIVATE_INSTITUTION) && !(institution.Active))
            {

                db.Institutions.Find(institution.ID).Active = true;
                InstitutionActionLog log = new InstitutionActionLog();
                log.ActionDate = DateTime.Now;
                log.EmployeeID = EmpRole.ID;
                log.InstitutionID = institution.ID;
                log.PermissionName = InstitutionPermissions.ACTIVATE_INSTITUTION;

                db.InstitutionActionLogs.Add(log);

                db.SaveChanges();
                return RedirectToAction("InstitutionProfile", new { id = institution.ID });

            }
            return getErrorView(HttpStatusCode.NotFound);

          
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

            Institution institution = db.Institutions.Find(id);
            if (institution == null)
            {
                return getErrorView(HttpStatusCode.NotFound);
            }
            if (hasInstitutionPermission(role.ID, InstitutionPermissions.DEACTIVATE_INSTITUTION) && (institution.Active))
            {
          
            return View(institution);

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

            Institution institution = db.Institutions.Find(id);
            if (institution == null)
            {
                return getErrorView(HttpStatusCode.NotFound);
            }
            if (hasInstitutionPermission(role.ID, InstitutionPermissions.DEACTIVATE_INSTITUTION) && (institution.Active))
            {

                db.Institutions.Find(institution.ID).Active = false;
                InstitutionActionLog log = new InstitutionActionLog();
                log.ActionDate = DateTime.Now;
                log.EmployeeID = EmpRole.ID;
                log.InstitutionID = institution.ID;
                log.PermissionName = InstitutionPermissions.DEACTIVATE_INSTITUTION;

                db.InstitutionActionLogs.Add(log);

                db.SaveChanges();
                return RedirectToAction("InstitutionProfile", new { id = institution.ID });

            }
            return getErrorView(HttpStatusCode.NotFound);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



      

        //Example /Employees/PersonProfile/2
        public ActionResult InstitutionProfile(int? id)
        {

       
            if (id == null || id <= 0)
            {
                return getErrorView(HttpStatusCode.BadRequest);

            }

            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);

            }

            Institution institution = db.Institutions.Find(id);
            if (institution == null)
            {
                return getErrorView(HttpStatusCode.NotFound);
            }

            Role role = EmpRole.Role;
         
            if (hasInstitutionPermission(role.ID, InstitutionPermissions.VIEW_INSTITUTION))
            {
                InstitutionProfileModel viewModel = new InstitutionProfileModel();
                viewModel.Institution = institution;
                viewModel.Institution.ImageURL = "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";
               
                
                viewModel.Employees = db.EmployeeRoles.Where(x => x.InstitutionID == institution.ID).Select(x=>x.Employee).Distinct().ToList<Employee>();




                viewModel.Files = getAvaiableFilesForMe().Where(x => x.EmployeeRole.InstitutionID == institution.ID).ToList<File>();
                
                
                
                viewModel.Children = db.Institutions.Where(x => x.ParentID == institution.ID).ToList<Institution>();
                viewModel.canEditInfo = hasInstitutionPermission(role.ID, InstitutionPermissions.EDIT_INSTITUTION_INFO);
                viewModel.canActive = hasInstitutionPermission(role.ID, InstitutionPermissions.ACTIVATE_INSTITUTION);
                viewModel.canDeactive = hasInstitutionPermission(role.ID, InstitutionPermissions.DEACTIVATE_INSTITUTION);


                return View(viewModel);
            }

            //IMPLMENT UNAUTHORIZED VIEW
            return getErrorView(HttpStatusCode.Unauthorized);
        }

    }
}
