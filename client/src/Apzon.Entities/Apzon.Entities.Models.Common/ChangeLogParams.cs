using System;

namespace Apzon.Entities.Models.Common;

public class ChangeLogParams
{
	public int LogInstance { get; set; }

	public DateTime UpdateDate { get; set; }

	public string UserName { get; set; }

	public string ObjectCode { get; set; }
}
