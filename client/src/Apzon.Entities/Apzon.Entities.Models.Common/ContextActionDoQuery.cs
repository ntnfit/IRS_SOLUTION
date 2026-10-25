using System.Collections.Generic;
using System.Data;

namespace Apzon.Entities.Models.Common;

public class ContextActionDoQuery
{
	public string QueryText { get; set; }

	public List<QueryParamater> Paramaters { get; set; }

	public string TempTableName { get; set; }

	public DataTable TempTableData { get; set; }
}
