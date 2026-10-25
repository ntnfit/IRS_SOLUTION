using System.ComponentModel;

namespace Apzon.Forms.Sales;

public class frm_SalesDraftsReport : frm_DocumentDraftsRP
{
	private IContainer components;

	public frm_SalesDraftsReport()
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
