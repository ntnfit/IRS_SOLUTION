using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Production;

public class ProductionOder_Impl_OWOR
{
	public int DocEntry { get; set; }

	public int DocNum { get; set; }

	public int? Series { get; set; }

	public string ItemCode { get; set; }

	public string ItemName { get; set; }

	public string Status { get; set; }

	public int MONo { get; set; }

	public string BatchIssue { get; set; }

	public string Type { get; set; }

	public double? PlannedQty { get; set; }

	public double? CmpltQty { get; set; }

	public double? RjctQty { get; set; }

	public DateTime? PostDate { get; set; }

	public DateTime? StartDate { get; set; }

	public DateTime? DueDate { get; set; }

	public int? OriginAbs { get; set; }

	public int? OriginNum { get; set; }

	public string OriginType { get; set; }

	public int? UserSign { get; set; }

	public string Comments { get; set; }

	public DateTime? CloseDate { get; set; }

	public DateTime? RlsDate { get; set; }

	public string CardCode { get; set; }

	public string Warehouse { get; set; }

	public string Uom { get; set; }

	public int? LineDirty { get; set; }

	public int? WOR1Count { get; set; }

	public string JrnlMemo { get; set; }

	public int? TransId { get; set; }

	public DateTime? CreateDate { get; set; }

	public string Printed { get; set; }

	public string OcrCode { get; set; }

	public string PIndicator { get; set; }

	public string OcrCode2 { get; set; }

	public string OcrCode3 { get; set; }

	public string OcrCode4 { get; set; }

	public string OcrCode5 { get; set; }

	public int? SeqCode { get; set; }

	public int? Serial { get; set; }

	public string SeriesStr { get; set; }

	public string SubStr { get; set; }

	public int? LogInstanc { get; set; }

	public int? UserSign2 { get; set; }

	public DateTime? UpdateDate { get; set; }

	public string Project { get; set; }

	public string SupplCode { get; set; }

	public int? UomEntry { get; set; }

	public string PickRmrk { get; set; }

	public string U_ARGNS_CUTTIC { get; set; }

	public double? U_AllocatedQty { get; set; }

	public double? U_ReservedQty { get; set; }

	public double? U_FreeQty { get; set; }

	public int? U_ARGNS_SOLine { get; set; }

	public int AtcEntry { get; set; }

	public List<AttachmentFile_Impl> AttachmentFiles { get; set; }

	public List<ProductionOder_LineImpl_WOR1> Line_WOR1 { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
