namespace BugTracker.Migrations
{
    using BugTracker.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            ApplicationUser user;
            if (!context.Users.Any(r => r.Email == "admin@coderfoundry.com"))
            {
                user = new ApplicationUser
                {
                    UserName = "admin@coderfoundry.com",
                    Email = "admin@coderfoundry.com",
                    FirstName = "Ria",
                    LastName = "Manglani",
                    DisplayName = "Ria Mang"
                };
                userManager.Create(user, "Bugtrack-1");
                
                userManager.AddToRole(user.Id, "Admin");
            }

            if (!context.Users.Any(r => r.Email == "tparrish@coderfoundry.com"))
            {
                user = new ApplicationUser
                {
                    UserName = "tparrish@coderfoundry.com",
                    Email = "tparrish@coderfoundry.com",
                    FirstName = "Thomas",
                    LastName = "Parrish",
                    DisplayName = "Thomas Parrish"
                };
                userManager.Create(user, "Password-1");

                userManager.AddToRole(user.Id, "Project Manager");
                userManager.AddToRole(user.Id, "Developer");
            }


            if (!context.Users.Any(r => r.Email == "ajensen@coderfoundry.com"))
            {
                user = new ApplicationUser
                {
                    UserName = "ajensen@coderfoundry.com",
                    Email = "ajensen@coderfoundry.com",
                    FirstName = "Andrew",
                    LastName = "Jensen",
                    DisplayName = "Andrew Jensen"
                };
                userManager.Create(user, "Password-1");

                userManager.AddToRole(user.Id, "Submitter");
                userManager.AddToRole(user.Id, "Developer");
            }
            //if (!userManager.IsInRole(user.Id, "Admin"))
            //{
            //    userManager.AddToRole(user.Id, "Admin");
            //}


            //// new from here --- for role
            //var roleManager1 = new RoleManager<IdentityRole>(
            //    new RoleStore<IdentityRole>(context));

            //if (!context.Roles.Any(r => r.Name == "Moderator"))
            //{
            //    roleManager1.Create(new IdentityRole { Name = "Moderator" });
            //}

            //// adding new user
            //var userManager1 = new UserManager<ApplicationUser>(
            //    new UserStore<ApplicationUser>(context));

            //ApplicationUser user1;
            //if (!context.Users.Any(r => r.Email == "lreaves@coderfoundry.com"))
            //{
            //    user1 = new ApplicationUser
            //    {
            //        UserName = "lreaves@coderfoundry.com",
            //        Email = "lreaves@coderfoundry.com",
            //        FirstName = "L",
            //        LastName = "Reaves",
            //        DisplayName = "LReaves",
            //        flag = "true"
            //    };
            //    userManager1.Create(user1, "Password-1");
            //}
            //else
            //{
            //    user1 = context.Users.Single(u => u.Email == "lreaves@coderfoundry.com");
            //}
            //if (!userManager1.IsInRole(user1.Id, "Moderator"))
            //{
            //    userManager1.AddToRole(user1.Id, "Moderator");
            //}


        }
    }
}
