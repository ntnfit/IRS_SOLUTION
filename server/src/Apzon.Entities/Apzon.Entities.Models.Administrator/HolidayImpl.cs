using System.Collections.Generic;

namespace Apzon.Entities.Models.Administrator;

public class HolidayImpl
{
	public string HldCode { get; set; }

	public string WndFrm { get; set; }

	public string WndTo { get; set; }

	public string isCurYear { get; set; }

	public string ignrWnd { get; set; }

	public string WeekNoRule { get; set; }

	public List<Holiday_LineImpl> Lines { get; set; }
}
