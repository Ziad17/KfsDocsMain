using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication2.Managers;
using WebApplication2.Models;
using WebApplication2.Models.ViewModels;

namespace WebApplication2.Controllers
{
    public abstract class BaseController : Controller
    {


        protected Employee UserRefrence;
        protected DMS_dbEntities8 db = new DMS_dbEntities8();




        public EmployeeRole getPrimaryRole()
        {

            Employee emp = getEmployeeRef();
            if (emp == null)
            {
                signOut();
                return null;

            }
            if (emp.PrimaryRoleID > 0)
            {
                return db.EmployeeRoles.Find(emp.PrimaryRoleID);

            }
            return null;

        }

        public Employee getEmployeeRef()
        {

            try
            {
                int ID = Int32.Parse(ControllerContext.HttpContext.User.Identity.Name);
                Employee emp = db.Employees.Find(ID);
                //if (UserRefrence == null)
                //{

                //    signOut();
                //    return null;
                //}
                return emp;

            }
            catch (Exception e)
            {
                signOut();
                return null;

            }
        }



        //protected string validateFile(HttpPostedFileBase file_content)
        //{
        //    var FileSizeInKb = 512;
        //    var supportedFiles = db.FileTypes.Select(x => x.Extension);
        //    var fileExt = System.IO.Path.GetExtension(file_content.FileName).Substring(1).ToLower();
        //    if (!supportedFiles.Contains(fileExt))
        //    {
        //        var err_msg = "Only File Format Is Allowed (";
        //        foreach (string ext in supportedFiles)
        //        {
        //            err_msg += ext + ",";
        //        }
        //        err_msg += ")";
        //        return err_msg;
        //    }
        //    if (file_content != null)
        //    {

        //        if (file_content.ContentLength > FileSizeInKb * 1024)
        //        {
        //            return "The File Size Should Not Exceeding " + FileSizeInKb + " Kb");

        //        }
        //        System.IO.StreamReader st = new System.IO.StreamReader(file_content.InputStream);
        //        FileManager filMG = new FileManager();
        //        EncryptionManager encryptionManager = new EncryptionManager(this);


        //        string fileName = FileManager.RandomString(1) + encryptionManager.Encrypt(file_content.FileName);
        //        if (fileName.Length > 10)
        //        {
        //            fileName = fileName.Substring(0, 10) + "." + fileExt;
        //        }
        //        if (filMG.uploadFile(fileName, st.BaseStream))
        //        {

                  


        //        }
        //        else
        //        {
        //           return "Uploading Failed";
               

        //        }



        //    }
        //    else
        //    {
        //        return "File Is Empty";
               

        //    }

        //}






        protected void signOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            //  clear authentication cookie


            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
            HttpCookie cookie2 = new HttpCookie(sessionStateSection.CookieName, "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);

            FormsAuthentication.RedirectToLoginPage();
        }

        protected bool hasPersonPermission(int roleID, string permissionName)
        {
            if (db.RolePersonPermissions.Where(s => s.RoleID == roleID && s.PermissionName.Equals(permissionName)).Count() == 1)
            {
                return true;
            }
            return false;
        }

       



        protected bool hasFileLevelPermission(int roleID, string fileLevel, string permissionName)
        {

            if ((db.FilesScopes.Where(s => s.RoleID == roleID && s.Level == fileLevel && s.Permission.Equals(permissionName)).Count() == 1))
            {
                return true;
            }
            return false;
        }
        protected bool isFileAuthor(int fileID,int empID)
        {

            if (db.Files.Find(fileID).AuthorID==empID)
            {
                return true;
            }
            return false;
        }

        protected bool hasInstitutionPermission(int roleID, string permissionName)
        {
            if (db.RoleInstitutionPermissions.Where(s => s.RoleID == roleID && s.PermissionName.Equals(permissionName)).Count() == 1)
            {
                return true;
            }
            return false;
        }

        protected bool isRolePriorityValid(int BiggerRoleID, int SmallerRoleID)
        {
            if (db.Roles.Find(BiggerRoleID).PriorityOrder < db.Roles.Find(SmallerRoleID).PriorityOrder)
            { return true; }
            return false;
        }


        protected ViewResult getErrorView(HttpStatusCode statusCode, string msg)
        {
            int codeValue = 0;
            switch (statusCode)
            {
                case HttpStatusCode.Unauthorized:
                    codeValue = 401;
                    break;
                case HttpStatusCode.BadRequest:
                    codeValue = 400;
                    break;
                case HttpStatusCode.NotFound:
                    codeValue = 404;
                    break;

            }
            ViewBag.msg = msg;
            return View("HttpErrors", new HttpErrors { StatusCode = codeValue });

        }

        protected ViewResult getErrorView(HttpStatusCode statusCode)
        {
            int codeValue = 0;
            switch (statusCode)
            {
                case HttpStatusCode.Unauthorized:
                    codeValue = 401;
                    break;
                case HttpStatusCode.BadRequest:
                    codeValue = 400;
                    break;
                case HttpStatusCode.NotFound:
                    codeValue = 404;
                    break;

            }
            return View("HttpErrors", new HttpErrors { StatusCode = codeValue });

        }





        protected static class PersonPermissions
        {
            
            public static string VIEW_PERSON_ROLE = "VIEW_PERSON_ROLE";

            public static string CREATE_PERSON_WITHIN_INSTITUTION = "CREATE_PERSON_WITHIN_INSTITUTION";
            public static string VIEW_PERSON_PROFILE = "VIEW_PERSON_PROFILE";
            public static string ATTACH_ROLE_TO_PERSON = "ATTACH_ROLE_TO_PERSON";
            public static string ACTIVATE_PERSON_WITHIN_INSTITUTION = "ACTIVATE_PERSON_WITHIN_INSTITUTION";
            public static string DEACTIVATE_PERSON_WITHIN_INSTITUTION = "DEACTIVATE_PERSON_WITHIN_INSTITUTION";


        }

        protected static class InstitutionPermissions
        {
            public static string CREATE_FILE = "CREATE_FILE";

            public static string CREATE_INSTITUTION = "CREATE_INSTITUTION";
            public static string VIEW_INSTITUTION = "VIEW_INSTITUTION";
            public static string EDIT_INSTITUTION_INFO = "EDIT_INSTITUTION_INFO";
            public static string DEACTIVATE_INSTITUTION = "DEACTIVATE_INSTITUTION";

            public static string ACTIVATE_INSTITUTION = "ACTIVATE_INSTITUTION";
        }

        protected static class FilePermissions
        {
            public static string CREATE_FILE = "CREATE_FILE";
            public static string VIEW_FILE = "VIEW_FILE";
            public static string EDIT_FILE = "EDIT_FILE";
            public static string ADD_VERSION = "ADD_VERSION";
            public static string DELETE_FILE = "DELETE_FILE";

            public static string SET_CURRENT_VERSION = "SET_CURRENT_VERSION";

  

        }
        //protected static class FileLevelPermissions
        //{
        //    public static string EDIT_FILE_VERSION = "EDIT_FILE_VERSION";
        //    public static string CREATE_FILE_VERSION = "CREATE_FILE_VERSION";
        //    public static string VIEW_FILE_CURRENT_VERSION = "VIEW_FILE_CURRENT_VERSION";
        //    public static string VIEW_FILE_ALL_VERSION = "VIEW_FILE_ALL_VERSION";
        //    public static string DELETE_FILE_VERSION = "DELETE_FILE_VERSION";

        //}



    }
}