using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Payments;

public class Payments_AccountsImpl
{
	public int Count { get; set; }

	public double EqualizationVatAmount { get; set; }

	public double GrossAmnt { get; set; }

	public double GrssAmntFC { get; set; }

	public int LineNum { get; set; }

	public int LocationCode { get; set; }

	public string OcrCode { get; set; }

	public string OcrCode2 { get; set; }

	public string OcrCode3 { get; set; }

	public string OcrCode4 { get; set; }

	public string OcrCode5 { get; set; }

	public string Project { get; set; }

	public double SumApplied { get; set; }

	public double AppliedFC { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public string VatGroup { get; set; }

	public int DocNum { get; set; }

	public int LineId { get; set; }

	public string AcctCode { get; set; }

	public string AcctName { get; set; }

	public decimal Amount { get; set; }

	public string Descrip { get; set; }

	public decimal VatPrcnt { get; set; }

	public string ObjType { get; set; }

	public decimal VatAmnt { get; set; }
}
