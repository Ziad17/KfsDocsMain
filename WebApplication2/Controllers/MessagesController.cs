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
using WebApplication2.Models.ViewModels.Messages;

namespace WebApplication2.Controllers
{
    public class MessagesController : BaseController
    {


        public ActionResult Sent()
        {

            var myEmp = getEmployeeRef();

            var messages = db.Messages.Where(x => x.DeletedFromSender == false && x.SenderID == myEmp.ID).Include(x => x.Employee1).Include(x => x.Employee);

            InboxModel viewModel = new InboxModel()
            { MyMessages = messages.ToList<Message>() };
            return View(viewModel);
        }

        public ActionResult Inbox()
        {

            var myEmp = getEmployeeRef();

            var messages = db.Messages.Where(x => x.DeletedFromReciever == false && x.RecieverID == myEmp.ID).Include(x => x.Employee1).Include(x => x.Employee);

            InboxModel viewModel = new InboxModel()
            { MyMessages = messages.ToList<Message>() };
            return View(viewModel);
        }

        public ActionResult View(int? id)
        {
            var myEmp = getEmployeeRef();
            if (id == null)
            {
                return getErrorView(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null || !(message.RecieverID == myEmp.ID || message.SenderID == myEmp.ID))
            {
                
                return getErrorView(HttpStatusCode.NotFound);
            }
            ViewMessageModel viewModel = new ViewMessageModel()
            {
                ID = message.ID,
                SenderID = message.SenderID,
                RecieverID = message.RecieverID,
                Sender = message.Employee1,
                Reciever = message.Employee,
                Header = message.HeaderText,
                DateCreated = message.DateCreated,
                Text = message.Text,
                TypeName= message.FileTypeName,
                Size=message.FileContent.FileSize

            };
            if (message.SenderID == myEmp.ID)
            { viewModel.Sent = true; }
            else { db.Messages.Find(message.ID).Seen = true;
                db.SaveChanges();
            }

            return View(viewModel);
        }

        public ActionResult Download(int? id)//message ID
        {
            if (id == null)
            {
                return getErrorView(HttpStatusCode.BadRequest);

            }
            var myEmp = getEmployeeRef();
            Message message = db.Messages.Find(id);
            if (message == null || !(message.RecieverID == myEmp.ID || message.SenderID == myEmp.ID))
            {
                return getErrorView(HttpStatusCode.NotFound);
            }
            FileManager fileMG = new FileManager();
            return File(fileMG.getFileStream(message.FileContent.Href), "applicatin/force-download", message.HeaderText + "." + message.FileType.Extension);



        }


        public ActionResult Send(int? id)
        {
            if (id == null)
            {
                return getErrorView(HttpStatusCode.BadRequest);
            }
            var myEmp = getEmployeeRef();
            if (myEmp == null)
            {
                signOut();
                return null;
            }
            if (myEmp.ID == id)
            {
                RedirectToAction("MyProfile");
            }
            var emp = db.Employees.Find(id);
            if (emp == null)
            {
                return getErrorView(HttpStatusCode.NotFound);
            }
            SendMessageModel viewModel = new SendMessageModel()
            {
                Sender = myEmp,
                SenderID = myEmp.ID,
                RecieverID = emp.ID,
                Reciever = emp
                
            };
            FileManager fileMG = new FileManager();
            ViewBag.Img = fileMG.getImageStream(emp.ImageURL);
            return View(viewModel);


        }

        [HttpPost, ActionName("Send")]
        [ValidateAntiForgeryToken]
        public ActionResult Send(SendMessageModel model)
        {


            if (model.SenderID == 0 || model.RecieverID == 0)
            {
                return getErrorView(HttpStatusCode.BadRequest);
            }

            Message message = new Message()
            {
                SenderID = model.SenderID,
                RecieverID = model.RecieverID,
                HeaderText = model.Header,
                Text = model.Text


            };


            var myEmp = getEmployeeRef();

            if (myEmp == null)
            {
                signOut();
                return null;
            }
            if (model.SenderID != myEmp.ID)
            {
                return getErrorView(HttpStatusCode.BadRequest);


            }

            if (myEmp.ID == model.RecieverID)
            {
                RedirectToAction("MyProfile");
            }
            var emp = db.Employees.Find(model.RecieverID);
            if (emp == null)
            {
                return getErrorView(HttpStatusCode.NotFound);
            }

            HttpPostedFileBase file_content = model.File;


            if (ModelState.IsValid)
            {

                try
                {
                    var FileSizeInKb = 1024;
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


                            message.Seen = false;
                            message.DeletedFromSender = false;
                            message.DeletedFromReciever = false;

                            message.DateCreated = DateTime.Now;
                            message.FileContentID = fileContent.ID;
                            message.FileTypeName = db.FileTypes.Where(x => x.Extension.ToLower().Equals(fileExt)).FirstOrDefault().Name;

                            db.Messages.Add(message);


                            db.SaveChanges();
                            //     return RedirectToAction("Index", new { id = file.ID });

                            return RedirectToAction("Index", "Home");


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
            ModelState.AddModelError("", "Input Error");

            return View(model);





        }




        [HttpPost, ActionName("DeleteForAll")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForAll(int id)
        {
            var myEmp = getEmployeeRef();


            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return getErrorView(HttpStatusCode.NotFound);

            }

            if (!(Math.Abs(message.DateCreated.Subtract(DateTime.Now).TotalMinutes) < 15))
            {
            return getErrorView(HttpStatusCode.Unauthorized);

            }

            if (message.SenderID == myEmp.ID)
            {
                db.Messages.Find(message.ID).DeletedFromSender = true;
                db.Messages.Find(message.ID).DeletedFromReciever = true;

                db.SaveChanges();
                return RedirectToAction("Sent");


            }
         
            return getErrorView(HttpStatusCode.Unauthorized);


        }





        [HttpPost, ActionName("DeleteForEither")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForEither(int id)
        {

            var myEmp = getEmployeeRef();




            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return getErrorView(HttpStatusCode.NotFound);

            }

            if (message.SenderID == myEmp.ID)
            {
                db.Messages.Find(message.ID).DeletedFromSender = true;
                db.SaveChanges();
                return RedirectToAction("Sent");


            }
            else if (message.RecieverID == myEmp.ID)
            {
                db.Messages.Find(message.ID).DeletedFromReciever = true;
                db.SaveChanges();
                return RedirectToAction("Inbox");

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
