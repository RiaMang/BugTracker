using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using System.Data.Entity;
using System.IO;


namespace BugTracker.Controllers
{
    [Authorize]
    public class CommAttachController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: CommEdit
        public ActionResult EditComm(int ? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment comment = db.TicketComments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult EditComm(TicketComment tcom)
        {
            if(ModelState.IsValid)
            {
                               
                db.Entry(tcom).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = tcom.TicketId });
            }
            return View(tcom);
        }

        // GET: CommDel
        public ActionResult DelComm(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment comment = db.TicketComments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult DelComm(int id)
        {
            var tcom = db.TicketComments.Find(id);    
            db.TicketComments.Remove(tcom);
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = tcom.TicketId });
            
            
        }

        // GET: CommEdit
        public ActionResult EditAttach(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment attach = db.TicketAttachments.Find(id);
            if (attach == null)
            {
                return HttpNotFound();
            }
            return View(attach);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult EditAttach([Bind(Include="Id, TicketId,UserId, Created, Description")]TicketAttachment tatt, HttpPostedFileBase attach)
        {
            if (attach != null && attach.ContentLength > 0)
            {
                //check the file name to make sure its an image
                var ext = Path.GetExtension(attach.FileName);
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != "gif" && ext != "bmp")
                    ModelState.AddModelError("attach", "Invalid Format."); // throw an error
            }

            if (ModelState.IsValid)
            {
                db.TicketAttachments.Attach(tatt);

                if (attach != null)
                {
                    //relative server path
                    var filePath = "/Uploads/";
                    // path on physical drive on server
                    var absPath = Server.MapPath("~" + filePath);
                    // media url for relative path
                    tatt.FileUrl = filePath + attach.FileName;
                    //save image
                    tatt.FilePath = Path.Combine(absPath, attach.FileName);
                    attach.SaveAs(tatt.FilePath);
                    db.Entry(tatt).Property("FilePath").IsModified = true;
                    db.Entry(tatt).Property("FileUrl").IsModified = true;
                }
                db.Entry(tatt).Property("Created").IsModified = true;
                db.Entry(tatt).Property("Description").IsModified = true;
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = tatt.TicketId });
            }
            return View(tatt);
        }


        // GET: Attachment Del
        public ActionResult DelAttach(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment attach = db.TicketAttachments.Find(id);
            if (attach == null)
            {
                return HttpNotFound();
            }
            return View(attach);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult DelAttach(int id)
        {
            var tatt = db.TicketAttachments.Find(id);
            db.TicketAttachments.Remove(tatt);
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = tatt.TicketId });


        }
    }
}