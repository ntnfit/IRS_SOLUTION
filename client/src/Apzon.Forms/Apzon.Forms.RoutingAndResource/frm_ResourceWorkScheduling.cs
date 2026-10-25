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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Global;
using DevExpress.DataProcessing;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.RoutingAndResource;

public class frm_ResourceWorkScheduling : XtraFormEx
{
	public const string OkString = "Ok";

	public const string SaveString = "Save";

	public const string UpdateString = "Update";

	private IContainer components;

	private SplitContainerControl splitDoc;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridControlEx grdLine;

	private GridViewEx gvLine;

	private SimpleButtonEx btnSave;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnSearch;

	private LabelLine lbCode;

	private LabelLine lbStatus;

	private ButtonEditEx txtCode;

	private LookUpEditEx txtStatus;

	private GridColumnEx gcDocCode;

	private GridColumnEx gcDocName;

	private GridColumnEx gcDocStatus;

	private RepositoryItemLookUpEdit rDocStatus;

	private GridColumnEx gcDocBranch;

	private GridColumnEx gcDocNotes;

	private RepositoryItemLookUpEdit rDocBranch;

	private GridColumnEx gcLineWorkDate;

	private RepositoryItemDateEditEx rDate;

	private GridColumnEx gcLineShift;

	private RepositoryItemButtonEdit rLineShift;

	private GridColumnEx gcLineNotes;

	private LookUpEditEx txtYear;

	private LabelLine lbYear;

	private ContextMenuStrip contextMenuStripDoc;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				btnCancel.Enabled = true;
				return;
			}
			switch (value)
			{
			case Enums.FormViewModeType.Add:
			{
				btnSave.Text = "Save";
				addNewToolStripMenuItem.Enabled = false;
				deleteRowToolStripMenuItem.Enabled = false;
				GridViewEx gridViewEx4 = gvDoc;
				bool readOnly = (gvLine.ReadOnly = false);
				gridViewEx4.ReadOnly = readOnly;
				gcDocCode.OptionsColumn.ReadOnly = false;
				SimpleButtonEx simpleButtonEx7 = btnSave;
				SimpleButtonEx simpleButtonEx8 = btnSearch;
				bool flag2 = (btnCancel.Enabled = true);
				readOnly = (simpleButtonEx8.Enabled = flag2);
				simpleButtonEx7.Enabled = readOnly;
				break;
			}
			case Enums.FormViewModeType.Update:
			{
				btnSave.Text = "Update";
				addNewToolStripMenuItem.Enabled = true;
				deleteRowToolStripMenuItem.Enabled = true;
				GridViewEx gridViewEx3 = gvDoc;
				bool readOnly = (gvLine.ReadOnly = false);
				gridViewEx3.ReadOnly = readOnly;
				gcDocCode.OptionsColumn.ReadOnly = true;
				SimpleButtonEx simpleButtonEx5 = btnSave;
				SimpleButtonEx simpleButtonEx6 = btnSearch;
				bool flag2 = (btnCancel.Enabled = true);
				readOnly = (simpleButtonEx6.Enabled = flag2);
				simpleButtonEx5.Enabled = readOnly;
				break;
			}
			case Enums.FormViewModeType.View:
			{
				btnSave.Text = "Ok";
				addNewToolStripMenuItem.Enabled = true;
				deleteRowToolStripMenuItem.Enabled = true;
				GridViewEx gridViewEx2 = gvDoc;
				bool readOnly = (gvLine.ReadOnly = false);
				gridViewEx2.ReadOnly = readOnly;
				gcDocCode.OptionsColumn.ReadOnly = true;
				SimpleButtonEx simpleButtonEx3 = btnSave;
				SimpleButtonEx simpleButtonEx4 = btnSearch;
				bool flag2 = (btnCancel.Enabled = true);
				readOnly = (simpleButtonEx4.Enabled = flag2);
				simpleButtonEx3.Enabled = readOnly;
				break;
			}
			case Enums.FormViewModeType.Busy:
			{
				GridViewEx gridViewEx = gvDoc;
				bool readOnly = (gvLine.ReadOnly = true);
				gridViewEx.ReadOnly = readOnly;
				addNewToolStripMenuItem.Enabled = false;
				deleteRowToolStripMenuItem.Enabled = false;
				SimpleButtonEx simpleButtonEx = btnSave;
				SimpleButtonEx simpleButtonEx2 = btnSearch;
				bool flag2 = (btnCancel.Enabled = false);
				readOnly = (simpleButtonEx2.Enabled = flag2);
				simpleButtonEx.Enabled = readOnly;
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_ResourceWorkScheduling()
	{
		InitializeComponent();
	}

	private async void frm_ResourceWorkScheduling_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			LoadStatus();
			LoadViewType();
			await LoadDataBranch();
			await SearchData(Function.ToString(txtCode.EditValue), Function.ToString(txtStatus.EditValue), Enums.NavigateType.Current);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gvDoc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			await GetDataLine(Function.ToString(gvDoc.GetFocusedRowCellValue(gcDocCode)), Function.ParseInt(txtYear.EditValue));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void txtYear_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			await GetDataLine(Function.ToString(gvDoc.GetFocusedRowCellValue(gcDocCode)), Function.ParseInt(txtYear.EditValue));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void gvLine_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void gvDoc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				e.Allow = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		REFRESH();
	}

	private async void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType formView = FormMode;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			FormMode = Enums.FormViewModeType.Busy;
			WinCommons._OpenProcessing();
			HttpResult httpResult = await HttpClient.ResourceWorkScheduleClient.Delete(Function.ToString(gvDoc.GetFocusedRowCellValue(gcDocCode)));
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				formView = Enums.FormViewModeType.View;
				await SearchData("", "", Enums.NavigateType.Current);
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = formView;
		WinCommons._CloseProcessing();
	}

	private void rLineShift_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string[] prms = new string[1] { Function.ToString(gvDoc.GetFocusedRowCellValue(gcDocBranch)) };
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.DefinedShift, multiSelect: true, "", "", Enums.ItemObjectType.NoItemType, null, "", prms);
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count <= 0)
			{
				return;
			}
			string text = "";
			foreach (DataRow chooseItem in frm_CFL2.ChooseItems)
			{
				text = ((!string.IsNullOrEmpty(text)) ? (text + "," + Function.ToString(chooseItem["Code"])) : Function.ToString(chooseItem["Code"]));
			}
			gvLine.SetFocusedRowCellValue(gcLineShift.FieldName, text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvLine_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gvLine.FocusedColumn.FieldName.Equals(gcLineShift.FieldName) && Function.ParseDateTimes(gvLine.GetFocusedRowCellValue(gcLineWorkDate)).Date < DateTime.Now.Date)
			{
				WinCommons.Warning(Enums.MessageCode.YouCannotChangeScheduleOfPastDay, "");
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btnSearch_Click(object sender, EventArgs e)
	{
		try
		{
			await SearchData(Function.ToString(txtCode.EditValue), Function.ToString(txtStatus.EditValue), Enums.NavigateType.Current);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListResourceWorkSchedule, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				txtCode.EditValue = Function.ToString(frm_CFL2.ChooseItem["Code"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvLine_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			if (e.Button != MouseButtons.Right || !(gvLine.FocusedColumn.FieldName == gcLineShift.FieldName))
			{
				return;
			}
			string[] prms = new string[1] { Function.ToString(gvDoc.GetFocusedRowCellValue(gcDocBranch)) };
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.DefinedShift, multiSelect: true, "", "", Enums.ItemObjectType.NoItemType, null, "", prms);
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count <= 0)
			{
				return;
			}
			string text = "";
			foreach (DataRow chooseItem in frm_CFL2.ChooseItems)
			{
				text = ((!string.IsNullOrEmpty(text)) ? (text + "," + Function.ToString(chooseItem["Code"])) : Function.ToString(chooseItem["Code"]));
			}
			int[] selectedRows = gvLine.GetSelectedRows();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				if (!(Function.ParseDateTimes(gvLine.GetRowCellValue(selectedRows[i], gcLineWorkDate.FieldName)).Date < DateTime.Now.Date))
				{
					gvLine.SetRowCellValue(selectedRows[i], gcLineShift.FieldName, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btnSave_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType formView = FormMode;
		try
		{
			if (formView != Enums.FormViewModeType.Update && formView != Enums.FormViewModeType.Add)
			{
				Close();
				return;
			}
			if (!grdDoc.DataSourceTable.IsNotNull() || !grdLine.DataSourceTable.IsNotNull())
			{
				return;
			}
			DataTable dataTable = grdDoc.DataSourceTable.Clone();
			DataRow dataRow = dataTable.NewRow();
			dataRow.ItemArray = gvDoc.GetFocusedDataRow().ItemArray;
			if (string.IsNullOrEmpty(Function.ToString(dataRow["Code"])))
			{
				WinCommons.Error(Enums.MessageCode.YouMustEnterCodeBeforeSave, "");
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			dataTable.Rows.Add(dataRow);
			DataTable dataSourceTable = grdLine.DataSourceTable;
			DataTable dataTable2 = grdLine.DataSourceTable.Clone();
			for (int i = 0; i < dataSourceTable.Rows.Count; i++)
			{
				string text = Function.ToString(dataSourceTable.Rows[i]["Shift"]);
				if (string.IsNullOrEmpty(text))
				{
					DataRow dataRow2 = dataTable2.NewRow();
					dataRow2.ItemArray = dataSourceTable.Rows[i].ItemArray;
					dataRow2["Line"] = 0;
					dataTable2.Rows.Add(dataRow2);
					continue;
				}
				int num = 0;
				string[] array = text.Split(',');
				foreach (string value in array)
				{
					DataRow dataRow2 = dataTable2.NewRow();
					dataRow2.ItemArray = dataSourceTable.Rows[i].ItemArray;
					dataRow2["Line"] = num;
					dataRow2["Shift"] = value;
					dataTable2.Rows.Add(dataRow2);
					num++;
				}
			}
			dataTable.TableName = "APZ_ORWS";
			dataTable2.TableName = "APZ_RWS1";
			DataSet dataSet = new DataSet();
			dataSet.Tables.AddRange(new DataTable[2] { dataTable, dataTable2 });
			HttpResult httpResult = ((formView != Enums.FormViewModeType.Add) ? (await HttpClient.ResourceWorkScheduleClient.Update(dataSet)) : (await HttpClient.ResourceWorkScheduleClient.AddNew(dataSet)));
			HttpResult httpResult2 = httpResult;
			if (string.IsNullOrEmpty(httpResult2.Message))
			{
				formView = Enums.FormViewModeType.View;
				await SearchData("", "", Enums.NavigateType.Current);
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult2.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = formView;
		WinCommons._CloseProcessing();
	}

	private void Cancel_Click(object sender, EventArgs e)
	{
		if ((FormMode != Enums.FormViewModeType.Update && FormMode != Enums.FormViewModeType.Add) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel) != DialogResult.Cancel)
		{
			Close();
		}
	}

	public async Task REFRESH()
	{
		try
		{
			if ((FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.No)
			{
				AddNewDoc();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			string code = ((gvDoc.FocusedRowHandle > 0) ? Function.ToString(gvDoc.GetFocusedRowCellValue(gcDocCode)) : "");
			await SearchData(code, "", Enums.NavigateType.Previous);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			string code = ((gvDoc.FocusedRowHandle > 0) ? Function.ToString(gvDoc.GetFocusedRowCellValue(gcDocCode)) : "");
			await SearchData(code, "", Enums.NavigateType.Last);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			string code = ((gvDoc.FocusedRowHandle > 0) ? Function.ToString(gvDoc.GetFocusedRowCellValue(gcDocCode)) : "");
			await SearchData(code, "", Enums.NavigateType.Next);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			string code = ((gvDoc.FocusedRowHandle > 0) ? Function.ToString(gvDoc.GetFocusedRowCellValue(gcDocCode)) : "");
			await SearchData(code, "", Enums.NavigateType.First);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			txtCode.EditValue = "";
			txtStatus.EditValue = "";
			await SearchData("", "", Enums.NavigateType.Current);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadStatus()
	{
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "A",
					Name = "Active"
				},
				new Common.Table_Code
				{
					Code = "I",
					Name = "Inactive"
				}
			};
			List<Common.Table_Code> dataSource2 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "",
					Name = ""
				},
				new Common.Table_Code
				{
					Code = "A",
					Name = "Active"
				},
				new Common.Table_Code
				{
					Code = "I",
					Name = "Inactive"
				}
			};
			txtStatus.Properties.DataSource = dataSource2;
			rDocStatus.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataBranch()
	{
		try
		{
			DataTable dataSource = await HttpClient.Commons.GetBranchList(GlobalData.UserName);
			rDocBranch.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadViewType()
	{
		try
		{
			List<Common.Table_Code> list = new List<Common.Table_Code>();
			int num = DateTime.Now.Year + 100;
			for (int i = 2000; i < num; i++)
			{
				list.Add(new Common.Table_Code
				{
					Code = Function.ToString(i),
					Name = Function.ToString(i)
				});
			}
			txtYear.Properties.DataSource = list;
			txtYear.EditValue = Function.ToString(DateTime.Now.Year);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SearchData(string code, string status, Enums.NavigateType type)
	{
		try
		{
			DataTable dataTable = await HttpClient.ResourceWorkScheduleClient.SearchData(code, status, type);
			grdDoc.DataSource = dataTable;
			grdDoc.Refresh();
			if (!dataTable.IsNotNull())
			{
				grdLine.DataSourceTable.Rows.Clear();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
	}

	private async Task GetDataLine(string code, int year)
	{
		try
		{
			DataTable dataTable = await HttpClient.ResourceWorkScheduleClient.GetDataLine(code, year);
			if (dataTable != null && dataTable.Columns.Count > 0 && dataTable.Rows.Count == 0)
			{
				DateTime dateTime = new DateTime(year, 1, 1);
				while (dateTime.Year == year)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["Code"] = code;
					dataRow["WorkDate"] = dateTime;
					dataTable.Rows.Add(dataRow);
					dateTime = dateTime.AddDays(1.0);
				}
				dataTable.AcceptChanges();
			}
			grdLine.DataSource = dataTable;
			grdLine.Refresh();
			DataRow dataRow2 = grdLine.DataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseDateTimes(t[gcLineWorkDate.FieldName]) == DateTime.Now.Date);
			if (dataRow2 != null)
			{
				int rowHandle = grdLine.DataSourceTable.AsEnumerable().IndexOf(dataRow2);
				int visibleIndex = gvLine.GetVisibleIndex(rowHandle);
				if (visibleIndex >= 0)
				{
					gvLine.FocusedRowHandle = visibleIndex;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewDoc()
	{
		try
		{
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			if (dataSourceTable == null || dataSourceTable.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.CannotGetStructureToAddNew, "");
				return;
			}
			grdDoc.BeginUpdate();
			dataSourceTable.Rows.Clear();
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["BplId"] = GlobalData.DefaultBranch;
			dataRow["Status"] = "A";
			dataRow["CreateDate"] = DateTime.Now.Date;
			dataSourceTable.Rows.Add(dataRow);
			grdDoc.DataSource = dataSourceTable;
			grdDoc.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.Add;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.RoutingAndResource.frm_ResourceWorkScheduling));
		this.splitDoc = new DevExpress.XtraEditors.SplitContainerControl();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStripDoc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcDocCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDocBranch = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocBranch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDocNotes = new Apzon.Controls.Grid.GridColumnEx();
		this.txtYear = new Apzon.Controls.Editor.LookUpEditEx();
		this.lbYear = new Apzon.Controls.Controls.LabelLine();
		this.grdLine = new Apzon.Controls.Grid.GridControlEx();
		this.gvLine = new Apzon.Controls.Grid.GridViewEx();
		this.gcLineWorkDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rDate = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gcLineShift = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineShift = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcLineNotes = new Apzon.Controls.Grid.GridColumnEx();
		this.btnSave = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbCode = new Apzon.Controls.Controls.LabelLine();
		this.lbStatus = new Apzon.Controls.Controls.LabelLine();
		this.txtCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.txtStatus = new Apzon.Controls.Editor.LookUpEditEx();
		((System.ComponentModel.ISupportInitialize)this.splitDoc).BeginInit();
		this.splitDoc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStripDoc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocBranch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtYear.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineShift).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtStatus.Properties).BeginInit();
		base.SuspendLayout();
		this.splitDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitDoc.Location = new System.Drawing.Point(1, 33);
		this.splitDoc.Name = "splitDoc";
		this.splitDoc.Panel1.Controls.Add(this.grdDoc);
		this.splitDoc.Panel1.Text = "Panel1";
		this.splitDoc.Panel2.Controls.Add(this.txtYear);
		this.splitDoc.Panel2.Controls.Add(this.lbYear);
		this.splitDoc.Panel2.Controls.Add(this.grdLine);
		this.splitDoc.Panel2.Text = "Panel2";
		this.splitDoc.Size = new System.Drawing.Size(687, 416);
		this.splitDoc.SplitterPosition = 424;
		this.splitDoc.TabIndex = 0;
		this.splitDoc.Text = "splitContainerControl1";
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStripDoc;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rDocStatus, this.rDocBranch });
		this.grdDoc.Size = new System.Drawing.Size(424, 416);
		this.grdDoc.TabIndex = 0;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStripDoc.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStripDoc.Name = "contextMenuStripDoc";
		this.contextMenuStripDoc.Size = new System.Drawing.Size(134, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvDoc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gvDoc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gcDocCode, this.gcDocName, this.gcDocStatus, this.gcDocBranch, this.gcDocNotes });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oResourceWorkScheduling";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "APZ_ORWS";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gvDoc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gvDoc.OptionsPrint.AutoWidth = false;
		this.gvDoc.OptionsView.BestFitMaxRowCount = 100;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvDoc.OptionsView.ShowGroupedColumns = true;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvDoc_FocusedRowObjectChanged);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gvDoc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gvDoc_BeforeLeaveRow);
		this.gcDocCode.Caption = "Code";
		this.gcDocCode.CurrencyDateField = null;
		this.gcDocCode.CurrencyField = null;
		this.gcDocCode.CurrencyFixed = null;
		this.gcDocCode.CurrencyFixedRate = 0.0;
		this.gcDocCode.CurrencyRateField = null;
		this.gcDocCode.FieldName = "Code";
		this.gcDocCode.Name = "gcDocCode";
		this.gcDocCode.PrimaryKey = true;
		this.gcDocCode.UnitField = null;
		this.gcDocCode.Visible = true;
		this.gcDocCode.VisibleIndex = 0;
		this.gcDocName.Caption = "Name";
		this.gcDocName.CurrencyDateField = null;
		this.gcDocName.CurrencyField = null;
		this.gcDocName.CurrencyFixed = null;
		this.gcDocName.CurrencyFixedRate = 0.0;
		this.gcDocName.CurrencyRateField = null;
		this.gcDocName.FieldName = "Name";
		this.gcDocName.Name = "gcDocName";
		this.gcDocName.PrimaryKey = false;
		this.gcDocName.UnitField = null;
		this.gcDocName.Visible = true;
		this.gcDocName.VisibleIndex = 1;
		this.gcDocStatus.Caption = "Status";
		this.gcDocStatus.ColumnEdit = this.rDocStatus;
		this.gcDocStatus.CurrencyDateField = null;
		this.gcDocStatus.CurrencyField = null;
		this.gcDocStatus.CurrencyFixed = null;
		this.gcDocStatus.CurrencyFixedRate = 0.0;
		this.gcDocStatus.CurrencyRateField = null;
		this.gcDocStatus.FieldName = "Status";
		this.gcDocStatus.Name = "gcDocStatus";
		this.gcDocStatus.PrimaryKey = false;
		this.gcDocStatus.UnitField = null;
		this.gcDocStatus.Visible = true;
		this.gcDocStatus.VisibleIndex = 2;
		this.rDocStatus.AutoHeight = false;
		this.rDocStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code")
		});
		this.rDocStatus.DisplayMember = "Name";
		this.rDocStatus.Name = "rDocStatus";
		this.rDocStatus.NullText = "";
		this.rDocStatus.ShowHeader = false;
		this.rDocStatus.ValueMember = "Code";
		this.gcDocBranch.Caption = "Branch";
		this.gcDocBranch.ColumnEdit = this.rDocBranch;
		this.gcDocBranch.CurrencyDateField = null;
		this.gcDocBranch.CurrencyField = null;
		this.gcDocBranch.CurrencyFixed = null;
		this.gcDocBranch.CurrencyFixedRate = 0.0;
		this.gcDocBranch.CurrencyRateField = null;
		this.gcDocBranch.FieldName = "BplId";
		this.gcDocBranch.Name = "gcDocBranch";
		this.gcDocBranch.PrimaryKey = false;
		this.gcDocBranch.UnitField = null;
		this.gcDocBranch.Visible = true;
		this.gcDocBranch.VisibleIndex = 3;
		this.rDocBranch.AutoHeight = false;
		this.rDocBranch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocBranch.Name = "rDocBranch";
		this.rDocBranch.NullText = "";
		this.gcDocNotes.Caption = "Notes";
		this.gcDocNotes.CurrencyDateField = null;
		this.gcDocNotes.CurrencyField = null;
		this.gcDocNotes.CurrencyFixed = null;
		this.gcDocNotes.CurrencyFixedRate = 0.0;
		this.gcDocNotes.CurrencyRateField = null;
		this.gcDocNotes.FieldName = "Notes";
		this.gcDocNotes.Name = "gcDocNotes";
		this.gcDocNotes.PrimaryKey = false;
		this.gcDocNotes.UnitField = null;
		this.gcDocNotes.Visible = true;
		this.gcDocNotes.VisibleIndex = 4;
		this.txtYear.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtYear.EditControl = null;
		this.txtYear.FieldName = "Year";
		this.txtYear.IsSearchControl = false;
		this.txtYear.Location = new System.Drawing.Point(156, 3);
		this.txtYear.Name = "txtYear";
		this.txtYear.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtYear.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code")
		});
		this.txtYear.Properties.DisplayMember = "Name";
		this.txtYear.Properties.NullText = "";
		this.txtYear.Properties.ShowHeader = false;
		this.txtYear.Properties.ValueMember = "Code";
		this.txtYear.SettingInVisible = false;
		this.txtYear.SettingReadonly = false;
		this.txtYear.Size = new System.Drawing.Size(95, 20);
		this.txtYear.TabIndex = 3;
		this.txtYear.EditValueChanged += new System.EventHandler(this.txtYear_EditValueChanged);
		this.lbYear.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbYear.EditControl = null;
		this.lbYear.EditValue = "Year";
		this.lbYear.FieldName = "lbYear";
		this.lbYear.HideWhenEmpty = false;
		this.lbYear.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbYear.Location = new System.Drawing.Point(100, 6);
		this.lbYear.Name = "lbYear";
		this.lbYear.ReadOnly = false;
		this.lbYear.SettingInVisible = false;
		this.lbYear.SettingReadonly = false;
		this.lbYear.ShowUnderLine = true;
		this.lbYear.Size = new System.Drawing.Size(54, 20);
		this.lbYear.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbYear.TabIndex = 1;
		this.lbYear.Utext = "Year";
		this.grdLine.AllowPasteExcel = true;
		this.grdLine.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdLine.ClipboardData = null;
		this.grdLine.Location = new System.Drawing.Point(0, 28);
		this.grdLine.MainView = this.gvLine;
		this.grdLine.Name = "grdLine";
		this.grdLine.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rDate, this.rLineShift });
		this.grdLine.Size = new System.Drawing.Size(258, 388);
		this.grdLine.TabIndex = 0;
		this.grdLine.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvLine });
		this.gvLine.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gvLine.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvLine.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvLine.ColumnKey = null;
		this.gvLine.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gcLineWorkDate, this.gcLineShift, this.gcLineNotes });
		this.gvLine.DisableFormSetting = false;
		this.gvLine.DisplayException = false;
		this.gvLine.EditColumn = null;
		this.gvLine.FieldCurrency = null;
		this.gvLine.FieldRate = null;
		this.gvLine.FormId = "oResourceWorkScheduling";
		this.gvLine.GridControl = this.grdLine;
		this.gvLine.IgnoreFilter = false;
		this.gvLine.IndexEditRow = -1;
		this.gvLine.IndicatorWidth = 35;
		this.gvLine.IsApzTable = false;
		this.gvLine.IsInsertNewRow = false;
		this.gvLine.IsLoadingData = false;
		this.gvLine.IsMappingContextAction = false;
		this.gvLine.ItemId = "APZ_RWS1";
		this.gvLine.LstColumnAllowBatch = null;
		this.gvLine.LstColumnNotAllowBatch = null;
		this.gvLine.LstColumnNotEditable = null;
		this.gvLine.LstColumNotSetting = null;
		this.gvLine.Name = "gvLine";
		this.gvLine.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gvLine.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gvLine.OptionsPrint.AutoWidth = false;
		this.gvLine.OptionsSelection.MultiSelect = true;
		this.gvLine.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.gvLine.OptionsView.BestFitMaxRowCount = 100;
		this.gvLine.OptionsView.ColumnAutoWidth = false;
		this.gvLine.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvLine.OptionsView.ShowGroupedColumns = true;
		this.gvLine.OptionsView.ShowGroupPanel = false;
		this.gvLine.UseBatchMod = false;
		this.gvLine.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvLine_ShowingEditor);
		this.gvLine.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvLine_CellValueChanged);
		this.gvLine.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gvLine_MouseDown);
		this.gcLineWorkDate.Caption = "Work Date";
		this.gcLineWorkDate.ColumnEdit = this.rDate;
		this.gcLineWorkDate.CurrencyDateField = null;
		this.gcLineWorkDate.CurrencyField = null;
		this.gcLineWorkDate.CurrencyFixed = null;
		this.gcLineWorkDate.CurrencyFixedRate = 0.0;
		this.gcLineWorkDate.CurrencyRateField = null;
		this.gcLineWorkDate.FieldName = "WorkDate";
		this.gcLineWorkDate.Name = "gcLineWorkDate";
		this.gcLineWorkDate.PrimaryKey = true;
		this.gcLineWorkDate.UnitField = null;
		this.gcLineWorkDate.Visible = true;
		this.gcLineWorkDate.VisibleIndex = 0;
		this.gcLineWorkDate.Width = 100;
		this.rDate.AutoHeight = false;
		this.rDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.rDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rDate.EditFormat.FormatString = "dd/MM/yyyy";
		this.rDate.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rDate.IsSearchControl = false;
		this.rDate.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.rDate.Name = "rDate";
		this.gcLineShift.Caption = "Shift";
		this.gcLineShift.ColumnEdit = this.rLineShift;
		this.gcLineShift.CurrencyDateField = null;
		this.gcLineShift.CurrencyField = null;
		this.gcLineShift.CurrencyFixed = null;
		this.gcLineShift.CurrencyFixedRate = 0.0;
		this.gcLineShift.CurrencyRateField = null;
		this.gcLineShift.FieldName = "Shift";
		this.gcLineShift.Name = "gcLineShift";
		this.gcLineShift.PrimaryKey = false;
		this.gcLineShift.UnitField = null;
		this.gcLineShift.Visible = true;
		this.gcLineShift.VisibleIndex = 1;
		this.rLineShift.AutoHeight = false;
		this.rLineShift.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rLineShift.Name = "rLineShift";
		this.rLineShift.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rLineShift_ButtonClick);
		this.gcLineNotes.Caption = "Notes";
		this.gcLineNotes.CurrencyDateField = null;
		this.gcLineNotes.CurrencyField = null;
		this.gcLineNotes.CurrencyFixed = null;
		this.gcLineNotes.CurrencyFixedRate = 0.0;
		this.gcLineNotes.CurrencyRateField = null;
		this.gcLineNotes.FieldName = "Remark";
		this.gcLineNotes.Name = "gcLineNotes";
		this.gcLineNotes.PrimaryKey = false;
		this.gcLineNotes.UnitField = null;
		this.gcLineNotes.Visible = true;
		this.gcLineNotes.VisibleIndex = 2;
		this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnSave.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btnSave.EditControl = null;
		this.btnSave.EditValue = "Save";
		this.btnSave.FieldName = "btnSave";
		this.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		this.btnSave.IsSearchControl = false;
		this.btnSave.Location = new System.Drawing.Point(527, 455);
		this.btnSave.Name = "btnSave";
		this.btnSave.ReadOnly = false;
		this.btnSave.SettingInVisible = false;
		this.btnSave.SettingReadonly = false;
		this.btnSave.Size = new System.Drawing.Size(75, 20);
		this.btnSave.TabIndex = 4;
		this.btnSave.Text = "Save";
		this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = "btnCancel";
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(608, 455);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 20);
		this.btnCancel.TabIndex = 2;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.Cancel_Click);
		this.btnSearch.EditControl = null;
		this.btnSearch.EditValue = "Search";
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = false;
		this.btnSearch.Location = new System.Drawing.Point(431, 7);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.ReadOnly = false;
		this.btnSearch.SettingInVisible = false;
		this.btnSearch.SettingReadonly = false;
		this.btnSearch.Size = new System.Drawing.Size(75, 20);
		this.btnSearch.TabIndex = 3;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.lbCode.EditControl = null;
		this.lbCode.EditValue = "Code";
		this.lbCode.FieldName = "lbCode";
		this.lbCode.HideWhenEmpty = false;
		this.lbCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbCode.Location = new System.Drawing.Point(13, 10);
		this.lbCode.Name = "lbCode";
		this.lbCode.ReadOnly = false;
		this.lbCode.SettingInVisible = false;
		this.lbCode.SettingReadonly = false;
		this.lbCode.ShowUnderLine = true;
		this.lbCode.Size = new System.Drawing.Size(80, 20);
		this.lbCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbCode.TabIndex = 0;
		this.lbCode.Utext = "Code";
		this.lbStatus.EditControl = null;
		this.lbStatus.EditValue = "Status";
		this.lbStatus.FieldName = "lbStatus";
		this.lbStatus.HideWhenEmpty = false;
		this.lbStatus.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbStatus.Location = new System.Drawing.Point(220, 10);
		this.lbStatus.Name = "lbStatus";
		this.lbStatus.ReadOnly = false;
		this.lbStatus.SettingInVisible = false;
		this.lbStatus.SettingReadonly = false;
		this.lbStatus.ShowUnderLine = true;
		this.lbStatus.Size = new System.Drawing.Size(80, 20);
		this.lbStatus.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbStatus.TabIndex = 0;
		this.lbStatus.Utext = "Status";
		this.txtCode.EditControl = null;
		this.txtCode.FieldName = "Code";
		this.txtCode.IsSearchControl = false;
		this.txtCode.Location = new System.Drawing.Point(94, 7);
		this.txtCode.Name = "txtCode";
		this.txtCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txtCode.SettingInVisible = false;
		this.txtCode.SettingReadonly = false;
		this.txtCode.Size = new System.Drawing.Size(120, 20);
		this.txtCode.TabIndex = 1;
		this.txtCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtCode_ButtonClick);
		this.txtStatus.EditControl = null;
		this.txtStatus.FieldName = "Status";
		this.txtStatus.IsSearchControl = false;
		this.txtStatus.Location = new System.Drawing.Point(301, 7);
		this.txtStatus.Name = "txtStatus";
		this.txtStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtStatus.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code")
		});
		this.txtStatus.Properties.DisplayMember = "Name";
		this.txtStatus.Properties.NullText = "";
		this.txtStatus.Properties.ShowHeader = false;
		this.txtStatus.Properties.ValueMember = "Code";
		this.txtStatus.SettingInVisible = false;
		this.txtStatus.SettingReadonly = false;
		this.txtStatus.Size = new System.Drawing.Size(120, 20);
		this.txtStatus.TabIndex = 2;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(689, 478);
		base.Controls.Add(this.txtStatus);
		base.Controls.Add(this.txtCode);
		base.Controls.Add(this.lbStatus);
		base.Controls.Add(this.lbCode);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.splitDoc);
		base.FormId = "oResourceWorkScheduling";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ResourceWorkScheduling";
		this.Text = "Resource Work Scheduling";
		base.Load += new System.EventHandler(this.frm_ResourceWorkScheduling_Load);
		((System.ComponentModel.ISupportInitialize)this.splitDoc).EndInit();
		this.splitDoc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStripDoc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocBranch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtYear.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineShift).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtStatus.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
