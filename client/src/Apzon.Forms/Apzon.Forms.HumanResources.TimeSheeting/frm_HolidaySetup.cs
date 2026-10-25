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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_HolidaySetup : XtraFormEx
{
	public readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	private RepositoryItemDateEditEx _repNoYear;

	private IContainer components;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Ok;

	private SplitContainer splitContainer1;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridControlEx grd_Line;

	private GridViewEx gv_Line;

	private GridColumnEx gc_doc_HldCode;

	private GridColumnEx gc_doc_WndFrm;

	private GridColumnEx gc_doc_WndTo;

	private GridColumnEx gc_line_HldCode;

	private GridColumnEx gc_doc_StrDate;

	private GridColumnEx gc_doc_EndDate;

	private GridColumnEx gc_doc_Rmrks;

	private RepositoryItemLookUpEdit rep_doc_WndFrm;

	private RepositoryItemLookUpEdit rep_doc_WndTo;

	private ContextMenuStrip ctm_Doc;

	private ToolStripMenuItem addToolStripMenuItem;

	private ContextMenuStrip ctm_Line;

	private ToolStripMenuItem toolStripMenuItem1;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private ToolStripMenuItem removeToolStripMenuItem;

	private GridColumnEx gridColumnEx1;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private GridColumnEx gridColumnEx2;

	private RepositoryItemDateEditEx repositoryItemDateEditEx1;

	private RepositoryItemDateEditEx repositoryItemDateEdit1;

	public static string hldCode { get; set; }

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
		case Enums.FormViewModeType.Add:
			btn_Ok.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_Ok.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Ok.Text = Resources.ButtonOK;
			break;
		}
	}

	public async Task REFRESH()
	{
		AddNewDoc();
	}

	public async Task REFRESH_DATA()
	{
		await LoadData();
	}

	public frm_HolidaySetup()
	{
		InitializeComponent();
		FormId = "oHolidaySetup";
		gv_Doc.FormId = FormId;
		gv_Doc.ItemId = "APZ_OHLD";
		gv_Line.FormId = FormId;
		gv_Line.ItemId = "APZ_HLD1";
		OnLoadAsync += frm_HolidaySetup_Load;
		gv_Line.CustomRowCellEdit += gv_Line_CustomRowCellEdit;
		gv_Doc.BeforeLeaveRow += gv_Doc_BeforeLeaveRow;
		gv_Doc.ShowingEditor += GvDocOnShowingEditor;
	}

	private void GvDocOnShowingEditor(object sender, CancelEventArgs e)
	{
		if (gv_Doc.FocusedColumn.FieldName == "HldCode" && FormMode != Enums.FormViewModeType.Add)
		{
			e.Cancel = true;
		}
	}

	private async Task frm_HolidaySetup_Load(object sender, EventArgs e)
	{
		try
		{
			CreateLkUpEditDatasource();
			await LoadData();
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

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		btn_Ok.Enabled = false;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				WinCommons._OpenProcessing();
				DataTable dataSourceTable = grd_Doc.DataSourceTable;
				if (dataSourceTable.IsNotNull())
				{
					string hldCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("HldCode"));
					if (string.IsNullOrWhiteSpace(hldCode))
					{
						WinCommons.Error(Enums.MessageCode.HolidayCannotBeEmpty, "");
					}
					else
					{
						DataRow dataRow = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["HldCode"]) == hldCode);
						if (dataRow != null)
						{
							DataTable dataSourceTable2 = grd_Line.DataSourceTable;
							if (dataSourceTable2.IsNotNull())
							{
								for (int num = 0; num < dataSourceTable2.Rows.Count; num++)
								{
									if (Function.ParseDateTimes(dataSourceTable2.Rows[num]["StrDate"]).CompareTo(Function.ParseDateTimes(dataSourceTable2.Rows[num]["EndDate"])) > 0)
									{
										WinCommons.Error(Enums.MessageCode.EndDateGreaterStartDate, "");
										btn_Ok.Enabled = true;
										return;
									}
									dataSourceTable2.Rows[num]["HldCode"] = dataRow["HldCode"];
								}
								DataTable dataTable = dataSourceTable.Clone();
								dataTable.ImportRow(dataRow);
								dataTable.AcceptChanges();
								DataSet dataSet = new DataSet();
								dataSet.Tables.Add(dataTable.Copy());
								dataSet.Tables.Add(dataSourceTable2.Copy());
								string text = await SaveData(dataSet);
								if (string.IsNullOrWhiteSpace(text))
								{
									WinCommons.Success(Enums.MessageCode.OperationCompletedSuccess, "");
									await LoadData();
									FormMode = Enums.FormViewModeType.View;
								}
								else
								{
									WinCommons.Error(Enums.MessageCode.None, text);
								}
							}
							else
							{
								WinCommons.Error(Enums.MessageCode.DocumentLineIsEmpty, "");
							}
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.DocumentIsEmpty, "");
						}
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.DocumentIsEmpty, "");
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btn_Ok.Enabled = true;
	}

	private void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewDoc();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void toolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Line.DataSourceTable;
			gv_Line.DeleteSelectedRows();
			if (dataSourceTable != null)
			{
				grd_Line.DataSource = dataSourceTable;
				grd_Line.RefreshDataSource();
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		btn_Ok.Enabled = false;
		try
		{
			int rHdnl = gv_Doc.FocusedRowHandle;
			string value = Function.ToString(gv_Doc.GetRowCellValue(rHdnl, "HldCode"));
			if (!string.IsNullOrEmpty(value))
			{
				string text = await _httpClient.HolidaySetupClient.Remove(value);
				if (string.IsNullOrWhiteSpace(text))
				{
					WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
					gv_Doc.DeleteRow(rHdnl);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, Resources.Msg_DeleteError + ": " + text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btn_Ok.Enabled = true;
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isLoading && FormMode != Enums.FormViewModeType.Add && FormMode == Enums.FormViewModeType.View)
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

	private void gv_Line_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		int dataRowCount = gv_Line.DataRowCount;
		try
		{
			if (!_isLoading)
			{
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
				else if (dataRowCount > 0 && !string.IsNullOrWhiteSpace(Function.ToString(gv_Line.GetRowCellValue(dataRowCount - 1, "Rmrks"))))
				{
					AddNewLine();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			await SearchLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Line_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "StrDate" || e.Column.FieldName == "EndDate")
			{
				string text = Function.ToString(gv_Line.GetRowCellValue(e.RowHandle, "isCurYear"));
				e.RepositoryItem = ((text == "N") ? _repNoYear : repositoryItemDateEditEx1);
			}
		}
		catch (Exception ex)
		{
			Logging.Write(ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (_isLoading || (FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update))
			{
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				e.Allow = false;
				return;
			}
			if (grd_Doc.DataSource is DataTable dataTable)
			{
				dataTable.RejectChanges();
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> SaveData(DataSet ds)
	{
		string result = string.Empty;
		try
		{
			WinCommons._OpenProcessing();
			result = await _httpClient.HolidaySetupClient.Save(ds);
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private async Task LoadData()
	{
		_isLoading = true;
		try
		{
			DataTable dataSource = await _httpClient.HolidaySetupClient.GetAll();
			grd_Doc.DataSource = dataSource;
			grd_Doc.RefreshDataSource();
			grd_Doc.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoading = false;
	}

	private async Task SearchLine()
	{
		btn_Ok.Enabled = false;
		try
		{
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("HldCode"));
			DataTable dataSource = await _httpClient.HolidaySetupClient.GetLine(text);
			grd_Line.DataSource = dataSource;
			grd_Line.RefreshDataSource();
			AddNewLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btn_Ok.Enabled = true;
	}

	private void CreateLkUpEditDatasource()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow[0] = "1";
			dataRow[1] = "Sunday";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2[0] = "2";
			dataRow2[1] = "Monday";
			dataTable.Rows.Add(dataRow2);
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3[0] = "3";
			dataRow3[1] = "Tuesday";
			dataTable.Rows.Add(dataRow3);
			DataRow dataRow4 = dataTable.NewRow();
			dataRow4[0] = "4";
			dataRow4[1] = "Wednesday";
			dataTable.Rows.Add(dataRow4);
			DataRow dataRow5 = dataTable.NewRow();
			dataRow5[0] = "5";
			dataRow5[1] = "Thursday";
			dataTable.Rows.Add(dataRow5);
			DataRow dataRow6 = dataTable.NewRow();
			dataRow6[0] = "6";
			dataRow6[1] = "Friday";
			dataTable.Rows.Add(dataRow6);
			DataRow dataRow7 = dataTable.NewRow();
			dataRow7[0] = "7";
			dataRow7[1] = "Saturday";
			dataTable.Rows.Add(dataRow7);
			dataTable.AcceptChanges();
			rep_doc_WndFrm.DataSource = dataTable;
			DataTable dataSource = dataTable.Copy();
			rep_doc_WndTo.DataSource = dataSource;
			_repNoYear = new RepositoryItemDateEditEx();
			string formatString = _repNoYear.DisplayFormat.FormatString;
			char? c = Function.ParseChar(GlobalData.DateSeparatorFormat);
			if (!c.HasValue)
			{
				return;
			}
			int num = -1;
			string[] array = formatString.Split(c.Value);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].ToLower().Contains("y"))
				{
					num = i;
					break;
				}
			}
			List<string> list = array.ToList();
			if (num != -1)
			{
				list.RemoveAt(num);
			}
			string formatString2 = string.Join(c.Value.ToString(), list.ToArray());
			_repNoYear.DisplayFormat.FormatString = formatString2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewDoc()
	{
		_isLoading = true;
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			btn_Ok.Enabled = true;
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OHLD");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["HldCode"] = "";
			dataRow["WndFrm"] = "1";
			dataRow["WndTo"] = "1";
			tableStructure.Rows.Add(dataRow);
			grd_Doc.DataSource = tableStructure;
			grd_Doc.RefreshDataSource();
			gv_Doc.Columns["HldCode"].OptionsColumn.AllowEdit = true;
			gv_Doc.Columns["HldCode"].OptionsColumn.ReadOnly = false;
			AddNewLine(newDoc: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoading = false;
	}

	private void AddNewLine(bool newDoc = false)
	{
		_isLoading = true;
		try
		{
			DataTable dataTable = WinFuntions.GetTableStructure("APZ_HLD1");
			if ((grd_Line.DataSource as DataTable).IsNotNull() && !newDoc)
			{
				try
				{
					dataTable = grd_Line.DataSourceTable;
				}
				catch (Exception)
				{
					throw;
				}
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["isCurYear"] = "Y";
			dataRow["HldCode"] = "";
			dataRow["Rmrks"] = "";
			dataTable.Rows.Add(dataRow);
			grd_Line.DataSource = dataTable;
			grd_Line.RefreshDataSource();
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
		_isLoading = false;
	}

	private void gv_Doc_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		if ((grd_Doc.DataSource as DataTable).IsNotNull())
		{
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("HldCode"));
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "HolidaySetup");
			DataTable dataSource = httpClientHelper.Get("GetLine", new
			{
				hldCode = text
			});
			grd_Line.DataSource = dataSource;
		}
	}

	private async void frm_HolidaySetup_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			DataTable dataSourceTable2 = grd_Line.DataSourceTable;
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataSourceTable);
			dataSet.Tables.Add(dataSourceTable2);
			await SaveData(dataSet);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_HolidaySetup));
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gc_doc_HldCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_doc_WndFrm = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_doc_WndFrm = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_doc_WndTo = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_doc_WndTo = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Line = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.gc_line_HldCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_doc_StrDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit1 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gc_doc_EndDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gc_doc_Rmrks = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctm_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_doc_WndFrm).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_doc_WndTo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		this.ctm_Line.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		base.SuspendLayout();
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(84, 504);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 17;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(3, 504);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 20);
		this.btn_Ok.TabIndex = 16;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.splitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainer1.Location = new System.Drawing.Point(3, 5);
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this.splitContainer1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainer1.Panel2.Controls.Add(this.grd_Line);
		this.splitContainer1.Size = new System.Drawing.Size(670, 489);
		this.splitContainer1.SplitterDistance = 223;
		this.splitContainer1.TabIndex = 18;
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctm_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_doc_WndFrm, this.rep_doc_WndTo });
		this.grd_Doc.Size = new System.Drawing.Size(670, 223);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctm_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addToolStripMenuItem, this.removeToolStripMenuItem });
		this.ctm_Doc.Name = "ctm_Doc";
		this.ctm_Doc.Size = new System.Drawing.Size(118, 48);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gc_doc_HldCode, this.gridColumnEx2, this.gc_doc_WndFrm, this.gc_doc_WndTo });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oHolidaySetup";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_OHLD";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowDetailButtons = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Doc_FocusedRowChanged);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gc_doc_HldCode.Caption = "Holidays Code";
		this.gc_doc_HldCode.CurrencyDateField = null;
		this.gc_doc_HldCode.CurrencyField = null;
		this.gc_doc_HldCode.CurrencyFixed = null;
		this.gc_doc_HldCode.CurrencyFixedRate = 0.0;
		this.gc_doc_HldCode.CurrencyRateField = null;
		this.gc_doc_HldCode.FieldName = "HldCode";
		this.gc_doc_HldCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_doc_HldCode.Name = "gc_doc_HldCode";
		this.gc_doc_HldCode.OptionsColumn.AllowEdit = false;
		this.gc_doc_HldCode.OptionsColumn.ReadOnly = true;
		this.gc_doc_HldCode.PrimaryKey = true;
		this.gc_doc_HldCode.UnitField = null;
		this.gc_doc_HldCode.Visible = true;
		this.gc_doc_HldCode.VisibleIndex = 0;
		this.gc_doc_HldCode.Width = 158;
		this.gridColumnEx2.Caption = "Name";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "HldName";
		this.gridColumnEx2.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 3;
		this.gc_doc_WndFrm.Caption = "Weekend From";
		this.gc_doc_WndFrm.ColumnEdit = this.rep_doc_WndFrm;
		this.gc_doc_WndFrm.CurrencyDateField = null;
		this.gc_doc_WndFrm.CurrencyField = null;
		this.gc_doc_WndFrm.CurrencyFixed = null;
		this.gc_doc_WndFrm.CurrencyFixedRate = 0.0;
		this.gc_doc_WndFrm.CurrencyRateField = null;
		this.gc_doc_WndFrm.FieldName = "WndFrm";
		this.gc_doc_WndFrm.Name = "gc_doc_WndFrm";
		this.gc_doc_WndFrm.PrimaryKey = false;
		this.gc_doc_WndFrm.UnitField = null;
		this.gc_doc_WndFrm.Visible = true;
		this.gc_doc_WndFrm.VisibleIndex = 1;
		this.gc_doc_WndFrm.Width = 97;
		this.rep_doc_WndFrm.AutoHeight = false;
		this.rep_doc_WndFrm.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_doc_WndFrm.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_doc_WndFrm.DisplayMember = "Name";
		this.rep_doc_WndFrm.Name = "rep_doc_WndFrm";
		this.rep_doc_WndFrm.NullText = "";
		this.rep_doc_WndFrm.ValueMember = "Code";
		this.gc_doc_WndTo.Caption = "To";
		this.gc_doc_WndTo.ColumnEdit = this.rep_doc_WndTo;
		this.gc_doc_WndTo.CurrencyDateField = null;
		this.gc_doc_WndTo.CurrencyField = null;
		this.gc_doc_WndTo.CurrencyFixed = null;
		this.gc_doc_WndTo.CurrencyFixedRate = 0.0;
		this.gc_doc_WndTo.CurrencyRateField = null;
		this.gc_doc_WndTo.FieldName = "WndTo";
		this.gc_doc_WndTo.Name = "gc_doc_WndTo";
		this.gc_doc_WndTo.PrimaryKey = false;
		this.gc_doc_WndTo.UnitField = null;
		this.gc_doc_WndTo.Visible = true;
		this.gc_doc_WndTo.VisibleIndex = 2;
		this.gc_doc_WndTo.Width = 142;
		this.rep_doc_WndTo.AutoHeight = false;
		this.rep_doc_WndTo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_doc_WndTo.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_doc_WndTo.DisplayMember = "Name";
		this.rep_doc_WndTo.Name = "rep_doc_WndTo";
		this.rep_doc_WndTo.NullText = "";
		this.rep_doc_WndTo.ValueMember = "Code";
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.ContextMenuStrip = this.ctm_Line;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemCheckEdit1, this.repositoryItemDateEdit1 });
		this.grd_Line.Size = new System.Drawing.Size(670, 262);
		this.grd_Line.TabIndex = 1;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.ctm_Line.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.toolStripMenuItem1, this.removeRowToolStripMenuItem });
		this.ctm_Line.Name = "ctm_Doc";
		this.ctm_Line.Size = new System.Drawing.Size(144, 48);
		this.toolStripMenuItem1.Name = "toolStripMenuItem1";
		this.toolStripMenuItem1.Size = new System.Drawing.Size(143, 22);
		this.toolStripMenuItem1.Text = "Add Row";
		this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gc_line_HldCode, this.gc_doc_StrDate, this.gc_doc_EndDate, this.gridColumnEx1, this.gc_doc_Rmrks });
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oHolidaySetup";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 35;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = "APZ_HLD1";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsView.ColumnAutoWidth = false;
		this.gv_Line.OptionsView.ShowDetailButtons = false;
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = false;
		this.gv_Line.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Line_CellValueChanged);
		this.gc_line_HldCode.Caption = "Holiday Code";
		this.gc_line_HldCode.CurrencyDateField = null;
		this.gc_line_HldCode.CurrencyField = null;
		this.gc_line_HldCode.CurrencyFixed = null;
		this.gc_line_HldCode.CurrencyFixedRate = 0.0;
		this.gc_line_HldCode.CurrencyRateField = null;
		this.gc_line_HldCode.FieldName = "HldCode";
		this.gc_line_HldCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_line_HldCode.Name = "gc_line_HldCode";
		this.gc_line_HldCode.PrimaryKey = true;
		this.gc_line_HldCode.UnitField = null;
		this.gc_line_HldCode.Width = 168;
		this.gc_doc_StrDate.Caption = "Start Date";
		this.gc_doc_StrDate.ColumnEdit = this.repositoryItemDateEdit1;
		this.gc_doc_StrDate.CurrencyDateField = null;
		this.gc_doc_StrDate.CurrencyField = null;
		this.gc_doc_StrDate.CurrencyFixed = null;
		this.gc_doc_StrDate.CurrencyFixedRate = 0.0;
		this.gc_doc_StrDate.CurrencyRateField = null;
		this.gc_doc_StrDate.FieldName = "StrDate";
		this.gc_doc_StrDate.Name = "gc_doc_StrDate";
		this.gc_doc_StrDate.PrimaryKey = true;
		this.gc_doc_StrDate.UnitField = null;
		this.gc_doc_StrDate.Visible = true;
		this.gc_doc_StrDate.VisibleIndex = 0;
		this.gc_doc_StrDate.Width = 91;
		this.repositoryItemDateEdit1.AutoHeight = false;
		this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.IsSearchControl = false;
		this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
		this.gc_doc_EndDate.Caption = "End Date";
		this.gc_doc_EndDate.ColumnEdit = this.repositoryItemDateEdit1;
		this.gc_doc_EndDate.CurrencyDateField = null;
		this.gc_doc_EndDate.CurrencyField = null;
		this.gc_doc_EndDate.CurrencyFixed = null;
		this.gc_doc_EndDate.CurrencyFixedRate = 0.0;
		this.gc_doc_EndDate.CurrencyRateField = null;
		this.gc_doc_EndDate.FieldName = "EndDate";
		this.gc_doc_EndDate.Name = "gc_doc_EndDate";
		this.gc_doc_EndDate.PrimaryKey = false;
		this.gc_doc_EndDate.UnitField = null;
		this.gc_doc_EndDate.Visible = true;
		this.gc_doc_EndDate.VisibleIndex = 1;
		this.gc_doc_EndDate.Width = 95;
		this.gridColumnEx1.Caption = "Valid for Only One Year";
		this.gridColumnEx1.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "isCurYear";
		this.gridColumnEx1.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 2;
		this.gridColumnEx1.Width = 126;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.gc_doc_Rmrks.Caption = "Remarks";
		this.gc_doc_Rmrks.CurrencyDateField = null;
		this.gc_doc_Rmrks.CurrencyField = null;
		this.gc_doc_Rmrks.CurrencyFixed = null;
		this.gc_doc_Rmrks.CurrencyFixedRate = 0.0;
		this.gc_doc_Rmrks.CurrencyRateField = null;
		this.gc_doc_Rmrks.FieldName = "Rmrks";
		this.gc_doc_Rmrks.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_doc_Rmrks.Name = "gc_doc_Rmrks";
		this.gc_doc_Rmrks.PrimaryKey = false;
		this.gc_doc_Rmrks.UnitField = null;
		this.gc_doc_Rmrks.Visible = true;
		this.gc_doc_Rmrks.VisibleIndex = 3;
		this.gc_doc_Rmrks.Width = 317;
		base.AcceptButton = this.btn_Ok;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(675, 529);
		base.Controls.Add(this.splitContainer1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.FormId = "oHolidaySetup";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_HolidaySetup";
		this.Text = "Holiday Setup";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_HolidaySetup_FormClosing);
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
		this.splitContainer1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctm_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_doc_WndFrm).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_doc_WndTo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		this.ctm_Line.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		base.ResumeLayout(false);
	}
}
