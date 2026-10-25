using System;

namespace Apzon.Entities.Models.Common;

public class FormSetting
{
	public string FormId { get; set; }

	public string ItemId { get; set; }

	public string ColumnId { get; set; }

	public string ColumnName { get; set; }

	public string UserSign { get; set; }

	public int Width { get; set; }

	public bool Visible { get; set; }

	public int VisIndex { get; set; }

	public bool Editable { get; set; }

	public bool IsReadOnly { get; set; }

	public string Fixed { get; set; }

	public bool AllowChSet { get; set; }

	public bool VisSetting { get; set; }

	public int LogInstanc { get; set; }

	public int UserUpdate { get; set; }

	public DateTime UpdateDate { get; set; }

	public string LngCode { get; set; }

	public bool NeverEditable { get; set; }

	public int GroupIndex { get; set; }

	public string CurrField { get; set; }
}
