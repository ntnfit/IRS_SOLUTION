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
using Apzon.Forms.Helper;
using Apzon.Forms.HumanResources.Biometric;
using Apzon.Forms.HumanResources.Biometric.KJTech;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_BiometricDataProcess : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _loadingData;

	private DataTable _dtPeriod;

	private int _currentEntry;

	private string _msg = "";

	private IContainer components;

	private GridControlEx grd_Devices;

	private GridViewEx gv_Devices;

	private SimpleButton btn_LoadFromDevice;

	private SimpleButton btn_Extract;

	private SeparatorControl separatorControl1;

	private LabelLine lbl_ValidFrom;

	private LabelLine lbl_ValidTo;

	private GridColumnEx gc_MachineName;

	private GridColumnEx gc_Upload;

	private GridColumnEx gc_Extract;

	private GridControlEx grd_DataLog;

	private GridViewEx gv_DataLog;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit1;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit2;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit3;

	private RepositoryItemLookUpEdit rep_MachineNo;

	private RepositoryItemLookUpEdit rep_UpStatus;

	private RepositoryItemLookUpEdit rep_ExtStatus;

	private DataTable dataTable1;

	private DataTable dataTable2;

	private DateEditEx date_ValidTo;

	private DateEditEx date_ValidFrom;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				btn_LoadFromDevice.Enabled = true;
				btn_Extract.Enabled = true;
				return;
			}
			_formMode = value;
			switch (_formMode)
			{
			case Enums.FormViewModeType.Update:
			case Enums.FormViewModeType.Add:
			case Enums.FormViewModeType.View:
			{
				SimpleButton simpleButton2 = btn_LoadFromDevice;
				bool enabled = (btn_Extract.Enabled = true);
				simpleButton2.Enabled = enabled;
				break;
			}
			case Enums.FormViewModeType.Busy:
			{
				SimpleButton simpleButton = btn_LoadFromDevice;
				bool enabled = (btn_Extract.Enabled = false);
				simpleButton.Enabled = enabled;
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

	public frm_BiometricDataProcess()
	{
		InitializeComponent();
		FormId = "oBiometricData";
		OnLoadAsync += OnOnLoadAsync;
		gv_Devices.FormId = FormId;
		gv_Devices.ItemId = "APZ_OBMC";
		gv_Devices.IsApzTable = true;
		gv_DataLog.ItemId = "APZ_TBD1";
		gv_DataLog.IsApzTable = true;
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

	private async Task LoadData()
	{
		_ = 1;
		try
		{
			_loadingData = true;
			DataTable dataTable = await _httpClient.TimeSheetingClient.GetAllBiometricConfig();
			dataTable.Columns.Add("Upload", typeof(string));
			dataTable.Columns.Add("Extract", typeof(string));
			foreach (DataRow row in dataTable.Rows)
			{
				row["Upload"] = "U";
				row["Extract"] = "U";
			}
			grd_Devices.DataSource = dataTable;
			gv_Devices.SelectAll();
			grd_DataLog.DataSource = WinFuntions.GetTableStructure("APZ_TBD1");
			_dtPeriod = await _httpClient.WorkPeriod.GetListWorkPeriod();
			_loadingData = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadInitForm()
	{
		try
		{
			List<Common.Table_Code> list = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "S",
					Name = "Success"
				},
				new Common.Table_Code
				{
					Code = "U",
					Name = "Unsuccess"
				}
			};
			LookUpEditEx lookUpEditEx = new LookUpEditEx();
			lookUpEditEx.SetDataSourceByList(list);
			rep_UpStatus.Assign(lookUpEditEx.Properties);
			rep_ExtStatus.Assign(lookUpEditEx.Properties);
			DataTable dataTable = await _httpClient.TimeSheetingClient.GetAllBiometricConfig();
			EnumerableRowCollection<DataRow> source = from t in dataTable.AsEnumerable()
				where Function.ToString(t["Status"]) != "N"
				select t;
			DataTable dataSource = (source.Any() ? source.CopyToDataTable() : dataTable.Clone());
			rep_MachineNo.DataSource = dataSource;
			rep_MachineNo.DisplayMember = "MachineName";
			rep_MachineNo.ValueMember = "MachineID";
			rep_MachineNo.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_MachineNo.ForceInitialize();
			rep_MachineNo.PopulateColumns();
			date_ValidFrom.EditValue = DateTime.Today.AddDays(-1.0);
			date_ValidTo.EditValue = DateTime.Today;
			btn_Extract.Enabled = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_LoadFromDevice_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			btn_LoadFromDevice.Enabled = false;
			DataTable dtGridCopy = grd_DataLog.DataSourceTable.Copy();
			dtGridCopy.SetPrimaryKey("MachineNo", "EnrollNo", "DateTimeRecord");
			dtGridCopy.Rows.Clear();
			new DataTable();
			gv_Devices.GetSelectedRows();
			_ = grd_Devices.DataSourceTable;
			List<DataRow> listDataSourceSelectedRow = gv_Devices.GetListDataSourceSelectedRow();
			DataTable dataTable = ((listDataSourceSelectedRow.Count <= 0 || !Function.ToString(listDataSourceSelectedRow[0]["DeviceCode"]).Equals("KJ")) ? (await DownLoadData()) : (await DownLoadDataKJtech()));
			if (dataTable.IsNotNull())
			{
				dataTable.SetPrimaryKey("MachineNo", "EnrollNo", "DateTimeRecord");
				dtGridCopy.Merge(dataTable);
				dtGridCopy.PrimaryKey = null;
				dtGridCopy.Columns["MachineNo"].AllowDBNull = true;
				dtGridCopy.Columns["EnrollNo"].AllowDBNull = true;
				dtGridCopy.Columns["DateTimeRecord"].AllowDBNull = true;
				grd_DataLog.DataSource = dtGridCopy.Copy();
				grd_DataLog.DataSourceTable.AcceptChanges();
				await SaveData();
				if (grd_DataLog.DataSourceTable.IsNotNull())
				{
					btn_Extract.Enabled = true;
				}
				btn_LoadFromDevice.Enabled = true;
				if (string.IsNullOrEmpty(_msg))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, _msg);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Extract_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			if (_currentEntry <= 0)
			{
				return;
			}
			FormMode = Enums.FormViewModeType.Busy;
			WinCommons._OpenProcessing();
			DataSet dataSet = await _httpClient.TimeSheetingClient.LoadInOutByBaseDoc(_currentEntry);
			if (dataSet.IsNotNull())
			{
				DataTable dtOt = dataSet.Tables[0];
				DataTable dtLineSave = GetDataSave(dtOt, "APZ_IOD1");
				DataTable dtOt2 = dataSet.Tables[1];
				DataTable dtOtSave = GetDataSave(dtOt2, "APZ_IOD2");
				DataTable dtHeader = WinFuntions.GetTableStructure("APZ_OIOD");
				DataRow Period = (from p in _dtPeriod.AsEnumerable()
					where Function.ParseDateTime(p["FromDate"]) <= Function.ParseDateTime(date_ValidTo.EditValue) && Function.ParseDateTime(date_ValidFrom.EditValue) <= Function.ParseDateTime(p["ToDate"])
					select p).FirstOrDefault();
				DataTable dataTable = await _httpClient.TimeSheetingClient.LoadInOutList();
				DataRow dataRow = dtHeader.NewRow();
				if (dataTable.IsNotNull())
				{
					DataRow dataRow2 = (from p in dataTable.AsEnumerable()
						where Function.ParseInt(p["BaseDoc"]) == _currentEntry
						select p).FirstOrDefault();
					if (Function.ParseInt(dataRow2) == 0)
					{
						Function.ParseInt(dataTable.AsEnumerable().Max((DataRow x) => x["AbsEntry"]));
						dataRow["BaseDoc"] = _currentEntry;
						dataRow["Period"] = Function.ToString(Period["PeriodCode"]);
						dataRow["Branch"] = Function.ParseInt(Period["Branch"]);
						dataRow["CreateDate"] = DateTime.Today.ToString("yyyy-MM-dd");
						dataRow["UserSign"] = GlobalData.UserID;
						dataRow["status"] = "O";
					}
					else
					{
						dataRow["AbsEntry"] = Function.ParseInt(dataRow2["AbsEntry"]);
						dataRow["Period"] = Function.ToString(dataRow2["PeriodCode"]);
						dataRow["Branch"] = Function.ParseInt(dataRow2["Branch"]);
						dataRow["CreateDate"] = dataRow2["CreateDate"];
						dataRow["UserSign"] = Function.ParseInt(dataRow2["CreateDate"]);
						dataRow["BaseDoc"] = Function.ParseInt(dataRow2["BaseDoc"]);
						dataRow["Remark"] = Function.ParseInt(dataRow2["Remark"]);
						dataRow["UserSign2"] = GlobalData.UserID;
						dataRow["UpdateDate"] = DateTime.Today.ToString("yyyy-MM-dd");
						dataRow["status"] = Function.ToString(dataRow2["status"]);
					}
				}
				else
				{
					dataRow["BaseDoc"] = _currentEntry;
					dataRow["Period"] = Function.ToString(Period["PeriodCode"]);
					dataRow["Branch"] = Function.ParseInt(Period["Branch"]);
					dataRow["CreateDate"] = DateTime.Today.ToString("yyyy-MM-dd");
					dataRow["UserSign"] = GlobalData.UserID;
					dataRow["status"] = "O";
				}
				dtHeader.Rows.Add(dataRow);
				dtHeader.AcceptChanges();
				DataSet dataSet2 = new DataSet();
				dataSet2.Tables.Add(dtHeader.Copy());
				dataSet2.Tables.Add(dtLineSave.Copy());
				dataSet2.Tables.Add(dtOtSave.Copy());
				if (string.IsNullOrEmpty((await _httpClient.TimeSheetingClient.SaveTimeSheetInOut(dataSet2)).Message))
				{
					foreach (DataRow row in grd_Devices.DataSourceTable.Rows)
					{
						if (Function.ToString(row["Upload"]).Equals("U"))
						{
							row["Extract"] = "S";
						}
					}
					grd_Devices.RefreshDataSource();
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
			}
			FormMode = Enums.FormViewModeType.View;
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.None, ex.Message);
		}
	}

	private async Task SaveData()
	{
		_ = 1;
		try
		{
			DataTable dtDataLog = grd_DataLog.DataSourceTable();
			if (!dtDataLog.IsNotNull())
			{
				Logging.Write("ERROR", Function.ToString(Enums.MessageCode.NoDataToSave));
				return;
			}
			DataSet ds = new DataSet();
			DataTable dtHeaderSave = WinFuntions.GetTableStructure("APZ_OTBD");
			DataRow r = dtHeaderSave.NewRow();
			DataRow Period = (from p in _dtPeriod.AsEnumerable()
				where Function.ParseDateTime(p["FromDate"]) <= Function.ParseDateTime(date_ValidTo.EditValue) && Function.ParseDateTime(date_ValidFrom.EditValue) <= Function.ParseDateTime(p["ToDate"])
				select p).FirstOrDefault();
			DataTable dataTable = await _httpClient.TimeSheetingClient.LoadImportList();
			int num = 1;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = (from p in dataTable.AsEnumerable()
					where Function.ToString(p["Period"]) == Function.ToString(Period["PeriodCode"])
					select p).FirstOrDefault();
				num = ((dataRow == null || Function.ParseInt(dataRow["AbsEntry"]) == 0) ? (Function.ParseInt(dataTable.AsEnumerable().Max((DataRow x) => x["AbsEntry"])) + 1) : Function.ParseInt(dataRow["AbsEntry"]));
			}
			r["AbsEntry"] = Function.ParseInt(num);
			r["Period"] = Function.ToString(Period["PeriodCode"]);
			r["Branch"] = Function.ToString(Period["branch"]);
			r["PFromDate"] = Function.ParseDateTime(Period["FromDate"]);
			r["PToDate"] = Function.ParseDateTime(Period["ToDate"]);
			r["FromDate"] = Function.ParseDateTime(date_ValidFrom.EditValue);
			r["ToDate"] = Function.ParseDateTime(date_ValidTo.EditValue);
			r["ImportDate"] = Function.ParseDateTime(DateTime.Now.Date);
			r["ImportTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
			r["status"] = "O";
			r["UserSign2"] = GlobalData.UserID;
			r["UpdateDate"] = DateTime.Today;
			dtHeaderSave.Rows.Add(r);
			dtHeaderSave.AcceptChanges();
			List<string> list = new List<string>();
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_TBD1");
			for (int num2 = 0; num2 < dtDataLog.Columns.Count; num2++)
			{
				DataColumn dataColumn = dtDataLog.Columns[num2];
				if (!tableStructure.Columns.Contains(Function.ToString(dataColumn.ColumnName)))
				{
					list.Add(dtDataLog.Columns[num2].ColumnName);
				}
			}
			foreach (string item in list)
			{
				dtDataLog.Columns.Remove(item);
			}
			ds.Tables.Add(dtHeaderSave.Copy());
			ds.Tables.Add(dtDataLog.Copy());
			_currentEntry = Function.ParseInt(num);
			_msg = Function.ToString((await _httpClient.TimeSheetingClient.SaveBiometricData(ds)).Message);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.None, ex.Message);
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

	private async Task<DataTable> DownLoadData()
	{
		DeviceManipulator manipulator = new DeviceManipulator();
		ZkemClient objZkeeper = new ZkemClient();
		try
		{
			DataTable dt = new DataTable();
			DateTime fromDate = Function.ParseDateTimes(date_ValidFrom.EditValue);
			DateTime toDate = Function.ParseDateTimes(date_ValidTo.EditValue).AddDays(1.0);
			if (toDate.TimeOfDay.Hours == 0)
			{
				TimeSpan timeSpan = new TimeSpan(23, 59, 59);
				toDate = toDate.Date + timeSpan;
			}
			string sTime = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
			string eTime = toDate.ToString("yyyy-MM-dd HH:mm:ss");
			List<int> listDataSourceSelectedRowIndex = gv_Devices.GetListDataSourceSelectedRowIndex();
			DataTable dtMachine = grd_Devices.DataSourceTable();
			foreach (int row in listDataSourceSelectedRowIndex)
			{
				int id = Function.ParseInt(dtMachine.Rows[row]["MachineID"]);
				string name = Function.ToString(dtMachine.Rows[row]["MachineName"]);
				if (id < 0)
				{
					continue;
				}
				if (!(await objZkeeper.ConnectDevice(id)))
				{
					WinCommons.Error(Enums.MessageCode.CannotConnectToDevicePleaseRecheckConfig, name);
					continue;
				}
				DataTable logDataTable = manipulator.GetLogDataTable(objZkeeper, id, sTime, eTime, fromDate, toDate);
				if (!logDataTable.IsNotNull())
				{
					WinCommons.Error(Enums.MessageCode.None, name, "Log data empty!");
					continue;
				}
				EnumerableRowCollection<DataRow> source = from t in logDataTable.AsEnumerable()
					where Function.ParseDateTimes(t["ClockDate"]).Date >= fromDate.Date && Function.ParseDateTimes(t["ClockDate"]).Date <= toDate.Date
					select t;
				if (source.Any())
				{
					if (!dt.IsNotNull())
					{
						dt = source.CopyToDataTable();
					}
					else
					{
						dt.Merge(source.CopyToDataTable(), preserveChanges: true);
					}
					gv_Devices.SetRowCellValue(row, "Upload", "S");
				}
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

	private async Task<DataTable> DownLoadDataKJtech()
	{
		try
		{
			DataTable dt = new DataTable();
			DateTime fromDate = Function.ParseDateTimes(date_ValidFrom.EditValue);
			DateTime toDate = Function.ParseDateTimes(date_ValidTo.EditValue).AddDays(1.0);
			if (toDate.TimeOfDay.Hours == 0)
			{
				TimeSpan timeSpan = new TimeSpan(23, 59, 59);
				toDate = toDate.Date + timeSpan;
			}
			fromDate.ToString("yyyy-MM-dd HH:mm:ss");
			toDate.ToString("yyyy-MM-dd HH:mm:ss");
			List<int> listDataSourceSelectedRowIndex = gv_Devices.GetListDataSourceSelectedRowIndex();
			DataTable dtMachine = grd_Devices.DataSourceTable();
			foreach (int row in listDataSourceSelectedRowIndex)
			{
				int num = Function.ParseInt(dtMachine.Rows[row]["MachineID"]);
				string name = Function.ToString(dtMachine.Rows[row]["MachineName"]);
				string ip = Function.ToString(dtMachine.Rows[row]["IPAddress"]);
				int port = Function.ParseInt(dtMachine.Rows[row]["TCPPort"]);
				if (num < 0)
				{
					continue;
				}
				DataTable dataTable = await new KJtechManipulator().GetLogDataTable(num, ip, port, fromDate, toDate);
				if (!dataTable.IsNotNull())
				{
					WinCommons.Error(Enums.MessageCode.None, name, "Log data empty!");
					continue;
				}
				EnumerableRowCollection<DataRow> source = from t in dataTable.AsEnumerable()
					where Function.ParseDateTimes(t["ClockDate"]).Date >= fromDate.Date && Function.ParseDateTimes(t["ClockDate"]).Date <= toDate.Date
					select t;
				if (source.Any())
				{
					if (!dt.IsNotNull())
					{
						dt = source.CopyToDataTable();
					}
					else
					{
						dt.Merge(source.CopyToDataTable(), preserveChanges: true);
					}
					gv_Devices.SetRowCellValue(row, "Upload", "S");
				}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_BiometricDataProcess));
		this.repositoryItemLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repositoryItemLookUpEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repositoryItemLookUpEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.grd_Devices = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Devices = new Apzon.Controls.Grid.GridViewEx();
		this.gc_MachineName = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_MachineNo = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Upload = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_UpStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Extract = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ExtStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btn_LoadFromDevice = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Extract = new DevExpress.XtraEditors.SimpleButton();
		this.separatorControl1 = new DevExpress.XtraEditors.SeparatorControl();
		this.lbl_ValidFrom = new Apzon.Controls.Controls.LabelLine();
		this.lbl_ValidTo = new Apzon.Controls.Controls.LabelLine();
		this.grd_DataLog = new Apzon.Controls.Grid.GridControlEx();
		this.gv_DataLog = new Apzon.Controls.Grid.GridViewEx();
		this.dataTable1 = new System.Data.DataTable();
		this.dataTable2 = new System.Data.DataTable();
		this.date_ValidTo = new Apzon.Controls.Editor.DateEditEx();
		this.date_ValidFrom = new Apzon.Controls.Editor.DateEditEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Devices).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Devices).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MachineNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UpStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ExtStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.separatorControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_DataLog).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_DataLog).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataTable1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataTable2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemLookUpEdit1.AutoHeight = false;
		this.repositoryItemLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit1.Name = "repositoryItemLookUpEdit1";
		this.repositoryItemLookUpEdit2.AutoHeight = false;
		this.repositoryItemLookUpEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit2.Name = "repositoryItemLookUpEdit2";
		this.repositoryItemLookUpEdit3.AutoHeight = false;
		this.repositoryItemLookUpEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit3.Name = "repositoryItemLookUpEdit3";
		this.grd_Devices.AllowPasteExcel = false;
		this.grd_Devices.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Devices.Location = new System.Drawing.Point(44, 79);
		this.grd_Devices.MainView = this.gv_Devices;
		this.grd_Devices.Name = "grd_Devices";
		this.grd_Devices.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_MachineNo, this.rep_UpStatus, this.rep_ExtStatus });
		this.grd_Devices.Size = new System.Drawing.Size(386, 248);
		this.grd_Devices.TabIndex = 0;
		this.grd_Devices.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Devices });
		this.gv_Devices.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Devices.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Devices.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Devices.ColumnKey = null;
		this.gv_Devices.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gc_MachineName, this.gc_Upload, this.gc_Extract });
		this.gv_Devices.DisableFormSetting = false;
		this.gv_Devices.DisplayException = false;
		this.gv_Devices.FieldCurrency = null;
		this.gv_Devices.FieldRate = null;
		this.gv_Devices.FormId = null;
		this.gv_Devices.GridControl = this.grd_Devices;
		this.gv_Devices.IndicatorWidth = 35;
		this.gv_Devices.IsApzTable = true;
		this.gv_Devices.IsInsertNewRow = false;
		this.gv_Devices.IsLoadingData = false;
		this.gv_Devices.ItemId = null;
		this.gv_Devices.LstColumnAllowBatch = null;
		this.gv_Devices.LstColumnNotAllowBatch = null;
		this.gv_Devices.LstColumnNotEditable = null;
		this.gv_Devices.LstColumNotSetting = null;
		this.gv_Devices.Name = "gv_Devices";
		this.gv_Devices.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Devices.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_Devices.OptionsPrint.AutoWidth = false;
		this.gv_Devices.OptionsPrint.UsePrintStyles = false;
		this.gv_Devices.OptionsSelection.MultiSelect = true;
		this.gv_Devices.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Devices.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Devices.OptionsView.ColumnAutoWidth = false;
		this.gv_Devices.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Devices.OptionsView.ShowGroupedColumns = true;
		this.gv_Devices.OptionsView.ShowGroupPanel = false;
		this.gv_Devices.UseBatchMod = false;
		this.gc_MachineName.Caption = "Machine Name";
		this.gc_MachineName.ColumnEdit = this.rep_MachineNo;
		this.gc_MachineName.CurrencyDateField = null;
		this.gc_MachineName.CurrencyField = null;
		this.gc_MachineName.CurrencyFixed = null;
		this.gc_MachineName.CurrencyFixedRate = 0.0;
		this.gc_MachineName.CurrencyRateField = null;
		this.gc_MachineName.FieldName = "MachineID";
		this.gc_MachineName.Name = "gc_MachineName";
		this.gc_MachineName.OptionsColumn.ReadOnly = true;
		this.gc_MachineName.PrimaryKey = false;
		this.gc_MachineName.UnitField = null;
		this.gc_MachineName.Visible = true;
		this.gc_MachineName.VisibleIndex = 1;
		this.gc_MachineName.Width = 90;
		this.rep_MachineNo.AutoHeight = false;
		this.rep_MachineNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_MachineNo.Name = "rep_MachineNo";
		this.gc_Upload.Caption = "Upload";
		this.gc_Upload.ColumnEdit = this.rep_UpStatus;
		this.gc_Upload.CurrencyDateField = null;
		this.gc_Upload.CurrencyField = null;
		this.gc_Upload.CurrencyFixed = null;
		this.gc_Upload.CurrencyFixedRate = 0.0;
		this.gc_Upload.CurrencyRateField = null;
		this.gc_Upload.FieldName = "Upload";
		this.gc_Upload.Name = "gc_Upload";
		this.gc_Upload.OptionsColumn.ReadOnly = true;
		this.gc_Upload.PrimaryKey = false;
		this.gc_Upload.UnitField = null;
		this.gc_Upload.Visible = true;
		this.gc_Upload.VisibleIndex = 2;
		this.rep_UpStatus.AutoHeight = false;
		this.rep_UpStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_UpStatus.Name = "rep_UpStatus";
		this.gc_Extract.Caption = "Extract";
		this.gc_Extract.ColumnEdit = this.rep_ExtStatus;
		this.gc_Extract.CurrencyDateField = null;
		this.gc_Extract.CurrencyField = null;
		this.gc_Extract.CurrencyFixed = null;
		this.gc_Extract.CurrencyFixedRate = 0.0;
		this.gc_Extract.CurrencyRateField = null;
		this.gc_Extract.FieldName = "Extract";
		this.gc_Extract.Name = "gc_Extract";
		this.gc_Extract.OptionsColumn.ReadOnly = true;
		this.gc_Extract.PrimaryKey = false;
		this.gc_Extract.UnitField = null;
		this.gc_Extract.Visible = true;
		this.gc_Extract.VisibleIndex = 3;
		this.rep_ExtStatus.AutoHeight = false;
		this.rep_ExtStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ExtStatus.Name = "rep_ExtStatus";
		this.btn_LoadFromDevice.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_LoadFromDevice.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_LoadFromDevice.ImageOptions.Image");
		this.btn_LoadFromDevice.Location = new System.Drawing.Point(12, 338);
		this.btn_LoadFromDevice.Name = "btn_LoadFromDevice";
		this.btn_LoadFromDevice.Size = new System.Drawing.Size(189, 23);
		this.btn_LoadFromDevice.TabIndex = 93;
		this.btn_LoadFromDevice.Text = "Load From Device";
		this.btn_LoadFromDevice.Click += new System.EventHandler(this.btn_LoadFromDevice_Click);
		this.btn_Extract.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Extract.Location = new System.Drawing.Point(280, 338);
		this.btn_Extract.Name = "btn_Extract";
		this.btn_Extract.Size = new System.Drawing.Size(189, 23);
		this.btn_Extract.TabIndex = 94;
		this.btn_Extract.Text = "Extract to check time";
		this.btn_Extract.Click += new System.EventHandler(this.btn_Extract_Click);
		this.separatorControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.separatorControl1.Location = new System.Drawing.Point(2, 50);
		this.separatorControl1.Name = "separatorControl1";
		this.separatorControl1.Size = new System.Drawing.Size(477, 23);
		this.separatorControl1.TabIndex = 95;
		this.lbl_ValidFrom.AllowDrop = true;
		this.lbl_ValidFrom.EditControl = null;
		this.lbl_ValidFrom.EditValue = "From Date";
		this.lbl_ValidFrom.FieldName = null;
		this.lbl_ValidFrom.HideWhenEmpty = false;
		this.lbl_ValidFrom.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ValidFrom.Location = new System.Drawing.Point(12, 27);
		this.lbl_ValidFrom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_ValidFrom.Name = "lbl_ValidFrom";
		this.lbl_ValidFrom.ReadOnly = false;
		this.lbl_ValidFrom.SettingInVisible = false;
		this.lbl_ValidFrom.SettingReadonly = false;
		this.lbl_ValidFrom.ShowUnderLine = true;
		this.lbl_ValidFrom.Size = new System.Drawing.Size(80, 20);
		this.lbl_ValidFrom.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ValidFrom.TabIndex = 97;
		this.lbl_ValidFrom.Utext = "From Date";
		this.lbl_ValidTo.AllowDrop = true;
		this.lbl_ValidTo.EditControl = null;
		this.lbl_ValidTo.EditValue = "To Date";
		this.lbl_ValidTo.FieldName = null;
		this.lbl_ValidTo.HideWhenEmpty = false;
		this.lbl_ValidTo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ValidTo.Location = new System.Drawing.Point(249, 27);
		this.lbl_ValidTo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_ValidTo.Name = "lbl_ValidTo";
		this.lbl_ValidTo.ReadOnly = false;
		this.lbl_ValidTo.SettingInVisible = false;
		this.lbl_ValidTo.SettingReadonly = false;
		this.lbl_ValidTo.ShowUnderLine = true;
		this.lbl_ValidTo.Size = new System.Drawing.Size(80, 20);
		this.lbl_ValidTo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ValidTo.TabIndex = 99;
		this.lbl_ValidTo.Utext = "To Date";
		this.grd_DataLog.AllowPasteExcel = false;
		this.grd_DataLog.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.grd_DataLog.Location = new System.Drawing.Point(440, 79);
		this.grd_DataLog.MainView = this.gv_DataLog;
		this.grd_DataLog.Name = "grd_DataLog";
		this.grd_DataLog.Size = new System.Drawing.Size(29, 106);
		this.grd_DataLog.TabIndex = 100;
		this.grd_DataLog.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_DataLog });
		this.grd_DataLog.Visible = false;
		this.gv_DataLog.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_DataLog.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_DataLog.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_DataLog.ColumnKey = null;
		this.gv_DataLog.DisableFormSetting = false;
		this.gv_DataLog.DisplayException = false;
		this.gv_DataLog.FieldCurrency = null;
		this.gv_DataLog.FieldRate = null;
		this.gv_DataLog.FormId = null;
		this.gv_DataLog.GridControl = this.grd_DataLog;
		this.gv_DataLog.IndicatorWidth = 35;
		this.gv_DataLog.IsApzTable = false;
		this.gv_DataLog.IsInsertNewRow = false;
		this.gv_DataLog.IsLoadingData = false;
		this.gv_DataLog.ItemId = null;
		this.gv_DataLog.LstColumnAllowBatch = null;
		this.gv_DataLog.LstColumnNotAllowBatch = null;
		this.gv_DataLog.LstColumnNotEditable = null;
		this.gv_DataLog.LstColumNotSetting = null;
		this.gv_DataLog.Name = "gv_DataLog";
		this.gv_DataLog.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_DataLog.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_DataLog.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_DataLog.OptionsPrint.AutoWidth = false;
		this.gv_DataLog.OptionsPrint.UsePrintStyles = false;
		this.gv_DataLog.OptionsView.BestFitMaxRowCount = 100;
		this.gv_DataLog.OptionsView.ColumnAutoWidth = false;
		this.gv_DataLog.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_DataLog.OptionsView.ShowGroupedColumns = true;
		this.gv_DataLog.OptionsView.ShowGroupPanel = false;
		this.gv_DataLog.UseBatchMod = false;
		this.dataTable1.Namespace = "";
		this.dataTable1.TableName = "Table1";
		this.dataTable2.Namespace = "";
		this.dataTable2.TableName = "Table1";
		this.date_ValidTo.EditControl = null;
		this.date_ValidTo.EditValue = null;
		this.date_ValidTo.FieldName = null;
		this.date_ValidTo.Location = new System.Drawing.Point(330, 24);
		this.date_ValidTo.Name = "date_ValidTo";
		this.date_ValidTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidTo.Properties.IsSearchControl = false;
		this.date_ValidTo.SettingInVisible = false;
		this.date_ValidTo.SettingReadonly = false;
		this.date_ValidTo.Size = new System.Drawing.Size(139, 20);
		this.date_ValidTo.TabIndex = 101;
		this.date_ValidFrom.EditControl = null;
		this.date_ValidFrom.EditValue = null;
		this.date_ValidFrom.FieldName = null;
		this.date_ValidFrom.Location = new System.Drawing.Point(87, 24);
		this.date_ValidFrom.Name = "date_ValidFrom";
		this.date_ValidFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidFrom.Properties.IsSearchControl = false;
		this.date_ValidFrom.SettingInVisible = false;
		this.date_ValidFrom.SettingReadonly = false;
		this.date_ValidFrom.Size = new System.Drawing.Size(139, 20);
		this.date_ValidFrom.TabIndex = 102;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(481, 373);
		base.Controls.Add(this.date_ValidFrom);
		base.Controls.Add(this.date_ValidTo);
		base.Controls.Add(this.grd_DataLog);
		base.Controls.Add(this.lbl_ValidTo);
		base.Controls.Add(this.lbl_ValidFrom);
		base.Controls.Add(this.separatorControl1);
		base.Controls.Add(this.btn_Extract);
		base.Controls.Add(this.btn_LoadFromDevice);
		base.Controls.Add(this.grd_Devices);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_BiometricDataProcess.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_BiometricDataProcess";
		this.Text = "Biometric Data Log";
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Devices).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Devices).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MachineNo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UpStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ExtStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.separatorControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_DataLog).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_DataLog).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataTable1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataTable2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
