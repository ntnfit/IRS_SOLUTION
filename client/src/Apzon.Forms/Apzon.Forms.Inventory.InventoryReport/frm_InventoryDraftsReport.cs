using System.ComponentModel;
using Apzon.Forms.Sales;

namespace Apzon.Forms.Inventory.InventoryReport;

public class frm_InventoryDraftsReport : frm_DocumentDraftsRP
{
	private IContainer components;

	public frm_InventoryDraftsReport()
	{
		InitializeComponent();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
	}
}
