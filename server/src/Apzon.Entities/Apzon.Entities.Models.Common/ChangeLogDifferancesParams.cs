using System;

namespace Apzon.Entities.Models.Common;

public class ChangeLogDifferancesParams
{
	public DateTime Date { get; set; }

	public string ChangeField { get; set; }

	public object OldValue { get; set; }

	public object NewValue { get; set; }

	public object ArrayOffet { get; set; }

	public int LineNumber { get; set; }
}
