using System;
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
using Apzon.Forms.Commons;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.KPIs;

public class frm_ListPointKPIs : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataTable data = new DataTable();

	private IContainer components;

	private GridControlEx grd_listPoint;

	private GridViewEx gv_listPoint;

	private GridColumnEx gc_SettingCode;

	private GridColumnEx gc_KPIsSet;

	private GridColumnEx gc_Remarks;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Ok;

	private GridColumnEx gc_ViewType;

	private RepositoryItemDateEditEx txt_FromDate;

	private RepositoryItemDateEditEx txt_ToDate;

	private RepositoryItemLookUpEdit txt_ViewType;

	private ContextMenuStrip contextMenuStrip;

	private SimpleButtonEx btn_Score;

	private GridColumnEx gc_NearestScore;

	private RepositoryItemDateEditEx repositoryItemDateEditEx1;

	private ToolStripMenuItem txt_context_Detail;

	private ToolStripMenuItem refreshToolStripMenuItem;

	private RepositoryItemButtonEditCFL txt_SetCode;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private GridColumnEx gc_FromDate;

	private GridColumnEx gc_ToDate;

	private GridColumnEx gc_Period;

	private RepositoryItemDateEditEx txt_period;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btn_Cancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.View:
			btn_Ok.Text = "OK";
			break;
		case Enums.FormViewModeType.Update:
			btn_Ok.Text = "Lưu";
			break;
		case Enums.FormViewModeType.Add:
			btn_Ok.Text = "Thêm";
			break;
		}
	}

	public frm_ListPointKPIs()
	{
		InitializeComponent();
		OnLoadAsync += frm_ListPointKPIs_Load;
	}

	private async Task LoadData()
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			LoadViewTypeSource();
			DataTable dataTable = await _httpClient.ListPointKPIsClient.GetListPoint();
			if (dataTable.IsNotNull())
			{
				data = dataTable.Copy();
				grd_listPoint.DataSource = dataTable;
				_ = grd_listPoint.DataSourceTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_ListPointKPIs_Load(object sender, EventArgs e)
	{
		try
		{
			gv_listPoint.ShowLoadingPanel();
			await LoadData();
			gv_listPoint.HideLoadingPanel();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadViewTypeSource()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Display", typeof(string));
			dataTable.Columns.Add("Value", typeof(string));
			dataTable.Rows.Add("Tuần", "W");
			dataTable.Rows.Add("Tháng", "M");
			dataTable.Rows.Add("Quý", "Q");
			dataTable.Rows.Add("Năm", "Y");
			txt_ViewType.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_header_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			ShowFormScoringDetail(Enums.FormViewModeType.Update);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void CloseForm()
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Save()
	{
		_ = 1;
		try
		{
			DataTable dataSourceTable = grd_listPoint.DataSourceTable;
			for (int i = 0; i < dataSourceTable.Rows.Count; i++)
			{
				dataSourceTable.Rows[i]["UserSign2"] = GlobalData.UserID;
			}
			dataSourceTable.AcceptChanges();
			string text = await _httpClient.ListPointKPIsClient.SaveListPointKPI(dataSourceTable);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				await LoadData();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				CloseForm();
			}
			else
			{
				await Save();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			CloseForm();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_listPoint_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
	}

	private void ShowFormScoringDetail(Enums.FormViewModeType type)
	{
		try
		{
			frm_KPIsScoringDetail frm_KPIsScoringDetail2 = new frm_KPIsScoringDetail();
			if (type == Enums.FormViewModeType.Update)
			{
				frm_KPIsScoringDetail2.dataFromListPoint = gv_listPoint.GetFocusedDataRow();
			}
			frm_KPIsScoringDetail2.ReloadDataListPoint = (Action)Delegate.Combine(frm_KPIsScoringDetail2.ReloadDataListPoint, (Action)(async () =>
			{
				await LoadData();
			}));
			frm_KPIsScoringDetail2.MdiParent = MdiParent;
			frm_KPIsScoringDetail2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Score_Click(object sender, EventArgs e)
	{
		try
		{
			ShowFormScoringDetail(Enums.FormViewModeType.Update);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_context_Detail_Click(object sender, EventArgs e)
	{
		try
		{
			ShowFormScoringDetail(Enums.FormViewModeType.Update);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void refreshToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			await LoadData();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_listPoint_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column == gc_Period)
			{
				RepositoryItemDateEditEx repositoryItemDateEditEx = new RepositoryItemDateEditEx();
				repositoryItemDateEditEx.DisplayFormat.FormatType = FormatType.Custom;
				repositoryItemDateEditEx.DisplayFormat.Format = new CustomFormatPeriod();
				e.RepositoryItem = repositoryItemDateEditEx;
				switch (Function.ToString(gv_listPoint.GetRowCellValue(e.RowHandle, gc_ViewType)))
				{
				case "M":
					e.RepositoryItem.Properties.DisplayFormat.FormatString = "apz_month";
					break;
				case "Q":
					e.RepositoryItem.Properties.DisplayFormat.FormatString = "apz_quarter";
					break;
				case "Y":
					e.RepositoryItem.Properties.DisplayFormat.FormatString = "apz_year";
					break;
				case "W":
					e.RepositoryItem.Properties.DisplayFormat.FormatString = "apz_week";
					break;
				}
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
		this.components = new System.ComponentModel.Container();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.KPIs.frm_ListPointKPIs));
		this.grd_listPoint = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.txt_context_Detail = new System.Windows.Forms.ToolStripMenuItem();
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_listPoint = new Apzon.Controls.Grid.GridViewEx();
		this.gc_SettingCode = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_SetCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_KPIsSet = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_FromDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ToDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ViewType = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_ViewType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Period = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_period = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gc_Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gc_NearestScore = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx1 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.txt_FromDate = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.txt_ToDate = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Score = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_listPoint).BeginInit();
		this.contextMenuStrip.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_listPoint).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SetCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ViewType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_period).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_period.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToDate.CalendarTimeProperties).BeginInit();
		base.SuspendLayout();
		this.grd_listPoint.AllowPasteExcel = true;
		this.grd_listPoint.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_listPoint.ClipboardData = null;
		this.grd_listPoint.ContextMenuStrip = this.contextMenuStrip;
		this.grd_listPoint.Location = new System.Drawing.Point(12, 12);
		this.grd_listPoint.MainView = this.gv_listPoint;
		this.grd_listPoint.Name = "grd_listPoint";
		this.grd_listPoint.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.txt_FromDate, this.txt_ToDate, this.txt_ViewType, this.repositoryItemDateEditEx1, this.txt_SetCode, this.repositoryItemMemoExEdit1, this.txt_period });
		this.grd_listPoint.Size = new System.Drawing.Size(846, 379);
		this.grd_listPoint.TabIndex = 2;
		this.grd_listPoint.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_listPoint });
		this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.txt_context_Detail, this.refreshToolStripMenuItem });
		this.contextMenuStrip.Name = "contextMenuStrip";
		this.contextMenuStrip.Size = new System.Drawing.Size(114, 48);
		this.txt_context_Detail.Name = "txt_context_Detail";
		this.txt_context_Detail.Size = new System.Drawing.Size(113, 22);
		this.txt_context_Detail.Text = "Chi tiết";
		this.txt_context_Detail.Click += new System.EventHandler(this.txt_context_Detail_Click);
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		this.refreshToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.refreshToolStripMenuItem.Text = "Refresh";
		this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
		this.gv_listPoint.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_listPoint.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_listPoint.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clKPIsListPoint;
		this.gv_listPoint.ColumnKey = null;
		this.gv_listPoint.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gc_SettingCode, this.gc_KPIsSet, this.gc_FromDate, this.gc_ToDate, this.gc_ViewType, this.gc_Period, this.gc_Remarks, this.gc_NearestScore });
		this.gv_listPoint.DisableFormSetting = false;
		this.gv_listPoint.DisplayException = false;
		this.gv_listPoint.EditColumn = null;
		this.gv_listPoint.FieldCurrency = null;
		this.gv_listPoint.FieldRate = null;
		this.gv_listPoint.FormId = "oKPIsListPoint";
		this.gv_listPoint.GridControl = this.grd_listPoint;
		this.gv_listPoint.IgnoreFilter = false;
		this.gv_listPoint.IndexEditRow = -1;
		this.gv_listPoint.IndicatorWidth = 35;
		this.gv_listPoint.IsApzTable = true;
		this.gv_listPoint.IsInsertNewRow = false;
		this.gv_listPoint.IsLoadingData = false;
		this.gv_listPoint.IsMappingContextAction = false;
		this.gv_listPoint.ItemId = "APZ_D_OKPR";
		this.gv_listPoint.LstColumnAllowBatch = null;
		this.gv_listPoint.LstColumnNotAllowBatch = null;
		this.gv_listPoint.LstColumnNotEditable = null;
		this.gv_listPoint.LstColumNotSetting = null;
		this.gv_listPoint.Name = "gv_listPoint";
		this.gv_listPoint.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_listPoint.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_listPoint.OptionsPrint.AutoWidth = false;
		this.gv_listPoint.OptionsPrint.UsePrintStyles = false;
		this.gv_listPoint.OptionsView.BestFitMaxRowCount = 100;
		this.gv_listPoint.OptionsView.ColumnAutoWidth = false;
		this.gv_listPoint.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_listPoint.OptionsView.ShowGroupedColumns = true;
		this.gv_listPoint.OptionsView.ShowGroupPanel = false;
		this.gv_listPoint.UseBatchMod = false;
		this.gv_listPoint.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_listPoint_CustomRowCellEdit);
		this.gv_listPoint.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_listPoint_CellValueChanged);
		this.gv_listPoint.DoubleClick += new System.EventHandler(this.gv_header_DoubleClick);
		this.gc_SettingCode.Caption = "Mã Bộ";
		this.gc_SettingCode.ColumnEdit = this.txt_SetCode;
		this.gc_SettingCode.CurrencyDateField = null;
		this.gc_SettingCode.CurrencyField = null;
		this.gc_SettingCode.CurrencyFixed = null;
		this.gc_SettingCode.CurrencyFixedRate = 0.0;
		this.gc_SettingCode.CurrencyRateField = null;
		this.gc_SettingCode.FieldName = "Code";
		this.gc_SettingCode.Name = "gc_SettingCode";
		this.gc_SettingCode.PrimaryKey = true;
		this.gc_SettingCode.UnitField = null;
		this.gc_SettingCode.Visible = true;
		this.gc_SettingCode.VisibleIndex = 0;
		this.txt_SetCode.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.txt_SetCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_SetCode.ContainerControl = this;
		this.txt_SetCode.DisplayMember = "Code";
		this.txt_SetCode.DisplayMember2 = "";
		this.txt_SetCode.LoadDataSourceWhenEnter = false;
		this.txt_SetCode.Name = "txt_SetCode";
		this.txt_SetCode.ReadOnly = true;
		this.txt_SetCode.SeparateCharacterForMulti = ';';
		this.txt_SetCode.SeparateString = ", ";
		this.txt_SetCode.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oSetIndicatorKPIs;
		this.txt_SetCode.UsingAutoCompleteBox = false;
		this.txt_SetCode.UsingDataSource = true;
		this.txt_SetCode.ValidateWhenNoDataSource = false;
		this.txt_SetCode.ValueMember = "Code";
		this.gc_KPIsSet.Caption = "Tên Bộ";
		this.gc_KPIsSet.CurrencyDateField = null;
		this.gc_KPIsSet.CurrencyField = null;
		this.gc_KPIsSet.CurrencyFixed = null;
		this.gc_KPIsSet.CurrencyFixedRate = 0.0;
		this.gc_KPIsSet.CurrencyRateField = null;
		this.gc_KPIsSet.FieldName = "Name";
		this.gc_KPIsSet.Name = "gc_KPIsSet";
		this.gc_KPIsSet.OptionsColumn.ReadOnly = true;
		this.gc_KPIsSet.PrimaryKey = false;
		this.gc_KPIsSet.UnitField = null;
		this.gc_KPIsSet.Visible = true;
		this.gc_KPIsSet.VisibleIndex = 1;
		this.gc_FromDate.Caption = "Từ Ngày";
		this.gc_FromDate.CurrencyDateField = null;
		this.gc_FromDate.CurrencyField = null;
		this.gc_FromDate.CurrencyFixed = null;
		this.gc_FromDate.CurrencyFixedRate = 0.0;
		this.gc_FromDate.CurrencyRateField = null;
		this.gc_FromDate.FieldName = "FromDate";
		this.gc_FromDate.Name = "gc_FromDate";
		this.gc_FromDate.OptionsColumn.ReadOnly = true;
		this.gc_FromDate.PrimaryKey = false;
		this.gc_FromDate.UnitField = null;
		this.gc_FromDate.Visible = true;
		this.gc_FromDate.VisibleIndex = 2;
		this.gc_ToDate.Caption = "Đến Ngày";
		this.gc_ToDate.CurrencyDateField = null;
		this.gc_ToDate.CurrencyField = null;
		this.gc_ToDate.CurrencyFixed = null;
		this.gc_ToDate.CurrencyFixedRate = 0.0;
		this.gc_ToDate.CurrencyRateField = null;
		this.gc_ToDate.FieldName = "ToDate";
		this.gc_ToDate.Name = "gc_ToDate";
		this.gc_ToDate.OptionsColumn.ReadOnly = true;
		this.gc_ToDate.PrimaryKey = false;
		this.gc_ToDate.UnitField = null;
		this.gc_ToDate.Visible = true;
		this.gc_ToDate.VisibleIndex = 3;
		this.gc_ViewType.Caption = "Kiểu Xem";
		this.gc_ViewType.ColumnEdit = this.txt_ViewType;
		this.gc_ViewType.CurrencyDateField = null;
		this.gc_ViewType.CurrencyField = null;
		this.gc_ViewType.CurrencyFixed = null;
		this.gc_ViewType.CurrencyFixedRate = 0.0;
		this.gc_ViewType.CurrencyRateField = null;
		this.gc_ViewType.FieldName = "ViewType";
		this.gc_ViewType.Name = "gc_ViewType";
		this.gc_ViewType.PrimaryKey = false;
		this.gc_ViewType.UnitField = null;
		this.gc_ViewType.Visible = true;
		this.gc_ViewType.VisibleIndex = 4;
		this.txt_ViewType.AutoHeight = false;
		this.txt_ViewType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_ViewType.DisplayMember = "Display";
		this.txt_ViewType.Name = "txt_ViewType";
		this.txt_ViewType.NullText = "";
		this.txt_ViewType.ReadOnly = true;
		this.txt_ViewType.ValueMember = "Value";
		this.gc_Period.Caption = "Kỳ";
		this.gc_Period.ColumnEdit = this.txt_period;
		this.gc_Period.CurrencyDateField = null;
		this.gc_Period.CurrencyField = null;
		this.gc_Period.CurrencyFixed = null;
		this.gc_Period.CurrencyFixedRate = 0.0;
		this.gc_Period.CurrencyRateField = null;
		this.gc_Period.FieldName = "Period";
		this.gc_Period.Name = "gc_Period";
		this.gc_Period.PrimaryKey = false;
		this.gc_Period.UnitField = null;
		this.gc_Period.Visible = true;
		this.gc_Period.VisibleIndex = 5;
		this.txt_period.AutoHeight = false;
		this.txt_period.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_period.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_period.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.txt_period.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.txt_period.EditFormat.FormatString = "dd/MM/yyyy";
		this.txt_period.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.txt_period.IsSearchControl = false;
		this.txt_period.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.txt_period.Name = "txt_period";
		this.txt_period.ReadOnly = true;
		this.gc_Remarks.Caption = "Ghi Chú";
		this.gc_Remarks.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gc_Remarks.CurrencyDateField = null;
		this.gc_Remarks.CurrencyField = null;
		this.gc_Remarks.CurrencyFixed = null;
		this.gc_Remarks.CurrencyFixedRate = 0.0;
		this.gc_Remarks.CurrencyRateField = null;
		this.gc_Remarks.FieldName = "Remarks";
		this.gc_Remarks.Name = "gc_Remarks";
		this.gc_Remarks.PrimaryKey = false;
		this.gc_Remarks.UnitField = null;
		this.gc_Remarks.Visible = true;
		this.gc_Remarks.VisibleIndex = 6;
		this.repositoryItemMemoExEdit1.AllowDropDownWhenReadOnly = DevExpress.Utils.DefaultBoolean.True;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ReadOnly = true;
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.gc_NearestScore.Caption = "Ngày đánh giá gần đây";
		this.gc_NearestScore.ColumnEdit = this.repositoryItemDateEditEx1;
		this.gc_NearestScore.CurrencyDateField = null;
		this.gc_NearestScore.CurrencyField = null;
		this.gc_NearestScore.CurrencyFixed = null;
		this.gc_NearestScore.CurrencyFixedRate = 0.0;
		this.gc_NearestScore.CurrencyRateField = null;
		this.gc_NearestScore.FieldName = "UpdateDate";
		this.gc_NearestScore.Name = "gc_NearestScore";
		this.gc_NearestScore.PrimaryKey = false;
		this.gc_NearestScore.UnitField = null;
		this.gc_NearestScore.Visible = true;
		this.gc_NearestScore.VisibleIndex = 7;
		this.repositoryItemDateEditEx1.AutoHeight = false;
		this.repositoryItemDateEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx1.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.IsSearchControl = false;
		this.repositoryItemDateEditEx1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx1.Name = "repositoryItemDateEditEx1";
		this.repositoryItemDateEditEx1.ReadOnly = true;
		this.txt_FromDate.AutoHeight = false;
		this.txt_FromDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_FromDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_FromDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.txt_FromDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.txt_FromDate.EditFormat.FormatString = "dd/MM/yyyy";
		this.txt_FromDate.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.txt_FromDate.IsSearchControl = false;
		this.txt_FromDate.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.txt_FromDate.Name = "txt_FromDate";
		this.txt_FromDate.ReadOnly = true;
		this.txt_ToDate.AutoHeight = false;
		this.txt_ToDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_ToDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_ToDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.txt_ToDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.txt_ToDate.EditFormat.FormatString = "dd/MM/yyyy";
		this.txt_ToDate.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.txt_ToDate.IsSearchControl = false;
		this.txt_ToDate.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.txt_ToDate.Name = "txt_ToDate";
		this.txt_ToDate.ReadOnly = true;
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(93, 397);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 3;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(12, 397);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 4;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Score.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Score.EditControl = null;
		this.btn_Score.EditValue = "Chấm Điểm";
		this.btn_Score.FieldName = null;
		this.btn_Score.IsSearchControl = false;
		this.btn_Score.Location = new System.Drawing.Point(768, 397);
		this.btn_Score.Name = "btn_Score";
		this.btn_Score.ReadOnly = false;
		this.btn_Score.SettingInVisible = false;
		this.btn_Score.SettingReadonly = false;
		this.btn_Score.Size = new System.Drawing.Size(90, 23);
		this.btn_Score.TabIndex = 5;
		this.btn_Score.Text = "Chấm Điểm";
		this.btn_Score.Click += new System.EventHandler(this.btn_Score_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(870, 432);
		this.ContextMenuStrip = this.contextMenuStrip;
		base.Controls.Add(this.btn_Score);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.grd_listPoint);
		base.FormId = "oKPIsListPoint";
		base.FormObjectType = Apzon.Commons.ApzObjectTypes.oKPIsListPoint;
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ListPointKPIs";
		this.Text = "Danh Sách Chấm Điểm KPIs";
		((System.ComponentModel.ISupportInitialize)this.grd_listPoint).EndInit();
		this.contextMenuStrip.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_listPoint).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SetCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ViewType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_period.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_period).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToDate).EndInit();
		base.ResumeLayout(false);
	}
}
