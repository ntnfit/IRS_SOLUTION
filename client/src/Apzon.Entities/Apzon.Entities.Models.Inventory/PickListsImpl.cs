using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Inventory;

public class PickListsImpl
{
	public int AbsEntry { get; set; }

	public string Name { get; set; }

	public string OwnerCode { get; set; }

	public string OwnerName { get; set; }

	public DateTime PickDate { get; set; }

	public string Remarks { get; set; }

	public string Canceled { get; set; }

	public int ShipType { get; set; }

	public string Status { get; set; }

	public string Printed { get; set; }

	public int LogInstac { get; set; }

	public string ObjType { get; set; }

	public DateTime UpdateDate { get; set; }

	public DateTime CreateDate { get; set; }

	public string UserSign { get; set; }

	public string UserSign2 { get; set; }

	public string UseBaseUn { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public List<PickLists_LineImpl> Lines { get; set; }
}
