using System.ComponentModel.DataAnnotations;

namespace LMS.Models.AppData
{
	public class ActivityType
    {
        public int Id { get; set; }

        [Display(Name = "Typ")]
        public string Name { get; set; }
    }
}