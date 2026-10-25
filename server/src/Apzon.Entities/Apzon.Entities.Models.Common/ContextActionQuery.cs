using System.Collections.Generic;
using System.Data;
using Apzon.Commons.Enums;

namespace Apzon.Entities.Models.Common;

public class ContextActionQuery
{
	public string QueryText { get; set; }

	public int QueryId { get; set; }

	public Enums.ContextActionDataFrom DataFrom { get; set; }

	public string DataParamType { get; set; }

	public DataTable TemporaryTable { get; set; }

	public List<QueryParamater> Paramaters { get; set; }
}
