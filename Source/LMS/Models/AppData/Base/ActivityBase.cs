using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMS.Models.AppData.Base
{
	public abstract class ActivityBase
	{
		public int Id { get; set; }

        [Display(Name = "Namn")]
		public string Name { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd hh:mm}")]
		[DataType(DataType.DateTime)]
        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd hh:mm}")]
		[DataType(DataType.DateTime)]
        [Display(Name = "Slutdatum")]
        public DateTime EndDate { get; set; }
	}
}