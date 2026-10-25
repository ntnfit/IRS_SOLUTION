using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
using Apzon.Entities.Models.Production;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid;

namespace Apzon.Forms.Production;

public class frm_MaterialShortageReport : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private const string UpDateString = "Update";

	private const string OkString = "OK";

	private const string FindString = "Find";

	private const string Runstring = "Run";

	private const string SaveString = "Add";

	private DataTable _docStructure;

	private string _materialCode = "";

	private bool _isLoad;

	private Enums.FormViewModeType _currentState = Enums.FormViewModeType.Add;

	private bool _isLoadControl;

	private IContainer components;

	private LabelLine lbType;

	private LabelLine lbFromDate;

	private LabelLine lbToDate;

	private LabelLine lbXclud;

	private LookUpEditEx txtType;

	private DateEditEx txtFromDate;

	private DateEditEx txtTodate;

	private ButtonEditEx txtXclud;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnRun;

	private PivotGridControlEx pgc_Doc;

	private PivotGridField field_WoID;

	private PivotGridField field_ProductNo;

	private PivotGridField field_ProductName;

	private PivotGridField field_Components;

	private PivotGridField field_targets;

	private PivotGridField field_Qty;

	private PivotGridField field_PlanQty;

	private PivotGridField field_OpenQty;

	private LookUpEditEx txtStatus;

	private LabelLine lbStatus;

	private ButtonEditEx txtCode;

	private LookUpEditEx txtBranch;

	private TextEditEx txtRemark;

	private TextEditEx txtName;

	private LabelLine lbCode;

	private LabelLine lbBranch;

	private LabelLine lbRemarks;

	private LabelLine lbName;

	private PivotGridField field_ProductUoM;

	private PivotGridField field_ComponentUoM;

	private PivotGridField pivotGridField2;

	private PivotGridField pivotGridField3;

	private RepositoryItemCalcEdit rCalEdit;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full)
			{
				FormMode = Enums.FormViewModeType.View;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.View:
				EnableDisableControl(isEnable: true);
				txtBranch.ReadOnly = true;
				txtBranch.Properties.Buttons[0].Visible = false;
				txtCode.ReadOnly = true;
				txtCode.Properties.Buttons[0].Visible = false;
				txtType.ReadOnly = true;
				txtType.Properties.Buttons[0].Visible = false;
				btnRun.Text = "OK";
				_currentState = Enums.FormViewModeType.Update;
				txtCode.Focus();
				break;
			case Enums.FormViewModeType.Update:
				EnableDisableControl(isEnable: true);
				txtBranch.ReadOnly = true;
				txtBranch.Properties.Buttons[0].Visible = false;
				txtCode.ReadOnly = true;
				txtCode.Properties.Buttons[0].Visible = false;
				txtType.ReadOnly = true;
				txtType.Properties.Buttons[0].Visible = false;
				btnRun.Text = "Update";
				break;
			case Enums.FormViewModeType.Find:
			{
				LoadStatus("D");
				EnableDisableControl(isEnable: true);
				SetDataDocControl(null);
				DataTable dataTable = pgc_Doc.DataSourceTable();
				if (dataTable.IsNotNull())
				{
					dataTable.Rows.Clear();
					pgc_Doc.DataSource = dataTable;
					pgc_Doc.DataSourceTable().AcceptChanges();
					pgc_Doc.RefreshData();
				}
				btnRun.Text = "Find";
				txtCode.Focus();
				break;
			}
			case Enums.FormViewModeType.Busy:
				EnableDisableControl(isEnable: false);
				break;
			case Enums.FormViewModeType.Add:
				EnableDisableControl(isEnable: true);
				txtCode.ReadOnly = true;
				btnRun.Text = "Add";
				txtCode.Properties.Buttons[0].Visible = false;
				break;
			case Enums.FormViewModeType.Run:
				EnableDisableControl(isEnable: true);
				txtCode.ReadOnly = true;
				btnRun.Text = "Run";
				txtCode.Properties.Buttons[0].Visible = false;
				break;
			}
			SetColorControll(value);
		}
		catch
		{
		}
	}

	public frm_MaterialShortageReport()
	{
		InitializeComponent();
	}

	public frm_MaterialShortageReport(string code)
	{
		InitializeComponent();
		_materialCode = code;
	}

	public async Task MOVE_PREVIOUS()
	{
		WinCommons._OpenProcessing();
		await SearchData(Enums.NavigateType.Previous);
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		WinCommons._OpenProcessing();
		await SearchData(Enums.NavigateType.Next);
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		WinCommons._OpenProcessing();
		await SearchData(Enums.NavigateType.First);
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		WinCommons._OpenProcessing();
		await SearchData(Enums.NavigateType.Last);
		WinCommons._CloseProcessing();
	}

	public async Task MODE_FIND()
	{
		FormMode = Enums.FormViewModeType.Find;
	}

	public async Task REFRESH_DATA()
	{
		WinCommons._OpenProcessing();
		await SearchData(Enums.NavigateType.Current);
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		FormMode = Enums.FormViewModeType.Busy;
		WinCommons._OpenProcessing();
		try
		{
			SetDataDocControl(null);
			_isLoad = true;
			ButtonEditEx buttonEditEx = txtCode;
			buttonEditEx.EditValue = await GetNewIdToAddNew();
			DataTable dataTable = pgc_Doc.DataSourceTable();
			if (dataTable.IsNotNull())
			{
				dataTable.Rows.Clear();
				pgc_Doc.DataSource = dataTable;
				pgc_Doc.DataSourceTable().AcceptChanges();
			}
			LoadStatus("D");
			txtStatus.EditValue = "D";
			DataTable dataTable2 = txtType.Properties.DataSource as DataTable;
			if (dataTable2.IsNotNull())
			{
				txtType.EditValue = Function.ToString(dataTable2.Rows[0]["Code"]);
			}
			txtName.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoad = false;
		_currentState = Enums.FormViewModeType.Add;
		FormMode = Enums.FormViewModeType.Run;
		WinCommons._CloseProcessing();
	}

	private async Task<int> GetNewIdToAddNew()
	{
		try
		{
			HttpResult httpResult = await _httpClient.MaterialShortageReportClient.GetNewId();
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

	private async Task LoadAtpType()
	{
		try
		{
			RepositoryItemLookUpEdit properties = txtType.Properties;
			properties.DataSource = await _httpClient.MaterialShortageReportClient.LoadAtpType();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadDataBranch()
	{
		try
		{
			if (!GlobalData.IsMultiBranch)
			{
				txtBranch.Enabled = false;
				return;
			}
			DataTable dataBrach = _httpClient.Commons.GetDataBrach(GlobalData.UserCodeLogin);
			txtBranch.Properties.DataSource = dataBrach;
			txtBranch.EditValue = GlobalData.DefaultBranch;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Run()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(txtFromDate.EditValue)))
			{
				WinCommons.Error(Enums.MessageCode.FromDateCanNotBeNull, "");
				txtFromDate.Focus();
				WinCommons._CloseProcessing();
				FormMode = Enums.FormViewModeType.Run;
				return;
			}
			if (string.IsNullOrEmpty(Function.ToString(txtTodate.EditValue)))
			{
				WinCommons.Error(Enums.MessageCode.ToDateCanNotBeNull, "");
				txtTodate.Focus();
				WinCommons._CloseProcessing();
				FormMode = Enums.FormViewModeType.Run;
				return;
			}
			MaterialShortageReport_Impl input = new MaterialShortageReport_Impl
			{
				Type = Function.ToString(txtType.EditValue),
				FromDate = Function.ParseDateTime(txtFromDate.EditValue),
				ToDate = Function.ParseDateTime(txtTodate.EditValue),
				XcludProductionOrder = Function.ToString(txtXclud.EditValue)
			};
			PivotGridControlEx pivotGridControlEx = pgc_Doc;
			pivotGridControlEx.DataSource = await _httpClient.MaterialShortageReportClient.Run(input);
			pgc_Doc.DataSourceTable().AcceptChanges();
			FormMode = _currentState;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			FormMode = Enums.FormViewModeType.Run;
		}
		WinCommons._CloseProcessing();
	}

	private string GetDataDoc(ref DataTable doc)
	{
		if (_docStructure == null || _docStructure.Columns.Count == 0)
		{
			return "Could not get data for function. Please restart program";
		}
		doc = _docStructure.Clone();
		DataRow dataRow = doc.NewRow();
		if (string.IsNullOrEmpty(Function.ToString(txtType.EditValue)))
		{
			txtType.Focus();
			return "You must choose type Atp";
		}
		if (string.IsNullOrEmpty(Function.ToString(txtFromDate.EditValue)))
		{
			txtFromDate.Focus();
			return "You must enter from date";
		}
		if (string.IsNullOrEmpty(Function.ToString(txtTodate.EditValue)))
		{
			txtTodate.Focus();
			return "You must enter to date";
		}
		if (string.IsNullOrEmpty(Function.ToString(txtStatus.EditValue)))
		{
			txtStatus.Focus();
			return "You must enter Atp status";
		}
		if (Function.ParseDateTimes(txtFromDate.EditValue).Date > Function.ParseDateTimes(txtTodate.EditValue))
		{
			txtFromDate.Focus();
			return "To date must be greater than or equal to from date";
		}
		dataRow["Type"] = Function.ToString(txtType.EditValue);
		dataRow["Code"] = Function.ParseInt(txtCode.EditValue);
		dataRow["Name"] = Function.ToString(txtName.EditValue);
		dataRow["FromDate"] = Function.ParseDateTime(txtFromDate.EditValue);
		dataRow["ToDate"] = Function.ParseDateTime(txtTodate.EditValue);
		dataRow["Status"] = Function.ToString(txtStatus.EditValue);
		dataRow["BplId"] = Function.ParseInt(txtBranch.EditValue);
		dataRow["Remarks"] = Function.ToString(txtRemark.EditValue);
		dataRow["ExcludeOrder"] = Function.ToString(txtXclud.EditValue);
		doc.Rows.Add(dataRow);
		doc.AcceptChanges();
		return "";
	}

	private async Task Save()
	{
		WinCommons._OpenProcessing();
		Enums.FormViewModeType oldView = FormMode;
		try
		{
			FormMode = Enums.FormViewModeType.Busy;
			DataTable doc = new DataTable();
			string dataDoc = GetDataDoc(ref doc);
			if (!string.IsNullOrEmpty(dataDoc))
			{
				FormMode = oldView;
				WinCommons.Error(Enums.MessageCode.None, dataDoc);
				return;
			}
			doc.TableName = "Doc";
			DataTable dataTable = pgc_Doc.DataSourceTable().Copy();
			if (!dataTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DataResultIsNullCannotSave, "");
				FormMode = oldView;
				return;
			}
			dataTable.TableName = "Line";
			DataSet dataSet = new DataSet();
			dataSet.Tables.AddRange(new DataTable[2] { doc, dataTable });
			HttpResult httpResult = ((_currentState != Enums.FormViewModeType.Add) ? (await _httpClient.MaterialShortageReportClient.Update(dataSet)) : (await _httpClient.MaterialShortageReportClient.AddNew(dataSet)));
			HttpResult httpResult2 = httpResult;
			if (!string.IsNullOrEmpty(httpResult2.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult2.Message);
				FormMode = oldView;
				return;
			}
			_isLoad = true;
			WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			txtCode.EditValue = Function.ParseInt(httpResult2.Content);
			LoadStatus(Function.ToString(txtStatus.EditValue));
			oldView = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = oldView;
		WinCommons._CloseProcessing();
		_isLoad = false;
	}

	private async Task GetDocStructure()
	{
		try
		{
			_docStructure = await _httpClient.MaterialShortageReportClient.GetDocStructure();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void InitFormatControl()
	{
		try
		{
			txtFromDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			txtTodate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			field_OpenQty.SetFormatControl(Enums.DisplayFormatType.Qty);
			field_PlanQty.SetFormatControl(Enums.DisplayFormatType.Qty);
			field_Qty.SetFormatControl(Enums.DisplayFormatType.Qty);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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

	private async Task SearchData(Enums.NavigateType type)
	{
		Enums.FormViewModeType oldView = FormMode;
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			DataSet dataSet = await _httpClient.MaterialShortageReportClient.SearchData(type, Function.ParseInt(txtCode.EditValue));
			if (dataSet == null || dataSet.Tables.Count != 2 || !dataSet.Tables[0].IsNotNull())
			{
				if (type == Enums.NavigateType.Current)
				{
					WinCommons.Error(Enums.MessageCode.CannotFindMaterialShortageReport, "");
				}
			}
			else
			{
				SetDataDocControl(dataSet.Tables[0]);
				pgc_Doc.DataSource = dataSet.Tables[1];
				pgc_Doc.DataSourceTable().AcceptChanges();
				oldView = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = oldView;
	}

	private void SetDataDocControl(DataTable dt)
	{
		try
		{
			_isLoad = true;
			if (dt.IsNotNull())
			{
				txtBranch.EditValue = Function.ParseInt(dt.Rows[0]["BplId"]);
				txtCode.EditValue = Function.ParseInt(dt.Rows[0]["Code"]);
				txtName.EditValue = Function.ToString(dt.Rows[0]["Name"]);
				txtFromDate.EditValue = Function.ParseDateTime(dt.Rows[0]["FromDate"]);
				txtTodate.EditValue = Function.ParseDateTime(dt.Rows[0]["ToDate"]);
				txtStatus.EditValue = Function.ToString(dt.Rows[0]["Status"]);
				txtType.EditValue = Function.ToString(dt.Rows[0]["Type"]);
				txtRemark.EditValue = Function.ToString(dt.Rows[0]["Remarks"]);
				txtXclud.EditValue = Function.ToString(dt.Rows[0]["ExcludeOrder"]);
			}
			else
			{
				DataTable dataTable = txtType.Properties.DataSource as DataTable;
				txtBranch.EditValue = GlobalData.DefaultBranch;
				txtCode.EditValue = null;
				txtName.EditValue = null;
				txtFromDate.EditValue = null;
				txtTodate.EditValue = null;
				txtStatus.EditValue = "D";
				txtType.EditValue = (dataTable.IsNotNull() ? Function.ToString(dataTable.Rows[0]["Code"]) : null);
				txtRemark.EditValue = null;
				txtXclud.EditValue = null;
			}
			LoadStatus(Function.ToString(txtStatus.EditValue));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoad = false;
	}

	private void EnableDisableControl(bool isEnable)
	{
		try
		{
			txtBranch.ReadOnly = !isEnable;
			txtBranch.Properties.Buttons[0].Visible = isEnable;
			txtCode.ReadOnly = !isEnable;
			txtCode.Properties.Buttons[0].Visible = isEnable;
			txtFromDate.ReadOnly = !isEnable;
			txtFromDate.Properties.Buttons[0].Visible = isEnable;
			txtName.ReadOnly = !isEnable;
			txtStatus.ReadOnly = !isEnable;
			txtStatus.Properties.Buttons[0].Visible = isEnable;
			txtTodate.ReadOnly = !isEnable;
			txtTodate.Properties.Buttons[0].Visible = isEnable;
			txtType.ReadOnly = !isEnable;
			txtType.Properties.Buttons[0].Visible = isEnable;
			txtRemark.ReadOnly = !isEnable;
			txtXclud.ReadOnly = !isEnable;
			txtXclud.Properties.Buttons[0].Visible = isEnable;
			btnCancel.Enabled = isEnable;
			btnRun.Enabled = isEnable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetColorControll(Enums.FormViewModeType type)
	{
		try
		{
			txtBranch.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtCode.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtFromDate.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtName.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtStatus.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtTodate.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtType.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtRemark.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
			txtXclud.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_MaterialShortageReport_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			InitFormatControl();
			await LoadAtpType();
			LoadDataBranch();
			await GetDocStructure();
			FormMode = Enums.FormViewModeType.Find;
			if (!string.IsNullOrEmpty(_materialCode))
			{
				txtCode.EditValue = _materialCode;
				_materialCode = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtXclud_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ProductionOrder, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count <= 0)
				{
					return;
				}
				string text = "";
				for (int i = 0; i < frm_CFL2.ChooseItems.Count; i++)
				{
					if (!string.IsNullOrEmpty(Function.ToString(frm_CFL2.ChooseItems[i]["DocEntry"])))
					{
						text = ((!string.IsNullOrEmpty(text)) ? (text + "," + Function.ToString(frm_CFL2.ChooseItems[i]["DocEntry"])) : Function.ToString(frm_CFL2.ChooseItems[i]["DocEntry"]));
					}
				}
				txtXclud.EditValue = text;
				return;
			}
			string text2 = Function.ToString(txtXclud.EditValue);
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
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.MaterialShortageReport, multiSelect: false, "")
				{
					ListXcludeOrder = text2
				};
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					docEntry = Function.ParseInt(frm_ChooseFromList2.ChooseItem["DocEntry"]);
				}
			}
			frm_ProductionOrder frm_ProductionOrder2 = new frm_ProductionOrder();
			frm_ProductionOrder2.MdiParent = MdiParent;
			frm_ProductionOrder2.DocEntry = docEntry;
			frm_ProductionOrder2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_MaterialShortageReport_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Run) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
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

	private async void btnRun_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Run:
				await Run();
				break;
			case Enums.FormViewModeType.Update:
			case Enums.FormViewModeType.Add:
				await Save();
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			case Enums.FormViewModeType.Find:
				txtCode_ButtonClick(null, null);
				break;
			case Enums.FormViewModeType.Busy:
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void Control_EditValueChanged(object sender, EventArgs e)
	{
		if (!_isLoad && sender is ButtonEdit && (sender as ButtonEdit).Name.Equals(txtCode.Name))
		{
			await SearchData(Enums.NavigateType.Current);
		}
	}

	private void txtCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.MaterialShortageReport, false, Function.ToString(txtCode.EditValue), "", Enums.ItemObjectType.NoItemType, null, "")
			{
				MaterialShortageType = Function.ToString(txtType.EditValue)
			};
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				txtCode.EditValue = Function.ParseInt(frm_CFL2.ChooseItem["Code"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pgc_Doc_CustomAppearance(object sender, PivotCustomAppearanceEventArgs e)
	{
		try
		{
			if (e == null || e.ColumnField == null || !e.ColumnField.FieldName.Equals(field_targets.FieldName))
			{
				return;
			}
			if (e.RowValueType == PivotGridValueType.Total)
			{
				e.Appearance.BackColor = Color.PapayaWhip;
				e.Appearance.BackColor2 = Color.PapayaWhip;
				return;
			}
			Point focusedCell = pgc_Doc.Cells.FocusedCell;
			PivotCellEventArgs cellInfo = pgc_Doc.Cells.GetCellInfo(focusedCell.X, focusedCell.Y);
			if (e.RowIndex == cellInfo.RowIndex)
			{
				e.Appearance.BackColor = Color.Wheat;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_MaterialShortageReport_OnControlValueChanged(object sender, ControlChangedEventArgs e)
	{
		try
		{
			if (_isLoad)
			{
				return;
			}
			if (e.FieldName.Equals(txtFromDate.FieldName) || e.FieldName.Equals(txtTodate.FieldName))
			{
				DateTime? dateTime = Function.ParseDateTime(e.Value);
				if (e.FieldName.Equals(txtFromDate.FieldName))
				{
					if (!dateTime.HasValue || dateTime.Value < DateTime.Now.Date)
					{
						txtFromDate.EditValue = DateTime.Now.Date;
						return;
					}
					DateTime? dateTime2 = Function.ParseDateTime(txtTodate.EditValue);
					if (!dateTime2.HasValue || dateTime.Value > dateTime2.Value)
					{
						txtTodate.EditValue = dateTime.Value;
					}
				}
				else if (e.FieldName.Equals(txtTodate.FieldName))
				{
					DateTime? dateTime3 = Function.ParseDateTime(txtFromDate.EditValue);
					if (!dateTime.HasValue)
					{
						if (!dateTime3.HasValue)
						{
							txtTodate.EditValue = DateTime.Now.Date;
						}
						else
						{
							txtTodate.EditValue = txtFromDate.EditValue;
						}
						return;
					}
					if (dateTime.Value < DateTime.Now.Date)
					{
						txtTodate.EditValue = DateTime.Now.Date;
						return;
					}
					if (!dateTime3.HasValue || dateTime.Value < dateTime3.Value)
					{
						txtFromDate.EditValue = dateTime.Value;
					}
				}
				if (FormMode != Enums.FormViewModeType.Find)
				{
					if (FormMode != Enums.FormViewModeType.Run)
					{
						_currentState = ((FormMode == Enums.FormViewModeType.Add) ? Enums.FormViewModeType.Add : Enums.FormViewModeType.Update);
					}
					FormMode = Enums.FormViewModeType.Run;
				}
			}
			else if (FormMode == Enums.FormViewModeType.View)
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Production.frm_MaterialShortageReport));
		this.rCalEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.txtType = new Apzon.Controls.Editor.LookUpEditEx();
		this.txtFromDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtTodate = new Apzon.Controls.Editor.DateEditEx();
		this.txtXclud = new Apzon.Controls.Editor.ButtonEditEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnRun = new Apzon.Controls.Controls.SimpleButtonEx();
		this.pgc_Doc = new Apzon.Controls.Pivot.PivotGridControlEx();
		this.field_WoID = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_ProductNo = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_ProductName = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_PlanQty = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_OpenQty = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_Components = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_targets = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_Qty = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_ProductUoM = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_ComponentUoM = new DevExpress.XtraPivotGrid.PivotGridField();
		this.lbXclud = new Apzon.Controls.Controls.LabelLine();
		this.lbToDate = new Apzon.Controls.Controls.LabelLine();
		this.lbFromDate = new Apzon.Controls.Controls.LabelLine();
		this.lbType = new Apzon.Controls.Controls.LabelLine();
		this.txtStatus = new Apzon.Controls.Editor.LookUpEditEx();
		this.lbStatus = new Apzon.Controls.Controls.LabelLine();
		this.txtCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.txtBranch = new Apzon.Controls.Editor.LookUpEditEx();
		this.txtRemark = new Apzon.Controls.Editor.TextEditEx();
		this.txtName = new Apzon.Controls.Editor.TextEditEx();
		this.lbCode = new Apzon.Controls.Controls.LabelLine();
		this.lbBranch = new Apzon.Controls.Controls.LabelLine();
		this.lbRemarks = new Apzon.Controls.Controls.LabelLine();
		this.lbName = new Apzon.Controls.Controls.LabelLine();
		this.pivotGridField2 = new DevExpress.XtraPivotGrid.PivotGridField();
		this.pivotGridField3 = new DevExpress.XtraPivotGrid.PivotGridField();
		((System.ComponentModel.ISupportInitialize)this.rCalEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtTodate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtTodate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtXclud.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pgc_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBranch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtRemark.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtName.Properties).BeginInit();
		base.SuspendLayout();
		this.rCalEdit.AutoHeight = false;
		this.rCalEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, true, true, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rCalEdit.Name = "rCalEdit";
		this.txtType.EditControl = null;
		this.txtType.FieldName = "Type";
		this.txtType.IsSearchControl = false;
		this.txtType.Location = new System.Drawing.Point(93, 7);
		this.txtType.Name = "txtType";
		this.txtType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name3"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name4", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.txtType.Properties.DisplayMember = "Name";
		this.txtType.Properties.NullText = "";
		this.txtType.Properties.ShowFooter = false;
		this.txtType.Properties.ShowHeader = false;
		this.txtType.Properties.ValueMember = "Code";
		this.txtType.SettingInVisible = false;
		this.txtType.SettingReadonly = false;
		this.txtType.Size = new System.Drawing.Size(120, 20);
		this.txtType.TabIndex = 1;
		this.txtFromDate.EditControl = null;
		this.txtFromDate.EditValue = null;
		this.txtFromDate.FieldName = "FromDate";
		this.txtFromDate.Location = new System.Drawing.Point(93, 32);
		this.txtFromDate.Name = "txtFromDate";
		this.txtFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.IsSearchControl = false;
		this.txtFromDate.SettingInVisible = false;
		this.txtFromDate.SettingReadonly = false;
		this.txtFromDate.Size = new System.Drawing.Size(120, 20);
		this.txtFromDate.TabIndex = 5;
		this.txtTodate.EditControl = null;
		this.txtTodate.EditValue = null;
		this.txtTodate.FieldName = "ToDate";
		this.txtTodate.Location = new System.Drawing.Point(308, 32);
		this.txtTodate.Name = "txtTodate";
		this.txtTodate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtTodate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtTodate.Properties.IsSearchControl = false;
		this.txtTodate.SettingInVisible = false;
		this.txtTodate.SettingReadonly = false;
		this.txtTodate.Size = new System.Drawing.Size(120, 20);
		this.txtTodate.TabIndex = 6;
		this.txtXclud.EditControl = null;
		this.txtXclud.FieldName = "ExcludeOrder";
		this.txtXclud.IsSearchControl = false;
		this.txtXclud.Location = new System.Drawing.Point(1021, 31);
		this.txtXclud.Name = "txtXclud";
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.txtXclud.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtXclud.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtXclud_Properties_ButtonClick);
		this.txtXclud.SettingInVisible = false;
		this.txtXclud.SettingReadonly = false;
		this.txtXclud.Size = new System.Drawing.Size(176, 20);
		this.txtXclud.TabIndex = 9;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(1183, 543);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 11;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnRun.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnRun.EditControl = null;
		this.btnRun.EditValue = "Run";
		this.btnRun.FieldName = null;
		this.btnRun.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnRun.ImageOptions.Image");
		this.btnRun.IsSearchControl = false;
		this.btnRun.Location = new System.Drawing.Point(1100, 543);
		this.btnRun.Name = "btnRun";
		this.btnRun.ReadOnly = false;
		this.btnRun.SettingInVisible = false;
		this.btnRun.SettingReadonly = false;
		this.btnRun.Size = new System.Drawing.Size(75, 23);
		this.btnRun.TabIndex = 10;
		this.btnRun.Text = "Run";
		this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
		this.pgc_Doc.AllowPasteExcel = false;
		this.pgc_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pgc_Doc.ClipboardData = null;
		this.pgc_Doc.Fields.AddRange(new DevExpress.XtraPivotGrid.PivotGridField[10] { this.field_WoID, this.field_ProductNo, this.field_ProductName, this.field_PlanQty, this.field_OpenQty, this.field_Components, this.field_targets, this.field_Qty, this.field_ProductUoM, this.field_ComponentUoM });
		this.pgc_Doc.FilterFieldControl = null;
		this.pgc_Doc.FilterValueControl = null;
		this.pgc_Doc.FormId = null;
		this.pgc_Doc.ItemId = null;
		this.pgc_Doc.Location = new System.Drawing.Point(1, 61);
		this.pgc_Doc.Name = "pgc_Doc";
		this.pgc_Doc.ObjectType = Apzon.Commons.ApzObjectTypes.oNone;
		this.pgc_Doc.OptionsView.ShowButtonMode = DevExpress.XtraPivotGrid.PivotShowButtonModeEnum.ShowAlways;
		this.pgc_Doc.OptionsView.ShowFilterHeaders = false;
		this.pgc_Doc.OptionsView.ShowRowGrandTotals = false;
		this.pgc_Doc.Size = new System.Drawing.Size(1268, 476);
		this.pgc_Doc.TabIndex = 0;
		this.pgc_Doc.CustomAppearance += new DevExpress.XtraPivotGrid.PivotCustomAppearanceEventHandler(this.pgc_Doc_CustomAppearance);
		this.field_WoID.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_WoID.AreaIndex = 0;
		this.field_WoID.Caption = "W/O ID";
		this.field_WoID.FieldName = "WOID";
		this.field_WoID.Name = "field_WoID";
		this.field_ProductNo.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_ProductNo.AreaIndex = 1;
		this.field_ProductNo.Caption = "Product No";
		this.field_ProductNo.FieldName = "PrdNo";
		this.field_ProductNo.Name = "field_ProductNo";
		this.field_ProductName.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_ProductName.AreaIndex = 2;
		this.field_ProductName.Caption = "Product Name";
		this.field_ProductName.FieldName = "PrdName";
		this.field_ProductName.Name = "field_ProductName";
		this.field_ProductName.Width = 120;
		this.field_PlanQty.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_PlanQty.AreaIndex = 4;
		this.field_PlanQty.Caption = "Production Plan Qty";
		this.field_PlanQty.FieldEdit = this.rCalEdit;
		this.field_PlanQty.FieldName = "PrdPlnQty";
		this.field_PlanQty.Name = "field_PlanQty";
		this.field_PlanQty.Width = 150;
		this.field_OpenQty.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_OpenQty.AreaIndex = 5;
		this.field_OpenQty.Caption = "Production Open Qty";
		this.field_OpenQty.FieldEdit = this.rCalEdit;
		this.field_OpenQty.FieldName = "PrdOpnQty";
		this.field_OpenQty.Name = "field_OpenQty";
		this.field_OpenQty.Width = 150;
		this.field_Components.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_Components.AreaIndex = 6;
		this.field_Components.Caption = "Components";
		this.field_Components.FieldName = "SubCode";
		this.field_Components.Name = "field_Components";
		this.field_targets.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
		this.field_targets.AreaIndex = 0;
		this.field_targets.Caption = "targets";
		this.field_targets.FieldName = "Note";
		this.field_targets.Name = "field_targets";
		this.field_Qty.Appearance.CellGrandTotal.BackColor = System.Drawing.Color.PeachPuff;
		this.field_Qty.Appearance.CellGrandTotal.BackColor2 = System.Drawing.Color.PeachPuff;
		this.field_Qty.Appearance.CellGrandTotal.Options.UseBackColor = true;
		this.field_Qty.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
		this.field_Qty.AreaIndex = 0;
		this.field_Qty.Caption = "Quantity";
		this.field_Qty.FieldEdit = this.rCalEdit;
		this.field_Qty.FieldName = "Qty";
		this.field_Qty.Name = "field_Qty";
		this.field_Qty.Options.ReadOnly = true;
		this.field_ProductUoM.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_ProductUoM.AreaIndex = 3;
		this.field_ProductUoM.Caption = "Product UoM";
		this.field_ProductUoM.FieldName = "PrdUoM";
		this.field_ProductUoM.Name = "field_ProductUoM";
		this.field_ComponentUoM.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_ComponentUoM.AreaIndex = 7;
		this.field_ComponentUoM.Caption = "Component UoM";
		this.field_ComponentUoM.FieldName = "SubUoM";
		this.field_ComponentUoM.Name = "field_ComponentUoM";
		this.field_ComponentUoM.Width = 125;
		this.lbXclud.EditControl = null;
		this.lbXclud.EditValue = "Exclude Production Order";
		this.lbXclud.FieldName = null;
		this.lbXclud.HideWhenEmpty = false;
		this.lbXclud.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbXclud.Location = new System.Drawing.Point(870, 35);
		this.lbXclud.Name = "lbXclud";
		this.lbXclud.ReadOnly = false;
		this.lbXclud.SettingInVisible = false;
		this.lbXclud.SettingReadonly = false;
		this.lbXclud.ShowUnderLine = true;
		this.lbXclud.Size = new System.Drawing.Size(151, 20);
		this.lbXclud.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbXclud.TabIndex = 0;
		this.lbXclud.Utext = "Exclude Production Order";
		this.lbToDate.EditControl = null;
		this.lbToDate.EditValue = "To Date";
		this.lbToDate.FieldName = null;
		this.lbToDate.HideWhenEmpty = false;
		this.lbToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbToDate.Location = new System.Drawing.Point(228, 35);
		this.lbToDate.Name = "lbToDate";
		this.lbToDate.ReadOnly = false;
		this.lbToDate.SettingInVisible = false;
		this.lbToDate.SettingReadonly = false;
		this.lbToDate.ShowUnderLine = true;
		this.lbToDate.Size = new System.Drawing.Size(80, 20);
		this.lbToDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbToDate.TabIndex = 0;
		this.lbToDate.Utext = "To Date";
		this.lbFromDate.EditControl = null;
		this.lbFromDate.EditValue = "From Date";
		this.lbFromDate.FieldName = null;
		this.lbFromDate.HideWhenEmpty = false;
		this.lbFromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbFromDate.Location = new System.Drawing.Point(13, 35);
		this.lbFromDate.Name = "lbFromDate";
		this.lbFromDate.ReadOnly = false;
		this.lbFromDate.SettingInVisible = false;
		this.lbFromDate.SettingReadonly = false;
		this.lbFromDate.ShowUnderLine = true;
		this.lbFromDate.Size = new System.Drawing.Size(80, 20);
		this.lbFromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbFromDate.TabIndex = 0;
		this.lbFromDate.Utext = "From Date";
		this.lbType.EditControl = null;
		this.lbType.EditValue = "Type";
		this.lbType.FieldName = null;
		this.lbType.HideWhenEmpty = false;
		this.lbType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbType.Location = new System.Drawing.Point(13, 10);
		this.lbType.Name = "lbType";
		this.lbType.ReadOnly = false;
		this.lbType.SettingInVisible = false;
		this.lbType.SettingReadonly = false;
		this.lbType.ShowUnderLine = true;
		this.lbType.Size = new System.Drawing.Size(80, 20);
		this.lbType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbType.TabIndex = 0;
		this.lbType.Utext = "Type";
		this.txtStatus.EditControl = null;
		this.txtStatus.FieldName = "Status";
		this.txtStatus.IsSearchControl = false;
		this.txtStatus.Location = new System.Drawing.Point(737, 31);
		this.txtStatus.Name = "txtStatus";
		this.txtStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtStatus.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name3"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Key", "Name4", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.txtStatus.Properties.DisplayMember = "Value";
		this.txtStatus.Properties.NullText = "";
		this.txtStatus.Properties.ShowFooter = false;
		this.txtStatus.Properties.ShowHeader = false;
		this.txtStatus.Properties.ValueMember = "Key";
		this.txtStatus.SettingInVisible = false;
		this.txtStatus.SettingReadonly = false;
		this.txtStatus.Size = new System.Drawing.Size(120, 20);
		this.txtStatus.TabIndex = 8;
		this.lbStatus.EditControl = null;
		this.lbStatus.EditValue = "Status";
		this.lbStatus.FieldName = null;
		this.lbStatus.HideWhenEmpty = false;
		this.lbStatus.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbStatus.Location = new System.Drawing.Point(657, 35);
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
		this.txtCode.Location = new System.Drawing.Point(308, 6);
		this.txtCode.Name = "txtCode";
		this.txtCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtCode.Properties.AppearanceFocused.BackColor2 = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtCode.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtCode.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtCode.Properties.AppearanceReadOnly.BackColor2 = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtCode.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txtCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txtCode.SettingInVisible = false;
		this.txtCode.SettingReadonly = false;
		this.txtCode.Size = new System.Drawing.Size(120, 20);
		this.txtCode.TabIndex = 2;
		this.txtCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtCode_ButtonClick);
		this.txtCode.EditValueChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.txtBranch.EditControl = null;
		this.txtBranch.FieldName = "BplId";
		this.txtBranch.IsSearchControl = false;
		this.txtBranch.Location = new System.Drawing.Point(736, 6);
		this.txtBranch.Name = "txtBranch";
		this.txtBranch.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtBranch.Properties.AppearanceFocused.BackColor2 = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtBranch.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtBranch.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtBranch.Properties.AppearanceReadOnly.BackColor2 = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtBranch.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txtBranch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtBranch.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name7"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name8", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.txtBranch.Properties.DisplayMember = "Name";
		this.txtBranch.Properties.NullText = "";
		this.txtBranch.Properties.ShowFooter = false;
		this.txtBranch.Properties.ShowHeader = false;
		this.txtBranch.Properties.ValueMember = "Value";
		this.txtBranch.SettingInVisible = false;
		this.txtBranch.SettingReadonly = false;
		this.txtBranch.Size = new System.Drawing.Size(120, 20);
		this.txtBranch.TabIndex = 4;
		this.txtRemark.EditControl = null;
		this.txtRemark.FieldName = "Remarks";
		this.txtRemark.IsSearchControl = false;
		this.txtRemark.Location = new System.Drawing.Point(522, 32);
		this.txtRemark.Name = "txtRemark";
		this.txtRemark.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtRemark.Properties.AppearanceFocused.BackColor2 = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtRemark.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtRemark.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtRemark.Properties.AppearanceReadOnly.BackColor2 = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtRemark.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txtRemark.SettingInVisible = false;
		this.txtRemark.SettingReadonly = false;
		this.txtRemark.Size = new System.Drawing.Size(120, 20);
		this.txtRemark.TabIndex = 7;
		this.txtName.EditControl = null;
		this.txtName.FieldName = "Name";
		this.txtName.IsSearchControl = false;
		this.txtName.Location = new System.Drawing.Point(522, 6);
		this.txtName.Name = "txtName";
		this.txtName.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtName.Properties.AppearanceFocused.BackColor2 = System.Drawing.Color.FromArgb(255, 224, 192);
		this.txtName.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtName.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtName.Properties.AppearanceReadOnly.BackColor2 = System.Drawing.Color.FromArgb(224, 224, 224);
		this.txtName.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		this.txtName.SettingInVisible = false;
		this.txtName.SettingReadonly = false;
		this.txtName.Size = new System.Drawing.Size(120, 20);
		this.txtName.TabIndex = 3;
		this.lbCode.EditControl = null;
		this.lbCode.EditValue = "Code";
		this.lbCode.FieldName = null;
		this.lbCode.HideWhenEmpty = false;
		this.lbCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbCode.Location = new System.Drawing.Point(228, 10);
		this.lbCode.Name = "lbCode";
		this.lbCode.ReadOnly = false;
		this.lbCode.SettingInVisible = false;
		this.lbCode.SettingReadonly = false;
		this.lbCode.ShowUnderLine = true;
		this.lbCode.Size = new System.Drawing.Size(80, 20);
		this.lbCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbCode.TabIndex = 0;
		this.lbCode.Utext = "Code";
		this.lbBranch.EditControl = null;
		this.lbBranch.EditValue = "Branch";
		this.lbBranch.FieldName = null;
		this.lbBranch.HideWhenEmpty = false;
		this.lbBranch.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbBranch.Location = new System.Drawing.Point(656, 10);
		this.lbBranch.Name = "lbBranch";
		this.lbBranch.ReadOnly = false;
		this.lbBranch.SettingInVisible = false;
		this.lbBranch.SettingReadonly = false;
		this.lbBranch.ShowUnderLine = true;
		this.lbBranch.Size = new System.Drawing.Size(80, 20);
		this.lbBranch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbBranch.TabIndex = 0;
		this.lbBranch.Utext = "Branch";
		this.lbRemarks.EditControl = null;
		this.lbRemarks.EditValue = "Remarks";
		this.lbRemarks.FieldName = null;
		this.lbRemarks.HideWhenEmpty = false;
		this.lbRemarks.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbRemarks.Location = new System.Drawing.Point(442, 35);
		this.lbRemarks.Name = "lbRemarks";
		this.lbRemarks.ReadOnly = false;
		this.lbRemarks.SettingInVisible = false;
		this.lbRemarks.SettingReadonly = false;
		this.lbRemarks.ShowUnderLine = true;
		this.lbRemarks.Size = new System.Drawing.Size(80, 20);
		this.lbRemarks.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbRemarks.TabIndex = 0;
		this.lbRemarks.Utext = "Remarks";
		this.lbName.EditControl = null;
		this.lbName.EditValue = "Name";
		this.lbName.FieldName = null;
		this.lbName.HideWhenEmpty = false;
		this.lbName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbName.Location = new System.Drawing.Point(442, 10);
		this.lbName.Name = "lbName";
		this.lbName.ReadOnly = false;
		this.lbName.SettingInVisible = false;
		this.lbName.SettingReadonly = false;
		this.lbName.ShowUnderLine = true;
		this.lbName.Size = new System.Drawing.Size(80, 20);
		this.lbName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbName.TabIndex = 0;
		this.lbName.Utext = "Name";
		this.pivotGridField2.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.pivotGridField2.AreaIndex = 2;
		this.pivotGridField2.Caption = "Product Name";
		this.pivotGridField2.FieldName = "PrdName";
		this.pivotGridField2.Name = "pivotGridField2";
		this.pivotGridField2.Width = 120;
		this.pivotGridField3.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.pivotGridField3.AreaIndex = 2;
		this.pivotGridField3.Caption = "Product Name";
		this.pivotGridField3.FieldName = "PrdName";
		this.pivotGridField3.Name = "pivotGridField3";
		this.pivotGridField3.Width = 120;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1270, 569);
		base.Controls.Add(this.txtCode);
		base.Controls.Add(this.txtBranch);
		base.Controls.Add(this.txtRemark);
		base.Controls.Add(this.txtName);
		base.Controls.Add(this.lbCode);
		base.Controls.Add(this.lbBranch);
		base.Controls.Add(this.lbRemarks);
		base.Controls.Add(this.lbName);
		base.Controls.Add(this.txtStatus);
		base.Controls.Add(this.lbStatus);
		base.Controls.Add(this.pgc_Doc);
		base.Controls.Add(this.btnRun);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.txtXclud);
		base.Controls.Add(this.txtTodate);
		base.Controls.Add(this.txtFromDate);
		base.Controls.Add(this.txtType);
		base.Controls.Add(this.lbXclud);
		base.Controls.Add(this.lbToDate);
		base.Controls.Add(this.lbFromDate);
		base.Controls.Add(this.lbType);
		base.FormId = "oMaterialShortageReport";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_MaterialShortageReport";
		this.Text = "Material Shortage Report";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_MaterialShortageReport_Load);
		base.OnControlValueChanged += new Apzon.Controls.Form.ControlValueChanged(this.frm_MaterialShortageReport_OnControlValueChanged);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_MaterialShortageReport_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.rCalEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtTodate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtTodate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtXclud.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pgc_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBranch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtRemark.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtName.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
