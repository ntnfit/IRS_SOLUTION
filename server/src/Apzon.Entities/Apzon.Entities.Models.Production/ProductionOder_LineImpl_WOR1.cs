using System;
using System.Collections.Generic;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Production;

public class ProductionOder_LineImpl_WOR1
{
	public int DocEntry { get; set; }

	public int LineNum { get; set; }

	public string ItemCode { get; set; }

	public double? BaseQty { get; set; }

	public double? PlannedQty { get; set; }

	public double? IssuedQty { get; set; }

	public string IssueType { get; set; }

	public string wareHouse { get; set; }

	public int? VisOrder { get; set; }

	public string WipActCode { get; set; }

	public double? CompTotal { get; set; }

	public string OcrCode { get; set; }

	public string OcrCode2 { get; set; }

	public string OcrCode3 { get; set; }

	public string OcrCode4 { get; set; }

	public string OcrCode5 { get; set; }

	public int? LocCode { get; set; }

	public int? LogInstanc { get; set; }

	public int? LoadFrBOM { get; set; }

	public string Project { get; set; }

	public int? UomEntry { get; set; }

	public string UomCode { get; set; }

	public string ItemType { get; set; }

	public double? AdditQty { get; set; }

	public string LineText { get; set; }

	public string PickStatus { get; set; }

	public double? PickQty { get; set; }

	public int? PickIdNo { get; set; }

	public double? ReleaseQty { get; set; }

	public string ResAlloc { get; set; }

	public BillOfMasterials_LineImpl_ITT1 line_ITT1 { get; set; }

	public string ItemName { get; set; }

	public string InvntryUom { get; set; }

	public double OnHand { get; set; }

	public double OnOrder { get; set; }

	public DateTime? StartDate { get; set; }

	public DateTime? EndDate { get; set; }

	public List<ItemsImpl> objItems { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public List<BatchNumbersImpl> BatchNumbers { get; set; }

	public List<SerialNumbersImpl> SerialNumbers { get; set; }
}
