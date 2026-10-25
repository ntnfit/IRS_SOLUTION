using System;

namespace Apzon.Entities.Models.MasterData;

public class Items_UoMPricesImpl
{
	public string ItemCode { get; set; }

	public short PriceList { get; set; }

	public int UomEntry { get; set; }

	public double Factor { get; set; }

	public double Price { get; set; }

	public string Currency { get; set; }

	public string AutoUpdate { get; set; }

	public double AddPrice1 { get; set; }

	public string Currency1 { get; set; }

	public double AddPrice2 { get; set; }

	public string Currency2 { get; set; }

	public int LogInstanc { get; set; }

	public string ObjType { get; set; }

	public double Factor1 { get; set; }

	public double Factor2 { get; set; }

	public DateTime? UpdateDate { get; set; }
}
