using Apzon.Commons.Enums;

namespace Apzon.Entities.Models.Common;

public class GetChangeLogParams
{
	public string PrimaryKey { get; set; }

	public string UDOObjectCode { get; set; }

	public Enums.ChangeLog Object { get; set; }

	public int LogInstance2 { get; set; }

	public int LogInstance { get; set; }

	public bool IsApzTable { get; set; }
}
