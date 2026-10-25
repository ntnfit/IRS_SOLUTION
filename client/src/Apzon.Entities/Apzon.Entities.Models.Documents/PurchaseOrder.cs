using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Apzon.Entities.Models.Documents;

public class PurchaseOrder
{
	public int DocEntry { get; set; }

	public string DocType { get; set; }

	[DisplayName("Document Type")]
	public string DocTypeName { get; set; }

	[Required]
	[DisplayName("Vendor No.")]
	public string CardCode { get; set; }

	[DisplayName("Vendor Name")]
	public string CardName { get; set; }

	[DisplayName("Posting Date")]
	public DateTime DocDate { get; set; }

	[DisplayName("Currency")]
	public string DocCur { get; set; }

	[DisplayName("Exchange Rate")]
	[Range(0, int.MaxValue, ErrorMessage = "Negative value is not allowed.")]
	public double DocRate { get; set; }

	[DisplayName("Federal Tax ID")]
	public string LicTradNum { get; set; }

	public int BPLId { get; set; }

	[DisplayName("Branch")]
	public string BranchName { get; set; }

	[DisplayName("Due Date")]
	public DateTime DocDueDate { get; set; }

	[DisplayName("Document Date")]
	public DateTime TaxDate { get; set; }

	[DisplayName("JV Number")]
	public string JVNumber { get; set; }

	[DisplayName("Bill To")]
	public string Address { get; set; }

	[DisplayName("Ship To")]
	public string Address2 { get; set; }

	[DisplayName("Remarks")]
	public string Comments { get; set; }

	[DisplayName("Total Tax")]
	public double VatSum { get; set; }

	[DisplayName("Total")]
	public double DocTotal { get; set; }

	[DisplayName("Authorization Status")]
	public string WddStatus { get; set; }
}
