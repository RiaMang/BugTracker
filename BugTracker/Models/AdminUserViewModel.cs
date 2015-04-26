﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class AdminUserViewModel
    {
        public ApplicationUser User { get; set; }
        public MultiSelectList Roles { get; set; }
        public string[] SelectedRoles { get; set; }

    }

    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public bool IsInRole { get; set; }
    }

    public class UserRolesVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public IList <string> Roles { get; set; }
    }
}