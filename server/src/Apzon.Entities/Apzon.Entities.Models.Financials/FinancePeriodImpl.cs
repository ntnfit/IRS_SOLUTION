using System;
using SAPbobsCOM;

namespace Apzon.Entities.Models.Financials;

public class FinancePeriodImpl
{
	public int AbsoluteEntry { get; set; }

	public BoYesNoEnum ActiveforFeed { get; set; }

	public BoYesNoEnum AdditionalSubPeriods { get; set; }

	public BoYesNoEnum Locked { get; set; }

	public string PeriodCode { get; set; }

	public string PeriodIndicator { get; set; }

	public string PeriodName { get; set; }

	public PeriodStatusEnum PeriodStatus { get; set; }

	public DateTime PostingDateFrom { get; set; }

	public DateTime PostingDateTo { get; set; }

	public int SubNum { get; set; }

	public DateTime TaxDateFrom { get; set; }

	public DateTime TaxDateTo { get; set; }

	public DateTime ValueDateFrom { get; set; }

	public DateTime ValueDateTo { get; set; }
}
