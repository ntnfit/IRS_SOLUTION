using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Payments;

public class BillOfExchangeImpl
{
	public string BPBankAct { get; set; }

	public string BPBankCode { get; set; }

	public string BPBankCountry { get; set; }

	public DateTime BillOfExchangeDueDate { get; set; }

	public string BillOfExchangeNo { get; set; }

	public string ControlKey { get; set; }

	public string Details { get; set; }

	public double DiscountAmount { get; set; }

	public DateTime DiscountDate { get; set; }

	public double FineAmount { get; set; }

	public DateTime FineDate { get; set; }

	public int FolioNumber { get; set; }

	public string FolioPrefixString { get; set; }

	public double IOFAmount { get; set; }

	public double InterestAmount { get; set; }

	public DateTime InterestDate { get; set; }

	public double OtherExpensesAmount { get; set; }

	public double OtherIncomesAmount { get; set; }

	public string PaymentEngineStatus1 { get; set; }

	public string PaymentEngineStatus2 { get; set; }

	public string PaymentEngineStatus3 { get; set; }

	public string PaymentMethodCode { get; set; }

	public string ReferenceNo { get; set; }

	public string Remarks { get; set; }

	public double ServiceFeeAmount { get; set; }

	public double StampTaxAmount { get; set; }

	public string StampTaxCode { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
