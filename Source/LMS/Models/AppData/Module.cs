using LMS.Models.AppData.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.AppData
{
	public class Module : ActivityBase
    {
        [Display(Name = "Färg")]
        public string ColorCode { get; set; }

        [Display(Name = "Kurs")]
        public int CourseId { get; set; }

		// navigational properties
		[ForeignKey(nameof(CourseId))]
		public virtual Course Course { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
    }
}