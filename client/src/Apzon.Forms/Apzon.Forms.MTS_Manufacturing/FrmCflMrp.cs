using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.PlanningProduction;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.MTS_Manufacturing;

public class FrmCflMrp : XtraFormEx
{
	private IHttpClientHelper<Mrp> _mrpClientHelper = new HttpClientHelper<Mrp>(Configuration.ApiServer, "Mrp");

	public Common.ListItems _Item = new Common.ListItems();

	private DataTable _dt = new DataTable();

	private IContainer components;

	private GridControlEx grd_MRP;

	private GridViewEx gv_MRP;

	private GridColumnEx cl_Code;

	private GridColumnEx cl_Name;

	private GridColumnEx cl_Status;

	private GridColumnEx cl_FromDate;

	private GridColumnEx cl_ToDate;

	private GridColumnEx cl_Whse;

	private ButtonEditEx txt_Search;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnChoose;

	private LabelLine labelLine1;

	public FrmCflMrp()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "FrmCflMrp"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "FrmCflMrp" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btnCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async void FrmCflMrp_Load(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			_dt = await GetData_CFL("2");
			grd_MRP.DataSource = _dt;
			grd_MRP.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async Task<DataTable> GetData_CFL(string status)
	{
		try
		{
			return await _mrpClientHelper.GetDataTableAsync("GetData_CFL", new { status });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private void ChooseItem()
	{
		try
		{
			if (gv_MRP.GetSelectedRows() != null && gv_MRP.GetSelectedRows().Length != 0)
			{
				object rowCellValue = gv_MRP.GetRowCellValue(gv_MRP.GetSelectedRows()[0], cl_Code);
				object rowCellValue2 = gv_MRP.GetRowCellValue(gv_MRP.GetSelectedRows()[0], cl_Name);
				if (rowCellValue != null && !string.IsNullOrEmpty(rowCellValue.ToString()))
				{
					_Item.Value = rowCellValue.ToString();
					_Item.Text = rowCellValue2.ToString();
					Close();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnChoose_Click(object sender, EventArgs e)
	{
		ChooseItem();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gv_MRP_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception)
		{
		}
	}

	private void gv_MRP_DoubleClick(object sender, EventArgs e)
	{
		ChooseItem();
	}

	private void gv_MRP_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				ChooseItem();
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTS_Manufacturing.FrmCflMrp));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.grd_MRP = new Apzon.Controls.Grid.GridControlEx();
		this.gv_MRP = new Apzon.Controls.Grid.GridViewEx();
		this.cl_Code = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_FromDate = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_ToDate = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Whse = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_Search = new Apzon.Controls.Editor.ButtonEditEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnChoose = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.grd_MRP).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_MRP).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Search.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_MRP.AllowPasteExcel = true;
		this.grd_MRP.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_MRP.ClipboardData = null;
		this.grd_MRP.Location = new System.Drawing.Point(-1, 33);
		this.grd_MRP.MainView = this.gv_MRP;
		this.grd_MRP.Name = "grd_MRP";
		this.grd_MRP.Size = new System.Drawing.Size(492, 302);
		this.grd_MRP.TabIndex = 5;
		this.grd_MRP.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_MRP });
		this.gv_MRP.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_MRP.ColumnKey = null;
		this.gv_MRP.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.cl_Code, this.cl_Name, this.cl_Status, this.cl_FromDate, this.cl_ToDate, this.cl_Whse });
		this.gv_MRP.DisableFormSetting = false;
		this.gv_MRP.DisplayException = false;
		this.gv_MRP.EditColumn = null;
		this.gv_MRP.FieldCurrency = null;
		this.gv_MRP.FieldRate = null;
		this.gv_MRP.FormId = null;
		this.gv_MRP.GridControl = this.grd_MRP;
		this.gv_MRP.IgnoreFilter = false;
		this.gv_MRP.IndexEditRow = -1;
		this.gv_MRP.IndicatorWidth = 30;
		this.gv_MRP.IsApzTable = false;
		this.gv_MRP.IsInsertNewRow = false;
		this.gv_MRP.IsLoadingData = false;
		this.gv_MRP.IsMappingContextAction = false;
		this.gv_MRP.ItemId = null;
		this.gv_MRP.LstColumnAllowBatch = null;
		this.gv_MRP.LstColumnNotAllowBatch = null;
		this.gv_MRP.LstColumnNotEditable = null;
		this.gv_MRP.LstColumNotSetting = null;
		this.gv_MRP.Name = "gv_MRP";
		this.gv_MRP.OptionsView.ShowGroupPanel = false;
		this.gv_MRP.UseBatchMod = false;
		this.gv_MRP.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_MRP_CustomDrawRowIndicator);
		this.gv_MRP.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gv_MRP_KeyDown);
		this.gv_MRP.DoubleClick += new System.EventHandler(this.gv_MRP_DoubleClick);
		this.cl_Code.Caption = "MRP Code";
		this.cl_Code.CurrencyDateField = null;
		this.cl_Code.CurrencyField = null;
		this.cl_Code.CurrencyFixed = null;
		this.cl_Code.CurrencyFixedRate = 0.0;
		this.cl_Code.CurrencyRateField = null;
		this.cl_Code.FieldName = "MrpCode";
		this.cl_Code.Name = "cl_Code";
		this.cl_Code.OptionsColumn.AllowEdit = false;
		this.cl_Code.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.cl_Code.PrimaryKey = true;
		this.cl_Code.UnitField = null;
		this.cl_Code.Visible = true;
		this.cl_Code.VisibleIndex = 0;
		this.cl_Code.Width = 118;
		this.cl_Name.Caption = "MRP Name";
		this.cl_Name.CurrencyDateField = null;
		this.cl_Name.CurrencyField = null;
		this.cl_Name.CurrencyFixed = null;
		this.cl_Name.CurrencyFixedRate = 0.0;
		this.cl_Name.CurrencyRateField = null;
		this.cl_Name.FieldName = "MrpName";
		this.cl_Name.Name = "cl_Name";
		this.cl_Name.OptionsColumn.AllowEdit = false;
		this.cl_Name.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.cl_Name.PrimaryKey = false;
		this.cl_Name.UnitField = null;
		this.cl_Name.Visible = true;
		this.cl_Name.VisibleIndex = 1;
		this.cl_Name.Width = 229;
		this.cl_Status.Caption = "Status";
		this.cl_Status.CurrencyDateField = null;
		this.cl_Status.CurrencyField = null;
		this.cl_Status.CurrencyFixed = null;
		this.cl_Status.CurrencyFixedRate = 0.0;
		this.cl_Status.CurrencyRateField = null;
		this.cl_Status.FieldName = "Status";
		this.cl_Status.Name = "cl_Status";
		this.cl_Status.OptionsColumn.AllowEdit = false;
		this.cl_Status.PrimaryKey = false;
		this.cl_Status.UnitField = null;
		this.cl_Status.Visible = true;
		this.cl_Status.VisibleIndex = 2;
		this.cl_Status.Width = 113;
		this.cl_FromDate.Caption = "From Date";
		this.cl_FromDate.CurrencyDateField = null;
		this.cl_FromDate.CurrencyField = null;
		this.cl_FromDate.CurrencyFixed = null;
		this.cl_FromDate.CurrencyFixedRate = 0.0;
		this.cl_FromDate.CurrencyRateField = null;
		this.cl_FromDate.FieldName = "BeginDate";
		this.cl_FromDate.Name = "cl_FromDate";
		this.cl_FromDate.PrimaryKey = false;
		this.cl_FromDate.UnitField = null;
		this.cl_ToDate.Caption = "To Date";
		this.cl_ToDate.CurrencyDateField = null;
		this.cl_ToDate.CurrencyField = null;
		this.cl_ToDate.CurrencyFixed = null;
		this.cl_ToDate.CurrencyFixedRate = 0.0;
		this.cl_ToDate.CurrencyRateField = null;
		this.cl_ToDate.FieldName = "EndDate";
		this.cl_ToDate.Name = "cl_ToDate";
		this.cl_ToDate.PrimaryKey = false;
		this.cl_ToDate.UnitField = null;
		this.cl_Whse.Caption = "Whse";
		this.cl_Whse.CurrencyDateField = null;
		this.cl_Whse.CurrencyField = null;
		this.cl_Whse.CurrencyFixed = null;
		this.cl_Whse.CurrencyFixedRate = 0.0;
		this.cl_Whse.CurrencyRateField = null;
		this.cl_Whse.FieldName = "Whs";
		this.cl_Whse.Name = "cl_Whse";
		this.cl_Whse.PrimaryKey = false;
		this.cl_Whse.UnitField = null;
		this.txt_Search.EditControl = null;
		this.txt_Search.FieldName = null;
		this.txt_Search.IsSearchControl = false;
		this.txt_Search.Location = new System.Drawing.Point(93, 5);
		this.txt_Search.Name = "txt_Search";
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.txt_Search.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_Search.SettingInVisible = false;
		this.txt_Search.SettingReadonly = false;
		this.txt_Search.Size = new System.Drawing.Size(358, 24);
		this.txt_Search.TabIndex = 4;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(93, 341);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 7;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnChoose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnChoose.EditControl = null;
		this.btnChoose.EditValue = "Choose";
		this.btnChoose.FieldName = null;
		this.btnChoose.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnChoose.ImageOptions.Image");
		this.btnChoose.IsSearchControl = false;
		this.btnChoose.Location = new System.Drawing.Point(12, 341);
		this.btnChoose.Name = "btnChoose";
		this.btnChoose.ReadOnly = false;
		this.btnChoose.SettingInVisible = false;
		this.btnChoose.SettingReadonly = false;
		this.btnChoose.Size = new System.Drawing.Size(75, 23);
		this.btnChoose.TabIndex = 6;
		this.btnChoose.Text = "Choose";
		this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Search";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(11, 10);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 8;
		this.labelLine1.Utext = "Search";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(490, 368);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.grd_MRP);
		base.Controls.Add(this.txt_Search);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnChoose);
		base.IconOptions.ShowIcon = false;
		base.Name = "FrmCflMrp";
		base.ShowInTaskbar = false;
		this.Text = "List Of MRP";
		base.Load += new System.EventHandler(this.FrmCflMrp_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_MRP).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_MRP).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Search.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
