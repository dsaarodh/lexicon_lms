using LMS.Models.AppData.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.AppData
{
	public class Activity : ActivityBase
    {
		//public int ActivityTypeId { get; set; }

		//[ForeignKey(nameof(Module))]
		//public int Module_Id { get; set; }   

        // navigational properties
        public virtual ActivityType ActivityType { get; set; }
        public virtual Module Module { get; set; }
    }
}