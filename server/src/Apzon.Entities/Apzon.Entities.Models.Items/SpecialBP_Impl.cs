using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Items;

public class SpecialBP_Impl
{
	public string ItemCode { get; set; }

	public string ItemName { get; set; }

	public string CardCode { get; set; }

	public string CardName { get; set; }

	public string CardType { get; set; }

	public double Price { get; set; }

	public string Currency { get; set; }

	public double Discount { get; set; }

	public int ListNum { get; set; }

	public string ListName { get; set; }

	public string AutoUpdt { get; set; }

	public string Expand { get; set; }

	public int UserSign { get; set; }

	public int SourcePrice { get; set; }

	public string Valid { get; set; }

	public DateTime ValidFrom { get; set; }

	public DateTime ValidTo { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public List<SpecialBP_LineSPP1Impl> lineSSP1 { get; set; }

	public List<SpecialBP_LineSPP2Impl> lineSSP2 { get; set; }

	public string ItmsGrpCod { get; set; }

	public string UoMName { get; set; }

	public double PrimPrice { get; set; }

	public double AddPrice1 { get; set; }

	public double AddPrice2 { get; set; }
}
