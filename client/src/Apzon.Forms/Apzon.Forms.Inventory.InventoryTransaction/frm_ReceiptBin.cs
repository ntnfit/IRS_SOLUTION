using Apzon.Commons;
using Apzon.Forms.Interfaces.FormInventory;

namespace Apzon.Forms.Inventory.InventoryTransaction;

public class frm_ReceiptBin : VirReceiptIssueBin
{
	public frm_ReceiptBin()
	{
		InitComponents();
	}

	public void InitComponents()
	{
		FormObjectType = ApzObjectTypes.oInventoryGenEntry;
		base.Text = "Goods Receipt - Bin";
	}
}
