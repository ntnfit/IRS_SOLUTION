using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
using Apzon.Forms.Helper;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Production;

public class frm_ProdOrderImpToolSetTime : XtraFormEx
{
	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	private readonly int _startDataRow = 1;

	public bool isSaved;

	private IContainer components;

	private GridControlEx grdEx_Doc;

	private GridViewEx gvEx_Doc;

	private SimpleButtonEx btn_Close;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private SimpleButtonEx btn_Save;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeRowsToolStripMenuItem;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private TextEditEx txt_CodeFG;

	private LabelLine lb_Name;

	private SimpleButtonEx btn_Browse;

	private GridColumnEx gridColumnEx6;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btn_Close.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_ProdOrderImpToolSetTime()
	{
		InitializeComponent();
	}

	private void frm_ProdOrderImpToolSetTime_Load(object sender, EventArgs e)
	{
		try
		{
			_isLoading = true;
			AddNewRow();
			GridColumn gridColumn = gvEx_Doc.Columns["U_Time"];
			if (gridColumn != null)
			{
				gridColumn.UnboundType = UnboundColumnType.String;
				RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
				repositoryItemTextEdit.DisplayFormat.FormatType = FormatType.Custom;
				repositoryItemTextEdit.DisplayFormat.FormatString = "##:##";
				repositoryItemTextEdit.EditFormat.FormatType = FormatType.Custom;
				repositoryItemTextEdit.EditFormat.FormatString = "##:##";
				repositoryItemTextEdit.Mask.EditMask = "(0?\\d|1\\d|2[0-3])[0-5]\\d";
				repositoryItemTextEdit.Mask.MaskType = MaskType.RegEx;
				gridColumn.ColumnEdit = repositoryItemTextEdit;
			}
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Close_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		EnableDisableControl(enable: false);
		_isLoading = true;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				DataTable dataTable = (grdEx_Doc.DataSource as DataTable).Copy();
				for (int num = dataTable.Rows.Count - 1; num >= 0; num--)
				{
					DataRow dataRow = dataTable.Rows[num];
					if (string.IsNullOrWhiteSpace(Function.ToString(dataRow["U_LotNo"])) || string.IsNullOrWhiteSpace(Function.ToString(dataRow["U_ProductionLine"])))
					{
						dataTable.Rows.Remove(dataRow);
					}
				}
				dataTable.AcceptChanges();
				if (dataTable.IsNotNull())
				{
					WinCommons._OpenProcessing();
					string text = await _httpClient.ProductionOrderImportTool.SaveComp(dataTable);
					WinCommons._CloseProcessing();
					if (string.IsNullOrEmpty(text))
					{
						WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
						FormMode = Enums.FormViewModeType.View;
						grdEx_Doc.DataSource = null;
						grdEx_Doc.RefreshDataSource();
						AddNewRow();
						isSaved = true;
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text);
						EnableDisableControl(enable: true);
						_isLoading = false;
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
					EnableDisableControl(enable: true);
					_isLoading = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableControl(enable: true);
	}

	private void grdEx_Doc_ProcessGridKey(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				WinCommons._OpenProcessing();
				ExtenseionMethods.PasteFromExcelToDataTable(grdEx_Doc, gvEx_Doc, new List<string> { "U_LotNo", "U_PlnCode", "U_ProductionLine" });
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void removeRowsToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grdEx_Doc.DataSource as DataTable;
			gvEx_Doc.DeleteSelectedRows();
			dataTable?.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		AddNewRow();
	}

	private void gvEx_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Browse_Click(object sender, EventArgs e)
	{
		try
		{
			grdEx_Doc.DataSource = null;
			grdEx_Doc.RefreshDataSource();
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Excels file (*.xlsx)|*.xlsx";
			openFileDialog.Multiselect = false;
			openFileDialog.ShowDialog(this);
			if (File.Exists(openFileDialog.FileName))
			{
				Tuple<string, DataTable> dataFromExcelFile = GetDataFromExcelFile(openFileDialog.FileName);
				if (dataFromExcelFile.Item1 == "")
				{
					grdEx_Doc.DataSource = dataFromExcelFile.Item2;
					grdEx_Doc.RefreshDataSource();
					FormMode = Enums.FormViewModeType.Update;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, dataFromExcelFile.Item1);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRow()
	{
		try
		{
			DataTable dataTable = grdEx_Doc.DataSource as DataTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = GetDefaultTable();
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["U_LotNo"])) && string.IsNullOrWhiteSpace(Function.ToString(r["U_PlnCode"]))))
			{
				DataRow row = dataTable.NewRow();
				dataTable.Rows.Add(row);
				dataTable.AcceptChanges();
			}
			grdEx_Doc.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private Tuple<string, DataTable> GetDataFromExcelFile(string path)
	{
		try
		{
			DataTable dataTableFromExcel = ExcelHelper.GetDataTableFromExcel(path, hasHeader: false, _startDataRow);
			if (dataTableFromExcel != null && dataTableFromExcel.Rows.Count > 0)
			{
				dataTableFromExcel.TableName = "Data";
				DataTable defaultTable = GetDefaultTable();
				for (int i = 0; i < dataTableFromExcel.Rows.Count; i++)
				{
					string plnCode = Function.ToString(dataTableFromExcel.Rows[i][0]);
					string lotNo = Function.ToString(dataTableFromExcel.Rows[i][1]);
					string productionLine = Function.ToString(dataTableFromExcel.Rows[i][2]);
					if (!defaultTable.AsEnumerable().Any((DataRow r) => Function.ToString(r["U_PlnCode"]) == plnCode && Function.ToString(r["U_LotNo"]) == lotNo && Function.ToString(r["U_ProductionLine"]) == productionLine))
					{
						DataRow dataRow = defaultTable.NewRow();
						dataRow["U_PlnCode"] = plnCode;
						dataRow["U_LotNo"] = lotNo;
						dataRow["U_ProductionLine"] = productionLine;
						DateTime? dateTime = Function.ParseDateTime(dataTableFromExcel.Rows[i][3], "yyyy-MM-dd");
						DateTime dateTime2;
						if (dateTime.HasValue)
						{
							_ = dateTime.Value;
							dateTime2 = dateTime.Value;
						}
						else
						{
							dateTime2 = new DateTime(1900, 1, 1);
						}
						dataRow["U_Date"] = dateTime2;
						dataRow["U_Shift"] = Function.ToString(dataTableFromExcel.Rows[i][4]);
						dataRow["U_Time"] = Function.ToString(dataTableFromExcel.Rows[i][5]);
						defaultTable.Rows.Add(dataRow);
					}
				}
				defaultTable.AcceptChanges();
				return new Tuple<string, DataTable>("", defaultTable);
			}
			return new Tuple<string, DataTable>("File contains no data", new DataTable());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new Tuple<string, DataTable>(ex.Message, new DataTable());
		}
	}

	private DataTable GetDefaultTable()
	{
		try
		{
			DataTable dataTable = new DataTable();
			DataColumn dataColumn = new DataColumn("U_PlnCode");
			DataColumn dataColumn2 = new DataColumn("U_LotNo");
			DataColumn dataColumn3 = new DataColumn("U_ProductionLine");
			DataColumn dataColumn4 = new DataColumn("U_Date");
			DataColumn dataColumn5 = new DataColumn("U_Shift");
			DataColumn dataColumn6 = new DataColumn("U_Time");
			dataColumn.DataType = typeof(string);
			dataColumn2.DataType = typeof(string);
			dataColumn3.DataType = typeof(string);
			dataColumn4.DataType = typeof(DateTime);
			dataColumn5.DataType = typeof(string);
			dataColumn6.DataType = typeof(string);
			dataTable.Columns.AddRange(new DataColumn[6] { dataColumn, dataColumn2, dataColumn3, dataColumn4, dataColumn5, dataColumn6 });
			dataTable.AcceptChanges();
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private void EnableDisableControl(bool enable)
	{
		btn_Save.Enabled = enable;
		btn_Close.Enabled = enable;
		btn_Browse.Enabled = enable;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Production.frm_ProdOrderImpToolSetTime));
		this.grdEx_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvEx_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Close = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_CodeFG = new Apzon.Controls.Editor.TextEditEx();
		this.lb_Name = new Apzon.Controls.Controls.LabelLine();
		this.btn_Browse = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grdEx_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvEx_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CodeFG.Properties).BeginInit();
		base.SuspendLayout();
		this.grdEx_Doc.AllowPasteExcel = true;
		this.grdEx_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdEx_Doc.ClipboardData = null;
		this.grdEx_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdEx_Doc.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdEx_Doc.Location = new System.Drawing.Point(10, 39);
		this.grdEx_Doc.MainView = this.gvEx_Doc;
		this.grdEx_Doc.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdEx_Doc.Name = "grdEx_Doc";
		this.grdEx_Doc.Size = new System.Drawing.Size(522, 287);
		this.grdEx_Doc.TabIndex = 11;
		this.grdEx_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvEx_Doc });
		this.grdEx_Doc.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(this.grdEx_Doc_ProcessGridKey);
		this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.removeRowsToolStripMenuItem, this.addRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(149, 48);
		this.removeRowsToolStripMenuItem.Name = "removeRowsToolStripMenuItem";
		this.removeRowsToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
		this.removeRowsToolStripMenuItem.Text = "Remove Rows";
		this.removeRowsToolStripMenuItem.Click += new System.EventHandler(this.removeRowsToolStripMenuItem_Click);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.gvEx_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvEx_Doc.ColumnKey = null;
		this.gvEx_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx6, this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5 });
		this.gvEx_Doc.DisableFormSetting = false;
		this.gvEx_Doc.DisplayException = false;
		this.gvEx_Doc.EditColumn = null;
		this.gvEx_Doc.FieldCurrency = null;
		this.gvEx_Doc.FieldRate = null;
		this.gvEx_Doc.FormId = "oProductionImpToolSetTime";
		this.gvEx_Doc.GridControl = this.grdEx_Doc;
		this.gvEx_Doc.IgnoreFilter = false;
		this.gvEx_Doc.IndexEditRow = -1;
		this.gvEx_Doc.IndicatorWidth = 70;
		this.gvEx_Doc.IsApzTable = false;
		this.gvEx_Doc.IsInsertNewRow = false;
		this.gvEx_Doc.IsLoadingData = false;
		this.gvEx_Doc.IsMappingContextAction = false;
		this.gvEx_Doc.ItemId = "ODATA";
		this.gvEx_Doc.LstColumnAllowBatch = null;
		this.gvEx_Doc.LstColumnNotAllowBatch = null;
		this.gvEx_Doc.LstColumnNotEditable = null;
		this.gvEx_Doc.LstColumNotSetting = null;
		this.gvEx_Doc.Name = "gvEx_Doc";
		this.gvEx_Doc.OptionsSelection.MultiSelect = true;
		this.gvEx_Doc.OptionsView.ColumnAutoWidth = false;
		this.gvEx_Doc.OptionsView.ShowGroupPanel = false;
		this.gvEx_Doc.UseBatchMod = false;
		this.gvEx_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvEx_Doc_CellValueChanged);
		this.gridColumnEx6.Caption = "Week No.";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "U_PlnCode";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 0;
		this.gridColumnEx1.Caption = "Lot No.";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "U_LotNo";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 1;
		this.gridColumnEx2.Caption = "Line";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "U_ProductionLine";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 2;
		this.gridColumnEx3.Caption = "Date";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "U_Date";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 3;
		this.gridColumnEx4.Caption = "Shift";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "U_Shift";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 4;
		this.gridColumnEx5.Caption = "Time";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "U_Time";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 5;
		this.btn_Close.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Close.EditControl = null;
		this.btn_Close.EditValue = "Close";
		this.btn_Close.FieldName = null;
		this.btn_Close.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Close.ImageOptions.Image");
		this.btn_Close.IsSearchControl = false;
		this.btn_Close.Location = new System.Drawing.Point(77, 330);
		this.btn_Close.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_Close.Name = "btn_Close";
		this.btn_Close.ReadOnly = false;
		this.btn_Close.SettingInVisible = false;
		this.btn_Close.SettingReadonly = false;
		this.btn_Close.Size = new System.Drawing.Size(64, 24);
		this.btn_Close.TabIndex = 12;
		this.btn_Close.Text = "Close";
		this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(10, 330);
		this.btn_Save.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(64, 24);
		this.btn_Save.TabIndex = 13;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.txt_CodeFG.EditControl = null;
		this.txt_CodeFG.FieldName = null;
		this.txt_CodeFG.IsSearchControl = false;
		this.txt_CodeFG.Location = new System.Drawing.Point(124, 10);
		this.txt_CodeFG.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.txt_CodeFG.Name = "txt_CodeFG";
		this.txt_CodeFG.SettingInVisible = false;
		this.txt_CodeFG.SettingReadonly = false;
		this.txt_CodeFG.Size = new System.Drawing.Size(364, 20);
		this.txt_CodeFG.TabIndex = 14;
		this.lb_Name.EditControl = null;
		this.lb_Name.EditValue = "File Name";
		this.lb_Name.FieldName = null;
		this.lb_Name.HideWhenEmpty = false;
		this.lb_Name.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Name.Location = new System.Drawing.Point(12, 13);
		this.lb_Name.Margin = new System.Windows.Forms.Padding(4);
		this.lb_Name.Name = "lb_Name";
		this.lb_Name.ReadOnly = false;
		this.lb_Name.SettingInVisible = false;
		this.lb_Name.SettingReadonly = false;
		this.lb_Name.ShowUnderLine = true;
		this.lb_Name.Size = new System.Drawing.Size(119, 20);
		this.lb_Name.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Name.TabIndex = 15;
		this.lb_Name.Utext = "File Name";
		this.btn_Browse.EditControl = null;
		this.btn_Browse.EditValue = "...";
		this.btn_Browse.FieldName = null;
		this.btn_Browse.IsSearchControl = false;
		this.btn_Browse.Location = new System.Drawing.Point(489, 10);
		this.btn_Browse.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_Browse.Name = "btn_Browse";
		this.btn_Browse.ReadOnly = false;
		this.btn_Browse.SettingInVisible = false;
		this.btn_Browse.SettingReadonly = false;
		this.btn_Browse.Size = new System.Drawing.Size(43, 20);
		this.btn_Browse.TabIndex = 16;
		this.btn_Browse.Text = "...";
		this.btn_Browse.Click += new System.EventHandler(this.btn_Browse_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(543, 359);
		base.Controls.Add(this.btn_Browse);
		base.Controls.Add(this.txt_CodeFG);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btn_Close);
		base.Controls.Add(this.grdEx_Doc);
		base.Controls.Add(this.lb_Name);
		base.FormId = "oProductionImpToolSetTime";
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "frm_ProdOrderImpToolSetTime";
		this.Text = "Set Time";
		base.Load += new System.EventHandler(this.frm_ProdOrderImpToolSetTime_Load);
		((System.ComponentModel.ISupportInitialize)this.grdEx_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvEx_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CodeFG.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
