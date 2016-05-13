using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelUNRegister.Models
{
    // 可以通过向 ApplicationUser 类添加更多属性来为用户添加配置文件数据。若要了解详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=317594。
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            userIdentity.AddClaim(new Claim("ActualName", this.ActualName));
            userIdentity.AddClaim(new Claim("IsAdministrator", manager.IsInRole(this.Id, "Administrators").ToString()));
            return userIdentity;
        }

        public virtual EnrollRequest EnrollRequest { get; set; }

        public virtual ICollection<EnrollQuestionAnswer> Answers { get; set; }

        [Display(Name = "姓名")]
        public string ActualName { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ApplicationDbContext>());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<ApplicationUser>()
            //    .HasOptional(u => u.EnrollRequest)
            //    .WithRequired(req => req.User)
            //    .Map(conf => conf.MapKey("UserId"))
            //    .WillCascadeOnDelete(true);

            modelBuilder.Entity<EnrollRequest>()
                .HasRequired(r => r.User)
                .WithOptional(u => u.EnrollRequest)
                .Map(conf => conf.MapKey("UserId"))
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<EnrollQuestionAnswer>()
                .HasRequired(r => r.User)
                .WithMany(u => u.Answers)
                .Map(conf => conf.MapKey("UserId"))
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<EnrollQuestionAnswer>()
                .HasRequired(r => r.Question)
                .WithMany()
                .Map(conf => conf.MapKey("QuestionId"))
                .WillCascadeOnDelete(true);
        }

        public DbSet<EnrollRequest> EnrollRequests { get; set; }

        public DbSet<EnrollQuestion> Questions { get; set; }

        public DbSet<EnrollQuestionAnswer> Answers { get; set; }

        public System.Data.Entity.DbSet<ModelUNRegister.Models.Article> Articles { get; set; }
    }
}