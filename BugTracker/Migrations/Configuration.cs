namespace BugTracker.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
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
            if (!context.Users.Any(r => r.Email == "rmanglani@coderfoundry.com"))
            {
                user = new ApplicationUser
                {
                    UserName = "rmanglani@coderfoundry.com",
                    Email = "rmanglani@coderfoundry.com",
                    FirstName = "Ria",
                    LastName = "Manglani",
                    DisplayName = "Ria Mang"
                };
                userManager.Create(user, "Bugtrack-1");

                userManager.AddToRole(user.Id, "Admin");
            }

            if (!context.Users.Any(r => r.Email == "manny@manglani.com"))
            {
                user = new ApplicationUser
                {
                    UserName = "manny@manglani.com",
                    Email = "manny@manglani.com",
                    FirstName = "Manny",
                    LastName = "Manglani",
                    DisplayName = "Manny Manglani"
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

                userManager.AddToRole(user.Id, "Project Manager");
                userManager.AddToRole(user.Id, "Developer");
            }

            if (!context.Users.Any(r => r.Email == "admin@manglani.com"))
            {
                user = new ApplicationUser
                {
                    UserName = "admin@manglani.com",
                    Email = "admin@manglani.com",
                    FirstName = "Admin",
                    LastName = "Guest",
                    DisplayName = "Guest Admin"
                };
                userManager.Create(user, "GuestAdmin-1");

                userManager.AddToRole(user.Id, "Admin");
                userManager.AddToRole(user.Id, "Developer");
            }

            if (!context.Users.Any(r => r.Email == "manager@manglani.com"))
            {
                user = new ApplicationUser
                {
                    UserName = "manager@manglani.com",
                    Email = "manager@manglani.com",
                    FirstName = "Project Manager",
                    LastName = "Guest",
                    DisplayName = "Guest PM"
                };
                userManager.Create(user, "GuestPM-1");

                userManager.AddToRole(user.Id, "Project Manager");
            }

            if (!context.Users.Any(r => r.Email == "developer@manglani.com"))
            {
                user = new ApplicationUser
                {
                    UserName = "developer@manglani.com",
                    Email = "developer@manglani.com",
                    FirstName = "Developer",
                    LastName = "Guest",
                    DisplayName = "Guest Dev"
                };
                userManager.Create(user, "GuestDev-1");

                userManager.AddToRole(user.Id, "Developer");
            }

            if (!context.Users.Any(r => r.Email == "submitter@manglani.com"))
            {
                user = new ApplicationUser
                {
                    UserName = "submitter@manglani.com",
                    Email = "submitter@manglani.com",
                    FirstName = "Submitter",
                    LastName = "Guest",
                    DisplayName = "Guest Submitter"
                };
                userManager.Create(user, "GuestSubmitter-1");

                userManager.AddToRole(user.Id, "Submitter");
            }
        }
    }
}
