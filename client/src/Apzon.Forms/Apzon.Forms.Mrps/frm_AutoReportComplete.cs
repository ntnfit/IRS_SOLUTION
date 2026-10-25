using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Mrps;

public class frm_AutoReportComplete : XtraFormEx
{
	private const string Controler = "Schedule";

	private IContainer components;

	private XtraTabControlEx tabControl;

	private XtraTabPageEx tab_Manual;

	private PanelControl panelControl1;

	private TextEditEx txtBarCodeManual;

	private LabelLine labelLine1;

	private GridControlEx grd_Manual;

	private GridViewEx gv_Manual;

	private XtraTabPageEx tab_Check;

	private PanelControl panelControl2;

	private TextEditEx txtBarCodeCheck;

	private LabelLine labelLine2;

	private GridControlEx grd_Check;

	private GridViewEx gv_Check;

	private ContextMenuStrip contextMenuStripMenual;

	private ToolStripMenuItem clearToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private SimpleButtonEx btnReport;

	private SimpleButtonEx btnCancel;

	public frm_AutoReportComplete()
	{
		InitializeComponent();
		OnLoadAsync += frm_AutoReportComplete_Load;
		FormMode = Enums.FormViewModeType.View;
	}

	private async Task LoadColumn()
	{
		try
		{
			DataTable dtStruc = await GetStructure();
			gv_Manual.CreateColumByStructure(dtStruc);
			gv_Check.CreateColumByStructure(dtStruc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable GetDataDoc(int branch, int series)
	{
		try
		{
			if (!DocumentsHelper.Document.IsNotNull())
			{
				return null;
			}
			DataTable dataTable = DocumentsHelper.Document.Clone();
			DataRow dataRow = dataTable.NewRow();
			dataRow["DocDate"] = DateTime.Now.Date;
			dataRow["DocDueDate"] = DateTime.Now.Date;
			dataRow["BplID"] = branch;
			dataRow["ObjType"] = 59;
			dataRow["Comments"] = "Receipt from production";
			dataRow["GroupNum"] = -1;
			dataRow["JrnlMemo"] = "Receipt from production";
			dataRow["DocStatus"] = "O";
			dataRow["Series"] = series;
			dataRow["DocRate"] = 1;
			dataRow["DocType"] = "I";
			dataRow["Printed"] = "N";
			dataRow["DocCur"] = GlobalData.MainCurrency;
			dataRow["UpdateDate"] = DateTime.Now.Date;
			dataRow["CreateDate"] = DateTime.Now.Date;
			dataRow["VatDate"] = DateTime.Now.Date;
			dataRow["ReqDate"] = DateTime.Now.Date;
			dataTable.Rows.Add(dataRow);
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	private DataTable GetDataLine()
	{
		try
		{
			if (!DocumentsHelper.Lines.IsNotNull())
			{
				return null;
			}
			DataTable dataTable = DocumentsHelper.Lines.Clone();
			dataTable.Columns.Add(new DataColumn("Barcode", typeof(string)));
			int[] selectedRows = gv_Manual.GetSelectedRows();
			List<DataColumn> list = (from DataColumn t in grd_Manual.DataSourceTable.Columns
				where t.ColumnName.StartsWith("U_")
				select (t)).ToList();
			for (int num = 0; num < selectedRows.Length; num++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["ItemCode"] = gv_Manual.GetRowCellValue(selectedRows[num], "ItemCode");
				dataRow["Dscription"] = gv_Manual.GetRowCellValue(selectedRows[num], "ItemName");
				dataRow["BaseEntry"] = gv_Manual.GetRowCellValue(selectedRows[num], "DocEntry");
				dataRow["BaseType"] = 202;
				dataRow["BaseRef"] = gv_Manual.GetRowCellValue(selectedRows[num], "PONum");
				dataRow["BaseLine"] = -1;
				dataRow["LineNum"] = GetMaxLineNum(dataTable);
				dataRow["Price"] = 0;
				dataRow["Quantity"] = gv_Manual.GetRowCellValue(selectedRows[num], "Quantity");
				dataRow["WhsCode"] = gv_Manual.GetRowCellValue(selectedRows[num], "WhsCode");
				dataRow["Currency"] = GlobalData.MainCurrency;
				dataRow["ShipDate"] = gv_Manual.GetRowCellValue(selectedRows[num], "DueDate");
				dataRow["DocDate"] = DateTime.Now.Date;
				dataRow["Barcode"] = gv_Manual.GetRowCellValue(selectedRows[num], "Barcode");
				if (list.Count > 0)
				{
					for (int num2 = 0; num2 < list.Count; num2++)
					{
						if (list[num2].DataType == typeof(DateTime))
						{
							dataRow[list[num2].ColumnName] = Function.ParseDateTime(gv_Manual.GetRowCellValue(selectedRows[num], list[num2].ColumnName));
						}
						else if (list[num2].DataType == typeof(double) || list[num2].DataType == typeof(decimal))
						{
							dataRow[list[num2].ColumnName] = Function.ParseDouble(gv_Manual.GetRowCellValue(selectedRows[num], list[num2].ColumnName));
						}
						else if (list[num2].DataType == typeof(int) || list[num2].DataType == typeof(short) || list[num2].DataType == typeof(long))
						{
							dataRow[list[num2].ColumnName] = Function.ParseInt(gv_Manual.GetRowCellValue(selectedRows[num], list[num2].ColumnName));
						}
						else
						{
							dataRow[list[num2].ColumnName] = Function.ToString(gv_Manual.GetRowCellValue(selectedRows[num], list[num2].ColumnName));
						}
					}
				}
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	private int GetMaxLineNum(DataTable dt)
	{
		if (!dt.IsNotNull())
		{
			return 0;
		}
		return Function.ParseInt(dt.Compute("MAX(LineNum)", "")) + 1;
	}

	private void EnableDisableControl(bool isEnable)
	{
		try
		{
			txtBarCodeCheck.ReadOnly = !isEnable;
			txtBarCodeManual.ReadOnly = !isEnable;
			btnCancel.Enabled = isEnable;
			btnReport.Enabled = isEnable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetStructure()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Schedule");
			return await http.GetAsync("GetDataStructureAutoreport");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	private async Task frm_AutoReportComplete_Load(object sender, EventArgs e)
	{
		await LoadColumn();
		txtBarCodeManual.Focus();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Report_Click(object sender, EventArgs e)
	{
		EnableDisableControl(isEnable: false);
		try
		{
			if (grd_Manual.DataSourceTable != null && grd_Manual.DataSourceTable.Rows.Count > 0)
			{
				int[] selectedRows = gv_Manual.GetSelectedRows();
				if (selectedRows.Length != 0)
				{
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmReportCompletePo, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
					{
						EnableDisableControl(isEnable: true);
						return;
					}
					WinCommons._OpenProcessing();
					DataTable dataDoc = GetDataDoc(Function.ParseInt(gv_Manual.GetRowCellValue(selectedRows[0], "BPLId")), Function.ParseInt(gv_Manual.GetRowCellValue(selectedRows[0], "Series")));
					DataTable dataLine = GetDataLine();
					if (!dataDoc.IsNotNull() || !dataLine.IsNotNull())
					{
						WinCommons.Error(Enums.MessageCode.CannotGetDataReport, "");
						EnableDisableControl(isEnable: true);
						WinCommons._CloseProcessing();
						return;
					}
					DataSet dataSet = new DataSet();
					dataSet.Tables.Add(dataDoc);
					dataSet.Tables.Add(dataLine);
					using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "GoodReceiptIt");
					HttpResult httpResult = await http.PostAsync("ReportComplete", dataSet);
					if (string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						grd_Manual.DataSource = null;
						grd_Manual.RefreshDataSource();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableControl(isEnable: true);
		WinCommons._CloseProcessing();
		txtBarCodeManual.Focus();
	}

	private void tabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			if (e.Page.Name.Equals("tab_Check"))
			{
				txtBarCodeCheck.Focus();
				if (grd_Check.DataSource != null)
				{
					DataTable dataSourceTable = grd_Check.DataSourceTable;
					if (dataSourceTable != null && dataSourceTable.Rows.Count > 0)
					{
						dataSourceTable.Rows.Clear();
					}
					grd_Check.DataSource = dataSourceTable;
					grd_Check.RefreshDataSource();
				}
				return;
			}
			txtBarCodeManual.Focus();
			if (grd_Manual.DataSource != null)
			{
				DataTable dataSourceTable2 = grd_Manual.DataSourceTable;
				if (dataSourceTable2 != null && dataSourceTable2.Rows.Count > 0)
				{
					dataSourceTable2.Rows.Clear();
				}
				grd_Manual.DataSource = dataSourceTable2;
				grd_Manual.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Manual_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async void txt_BarCodeManual_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode != Keys.Return)
			{
				return;
			}
			if (!string.IsNullOrEmpty(txtBarCodeManual.Text.Trim()))
			{
				if (grd_Manual.DataSourceTable != null)
				{
					DataTable dataSourceTable = grd_Manual.DataSourceTable;
					if (dataSourceTable != null && dataSourceTable.Rows.Count > 0 && dataSourceTable.Select("BarCode = '" + Function.ToString(txtBarCodeManual.Text) + "'").Length != 0)
					{
						txtBarCodeManual.Text = "";
						txtBarCodeManual.Focus();
						WinCommons.Warning(Enums.MessageCode.BarcodeExists, "");
						WinCommons._CloseProcessing();
						return;
					}
				}
				using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Schedule");
				DataTable dataTable = await http.GetAsync("GetBarcodeManual", new
				{
					barcode = Function.ToString(txtBarCodeManual.EditValue)
				});
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					if (grd_Manual.DataSourceTable == null || grd_Manual.DataSourceTable.Rows.Count == 0)
					{
						grd_Manual.DataSource = dataTable;
						grd_Manual.RefreshDataSource();
					}
					else
					{
						DataTable dataSourceTable2 = grd_Manual.DataSourceTable;
						for (int i = 0; i < dataTable.Rows.Count; i++)
						{
							dataSourceTable2.ImportRow(dataTable.Rows[i]);
						}
						grd_Manual.DataSource = dataSourceTable2;
						grd_Manual.RefreshDataSource();
					}
				}
				else
				{
					WinCommons.Warning(Enums.MessageCode.BarcodeDoesNotExists, "");
				}
			}
			gv_Manual.SelectAll();
			txtBarCodeManual.Text = "";
			txtBarCodeManual.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Check_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async void txt_BarCodeCheck_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode != Keys.Return)
			{
				return;
			}
			if (!string.IsNullOrEmpty(txtBarCodeCheck.Text.Trim()))
			{
				using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Schedule");
				DataTable dataTable = await http.GetAsync("GetBarcodeManual", new
				{
					barcode = Function.ToString(txtBarCodeCheck.Text)
				});
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					grd_Check.DataSource = dataTable;
					grd_Check.RefreshDataSource();
				}
			}
			txtBarCodeCheck.Text = "";
			txtBarCodeCheck.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void clearToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			string name = tabControl.SelectedTabPage.Name;
			if (!(name == "tab_Manual"))
			{
				if (name == "tab_Check")
				{
					grd_Check.DataSource = null;
					grd_Check.RefreshDataSource();
					txtBarCodeCheck.Text = string.Empty;
					txtBarCodeCheck.Focus();
				}
			}
			else
			{
				grd_Manual.DataSource = null;
				grd_Manual.RefreshDataSource();
				txtBarCodeManual.Text = string.Empty;
				txtBarCodeManual.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			string name = tabControl.SelectedTabPage.Name;
			if (!(name == "tab_Manual"))
			{
				if (name == "tab_Check")
				{
					if (grd_Check.DataSourceTable != null && grd_Check.DataSourceTable.Rows.Count > 0)
					{
						gv_Check.DeleteRow(gv_Check.FocusedRowHandle);
						grd_Check.RefreshDataSource();
						grd_Check.DataSourceTable.AcceptChanges();
					}
					txtBarCodeCheck.Text = string.Empty;
					txtBarCodeCheck.Focus();
				}
			}
			else
			{
				if (grd_Manual.DataSourceTable != null && grd_Manual.DataSourceTable.Rows.Count > 0)
				{
					gv_Manual.DeleteRow(gv_Manual.FocusedRowHandle);
					grd_Manual.RefreshDataSource();
					grd_Manual.DataSourceTable.AcceptChanges();
				}
				txtBarCodeManual.Text = string.Empty;
				txtBarCodeManual.Focus();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_AutoReportComplete));
		this.tabControl = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Manual = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnReport = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtBarCodeManual = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.grd_Manual = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStripMenual = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Manual = new Apzon.Controls.Grid.GridViewEx();
		this.tab_Check = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.txtBarCodeCheck = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.grd_Check = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Check = new Apzon.Controls.Grid.GridViewEx();
		((System.ComponentModel.ISupportInitialize)this.tabControl).BeginInit();
		this.tabControl.SuspendLayout();
		this.tab_Manual.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtBarCodeManual.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Manual).BeginInit();
		this.contextMenuStripMenual.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Manual).BeginInit();
		this.tab_Check.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtBarCodeCheck.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Check).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Check).BeginInit();
		base.SuspendLayout();
		this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tabControl.IsSearchControl = false;
		this.tabControl.Location = new System.Drawing.Point(0, 0);
		this.tabControl.Name = "tabControl";
		this.tabControl.SelectedTabPage = this.tab_Manual;
		this.tabControl.Size = new System.Drawing.Size(584, 380);
		this.tabControl.TabIndex = 0;
		this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.tab_Manual, this.tab_Check });
		this.tabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tabControl_SelectedPageChanged);
		this.tab_Manual.Controls.Add(this.panelControl1);
		this.tab_Manual.EditControl = null;
		this.tab_Manual.EditValue = "Manual";
		this.tab_Manual.FieldName = null;
		this.tab_Manual.IsSearchControl = false;
		this.tab_Manual.Name = "tab_Manual";
		this.tab_Manual.ReadOnly = false;
		this.tab_Manual.SettingInVisible = false;
		this.tab_Manual.SettingReadonly = false;
		this.tab_Manual.Size = new System.Drawing.Size(582, 355);
		this.tab_Manual.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Manual.Text = "Manual";
		this.panelControl1.Controls.Add(this.btnCancel);
		this.panelControl1.Controls.Add(this.btnReport);
		this.panelControl1.Controls.Add(this.txtBarCodeManual);
		this.panelControl1.Controls.Add(this.labelLine1);
		this.panelControl1.Controls.Add(this.grd_Manual);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(582, 355);
		this.panelControl1.TabIndex = 0;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(85, 331);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(70, 20);
		this.btnCancel.TabIndex = 8;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btnReport.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnReport.EditControl = null;
		this.btnReport.EditValue = "Report";
		this.btnReport.FieldName = null;
		this.btnReport.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnReport.ImageOptions.Image");
		this.btnReport.IsSearchControl = false;
		this.btnReport.Location = new System.Drawing.Point(11, 331);
		this.btnReport.Name = "btnReport";
		this.btnReport.ReadOnly = false;
		this.btnReport.SettingInVisible = false;
		this.btnReport.SettingReadonly = false;
		this.btnReport.Size = new System.Drawing.Size(70, 20);
		this.btnReport.TabIndex = 7;
		this.btnReport.Text = "Report";
		this.btnReport.Click += new System.EventHandler(this.btn_Report_Click);
		this.txtBarCodeManual.EditControl = null;
		this.txtBarCodeManual.FieldName = null;
		this.txtBarCodeManual.IsSearchControl = false;
		this.txtBarCodeManual.Location = new System.Drawing.Point(78, 5);
		this.txtBarCodeManual.Name = "txtBarCodeManual";
		this.txtBarCodeManual.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txtBarCodeManual.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtBarCodeManual.SettingInVisible = false;
		this.txtBarCodeManual.SettingReadonly = false;
		this.txtBarCodeManual.Size = new System.Drawing.Size(220, 20);
		this.txtBarCodeManual.TabIndex = 1;
		this.txtBarCodeManual.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_BarCodeManual_KeyDown);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "BarCode";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(9, 8);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(70, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "BarCode";
		this.grd_Manual.AllowPasteExcel = true;
		this.grd_Manual.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Manual.ClipboardData = null;
		this.grd_Manual.ContextMenuStrip = this.contextMenuStripMenual;
		this.grd_Manual.Location = new System.Drawing.Point(0, 32);
		this.grd_Manual.MainView = this.gv_Manual;
		this.grd_Manual.Name = "grd_Manual";
		this.grd_Manual.Size = new System.Drawing.Size(582, 292);
		this.grd_Manual.TabIndex = 4;
		this.grd_Manual.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Manual });
		this.contextMenuStripMenual.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.clearToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStripMenual.Name = "contextMenuStripMenual";
		this.contextMenuStripMenual.Size = new System.Drawing.Size(134, 48);
		this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
		this.clearToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.clearToolStripMenuItem.Text = "Clear";
		this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gv_Manual.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Manual.ColumnKey = null;
		this.gv_Manual.DisableFormSetting = false;
		this.gv_Manual.DisplayException = false;
		this.gv_Manual.EditColumn = null;
		this.gv_Manual.FieldCurrency = null;
		this.gv_Manual.FieldRate = null;
		this.gv_Manual.FormId = "oReportCompleteByBarcode";
		this.gv_Manual.GridControl = this.grd_Manual;
		this.gv_Manual.IgnoreFilter = false;
		this.gv_Manual.IndexEditRow = -1;
		this.gv_Manual.IndicatorWidth = 40;
		this.gv_Manual.IsApzTable = false;
		this.gv_Manual.IsInsertNewRow = false;
		this.gv_Manual.IsLoadingData = false;
		this.gv_Manual.IsMappingContextAction = false;
		this.gv_Manual.ItemId = "OMANUAL";
		this.gv_Manual.LstColumnAllowBatch = null;
		this.gv_Manual.LstColumnNotAllowBatch = null;
		this.gv_Manual.LstColumnNotEditable = null;
		this.gv_Manual.LstColumNotSetting = null;
		this.gv_Manual.Name = "gv_Manual";
		this.gv_Manual.OptionsSelection.MultiSelect = true;
		this.gv_Manual.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Manual.OptionsView.ColumnAutoWidth = false;
		this.gv_Manual.OptionsView.ShowGroupPanel = false;
		this.gv_Manual.UseBatchMod = false;
		this.gv_Manual.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Manual_CustomDrawRowIndicator);
		this.tab_Check.Controls.Add(this.panelControl2);
		this.tab_Check.EditControl = null;
		this.tab_Check.EditValue = "Check";
		this.tab_Check.FieldName = null;
		this.tab_Check.IsSearchControl = false;
		this.tab_Check.Name = "tab_Check";
		this.tab_Check.ReadOnly = false;
		this.tab_Check.SettingInVisible = false;
		this.tab_Check.SettingReadonly = false;
		this.tab_Check.Size = new System.Drawing.Size(582, 355);
		this.tab_Check.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Check.Text = "Check";
		this.panelControl2.Controls.Add(this.txtBarCodeCheck);
		this.panelControl2.Controls.Add(this.labelLine2);
		this.panelControl2.Controls.Add(this.grd_Check);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl2.Location = new System.Drawing.Point(0, 0);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(582, 355);
		this.panelControl2.TabIndex = 0;
		this.txtBarCodeCheck.EditControl = null;
		this.txtBarCodeCheck.FieldName = null;
		this.txtBarCodeCheck.IsSearchControl = false;
		this.txtBarCodeCheck.Location = new System.Drawing.Point(77, 5);
		this.txtBarCodeCheck.Name = "txtBarCodeCheck";
		this.txtBarCodeCheck.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txtBarCodeCheck.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtBarCodeCheck.SettingInVisible = false;
		this.txtBarCodeCheck.SettingReadonly = false;
		this.txtBarCodeCheck.Size = new System.Drawing.Size(220, 20);
		this.txtBarCodeCheck.TabIndex = 0;
		this.txtBarCodeCheck.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_BarCodeCheck_KeyDown);
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "BarCode";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(8, 8);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(70, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 0;
		this.labelLine2.Utext = "BarCode";
		this.grd_Check.AllowPasteExcel = false;
		this.grd_Check.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Check.ClipboardData = null;
		this.grd_Check.ContextMenuStrip = this.contextMenuStripMenual;
		this.grd_Check.Location = new System.Drawing.Point(0, 31);
		this.grd_Check.MainView = this.gv_Check;
		this.grd_Check.Name = "grd_Check";
		this.grd_Check.Size = new System.Drawing.Size(582, 324);
		this.grd_Check.TabIndex = 2;
		this.grd_Check.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Check });
		this.gv_Check.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Check.ColumnKey = null;
		this.gv_Check.DisableFormSetting = false;
		this.gv_Check.DisplayException = false;
		this.gv_Check.EditColumn = null;
		this.gv_Check.FieldCurrency = null;
		this.gv_Check.FieldRate = null;
		this.gv_Check.FormId = "oReportCompleteByBarcode";
		this.gv_Check.GridControl = this.grd_Check;
		this.gv_Check.IgnoreFilter = false;
		this.gv_Check.IndexEditRow = -1;
		this.gv_Check.IndicatorWidth = 40;
		this.gv_Check.IsApzTable = false;
		this.gv_Check.IsInsertNewRow = false;
		this.gv_Check.IsLoadingData = false;
		this.gv_Check.IsMappingContextAction = false;
		this.gv_Check.ItemId = "OCHECK";
		this.gv_Check.LstColumnAllowBatch = null;
		this.gv_Check.LstColumnNotAllowBatch = null;
		this.gv_Check.LstColumnNotEditable = null;
		this.gv_Check.LstColumNotSetting = null;
		this.gv_Check.Name = "gv_Check";
		this.gv_Check.OptionsView.ColumnAutoWidth = false;
		this.gv_Check.OptionsView.ShowGroupPanel = false;
		this.gv_Check.UseBatchMod = false;
		this.gv_Check.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Check_CustomDrawRowIndicator);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(584, 380);
		base.Controls.Add(this.tabControl);
		base.FormId = "oReportCompleteByBarcode";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_AutoReportComplete.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AutoReportComplete";
		this.Text = "Report Complete by Barcode";
		((System.ComponentModel.ISupportInitialize)this.tabControl).EndInit();
		this.tabControl.ResumeLayout(false);
		this.tab_Manual.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtBarCodeManual.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Manual).EndInit();
		this.contextMenuStripMenual.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Manual).EndInit();
		this.tab_Check.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtBarCodeCheck.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Check).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Check).EndInit();
		base.ResumeLayout(false);
	}
}
