using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Items;

public class SpecialBP_LineSPP1Impl
{
	public string ItemCode { get; set; }

	public string CardCode { get; set; }

	public int LINENUM { get; set; }

	public double Price { get; set; }

	public string Currency { get; set; }

	public double Discount { get; set; }

	public int ListNum { get; set; }

	public DateTime? FromDate { get; set; }

	public DateTime? ToDate { get; set; }

	public string AutoUpdt { get; set; }

	public string Expand { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
