using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BugTracker.Models
{
    public class UserRolesHelper
    {
        private UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        public bool IsUserInRole(string userId, string roleName)
        {
            return manager.IsInRole(userId, roleName);
        }

        public ICollection<string> ListUserRoles(string userId)
        {
            return manager.GetRoles(userId);
        }

        public bool AddUserToRole(string userId, string roleName)
        {
            var result = manager.AddToRole(userId, roleName);
            return result.Succeeded;
        }
        public bool RemoveUserFromRole(string userId,string roleName)
        {
            var result = manager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }

        public ICollection<ApplicationUser> UsersInRole(string roleName)
        {
            var resultList = new List<ApplicationUser>();
            var List = manager.Users.ToList();
            foreach (var user in List)
            {
                if (IsUserInRole(user.Id, roleName))
                    resultList.Add(user);
            }
            return resultList;
        }

        public ICollection<ApplicationUser> UsersNotInRole(string roleName)
        {
            var resultList = new List<ApplicationUser>();
            var List = manager.Users.ToList();
            foreach (var user in List)
            {
                if (!IsUserInRole(user.Id, roleName))
                    resultList.Add(user);
            }
            return resultList;
        }

    }
}