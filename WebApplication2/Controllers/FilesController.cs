using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Managers;
using WebApplication2.Models;
using WebApplication2.Models.ViewModels;

namespace WebApplication2.Controllers
{
    public class FilesController : BaseController
    {

      

        public ActionResult View(int? id)
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

            File file = db.Files.Find(id);
            if (file == null)
            {
                return getErrorView(HttpStatusCode.NotFound);

            }
            if (hasFileLevelPermission(EmpRole.Role.ID, file.LevelID, FilePermissions.VIEW_FILE,EmpRole.ID,file.ID) || isFileAuthor(file.ID, EmpRole.ID))
            {


                List<FileVersion> versions = db.FileVersions.Where(x => x.FileID == file.ID && x.ID != file.CurrentVersion).ToList<FileVersion>();
                FileVersion currentVersion=null;
                if (file.CurrentVersion != null)
                {
                    currentVersion = db.FileVersions.Find(file.CurrentVersion);
                }

                EmployeeRole owner = db.EmployeeRoles.Find(file.AuthorID);

                ViewFileModel viewModel = new ViewFileModel()
                {
                    File = file,

                    Owner=owner,
                    Versions=versions,
                    CurrentVersion=  currentVersion
                    
                    
                };

                var bookmark = db.Bookmarks.Where(x => x.EmployeeID == EmpRole.ID && x.FileID == file.ID).FirstOrDefault();
                if (bookmark != null)
                {
                    viewModel.isBookmarked = true;
                }
                viewModel.isAuthor = isFileAuthor(file.ID, EmpRole.ID);
                viewModel.canEdit = hasFileLevelPermission(EmpRole.Role.ID, file.LevelID, FilePermissions.EDIT_FILE,EmpRole.ID,file.ID) || isFileAuthor(file.ID, EmpRole.ID);
                viewModel.canAddVersion = hasFileLevelPermission(EmpRole.Role.ID, file.LevelID, FilePermissions.ADD_VERSION, EmpRole.ID, file.ID) || isFileAuthor(file.ID, EmpRole.ID);
                viewModel.canDelete = hasFileLevelPermission(EmpRole.Role.ID, file.LevelID, FilePermissions.DELETE_FILE, EmpRole.ID, file.ID) || isFileAuthor(file.ID, EmpRole.ID);
                viewModel.canSetCurrentVersion = hasFileLevelPermission(EmpRole.Role.ID, file.LevelID, FilePermissions.SET_CURRENT_VERSION, EmpRole.ID, file.ID) || isFileAuthor(file.ID, EmpRole.ID);
                viewModel.MyRef = EmpRole;
                return View(viewModel);

            }



            return getErrorView(HttpStatusCode.Unauthorized);






        }


        public ActionResult SetCurrentVersion(int id)
        {
            var EmpRole = getPrimaryRole();

            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }
            var fileVersion = db.FileVersions.Find(id);
            var file = db.Files.Find(fileVersion.FileID);
            if (fileVersion == null || file==null)
            { return getErrorView(HttpStatusCode.NotFound); }



                  
            if (hasFileLevelPermission(EmpRole.Role.ID, file.LevelID, FilePermissions.SET_CURRENT_VERSION, EmpRole.ID, file.ID) || isFileAuthor(file.ID, EmpRole.ID))
            {
                db.Files.Find(file.ID).CurrentVersion = fileVersion.ID;
                db.FileActionLogs.Add(createFileLog(EmpRole.ID, file.ID, FilePermissions.SET_CURRENT_VERSION));
                db.SaveChanges();
                return RedirectToAction("View",new { id=file.ID});

            }
            return getErrorView(HttpStatusCode.Unauthorized);


        }


        private FileActionLog createFileLog(int EmpID,int FileID,string permission)
        {
            return new FileActionLog() { EmployeeID = EmpID,
                FileID = FileID,
                ActionDate=DateTime.Now,
                PermissionName = permission };
                
        }





        public ActionResult Create()
        {



            var EmpRole = getPrimaryRole();

            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }
            if (hasInstitutionPermission(EmpRole.Role.ID, InstitutionPermissions.CREATE_FILE))
            {


                CreateFileModel viewModel = new CreateFileModel();
                viewModel.Levels = new SelectList(db.FileLevels, "ID", "Name");

                viewModel.InstitutionName = EmpRole.Institution.ArabicName;
                viewModel.RoleName = EmpRole.Role.ArabicName;
                viewModel.AuthorName = EmpRole.Employee.Name;
                viewModel.DateCreated = DateTime.Now;
                viewModel.RoleID = EmpRole.RoleID;
                viewModel.InstitutionID = EmpRole.InstitutionID;
                viewModel.AvailableEmployees = db.Employees.Where(x => x.ID != EmpRole.EmployeeID && x.Active == true).Select(x=>new SelectListItem() {Text=x.Name,Value=x.ID.ToString()  }).ToList();
                return View(viewModel);
            }
            return getErrorView(HttpStatusCode.Unauthorized);

        }
        public ActionResult CreateLevel()
        {
            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);

            }
            if (hasInstitutionPermission(EmpRole.RoleID, InstitutionPermissions.CREATE_FILE_LEVEL))
            {

                var availableRolesIDS = db.Roles.Where(x => x.ParentID != null).Select(x=>x.ID).ToList();
                var availableRolesNames = db.Roles.Where(x => x.ParentID != null).Select(x => x.ArabicName).ToList();

                //   Dictionary<int, List<SelectListItem>> roleToPermissions = new Dictionary<int, List<SelectListItem>>();


                var availablePermissions = db.FilePermissions.Where(x => x.Grantable == true).Select(x => new SelectListItem() { Text = x.ArabicName, Value = x.Name }).ToList();
                var overAllPermissions = new List<List<SelectListItem>>();
                for (int i=0;i<availableRolesIDS.Count;i++)
                {
                    overAllPermissions.Add(availablePermissions);
                }

                CreateFileLevelModel viewModel = new CreateFileLevelModel()
                {

                    RolesIds = availableRolesIDS,
                    RolesNames= availableRolesNames,
                    Permissions = overAllPermissions

                };


                return View(viewModel);
            }
            return getErrorView(HttpStatusCode.Unauthorized);

        }

        public ActionResult DeleteLevel(int? id)
        {
            if (id == null)
            {
                return getErrorView(HttpStatusCode.BadRequest);
            }
            FileLevel level = db.FileLevels.Find(id);
            if (level == null)
            {
                return getErrorView(HttpStatusCode.NotFound);

            }
            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);

            }

            if (hasInstitutionPermission(EmpRole.RoleID, InstitutionPermissions.DELETE_FILE_LEVEL) && isFileLevelDeleteable(level.ID))
            {


                DeleteFileLevelModel viewModel = new DeleteFileLevelModel();
                List<Role> roles = db.Roles.Where(x => x.ParentID != null).OrderBy(x => x.PriorityOrder).ToList<Role>();

                Dictionary<String, List<String>> rolesAndPermissions = new Dictionary<string, List<string>>();

                foreach (var role in roles)
                {
                    List<String> permissions = db.FilesScopes.Where(x => x.RoleID == role.ID && x.LevelID == level.ID).Select(x => x.FilePermission.ArabicName).ToList<String>();
                    rolesAndPermissions.Add(role.ArabicName, permissions);




                }
                viewModel.Name = level.Name;
                viewModel.Desc = level.LevelDesc;

                viewModel.ID = level.ID;
                viewModel.RolesDirectory = rolesAndPermissions;






                return View(viewModel);





            }
            return getErrorView(HttpStatusCode.Unauthorized);





        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteLevel(DeleteFileLevelModel viewModel)
        {
            int id = viewModel.ID;
            if (id==0)
            {
                return getErrorView(HttpStatusCode.BadRequest);
            }
            FileLevel level = db.FileLevels.Find(id);
            if (level == null)
            {
                return getErrorView(HttpStatusCode.NotFound);

            }
            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);

            }

            if (hasInstitutionPermission(EmpRole.RoleID, InstitutionPermissions.DELETE_FILE_LEVEL) && isFileLevelDeleteable(level.ID))
            {








                foreach (var scope in db.FilesScopes.Where(x => x.LevelID == level.ID))
                {
                    db.FilesScopes.Remove(scope);
                }

                db.FileLevels.Remove(level);



                db.SaveChanges();


                return RedirectToAction("FileLevels");





            }
            return getErrorView(HttpStatusCode.Unauthorized);





        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditLevel(EditFileLevelModel viewModel)
        {
            int? id = viewModel.ID;
            if (id == null)
            {
                return getErrorView(HttpStatusCode.BadRequest);
            }
            FileLevel level = db.FileLevels.Find(id);
            if (level == null)
            {
                return getErrorView(HttpStatusCode.NotFound);

            }
            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);

            }

            if (hasInstitutionPermission(EmpRole.RoleID, InstitutionPermissions.EDIT_FILE_LEVEL))
            {



                db.FileLevels.Find(level.ID).Name = viewModel.levelName;
                db.FileLevels.Find(level.ID).LevelDesc = viewModel.Desc;



            
                foreach (var scopre in db.FilesScopes.Where(x=>x.LevelID==level.ID))
                {
                    db.FilesScopes.Remove(scopre);
                }
                if (viewModel.Permissions != null)
                {
                    for (int i = 0; i < viewModel.Permissions.Count; i++)
                    {
                        foreach (var perm in viewModel.Permissions[i])
                        {
                            if (perm.Selected)
                            {

                                FilesScope filesScope = new FilesScope()
                                {
                                    RoleID = viewModel.RolesIds[i],
                                    Permission = perm.Value,
                                    LevelID = level.ID
                                };
                                db.FilesScopes.Add(filesScope);

                            }
                        }
                    }
                }
                db.SaveChanges();


                return RedirectToAction("FileLevels","Files",new { id=level.ID});




            }
            return getErrorView(HttpStatusCode.Unauthorized);




        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLevel(CreateFileLevelModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var EmpRole = getPrimaryRole();                                                                    
                if (EmpRole == null)
                {
                    return getErrorView(HttpStatusCode.Unauthorized);

                }
                if (hasInstitutionPermission(EmpRole.RoleID, InstitutionPermissions.CREATE_FILE_LEVEL))
                {




                    FileLevel fileLevel = new FileLevel();
                    fileLevel.Name = viewModel.levelName;
                    fileLevel.LevelDesc = viewModel.Desc;

                    db.FileLevels.Add(fileLevel);

                    if (viewModel.Permissions != null)
                    {
                        for (int i = 0; i < viewModel.Permissions.Count; i++)
                        {
                            foreach (var perm in viewModel.Permissions[i])
                            {
                                if (perm.Selected)
                                {

                                    FilesScope filesScope = new FilesScope()
                                    {
                                        RoleID = viewModel.RolesIds[i],
                                        Permission = perm.Value,
                                        LevelID = fileLevel.ID
                                    };
                                    db.FilesScopes.Add(filesScope);

                                }
                            }
                        }
                    }
                    db.SaveChanges();



                 return RedirectToAction("FileLevels", "Files", new { id = fileLevel.ID });






                }
                return getErrorView(HttpStatusCode.Unauthorized);
            }

            return View(viewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateFileModel viewModel)
        {


            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }

            if (ModelState.IsValid)
            {

                if (hasInstitutionPermission(EmpRole.Role.ID, FilePermissions.CREATE_FILE))
                {
                    File file = new File()
                    {
                        Name = viewModel.Name,
                        LevelID = viewModel.LevelID,
                        DateCreated = viewModel.DateCreated

                    };
                    file.Active = true;
                    file.Locked = false;
                    file.DateCreatedSys = DateTime.Now;
                    file.AuthorID = EmpRole.ID;

                    db.Files.Add(file);


                    FileActionLog log = new FileActionLog() { EmployeeID = EmpRole.ID, FileID = file.ID, ActionDate = DateTime.Now, PermissionName = FilePermissions.CREATE_FILE };

                    db.FileActionLogs.Add(log);

                    List<int> MentionedIDs = new List<int>();
                    foreach (var emp in viewModel.AvailableEmployees)
                    {
                        if (emp.Selected)
                        {
                            MentionedIDs.Add(int.Parse(emp.Value));
                        }
                    }


                    foreach (var ID in MentionedIDs)
                    {
                        FileMention mention = new FileMention()
                        {
                            FileID = file.ID,
                            EmployeeID = ID,
                            CreatorID = EmpRole.ID,
                            DateCreated = DateTime.Now,
                            Seen = false
                        };
                        db.FileMentions.Add(mention);
                    }


                    db.SaveChanges();
                    return RedirectToAction("AddVersion", new { id = file.ID });


                }
                return getErrorView(HttpStatusCode.Unauthorized);

            }


            viewModel.Levels = new SelectList(db.FileLevels, "ID", "Name");

            return View(viewModel);
        }





        public ActionResult AddVersion(int id)
        {

            var EmpRole = getPrimaryRole();

            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }
            var file = db.Files.Find(id);
            if (file == null)
            { return getErrorView(HttpStatusCode.NotFound); }



            //      
            if (hasFileLevelPermission(EmpRole.Role.ID, file.LevelID, FilePermissions.ADD_VERSION, EmpRole.ID, file.ID) || isFileAuthor(id, EmpRole.ID))
            {
                AddVersionModel model = new AddVersionModel()
                {
                    FileID = id,
                    FileName = file.Name,
                    DateCreated = DateTime.Now

                };
                ViewBag.FileName = file.Name;
                return View(model);

            }
            return getErrorView(HttpStatusCode.Unauthorized);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddVersion( AddVersionModel model)
        {

            FileVersion fileVersion = new FileVersion()
            {
                Name = model.Name,
                DateCreated=model.DateCreated,
                Notes=model.Notes,
                FileID=model.FileID
                

            };

            HttpPostedFileBase file_content = model.file_content;
            if (fileVersion.FileID == null)
            { return getErrorView(HttpStatusCode.BadRequest); }

            int id =(int) fileVersion.FileID;
            var EmpRole = getPrimaryRole();

            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }
             var file = db.Files.Find(id);
            if (file == null)
            {
                  return getErrorView(HttpStatusCode.NotFound);

            }


            if (ModelState.IsValid)
            {


                //      
                if (hasFileLevelPermission(EmpRole.Role.ID, file.LevelID, FilePermissions.ADD_VERSION, EmpRole.ID, file.ID) || isFileAuthor(id, EmpRole.ID))
                {

                    try
                    {
                        var FileSizeInKb = 512;
                        var supportedFiles = db.FileTypes.Select(x => x.Extension);
                        var fileExt = System.IO.Path.GetExtension(file_content.FileName).Substring(1).ToLower();
                        if (!supportedFiles.Contains(fileExt))
                        {
                            var err_msg = "Only File Format Is Allowed (";
                            foreach (string ext in supportedFiles)
                            {
                                err_msg += ext + ",";
                            }
                            err_msg += ")";
                            ModelState.AddModelError("", err_msg);
                            return View(model);
                        }
                        if (file_content != null)
                        {

                            if (file_content.ContentLength > FileSizeInKb * 1024)
                            {
                                ModelState.AddModelError("", "The File Size Should Not Exceeding " + FileSizeInKb + " Kb");
                                return View(model);

                            }
                            System.IO.StreamReader st = new System.IO.StreamReader(file_content.InputStream);
                            FileManager filMG = new FileManager();
                            EncryptionManager encryptionManager = new EncryptionManager(this);


                            string fileName = FileManager.RandomString(1) + encryptionManager.Encrypt(file_content.FileName);
                            if (fileName.Length > 10)
                            {
                                fileName = fileName.Substring(0, 10) + "." + fileExt;
                            }
                            if (filMG.uploadFile(fileName, st.BaseStream))
                            {

                                FileContent fileContent = new FileContent()
                                {
                                    FileSize = file_content.ContentLength,
                                    Href = fileName
                                };
                                db.FileContents.Add(fileContent);


                                fileVersion.AuthorID = EmpRole.ID;
                                fileVersion.DateCreatedSys = DateTime.Now;
                                fileVersion.FileContentID = fileContent.ID;
                                fileVersion.FileTypeName = db.FileTypes.Where(x=> x.Extension.ToLower().Equals(fileExt)).FirstOrDefault().Name;

                                db.FileVersions.Add(fileVersion);


                                FileActionLog log1 = new FileActionLog() { EmployeeID = EmpRole.ID, FileID = file.ID, ActionDate = DateTime.Now, PermissionName = FilePermissions.ADD_VERSION };

                                db.FileActionLogs.Add(log1);

                                db.SaveChanges();
                                //return RedirectToAction("AddVersion", new { id = file.ID });
                                return RedirectToAction("View",new { id=file.ID});


                            }
                            else
                            {
                                ModelState.AddModelError("", "Uploading Failed");
                                return View(model);

                            }



                        }
                        else
                        {
                            ModelState.AddModelError("", "File Is Empty");
                            return View(model);

                        }
                    }



                    catch (Exception e)
                    {
                        ModelState.AddModelError("", "Internal Error");
                        return View(model);
                    }



                }
                return getErrorView(HttpStatusCode.Unauthorized);

            }
           
            ModelState.AddModelError("", "Input Error");

            return View(model);

        }



        public ActionResult Edit(int? id)
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

            File file = db.Files.Find(id);
            if (file == null)
            {
                return getErrorView(HttpStatusCode.NotFound);

            }
            if (hasFileLevelPermission(EmpRole.Role.ID, file.LevelID, FilePermissions.EDIT_FILE, EmpRole.ID, file.ID) || isFileAuthor(file.ID, EmpRole.ID))
            {

                var Mentions = db.FileMentions.Where(x => x.FileID == file.ID).Select(x => x.EmployeeID).ToList();
                var availableEmployees = db.Employees.Where(x => x.ID != EmpRole.EmployeeID && x.Active == true && x.ID!=file.EmployeeRole.EmployeeID).Select(x => new SelectListItem() { Text = x.Name, Value = x.ID.ToString() }).ToList();

                EditFileModel viewModel = new EditFileModel()
                {
                     AvailableEmployees=availableEmployees,
                     ID=file.ID,
                     FileName=file.Name

            };
                foreach (var emp in availableEmployees)
                {
                    if (Mentions.Contains(int.Parse(emp.Value)))
                    { emp.Selected = true; }
                }
                if (isFileAuthor(file.ID, EmpRole.ID))
                {
                    viewModel.Levels = new SelectList(db.FileLevels, "ID", "Name", file.LevelID);

                }
                else {
                    var availLevels = db.FilesScopes.Where(x => x.Permission == FilePermissions.EDIT_FILE && x.RoleID == EmpRole.RoleID).Select(x => x.LevelID);
                viewModel.Levels = new SelectList(db.FileLevels.Where(x=>availLevels.Contains(x.ID)), "ID", "Name", file.LevelID);

                }
                return View(viewModel);
            }
            return getErrorView(HttpStatusCode.Unauthorized);

            
        }



    

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditFileModel viewModel)
        {
            if (ModelState.IsValid)
            {
                File acutalFile = db.Files.Find(viewModel.ID);
                if (acutalFile == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                var EmpRole = getPrimaryRole();
                if (EmpRole == null)
                {
                    return getErrorView(HttpStatusCode.Unauthorized);
                }

                if (hasFileLevelPermission(EmpRole.Role.ID, acutalFile.LevelID, FilePermissions.EDIT_FILE, EmpRole.ID, acutalFile.ID) || isFileAuthor(acutalFile.ID, EmpRole.ID))
                {

                    db.Files.Find(acutalFile.ID).Name = viewModel.FileName;
                    db.Files.Find(acutalFile.ID).LevelID = viewModel.LevelID;

                    foreach (var per in db.FileMentions.Where(x => x.FileID == acutalFile.ID))
                    {
                       
                        db.FileMentions.Remove(per);
                    }

                    List<int> MentionedIDs = new List<int>();
                    foreach (var emp in viewModel.AvailableEmployees)
                    {
                        if (emp.Selected)
                        {
                            MentionedIDs.Add(int.Parse(emp.Value));
                        }
                    }


                    foreach (var ID in MentionedIDs)
                    {
                        FileMention mention = new FileMention()
                        {
                            FileID = acutalFile.ID,
                            EmployeeID = ID,
                            CreatorID = EmpRole.ID,
                            DateCreated = DateTime.Now,
                            Seen = false
                        };
                        db.FileMentions.Add(mention);
                    }

                    db.SaveChanges();
                    return RedirectToAction("View", new { id = acutalFile.ID });

                }
                return getErrorView(HttpStatusCode.Unauthorized);
            }
            viewModel.Levels = new SelectList(db.FileLevels, "ID", "Name", viewModel.Levels.SelectedValue);
            return View(viewModel);
        }


        public ActionResult Delete(int? id)
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

            File file = db.Files.Find(id);
            if (file == null)
            {
                return getErrorView(HttpStatusCode.NotFound);

            }
            if (hasFileLevelPermission(EmpRole.Role.ID, file.LevelID, FilePermissions.DELETE_FILE, EmpRole.ID, file.ID) || isFileAuthor(file.ID, EmpRole.ID))
            {


                return View(file);
            }
            return getErrorView(HttpStatusCode.Unauthorized);

        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {


                File acutalFile = db.Files.Find(id);
                if (acutalFile == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                var EmpRole = getPrimaryRole();
                if (EmpRole == null)
                {
                    return getErrorView(HttpStatusCode.Unauthorized);
                }

                if (hasFileLevelPermission(EmpRole.Role.ID, acutalFile.LevelID, FilePermissions.DELETE_FILE, EmpRole.ID, acutalFile.ID) ||isFileAuthor(acutalFile.ID, EmpRole.ID))
                {

                //try
                //{
                foreach (var filelog in db.FileActionLogs.Where(x=>x.FileID==acutalFile.ID))
                {
                    db.FileActionLogs.Remove(filelog);
                }
                foreach (var fileversion in db.FileVersions.Where(x => x.FileID == acutalFile.ID)) 
                    {
                        db.FileVersions.Remove(fileversion);
                    }
                    foreach (var fileMention in db.FileMentions.Where(x => x.FileID == acutalFile.ID))
                    {
                        db.FileMentions.Remove(fileMention);
                    }
                 
                    foreach (var bookmark in db.Bookmarks.Where(x => x.FileID == acutalFile.ID))
                    {
                        db.Bookmarks.Remove(bookmark);
                    }
                    db.Files.Remove(acutalFile);
                    
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");

                //}
                //catch (Exception e) {
                //    return getErrorView(HttpStatusCode.InternalServerError);

                //}
            }
                return getErrorView(HttpStatusCode.Unauthorized);


        }

        public ActionResult FileLevels(int?   id)
        {
            var emp = getEmployeeRef();
            if (emp == null)
            {
                signOut();
                return null;
            }


            var level_from_db = db.FileLevels.Find(id);
            if (level_from_db == null)
            {
                level_from_db = db.FileLevels.FirstOrDefault();
                if (level_from_db == null)
                {
                    return RedirectToAction("CreateLevel");
                }
            }

            ViewFileLevelsModel viewModel = new ViewFileLevelsModel();
            List<String> levels = db.FileLevels.Select(x => x.Name).ToList<String>();
             Dictionary<String, List<String>> DICT = new  Dictionary<string, List<string>>();
          
                List<Role> roles = db.Roles.Where(x => x.ParentID != null).OrderBy(x=>x.PriorityOrder).ToList<Role>();

                Dictionary<String, List<String>> rolesAndPermissions = new Dictionary<string, List<string>>();

                foreach (var role in roles)
                {
                    List<String> permissions = db.FilesScopes.Where(x => x.RoleID == role.ID && x.LevelID== level_from_db.ID).Select(x => x.FilePermission.ArabicName).ToList<String>();
                    rolesAndPermissions.Add(role.ArabicName, permissions);

                }


                
            ViewBag.Levels = new SelectList( db.FileLevels,"ID","Name",level_from_db.ID);

            var EmpRole = getPrimaryRole();

            viewModel.Level = level_from_db;
            viewModel.RolesDirectory = rolesAndPermissions;
            if (EmpRole != null)
            {
                viewModel.canEdit = hasInstitutionPermission(EmpRole.Role.ID, InstitutionPermissions.EDIT_FILE_LEVEL);
                viewModel.canDelete = hasInstitutionPermission(EmpRole.Role.ID, InstitutionPermissions.EDIT_FILE_LEVEL) && isFileLevelDeleteable(level_from_db.ID);
              }
            return View(viewModel);
            
        }

        private bool isFileLevelDeleteable(int fileLevelID)
        {
            var files = db.Files.Where(x => x.LevelID == fileLevelID);
            if (files == null || files.Count() == 0)
            {
                return true;
            }
            return false;
        }

        public ActionResult DeleteVersion(int id)
        {
            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }
            var Version = db.FileVersions.Find(id);
            if (Version == null)
            {
                return getErrorView(HttpStatusCode.NotFound);

            }
            if (Version.AuthorID == EmpRole.ID || Version.File.AuthorID == EmpRole.ID)
            {
                try 
                {
                    int fileID = (int)Version.FileID;
                    db.Files.Find( Version.FileID).CurrentVersion = null;
                    db.FileVersions.Remove(Version);
                    db.SaveChanges();
                    return RedirectToAction("View", new { id = fileID });
                }
                catch (Exception e) {
                    return getErrorView(HttpStatusCode.InternalServerError);

                }

            }
                return getErrorView(HttpStatusCode.Unauthorized);
            

        }


 
        public ActionResult Mentions()
        {
            var myEmp = getEmployeeRef();
            if (myEmp == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }

            var Files=db.FileMentions.Where(x => x.EmployeeID == myEmp.ID && x.CreatorID != myEmp.ID);
            foreach (var file in Files)
            {
                if (!file.Seen)
                {
                    file.Seen = true;
                }
            }
            db.SaveChanges();
            return View(Files);

        }

        public ActionResult EditLevel(int? id)
        {

            if (id == null)
            {
                return getErrorView(HttpStatusCode.BadRequest);
            }
            FileLevel level = db.FileLevels.Find(id);
            if (level == null)
            {
                return getErrorView(HttpStatusCode.NotFound);

            }
            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);

            }
            List<string> list = new List<string>();

            if (hasInstitutionPermission(EmpRole.RoleID, InstitutionPermissions.EDIT_FILE_LEVEL))
            {


                var availableRolesIDS = db.Roles.Where(x => x.ParentID != null).Select(x => x.ID).ToList();
                var availableRolesNames = db.Roles.Where(x => x.ParentID != null).Select(x => x.ArabicName).ToList();



                var overAllPermissions = new List<List<SelectListItem>>();

                for (int i = 0; i < availableRolesIDS.Count; i++)
                {
                    var availablePermissions = db.FilePermissions.Where(x => x.Grantable == true).Select(x => new SelectListItem() { Text = x.ArabicName, Value = x.Name }).ToList();

                    int roleID = availableRolesIDS[i];


                    var alreadyGrantedPerms = db.FilesScopes.Where(x => x.RoleID == roleID && x.LevelID == level.ID).Select(x => x.Permission).ToList();
                
                    for (int j=0;j < availablePermissions.Count;j++)
                    {
                        if (alreadyGrantedPerms.Contains(availablePermissions[j].Value))
                        {
                            availablePermissions[j].Selected = true;
                        }
                    }
                    overAllPermissions.Add(availablePermissions);


                }


                ViewBag.g = list;

                EditFileLevelModel viewModel = new EditFileLevelModel()
                {

                    levelName=level.Name,
                    Desc=level.LevelDesc,
                    ID=level.ID,
                    RolesIds = availableRolesIDS,
                    RolesNames = availableRolesNames,
                    Permissions = overAllPermissions

                };

            return View(viewModel);


            }




            return getErrorView(HttpStatusCode.Unauthorized);














            


        }

        public ActionResult Download(int? id)//version ID
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
            FileVersion fileVersion = db.FileVersions.Find(id);
            if (fileVersion == null)
            {
                return getErrorView(HttpStatusCode.NotFound);

            }
            if (hasFileLevelPermission(EmpRole.Role.ID, fileVersion.File.LevelID, FilePermissions.VIEW_FILE, EmpRole.ID, fileVersion.File.ID) || isFileAuthor((int)fileVersion.FileID, EmpRole.ID))
            {
                FileManager fileMG = new FileManager();
                return File(fileMG.getFileStream(fileVersion.FileContent.Href), "applicatin/force-download", fileVersion.Name+"."+fileVersion.FileType.Extension);


            }
            return getErrorView(HttpStatusCode.Unauthorized);



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
