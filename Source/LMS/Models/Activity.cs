using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class Activity
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int ActivityTypeId { get; set; }
        public int ModuleId { get; set; }   

        // navigational properties
        public virtual ActivityType ActivityType { get; set; }
        public virtual Module Module { get; set; }
    }
}