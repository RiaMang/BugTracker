using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class Ticket
    {
        public Ticket()
        {
            this.TicketComments = new HashSet<TicketComment>();
            this.TicketAttachments = new HashSet<TicketAttachment>();
            this.TicketHistories = new HashSet<TicketHistory>();
        }
        
        public int Id {get; set;}
        [Required]
        public string Title { get; set; }
        [Required]
        [AllowHtml]
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }

        public int ProjectId { get; set; }
        public int TicketStatusId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketTypeId { get; set; }
        public string OwnerUserId { get; set; }
        public string AssignedToUserId { get; set; }

        public virtual Project Project { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual ApplicationUser OwnerUser { get; set; }
        public virtual ApplicationUser AssignedToUser { get; set; }

        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }
       


    }
}