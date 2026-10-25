using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Apzon.Entities.Models.Documents;

public class PurchaseOrderLine
{
	public int DocEntry { get; set; }

	public int LineNum { get; set; }

	[DisplayName("Item No.")]
	public string ItemCode { get; set; }

	[DisplayName("Item Description")]
	public string Dscription { get; set; }

	[DisplayName("Purchase Quantity")]
	public decimal Quantity { get; set; }

	[DisplayName("Unit Price")]
	[Range(0.0, double.MaxValue, ErrorMessage = "Negative value is not allowed.")]
	public decimal PriceBefDi { get; set; }

	[DisplayName("Num In Buy")]
	[Range(0.0, double.MaxValue, ErrorMessage = "Negative value is not allowed.")]
	public decimal NumInBuy { get; set; }

	[DisplayName("Tax Group")]
	public string VatGroup { get; set; }

	[DisplayName("Tax Rate")]
	public decimal VatPrcnt { get; set; }

	[DisplayName("Delivery Date")]
	public DateTime ShipDate { get; set; }

	[DisplayName("Free Text")]
	public string FreeTxt { get; set; }

	[DisplayName("Purchase UoM")]
	public string BuyUnitMsr { get; set; }

	[DisplayName("Inventory UoM")]
	public string InvntryUom { get; set; }
}
