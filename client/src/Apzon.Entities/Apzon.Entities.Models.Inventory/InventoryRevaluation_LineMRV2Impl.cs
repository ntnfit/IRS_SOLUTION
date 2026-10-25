using System;

namespace Apzon.Entities.Models.Inventory;

public class InventoryRevaluation_LineMRV2Impl
{
	public int DocEntry { get; set; }

	public int BaseLine { get; set; }

	public int LineNum { get; set; }

	public double Quantity { get; set; }

	public double Price { get; set; }

	public double LineTotal { get; set; }

	public double RToStock { get; set; }

	public double RActPrice { get; set; }

	public int INMInst { get; set; }

	public int INMTransTy { get; set; }

	public string INMCreatBy { get; set; }

	public string INMBaseRef { get; set; }

	public DateTime INMDocDate { get; set; }

	public double INMOpenQty { get; set; }

	public string ObjType { get; set; }

	public int LogInstanc { get; set; }

	public int IVLTransSe { get; set; }

	public int IVLLayerID { get; set; }

	public int INMLineNum { get; set; }

	public int INMSubLine { get; set; }
}
