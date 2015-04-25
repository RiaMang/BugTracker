using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace BugTracker.Helpers
{
    public class ProjectsHelper
    {

        ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserInProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var flag = project.Users.Any(u => u.Id == userId);
            return (flag);
        }

        public IList<Project> ListUserProjects(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);

            var projects = user.Projects.ToList();
            return (projects);
        }

        public bool AddUserToProject(string userId, int projectId)
        {
            try
            {
                Project proj = db.Projects.Find(projectId);
                var newUser = new ApplicationUser { Id = userId, UserName="temp" };
                db.Users.Attach(newUser);
                db.Entry(newUser).State = System.Data.Entity.EntityState.Unchanged;
                proj.Users.Add(newUser);
                
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                
                return false;
            }
            catch(Exception e)
            {
                return false;
            }
            
            return (true);
        }

        public bool RemoveUserFromProject(string userId, int projectId)
        {
            Project proj = db.Projects.Find(projectId);
            var delUser = db.Users.Find(userId);
           
            //db.Users.Attach(delUser);
            
            proj.Users.Remove(delUser);
            db.SaveChanges();
            return (true);
        }

        public IList<ApplicationUser> UsersInProject(int projectId)
        {
            var project = db.Projects.Find(projectId);
            IList<ApplicationUser> resultList = project.Users.ToList();
            return (resultList);
        }

        public IList<ApplicationUser> UsersNotInProject(int projectId)
        {
            var project = db.Projects.Find(projectId);
            IList<ApplicationUser> inList = project.Users.ToList();
            IList<ApplicationUser> resultList = (IList<ApplicationUser>)db.Users.Except(inList);
            return (resultList);
        }
    }
}