using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons.CFL;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Mrps;

public class frm_GenPOFromMRP : XtraFormEx
{
	private readonly string _controlerMRP = "Mrps";

	private readonly string _controlerPOOrder = "ProductionOder";

	private readonly string _formatNum = "n3";

	private readonly string _formatDate = "dd/MM/yyyy";

	private IContainer components;

	private LabelLine labelLine1;

	private ButtonEditEx txt_MRP;

	private XtraTabControlEx tabControl;

	private XtraTabPageEx tab_Plan;

	private PanelControl panelControl1;

	private GridControlEx grd_Plan;

	private GridViewEx gv_Plan;

	private XtraTabPageEx tab_Release;

	private PanelControl panelControl2;

	private GridControlEx grd_Release;

	private GridViewEx gv_Release;

	private XtraTabPageEx tab_Report;

	private PanelControl panelControl3;

	private GridControlEx grd_Report;

	private GridViewEx gv_Report;

	private XtraTabPageEx tab_Cancel;

	private PanelControl panelControl4;

	private GridControlEx grd_Cancel;

	private GridViewEx gv_Cancel;

	private XtraTabPageEx tab_Closed;

	private PanelControl panelControl5;

	private GridControlEx grd_Close;

	private GridViewEx gv_Close;

	public SimpleButtonEx btnLoad;

	public SimpleButtonEx btn_CreatePO;

	public SimpleButtonEx btnRelease;

	public SimpleButtonEx btnReport;

	public SimpleButtonEx btnCancel;

	public SimpleButtonEx btnClose;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private RepositoryItemDateEdit rep_Date;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn9;

	private RepositoryItemButtonEdit rep_Project;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private RepositoryItemButtonEdit rep_Whse;

	public frm_GenPOFromMRP()
	{
		InitializeComponent();
	}

	private void AddColumn(GridView gv, string displayColumn, string columnName, int groupIndex, int minWidth, int visibleIndex, bool visible, string fieldName, FixedStyle fix = FixedStyle.None)
	{
		GridColumn gridColumn = gv.Columns.Add();
		gridColumn.Caption = displayColumn;
		gridColumn.FieldName = fieldName;
		gridColumn.Fixed = fix;
		gridColumn.MinWidth = minWidth;
		gridColumn.Name = columnName;
		gridColumn.VisibleIndex = visibleIndex;
		gridColumn.Visible = visible;
		gridColumn.GroupIndex = groupIndex;
		gridColumn.OptionsColumn.AllowSort = DefaultBoolean.False;
		gridColumn.OptionsColumn.AllowEdit = false;
		gridColumn.OptionsColumn.AllowMove = false;
		gridColumn.OptionsColumn.AllowSize = true;
		gridColumn.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
		if (fieldName.Equals("Quantity"))
		{
			RepositoryItemCalcEdit repositoryItemCalcEdit = new RepositoryItemCalcEdit();
			repositoryItemCalcEdit.DisplayFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit.DisplayFormat.FormatString = _formatNum;
			repositoryItemCalcEdit.EditFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit.EditFormat.FormatString = _formatNum;
			repositoryItemCalcEdit.EditMask = _formatNum;
			gridColumn.DisplayFormat.FormatType = FormatType.Numeric;
			gridColumn.DisplayFormat.FormatString = _formatNum;
			gridColumn.ColumnEdit = repositoryItemCalcEdit;
		}
		else if (fieldName.Equals("Project") || fieldName.Equals("Whse"))
		{
			RepositoryItemButtonEdit columnEdit = new RepositoryItemButtonEdit();
			gridColumn.DisplayFormat.FormatType = FormatType.Numeric;
			gridColumn.DisplayFormat.FormatString = _formatDate;
			gridColumn.ColumnEdit = columnEdit;
		}
		else if (fieldName.Equals("DueDate") || fieldName.Equals("Whse"))
		{
			RepositoryItemDateEdit repositoryItemDateEdit = new RepositoryItemDateEdit();
			repositoryItemDateEdit.DisplayFormat.FormatType = FormatType.Numeric;
			repositoryItemDateEdit.DisplayFormat.FormatString = _formatDate;
			repositoryItemDateEdit.EditFormat.FormatType = FormatType.Numeric;
			repositoryItemDateEdit.EditFormat.FormatString = _formatDate;
			repositoryItemDateEdit.EditMask = _formatDate;
			gridColumn.DisplayFormat.FormatType = FormatType.Numeric;
			gridColumn.DisplayFormat.FormatString = _formatDate;
			gridColumn.ColumnEdit = repositoryItemDateEdit;
		}
	}

	private void LoadColumn_Plan(DataTable dt)
	{
		try
		{
			gv_Plan.Columns.Clear();
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				Type dataType = dt.Columns[i].DataType;
				if (dataType == typeof(double) || dataType == typeof(decimal))
				{
					AddColumn(gv_Plan, dt.Columns[i].ColumnName, dt.Columns[i].ColumnName, -1, 80, i, visible: true, dt.Columns[i].ColumnName);
				}
				else if (dataType == typeof(float) || dataType == typeof(short) || dataType == typeof(int) || dataType == typeof(long))
				{
					AddColumn(gv_Plan, dt.Columns[i].ColumnName, dt.Columns[i].ColumnName, -1, 80, i, visible: true, dt.Columns[i].ColumnName);
				}
				else if (dataType == typeof(string) || dataType == typeof(char))
				{
					AddColumn(gv_Plan, dt.Columns[i].ColumnName, dt.Columns[i].ColumnName, -1, 120, i, visible: true, dt.Columns[i].ColumnName);
				}
				else if (dataType == typeof(bool))
				{
					AddColumn(gv_Plan, dt.Columns[i].ColumnName, dt.Columns[i].ColumnName, -1, 40, i, visible: true, dt.Columns[i].ColumnName, FixedStyle.Right);
				}
				else if (dataType == typeof(DateTime))
				{
					if (dt.Columns[i].ColumnName.Equals("CloseDate"))
					{
						AddColumn(gv_Plan, dt.Columns[i].ColumnName, dt.Columns[i].ColumnName, -1, 70, -1, visible: false, dt.Columns[i].ColumnName);
					}
					else
					{
						AddColumn(gv_Plan, dt.Columns[i].ColumnName, dt.Columns[i].ColumnName, -1, 70, i, visible: true, dt.Columns[i].ColumnName);
					}
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void GetProductionMRP(string type)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controlerMRP);
			int mrpNo = Function.ParseInt(txt_MRP.Text);
			DataTable dataTable = await _helper.GetAsync("GetProductionMRP", new { mrpNo, type });
			if (dataTable != null && type == "P")
			{
				grd_Plan.DataSource = dataTable;
				grd_Plan.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Plan_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void rep_Project_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Project, multiSelect: false, null);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				DataRowView dataRowView = frm_ChooseFromList2.ChooseItem;
				gv_Plan.SetRowCellValue(gv_Plan.GetSelectedRows()[0], "Project", Function.ToString(dataRowView["PrjCode"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_Whse_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, null);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				DataRow dataRow = frm_ChooseFromList2.ChooseItem;
				gv_Plan.SetRowCellValue(gv_Plan.GetSelectedRows()[0], "Whse", dataRow["WhsCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_CreatePO_Click(object sender, EventArgs e)
	{
		try
		{
			if (grd_Plan.DataSource != null && grd_Plan.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				dataTable.BeginLoadData();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					Function.ToString(dataTable.Rows[i]["Checked"]).ToUpper().Equals("TRUE");
				}
				dataTable.EndLoadData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_MRP_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.MRP, multiSelect: false, null);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				DataRowView dataRowView = (DataRowView)frm_ChooseFromList2.ChooseItem;
				txt_MRP.Text = dataRowView["Id"].ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnLoad_Click(object sender, EventArgs e)
	{
		GetProductionMRP("P");
	}

	private void tabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			switch (e.Page.Name)
			{
			case "tab_Plan":
				GetProductionMRP("P");
				break;
			case "tab_Release":
				GetProductionMRP("R");
				break;
			case "tab_Report":
				GetProductionMRP("E");
				break;
			case "tab_Cancel":
				GetProductionMRP("C");
				break;
			case "tab_Closed":
				GetProductionMRP("L");
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnClose_Click(object sender, EventArgs e)
	{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_GenPOFromMRP));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txt_MRP = new Apzon.Controls.Editor.ButtonEditEx();
		this.tabControl = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Plan = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.btn_CreatePO = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Plan = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Plan = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Date = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Whse = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Project = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.tab_Release = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.btnRelease = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Release = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Release = new Apzon.Controls.Grid.GridViewEx();
		this.tab_Report = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		this.btnReport = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Report = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Report = new Apzon.Controls.Grid.GridViewEx();
		this.tab_Cancel = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Cancel = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Cancel = new Apzon.Controls.Grid.GridViewEx();
		this.tab_Closed = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl5 = new DevExpress.XtraEditors.PanelControl();
		this.btnClose = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Close = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Close = new Apzon.Controls.Grid.GridViewEx();
		this.btnLoad = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.txt_MRP.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl).BeginInit();
		this.tabControl.SuspendLayout();
		this.tab_Plan.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Plan).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Plan).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Whse).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Project).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		this.tab_Release.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Release).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Release).BeginInit();
		this.tab_Report.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Report).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Report).BeginInit();
		this.tab_Cancel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl4).BeginInit();
		this.panelControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Cancel).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Cancel).BeginInit();
		this.tab_Closed.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl5).BeginInit();
		this.panelControl5.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Close).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Close).BeginInit();
		base.SuspendLayout();
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "MRP";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(28, 11);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(101, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "MRP";
		this.txt_MRP.EditControl = null;
		this.txt_MRP.FieldName = null;
		this.txt_MRP.IsSearchControl = false;
		this.txt_MRP.Location = new System.Drawing.Point(126, 6);
		this.txt_MRP.Name = "txt_MRP";
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.txt_MRP.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_MRP.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.txt_MRP.SettingInVisible = false;
		this.txt_MRP.SettingReadonly = false;
		this.txt_MRP.Size = new System.Drawing.Size(157, 24);
		this.txt_MRP.TabIndex = 1;
		this.txt_MRP.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_MRP_ButtonClick);
		this.tabControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tabControl.IsSearchControl = false;
		this.tabControl.Location = new System.Drawing.Point(1, 35);
		this.tabControl.Name = "tabControl";
		this.tabControl.SelectedTabPage = this.tab_Plan;
		this.tabControl.Size = new System.Drawing.Size(630, 443);
		this.tabControl.TabIndex = 3;
		this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[5] { this.tab_Plan, this.tab_Release, this.tab_Report, this.tab_Cancel, this.tab_Closed });
		this.tabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tabControl_SelectedPageChanged);
		this.tab_Plan.Controls.Add(this.panelControl1);
		this.tab_Plan.EditControl = null;
		this.tab_Plan.EditValue = "Plan";
		this.tab_Plan.FieldName = null;
		this.tab_Plan.IsSearchControl = false;
		this.tab_Plan.Name = "tab_Plan";
		this.tab_Plan.ReadOnly = false;
		this.tab_Plan.SettingInVisible = false;
		this.tab_Plan.SettingReadonly = false;
		this.tab_Plan.Size = new System.Drawing.Size(628, 418);
		this.tab_Plan.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Plan.Text = "Plan";
		this.panelControl1.Controls.Add(this.btn_CreatePO);
		this.panelControl1.Controls.Add(this.grd_Plan);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(628, 418);
		this.panelControl1.TabIndex = 0;
		this.btn_CreatePO.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_CreatePO.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_CreatePO.Appearance.Options.UseBackColor = true;
		this.btn_CreatePO.EditControl = null;
		this.btn_CreatePO.EditValue = "Create PO";
		this.btn_CreatePO.FieldName = null;
		this.btn_CreatePO.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CreatePO.ImageOptions.Image");
		this.btn_CreatePO.IsSearchControl = false;
		this.btn_CreatePO.Location = new System.Drawing.Point(10, 391);
		this.btn_CreatePO.Name = "btn_CreatePO";
		this.btn_CreatePO.ReadOnly = false;
		this.btn_CreatePO.SettingInVisible = false;
		this.btn_CreatePO.SettingReadonly = false;
		this.btn_CreatePO.Size = new System.Drawing.Size(99, 20);
		this.btn_CreatePO.TabIndex = 17;
		this.btn_CreatePO.Text = "Create PO";
		this.btn_CreatePO.Click += new System.EventHandler(this.btn_CreatePO_Click);
		this.grd_Plan.AllowPasteExcel = true;
		this.grd_Plan.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Plan.ClipboardData = null;
		this.grd_Plan.Location = new System.Drawing.Point(0, 0);
		this.grd_Plan.MainView = this.gv_Plan;
		this.grd_Plan.Name = "grd_Plan";
		this.grd_Plan.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_Date, this.repositoryItemCheckEdit1, this.rep_Project, this.rep_Whse });
		this.grd_Plan.Size = new System.Drawing.Size(628, 385);
		this.grd_Plan.TabIndex = 0;
		this.grd_Plan.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Plan });
		this.gv_Plan.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Plan.ColumnKey = null;
		this.gv_Plan.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn9, this.gridColumn7, this.gridColumn8 });
		this.gv_Plan.DisableFormSetting = false;
		this.gv_Plan.DisplayException = false;
		this.gv_Plan.EditColumn = null;
		this.gv_Plan.FieldCurrency = null;
		this.gv_Plan.FieldRate = null;
		this.gv_Plan.FormId = "oGenPoFromMrp";
		this.gv_Plan.GridControl = this.grd_Plan;
		this.gv_Plan.IgnoreFilter = false;
		this.gv_Plan.IndexEditRow = -1;
		this.gv_Plan.IndicatorWidth = 40;
		this.gv_Plan.IsApzTable = false;
		this.gv_Plan.IsInsertNewRow = false;
		this.gv_Plan.IsLoadingData = false;
		this.gv_Plan.IsMappingContextAction = false;
		this.gv_Plan.ItemId = "OPLAN";
		this.gv_Plan.LstColumnAllowBatch = null;
		this.gv_Plan.LstColumnNotAllowBatch = null;
		this.gv_Plan.LstColumnNotEditable = null;
		this.gv_Plan.LstColumNotSetting = null;
		this.gv_Plan.Name = "gv_Plan";
		this.gv_Plan.OptionsView.ColumnAutoWidth = false;
		this.gv_Plan.OptionsView.ShowGroupPanel = false;
		this.gv_Plan.UseBatchMod = false;
		this.gv_Plan.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Plan_CustomDrawRowIndicator);
		this.gridColumn1.Caption = "PO No";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "PONo";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 53;
		this.gridColumn2.Caption = "Item Code";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "ItemCode";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.AllowEdit = false;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 93;
		this.gridColumn3.Caption = "Item Name";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "ItemName";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.AllowEdit = false;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 2;
		this.gridColumn3.Width = 120;
		this.gridColumn4.Caption = "Quantity";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.DisplayFormat.FormatString = "n3";
		this.gridColumn4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn4.FieldName = "Quantity";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.AllowEdit = false;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 3;
		this.gridColumn4.Width = 96;
		this.gridColumn5.Caption = "Due Date";
		this.gridColumn5.ColumnEdit = this.rep_Date;
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.gridColumn5.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gridColumn5.FieldName = "DueDate";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsColumn.AllowEdit = false;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 4;
		this.rep_Date.AutoHeight = false;
		this.rep_Date.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Date.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Date.Name = "rep_Date";
		this.gridColumn6.Caption = "Whse";
		this.gridColumn6.ColumnEdit = this.rep_Whse;
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "Whse";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 5;
		this.rep_Whse.AutoHeight = false;
		this.rep_Whse.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rep_Whse.Name = "rep_Whse";
		this.rep_Whse.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Whse_ButtonClick);
		this.gridColumn9.Caption = "Project";
		this.gridColumn9.ColumnEdit = this.rep_Project;
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "Project";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 6;
		this.rep_Project.AutoHeight = false;
		this.rep_Project.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rep_Project.Name = "rep_Project";
		this.rep_Project.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Project_ButtonClick);
		this.gridColumn7.Caption = "Remark";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "Remark";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 7;
		this.gridColumn8.Caption = "Checked";
		this.gridColumn8.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "Checked";
		this.gridColumn8.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 8;
		this.gridColumn8.Width = 40;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.tab_Release.Controls.Add(this.panelControl2);
		this.tab_Release.EditControl = null;
		this.tab_Release.EditValue = "Release";
		this.tab_Release.FieldName = null;
		this.tab_Release.IsSearchControl = false;
		this.tab_Release.Name = "tab_Release";
		this.tab_Release.ReadOnly = false;
		this.tab_Release.SettingInVisible = false;
		this.tab_Release.SettingReadonly = false;
		this.tab_Release.Size = new System.Drawing.Size(628, 418);
		this.tab_Release.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Release.Text = "Release";
		this.panelControl2.Controls.Add(this.btnRelease);
		this.panelControl2.Controls.Add(this.grd_Release);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl2.Location = new System.Drawing.Point(0, 0);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(628, 418);
		this.panelControl2.TabIndex = 1;
		this.btnRelease.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnRelease.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnRelease.Appearance.Options.UseBackColor = true;
		this.btnRelease.EditControl = null;
		this.btnRelease.EditValue = "Release";
		this.btnRelease.FieldName = null;
		this.btnRelease.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnRelease.ImageOptions.Image");
		this.btnRelease.IsSearchControl = false;
		this.btnRelease.Location = new System.Drawing.Point(10, 393);
		this.btnRelease.Name = "btnRelease";
		this.btnRelease.ReadOnly = false;
		this.btnRelease.SettingInVisible = false;
		this.btnRelease.SettingReadonly = false;
		this.btnRelease.Size = new System.Drawing.Size(99, 20);
		this.btnRelease.TabIndex = 18;
		this.btnRelease.Text = "Release";
		this.grd_Release.AllowPasteExcel = false;
		this.grd_Release.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Release.ClipboardData = null;
		this.grd_Release.Location = new System.Drawing.Point(0, 0);
		this.grd_Release.MainView = this.gv_Release;
		this.grd_Release.Name = "grd_Release";
		this.grd_Release.Size = new System.Drawing.Size(628, 385);
		this.grd_Release.TabIndex = 2;
		this.grd_Release.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Release });
		this.gv_Release.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Release.ColumnKey = null;
		this.gv_Release.DisableFormSetting = false;
		this.gv_Release.DisplayException = false;
		this.gv_Release.EditColumn = null;
		this.gv_Release.FieldCurrency = null;
		this.gv_Release.FieldRate = null;
		this.gv_Release.FormId = "oGenPoFromMrp";
		this.gv_Release.GridControl = this.grd_Release;
		this.gv_Release.IgnoreFilter = false;
		this.gv_Release.IndexEditRow = -1;
		this.gv_Release.IndicatorWidth = 40;
		this.gv_Release.IsApzTable = false;
		this.gv_Release.IsInsertNewRow = false;
		this.gv_Release.IsLoadingData = false;
		this.gv_Release.IsMappingContextAction = false;
		this.gv_Release.ItemId = "ORELEASE";
		this.gv_Release.LstColumnAllowBatch = null;
		this.gv_Release.LstColumnNotAllowBatch = null;
		this.gv_Release.LstColumnNotEditable = null;
		this.gv_Release.LstColumNotSetting = null;
		this.gv_Release.Name = "gv_Release";
		this.gv_Release.OptionsView.ColumnAutoWidth = false;
		this.gv_Release.OptionsView.ShowGroupPanel = false;
		this.gv_Release.UseBatchMod = false;
		this.tab_Report.Controls.Add(this.panelControl3);
		this.tab_Report.EditControl = null;
		this.tab_Report.EditValue = "Report";
		this.tab_Report.FieldName = null;
		this.tab_Report.IsSearchControl = false;
		this.tab_Report.Name = "tab_Report";
		this.tab_Report.ReadOnly = false;
		this.tab_Report.SettingInVisible = false;
		this.tab_Report.SettingReadonly = false;
		this.tab_Report.Size = new System.Drawing.Size(628, 418);
		this.tab_Report.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Report.Text = "Report";
		this.panelControl3.Controls.Add(this.btnReport);
		this.panelControl3.Controls.Add(this.grd_Report);
		this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl3.Location = new System.Drawing.Point(0, 0);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(628, 418);
		this.panelControl3.TabIndex = 1;
		this.btnReport.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnReport.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnReport.Appearance.Options.UseBackColor = true;
		this.btnReport.EditControl = null;
		this.btnReport.EditValue = "Report";
		this.btnReport.FieldName = null;
		this.btnReport.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnReport.ImageOptions.Image");
		this.btnReport.IsSearchControl = false;
		this.btnReport.Location = new System.Drawing.Point(10, 393);
		this.btnReport.Name = "btnReport";
		this.btnReport.ReadOnly = false;
		this.btnReport.SettingInVisible = false;
		this.btnReport.SettingReadonly = false;
		this.btnReport.Size = new System.Drawing.Size(99, 20);
		this.btnReport.TabIndex = 19;
		this.btnReport.Text = "Report";
		this.grd_Report.AllowPasteExcel = false;
		this.grd_Report.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Report.ClipboardData = null;
		this.grd_Report.Location = new System.Drawing.Point(0, 0);
		this.grd_Report.MainView = this.gv_Report;
		this.grd_Report.Name = "grd_Report";
		this.grd_Report.Size = new System.Drawing.Size(628, 385);
		this.grd_Report.TabIndex = 4;
		this.grd_Report.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Report });
		this.gv_Report.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Report.ColumnKey = null;
		this.gv_Report.DisableFormSetting = false;
		this.gv_Report.DisplayException = false;
		this.gv_Report.EditColumn = null;
		this.gv_Report.FieldCurrency = null;
		this.gv_Report.FieldRate = null;
		this.gv_Report.FormId = "oGenPoFromMrp";
		this.gv_Report.GridControl = this.grd_Report;
		this.gv_Report.IgnoreFilter = false;
		this.gv_Report.IndexEditRow = -1;
		this.gv_Report.IndicatorWidth = 40;
		this.gv_Report.IsApzTable = false;
		this.gv_Report.IsInsertNewRow = false;
		this.gv_Report.IsLoadingData = false;
		this.gv_Report.IsMappingContextAction = false;
		this.gv_Report.ItemId = "OREPORT";
		this.gv_Report.LstColumnAllowBatch = null;
		this.gv_Report.LstColumnNotAllowBatch = null;
		this.gv_Report.LstColumnNotEditable = null;
		this.gv_Report.LstColumNotSetting = null;
		this.gv_Report.Name = "gv_Report";
		this.gv_Report.OptionsView.ColumnAutoWidth = false;
		this.gv_Report.OptionsView.ShowGroupPanel = false;
		this.gv_Report.UseBatchMod = false;
		this.tab_Cancel.Controls.Add(this.panelControl4);
		this.tab_Cancel.EditControl = null;
		this.tab_Cancel.EditValue = "Cancel";
		this.tab_Cancel.FieldName = null;
		this.tab_Cancel.IsSearchControl = false;
		this.tab_Cancel.Name = "tab_Cancel";
		this.tab_Cancel.ReadOnly = false;
		this.tab_Cancel.SettingInVisible = false;
		this.tab_Cancel.SettingReadonly = false;
		this.tab_Cancel.Size = new System.Drawing.Size(628, 418);
		this.tab_Cancel.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Cancel.Text = "Cancel";
		this.panelControl4.Controls.Add(this.btnCancel);
		this.panelControl4.Controls.Add(this.grd_Cancel);
		this.panelControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl4.Location = new System.Drawing.Point(0, 0);
		this.panelControl4.Name = "panelControl4";
		this.panelControl4.Size = new System.Drawing.Size(628, 418);
		this.panelControl4.TabIndex = 1;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(10, 393);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(99, 20);
		this.btnCancel.TabIndex = 20;
		this.btnCancel.Text = "Cancel";
		this.grd_Cancel.AllowPasteExcel = false;
		this.grd_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Cancel.ClipboardData = null;
		this.grd_Cancel.Location = new System.Drawing.Point(0, 0);
		this.grd_Cancel.MainView = this.gv_Cancel;
		this.grd_Cancel.Name = "grd_Cancel";
		this.grd_Cancel.Size = new System.Drawing.Size(628, 385);
		this.grd_Cancel.TabIndex = 6;
		this.grd_Cancel.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Cancel });
		this.gv_Cancel.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Cancel.ColumnKey = null;
		this.gv_Cancel.DisableFormSetting = false;
		this.gv_Cancel.DisplayException = false;
		this.gv_Cancel.EditColumn = null;
		this.gv_Cancel.FieldCurrency = null;
		this.gv_Cancel.FieldRate = null;
		this.gv_Cancel.FormId = "oGenPoFromMrp";
		this.gv_Cancel.GridControl = this.grd_Cancel;
		this.gv_Cancel.IgnoreFilter = false;
		this.gv_Cancel.IndexEditRow = -1;
		this.gv_Cancel.IndicatorWidth = 40;
		this.gv_Cancel.IsApzTable = false;
		this.gv_Cancel.IsInsertNewRow = false;
		this.gv_Cancel.IsLoadingData = false;
		this.gv_Cancel.IsMappingContextAction = false;
		this.gv_Cancel.ItemId = "OCANCEL";
		this.gv_Cancel.LstColumnAllowBatch = null;
		this.gv_Cancel.LstColumnNotAllowBatch = null;
		this.gv_Cancel.LstColumnNotEditable = null;
		this.gv_Cancel.LstColumNotSetting = null;
		this.gv_Cancel.Name = "gv_Cancel";
		this.gv_Cancel.OptionsView.ColumnAutoWidth = false;
		this.gv_Cancel.OptionsView.ShowGroupPanel = false;
		this.gv_Cancel.UseBatchMod = false;
		this.tab_Closed.Controls.Add(this.panelControl5);
		this.tab_Closed.EditControl = null;
		this.tab_Closed.EditValue = "Closed";
		this.tab_Closed.FieldName = null;
		this.tab_Closed.IsSearchControl = false;
		this.tab_Closed.Name = "tab_Closed";
		this.tab_Closed.ReadOnly = false;
		this.tab_Closed.SettingInVisible = false;
		this.tab_Closed.SettingReadonly = false;
		this.tab_Closed.Size = new System.Drawing.Size(628, 418);
		this.tab_Closed.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Closed.Text = "Closed";
		this.panelControl5.Controls.Add(this.btnClose);
		this.panelControl5.Controls.Add(this.grd_Close);
		this.panelControl5.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl5.Location = new System.Drawing.Point(0, 0);
		this.panelControl5.Name = "panelControl5";
		this.panelControl5.Size = new System.Drawing.Size(628, 418);
		this.panelControl5.TabIndex = 1;
		this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnClose.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnClose.Appearance.Options.UseBackColor = true;
		this.btnClose.EditControl = null;
		this.btnClose.EditValue = "Close";
		this.btnClose.FieldName = null;
		this.btnClose.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnClose.ImageOptions.Image");
		this.btnClose.IsSearchControl = false;
		this.btnClose.Location = new System.Drawing.Point(10, 393);
		this.btnClose.Name = "btnClose";
		this.btnClose.ReadOnly = false;
		this.btnClose.SettingInVisible = false;
		this.btnClose.SettingReadonly = false;
		this.btnClose.Size = new System.Drawing.Size(99, 20);
		this.btnClose.TabIndex = 21;
		this.btnClose.Text = "Close";
		this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
		this.grd_Close.AllowPasteExcel = false;
		this.grd_Close.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Close.ClipboardData = null;
		this.grd_Close.Location = new System.Drawing.Point(0, 0);
		this.grd_Close.MainView = this.gv_Close;
		this.grd_Close.Name = "grd_Close";
		this.grd_Close.Size = new System.Drawing.Size(628, 385);
		this.grd_Close.TabIndex = 8;
		this.grd_Close.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Close });
		this.gv_Close.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Close.ColumnKey = null;
		this.gv_Close.DisableFormSetting = false;
		this.gv_Close.DisplayException = false;
		this.gv_Close.EditColumn = null;
		this.gv_Close.FieldCurrency = null;
		this.gv_Close.FieldRate = null;
		this.gv_Close.FormId = "oGenPoFromMrp";
		this.gv_Close.GridControl = this.grd_Close;
		this.gv_Close.IgnoreFilter = false;
		this.gv_Close.IndexEditRow = -1;
		this.gv_Close.IndicatorWidth = 40;
		this.gv_Close.IsApzTable = false;
		this.gv_Close.IsInsertNewRow = false;
		this.gv_Close.IsLoadingData = false;
		this.gv_Close.IsMappingContextAction = false;
		this.gv_Close.ItemId = "OCLOSED";
		this.gv_Close.LstColumnAllowBatch = null;
		this.gv_Close.LstColumnNotAllowBatch = null;
		this.gv_Close.LstColumnNotEditable = null;
		this.gv_Close.LstColumNotSetting = null;
		this.gv_Close.Name = "gv_Close";
		this.gv_Close.OptionsView.ColumnAutoWidth = false;
		this.gv_Close.OptionsView.ShowGroupPanel = false;
		this.gv_Close.UseBatchMod = false;
		this.btnLoad.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnLoad.Appearance.Options.UseBackColor = true;
		this.btnLoad.EditControl = null;
		this.btnLoad.EditValue = "Load";
		this.btnLoad.FieldName = null;
		this.btnLoad.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnLoad.ImageOptions.Image");
		this.btnLoad.IsSearchControl = false;
		this.btnLoad.Location = new System.Drawing.Point(289, 6);
		this.btnLoad.Name = "btnLoad";
		this.btnLoad.ReadOnly = false;
		this.btnLoad.SettingInVisible = false;
		this.btnLoad.SettingReadonly = false;
		this.btnLoad.Size = new System.Drawing.Size(99, 20);
		this.btnLoad.TabIndex = 16;
		this.btnLoad.Text = "Load";
		this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(630, 476);
		base.Controls.Add(this.btnLoad);
		base.Controls.Add(this.tabControl);
		base.Controls.Add(this.txt_MRP);
		base.Controls.Add(this.labelLine1);
		base.FormId = "oGenPoFromMrp";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_GenPOFromMRP";
		this.Text = "Production Order From MRP";
		((System.ComponentModel.ISupportInitialize)this.txt_MRP.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl).EndInit();
		this.tabControl.ResumeLayout(false);
		this.tab_Plan.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Plan).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Plan).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Whse).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Project).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		this.tab_Release.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Release).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Release).EndInit();
		this.tab_Report.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Report).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Report).EndInit();
		this.tab_Cancel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl4).EndInit();
		this.panelControl4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Cancel).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Cancel).EndInit();
		this.tab_Closed.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl5).EndInit();
		this.panelControl5.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Close).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Close).EndInit();
		base.ResumeLayout(false);
	}
}
