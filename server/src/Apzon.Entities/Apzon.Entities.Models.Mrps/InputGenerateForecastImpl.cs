using System;

namespace Apzon.Entities.Models.Mrps;

public class InputGenerateForecastImpl
{
	public DateTime? FromDate { get; set; }

	public DateTime? ToDate { get; set; }

	public string FromItem { get; set; }

	public string ToItem { get; set; }

	public string ItemGroup { get; set; }

	public string WhsCode { get; set; }

	public string ViewBy { get; set; }

	public int BwLength { get; set; }

	public int FwLength { get; set; }

	public string BwBase { get; set; }

	public string FwBase { get; set; }

	public string Propeties { get; set; }

	public string CompareType { get; set; }

	public string IgnorePropeties { get; set; }

	public InputGenerateForecastImpl()
	{
		FromDate = null;
		ToDate = null;
		FromItem = "";
		ToItem = "";
		ItemGroup = "";
		WhsCode = "";
		ViewBy = "";
		BwLength = 0;
		FwLength = 0;
		BwBase = "";
		FwBase = "";
		Propeties = "";
		CompareType = "";
		IgnorePropeties = "Y";
	}
}
