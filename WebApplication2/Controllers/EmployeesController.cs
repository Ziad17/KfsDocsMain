using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Managers;
using WebApplication2.Models;
using WebApplication2.Models.HelperModels;
using WebApplication2.Models.ViewModels;

namespace WebApplication2.Controllers
{
    public class EmployeesController : BaseController
    {




        public ActionResult ChangePhoto()
        {
            Employee employee = getEmployeeRef();

            if (employee == null)
            {
                signOut();
                return RedirectToAction("Login", "Home");
            }

            return View();
        }



        [HttpPost]
        public ActionResult ChangePhoto(HttpPostedFileBase Img)
        {

            if (ModelState.IsValid)
            {
                var emp = getEmployeeRef();
                if (emp == null)
                {
                    signOut();
                    return RedirectToAction("Login", "Home");
                }

                var FileSizeInKb = 512;
                var supportedImages = new[] { "jpg", "png", "jpeg" };
                try
                {
                    if (Img != null)
                    {
                        var fileExt = Path.GetExtension(Img.FileName).Substring(1).ToLower();
                        if (!supportedImages.Contains(fileExt))
                        {
                            ModelState.AddModelError("", "Only Image Format Is Allowed (jpg,png,jpeg)");
                            return View();
                        }
                        if (Img.ContentLength > FileSizeInKb * 1024)
                        {
                            ModelState.AddModelError("", "The Image Size Should Not Exceed 512Kb");
                            return View();

                        }
                        StreamReader st = new StreamReader(Img.InputStream);
                        FileManager filMG = new FileManager();
                        EncryptionManager encryptionManager = new EncryptionManager(this);


                        string fileName = FileManager.RandomString(1) + encryptionManager.Encrypt(Img.FileName);
                        if (fileName.Length > 10)
                        {
                            fileName = fileName.Substring(0, 10) + "." + fileExt;
                        }
                        if (filMG.uploadImage(fileName, st.BaseStream))
                        {

                            db.Employees.Find(emp.ID).ImageURL = fileName;
                            db.SaveChanges();
                            return RedirectToAction("MyProfile");


                        }
                        else
                        {
                            ModelState.AddModelError("", "Uploading Failed");
                            return View();

                        }

                    }
                    ModelState.AddModelError("", "Image Empty");
                    return View();


                }
                catch (Exception e)
                {


                    ModelState.AddModelError("", "Internal Error");
                    return View();

                }

            }
            ModelState.AddModelError("", "No Image Selected");
            return View();
        }


   


        // GET: Employees
        public ActionResult Index()
        {
    

            var employees = db.Employees.Include(e => e.City);
            return View(employees.ToList());
        }

    
        // GET: Employees/Create
        public ActionResult Create()
        {

            var EmpRole = getPrimaryRole();

            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }
            var role = EmpRole.Role;

            if (hasPersonPermission(role.ID, PersonPermissions.CREATE_PERSON_WITHIN_INSTITUTION))
            {
                ViewBag.CityID = new SelectList(db.Cities, "ID", "Name");
                ViewBag.Gender = new SelectList(new List<Genders> { new Genders { ID = "M", Name = "Male" }, new Genders { ID = "F", Name = "Female" } }, "ID", "Name");
                return View();
            }
            return getErrorView(HttpStatusCode.Unauthorized);
       
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Gender,UserPassword,AcadmicNumber,CityID,Email")] Employee employee)
        {
            var EmpRole = getPrimaryRole();

            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }

            if (ModelState.IsValid)
            {


                
                var role = EmpRole.Role;
                if (hasPersonPermission(role.ID, PersonPermissions.CREATE_PERSON_WITHIN_INSTITUTION))
                {
                    
                        employee.Active = true;
                        db.Employees.Add(employee);
                        PersonActionLog log = new PersonActionLog()
                        {
                            ConductorEmployeeID = EmpRole.ID,
                            AffectedEmployeeID = employee.ID,
                            PermissionName = PersonPermissions.CREATE_PERSON_WITHIN_INSTITUTION,
                            ActionDate = DateTime.Now
                        };
                        db.PersonActionLogs.Add(log);
                        db.SaveChanges();
                        return RedirectToAction("Create", "Roles", new { id = (employee.ID) });
                    
                }
                return getErrorView(HttpStatusCode.Unauthorized);

            }

            ViewBag.CityID = new SelectList(db.Cities, "ID", "Name", employee.CityID);
            return View(employee);
        }


    
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMyProfile([Bind(Include = "Name,CityID,PhoneNumber,PHD,ImageURL,Bio")] Employee employee)
        {
            var Emp = getEmployeeRef();

            if (Emp == null)
            {
                signOut();
                return RedirectToAction("Login","Home");
            }
       
            if (ModelState.IsValid)
            {
                db.Employees.Find(Emp.ID).Name = employee.Name;
                db.Employees.Find(Emp.ID).CityID = employee.CityID;
                db.Employees.Find(Emp.ID).PhoneNumber = employee.PhoneNumber;
                db.Employees.Find(Emp.ID).PHD = employee.PHD;
                db.Employees.Find(Emp.ID).ImageURL = employee.ImageURL;
                db.Employees.Find(Emp.ID).Bio = employee.Bio;


                db.SaveChanges();
                return RedirectToAction("MyProfile");
            }
            ViewBag.CityID = new SelectList(db.Cities, "ID", "Name", employee.CityID);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        public ActionResult MyProfile()
        {
          

            Employee employee = getEmployeeRef();
            if (employee == null)
            {
                signOut();
                return RedirectToAction("Login", "Home");
            }

            List<EmployeeRole> employeeRoles = db.EmployeeRoles.Where(x => x.EmployeeID == employee.ID).ToList<EmployeeRole>();



        


            FileManager filMg = new FileManager();

            ViewBag.Img = filMg.getImageStream(employee.ImageURL);



            MyProfileModel viewModel = new MyProfileModel() { Employee = employee,
                Roles = employeeRoles
        };
            return View(viewModel);
        }




        public ActionResult EditMyProfile()
        {
           

            Employee employee =getEmployeeRef();

            if (employee == null)
            {
                signOut();
                return RedirectToAction("Login", "Home");
            }

            ViewBag.CityID = new SelectList(db.Cities, "ID", "Name", employee.CityID);
            return View(employee);

        }
        //Example /Employees/PersonProfile/2
        public ActionResult PersonProfile(int? id)
        {

            var myRef=getEmployeeRef();
            if (myRef == null)
            {
                signOut();
                return RedirectToAction("Login", "Home");
            }

            if (id == null || id <= 0)
            {
                return getErrorView(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return getErrorView(HttpStatusCode.NotFound);
            }

            if (employee.ID == myRef.ID)
            {
                return RedirectToAction("MyProfile");
            }

            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }
            Role role = EmpRole.Role;

            if (hasPersonPermission(role.ID,PersonPermissions.VIEW_PERSON_PROFILE))
            {
                EmployeeProfileModel viewModel = new EmployeeProfileModel();
                viewModel.Employee = employee;

                FileManager filMg = new FileManager();

                ViewBag.Img = filMg.getImageStream(employee.ImageURL);


                viewModel.Roles = employee.EmployeeRoles.ToList<EmployeeRole>();

                return View(viewModel);
            }

            //IMPLMENT UNAUTHORIZED VIEW
            return getErrorView(HttpStatusCode.Unauthorized);
        }
      






        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
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
