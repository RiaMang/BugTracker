using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PagedList;
using PagedList.Mvc;
using BugTracker.Helpers;
using System.IO;
using DataTables.Mvc;

namespace BugTracker.Models
{
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        [Authorize]
        public ActionResult Index()
        {
            return View();
            //var tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);

            //string userId = User.Identity.GetUserId();
            //var user = db.Users.Find(userId); //check navigational properties - find looks in memory first for user if not, takes from database
            //var tickets = new List<Ticket>();
            //if (role == "dev")
            //{
            //    ViewBag.Dev = "dev";
            //    tickets = db.Tickets.Where(t => t.AssignedToUserId == userId).ToList();
            //}
            
            //else if(User.IsInRole("Admin"))
            //{
            //    tickets = db.Tickets.ToList();
            //}
            //else if(User.IsInRole("Project Manager") || User.IsInRole("Developer"))
            //{
            //    //tickets=(List<Ticket>)user.ListTicketsForUser().Where(u=>u.Id == u.Id);
            //    tickets = user.Projects.SelectMany(p => p.Tickets).ToList();
                
            //} 
            //else if (User.IsInRole("Submitter"))
            //{
            //    tickets = db.Tickets.Where(t => t.OwnerUserId == userId).ToList();
            //}
            ////db.Tickets.Include(t => t.AssignedToUser)

            //return View(tickets.OrderByDescending(p => p.Created).ToList());

            ////return View(tickets.ToList().OrderByDescending(p => p.Created).ToPagedList(page ?? 1, 10));
           
        }


        public JsonResult GetTickets([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest request, bool myTickets, 
            DateTimeOffset? date, string type, string priority, string status)
        {
            string userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId); //check navigational properties - find looks in memory first for user if not, takes from database
            IQueryable<Ticket> tickets;
            tickets = db.Tickets.Where(t => t.OwnerUserId == userId);

            if (User.IsInRole("Developer") && myTickets == true)
            {
                ViewBag.Dev = "dev";
                tickets = db.Tickets.Where(t => t.AssignedToUserId == userId);
            } 
            else if (User.IsInRole("Admin"))
            {
                tickets = db.Tickets;
            }
            else if (User.IsInRole("Project Manager") || User.IsInRole("Developer"))
            {
                //tickets=(List<Ticket>)user.ListTicketsForUser().Where(u=>u.Id == u.Id);
                tickets = user.Projects.SelectMany(p => p.Tickets).AsQueryable();

            }
            else if (User.IsInRole("Submitter"))
            {
                tickets = db.Tickets.Where(t => t.OwnerUserId == userId);
            }
            //db.Tickets.Include(t => t.AssignedToUser)

            if(date != null)
            {
                tickets = tickets.Where(t => t.Created > date);
            }
            if(type!=null && type!="All")
            {
                tickets = tickets.Where(t => t.TicketType.Name == type);
            }
            if (priority != null && priority != "All")
            {
                tickets = tickets.Where(t => t.TicketPriority.Name == priority);
            }
            if (status != null && status != "All")
            {
                tickets = tickets.Where(t => t.TicketStatus.Name == status);
            }

            var totalCount = tickets.Count();
            var search = request.Search.Value;


            if(!string.IsNullOrWhiteSpace(search))
            {
                tickets = tickets.Where(t => t.Title.Contains(search) || t.Description.Contains(search) 
                    || ( t.AssignedToUserId != "" && t.AssignedToUserId != null && t.AssignedToUser.DisplayName.Contains(search)) 
                    || t.Project.Name.Contains(search)
                    || t.OwnerUser.DisplayName.Contains(search) || t.TicketStatus.Name.Contains(search)
                    || t.TicketPriority.Name.Contains(search) || t.TicketType.Name.Contains(search));

            }

            
            tickets = tickets.OrderByDescending(t => t.Created);

            var column = request.Columns.FirstOrDefault(r=>r.IsOrdered == true);
            if(column != null)
            {
                if(column.SortDirection == Column.OrderDirection.Descendant)
                {
                    switch (column.Data)
                    {
                        case "Title":
                            tickets = tickets.OrderByDescending(t => t.Title);
                            break;
                        case "Description":
                            tickets = tickets.OrderByDescending(t => t.Description);
                            break;
                        case "Created":
                            tickets = tickets.OrderByDescending(t => t.Created);
                            break;
                        case "Updated":
                            tickets = tickets.OrderByDescending(t => t.Updated);
                            break;
                        case "Project":
                            tickets = tickets.OrderByDescending(t => t.Project.Name);
                            break;
                        case "AssignedUser":
                            tickets = tickets.OrderByDescending(t => t.AssignedToUser.DisplayName);
                            break;
                        case "OwnerUser":
                            tickets = tickets.OrderByDescending(t => t.OwnerUser.DisplayName);
                            break;
                        case "TicketStatus":
                            tickets = tickets.OrderByDescending(t => t.TicketStatus.Name);
                            break;
                        case "TicketType":
                            tickets = tickets.OrderByDescending(t => t.TicketType.Name);
                            break;
                        case "TicketPriority":
                            tickets = tickets.OrderByDescending(t => t.TicketPriority.Name);
                            break;

                    }
                }
                else
                {
                    switch (column.Data)
                    {
                        case "Title":
                            tickets = tickets.OrderBy(t => t.Title);
                            break;
                        case "Description":
                            tickets = tickets.OrderBy(t => t.Description);
                            break;
                        case "Created":
                            tickets = tickets.OrderBy(t => t.Created);
                            break;
                        case "Updated":
                            tickets = tickets.OrderBy(t => t.Updated);
                            break;
                        case "Project":
                            tickets = tickets.OrderBy(t => t.Project.Name);
                            break;
                        case "AssignedUser":
                            tickets = tickets.OrderBy(t => t.AssignedToUser.DisplayName);
                            break;
                        case "OwnerUser":
                            tickets = tickets.OrderBy(t => t.OwnerUser.DisplayName);
                            break;
                        case "TicketStatus":
                            tickets = tickets.OrderBy(t => t.TicketStatus.Name);
                            break;
                        case "TicketType":
                            tickets = tickets.OrderBy(t => t.TicketType.Name);
                            break;
                        case "TicketPriority":
                            tickets = tickets.OrderBy(t => t.TicketPriority.Name);
                            break;
                    }
                }
            }

            var uug = tickets.Skip(request.Start).Take(request.Length);
            var paged = uug.Select(t => new TicketViewModel { 
                Project = t.Project.Name,
                TicketStatus = "<span style=\"color:green;\">"+t.TicketStatus.Name+"</span>",
                TicketPriority = t.TicketPriority.Name,
                TicketType = t.TicketType.Name,
                OwnerUser = t.OwnerUser.DisplayName,
                AssignedToUser = t.AssignedToUser == null ? "" : "<span style=\"color:green;\">"+t.AssignedToUser.DisplayName+"</span>",
                Title = "<span style=\"color:green;\">" + t.Title + "</span>",
                Created = t.Created,
                Updated = t.Updated,
                Id = t.Id,
                Description = t.Description,
                link = "<a href=\"/Tickets/Details/"+t.Id+"\">Details</a>",

            });
            return Json(new DataTablesResponse(request.Draw, paged, tickets.Count(), totalCount), JsonRequestBehavior.AllowGet);
        }

        // GET: Tickets/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            TempData["tic"] = ticket;
            UserRolesHelper urh = new UserRolesHelper();
            ViewBag.AssignedToUserId = new SelectList(urh.UsersInRole("Developer"), "Id", "DisplayName", ticket.AssignedToUserId);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }
        
        //Post Create Comments
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateComment([Bind(Include="TicketId, UserId,Comment")]TicketComment tcom)
        {
            if(ModelState.IsValid)
            {
                var now = System.DateTimeOffset.Now;
                Ticket ticket = db.Tickets.Find(tcom.TicketId);
                if(ticket.AssignedToUserId != null)
                { 
                ApplicationUser user = db.Users.Find(ticket.AssignedToUserId);
                Notification note = new Notification
                {
                    TicketId = tcom.TicketId,
                    UserId = user.Id,
                    Change = "Comment Added",
                    Details = tcom.Comment,
                    DateNotified = now,
                };
                db.Notifications.Add(note);
                //user.SendNotification(note);
                }
            tcom.Created = now;
            db.TicketComments.Add(tcom);
            db.SaveChanges();
            }
            return RedirectToAction("Details", new { id=tcom.TicketId });

        }

        //Post Create Comments
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAttach([Bind(Include = "TicketId, UserId,Description")]TicketAttachment tatt, HttpPostedFileBase attach)
        {
            if (attach != null && attach.ContentLength > 0)
            {
                //check the file name to make sure its an image
                var ext = Path.GetExtension(attach.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != "gif" && ext != "bmp" && ext !="pdf" && ext != "txt")
                    ModelState.AddModelError("attach", "Invalid Format."); // throw an error
            }
            if (ModelState.IsValid)
            {
                if (attach != null)
                {
                    //relative server path
                    var filePath = "/Uploads/";
                    // path on physical drive on server
                    var absPath = Server.MapPath("~" + filePath);
                    // media url for relative path
                    tatt.FileUrl = filePath + attach.FileName;
                    //save image
                    tatt.FilePath = Path.Combine(absPath, attach.FileName);
                    attach.SaveAs(tatt.FilePath);
                }
                var now = System.DateTimeOffset.Now;
                Ticket ticket = db.Tickets.Find(tatt.TicketId);
                if(ticket.AssignedToUserId != null)
                { 
                ApplicationUser user = db.Users.Find(ticket.AssignedToUserId);
                Notification note = new Notification
                {
                    TicketId = tatt.TicketId,
                    UserId = user.Id,
                    Change = "Attachment Added",
                    Details = tatt.Description,
                    DateNotified = now,
                };
                db.Notifications.Add(note);
                //user.SendNotification(note);
                }
                
                tatt.Created = now;
                db.TicketAttachments.Add(tatt);
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = tatt.TicketId });

        }
        // GET: Tickets/Create
        [Authorize]
        public ActionResult Create()
        {
            UserRolesHelper urh = new UserRolesHelper();
            ViewBag.AssignedToUserId = new SelectList(urh.UsersInRole("Developer"), "Id", "FirstName");
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.Created = System.DateTimeOffset.Now;
                ticket.TicketStatusId = db.TicketStatuses.Single(t=>t.Name == "Open").Id;
                ticket.OwnerUserId = User.Identity.GetUserId();
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            UserRolesHelper urh = new UserRolesHelper();
            ViewBag.AssignedToUserId = new SelectList(urh.UsersInRole("Developer"), "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        //get Assign Ticket
        public ActionResult AssignTicket(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            
            if (ticket == null)
            {
                return HttpNotFound();
            }
            //TempData["tic"] = ticket;
            UserRolesHelper urh = new UserRolesHelper();
            ViewBag.AssignedToUserId = new SelectList(urh.UsersInRole("Developer").Where(u=>u.Projects.Any(p=>p.Id == ticket.ProjectId)), "Id", "DisplayName", ticket.AssignedToUserId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            //TempData["tic"] = ticket;
            if (ticket == null)
            {
                return HttpNotFound();
            }
            UserRolesHelper urh = new UserRolesHelper();
            ViewBag.AssignedToUserId = new SelectList(urh.UsersInRole("Developer").Where(u => u.Projects.Any(p => p.Id == ticket.ProjectId)), "Id", "DisplayName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            //Session["tic"] = ticket;
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,ProjectId,TicketStatusId,TicketPriorityId,TicketTypeId,OwnerUserId,AssignedToUserId")] Ticket ticket)
        {
            //if (ModelState.IsValid)
            //{
            //    //Ticket oldTic = db.Tickets.Find(ticket.Id);
            //    //Ticket oldTic = (Ticket)TempData["tic"];

            //     var oldTic = (from t in db.Tickets.AsNoTracking()
            //      where t.Id == ticket.Id
            //      select t).FirstOrDefault(); // same as

            //     // var oldTic = db.Tickets.AsNoTracking().FirstOrDefault(t=>t.t.Id == ticket.Id);


            //    string userid = User.Identity.GetUserId();
            //    var changed = System.DateTimeOffset.Now;


            //    var editId = Guid.NewGuid().ToString(); // store in db as string and then group by - will return list of lists
            //    // key value pairs - key is edit id and value is the list.

            //    if (ticket.AssignedToUserId != null && ticket.TicketStatusId == 1)
            //    {
            //        ticket.TicketStatusId = 2;

            //    }
            //    if(oldTic.Title != ticket.Title)
            //    {
            //        TicketHistory th1 = new TicketHistory
            //        {
            //            TicketId = ticket.Id,
            //            Property = "Title",
            //            OldValue = oldTic.Title,
            //            NewValue = ticket.Title,
            //            EditId = editId,
            //            Changed = changed,
            //            UserId = userid
            //        };
            //        db.TicketHistories.Add(th1);
            //    }
            //    if (oldTic.Description != ticket.Description)
            //    {
            //        TicketHistory th2 = new TicketHistory
            //        {
            //            TicketId = ticket.Id,
            //            Property = "Description",
            //            OldValue = oldTic.Description,
            //            NewValue = ticket.Description,
            //            EditId = editId,
            //            Changed = changed,
            //            UserId = userid
            //        };
            //        db.TicketHistories.Add(th2);
            //    }

            //    if (oldTic.ProjectId != ticket.ProjectId)
            //    {

            //        TicketHistory th3 = new TicketHistory
            //        {
            //            TicketId = ticket.Id,
            //            Property = "Project",
            //            OldValue = db.Projects.Find(oldTic.ProjectId).Name,
            //            NewValue = db.Projects.Find(ticket.ProjectId).Name,
            //            EditId = editId,
            //            Changed = changed,
            //            UserId = userid
            //        };
            //        db.TicketHistories.Add(th3);
            //    }

            //    if (oldTic.TicketStatusId != ticket.TicketStatusId)
            //    {
            //        TicketHistory th4 = new TicketHistory
            //        {
            //            TicketId = ticket.Id,
            //            Property = "TicketStatus",
            //            OldValue = db.TicketStatuses.Find(oldTic.TicketStatusId).Name,
            //            NewValue = db.TicketStatuses.Find(ticket.TicketStatusId).Name,
            //            EditId = editId,
            //            Changed = changed,
            //            UserId = userid
            //        };
            //        db.TicketHistories.Add(th4);
            //    }

            //    if (oldTic.TicketPriorityId != ticket.TicketPriorityId)
            //    {
            //        ApplicationUser user = db.Users.Find(ticket.AssignedToUserId);
            //        TicketHistory th5 = new TicketHistory
            //        {
            //            TicketId = ticket.Id,
            //            Property = "TicketPriority",
            //            OldValue = db.TicketPriorities.Find(oldTic.TicketPriorityId).Name,
            //            NewValue = db.TicketPriorities.Find(ticket.TicketPriorityId).Name,
            //            EditId = editId,
            //            Changed = changed,
            //            UserId = userid
            //        };
            //        Notification note = new Notification
            //        {
            //            TicketId = ticket.Id,
            //            UserId = user.Id,
            //            Change = "Priority",
            //            Details = th5.NewValue,
            //            DateNotified = changed,
            //        };
            //        db.Notifications.Add(note);
            //        //user.SendNotification(note);
            //        db.TicketHistories.Add(th5);
            //    }

            //    if (oldTic.TicketTypeId != ticket.TicketTypeId)
            //    {
            //        TicketHistory th6 = new TicketHistory
            //        {
            //            TicketId = ticket.Id,
            //            Property = "TicketType",
            //            OldValue = db.TicketTypes.Find(oldTic.TicketTypeId).Name,
            //            NewValue = db.TicketTypes.Find(ticket.TicketTypeId).Name,
            //            EditId = editId,
            //            Changed = changed,
            //            UserId = userid
            //        };
            //        db.TicketHistories.Add(th6);
            //    }

            //    if (oldTic.AssignedToUserId != ticket.AssignedToUserId)
            //    {
            //        ApplicationUser user = db.Users.Find(ticket.AssignedToUserId);
            //        TicketHistory th7 = new TicketHistory
            //        {
            //            TicketId = ticket.Id,
            //            Property = "AssignedToUser",
            //            OldValue = oldTic.AssignedToUserId == null ? "" : db.Users.Find(oldTic.AssignedToUserId).DisplayName,
            //            NewValue = user.DisplayName,
            //            EditId = editId,
            //            Changed = changed,
            //            UserId = userid
            //        };

            //        Notification note = new Notification { TicketId = ticket.Id, UserId = user.Id, Change="Assigned", 
            //            Details=user.DisplayName, DateNotified=changed};
            //        db.Notifications.Add(note);
            //        //user.SendNotification(note);
            //        db.TicketHistories.Add(th7);
            //    }

            //    ticket.Updated = System.DateTimeOffset.Now;
            //    db.Entry(ticket).State = EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("Details", new {id = ticket.Id });
            //}

            var editable = new List<string>() { "Title", "Description", "AssignedToUserId", "TicketTypeId", "TicketPriorityId", "TicketStatusId", "ProjectId" };
            //if (User.IsInRole("Admin"))
            //    editable.Add("ProjectId");
            //if (User.IsInRole("Project Manager"))
            //    editable.AddRange(new string[] { "AssignedUserId", "TypeId", "PriorityId", "StatusId" });

            if (ModelState.IsValid)
            {
                string userid = User.Identity.GetUserId();
                var changed = System.DateTimeOffset.Now;
                var editId = Guid.NewGuid().ToString();

                var oldTicket = db.Tickets.AsNoTracking()
                    .FirstOrDefault(t => t.Id == ticket.Id);
                var histories = GetTicketHistories(oldTicket, ticket,userid, changed, editId)
                    .Where(h => editable.Contains(h.History.Property));

                var mailer = new EmailService();

                foreach (var item in histories)
                {
                    db.TicketHistories.Add(item.History);
                    if (item.Notification != null)
                        mailer.SendAsync(item.Notification);
                }

                db.Update(ticket, editable.ToArray()); // "Title", "Description", "AssignedToUserId", "TicketTypeId", "TicketPriorityId", "TicketStatusId", "ProjectId"
                db.SaveChanges();

                return RedirectToAction("Details",new {id= ticket.Id });
            }
            UserRolesHelper urh = new UserRolesHelper();
            ViewBag.AssignedToUserId = new SelectList(urh.UsersInRole("Developer").Where(u => u.Projects.Any(p => p.Id == ticket.ProjectId)), "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        private List<TicketHistoryWithNotification> GetTicketHistories(Ticket oldTicket, Ticket newTicket, string userid, DateTimeOffset changed, string editId)
        {
            var histories = new List<TicketHistoryWithNotification>();
            if (oldTicket.AssignedToUserId != newTicket.AssignedToUserId)
            {
                var newUser = db.Users.Find(newTicket.AssignedToUserId);

                histories.Add(new TicketHistoryWithNotification()
                {
                    History = new TicketHistory()
                    {
                        TicketId = newTicket.Id,
                        Property = "AssignedToUser",
                        OldValue = oldTicket.AssignedToUserId == null ? "" : db.Users.Find(oldTicket.AssignedToUserId).DisplayName,
                        NewValue = newUser?.UserName,
                        EditId = editId,
                        Changed = changed,
                        UserId = userid

                    },
                    Notification = newUser != null ? new IdentityMessage()
                    {
                        Subject = "You have a new Notification",
                        Destination = newUser.Email,
                        Body = "You have been assigned to a new ticket with Id " + newTicket.Id + "!"
                    } : null
                });
            }
            if (oldTicket.Description != newTicket.Description)
                histories.Add(new TicketHistoryWithNotification()
                {
                    History = new TicketHistory()
                    {
                        TicketId = newTicket.Id,
                        Property = "Description",
                        OldValue = oldTicket.Description,
                        NewValue = newTicket.Description,
                        EditId = editId,
                        Changed = changed,
                        UserId = userid
                    },
                    Notification = null
                });
            if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
                histories.Add(new TicketHistoryWithNotification()
                {
                    History = new TicketHistory()
                    {
                        TicketId = newTicket.Id,
                        Property = "Priority",
                        OldValue = oldTicket.TicketPriority?.Name,
                        NewValue = db.TicketPriorities.Find(newTicket.TicketPriorityId)?.Name,
                        EditId = editId,
                        Changed = changed,
                        UserId = userid
                    },
                    Notification = null
                });

            return histories;
        }

        private class TicketHistoryWithNotification
        {
            public TicketHistory History { get; set; }
            public IdentityMessage Notification { get; set; }
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
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
