using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.MasterData;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Inventory.InventoryReport;

public class frm_ItemSummary : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public Dictionary<string, string> DicDisplay = new Dictionary<string, string>
	{
		{ "%", "All" },
		{ "Y", "Items have quantity in stock" }
	};

	public Dictionary<string, string> DicNumberDecimalDigits = new Dictionary<string, string>
	{
		{ "0", "0" },
		{ "1", "1" },
		{ "2", "2" },
		{ "3", "3" },
		{ "4", "4" },
		{ "5", "5" },
		{ "6", "6" }
	};

	public List<Tuple<GridViewEx, string>> FormItem = new List<Tuple<GridViewEx, string>>();

	public new string FormId = "ItemSummery";

	private IContainer components;

	private LabelLine labelLine1;

	private DateEditEx date_From;

	private DateEditEx date_To;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private CheckedComboBoxEdit chkcb_Items;

	private CheckedComboBoxEdit chkcb_Warehouse;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private LookUpEdit look_Display;

	private LookUpEdit look_NumberDecimalDigits;

	private LabelLine labelLine6;

	private SimpleButtonEx btn_Load;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	public frm_ItemSummary()
	{
		InitializeComponent();
	}

	private async void frm_ItemSummary_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			this.ResetData();
			FormItem.Add(new Tuple<GridViewEx, string>(gv_Doc, "ITMSUM"));
			look_Display.SetDataSource(DicDisplay);
			look_NumberDecimalDigits.SetDataSource(DicNumberDecimalDigits);
			DataSet dataSet = await _httpClient.Items.Cfl("", 1.ToString(), GlobalData.UserName, GlobalData.DefaultBranch.ToString());
			if (dataSet.IsHasStructure())
			{
				DataTable dataSource = dataSet.Tables[1];
				chkcb_Items.Properties.DataSource = dataSource;
				chkcb_Items.Properties.DisplayMember = "ItemCode";
				chkcb_Items.Properties.ValueMember = "ItemCode";
				chkcb_Items.SelectAll();
			}
			using (HttpClientHelper<WarehouseImpl> helper = new HttpClientHelper<WarehouseImpl>(Configuration.ApiServer, "Warehouse"))
			{
				List<WarehouseImpl> dataSource2 = await helper.GetListAsync("Cfl");
				chkcb_Warehouse.Properties.DataSource = dataSource2;
				chkcb_Warehouse.Properties.DisplayMember = "WarehouseCode";
				chkcb_Warehouse.Properties.ValueMember = "WarehouseCode";
				chkcb_Warehouse.SelectAll();
			}
			look_Display.EditValue = "%";
			look_NumberDecimalDigits.EditValue = "0";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Load_Click(object sender, EventArgs e)
	{
		try
		{
			DateTime dateTime = date_From.DateTime;
			DateTime dateTime2 = date_To.DateTime;
			string item = Function.ToString(chkcb_Items.EditValue);
			string whs = Function.ToString(chkcb_Warehouse.EditValue);
			string display = Function.ToString(look_Display.EditValue);
			string num = Function.ToString(look_NumberDecimalDigits.EditValue);
			gv_Doc.ShowLoadingPanel();
			DataTable dataSource = await _httpClient.Items.ReportItemSummary(dateTime, dateTime2, item, whs, display, num);
			grd_Doc.DataSource = dataSource;
			grd_Doc.RefreshDataSource();
			gv_Doc.PopulateColumns();
			gv_Doc.OptionsView.ColumnAutoWidth = false;
			foreach (GridColumn column in gv_Doc.Columns)
			{
				column.Width = 100;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Doc.HideLoadingPanel();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.InventoryReport.frm_ItemSummary));
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.date_From = new Apzon.Controls.Editor.DateEditEx();
		this.date_To = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.chkcb_Items = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.chkcb_Warehouse = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.look_Display = new DevExpress.XtraEditors.LookUpEdit();
		this.look_NumberDecimalDigits = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Load = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkcb_Items.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkcb_Warehouse.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Display.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_NumberDecimalDigits.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		base.SuspendLayout();
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "From Date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 13);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "From Date";
		this.date_From.EditControl = null;
		this.date_From.EditValue = null;
		this.date_From.FieldName = null;
		this.date_From.Location = new System.Drawing.Point(91, 10);
		this.date_From.Name = "date_From";
		this.date_From.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_From.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_From.Properties.IsSearchControl = false;
		this.date_From.SettingInVisible = false;
		this.date_From.SettingReadonly = false;
		this.date_From.Size = new System.Drawing.Size(100, 20);
		this.date_From.TabIndex = 1;
		this.date_To.EditControl = null;
		this.date_To.EditValue = null;
		this.date_To.FieldName = null;
		this.date_To.Location = new System.Drawing.Point(91, 36);
		this.date_To.Name = "date_To";
		this.date_To.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_To.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_To.Properties.IsSearchControl = false;
		this.date_To.SettingInVisible = false;
		this.date_To.SettingReadonly = false;
		this.date_To.Size = new System.Drawing.Size(100, 20);
		this.date_To.TabIndex = 3;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "To Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(12, 39);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 2;
		this.labelLine2.Utext = "To Date";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Items";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(217, 13);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(121, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 4;
		this.labelLine3.Utext = "Items";
		this.chkcb_Items.Location = new System.Drawing.Point(337, 10);
		this.chkcb_Items.Name = "chkcb_Items";
		this.chkcb_Items.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.chkcb_Items.Size = new System.Drawing.Size(362, 20);
		this.chkcb_Items.TabIndex = 5;
		this.chkcb_Warehouse.Location = new System.Drawing.Point(337, 36);
		this.chkcb_Warehouse.Name = "chkcb_Warehouse";
		this.chkcb_Warehouse.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.chkcb_Warehouse.Size = new System.Drawing.Size(362, 20);
		this.chkcb_Warehouse.TabIndex = 7;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Warehouse";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(217, 39);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(121, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 6;
		this.labelLine4.Utext = "Warehouse";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Display";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(13, 66);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 8;
		this.labelLine5.Utext = "Display";
		this.look_Display.Location = new System.Drawing.Point(91, 63);
		this.look_Display.Name = "look_Display";
		this.look_Display.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Display.Properties.NullText = "";
		this.look_Display.Size = new System.Drawing.Size(100, 20);
		this.look_Display.TabIndex = 9;
		this.look_NumberDecimalDigits.Location = new System.Drawing.Point(337, 63);
		this.look_NumberDecimalDigits.Name = "look_NumberDecimalDigits";
		this.look_NumberDecimalDigits.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_NumberDecimalDigits.Properties.NullText = "";
		this.look_NumberDecimalDigits.Size = new System.Drawing.Size(100, 20);
		this.look_NumberDecimalDigits.TabIndex = 11;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Number Decimal Digits";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(217, 66);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(121, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 10;
		this.labelLine6.Utext = "Number Decimal Digits";
		this.btn_Load.EditControl = null;
		this.btn_Load.EditValue = "Load";
		this.btn_Load.FieldName = null;
		this.btn_Load.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Load.ImageOptions.Image");
		this.btn_Load.IsSearchControl = false;
		this.btn_Load.Location = new System.Drawing.Point(624, 63);
		this.btn_Load.Name = "btn_Load";
		this.btn_Load.ReadOnly = false;
		this.btn_Load.SettingInVisible = false;
		this.btn_Load.SettingReadonly = false;
		this.btn_Load.Size = new System.Drawing.Size(75, 23);
		this.btn_Load.TabIndex = 12;
		this.btn_Load.Text = "Load";
		this.btn_Load.Click += new System.EventHandler(this.btn_Load_Click);
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(3, 93);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(1060, 457);
		this.grd_Doc.TabIndex = 13;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = null;
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = null;
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.UseBatchMod = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1063, 553);
		base.Controls.Add(this.grd_Doc);
		base.Controls.Add(this.btn_Load);
		base.Controls.Add(this.look_NumberDecimalDigits);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.look_Display);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.chkcb_Warehouse);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.chkcb_Items);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.date_To);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.date_From);
		base.Controls.Add(this.labelLine1);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ItemSummary.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ItemSummary";
		base.ShowInTaskbar = false;
		this.Text = "Item Summary";
		base.Load += new System.EventHandler(this.frm_ItemSummary_Load);
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkcb_Items.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkcb_Warehouse.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Display.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_NumberDecimalDigits.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		base.ResumeLayout(false);
	}
}
