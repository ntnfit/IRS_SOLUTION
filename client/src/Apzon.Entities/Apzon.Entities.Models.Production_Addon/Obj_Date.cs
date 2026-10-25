using System;
using System.Collections.Generic;

namespace Apzon.Entities.Models.Production_Addon;

public class Obj_Date
{
	public string ResCode { get; set; }

	public DateTime validFrom { get; set; }

	public DateTime validTo { get; set; }

	public List<Obj_Capacity> lstCapacity { get; set; }
}
