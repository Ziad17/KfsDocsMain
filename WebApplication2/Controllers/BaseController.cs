using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        protected KfsDocsEntities db = new KfsDocsEntities();




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
                var emprole= db.EmployeeRoles.Where(x=>x.ID==emp.PrimaryRoleID&& x.Active==true&&x.Institution.Active==true).FirstOrDefault();
                return emprole;
            }
            return null;

        }

        [ChildActionOnly]
        public ActionResult RenderMenu()
        {
            var emp = getEmployeeRef();
            if (emp == null)
            {
                signOut();
                return null;
            }
            FileManager fileMg = new FileManager();
            MenuModel viewModel = new MenuModel()
            {
                ID = emp.ID,
                Name = emp.Name,
                Img = fileMg.getImageStream(emp.ImageURL)
            };

            var EmpRole = getPrimaryRole();
            if (EmpRole != null)
            {
                viewModel.canCreateEmployee = hasPersonPermission(EmpRole.RoleID, PersonPermissions.CREATE_PERSON);
              //  viewModel.canDeleteEmployee = hasPersonPermission(EmpRole.RoleID, PersonPermissions.DELETE_PERSON);



                viewModel.canCreateInstitution = hasInstitutionPermission(EmpRole.RoleID, InstitutionPermissions.CREATE_INSTITUTION);
                viewModel.canCreateFile = hasInstitutionPermission(EmpRole.RoleID, InstitutionPermissions.CREATE_FILE);
                viewModel.canCreateLevel = hasInstitutionPermission(EmpRole.RoleID, InstitutionPermissions.CREATE_FILE_LEVEL);


                viewModel.canViewAllRoles = hasInstitutionPermission(EmpRole.RoleID, InstitutionPermissions.VIEW_ROLES);
                viewModel.canAttachRole = hasPersonPermission(EmpRole.RoleID, PersonPermissions.ATTACH_ROLE_TO_PERSON);
                viewModel.canCreateRole = hasInstitutionPermission(EmpRole.RoleID, InstitutionPermissions.CREATE_ROLE);








            }






            return PartialView("_MenuBar",viewModel);
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


        protected bool isPartOfInstitution(int EmployeeRoleID, int InstitutionID)
        {
            var Institution = db.Institutions.Find(InstitutionID);
            var EmpRole = db.EmployeeRoles.Find(EmployeeRoleID);
            if (EmpRole.InstitutionID == InstitutionID || Institution.ParentID==EmpRole.InstitutionID)
            { return true; }
            return false;
        }


        
        protected IQueryable<File> getMyFiles()
        {
            var emp = getEmployeeRef();
            var myrole = getPrimaryRole();
            if (myrole == null)
            {
                return Enumerable.Empty<File>().AsQueryable();
            }

            return db.Files.Where(x => x.AuthorID==myrole.ID && x.Active == true);

       
        }

        protected IQueryable<File> getMyMentions()
        {
            var emp = getEmployeeRef();
            return db.FileMentions.Where(x => x.EmployeeID == emp.ID).Select(x => x.File).Where(x => x.Active == true);
        }




        protected IQueryable<File> getMentionsForSomeone(int empID)
        {

            //return SomoneMentions.Intersect(MyMentions+getAvaiableFilesForMeByScope+getMyFiles)

            var emp = getEmployeeRef();

            return db.FileMentions.Where(x => x.EmployeeID==empID ).Select(x=>x.File).Where(x=>x.Active==true);


        }

        //protected IQueryable<File> getSomeoneFiles(int id)
        //{
        //    //return getSomeoneFiles.Intersect(MyMentions+getAvaiableFilesForMeByScope)

        //    var emp = db.Employees.Find(id);

        //    return db.Files.Where(x => myroles.Contains(x.AuthorID) && x.Active == true);

        //}





        protected IQueryable<File> getAvaiableFilesForMeByScope()
        {
            
            //return files that available in the scope
                var myRole = getPrimaryRole();
                //
                if (myRole == null)
                {
                return Enumerable.Empty<File>().AsQueryable();
                }
               
                List<int> levels = db.FilesScopes.Where(x => x.RoleID ==myRole.RoleID).Select(x => x.LevelID).Distinct().ToList();

                var AvailableFiles = db.Files.Where(x => levels.Contains(x.LevelID) && x.EmployeeRole.InstitutionID == myRole.InstitutionID);

                return AvailableFiles;

        }



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

       



        protected bool hasFileLevelPermission(int roleID, int fileLevelID, string permissionName,int EmpRoleID,int FileID)
        {

            if ((db.FilesScopes.Where(s => s.RoleID == roleID && s.LevelID == fileLevelID && s.Permission.Equals(permissionName)).Count() == 1))
            {
                return isFileinTheSameInstitution(EmpRoleID,FileID);
            }
            return false;
        }

        protected bool isFileinTheSameInstitution(int EmproleID, int fileID)
        {

            var File = db.Files.Find(fileID);
            var EmpRole = db.EmployeeRoles.Where(x=>x.ID==EmproleID && x.Active==true).FirstOrDefault();
            if (File == null || EmpRole==null)
            {
            return false;

            }
            if (File.EmployeeRole.InstitutionID==EmpRole.InstitutionID)
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


         protected bool operationValidInInstitution(int EmpRoleID, int institutionToCompareID)
        {

            var myInstitution = db.EmployeeRoles.Find(EmpRoleID).Institution;
            if (myInstitution.Active == false)
            {
                return false;

            }
            var availableInstitutions = getChildrenInstitutionWithParent(myInstitution.ID);
            if (availableInstitutions.Contains(db.Institutions.Find(institutionToCompareID)))
                { return true; }
            return false;
           
        }


        protected IEnumerable<Institution> getChildrenInstitutionWithParent(int institutionID)
        {

            var myInstitution = db.Institutions.Find(institutionID);

            var availableInstitutions = getChildrenInstitution(institutionID);
            return availableInstitutions.Append(myInstitution);



        }

        protected IQueryable<Institution> getChildrenInstitution(int institutionID)
        {

            return db.Institutions.Where(x=>x.ParentID==institutionID && x.Active==true).OrderBy(x=>x.ArabicName);
        }

        protected bool hasInstitutionPermission(int roleID, string permissionName)
        {
            if (db.RoleInstitutionPermissions.Where(s => s.RoleID == roleID && s.PermissionName.Equals(permissionName)).Count() == 1)
            {
                return true;
            }
            return false;
        }

        protected bool isRolePriorityValidInTheSameInstitution(int BiggerRoleID, int SmallerRoleID)
        {
            if (db.Roles.Find(BiggerRoleID).PriorityOrder < db.Roles.Find(SmallerRoleID).PriorityOrder)
            { return true; }
            return false;
        }
        protected bool isRolePriorityValidInTheChildInstitution(int BiggerRoleID, int SmallerRoleID)
        {
            if (db.Roles.Find(BiggerRoleID).PriorityOrder <= db.Roles.Find(SmallerRoleID).PriorityOrder)
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
   

            public static string DELETE_PERSON = "DELETE_PERSON";
            public static string CREATE_PERSON = "CREATE_PERSON_WITHIN_INSTITUTION";
            public static string VIEW_PERSON_PROFILE = "VIEW_PERSON_PROFILE";


            public static string VIEW_ALL_EMPLOYEE_ROLES = "VIEW_ALL_EMPLOYEE_ROLES";

            public static string DELETE_EMPLOYEE_ROLE = "DELETE_EMPLOYEE_ROLE";
            public static string VIEW_EMPLOYEE_ROLE = "VIEW_EMPLOYEE_ROLE";
            public static string ATTACH_ROLE_TO_PERSON = "ATTACH_ROLE_TO_PERSON";
            public static string ACTIVE_EMPLOYEE_ROLE = "ACTIVATE_EMPLOYEE_ROLE";
            public static string DEACTIVATE_EMPLOYEE_ROLE = "DEACTIVATE_EMPLOYEE_ROLE";


        }

        protected static class InstitutionPermissions
        {
            public static string EDIT_FILE_LEVEL = "EDIT_FILE_LEVEL";
            public static string DELETE_FILE_LEVEL = "DELETE_FILE_LEVEL";
            public static string CREATE_FILE_LEVEL = "CREATE_FILE_LEVEL";
            public static string CREATE_FILE = "CREATE_FILE";
            
            public static string CREATE_INSTITUTION = "CREATE_INSTITUTION";
            public static string VIEW_INSTITUTION = "VIEW_INSTITUTION";
            public static string EDIT_INSTITUTION_INFO = "EDIT_INSTITUTION_INFO";



            public static string VIEW_INSTITUTION_TYPES = "VIEW_INSTITUTION_TYPES";
            public static string CREATE_INSTITUTION_TYPE = "CREATE_INSTITUTION_TYPE";
            public static string DELETE_INSTITUTION_TYPE = "DELETE_INSTITUTION_TYPE";
            public static string EDIT_INSTITUTION_TYPE = "EDIT_INSTITUTION_TYPE";


            public static string VIEW_ROLES = "VIEW_ROLES";
            public static string CREATE_ROLE = "CREATE_ROLE";
            public static string DELETE_ROLE = "DELETE_ROLE";
            public static string EDIT_ROLE = "EDIT_ROLE";



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