using System.Data;

namespace Apzon.Entities.Models;

public static class CtxTableHelper
{
	public static DataTable PurchaseLandedCosts { get; set; }

	public static DataTable PurchaseLandedCosts_Costs { get; set; }

	public static DataTable PurchaseLandedCosts_Customs { get; set; }

	public static DataTable PurchaseLandedCosts_Rows { get; set; }
}
