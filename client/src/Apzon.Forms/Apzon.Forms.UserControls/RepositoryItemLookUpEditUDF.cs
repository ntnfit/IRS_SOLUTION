using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;

namespace Apzon.Forms.UserControls;

[UserRepositoryItem("RepositoryItemLookUpEditUDF")]
public class RepositoryItemLookUpEditUDF : RepositoryItemLookUpEdit
{
	public const string CustomEditName = "LookUpEditUDF";

	public override string EditorTypeName => "LookUpEditUDF";

	static RepositoryItemLookUpEditUDF()
	{
		RegisterLookUpEditUDF();
	}

	public RepositoryItemLookUpEditUDF()
	{
		Buttons.Clear();
		Buttons.Add(new EditorButton(ButtonPredefines.Combo));
		NullText = "";
		ShowHeader = false;
		ShowFooter = false;
		BestFitMode = BestFitMode.BestFit;
		Popup += luk_Popup;
		AutoHeight = false;
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("ID", typeof(int));
		dataTable.Columns.Add("Name", typeof(string));
		dataTable.Rows.Add(-1, "");
		dataTable.Rows.Add(23, "Sales Quotations");
		dataTable.Rows.Add(17, "Sales Orders");
		dataTable.Rows.Add(15, "Deliveries");
		dataTable.Rows.Add(16, "Returns");
		dataTable.Rows.Add(13, "A/R Invoices");
		dataTable.Rows.Add(14, "A/R Credit Memos");
		dataTable.Rows.Add(203, "A/R Down Payment");
		dataTable.Rows.Add(1470000113, "Purchase Request");
		dataTable.Rows.Add(540000006, "Purchase Quotation");
		dataTable.Rows.Add(22, "Purchase Orders");
		dataTable.Rows.Add(20, "Goods Receipt PO");
		dataTable.Rows.Add(21, "Goods Return");
		dataTable.Rows.Add(18, "A/P Invoices");
		dataTable.Rows.Add(19, "A/P Credit Memos");
		dataTable.Rows.Add(48, "Landed Costs");
		dataTable.Rows.Add(204, "A/P Down Payment");
		dataTable.Rows.Add(24, "Incoming Payments");
		dataTable.Rows.Add(46, "Outgoing Payments");
		dataTable.Rows.Add(1250000001, "Inventory Transfer Request");
		dataTable.Rows.Add(67, "Inventory Transfer");
		dataTable.Rows.Add(60, "Goods Issue");
		dataTable.Rows.Add(59, "Goods Receipt");
		dataTable.Rows.Add(162, "Inventory Revaluation");
		dataTable.Rows.Add(202, "Production Orders");
		dataTable.Rows.Add(1250000025, "Blanket Agreements");
		dataTable.Rows.Add(ApzObjectTypes.oTrackingIssue, "Tracking Issue");
		dataTable.Rows.Add(ApzObjectTypes.oServiceCalls, "Service Call");
		dataTable.Rows.Add(ApzObjectTypes.oMarketingCampaign, "Marketing Campaign");
		dataTable.AcceptChanges();
		DataSource = dataTable;
		DisplayMember = "Name";
		ValueMember = "ID";
	}

	private void luk_Popup(object sender, EventArgs e)
	{
		try
		{
			LookUpEdit lookUpEdit = sender as LookUpEdit;
			PopupLookUpEditForm popupLookUpEditForm = ((IPopupControl)lookUpEdit).PopupWindow as PopupLookUpEditForm;
			_ = popupLookUpEditForm.Width;
			lookUpEdit.Properties.BestFit();
			if (lookUpEdit.Properties.Columns.Count > 0)
			{
				float num = 0f;
				if (popupLookUpEditForm.Filter.RowCount > 0)
				{
					for (int i = 0; i < lookUpEdit.Properties.Columns.Count; i++)
					{
						if (lookUpEdit.Properties.Columns[i].Visible)
						{
							num += (float)(lookUpEdit.Properties.Columns[i].Width + 18);
						}
					}
					int num2 = Function.ParseInt(Math.Round(num, 0).ToString());
					if (num2 > lookUpEdit.Width)
					{
						popupLookUpEditForm.Width = num2;
					}
					else
					{
						popupLookUpEditForm.Width = lookUpEdit.Width;
					}
				}
			}
			else
			{
				popupLookUpEditForm.Width = lookUpEdit.Width;
			}
			if (popupLookUpEditForm.PointToScreen(Point.Empty).X < lookUpEdit.PointToScreen(Point.Empty).X)
			{
				popupLookUpEditForm.Location = new Point(lookUpEdit.PointToScreen(Point.Empty).X, popupLookUpEditForm.PointToScreen(Point.Empty).Y);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public static void RegisterLookUpEditUDF()
	{
		Image image = null;
		EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("LookUpEditUDF", typeof(LookUpEditUDF), typeof(RepositoryItemLookUpEditUDF), typeof(LookUpEditUDFViewInfo), new LookUpEditUDFPainter(), designTimeVisible: true, image));
	}

	public override void Assign(RepositoryItem item)
	{
		BeginUpdate();
		try
		{
			base.Assign(item);
			_ = item is RepositoryItemLookUpEditUDF;
		}
		finally
		{
			EndUpdate();
		}
	}
}
