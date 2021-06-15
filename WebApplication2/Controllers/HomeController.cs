using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication2.Managers;
using WebApplication2.Models;
using WebApplication2.Models.ViewModels;

namespace WebApplication2.Controllers
{

    [RequireHttps]
    public class HomeController : BaseController
    {



        public ActionResult Index()
        {


            var myEmp = getEmployeeRef();

            var files = getAvaiableFilesForMeByScope().Union(getMyFiles());

           

                MainIndexModel viewModel = new MainIndexModel()
                {
                    Files = files.ToList()
                };
                return View(viewModel);
          
        }





        [AllowAnonymous]
        public ActionResult SetupAccount()
        {

            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetupAccount(FirstLoginModel firstLoginModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Employee emp = db.Employees.Where(x => x.AcadmicNumber == firstLoginModel.AcadmicNumber).FirstOrDefault();
            if (emp!=null)
            {

                if (firstLoginModel.Password.Equals(firstLoginModel.ConfirmPassword))
                {
                    if (db.EmployeeCredentials.Where(x => x.EmployeeID == emp.ID).FirstOrDefault() == null)
                    {
                        EncryptionManager encMgr = new EncryptionManager(this);

                        string enc_pass = encMgr.Encrypt(firstLoginModel.Password);
                        
                        EmployeeCredential cred = new EmployeeCredential() { Email = firstLoginModel.Email, Password = enc_pass, EmployeeID = emp.ID };
                        db.EmployeeCredentials.Add(cred);
                   
                        db.SaveChanges();
                        cred.Password = firstLoginModel.Password;

                        return RedirectToAction("Login");

                    }
                    ModelState.AddModelError("", "You Already Have Email And Password");
                    return View();

                }
                ModelState.AddModelError("", "The Passwords Are Not Identical");
                return View();

            }
            ModelState.AddModelError("", "The Acadmic Number Is Invaild");
            return View();

        }

        [AllowAnonymous]
        public ActionResult Login()
        {

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(EmployeeCredential employeeCredential)
        {


            if (!ModelState.IsValid)
            {
                return View();
            }
            EmployeeCredential employeeToValidate = db.EmployeeCredentials.Where(e => e.Email.Equals(employeeCredential.Email)).FirstOrDefault();
            if (employeeToValidate == null)
            {
                ModelState.AddModelError("", "Invalid Email");
                return View();
            }

            //APPLY ENCRYPTION
            EncryptionManager encMgr = new EncryptionManager(this);
            if (encMgr.Decrypt(employeeToValidate.Password).Equals(( employeeCredential.Password)))

                {
                    FormsAuthentication.SetAuthCookie(employeeToValidate.EmployeeID.ToString(), false);
                return RedirectToAction("Index");
            }
            else {
                ModelState.AddModelError("", "invalid Password");
                return View();
            }


        }









        public ActionResult Logout()
        {
            signOut();
            return RedirectToAction("Login");
        }


        public ActionResult Search(string query)
        {



            var emp = getEmployeeRef();
            if (emp == null)
            { return getErrorView(HttpStatusCode.Unauthorized); }
                if (query == null || query.Trim().Equals(""))
            {
                return getErrorView(HttpStatusCode.BadRequest);
            }


            var empRole = getPrimaryRole();

            SearchModel viewModel = new SearchModel();
            viewModel.Employees = db.Employees.Where(x => x.Name.Contains(query)).ToList<Employee>();
            viewModel.Institutions = db.Institutions.Where(x => x.ArabicName.Contains(query)).ToList<Institution>();
            viewModel.Files = getAvaiableFilesForMeByScope().Where(x => x.Name.Contains(query)).OrderBy(x => x.DateCreated).ToList <File>();

            return View(viewModel);




        }





    





    }
}