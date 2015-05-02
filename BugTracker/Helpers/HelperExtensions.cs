using BugTracker.Models;
using Microsoft.AspNet.Identity;
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

    }
}