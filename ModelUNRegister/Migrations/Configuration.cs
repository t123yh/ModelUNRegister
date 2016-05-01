namespace ModelUNRegister.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web;
    using Utilities;
    internal sealed class Configuration : DbMigrationsConfiguration<ModelUNRegister.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "ModelUNRegister.Models.ApplicationDbContext";
        }

        const string adminRoleName = "Administrators";
        const string userRoleName = "Users";

        dynamic originRoles = new[] {
                new { Name = adminRoleName, DisplayName = "系统管理员组" } ,
                new { Name = userRoleName, DisplayName = "用户组" }
        };

        protected override void Seed(ModelUNRegister.Models.ApplicationDbContext context)
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

            var roleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context));

            foreach (var item in originRoles)
            {
                if (!roleManager.RoleExists((string)item.Name))
                {
                    IdentityRole administrators = new IdentityRole(item.Name);
                    var result = roleManager.Create(administrators);
                    if (!result.Succeeded) throw new InvalidOperationException(string.Concat(result.Errors));
                }
            }

            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            if (userManager.FindByName(AppSettings.InitalAdminAccount) == null)
            {
                ApplicationUser newUser = new ApplicationUser() { UserName = AppSettings.InitalAdminAccount, Email = AppSettings.InitalAdminEmail };
                var result = userManager.Create(newUser, AppSettings.InitalAdminPassword);
                if (!result.Succeeded) throw new InvalidOperationException(string.Concat(result.Errors));
                result = userManager.AddToRole(newUser.Id, adminRoleName);
                if (!result.Succeeded) throw new InvalidOperationException(string.Concat(result.Errors));
            }
            else
            {
                var adminUser = userManager.FindByName(AppSettings.InitalAdminAccount);

                var result = userManager.RemovePassword(adminUser.Id);
                if (!result.Succeeded) throw new InvalidOperationException(string.Concat(result.Errors));

                result = userManager.AddPassword(adminUser.Id, AppSettings.InitalAdminPassword);
                if (!result.Succeeded) throw new InvalidOperationException(string.Concat(result.Errors));

                if (!userManager.IsInRole(adminUser.Id, adminRoleName))
                {
                    result = userManager.AddToRole(adminUser.Id, adminRoleName);
                    if (!result.Succeeded) throw new InvalidOperationException(string.Concat(result.Errors));
                }
            }
        }
    }
}
