using System.Collections.Generic;
using LMS.Models.Identity;
using LMS.Models.AppData.Base;

namespace LMS.Models.AppData
{
	public class Course : ActivityBase
    {
        // navigational properties
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Module> Modules { get; set; }
    }
}