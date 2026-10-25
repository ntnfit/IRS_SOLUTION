using System;
using System.Collections.Generic;

namespace Apzon.Entities.Models.SalesOpportunity;

public class OpportunitiesImpl
{
	public int OpprId { get; set; }

	public string CardCode { get; set; }

	public int? SlpCode { get; set; }

	public int? CprCode { get; set; }

	public int? Source { get; set; }

	public int? IntCat1 { get; set; }

	public int? IntCat2 { get; set; }

	public int? IntCat3 { get; set; }

	public int? IntRate { get; set; }

	public DateTime? OpenDate { get; set; }

	public string DifType { get; set; }

	public DateTime? PredDate { get; set; }

	public decimal? MaxSumLoc { get; set; }

	public decimal? MaxSumSys { get; set; }

	public decimal? WtSumLoc { get; set; }

	public decimal? WtSumSys { get; set; }

	public decimal? PrcnProf { get; set; }

	public decimal? SumProfL { get; set; }

	public decimal? SumProfS { get; set; }

	public string Memo { get; set; }

	public string Status { get; set; }

	public string StatusRem { get; set; }

	public int? Reason { get; set; }

	public decimal? RealSumLoc { get; set; }

	public decimal? RealSumSys { get; set; }

	public decimal? RealProfL { get; set; }

	public decimal? RealProfS { get; set; }

	public decimal? CloPrcnt { get; set; }

	public short? StepLast { get; set; }

	public short? UserSign { get; set; }

	public string Transfered { get; set; }

	public short? Instance { get; set; }

	public string CardName { get; set; }

	public DateTime? CloseDate { get; set; }

	public int? LastSlp { get; set; }

	public string Name { get; set; }

	public int? Territory { get; set; }

	public int? Industry { get; set; }

	public string ChnCrdCode { get; set; }

	public string ChnCrdName { get; set; }

	public string PrjCode { get; set; }

	public short? CardGroup { get; set; }

	public int? ChnCrdCon { get; set; }

	public int? Owner { get; set; }

	public string attachment { get; set; }

	public string DocType { get; set; }

	public int? DocNum { get; set; }

	public int? DocEntry { get; set; }

	public string DocChkbox { get; set; }

	public int? AtcEntry { get; set; }

	public int? LogInstanc { get; set; }

	public short? UserSign2 { get; set; }

	public DateTime? UpdateDate { get; set; }

	public string OpprType { get; set; }

	public string U_CostCenter { get; set; }

	public string U_Indicator { get; set; }

	public List<OpportunityStageImpl> ListStage { get; set; }

	public List<OpportunityCompetitorImpl> ListCompetitor { get; set; }

	public List<OpportunityPartnerImpl> ListPartner { get; set; }

	public List<OpportunityReasonImpl> ListReason { get; set; }

	public object Stage { get; set; }
}
