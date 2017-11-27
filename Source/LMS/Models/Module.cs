using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class Module
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ColorCode { get; set; }

        public int CourseId { get; set; }   

        // navigational properties
        public virtual Course Courses { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
    }
}