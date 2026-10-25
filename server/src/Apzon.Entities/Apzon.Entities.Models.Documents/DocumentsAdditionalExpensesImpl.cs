using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Documents;

public class DocumentsAdditionalExpensesImpl
{
	public string IsAcquistn { get; set; }

	public int DocEntry { get; set; }

	public int BaseAbsEnt { get; set; }

	public int BaseLnNum { get; set; }

	public int BaseType { get; set; }

	public int LineNum { get; set; }

	public int Count { get; set; }

	public double DedVatSum { get; set; }

	public double DedVatSumF { get; set; }

	public double DedVatSumS { get; set; }

	public string DistrbMthd { get; set; }

	public string OcrCode { get; set; }

	public string OcrCode2 { get; set; }

	public string OcrCode3 { get; set; }

	public string OcrCode4 { get; set; }

	public string OcrCode5 { get; set; }

	public double EquVatSumF { get; set; }

	public double EquVatPer { get; set; }

	public double EquVatSum { get; set; }

	public double EquVatSumS { get; set; }

	public int ExpnsCode { get; set; }

	public string LstPchPrce { get; set; }

	public double LineTotal { get; set; }

	public double TotalFrgn { get; set; }

	public double TotalSumSy { get; set; }

	public double PaidToDate { get; set; }

	public double PaidFC { get; set; }

	public double PaidSys { get; set; }

	public string Project { get; set; }

	public string Comments { get; set; }

	public string Status { get; set; }

	public string Stock { get; set; }

	public int TrgAbsEnt { get; set; }

	public int TrgType { get; set; }

	public string TaxCode { get; set; }

	public List<TaxJurisdictionsImpl> TaxJurisdictions { get; set; }

	public string TaxStatus { get; set; }

	public double LineVat { get; set; }

	public double LineVatF { get; set; }

	public double LineVatS { get; set; }

	public double VatPrcnt { get; set; }

	public double VatSum { get; set; }

	public double VatSumFrgn { get; set; }

	public double VatSumSy { get; set; }

	public string TaxType { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public string VatGroup { get; set; }

	public double GrsAmount { get; set; }

	public double GrsFC { get; set; }
}
