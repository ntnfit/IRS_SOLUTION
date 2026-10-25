using System;
using System.Collections.Generic;
using SAPbobsCOM;

namespace Apzon.Entities.Models.Documents;

public class DownPaymentsToDrawImpl
{
	public double AmountToDraw { get; set; }

	public double AmountToDrawFC { get; set; }

	public double AmountToDrawSC { get; set; }

	public int Count { get; set; }

	public string Details { get; set; }

	public int DocEntry { get; set; }

	public int DocInternalID { get; set; }

	public int DocNumber { get; set; }

	public DownPaymentTypeEnum DownPaymentType { get; set; }

	public List<DownPaymentToDrawDetailImpl> DownPaymentsToDrawDetails { get; set; }

	public DateTime DueDate { get; set; }

	public double GrossAmountToDraw { get; set; }

	public double GrossAmountToDrawFC { get; set; }

	public double GrossAmountToDrawSC { get; set; }

	public BoYesNoEnum IsGrossLine { get; set; }

	public string Name { get; set; }

	public DateTime PostingDate { get; set; }

	public int RowNum { get; set; }

	public double Tax { get; set; }

	public double TaxFC { get; set; }

	public double TaxSC { get; set; }
}
