using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Financials;

public class ChartOfAccount
{
	public int AccountLevel { get; set; }

	public string AccountPurposeCode { get; set; }

	public string AccountType { get; set; }

	public string AcctCurrency { get; set; }

	public bool ActiveAccount { get; set; }

	public bool AllowChangeVatGroup { get; set; }

	public bool AllowMultipleLinking { get; set; }

	public int BPLID { get; set; }

	public string BPLName { get; set; }

	public double Balance { get; set; }

	public double Balance_FrgnCurr { get; set; }

	public double Balance_syscurr { get; set; }

	public bool BudgetAccount { get; set; }

	public bool CashAccount { get; set; }

	public int Category { get; set; }

	public string Code { get; set; }

	public string DataExportCode { get; set; }

	public int DatevAccount { get; set; }

	public bool DatevAutoAccount { get; set; }

	public bool DatevFirstDataEntry { get; set; }

	public string DefaultVatGroup { get; set; }

	public string Details { get; set; }

	public bool DistributionRule2Relevant { get; set; }

	public bool DistributionRule3Relevant { get; set; }

	public bool DistributionRule4Relevant { get; set; }

	public bool DistributionRule5Relevant { get; set; }

	public bool DistributionRuleRelevant { get; set; }

	public string ExternalCode { get; set; }

	public int ExternalReconNo { get; set; }

	public string FatherAccountKey { get; set; }

	public string ForeignName { get; set; }

	public string FormatCode { get; set; }

	public bool FrozenFor { get; set; }

	public DateTime FrozenFrom { get; set; }

	public string FrozenRemarks { get; set; }

	public DateTime FrozenTo { get; set; }

	public int InternalReconNo { get; set; }

	public bool LiableForAdvances { get; set; }

	public string LoadingFactorCode { get; set; }

	public string LoadingFactorCode2 { get; set; }

	public string LoadingFactorCode3 { get; set; }

	public string LoadingFactorCode4 { get; set; }

	public string LoadingFactorCode5 { get; set; }

	public bool LoadingType { get; set; }

	public bool LockManualTransaction { get; set; }

	public string Name { get; set; }

	public string PlanningLevel { get; set; }

	public bool PrimaryAccount { get; set; }

	public string ProjectCode { get; set; }

	public bool ProjectRelevant { get; set; }

	public bool Protected { get; set; }

	public bool RateConversion { get; set; }

	public bool ReconciledAccount { get; set; }

	public string ReferentialAccountCode { get; set; }

	public bool RevaluationCoordinated { get; set; }

	public bool TaxExemptAccount { get; set; }

	public bool TaxLiableAccount { get; set; }

	public string TransactionCode { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public string VATRegNum { get; set; }

	public bool ValidFor { get; set; }

	public DateTime ValidFrom { get; set; }

	public string ValidRemarks { get; set; }

	public DateTime ValidTo { get; set; }
}
