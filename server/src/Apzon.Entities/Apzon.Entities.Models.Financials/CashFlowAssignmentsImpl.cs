using SAPbobsCOM;

namespace Apzon.Entities.Models.Financials;

public class CashFlowAssignmentsImpl
{
	public double AmountFC { get; set; }

	public double AmountLC { get; set; }

	public int CashFlowAssignmentsID { get; set; }

	public int CashFlowLineItemID { get; set; }

	public string CheckNumber { get; set; }

	public int Count { get; set; }

	public int JDTLineId { get; set; }

	public PaymentMeansTypeEnum PaymentMeans { get; set; }
}
