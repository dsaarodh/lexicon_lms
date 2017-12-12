using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMS.ViewModels
{
    public class UserManagerViewModel
    {
        public string Id { get; set; }

        [Display(Name ="Last name")]
        public string LastName { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Display(Name = "Course")]
        public string Course { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }
    }
}