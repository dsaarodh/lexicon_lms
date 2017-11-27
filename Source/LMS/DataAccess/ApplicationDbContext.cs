using LMS.Models;
using LMS.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace LMS.DataAccess
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<CourseModel> Courses { get; set; }
        public DbSet<ModuleModel> Modules { get; set; }

        public ApplicationDbContext() : base("LexiconLMSDbConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}