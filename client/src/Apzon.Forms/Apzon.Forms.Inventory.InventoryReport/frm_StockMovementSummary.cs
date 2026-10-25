using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Inventory.InventoryReport;

public class frm_StockMovementSummary : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GroupControl grp_Control;

	private XtraScrollableControl xtr_Control;

	private GridControlEx grd_Data;

	private GridViewEx gv_Data;

	private LookUpEdit lue_Branch;

	private LabelLine labelLine6;

	private LabelLine lbl_Branch;

	private DateEditEx date_To;

	private DateEditEx date_From;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private LabelLine labelLine1;

	private CheckedComboBoxEdit chk_Whse;

	private LabelLine labelLine2;

	private LookUpEdit lue_Filter;

	private SimpleButtonEx btn_Run;

	private ButtonEditEx txt_ItemCode;

	public frm_StockMovementSummary()
	{
		InitializeComponent();
	}

	private async void frm_StockMovementSummary_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			RepositoryItemCheckedComboBoxEdit properties = chk_Whse.Properties;
			properties.DataSource = await _httpClient.Warehouse.GetAllCode();
			chk_Whse.Properties.DisplayMember = "WhsCode";
			chk_Whse.Properties.ValueMember = "WhsCode";
			chk_Whse.EditValue = "";
			DataTable dataTable = await _httpClient.Commons.GetBranchList(GlobalData.UserName);
			if (dataTable.IsNotNull())
			{
				for (int i = 2; i < dataTable.Columns.Count; i++)
				{
					dataTable.Columns.Remove(dataTable.Columns[2]);
				}
			}
			lue_Branch.Properties.DataSource = dataTable;
			lue_Branch.Properties.DisplayMember = "BPLName";
			lue_Branch.Properties.ValueMember = "BPLId";
			lue_Branch.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			List<Common.Table_ID> list = new List<Common.Table_ID>();
			list.Add(new Common.Table_ID
			{
				ID = "Y",
				Name = "Yes"
			});
			list.Add(new Common.Table_ID
			{
				ID = "N",
				Name = "No"
			});
			lue_Filter.Properties.DataSource = list;
			lue_Filter.Properties.DisplayMember = "Name";
			lue_Filter.Properties.ValueMember = "ID";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_ItemCode_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string itemcode = Function.ToString(txt_ItemCode.EditValue);
			txt_ItemCode.EditValue = ChooseFromListItems(itemcode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_ItemCode_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Tab)
			{
				string itemcode = Function.ToString(txt_ItemCode.EditValue);
				txt_ItemCode.EditValue = ChooseFromListItems(itemcode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private string ChooseFromListItems(string itemcode)
	{
		string text = "";
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, true, itemcode, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems[0].IsExistsColumn("ItemCode"))
			{
				foreach (DataRow chooseItem in frm_CFL2.ChooseItems)
				{
					text = text + "," + Function.ToString(chooseItem["ItemCode"]);
				}
				text = ((!string.IsNullOrEmpty(text) && text.StartsWith(",")) ? text.Substring(1, text.Length - 1) : "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return text;
	}

	private async void btn_Run_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			btn_Run.Enabled = false;
			DataTable dataTable = new DataTable
			{
				Columns = { 
				{
					"Code",
					typeof(string)
				} }
			};
			DataRow row = dataTable.NewRow();
			dataTable.Rows.Add(row);
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("Branch", typeof(string));
			dataTable2.Columns.Add("DateFrom", typeof(DateTime));
			dataTable2.Columns.Add("DateTo", typeof(DateTime));
			dataTable2.Columns.Add("WhsCode", typeof(string));
			dataTable2.Columns.Add("Filter", typeof(string));
			dataTable2.Columns.Add("Item", typeof(string));
			DataRow dataRow = dataTable2.NewRow();
			dataRow["Branch"] = Function.ToString(lue_Branch.EditValue);
			dataRow["DateFrom"] = Function.ParseDateTime(date_From.EditValue) ?? DateTime.Today;
			dataRow["DateTo"] = Function.ParseDateTime(date_To.EditValue) ?? DateTime.Today;
			dataRow["WhsCode"] = Function.ToString(chk_Whse.EditValue);
			dataRow["Filter"] = Function.ToString(lue_Filter.EditValue);
			dataRow["Item"] = txt_ItemCode.Text;
			dataTable2.Rows.Add(dataRow);
			DataSet ds = await StockMovementSummary(dataTable2);
			await GridViewHelper.GenerateGrid(grd_Data, gv_Data, ds, null, expand: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_Run.Enabled = true;
		WinCommons._CloseProcessing();
	}

	public async Task<DataSet> StockMovementSummary(DataTable dt)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "InventoryReport");
			return await helper.PostAsync("StockMovementSummary", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.InventoryReport.frm_StockMovementSummary));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grp_Control = new DevExpress.XtraEditors.GroupControl();
		this.xtr_Control = new DevExpress.XtraEditors.XtraScrollableControl();
		this.txt_ItemCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.btn_Run = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lue_Filter = new DevExpress.XtraEditors.LookUpEdit();
		this.chk_Whse = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.date_To = new Apzon.Controls.Editor.DateEditEx();
		this.date_From = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Branch = new Apzon.Controls.Controls.LabelLine();
		this.lue_Branch = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.grd_Data = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Data = new Apzon.Controls.Grid.GridViewEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grp_Control).BeginInit();
		this.grp_Control.SuspendLayout();
		this.xtr_Control.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_ItemCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lue_Filter.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Whse.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lue_Branch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Data).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Data).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grp_Control);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Data);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(884, 512);
		this.splitContainerControl1.SplitterPosition = 93;
		this.splitContainerControl1.TabIndex = 5;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grp_Control.Controls.Add(this.xtr_Control);
		this.grp_Control.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grp_Control.Location = new System.Drawing.Point(0, 0);
		this.grp_Control.Name = "grp_Control";
		this.grp_Control.ShowCaption = false;
		this.grp_Control.Size = new System.Drawing.Size(884, 93);
		this.grp_Control.TabIndex = 0;
		this.grp_Control.Text = "Search";
		this.xtr_Control.Controls.Add(this.txt_ItemCode);
		this.xtr_Control.Controls.Add(this.btn_Run);
		this.xtr_Control.Controls.Add(this.lue_Filter);
		this.xtr_Control.Controls.Add(this.chk_Whse);
		this.xtr_Control.Controls.Add(this.labelLine2);
		this.xtr_Control.Controls.Add(this.labelLine1);
		this.xtr_Control.Controls.Add(this.date_To);
		this.xtr_Control.Controls.Add(this.date_From);
		this.xtr_Control.Controls.Add(this.labelLine4);
		this.xtr_Control.Controls.Add(this.labelLine5);
		this.xtr_Control.Controls.Add(this.lbl_Branch);
		this.xtr_Control.Controls.Add(this.lue_Branch);
		this.xtr_Control.Controls.Add(this.labelLine6);
		this.xtr_Control.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtr_Control.Location = new System.Drawing.Point(2, 2);
		this.xtr_Control.Name = "xtr_Control";
		this.xtr_Control.Size = new System.Drawing.Size(880, 89);
		this.xtr_Control.TabIndex = 0;
		this.txt_ItemCode.EditControl = null;
		this.txt_ItemCode.FieldName = null;
		this.txt_ItemCode.IsSearchControl = false;
		this.txt_ItemCode.Location = new System.Drawing.Point(135, 62);
		this.txt_ItemCode.Name = "txt_ItemCode";
		this.txt_ItemCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_ItemCode.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_ItemCode_Properties_ButtonClick);
		this.txt_ItemCode.Properties.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_ItemCode_KeyDown);
		this.txt_ItemCode.SettingInVisible = false;
		this.txt_ItemCode.SettingReadonly = false;
		this.txt_ItemCode.Size = new System.Drawing.Size(142, 20);
		this.txt_ItemCode.TabIndex = 27;
		this.btn_Run.EditControl = null;
		this.btn_Run.EditValue = "Run";
		this.btn_Run.FieldName = null;
		this.btn_Run.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Run.ImageOptions.Image");
		this.btn_Run.IsSearchControl = false;
		this.btn_Run.Location = new System.Drawing.Point(539, 6);
		this.btn_Run.Name = "btn_Run";
		this.btn_Run.ReadOnly = false;
		this.btn_Run.SettingInVisible = false;
		this.btn_Run.SettingReadonly = false;
		this.btn_Run.Size = new System.Drawing.Size(75, 23);
		this.btn_Run.TabIndex = 26;
		this.btn_Run.Text = "Run";
		this.btn_Run.Click += new System.EventHandler(this.btn_Run_Click);
		this.lue_Filter.Location = new System.Drawing.Point(357, 61);
		this.lue_Filter.Name = "lue_Filter";
		this.lue_Filter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lue_Filter.Properties.NullText = "";
		this.lue_Filter.Size = new System.Drawing.Size(142, 20);
		this.lue_Filter.TabIndex = 25;
		this.chk_Whse.Location = new System.Drawing.Point(357, 8);
		this.chk_Whse.Name = "chk_Whse";
		this.chk_Whse.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.chk_Whse.Size = new System.Drawing.Size(142, 20);
		this.chk_Whse.TabIndex = 23;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Warehouse";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(278, 8);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 24;
		this.labelLine2.Utext = "Warehouse";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Filter";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(278, 63);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 22;
		this.labelLine1.Utext = "Filter";
		this.date_To.EditControl = null;
		this.date_To.EditValue = null;
		this.date_To.FieldName = null;
		this.date_To.Location = new System.Drawing.Point(357, 35);
		this.date_To.Name = "date_To";
		this.date_To.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_To.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_To.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.date_To.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_To.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.date_To.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_To.Properties.IsSearchControl = false;
		this.date_To.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.date_To.SettingInVisible = false;
		this.date_To.SettingReadonly = false;
		this.date_To.Size = new System.Drawing.Size(142, 20);
		this.date_To.TabIndex = 18;
		this.date_From.EditControl = null;
		this.date_From.EditValue = null;
		this.date_From.FieldName = null;
		this.date_From.Location = new System.Drawing.Point(135, 34);
		this.date_From.Name = "date_From";
		this.date_From.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_From.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_From.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.date_From.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_From.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.date_From.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_From.Properties.IsSearchControl = false;
		this.date_From.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.date_From.SettingInVisible = false;
		this.date_From.SettingReadonly = false;
		this.date_From.Size = new System.Drawing.Size(142, 20);
		this.date_From.TabIndex = 17;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "To";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(278, 37);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 20;
		this.labelLine4.Utext = "To";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Date - From";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(10, 36);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(128, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 19;
		this.labelLine5.Utext = "Date - From";
		this.lbl_Branch.EditControl = null;
		this.lbl_Branch.EditValue = "Branch";
		this.lbl_Branch.FieldName = null;
		this.lbl_Branch.HideWhenEmpty = false;
		this.lbl_Branch.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Branch.Location = new System.Drawing.Point(10, 8);
		this.lbl_Branch.Name = "lbl_Branch";
		this.lbl_Branch.ReadOnly = false;
		this.lbl_Branch.SettingInVisible = false;
		this.lbl_Branch.SettingReadonly = false;
		this.lbl_Branch.ShowUnderLine = true;
		this.lbl_Branch.Size = new System.Drawing.Size(127, 20);
		this.lbl_Branch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Branch.TabIndex = 16;
		this.lbl_Branch.Utext = "Branch";
		this.lue_Branch.Location = new System.Drawing.Point(135, 8);
		this.lue_Branch.Name = "lue_Branch";
		this.lue_Branch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lue_Branch.Properties.NullText = "";
		this.lue_Branch.Size = new System.Drawing.Size(142, 20);
		this.lue_Branch.TabIndex = 15;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Item Code";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(9, 64);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(127, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 14;
		this.labelLine6.Utext = "Item Code";
		this.grd_Data.AllowPasteExcel = true;
		this.grd_Data.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Data.ClipboardData = null;
		this.grd_Data.Location = new System.Drawing.Point(0, 0);
		this.grd_Data.MainView = this.gv_Data;
		this.grd_Data.Name = "grd_Data";
		this.grd_Data.Size = new System.Drawing.Size(884, 414);
		this.grd_Data.TabIndex = 1;
		this.grd_Data.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Data });
		this.gv_Data.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Data.ColumnKey = null;
		this.gv_Data.DisableFormSetting = false;
		this.gv_Data.DisplayException = false;
		this.gv_Data.EditColumn = null;
		this.gv_Data.FieldCurrency = null;
		this.gv_Data.FieldRate = null;
		this.gv_Data.FormId = "oStockMovementSummary";
		this.gv_Data.GridControl = this.grd_Data;
		this.gv_Data.IgnoreFilter = false;
		this.gv_Data.IndexEditRow = -1;
		this.gv_Data.IndicatorWidth = 35;
		this.gv_Data.IsApzTable = false;
		this.gv_Data.IsInsertNewRow = false;
		this.gv_Data.IsLoadingData = false;
		this.gv_Data.IsMappingContextAction = false;
		this.gv_Data.ItemId = "ODATA";
		this.gv_Data.LstColumnAllowBatch = null;
		this.gv_Data.LstColumnNotAllowBatch = null;
		this.gv_Data.LstColumnNotEditable = null;
		this.gv_Data.LstColumNotSetting = null;
		this.gv_Data.Name = "gv_Data";
		this.gv_Data.OptionsView.ShowGroupPanel = false;
		this.gv_Data.UseBatchMod = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(884, 512);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oStockMovementSummary";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_StockMovementSummary";
		base.ShowInTaskbar = false;
		this.Text = "Stock Movement Summary";
		base.Load += new System.EventHandler(this.frm_StockMovementSummary_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grp_Control).EndInit();
		this.grp_Control.ResumeLayout(false);
		this.xtr_Control.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_ItemCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lue_Filter.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Whse.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lue_Branch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Data).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Data).EndInit();
		base.ResumeLayout(false);
	}
}
