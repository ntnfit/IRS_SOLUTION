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
using Apzon.Forms.Helper;
using Apzon.Forms.HumanResources.Biometric;
using Apzon.Forms.HumanResources.Biometric.FP_CLOCK;
using Apzon.Forms.HumanResources.Biometric.KJTech;
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

public class frm_BiometricData : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _loadingData;

	private bool _isChanging;

	private DataTable _dtPeriod;

	private RepositoryItemButtonEditCFL rep_CFLEnroll_NoSearch = new RepositoryItemButtonEditCFL();

	private List<GridCell> modifiedCells = new List<GridCell>();

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

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx8;

	private SimpleButton btn_CopyTo;

	private SimpleButton btn_LoadFromDevice;

	private ContextMenuStrip ctx_Doc;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private RepositoryItemDateEditEx repositoryItemDateEditEx1;

	private GridColumnEx gridColumnEx13;

	private GridColumnEx gridColumnEx15;

	private RepositoryItemHourEdit repositoryItemHourEdit1;

	private RepositoryItemDateEditEx repositoryItemDateEditEx2;

	private RepositoryItemHourEdit repositoryItemHourEdit2;

	private LookUpEditEx luk_MachineNo;

	private ToolStripMenuItem removeToolStripMenuItem;

	private GridColumnEx gridColumnEx16;

	private RepositoryItemTimeEdit repositoryItemTimeEdit1;

	private RepositoryItemLookUpEdit rep_Source;

	private RepositoryItemLookUpEdit rep_OriginType;

	private ContextMenuStrip ctx_Line;

	private ToolStripMenuItem stripItem_AddLine;

	private ToolStripMenuItem stripItem_RemoveLine;

	private RepositoryItemLookUpEdit rep_MachineNo;

	private RepositoryItemButtonEditCFL rep_cfl_enrollNo;

	private RepositoryItemLookUpEdit rep_UserCode;

	private RepositoryItemDateEditEx rep_FromDate;

	private RepositoryItemDateEditEx rep_ToDate;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx17;

	private RepositoryItemDateEditEx rep_PFromDate;

	private RepositoryItemDateEditEx rep_PToDate;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
			{
				SimpleButtonEx simpleButtonEx4 = btn_Save;
				bool enabled = (btnCancel.Enabled = true);
				simpleButtonEx4.Enabled = enabled;
				btn_Save.Text = Resources.ButtonAdd;
				btn_LoadFromDevice.Enabled = true;
				btn_CopyTo.Enabled = false;
				rep_Period.TextEditStyle = TextEditStyles.Standard;
				rep_Branch.TextEditStyle = TextEditStyles.Standard;
				for (int k = 0; k < rep_Period.Buttons.Count; k++)
				{
					rep_Period.Buttons[k].Enabled = true;
				}
				rep_FromDate.ReadOnly = false;
				rep_ToDate.ReadOnly = false;
				stripItem_AddLine.Enabled = false;
				stripItem_RemoveLine.Enabled = false;
				break;
			}
			case Enums.FormViewModeType.Update:
			{
				SimpleButtonEx simpleButtonEx3 = btn_Save;
				bool enabled = (btnCancel.Enabled = true);
				simpleButtonEx3.Enabled = enabled;
				btn_Save.Text = Resources.ButtonUpdate;
				btn_LoadFromDevice.Enabled = true;
				btn_CopyTo.Enabled = false;
				rep_Period.TextEditStyle = TextEditStyles.DisableTextEditor;
				rep_Branch.TextEditStyle = TextEditStyles.DisableTextEditor;
				for (int j = 0; j < rep_Period.Buttons.Count; j++)
				{
					rep_Period.Buttons[j].Enabled = false;
				}
				rep_FromDate.ReadOnly = true;
				rep_ToDate.ReadOnly = true;
				stripItem_AddLine.Enabled = true;
				stripItem_RemoveLine.Enabled = true;
				break;
			}
			case Enums.FormViewModeType.View:
			{
				SimpleButtonEx simpleButtonEx2 = btn_Save;
				bool enabled = (btnCancel.Enabled = true);
				simpleButtonEx2.Enabled = enabled;
				btn_Save.Text = Resources.ButtonOK;
				btn_LoadFromDevice.Enabled = true;
				btn_CopyTo.Enabled = true;
				rep_Period.TextEditStyle = TextEditStyles.DisableTextEditor;
				rep_Branch.TextEditStyle = TextEditStyles.DisableTextEditor;
				for (int i = 0; i < rep_Period.Buttons.Count; i++)
				{
					rep_Period.Buttons[i].Enabled = false;
				}
				rep_FromDate.ReadOnly = true;
				rep_ToDate.ReadOnly = true;
				stripItem_AddLine.Enabled = true;
				stripItem_RemoveLine.Enabled = true;
				break;
			}
			case Enums.FormViewModeType.Busy:
			{
				SimpleButtonEx simpleButtonEx = btn_Save;
				bool enabled = (btnCancel.Enabled = false);
				simpleButtonEx.Enabled = enabled;
				btn_LoadFromDevice.Enabled = false;
				btn_CopyTo.Enabled = false;
				stripItem_AddLine.Enabled = true;
				stripItem_RemoveLine.Enabled = true;
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

	public frm_BiometricData()
	{
		InitializeComponent();
		FormId = "oBiometricData";
		OnLoadAsync += OnOnLoadAsync;
		gv_Doc.FormId = FormId;
		gv_Doc.ItemId = "APZ_OTBD";
		gv_Line.FormId = FormId;
		gv_Line.ItemId = "APZ_TBD1";
		gv_Doc.ChangeLogType = Enums.ChangeLog.clBiometricData;
		gv_Doc.IsApzTable = true;
	}

	private async Task OnOnLoadAsync(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await LoadInitForm();
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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

	public override void CHANGELOG()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update)
			{
				string primaryKey = Function.ToString(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
				Enums.ChangeLog changeLog = Enums.ChangeLog.clBiometricData;
				GetChangeLogParams getChangeLogParams = new GetChangeLogParams();
				getChangeLogParams.PrimaryKey = primaryKey;
				getChangeLogParams.Object = changeLog;
				getChangeLogParams.IsApzTable = true;
				frm_ChangeLogList frm_ChangeLogList2 = new frm_ChangeLogList();
				frm_ChangeLogList2.GetChangeLogParams = getChangeLogParams;
				frm_ChangeLogList2.MdiParent = MdiParent;
				frm_ChangeLogList2.Show();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	private async Task LoadData()
	{
		_ = 1;
		try
		{
			_loadingData = true;
			DataTable dataTable = await _httpClient.TimeSheetingClient.LoadImportList();
			if (dataTable.Rows.Count == 0)
			{
				await REFRESH();
			}
			else
			{
				FormMode = Enums.FormViewModeType.View;
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
				grd_Doc.DataSourceTable.AcceptChanges();
			}
			_loadingData = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_loadingData = false;
		}
	}

	private async Task LoadInitForm()
	{
		_ = 3;
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
			DataTable dataTable2 = await _httpClient.User.LoadData();
			DataRow row2 = dataTable2.NewRow();
			dataTable2.Rows.InsertAt(row2, 0);
			dataTable2.AcceptChanges();
			rep_UserCode.DataSource = dataTable2.Copy();
			rep_UserCode.DisplayMember = "USER_CODE";
			rep_UserCode.ValueMember = "USERID";
			rep_UserCode.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_UserCode.ForceInitialize();
			rep_UserCode.PopulateColumns();
			rep_UserCode.Columns["USERID"].Visible = false;
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
				gv_Doc.SetFocusedRowCellValue("Branch", Function.ParseInt(dataRow["BranchCode"]));
				gv_Doc.SetFocusedRowCellValue("Period", Function.ToString(dataRow["PeriodCode"]));
				if (Function.ParseInt(dataRow["BranchCode"]) != Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Branch")) && grd_Line.DataSource is DataTable dataTable3)
				{
					dataTable3.Rows.Clear();
				}
				gv_Doc.PostEditor();
				gv_Doc.RefreshData();
			}));
			_dtPeriod = await _httpClient.WorkPeriod.GetListWorkPeriod();
			List<Common.Table_Code> list2 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "0",
					Name = "FP_OR_PW_OR_RF"
				},
				new Common.Table_Code
				{
					Code = "1",
					Name = "FP"
				},
				new Common.Table_Code
				{
					Code = "2",
					Name = "PIN"
				},
				new Common.Table_Code
				{
					Code = "3",
					Name = "PW"
				},
				new Common.Table_Code
				{
					Code = "4",
					Name = "RF"
				},
				new Common.Table_Code
				{
					Code = "5",
					Name = "FP_OR_PW"
				},
				new Common.Table_Code
				{
					Code = "6",
					Name = "FP_OR_RF"
				},
				new Common.Table_Code
				{
					Code = "7",
					Name = "PW_OR_RF"
				},
				new Common.Table_Code
				{
					Code = "8",
					Name = "PIN_AND_FP"
				},
				new Common.Table_Code
				{
					Code = "9",
					Name = "FP_AND_PW"
				},
				new Common.Table_Code
				{
					Code = "10",
					Name = "FP_AND_RF"
				},
				new Common.Table_Code
				{
					Code = "11",
					Name = "PW_AND_RF"
				},
				new Common.Table_Code
				{
					Code = "12",
					Name = "FP_AND_PW_AND_RF"
				},
				new Common.Table_Code
				{
					Code = "13",
					Name = "PIN_AND_FP_AND_PW"
				},
				new Common.Table_Code
				{
					Code = "14",
					Name = "FP_AND_RF_OR_PIN"
				},
				new Common.Table_Code
				{
					Code = "99",
					Name = "Manual"
				}
			};
			LookUpEditEx lookUpEditEx = new LookUpEditEx();
			lookUpEditEx.SetDataSourceByList(list2);
			rep_Source.Assign(lookUpEditEx.Properties);
			List<Common.Table_Code> list3 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "0",
					Name = "Check In"
				},
				new Common.Table_Code
				{
					Code = "1",
					Name = "Check-Out"
				},
				new Common.Table_Code
				{
					Code = "2",
					Name = "Break-Out"
				},
				new Common.Table_Code
				{
					Code = "3",
					Name = "Break-In"
				},
				new Common.Table_Code
				{
					Code = "4",
					Name = "OT-In"
				},
				new Common.Table_Code
				{
					Code = "5",
					Name = "OT-Out"
				},
				new Common.Table_Code
				{
					Code = "255",
					Name = "Unknown"
				}
			};
			LookUpEditEx lookUpEditEx2 = new LookUpEditEx();
			lookUpEditEx2.SetDataSourceByList(list3);
			rep_OriginType.Assign(lookUpEditEx2.Properties);
			await luk_MachineNo.LoadBioMachines();
			rep_MachineNo.Assign(luk_MachineNo.Properties);
			rep_cfl_enrollNo.ContainerControl = this;
			rep_cfl_enrollNo.AutoLoadDataSource = false;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL3 = rep_cfl_enrollNo;
			repositoryItemButtonEditCFL3.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL3.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm.MachineNo = Function.ParseInt(gv_Line.GetFocusedRowCellValue("MachineNo"));
			}));
			rep_cfl_enrollNo.FindCFLWith(Enums.TypeCFL.BioMapping, true, "", Enums.ItemObjectType.NoItemType, null);
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL4 = rep_cfl_enrollNo;
			repositoryItemButtonEditCFL4.HandleChooseItemWithEditor = (Action<DataRow, ButtonEditCFL>)Delegate.Combine(repositoryItemButtonEditCFL4.HandleChooseItemWithEditor, new Action<DataRow, ButtonEditCFL>(HandleChooseItem));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL5 = rep_cfl_enrollNo;
			repositoryItemButtonEditCFL5.HandleChooseItemMultiWithEditor = (Action<List<DataRow>, ButtonEditCFL>)Delegate.Combine(repositoryItemButtonEditCFL5.HandleChooseItemMultiWithEditor, new Action<List<DataRow>, ButtonEditCFL>(HandleChooseItemMulti));
			rep_cfl_enrollNo.RefreshDataSource();
			rep_CFLEnroll_NoSearch.Assign(rep_cfl_enrollNo);
			rep_CFLEnroll_NoSearch.Buttons[0].Visible = false;
			rep_CFLEnroll_NoSearch.TextEditStyle = TextEditStyles.DisableTextEditor;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void HandleChooseItem(DataRow dataRow, ButtonEdit sender)
	{
		try
		{
			if ((sender.Parent as GridControlEx)?.FocusedView is GridViewEx gridViewEx)
			{
				if (Function.ToString(gridViewEx.GetFocusedRowCellValue("LineNum")) == "")
				{
					gridViewEx.SetFocusedRowCellValue("EnrollNo", Function.ParseInt(dataRow["EnrollNo"]));
					gridViewEx.PostEditor();
					gridViewEx.RefreshData();
				}
				else
				{
					WinCommons.Warning(Enums.MessageCode.CannotReselectEnRollNoForExistedRecord, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void HandleChooseItemMulti(List<DataRow> rows, ButtonEdit sender)
	{
		try
		{
			if (!((sender.Parent as GridControlEx)?.FocusedView is GridViewEx gridViewEx) || !(grd_Line.DataSource is DataTable dataTable) || dataTable.Rows.Count <= 0 || rows.Count <= 0)
			{
				return;
			}
			_isChanging = true;
			gridViewEx.SetFocusedRowCellValue("EnrollNo", Function.ParseInt(rows[0]["EnrollNo"]));
			gridViewEx.SetFocusedRowCellValue("EnrollName", Function.ToString(rows[0]["EnrollName"]));
			for (int i = 1; i < rows.Count; i++)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["EnrollNo"])));
				if (dataRow != null)
				{
					int dataSourceIndex = dataTable.Rows.IndexOf(dataRow);
					gridViewEx.SetRowCellValue(gridViewEx.GetRowHandle(dataSourceIndex), "EnrollNo", Function.ParseInt(rows[i]["EnrollNo"]));
					gridViewEx.SetRowCellValue(gridViewEx.GetRowHandle(dataSourceIndex), "EnrollName", Function.ToString(rows[i]["EnrollName"]));
					continue;
				}
				if (gridViewEx == gv_Line)
				{
					AddNewLine();
				}
				DataRow row = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["EnrollNo"])));
				int dataSourceIndex2 = dataTable.Rows.IndexOf(row);
				gridViewEx.SetRowCellValue(gridViewEx.GetRowHandle(dataSourceIndex2), "EnrollNo", Function.ParseInt(rows[i]["EnrollNo"]));
				gridViewEx.SetRowCellValue(gridViewEx.GetRowHandle(dataSourceIndex2), "EnrollName", Function.ToString(rows[i]["EnrollName"]));
			}
			_isChanging = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> DownLoadData(DateTime fromDate, DateTime toDate, int machineNo)
	{
		DeviceManipulator manipulator = new DeviceManipulator();
		ZkemClient objZkeeper = new ZkemClient();
		try
		{
			DataTable dt = new DataTable();
			if (toDate.TimeOfDay.Hours == 0)
			{
				TimeSpan timeSpan = new TimeSpan(23, 59, 59);
				toDate = toDate.Date + timeSpan;
			}
			string sTime = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
			string eTime = toDate.ToString("yyyy-MM-dd HH:mm:ss");
			if (!(await objZkeeper.ConnectDevice(machineNo)))
			{
				WinCommons.Error(Enums.MessageCode.CannotConnectToDevicePleaseRecheckConfig, "");
				return dt;
			}
			DataTable logDataTable = manipulator.GetLogDataTable(objZkeeper, machineNo, sTime, eTime, fromDate, toDate);
			if (!logDataTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.None, "Log data empty!");
				return dt;
			}
			EnumerableRowCollection<DataRow> source = from t in logDataTable.AsEnumerable()
				where Function.ParseDateTimes(t["ClockDate"]).Date >= fromDate.Date && Function.ParseDateTimes(t["ClockDate"]).Date <= toDate.Date
				select t;
			if (source.Any())
			{
				dt = source.CopyToDataTable();
			}
			return dt.Copy();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
		finally
		{
			objZkeeper.Disconnect();
		}
	}

	private async Task<DataTable> DownLoadDataKJtech(DateTime fromDate, DateTime toDate, int machineNo)
	{
		try
		{
			DataTable dt = new DataTable();
			if (toDate.TimeOfDay.Hours == 0)
			{
				TimeSpan timeSpan = new TimeSpan(23, 59, 59);
				toDate = toDate.Date + timeSpan;
			}
			fromDate.ToString("yyyy-MM-dd HH:mm:ss");
			toDate.ToString("yyyy-MM-dd HH:mm:ss");
			DataRow dataRow = ((DataTable)luk_MachineNo.Properties.DataSource).AsEnumerable().FirstOrDefault((DataRow myRow) => myRow.Field<int>("MachineID") == machineNo);
			int machineNumber = Function.ParseInt(dataRow["MachineID"]);
			Function.ToString(dataRow["MachineName"]);
			string ip = Function.ToString(dataRow["IPAddress"]);
			int port = Function.ParseInt(dataRow["TCPPort"]);
			DataTable dataTable = await new KJtechManipulator().GetLogDataTable(machineNumber, ip, port, fromDate, toDate);
			if (!dataTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.None, "Log data empty!");
				return dt;
			}
			EnumerableRowCollection<DataRow> source = from t in dataTable.AsEnumerable()
				where Function.ParseDateTimes(t["ClockDate"]).Date >= fromDate.Date && Function.ParseDateTimes(t["ClockDate"]).Date <= toDate.Date
				select t;
			if (source.Any())
			{
				dt = source.CopyToDataTable();
			}
			return dt.Copy();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private async Task<DataTable> DownLoadDataFp(DateTime fromDate, DateTime toDate, int machineNo)
	{
		FpClockManipulator fpClockManipulator = new FpClockManipulator();
		try
		{
			DataTable dataTable = new DataTable();
			if (toDate.TimeOfDay.Hours == 0)
			{
				TimeSpan timeSpan = new TimeSpan(23, 59, 59);
				toDate = toDate.Date + timeSpan;
			}
			fromDate.ToString("yyyy-MM-dd HH:mm:ss");
			toDate.ToString("yyyy-MM-dd HH:mm:ss");
			DataRow dataRow = (from r in ((DataTable)luk_MachineNo.Properties.DataSource).AsEnumerable()
				where Function.ParseInt(r["MachineID"]) == machineNo
				select r).FirstOrDefault();
			int port = Function.ParseInt(dataRow["TCPPort"]);
			string ip = Function.ToString(dataRow["IPAddress"]);
			if (!fpClockManipulator.SetIPAddress(ip, port))
			{
				WinCommons.Error(Enums.MessageCode.None, "Invalid IP or password");
				return dataTable;
			}
			if (!fpClockManipulator.OpenCommPort())
			{
				WinCommons.Error(Enums.MessageCode.CannotConnectToDevicePleaseRecheckConfig, "");
				return dataTable;
			}
			fpClockManipulator.DisableDevice();
			fpClockManipulator.EnableDevice();
			DataTable allGLogDataWithSecond = fpClockManipulator.GetAllGLogDataWithSecond(fromDate, toDate, machineNo);
			if (!allGLogDataWithSecond.IsNotNull())
			{
				if (!string.IsNullOrWhiteSpace(fpClockManipulator.GetErrorString()))
				{
					WinCommons.Error(Enums.MessageCode.None, fpClockManipulator.GetErrorString());
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, "Log data empty!");
				}
				return dataTable;
			}
			EnumerableRowCollection<DataRow> source = from t in allGLogDataWithSecond.AsEnumerable()
				where Function.ParseDateTimes(t["ClockDate"]).Date >= fromDate.Date && Function.ParseDateTimes(t["ClockDate"]).Date <= toDate.Date
				select t;
			if (source.Any())
			{
				dataTable = source.CopyToDataTable();
			}
			fpClockManipulator.CloseCommPort();
			return dataTable.Copy();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private async Task SaveData()
	{
		_ = 1;
		try
		{
			DataTable dataSourceTable = grd_Line.DataSourceTable;
			DataTable dataSourceTable2 = grd_Doc.DataSourceTable;
			if (!dataSourceTable2.IsNotNull() || !dataSourceTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
				return;
			}
			DataSet dataSet = new DataSet();
			DataRow dataRow = gv_Doc.GetDataRow(gv_Doc.FocusedRowHandle);
			dataRow["ImportTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
			dataRow["status"] = "O";
			dataRow["UserSign2"] = GlobalData.UserID;
			dataRow["UpdateDate"] = DateTime.Today;
			DataTable dataTable = dataSourceTable2.Clone();
			dataTable.ImportRow(dataRow);
			dataTable.AcceptChanges();
			List<string> list = new List<string>();
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_TBD1");
			for (int i = 0; i < dataSourceTable.Columns.Count; i++)
			{
				DataColumn dataColumn = dataSourceTable.Columns[i];
				if (!tableStructure.Columns.Contains(Function.ToString(dataColumn.ColumnName)))
				{
					list.Add(dataSourceTable.Columns[i].ColumnName);
				}
			}
			foreach (string item in list)
			{
				dataSourceTable.Columns.Remove(item);
			}
			dataSourceTable2.AcceptChanges();
			dataSet.Tables.Add(dataTable.Copy());
			dataSet.Tables.Add(dataSourceTable.Copy());
			int currentEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			WinCommons._OpenProcessing();
			HttpResult httpResult = await _httpClient.TimeSheetingClient.SaveBiometricData(dataSet);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				FormMode = Enums.FormViewModeType.View;
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

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				btn_Save.Enabled = false;
				btnCancel.Enabled = false;
				await SaveData();
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

	private async void btn_LoadFromDevice_Click(object sender, EventArgs e)
	{
		_ = 6;
		try
		{
			btn_LoadFromDevice.Enabled = false;
			WinCommons._OpenProcessing();
			DataTable dtGridCopy = grd_Line.DataSourceTable.Copy();
			string[] cond = new string[3] { "MachineNo", "EnrollNo", "DateTimeRecord" };
			dtGridCopy.SetPrimaryKey(cond);
			new DataTable();
			DataTable dtMachine = (DataTable)luk_MachineNo.Properties.DataSource;
			if (dtMachine.IsNotNull())
			{
				int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Branch"));
				string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("Period"));
				if (num == 0 || text == "")
				{
					WinCommons.Error(Enums.MessageCode.BranchOrPeriodIsEmpty, "");
					return;
				}
				if (!(await _httpClient.WorkPeriod.GetWorkPeriodByCode(text, num)).IsNotNull())
				{
					WinCommons.Error(Enums.MessageCode.BranchOrPeriodIsEmpty, "");
					return;
				}
				int machineNo = Function.ParseInt(luk_MachineNo.EditValue);
				if (machineNo < 0)
				{
					DateTime dfromDate = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("FromDate"));
					DateTime dtoDate = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("ToDate")).AddDays(1.0);
					if (dtoDate.TimeOfDay.Hours == 0)
					{
						TimeSpan timeSpan = new TimeSpan(23, 59, 59);
						dtoDate = dtoDate.Date + timeSpan;
					}
					foreach (DataRow row in dtMachine.Rows)
					{
						if (Function.ParseInt(row["MachineID"]) < 0)
						{
							continue;
						}
						string text2 = Function.ToString(row["DeviceCode"]);
						DataTable dataTable;
						if (text2 == "KJ")
						{
							dataTable = await DownLoadDataKJtech(dfromDate, dtoDate, Function.ParseInt(row["MachineID"]));
						}
						else
						{
							dataTable = ((!(text2 == "FP")) ? (await DownLoadData(dfromDate, dtoDate, Function.ParseInt(row["MachineID"]))) : (await DownLoadDataFp(dfromDate, dtoDate, Function.ParseInt(row["MachineID"]))));
						}
						if (!dataTable.IsNotNull())
						{
							continue;
						}
						DataTable dataTable2 = (from r in dataTable.AsEnumerable()
							group r by string.Join("||", cond.Select((string col) => r[col]?.ToString())) into @group
							select @group.First()).CopyToDataTable();
						dataTable2.SetPrimaryKey(cond);
						dtGridCopy.Merge(dataTable2);
						dtGridCopy.PrimaryKey = null;
						dtGridCopy.Columns["MachineNo"].AllowDBNull = true;
						dtGridCopy.Columns["EnrollNo"].AllowDBNull = true;
						dtGridCopy.Columns["DateTimeRecord"].AllowDBNull = true;
						dataTable.Rows.Clear();
					}
				}
				else
				{
					DateTime fromDate = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("FromDate"));
					DateTime toDate = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("ToDate")).AddDays(1.0);
					if (toDate.TimeOfDay.Hours == 0)
					{
						TimeSpan timeSpan2 = new TimeSpan(23, 59, 59);
						toDate = toDate.Date + timeSpan2;
					}
					string text2 = Function.ToString((from r in dtMachine.AsEnumerable()
						where Function.ParseInt(r["MachineID"]) == machineNo
						select r).FirstOrDefault()["DeviceCode"]);
					DataTable dataTable;
					if (text2 == "KJ")
					{
						dataTable = await DownLoadDataKJtech(fromDate, toDate, Function.ParseInt(machineNo));
					}
					else
					{
						dataTable = ((!(text2 == "FP")) ? (await DownLoadData(fromDate, toDate, Function.ParseInt(machineNo))) : (await DownLoadDataFp(fromDate, toDate, Function.ParseInt(machineNo))));
					}
					if (dataTable.IsNotNull())
					{
						DataTable dataTable3 = (from r in dataTable.AsEnumerable()
							group r by string.Join("||", cond.Select((string col) => r[col]?.ToString())) into @group
							select @group.First()).CopyToDataTable();
						dataTable3.SetPrimaryKey(cond);
						dtGridCopy.Merge(dataTable3);
						dtGridCopy.PrimaryKey = null;
						dtGridCopy.Columns["MachineNo"].AllowDBNull = true;
						dtGridCopy.Columns["EnrollNo"].AllowDBNull = true;
						dtGridCopy.Columns["DateTimeRecord"].AllowDBNull = true;
						dataTable.Rows.Clear();
					}
				}
				grd_Line.DataSource = dtGridCopy.Copy();
				string text3 = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("FromDate")).ToString("yyyy-MM-dd HH:mm:ss");
				string text4 = (Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("ToDate")) + new TimeSpan(23, 59, 59)).ToString("yyyy-MM-dd HH:mm:ss");
				grd_Line.DataSourceTable.DefaultView.RowFilter = "ClockDate >='" + text3 + "' AND ClockDate <= '" + text4 + "'";
				grd_Line.DataSourceTable.DefaultView.Sort = "EnrollNo,DateTimeRecord";
				grd_Line.DataSourceTable.AcceptChanges();
				Control_EditValueChanged(null, null);
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, "No divices found.");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			btn_LoadFromDevice.Enabled = true;
			WinCommons._CloseProcessing();
		}
	}

	public async Task REFRESH()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OTBD");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["ImportDate"] = DateTime.Today.ToString("yyyy-MM-dd");
			dataRow["status"] = "D";
			dataRow["UserImport"] = GlobalData.UserID;
			tableStructure.Rows.Add(dataRow);
			grd_Doc.DataSource = tableStructure;
			tableStructure.AcceptChanges();
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
			int absEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			switch (Function.ToString(gv_Doc.GetFocusedRowCellValue("status")))
			{
			case "D":
				btn_CopyTo.Enabled = false;
				btn_LoadFromDevice.Enabled = true;
				luk_MachineNo.Enabled = true;
				stripItem_AddLine.Enabled = true;
				stripItem_RemoveLine.Enabled = true;
				break;
			case "O":
				btn_CopyTo.Enabled = true;
				btn_LoadFromDevice.Enabled = true;
				luk_MachineNo.Enabled = true;
				stripItem_AddLine.Enabled = true;
				stripItem_RemoveLine.Enabled = true;
				break;
			case "C":
				btn_CopyTo.Enabled = false;
				btn_LoadFromDevice.Enabled = false;
				luk_MachineNo.Enabled = false;
				stripItem_AddLine.Enabled = false;
				stripItem_RemoveLine.Enabled = false;
				break;
			}
			DataTable dataSource = await Task.Run(() => _httpClient.TimeSheetingClient.LoadClockReportByEntry(absEntry));
			grd_Line.DataSource = dataSource;
			grd_Line.RefreshDataSource();
			grd_Line.DataSourceTable.AcceptChanges();
			modifiedCells.Clear();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_CopyTo_Click(object sender, EventArgs e)
	{
		try
		{
			frm_TimeSheetInOut frm_TimeSheetInOut2 = new frm_TimeSheetInOut();
			frm_TimeSheetInOut2.BaseDoc = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			frm_TimeSheetInOut2.Branch = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Branch"));
			frm_TimeSheetInOut2.Period = Function.ToString(gv_Doc.GetFocusedRowCellValue("Period"));
			frm_TimeSheetInOut2.MdiParent = MdiParent;
			frm_TimeSheetInOut2.Show();
			frm_TimeSheetInOut2.FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Doc_CellValueChangedAsync(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName.Equals("Period"))
			{
				if (!string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue("Period"))))
				{
					_dtPeriod = await _httpClient.WorkPeriod.GetWorkPeriodByCode(Function.ToString(gv_Doc.GetFocusedRowCellValue("Period")), Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Branch")));
					_isChanging = true;
					gv_Doc.SetFocusedRowCellValue("PFromDate", Function.ParseDateTime(_dtPeriod.Rows[0]["FromDate"]));
					gv_Doc.SetFocusedRowCellValue("PToDate", Function.ParseDateTime(_dtPeriod.Rows[0]["ToDate"]));
					gv_Doc.SetFocusedRowCellValue("FromDate", Function.ParseDateTime(_dtPeriod.Rows[0]["FromDate"]));
					gv_Doc.SetFocusedRowCellValue("ToDate", Function.ParseDateTime(_dtPeriod.Rows[0]["ToDate"]));
					_isChanging = false;
				}
			}
			else if (e.Column.FieldName.Equals("FromDate"))
			{
				if (_isChanging)
				{
					return;
				}
				if (string.IsNullOrWhiteSpace(Function.ToString(gv_Doc.GetFocusedRowCellValue("Period"))))
				{
					XtraMessageBoxEx.Show(Enums.MessageCode.PeriodCodeMustNotBeNull, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK);
					gv_Doc.SetFocusedRowCellValue("FromDate", "");
					Control_EditValueChanged(null, null);
					return;
				}
				if (FormMode == Enums.FormViewModeType.Add)
				{
					if (Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("FromDate")) < Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("PFromDate")) || Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("FromDate")) > Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("PToDate")))
					{
						XtraMessageBoxEx.Show(Enums.MessageCode.FromDateNotInPeriodRange, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK);
						gv_Doc.SetFocusedRowCellValue("FromDate", Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("PFromDate")));
						Control_EditValueChanged(null, null);
						return;
					}
					if (Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("FromDate")).HasValue && Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("ToDate")).HasValue && Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("ToDate")) < Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("FromDate")))
					{
						XtraMessageBoxEx.Show(Enums.MessageCode.FromDateCanNotLargerThanToDate, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK);
						gv_Doc.SetFocusedRowCellValue("FromDate", Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("PFromDate")));
						Control_EditValueChanged(null, null);
						return;
					}
				}
			}
			else if (e.Column.FieldName.Equals("ToDate"))
			{
				if (_isChanging)
				{
					return;
				}
				if (string.IsNullOrWhiteSpace(Function.ToString(gv_Doc.GetFocusedRowCellValue("Period"))))
				{
					XtraMessageBoxEx.Show(Enums.MessageCode.PeriodCodeMustNotBeNull, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK);
					gv_Doc.SetFocusedRowCellValue("ToDate", "");
					Control_EditValueChanged(null, null);
					return;
				}
				if (FormMode == Enums.FormViewModeType.Add)
				{
					if (Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("ToDate")) < Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("PFromDate")) || Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("ToDate")) > Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("PToDate")))
					{
						XtraMessageBoxEx.Show(Enums.MessageCode.ToDateNotInPeriodRange, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK);
						gv_Doc.SetFocusedRowCellValue("ToDate", Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("PToDate")));
						Control_EditValueChanged(null, null);
						return;
					}
					if (Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("FromDate")).HasValue && Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("ToDate")).HasValue && Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("ToDate")) < Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("FromDate")))
					{
						XtraMessageBoxEx.Show(Enums.MessageCode.FromDateCanNotLargerThanToDate, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK);
						gv_Doc.SetFocusedRowCellValue("ToDate", Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("PToDate")));
						Control_EditValueChanged(null, null);
						return;
					}
				}
			}
			Control_EditValueChanged(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Control_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
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
			if (FormMode == Enums.FormViewModeType.Add)
			{
				return;
			}
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue("status")) != "C")
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					return;
				}
				int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
				if (num > 0)
				{
					string text = await _httpClient.TimeSheetingClient.RemoveBiometricData(num);
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

	private void gv_Line_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (!_isChanging && sender is GridViewEx gridViewEx)
		{
			_isChanging = true;
			if (string.IsNullOrWhiteSpace(Function.ToString(gridViewEx.GetRowCellValue(e.RowHandle, "LineNum"))))
			{
				gridViewEx.SetRowCellValue(e.RowHandle, "FixedNote", "Added by " + GlobalData.UserCodeLogin);
			}
			else if (!CellExists(gridViewEx.GetDataSourceRowIndex(e.RowHandle), e.Column) && (e.Column.FieldName == "ClockDate" || e.Column.FieldName == "DateTimeRecord" || e.Column.FieldName == "OriginType" || e.Column.FieldName == "Remarks"))
			{
				modifiedCells.Add(new GridCell(gridViewEx.GetDataSourceRowIndex(e.RowHandle), e.Column));
				gridViewEx.SetRowCellValue(e.RowHandle, "FixedNote", "Last Modified by " + GlobalData.UserCodeLogin);
			}
			if (e.Column.FieldName == "ClockDate")
			{
				TimeSpan timeOfDay = Function.ParseDateTimes(gridViewEx.GetRowCellValue(e.RowHandle, "DateTimeRecord")).TimeOfDay;
				gridViewEx.SetRowCellValue(e.RowHandle, "DateTimeRecord", Function.ParseDateTimes(e.Value).Add(timeOfDay));
			}
			if (e.Column.FieldName == "DateTimeRecord")
			{
				TimeSpan timeOfDay2 = Function.ParseDateTimes(e.Value).TimeOfDay;
				gridViewEx.SetRowCellValue(e.RowHandle, "DateTimeRecord", Function.ParseDateTimes(gridViewEx.GetRowCellValue(e.RowHandle, "ClockDate")).Add(timeOfDay2));
			}
			Control_EditValueChanged(null, null);
			_isChanging = false;
		}
	}

	private void gv_Line_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if (sender is GridViewEx gridViewEx)
		{
			if (string.IsNullOrWhiteSpace(Function.ToString(gridViewEx.GetRowCellValue(e.RowHandle, "LineNum"))))
			{
				e.Appearance.BackColor = Color.PowderBlue;
			}
			if (e.Column.FieldName == "Source" && Function.ToString(e.CellValue) == "99")
			{
				e.Appearance.ForeColor = Color.Green;
			}
			if (Form.ActiveForm == this && CellExists(gridViewEx.GetDataSourceRowIndex(e.RowHandle), e.Column))
			{
				e.Appearance.BackColor = Color.Wheat;
			}
		}
	}

	private void stripItem_AddLine_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Branch"));
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("Period"));
			if (num == 0 || text == "")
			{
				WinCommons.Error(Enums.MessageCode.BranchOrPeriodIsEmpty, "");
				return;
			}
			AddNewLine();
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

	private void AddNewLine()
	{
		DataTable dataTable = grd_Line.DataSourceTable();
		try
		{
			gv_Line.BeginInsertRow();
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("APZ_TBD1");
				dataTable.Columns.Add("EnrollName", typeof(string));
			}
			int num = Function.ParseInt(gv_Line.GetRowCellValue(gv_Line.FocusedRowHandle, "MachineNo"));
			DataRow dataRow = dataTable.NewRow();
			dataRow["LineNum"] = DBNull.Value;
			dataRow["MachineNo"] = ((num == 0) ? DBNull.Value : ((object)num));
			dataRow["Source"] = 99;
			dataTable.Rows.Add(dataRow);
			grd_Line.DataSource = dataTable;
			grd_Line.RefreshDataSource();
			gv_Line.ExpandAllGroups();
			gv_Line.MakeRowVisible(gv_Line.GetRowHandle(dataTable.Rows.Count - 1));
			gv_Line.EndInsertRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Line.EndInsertRow();
		dataTable.AcceptChanges();
	}

	private void stripItem_RemoveLine_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Line.DataSourceTable;
			DataTable source = dataSourceTable.Clone();
			List<DataRow> listDataSourceSelectedRow = gv_Line.GetListDataSourceSelectedRow();
			if (listDataSourceSelectedRow.Any())
			{
				source = listDataSourceSelectedRow.CopyToDataTable();
			}
			if (source.AsEnumerable().FirstOrDefault((DataRow t) => !string.IsNullOrWhiteSpace(Function.ToString(t["LineNum"])) || Function.ParseInt(t["Source"]) != 99) == null || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteRecordWithIrrever, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				gv_Line.BeginUpdate();
				gv_Line.DeleteSelectedRows();
				gv_Line.EndUpdate();
				grd_Line.RefreshDataSource();
				dataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Line_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (!(sender is GridViewEx gridViewEx))
			{
				return;
			}
			if (gridViewEx.FocusedColumn.FieldName == "FixedNote" || gridViewEx.FocusedColumn.FieldName == "Source" || gridViewEx.FocusedColumn.FieldName == "LineNum" || gridViewEx.FocusedColumn.FieldName == "AbsEntry")
			{
				e.Cancel = true;
				return;
			}
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue("status")) == "C")
			{
				e.Cancel = true;
				return;
			}
			string text = Function.ToString(gridViewEx.GetFocusedRowCellValue("Source"));
			if (FormMode == Enums.FormViewModeType.Add && text != "99")
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Line_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (sender is GridViewEx gridViewEx && e.Column == gc_empNo)
			{
				if (!string.IsNullOrWhiteSpace(Function.ToString(gridViewEx.GetRowCellValue(e.RowHandle, "LineNum"))))
				{
					e.RepositoryItem = rep_CFLEnroll_NoSearch;
				}
				else
				{
					e.RepositoryItem = rep_cfl_enrollNo;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private bool CellExists(int sourceHandle, GridColumn col)
	{
		return modifiedCells.FirstOrDefault((GridCell c) => c.Column == col && c.RowHandle == sourceHandle) != null;
	}

	private void gv_Line_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (!(sender is GridViewEx gridViewEx) || !(((GridColumnEx)gridViewEx.FocusedColumn).FieldName == "ClockDate"))
			{
				return;
			}
			string period = Function.ToString(gv_Doc.GetFocusedRowCellValue("Period"));
			DataRow dataRow = _dtPeriod.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["PeriodCode"]) == period);
			if (dataRow != null)
			{
				DateTime dateTime = Function.ParseDateTimes(dataRow["FromDate"]);
				DateTime dateTime2 = Function.ParseDateTimes(dataRow["ToDate"]);
				DateTime dateTime3 = Function.ParseDateTimes(e.Value);
				if (dateTime3 < dateTime || dateTime3 > dateTime2)
				{
					e.Valid = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue("status")) == "C")
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
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

	private void gv_Line_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		try
		{
			if (sender is ColumnView columnView && !(((GridColumnEx)columnView.FocusedColumn).FieldName != "ClockDate"))
			{
				e.ExceptionMode = ExceptionMode.DisplayError;
				e.WindowCaption = "Input Error";
				e.ErrorText = "Date is not valid (Not in period's Time range)!";
				columnView.HideEditor();
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_BiometricData));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Period = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Period = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_PFromDate = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_PToDate = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx1 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemHourEdit1 = new Apzon.Controls.Controls.RepositoryItemHourEdit();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_UserCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_FromDate = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ToDate = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Line = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.stripItem_AddLine = new System.Windows.Forms.ToolStripMenuItem();
		this.stripItem_RemoveLine = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_empNo = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_cfl_enrollNo = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx2 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_OriginType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Source = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_MachineNo = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemTimeEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTimeEdit();
		this.repositoryItemHourEdit2 = new Apzon.Controls.Controls.RepositoryItemHourEdit();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_LoadFromDevice = new DevExpress.XtraEditors.SimpleButton();
		this.btn_CopyTo = new DevExpress.XtraEditors.SimpleButton();
		this.luk_MachineNo = new Apzon.Controls.Editor.LookUpEditEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctx_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Period).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PFromDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PFromDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PToDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PToDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UserCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_FromDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_FromDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ToDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ToDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		this.ctx_Line.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_enrollNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_OriginType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Source).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MachineNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTimeEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_MachineNo.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 4);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Line);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1046, 467);
		this.splitContainerControl1.SplitterPosition = 222;
		this.splitContainerControl1.TabIndex = 91;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctx_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[12]
		{
			this.rep_Branch, this.repositoryItemMemoExEdit2, this.rep_Period, this.repositoryItemCheckEdit1, this.rep_Status, this.repositoryItemDateEditEx1, this.repositoryItemHourEdit1, this.rep_UserCode, this.rep_FromDate, this.rep_ToDate,
			this.rep_PFromDate, this.rep_PToDate
		});
		this.grd_Doc.Size = new System.Drawing.Size(1046, 222);
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
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clBiometricData;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.gridColumnEx7, this.gc_Period, this.gridColumnEx10, this.gridColumnEx11, this.gridColumnEx9, this.gridColumnEx12, this.gridColumnEx2, this.gridColumnEx4, this.gridColumnEx15, this.gridColumnEx14,
			this.gridColumnEx3, this.gridColumnEx17
		});
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oBiometricData";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 30;
		this.gv_Doc.IsApzTable = true;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_OTBD";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.RowAutoHeight = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChangedAsync);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gridColumnEx7.Caption = "Doc No.";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "AbsEntry";
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
		this.gc_Period.PrimaryKey = false;
		this.gc_Period.UnitField = null;
		this.gc_Period.Visible = true;
		this.gc_Period.VisibleIndex = 0;
		this.gc_Period.Width = 92;
		this.rep_Period.AutoHeight = false;
		this.rep_Period.AutoLoadDataSource = true;
		this.rep_Period.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Period.ContainerControl = this;
		this.rep_Period.DisplayMember = "";
		this.rep_Period.DisplayMember2 = "";
		this.rep_Period.LoadDataSourceWhenEnter = false;
		this.rep_Period.Name = "rep_Period";
		this.rep_Period.SeparateCharacterForMulti = ';';
		this.rep_Period.SeparateString = ", ";
		this.rep_Period.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.WorkPeriod;
		this.rep_Period.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oWorkPeriod;
		this.rep_Period.UsingAutoCompleteBox = false;
		this.rep_Period.UsingDataSource = true;
		this.rep_Period.ValidateWhenNoDataSource = false;
		this.rep_Period.ValueMember = "";
		this.gridColumnEx10.Caption = "Period Start Date";
		this.gridColumnEx10.ColumnEdit = this.rep_PFromDate;
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "PFromDate";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 2;
		this.gridColumnEx10.Width = 101;
		this.rep_PFromDate.AutoHeight = false;
		this.rep_PFromDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, false, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_PFromDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_PFromDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.rep_PFromDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_PFromDate.EditFormat.FormatString = "dd/MM/yyyy";
		this.rep_PFromDate.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_PFromDate.IsSearchControl = false;
		this.rep_PFromDate.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.rep_PFromDate.Name = "rep_PFromDate";
		this.rep_PFromDate.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.rep_PFromDate.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.gridColumnEx11.Caption = "Period End Date";
		this.gridColumnEx11.ColumnEdit = this.rep_PToDate;
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "PToDate";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 3;
		this.gridColumnEx11.Width = 104;
		this.rep_PToDate.AutoHeight = false;
		this.rep_PToDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, false, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_PToDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_PToDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.rep_PToDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_PToDate.EditFormat.FormatString = "dd/MM/yyyy";
		this.rep_PToDate.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_PToDate.IsSearchControl = false;
		this.rep_PToDate.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.rep_PToDate.Name = "rep_PToDate";
		this.rep_PToDate.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.rep_PToDate.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
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
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 1;
		this.gridColumnEx9.Width = 92;
		this.rep_Branch.AutoHeight = false;
		this.rep_Branch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, false, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Branch.Name = "rep_Branch";
		this.rep_Branch.NullText = "";
		this.rep_Branch.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.gridColumnEx12.Caption = "Remark";
		this.gridColumnEx12.ColumnEdit = this.repositoryItemMemoExEdit2;
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "Remark";
		this.gridColumnEx12.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Visible = true;
		this.gridColumnEx12.VisibleIndex = 6;
		this.gridColumnEx12.Width = 215;
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
		this.gridColumnEx2.VisibleIndex = 5;
		this.gridColumnEx2.Width = 65;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, false, false, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Status.Name = "rep_Status";
		this.rep_Status.NullText = "";
		this.rep_Status.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.gridColumnEx4.Caption = "Import Date";
		this.gridColumnEx4.ColumnEdit = this.repositoryItemDateEditEx1;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "ImportDate";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 4;
		this.gridColumnEx4.Width = 106;
		this.repositoryItemDateEditEx1.AutoHeight = false;
		this.repositoryItemDateEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, false, false, imageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repositoryItemDateEditEx1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.IsSearchControl = false;
		this.repositoryItemDateEditEx1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx1.Name = "repositoryItemDateEditEx1";
		this.repositoryItemDateEditEx1.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.repositoryItemDateEditEx1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.gridColumnEx15.Caption = "Import Time";
		this.gridColumnEx15.ColumnEdit = this.repositoryItemHourEdit1;
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "ImportTime";
		this.gridColumnEx15.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.repositoryItemHourEdit1.AutoHeight = false;
		this.repositoryItemHourEdit1.IsSearchControl = false;
		this.repositoryItemHourEdit1.MaxLength = 5;
		this.repositoryItemHourEdit1.Name = "repositoryItemHourEdit1";
		this.repositoryItemHourEdit1.ReadOnly = true;
		this.gridColumnEx14.Caption = "User Import";
		this.gridColumnEx14.ColumnEdit = this.rep_UserCode;
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "UserImport";
		this.gridColumnEx14.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx14.Visible = true;
		this.gridColumnEx14.VisibleIndex = 7;
		this.gridColumnEx14.Width = 118;
		this.rep_UserCode.AutoHeight = false;
		this.rep_UserCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, false, false, imageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_UserCode.Name = "rep_UserCode";
		this.rep_UserCode.NullText = "";
		this.rep_UserCode.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.gridColumnEx3.Caption = "From Date";
		this.gridColumnEx3.ColumnEdit = this.rep_FromDate;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "FromDate";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 8;
		this.gridColumnEx3.Width = 101;
		this.rep_FromDate.AutoHeight = false;
		this.rep_FromDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_FromDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_FromDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.rep_FromDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_FromDate.EditFormat.FormatString = "dd/MM/yyyy";
		this.rep_FromDate.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_FromDate.IsSearchControl = false;
		this.rep_FromDate.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.rep_FromDate.Name = "rep_FromDate";
		this.rep_FromDate.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.gridColumnEx17.Caption = "To Date";
		this.gridColumnEx17.ColumnEdit = this.rep_ToDate;
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.FieldName = "ToDate";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx17.Visible = true;
		this.gridColumnEx17.VisibleIndex = 9;
		this.gridColumnEx17.Width = 101;
		this.rep_ToDate.AutoHeight = false;
		this.rep_ToDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ToDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ToDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.rep_ToDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_ToDate.EditFormat.FormatString = "dd/MM/yyyy";
		this.rep_ToDate.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_ToDate.IsSearchControl = false;
		this.rep_ToDate.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.rep_ToDate.Name = "rep_ToDate";
		this.rep_ToDate.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.ContextMenuStrip = this.ctx_Line;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.repositoryItemDateEditEx2, this.repositoryItemHourEdit2, this.repositoryItemTimeEdit1, this.rep_Source, this.rep_OriginType, this.rep_MachineNo, this.rep_cfl_enrollNo });
		this.grd_Line.Size = new System.Drawing.Size(1046, 235);
		this.grd_Line.TabIndex = 83;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.ctx_Line.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.stripItem_AddLine, this.stripItem_RemoveLine });
		this.ctx_Line.Name = "ctx_Doc";
		this.ctx_Line.Size = new System.Drawing.Size(143, 48);
		this.stripItem_AddLine.Name = "stripItem_AddLine";
		this.stripItem_AddLine.Size = new System.Drawing.Size(142, 22);
		this.stripItem_AddLine.Text = "Add Line";
		this.stripItem_AddLine.Click += new System.EventHandler(this.stripItem_AddLine_Click);
		this.stripItem_RemoveLine.Name = "stripItem_RemoveLine";
		this.stripItem_RemoveLine.Size = new System.Drawing.Size(142, 22);
		this.stripItem_RemoveLine.Text = "Remove Line";
		this.stripItem_RemoveLine.Click += new System.EventHandler(this.stripItem_RemoveLine_Click);
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = "LineNum";
		this.gv_Line.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumnEx13, this.gc_empNo, this.gridColumnEx1, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx8, this.gridColumnEx16 });
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oBiometricData";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 30;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = "APZ_TBD1";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = new string[1] { "AbsEntry" };
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_Line.OptionsSelection.MultiSelect = true;
		this.gv_Line.OptionsView.RowAutoHeight = true;
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = true;
		this.gv_Line.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Line_RowCellStyle);
		this.gv_Line.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Line_CustomRowCellEdit);
		this.gv_Line.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Line_ShowingEditor);
		this.gv_Line.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Line_CellValueChanged);
		this.gv_Line.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Line_ValidatingEditor);
		this.gv_Line.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gv_Line_InvalidValueException);
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
		this.gc_empNo.Caption = "Enroll Number";
		this.gc_empNo.ColumnEdit = this.rep_cfl_enrollNo;
		this.gc_empNo.CurrencyDateField = null;
		this.gc_empNo.CurrencyField = null;
		this.gc_empNo.CurrencyFixed = null;
		this.gc_empNo.CurrencyFixedRate = 0.0;
		this.gc_empNo.CurrencyRateField = null;
		this.gc_empNo.FieldName = "EnrollNo";
		this.gc_empNo.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_empNo.Name = "gc_empNo";
		this.gc_empNo.PrimaryKey = true;
		this.gc_empNo.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gc_empNo.UnitField = null;
		this.gc_empNo.Visible = true;
		this.gc_empNo.VisibleIndex = 0;
		this.gc_empNo.Width = 136;
		this.rep_cfl_enrollNo.AutoHeight = false;
		this.rep_cfl_enrollNo.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions9.Image");
		this.rep_cfl_enrollNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_enrollNo.ContainerControl = this;
		this.rep_cfl_enrollNo.DisplayMember = "EnrollNo";
		this.rep_cfl_enrollNo.DisplayMember2 = "";
		this.rep_cfl_enrollNo.LoadDataSourceWhenEnter = false;
		this.rep_cfl_enrollNo.Name = "rep_cfl_enrollNo";
		this.rep_cfl_enrollNo.SeparateCharacterForMulti = ';';
		this.rep_cfl_enrollNo.SeparateString = ", ";
		this.rep_cfl_enrollNo.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.BioMapping;
		this.rep_cfl_enrollNo.UsingAutoCompleteBox = false;
		this.rep_cfl_enrollNo.UsingDataSource = true;
		this.rep_cfl_enrollNo.ValidateWhenNoDataSource = false;
		this.rep_cfl_enrollNo.ValueMember = "EnrollNo";
		this.gridColumnEx1.Caption = "Clock Date";
		this.gridColumnEx1.ColumnEdit = this.repositoryItemDateEditEx2;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "ClockDate";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 1;
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
		this.gridColumnEx5.Caption = "Origin Type";
		this.gridColumnEx5.ColumnEdit = this.rep_OriginType;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "OriginType";
		this.gridColumnEx5.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 2;
		this.rep_OriginType.AutoHeight = false;
		this.rep_OriginType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_OriginType.Name = "rep_OriginType";
		this.rep_OriginType.NullText = "";
		this.gridColumnEx6.Caption = "Source";
		this.gridColumnEx6.ColumnEdit = this.rep_Source;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Source";
		this.gridColumnEx6.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 3;
		this.rep_Source.AutoHeight = false;
		this.rep_Source.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Source.Name = "rep_Source";
		this.rep_Source.NullText = "";
		this.gridColumnEx8.Caption = "Machine Number";
		this.gridColumnEx8.ColumnEdit = this.rep_MachineNo;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "MachineNo";
		this.gridColumnEx8.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 4;
		this.rep_MachineNo.AutoHeight = false;
		this.rep_MachineNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_MachineNo.Name = "rep_MachineNo";
		this.rep_MachineNo.NullText = "";
		this.gridColumnEx16.Caption = "Full DateTime";
		this.gridColumnEx16.ColumnEdit = this.repositoryItemTimeEdit1;
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "DateTimeRecord";
		this.gridColumnEx16.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx16.Visible = true;
		this.gridColumnEx16.VisibleIndex = 5;
		this.repositoryItemTimeEdit1.AutoHeight = false;
		this.repositoryItemTimeEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemTimeEdit1.Mask.UseMaskAsDisplayFormat = true;
		this.repositoryItemTimeEdit1.Name = "repositoryItemTimeEdit1";
		this.repositoryItemHourEdit2.AutoHeight = false;
		this.repositoryItemHourEdit2.IsSearchControl = false;
		this.repositoryItemHourEdit2.MaxLength = 5;
		this.repositoryItemHourEdit2.Name = "repositoryItemHourEdit2";
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
		this.btn_LoadFromDevice.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_LoadFromDevice.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_LoadFromDevice.ImageOptions.Image");
		this.btn_LoadFromDevice.Location = new System.Drawing.Point(168, 475);
		this.btn_LoadFromDevice.Name = "btn_LoadFromDevice";
		this.btn_LoadFromDevice.Size = new System.Drawing.Size(189, 23);
		this.btn_LoadFromDevice.TabIndex = 92;
		this.btn_LoadFromDevice.Text = "Load From Device";
		this.btn_LoadFromDevice.Click += new System.EventHandler(this.btn_LoadFromDevice_Click);
		this.btn_CopyTo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_CopyTo.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CopyTo.ImageOptions.Image");
		this.btn_CopyTo.Location = new System.Drawing.Point(903, 475);
		this.btn_CopyTo.Name = "btn_CopyTo";
		this.btn_CopyTo.Size = new System.Drawing.Size(145, 23);
		this.btn_CopyTo.TabIndex = 93;
		this.btn_CopyTo.Text = "Copy To InOut Table";
		this.btn_CopyTo.Click += new System.EventHandler(this.btn_CopyTo_Click);
		this.luk_MachineNo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.luk_MachineNo.EditControl = null;
		this.luk_MachineNo.FieldName = null;
		this.luk_MachineNo.IsSearchControl = false;
		this.luk_MachineNo.Location = new System.Drawing.Point(363, 477);
		this.luk_MachineNo.Name = "luk_MachineNo";
		this.luk_MachineNo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_MachineNo.Properties.NullText = "";
		this.luk_MachineNo.SettingInVisible = false;
		this.luk_MachineNo.SettingReadonly = false;
		this.luk_MachineNo.Size = new System.Drawing.Size(209, 20);
		this.luk_MachineNo.TabIndex = 94;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1049, 500);
		base.Controls.Add(this.luk_MachineNo);
		base.Controls.Add(this.btn_CopyTo);
		base.Controls.Add(this.btn_LoadFromDevice);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.FormId = "oBiometricData";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_BiometricData";
		this.Text = "Biometric Data Log";
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctx_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Period).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PFromDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PFromDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PToDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PToDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UserCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_FromDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_FromDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ToDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ToDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		this.ctx_Line.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_enrollNo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_OriginType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Source).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MachineNo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTimeEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_MachineNo.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
