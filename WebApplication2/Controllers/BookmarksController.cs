using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class BookmarksController : BaseController
    {


        public ActionResult Index()
        {
            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }
            var bookmarks = db.Bookmarks.Where(x=>x.EmployeeID==EmpRole.ID).Include(b => b.EmployeeRole).Include(b => b.File);
            return View(bookmarks.ToList());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Change(int  id)//file id
        {


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

            var bookmark = db.Bookmarks.Where(x => x.FileID == id && x.EmployeeID == EmpRole.ID).FirstOrDefault();
            if (bookmark != null)
            {
                db.Bookmarks.Remove(bookmark);
                db.SaveChanges();
                return RedirectToAction("View", "Files", new { id = id });
            }

            Bookmark bookmarkToAdd = new Bookmark() {
                EmployeeID = EmpRole.ID,
                FileID = id,
                DateCreated = DateTime.Now
            };
            db.Bookmarks.Add(bookmarkToAdd);
                db.SaveChanges();
            return RedirectToAction("View", "Files", new { id = id });



        }


        public ActionResult Delete(int id) //bookmark id
        {

            var EmpRole = getPrimaryRole();
            if (EmpRole == null)
            {
                return getErrorView(HttpStatusCode.Unauthorized);
            }

            Bookmark bookmark = db.Bookmarks.Where(x=>x.ID==id && x.EmployeeID==EmpRole.ID).FirstOrDefault();
            if (bookmark == null)
            {
                return getErrorView(HttpStatusCode.NotFound);

            }
            db.Bookmarks.Remove(bookmark);
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
