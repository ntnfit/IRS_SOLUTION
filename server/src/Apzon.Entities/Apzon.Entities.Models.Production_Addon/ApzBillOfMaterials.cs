using System;

namespace Apzon.Entities.Models.Production_Addon;

public class ApzBillOfMaterials
{
	public string Code { get; set; }

	public string Status { get; set; }

	public DateTime? ValidFrom { get; set; }

	public DateTime? ValidTo { get; set; }

	public string CardCode { get; set; }

	public string TreeType { get; set; }

	public string OcrCode { get; set; }

	public string OcrCode2 { get; set; }

	public string OcrCode3 { get; set; }

	public string OcrCode4 { get; set; }

	public string OcrCode5 { get; set; }

	public string ToWH { get; set; }

	public string Project { get; set; }

	public DateTime CreateDate { get; set; }

	public int PriceList { get; set; }

	public decimal Quantity { get; set; }

	public string Remark { get; set; }

	public decimal Scrap { get; set; }

	public string ItemName { get; set; }

	public string CardName { get; set; }

	public ApzBillOfMaterials()
	{
		Code = "";
		ItemName = "";
		CardCode = "";
		CardName = "";
		Status = "";
		ValidFrom = DateTime.Today;
		ValidTo = DateTime.Today;
		TreeType = "";
		OcrCode = "";
		OcrCode2 = "";
		OcrCode3 = "";
		OcrCode4 = "";
		OcrCode5 = "";
		ToWH = "";
		Project = "";
		CreateDate = DateTime.Today;
		PriceList = 0;
		Quantity = 1m;
		Remark = "";
		Scrap = 0m;
	}
}
