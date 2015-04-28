using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Helpers
{
    public static class HelperExtensions
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
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



    }
}