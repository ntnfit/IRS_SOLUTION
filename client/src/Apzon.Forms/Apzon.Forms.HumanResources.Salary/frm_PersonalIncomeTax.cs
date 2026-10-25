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
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.HumanResources.Salary;

public class frm_PersonalIncomeTax : XtraFormEx
{
	private const string Controller = "AllowanceType";

	private bool _preAdd;

	private bool _isLoading;

	private DataTable _dtMain;

	private DataTable _dtDetail;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx xtraTabPage_Detail;

	private GridControlEx grd_Detail;

	private GridViewEx gv_Detail;

	private GridColumnEx gridColumn1;

	private GridColumnEx col_MinAmt;

	private GridColumnEx col_MaxAmt;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridColumnEx col_TaxLevel;

	private GridColumnEx col_Ratio;

	private RepositoryItemCalcEdit ItemCalcEdit_Ratio;

	private ContextMenuStrip contextMenuStrip_PerInComeTax;

	private ToolStripMenuItem toolStripMenuItem_Remove;

	private ContextMenuStrip contextMenuStrip_Detail;

	private ToolStripMenuItem toolStripMenuItem_RemoveRow;

	private ToolStripMenuItem toolStripMenuItem_AddPer;

	private ToolStripMenuItem toolStripMenuItem_AddRow;

	private GridControlEx grd_PersonalIncomeTax;

	private GridViewEx gv_PersonalIncomeTax;

	private GridColumnEx detail_Code;

	private GridColumnEx col_Name;

	private GridColumnEx col_FromDate;

	private RepositoryItemDateEditEx rep_dateFrom;

	private GridColumnEx col_ToDate;

	private RepositoryItemDateEditEx rep_dateTo;

	private RepositoryItemCalcEdit rep_caComp;

	private RepositoryItemCalcEdit rep_caEmp;

	private GridColumnEx Apply;

	private RepositoryItemCheckEdit rep_checkApply;

	private GridColumnEx col_Remark;

	private SplitContainerControl splitContainerControl1;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				btnCancel.Enabled = true;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				btn_Save.Text = Resources.ButtonAdd;
				break;
			case Enums.FormViewModeType.Update:
				btn_Save.Text = Resources.ButtonUpdate;
				break;
			case Enums.FormViewModeType.Find:
				btn_Save.Text = Resources.ButtonFind;
				break;
			case Enums.FormViewModeType.View:
				btn_Save.Text = Resources.ButtonOK;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_PersonalIncomeTax()
	{
		InitializeComponent();
		OnLoadAsync += frm_PersonalIncomeTax_Load;
		FormId = "oPersonalIncomeTax";
		gv_PersonalIncomeTax.ItemId = "APZ_OPIT";
		gv_PersonalIncomeTax.FormId = FormId;
		gv_Detail.ItemId = "APZ_PIT1";
		gv_Detail.FormId = FormId;
		gv_PersonalIncomeTax.ChangeLogType = Enums.ChangeLog.clPersionalIncomeTax;
		gv_PersonalIncomeTax.IsApzTable = true;
	}

	public async Task RefreshPersonalIncomeTax()
	{
		try
		{
			DataTable dataTable = await GetPersonalIncomeTax();
			dataTable.SetDefaultAndMaxLen(gv_PersonalIncomeTax.ItemId);
			grd_PersonalIncomeTax.DataSource = dataTable;
			if (!dataTable.IsNotNull())
			{
				gv_PersonalIncomeTax_FocusedRowObjectChanged(null, null);
			}
			grd_PersonalIncomeTax.DataSourceTable.AcceptChanges();
			grd_PersonalIncomeTax.RefreshDataSource();
			AddNew_OPIT();
			AddNew_PIT1();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<DataTable> GetPersonalIncomeTax()
	{
		try
		{
			return await _httpClient.AllowanceType.GetPerIncomeTax();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public async Task MOVE_PREVIOUS()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Previous) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Next) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.First) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Last) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			grd_PersonalIncomeTax.DataSourceTable.Rows.Clear();
			grd_Detail.DataSourceTable.Rows.Clear();
			_dtMain = grd_PersonalIncomeTax.DataSourceTable.Clone();
			if (_dtMain == null && _dtMain.Columns.Count < 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			DataRow dataRow = _dtMain.NewRow();
			dataRow["Name"] = "";
			dataRow["Apply"] = "Y";
			dataRow["Remarks"] = "";
			_dtMain.Rows.Add(dataRow);
			_dtMain.SetDefaultAndMaxLen(gv_PersonalIncomeTax.ItemId);
			grd_PersonalIncomeTax.DataSource = _dtMain;
			_dtDetail = grd_Detail.DataSourceTable.Clone();
			if (_dtDetail == null && _dtDetail.Columns.Count < 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			DataRow row = _dtDetail.NewRow();
			_dtDetail.Rows.Add(row);
			_dtDetail.SetDefaultAndMaxLen(gv_Detail.ItemId);
			grd_Detail.DataSource = _dtDetail;
			grd_Detail.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		_ = 1;
		try
		{
			DataTable dataTable = await GetPersonalIncomeTax();
			if (dataTable.IsNotNull())
			{
				dataTable.SetDefaultAndMaxLen(gv_PersonalIncomeTax.ItemId);
				grd_PersonalIncomeTax.DataSource = dataTable;
				grd_PersonalIncomeTax.RefreshDataSource();
				grd_PersonalIncomeTax.DataSourceTable.AcceptChanges();
				int code = Function.ParseInt(gv_PersonalIncomeTax.GetFocusedRowCellValue("Code"));
				await GetDetails(code);
			}
			else
			{
				grd_PersonalIncomeTax.DataSource = dataTable;
				grd_PersonalIncomeTax.RefreshDataSource();
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void CloseForm()
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			Close();
		}
	}

	private async void RemoveDetail()
	{
		try
		{
			int focusedRowHandle = gv_Detail.FocusedRowHandle;
			_dtDetail.Rows.RemoveAt(focusedRowHandle);
			_dtDetail.SetDefaultAndMaxLen(gv_Detail.ItemId);
			grd_Detail.DataSource = _dtDetail;
			grd_Detail.RefreshDataSource();
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNew_OPIT()
	{
		try
		{
			gv_PersonalIncomeTax.BeginInsertRow();
			DataTable dataSourceTable = grd_PersonalIncomeTax.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow["Code"] = 0;
				dataRow["Name"] = "";
				dataRow["Apply"] = "Y";
				dataRow["Remarks"] = "";
				dataSourceTable.Rows.Add(dataRow);
				dataSourceTable.AcceptChanges();
				dataSourceTable.SetDefaultAndMaxLen(gv_PersonalIncomeTax.ItemId);
				grd_PersonalIncomeTax.DataSource = dataSourceTable;
				grd_PersonalIncomeTax.RefreshDataSource();
				grd_PersonalIncomeTax.DataSourceTable.AcceptChanges();
			}
			else
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OPIT");
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["Code"] = 0;
				dataRow2["Name"] = "";
				dataRow2["Apply"] = "Y";
				dataRow2["Remarks"] = "";
				tableStructure.Rows.Add(dataRow2);
				tableStructure.AcceptChanges();
				tableStructure.SetDefaultAndMaxLen(gv_PersonalIncomeTax.ItemId);
				grd_PersonalIncomeTax.DataSource = tableStructure;
				grd_PersonalIncomeTax.RefreshDataSource();
				grd_PersonalIncomeTax.DataSourceTable.AcceptChanges();
			}
			gv_PersonalIncomeTax.EndInsertRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gv_PersonalIncomeTax.EndInsertRow();
	}

	private void AddNew_PIT1()
	{
		try
		{
			gv_Detail.BeginInsertRow();
			DataTable dataSourceTable = grd_Detail.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow["Code"] = 0;
				dataRow["TaxLevel"] = 0;
				dataRow["MinAmt"] = 0.0;
				dataRow["MaxAmt"] = 0.0;
				dataRow["Ratio"] = 0.0;
				dataSourceTable.Rows.Add(dataRow);
				dataSourceTable.AcceptChanges();
				dataSourceTable.SetDefaultAndMaxLen(gv_Detail.ItemId);
				grd_Detail.DataSource = dataSourceTable;
				grd_Detail.RefreshDataSource();
				grd_Detail.DataSourceTable.AcceptChanges();
			}
			else
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_PIT1");
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["Code"] = 0;
				dataRow2["TaxLevel"] = 0;
				dataRow2["MinAmt"] = 0.0;
				dataRow2["MaxAmt"] = 0.0;
				dataRow2["Ratio"] = 0.0;
				tableStructure.Rows.Add(dataRow2);
				tableStructure.AcceptChanges();
				tableStructure.SetDefaultAndMaxLen(gv_Detail.ItemId);
				grd_Detail.DataSource = tableStructure;
				grd_Detail.RefreshDataSource();
				grd_Detail.DataSourceTable.AcceptChanges();
			}
			gv_Detail.EndInsertRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Detail.EndInsertRow();
	}

	public async Task<int> FindDataNextLast(Enums.NavigateType type)
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.AllowanceType.FindDataNextLast(Function.ParseInt(gv_PersonalIncomeTax.GetFocusedRowCellValue("Code")), (int)type);
			if (dataTable.IsNotNull())
			{
				dataTable.SetDefaultAndMaxLen(gv_PersonalIncomeTax.ItemId);
				grd_PersonalIncomeTax.DataSource = dataTable;
				int code = Function.ParseInt(dataTable.Rows[0]["Code"]);
				await GetDetails(code);
				FormMode = Enums.FormViewModeType.View;
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private async Task<int> PersonalIncomeTaxSave()
	{
		_ = 2;
		try
		{
			_dtMain = grd_PersonalIncomeTax.DataSourceTable.Clone();
			DataRow dataRow = _dtMain.NewRow();
			dataRow.ItemArray = gv_PersonalIncomeTax.GetFocusedDataRow().ItemArray;
			_dtMain.Rows.Add(dataRow);
			_dtMain.AcceptChanges();
			_dtDetail = grd_Detail.DataSourceTable;
			if (string.IsNullOrEmpty(Function.ToString(_dtMain.Rows[0]["FromDate"])))
			{
				WinCommons.Error(Enums.MessageCode.ToDateCanNotBeNull, "");
				return 1;
			}
			if (string.IsNullOrEmpty(Function.ToString(_dtMain.Rows[0]["Name"])))
			{
				WinCommons.Error(Enums.MessageCode.NameCannotBeEmpty, "");
				return 1;
			}
			if (_dtDetail.IsNotNull())
			{
				if (_dtDetail.Copy().AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["TaxLevel"]))) != null)
				{
					WinCommons.Error(Enums.MessageCode.TaxLevelCannotBeEmpty, "");
					return 1;
				}
				if ((from x in _dtDetail.Copy().AsEnumerable()
					group x by Function.ParseInt(x["TaxLevel"])).Any((IGrouping<int, DataRow> x) => x.Skip(1).Any()))
				{
					WinCommons.Error(Enums.MessageCode.TaxLevelCannotBeDuplicated, "");
					return 1;
				}
				if (_dtDetail.Copy().AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["MinAmt"]))) != null)
				{
					WinCommons.Error(Enums.MessageCode.MinAmountCannotBeEmpty, "");
					return 1;
				}
				if (_dtDetail.Copy().AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["MaxAmt"]))) != null)
				{
					WinCommons.Error(Enums.MessageCode.MaxAmountCannotBeEmpty, "");
					return 1;
				}
				if (_dtDetail.Copy().AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Ratio"]))) != null)
				{
					WinCommons.Error(Enums.MessageCode.RatioCannotBeEmpty, "");
					return 1;
				}
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DataSet dataSet = new DataSet();
				Function.ParseInt(gv_PersonalIncomeTax.GetFocusedRowCellValue("Code"));
				_dtMain.Rows[0]["UserSign2"] = GlobalData.UserID;
				_dtMain.Rows[0]["UpdateDate"] = DateTime.Today;
				_dtMain.TableName = "Main";
				_dtDetail.TableName = "Detail";
				dataSet.Tables.Add(_dtMain);
				dataSet.Tables.Add(_dtDetail.Copy());
				HttpResult httpResult = ((FormMode != Enums.FormViewModeType.Update) ? (await _httpClient.AllowanceType.SavePersonalIncomeTax(dataSet)) : (await _httpClient.AllowanceType.UpdatePersonalIncomeTax(dataSet)));
				HttpResult httpResult2 = httpResult;
				if (string.IsNullOrEmpty(httpResult2.Message))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					DataTable dataTable = await GetPerIncomeTax();
					dataTable.SetDefaultAndMaxLen(gv_PersonalIncomeTax.ItemId);
					grd_PersonalIncomeTax.DataSource = dataTable;
					FormMode = Enums.FormViewModeType.View;
					return 0;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult2.Message);
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
		return 1;
	}

	public async Task GetDetails(int code)
	{
		try
		{
			_isLoading = true;
			WinCommons._OpenProcessing();
			this.ResetData();
			DataTable dataTable = await _httpClient.AllowanceType.GetDetail(code);
			dataTable.SetDefaultAndMaxLen(gv_Detail.ItemId);
			grd_Detail.DataSource = dataTable;
			grd_Detail.RefreshDataSource();
			grd_Detail.DataSourceTable.AcceptChanges();
			WinCommons._CloseProcessing();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	public async Task<DataTable> GetPerIncomeTax()
	{
		try
		{
			return await _httpClient.AllowanceType.GetPerIncomeTax();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private void CheckRowNull(DataTable dt, string key)
	{
		try
		{
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				if (Function.ToString(dt.Rows[i][key]) == "")
				{
					dt.Rows.RemoveAt(i--);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_PersonalIncomeTax_Load(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = await GetPerIncomeTax();
			dataTable.SetDefaultAndMaxLen(gv_PersonalIncomeTax.ItemId);
			grd_PersonalIncomeTax.DataSource = dataTable;
			grd_PersonalIncomeTax.DataSourceTable.AcceptChanges();
			if (!dataTable.IsNotNull())
			{
				gv_PersonalIncomeTax_FocusedRowObjectChanged(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Update:
			case Enums.FormViewModeType.Add:
				await PersonalIncomeTaxSave();
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_PersonalIncomeTax_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				await GetDetails(Function.ParseInt(gv_PersonalIncomeTax.GetFocusedRowCellValue("Code")));
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void toolStripMenuItem_Remove_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (gv_PersonalIncomeTax.GetSelectedRows() == null || gv_PersonalIncomeTax.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				int[] selectedRows = gv_PersonalIncomeTax.GetSelectedRows();
				string text = "";
				int[] array = selectedRows;
				foreach (int rowHandle in array)
				{
					text = text + gv_PersonalIncomeTax.GetRowCellValue(rowHandle, "Code").ToString() + ",";
				}
				if (text.Length > 0)
				{
					text = text.Remove(text.Length - 1);
					string text2 = await _httpClient.AllowanceType.RemovePerIncomeTax(text);
					if (string.IsNullOrEmpty(text2))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						DataTable dataTable = await GetPerIncomeTax();
						dataTable.SetDefaultAndMaxLen(gv_PersonalIncomeTax.ItemId);
						grd_PersonalIncomeTax.DataSource = dataTable;
						grd_Detail.DataSource = null;
						gv_Detail.RefreshData();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text2);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void toolStripMenuItem_RemoveRow_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				RemoveDetail();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_PersonalIncomeTax_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_PersonalIncomeTax_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Detail_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Detail_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			string mainCurrency = GlobalData.MainCurrency;
			int num = -1;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(GlobalData.MainCurrency.ToUpper()));
				if (currencyCodesImpl != null)
				{
					num = currencyCodesImpl.Decimals;
				}
			}
			if (e.Column.FieldName.ToUpper().Equals("MINAMT"))
			{
				e.DisplayText = string.Format("{0:n" + ((num == -1) ? GlobalData.UnitPriceFormat.NumberDecimalDigits : num) + "} {1}", e.Value, mainCurrency.ToUpper());
			}
			if (e.Column.FieldName.ToUpper().Equals("MAXAMT"))
			{
				e.DisplayText = string.Format("{0:n" + ((num == -1) ? GlobalData.UnitPriceFormat.NumberDecimalDigits : num) + "} {1}", e.Value, mainCurrency.ToUpper());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Detail_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void toolStripMenuItem_AddPer_Click(object sender, EventArgs e)
	{
		try
		{
			REFRESH();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void toolStripMenuItem_AddRow_Click(object sender, EventArgs e)
	{
		try
		{
			_dtDetail = grd_Detail.DataSourceTable;
			if (_dtDetail == null && _dtDetail.Columns.Count < 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				return;
			}
			DataRow dataRow = _dtDetail.NewRow();
			dataRow["TaxLevel"] = _dtDetail.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["TaxLevel"])) + 1;
			_dtDetail.Rows.Add(dataRow);
			_dtDetail.SetDefaultAndMaxLen(gv_Detail.ItemId);
			grd_Detail.DataSource = _dtDetail;
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Detail_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			if (!(sender is GridView { FocusedColumn: var focusedColumn } gridView))
			{
				return;
			}
			if (focusedColumn.FieldName.ToUpper().Equals("MINAMT"))
			{
				int focusedRowHandle = gridView.FocusedRowHandle;
				List<string> currency = Function.GetCurrency(Function.ToString(e.Value));
				int lineNum = Function.ParseInt(gridView.GetRowCellValue(focusedRowHandle, "TaxLevel"));
				if (((DataTable)grd_Detail.DataSource).AsEnumerable().FirstOrDefault((DataRow s) => Function.ParseInt(s["TaxLevel"]) == lineNum) == null)
				{
					WinCommons.Error(Enums.MessageCode.MinAmountIsNull, "");
					return;
				}
				DataRow dataRow = ((DataTable)gridView.GridControl.DataSource).AsEnumerable().FirstOrDefault((DataRow s) => Function.ParseInt(s["TaxLevel"]) == lineNum);
				if (currency.Count > 0 && dataRow != null)
				{
					decimal num = Function.ParseDecimal(currency[0]);
					string alpha = Function.ToString(currency[1]);
					if (string.IsNullOrEmpty(alpha))
					{
						alpha = GlobalData.MainCurrency;
					}
					if (GlobalData.Currencies.Exists((Common.Table_Code t) => t.Code.ToUpper().Equals(alpha.ToUpper())))
					{
						GlobalData.MainCurrency = alpha;
						e.Value = num;
						gv_Detail.SetRowCellValue(focusedRowHandle, focusedColumn, num);
						e.Valid = true;
					}
					else
					{
						e.Valid = false;
					}
				}
			}
			if (!focusedColumn.FieldName.ToUpper().Equals("MAXAMT"))
			{
				return;
			}
			int focusedRowHandle2 = gridView.FocusedRowHandle;
			List<string> currency2 = Function.GetCurrency(Function.ToString(e.Value));
			int lineNum2 = Function.ParseInt(gridView.GetRowCellValue(focusedRowHandle2, "TaxLevel"));
			((DataTable)gridView.GridControl.DataSource).AsEnumerable().FirstOrDefault((DataRow s) => Function.ParseInt(s["TaxLevel"]) == lineNum2);
			if (currency2.Count > 0)
			{
				decimal num2 = Function.ParseDecimal(currency2[0]);
				string alpha2 = Function.ToString(currency2[1]);
				if (string.IsNullOrEmpty(alpha2))
				{
					alpha2 = GlobalData.MainCurrency;
				}
				if (GlobalData.Currencies.Exists((Common.Table_Code t) => t.Code.ToUpper().Equals(alpha2.ToUpper())))
				{
					GlobalData.MainCurrency = alpha2;
					e.Value = num2;
					gv_Detail.SetRowCellValue(focusedRowHandle2, focusedColumn, num2);
					e.Valid = true;
				}
				else
				{
					e.Valid = false;
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.InvalidCurrency, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private void frm_PersonalIncomeTax_FormClosing(object sender, FormClosingEventArgs e)
	{
		if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
		{
			e.Cancel = true;
		}
	}

	private void gv_PersonalIncomeTax_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (!_preAdd && !_isLoading && FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					e.Allow = false;
				}
				else
				{
					grd_PersonalIncomeTax.DataSourceTable.RejectChanges();
				}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.Salary.frm_PersonalIncomeTax));
		this.contextMenuStrip_PerInComeTax = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_AddPer = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage_Detail = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Detail = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Detail = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_AddRow = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_RemoveRow = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Detail = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.col_TaxLevel = new Apzon.Controls.Grid.GridColumnEx();
		this.col_MinAmt = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.col_MaxAmt = new Apzon.Controls.Grid.GridColumnEx();
		this.col_Ratio = new Apzon.Controls.Grid.GridColumnEx();
		this.ItemCalcEdit_Ratio = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_PersonalIncomeTax = new Apzon.Controls.Grid.GridControlEx();
		this.gv_PersonalIncomeTax = new Apzon.Controls.Grid.GridViewEx();
		this.detail_Code = new Apzon.Controls.Grid.GridColumnEx();
		this.col_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.col_FromDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_dateFrom = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.col_ToDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_dateTo = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.Apply = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_checkApply = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.col_Remark = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_caComp = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.rep_caEmp = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.contextMenuStrip_PerInComeTax.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage_Detail.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Detail).BeginInit();
		this.contextMenuStrip_Detail.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Detail).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ItemCalcEdit_Ratio).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_PersonalIncomeTax).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_PersonalIncomeTax).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateFrom).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateFrom.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateTo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateTo.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_checkApply).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_caComp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_caEmp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		base.SuspendLayout();
		this.contextMenuStrip_PerInComeTax.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.toolStripMenuItem_AddPer, this.toolStripMenuItem_Remove });
		this.contextMenuStrip_PerInComeTax.Name = "contextMenuStrip_PerInComeTax";
		this.contextMenuStrip_PerInComeTax.Size = new System.Drawing.Size(118, 48);
		this.toolStripMenuItem_AddPer.Name = "toolStripMenuItem_AddPer";
		this.toolStripMenuItem_AddPer.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_AddPer.Text = "Add";
		this.toolStripMenuItem_AddPer.Click += new System.EventHandler(this.toolStripMenuItem_AddPer_Click);
		this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
		this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Remove.Text = "Remove";
		this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage_Detail;
		this.xtraTabControl1.Size = new System.Drawing.Size(704, 221);
		this.xtraTabControl1.TabIndex = 82;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtraTabPage_Detail });
		this.xtraTabPage_Detail.Controls.Add(this.grd_Detail);
		this.xtraTabPage_Detail.EditControl = null;
		this.xtraTabPage_Detail.EditValue = "Detail";
		this.xtraTabPage_Detail.FieldName = "xtraTabPage_Detail";
		this.xtraTabPage_Detail.IsSearchControl = false;
		this.xtraTabPage_Detail.Name = "xtraTabPage_Detail";
		this.xtraTabPage_Detail.ReadOnly = false;
		this.xtraTabPage_Detail.SettingInVisible = false;
		this.xtraTabPage_Detail.SettingReadonly = false;
		this.xtraTabPage_Detail.Size = new System.Drawing.Size(702, 196);
		this.xtraTabPage_Detail.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage_Detail.Text = "Detail";
		this.grd_Detail.AllowPasteExcel = true;
		this.grd_Detail.ClipboardData = null;
		this.grd_Detail.ContextMenuStrip = this.contextMenuStrip_Detail;
		this.grd_Detail.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Detail.Location = new System.Drawing.Point(0, 0);
		this.grd_Detail.MainView = this.gv_Detail;
		this.grd_Detail.Name = "grd_Detail";
		this.grd_Detail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.ItemCalcEdit_Ratio, this.repositoryItemCalcEdit1 });
		this.grd_Detail.Size = new System.Drawing.Size(702, 196);
		this.grd_Detail.TabIndex = 81;
		this.grd_Detail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Detail });
		this.contextMenuStrip_Detail.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.toolStripMenuItem_AddRow, this.toolStripMenuItem_RemoveRow });
		this.contextMenuStrip_Detail.Name = "contextMenuStrip_Detail";
		this.contextMenuStrip_Detail.Size = new System.Drawing.Size(141, 48);
		this.toolStripMenuItem_AddRow.Name = "toolStripMenuItem_AddRow";
		this.toolStripMenuItem_AddRow.Size = new System.Drawing.Size(140, 22);
		this.toolStripMenuItem_AddRow.Text = "Add Row";
		this.toolStripMenuItem_AddRow.Click += new System.EventHandler(this.toolStripMenuItem_AddRow_Click);
		this.toolStripMenuItem_RemoveRow.Name = "toolStripMenuItem_RemoveRow";
		this.toolStripMenuItem_RemoveRow.Size = new System.Drawing.Size(140, 22);
		this.toolStripMenuItem_RemoveRow.Text = "Remove row";
		this.toolStripMenuItem_RemoveRow.Click += new System.EventHandler(this.toolStripMenuItem_RemoveRow_Click);
		this.gv_Detail.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Detail.ColumnKey = null;
		this.gv_Detail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumn1, this.col_TaxLevel, this.col_MinAmt, this.col_MaxAmt, this.col_Ratio });
		this.gv_Detail.DisableFormSetting = false;
		this.gv_Detail.DisplayException = false;
		this.gv_Detail.EditColumn = null;
		this.gv_Detail.FieldCurrency = null;
		this.gv_Detail.FieldRate = null;
		this.gv_Detail.FormId = "oPersonalIncomeTax";
		this.gv_Detail.GridControl = this.grd_Detail;
		this.gv_Detail.IgnoreFilter = false;
		this.gv_Detail.IndexEditRow = -1;
		this.gv_Detail.IndicatorWidth = 30;
		this.gv_Detail.IsApzTable = false;
		this.gv_Detail.IsInsertNewRow = false;
		this.gv_Detail.IsLoadingData = false;
		this.gv_Detail.IsMappingContextAction = false;
		this.gv_Detail.ItemId = "APZ_PIT1";
		this.gv_Detail.LstColumnAllowBatch = null;
		this.gv_Detail.LstColumnNotAllowBatch = null;
		this.gv_Detail.LstColumnNotEditable = null;
		this.gv_Detail.LstColumNotSetting = null;
		this.gv_Detail.Name = "gv_Detail";
		this.gv_Detail.OptionsView.RowAutoHeight = true;
		this.gv_Detail.OptionsView.ShowGroupPanel = false;
		this.gv_Detail.UseBatchMod = false;
		this.gv_Detail.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Detail_CustomDrawRowIndicator);
		this.gv_Detail.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Detail_CellValueChanging);
		this.gv_Detail.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_Detail_CustomColumnDisplayText);
		this.gv_Detail.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Detail_ValidatingEditor);
		this.gridColumn1.Caption = "Code";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "Code";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Width = 70;
		this.col_TaxLevel.Caption = "Tax Level";
		this.col_TaxLevel.CurrencyDateField = null;
		this.col_TaxLevel.CurrencyField = null;
		this.col_TaxLevel.CurrencyFixed = null;
		this.col_TaxLevel.CurrencyFixedRate = 0.0;
		this.col_TaxLevel.CurrencyRateField = null;
		this.col_TaxLevel.FieldName = "TaxLevel";
		this.col_TaxLevel.Name = "col_TaxLevel";
		this.col_TaxLevel.PrimaryKey = true;
		this.col_TaxLevel.UnitField = null;
		this.col_TaxLevel.Visible = true;
		this.col_TaxLevel.VisibleIndex = 0;
		this.col_TaxLevel.Width = 130;
		this.col_MinAmt.Caption = "Min Amount";
		this.col_MinAmt.ColumnEdit = this.repositoryItemCalcEdit1;
		this.col_MinAmt.CurrencyDateField = null;
		this.col_MinAmt.CurrencyField = null;
		this.col_MinAmt.CurrencyFixed = null;
		this.col_MinAmt.CurrencyFixedRate = 0.0;
		this.col_MinAmt.CurrencyRateField = null;
		this.col_MinAmt.DisplayFormat.FormatString = "{0:N4}";
		this.col_MinAmt.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.col_MinAmt.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.col_MinAmt.FieldName = "MinAmt";
		this.col_MinAmt.Name = "col_MinAmt";
		this.col_MinAmt.PrimaryKey = false;
		this.col_MinAmt.UnitField = null;
		this.col_MinAmt.Visible = true;
		this.col_MinAmt.VisibleIndex = 1;
		this.col_MinAmt.Width = 151;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.col_MaxAmt.Caption = "Max Amount";
		this.col_MaxAmt.ColumnEdit = this.repositoryItemCalcEdit1;
		this.col_MaxAmt.CurrencyDateField = null;
		this.col_MaxAmt.CurrencyField = null;
		this.col_MaxAmt.CurrencyFixed = null;
		this.col_MaxAmt.CurrencyFixedRate = 0.0;
		this.col_MaxAmt.CurrencyRateField = null;
		this.col_MaxAmt.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.col_MaxAmt.FieldName = "MaxAmt";
		this.col_MaxAmt.Name = "col_MaxAmt";
		this.col_MaxAmt.PrimaryKey = false;
		this.col_MaxAmt.UnitField = null;
		this.col_MaxAmt.Visible = true;
		this.col_MaxAmt.VisibleIndex = 2;
		this.col_MaxAmt.Width = 192;
		this.col_Ratio.Caption = "Ratio";
		this.col_Ratio.ColumnEdit = this.ItemCalcEdit_Ratio;
		this.col_Ratio.CurrencyDateField = null;
		this.col_Ratio.CurrencyField = null;
		this.col_Ratio.CurrencyFixed = null;
		this.col_Ratio.CurrencyFixedRate = 0.0;
		this.col_Ratio.CurrencyRateField = null;
		this.col_Ratio.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Rate;
		this.col_Ratio.FieldName = "Ratio";
		this.col_Ratio.Name = "col_Ratio";
		this.col_Ratio.PrimaryKey = false;
		this.col_Ratio.UnitField = null;
		this.col_Ratio.Visible = true;
		this.col_Ratio.VisibleIndex = 3;
		this.col_Ratio.Width = 131;
		this.ItemCalcEdit_Ratio.AutoHeight = false;
		this.ItemCalcEdit_Ratio.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ItemCalcEdit_Ratio.Name = "ItemCalcEdit_Ratio";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(6, 460);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(93, 22);
		this.btn_Save.TabIndex = 84;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(103, 460);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 22);
		this.btnCancel.TabIndex = 83;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.grd_PersonalIncomeTax.AllowPasteExcel = false;
		this.grd_PersonalIncomeTax.ClipboardData = null;
		this.grd_PersonalIncomeTax.ContextMenuStrip = this.contextMenuStrip_PerInComeTax;
		this.grd_PersonalIncomeTax.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_PersonalIncomeTax.Location = new System.Drawing.Point(0, 0);
		this.grd_PersonalIncomeTax.MainView = this.gv_PersonalIncomeTax;
		this.grd_PersonalIncomeTax.Name = "grd_PersonalIncomeTax";
		this.grd_PersonalIncomeTax.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rep_checkApply, this.rep_dateFrom, this.rep_dateTo, this.rep_caComp, this.rep_caEmp });
		this.grd_PersonalIncomeTax.Size = new System.Drawing.Size(704, 214);
		this.grd_PersonalIncomeTax.TabIndex = 85;
		this.grd_PersonalIncomeTax.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_PersonalIncomeTax });
		this.gv_PersonalIncomeTax.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_PersonalIncomeTax.ColumnKey = null;
		this.gv_PersonalIncomeTax.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.detail_Code, this.col_Name, this.col_FromDate, this.col_ToDate, this.Apply, this.col_Remark });
		this.gv_PersonalIncomeTax.DisableFormSetting = false;
		this.gv_PersonalIncomeTax.DisplayException = false;
		this.gv_PersonalIncomeTax.EditColumn = null;
		this.gv_PersonalIncomeTax.FieldCurrency = null;
		this.gv_PersonalIncomeTax.FieldRate = null;
		this.gv_PersonalIncomeTax.FormId = "oPersonalIncomeTax";
		this.gv_PersonalIncomeTax.GridControl = this.grd_PersonalIncomeTax;
		this.gv_PersonalIncomeTax.IgnoreFilter = false;
		this.gv_PersonalIncomeTax.IndexEditRow = -1;
		this.gv_PersonalIncomeTax.IndicatorWidth = 30;
		this.gv_PersonalIncomeTax.IsApzTable = false;
		this.gv_PersonalIncomeTax.IsInsertNewRow = false;
		this.gv_PersonalIncomeTax.IsLoadingData = false;
		this.gv_PersonalIncomeTax.IsMappingContextAction = false;
		this.gv_PersonalIncomeTax.ItemId = "APZ_OPIT";
		this.gv_PersonalIncomeTax.LstColumnAllowBatch = null;
		this.gv_PersonalIncomeTax.LstColumnNotAllowBatch = null;
		this.gv_PersonalIncomeTax.LstColumnNotEditable = null;
		this.gv_PersonalIncomeTax.LstColumNotSetting = null;
		this.gv_PersonalIncomeTax.Name = "gv_PersonalIncomeTax";
		this.gv_PersonalIncomeTax.OptionsCustomization.AllowSort = false;
		this.gv_PersonalIncomeTax.OptionsView.ShowGroupPanel = false;
		this.gv_PersonalIncomeTax.UseBatchMod = false;
		this.gv_PersonalIncomeTax.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_PersonalIncomeTax_CustomDrawRowIndicator);
		this.gv_PersonalIncomeTax.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_PersonalIncomeTax_FocusedRowObjectChanged);
		this.gv_PersonalIncomeTax.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_PersonalIncomeTax_CellValueChanging);
		this.gv_PersonalIncomeTax.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_PersonalIncomeTax_BeforeLeaveRow);
		this.detail_Code.Caption = "Code";
		this.detail_Code.CurrencyDateField = null;
		this.detail_Code.CurrencyField = null;
		this.detail_Code.CurrencyFixed = null;
		this.detail_Code.CurrencyFixedRate = 0.0;
		this.detail_Code.CurrencyRateField = null;
		this.detail_Code.FieldName = "Code";
		this.detail_Code.Name = "detail_Code";
		this.detail_Code.OptionsColumn.ReadOnly = true;
		this.detail_Code.PrimaryKey = false;
		this.detail_Code.UnitField = null;
		this.col_Name.Caption = "Name";
		this.col_Name.CurrencyDateField = null;
		this.col_Name.CurrencyField = null;
		this.col_Name.CurrencyFixed = null;
		this.col_Name.CurrencyFixedRate = 0.0;
		this.col_Name.CurrencyRateField = null;
		this.col_Name.FieldName = "Name";
		this.col_Name.Name = "col_Name";
		this.col_Name.PrimaryKey = false;
		this.col_Name.UnitField = null;
		this.col_Name.Visible = true;
		this.col_Name.VisibleIndex = 0;
		this.col_FromDate.Caption = "From Date";
		this.col_FromDate.ColumnEdit = this.rep_dateFrom;
		this.col_FromDate.CurrencyDateField = null;
		this.col_FromDate.CurrencyField = null;
		this.col_FromDate.CurrencyFixed = null;
		this.col_FromDate.CurrencyFixedRate = 0.0;
		this.col_FromDate.CurrencyRateField = null;
		this.col_FromDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.col_FromDate.FieldName = "FromDate";
		this.col_FromDate.Name = "col_FromDate";
		this.col_FromDate.PrimaryKey = false;
		this.col_FromDate.UnitField = null;
		this.col_FromDate.Visible = true;
		this.col_FromDate.VisibleIndex = 1;
		this.rep_dateFrom.AutoHeight = false;
		this.rep_dateFrom.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateFrom.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateFrom.IsSearchControl = false;
		this.rep_dateFrom.Name = "rep_dateFrom";
		this.col_ToDate.Caption = "To Date";
		this.col_ToDate.ColumnEdit = this.rep_dateTo;
		this.col_ToDate.CurrencyDateField = null;
		this.col_ToDate.CurrencyField = null;
		this.col_ToDate.CurrencyFixed = null;
		this.col_ToDate.CurrencyFixedRate = 0.0;
		this.col_ToDate.CurrencyRateField = null;
		this.col_ToDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.col_ToDate.FieldName = "ToDate";
		this.col_ToDate.Name = "col_ToDate";
		this.col_ToDate.PrimaryKey = false;
		this.col_ToDate.UnitField = null;
		this.col_ToDate.Visible = true;
		this.col_ToDate.VisibleIndex = 2;
		this.rep_dateTo.AutoHeight = false;
		this.rep_dateTo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateTo.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateTo.IsSearchControl = false;
		this.rep_dateTo.Name = "rep_dateTo";
		this.Apply.Caption = "Apply";
		this.Apply.ColumnEdit = this.rep_checkApply;
		this.Apply.CurrencyDateField = null;
		this.Apply.CurrencyField = null;
		this.Apply.CurrencyFixed = null;
		this.Apply.CurrencyFixedRate = 0.0;
		this.Apply.CurrencyRateField = null;
		this.Apply.FieldName = "Apply";
		this.Apply.Name = "Apply";
		this.Apply.PrimaryKey = false;
		this.Apply.UnitField = null;
		this.Apply.Visible = true;
		this.Apply.VisibleIndex = 3;
		this.rep_checkApply.AutoHeight = false;
		this.rep_checkApply.Name = "rep_checkApply";
		this.rep_checkApply.ValueChecked = "Y";
		this.rep_checkApply.ValueGrayed = "N";
		this.rep_checkApply.ValueUnchecked = "N";
		this.col_Remark.Caption = "Remarks";
		this.col_Remark.CurrencyDateField = null;
		this.col_Remark.CurrencyField = null;
		this.col_Remark.CurrencyFixed = null;
		this.col_Remark.CurrencyFixedRate = 0.0;
		this.col_Remark.CurrencyRateField = null;
		this.col_Remark.FieldName = "Remarks";
		this.col_Remark.Name = "col_Remark";
		this.col_Remark.PrimaryKey = false;
		this.col_Remark.UnitField = null;
		this.col_Remark.Visible = true;
		this.col_Remark.VisibleIndex = 4;
		this.rep_caComp.AutoHeight = false;
		this.rep_caComp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_caComp.Name = "rep_caComp";
		this.rep_caEmp.AutoHeight = false;
		this.rep_caEmp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_caEmp.Name = "rep_caEmp";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(6, 4);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_PersonalIncomeTax);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControl1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(704, 445);
		this.splitContainerControl1.SplitterPosition = 214;
		this.splitContainerControl1.TabIndex = 86;
		this.splitContainerControl1.Text = "splitContainerControl1";
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(722, 489);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.FormId = "oPersonalIncomeTax";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PersonalIncomeTax";
		this.Text = "Personal Income Tax";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_PersonalIncomeTax_FormClosing);
		this.contextMenuStrip_PerInComeTax.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage_Detail.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Detail).EndInit();
		this.contextMenuStrip_Detail.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Detail).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ItemCalcEdit_Ratio).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_PersonalIncomeTax).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_PersonalIncomeTax).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateFrom.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateFrom).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateTo.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateTo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_checkApply).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_caComp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_caEmp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
