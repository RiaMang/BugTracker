using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;

using Microsoft.AspNet.Identity;
using BugTracker.Helpers;

namespace BugTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectsHelper ph = new ProjectsHelper();
        private UserRolesHelper urHelper = new UserRolesHelper();

        // GET: Projects
        [Authorize (Roles="Admin,Project Manager,Developer")]
        public ActionResult Index()
        {
            
            if(User.IsInRole("Admin"))
            {
                return View(db.Projects.ToList());
            } 
            else if(User.IsInRole("Project Manager") || User.IsInRole("Developer"))
            {
                
                return View( db.Users.Find(User.Identity.GetUserId()).Projects.ToList() );
            }
            return View(db.Projects.ToList());
        }

        // GET: Projects/Details/5
        [Authorize(Roles = "Admin,Project Manager,Developer")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            var ticsIssued = project.Tickets.Count();
            var ticsResolved = project.Tickets.Where(t => t.TicketStatus.Name == "Resolved").Count();
            ProjUsersVM projUsersVM = new ProjUsersVM();
            projUsersVM.ProjectId = project.Id;
            projUsersVM.ProjectName = project.Name;
            projUsersVM.Users = project.Users.ToList();
            projUsersVM.ticsIssued = ticsIssued;
            projUsersVM.ticsResolved = ticsResolved;

            return View(projUsersVM);
        }

        // GET: List Tickets
        [Authorize(Roles = "Admin,Project Manager,Developer")]
        public ActionResult ListTickets(int? projectId, string ownerId, string assignedId)
        {

            if (projectId == null && ownerId == null && assignedId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(projectId != null)
            {

                Project project = db.Projects.Find(projectId);

                if (project == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Title = "Project - "+project.Name;
                return View(project.Tickets.OrderByDescending(t => t.Created).ToList());
            }
            if (ownerId != null)
            {

                ApplicationUser user = db.Users.Find(ownerId);

                if (user == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Title = "Owner - "+user.DisplayName;
                return View(db.Tickets.Where(t=>t.OwnerUserId == user.Id).OrderByDescending(t => t.Created).ToList());
            }
            if (assignedId != null)
            {

                ApplicationUser user = db.Users.Find(assignedId);

                if (user == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Title = "Assigned to - "+user.DisplayName;
                return View(db.Tickets.Where(t => t.AssignedToUserId == user.Id).OrderByDescending(t => t.Created).ToList());
            }
            return RedirectToAction("Index", "Tickets");
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin,Project Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }


        [Authorize (Roles="Admin,Project Manager")]
        public ActionResult AssignUsers(int projectId)
        {
            var proj = db.Projects.Find(projectId);
             //db.Users.ToList();
            var userList = new List<ApplicationUser>();
            userList = (List<ApplicationUser>)urHelper.UsersInRole("Developer");
            if (User.IsInRole("Admin"))
            {
                //userList = db.Users.ToList();
                var pmList = urHelper.UsersInRole("Project Manager");
                foreach (var pm in pmList)
                {
                    if (!userList.Contains(pm))
                    {
                        userList.Add(pm);
                    }
                }
                var adList = urHelper.UsersInRole("Admin");
                foreach (var ad in adList)
                {
                    if (!userList.Contains(ad))
                    {
                        userList.Add(ad);
                    }
                }
            }
                        
            var selected = ph.UsersOnProject(projectId).Select(n => n.Id).ToArray();
            var selectList = new MultiSelectList(userList, "Id", "DisplayName", selected);
            var model = new ProjUserViewModel
            {
               Project = proj,
                Users = selectList
                
            };

            return View(model);
            
        }

        [Authorize(Roles = "Admin,Project Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignUsers(ProjUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                    var proj = db.Projects.Find(model.Project.Id);
                    var userList = new List<ApplicationUser>();
                    if (User.IsInRole("Project Manager"))
                        userList = urHelper.UsersInRole("Developer").ToList();
                    else
                        userList = urHelper.UsersNotInRole("Submitter").ToList();
                    string [] sel = {};
                    var selected = model.SelectedUsers != null ? model.SelectedUsers : sel;
                    foreach (var user in userList)
                    {
                        if (selected.Contains(user.Id))
                        {
                            ph.AddUserToProject(user.Id, proj.Id);
                        }
                        else
                        { 
                            ph.RemoveUserFromProject(user.Id, proj.Id);
                        }
                    }
                
                return RedirectToAction("Index");
            }
            return RedirectToAction("AssignUsers",model.Project.Id);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
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
