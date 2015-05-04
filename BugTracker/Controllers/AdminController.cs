using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Net;

namespace BugTracker.Controllers
{
    [Authorize (Roles="Admin")]
    public class AdminController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: Admin
        public ActionResult Users()
        {
            return View(db.Users.ToList());
        }

        // GET: List Tickets
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult ListTickets(int? projectId)
        {

            if (projectId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(projectId);

            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = project.Name;
            return View(project.Tickets.OrderByDescending(t => t.Created).ToList());
        }
        
        public ActionResult EditUser(string Id)
        {
            var user = db.Users.Find(Id);
            var roleList = db.Roles.Select(r=> new UserRoleViewModel { Name = r.Name, UserId= Id, IsInRole =  r.Users.Any(u=>u.UserId==Id)});
            var selected = roleList.Where(r => r.IsInRole).Select(n => n.Name).ToArray();
            var selectList = new MultiSelectList(roleList, "Name",  "Name", selected);
            var model=new AdminUserViewModel{
                User = user,
                Roles = selectList,
                SelectedRoles = selected
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult EditUser(AdminUserViewModel model)
        {
            var user = db.Users.Find(model.User.Id);
            var um = Request.GetOwinContext().Get<ApplicationUserManager>();

            foreach(var role in db.Roles.ToList())
            {
                if (model.SelectedRoles.Contains(role.Name))
                    um.AddToRole(user.Id, role.Name);
                else
                    um.RemoveFromRole(user.Id, role.Name);
            }
            //return RedirectToAction("EditUser", new { Id = model.User.Id });
            return RedirectToAction("DetailsUserRoles", new { Id = model.User.Id });
        }

        public ActionResult DetailsUserRoles(string Id)
        {
            var user = db.Users.Find(Id);
            UserRolesHelper helper = new UserRolesHelper();
            var urvm = new UserRolesVM();
            urvm.UserId = user.Id;
            urvm.UserName = user.DisplayName;
            urvm.Roles = helper.ListUserRoles(user.Id);
            return View(urvm);


        }

    }
}