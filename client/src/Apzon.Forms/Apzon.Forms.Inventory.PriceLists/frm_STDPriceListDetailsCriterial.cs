using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.MasterData;
using Apzon.Forms.Commons.CFL;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Inventory.PriceLists;

public class frm_STDPriceListDetailsCriterial : XtraFormEx
{
	public string _GVCriterialString = "";

	private IContainer components;

	private LabelLine lb_Find;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private ButtonEditEx bte_VendorFrom;

	private ButtonEditEx bte_VendorTo;

	private ButtonEditEx bte_ItemNoFrom;

	private ButtonEditEx bte_ItemNoTo;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx bt_OK;

	private LookUpEdit luk_Group;

	private CheckEditEx chk_HUPriceItem;

	private CheckEditEx chk_HInactiveItem;

	private SimpleButtonEx btn_Properties;

	private TextEditEx textEdit1;

	public frm_STDPriceListDetailsCriterial()
	{
		InitializeComponent();
	}

	private async void frm_STDPriceListDetailsCriterial_Load(object sender, EventArgs e)
	{
		await LoadItemGroups();
	}

	private async Task LoadItemGroups()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemGroups");
			DataTable dataSource = await hp.GetDataTableAsync("GetItemGroup");
			luk_Group.Properties.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void bt_OK_Click(object sender, EventArgs e)
	{
		try
		{
			_GVCriterialString = "";
			if (!string.IsNullOrWhiteSpace(bte_VendorFrom.Text))
			{
				_GVCriterialString = _GVCriterialString + " AND [CardCode] >= '" + bte_VendorFrom.Text.Trim() + "'";
			}
			if (!string.IsNullOrWhiteSpace(bte_VendorTo.Text))
			{
				_GVCriterialString = _GVCriterialString + " AND [CardCode] <= '" + bte_VendorTo.Text.Trim() + "'";
			}
			if (!string.IsNullOrWhiteSpace(bte_ItemNoFrom.Text))
			{
				_GVCriterialString = _GVCriterialString + " AND [ItemCode] >= '" + bte_ItemNoFrom.Text.Trim() + "'";
			}
			if (!string.IsNullOrWhiteSpace(bte_ItemNoTo.Text))
			{
				_GVCriterialString = _GVCriterialString + " AND [ItemCode] <= '" + bte_ItemNoTo.Text.Trim() + "'";
			}
			if (luk_Group.EditValue != null && !string.IsNullOrWhiteSpace(luk_Group.EditValue.ToString()))
			{
				_GVCriterialString = _GVCriterialString + " AND [ITMSGRPCOD] = " + luk_Group.EditValue;
			}
			if (_GVCriterialString.Length > 5)
			{
				_GVCriterialString = _GVCriterialString.Substring(5);
			}
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_VendorFrom_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Vendor, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				BusinessPartnersImpl businessPartnersImpl = frm_ChooseFromList2.ChooseItem as BusinessPartnersImpl;
				bte_VendorFrom.Text = businessPartnersImpl.CardCode;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_VendorTo_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Vendor, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				BusinessPartnersImpl businessPartnersImpl = frm_ChooseFromList2.ChooseItem as BusinessPartnersImpl;
				bte_VendorTo.Text = businessPartnersImpl.CardCode;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_ItemNoFrom_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ListItems, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				ItemsImpl itemsImpl = frm_ChooseFromList2.ChooseItem as ItemsImpl;
				bte_ItemNoFrom.Text = itemsImpl.ItemCode;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_ItemNoTo_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ListItems, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				ItemsImpl itemsImpl = frm_ChooseFromList2.ChooseItem as ItemsImpl;
				bte_ItemNoTo.Text = itemsImpl.ItemCode;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.PriceLists.frm_STDPriceListDetailsCriterial));
		this.bte_VendorFrom = new Apzon.Controls.Editor.ButtonEditEx();
		this.bte_VendorTo = new Apzon.Controls.Editor.ButtonEditEx();
		this.bte_ItemNoFrom = new Apzon.Controls.Editor.ButtonEditEx();
		this.bte_ItemNoTo = new Apzon.Controls.Editor.ButtonEditEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.luk_Group = new DevExpress.XtraEditors.LookUpEdit();
		this.chk_HUPriceItem = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_HInactiveItem = new Apzon.Controls.Editor.CheckEditEx();
		this.btn_Properties = new Apzon.Controls.Controls.SimpleButtonEx();
		this.textEdit1 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.lb_Find = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.bte_VendorFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_VendorTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ItemNoFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ItemNoTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Group.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_HUPriceItem.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_HInactiveItem.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit1.Properties).BeginInit();
		base.SuspendLayout();
		this.bte_VendorFrom.FieldName = null;
		this.bte_VendorFrom.IsSearchControl = false;
		this.bte_VendorFrom.Location = new System.Drawing.Point(120, 9);
		this.bte_VendorFrom.Name = "bte_VendorFrom";
		this.bte_VendorFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_VendorFrom.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_VendorFrom_Properties_ButtonPressed);
		this.bte_VendorFrom.Size = new System.Drawing.Size(100, 20);
		this.bte_VendorFrom.TabIndex = 13;
		this.bte_VendorTo.FieldName = null;
		this.bte_VendorTo.IsSearchControl = false;
		this.bte_VendorTo.Location = new System.Drawing.Point(271, 9);
		this.bte_VendorTo.Name = "bte_VendorTo";
		this.bte_VendorTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_VendorTo.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_VendorTo_Properties_ButtonPressed);
		this.bte_VendorTo.Size = new System.Drawing.Size(100, 20);
		this.bte_VendorTo.TabIndex = 14;
		this.bte_ItemNoFrom.FieldName = null;
		this.bte_ItemNoFrom.IsSearchControl = false;
		this.bte_ItemNoFrom.Location = new System.Drawing.Point(120, 43);
		this.bte_ItemNoFrom.Name = "bte_ItemNoFrom";
		this.bte_ItemNoFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_ItemNoFrom.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_ItemNoFrom_Properties_ButtonPressed);
		this.bte_ItemNoFrom.Size = new System.Drawing.Size(100, 20);
		this.bte_ItemNoFrom.TabIndex = 15;
		this.bte_ItemNoTo.FieldName = null;
		this.bte_ItemNoTo.IsSearchControl = false;
		this.bte_ItemNoTo.Location = new System.Drawing.Point(271, 43);
		this.bte_ItemNoTo.Name = "bte_ItemNoTo";
		this.bte_ItemNoTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_ItemNoTo.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_ItemNoTo_Properties_ButtonPressed);
		this.bte_ItemNoTo.Size = new System.Drawing.Size(100, 20);
		this.bte_ItemNoTo.TabIndex = 16;
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(92, 207);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 18;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.bt_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_OK.FieldName = null;
		this.bt_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_OK.ImageOptions.Image");
		this.bt_OK.IsSearchControl = false;
		this.bt_OK.Location = new System.Drawing.Point(13, 207);
		this.bt_OK.Name = "bt_OK";
		this.bt_OK.Size = new System.Drawing.Size(75, 23);
		this.bt_OK.TabIndex = 17;
		this.bt_OK.Text = "OK";
		this.bt_OK.Click += new System.EventHandler(this.bt_OK_Click);
		this.luk_Group.Location = new System.Drawing.Point(120, 78);
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
		this.luk_Group.Size = new System.Drawing.Size(100, 20);
		this.luk_Group.TabIndex = 19;
		this.chk_HUPriceItem.Enabled = false;
		this.chk_HUPriceItem.FieldName = null;
		this.chk_HUPriceItem.IsSearchControl = false;
		this.chk_HUPriceItem.Location = new System.Drawing.Point(12, 150);
		this.chk_HUPriceItem.Name = "chk_HUPriceItem";
		this.chk_HUPriceItem.Properties.Caption = "Hide Unpriced Items";
		this.chk_HUPriceItem.Size = new System.Drawing.Size(127, 20);
		this.chk_HUPriceItem.TabIndex = 20;
		this.chk_HInactiveItem.Enabled = false;
		this.chk_HInactiveItem.FieldName = null;
		this.chk_HInactiveItem.IsSearchControl = false;
		this.chk_HInactiveItem.Location = new System.Drawing.Point(12, 175);
		this.chk_HInactiveItem.Name = "chk_HInactiveItem";
		this.chk_HInactiveItem.Properties.Caption = "Hide Inactive Items";
		this.chk_HInactiveItem.Size = new System.Drawing.Size(127, 20);
		this.chk_HInactiveItem.TabIndex = 21;
		this.btn_Properties.Enabled = false;
		this.btn_Properties.FieldName = null;
		this.btn_Properties.IsSearchControl = false;
		this.btn_Properties.Location = new System.Drawing.Point(13, 114);
		this.btn_Properties.Name = "btn_Properties";
		this.btn_Properties.Size = new System.Drawing.Size(101, 23);
		this.btn_Properties.TabIndex = 23;
		this.btn_Properties.Text = "Item Properties";
		this.textEdit1.Enabled = false;
		this.textEdit1.FieldName = null;
		this.textEdit1.IsSearchControl = false;
		this.textEdit1.Location = new System.Drawing.Point(120, 116);
		this.textEdit1.Name = "textEdit1";
		this.textEdit1.Properties.ReadOnly = true;
		this.textEdit1.Size = new System.Drawing.Size(100, 20);
		this.textEdit1.TabIndex = 24;
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(12, 81);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(110, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 12;
		this.labelLine4.Utext = "Group";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(233, 46);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(42, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 11;
		this.labelLine3.Utext = "To";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(233, 12);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(42, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 10;
		this.labelLine2.Utext = "To";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 46);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(110, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 9;
		this.labelLine1.Utext = "Item No. From";
		this.lb_Find.FieldName = null;
		this.lb_Find.HideWhenEmpty = false;
		this.lb_Find.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Find.Location = new System.Drawing.Point(12, 12);
		this.lb_Find.Name = "lb_Find";
		this.lb_Find.ShowUnderLine = true;
		this.lb_Find.Size = new System.Drawing.Size(110, 20);
		this.lb_Find.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Find.TabIndex = 8;
		this.lb_Find.Utext = "Vendor Code From";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(381, 242);
		base.Controls.Add(this.textEdit1);
		base.Controls.Add(this.btn_Properties);
		base.Controls.Add(this.chk_HInactiveItem);
		base.Controls.Add(this.chk_HUPriceItem);
		base.Controls.Add(this.luk_Group);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.bt_OK);
		base.Controls.Add(this.bte_ItemNoTo);
		base.Controls.Add(this.bte_ItemNoFrom);
		base.Controls.Add(this.bte_VendorTo);
		base.Controls.Add(this.bte_VendorFrom);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.lb_Find);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_STDPriceListDetailsCriterial";
		this.Text = "Price List - Select Criterial";
		base.Load += new System.EventHandler(this.frm_STDPriceListDetailsCriterial_Load);
		((System.ComponentModel.ISupportInitialize)this.bte_VendorFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_VendorTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ItemNoFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ItemNoTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Group.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_HUPriceItem.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_HInactiveItem.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit1.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
