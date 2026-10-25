namespace Apzon.Entities.Models.Payments;

public class IncomingPaymentInvoicesImpl
{
	public int DocNum { get; set; }

	public int InvoiceId { get; set; }

	public int DocEntry { get; set; }

	public decimal SumApplied { get; set; }

	public decimal InvType { get; set; }

	public decimal DocRate { get; set; }

	public int DocLine { get; set; }

	public decimal vatApplied { get; set; }

	public string ObjType { get; set; }

	public decimal Dcount { get; set; }

	public decimal DcntSum { get; set; }

	public decimal BfDcntSum { get; set; }

	public decimal BfNetDcnt { get; set; }
}
