using System;

namespace Apzon.Entities.Models.SalesOpportunity;

public class OpportunityStageImpl
{
	public int OpprId { get; set; }

	public short Line { get; set; }

	public int? SlpCode { get; set; }

	public int? CntctCode { get; set; }

	public DateTime? OpenDate { get; set; }

	public DateTime? CloseDate { get; set; }

	public int? Step_Id { get; set; }

	public decimal? ClosePrcnt { get; set; }

	public decimal? MaxSumLoc { get; set; }

	public decimal? MaxSumSys { get; set; }

	public string Memo { get; set; }

	public int? DocId { get; set; }

	public string ObjType { get; set; }

	public string Status { get; set; }

	public string Linked { get; set; }

	public decimal? WtSumLoc { get; set; }

	public decimal? WtSumSys { get; set; }

	public short? UserSign { get; set; }

	public string ChnCrdCode { get; set; }

	public string ChnCrdName { get; set; }

	public int? ChnCrdCon { get; set; }

	public string DocChkbox { get; set; }

	public int? Owner { get; set; }

	public int? DocNumber { get; set; }
}
