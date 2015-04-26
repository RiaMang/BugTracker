using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class ProjUserViewModel
    {
        public Project Project { get; set; }
        public MultiSelectList Users { get; set; }
        public string[] SelectedUsers { get; set; }

    }
    public class ProjListViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsInRole { get; set; }
    }

    public class ProjUsersVM
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public IList<ApplicationUser> Users { get; set; }
    }
}