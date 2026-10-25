using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Payments;

public class Payments_InvoicesImpl
{
	public double AppliedFC { get; set; }

	public int Count { get; set; }

	public double DiscountPercent { get; set; }

	public string DistributionRule { get; set; }

	public string DistributionRule2 { get; set; }

	public string DistributionRule3 { get; set; }

	public string DistributionRule4 { get; set; }

	public string DistributionRule5 { get; set; }

	public int DocEntry { get; set; }

	public int DocLine { get; set; }

	public int InstallmentId { get; set; }

	public string InvType { get; set; }

	public int LineNum { get; set; }

	public DateTime LinkDate { get; set; }

	public double PaidSum { get; set; }

	public double SumApplied { get; set; }

	public double TotalDiscount { get; set; }

	public double TotalDiscountFC { get; set; }

	public double TotalDiscountSC { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public double WitholdingTaxApplied { get; set; }

	public double WitholdingTaxAppliedFC { get; set; }

	public double WitholdingTaxAppliedSC { get; set; }
}
