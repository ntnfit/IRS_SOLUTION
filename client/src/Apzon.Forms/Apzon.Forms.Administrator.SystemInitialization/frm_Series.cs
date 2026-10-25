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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator.SystemInitialization;

public class frm_Series : XtraFormEx
{
	public delegate void ReloadForm();

	private const string Controller = "Series";

	private readonly string _obCode;

	private readonly string _docType;

	private readonly string _seriesType;

	public Action<int> SetDefaultSeries;

	public ReloadForm Fill;

	private IContainer components;

	private GridControlEx grd_Doc;

	private AdvBandedGridViewEx adv_Doc;

	private BandedGridColumn bandedGridColumn1;

	private BandedGridColumn bandedGridColumn2;

	private BandedGridColumn bandedGridColumn3;

	private BandedGridColumn bandedGridColumn5;

	private BandedGridColumn bandedGridColumn6;

	private BandedGridColumn bandedGridColumn4;

	private BandedGridColumn bandedGridColumn7;

	private BandedGridColumn bandedGridColumn8;

	private BandedGridColumn bandedGridColumn9;

	private BandedGridColumn bandedGridColumn10;

	private BandedGridColumn bandedGridColumn11;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private RepositoryItemComboBox repositoryItemComboBox1;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private Label label1;

	private BandedGridColumn bandedGridColumn12;

	private BandedGridColumn bandedGridColumn14;

	private BandedGridColumn bandedGridColumn13;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private ComboBoxEditEx cb_Period;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private RepositoryItemSpinEdit repositoryItemSpinEdit1;

	private SimpleButtonEx btn_Default;

	private CheckEditEx checkEdit2;

	private BandedGridColumn bandedGridColumn15;

	private GridBand A;

	private GridBand gridBand1;

	private GridBand gridband_String;

	private GridBand gridBand3;

	private BandedGridColumn bandedGridColumn16;

	public DataTable DataSeries { get; set; }

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
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_Series(string obCode, string docType, string seriesType)
	{
		_obCode = obCode;
		_docType = docType;
		_seriesType = seriesType;
		InitializeComponent();
	}

	private void LoadInItForm()
	{
		try
		{
			string text = DateTime.Today.ToString("yyyy");
			string text2 = Function.ToString(Function.ParseInt(text) - 1);
			for (int i = 1; i <= 12; i++)
			{
				string item = "Year " + Function.ParseDateTimes(i + "/" + text2).ToString("yyyy-MM");
				cb_Period.Properties.Items.Add(item);
			}
			for (int j = 1; j <= 12; j++)
			{
				string item2 = "Year " + Function.ParseDateTimes(j + "/" + text).ToString("yyyy-MM");
				cb_Period.Properties.Items.Add(item2);
			}
			cb_Period.Properties.Items.Add("All Periods");
			cb_Period.SelectedIndex = 24;
			if (_seriesType == "D")
			{
				adv_Doc.Columns["NumSize"].OptionsColumn.ReadOnly = true;
				adv_Doc.Columns["BeginStr"].OptionsColumn.ReadOnly = true;
				adv_Doc.Columns["EndStr"].OptionsColumn.ReadOnly = true;
				adv_Doc.Columns["NumSize"].Visible = false;
				adv_Doc.Columns["BeginStr"].Visible = false;
				adv_Doc.Columns["EndStr"].Visible = false;
			}
			else
			{
				adv_Doc.Columns["NumSize"].OptionsColumn.ReadOnly = false;
				adv_Doc.Columns["BeginStr"].OptionsColumn.ReadOnly = false;
				adv_Doc.Columns["EndStr"].OptionsColumn.ReadOnly = false;
				adv_Doc.Columns["NumSize"].Visible = true;
				adv_Doc.Columns["BeginStr"].Visible = true;
				adv_Doc.Columns["EndStr"].Visible = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			DataTable dataSource;
			if (DataSeries.IsNotNull())
			{
				dataSource = DataSeries.Copy();
			}
			else
			{
				using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Series");
				dataSource = await helper.GetAsync("LoadDoc", new
				{
					objCode = _obCode,
					docSubType = _docType,
					userId = GlobalData.UserID
				});
			}
			grd_Doc.DataSource = dataSource;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				DataSeries = grd_Doc.DataSourceTable.Copy();
				DialogResult = DialogResult.Yes;
				Close();
			}
			else
			{
				DialogResult = DialogResult.No;
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridControl1_Click(object sender, EventArgs e)
	{
	}

	private async void frm_Series_Load(object sender, EventArgs e)
	{
		try
		{
			LoadInItForm();
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void adv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
		try
		{
			if (e.Column.FieldName.Equals("InitialNum"))
			{
				int num = Function.ParseInt(e.Value);
				adv_Doc.SetRowCellValue(e.RowHandle, "NextNumber", num);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void adv_Doc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_Series_FormClosing(object sender, FormClosingEventArgs e)
	{
	}

	private void adv_Doc_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			DataRow dataRow = dataTable.NewRow();
			dataRow["GroupCode"] = 1;
			dataRow["Indicator"] = "Default";
			dataRow["IsManual"] = "N";
			dataRow["ObjectCode"] = _obCode;
			dataRow["DocSubType"] = _docType;
			dataRow["Series"] = -1;
			dataRow["SeriesType"] = _seriesType;
			dataRow["DefESeries"] = 0;
			dataRow["IsDigSerie"] = "N";
			dataRow["IsForCncl"] = "N";
			dataRow["IsElAuth"] = "N";
			dataRow["Locked"] = "N";
			dataRow["YearTransf"] = "N";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void simpleButton1_Click(object sender, EventArgs e)
	{
		try
		{
			foreach (DataRow row in grd_Doc.DataSourceTable.Rows)
			{
				row["IsDflt"] = "N";
			}
			grd_Doc.DataSourceTable.AcceptChanges();
			adv_Doc.SetFocusedRowCellValue("IsDflt", "Y");
			SetDefaultSeries(Function.ParseInt(adv_Doc.GetFocusedRowCellValue("Series")));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void adv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			string text = Function.ToString(adv_Doc.GetFocusedRowCellValue("IsManual"));
			e.Cancel = text == "Y";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void adv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (Function.ToString(adv_Doc.GetRowCellValue(e.RowHandle, "IsManual")) == "Y")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			if (e.Column.OptionsColumn.ReadOnly)
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			if (Function.ToString(adv_Doc.GetRowCellValue(e.RowHandle, "IsDflt")).Equals("Y"))
			{
				e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SystemInitialization.frm_Series));
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.adv_Doc = new Apzon.Controls.Grid.AdvBandedGridViewEx();
		this.A = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		this.bandedGridColumn1 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.gridBand1 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		this.bandedGridColumn2 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.repositoryItemSpinEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
		this.bandedGridColumn3 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.bandedGridColumn4 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.gridband_String = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		this.bandedGridColumn5 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.bandedGridColumn6 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.gridBand3 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		this.bandedGridColumn7 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.bandedGridColumn8 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.bandedGridColumn15 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.bandedGridColumn9 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.bandedGridColumn10 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.bandedGridColumn11 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.bandedGridColumn12 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.bandedGridColumn14 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.bandedGridColumn13 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.bandedGridColumn16 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.label1 = new System.Windows.Forms.Label();
		this.cb_Period = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.btn_Default = new Apzon.Controls.Controls.SimpleButtonEx();
		this.checkEdit2 = new Apzon.Controls.Editor.CheckEditEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.adv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemSpinEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Period.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.checkEdit2.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Location = new System.Drawing.Point(-2, 0);
		this.grd_Doc.MainView = this.adv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.repositoryItemCheckEdit1, this.repositoryItemComboBox1, this.repositoryItemCalcEdit1, this.repositoryItemSpinEdit1 });
		this.grd_Doc.Size = new System.Drawing.Size(1302, 484);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.adv_Doc });
		this.grd_Doc.Click += new System.EventHandler(this.gridControl1_Click);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.addRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(123, 26);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.adv_Doc.Appearance.BandPanel.Options.UseTextOptions = true;
		this.adv_Doc.Appearance.BandPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.adv_Doc.Bands.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[4] { this.A, this.gridBand1, this.gridband_String, this.gridBand3 });
		this.adv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.adv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn[16]
		{
			this.bandedGridColumn1, this.bandedGridColumn2, this.bandedGridColumn3, this.bandedGridColumn4, this.bandedGridColumn5, this.bandedGridColumn6, this.bandedGridColumn7, this.bandedGridColumn8, this.bandedGridColumn9, this.bandedGridColumn10,
			this.bandedGridColumn11, this.bandedGridColumn12, this.bandedGridColumn14, this.bandedGridColumn13, this.bandedGridColumn15, this.bandedGridColumn16
		});
		this.adv_Doc.FormId = null;
		this.adv_Doc.GridControl = this.grd_Doc;
		this.adv_Doc.IndicatorWidth = 30;
		this.adv_Doc.IsApzTable = false;
		this.adv_Doc.ItemId = null;
		this.adv_Doc.Name = "adv_Doc";
		this.adv_Doc.OptionsView.ShowGroupPanel = false;
		this.adv_Doc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.adv_Doc_CustomDrawRowIndicator);
		this.adv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.adv_Doc_RowCellStyle);
		this.adv_Doc.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.adv_Doc_CustomRowCellEdit);
		this.adv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.adv_Doc_ShowingEditor);
		this.adv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.adv_Doc_CellValueChanged);
		this.A.Columns.Add(this.bandedGridColumn1);
		this.A.Name = "A";
		this.A.VisibleIndex = 0;
		this.A.Width = 157;
		this.bandedGridColumn1.Caption = "Name";
		this.bandedGridColumn1.FieldName = "SeriesName";
		this.bandedGridColumn1.Name = "bandedGridColumn1";
		this.bandedGridColumn1.Visible = true;
		this.bandedGridColumn1.Width = 157;
		this.gridBand1.Caption = "Number";
		this.gridBand1.Columns.Add(this.bandedGridColumn2);
		this.gridBand1.Columns.Add(this.bandedGridColumn3);
		this.gridBand1.Columns.Add(this.bandedGridColumn4);
		this.gridBand1.Name = "gridBand1";
		this.gridBand1.VisibleIndex = 1;
		this.gridBand1.Width = 347;
		this.bandedGridColumn2.Caption = "First No";
		this.bandedGridColumn2.ColumnEdit = this.repositoryItemSpinEdit1;
		this.bandedGridColumn2.DisplayFormat.FormatString = "n0";
		this.bandedGridColumn2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.bandedGridColumn2.FieldName = "InitialNum";
		this.bandedGridColumn2.Name = "bandedGridColumn2";
		this.bandedGridColumn2.Visible = true;
		this.bandedGridColumn2.Width = 115;
		this.repositoryItemSpinEdit1.AutoHeight = false;
		this.repositoryItemSpinEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemSpinEdit1.DisplayFormat.FormatString = "n0";
		this.repositoryItemSpinEdit1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemSpinEdit1.MaxValue = new decimal(new int[4] { 2147483647, 0, 0, 0 });
		this.repositoryItemSpinEdit1.Name = "repositoryItemSpinEdit1";
		this.bandedGridColumn3.Caption = "Next No.";
		this.bandedGridColumn3.ColumnEdit = this.repositoryItemSpinEdit1;
		this.bandedGridColumn3.DisplayFormat.FormatString = "n0";
		this.bandedGridColumn3.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.bandedGridColumn3.FieldName = "NextNumber";
		this.bandedGridColumn3.Name = "bandedGridColumn3";
		this.bandedGridColumn3.OptionsColumn.ReadOnly = true;
		this.bandedGridColumn3.Visible = true;
		this.bandedGridColumn3.Width = 115;
		this.bandedGridColumn4.Caption = "Last No.";
		this.bandedGridColumn4.ColumnEdit = this.repositoryItemSpinEdit1;
		this.bandedGridColumn4.DisplayFormat.FormatString = "n0";
		this.bandedGridColumn4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.bandedGridColumn4.FieldName = "LastNum";
		this.bandedGridColumn4.Name = "bandedGridColumn4";
		this.bandedGridColumn4.Visible = true;
		this.bandedGridColumn4.Width = 117;
		this.gridband_String.Caption = "String";
		this.gridband_String.Columns.Add(this.bandedGridColumn5);
		this.gridband_String.Columns.Add(this.bandedGridColumn6);
		this.gridband_String.Name = "gridband_String";
		this.gridband_String.VisibleIndex = 2;
		this.gridband_String.Width = 215;
		this.bandedGridColumn5.Caption = "Prefix";
		this.bandedGridColumn5.FieldName = "BeginStr";
		this.bandedGridColumn5.Name = "bandedGridColumn5";
		this.bandedGridColumn5.Visible = true;
		this.bandedGridColumn5.Width = 106;
		this.bandedGridColumn6.Caption = "Suffix";
		this.bandedGridColumn6.FieldName = "EndStr";
		this.bandedGridColumn6.Name = "bandedGridColumn6";
		this.bandedGridColumn6.Visible = true;
		this.bandedGridColumn6.Width = 109;
		this.gridBand3.Columns.Add(this.bandedGridColumn7);
		this.gridBand3.Columns.Add(this.bandedGridColumn8);
		this.gridBand3.Columns.Add(this.bandedGridColumn15);
		this.gridBand3.Columns.Add(this.bandedGridColumn9);
		this.gridBand3.Columns.Add(this.bandedGridColumn10);
		this.gridBand3.Columns.Add(this.bandedGridColumn11);
		this.gridBand3.Name = "gridBand3";
		this.gridBand3.VisibleIndex = 3;
		this.gridBand3.Width = 583;
		this.bandedGridColumn7.Caption = "Remarks";
		this.bandedGridColumn7.FieldName = "Remark";
		this.bandedGridColumn7.Name = "bandedGridColumn7";
		this.bandedGridColumn7.Visible = true;
		this.bandedGridColumn7.Width = 124;
		this.bandedGridColumn8.Caption = "Group";
		this.bandedGridColumn8.ColumnEdit = this.repositoryItemComboBox1;
		this.bandedGridColumn8.FieldName = "GroupCode";
		this.bandedGridColumn8.Name = "bandedGridColumn8";
		this.bandedGridColumn8.Visible = true;
		this.bandedGridColumn8.Width = 68;
		this.repositoryItemComboBox1.AutoHeight = false;
		this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemComboBox1.Items.AddRange(new object[10] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
		this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
		this.bandedGridColumn15.Caption = "No. Of Digits";
		this.bandedGridColumn15.FieldName = "NumSize";
		this.bandedGridColumn15.Name = "bandedGridColumn15";
		this.bandedGridColumn15.Visible = true;
		this.bandedGridColumn15.Width = 83;
		this.bandedGridColumn9.Caption = "Period Ind.";
		this.bandedGridColumn9.FieldName = "Indicator";
		this.bandedGridColumn9.Name = "bandedGridColumn9";
		this.bandedGridColumn9.Visible = true;
		this.bandedGridColumn9.Width = 148;
		this.bandedGridColumn10.Caption = "Cancellation";
		this.bandedGridColumn10.ColumnEdit = this.repositoryItemCheckEdit1;
		this.bandedGridColumn10.FieldName = "IsForCncl";
		this.bandedGridColumn10.Name = "bandedGridColumn10";
		this.bandedGridColumn10.Width = 103;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.bandedGridColumn11.Caption = "Lock";
		this.bandedGridColumn11.ColumnEdit = this.repositoryItemCheckEdit1;
		this.bandedGridColumn11.FieldName = "Locked";
		this.bandedGridColumn11.Name = "bandedGridColumn11";
		this.bandedGridColumn11.Visible = true;
		this.bandedGridColumn11.Width = 160;
		this.bandedGridColumn12.Caption = "ObjectCode";
		this.bandedGridColumn12.FieldName = "ObjectCode";
		this.bandedGridColumn12.Name = "bandedGridColumn12";
		this.bandedGridColumn14.Caption = "DocSubType";
		this.bandedGridColumn14.FieldName = "DocSubType";
		this.bandedGridColumn14.Name = "bandedGridColumn14";
		this.bandedGridColumn13.Caption = "Series";
		this.bandedGridColumn13.FieldName = "Series";
		this.bandedGridColumn13.Name = "bandedGridColumn13";
		this.bandedGridColumn16.Caption = "Is Default";
		this.bandedGridColumn16.FieldName = "IsDflt";
		this.bandedGridColumn16.Name = "bandedGridColumn16";
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(83, 490);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 4;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(5, 490);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 20);
		this.btn_OK.TabIndex = 3;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(432, 494);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(255, 13);
		this.label1.TabIndex = 5;
		this.label1.Text = "Display Series Linked to Selected Period(s) Indicator";
		this.label1.Visible = false;
		this.cb_Period.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.cb_Period.EditControl = null;
		this.cb_Period.FieldName = null;
		this.cb_Period.IsSearchControl = false;
		this.cb_Period.Location = new System.Drawing.Point(693, 490);
		this.cb_Period.Name = "cb_Period";
		this.cb_Period.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_Period.Properties.PopupSizeable = true;
		this.cb_Period.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cb_Period.SettingInVisible = false;
		this.cb_Period.SettingReadonly = false;
		this.cb_Period.Size = new System.Drawing.Size(165, 20);
		this.cb_Period.TabIndex = 6;
		this.cb_Period.Visible = false;
		this.btn_Default.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Default.EditControl = null;
		this.btn_Default.EditValue = "Set as Default";
		this.btn_Default.FieldName = null;
		this.btn_Default.ImageOptions.Image = Apzon.Forms.Properties.Resources.withtextwrapping_topright_16x16;
		this.btn_Default.IsSearchControl = false;
		this.btn_Default.Location = new System.Drawing.Point(1129, 490);
		this.btn_Default.Name = "btn_Default";
		this.btn_Default.ReadOnly = false;
		this.btn_Default.SettingInVisible = false;
		this.btn_Default.SettingReadonly = false;
		this.btn_Default.Size = new System.Drawing.Size(165, 20);
		this.btn_Default.TabIndex = 7;
		this.btn_Default.Text = "Set as Default";
		this.btn_Default.Click += new System.EventHandler(this.simpleButton1_Click);
		this.checkEdit2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.checkEdit2.EditControl = null;
		this.checkEdit2.FieldName = null;
		this.checkEdit2.IsSearchControl = false;
		this.checkEdit2.Location = new System.Drawing.Point(164, 491);
		this.checkEdit2.Name = "checkEdit2";
		this.checkEdit2.Properties.Caption = "Display Confirmation Message when Adding Business Partner for Non-Manual Series";
		this.checkEdit2.SettingInVisible = false;
		this.checkEdit2.SettingReadonly = false;
		this.checkEdit2.Size = new System.Drawing.Size(262, 20);
		this.checkEdit2.TabIndex = 9;
		this.checkEdit2.Visible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1298, 515);
		base.Controls.Add(this.checkEdit2);
		base.Controls.Add(this.btn_Default);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_Doc);
		base.Controls.Add(this.cb_Period);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_Series.IconOptions.Icon");
		base.Name = "frm_Series";
		this.Text = "Series";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Series_FormClosing);
		base.Load += new System.EventHandler(this.frm_Series_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.adv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemSpinEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Period.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.checkEdit2.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
