using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Managers;

namespace WebApplication2.Controllers
{
    [RequireHttps]
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {



            return View();
        }

        [HttpPost]
        public ActionResult UploadFiles(HttpPostedFileBase fileE)
        {
            string msg = "";

            if (ModelState.IsValid)
            {
                try
                {
                    if (fileE != null)
                    {
                        string ln;
                        string path = Path.Combine(Server.MapPath("~/UploadedFiles"), Path.GetFileName(fileE.FileName));
                        StreamReader st = new StreamReader(fileE.InputStream);
                        FileManager filMG = new FileManager();
                        if (!filMG.uploadFile(fileE.FileName, st.BaseStream))
                        {
                            ViewBag.FileStatus = "Error while file uploading.--" + fileE.FileName + "--" + st;

                        }
                        else {
                            ViewBag.FileStatus = "file Uploaded Successfully.--" + fileE.FileName + "--" + st;

                        }

                    }
                    else
                    {
                        ViewBag.FileStatus = "File Null successfully..--" + fileE.FileName;
                    }
                }
                catch (Exception e)
                {

                    
                    ViewBag.FileStatus = "Error while file uploading.--"+e.ToString();
                }

            }
            return View("Index");
        }

    }
}