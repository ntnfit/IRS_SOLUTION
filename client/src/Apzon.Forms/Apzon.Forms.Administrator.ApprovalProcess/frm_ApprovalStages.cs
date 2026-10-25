using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator.ApprovalProcess;

public class frm_ApprovalStages : XtraFormEx
{
	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isDuplicated;

	private string _userName = string.Empty;

	private IContainer components;

	private LabelLine lbl_StageCode;

	private ButtonEditEx btn_StageCode;

	private LabelLine lbl_StageName;

	private ButtonEditEx btn_StageName;

	private SimpleButtonEx btn_Search;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridControlEx grd_Lines;

	private GridViewEx gv_Lines;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem mnu_DeleteDoc;

	private ToolStripMenuItem mnu_DuplicateDoc;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem deleteToolStripMenuItem1;

	private GridColumnEx gc_StageCode;

	private GridColumnEx gc_StageName;

	private GridColumnEx gc_MaxReqr;

	private GridColumnEx gc_MaxRejReqr;

	private GridColumnEx gc_Remarks;

	private GridColumnEx gc_UserSign;

	private GridColumnEx gc_StageCodeLine;

	private GridColumnEx gc_Authorizer;

	private GridColumnEx gc_Department;

	private RepositoryItemButtonEdit rep_Authorizers;

	private RepositoryItemButtonEdit rep_Department;

	private RepositoryItemCalcEdit rep_MaxReqr;

	private RepositoryItemCalcEdit rep_MaxRejReqr;

	private GridColumnEx gc_AuthorizerId;

	private GridColumnEx gc_DepartmentCode;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btn_OK.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_ApprovalStages()
	{
		InitializeComponent();
	}

	public async Task REFRESH()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			addNewToolStripMenuItem_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.First, Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, gc_StageCode.FieldName)));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.Previous, Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, gc_StageCode.FieldName)));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.Next, Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, gc_StageCode.FieldName)));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.Last, Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, gc_StageCode.FieldName)));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			WinCommons._OpenProcessing();
			btn_StageCode.EditValue = null;
			btn_StageName.EditValue = null;
			frm_ApprovalStages_Load(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void InitializeForm()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ApprovalStages"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ApprovalStages" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				contextMenuStrip1.Enabled = false;
				contextMenuStrip2.Enabled = false;
				btn_Cancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async Task LoadData()
	{
		try
		{
			GridControlEx gridControlEx = grd_Doc;
			gridControlEx.DataSource = await _httpClient.ApprovalStages.GetData();
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataLine(int stageCode)
	{
		try
		{
			GridControlEx gridControlEx = grd_Lines;
			gridControlEx.DataSource = await _httpClient.ApprovalStages.GetDataLines(stageCode);
			grd_Lines.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewDoc()
	{
		try
		{
			DataTable dataTable = grd_Doc.DataSourceTable.Clone();
			DataRow dataRow = dataTable.NewRow();
			dataRow["WstCode"] = _httpClient.ApprovalStages.GetNewCode();
			dataRow["MaxReqr"] = 1;
			dataRow["MaxRejReqr"] = 1;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task AddNewLine(bool isNew, int stageCode)
	{
		try
		{
			DataTable dataTable = grd_Lines.DataSourceTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				await LoadDataLine(0);
				dataTable = grd_Lines.DataSourceTable;
			}
			if (isNew)
			{
				dataTable = dataTable.Clone();
				dataTable.AcceptChanges();
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["WstCode"] = stageCode;
			dataRow["UserId"] = DBNull.Value;
			dataRow["Departmnt"] = DBNull.Value;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Lines.DataSource = dataTable;
			grd_Lines.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChangeViewMode()
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ShowCfl(string searchType)
	{
		try
		{
			using frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ApprovalStages, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				if (searchType.Equals("C"))
				{
					btn_StageCode.Text = Function.ToString(frm_CFL2.ChooseItem["WstCode"]);
				}
				else
				{
					btn_StageName.Text = Function.ToString(frm_CFL2.ChooseItem["Name"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetRecord(Enums.NavigateType type, string stageCode)
	{
		try
		{
			DataTable dataTable = new DataTable();
			switch (type)
			{
			case Enums.NavigateType.First:
				dataTable = _httpClient.ApprovalStages.GetRecord("F", stageCode);
				break;
			case Enums.NavigateType.Last:
				dataTable = _httpClient.ApprovalStages.GetRecord("L", stageCode);
				break;
			case Enums.NavigateType.Previous:
				dataTable = _httpClient.ApprovalStages.GetRecord("P", stageCode);
				break;
			case Enums.NavigateType.Next:
				dataTable = _httpClient.ApprovalStages.GetRecord("N", stageCode);
				break;
			}
			if (dataTable.IsNotNull())
			{
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void frm_ApprovalStages_Load(object sender, EventArgs e)
	{
		try
		{
			await LoadData();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewDoc();
			await AddNewLine(isNew: true, -1);
			_isDuplicated = false;
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void rep_Authorizers_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			List<Common.ParameterValue> list = new List<Common.ParameterValue>();
			if (string.IsNullOrEmpty(Function.ToString(gv_Lines.GetRowCellValue(gv_Lines.FocusedRowHandle, gc_Department.FieldName))))
			{
				list.Add(new Common.ParameterValue("department", -1));
			}
			else
			{
				list.Add(new Common.ParameterValue("department", Function.ParseInt(gv_Lines.GetRowCellValue(gv_Lines.FocusedRowHandle, gc_DepartmentCode.FieldName))));
			}
			if (!string.IsNullOrEmpty(_userName))
			{
				list.Add(new Common.ParameterValue("username", _userName));
			}
			using frm_CFL frm = new frm_CFL(Enums.TypeCFL.ApprovalUsers, true, "", "", Enums.ItemObjectType.NoItemType, list, "");
			DataTable dataTable = grd_Lines.DataSourceTable.Copy();
			dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
			dataTable.AcceptChanges();
			frm.dtSource = dataTable;
			frm.ShowDialog(this);
			if (frm.ChooseItems != null && frm.ChooseItems.Count > 0)
			{
				List<DataRow> chooseItems = frm.ChooseItems;
				DataTable dataSourceTable = grd_Lines.DataSourceTable;
				int focusedRowHandle = gv_Lines.FocusedRowHandle;
				dataSourceTable.Rows[focusedRowHandle]["UserId"] = chooseItems[0]["UserId"];
				dataSourceTable.Rows[focusedRowHandle]["UserName"] = chooseItems[0]["UserName"];
				dataSourceTable.Rows[focusedRowHandle]["DepartmntCode"] = chooseItems[0]["DepartmntCode"];
				dataSourceTable.Rows[focusedRowHandle]["Departmnt"] = chooseItems[0]["Departmnt"];
				for (int i = 1; i < chooseItems.Count; i++)
				{
					DataRow dataRow = dataSourceTable.NewRow();
					dataRow["UserId"] = chooseItems[i]["UserId"];
					dataRow["UserName"] = chooseItems[i]["UserName"];
					dataRow["DepartmntCode"] = chooseItems[i]["DepartmntCode"];
					dataRow["Departmnt"] = chooseItems[i]["Departmnt"];
					dataSourceTable.Rows.Add(dataRow);
				}
				dataSourceTable.AcceptChanges();
				grd_Lines.DataSource = dataSourceTable;
				grd_Lines.RefreshDataSource();
				await AddNewLine(isNew: false, Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, gc_StageCode.FieldName)));
				ChangeViewMode();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Lines_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		_ = 1;
		try
		{
			if (e.Column.Name.Equals(gc_Authorizer.Name))
			{
				DataRow rowFocus = gv_Lines.GetDataRow(e.RowHandle);
				int stageCode = Function.ParseInt(gv_Lines.GetRowCellValue(e.RowHandle, gc_StageCodeLine.FieldName));
				string userName = Function.ToString(e.Value);
				DataTable dataTable = await _httpClient.ApprovalStages.GetUserInfo(userName);
				if (dataTable.IsNotNull())
				{
					DataRow dataRow = dataTable.Rows[0];
					rowFocus["UserID"] = Function.ParseInt(dataRow["USERID"]);
					rowFocus["UserName"] = Function.ToString(dataRow["UserName"]);
					rowFocus["DepartmntCode"] = Function.ToString(dataRow["DepartmntCode"]);
					rowFocus["Departmnt"] = Function.ToString(dataRow["DepartmntName"]);
					rowFocus.AcceptChanges();
					await AddNewLine(isNew: false, stageCode);
					_userName = string.Empty;
				}
				else
				{
					_userName = Function.ToString(e.Value);
					rep_Authorizers_ButtonClick(null, null);
				}
			}
			ChangeViewMode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Department_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index != 0)
			{
				return;
			}
			using frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Departments, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gv_Lines.SetRowCellValue(gv_Lines.FocusedRowHandle, gc_DepartmentCode.FieldName, frm_CFL2.ChooseItem["Code"]);
				gv_Lines.SetRowCellValue(gv_Lines.FocusedRowHandle, gc_Department.FieldName, frm_CFL2.ChooseItem["Name"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.Name.Equals(gc_StageCode.Name))
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gv_Doc.FocusedColumn.Name.Equals(gc_StageCode.Name))
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Lines.SelectedRowsCount != 1 || gv_Lines.FocusedRowHandle != gv_Lines.RowCount - 1 || !string.IsNullOrEmpty(Function.ToString(gv_Lines.GetRowCellValue(gv_Lines.FocusedRowHandle, gc_Authorizer.FieldName))) || !string.IsNullOrEmpty(Function.ToString(gv_Lines.GetRowCellValue(gv_Lines.FocusedRowHandle, gc_Department.FieldName))))
			{
				gv_Lines.DeleteDataSourceRowSelected();
				gv_Lines.RefreshEditor(updateEditorValue: true);
				if (gv_Lines.RowCount == 0)
				{
					await AddNewLine(isNew: false, Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, gc_StageCode.FieldName)));
				}
				ChangeViewMode();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				btn_OK.Enabled = false;
				grd_Doc.DataSourceTable.AcceptChanges();
				grd_Lines.DataSourceTable.AcceptChanges();
				DataRow dataRow = gv_Doc.GetDataRow(gv_Doc.FocusedRowHandle);
				Tuple<DataTable, DataTable, string> tuple = new Tuple<DataTable, DataTable, string>(new List<DataRow> { dataRow }.CopyToDataTable(), grd_Lines.DataSourceTable.Copy(), (FormMode == Enums.FormViewModeType.Add) ? "A" : "U");
				HttpResult httpResult = await _httpClient.ApprovalStages.Save(tuple);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					btn_OK.Enabled = true;
					return;
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				await LoadData();
				FormMode = Enums.FormViewModeType.View;
				_isDuplicated = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btn_OK.Enabled = true;
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		ChangeViewMode();
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (gv_Doc.FocusedRowHandle == -1)
			{
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				int item = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, gc_StageCode.FieldName));
				HttpResult httpResult = await _httpClient.ApprovalStages.Delete(new Tuple<int>(item));
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					return;
				}
				WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
				await LoadData();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.FocusedRowHandle == -1)
			{
				return;
			}
			_isDuplicated = true;
			DataTable dataTable = grd_Doc.DataSourceTable.Copy().Clone();
			DataTable dataTable2 = grd_Lines.DataSourceTable.Copy();
			int newCode = _httpClient.ApprovalStages.GetNewCode();
			DataRow dataRow = dataTable.NewRow();
			dataRow.ItemArray = gv_Doc.GetDataRow(gv_Doc.FocusedRowHandle).ItemArray;
			dataRow["WstCode"] = newCode;
			dataRow["Name"] = DBNull.Value;
			dataTable.Rows.Add(dataRow);
			foreach (DataRow row in dataTable2.Rows)
			{
				row["WstCode"] = newCode;
			}
			dataTable.AcceptChanges();
			dataTable2.AcceptChanges();
			grd_Doc.DataSource = dataTable;
			grd_Lines.DataSource = dataTable2;
			grd_Doc.DataSourceTable.AcceptChanges();
			grd_Lines.DataSourceTable.AcceptChanges();
			FormMode = Enums.FormViewModeType.Add;
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

	private void frm_ApprovalStages_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			btn_Search.Enabled = false;
			DataTable dataTable = await _httpClient.ApprovalStages.SearchData(Function.ParseInt(btn_StageCode.Text), btn_StageName.Text);
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
			if (!dataTable.IsNotNull())
			{
				grd_Lines.DataSourceTable.Clear();
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btn_Search.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void btn_StageCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		ShowCfl("C");
	}

	private void btn_StageName_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		ShowCfl("N");
	}

	private void gv_Doc_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			if (gv_Doc.RowCount == 0)
			{
				mnu_DeleteDoc.Enabled = false;
				mnu_DuplicateDoc.Enabled = false;
			}
			else
			{
				mnu_DeleteDoc.Enabled = true;
				mnu_DuplicateDoc.Enabled = true;
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
		_ = 1;
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No)
			{
				int stageCode = Function.ParseInt(gv_Doc.GetRowCellValue(e.FocusedRowHandle, gc_StageCode.FieldName));
				if (!_isDuplicated)
				{
					await LoadDataLine(stageCode);
				}
				await AddNewLine(isNew: false, stageCode);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.ApprovalProcess.frm_ApprovalStages));
		this.lbl_StageCode = new Apzon.Controls.Controls.LabelLine();
		this.btn_StageCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.lbl_StageName = new Apzon.Controls.Controls.LabelLine();
		this.btn_StageName = new Apzon.Controls.Editor.ButtonEditEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.mnu_DeleteDoc = new System.Windows.Forms.ToolStripMenuItem();
		this.mnu_DuplicateDoc = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gc_StageCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_StageName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_MaxReqr = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_MaxReqr = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_MaxRejReqr = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_MaxRejReqr = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.grd_Lines = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Lines = new Apzon.Controls.Grid.GridViewEx();
		this.gc_StageCodeLine = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Department = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_AuthorizerId = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Authorizer = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Authorizers = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_DepartmentCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Department = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.btn_StageCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_StageName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MaxReqr).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MaxRejReqr).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Lines).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Lines).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Department).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Authorizers).BeginInit();
		base.SuspendLayout();
		this.lbl_StageCode.EditControl = null;
		this.lbl_StageCode.EditValue = "Stage Code";
		this.lbl_StageCode.FieldName = null;
		this.lbl_StageCode.HideWhenEmpty = false;
		this.lbl_StageCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_StageCode.Location = new System.Drawing.Point(12, 10);
		this.lbl_StageCode.Name = "lbl_StageCode";
		this.lbl_StageCode.ReadOnly = false;
		this.lbl_StageCode.SettingInVisible = false;
		this.lbl_StageCode.SettingReadonly = false;
		this.lbl_StageCode.ShowUnderLine = true;
		this.lbl_StageCode.Size = new System.Drawing.Size(80, 20);
		this.lbl_StageCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_StageCode.TabIndex = 0;
		this.lbl_StageCode.Utext = "Stage Code";
		this.btn_StageCode.EditControl = null;
		this.btn_StageCode.FieldName = null;
		this.btn_StageCode.IsSearchControl = false;
		this.btn_StageCode.Location = new System.Drawing.Point(92, 7);
		this.btn_StageCode.Name = "btn_StageCode";
		this.btn_StageCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.btn_StageCode.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_StageCode.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.btn_StageCode.Properties.AppearanceFocused.Options.UseFont = true;
		this.btn_StageCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btn_StageCode.SettingInVisible = false;
		this.btn_StageCode.SettingReadonly = false;
		this.btn_StageCode.Size = new System.Drawing.Size(146, 20);
		this.btn_StageCode.TabIndex = 1;
		this.btn_StageCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_StageCode_ButtonClick);
		this.lbl_StageName.EditControl = null;
		this.lbl_StageName.EditValue = "Stage Name";
		this.lbl_StageName.FieldName = null;
		this.lbl_StageName.HideWhenEmpty = false;
		this.lbl_StageName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_StageName.Location = new System.Drawing.Point(251, 10);
		this.lbl_StageName.Name = "lbl_StageName";
		this.lbl_StageName.ReadOnly = false;
		this.lbl_StageName.SettingInVisible = false;
		this.lbl_StageName.SettingReadonly = false;
		this.lbl_StageName.ShowUnderLine = true;
		this.lbl_StageName.Size = new System.Drawing.Size(80, 20);
		this.lbl_StageName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_StageName.TabIndex = 0;
		this.lbl_StageName.Utext = "Stage Name";
		this.btn_StageName.EditControl = null;
		this.btn_StageName.FieldName = null;
		this.btn_StageName.IsSearchControl = false;
		this.btn_StageName.Location = new System.Drawing.Point(331, 7);
		this.btn_StageName.Name = "btn_StageName";
		this.btn_StageName.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.btn_StageName.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_StageName.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.btn_StageName.Properties.AppearanceFocused.Options.UseFont = true;
		this.btn_StageName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btn_StageName.SettingInVisible = false;
		this.btn_StageName.SettingReadonly = false;
		this.btn_StageName.Size = new System.Drawing.Size(146, 20);
		this.btn_StageName.TabIndex = 2;
		this.btn_StageName.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_StageName_ButtonClick);
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(483, 7);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 20);
		this.btn_Search.TabIndex = 3;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(5, 36);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Lines);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(710, 423);
		this.splitContainerControl1.SplitterPosition = 186;
		this.splitContainerControl1.TabIndex = 4;
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_MaxReqr, this.rep_MaxRejReqr });
		this.grd_Doc.Size = new System.Drawing.Size(710, 186);
		this.grd_Doc.TabIndex = 5;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addNewToolStripMenuItem, this.mnu_DeleteDoc, this.mnu_DuplicateDoc });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(125, 70);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.mnu_DeleteDoc.Name = "mnu_DeleteDoc";
		this.mnu_DeleteDoc.Size = new System.Drawing.Size(124, 22);
		this.mnu_DeleteDoc.Text = "Delete";
		this.mnu_DeleteDoc.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.mnu_DuplicateDoc.Name = "mnu_DuplicateDoc";
		this.mnu_DuplicateDoc.Size = new System.Drawing.Size(124, 22);
		this.mnu_DuplicateDoc.Text = "Duplicate";
		this.mnu_DuplicateDoc.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gc_StageCode, this.gc_StageName, this.gc_MaxReqr, this.gc_MaxRejReqr, this.gc_Remarks, this.gc_UserSign });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oApprovalStages";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OWST";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Doc.OptionsPrint.AutoWidth = false;
		this.gv_Doc.OptionsPrint.UsePrintStyles = false;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupedColumns = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_Doc_MouseDown);
		this.gc_StageCode.Caption = "Code";
		this.gc_StageCode.CurrencyDateField = null;
		this.gc_StageCode.CurrencyField = null;
		this.gc_StageCode.CurrencyFixed = null;
		this.gc_StageCode.CurrencyFixedRate = 0.0;
		this.gc_StageCode.CurrencyRateField = null;
		this.gc_StageCode.FieldName = "WstCode";
		this.gc_StageCode.Name = "gc_StageCode";
		this.gc_StageCode.OptionsColumn.AllowEdit = false;
		this.gc_StageCode.OptionsColumn.ReadOnly = true;
		this.gc_StageCode.PrimaryKey = true;
		this.gc_StageCode.UnitField = null;
		this.gc_StageCode.Visible = true;
		this.gc_StageCode.VisibleIndex = 0;
		this.gc_StageName.Caption = "Name";
		this.gc_StageName.CurrencyDateField = null;
		this.gc_StageName.CurrencyField = null;
		this.gc_StageName.CurrencyFixed = null;
		this.gc_StageName.CurrencyFixedRate = 0.0;
		this.gc_StageName.CurrencyRateField = null;
		this.gc_StageName.FieldName = "Name";
		this.gc_StageName.Name = "gc_StageName";
		this.gc_StageName.PrimaryKey = false;
		this.gc_StageName.UnitField = null;
		this.gc_StageName.Visible = true;
		this.gc_StageName.VisibleIndex = 1;
		this.gc_MaxReqr.Caption = "No. of Approvals Required";
		this.gc_MaxReqr.ColumnEdit = this.rep_MaxReqr;
		this.gc_MaxReqr.CurrencyDateField = null;
		this.gc_MaxReqr.CurrencyField = null;
		this.gc_MaxReqr.CurrencyFixed = null;
		this.gc_MaxReqr.CurrencyFixedRate = 0.0;
		this.gc_MaxReqr.CurrencyRateField = null;
		this.gc_MaxReqr.FieldName = "MaxReqr";
		this.gc_MaxReqr.Name = "gc_MaxReqr";
		this.gc_MaxReqr.PrimaryKey = false;
		this.gc_MaxReqr.UnitField = null;
		this.gc_MaxReqr.Visible = true;
		this.gc_MaxReqr.VisibleIndex = 2;
		this.gc_MaxReqr.Width = 150;
		this.rep_MaxReqr.AutoHeight = false;
		this.rep_MaxReqr.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_MaxReqr.Name = "rep_MaxReqr";
		this.gc_MaxRejReqr.Caption = "No. of Rejections Required";
		this.gc_MaxRejReqr.ColumnEdit = this.rep_MaxRejReqr;
		this.gc_MaxRejReqr.CurrencyDateField = null;
		this.gc_MaxRejReqr.CurrencyField = null;
		this.gc_MaxRejReqr.CurrencyFixed = null;
		this.gc_MaxRejReqr.CurrencyFixedRate = 0.0;
		this.gc_MaxRejReqr.CurrencyRateField = null;
		this.gc_MaxRejReqr.FieldName = "MaxRejReqr";
		this.gc_MaxRejReqr.Name = "gc_MaxRejReqr";
		this.gc_MaxRejReqr.PrimaryKey = false;
		this.gc_MaxRejReqr.UnitField = null;
		this.gc_MaxRejReqr.Visible = true;
		this.gc_MaxRejReqr.VisibleIndex = 3;
		this.gc_MaxRejReqr.Width = 150;
		this.rep_MaxRejReqr.AutoHeight = false;
		this.rep_MaxRejReqr.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_MaxRejReqr.Name = "rep_MaxRejReqr";
		this.gc_Remarks.Caption = "Remarks";
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
		this.gc_Remarks.VisibleIndex = 4;
		this.gc_UserSign.Caption = "User Signature";
		this.gc_UserSign.CurrencyDateField = null;
		this.gc_UserSign.CurrencyField = null;
		this.gc_UserSign.CurrencyFixed = null;
		this.gc_UserSign.CurrencyFixedRate = 0.0;
		this.gc_UserSign.CurrencyRateField = null;
		this.gc_UserSign.FieldName = "UserSign";
		this.gc_UserSign.Name = "gc_UserSign";
		this.gc_UserSign.OptionsColumn.AllowEdit = false;
		this.gc_UserSign.OptionsColumn.ReadOnly = true;
		this.gc_UserSign.PrimaryKey = false;
		this.gc_UserSign.UnitField = null;
		this.grd_Lines.AllowPasteExcel = true;
		this.grd_Lines.ClipboardData = null;
		this.grd_Lines.ContextMenuStrip = this.contextMenuStrip2;
		this.grd_Lines.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Lines.Location = new System.Drawing.Point(0, 0);
		this.grd_Lines.MainView = this.gv_Lines;
		this.grd_Lines.Name = "grd_Lines";
		this.grd_Lines.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_Authorizers, this.rep_Department });
		this.grd_Lines.Size = new System.Drawing.Size(710, 227);
		this.grd_Lines.TabIndex = 6;
		this.grd_Lines.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Lines });
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteToolStripMenuItem1 });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(108, 26);
		this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
		this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
		this.deleteToolStripMenuItem1.Text = "Delete";
		this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.deleteToolStripMenuItem1_Click);
		this.gv_Lines.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Lines.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Lines.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Lines.ColumnKey = null;
		this.gv_Lines.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gc_StageCodeLine, this.gc_AuthorizerId, this.gc_Authorizer, this.gc_DepartmentCode, this.gc_Department });
		this.gv_Lines.DisableFormSetting = false;
		this.gv_Lines.DisplayException = false;
		this.gv_Lines.EditColumn = null;
		this.gv_Lines.FieldCurrency = null;
		this.gv_Lines.FieldRate = null;
		this.gv_Lines.FormId = "oApprovalStages";
		this.gv_Lines.GridControl = this.grd_Lines;
		this.gv_Lines.IgnoreFilter = false;
		this.gv_Lines.IndexEditRow = -1;
		this.gv_Lines.IndicatorWidth = 35;
		this.gv_Lines.IsApzTable = false;
		this.gv_Lines.IsInsertNewRow = false;
		this.gv_Lines.IsLoadingData = false;
		this.gv_Lines.IsMappingContextAction = false;
		this.gv_Lines.ItemId = "WST1";
		this.gv_Lines.LstColumnAllowBatch = null;
		this.gv_Lines.LstColumnNotAllowBatch = null;
		this.gv_Lines.LstColumnNotEditable = null;
		this.gv_Lines.LstColumNotSetting = null;
		this.gv_Lines.Name = "gv_Lines";
		this.gv_Lines.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Lines.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Lines.OptionsNavigation.AutoFocusNewRow = true;
		this.gv_Lines.OptionsPrint.AutoWidth = false;
		this.gv_Lines.OptionsPrint.UsePrintStyles = false;
		this.gv_Lines.OptionsSelection.MultiSelect = true;
		this.gv_Lines.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Lines.OptionsView.ColumnAutoWidth = false;
		this.gv_Lines.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Lines.OptionsView.ShowGroupedColumns = true;
		this.gv_Lines.OptionsView.ShowGroupPanel = false;
		this.gv_Lines.UseBatchMod = false;
		this.gv_Lines.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Lines_CellValueChanged);
		this.gc_StageCodeLine.Caption = "Stage Code";
		this.gc_StageCodeLine.ColumnEdit = this.rep_Department;
		this.gc_StageCodeLine.CurrencyDateField = null;
		this.gc_StageCodeLine.CurrencyField = null;
		this.gc_StageCodeLine.CurrencyFixed = null;
		this.gc_StageCodeLine.CurrencyFixedRate = 0.0;
		this.gc_StageCodeLine.CurrencyRateField = null;
		this.gc_StageCodeLine.FieldName = "WstCode";
		this.gc_StageCodeLine.Name = "gc_StageCodeLine";
		this.gc_StageCodeLine.OptionsColumn.AllowEdit = false;
		this.gc_StageCodeLine.PrimaryKey = true;
		this.gc_StageCodeLine.UnitField = null;
		this.rep_Department.AutoHeight = false;
		this.rep_Department.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Department.Name = "rep_Department";
		this.rep_Department.ReadOnly = true;
		this.rep_Department.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Department_ButtonClick);
		this.gc_AuthorizerId.Caption = "User Id";
		this.gc_AuthorizerId.CurrencyDateField = null;
		this.gc_AuthorizerId.CurrencyField = null;
		this.gc_AuthorizerId.CurrencyFixed = null;
		this.gc_AuthorizerId.CurrencyFixedRate = 0.0;
		this.gc_AuthorizerId.CurrencyRateField = null;
		this.gc_AuthorizerId.FieldName = "UserId";
		this.gc_AuthorizerId.Name = "gc_AuthorizerId";
		this.gc_AuthorizerId.OptionsColumn.AllowEdit = false;
		this.gc_AuthorizerId.OptionsColumn.ReadOnly = true;
		this.gc_AuthorizerId.PrimaryKey = false;
		this.gc_AuthorizerId.UnitField = null;
		this.gc_Authorizer.Caption = "Authorizer";
		this.gc_Authorizer.ColumnEdit = this.rep_Authorizers;
		this.gc_Authorizer.CurrencyDateField = null;
		this.gc_Authorizer.CurrencyField = null;
		this.gc_Authorizer.CurrencyFixed = null;
		this.gc_Authorizer.CurrencyFixedRate = 0.0;
		this.gc_Authorizer.CurrencyRateField = null;
		this.gc_Authorizer.FieldName = "UserName";
		this.gc_Authorizer.Name = "gc_Authorizer";
		this.gc_Authorizer.PrimaryKey = false;
		this.gc_Authorizer.UnitField = null;
		this.gc_Authorizer.Visible = true;
		this.gc_Authorizer.VisibleIndex = 0;
		this.gc_Authorizer.Width = 100;
		this.rep_Authorizers.AutoHeight = false;
		this.rep_Authorizers.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Authorizers.Name = "rep_Authorizers";
		this.rep_Authorizers.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Authorizers_ButtonClick);
		this.gc_DepartmentCode.Caption = "Department Code";
		this.gc_DepartmentCode.CurrencyDateField = null;
		this.gc_DepartmentCode.CurrencyField = null;
		this.gc_DepartmentCode.CurrencyFixed = null;
		this.gc_DepartmentCode.CurrencyFixedRate = 0.0;
		this.gc_DepartmentCode.CurrencyRateField = null;
		this.gc_DepartmentCode.FieldName = "DepartmntCode";
		this.gc_DepartmentCode.Name = "gc_DepartmentCode";
		this.gc_DepartmentCode.OptionsColumn.AllowEdit = false;
		this.gc_DepartmentCode.OptionsColumn.ReadOnly = true;
		this.gc_DepartmentCode.PrimaryKey = false;
		this.gc_DepartmentCode.UnitField = null;
		this.gc_Department.Caption = "Department";
		this.gc_Department.ColumnEdit = this.rep_Department;
		this.gc_Department.CurrencyDateField = null;
		this.gc_Department.CurrencyField = null;
		this.gc_Department.CurrencyFixed = null;
		this.gc_Department.CurrencyFixedRate = 0.0;
		this.gc_Department.CurrencyRateField = null;
		this.gc_Department.FieldName = "Departmnt";
		this.gc_Department.Name = "gc_Department";
		this.gc_Department.PrimaryKey = false;
		this.gc_Department.UnitField = null;
		this.gc_Department.Visible = true;
		this.gc_Department.VisibleIndex = 1;
		this.gc_Department.Width = 100;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(561, 465);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 20);
		this.btn_OK.TabIndex = 7;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(640, 465);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 8;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(720, 495);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.btn_StageName);
		base.Controls.Add(this.lbl_StageName);
		base.Controls.Add(this.btn_StageCode);
		base.Controls.Add(this.lbl_StageCode);
		base.FormId = "oApprovalStages";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ApprovalStages";
		base.ShowInTaskbar = false;
		this.Text = "Approval Stages";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_ApprovalStages_FormClosing);
		base.Load += new System.EventHandler(this.frm_ApprovalStages_Load);
		((System.ComponentModel.ISupportInitialize)this.btn_StageCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_StageName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MaxReqr).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MaxRejReqr).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Lines).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Lines).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Department).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Authorizers).EndInit();
		base.ResumeLayout(false);
	}
}
