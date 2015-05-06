using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            return View();
            
        }
        [Authorize(Roles="Developer")]
        public ActionResult Notifications()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string userid = User.Identity.GetUserId();
            var notelist = db.Notifications.Where(n=>n.UserId == userid).ToList();
            
            return View(notelist);

        }
    }
}