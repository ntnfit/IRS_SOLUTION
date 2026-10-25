namespace Apzon.Entities.Models.Production_Addon;

public class ApzBillOfMaterialLines
{
	public string FatherCode { get; set; }

	public string CardCode { get; set; }

	public string Code { get; set; }

	public string ChildNum { get; set; }

	public decimal Quantity { get; set; }

	public string Warehouse { get; set; }

	public int PriceList { get; set; }

	public string IssueMthd { get; set; }

	public string OcrCode { get; set; }

	public string OcrCode2 { get; set; }

	public string OcrCode3 { get; set; }

	public string OcrCode4 { get; set; }

	public string OcrCode5 { get; set; }

	public decimal Scrap { get; set; }

	public string Project { get; set; }

	public string WipActCode { get; set; }

	public string Type { get; set; }

	public string Comments { get; set; }

	public string Uom { get; set; }

	public decimal Price { get; set; }

	public string Currency { get; set; }

	public string ItemName { get; set; }

	public string CardName { get; set; }
}
