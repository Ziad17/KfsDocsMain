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
            if (hasFileLevelPermission(EmpRole.Role.ID, file.Level, FilePermissions.VIEW_FILE) || isFileAuthor(file.ID, EmpRole.ID))
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
                viewModel.canEdit = hasFileLevelPermission(EmpRole.Role.ID, file.Level, FilePermissions.EDIT_FILE) || isFileAuthor(file.ID, EmpRole.ID);
                viewModel.canAddVersion = hasFileLevelPermission(EmpRole.Role.ID, file.Level, FilePermissions.ADD_VERSION) || isFileAuthor(file.ID, EmpRole.ID);
                viewModel.canDelete = hasFileLevelPermission(EmpRole.Role.ID, file.Level, FilePermissions.DELETE_FILE) || isFileAuthor(file.ID, EmpRole.ID);
                viewModel.canSetCurrentVersion = hasFileLevelPermission(EmpRole.Role.ID, file.Level, FilePermissions.SET_CURRENT_VERSION) || isFileAuthor(file.ID, EmpRole.ID);

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



                  
            if (hasFileLevelPermission(EmpRole.Role.ID, file.Level, FilePermissions.SET_CURRENT_VERSION) || isFileAuthor(file.ID, EmpRole.ID))
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
                ViewBag.Level = new SelectList(db.FileLevels, "Level", "Level");
                viewModel.InstitutionName = EmpRole.Institution.ArabicName;
                viewModel.RoleName = EmpRole.Role.ArabicName;
                viewModel.AuthorName = EmpRole.Employee.Name;
                viewModel.DateCreated = DateTime.Now;
                return View();
            }
            return getErrorView(HttpStatusCode.Unauthorized);

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
                        Name=viewModel.Name,
                        Level=viewModel.Level,
                        DateCreated=viewModel.DateCreated

                    };
                    file.Active = true;
                    file.Locked = false;
                    file.DateCreatedSys = DateTime.Now;
                    file.AuthorID = EmpRole.ID;

                    db.Files.Add(file);


                    FileActionLog log = new FileActionLog() { EmployeeID = EmpRole.ID, FileID = file.ID, ActionDate = DateTime.Now, PermissionName = FilePermissions.CREATE_FILE };

                    db.FileActionLogs.Add(log);


                    foreach (var ID in viewModel.MentionedIDs)
                    {
                        FileMention mention = new FileMention()
                        {
                            FileID=file.ID,
                            EmployeeID=ID,
                            CreatorID=EmpRole.ID,
                            DateCreated=DateTime.Now,
                            Seen=false
                        };
                        db.FileMentions.Add(mention);
                    }


                    db.SaveChanges();
                    return RedirectToAction("AddVersion", new { id = file.ID });


                }
                return getErrorView(HttpStatusCode.Unauthorized);

            }


            ViewBag.Level = new SelectList(db.FileLevels, "Level", "Level", viewModel.Level);
            return View();
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
            if (hasFileLevelPermission(EmpRole.Role.ID, file.Level, FilePermissions.ADD_VERSION) || isFileAuthor(id, EmpRole.ID))
            {
                AddVersionModel model = new AddVersionModel()
                {
                    FileID = id,
                    Name = file.Name,
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
                if (hasFileLevelPermission(EmpRole.Role.ID, file.Level, FilePermissions.ADD_VERSION) || isFileAuthor(id, EmpRole.ID))
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
            if (hasFileLevelPermission(EmpRole.Role.ID, file.Level, FilePermissions.EDIT_FILE) || isFileAuthor(file.ID, EmpRole.ID))
            {

                
                ViewBag.Level = new SelectList(db.FileLevels, "Level", "Level", file.Level);
                return View(file);
            }
            return getErrorView(HttpStatusCode.Unauthorized);

            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Level,Name")] File file)
        {
            if (ModelState.IsValid)
            {
                File acutalFile = db.Files.Find(file.ID);
                if (acutalFile == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                var EmpRole = getPrimaryRole();
                if (EmpRole == null)
                {
                    return getErrorView(HttpStatusCode.Unauthorized);
                }

                if (hasFileLevelPermission(EmpRole.Role.ID, acutalFile.Level, FilePermissions.EDIT_FILE) || isFileAuthor(acutalFile.ID, EmpRole.ID))
                {

                    db.Files.Find(acutalFile.ID).Name = file.Name;
                    db.Files.Find(acutalFile.ID).Level = file.Level;
                    db.SaveChanges();
                    return RedirectToAction("View", new { id = acutalFile.ID });

                }
                return getErrorView(HttpStatusCode.Unauthorized);
            }
            ViewBag.Level = new SelectList(db.FileLevels, "Level", "LevelDesc", file.Level);
            return View(file);
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
            if (hasFileLevelPermission(EmpRole.Role.ID, file.Level, FilePermissions.DELETE_FILE) || isFileAuthor(file.ID, EmpRole.ID))
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

                if (isFileAuthor(acutalFile.ID, EmpRole.ID))
                {

                    
                    db.Files.Remove(acutalFile);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");

                }
                return getErrorView(HttpStatusCode.Unauthorized);
            
           
        }


        public ActionResult FileLevels()
        {
            var emp = getEmployeeRef();
            if (emp == null)
            {
                signOut();
                return null;
            }
            ViewFileLevelsModel viewModel = new ViewFileLevelsModel();
            List<String> levels = db.FileLevels.Select(x => x.Level).ToList<String>();
            Dictionary<String, Dictionary<String, List<String>>> DICT = new Dictionary<string, Dictionary<string, List<string>>>();
            foreach (var level in levels)
            {
                List<Role> roles = db.Roles.OrderBy(x=>x.PriorityOrder).ToList<Role>();

                Dictionary<String, List<String>> rolesAndPermissions = new Dictionary<string, List<string>>();

                foreach (var role in roles)
                {
                    List<String> permissions = db.FilesScopes.Where(x => x.RoleID == role.ID).Select(x => x.FilePermission.ArabicName).ToList<String>();
                    rolesAndPermissions.Add(role.ArabicName, permissions);

                }
                DICT.Add(level, rolesAndPermissions);

            }
            viewModel.LevelsDictionary = DICT;
            return View(viewModel);
            
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
            if (hasFileLevelPermission(EmpRole.Role.ID, fileVersion.File.Level, FilePermissions.VIEW_FILE) || isFileAuthor((int)fileVersion.FileID, EmpRole.ID))
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
