using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Forms.Commons.CFL;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Inventory.PriceLists;

public class frm_PeriodAndVolumeDiscountsAddItems : Form
{
	public string _codeF = "";

	public string _codeT = "";

	public string _vF = "";

	public string _vT = "";

	private IContainer components;

	private ButtonEditEx bte_ItemNoTo;

	private ButtonEditEx bte_ItemNoFrom;

	private ButtonEditEx bte_VendorTo;

	private ButtonEditEx bte_VendorFrom;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private LabelLine lb_Find;

	private LookUpEdit luk_Group;

	private LabelLine labelLine4;

	private TextEditEx textEdit1;

	private SimpleButtonEx btn_Properties;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx bt_OK;

	public frm_PeriodAndVolumeDiscountsAddItems()
	{
		InitializeComponent();
	}

	private void bte_ItemNoTo_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		ChooseItemListItem(bte_ItemNoTo);
		_codeT = Function.ToString(bte_ItemNoTo.EditValue);
	}

	private void bte_ItemNoFrom_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		ChooseItemListItem(bte_ItemNoFrom);
		_codeT = Function.ToString(bte_ItemNoFrom.EditValue);
	}

	private void bte_VendorTo_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		ChooseItemListBP(bte_VendorTo);
		_codeT = Function.ToString(bte_VendorTo.EditValue);
	}

	private void bte_VendorFrom_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		ChooseItemListBP(bte_VendorFrom);
		_codeT = Function.ToString(bte_VendorFrom.EditValue);
	}

	private void ChooseItemListBP(ButtonEdit bte)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				bte.EditValue = Function.ToString(frm_CFL2.ChooseItem["CardCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChooseItemListItem(ButtonEdit bte)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				bte.EditValue = Function.ToString(frm_CFL2.ChooseItem["ItemCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.PriceLists.frm_PeriodAndVolumeDiscountsAddItems));
		this.bte_ItemNoTo = new Apzon.Controls.Editor.ButtonEditEx();
		this.bte_ItemNoFrom = new Apzon.Controls.Editor.ButtonEditEx();
		this.bte_VendorTo = new Apzon.Controls.Editor.ButtonEditEx();
		this.bte_VendorFrom = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.lb_Find = new Apzon.Controls.Controls.LabelLine();
		this.luk_Group = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.textEdit1 = new Apzon.Controls.Editor.TextEditEx();
		this.btn_Properties = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.bte_ItemNoTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ItemNoFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_VendorTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_VendorFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Group.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit1.Properties).BeginInit();
		base.SuspendLayout();
		this.bte_ItemNoTo.Location = new System.Drawing.Point(270, 18);
		this.bte_ItemNoTo.Name = "bte_ItemNoTo";
		this.bte_ItemNoTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_ItemNoTo.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_ItemNoTo_Properties_ButtonPressed);
		this.bte_ItemNoTo.Size = new System.Drawing.Size(100, 20);
		this.bte_ItemNoTo.TabIndex = 24;
		this.bte_ItemNoFrom.Location = new System.Drawing.Point(119, 18);
		this.bte_ItemNoFrom.Name = "bte_ItemNoFrom";
		this.bte_ItemNoFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_ItemNoFrom.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_ItemNoFrom_Properties_ButtonPressed);
		this.bte_ItemNoFrom.Size = new System.Drawing.Size(100, 20);
		this.bte_ItemNoFrom.TabIndex = 23;
		this.bte_VendorTo.Location = new System.Drawing.Point(270, 41);
		this.bte_VendorTo.Name = "bte_VendorTo";
		this.bte_VendorTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_VendorTo.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_VendorTo_Properties_ButtonPressed);
		this.bte_VendorTo.Size = new System.Drawing.Size(100, 20);
		this.bte_VendorTo.TabIndex = 22;
		this.bte_VendorFrom.Location = new System.Drawing.Point(119, 41);
		this.bte_VendorFrom.Name = "bte_VendorFrom";
		this.bte_VendorFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_VendorFrom.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_VendorFrom_Properties_ButtonPressed);
		this.bte_VendorFrom.Size = new System.Drawing.Size(100, 20);
		this.bte_VendorFrom.TabIndex = 21;
		this.labelLine3.Location = new System.Drawing.Point(232, 21);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.Size = new System.Drawing.Size(42, 20);
		this.labelLine3.TabIndex = 20;
		this.labelLine3.Utext = "To";
		this.labelLine2.Location = new System.Drawing.Point(232, 44);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.Size = new System.Drawing.Size(42, 20);
		this.labelLine2.TabIndex = 19;
		this.labelLine2.Utext = "To";
		this.labelLine1.Location = new System.Drawing.Point(11, 21);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.Size = new System.Drawing.Size(110, 20);
		this.labelLine1.TabIndex = 18;
		this.labelLine1.Utext = "Code              From";
		this.lb_Find.Location = new System.Drawing.Point(11, 44);
		this.lb_Find.Name = "lb_Find";
		this.lb_Find.Size = new System.Drawing.Size(110, 20);
		this.lb_Find.TabIndex = 17;
		this.lb_Find.Utext = "Vendor           From";
		this.luk_Group.Location = new System.Drawing.Point(119, 83);
		this.luk_Group.Name = "luk_Group";
		this.luk_Group.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Group.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ITMSGRPNAM", "Name")
		});
		this.luk_Group.Properties.DisplayMember = "ITMSGRPNAM";
		this.luk_Group.Properties.NullText = "";
		this.luk_Group.Properties.ValueMember = "ITMSGRPCOD";
		this.luk_Group.Size = new System.Drawing.Size(204, 20);
		this.luk_Group.TabIndex = 26;
		this.labelLine4.Location = new System.Drawing.Point(11, 86);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.Size = new System.Drawing.Size(110, 20);
		this.labelLine4.TabIndex = 25;
		this.labelLine4.Utext = "Item Group";
		this.textEdit1.Enabled = false;
		this.textEdit1.Location = new System.Drawing.Point(118, 108);
		this.textEdit1.Name = "textEdit1";
		this.textEdit1.Properties.ReadOnly = true;
		this.textEdit1.Size = new System.Drawing.Size(100, 20);
		this.textEdit1.TabIndex = 28;
		this.btn_Properties.Enabled = false;
		this.btn_Properties.Location = new System.Drawing.Point(11, 106);
		this.btn_Properties.Name = "btn_Properties";
		this.btn_Properties.Size = new System.Drawing.Size(101, 23);
		this.btn_Properties.TabIndex = 27;
		this.btn_Properties.Text = "Properties";
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(92, 182);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 30;
		this.btn_Cancel.Text = "Cancel";
		this.bt_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_OK.Image = (System.Drawing.Image)resources.GetObject("bt_OK.Image");
		this.bt_OK.Location = new System.Drawing.Point(11, 182);
		this.bt_OK.Name = "bt_OK";
		this.bt_OK.Size = new System.Drawing.Size(75, 23);
		this.bt_OK.TabIndex = 29;
		this.bt_OK.Text = "OK";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(386, 217);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.bt_OK);
		base.Controls.Add(this.textEdit1);
		base.Controls.Add(this.btn_Properties);
		base.Controls.Add(this.luk_Group);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.bte_ItemNoTo);
		base.Controls.Add(this.bte_ItemNoFrom);
		base.Controls.Add(this.bte_VendorTo);
		base.Controls.Add(this.bte_VendorFrom);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.lb_Find);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.Name = "frm_PeriodAndVolumeDiscountsAddItems";
		base.ShowIcon = false;
		this.Text = "Special Prices - Items - Selection Criteria";
		((System.ComponentModel.ISupportInitialize)this.bte_ItemNoTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ItemNoFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_VendorTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_VendorFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Group.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit1.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
