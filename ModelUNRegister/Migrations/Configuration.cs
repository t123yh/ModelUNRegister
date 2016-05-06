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
        const string adminActualName = "系统管理员";
        const string userRoleName = "Users";

        dynamic originRoles = new[] {
                new { Name = adminRoleName, DisplayName = "系统管理员组" } ,
                new { Name = userRoleName, DisplayName = "用户组" }
        };

        protected override void Seed(ModelUNRegister.Models.ApplicationDbContext context)
        {
            IdentityResult result;

            var roleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context));

            foreach (var item in originRoles)
            {
                if (!roleManager.RoleExists((string)item.Name))
                {
                    IdentityRole administrators = new IdentityRole(item.Name);
                    result = roleManager.Create(administrators);
                    if (!result.Succeeded) throw new InvalidOperationException(string.Concat(result.Errors));
                }
            }
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            ApplicationUser adminUser;
            if ((adminUser = userManager.FindByName(AppSettings.InitalAdminAccount)) == null)
            {
                adminUser = new ApplicationUser() { UserName = AppSettings.InitalAdminAccount, Email = AppSettings.InitalAdminEmail };

                result = userManager.Create(adminUser, AppSettings.InitalAdminPassword);
                if (!result.Succeeded) throw new InvalidOperationException(string.Concat(result.Errors));
            }

            adminUser.EmailConfirmed = true;
            adminUser.ActualName = adminActualName;
            userManager.Update(adminUser);

            result = userManager.RemovePassword(adminUser.Id);
            if (!result.Succeeded) throw new InvalidOperationException(string.Concat(result.Errors));

            result = userManager.AddPassword(adminUser.Id, AppSettings.InitalAdminPassword);
            if (!result.Succeeded) throw new InvalidOperationException(string.Concat(result.Errors));

            if (!userManager.IsInRole(adminUser.Id, adminRoleName))
            {
                result = userManager.AddToRole(adminUser.Id, adminRoleName);
                if (!result.Succeeded) throw new InvalidOperationException(string.Concat(result.Errors));
            }
            context.SaveChanges();
        }
    }
}
