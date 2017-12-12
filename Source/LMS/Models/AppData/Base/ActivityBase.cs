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

		public string Name { get; set; }
		public string Description { get; set; }

		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd hh:mm}")]
		[DataType(DataType.DateTime)]
		public DateTime StartDate { get; set; }

		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd hh:mm}")]
		[DataType(DataType.DateTime)]
		public DateTime EndDate { get; set; }
	}
}