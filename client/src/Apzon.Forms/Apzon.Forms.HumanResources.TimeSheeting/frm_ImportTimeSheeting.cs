using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_ImportTimeSheeting : XtraFormEx
{
	private const string FormController = "TimeSheeting";

	private bool isCancel;

	private bool _isLoading;

	private bool autoLoadingTemplate;

	private Dictionary<string, Tuple<DateTime, DateTime>> dictionPeriod = new Dictionary<string, Tuple<DateTime, DateTime>>();

	private DataTable dtAPZ_OHWP;

	private DataTable dtOUBR;

	private DataTable dtImport;

	private List<int> empList = new List<int>();

	private List<string> workcodeList = new List<string>();

	private List<string> requiredFields = new List<string> { "EmpId", "WorkDay" };

	private string formatDateTime = "";

	private Dictionary<string, string> dictionaryTarget;

	private Dictionary<string, string> tempDictionary;

	private int numberOfRequiredFields = 3;

	private string tableName = "APZ_TSL1";

	private DataTable dtAPZ_OHOT;

	private List<string> listOTCode = new List<string>();

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public int BranchValue;

	public string PeriodValue = "";

	private IContainer components;

	public XtraTabControlEx xTab_MapField;

	public XtraTabPageEx xTPage_MappingRule;

	public GridControlEx grd_MappingRule;

	public GridViewEx gv_MappingRule;

	public GridColumnEx gridColumn5;

	public XtraTabPageEx xTPage_SourceData;

	public SimpleButtonEx btCancel;

	public SimpleButtonEx btnOK;

	public RepositoryItemLookUpEdit rep_luk_TargetField;

	public SimpleButtonEx btnImport;

	public GridColumnEx gridColumn6;

	public LookUpEditDefineNew luk_Period;

	public GridControlEx grd_SourceData;

	public GridViewEx gv_SourceData;

	public ButtonEditEx bte_Import;

	public TextEditEx txt_dateFormat;

	public SimpleButtonEx btnSaveTemp;

	public SimpleButtonEx btnLoadTemp;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private CheckEditEx chk_DateFormat;

	public LookUpEditDefineNew luk_Branch;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btnOK.Text = Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btnOK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_ImportTimeSheeting()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_TimeSheeting"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_TimeSheeting" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	public virtual async void frm_ImportTimeSheeting_Load(object sender, EventArgs e)
	{
		try
		{
			bte_Import.Properties.Buttons[0].Visible = false;
			numberOfRequiredFields = requiredFields.Count;
			luk_Branch.Properties.DefineNewForm = typeof(frm_HRBranches);
			luk_Branch.Properties.RefreshMethod = RefreshBranch;
			luk_Period.Properties.DefineNewForm = typeof(frm_WorkPeriod);
			luk_Period.Properties.RefreshMethodAsync = SetupPeriod_LookUp;
			_isLoading = true;
			Task task = SetupPeriod_LookUp();
			Task task2 = LoadEmployees();
			Task workCode = GetWorkCode();
			Task task3 = SetupBranch_LookUp();
			SetupTarget_LookUp();
			await Task.WhenAll(task, task2, workCode, task3);
			if (BranchValue != 0)
			{
				luk_Branch.EditValue = Function.ParseShort(BranchValue);
			}
			if (PeriodValue != "")
			{
				luk_Period.EditValue = PeriodValue;
			}
			PeriodFilter();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void btCancel_Click(object sender, EventArgs e)
	{
		isCancel = true;
		Close();
	}

	public virtual void btnOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			btnOK.Enabled = false;
			UpdateData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			btnOK.Enabled = true;
		}
	}

	public virtual string SaveData(DataSet dt)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "TimeSheeting");
			string text = httpClientHelper.Post("SaveImportTimeSheeting", dt);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.ImportSuccess, "");
				return "success";
			}
			WinCommons.Error(Enums.MessageCode.None, text);
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	public virtual void UpdateData()
	{
		WinCommons._OpenProcessing();
		try
		{
			_isLoading = true;
			DataSet dataSet = MappingData();
			if (!dataSet.IsNotNull() || SaveData(dataSet) != "success")
			{
				return;
			}
			FormMode = Enums.FormViewModeType.View;
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public virtual DataSet MappingData()
	{
		try
		{
			string text = Function.ToString(luk_Branch.EditValue);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.PleaseChooseABranch, "");
				return new DataSet();
			}
			string text2 = Function.ToString(luk_Period.EditValue);
			if (string.IsNullOrEmpty(text2))
			{
				WinCommons.Error(Enums.MessageCode.PleaseSelectAPeriod, "");
				return new DataSet();
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(txt_dateFormat.Text))
			{
				formatDateTime = txt_dateFormat.Text;
			}
			DataTable source = grd_MappingRule.DataSource as DataTable;
			(((RepositoryItemLookUpEdit)gv_MappingRule.Columns[1].ColumnEdit).DataSource as List<KeyValuePair<string, string>>).AsEnumerable().ToDictionary((KeyValuePair<string, string> s) => s.Key, (KeyValuePair<string, string> s) => s.Value);
			foreach (string item in requiredFields)
			{
				if (!source.AsEnumerable().Any((DataRow s) => Function.ToString(s["Target"]) == item))
				{
					WinCommons.Error(Enums.MessageCode.PleaseMapSourceFieldToThisTargetField, "");
					return new DataSet();
				}
			}
			int num = (from s in source.AsEnumerable()
				where Function.ToString(s["Target"]) != ""
				select s["Target"]).Count();
			int num2 = (from s in source.AsEnumerable()
				where Function.ToString(s["Target"]) != ""
				select s["Target"]).Distinct().Count();
			if (num > num2)
			{
				WinCommons.Error(Enums.MessageCode.ATargetFieldCanOnlyBeMapTo1SourceField, "");
				return new DataSet();
			}
			foreach (DataRow item4 in (grd_MappingRule.DataSource as DataTable).AsEnumerable())
			{
				if (Function.ToString(item4["Target"]) != "")
				{
					dictionary.Add(Function.ToString(item4["Target"]), Function.ToString(item4["Source"]));
				}
			}
			DateTime item2 = dictionPeriod[text2 + "_" + text].Item1;
			DateTime item3 = dictionPeriod[text2 + "_" + text].Item2;
			DataTable dataTable = new DataTable();
			dataTable = WinFuntions.GetTableStructure("APZ_TSL1");
			DataTable dataTable2 = new DataTable();
			dataTable2 = WinFuntions.GetTableStructure("APZ_TSH1");
			dataTable2.AcceptChanges();
			grd_SourceData.DataSourceTable.AcceptChanges();
			foreach (DataRow item5 in grd_SourceData.DataSourceTable.AsEnumerable())
			{
				DataRow dataRow = dataTable.NewRow();
				DataRow dataRow2 = dataTable2.NewRow();
				DateTime dateTime = default;
				foreach (DataColumn column in dataRow.Table.Columns)
				{
					if (!dictionary.ContainsKey(column.ColumnName))
					{
						continue;
					}
					string text3 = dictionary[column.ColumnName];
					if (requiredFields.Contains(column.ColumnName) && item5[text3] == DBNull.Value)
					{
						WinCommons.Error(Enums.MessageCode.NullValueDetected, "", text3);
						return new DataSet();
					}
					switch (column.DataType.ToString().ToLower())
					{
					case "system.string":
						dataRow[column] = Function.ToString(item5[text3]);
						break;
					case "system.int16":
						dataRow[column] = Function.ParseShort(item5[text3]);
						break;
					case "system.int32":
						if (column.ColumnName == "InTime" || column.ColumnName == "OutTime")
						{
							string text4 = Function.ToString(item5[text3]);
							if (!string.IsNullOrEmpty(text4))
							{
								dataRow[column] = Function.ParseTimeSpanToInt(TimeSpan.Parse(text4));
							}
						}
						else
						{
							dataRow[column] = Function.ParseInt(item5[text3]);
						}
						break;
					case "system.int64":
						dataRow[column] = Function.ParseLong(item5[text3]);
						break;
					case "system.datetime":
						try
						{
							if (chk_DateFormat.Checked && !string.IsNullOrEmpty(formatDateTime))
							{
								DateTime? dateTime2 = Function.ParseDateTime(Function.ToString(item5[text3]), formatDateTime);
								dateTime = dateTime2.Value;
								dataRow[column] = dateTime2;
								break;
							}
							DateTime dateTime3 = Function.ParseDateTimes(item5[text3]);
							dataRow[column] = dateTime3;
							dateTime = dateTime3;
							if (dateTime3.Date == DateTime.Now.Date && dateTime3.Minute == DateTime.Now.Minute)
							{
								throw new Exception();
							}
						}
						catch
						{
							try
							{
								DateTime dateTime4 = DateTime.FromOADate(Function.ParseDouble(item5[text3]));
								dataRow[column] = dateTime4;
								dateTime = dateTime4;
								if (dateTime4.Year <= 1950)
								{
									throw new Exception();
								}
							}
							catch
							{
								WinCommons.Error(Enums.MessageCode.InvalidDateFormat, "", Function.ToString(item5[text3]));
								return new DataSet();
							}
						}
						break;
					case "system.decimal":
					case "system.double":
					case "system.float":
						dataRow[column] = Function.ParseDouble(item5[text3]);
						break;
					default:
						dataRow[column] = Function.ToString(item5[text3]);
						break;
					}
				}
				if (!empList.Contains(Function.ParseInt(dataRow["EmpId"])))
				{
					WinCommons.Error(Enums.MessageCode.EmployeeIsInvalid, "", Function.ToString(dataRow["EmpId"]));
					return new DataSet();
				}
				string text5 = Function.ToString(dataRow["WorkCode"]);
				List<string> list = text5.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
				foreach (string item6 in list)
				{
					if (!string.IsNullOrEmpty(item6) && !workcodeList.Contains(item6))
					{
						WinCommons.Error(Enums.MessageCode.WorkCodeIsInvalid, "", Function.ToString(dataRow["WorkCode"]));
						return new DataSet();
					}
				}
				if (!(dateTime.Date >= item2.Date) || !(dateTime.Date <= item3.Date))
				{
					WinCommons.Error(Enums.MessageCode.DateIsNotInSelectedPeriod, "", dateTime.ToString(GlobalData.DateTimeFormat), text2);
					return new DataSet();
				}
				dataRow["Period"] = text2;
				if (!string.IsNullOrEmpty(text5))
				{
					dataRow["WorkCode"] = string.Join(",", list);
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
				}
				dataRow2["Period"] = text2;
				dataRow2["EmpId"] = Function.ParseInt(dataRow["EmpId"]);
				dataRow2["WorkDay"] = dateTime;
				dataRow2["FreeTxt"] = Function.ToString(dataRow["FreeTxt"]);
				foreach (string key in dictionary.Keys)
				{
					if (listOTCode.Contains(key))
					{
						string columnName = dictionary[key];
						double num3 = Function.ParseDouble(item5[columnName]);
						if (num3 != 0.0)
						{
							dataRow2["OvertimeType"] = key;
							dataRow2["OvertimeHour"] = num3;
							DataRow dataRow3 = dataTable2.NewRow();
							dataRow3.ItemArray = dataRow2.ItemArray;
							dataTable2.Rows.Add(dataRow3);
							dataTable2.AcceptChanges();
						}
					}
				}
			}
			DataSet dataSet = new DataSet();
			dataTable.TableName = "APZ_TSL1";
			dataTable2.TableName = "APZ_TSH1";
			dataSet.Tables.Add(dataTable);
			dataSet.Tables.Add(dataTable2);
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataSet();
		}
	}

	public virtual async void gridView1_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		if (!_isLoading)
		{
			CheckSaveState();
		}
	}

	private void CheckSaveState()
	{
		if (!_isLoading)
		{
			btnOK.Text = "Import";
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.No)
			{
				frm_ImportTimeSheeting_Load(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_Absence_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (btnOK.Text == "Import" && !isCancel)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
				{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					UpdateData();
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual async Task<DataTable> GetPeriod()
	{
		try
		{
			DataTable dataTable = await _httpClient.WorkPeriod.GetListWorkPeriod();
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("APZ_OHWP");
			}
			DataView defaultView = dataTable.DefaultView;
			defaultView.RowFilter = "PeriodStatus='U'";
			return defaultView.ToTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public virtual async Task<DataTable> GetBranch()
	{
		try
		{
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("OUBR");
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public virtual async Task GetWorkCode()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			DataTable dataTable = await _help.GetAsync("GetWorkCode", new
			{
				type = 2
			});
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("APZ_OHWC");
			}
			workcodeList.Clear();
			foreach (DataRow item in dataTable.AsEnumerable())
			{
				workcodeList.Add(Function.ToString(item["Code"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual async Task LoadEmployees()
	{
		try
		{
			DataTable dataTable = await _httpClient.Commons.GetAllEmployees();
			if (!dataTable.IsNotNull())
			{
				return;
			}
			empList.Clear();
			foreach (DataRow item in dataTable.AsEnumerable())
			{
				empList.Add(Function.ParseInt(item["EmpId"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual async Task SetupPeriod_LookUp()
	{
		try
		{
			dtAPZ_OHWP = await GetPeriod();
			if (!dtAPZ_OHWP.IsNotNull())
			{
				return;
			}
			dictionPeriod.Clear();
			foreach (DataRow item in dtAPZ_OHWP.AsEnumerable())
			{
				dictionPeriod.Add(Function.ToString(item["PeriodCode"]) + "_" + Function.ToString(item["Branch"]), new Tuple<DateTime, DateTime>(Function.ParseDateTimes(item["FromDate"]), Function.ParseDateTimes(item["ToDate"])));
			}
			DataTable dataTable = dtAPZ_OHWP.Copy();
			DataRow dataRow = dataTable.NewRow();
			dataRow["PeriodCode"] = "";
			dataRow["PeriodName"] = "";
			dataTable.Rows.InsertAt(dataRow, 0);
			dataTable.AcceptChanges();
			luk_Period.Properties.NullText = "";
			luk_Period.Properties.ValueMember = "PeriodCode";
			luk_Period.Properties.DisplayMember = "PeriodName";
			luk_Period.Properties.ShowHeader = false;
			luk_Period.Properties.DropDownRows = dataTable.Rows.Count;
			luk_Period.Properties.DataSource = dataTable.Copy();
			luk_Period.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual async Task SetupBranch_LookUp()
	{
		try
		{
			dtOUBR = await GetBranch();
			if (dtOUBR.IsNotNull())
			{
				DataTable dataTable = dtOUBR.Copy();
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = 0;
				dataRow["Name"] = "";
				dataTable.Rows.InsertAt(dataRow, 0);
				dataTable.AcceptChanges();
				luk_Branch.Properties.NullText = "";
				luk_Branch.Properties.ValueMember = "Code";
				luk_Branch.Properties.DisplayMember = "Name";
				luk_Branch.Properties.ShowHeader = false;
				luk_Branch.Properties.DropDownRows = dataTable.Rows.Count;
				luk_Branch.Properties.DataSource = dataTable.Copy();
				luk_Branch.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void RefreshBranch()
	{
		await SetupBranch_LookUp();
	}

	public virtual async void SetupTarget_LookUp()
	{
		try
		{
			dtAPZ_OHOT = await GetOvertimeType();
			dictionaryTarget = new Dictionary<string, string>
			{
				{ " ", "" },
				{ "EmpId", "Employee No." },
				{ "WorkDay", "Work Day" },
				{ "WorkCode", "Work Code" },
				{ "InTime", "In Time [Optional]" },
				{ "OutTime", "Out Time [Optional]" },
				{ "FreeTxt", "Free Text [Optional]" }
			};
			listOTCode = new List<string>();
			foreach (DataRow item in dtAPZ_OHOT.AsEnumerable())
			{
				string text = Function.ToString(item["Code"]);
				string value = Function.ToString(item["Name"]);
				dictionaryTarget.Add(text, value);
				listOTCode.Add(text);
			}
			tempDictionary = new Dictionary<string, string>(dictionaryTarget);
			rep_luk_TargetField.NullText = "";
			rep_luk_TargetField.ValueMember = "Key";
			rep_luk_TargetField.DisplayMember = "Value";
			rep_luk_TargetField.Columns["Key"].Visible = false;
			rep_luk_TargetField.ShowHeader = false;
			rep_luk_TargetField.DropDownRows = dictionaryTarget.Count();
			rep_luk_TargetField.DataSource = dictionaryTarget.ToList();
			rep_luk_TargetField.BestFitMode = BestFitMode.BestFitResizePopup;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected async Task<DataTable> GetOvertimeType()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			DataTable dataTable = await _help.GetAsync("GetOvertimeType");
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("APZ_OHOT");
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	public virtual void ImportFromExcel()
	{
		WinCommons._OpenProcessing();
		try
		{
			string text = Function.ToString(bte_Import.EditValue);
			DataTable dataTableFromExcel = ExcelHelper.GetDataTableFromExcel(text);
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("Source", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Target", typeof(string)));
			dataTable.AcceptChanges();
			if (!dataTableFromExcel.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.FileIsInvalidFormatHavingNoLine, "");
				return;
			}
			if (dataTableFromExcel.Columns.Count < numberOfRequiredFields)
			{
				WinCommons.Error(Enums.MessageCode.FileIsInvalidFormatNumberOfRequiredFieldNotMetWIthPath, "", text);
				return;
			}
			foreach (DataColumn column in dataTableFromExcel.Columns)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Source"] = column.Caption;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			grd_MappingRule.DataSource = dataTable;
			grd_MappingRule.RefreshDataSource();
			grd_SourceData.DataSource = dataTableFromExcel.Copy();
			gv_SourceData.PopulateColumns();
			dtImport = dataTableFromExcel.Copy();
			tempDictionary = new Dictionary<string, string>(dictionaryTarget);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public virtual void btnImport_Click(object sender, EventArgs e)
	{
		try
		{
			string filter = "Excels file (*.xlsx)|*.xlsx";
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = filter;
			openFileDialog.Multiselect = false;
			if (openFileDialog.ShowDialog() == DialogResult.OK && File.Exists(openFileDialog.FileName))
			{
				bte_Import.EditValue = openFileDialog.FileName;
				ImportFromExcel();
				autoLoadingTemplate = true;
				LoadTemplate();
				autoLoadingTemplate = false;
				CheckSaveState();
				AfterOpen(null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_Import_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			bte_Import.EditValue = "";
			grd_MappingRule.DataSource = null;
			grd_SourceData.DataSource = null;
			gv_SourceData.PopulateColumns();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bte_Import_TextChanged(object sender, EventArgs e)
	{
		if (Function.ToString(bte_Import.EditValue) == "")
		{
			bte_Import.Properties.Buttons[0].Visible = false;
		}
		else
		{
			bte_Import.Properties.Buttons[0].Visible = true;
		}
	}

	private void gv_MappingRule_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		CheckSaveState();
	}

	private void gv_MappingRule_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		CheckSaveState();
	}

	public virtual void rep_luk_TargetField_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			string text = Function.ToString(e.NewValue);
			string text2 = Function.ToString(e.OldValue);
			if (text != "")
			{
				tempDictionary.Remove(text);
			}
			if (text2 != "" && !tempDictionary.ContainsKey(text2))
			{
				tempDictionary.Add(text2, dictionaryTarget[text2]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_luk_TargetField_PopupFilter(object sender, PopupFilterEventArgs e)
	{
		try
		{
			if (tempDictionary.Count != 0)
			{
				e.Criteria = new InOperator("Key", tempDictionary.Keys);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void luk_Branch_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				PeriodFilter();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void PeriodFilter()
	{
		int num = Function.ParseInt(luk_Branch.EditValue);
		DataTable table = dtAPZ_OHWP.Copy();
		DataView dataView = table.AsDataView();
		dataView.RowFilter = "Branch= " + num;
		table = dataView.ToTable();
		luk_Period.Properties.DataSource = table;
		luk_Period.Properties.DropDownRows = table.Rows.Count;
		luk_Period.Properties.ValueMember = "PeriodCode";
		luk_Period.Properties.DisplayMember = "PeriodName";
		if (luk_Period.Text == "")
		{
			luk_Period.EditValue = "";
		}
	}

	public virtual void SaveTemplate()
	{
		try
		{
			if (gv_MappingRule.DataRowCount <= 0)
			{
				return;
			}
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OTSI");
			foreach (DataRow item in grd_MappingRule.DataSourceTable.AsEnumerable())
			{
				DataRow dataRow = tableStructure.NewRow();
				dataRow["SourceField"] = Function.ToString(item["Source"]);
				dataRow["TargetField"] = Function.ToString(item["Target"]);
				dataRow["TableName"] = tableName;
				dataRow["UserSign"] = GlobalData.UserName;
				tableStructure.Rows.Add(dataRow);
				tableStructure.AcceptChanges();
			}
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "TimeSheeting");
			string text = httpClientHelper.Post("SaveImportTimeSheetingTemplate", tableStructure);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnSaveTemp_Click(object sender, EventArgs e)
	{
		try
		{
			SaveTemplate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void LoadTemplate()
	{
		try
		{
			if (gv_MappingRule.DataRowCount <= 0)
			{
				return;
			}
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			DataTable dataTable = httpClientHelper.Get("LoadImportTimeSheetingTemplate", new
			{
				tableName = tableName,
				userSign = GlobalData.UserName
			});
			if (dataTable.IsNotNull())
			{
				Dictionary<string, string> dictionary = dataTable.AsEnumerable().ToDictionary((DataRow s) => Function.ToString(s["SourceField"]), (DataRow s) => Function.ToString(s["TargetField"]));
				tempDictionary = new Dictionary<string, string>(dictionaryTarget);
				{
					foreach (DataRow item in grd_MappingRule.DataSourceTable.AsEnumerable())
					{
						string key = Function.ToString(item["Source"]);
						if (dictionary.ContainsKey(key))
						{
							item["Target"] = dictionary[key];
							if (tempDictionary.ContainsKey(dictionary[key]) && dictionary[key] != "")
							{
								tempDictionary.Remove(dictionary[key]);
							}
						}
					}
					return;
				}
			}
			if (!autoLoadingTemplate)
			{
				WinCommons.Error(Enums.MessageCode.NoTemplateWereFound, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnLoadTemp_Click(object sender, EventArgs e)
	{
		try
		{
			LoadTemplate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void xTab_MapField_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			if (e.Page.Name == "xTPage_MappingRule")
			{
				btnSaveTemp.Visible = true;
				btnLoadTemp.Visible = true;
			}
			else
			{
				btnSaveTemp.Visible = false;
				btnLoadTemp.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_DateFormat_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (chk_DateFormat.Checked)
			{
				txt_dateFormat.Visible = true;
			}
			else
			{
				txt_dateFormat.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_ImportTimeSheeting));
		this.rep_luk_TargetField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.xTab_MapField = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xTPage_MappingRule = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_MappingRule = new Apzon.Controls.Grid.GridControlEx();
		this.gv_MappingRule = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.xTPage_SourceData = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_SourceData = new Apzon.Controls.Grid.GridControlEx();
		this.gv_SourceData = new Apzon.Controls.Grid.GridViewEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnImport = new Apzon.Controls.Controls.SimpleButtonEx();
		this.luk_Period = new Apzon.Forms.UserControls.LookUpEditDefineNew();
		this.bte_Import = new Apzon.Controls.Editor.ButtonEditEx();
		this.txt_dateFormat = new Apzon.Controls.Editor.TextEditEx();
		this.btnSaveTemp = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnLoadTemp = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.chk_DateFormat = new Apzon.Controls.Editor.CheckEditEx();
		this.luk_Branch = new Apzon.Forms.UserControls.LookUpEditDefineNew();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_TargetField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xTab_MapField).BeginInit();
		this.xTab_MapField.SuspendLayout();
		this.xTPage_MappingRule.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_MappingRule).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_MappingRule).BeginInit();
		this.xTPage_SourceData.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_SourceData).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SourceData).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Period.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_Import.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_dateFormat.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_DateFormat.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).BeginInit();
		base.SuspendLayout();
		this.rep_luk_TargetField.AutoHeight = false;
		this.rep_luk_TargetField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_TargetField.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Value"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Key", "Key")
		});
		this.rep_luk_TargetField.DisplayMember = "Value";
		this.rep_luk_TargetField.Name = "rep_luk_TargetField";
		this.rep_luk_TargetField.NullText = "";
		this.rep_luk_TargetField.ValueMember = "Key";
		this.rep_luk_TargetField.PopupFilter += new DevExpress.XtraEditors.Controls.PopupFilterEventHandler(this.rep_luk_TargetField_PopupFilter);
		this.rep_luk_TargetField.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rep_luk_TargetField_EditValueChanging);
		this.xTab_MapField.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xTab_MapField.IsSearchControl = false;
		this.xTab_MapField.Location = new System.Drawing.Point(0, 74);
		this.xTab_MapField.Name = "xTab_MapField";
		this.xTab_MapField.SelectedTabPage = this.xTPage_MappingRule;
		this.xTab_MapField.Size = new System.Drawing.Size(547, 314);
		this.xTab_MapField.TabIndex = 1;
		this.xTab_MapField.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xTPage_MappingRule, this.xTPage_SourceData });
		this.xTab_MapField.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xTab_MapField_SelectedPageChanged);
		this.xTPage_MappingRule.Controls.Add(this.grd_MappingRule);
		this.xTPage_MappingRule.EditControl = null;
		this.xTPage_MappingRule.EditValue = "Mapping Rules";
		this.xTPage_MappingRule.FieldName = null;
		this.xTPage_MappingRule.IsSearchControl = false;
		this.xTPage_MappingRule.Name = "xTPage_MappingRule";
		this.xTPage_MappingRule.ReadOnly = false;
		this.xTPage_MappingRule.SettingInVisible = false;
		this.xTPage_MappingRule.SettingReadonly = false;
		this.xTPage_MappingRule.Size = new System.Drawing.Size(545, 289);
		this.xTPage_MappingRule.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xTPage_MappingRule.Text = "Mapping Rules";
		this.grd_MappingRule.AllowPasteExcel = true;
		this.grd_MappingRule.ClipboardData = null;
		this.grd_MappingRule.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_MappingRule.Location = new System.Drawing.Point(0, 0);
		this.grd_MappingRule.MainView = this.gv_MappingRule;
		this.grd_MappingRule.Name = "grd_MappingRule";
		this.grd_MappingRule.Size = new System.Drawing.Size(545, 289);
		this.grd_MappingRule.TabIndex = 5;
		this.grd_MappingRule.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_MappingRule });
		this.gv_MappingRule.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_MappingRule.ColumnKey = null;
		this.gv_MappingRule.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumn5, this.gridColumn6 });
		this.gv_MappingRule.DisableFormSetting = false;
		this.gv_MappingRule.DisplayException = false;
		this.gv_MappingRule.EditColumn = null;
		this.gv_MappingRule.FieldCurrency = null;
		this.gv_MappingRule.FieldRate = null;
		this.gv_MappingRule.FormId = "ImportTimeSheeting";
		this.gv_MappingRule.GridControl = this.grd_MappingRule;
		this.gv_MappingRule.IgnoreFilter = false;
		this.gv_MappingRule.IndexEditRow = -1;
		this.gv_MappingRule.IndicatorWidth = 40;
		this.gv_MappingRule.IsApzTable = false;
		this.gv_MappingRule.IsInsertNewRow = false;
		this.gv_MappingRule.IsLoadingData = false;
		this.gv_MappingRule.IsMappingContextAction = false;
		this.gv_MappingRule.ItemId = "OMAPPING";
		this.gv_MappingRule.LstColumnAllowBatch = null;
		this.gv_MappingRule.LstColumnNotAllowBatch = null;
		this.gv_MappingRule.LstColumnNotEditable = null;
		this.gv_MappingRule.LstColumNotSetting = null;
		this.gv_MappingRule.Name = "gv_MappingRule";
		this.gv_MappingRule.OptionsView.ShowGroupPanel = false;
		this.gv_MappingRule.UseBatchMod = false;
		this.gv_MappingRule.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_MappingRule_CellValueChanged);
		this.gv_MappingRule.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_MappingRule_CellValueChanging);
		this.gridColumn5.Caption = "Source Fields";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "Source";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsColumn.AllowEdit = false;
		this.gridColumn5.OptionsColumn.ReadOnly = true;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 0;
		this.gridColumn6.Caption = "Target Fields";
		this.gridColumn6.ColumnEdit = this.rep_luk_TargetField;
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "Target";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 1;
		this.xTPage_SourceData.Controls.Add(this.grd_SourceData);
		this.xTPage_SourceData.EditControl = null;
		this.xTPage_SourceData.EditValue = "Source Data";
		this.xTPage_SourceData.FieldName = null;
		this.xTPage_SourceData.IsSearchControl = false;
		this.xTPage_SourceData.Name = "xTPage_SourceData";
		this.xTPage_SourceData.ReadOnly = false;
		this.xTPage_SourceData.SettingInVisible = false;
		this.xTPage_SourceData.SettingReadonly = false;
		this.xTPage_SourceData.Size = new System.Drawing.Size(545, 289);
		this.xTPage_SourceData.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xTPage_SourceData.Text = "Source Data";
		this.grd_SourceData.AllowPasteExcel = false;
		this.grd_SourceData.ClipboardData = null;
		this.grd_SourceData.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_SourceData.Location = new System.Drawing.Point(0, 0);
		this.grd_SourceData.MainView = this.gv_SourceData;
		this.grd_SourceData.Name = "grd_SourceData";
		this.grd_SourceData.Size = new System.Drawing.Size(545, 289);
		this.grd_SourceData.TabIndex = 0;
		this.grd_SourceData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_SourceData });
		this.gv_SourceData.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_SourceData.ColumnKey = null;
		this.gv_SourceData.DisableFormSetting = false;
		this.gv_SourceData.DisplayException = false;
		this.gv_SourceData.EditColumn = null;
		this.gv_SourceData.FieldCurrency = null;
		this.gv_SourceData.FieldRate = null;
		this.gv_SourceData.FormId = "ImportTimeSheeting";
		this.gv_SourceData.GridControl = this.grd_SourceData;
		this.gv_SourceData.IgnoreFilter = false;
		this.gv_SourceData.IndexEditRow = -1;
		this.gv_SourceData.IndicatorWidth = 35;
		this.gv_SourceData.IsApzTable = false;
		this.gv_SourceData.IsInsertNewRow = false;
		this.gv_SourceData.IsLoadingData = false;
		this.gv_SourceData.IsMappingContextAction = false;
		this.gv_SourceData.ItemId = "APZ_TSL1";
		this.gv_SourceData.LstColumnAllowBatch = null;
		this.gv_SourceData.LstColumnNotAllowBatch = null;
		this.gv_SourceData.LstColumnNotEditable = null;
		this.gv_SourceData.LstColumNotSetting = null;
		this.gv_SourceData.Name = "gv_SourceData";
		this.gv_SourceData.OptionsView.ShowGroupPanel = false;
		this.gv_SourceData.UseBatchMod = false;
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(93, 394);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 16;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(12, 394);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(75, 23);
		this.btnOK.TabIndex = 15;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		this.btnImport.EditControl = null;
		this.btnImport.EditValue = "Select";
		this.btnImport.FieldName = null;
		this.btnImport.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnImport.ImageOptions.Image");
		this.btnImport.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
		this.btnImport.IsSearchControl = false;
		this.btnImport.Location = new System.Drawing.Point(12, 15);
		this.btnImport.Name = "btnImport";
		this.btnImport.ReadOnly = false;
		this.btnImport.SettingInVisible = false;
		this.btnImport.SettingReadonly = false;
		this.btnImport.Size = new System.Drawing.Size(75, 23);
		this.btnImport.TabIndex = 17;
		this.btnImport.Text = "Select";
		this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
		this.luk_Period.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_Period.EditControl = null;
		this.luk_Period.FieldName = null;
		this.luk_Period.IsSearchControl = false;
		this.luk_Period.Location = new System.Drawing.Point(421, 44);
		this.luk_Period.Name = "luk_Period";
		this.luk_Period.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Period.Properties.CheckFields = null;
		this.luk_Period.Properties.CheckTables = null;
		this.luk_Period.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("PeriodCode", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("PeriodName", "Name")
		});
		this.luk_Period.Properties.DropDownRows = 3;
		this.luk_Period.Properties.NullText = "";
		this.luk_Period.SettingInVisible = false;
		this.luk_Period.SettingReadonly = false;
		this.luk_Period.Size = new System.Drawing.Size(114, 20);
		this.luk_Period.TabIndex = 22;
		this.bte_Import.EditControl = null;
		this.bte_Import.FieldName = null;
		this.bte_Import.IsSearchControl = false;
		this.bte_Import.Location = new System.Drawing.Point(93, 17);
		this.bte_Import.Name = "bte_Import";
		this.bte_Import.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Clear)
		});
		this.bte_Import.SettingInVisible = false;
		this.bte_Import.SettingReadonly = false;
		this.bte_Import.Size = new System.Drawing.Size(230, 20);
		this.bte_Import.TabIndex = 23;
		this.bte_Import.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_Import_ButtonPressed);
		this.bte_Import.TextChanged += new System.EventHandler(this.bte_Import_TextChanged);
		this.txt_dateFormat.EditControl = null;
		this.txt_dateFormat.FieldName = null;
		this.txt_dateFormat.IsSearchControl = false;
		this.txt_dateFormat.Location = new System.Drawing.Point(142, 44);
		this.txt_dateFormat.Name = "txt_dateFormat";
		this.txt_dateFormat.SettingInVisible = false;
		this.txt_dateFormat.SettingReadonly = false;
		this.txt_dateFormat.Size = new System.Drawing.Size(113, 20);
		this.txt_dateFormat.TabIndex = 24;
		this.txt_dateFormat.Visible = false;
		this.btnSaveTemp.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnSaveTemp.EditControl = null;
		this.btnSaveTemp.EditValue = "Save Template";
		this.btnSaveTemp.FieldName = null;
		this.btnSaveTemp.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSaveTemp.ImageOptions.Image");
		this.btnSaveTemp.IsSearchControl = false;
		this.btnSaveTemp.Location = new System.Drawing.Point(281, 394);
		this.btnSaveTemp.Name = "btnSaveTemp";
		this.btnSaveTemp.ReadOnly = false;
		this.btnSaveTemp.SettingInVisible = false;
		this.btnSaveTemp.SettingReadonly = false;
		this.btnSaveTemp.Size = new System.Drawing.Size(124, 23);
		this.btnSaveTemp.TabIndex = 28;
		this.btnSaveTemp.Text = "Save Template";
		this.btnSaveTemp.Click += new System.EventHandler(this.btnSaveTemp_Click);
		this.btnLoadTemp.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnLoadTemp.EditControl = null;
		this.btnLoadTemp.EditValue = "Load Template";
		this.btnLoadTemp.FieldName = null;
		this.btnLoadTemp.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnLoadTemp.ImageOptions.Image");
		this.btnLoadTemp.IsSearchControl = false;
		this.btnLoadTemp.Location = new System.Drawing.Point(411, 394);
		this.btnLoadTemp.Name = "btnLoadTemp";
		this.btnLoadTemp.ReadOnly = false;
		this.btnLoadTemp.SettingInVisible = false;
		this.btnLoadTemp.SettingReadonly = false;
		this.btnLoadTemp.Size = new System.Drawing.Size(124, 23);
		this.btnLoadTemp.TabIndex = 29;
		this.btnLoadTemp.Text = "Load Template";
		this.btnLoadTemp.Click += new System.EventHandler(this.btnLoadTemp_Click);
		this.labelLine1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Branch";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(375, 21);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(49, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 30;
		this.labelLine1.Utext = "Branch";
		this.labelLine2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Period";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(375, 47);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(49, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 31;
		this.labelLine2.Utext = "Period";
		this.chk_DateFormat.EditControl = null;
		this.chk_DateFormat.FieldName = null;
		this.chk_DateFormat.IsSearchControl = false;
		this.chk_DateFormat.Location = new System.Drawing.Point(12, 44);
		this.chk_DateFormat.Name = "chk_DateFormat";
		this.chk_DateFormat.Properties.Caption = "Custom Date Format";
		this.chk_DateFormat.SettingInVisible = false;
		this.chk_DateFormat.SettingReadonly = false;
		this.chk_DateFormat.Size = new System.Drawing.Size(124, 20);
		this.chk_DateFormat.TabIndex = 33;
		this.chk_DateFormat.CheckedChanged += new System.EventHandler(this.chk_DateFormat_CheckedChanged);
		this.luk_Branch.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_Branch.EditControl = null;
		this.luk_Branch.FieldName = null;
		this.luk_Branch.IsSearchControl = false;
		this.luk_Branch.Location = new System.Drawing.Point(421, 18);
		this.luk_Branch.Name = "luk_Branch";
		this.luk_Branch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Branch.Properties.CheckFields = null;
		this.luk_Branch.Properties.CheckTables = null;
		this.luk_Branch.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Branch.Properties.NullText = "";
		this.luk_Branch.SettingInVisible = false;
		this.luk_Branch.SettingReadonly = false;
		this.luk_Branch.Size = new System.Drawing.Size(114, 20);
		this.luk_Branch.TabIndex = 27;
		this.luk_Branch.EditValueChanged += new System.EventHandler(this.luk_Branch_EditValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(547, 429);
		base.Controls.Add(this.chk_DateFormat);
		base.Controls.Add(this.txt_dateFormat);
		base.Controls.Add(this.luk_Period);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.luk_Branch);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btnLoadTemp);
		base.Controls.Add(this.btnSaveTemp);
		base.Controls.Add(this.bte_Import);
		base.Controls.Add(this.btnImport);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.xTab_MapField);
		base.FormId = "ImportTimeSheeting";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ImportTimeSheeting";
		base.ShowInTaskbar = false;
		this.Text = "Import Time Sheeting";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Absence_FormClosing);
		base.Load += new System.EventHandler(this.frm_ImportTimeSheeting_Load);
		((System.ComponentModel.ISupportInitialize)this.rep_luk_TargetField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xTab_MapField).EndInit();
		this.xTab_MapField.ResumeLayout(false);
		this.xTPage_MappingRule.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_MappingRule).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_MappingRule).EndInit();
		this.xTPage_SourceData.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_SourceData).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SourceData).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Period.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_Import.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_dateFormat.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_DateFormat.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
