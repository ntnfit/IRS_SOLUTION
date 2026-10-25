using System;
using System.Collections.Generic;

namespace Apzon.Entities.Models.DMS;

public class SurveyImpl
{
	public int DocEntry { get; set; }

	public string PersonName { get; set; }

	public string Address { get; set; }

	public string Phone { get; set; }

	public string Email { get; set; }

	public string Notes { get; set; }

	public int Rate { get; set; }

	public int SurveyQuessionId { get; set; }

	public int ClgCode { get; set; }

	public DateTime CreateTime { get; set; }

	public DateTime UpdateTime { get; set; }

	public List<SurveyLineImpl> SurveyLine { get; set; }
}
