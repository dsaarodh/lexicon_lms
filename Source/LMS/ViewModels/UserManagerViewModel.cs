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

        [Display(Name ="Efternamn")]
        public string LastName { get; set; }

        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Display(Name = "Kurs")]
        public string Course { get; set; }

        [Display(Name = "Roll")]
        public string Role { get; set; }
    }
}