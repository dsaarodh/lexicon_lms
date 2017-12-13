using LMS.Models.AppData.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.AppData
{
    public class Activity : ActivityBase
    {
		[Display(Name = "Typ")]
		public int ActivityTypeId { get; set; }

        [Display(Name = "Modul")]
        public int ModuleId { get; set; }

		// navigational properties
		[ForeignKey(nameof(ActivityTypeId))]
        public virtual ActivityType ActivityType { get; set; }
		[ForeignKey(nameof(ModuleId))]
		public virtual Module Module { get; set; }
    }
}