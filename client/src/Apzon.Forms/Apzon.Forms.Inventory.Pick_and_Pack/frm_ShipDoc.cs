using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Interfaces.FormInventory;

namespace Apzon.Forms.Inventory.Pick_and_Pack;

public class frm_ShipDoc : VirShipping
{
	public frm_ShipDoc()
	{
		InitComponents();
	}

	public frm_ShipDoc(ApzObjectTypes fromType, int docNum)
	{
		InitComponents();
		FromType = fromType;
		FromEntry = docNum;
	}

	public frm_ShipDoc(int docNum)
	{
		InitComponents();
		DocEntry = docNum;
	}

	public void InitComponents()
	{
		base.Text = MenuCaption.ShipDoc;
		FormObjectType = ApzObjectTypes.oShipping;
		PopCopyFrom = new List<ApzObjectTypes>
		{
			ApzObjectTypes.oPickLists,
			ApzObjectTypes.oPacking
		};
		PopCopyTo = new List<ApzObjectTypes>
		{
			ApzObjectTypes.oDeliveryNotes,
			ApzObjectTypes.oInvoices
		};
		TableDoc = "APZ_SHIP";
		TableLine = "APZ_SHIP1";
	}

	protected override async Task VirShipping_Load(object sender, EventArgs e)
	{
		await base.VirShipping_Load(sender, e);
		try
		{
			lbl_DocNum.Text = "Shipping No.";
			col_DocEntry.Caption = "Shipping No.";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override async Task<DataTable> GetDoc(DataTable dt)
	{
		return await _httpClient.Shipping.GetDoc(dt);
	}

	protected override async Task<DataTable> GetLines(int docentry)
	{
		return await _httpClient.Shipping.GetLines(docentry);
	}

	protected override Task<int> GetNextId()
	{
		return _httpClient.Shipping.GetNextId();
	}

	protected override async Task<HttpResult> Close(int docentry)
	{
		return await _httpClient.Shipping.Close(docentry);
	}

	protected override async Task<HttpResult> Cancel(int docentry)
	{
		return await _httpClient.Shipping.Cancel(docentry);
	}

	protected override async Task<HttpResult> Save(DataSet ds)
	{
		return await _httpClient.Shipping.Save(ds);
	}

	private new void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.Pick_and_Pack.frm_ShipDoc));
		((System.ComponentModel.ISupportInitialize)base.splitContainerControl1).BeginInit();
		base.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)base.date_From.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.date_From.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.date_To.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.date_To.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txt_CardCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txt_DocNum.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.xtraTabControl1).BeginInit();
		base.xtraTabControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)base.look_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_DocStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_CardCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_Vehicle).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_BaseNum).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_PickNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_BatchNumber).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rep_WhsCode).BeginInit();
		base.SuspendLayout();
		base.date_From.EditValue = null;
		base.date_To.EditValue = null;
		base.tabpage_Contents.Size = new System.Drawing.Size(976, 278);
		base.drlCopyTo.Appearance.Options.UseBackColor = true;
		base.drlCopyFrom.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.ClientSize = new System.Drawing.Size(979, 568);
		base.FormId = "oShipping";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ShipDoc.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ShipDoc";
		base.Controls.SetChildIndex(base.splitContainerControl1, 0);
		base.Controls.SetChildIndex(base.labelLine1, 0);
		base.Controls.SetChildIndex(base.date_From, 0);
		base.Controls.SetChildIndex(base.labelLine2, 0);
		base.Controls.SetChildIndex(base.date_To, 0);
		base.Controls.SetChildIndex(base.labelLine3, 0);
		base.Controls.SetChildIndex(base.txt_CardCode, 0);
		base.Controls.SetChildIndex(base.lbl_DocNum, 0);
		base.Controls.SetChildIndex(base.txt_DocNum, 0);
		base.Controls.SetChildIndex(base.btn_Search, 0);
		base.Controls.SetChildIndex(base.btn_OK, 0);
		base.Controls.SetChildIndex(base.btn_Cancel, 0);
		base.Controls.SetChildIndex(base.drlCopyFrom, 0);
		base.Controls.SetChildIndex(base.drlCopyTo, 0);
		base.Controls.SetChildIndex(base.look_Status, 0);
		base.Controls.SetChildIndex(base.labelLine5, 0);
		((System.ComponentModel.ISupportInitialize)base.splitContainerControl1).EndInit();
		base.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)base.date_From.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.date_From.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.date_To.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.date_To.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txt_CardCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txt_DocNum.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.xtraTabControl1).EndInit();
		base.xtraTabControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)base.look_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_DocStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_CardCode).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_Vehicle).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_BaseNum).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_PickNo).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_BatchNumber).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rep_WhsCode).EndInit();
		base.ResumeLayout(false);
	}
}
