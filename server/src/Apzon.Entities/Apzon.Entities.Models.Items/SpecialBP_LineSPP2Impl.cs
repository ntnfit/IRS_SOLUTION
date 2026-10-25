using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Items;

public class SpecialBP_LineSPP2Impl
{
	public string ItemCode { get; set; }

	public string CardCode { get; set; }

	public int LINENUM { get; set; }

	public int SPP2LNum { get; set; }

	public double? Amount { get; set; }

	public double? Discount { get; set; }

	public double? Price { get; set; }

	public string Currency { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
