using System;
using System.Collections.Generic;

namespace Apzon.Entities.Models.DMS;

public class QuestionTemplateImpl
{
	public int DocEntry { get; set; }

	public int UserSign { get; set; }

	public string Status { get; set; }

	public DateTime CreateTime { get; set; }

	public DateTime UpdateTime { get; set; }

	public string Remark { get; set; }

	public List<QuestionTemplateLineImpl> QuetsionTemplateLine { get; set; }
}
