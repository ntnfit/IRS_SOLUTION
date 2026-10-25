using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Entities.Models.Documents;

public class Document_LinesAdditionalExpensesImpl
{
	public BoYesNoEnum AquisitionTax { get; set; }

	public int BaseGroup { get; set; }

	public int Count { get; set; }

	public double DeductibleTaxSum { get; set; }

	public double DeductibleTaxSumFC { get; set; }

	public double DeductibleTaxSumSys { get; set; }

	public string DistributionRule { get; set; }

	public string DistributionRule2 { get; set; }

	public string DistributionRule3 { get; set; }

	public string DistributionRule4 { get; set; }

	public string DistributionRule5 { get; set; }

	public double EqualizationTaxFC { get; set; }

	public double EqualizationTaxPercent { get; set; }

	public double EqualizationTaxSum { get; set; }

	public double EqualizationTaxSys { get; set; }

	public int ExpenseCode { get; set; }

	public int GroupCode { get; set; }

	public int LineNumber { get; set; }

	public double LineTotal { get; set; }

	public double LineTotalFC { get; set; }

	public double LineTotalSys { get; set; }

	public double PaidToDate { get; set; }

	public double PaidToDateFC { get; set; }

	public double PaidToDateSys { get; set; }

	public string Project { get; set; }

	public string TaxCode { get; set; }

	public List<TaxJurisdictionsImpl> TaxJurisdictions { get; set; }

	public BoYesNoEnum TaxLiable { get; set; }

	public double TaxPaid { get; set; }

	public double TaxPaidFC { get; set; }

	public double TaxPaidSys { get; set; }

	public double TaxPercent { get; set; }

	public double TaxSum { get; set; }

	public double TaxSumFC { get; set; }

	public double TaxSumSys { get; set; }

	public double TaxTotalSum { get; set; }

	public double TaxTotalSumFC { get; set; }

	public double TaxTotalSumSys { get; set; }

	public BoAdEpnsTaxTypes TaxType { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public string VatGroup { get; set; }

	public BoYesNoEnum WTLiable { get; set; }
}
