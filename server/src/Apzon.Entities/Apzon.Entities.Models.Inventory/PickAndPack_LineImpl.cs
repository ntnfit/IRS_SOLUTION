namespace Apzon.Entities.Models.Inventory;

public class PickAndPack_LineImpl
{
	public int DocEntry { get; set; }

	public int LineId { get; set; }

	public int VisOrder { get; set; }

	public string Object { get; set; }

	public int LogInst { get; set; }

	public string U_DistNumber { get; set; }

	public decimal U_Width { get; set; }

	public decimal U_Length { get; set; }

	public decimal U_QtyUoM { get; set; }

	public string U_UnitUoM { get; set; }

	public decimal U_Quantity { get; set; }

	public string U_WhsCode { get; set; }

	public string U_SONo { get; set; }

	public int U_SOLine { get; set; }
}
