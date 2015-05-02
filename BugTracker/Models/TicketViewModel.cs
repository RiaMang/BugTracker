using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketViewModel
    {
        public TicketViewModel() { }

        public TicketViewModel(Ticket t)
        {
            Project = t.Project.Name;
            TicketStatus = t.TicketStatus.Name;
            TicketPriority = t.TicketPriority.Name;
            TicketType = t.TicketType.Name;
            OwnerUser = t.OwnerUser.DisplayName;
            AssignedToUser = t.AssignedToUser.DisplayName;
            Title = t.Title;
            Created = t.Created;
            Updated = t.Updated;
            Id = t.Id;
            Description = t.Description;
            link = " ";

        }
        
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }

        public string Project { get; set; }
        public string TicketStatus { get; set; }
        public string TicketPriority { get; set; }
        public string TicketType { get; set; }
        public string OwnerUser { get; set; }
        public string AssignedToUser { get; set; }
        public string link { get; set; }
    }
}