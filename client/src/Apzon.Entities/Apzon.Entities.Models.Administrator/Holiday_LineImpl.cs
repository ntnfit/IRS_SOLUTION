using System;

namespace Apzon.Entities.Models.Administrator;

public class Holiday_LineImpl
{
	public int Code { get; set; }

	public string HldCode { get; set; }

	public DateTime StrDate { get; set; }

	public DateTime EndDate { get; set; }

	public string IsCurYear { get; set; }

	public string Rmrks { get; set; }
}
