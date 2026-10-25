namespace Apzon.Entities.Models.Inventory;

public class InventoryRevaluation_LineMRV1Impl
{
	public int DocEntry { get; set; }

	public int LineNum { get; set; }

	public string ItemCode { get; set; }

	public string Dscription { get; set; }

	public double Quantity { get; set; }

	public double Price { get; set; }

	public double LineTotal { get; set; }

	public string WhsCode { get; set; }

	public string RIncmAcct { get; set; }

	public string RDcrmAcct { get; set; }

	public double RToStock { get; set; }

	public double RActPrice { get; set; }

	public double ROnHand { get; set; }

	public int LogInstanc { get; set; }

	public string ObjType { get; set; }

	public string EvalSystem { get; set; }

	public string OcrCode { get; set; }

	public string UnitMsr { get; set; }

	public string OcrCode2 { get; set; }

	public string OcrCode3 { get; set; }

	public string OcrCode4 { get; set; }

	public string OcrCode5 { get; set; }
}
