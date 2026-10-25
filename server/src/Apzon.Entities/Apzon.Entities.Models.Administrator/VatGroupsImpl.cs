using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Administrator;

public class VatGroupsImpl
{
	public string AcquisitionReverse { get; set; }

	public string AcquisitionTax { get; set; }

	public string CashDiscountAccount { get; set; }

	public string Category { get; set; }

	public string Code { get; set; }

	public string Correction { get; set; }

	public string DeferredTaxAcc { get; set; }

	public string DownPaymentTaxOffsetAccount { get; set; }

	public string EU { get; set; }

	public string EqualizationTaxAccount { get; set; }

	public string GoodsShipment { get; set; }

	public string Inactive { get; set; }

	public string Name { get; set; }

	public double? Rate { get; set; }

	public double NonDeduct { get; set; }

	public string NonDeductAcc { get; set; }

	public string Report349Code { get; set; }

	public string ServiceSupply { get; set; }

	public string TaxAccount { get; set; }

	public string TaxRegion { get; set; }

	public string TaxTypeBlackList { get; set; }

	public string TriangularDeal { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public string VATDeductibleAccount { get; set; }

	public string VATInRevenueAccount { get; set; }

	public string VatCorrection { get; set; }

	public List<VatGroups_LinesImpl> VatGroups_Lines { get; set; }

	public DateTime EffecDate { get; set; }

	public DateTime UpdateDate { get; set; }

	public int UserSign { get; set; }

	public string Locked { get; set; }

	public string Account { get; set; }
}
