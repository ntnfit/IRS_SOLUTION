namespace Apzon.Entities.Models.Inventory;

public class FIFOLayersImpl
{
	public int BaseLine { get; set; }

	public int Count { get; set; }

	public int LayerID { get; set; }

	public double LineTotal { get; set; }

	public double Price { get; set; }

	public double Quantity { get; set; }

	public int TransactionSequenceNum { get; set; }
}
