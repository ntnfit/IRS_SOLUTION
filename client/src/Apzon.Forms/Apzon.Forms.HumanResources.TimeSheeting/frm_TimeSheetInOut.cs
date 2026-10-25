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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_TimeSheetInOut : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _loadingData;

	public int BaseDoc;

	public int Branch;

	public string Period;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gc_Period;

	private RepositoryItemButtonEditCFL rep_Period;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gridColumnEx9;

	private RepositoryItemLookUpEdit rep_Branch;

	private GridColumnEx gridColumnEx12;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit2;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private GridControlEx grd_Line;

	private GridViewEx gv_Line;

	private GridColumnEx gc_empNo;

	private GridColumnEx gridColumnEx2;

	private RepositoryItemLookUpEdit rep_Status;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx14;

	private ContextMenuStrip ctx_Doc;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private RepositoryItemDateEditEx repositoryItemDateEditEx1;

	private GridColumnEx gridColumnEx13;

	private GridColumnEx gridColumnEx15;

	private RepositoryItemButtonEditCFL rep_BaseDoc;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx3;

	private RepositoryItemDateEditEx repositoryItemDateEditEx2;

	private GridColumnEx gridColumnEx5;

	private RepositoryItemHourEdit repositoryItemHourEdit1;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx16;

	private GridColumnEx gridColumnEx17;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private GridColumnEx gridColumnEx18;

	private GridColumnEx gridColumnEx19;

	private GridColumnEx gridColumnEx20;

	private SimpleButton btn_CopyTo;

	private ToolStripMenuItem removeToolStripMenuItem;

	private SplitContainerControl splitContainerControl2;

	private GridControlEx grd_OverTime;

	private GridViewEx gv_OverTime;

	private GridColumnEx gridColumnEx21;

	private GridColumnEx gridColumnEx22;

	private GridColumnEx gridColumnEx23;

	private GridColumnEx gridColumnEx24;

	private RepositoryItemDateEditEx repositoryItemDateEditEx3;

	private RepositoryItemHourEdit repositoryItemHourEdit2;

	private RepositoryItemCalcEdit repositoryItemCalcEdit2;

	private GridColumnEx gridColumnEx25;

	private GridColumnEx gridColumnEx26;

	private GridColumnEx gc_ScheID;

	private RepositoryItemLookUpEdit rScheID;

	private GridColumnEx gc_ShiftID;

	private RepositoryItemLookUpEdit rShiftID;

	private GridColumnEx gc_LinePeriod;

	private RepositoryItemLookUpEdit rLinePeriod;

	private GridColumnEx gc_WorkCode;

	private SimpleButton btn_CopyTo_OT;

	private GridColumnEx gc_dept;

	private RepositoryItemLookUpEdit rDepartment;

	private RepositoryItemLookUpEdit rep_User;

	private RepositoryItemButtonEditCFL rep_WorkCode;

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
			{
				SimpleButtonEx simpleButtonEx4 = btn_Save;
				bool enabled = (btnCancel.Enabled = true);
				simpleButtonEx4.Enabled = enabled;
				btn_Save.Text = Resources.ButtonAdd;
				btn_CopyTo.Enabled = false;
				btn_CopyTo_OT.Enabled = false;
				break;
			}
			case Enums.FormViewModeType.Update:
			{
				SimpleButtonEx simpleButtonEx3 = btn_Save;
				bool enabled = (btnCancel.Enabled = true);
				simpleButtonEx3.Enabled = enabled;
				btn_Save.Text = Resources.ButtonUpdate;
				btn_CopyTo.Enabled = false;
				btn_CopyTo_OT.Enabled = false;
				break;
			}
			case Enums.FormViewModeType.View:
			{
				SimpleButtonEx simpleButtonEx2 = btn_Save;
				bool enabled = (btnCancel.Enabled = true);
				simpleButtonEx2.Enabled = enabled;
				btn_Save.Text = Resources.ButtonOK;
				btn_CopyTo.Enabled = true;
				btn_CopyTo_OT.Enabled = true;
				break;
			}
			case Enums.FormViewModeType.Busy:
			{
				SimpleButtonEx simpleButtonEx = btn_Save;
				bool enabled = (btnCancel.Enabled = false);
				simpleButtonEx.Enabled = enabled;
				btn_CopyTo.Enabled = false;
				btn_CopyTo_OT.Enabled = false;
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_TimeSheetInOut()
	{
		InitializeComponent();
		FormId = "oTimeSheetInOut";
		OnLoadAsync += OnOnLoadAsync;
		gv_Doc.FormId = FormId;
		gv_Doc.ItemId = "APZ_OIOD";
		gv_Line.FormId = FormId;
		gv_Line.ItemId = "APZ_IOD1";
		gv_OverTime.FormId = FormId;
		gv_OverTime.ItemId = "APZ_IOD2";
		FormObjectType = ApzObjectTypes.oTimeSheetInOut;
	}

	private async Task OnOnLoadAsync(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			await LoadInitForm();
			await LoadData();
			Dictionary<string, GridViewEx> tblFormular = new Dictionary<string, GridViewEx>
			{
				["APZ_OIOD"] = gv_Doc,
				["APZ_IOD1"] = gv_Line,
				["APZ_IOD2"] = gv_OverTime
			};
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_Doc, "APZ_OIOD", tblFormular), WinFuntions.LoadUdfColumns(gv_Line, "APZ_IOD1", tblFormular), WinFuntions.LoadUdfColumns(gv_OverTime, "APZ_IOD2", tblFormular));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadData()
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			_loadingData = true;
			if (Function.ParseInt(BaseDoc) != 0)
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OIOD");
				DataRow dataRow = tableStructure.NewRow();
				dataRow["Branch"] = Branch;
				dataRow["Period"] = Period;
				dataRow["status"] = "D";
				tableStructure.Rows.Add(dataRow);
				grd_Doc.DataSource = tableStructure;
				grd_Doc.RefreshDataSource();
				_loadingData = false;
				gv_Doc.SetFocusedRowCellValue("BaseDoc", BaseDoc);
				grd_Doc.DataSourceTable.AcceptChanges();
				_loadingData = true;
			}
			else
			{
				DataTable tableStructure = await _httpClient.TimeSheetingClient.LoadInOutList();
				if (tableStructure.Rows.Count == 0)
				{
					await REFRESH();
				}
				else
				{
					FormMode = Enums.FormViewModeType.View;
					grd_Doc.DataSource = tableStructure;
					grd_Doc.RefreshDataSource();
				}
			}
			_loadingData = false;
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
			_loadingData = false;
		}
	}

	private void SetDataSourceByList(RepositoryItemLookUpEdit luk, List<Common.Table_Code> list)
	{
		try
		{
			luk.DataSource = list;
			luk.DisplayMember = "Name";
			luk.ValueMember = "Code";
			luk.BestFitMode = BestFitMode.BestFitResizePopup;
			luk.ForceInitialize();
			luk.PopulateColumns();
			luk.Columns["Code"].Visible = false;
			luk.DropDownRows = list.Count;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadInitForm()
	{
		_ = 1;
		try
		{
			List<Common.Table_Code> list = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "D",
					Name = "Draft"
				},
				new Common.Table_Code
				{
					Code = "O",
					Name = "Open"
				},
				new Common.Table_Code
				{
					Code = "C",
					Name = "Closed"
				}
			};
			SetDataSourceByList(rep_Status, list);
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			DataRow row = dataTable.NewRow();
			dataTable.Rows.InsertAt(row, 0);
			dataTable.AcceptChanges();
			rep_Branch.DataSource = dataTable.Copy();
			rep_Branch.DisplayMember = "Name";
			rep_Branch.ValueMember = "Code";
			rep_Branch.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Branch.ForceInitialize();
			rep_Branch.PopulateColumns();
			rep_Branch.Columns["Code"].Visible = false;
			rep_Period.DisplayMember = "PeriodCode";
			rep_Period.ValueMember = "PeriodCode";
			rep_Period.Buttons[1].Visible = false;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rep_Period;
			repositoryItemButtonEditCFL.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL cfl) =>
			{
				cfl.StatusPeriod = "U";
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = rep_Period;
			repositoryItemButtonEditCFL2.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL2.HandleChooseItem, (Action<DataRow>)((DataRow dataRow) =>
			{
				gv_Doc.SetFocusedRowCellValue("Period", Function.ToString(dataRow["PeriodCode"]));
				if (Function.ParseInt(dataRow["BranchCode"]) != Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Branch")))
				{
					if (grd_Line.DataSource is DataTable dataTable2)
					{
						dataTable2.Rows.Clear();
					}
					if (grd_OverTime.DataSource is DataTable dataTable3)
					{
						dataTable3.Rows.Clear();
					}
				}
				gv_Doc.SetFocusedRowCellValue("Branch", Function.ParseInt(dataRow["BranchCode"]));
				gv_Doc.SetFocusedRowCellValue("BaseDoc", DBNull.Value);
				gv_Doc.PostEditor();
				gv_Doc.RefreshData();
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL3 = rep_BaseDoc;
			repositoryItemButtonEditCFL3.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL3.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm.Branch = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Branch"));
				frm.period = Function.ToString(gv_Doc.GetFocusedRowCellValue("Period"));
			}));
			rep_BaseDoc.DisplayMember = "AbsEntry";
			rep_BaseDoc.ValueMember = "AbsEntry";
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL4 = rep_BaseDoc;
			repositoryItemButtonEditCFL4.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL4.HandleChooseItem, (Action<DataRow>)((DataRow dataRow) =>
			{
				_loadingData = true;
				gv_Doc.SetFocusedRowCellValue("Branch", dataRow["Branch"]);
				gv_Doc.SetFocusedRowCellValue("Period", dataRow["Period"]);
				_loadingData = false;
				gv_Doc.SetFocusedRowCellValue("BaseDoc", dataRow["AbsEntry"]);
			}));
			rep_User.ValueMember = "USERID";
			rep_User.DisplayMember = "U_NAME";
			rep_User.BestFitMode = BestFitMode.BestFitResizePopup;
			DataSetResult dataSetResult = await HttpClient.TimeSheetingClient.GetDataSetting();
			if (string.IsNullOrEmpty(dataSetResult.Message))
			{
				rLinePeriod.DataSource = dataSetResult.Data.Tables[2];
				rScheID.DataSource = dataSetResult.Data.Tables[0];
				rShiftID.DataSource = dataSetResult.Data.Tables[3];
				rep_WorkCode.DataSource = dataSetResult.Data.Tables[1];
				rDepartment.DataSource = dataSetResult.Data.Tables[4];
				rep_User.DataSource = dataSetResult.Data.Tables[5];
			}
			rep_WorkCode.MultiSelectMode = true;
			rep_WorkCode.SeparateCharacterForMulti = ',';
			rep_WorkCode.Click += Rep_WorkCode_Click;
			rep_WorkCode.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Rep_WorkCode_Click(object sender, EventArgs e)
	{
		try
		{
			if (!Function.ToString(gv_Doc.GetFocusedRowCellValue("status")).Equals("O"))
			{
				return;
			}
			string textToSearch = Function.ToString(gv_Line.GetFocusedRowCellValue("WorkCode"));
			frm_CFL frm = new frm_CFL(Enums.TypeCFL.WorkCodeAll, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
			DataSet dataSet = new DataSet();
			DataTable table = rep_WorkCode.DataSource.Copy();
			dataSet.Tables.Add(table);
			frm.DataSource = dataSet;
			frm.ButtonEditMultiSelect = true;
			frm_CFL frm_CFL2 = frm;
			frm_CFL2.AfterLoadForm = (Action)Delegate.Combine(frm_CFL2.AfterLoadForm, (Action)(() =>
			{
				frm.gv_CFL.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
				string[] array = textToSearch.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				GridColumn column = frm.gv_CFL.Columns["Code"];
				frm.gv_CFL.ClearSelection();
				frm.gv_CFL.OptionsBehavior.ImmediateUpdateRowPosition = false;
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					int rowHandle = frm.gv_CFL.LocateByDisplayText(0, column, text2.Trim());
					frm.gv_CFL.SelectRow(rowHandle);
				}
			}));
			frm.ShowDialog(this);
			string text = "";
			if (frm.ChooseItems != null && frm.ChooseItems.Count > 0)
			{
				foreach (DataRow chooseItem in frm.ChooseItems)
				{
					text = text + "," + Function.ToString(chooseItem["Code"]);
				}
				text = ((!string.IsNullOrEmpty(text) && text.StartsWith(",")) ? text.Substring(1, text.Length - 1) : "");
			}
			gv_Line.SetFocusedRowCellValue("WorkCode", text);
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SaveData()
	{
		_ = 1;
		try
		{
			DataTable dataSourceTable = grd_Line.DataSourceTable;
			DataTable dataSave = GetDataSave(dataSourceTable, "APZ_IOD1");
			DataTable dataSourceTable2 = grd_OverTime.DataSourceTable;
			DataTable dataSave2 = GetDataSave(dataSourceTable2, "APZ_IOD2");
			DataRow dataRow = gv_Doc.GetDataRow(gv_Doc.FocusedRowHandle);
			DataSet dataSet = new DataSet();
			if (FormMode == Enums.FormViewModeType.Add)
			{
				dataRow["CreateDate"] = DateTime.Today.ToString("yyyy-MM-dd");
				dataRow["UserSign"] = GlobalData.UserID;
				dataRow["status"] = "O";
			}
			else
			{
				dataRow["UserSign2"] = GlobalData.UserID;
				dataRow["UpdateDate"] = DateTime.Today.ToString("yyyy-MM-dd");
			}
			DataTable dataTable = grd_Doc.DataSourceTable.Clone();
			dataTable.ImportRow(dataRow);
			dataTable.AcceptChanges();
			dataSet.Tables.Add(dataTable.Copy());
			dataSet.Tables.Add(dataSave.Copy());
			dataSet.Tables.Add(dataSave2.Copy());
			int currentEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			WinCommons._OpenProcessing();
			HttpResult httpResult = await _httpClient.TimeSheetingClient.SaveTimeSheetInOut(dataSet);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				FormMode = Enums.FormViewModeType.View;
				BaseDoc = 0;
				await REFRESH_DATA();
				int num = gv_Doc.LocateByValue("AbsEntry", currentEntry);
				if (num != int.MinValue)
				{
					gv_Doc.FocusedRowHandle = num;
					gv_Doc.MakeRowVisible(num);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private DataTable GetDataSave(DataTable dtOt, string tableName)
	{
		DataTable tableStructure = WinFuntions.GetTableStructure(tableName);
		for (int i = 0; i < dtOt.Rows.Count; i++)
		{
			DataRow dataRow = tableStructure.NewRow();
			for (int j = 0; j < tableStructure.Columns.Count; j++)
			{
				DataColumn dataColumn = tableStructure.Columns[j];
				if (dtOt.Columns.Contains(dataColumn.ColumnName))
				{
					dataRow[dataColumn.ColumnName] = dtOt.Rows[i][dataColumn.ColumnName];
				}
			}
			tableStructure.Rows.Add(dataRow);
		}
		return tableStructure;
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				btn_Save.Enabled = false;
				btnCancel.Enabled = false;
				await SaveData();
				if (Function.ParseInt(BaseDoc) != 0)
				{
					BaseDoc = 0;
				}
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await REFRESH();
	}

	public async Task REFRESH()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OIOD");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["status"] = "D";
			dataRow["UserSign"] = GlobalData.UserID;
			tableStructure.Rows.Add(dataRow);
			grd_Doc.DataSource = tableStructure;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			int absEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			string status = Function.ToString(gv_Doc.GetFocusedRowCellValue("status"));
			WinFuntions.GetTableStructure("APZ_IOD1");
			WinFuntions.GetTableStructure("APZ_IOD2");
			new DataSet();
			if (Function.ParseInt(BaseDoc) != 0)
			{
				return;
			}
			DataSet dataSet = await Task.Run(() => _httpClient.TimeSheetingClient.LoadInOutByAbsEntry(absEntry));
			DataTable dataSource = dataSet.Tables[0];
			DataTable dataSource2 = dataSet.Tables[1];
			grd_Line.DataSource = dataSource;
			grd_Line.RefreshDataSource();
			grd_OverTime.DataSource = dataSource2;
			grd_OverTime.RefreshDataSource();
			if (status == "O")
			{
				if (Function.ParseInt(dataSet.Tables[2].Rows[0][0]) == 0)
				{
					btn_CopyTo.Enabled = true;
				}
				else
				{
					btn_CopyTo.Enabled = false;
				}
				if (grd_OverTime.DataSourceTable.Rows.Count == 0)
				{
					btn_CopyTo_OT.Enabled = false;
				}
				else if (Function.ParseInt(dataSet.Tables[2].Rows[0][1]) == 0)
				{
					btn_CopyTo_OT.Enabled = true;
				}
				else
				{
					btn_CopyTo_OT.Enabled = false;
				}
			}
			else
			{
				btn_CopyTo.Enabled = false;
				btn_CopyTo_OT.Enabled = false;
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private async Task gv_Doc_CellValueChangedAsync(CellValueChangedEventArgs e)
	{
		try
		{
			if (_loadingData)
			{
				return;
			}
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			if (e.Column.FieldName == "BaseDoc")
			{
				WinCommons._OpenProcessing();
				if (Function.ParseInt(e.Value) == 0)
				{
					grd_Line.DataSourceTable.Clear();
					grd_Line.RefreshDataSource();
					grd_OverTime.DataSourceTable.Clear();
					grd_OverTime.RefreshDataSource();
				}
				else
				{
					DataSet dataSet = await Task.Run(() => _httpClient.TimeSheetingClient.LoadInOutByBaseDoc(Function.ParseInt(e.Value)));
					grd_Line.DataSource = dataSet.Tables[0];
					grd_Line.RefreshDataSource();
					grd_OverTime.DataSource = dataSet.Tables[1];
					grd_OverTime.RefreshDataSource();
				}
				WinCommons._CloseProcessing();
			}
			else if (e.Column.FieldName == "status" && e.Value == "D")
			{
				WinCommons.Error(Enums.MessageCode.ExceptionMessage, "Cannot update status to Draft");
				gv_Doc.SetRowCellValue(e.RowHandle, e.Column, "O");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private void btn_CopyTo_Click(object sender, EventArgs e)
	{
		try
		{
			frm_TimeSheeting frm_TimeSheeting2 = new frm_TimeSheeting();
			frm_TimeSheeting2.BaseEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			frm_TimeSheeting2.MdiParent = MdiParent;
			frm_TimeSheeting2.Show();
			frm_TimeSheeting2.FormClosed += FrmTimeSheet_FormClosed;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void FrmTimeSheet_FormClosed(object sender, FormClosedEventArgs e)
	{
		int focusedRowHandle = gv_Doc.FocusedRowHandle;
		REFRESH_DATA();
		gv_Doc.MoveBy(focusedRowHandle);
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (_loadingData || FormMode != Enums.FormViewModeType.Update)
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

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue("status")) != "C")
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					return;
				}
				int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
				if (num > 0)
				{
					string text = await _httpClient.TimeSheetingClient.RemoveTimeSheetInOut(num);
					if (string.IsNullOrEmpty(text))
					{
						WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
						await LoadData();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text);
					}
				}
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.CannotDeleteCloseRecord, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("status"));
			if (text == "C")
			{
				e.Cancel = true;
			}
			else if (text == "D")
			{
				if (gv_Doc.FocusedColumn.FieldName != "Period" && gv_Doc.FocusedColumn.FieldName != "BaseDoc" && gv_Doc.FocusedColumn.FieldName != "Remarks")
				{
					e.Cancel = true;
				}
			}
			else if (gv_Doc.FocusedColumn.FieldName != "Period" && gv_Doc.FocusedColumn.FieldName != "BaseDoc" && gv_Doc.FocusedColumn.FieldName != "Remarks" && gv_Doc.FocusedColumn.FieldName != "status")
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "status")
			{
				if (Function.ToString(e.CellValue) == "O")
				{
					e.Appearance.ForeColor = Color.Green;
				}
				else if (Function.ToString(e.CellValue) == "C")
				{
					e.Appearance.ForeColor = Color.Brown;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Line_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (!(e.Column.FieldName == "OutTime") && !(e.Column.FieldName == "OutWorkTime") && !(e.Column.FieldName == "InTime") && !(e.Column.FieldName == "InWorkTime"))
			{
				return;
			}
			if (Function.ToString(e.CellValue) == "")
			{
				e.Appearance.BackColor = Color.LightPink;
				return;
			}
			if (e.Column.FieldName == "OutWorkTime" && Function.ParseInt(gv_Line.GetRowCellValue(e.RowHandle, "Early")) > 0)
			{
				e.Appearance.BackColor = Color.LightCyan;
			}
			if (e.Column.FieldName == "InWorkTime" && Function.ParseInt(gv_Line.GetRowCellValue(e.RowHandle, "Late")) > 0)
			{
				e.Appearance.BackColor = Color.LightCyan;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_CopyTo_OT_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = await _httpClient.TimeSheetingClient.GetOvertimeTypeByBaseDoc(Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry")));
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				frm_TimeSheetingHour frm_TimeSheetingHour2 = new frm_TimeSheetingHour();
				frm_TimeSheetingHour2.BaseEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
				frm_TimeSheetingHour2.overtimeType = Function.ToString(dataTable.Rows[i][0]);
				frm_TimeSheetingHour2.MdiParent = MdiParent;
				frm_TimeSheetingHour2.Show();
				frm_TimeSheetingHour2.FormClosed += FrmTimeSheetingOT_FormClosed;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void FrmTimeSheetingOT_FormClosed(object sender, FormClosedEventArgs e)
	{
		int focusedRowHandle = gv_Doc.FocusedRowHandle;
		REFRESH_DATA();
		gv_Doc.MoveBy(focusedRowHandle);
	}

	private async Task gv_Line_CellValueChangedAsync(CellValueChangedEventArgs e)
	{
		_ = 1;
		try
		{
			if (e.Column.FieldName == "InTime")
			{
				int inTime = Function.ParseInt(e.Value);
				int outTime = Function.ParseInt(gv_Line.GetFocusedRowCellValue("OutTime"));
				string period = Function.ToString(gv_Doc.GetFocusedRowCellValue("Period"));
				int empId = Function.ParseInt(gv_Line.GetFocusedRowCellValue("empID"));
				string workDate = Function.ParseDateTimes(gv_Line.GetFocusedRowCellValue("WorkDay")).Date.ToString("yyyy-MM-dd");
				DataSet dataSet = await _httpClient.TimeSheetingClient.LoadWorkCodeForEmpByDate(inTime, outTime, period, empId, workDate);
				if (dataSet.IsNotNull())
				{
					gv_Line.SetFocusedRowCellValue("InWorkTime", dataSet.Tables[0].Rows[0]["InWorkTime"]);
					gv_Line.SetFocusedRowCellValue("OutWorkTime", dataSet.Tables[0].Rows[0]["OutWorkTime"]);
					gv_Line.SetFocusedRowCellValue("SchID", dataSet.Tables[0].Rows[0]["SchID"]);
					gv_Line.SetFocusedRowCellValue("ShiftID", dataSet.Tables[0].Rows[0]["ShiftID"]);
					gv_Line.SetFocusedRowCellValue("WorkCode", dataSet.Tables[0].Rows[0]["WorkCode"]);
					gv_Line.SetFocusedRowCellValue("WorkTotal", dataSet.Tables[0].Rows[0]["WorkTotal"]);
					gv_Line.SetFocusedRowCellValue("Late", dataSet.Tables[0].Rows[0]["Late"]);
					gv_Line.SetFocusedRowCellValue("Early", dataSet.Tables[0].Rows[0]["Early"]);
					gv_Line.SetFocusedRowCellValue("BeforeOT", dataSet.Tables[0].Rows[0]["BeforeOT"]);
					gv_Line.SetFocusedRowCellValue("AfterOT", dataSet.Tables[0].Rows[0]["AfterOT"]);
					gv_Line.SetFocusedRowCellValue("DHour", dataSet.Tables[0].Rows[0]["DHour"]);
					gv_Line.SetFocusedRowCellValue("DMinutes", dataSet.Tables[0].Rows[0]["DMinutes"]);
					gv_Line.SetFocusedRowCellValue("DWorkHour", dataSet.Tables[0].Rows[0]["DWorkHour"]);
					gv_Line.SetFocusedRowCellValue("DWorkMinutes", dataSet.Tables[0].Rows[0]["DWorkMinutes"]);
					DataTable dataSourceTable = grd_OverTime.DataSourceTable;
					foreach (DataRow item in (from t in dataSourceTable.AsEnumerable()
						where Function.ParseInt(t["empID"]) == empId && Function.ParseDateTimes(t["WorkDay"]) == Function.ParseDateTimes(gv_Line.GetFocusedRowCellValue("WorkDay"))
						select t).ToList())
					{
						dataSourceTable.Rows.Remove(item);
					}
					for (int num = 0; num < dataSet.Tables[1].Rows.Count; num++)
					{
						dataSourceTable.Rows.Add(dataSet.Tables[1].Rows[num].ItemArray);
					}
				}
			}
			else if (e.Column.FieldName == "OutTime")
			{
				int inTime2 = Function.ParseInt(gv_Line.GetFocusedRowCellValue("InTime"));
				int outTime2 = Function.ParseInt(e.Value);
				string period2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("Period"));
				int empId2 = Function.ParseInt(gv_Line.GetFocusedRowCellValue("empID"));
				string workDate2 = Function.ParseDateTimes(gv_Line.GetFocusedRowCellValue("WorkDay")).Date.ToString("yyyy-MM-dd");
				DataSet dataSet2 = await _httpClient.TimeSheetingClient.LoadWorkCodeForEmpByDate(inTime2, outTime2, period2, empId2, workDate2);
				if (dataSet2.IsNotNull())
				{
					gv_Line.SetFocusedRowCellValue("InWorkTime", dataSet2.Tables[0].Rows[0]["InWorkTime"]);
					gv_Line.SetFocusedRowCellValue("OutWorkTime", dataSet2.Tables[0].Rows[0]["OutWorkTime"]);
					gv_Line.SetFocusedRowCellValue("SchID", dataSet2.Tables[0].Rows[0]["SchID"]);
					gv_Line.SetFocusedRowCellValue("ShiftID", dataSet2.Tables[0].Rows[0]["ShiftID"]);
					gv_Line.SetFocusedRowCellValue("WorkCode", dataSet2.Tables[0].Rows[0]["WorkCode"]);
					gv_Line.SetFocusedRowCellValue("WorkTotal", dataSet2.Tables[0].Rows[0]["WorkTotal"]);
					gv_Line.SetFocusedRowCellValue("Late", dataSet2.Tables[0].Rows[0]["Late"]);
					gv_Line.SetFocusedRowCellValue("Early", dataSet2.Tables[0].Rows[0]["Early"]);
					gv_Line.SetFocusedRowCellValue("BeforeOT", dataSet2.Tables[0].Rows[0]["BeforeOT"]);
					gv_Line.SetFocusedRowCellValue("AfterOT", dataSet2.Tables[0].Rows[0]["AfterOT"]);
					gv_Line.SetFocusedRowCellValue("DHour", dataSet2.Tables[0].Rows[0]["DHour"]);
					gv_Line.SetFocusedRowCellValue("DMinutes", dataSet2.Tables[0].Rows[0]["DMinutes"]);
					gv_Line.SetFocusedRowCellValue("DWorkHour", dataSet2.Tables[0].Rows[0]["DWorkHour"]);
					gv_Line.SetFocusedRowCellValue("DWorkMinutes", dataSet2.Tables[0].Rows[0]["DWorkMinutes"]);
					DataTable dataSourceTable2 = grd_OverTime.DataSourceTable;
					foreach (DataRow item2 in (from t in dataSourceTable2.AsEnumerable()
						where Function.ParseInt(t["empID"]) == empId2 && Function.ParseDateTimes(t["WorkDay"]) == Function.ParseDateTimes(gv_Line.GetFocusedRowCellValue("WorkDay"))
						select t).ToList())
					{
						dataSourceTable2.Rows.Remove(item2);
					}
					for (int num2 = 0; num2 < dataSet2.Tables[1].Rows.Count; num2++)
					{
						dataSourceTable2.Rows.Add(dataSet2.Tables[1].Rows[num2].ItemArray);
					}
				}
			}
			if (FormMode == Enums.FormViewModeType.View)
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_TimeSheetInOut));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Period = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Period = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_BaseDoc = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx1 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_User = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_empNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx2 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemHourEdit1 = new Apzon.Controls.Controls.RepositoryItemHourEdit();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ScheID = new Apzon.Controls.Grid.GridColumnEx();
		this.rScheID = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_ShiftID = new Apzon.Controls.Grid.GridColumnEx();
		this.rShiftID = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_LinePeriod = new Apzon.Controls.Grid.GridColumnEx();
		this.rLinePeriod = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_WorkCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_WorkCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_dept = new Apzon.Controls.Grid.GridColumnEx();
		this.rDepartment = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.grd_OverTime = new Apzon.Controls.Grid.GridControlEx();
		this.gv_OverTime = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx24 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx3 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx26 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemHourEdit2 = new Apzon.Controls.Controls.RepositoryItemHourEdit();
		this.repositoryItemCalcEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_CopyTo = new DevExpress.XtraEditors.SimpleButton();
		this.btn_CopyTo_OT = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctx_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Period).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BaseDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_User).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rScheID).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rShiftID).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLinePeriod).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WorkCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDepartment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_OverTime).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_OverTime).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx3.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 4);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.splitContainerControl2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1046, 467);
		this.splitContainerControl1.SplitterPosition = 132;
		this.splitContainerControl1.TabIndex = 91;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctx_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.rep_Branch, this.repositoryItemMemoExEdit2, this.rep_Period, this.repositoryItemCheckEdit1, this.rep_Status, this.repositoryItemDateEditEx1, this.rep_BaseDoc, this.rep_User });
		this.grd_Doc.Size = new System.Drawing.Size(1046, 132);
		this.grd_Doc.TabIndex = 86;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctx_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.removeToolStripMenuItem });
		this.ctx_Doc.Name = "ctx_Doc";
		this.ctx_Doc.Size = new System.Drawing.Size(124, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.gridColumnEx7, this.gc_Period, this.gridColumnEx15, this.gridColumnEx10, this.gridColumnEx11, this.gridColumnEx9, this.gridColumnEx12, this.gridColumnEx2, this.gridColumnEx4, this.gridColumnEx14 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oBiometricData";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.HasFormSetting = false;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 30;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_OIOD";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.LstOldUDF = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.RowAutoHeight = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CellValueChangedAsync += new Apzon.Controls.Grid.CellValueChangedAsync(this.gv_Doc_CellValueChangedAsync);
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gridColumnEx7.Caption = "Doc No.";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "DocNum";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.OptionsColumn.ReadOnly = true;
		this.gridColumnEx7.PrimaryKey = true;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Width = 157;
		this.gc_Period.Caption = "Period";
		this.gc_Period.ColumnEdit = this.rep_Period;
		this.gc_Period.CurrencyDateField = null;
		this.gc_Period.CurrencyField = null;
		this.gc_Period.CurrencyFixed = null;
		this.gc_Period.CurrencyFixedRate = 0.0;
		this.gc_Period.CurrencyRateField = null;
		this.gc_Period.FieldName = "Period";
		this.gc_Period.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_Period.Name = "gc_Period";
		this.gc_Period.OptionsColumn.ReadOnly = true;
		this.gc_Period.PrimaryKey = false;
		this.gc_Period.UnitField = null;
		this.gc_Period.Visible = true;
		this.gc_Period.VisibleIndex = 0;
		this.gc_Period.Width = 138;
		this.rep_Period.AutoHeight = false;
		this.rep_Period.AutoLoadDataSource = true;
		this.rep_Period.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Period.ContainerControl = this;
		this.rep_Period.DisplayMember = "";
		this.rep_Period.DisplayMember2 = "";
		this.rep_Period.LoadDataSourceWhenEnter = false;
		this.rep_Period.Name = "rep_Period";
		this.rep_Period.ReadOnly = true;
		this.rep_Period.SeparateCharacterForMulti = ';';
		this.rep_Period.SeparateString = ", ";
		this.rep_Period.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.rep_Period.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.WorkPeriod;
		this.rep_Period.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oWorkPeriod;
		this.rep_Period.UsingAutoCompleteBox = false;
		this.rep_Period.UsingDataSource = true;
		this.rep_Period.ValidateWhenNoDataSource = false;
		this.rep_Period.ValueMember = "";
		this.gridColumnEx15.Caption = "Base Doc";
		this.gridColumnEx15.ColumnEdit = this.rep_BaseDoc;
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "BaseDoc";
		this.gridColumnEx15.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx15.Visible = true;
		this.gridColumnEx15.VisibleIndex = 2;
		this.rep_BaseDoc.AutoHeight = false;
		this.rep_BaseDoc.AutoLoadDataSource = true;
		this.rep_BaseDoc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_BaseDoc.ContainerControl = this;
		this.rep_BaseDoc.DisplayMember = "";
		this.rep_BaseDoc.DisplayMember2 = "";
		this.rep_BaseDoc.LoadDataSourceWhenEnter = false;
		this.rep_BaseDoc.Name = "rep_BaseDoc";
		this.rep_BaseDoc.SeparateCharacterForMulti = ';';
		this.rep_BaseDoc.SeparateString = ", ";
		this.rep_BaseDoc.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.rep_BaseDoc.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.BioDataList;
		this.rep_BaseDoc.UsingAutoCompleteBox = false;
		this.rep_BaseDoc.UsingDataSource = true;
		this.rep_BaseDoc.ValidateWhenNoDataSource = false;
		this.rep_BaseDoc.ValueMember = "";
		this.gridColumnEx10.Caption = "From Date";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "FromDate";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.OptionsColumn.ReadOnly = true;
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx11.Caption = "To Date";
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "ToDate";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.OptionsColumn.ReadOnly = true;
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx9.Caption = "Branch";
		this.gridColumnEx9.ColumnEdit = this.rep_Branch;
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "Branch";
		this.gridColumnEx9.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.OptionsColumn.ReadOnly = true;
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 1;
		this.gridColumnEx9.Width = 139;
		this.rep_Branch.AutoHeight = false;
		this.rep_Branch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, false, false, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Branch.Name = "rep_Branch";
		this.rep_Branch.NullText = "";
		this.gridColumnEx12.Caption = "Remark";
		this.gridColumnEx12.ColumnEdit = this.repositoryItemMemoExEdit2;
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "Remarks";
		this.gridColumnEx12.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Visible = true;
		this.gridColumnEx12.VisibleIndex = 5;
		this.gridColumnEx12.Width = 293;
		this.repositoryItemMemoExEdit2.AutoHeight = false;
		this.repositoryItemMemoExEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit2.Name = "repositoryItemMemoExEdit2";
		this.repositoryItemMemoExEdit2.ShowIcon = false;
		this.gridColumnEx2.Caption = "Status";
		this.gridColumnEx2.ColumnEdit = this.rep_Status;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "status";
		this.gridColumnEx2.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 4;
		this.gridColumnEx2.Width = 99;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status.Name = "rep_Status";
		this.rep_Status.NullText = "";
		this.gridColumnEx4.Caption = "Create Date";
		this.gridColumnEx4.ColumnEdit = this.repositoryItemDateEditEx1;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "CreateDate";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.gridColumnEx4.Width = 159;
		this.repositoryItemDateEditEx1.AutoHeight = false;
		this.repositoryItemDateEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.IsSearchControl = false;
		this.repositoryItemDateEditEx1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx1.Name = "repositoryItemDateEditEx1";
		this.repositoryItemDateEditEx1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.gridColumnEx14.Caption = "User Created";
		this.gridColumnEx14.ColumnEdit = this.rep_User;
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "UserSign";
		this.gridColumnEx14.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx14.Visible = true;
		this.gridColumnEx14.VisibleIndex = 6;
		this.gridColumnEx14.Width = 186;
		this.rep_User.AutoHeight = false;
		this.rep_User.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, false, false, false, imageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_User.Name = "rep_User";
		this.rep_User.NullText = "";
		this.rep_User.ReadOnly = true;
		this.rep_User.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.grd_Line);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.grd_OverTime);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(1046, 325);
		this.splitContainerControl2.SplitterPosition = 215;
		this.splitContainerControl2.TabIndex = 84;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.repositoryItemDateEditEx2, this.repositoryItemHourEdit1, this.repositoryItemCalcEdit1, this.rScheID, this.rShiftID, this.rLinePeriod, this.rDepartment, this.rep_WorkCode });
		this.grd_Line.Size = new System.Drawing.Size(1046, 215);
		this.grd_Line.TabIndex = 83;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = "EmpId";
		this.gv_Line.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[17]
		{
			this.gridColumnEx13, this.gc_empNo, this.gridColumnEx1, this.gridColumnEx3, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx8, this.gridColumnEx16, this.gridColumnEx17, this.gridColumnEx18,
			this.gridColumnEx19, this.gridColumnEx20, this.gc_ScheID, this.gc_ShiftID, this.gc_LinePeriod, this.gc_WorkCode, this.gc_dept
		});
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oBiometricData";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.HasFormSetting = false;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 30;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = "APZ_IOD1";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.LstOldUDF = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_Line.OptionsView.RowAutoHeight = true;
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = false;
		this.gv_Line.CellValueChangedAsync += new Apzon.Controls.Grid.CellValueChangedAsync(this.gv_Line_CellValueChangedAsync);
		this.gv_Line.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Line_RowCellStyle);
		this.gridColumnEx13.Caption = "AbsEntry";
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "AbsEntry";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = true;
		this.gridColumnEx13.UnitField = null;
		this.gc_empNo.Caption = "Employee No.";
		this.gc_empNo.CurrencyDateField = null;
		this.gc_empNo.CurrencyField = null;
		this.gc_empNo.CurrencyFixed = null;
		this.gc_empNo.CurrencyFixedRate = 0.0;
		this.gc_empNo.CurrencyRateField = null;
		this.gc_empNo.FieldName = "empID";
		this.gc_empNo.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_empNo.Name = "gc_empNo";
		this.gc_empNo.PrimaryKey = true;
		this.gc_empNo.UnitField = null;
		this.gc_empNo.Visible = true;
		this.gc_empNo.VisibleIndex = 0;
		this.gc_empNo.Width = 136;
		this.gridColumnEx1.Caption = "Line Num";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "LineNum";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 1;
		this.gridColumnEx3.Caption = "Work Day";
		this.gridColumnEx3.ColumnEdit = this.repositoryItemDateEditEx2;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "WorkDay";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.repositoryItemDateEditEx2.AutoHeight = false;
		this.repositoryItemDateEditEx2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx2.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx2.IsSearchControl = false;
		this.repositoryItemDateEditEx2.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx2.Name = "repositoryItemDateEditEx2";
		this.gridColumnEx5.Caption = "In Time";
		this.gridColumnEx5.ColumnEdit = this.repositoryItemHourEdit1;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "InTime";
		this.gridColumnEx5.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 3;
		this.repositoryItemHourEdit1.AutoHeight = false;
		this.repositoryItemHourEdit1.IsSearchControl = false;
		this.repositoryItemHourEdit1.MaxLength = 5;
		this.repositoryItemHourEdit1.Name = "repositoryItemHourEdit1";
		this.gridColumnEx6.Caption = "Out Time";
		this.gridColumnEx6.ColumnEdit = this.repositoryItemHourEdit1;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "OutTime";
		this.gridColumnEx6.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 4;
		this.gridColumnEx8.Caption = "In Work Time";
		this.gridColumnEx8.ColumnEdit = this.repositoryItemHourEdit1;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "InWorkTime";
		this.gridColumnEx8.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 5;
		this.gridColumnEx16.Caption = "Out Work Time";
		this.gridColumnEx16.ColumnEdit = this.repositoryItemHourEdit1;
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "OutWorkTime";
		this.gridColumnEx16.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx16.Visible = true;
		this.gridColumnEx16.VisibleIndex = 6;
		this.gridColumnEx17.Caption = "Dif Hours";
		this.gridColumnEx17.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.FieldName = "DHour";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx17.Visible = true;
		this.gridColumnEx17.VisibleIndex = 7;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.gridColumnEx18.Caption = "Dif Minutes";
		this.gridColumnEx18.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = null;
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = null;
		this.gridColumnEx18.FieldName = "DMinutes";
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.PrimaryKey = false;
		this.gridColumnEx18.UnitField = null;
		this.gridColumnEx18.Visible = true;
		this.gridColumnEx18.VisibleIndex = 8;
		this.gridColumnEx19.Caption = "Rate";
		this.gridColumnEx19.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx19.CurrencyDateField = null;
		this.gridColumnEx19.CurrencyField = null;
		this.gridColumnEx19.CurrencyFixed = null;
		this.gridColumnEx19.CurrencyFixedRate = 0.0;
		this.gridColumnEx19.CurrencyRateField = null;
		this.gridColumnEx19.FieldName = "WorkTotal";
		this.gridColumnEx19.Name = "gridColumnEx19";
		this.gridColumnEx19.PrimaryKey = false;
		this.gridColumnEx19.UnitField = null;
		this.gridColumnEx19.Visible = true;
		this.gridColumnEx19.VisibleIndex = 9;
		this.gridColumnEx20.Caption = "Note";
		this.gridColumnEx20.CurrencyDateField = null;
		this.gridColumnEx20.CurrencyField = null;
		this.gridColumnEx20.CurrencyFixed = null;
		this.gridColumnEx20.CurrencyFixedRate = 0.0;
		this.gridColumnEx20.CurrencyRateField = null;
		this.gridColumnEx20.FieldName = "FreeTxt";
		this.gridColumnEx20.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx20.Name = "gridColumnEx20";
		this.gridColumnEx20.PrimaryKey = false;
		this.gridColumnEx20.UnitField = null;
		this.gridColumnEx20.Visible = true;
		this.gridColumnEx20.VisibleIndex = 10;
		this.gc_ScheID.Caption = "ScheID";
		this.gc_ScheID.ColumnEdit = this.rScheID;
		this.gc_ScheID.CurrencyDateField = null;
		this.gc_ScheID.CurrencyField = null;
		this.gc_ScheID.CurrencyFixed = null;
		this.gc_ScheID.CurrencyFixedRate = 0.0;
		this.gc_ScheID.CurrencyRateField = null;
		this.gc_ScheID.FieldName = "SchID";
		this.gc_ScheID.Name = "gc_ScheID";
		this.gc_ScheID.PrimaryKey = false;
		this.gc_ScheID.UnitField = null;
		this.gc_ScheID.Visible = true;
		this.gc_ScheID.VisibleIndex = 11;
		this.rScheID.AutoHeight = false;
		this.rScheID.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rScheID.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SchID", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SchName", "Name")
		});
		this.rScheID.DisplayMember = "SchName";
		this.rScheID.Name = "rScheID";
		this.rScheID.NullText = "";
		this.rScheID.ShowHeader = false;
		this.rScheID.ValueMember = "SchID";
		this.gc_ShiftID.Caption = "ShiftID";
		this.gc_ShiftID.ColumnEdit = this.rShiftID;
		this.gc_ShiftID.CurrencyDateField = null;
		this.gc_ShiftID.CurrencyField = null;
		this.gc_ShiftID.CurrencyFixed = null;
		this.gc_ShiftID.CurrencyFixedRate = 0.0;
		this.gc_ShiftID.CurrencyRateField = null;
		this.gc_ShiftID.FieldName = "ShiftID";
		this.gc_ShiftID.Name = "gc_ShiftID";
		this.gc_ShiftID.PrimaryKey = false;
		this.gc_ShiftID.UnitField = null;
		this.gc_ShiftID.Visible = true;
		this.gc_ShiftID.VisibleIndex = 12;
		this.rShiftID.AutoHeight = false;
		this.rShiftID.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rShiftID.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ShiftID", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ShiftCode", "Name")
		});
		this.rShiftID.DisplayMember = "ShiftCode";
		this.rShiftID.Name = "rShiftID";
		this.rShiftID.NullText = "";
		this.rShiftID.ShowHeader = false;
		this.rShiftID.ValueMember = "ShiftID";
		this.gc_LinePeriod.Caption = "Period";
		this.gc_LinePeriod.ColumnEdit = this.rLinePeriod;
		this.gc_LinePeriod.CurrencyDateField = null;
		this.gc_LinePeriod.CurrencyField = null;
		this.gc_LinePeriod.CurrencyFixed = null;
		this.gc_LinePeriod.CurrencyFixedRate = 0.0;
		this.gc_LinePeriod.CurrencyRateField = null;
		this.gc_LinePeriod.FieldName = "Period";
		this.gc_LinePeriod.Name = "gc_LinePeriod";
		this.gc_LinePeriod.PrimaryKey = false;
		this.gc_LinePeriod.UnitField = null;
		this.gc_LinePeriod.Visible = true;
		this.gc_LinePeriod.VisibleIndex = 13;
		this.rLinePeriod.AutoHeight = false;
		this.rLinePeriod.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLinePeriod.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("PeriodCode", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("PeriodName", "Name")
		});
		this.rLinePeriod.DisplayMember = "PeriodName";
		this.rLinePeriod.Name = "rLinePeriod";
		this.rLinePeriod.NullText = "";
		this.rLinePeriod.ShowHeader = false;
		this.rLinePeriod.ValueMember = "PeriodCode";
		this.gc_WorkCode.Caption = "WorkCode";
		this.gc_WorkCode.ColumnEdit = this.rep_WorkCode;
		this.gc_WorkCode.CurrencyDateField = null;
		this.gc_WorkCode.CurrencyField = null;
		this.gc_WorkCode.CurrencyFixed = null;
		this.gc_WorkCode.CurrencyFixedRate = 0.0;
		this.gc_WorkCode.CurrencyRateField = null;
		this.gc_WorkCode.FieldName = "WorkCode";
		this.gc_WorkCode.Name = "gc_WorkCode";
		this.gc_WorkCode.PrimaryKey = false;
		this.gc_WorkCode.UnitField = null;
		this.gc_WorkCode.Visible = true;
		this.gc_WorkCode.VisibleIndex = 14;
		this.rep_WorkCode.AutoHeight = false;
		this.rep_WorkCode.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		this.rep_WorkCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_WorkCode.ContainerControl = this;
		this.rep_WorkCode.DisplayMember = "Name";
		this.rep_WorkCode.DisplayMember2 = "";
		this.rep_WorkCode.LoadDataSourceWhenEnter = false;
		this.rep_WorkCode.Name = "rep_WorkCode";
		this.rep_WorkCode.SeparateCharacterForMulti = ';';
		this.rep_WorkCode.SeparateString = ", ";
		this.rep_WorkCode.UsingAutoCompleteBox = false;
		this.rep_WorkCode.UsingDataSource = true;
		this.rep_WorkCode.ValidateWhenNoDataSource = false;
		this.rep_WorkCode.ValueMember = "Code";
		this.gc_dept.Caption = "dept";
		this.gc_dept.ColumnEdit = this.rDepartment;
		this.gc_dept.CurrencyDateField = null;
		this.gc_dept.CurrencyField = null;
		this.gc_dept.CurrencyFixed = null;
		this.gc_dept.CurrencyFixedRate = 0.0;
		this.gc_dept.CurrencyRateField = null;
		this.gc_dept.FieldName = "dept";
		this.gc_dept.MinWidth = 150;
		this.gc_dept.Name = "gc_dept";
		this.gc_dept.PrimaryKey = false;
		this.gc_dept.UnitField = null;
		this.gc_dept.Visible = true;
		this.gc_dept.VisibleIndex = 15;
		this.gc_dept.Width = 150;
		this.rDepartment.AutoHeight = false;
		this.rDepartment.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDepartment.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rDepartment.DisplayMember = "Name";
		this.rDepartment.Name = "rDepartment";
		this.rDepartment.NullText = "";
		this.rDepartment.ShowHeader = false;
		this.rDepartment.ValueMember = "Code";
		this.grd_OverTime.AllowPasteExcel = true;
		this.grd_OverTime.ClipboardData = null;
		this.grd_OverTime.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_OverTime.Location = new System.Drawing.Point(0, 0);
		this.grd_OverTime.MainView = this.gv_OverTime;
		this.grd_OverTime.Name = "grd_OverTime";
		this.grd_OverTime.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemDateEditEx3, this.repositoryItemHourEdit2, this.repositoryItemCalcEdit2 });
		this.grd_OverTime.Size = new System.Drawing.Size(1046, 100);
		this.grd_OverTime.TabIndex = 84;
		this.grd_OverTime.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_OverTime });
		this.gv_OverTime.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_OverTime.ColumnKey = "EmpId";
		this.gv_OverTime.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx21, this.gridColumnEx22, this.gridColumnEx23, this.gridColumnEx24, this.gridColumnEx25, this.gridColumnEx26 });
		this.gv_OverTime.DisableFormSetting = false;
		this.gv_OverTime.DisplayException = false;
		this.gv_OverTime.EditColumn = null;
		this.gv_OverTime.FieldCurrency = null;
		this.gv_OverTime.FieldRate = null;
		this.gv_OverTime.FormId = "oBiometricData";
		this.gv_OverTime.GridControl = this.grd_OverTime;
		this.gv_OverTime.HasFormSetting = false;
		this.gv_OverTime.IgnoreFilter = false;
		this.gv_OverTime.IndexEditRow = -1;
		this.gv_OverTime.IndicatorWidth = 30;
		this.gv_OverTime.IsApzTable = false;
		this.gv_OverTime.IsInsertNewRow = false;
		this.gv_OverTime.IsLoadingData = false;
		this.gv_OverTime.IsMappingContextAction = false;
		this.gv_OverTime.ItemId = "APZ_IOD2";
		this.gv_OverTime.LstColumnAllowBatch = null;
		this.gv_OverTime.LstColumnNotAllowBatch = null;
		this.gv_OverTime.LstColumnNotEditable = null;
		this.gv_OverTime.LstColumNotSetting = null;
		this.gv_OverTime.LstOldUDF = null;
		this.gv_OverTime.Name = "gv_OverTime";
		this.gv_OverTime.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_OverTime.OptionsBehavior.ReadOnly = true;
		this.gv_OverTime.OptionsView.RowAutoHeight = true;
		this.gv_OverTime.OptionsView.ShowGroupPanel = false;
		this.gv_OverTime.UseBatchMod = false;
		this.gridColumnEx21.Caption = "AbsEntry";
		this.gridColumnEx21.CurrencyDateField = null;
		this.gridColumnEx21.CurrencyField = null;
		this.gridColumnEx21.CurrencyFixed = null;
		this.gridColumnEx21.CurrencyFixedRate = 0.0;
		this.gridColumnEx21.CurrencyRateField = null;
		this.gridColumnEx21.FieldName = "AbsEntry";
		this.gridColumnEx21.Name = "gridColumnEx21";
		this.gridColumnEx21.PrimaryKey = true;
		this.gridColumnEx21.UnitField = null;
		this.gridColumnEx22.Caption = "Employee No.";
		this.gridColumnEx22.CurrencyDateField = null;
		this.gridColumnEx22.CurrencyField = null;
		this.gridColumnEx22.CurrencyFixed = null;
		this.gridColumnEx22.CurrencyFixedRate = 0.0;
		this.gridColumnEx22.CurrencyRateField = null;
		this.gridColumnEx22.FieldName = "empID";
		this.gridColumnEx22.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx22.Name = "gridColumnEx22";
		this.gridColumnEx22.PrimaryKey = true;
		this.gridColumnEx22.UnitField = null;
		this.gridColumnEx22.Visible = true;
		this.gridColumnEx22.VisibleIndex = 0;
		this.gridColumnEx22.Width = 136;
		this.gridColumnEx23.Caption = "Line Num";
		this.gridColumnEx23.CurrencyDateField = null;
		this.gridColumnEx23.CurrencyField = null;
		this.gridColumnEx23.CurrencyFixed = null;
		this.gridColumnEx23.CurrencyFixedRate = 0.0;
		this.gridColumnEx23.CurrencyRateField = null;
		this.gridColumnEx23.FieldName = "LineNum";
		this.gridColumnEx23.Name = "gridColumnEx23";
		this.gridColumnEx23.PrimaryKey = false;
		this.gridColumnEx23.UnitField = null;
		this.gridColumnEx23.Visible = true;
		this.gridColumnEx23.VisibleIndex = 1;
		this.gridColumnEx24.Caption = "Work Day";
		this.gridColumnEx24.ColumnEdit = this.repositoryItemDateEditEx3;
		this.gridColumnEx24.CurrencyDateField = null;
		this.gridColumnEx24.CurrencyField = null;
		this.gridColumnEx24.CurrencyFixed = null;
		this.gridColumnEx24.CurrencyFixedRate = 0.0;
		this.gridColumnEx24.CurrencyRateField = null;
		this.gridColumnEx24.FieldName = "WorkDay";
		this.gridColumnEx24.Name = "gridColumnEx24";
		this.gridColumnEx24.PrimaryKey = false;
		this.gridColumnEx24.UnitField = null;
		this.gridColumnEx24.Visible = true;
		this.gridColumnEx24.VisibleIndex = 2;
		this.repositoryItemDateEditEx3.AutoHeight = false;
		this.repositoryItemDateEditEx3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx3.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx3.IsSearchControl = false;
		this.repositoryItemDateEditEx3.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx3.Name = "repositoryItemDateEditEx3";
		this.gridColumnEx25.Caption = "OT Type";
		this.gridColumnEx25.CurrencyDateField = null;
		this.gridColumnEx25.CurrencyField = null;
		this.gridColumnEx25.CurrencyFixed = null;
		this.gridColumnEx25.CurrencyFixedRate = 0.0;
		this.gridColumnEx25.CurrencyRateField = null;
		this.gridColumnEx25.FieldName = "OvertimeType";
		this.gridColumnEx25.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx25.Name = "gridColumnEx25";
		this.gridColumnEx25.PrimaryKey = false;
		this.gridColumnEx25.UnitField = null;
		this.gridColumnEx25.Visible = true;
		this.gridColumnEx25.VisibleIndex = 3;
		this.gridColumnEx26.Caption = "OT Work Total";
		this.gridColumnEx26.CurrencyDateField = null;
		this.gridColumnEx26.CurrencyField = null;
		this.gridColumnEx26.CurrencyFixed = null;
		this.gridColumnEx26.CurrencyFixedRate = 0.0;
		this.gridColumnEx26.CurrencyRateField = null;
		this.gridColumnEx26.FieldName = "OvertimeHour";
		this.gridColumnEx26.Name = "gridColumnEx26";
		this.gridColumnEx26.PrimaryKey = false;
		this.gridColumnEx26.UnitField = null;
		this.gridColumnEx26.Visible = true;
		this.gridColumnEx26.VisibleIndex = 4;
		this.repositoryItemHourEdit2.AutoHeight = false;
		this.repositoryItemHourEdit2.IsSearchControl = false;
		this.repositoryItemHourEdit2.MaxLength = 5;
		this.repositoryItemHourEdit2.Name = "repositoryItemHourEdit2";
		this.repositoryItemCalcEdit2.AutoHeight = false;
		this.repositoryItemCalcEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit2.Name = "repositoryItemCalcEdit2";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(2, 477);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 90;
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
		this.btnCancel.Location = new System.Drawing.Point(69, 477);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 89;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btn_CopyTo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_CopyTo.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CopyTo.ImageOptions.Image");
		this.btn_CopyTo.Location = new System.Drawing.Point(892, 475);
		this.btn_CopyTo.Name = "btn_CopyTo";
		this.btn_CopyTo.Size = new System.Drawing.Size(145, 23);
		this.btn_CopyTo.TabIndex = 94;
		this.btn_CopyTo.Text = "Copy To TimeSheeting";
		this.btn_CopyTo.Click += new System.EventHandler(this.btn_CopyTo_Click);
		this.btn_CopyTo_OT.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_CopyTo_OT.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CopyTo_OT.ImageOptions.Image");
		this.btn_CopyTo_OT.Location = new System.Drawing.Point(712, 475);
		this.btn_CopyTo_OT.Name = "btn_CopyTo_OT";
		this.btn_CopyTo_OT.Size = new System.Drawing.Size(175, 23);
		this.btn_CopyTo_OT.TabIndex = 95;
		this.btn_CopyTo_OT.Text = "Copy To TimeSheeting OT";
		this.btn_CopyTo_OT.Click += new System.EventHandler(this.btn_CopyTo_OT_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1049, 500);
		base.Controls.Add(this.btn_CopyTo_OT);
		base.Controls.Add(this.btn_CopyTo);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.FormId = "oBiometricData";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_TimeSheetInOut";
		this.Text = "TimeSheet In Out";
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctx_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Period).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BaseDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_User).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rScheID).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rShiftID).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLinePeriod).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WorkCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDepartment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_OverTime).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_OverTime).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx3.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).EndInit();
		base.ResumeLayout(false);
	}
}
