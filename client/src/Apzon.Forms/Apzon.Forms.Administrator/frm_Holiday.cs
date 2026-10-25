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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_Holiday : XtraFormEx
{
	private string _formController = "Holiday";

	private bool _isLoading;

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

	private GridColumnEx gc_doc_isCurYear;

	private GridColumnEx gc_doc_ignrWnd;

	private GridColumnEx gc_doc_WeekNoRule;

	private GridColumnEx gc_line_HldCode;

	private GridColumnEx gc_doc_StrDate;

	private GridColumnEx gc_doc_EndDate;

	private GridColumnEx gc_doc_Rmrks;

	private RepositoryItemLookUpEdit rep_doc_WndFrm;

	private RepositoryItemLookUpEdit rep_doc_WndTo;

	private RepositoryItemCheckEdit rep_doc_isCurYear;

	private RepositoryItemCheckEdit rep_doc_ignrWnd;

	private RepositoryItemLookUpEdit rep_doc_WeekNoRule;

	private ContextMenuStrip ctm_Doc;

	private ToolStripMenuItem addToolStripMenuItem;

	private ContextMenuStrip ctm_Line;

	private ToolStripMenuItem toolStripMenuItem1;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private ToolStripMenuItem removeToolStripMenuItem;

	public SimpleButtonEx btRefresh;

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

	public frm_Holiday()
	{
		InitializeComponent();
	}

	private async void frm_Holiday_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			SettingLanguage();
			CreateLkUpEditDatasource();
			await RefreshData();
			await SearchLine();
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
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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
				if (grd_Doc.DataSource is List<HolidayImpl> { Count: >0 } list)
				{
					string hldCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("HldCode"));
					if (string.IsNullOrWhiteSpace(hldCode))
					{
						WinCommons.Error(Enums.MessageCode.HolidayCodeCannotbeEmpty, "");
					}
					else
					{
						HolidayImpl holidayImpl = list.FirstOrDefault((HolidayImpl t) => t.HldCode == hldCode);
						if (holidayImpl != null)
						{
							if (grd_Line.DataSource is List<Holiday_LineImpl> { Count: >0 } list2)
							{
								for (int num = 0; num < list2.Count; num++)
								{
									if (Function.ParseDateTimes(list2[num].StrDate).CompareTo(Function.ParseDateTimes(list2[num].EndDate)) > 0)
									{
										WinCommons.Error(Enums.MessageCode.EndDateGreaterStartDate, "");
										btn_Ok.Enabled = true;
										return;
									}
									list2[num].HldCode = holidayImpl.HldCode;
								}
								holidayImpl.Lines = list2;
								string text = await SaveData(holidayImpl);
								if (string.IsNullOrWhiteSpace(text))
								{
									WinCommons.Success(Enums.MessageCode.OperationCompletedSuccess, "");
									await RefreshData();
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

	private async void btRefresh_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			FormMode = Enums.FormViewModeType.View;
			await RefreshData();
			await SearchLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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
			try
			{
				List<Holiday_LineImpl> list = grd_Line.DataSource as List<Holiday_LineImpl>;
				gv_Line.DeleteSelectedRows();
				if (list != null)
				{
					grd_Line.DataSource = list;
					grd_Line.RefreshDataSource();
				}
			}
			catch (Exception)
			{
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
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
				using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, _formController);
				string text = await hp.GetAsync("DeleteDoc", new
				{
					hldCode = value
				});
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

	private void gv_Doc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Line_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async Task<string> SaveData(HolidayImpl doc)
	{
		string result = string.Empty;
		try
		{
			using HttpClientHelper<Tuple<DataTable, DataTable>> hp = new HttpClientHelper<Tuple<DataTable, DataTable>>(Configuration.ApiServer, _formController);
			result = await hp.PostParamsAsJsonAsync("SaveDoc", doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private async Task RefreshData()
	{
		gv_Doc.Columns["HldCode"].OptionsColumn.AllowEdit = false;
		gv_Doc.Columns["HldCode"].OptionsColumn.ReadOnly = true;
		_isLoading = true;
		try
		{
			using HttpClientHelper<List<HolidayImpl>> hp = new HttpClientHelper<List<HolidayImpl>>(Configuration.ApiServer, _formController);
			List<HolidayImpl> dataSource = await hp.GetAsync("Search");
			grd_Doc.DataSource = dataSource;
			grd_Doc.RefreshDataSource();
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
		if (_isLoading)
		{
			return;
		}
		btn_Ok.Enabled = false;
		try
		{
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("HldCode"));
			using HttpClientHelper<List<Holiday_LineImpl>> hp = new HttpClientHelper<List<Holiday_LineImpl>>(Configuration.ApiServer, _formController);
			List<Holiday_LineImpl> dataSource = await hp.GetAsync("SearchLine", new
			{
				hldCode = text
			});
			grd_Line.DataSource = dataSource;
			grd_Line.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btn_Ok.Enabled = true;
	}

	private void SettingLanguage()
	{
		try
		{
			btn_Cancel.Text = Resources.ButtonCancel;
			btn_Ok.Text = Resources.ButtonOK;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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
			DataTable dataTable2 = dataTable.Clone();
			DataRow dataRow8 = dataTable2.NewRow();
			dataRow8[0] = "J";
			dataRow8[1] = "First week starts on January 1";
			dataTable2.Rows.Add(dataRow8);
			DataRow dataRow9 = dataTable2.NewRow();
			dataRow9[0] = "D";
			dataRow9[1] = "First week starts in first 4-day week";
			dataTable2.Rows.Add(dataRow9);
			DataRow dataRow10 = dataTable2.NewRow();
			dataRow10[0] = "F";
			dataRow10[1] = "First week starts in first full week";
			dataTable2.Rows.Add(dataRow10);
			dataTable2.AcceptChanges();
			rep_doc_WeekNoRule.DataSource = dataTable2;
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
			List<HolidayImpl> list = new List<HolidayImpl>();
			HolidayImpl holidayImpl = new HolidayImpl();
			holidayImpl.HldCode = "";
			holidayImpl.WndFrm = "1";
			holidayImpl.WndTo = "1";
			holidayImpl.isCurYear = "Y";
			holidayImpl.ignrWnd = "Y";
			holidayImpl.WeekNoRule = "J";
			list.Add(holidayImpl);
			grd_Doc.DataSource = list;
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
			List<Holiday_LineImpl> list = new List<Holiday_LineImpl>();
			Holiday_LineImpl holiday_LineImpl = new Holiday_LineImpl();
			if (grd_Line.DataSource != null && !newDoc)
			{
				try
				{
					list = grd_Line.DataSource as List<Holiday_LineImpl>;
				}
				catch (Exception)
				{
				}
			}
			holiday_LineImpl.HldCode = "";
			holiday_LineImpl.StrDate = DateTime.Today;
			holiday_LineImpl.EndDate = DateTime.Today;
			holiday_LineImpl.Rmrks = "";
			list.Add(holiday_LineImpl);
			grd_Line.DataSource = list;
			grd_Line.RefreshDataSource();
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
		_isLoading = false;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_Holiday));
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gc_doc_HldCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_doc_WndFrm = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_doc_WndFrm = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_doc_WndTo = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_doc_WndTo = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_doc_isCurYear = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_doc_isCurYear = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gc_doc_ignrWnd = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_doc_ignrWnd = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gc_doc_WeekNoRule = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_doc_WeekNoRule = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Line = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.gc_line_HldCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_doc_StrDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_doc_EndDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_doc_Rmrks = new Apzon.Controls.Grid.GridColumnEx();
		this.btRefresh = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctm_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_doc_WndFrm).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_doc_WndTo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_doc_isCurYear).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_doc_ignrWnd).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_doc_WeekNoRule).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		this.ctm_Line.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		base.SuspendLayout();
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(183, 504);
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
		this.splitContainer1.Size = new System.Drawing.Size(670, 493);
		this.splitContainer1.SplitterDistance = 262;
		this.splitContainer1.TabIndex = 18;
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctm_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rep_doc_WndFrm, this.rep_doc_WndTo, this.rep_doc_WeekNoRule, this.rep_doc_ignrWnd, this.rep_doc_isCurYear });
		this.grd_Doc.Size = new System.Drawing.Size(670, 262);
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
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gc_doc_HldCode, this.gc_doc_WndFrm, this.gc_doc_WndTo, this.gc_doc_isCurYear, this.gc_doc_ignrWnd, this.gc_doc_WeekNoRule });
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
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowDetailButtons = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Doc_CustomDrawRowIndicator);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gc_doc_HldCode.Caption = "Holidays Name";
		this.gc_doc_HldCode.CurrencyDateField = null;
		this.gc_doc_HldCode.CurrencyField = null;
		this.gc_doc_HldCode.CurrencyFixed = null;
		this.gc_doc_HldCode.CurrencyFixedRate = 0.0;
		this.gc_doc_HldCode.CurrencyRateField = null;
		this.gc_doc_HldCode.FieldName = "HldCode";
		this.gc_doc_HldCode.Name = "gc_doc_HldCode";
		this.gc_doc_HldCode.OptionsColumn.AllowEdit = false;
		this.gc_doc_HldCode.OptionsColumn.ReadOnly = true;
		this.gc_doc_HldCode.PrimaryKey = false;
		this.gc_doc_HldCode.UnitField = null;
		this.gc_doc_HldCode.Visible = true;
		this.gc_doc_HldCode.VisibleIndex = 0;
		this.gc_doc_HldCode.Width = 158;
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
		this.gc_doc_WndTo.Width = 92;
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
		this.gc_doc_isCurYear.Caption = "Valid for One Year Only";
		this.gc_doc_isCurYear.ColumnEdit = this.rep_doc_isCurYear;
		this.gc_doc_isCurYear.CurrencyDateField = null;
		this.gc_doc_isCurYear.CurrencyField = null;
		this.gc_doc_isCurYear.CurrencyFixed = null;
		this.gc_doc_isCurYear.CurrencyFixedRate = 0.0;
		this.gc_doc_isCurYear.CurrencyRateField = null;
		this.gc_doc_isCurYear.FieldName = "isCurYear";
		this.gc_doc_isCurYear.Name = "gc_doc_isCurYear";
		this.gc_doc_isCurYear.PrimaryKey = false;
		this.gc_doc_isCurYear.UnitField = null;
		this.gc_doc_isCurYear.Visible = true;
		this.gc_doc_isCurYear.VisibleIndex = 3;
		this.rep_doc_isCurYear.AutoHeight = false;
		this.rep_doc_isCurYear.Name = "rep_doc_isCurYear";
		this.rep_doc_isCurYear.ValueChecked = "Y";
		this.rep_doc_isCurYear.ValueUnchecked = "N";
		this.gc_doc_ignrWnd.Caption = "Set Weekends as Work Days";
		this.gc_doc_ignrWnd.ColumnEdit = this.rep_doc_ignrWnd;
		this.gc_doc_ignrWnd.CurrencyDateField = null;
		this.gc_doc_ignrWnd.CurrencyField = null;
		this.gc_doc_ignrWnd.CurrencyFixed = null;
		this.gc_doc_ignrWnd.CurrencyFixedRate = 0.0;
		this.gc_doc_ignrWnd.CurrencyRateField = null;
		this.gc_doc_ignrWnd.FieldName = "ignrWnd";
		this.gc_doc_ignrWnd.Name = "gc_doc_ignrWnd";
		this.gc_doc_ignrWnd.PrimaryKey = false;
		this.gc_doc_ignrWnd.UnitField = null;
		this.gc_doc_ignrWnd.Visible = true;
		this.gc_doc_ignrWnd.VisibleIndex = 4;
		this.gc_doc_ignrWnd.Width = 100;
		this.rep_doc_ignrWnd.AutoHeight = false;
		this.rep_doc_ignrWnd.Name = "rep_doc_ignrWnd";
		this.rep_doc_ignrWnd.ValueChecked = "Y";
		this.rep_doc_ignrWnd.ValueUnchecked = "N";
		this.gc_doc_WeekNoRule.Caption = "Rule to Calculate Week Number";
		this.gc_doc_WeekNoRule.ColumnEdit = this.rep_doc_WeekNoRule;
		this.gc_doc_WeekNoRule.CurrencyDateField = null;
		this.gc_doc_WeekNoRule.CurrencyField = null;
		this.gc_doc_WeekNoRule.CurrencyFixed = null;
		this.gc_doc_WeekNoRule.CurrencyFixedRate = 0.0;
		this.gc_doc_WeekNoRule.CurrencyRateField = null;
		this.gc_doc_WeekNoRule.FieldName = "WeekNoRule";
		this.gc_doc_WeekNoRule.Name = "gc_doc_WeekNoRule";
		this.gc_doc_WeekNoRule.PrimaryKey = false;
		this.gc_doc_WeekNoRule.UnitField = null;
		this.gc_doc_WeekNoRule.Visible = true;
		this.gc_doc_WeekNoRule.VisibleIndex = 5;
		this.gc_doc_WeekNoRule.Width = 165;
		this.rep_doc_WeekNoRule.AutoHeight = false;
		this.rep_doc_WeekNoRule.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_doc_WeekNoRule.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_doc_WeekNoRule.DisplayMember = "Name";
		this.rep_doc_WeekNoRule.Name = "rep_doc_WeekNoRule";
		this.rep_doc_WeekNoRule.NullText = "";
		this.rep_doc_WeekNoRule.ValueMember = "Code";
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.ContextMenuStrip = this.ctm_Line;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.Size = new System.Drawing.Size(670, 227);
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
		this.gv_Line.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gc_line_HldCode, this.gc_doc_StrDate, this.gc_doc_EndDate, this.gc_doc_Rmrks });
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = null;
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 35;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = null;
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsView.ColumnAutoWidth = false;
		this.gv_Line.OptionsView.ShowDetailButtons = false;
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = false;
		this.gv_Line.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Line_CustomDrawRowIndicator);
		this.gv_Line.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Line_CellValueChanged);
		this.gc_line_HldCode.Caption = "Holiday Code";
		this.gc_line_HldCode.CurrencyDateField = null;
		this.gc_line_HldCode.CurrencyField = null;
		this.gc_line_HldCode.CurrencyFixed = null;
		this.gc_line_HldCode.CurrencyFixedRate = 0.0;
		this.gc_line_HldCode.CurrencyRateField = null;
		this.gc_line_HldCode.FieldName = "HldCode";
		this.gc_line_HldCode.Name = "gc_line_HldCode";
		this.gc_line_HldCode.PrimaryKey = true;
		this.gc_line_HldCode.UnitField = null;
		this.gc_line_HldCode.Width = 168;
		this.gc_doc_StrDate.Caption = "Start Date";
		this.gc_doc_StrDate.CurrencyDateField = null;
		this.gc_doc_StrDate.CurrencyField = null;
		this.gc_doc_StrDate.CurrencyFixed = null;
		this.gc_doc_StrDate.CurrencyFixedRate = 0.0;
		this.gc_doc_StrDate.CurrencyRateField = null;
		this.gc_doc_StrDate.FieldName = "StrDate";
		this.gc_doc_StrDate.Name = "gc_doc_StrDate";
		this.gc_doc_StrDate.PrimaryKey = false;
		this.gc_doc_StrDate.UnitField = null;
		this.gc_doc_StrDate.Visible = true;
		this.gc_doc_StrDate.VisibleIndex = 0;
		this.gc_doc_StrDate.Width = 91;
		this.gc_doc_EndDate.Caption = "End Date";
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
		this.gc_doc_Rmrks.Caption = "Remarks";
		this.gc_doc_Rmrks.CurrencyDateField = null;
		this.gc_doc_Rmrks.CurrencyField = null;
		this.gc_doc_Rmrks.CurrencyFixed = null;
		this.gc_doc_Rmrks.CurrencyFixedRate = 0.0;
		this.gc_doc_Rmrks.CurrencyRateField = null;
		this.gc_doc_Rmrks.FieldName = "Rmrks";
		this.gc_doc_Rmrks.Name = "gc_doc_Rmrks";
		this.gc_doc_Rmrks.PrimaryKey = false;
		this.gc_doc_Rmrks.UnitField = null;
		this.gc_doc_Rmrks.Visible = true;
		this.gc_doc_Rmrks.VisibleIndex = 2;
		this.gc_doc_Rmrks.Width = 317;
		this.btRefresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btRefresh.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.btRefresh.Appearance.Options.UseBackColor = true;
		this.btRefresh.EditControl = null;
		this.btRefresh.EditValue = "Refresh";
		this.btRefresh.FieldName = null;
		this.btRefresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btRefresh.ImageOptions.Image");
		this.btRefresh.IsSearchControl = false;
		this.btRefresh.Location = new System.Drawing.Point(81, 504);
		this.btRefresh.Name = "btRefresh";
		this.btRefresh.ReadOnly = false;
		this.btRefresh.SettingInVisible = false;
		this.btRefresh.SettingReadonly = false;
		this.btRefresh.Size = new System.Drawing.Size(99, 20);
		this.btRefresh.TabIndex = 44;
		this.btRefresh.Text = "Refresh";
		this.btRefresh.Click += new System.EventHandler(this.btRefresh_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(675, 529);
		base.Controls.Add(this.btRefresh);
		base.Controls.Add(this.splitContainer1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Holiday";
		this.Text = "Holiday Setup";
		base.Load += new System.EventHandler(this.frm_Holiday_Load);
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
		this.splitContainer1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctm_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_doc_WndFrm).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_doc_WndTo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_doc_isCurYear).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_doc_ignrWnd).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_doc_WeekNoRule).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		this.ctm_Line.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		base.ResumeLayout(false);
	}
}
