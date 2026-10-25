using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons.Enums;
using Apzon.Forms.Interfaces.Mrp;
using DevExpress.Utils.Menu;

namespace Apzon.Forms.Mrps;

public class frm_GenPurchaseFromSchedule : VirGenPurchase
{
	public frm_GenPurchaseFromSchedule()
	{
		InitComponents();
	}

	public void InitComponents()
	{
		Controller = "Schedule";
		Text = "Purchase From Schedule";
		FormMode = Enums.FormViewModeType.View;
		string formId = (gvDoc.FormId = "GenPurchaseFromSchedule");
		FormId = formId;
		gvDoc.ItemId = "Lines";
	}

	private void InitializeComponent()
	{
		DevExpress.Utils.Menu.DXPopupMenu dXPopupMenu = new DevExpress.Utils.Menu.DXPopupMenu();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_GenPurchaseFromSchedule));
		((System.ComponentModel.ISupportInitialize)base.txtCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtInput.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rWhs).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rBran).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rSeri).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.rCarCode).BeginInit();
		base.SuspendLayout();
		base.txtCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtCode.Properties.AppearanceFocused.Options.UseBackColor = true;
		dXPopupMenu.AllowToolbarTransparency = false;
		dXPopupMenu.BeginGroup = false;
		dXPopupMenu.Caption = "";
		dXPopupMenu.Enabled = true;
		dXPopupMenu.Image = null;
		dXPopupMenu.ImageDisabled = null;
		dXPopupMenu.IsDesignTimeMenu = false;
		dXPopupMenu.IsRightToLeft = false;
		dXPopupMenu.LargeImage = null;
		dXPopupMenu.LargeImageDisabled = null;
		dXPopupMenu.Owner = null;
		dXPopupMenu.OwnerPopup = null;
		dXPopupMenu.Priority = DevExpress.Utils.Menu.DXMenuItemPriority.Normal;
		dXPopupMenu.Shortcut = System.Windows.Forms.Shortcut.None;
		dXPopupMenu.ShowItemToolTips = false;
		dXPopupMenu.SuperTip = null;
		dXPopupMenu.SvgImage = null;
		dXPopupMenu.Tag = null;
		dXPopupMenu.ToolbarHideDistance = -1;
		dXPopupMenu.ToolbarRowCount = 2;
		dXPopupMenu.Visible = true;
		base.btnSave.DropDownControl = dXPopupMenu;
		base.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		base.txtInput.EditValue = "S";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.ClientSize = new System.Drawing.Size(506, 404);
		base.Name = "frm_GenPurchaseFromSchedule";
		((System.ComponentModel.ISupportInitialize)base.txtCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtInput.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rWhs).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rBran).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rSeri).EndInit();
		((System.ComponentModel.ISupportInitialize)base.rCarCode).EndInit();
		base.ResumeLayout(false);
	}
}
