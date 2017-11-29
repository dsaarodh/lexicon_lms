using LMS.Models.AppData.Base;

namespace LMS.Models.AppData
{
	public class Activity : ActivityBase
    {
        //public int ActivityTypeId { get; set; }
        //public int ModuleId { get; set; }   

        // navigational properties
        public virtual ActivityType ActivityType { get; set; }
        public virtual Module Module { get; set; }
    }
}