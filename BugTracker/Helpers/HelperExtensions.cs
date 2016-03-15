using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace BugTracker.Helpers
{
    /// <summary>
    /// Love the comments
    /// 
    /// 
    /// 
    /// 
    /// 
    /// </summary>
    public static class HelperExtensions
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static string GetName(this IIdentity user)
        {
            var ClaimsUser = (ClaimsIdentity)user;
            var claim = ClaimsUser.Claims.FirstOrDefault(c => c.Type == "Name");
            if(claim != null)
            {
                return claim.Value;
            }
            else
            {
                return null;
            }
        }

        public static ICollection<Ticket> ListTicketsForUser(this ApplicationUser user)
        {
            var projects = user.Projects.ToList();
            var tickets = new List<Ticket>();
            foreach (var p in projects)
            {
                tickets.AddRange(p.Tickets);
            }
            return (tickets);
        }

        public static bool IsUserInRole(this ApplicationUser user, string roleName)
        {
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            return manager.IsInRole(user.Id, roleName);
        }

        public static void SendNotification(this ApplicationUser user, Notification note)
        {
            Ticket ticket = db.Tickets.Find(note.TicketId);
            EmailService es = new EmailService();
            IdentityMessage message = new IdentityMessage
            {
                Destination = user.Email,
                Subject = ticket.Title,
                Body = note.Change+" : "+note.Details,
            };

            es.SendAsync(message);
        }

        public static bool Update<T>(this ApplicationDbContext context, T item, params string[] changedPropertyNames) where T : class, new()
        {
            context.Set<T>().Attach(item);
            foreach (var propertyName in changedPropertyNames)
            {
                // If we can't find the property, this line will throw an exception, 
                //which is good as we want to know about it
                context.Entry(item).Property(propertyName).IsModified = true;
            }
            return true;
        }

    }
}