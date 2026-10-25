using System;
using System.Collections.Generic;

namespace Apzon.Entities.Models.Inventory;

public class InventoryRevaluationImpl
{
	public int DocEntry { get; set; }

	public int DocNum { get; set; }

	public DateTime DocDate { get; set; }

	public string Ref1 { get; set; }

	public string Ref2 { get; set; }

	public string Comments { get; set; }

	public string JrnlMemo { get; set; }

	public int TransId { get; set; }

	public TimeSpan DocTime { get; set; }

	public string RevalType { get; set; }

	public DateTime UpdateDate { get; set; }

	public DateTime CreateDate { get; set; }

	public int Series { get; set; }

	public DateTime TaxDate { get; set; }

	public string DataSource { get; set; }

	public int UserSign { get; set; }

	public int UserSign2 { get; set; }

	public int StationID { get; set; }

	public string RIncmAcct { get; set; }

	public string RExpnAcct { get; set; }

	public string LogInstanc { get; set; }

	public string ObjType { get; set; }

	public int SeqCode { get; set; }

	public int Serial { get; set; }

	public string SeriesStr { get; set; }

	public string SubStr { get; set; }

	public string VersionNum { get; set; }

	public List<InventoryRevaluation_LineMRV1Impl> LinesMRV1 { get; set; }

	public List<InventoryRevaluation_LineMRV2Impl> LinesMRV2 { get; set; }
}
