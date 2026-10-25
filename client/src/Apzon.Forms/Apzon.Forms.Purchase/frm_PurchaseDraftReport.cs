using System.ComponentModel;
using Apzon.Forms.Sales;

namespace Apzon.Forms.Purchase;

public class frm_PurchaseDraftReport : frm_DocumentDraftsRP
{
	private IContainer components;

	public frm_PurchaseDraftReport()
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
