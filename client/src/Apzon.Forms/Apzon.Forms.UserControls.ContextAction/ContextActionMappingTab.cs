using System.Collections.Generic;
using System.Data;

namespace Apzon.Forms.UserControls.ContextAction;

public class ContextActionMappingTab
{
	public string Name { get; set; }

	public string Caption { get; set; }

	public string MappingType { get; set; }

	public string TargetItem { get; set; }

	public bool HasSumaryType { get; set; }

	public string ResourceName { get; set; }

	public List<ContextActionMappingFixedRow> FixedRow { get; set; }

	public List<string> IgnoreFields { get; set; }

	public DataTable Structure { get; set; }

	public DataTable MappingSource { get; set; }

	public UcContextActionMappingGridView GridView { get; set; }
}
