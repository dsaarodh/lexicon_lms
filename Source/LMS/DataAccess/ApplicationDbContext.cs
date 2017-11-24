using LMS.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LMS.DataAccess
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("LexiconLMSDbConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}