using System.Collections.Generic;
using LMS.Models.Identity;
using LMS.Models.AppData.Base;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models.AppData
{
	public class Course : ActivityBase
    {
        // navigational properties
        [Display(Name = "Kursdeltagare")]
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Module> Modules { get; set; }
    }
}