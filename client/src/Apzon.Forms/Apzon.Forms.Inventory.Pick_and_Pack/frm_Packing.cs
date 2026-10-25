using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Apzon.Commons;
using Apzon.Commons.Properties;
using Apzon.Forms.Interfaces.FormInventory;

namespace Apzon.Forms.Inventory.Pick_and_Pack;

public class frm_Packing : VirShipping
{
	public frm_Packing()
	{
		base.Text = MenuCaption.Packing;
		FormObjectType = ApzObjectTypes.oPacking;
		PopCopyFrom = new List<ApzObjectTypes>
		{
			ApzObjectTypes.oPickLists,
			ApzObjectTypes.oPacking
		};
		PopCopyTo = new List<ApzObjectTypes>
		{
			ApzObjectTypes.oPacking,
			ApzObjectTypes.oShipping,
			ApzObjectTypes.oDeliveryNotes,
			ApzObjectTypes.oInvoices
		};
		TableDoc = "APZ_OPAK";
		TableLine = "APZ_PAK1";
	}

	private new void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.Pick_and_Pack.frm_Packing));
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
		base.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		base.tabpage_Contents.Size = new System.Drawing.Size(976, 278);
		base.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		base.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		base.drlCopyTo.Appearance.Options.UseBackColor = true;
		base.drlCopyTo.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("drlCopyTo.ImageOptions.Image");
		base.drlCopyFrom.Appearance.Options.UseBackColor = true;
		base.drlCopyFrom.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("drlCopyFrom.ImageOptions.Image");
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.ClientSize = new System.Drawing.Size(979, 568);
		base.FormId = "oNone";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_Packing.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Packing";
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
