using LMS.Models.AppData.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.AppData
{
	public class Activity : ActivityBase
    {
		public int ActivityTypeId { get; set; }
		public int ModuleId { get; set; }

		// navigational properties
		[ForeignKey(nameof(ActivityTypeId))]
		public virtual ActivityType ActivityType { get; set; }
		[ForeignKey(nameof(ModuleId))]
		public virtual Module Module { get; set; }
    }
}