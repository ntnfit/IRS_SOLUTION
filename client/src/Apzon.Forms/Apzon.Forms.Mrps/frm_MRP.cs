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
using Apzon.Controls.Pivot;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.Sales;
using Apzon.Forms.UserControls;
using Apzon.Forms.UserControls.FilterControl;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPivotGrid;

namespace Apzon.Forms.Mrps;

public class frm_MRP : XtraFormEx
{
	private const string UpDateString = "Update";

	private const string OkString = "OK";

	private const string FindString = "Find";

	private const string Runstring = "Run";

	private int _runRequestTimeout = 30;

	private bool _isLoadControl;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient(ApzObjectTypes.oMrp);

	private DataTable _sourceFilterValue = new DataTable();

	private IContainer components;

	private PanelControl panelControl1;

	private TextEditEx txt_Comment;

	private TextEditEx txt_Name;

	private LabelLine lbStatus;

	private LabelLine lbComment;

	private LabelLine lbWhsCode;

	private LabelLine lbToForecastDate;

	private LabelLine lbFixedDate;

	private LabelLine lbFromDate;

	private LabelLine lbForecast;

	private LabelLine lbType;

	private LabelLine lbMRPName;

	private LabelLine lbMRPNo;

	private ButtonEditEx txt_MrpNo;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Run;

	private PivotGridControlEx pgcDetail;

	private PivotGridField fieldItemCode;

	private PivotGridField field_Note;

	private PivotGridField field_OB;

	private PivotGridField field_Amount;

	private PivotGridField field_Date;

	private LookUpEditEx txtStatus;

	private LookUpEditEx txt_Type;

	private DateEditEx txt_ToFCastDate;

	private ButtonEditEx txt_Forecast;

	private DateEditEx txt_FromDate;

	private DateEditEx txtFixedDate;

	private RepositoryItemCalcEdit rCal;

	private ContextMenuStrip contextMenuStrip;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ToolStripMenuItem addToolStripMenuItem;

	private LabelLine lbBranch;

	private LookUpEditEx txtBranch;

	private LabelLine lbSchedule;

	private ButtonEditEx txtSche;

	private PivotGridField field_BP;

	private LabelLine lbSalesOrder;

	private ButtonEditEx txtSalesOrder;

	private PivotGridField fieldItemName;

	private PivotGridField field_WeekOfDay;

	private LookUpEditEx txtViewMode;

	private LabelLine lbViewMode;

	private PivotGridField field_MonthOfDay;

	private LabelLine lbPrjCode;

	private ButtonEditEx txtPrjCode;

	private PivotGridField field_NoteType;

	private LookUpEditEx txtFilterField;

	private LabelLine lbFilterBy;

	private FilterValueControlEx txtFilterValue;

	private ToolStripMenuItemFilter toolStripMenuItemFilter1;

	private LookUpEditEx txtPeriodType;

	private LabelLine lbPeriodType;

	private GridView searchLookUpEdit1View;

	private SearchLookUpEdit txt_WhsCode;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth == Enums.FormAuthorities.Read && value != Enums.FormViewModeType.Find)
			{
				_formMode = Enums.FormViewModeType.View;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.View:
				EnableDisableControl(isEnable: true);
				txt_MrpNo.ReadOnly = false;
				txt_MrpNo.Properties.Buttons[0].Visible = false;
				btn_Run.Text = "OK";
				txt_MrpNo.ReadOnly = true;
				break;
			case Enums.FormViewModeType.Update:
				EnableDisableControl(isEnable: true);
				txt_MrpNo.ReadOnly = true;
				btn_Run.Text = "Update";
				break;
			case Enums.FormViewModeType.Find:
				LoadStatus("D");
				EnableDisableControl(isEnable: true);
				contextMenuStrip.Enabled = false;
				SetDataControl(null);
				SetDataPivot(null);
				btn_Run.Text = "Find";
				break;
			case Enums.FormViewModeType.Busy:
				EnableDisableControl(isEnable: false);
				break;
			case Enums.FormViewModeType.Add:
				EnableDisableControl(isEnable: true);
				txt_MrpNo.ReadOnly = true;
				btn_Run.Text = "Run";
				txt_MrpNo.Properties.Buttons[0].Visible = false;
				txtStatus.ReadOnly = true;
				break;
			}
			SetColorControll(value);
		}
		catch
		{
		}
	}

	public frm_MRP()
	{
		InitializeComponent();
		OnLoadAsync += frm_MRP_Load;
	}

	public frm_MRP(int mrpCode)
	{
		InitializeComponent();
		OnLoadAsync += frm_MRP_Load;
		txt_MrpNo.Text = Function.ToString(mrpCode);
	}

	public async Task MOVE_PREVIOUS()
	{
		WinCommons._OpenProcessing();
		try
		{
			await NextLastData(Enums.NavigateType.Previous);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		WinCommons._OpenProcessing();
		try
		{
			await NextLastData(Enums.NavigateType.Next);
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
		WinCommons._OpenProcessing();
		try
		{
			await NextLastData(Enums.NavigateType.First);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		WinCommons._OpenProcessing();
		try
		{
			await NextLastData(Enums.NavigateType.Last);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MODE_FIND()
	{
		FormMode = Enums.FormViewModeType.Find;
	}

	public async Task REFRESH_DATA()
	{
		WinCommons._OpenProcessing();
		try
		{
			await NextLastData(Enums.NavigateType.Current);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		FormMode = Enums.FormViewModeType.Busy;
		WinCommons._OpenProcessing();
		try
		{
			SetDataControl(null);
			ButtonEditEx buttonEditEx = txt_MrpNo;
			buttonEditEx.EditValue = await GetNewIdToAddNew();
			SetDataPivot(null);
			LoadStatus("D");
			await SetDataWhs();
			txtStatus.EditValue = "D";
			DataTable dataTable = txt_Type.Properties.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				txt_Type.EditValue = Function.ToString(dataTable.Rows[0]["Code"]);
			}
			txtPeriodType.EditValue = "D";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.Add;
		WinCommons._CloseProcessing();
	}

	private void LoadControlViewModeData()
	{
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Daily",
					Code = "D"
				},
				new Common.Table_Code
				{
					Name = "Weekly",
					Code = "W"
				},
				new Common.Table_Code
				{
					Name = "Monthly",
					Code = "M"
				},
				new Common.Table_Code
				{
					Name = "Yearly",
					Code = "Y"
				}
			};
			txtViewMode.Properties.DataSource = dataSource;
			txtPeriodType.Properties.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> LoadDataType()
	{
		try
		{
			return await _httpClient.Commons.GetDataTypeObject(ApzObjectTypes.oMrp);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private void InitFormatControl()
	{
		try
		{
			txtFixedDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			txt_FromDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			txt_ToFCastDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			field_Date.SetFormatControl(Enums.DisplayFormatType.DateTime);
			field_Amount.SetFormatControl(Enums.DisplayFormatType.Qty);
			field_OB.SetFormatControl(Enums.DisplayFormatType.Qty);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EnableDisableControl(bool isEnable)
	{
		try
		{
			DateEditEx dateEditEx = txtFixedDate;
			LookUpEditEx lookUpEditEx = txtStatus;
			TextEditEx textEditEx = txt_Comment;
			ButtonEditEx buttonEditEx = txt_Forecast;
			DateEditEx dateEditEx2 = txt_FromDate;
			ButtonEditEx buttonEditEx2 = txt_MrpNo;
			TextEditEx textEditEx2 = txt_Name;
			DateEditEx dateEditEx3 = txt_ToFCastDate;
			LookUpEditEx lookUpEditEx2 = txt_Type;
			SearchLookUpEdit searchLookUpEdit = txt_WhsCode;
			LookUpEditEx lookUpEditEx3 = txtBranch;
			ButtonEditEx buttonEditEx3 = txtSche;
			ButtonEditEx buttonEditEx4 = txtSalesOrder;
			ButtonEditEx buttonEditEx5 = txtPrjCode;
			bool flag = (txtPeriodType.ReadOnly = !isEnable);
			bool flag3 = (buttonEditEx5.ReadOnly = flag);
			bool flag5 = (buttonEditEx4.ReadOnly = flag3);
			bool flag7 = (buttonEditEx3.ReadOnly = flag5);
			bool flag9 = (lookUpEditEx3.ReadOnly = flag7);
			bool flag11 = (searchLookUpEdit.ReadOnly = flag9);
			bool flag13 = (lookUpEditEx2.ReadOnly = flag11);
			bool flag15 = (dateEditEx3.ReadOnly = flag13);
			bool flag17 = (textEditEx2.ReadOnly = flag15);
			bool flag19 = (buttonEditEx2.ReadOnly = flag17);
			bool flag21 = (dateEditEx2.ReadOnly = flag19);
			bool flag23 = (buttonEditEx.ReadOnly = flag21);
			bool flag25 = (textEditEx.ReadOnly = flag23);
			bool readOnly = (lookUpEditEx.ReadOnly = flag25);
			dateEditEx.ReadOnly = readOnly;
			SimpleButtonEx simpleButtonEx = btn_Run;
			SimpleButtonEx simpleButtonEx2 = btn_Cancel;
			ContextMenuStrip contextMenuStrip = this.contextMenuStrip;
			EditorButton editorButton = txt_MrpNo.Properties.Buttons[0];
			EditorButton editorButton2 = txt_Forecast.Properties.Buttons[0];
			EditorButton editorButton3 = txtSalesOrder.Properties.Buttons[0];
			EditorButton editorButton4 = txtSche.Properties.Buttons[0];
			ToolStripMenuItem toolStripMenuItem = deleteToolStripMenuItem;
			flag13 = (txtPrjCode.Properties.Buttons[0].Visible = isEnable);
			flag15 = (toolStripMenuItem.Enabled = flag13);
			flag17 = (editorButton4.Visible = flag15);
			flag19 = (editorButton3.Visible = flag17);
			flag21 = (editorButton2.Visible = flag19);
			flag23 = (editorButton.Visible = flag21);
			flag25 = (contextMenuStrip.Enabled = flag23);
			readOnly = (simpleButtonEx2.Enabled = flag25);
			simpleButtonEx.Enabled = readOnly;
			if (isEnable)
			{
				LoadControlByType(Function.ToString(txt_Type.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadControlByType(string type)
	{
		bool flag = Function.ToString(txtStatus.EditValue) != "D" && !string.IsNullOrEmpty(Function.ToString(txtStatus.EditValue)) && FormMode != Enums.FormViewModeType.Add;
		DateEditEx dateEditEx = txtFixedDate;
		ButtonEditEx buttonEditEx = txt_Forecast;
		DateEditEx dateEditEx2 = txt_FromDate;
		DateEditEx dateEditEx3 = txt_ToFCastDate;
		LookUpEditEx lookUpEditEx = txt_Type;
		SearchLookUpEdit searchLookUpEdit = txt_WhsCode;
		LookUpEditEx lookUpEditEx2 = txtBranch;
		ButtonEditEx buttonEditEx2 = txtSche;
		ButtonEditEx buttonEditEx3 = txtSalesOrder;
		bool flag2 = (txtPrjCode.ReadOnly = flag);
		bool flag4 = (buttonEditEx3.ReadOnly = flag2);
		bool flag6 = (buttonEditEx2.ReadOnly = flag4);
		bool flag8 = (lookUpEditEx2.ReadOnly = flag6);
		bool flag10 = (searchLookUpEdit.ReadOnly = flag8);
		bool flag12 = (lookUpEditEx.ReadOnly = flag10);
		bool flag14 = (dateEditEx3.ReadOnly = flag12);
		bool flag16 = (dateEditEx2.ReadOnly = flag14);
		bool readOnly = (buttonEditEx.ReadOnly = flag16);
		dateEditEx.ReadOnly = readOnly;
		EditorButton editorButton = txt_Forecast.Properties.Buttons[0];
		EditorButton editorButton2 = txtSche.Properties.Buttons[0];
		EditorButton editorButton3 = txtSalesOrder.Properties.Buttons[0];
		flag14 = (txtPrjCode.Properties.Buttons[0].Visible = !flag);
		flag16 = (editorButton3.Visible = flag14);
		readOnly = (editorButton2.Visible = flag16);
		editorButton.Visible = readOnly;
		lbSalesOrder.Utext = "Sales Order";
		switch (type)
		{
		case "1":
			txtSche.ReadOnly = true;
			txtSche.Properties.Buttons[0].Visible = false;
			txtSche.EditValue = "";
			txtSalesOrder.ReadOnly = true;
			txtSalesOrder.Properties.Buttons[0].Visible = false;
			txtSalesOrder.EditValue = "";
			break;
		case "2":
			txtSche.ReadOnly = true;
			txtSche.Properties.Buttons[0].Visible = false;
			txtSche.EditValue = "";
			lbSalesOrder.Utext = "Exclude Orders";
			break;
		case "3":
			txtSalesOrder.ReadOnly = true;
			txt_Forecast.ReadOnly = true;
			txtSalesOrder.Properties.Buttons[0].Visible = false;
			txt_Forecast.Properties.Buttons[0].Visible = false;
			txtSalesOrder.EditValue = "";
			txt_Forecast.EditValue = "";
			break;
		case "4":
			txt_Forecast.ReadOnly = true;
			txt_Forecast.Properties.Buttons[0].Visible = false;
			txt_Forecast.EditValue = "";
			txtSche.ReadOnly = true;
			txtSche.Properties.Buttons[0].Visible = false;
			txtSche.EditValue = "";
			break;
		}
		txt_Comment.ReadOnly = false;
	}

	private async Task<DataTable> GetListWhs()
	{
		DataTable dt = new DataTable();
		try
		{
			if (GlobalData.IsMultiBranch && string.IsNullOrEmpty(txtBranch.Text))
			{
				return dt;
			}
			dt = await _httpClient.Mrp.GetWarehouse(Function.ParseInt(txtBranch.EditValue));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return dt;
	}

	private void SetDataControl(DataRow row)
	{
		try
		{
			_isLoadControl = true;
			if (row != null)
			{
				txtSche.EditValue = Function.ToString(row["PRSId"]);
				txt_Type.EditValue = Function.ToString(row["Type"]);
				txtBranch.EditValue = Function.ParseInt(row["BplID"]);
				txt_MrpNo.EditValue = Function.ParseInt(row["MrpCode"]);
				txt_Comment.EditValue = Function.ToString(row["Notes"]);
				txt_Forecast.EditValue = Function.ToString(row["FCCode"]);
				txt_FromDate.EditValue = Function.ParseDateTime(row["BeginDate"]);
				txt_Name.EditValue = Function.ToString(row["MrpName"]);
				txt_WhsCode.EditValue = Function.ToString(row["Whs"]);
				txtStatus.EditValue = Function.ToString(row["Status"]);
				txt_ToFCastDate.EditValue = Function.ParseDateTime(row["FCstDate"]);
				txtFixedDate.EditValue = Function.ParseDateTimes(row["FixDate"]);
				txtSalesOrder.EditValue = Function.ToString(row["SoId"]);
				txtPrjCode.EditValue = Function.ToString(row["PrjCode"]);
				txtPeriodType.EditValue = Function.ToString(row["PeriodType"]);
			}
			else
			{
				txtSche.EditValue = null;
				DataTable dataTable = txt_Type.Properties.DataSource as DataTable;
				if (dataTable.IsNotNull())
				{
					txt_Type.EditValue = Function.ToString(dataTable.Rows[0]["Code"]);
				}
				txtBranch.EditValue = GlobalData.DefaultBranch;
				txtFixedDate.EditValue = null;
				txt_Comment.EditValue = null;
				txt_Forecast.EditValue = null;
				txt_FromDate.EditValue = null;
				txt_Name.EditValue = null;
				txt_WhsCode.EditValue = null;
				txtStatus.EditValue = null;
				txt_MrpNo.EditValue = null;
				txtSalesOrder.EditValue = null;
				txtPrjCode.EditValue = null;
				txtPeriodType.EditValue = "D";
			}
			txtViewMode.EditValue = "D";
			_isLoadControl = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DateTime?> GetEndateForecast(string forecastCode)
	{
		try
		{
			return Function.ParseDateTime(await _httpClient.Mrp.GetEndateForecast(forecastCode), "yyyyMMdd");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return null;
	}

	private string GetDataDoc(ref DataTable dt, Enums.TransactionType type)
	{
		if (string.IsNullOrEmpty(Function.ToString(txt_WhsCode.EditValue)))
		{
			txt_WhsCode.Focus();
			return "Warehouse can not be null";
		}
		if (string.IsNullOrEmpty(Function.ToString(txt_FromDate.EditValue)))
		{
			txt_FromDate.Focus();
			return "Begin date can not be null";
		}
		if (string.IsNullOrEmpty(Function.ToString(txtFixedDate.EditValue)))
		{
			txtFixedDate.Focus();
			return "Fix date can not be null";
		}
		if (string.IsNullOrEmpty(Function.ToString(txt_ToFCastDate.EditValue)))
		{
			txt_ToFCastDate.Focus();
			return "Forecast date can not be null";
		}
		if (Function.ParseDateTimes(txt_FromDate.EditValue).Date > Function.ParseDateTimes(txtFixedDate.EditValue).Date)
		{
			txt_FromDate.Focus();
			return "From date can not be greater than fix date";
		}
		if (Function.ParseDateTimes(txtFixedDate.EditValue).Date > Function.ParseDateTimes(txt_ToFCastDate.EditValue).Date)
		{
			txt_FromDate.Focus();
			return "Fixed date can not be greater than to forecast date";
		}
		if (GlobalData.IsMultiBranch && string.IsNullOrEmpty(Function.ToString(txtBranch.EditValue)))
		{
			txtBranch.Focus();
			return "branch could not be null!";
		}
		dt.Columns.Add("MrpCode", typeof(string));
		dt.Columns.Add("MrpName", typeof(string));
		dt.Columns.Add("BeginDate", typeof(DateTime));
		dt.Columns.Add("FCstDate", typeof(DateTime));
		dt.Columns.Add("Status", typeof(string));
		dt.Columns.Add("CreateDate", typeof(DateTime));
		dt.Columns.Add("FCCode", typeof(string));
		dt.Columns.Add("Whs", typeof(string));
		dt.Columns.Add("Notes", typeof(string));
		dt.Columns.Add("FixDate", typeof(DateTime));
		dt.Columns.Add("Type", typeof(string));
		dt.Columns.Add("BplID", typeof(int));
		dt.Columns.Add("PRSId", typeof(string));
		dt.Columns.Add("SalesOrder", typeof(string));
		dt.Columns.Add("PrjCode", typeof(string));
		dt.Columns.Add("PeriodType", typeof(string));
		DataRow dataRow = dt.NewRow();
		dataRow["MrpCode"] = ((type == Enums.TransactionType.U) ? Function.ParseInt(txt_MrpNo.EditValue) : 0);
		dataRow["MrpName"] = Function.ToString(txt_Name.EditValue);
		dataRow["BeginDate"] = Function.ParseDateTimes(txt_FromDate.EditValue);
		dataRow["FCstDate"] = Function.ParseDateTimes(txt_ToFCastDate.EditValue);
		dataRow["FixDate"] = txtFixedDate.EditValue;
		dataRow["Status"] = txtStatus.EditValue;
		dataRow["CreateDate"] = DateTime.Now.Date;
		dataRow["FCCode"] = Function.ToString(txt_Forecast.EditValue);
		dataRow["Whs"] = Function.ToString(txt_WhsCode.EditValue);
		dataRow["Notes"] = Function.ToString(txt_Comment.EditValue);
		dataRow["Type"] = Function.ToString(txt_Type.EditValue);
		dataRow["BplID"] = Function.ParseInt(txtBranch.EditValue);
		dataRow["PRSId"] = Function.ToString(txtSche.EditValue);
		dataRow["SalesOrder"] = Function.ToString(txtSalesOrder.EditValue);
		dataRow["PrjCode"] = Function.ToString(txtPrjCode.EditValue);
		dataRow["PeriodType"] = Function.ToString(txtPeriodType.EditValue);
		dt.Rows.Add(dataRow);
		return "";
	}

	private async Task<DataTable> GetDataPivot(int id)
	{
		DataTable dt = new DataTable();
		try
		{
			dt = await _httpClient.Mrp.GetDataLine(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		if (!dt.IsNotNull())
		{
			WinCommons.Warning(Enums.MessageCode.DataLinesIsEmpty, "");
		}
		return dt;
	}

	private async Task<string> RunMrp(Enums.TransactionType type)
	{
		try
		{
			DataTable dt = new DataTable();
			string dataDoc = GetDataDoc(ref dt, type);
			if (string.IsNullOrEmpty(dataDoc))
			{
				HttpResult httpResult = await _httpClient.Mrp.RunMrp(dt, _runRequestTimeout);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					txt_MrpNo.EditValue = Function.ParseInt(httpResult.Content);
					LoadStatus(Function.ToString(txtStatus.EditValue));
					return "";
				}
				return httpResult.Message;
			}
			return dataDoc;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private async Task Find(bool isFind, Enums.FormViewModeType oldview = Enums.FormViewModeType.Busy)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.MRP, false, txt_MrpNo.Text, "", Enums.ItemObjectType.NoItemType, null, "")
			{
				MrpCode = Function.ToString(txt_MrpNo.EditValue),
				MrpStatus = (isFind ? Function.ToString(txtStatus.EditValue) : ""),
				ForecastCode = (isFind ? Function.ToString(txt_Forecast.EditValue) : ""),
				MrpType = (isFind ? Function.ToString(txt_Type.EditValue) : "")
			};
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				LoadStatus(Function.ToString(frm_CFL2.ChooseItem["Status"]) ?? "D");
				SetDataControl(frm_CFL2.ChooseItem);
				SetDataPivot(await GetDataPivot(Function.ParseInt(txt_MrpNo.EditValue)));
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task NextLastData(Enums.NavigateType type)
	{
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			DataTable dataTable = await _httpClient.Mrp.GetDataDoc(Function.ParseInt(txt_MrpNo.EditValue), type);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				SetDataControl(dataTable.Rows[0]);
				SetDataPivot(await GetDataPivot(Function.ParseInt(txt_MrpNo.EditValue)));
				LoadStatus(Function.ToString(txtStatus.EditValue));
				FormMode = Enums.FormViewModeType.View;
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.Find;
	}

	private async Task Delete(int id)
	{
		Enums.FormViewModeType oldview = FormMode;
		FormMode = Enums.FormViewModeType.Busy;
		WinCommons._OpenProcessing();
		try
		{
			HttpResult httpResult = await _httpClient.Mrp.Delete(id);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
				oldview = Enums.FormViewModeType.Find;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = oldview;
		WinCommons._CloseProcessing();
	}

	private void LoadStatus(string currentStatus)
	{
		try
		{
			Dictionary<string, string> dataSource = new Dictionary<string, string>();
			switch (currentStatus)
			{
			case "D":
				dataSource = new Dictionary<string, string>
				{
					{ "D", "Draft" },
					{ "P", "Pending" },
					{ "A", "Approved" },
					{ "C", "Closed" }
				};
				break;
			case "P":
				dataSource = new Dictionary<string, string>
				{
					{ "P", "Pending" },
					{ "A", "Approved" },
					{ "C", "Closed" }
				};
				break;
			case "A":
				dataSource = new Dictionary<string, string>
				{
					{ "A", "Approved" },
					{ "C", "Closed" }
				};
				break;
			case "C":
				dataSource = new Dictionary<string, string> { { "C", "Closed" } };
				break;
			}
			txtStatus.Properties.DataSource = dataSource;
			txtStatus.EditValue = currentStatus;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<string> MrpUpdate()
	{
		_ = 2;
		try
		{
			if (btn_Run.Text == "Run")
			{
				string text = await RunMrp(Enums.TransactionType.U);
				if (string.IsNullOrEmpty(text))
				{
					SetDataPivot(await GetDataPivot(Function.ParseInt(txt_MrpNo.EditValue)));
					WinCommons.Success(Enums.MessageCode.RunSuccess, "");
					return "";
				}
				return text;
			}
			DataTable dt = new DataTable();
			string dataDoc = GetDataDoc(ref dt, Enums.TransactionType.U);
			if (!string.IsNullOrEmpty(dataDoc))
			{
				return dataDoc;
			}
			HttpResult httpResult = await _httpClient.Mrp.Update(dt);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.UpdateSuccess, "");
				return "";
			}
			return httpResult.Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private DataTable GetDataBrach()
	{
		try
		{
			return _httpClient.Commons.GetDataBrach(GlobalData.UserCodeLogin);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private void SetDataBranch()
	{
		try
		{
			if (!GlobalData.IsMultiBranch)
			{
				txtBranch.Enabled = false;
				return;
			}
			DataTable dataBrach = GetDataBrach();
			txtBranch.Properties.DataSource = dataBrach;
			txtBranch.EditValue = GlobalData.DefaultBranch;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SetDataWhs()
	{
		try
		{
			RepositoryItemSearchLookUpEdit properties = txt_WhsCode.Properties;
			properties.DataSource = await GetListWhs();
			txt_WhsCode.Properties.ValueMember = "WhsCode";
			txt_WhsCode.Properties.DisplayMember = "WhsName";
			txt_WhsCode.EditValue = "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<HttpResult> ValidateScheduleForMrp(string lstPsCode)
	{
		try
		{
			return await _httpClient.Mrp.ValidateScheduleForMrp(lstPsCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	private void SetColorControll(Enums.FormViewModeType type)
	{
		try
		{
			AppearanceObject appearance = txtBranch.Properties.Appearance;
			AppearanceObject appearance2 = txtFixedDate.Properties.Appearance;
			AppearanceObject appearance3 = txtSalesOrder.Properties.Appearance;
			AppearanceObject appearance4 = txtSche.Properties.Appearance;
			AppearanceObject appearance5 = txtStatus.Properties.Appearance;
			AppearanceObject appearance6 = txt_Comment.Properties.Appearance;
			AppearanceObject appearance7 = txt_Forecast.Properties.Appearance;
			AppearanceObject appearance8 = txt_FromDate.Properties.Appearance;
			AppearanceObject appearance9 = txt_MrpNo.Properties.Appearance;
			AppearanceObject appearance10 = txt_Name.Properties.Appearance;
			AppearanceObject appearance11 = txt_ToFCastDate.Properties.Appearance;
			AppearanceObject appearance12 = txt_Type.Properties.Appearance;
			AppearanceObject appearance13 = txt_WhsCode.Properties.Appearance;
			AppearanceObject appearance14 = txtPrjCode.Properties.Appearance;
			Color color = (txtPeriodType.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White));
			Color color3 = (appearance14.BackColor = color);
			Color color5 = (appearance13.BackColor = color3);
			Color color7 = (appearance12.BackColor = color5);
			Color color9 = (appearance11.BackColor = color7);
			Color color11 = (appearance10.BackColor = color9);
			Color color13 = (appearance9.BackColor = color11);
			Color color15 = (appearance8.BackColor = color13);
			Color color17 = (appearance7.BackColor = color15);
			Color color19 = (appearance6.BackColor = color17);
			Color color21 = (appearance5.BackColor = color19);
			Color color23 = (appearance4.BackColor = color21);
			Color color25 = (appearance3.BackColor = color23);
			Color backColor = (appearance2.BackColor = color25);
			appearance.BackColor = backColor;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<int> GetNewIdToAddNew()
	{
		try
		{
			HttpResult httpResult = await _httpClient.Mrp.GetNewIdMrp();
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				return Function.ParseInt(httpResult.Content);
			}
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return 0;
	}

	private void SetDataPivot(DataTable dt)
	{
		pgcDetail.DataSource = dt;
		pgcDetail.RefreshData();
	}

	private async Task frm_MRP_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			_runRequestTimeout = await _httpClient.Mrp.GetRequestTimeout();
			SetDataBranch();
			LoadControlViewModeData();
			InitFormatControl();
			RepositoryItemLookUpEdit properties = txt_Type.Properties;
			properties.DataSource = await LoadDataType();
			if (!string.IsNullOrEmpty(txt_MrpNo.Text))
			{
				await NextLastData(Enums.NavigateType.Current);
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.Find;
	}

	private void txt_Forecast_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.AdvanceForecast, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count <= 0)
				{
					return;
				}
				string text = "";
				for (int i = 0; i < frm_CFL2.ChooseItems.Count; i++)
				{
					if (!string.IsNullOrEmpty(Function.ToString(frm_CFL2.ChooseItems[i]["Code"])))
					{
						text = ((!string.IsNullOrEmpty(text)) ? (text + "," + Function.ToString(frm_CFL2.ChooseItems[i]["Code"])) : Function.ToString(frm_CFL2.ChooseItems[i]["Code"]));
					}
				}
				txt_Forecast.EditValue = text;
				txt_FromDate.Focus();
				return;
			}
			string text2 = Function.ToString(txt_Forecast.EditValue);
			if (string.IsNullOrEmpty(text2))
			{
				return;
			}
			string text3 = "";
			if (text2.Split(',').Length == 1)
			{
				text3 = text2;
			}
			else
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.AdvanceForecast, multiSelect: false, "")
				{
					LstForecast = Function.ToString(text2)
				};
				frm_ChooseFromList2.ShowDialog(this);
				if (!((frm_ChooseFromList2.ChooseItem != null) ? true : false))
				{
					return;
				}
				text3 = Function.ToString(frm_ChooseFromList2.ChooseItem["Code"]);
			}
			frm_AdvanceForecast frm_AdvanceForecast2 = new frm_AdvanceForecast(text3);
			frm_AdvanceForecast2.MdiParent = MdiParent;
			frm_AdvanceForecast2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void txt_MrpNo_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			await Find(isFind: false);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async void txt_Forecast_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (!_isLoadControl && pgcDetail != null)
			{
				DateEditEx dateEditEx = txt_ToFCastDate;
				dateEditEx.EditValue = await GetEndateForecast(Function.ToString(txt_Forecast.EditValue));
			}
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

	private async void btn_Run_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType oldView = FormMode;
		try
		{
			string mess = "";
			switch (oldView)
			{
			case Enums.FormViewModeType.Add:
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRun, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
				{
					FormMode = oldView;
					WinCommons._CloseProcessing();
					return;
				}
				WinCommons._OpenProcessing();
				FormMode = Enums.FormViewModeType.Busy;
				mess = await RunMrp(Enums.TransactionType.A);
				if (string.IsNullOrEmpty(mess))
				{
					SetDataPivot(await GetDataPivot(Function.ParseInt(txt_MrpNo.EditValue)));
					LoadStatus(Function.ToString(txtStatus.EditValue));
					oldView = Enums.FormViewModeType.View;
				}
				break;
			case Enums.FormViewModeType.Find:
				WinCommons._OpenProcessing();
				await Find(isFind: true, oldView);
				WinCommons._CloseProcessing();
				return;
			case Enums.FormViewModeType.View:
				Close();
				return;
			case Enums.FormViewModeType.Update:
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
				{
					FormMode = oldView;
					WinCommons._CloseProcessing();
					return;
				}
				WinCommons._OpenProcessing();
				FormMode = Enums.FormViewModeType.Busy;
				mess = await MrpUpdate();
				if (string.IsNullOrEmpty(mess))
				{
					LoadStatus(Function.ToString(txtStatus.EditValue));
					oldView = Enums.FormViewModeType.View;
				}
				break;
			}
			if (!string.IsNullOrEmpty(mess))
			{
				WinCommons.Error(Enums.MessageCode.None, mess);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = oldView;
		WinCommons._CloseProcessing();
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				await Delete(Function.ParseInt(txt_MrpNo.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.Busy;
		WinCommons._OpenProcessing();
		try
		{
			SetDataControl(null);
			ButtonEditEx buttonEditEx = txt_MrpNo;
			buttonEditEx.EditValue = await GetNewIdToAddNew();
			SetDataPivot(null);
			LoadStatus("D");
			await SetDataWhs();
			txtStatus.EditValue = "D";
			DataTable dataTable = txt_Type.Properties.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				txt_Type.EditValue = Function.ToString(dataTable.Rows[0]["Code"]);
			}
			txtPeriodType.EditValue = "D";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.Add;
		WinCommons._CloseProcessing();
	}

	private async void Control_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!(sender is Control control))
			{
				return;
			}
			if (control.Name.Equals(txt_Type.Name))
			{
				if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.View)
				{
					btn_Run.Text = "Run";
				}
				LoadControlByType(Function.ToString(txt_Type.EditValue));
			}
			else if (control.Name.Equals(txt_Forecast.Name))
			{
				if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.View)
				{
					btn_Run.Text = "Run";
				}
			}
			else if (control.Name.Equals(txtStatus.Name))
			{
				LoadControlByType(Function.ToString(txt_Type.EditValue));
			}
			else if (control.Name.Equals(txt_WhsCode.Name))
			{
				if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.View)
				{
					btn_Run.Text = "Run";
				}
			}
			else if (control.Name.Equals(txt_FromDate.Name) || control.Name.Equals(txt_ToFCastDate.Name))
			{
				if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.View)
				{
					btn_Run.Text = "Run";
				}
			}
			else if (control.Name.Equals(txtBranch.Name))
			{
				if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.View)
				{
					btn_Run.Text = "Run";
				}
				await SetDataWhs();
			}
			else if (control.Name.Equals(txtSche.Name))
			{
				if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.View)
				{
					btn_Run.Text = "Run";
				}
			}
			else if (control.Name.Equals(txtSalesOrder.Name))
			{
				if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.View)
				{
					btn_Run.Text = "Run";
				}
			}
			else if (control.Name.Equals(txtPeriodType.Name) && (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.View))
			{
				btn_Run.Text = "Run";
			}
			if (FormMode == Enums.FormViewModeType.View)
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

	private void pgcDetail_CustomCellDisplayText(object sender, PivotCellDisplayTextEventArgs e)
	{
		try
		{
			if (e == null)
			{
				return;
			}
			if (e.RowValueType == PivotGridValueType.Total)
			{
				e.DisplayText = "";
			}
			else if (e.ColumnValueType == PivotGridValueType.GrandTotal)
			{
				if (Function.ToString(pgcDetail.GetFieldValue(pgcDetail.Fields[field_Note.FieldName], e.RowIndex)).StartsWith("IV."))
				{
					e.DisplayText = "";
				}
				else if (Function.ParseDouble(e.Value).Equals(0.0))
				{
					e.DisplayText = "";
				}
			}
			else if (e.RowValueType == PivotGridValueType.GrandTotal)
			{
				e.DisplayText = "";
			}
			else if (e.ColumnValueType == PivotGridValueType.Total)
			{
				e.DisplayText = "";
			}
			else if (e.RowValueType == PivotGridValueType.Value && Function.ParseDouble(e.Value).Equals(0.0))
			{
				e.DisplayText = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pgcDetail_FieldValueDisplayText(object sender, PivotFieldDisplayTextEventArgs e)
	{
		try
		{
			if (e != null && e.Field != null)
			{
				if (e.ValueType == PivotGridValueType.Total)
				{
					e.DisplayText = "";
				}
				else if ((e.Field.FieldName.Equals(field_Date.FieldName) || e.Field.FieldName.Equals(field_WeekOfDay.FieldName) || e.Field.FieldName.Equals(field_MonthOfDay.FieldName)) && e.FieldIndex == 0)
				{
					e.DisplayText = "Over Due";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_WhsCode_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (GlobalData.IsMultiBranch && string.IsNullOrEmpty(Function.ToString(txtBranch.EditValue)))
			{
				WinCommons.Error(Enums.MessageCode.YouMustChooseBranchAfterChooseWhs, "");
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pgcDetail_CustomAppearance(object sender, PivotCustomAppearanceEventArgs e)
	{
		try
		{
			if (e == null || e.ColumnField == null)
			{
				return;
			}
			if (e.RowValueType == PivotGridValueType.Total)
			{
				e.Appearance.BackColor = Color.OldLace;
				e.Appearance.BackColor2 = Color.OldLace;
			}
			else
			{
				if (!e.ColumnField.FieldName.Equals(field_Date.FieldName) && !e.ColumnField.FieldName.Equals(field_WeekOfDay.FieldName) && !e.ColumnField.FieldName.Equals(field_MonthOfDay.FieldName))
				{
					return;
				}
				Point focusedCell = pgcDetail.Cells.FocusedCell;
				PivotCellEventArgs cellInfo = pgcDetail.Cells.GetCellInfo(focusedCell.X, focusedCell.Y);
				if (e.RowIndex == cellInfo.RowIndex)
				{
					e.Appearance.BackColor = Color.Wheat;
				}
				else
				{
					if (!e.ColumnField.FieldName.Equals(field_Date.FieldName))
					{
						return;
					}
					if (e.ColumnIndex == 0)
					{
						e.Appearance.BackColor = pgcDetail.BackColor;
						return;
					}
					switch (Function.ParseDateTimes(pgcDetail.GetFieldValue(pgcDetail.Fields[field_Date.FieldName], e.ColumnIndex)).DayOfWeek)
					{
					case DayOfWeek.Saturday:
						e.Appearance.BackColor = Color.FromArgb(233, 249, 242);
						break;
					case DayOfWeek.Sunday:
						e.Appearance.BackColor = Color.FromArgb(225, 234, 237);
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pgcDetail_CustomDrawFieldValue(object sender, PivotCustomDrawFieldValueEventArgs e)
	{
		try
		{
			if (e != null && e.ValueType == PivotGridValueType.Total)
			{
				e.Handled = true;
				Rectangle bounds = e.Bounds;
				bounds.Inflate(-1, -1);
				e.GraphicsCache.FillRectangle(Color.OldLace, bounds);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void txtSche_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (pgcDetail == null || string.IsNullOrEmpty(Function.ToString(txtSche.EditValue)))
			{
				return;
			}
			txtBranch.ReadOnly = Function.ParseInt(txtSche.EditValue) != 0;
			if (Function.ParseInt(txtSche.EditValue) == 0)
			{
				return;
			}
			HttpResult httpResult = await ValidateScheduleForMrp(Function.ToString(txtSche.EditValue));
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				txtBranch.EditValue = GlobalData.DefaultBranch;
				txtSche.EditValue = "";
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return;
			}
			DataTable dataTable = httpResult.Content as DataTable;
			if (dataTable.IsNotNull())
			{
				txtBranch.EditValue = Function.ParseInt(dataTable.Rows[0]["BplID"]);
				txt_WhsCode.EditValue = Function.ToString(dataTable.Rows[0]["Whs"]);
				txtPrjCode.EditValue = Function.ToString(dataTable.Rows[0]["PrjCode"]);
				txt_FromDate.EditValue = dataTable.Rows[0]["BeginDate"];
				txt_ToFCastDate.EditValue = dataTable.Rows[0]["FCstDate"];
				txtFixedDate.EditValue = dataTable.Rows[0]["FixDate"];
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtSche_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Schedule, true, Function.ToString(txtSche.EditValue), "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ScheduleType = "O;T";
				frm_CFL2.ScheduleStatus = "A";
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
				{
					string editValue = string.Join(",", from t in frm_CFL2.ChooseItems.AsQueryable()
						select Function.ToString(t["PSCode"]));
					txtSche.EditValue = editValue;
					txt_FromDate.Focus();
				}
				return;
			}
			string text = Function.ToString(txtSche.EditValue);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			string text2 = "";
			if (text.Split(',').Length == 1)
			{
				text2 = text;
			}
			else
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Schedule, multiSelect: false, "")
				{
					LstSchedule = Function.ToString(text)
				};
				frm_ChooseFromList2.ShowDialog(this);
				if (!((frm_ChooseFromList2.ChooseItem != null) ? true : false))
				{
					return;
				}
				text2 = Function.ToString(frm_ChooseFromList2.ChooseItem["Code"]);
			}
			frm_Schedule frm_Schedule2 = new frm_Schedule(Function.ParseInt(text2));
			frm_Schedule2.MdiParent = MdiParent;
			frm_Schedule2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtSalesOrder_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.SalesOder, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count != 0)
				{
					string text = frm_CFL2.ChooseItems.Aggregate("", (string current, DataRow t) => current + Function.ToString(t["DocNum"]) + ",");
					text = text.Substring(0, text.Length - 1);
					txtSalesOrder.EditValue = text;
				}
				return;
			}
			string text2 = Function.ToString(txtSalesOrder.EditValue);
			if (string.IsNullOrEmpty(text2))
			{
				return;
			}
			int docEntry = 0;
			if (text2.Split(',').Length == 1)
			{
				docEntry = Function.ParseInt(text2);
			}
			else
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.MrpSalesOrder, multiSelect: false, "")
				{
					LstMrpSalesOrder = text2
				};
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					docEntry = Function.ParseInt(frm_ChooseFromList2.ChooseItem["DocNum"]);
				}
			}
			frm_SalesOrder frm_SalesOrder2 = new frm_SalesOrder(docEntry);
			frm_SalesOrder2.MdiParent = MdiParent;
			frm_SalesOrder2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtViewMode_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			switch (Function.ToString(txtViewMode.EditValue))
			{
			case "D":
				field_WeekOfDay.Visible = false;
				field_MonthOfDay.Visible = false;
				field_Date.Visible = true;
				break;
			case "W":
				field_WeekOfDay.Visible = true;
				field_MonthOfDay.Visible = false;
				field_Date.Visible = false;
				break;
			case "M":
				field_WeekOfDay.Visible = false;
				field_MonthOfDay.Visible = true;
				field_Date.Visible = false;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_MRP_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View && FormMode != Enums.FormViewModeType.Find && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
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

	private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			Point point = PointToClient(Cursor.Position);
			if ((point.X < (fieldItemCode.Visible ? fieldItemCode.Width : 0) + (fieldItemName.Visible ? fieldItemName.Width : 0) + (field_Note.Visible ? field_Note.Width : 0) + (field_OB.Visible ? field_OB.Width : 0) + 20 * Main.WidScreen / 1920 && point.Y > pgcDetail.Location.Y) || (point.Y > pgcDetail.Location.Y && point.Y < pgcDetail.Location.Y + 65 * Main.HeightScreen / 1080))
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

	private void txtPrjCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Project, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count <= 0)
				{
					return;
				}
				string text = "";
				for (int i = 0; i < frm_CFL2.ChooseItems.Count; i++)
				{
					if (!string.IsNullOrEmpty(Function.ToString(frm_CFL2.ChooseItems[i]["PrjCode"])))
					{
						text = ((!string.IsNullOrEmpty(text)) ? (text + "," + Function.ToString(frm_CFL2.ChooseItems[i]["PrjCode"])) : Function.ToString(frm_CFL2.ChooseItems[i]["PrjCode"]));
					}
				}
				txtPrjCode.EditValue = text;
				return;
			}
			string text2 = Function.ToString(txtPrjCode.EditValue);
			if (string.IsNullOrEmpty(text2))
			{
				return;
			}
			string num = "";
			if (text2.Split(',').Length == 1)
			{
				num = text2;
			}
			else
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.MrpProject, multiSelect: false, "")
				{
					LstProjectCode = Function.ToString(text2)
				};
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					num = Function.ToString(frm_ChooseFromList2.ChooseItem["PrjCode"]);
				}
			}
			this.DrillDown(ApzObjectTypes.oProjectCodes, num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pgcDetail_CustomSummary(object sender, PivotGridCustomSummaryEventArgs e)
	{
		try
		{
			if (e == null)
			{
				return;
			}
			PivotDrillDownDataSource pivotDrillDownDataSource = e.CreateDrillDownDataSource();
			if (pivotDrillDownDataSource != null && pivotDrillDownDataSource.RowCount > 0)
			{
				string text = Function.ToString(pivotDrillDownDataSource[0]["NoteType"]);
				if (text.Equals("O"))
				{
					DateTime dateTime = Function.ParseDateTimes(pivotDrillDownDataSource[0][field_Date.FieldName]);
					double num = Function.ParseDouble(pivotDrillDownDataSource[0][field_Amount.FieldName]);
					if (Function.ParseInt(pivotDrillDownDataSource[0][field_WeekOfDay.FieldName]) == -1)
					{
						dateTime = DateTime.MaxValue.Date;
						num = 0.0;
					}
					for (int i = 1; i < pivotDrillDownDataSource.RowCount; i++)
					{
						DateTime dateTime2 = Function.ParseDateTimes(pivotDrillDownDataSource[i][field_Date.FieldName]);
						if (dateTime > dateTime2)
						{
							num = Function.ParseDouble(pivotDrillDownDataSource[i][field_Amount.FieldName]);
							dateTime = dateTime2;
						}
					}
					e.CustomValue = num;
					return;
				}
				if (text.Equals("E"))
				{
					DateTime dateTime3 = Function.ParseDateTimes(pivotDrillDownDataSource[0][field_Date.FieldName]);
					double num = Function.ParseDouble(pivotDrillDownDataSource[0][field_Amount.FieldName]);
					for (int j = 1; j < pivotDrillDownDataSource.RowCount; j++)
					{
						DateTime dateTime4 = Function.ParseDateTimes(pivotDrillDownDataSource[j][field_Date.FieldName]);
						if (dateTime3 < dateTime4)
						{
							num = Function.ParseDouble(pivotDrillDownDataSource[j][field_Amount.FieldName]);
							dateTime3 = dateTime4;
						}
					}
					e.CustomValue = num;
					return;
				}
			}
			e.CustomValue = e.SummaryValue.Summary;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pgcDetail_CellDoubleClick(object sender, PivotCellEventArgs e)
	{
		try
		{
			if (e == null || e.ColumnField == null || !e.ColumnField.FieldName.Equals(field_Date.FieldName))
			{
				return;
			}
			PivotDrillDownDataSource pivotDrillDownDataSource = pgcDetail.CreateDrillDownDataSource(e.ColumnIndex, e.RowIndex);
			if (pivotDrillDownDataSource == null || pivotDrillDownDataSource.RowCount <= 0)
			{
				return;
			}
			DataTable dataTable = pivotDrillDownDataSource.ConvertPivotDrillDownDataSourceToDataTable();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[i]["DemandType"])) || Math.Abs(Function.ParseDouble(dataTable.Rows[i]["Qty"])) < 1E-07)
				{
					dataTable.Rows.RemoveAt(i);
					i--;
				}
			}
			dataTable.Columns["DueDate"].ColumnName = "WorkDate";
			dataTable.AcceptChanges();
			frm_DrillDownSourceCellMrp frm_DrillDownSourceCellMrp2 = new frm_DrillDownSourceCellMrp(dataTable, txt_FromDate.DateTime, txtFixedDate.DateTime, txt_ToFCastDate.DateTime, Enums.FormViewModeType.View);
			frm_DrillDownSourceCellMrp2.MdiParent = MdiParent;
			frm_DrillDownSourceCellMrp2.Text = string.Format("Source - Item {0} - Duedate {1}", Function.ToString(pivotDrillDownDataSource[0]["ItemCode"]), Function.ParseDateTimes(pivotDrillDownDataSource[0]["DueDate"]).ToString(GlobalData.DateTimeFormat));
			frm_DrillDownSourceCellMrp2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pgcDetail_CustomRowHeight(object sender, PivotCustomRowHeightEventArgs e)
	{
		try
		{
			if (e.ValueType == PivotGridValueType.Total)
			{
				e.RowHeight = e.RowHeight * 2 / 3;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void txt_WhsCode_BeforePopup(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(txt_WhsCode.EditValue);
			if (!string.IsNullOrWhiteSpace(text))
			{
				DataTable dataTable = txt_WhsCode.Properties.DataSource as DataTable;
				string[] source = text.Split(',');
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					if (source.Contains(dataTable.Rows[i]["WhsCode"]))
					{
						searchLookUpEdit1View.SelectRow(i);
					}
				}
			}
			else
			{
				searchLookUpEdit1View.ClearSelection();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void txt_WhsCode_Closed(object sender, ClosedEventArgs e)
	{
		try
		{
			int[] selectedRows = searchLookUpEdit1View.GetSelectedRows();
			if (selectedRows.Length == 0)
			{
				txt_WhsCode.EditValue = "";
				return;
			}
			string text = string.Empty;
			DataTable dataTable = txt_WhsCode.Properties.DataSource as DataTable;
			for (int i = 0; i < selectedRows.Length; i++)
			{
				text = text + Function.ToString(dataTable.Rows[selectedRows[i]]["WhsCode"]) + ",";
			}
			txt_WhsCode.EditValue = text.Remove(text.Length - 1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void txt_WhsCode_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		if (string.IsNullOrWhiteSpace(Function.ToString(txt_WhsCode.EditValue)))
		{
			return;
		}
		string[] array = Function.ToString(txt_WhsCode.EditValue).Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		List<string> list = new List<string>();
		DataTable source = txt_WhsCode.Properties.DataSource as DataTable;
		string[] array2 = array;
		foreach (string item in array2)
		{
			DataRow dataRow = (from t in source.AsEnumerable()
				where Function.ToString(t["WhsCode"]).Equals(Function.ToString(item))
				select t).FirstOrDefault();
			list.Add(Function.ToString(dataRow["WhsName"]));
		}
		string displayText = string.Join(Function.ToString(','), list);
		e.DisplayText = displayText;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_MRP));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItemFilter1 = new Apzon.Forms.UserControls.ToolStripMenuItemFilter(this.components);
		this.pgcDetail = new Apzon.Controls.Pivot.PivotGridControlEx();
		this.fieldItemCode = new DevExpress.XtraPivotGrid.PivotGridField();
		this.fieldItemName = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_BP = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_NoteType = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_Note = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_OB = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_Amount = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_MonthOfDay = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_WeekOfDay = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_Date = new DevExpress.XtraPivotGrid.PivotGridField();
		this.txtFilterField = new Apzon.Controls.Editor.LookUpEditEx();
		this.txtFilterValue = new Apzon.Forms.UserControls.FilterControl.FilterValueControlEx();
		this.rCal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.txt_WhsCode = new DevExpress.XtraEditors.SearchLookUpEdit();
		this.searchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.txtPeriodType = new Apzon.Controls.Editor.LookUpEditEx();
		this.lbPeriodType = new Apzon.Controls.Controls.LabelLine();
		this.lbFilterBy = new Apzon.Controls.Controls.LabelLine();
		this.lbPrjCode = new Apzon.Controls.Controls.LabelLine();
		this.txtPrjCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.txtViewMode = new Apzon.Controls.Editor.LookUpEditEx();
		this.lbViewMode = new Apzon.Controls.Controls.LabelLine();
		this.lbSalesOrder = new Apzon.Controls.Controls.LabelLine();
		this.txtSalesOrder = new Apzon.Controls.Editor.ButtonEditEx();
		this.lbSchedule = new Apzon.Controls.Controls.LabelLine();
		this.lbBranch = new Apzon.Controls.Controls.LabelLine();
		this.btn_Run = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_Comment = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Name = new Apzon.Controls.Editor.TextEditEx();
		this.lbStatus = new Apzon.Controls.Controls.LabelLine();
		this.lbComment = new Apzon.Controls.Controls.LabelLine();
		this.lbWhsCode = new Apzon.Controls.Controls.LabelLine();
		this.lbToForecastDate = new Apzon.Controls.Controls.LabelLine();
		this.lbFixedDate = new Apzon.Controls.Controls.LabelLine();
		this.lbFromDate = new Apzon.Controls.Controls.LabelLine();
		this.lbForecast = new Apzon.Controls.Controls.LabelLine();
		this.lbType = new Apzon.Controls.Controls.LabelLine();
		this.lbMRPName = new Apzon.Controls.Controls.LabelLine();
		this.lbMRPNo = new Apzon.Controls.Controls.LabelLine();
		this.txt_MrpNo = new Apzon.Controls.Editor.ButtonEditEx();
		this.txtStatus = new Apzon.Controls.Editor.LookUpEditEx();
		this.txt_Type = new Apzon.Controls.Editor.LookUpEditEx();
		this.txt_ToFCastDate = new Apzon.Controls.Editor.DateEditEx();
		this.txt_Forecast = new Apzon.Controls.Editor.ButtonEditEx();
		this.txt_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtFixedDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtBranch = new Apzon.Controls.Editor.LookUpEditEx();
		this.txtSche = new Apzon.Controls.Editor.ButtonEditEx();
		this.contextMenuStrip.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pgcDetail).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFilterField.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFilterValue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_WhsCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.searchLookUpEdit1View).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPeriodType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPrjCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtViewMode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtSalesOrder.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Comment.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MrpNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Type.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToFCastDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToFCastDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Forecast.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFixedDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFixedDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBranch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtSche.Properties).BeginInit();
		base.SuspendLayout();
		this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.deleteToolStripMenuItem, this.addToolStripMenuItem, this.toolStripMenuItemFilter1 });
		this.contextMenuStrip.Name = "contextMenuStrip";
		this.contextMenuStrip.Size = new System.Drawing.Size(141, 70);
		this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Visible = false;
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.toolStripMenuItemFilter1.DataSource = null;
		this.toolStripMenuItemFilter1.Name = "toolStripMenuItemFilter1";
		this.toolStripMenuItemFilter1.objType = Apzon.Commons.ApzObjectTypes.oMrp;
		this.toolStripMenuItemFilter1.pivod = this.pgcDetail;
		this.toolStripMenuItemFilter1.Size = new System.Drawing.Size(140, 22);
		this.toolStripMenuItemFilter1.Text = "Filter Setting";
		this.pgcDetail.AllowPasteExcel = false;
		this.pgcDetail.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pgcDetail.ClipboardData = null;
		this.pgcDetail.ContextMenuStrip = this.contextMenuStrip;
		this.pgcDetail.Fields.AddRange(new DevExpress.XtraPivotGrid.PivotGridField[10] { this.fieldItemCode, this.fieldItemName, this.field_BP, this.field_NoteType, this.field_Note, this.field_OB, this.field_Amount, this.field_MonthOfDay, this.field_WeekOfDay, this.field_Date });
		this.pgcDetail.FilterFieldControl = this.txtFilterField;
		this.pgcDetail.FilterValueControl = this.txtFilterValue;
		this.pgcDetail.FormId = "oMrp";
		this.pgcDetail.ItemId = "APZ_MRP1";
		this.pgcDetail.Location = new System.Drawing.Point(2, 83);
		this.pgcDetail.Name = "pgcDetail";
		this.pgcDetail.ObjectType = Apzon.Commons.ApzObjectTypes.oMrp;
		this.pgcDetail.OptionsView.ShowButtonMode = DevExpress.XtraPivotGrid.PivotShowButtonModeEnum.ShowAlways;
		this.pgcDetail.OptionsView.ShowFilterHeaders = false;
		this.pgcDetail.OptionsView.ShowRowGrandTotals = false;
		this.pgcDetail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rCal });
		this.pgcDetail.Size = new System.Drawing.Size(1009, 481);
		this.pgcDetail.TabIndex = 0;
		this.pgcDetail.CustomSummary += new DevExpress.XtraPivotGrid.PivotGridCustomSummaryEventHandler(this.pgcDetail_CustomSummary);
		this.pgcDetail.FieldValueDisplayText += new DevExpress.XtraPivotGrid.PivotFieldDisplayTextEventHandler(this.pgcDetail_FieldValueDisplayText);
		this.pgcDetail.CustomCellDisplayText += new DevExpress.XtraPivotGrid.PivotCellDisplayTextEventHandler(this.pgcDetail_CustomCellDisplayText);
		this.pgcDetail.CellDoubleClick += new DevExpress.XtraPivotGrid.PivotCellEventHandler(this.pgcDetail_CellDoubleClick);
		this.pgcDetail.CustomAppearance += new DevExpress.XtraPivotGrid.PivotCustomAppearanceEventHandler(this.pgcDetail_CustomAppearance);
		this.pgcDetail.CustomDrawFieldValue += new DevExpress.XtraPivotGrid.PivotCustomDrawFieldValueEventHandler(this.pgcDetail_CustomDrawFieldValue);
		this.pgcDetail.CustomRowHeight += new System.EventHandler<DevExpress.XtraPivotGrid.PivotCustomRowHeightEventArgs>(this.pgcDetail_CustomRowHeight);
		this.fieldItemCode.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.fieldItemCode.AreaIndex = 0;
		this.fieldItemCode.Caption = "Item Code";
		this.fieldItemCode.FieldName = "ItemCode";
		this.fieldItemCode.Name = "fieldItemCode";
		this.fieldItemCode.Options.ShowCustomTotals = false;
		this.fieldItemCode.Options.ShowGrandTotal = false;
		this.fieldItemCode.Options.ShowTotals = false;
		this.fieldItemName.Appearance.CellTotal.BackColor = System.Drawing.Color.Blue;
		this.fieldItemName.Appearance.CellTotal.Options.UseBackColor = true;
		this.fieldItemName.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.fieldItemName.AreaIndex = 1;
		this.fieldItemName.Caption = "Description";
		this.fieldItemName.FieldName = "ItemName";
		this.fieldItemName.Name = "fieldItemName";
		this.fieldItemName.Width = 120;
		this.field_BP.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_BP.AreaIndex = 1;
		this.field_BP.FieldName = "CardCode";
		this.field_BP.Name = "field_BP";
		this.field_BP.Visible = false;
		this.field_NoteType.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_NoteType.AreaIndex = 4;
		this.field_NoteType.FieldName = "NoteType";
		this.field_NoteType.Name = "field_NoteType";
		this.field_NoteType.Visible = false;
		this.field_Note.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_Note.AreaIndex = 2;
		this.field_Note.Caption = "Note";
		this.field_Note.FieldName = "TypeName";
		this.field_Note.Name = "field_Note";
		this.field_Note.Options.ShowCustomTotals = false;
		this.field_Note.Options.ShowGrandTotal = false;
		this.field_Note.Options.ShowTotals = false;
		this.field_Note.Width = 200;
		this.field_OB.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_OB.AreaIndex = 3;
		this.field_OB.Caption = "OB";
		this.field_OB.FieldName = "OB";
		this.field_OB.Name = "field_OB";
		this.field_Amount.Appearance.CellGrandTotal.BackColor = System.Drawing.Color.PeachPuff;
		this.field_Amount.Appearance.CellGrandTotal.Options.UseBackColor = true;
		this.field_Amount.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
		this.field_Amount.AreaIndex = 0;
		this.field_Amount.Caption = "Amount";
		this.field_Amount.FieldName = "Qty";
		this.field_Amount.Name = "field_Amount";
		this.field_Amount.SummaryType = DevExpress.Data.PivotGrid.PivotSummaryType.Custom;
		this.field_Amount.Width = 80;
		this.field_MonthOfDay.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
		this.field_MonthOfDay.AreaIndex = 1;
		this.field_MonthOfDay.Caption = "Month";
		this.field_MonthOfDay.FieldName = "MonthOfDay";
		this.field_MonthOfDay.Name = "field_MonthOfDay";
		this.field_MonthOfDay.Visible = false;
		this.field_WeekOfDay.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
		this.field_WeekOfDay.AreaIndex = 1;
		this.field_WeekOfDay.Caption = "Week";
		this.field_WeekOfDay.FieldName = "WeekOfDay";
		this.field_WeekOfDay.Name = "field_WeekOfDay";
		this.field_WeekOfDay.Visible = false;
		this.field_Date.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
		this.field_Date.AreaIndex = 0;
		this.field_Date.Caption = "Date";
		this.field_Date.FieldName = "DueDate";
		this.field_Date.Name = "field_Date";
		this.field_Date.Width = 80;
		this.txtFilterField.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.txtFilterField.EditControl = null;
		this.txtFilterField.FieldName = "FilterField";
		this.txtFilterField.IsSearchControl = false;
		this.txtFilterField.Location = new System.Drawing.Point(583, 570);
		this.txtFilterField.Name = "txtFilterField";
		this.txtFilterField.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFilterField.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("FilterCode", "FilterCode"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("FilterName", "FilterName")
		});
		this.txtFilterField.Properties.DisplayMember = "FilterName";
		this.txtFilterField.Properties.NullText = "";
		this.txtFilterField.Properties.ShowHeader = false;
		this.txtFilterField.Properties.ValueMember = "FilterCode";
		this.txtFilterField.SettingInVisible = false;
		this.txtFilterField.SettingReadonly = false;
		this.txtFilterField.Size = new System.Drawing.Size(100, 20);
		this.txtFilterField.TabIndex = 18;
		this.txtFilterValue.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.txtFilterValue.EditControl = null;
		this.txtFilterValue.FieldName = "FilterValue";
		this.txtFilterValue.IsSearchControl = false;
		this.txtFilterValue.Location = new System.Drawing.Point(690, 570);
		this.txtFilterValue.Name = "txtFilterValue";
		this.txtFilterValue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txtFilterValue.SettingInVisible = false;
		this.txtFilterValue.SettingReadonly = false;
		this.txtFilterValue.Size = new System.Drawing.Size(120, 20);
		this.txtFilterValue.SourceFilter = null;
		this.txtFilterValue.TabIndex = 19;
		this.rCal.AutoHeight = false;
		this.rCal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rCal.Name = "rCal";
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = "btnCancel";
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(83, 570);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(70, 20);
		this.btn_Cancel.TabIndex = 16;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.panelControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panelControl1.ContextMenuStrip = this.contextMenuStrip;
		this.panelControl1.Controls.Add(this.txt_WhsCode);
		this.panelControl1.Controls.Add(this.txtPeriodType);
		this.panelControl1.Controls.Add(this.lbPeriodType);
		this.panelControl1.Controls.Add(this.txtFilterValue);
		this.panelControl1.Controls.Add(this.txtFilterField);
		this.panelControl1.Controls.Add(this.lbFilterBy);
		this.panelControl1.Controls.Add(this.lbPrjCode);
		this.panelControl1.Controls.Add(this.txtPrjCode);
		this.panelControl1.Controls.Add(this.txtViewMode);
		this.panelControl1.Controls.Add(this.lbViewMode);
		this.panelControl1.Controls.Add(this.lbSalesOrder);
		this.panelControl1.Controls.Add(this.txtSalesOrder);
		this.panelControl1.Controls.Add(this.lbSchedule);
		this.panelControl1.Controls.Add(this.lbBranch);
		this.panelControl1.Controls.Add(this.pgcDetail);
		this.panelControl1.Controls.Add(this.btn_Cancel);
		this.panelControl1.Controls.Add(this.btn_Run);
		this.panelControl1.Controls.Add(this.txt_Comment);
		this.panelControl1.Controls.Add(this.txt_Name);
		this.panelControl1.Controls.Add(this.lbStatus);
		this.panelControl1.Controls.Add(this.lbComment);
		this.panelControl1.Controls.Add(this.lbWhsCode);
		this.panelControl1.Controls.Add(this.lbToForecastDate);
		this.panelControl1.Controls.Add(this.lbFixedDate);
		this.panelControl1.Controls.Add(this.lbFromDate);
		this.panelControl1.Controls.Add(this.lbForecast);
		this.panelControl1.Controls.Add(this.lbType);
		this.panelControl1.Controls.Add(this.lbMRPName);
		this.panelControl1.Controls.Add(this.lbMRPNo);
		this.panelControl1.Controls.Add(this.txt_MrpNo);
		this.panelControl1.Controls.Add(this.txtStatus);
		this.panelControl1.Controls.Add(this.txt_Type);
		this.panelControl1.Controls.Add(this.txt_ToFCastDate);
		this.panelControl1.Controls.Add(this.txt_Forecast);
		this.panelControl1.Controls.Add(this.txt_FromDate);
		this.panelControl1.Controls.Add(this.txtFixedDate);
		this.panelControl1.Controls.Add(this.txtBranch);
		this.panelControl1.Controls.Add(this.txtSche);
		this.panelControl1.Location = new System.Drawing.Point(2, 1);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(1012, 595);
		this.panelControl1.TabIndex = 0;
		this.txt_WhsCode.Location = new System.Drawing.Point(677, 29);
		this.txt_WhsCode.Name = "txt_WhsCode";
		this.txt_WhsCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_WhsCode.Properties.NullText = "";
		this.txt_WhsCode.Properties.PopupView = this.searchLookUpEdit1View;
		this.txt_WhsCode.Size = new System.Drawing.Size(120, 20);
		this.txt_WhsCode.TabIndex = 25;
		this.txt_WhsCode.BeforePopup += new System.EventHandler(this.txt_WhsCode_BeforePopup);
		this.txt_WhsCode.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.txt_WhsCode_Closed);
		this.txt_WhsCode.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.txt_WhsCode_CustomDisplayText);
		this.searchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.searchLookUpEdit1View.Name = "searchLookUpEdit1View";
		this.searchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.searchLookUpEdit1View.OptionsSelection.MultiSelect = true;
		this.searchLookUpEdit1View.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.searchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
		this.txtPeriodType.EditControl = null;
		this.txtPeriodType.FieldName = "PeriodType";
		this.txtPeriodType.IsSearchControl = false;
		this.txtPeriodType.Location = new System.Drawing.Point(884, 53);
		this.txtPeriodType.Name = "txtPeriodType";
		this.txtPeriodType.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txtPeriodType.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtPeriodType.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.WhiteSmoke;
		this.txtPeriodType.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txtPeriodType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtPeriodType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code")
		});
		this.txtPeriodType.Properties.DisplayMember = "Name";
		this.txtPeriodType.Properties.NullText = "";
		this.txtPeriodType.Properties.ShowHeader = false;
		this.txtPeriodType.Properties.ValueMember = "Code";
		this.txtPeriodType.SettingInVisible = false;
		this.txtPeriodType.SettingReadonly = false;
		this.txtPeriodType.Size = new System.Drawing.Size(120, 20);
		this.txtPeriodType.TabIndex = 21;
		this.txtPeriodType.EditValueChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.lbPeriodType.EditControl = null;
		this.lbPeriodType.EditValue = "Period Type";
		this.lbPeriodType.FieldName = "lbPeriodType";
		this.lbPeriodType.HideWhenEmpty = false;
		this.lbPeriodType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbPeriodType.Location = new System.Drawing.Point(809, 56);
		this.lbPeriodType.Name = "lbPeriodType";
		this.lbPeriodType.ReadOnly = false;
		this.lbPeriodType.SettingInVisible = false;
		this.lbPeriodType.SettingReadonly = false;
		this.lbPeriodType.ShowUnderLine = true;
		this.lbPeriodType.Size = new System.Drawing.Size(75, 20);
		this.lbPeriodType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbPeriodType.TabIndex = 20;
		this.lbPeriodType.Utext = "Period Type";
		this.lbFilterBy.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.lbFilterBy.EditControl = null;
		this.lbFilterBy.EditValue = "Filter by";
		this.lbFilterBy.FieldName = "lbFilterBy";
		this.lbFilterBy.HideWhenEmpty = false;
		this.lbFilterBy.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbFilterBy.Location = new System.Drawing.Point(501, 572);
		this.lbFilterBy.Name = "lbFilterBy";
		this.lbFilterBy.ReadOnly = false;
		this.lbFilterBy.SettingInVisible = false;
		this.lbFilterBy.SettingReadonly = false;
		this.lbFilterBy.ShowUnderLine = true;
		this.lbFilterBy.Size = new System.Drawing.Size(80, 20);
		this.lbFilterBy.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbFilterBy.TabIndex = 17;
		this.lbFilterBy.Utext = "Filter by";
		this.lbPrjCode.EditControl = null;
		this.lbPrjCode.EditValue = "Project Code";
		this.lbPrjCode.FieldName = "lbPrjCode";
		this.lbPrjCode.HideWhenEmpty = false;
		this.lbPrjCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbPrjCode.Location = new System.Drawing.Point(809, 31);
		this.lbPrjCode.Name = "lbPrjCode";
		this.lbPrjCode.ReadOnly = false;
		this.lbPrjCode.SettingInVisible = false;
		this.lbPrjCode.SettingReadonly = false;
		this.lbPrjCode.ShowUnderLine = true;
		this.lbPrjCode.Size = new System.Drawing.Size(75, 20);
		this.lbPrjCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbPrjCode.TabIndex = 0;
		this.lbPrjCode.Utext = "Project Code";
		this.txtPrjCode.EditControl = null;
		this.txtPrjCode.FieldName = "PrjCode";
		this.txtPrjCode.IsSearchControl = false;
		this.txtPrjCode.Location = new System.Drawing.Point(884, 28);
		this.txtPrjCode.Name = "txtPrjCode";
		this.txtPrjCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txtPrjCode.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtPrjCode.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.WhiteSmoke;
		this.txtPrjCode.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.txtPrjCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtPrjCode.SettingInVisible = false;
		this.txtPrjCode.SettingReadonly = false;
		this.txtPrjCode.Size = new System.Drawing.Size(120, 20);
		this.txtPrjCode.TabIndex = 14;
		this.txtPrjCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtPrjCode_ButtonClick);
		this.txtViewMode.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.txtViewMode.EditControl = null;
		this.txtViewMode.FieldName = "ViewMode";
		this.txtViewMode.IsSearchControl = false;
		this.txtViewMode.Location = new System.Drawing.Point(886, 570);
		this.txtViewMode.Name = "txtViewMode";
		this.txtViewMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtViewMode.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.txtViewMode.Properties.DisplayMember = "Name";
		this.txtViewMode.Properties.NullText = "";
		this.txtViewMode.Properties.ShowFooter = false;
		this.txtViewMode.Properties.ShowHeader = false;
		this.txtViewMode.Properties.ValueMember = "Code";
		this.txtViewMode.SettingInVisible = false;
		this.txtViewMode.SettingReadonly = false;
		this.txtViewMode.Size = new System.Drawing.Size(120, 20);
		this.txtViewMode.TabIndex = 14;
		this.txtViewMode.EditValueChanged += new System.EventHandler(this.txtViewMode_EditValueChanged);
		this.lbViewMode.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.lbViewMode.EditControl = null;
		this.lbViewMode.EditValue = "View Mode";
		this.lbViewMode.FieldName = "lbViewMode";
		this.lbViewMode.HideWhenEmpty = false;
		this.lbViewMode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbViewMode.Location = new System.Drawing.Point(816, 572);
		this.lbViewMode.Name = "lbViewMode";
		this.lbViewMode.ReadOnly = false;
		this.lbViewMode.SettingInVisible = false;
		this.lbViewMode.SettingReadonly = false;
		this.lbViewMode.ShowUnderLine = true;
		this.lbViewMode.Size = new System.Drawing.Size(70, 20);
		this.lbViewMode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbViewMode.TabIndex = 0;
		this.lbViewMode.Utext = "View Mode";
		this.lbSalesOrder.EditControl = null;
		this.lbSalesOrder.EditValue = "Sales Order";
		this.lbSalesOrder.FieldName = "lbSalesOrder";
		this.lbSalesOrder.HideWhenEmpty = false;
		this.lbSalesOrder.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbSalesOrder.Location = new System.Drawing.Point(809, 6);
		this.lbSalesOrder.Name = "lbSalesOrder";
		this.lbSalesOrder.ReadOnly = false;
		this.lbSalesOrder.SettingInVisible = false;
		this.lbSalesOrder.SettingReadonly = false;
		this.lbSalesOrder.ShowUnderLine = true;
		this.lbSalesOrder.Size = new System.Drawing.Size(75, 20);
		this.lbSalesOrder.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbSalesOrder.TabIndex = 0;
		this.lbSalesOrder.Utext = "Sales Order";
		this.txtSalesOrder.EditControl = null;
		this.txtSalesOrder.FieldName = "SoId";
		this.txtSalesOrder.IsSearchControl = false;
		this.txtSalesOrder.Location = new System.Drawing.Point(884, 3);
		this.txtSalesOrder.Name = "txtSalesOrder";
		this.txtSalesOrder.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txtSalesOrder.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtSalesOrder.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.WhiteSmoke;
		this.txtSalesOrder.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.txtSalesOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtSalesOrder.SettingInVisible = false;
		this.txtSalesOrder.SettingReadonly = false;
		this.txtSalesOrder.Size = new System.Drawing.Size(120, 20);
		this.txtSalesOrder.TabIndex = 13;
		this.txtSalesOrder.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtSalesOrder_ButtonClick);
		this.txtSalesOrder.EditValueChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.lbSchedule.EditControl = null;
		this.lbSchedule.EditValue = "Schedule";
		this.lbSchedule.FieldName = "lbScheduleNo";
		this.lbSchedule.HideWhenEmpty = false;
		this.lbSchedule.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbSchedule.Location = new System.Drawing.Point(206, 56);
		this.lbSchedule.Name = "lbSchedule";
		this.lbSchedule.ReadOnly = false;
		this.lbSchedule.SettingInVisible = false;
		this.lbSchedule.SettingReadonly = false;
		this.lbSchedule.ShowUnderLine = true;
		this.lbSchedule.Size = new System.Drawing.Size(70, 20);
		this.lbSchedule.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbSchedule.TabIndex = 0;
		this.lbSchedule.Utext = "Schedule";
		this.lbBranch.EditControl = null;
		this.lbBranch.EditValue = "Branch";
		this.lbBranch.FieldName = "lbBranch";
		this.lbBranch.HideWhenEmpty = false;
		this.lbBranch.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbBranch.Location = new System.Drawing.Point(607, 6);
		this.lbBranch.Name = "lbBranch";
		this.lbBranch.ReadOnly = false;
		this.lbBranch.SettingInVisible = false;
		this.lbBranch.SettingReadonly = false;
		this.lbBranch.ShowUnderLine = true;
		this.lbBranch.Size = new System.Drawing.Size(70, 20);
		this.lbBranch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbBranch.TabIndex = 0;
		this.lbBranch.Utext = "Branch";
		this.btn_Run.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Run.EditControl = null;
		this.btn_Run.EditValue = "Run";
		this.btn_Run.FieldName = "btnSave";
		this.btn_Run.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Run.ImageOptions.Image");
		this.btn_Run.IsSearchControl = false;
		this.btn_Run.Location = new System.Drawing.Point(6, 570);
		this.btn_Run.Name = "btn_Run";
		this.btn_Run.ReadOnly = false;
		this.btn_Run.SettingInVisible = false;
		this.btn_Run.SettingReadonly = false;
		this.btn_Run.Size = new System.Drawing.Size(70, 20);
		this.btn_Run.TabIndex = 15;
		this.btn_Run.Text = "Run";
		this.btn_Run.Click += new System.EventHandler(this.btn_Run_Click);
		this.txt_Comment.EditControl = null;
		this.txt_Comment.FieldName = "Comment";
		this.txt_Comment.IsSearchControl = false;
		this.txt_Comment.Location = new System.Drawing.Point(677, 53);
		this.txt_Comment.Name = "txt_Comment";
		this.txt_Comment.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_Comment.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_Comment.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.WhiteSmoke;
		this.txt_Comment.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txt_Comment.SettingInVisible = false;
		this.txt_Comment.SettingReadonly = false;
		this.txt_Comment.Size = new System.Drawing.Size(120, 20);
		this.txt_Comment.TabIndex = 12;
		this.txt_Comment.EditValueChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.txt_Name.EditControl = null;
		this.txt_Name.FieldName = "Name";
		this.txt_Name.IsSearchControl = false;
		this.txt_Name.Location = new System.Drawing.Point(80, 53);
		this.txt_Name.Name = "txt_Name";
		this.txt_Name.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_Name.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_Name.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.WhiteSmoke;
		this.txt_Name.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txt_Name.SettingInVisible = false;
		this.txt_Name.SettingReadonly = false;
		this.txt_Name.Size = new System.Drawing.Size(120, 20);
		this.txt_Name.TabIndex = 3;
		this.txt_Name.EditValueChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.lbStatus.EditControl = null;
		this.lbStatus.EditValue = "Status";
		this.lbStatus.FieldName = "lbStatus";
		this.lbStatus.HideWhenEmpty = false;
		this.lbStatus.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbStatus.Location = new System.Drawing.Point(206, 6);
		this.lbStatus.Name = "lbStatus";
		this.lbStatus.ReadOnly = false;
		this.lbStatus.SettingInVisible = false;
		this.lbStatus.SettingReadonly = false;
		this.lbStatus.ShowUnderLine = true;
		this.lbStatus.Size = new System.Drawing.Size(70, 20);
		this.lbStatus.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbStatus.TabIndex = 0;
		this.lbStatus.Utext = "Status";
		this.lbComment.EditControl = null;
		this.lbComment.EditValue = "Commnent";
		this.lbComment.FieldName = "lbCommnet";
		this.lbComment.HideWhenEmpty = false;
		this.lbComment.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbComment.Location = new System.Drawing.Point(607, 56);
		this.lbComment.Name = "lbComment";
		this.lbComment.ReadOnly = false;
		this.lbComment.SettingInVisible = false;
		this.lbComment.SettingReadonly = false;
		this.lbComment.ShowUnderLine = true;
		this.lbComment.Size = new System.Drawing.Size(70, 20);
		this.lbComment.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbComment.TabIndex = 0;
		this.lbComment.Utext = "Commnent";
		this.lbWhsCode.EditControl = null;
		this.lbWhsCode.EditValue = "Warehouse";
		this.lbWhsCode.FieldName = "lbWhs";
		this.lbWhsCode.HideWhenEmpty = false;
		this.lbWhsCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbWhsCode.Location = new System.Drawing.Point(607, 31);
		this.lbWhsCode.Name = "lbWhsCode";
		this.lbWhsCode.ReadOnly = false;
		this.lbWhsCode.SettingInVisible = false;
		this.lbWhsCode.SettingReadonly = false;
		this.lbWhsCode.ShowUnderLine = true;
		this.lbWhsCode.Size = new System.Drawing.Size(70, 20);
		this.lbWhsCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbWhsCode.TabIndex = 0;
		this.lbWhsCode.Utext = "Warehouse";
		this.lbToForecastDate.EditControl = null;
		this.lbToForecastDate.EditValue = "To Fst Date";
		this.lbToForecastDate.FieldName = "lbFCstDate";
		this.lbToForecastDate.HideWhenEmpty = false;
		this.lbToForecastDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbToForecastDate.Location = new System.Drawing.Point(405, 54);
		this.lbToForecastDate.Name = "lbToForecastDate";
		this.lbToForecastDate.ReadOnly = false;
		this.lbToForecastDate.SettingInVisible = false;
		this.lbToForecastDate.SettingReadonly = false;
		this.lbToForecastDate.ShowUnderLine = true;
		this.lbToForecastDate.Size = new System.Drawing.Size(70, 20);
		this.lbToForecastDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbToForecastDate.TabIndex = 0;
		this.lbToForecastDate.Utext = "To Fst Date";
		this.lbFixedDate.EditControl = null;
		this.lbFixedDate.EditValue = "Fixed Date";
		this.lbFixedDate.FieldName = "lbFixDate";
		this.lbFixedDate.HideWhenEmpty = false;
		this.lbFixedDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbFixedDate.Location = new System.Drawing.Point(405, 31);
		this.lbFixedDate.Name = "lbFixedDate";
		this.lbFixedDate.ReadOnly = false;
		this.lbFixedDate.SettingInVisible = false;
		this.lbFixedDate.SettingReadonly = false;
		this.lbFixedDate.ShowUnderLine = true;
		this.lbFixedDate.Size = new System.Drawing.Size(70, 20);
		this.lbFixedDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbFixedDate.TabIndex = 0;
		this.lbFixedDate.Utext = "Fixed Date";
		this.lbFromDate.EditControl = null;
		this.lbFromDate.EditValue = "From Date";
		this.lbFromDate.FieldName = "lbFromDate";
		this.lbFromDate.HideWhenEmpty = false;
		this.lbFromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbFromDate.Location = new System.Drawing.Point(405, 6);
		this.lbFromDate.Name = "lbFromDate";
		this.lbFromDate.ReadOnly = false;
		this.lbFromDate.SettingInVisible = false;
		this.lbFromDate.SettingReadonly = false;
		this.lbFromDate.ShowUnderLine = true;
		this.lbFromDate.Size = new System.Drawing.Size(70, 20);
		this.lbFromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbFromDate.TabIndex = 0;
		this.lbFromDate.Utext = "From Date";
		this.lbForecast.EditControl = null;
		this.lbForecast.EditValue = "Forecast";
		this.lbForecast.FieldName = "lbForecast";
		this.lbForecast.HideWhenEmpty = false;
		this.lbForecast.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbForecast.Location = new System.Drawing.Point(206, 31);
		this.lbForecast.Name = "lbForecast";
		this.lbForecast.ReadOnly = false;
		this.lbForecast.SettingInVisible = false;
		this.lbForecast.SettingReadonly = false;
		this.lbForecast.ShowUnderLine = true;
		this.lbForecast.Size = new System.Drawing.Size(70, 20);
		this.lbForecast.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbForecast.TabIndex = 0;
		this.lbForecast.Utext = "Forecast";
		this.lbType.EditControl = null;
		this.lbType.EditValue = "Type";
		this.lbType.FieldName = "lbType";
		this.lbType.HideWhenEmpty = false;
		this.lbType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbType.Location = new System.Drawing.Point(10, 6);
		this.lbType.Name = "lbType";
		this.lbType.ReadOnly = false;
		this.lbType.SettingInVisible = false;
		this.lbType.SettingReadonly = false;
		this.lbType.ShowUnderLine = true;
		this.lbType.Size = new System.Drawing.Size(70, 20);
		this.lbType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbType.TabIndex = 0;
		this.lbType.Utext = "Type";
		this.lbMRPName.EditControl = null;
		this.lbMRPName.EditValue = "Description";
		this.lbMRPName.FieldName = "lbDesr";
		this.lbMRPName.HideWhenEmpty = false;
		this.lbMRPName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbMRPName.Location = new System.Drawing.Point(10, 56);
		this.lbMRPName.Name = "lbMRPName";
		this.lbMRPName.ReadOnly = false;
		this.lbMRPName.SettingInVisible = false;
		this.lbMRPName.SettingReadonly = false;
		this.lbMRPName.ShowUnderLine = true;
		this.lbMRPName.Size = new System.Drawing.Size(70, 20);
		this.lbMRPName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbMRPName.TabIndex = 0;
		this.lbMRPName.Utext = "Description";
		this.lbMRPNo.EditControl = null;
		this.lbMRPNo.EditValue = "MRP No.";
		this.lbMRPNo.FieldName = "lbMrpNo";
		this.lbMRPNo.HideWhenEmpty = false;
		this.lbMRPNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbMRPNo.Location = new System.Drawing.Point(10, 31);
		this.lbMRPNo.Name = "lbMRPNo";
		this.lbMRPNo.ReadOnly = false;
		this.lbMRPNo.SettingInVisible = false;
		this.lbMRPNo.SettingReadonly = false;
		this.lbMRPNo.ShowUnderLine = true;
		this.lbMRPNo.Size = new System.Drawing.Size(70, 20);
		this.lbMRPNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbMRPNo.TabIndex = 0;
		this.lbMRPNo.Utext = "MRP No.";
		this.txt_MrpNo.EditControl = null;
		this.txt_MrpNo.FieldName = "MrpCode";
		this.txt_MrpNo.IsSearchControl = false;
		this.txt_MrpNo.Location = new System.Drawing.Point(80, 28);
		this.txt_MrpNo.Name = "txt_MrpNo";
		this.txt_MrpNo.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_MrpNo.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_MrpNo.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.WhiteSmoke;
		this.txt_MrpNo.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txt_MrpNo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txt_MrpNo.SettingInVisible = false;
		this.txt_MrpNo.SettingReadonly = false;
		this.txt_MrpNo.Size = new System.Drawing.Size(120, 20);
		this.txt_MrpNo.TabIndex = 2;
		this.txt_MrpNo.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_MrpNo_ButtonClick);
		this.txtStatus.EditControl = null;
		this.txtStatus.FieldName = "Status";
		this.txtStatus.IsSearchControl = false;
		this.txtStatus.Location = new System.Drawing.Point(276, 3);
		this.txtStatus.Name = "txtStatus";
		this.txtStatus.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txtStatus.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtStatus.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.WhiteSmoke;
		this.txtStatus.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txtStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtStatus.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Key", "Name3", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name4")
		});
		this.txtStatus.Properties.DisplayMember = "Value";
		this.txtStatus.Properties.NullText = "";
		this.txtStatus.Properties.ShowFooter = false;
		this.txtStatus.Properties.ShowHeader = false;
		this.txtStatus.Properties.ValueMember = "Key";
		this.txtStatus.SettingInVisible = false;
		this.txtStatus.SettingReadonly = false;
		this.txtStatus.Size = new System.Drawing.Size(120, 20);
		this.txtStatus.TabIndex = 4;
		this.txtStatus.EditValueChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.txt_Type.EditControl = null;
		this.txt_Type.FieldName = "Type";
		this.txt_Type.IsSearchControl = false;
		this.txt_Type.Location = new System.Drawing.Point(80, 3);
		this.txt_Type.Name = "txt_Type";
		this.txt_Type.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_Type.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_Type.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.WhiteSmoke;
		this.txt_Type.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txt_Type.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_Type.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name1", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name2")
		});
		this.txt_Type.Properties.DisplayMember = "Name";
		this.txt_Type.Properties.NullText = "";
		this.txt_Type.Properties.ShowFooter = false;
		this.txt_Type.Properties.ShowHeader = false;
		this.txt_Type.Properties.ValueMember = "Code";
		this.txt_Type.SettingInVisible = false;
		this.txt_Type.SettingReadonly = false;
		this.txt_Type.Size = new System.Drawing.Size(120, 20);
		this.txt_Type.TabIndex = 1;
		this.txt_Type.EditValueChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.txt_ToFCastDate.EditControl = null;
		this.txt_ToFCastDate.EditValue = null;
		this.txt_ToFCastDate.FieldName = "FCstDate";
		this.txt_ToFCastDate.Location = new System.Drawing.Point(475, 51);
		this.txt_ToFCastDate.Name = "txt_ToFCastDate";
		this.txt_ToFCastDate.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_ToFCastDate.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_ToFCastDate.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.WhiteSmoke;
		this.txt_ToFCastDate.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txt_ToFCastDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_ToFCastDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_ToFCastDate.Properties.IsSearchControl = false;
		this.txt_ToFCastDate.SettingInVisible = false;
		this.txt_ToFCastDate.SettingReadonly = false;
		this.txt_ToFCastDate.Size = new System.Drawing.Size(120, 20);
		this.txt_ToFCastDate.TabIndex = 8;
		this.txt_ToFCastDate.EditValueChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.txt_Forecast.EditControl = null;
		this.txt_Forecast.FieldName = "FCCode";
		this.txt_Forecast.IsSearchControl = false;
		this.txt_Forecast.Location = new System.Drawing.Point(276, 28);
		this.txt_Forecast.Name = "txt_Forecast";
		this.txt_Forecast.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_Forecast.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_Forecast.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.WhiteSmoke;
		this.txt_Forecast.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.txt_Forecast.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_Forecast.SettingInVisible = false;
		this.txt_Forecast.SettingReadonly = false;
		this.txt_Forecast.Size = new System.Drawing.Size(120, 20);
		this.txt_Forecast.TabIndex = 5;
		this.txt_Forecast.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_Forecast_ButtonClick);
		this.txt_Forecast.EditValueChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.txt_Forecast.Validating += new System.ComponentModel.CancelEventHandler(this.txt_Forecast_Validating);
		this.txt_FromDate.EditControl = null;
		this.txt_FromDate.EditValue = null;
		this.txt_FromDate.FieldName = "FromDate";
		this.txt_FromDate.Location = new System.Drawing.Point(475, 3);
		this.txt_FromDate.Name = "txt_FromDate";
		this.txt_FromDate.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_FromDate.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_FromDate.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.WhiteSmoke;
		this.txt_FromDate.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txt_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_FromDate.Properties.IsSearchControl = false;
		this.txt_FromDate.SettingInVisible = false;
		this.txt_FromDate.SettingReadonly = false;
		this.txt_FromDate.Size = new System.Drawing.Size(120, 20);
		this.txt_FromDate.TabIndex = 7;
		this.txt_FromDate.EditValueChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.txtFixedDate.EditControl = null;
		this.txtFixedDate.EditValue = null;
		this.txtFixedDate.FieldName = "FixDate";
		this.txtFixedDate.Location = new System.Drawing.Point(475, 28);
		this.txtFixedDate.Name = "txtFixedDate";
		this.txtFixedDate.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txtFixedDate.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtFixedDate.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.WhiteSmoke;
		this.txtFixedDate.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txtFixedDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFixedDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFixedDate.Properties.IsSearchControl = false;
		this.txtFixedDate.SettingInVisible = false;
		this.txtFixedDate.SettingReadonly = false;
		this.txtFixedDate.Size = new System.Drawing.Size(120, 20);
		this.txtFixedDate.TabIndex = 9;
		this.txtFixedDate.EditValueChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.txtBranch.EditControl = null;
		this.txtBranch.FieldName = "Branch";
		this.txtBranch.IsSearchControl = false;
		this.txtBranch.Location = new System.Drawing.Point(677, 3);
		this.txtBranch.Name = "txtBranch";
		this.txtBranch.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txtBranch.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtBranch.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.WhiteSmoke;
		this.txtBranch.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txtBranch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtBranch.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name3"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name4", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.txtBranch.Properties.DisplayMember = "Name";
		this.txtBranch.Properties.NullText = "";
		this.txtBranch.Properties.ShowFooter = false;
		this.txtBranch.Properties.ShowHeader = false;
		this.txtBranch.Properties.ValueMember = "Value";
		this.txtBranch.SettingInVisible = false;
		this.txtBranch.SettingReadonly = false;
		this.txtBranch.Size = new System.Drawing.Size(120, 20);
		this.txtBranch.TabIndex = 10;
		this.txtBranch.EditValueChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.txtSche.EditControl = null;
		this.txtSche.FieldName = "ScheCode";
		this.txtSche.IsSearchControl = false;
		this.txtSche.Location = new System.Drawing.Point(276, 53);
		this.txtSche.Name = "txtSche";
		this.txtSche.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txtSche.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtSche.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.WhiteSmoke;
		this.txtSche.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.txtSche.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtSche.SettingInVisible = false;
		this.txtSche.SettingReadonly = false;
		this.txtSche.Size = new System.Drawing.Size(120, 20);
		this.txtSche.TabIndex = 6;
		this.txtSche.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtSche_ButtonClick);
		this.txtSche.EditValueChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.txtSche.Validating += new System.ComponentModel.CancelEventHandler(this.txtSche_Validating);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(1016, 597);
		this.ContextMenuStrip = this.contextMenuStrip;
		base.Controls.Add(this.panelControl1);
		base.FormId = "oMrp";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_MRP";
		this.Text = "Mrp Running";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_MRP_FormClosing);
		this.contextMenuStrip.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pgcDetail).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFilterField.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFilterValue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_WhsCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.searchLookUpEdit1View).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPeriodType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPrjCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtViewMode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtSalesOrder.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Comment.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MrpNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Type.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToFCastDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToFCastDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Forecast.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFixedDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFixedDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBranch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtSche.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
