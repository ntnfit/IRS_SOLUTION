using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Inventory;

public class MaterialRevaluation_linesImpl
{
	public double ActualPrice { get; set; }

	public int Count { get; set; }

	public double DebitCredit { get; set; }

	public string DistributionRule { get; set; }

	public string DistributionRule2 { get; set; }

	public string DistributionRule3 { get; set; }

	public string DistributionRule4 { get; set; }

	public string DistributionRule5 { get; set; }

	public int DocEntry { get; set; }

	public List<FIFOLayersImpl> FIFOLayers { get; set; }

	public string ItemCode { get; set; }

	public string ItemDescription { get; set; }

	public int LineNum { get; set; }

	public double OnHand { get; set; }

	public double Price { get; set; }

	public string Project { get; set; }

	public double Quantity { get; set; }

	public double RevalAmountToStock { get; set; }

	public string RevaluationDecrementAccount { get; set; }

	public string RevaluationIncrementAccount { get; set; }

	public List<SNBLinesImpl> SNBLines { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public string WarehouseCode { get; set; }
}
