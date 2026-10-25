using System.Collections.Generic;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Production;

public class BillOfMasterials_LineImpl_ITT1
{
	public string Father { get; set; }

	public int ChildNum { get; set; }

	public string Code { get; set; }

	public decimal Quantity { get; set; }

	public string Warehouse { get; set; }

	public decimal Price { get; set; }

	public string Currency { get; set; }

	public short PriceList { get; set; }

	public decimal OrigPrice { get; set; }

	public string OrigCurr { get; set; }

	public string IssueMthd { get; set; }

	public string Uom { get; set; }

	public string Comment { get; set; }

	public int LogInstanc { get; set; }

	public string Object { get; set; }

	public string OcrCode { get; set; }

	public string OcrCode2 { get; set; }

	public string OcrCode3 { get; set; }

	public string OcrCode4 { get; set; }

	public string OcrCode5 { get; set; }

	public string PrncpInput { get; set; }

	public string Project { get; set; }

	public int Type { get; set; }

	public string WipActCode { get; set; }

	public decimal AddQuantit { get; set; }

	public string LineText { get; set; }

	public string ItemCode { get; set; }

	public string ItemName { get; set; }

	public string InvntryUom { get; set; }

	public double OnHand { get; set; }

	public double OnOrder { get; set; }

	public List<ItemsImpl> objItems { get; set; }

	public int DocEntry { get; set; }

	public int LineNum { get; set; }

	public string ItemType { get; set; }

	public string wareHouse { get; set; }

	public double? BaseQty { get; set; }

	public double? PlannedQty { get; set; }

	public string IssueType { get; set; }

	public string UomCode { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
