using LMS.Models.AppData;
using LMS.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace LMS.DataAccess
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityType> ActivityTypes { get; set; }

        public ApplicationDbContext() : base("LexiconLMSDbConnection", throwIfV1Schema: false)
        {
			Database.CreateIfNotExists();
		}

		public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}