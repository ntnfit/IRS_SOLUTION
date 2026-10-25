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
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.PM;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout;
using DevExpress.XtraTab;

namespace Apzon.Forms.UserControls;

public class ucTrackingIssue : XtraUserControl
{
	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	private DataTable _dtdoc;

	private DataTable _dtHistory;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public bool _isLoading;

	public bool _isChangStatus;

	public DataRow DataRowFromCal;

	private Enums.FormViewModeType _formView;

	public Func<Task> RefreshSettings;

	public Func<int, Task> SendDataBackToMain;

	private int employeeId;

	public EventHandler RefeshGrid_Click;

	public bool _isSuccess;

	public bool _isSearch;

	public bool CallFromCal;

	private readonly Dictionary<string, string> _lstPriority = new Dictionary<string, string>
	{
		{ "4", "Critical" },
		{ "3", "High" },
		{ "2", "Medium" },
		{ "1", "Low" }
	};

	private IContainer components;

	public TextEditEx txt_ServiceCall;

	public SimpleButtonEx btn_ChangeStatus;

	public DateEditEx date_IDate;

	public DateEditEx date_rDate;

	public LabelLine labelLine4;

	public LabelLine labelLine7;

	public LabelLine labelLine8;

	public LabelLine labelLine9;

	public LabelLine labelLine10;

	public LabelLine labelLine21;

	public LookUpEditDefineNew luk_Type;

	public ButtonEditCFL btn_sBy;

	public LookUpEditSimple luk_priority;

	public LabelLine labelLine16;

	public TextEditEx txt_solutionDesc;

	public ButtonEditCFL btn_report;

	public ButtonEditCFL btn_solution;

	public ButtonEditCFL btn_handle;

	public LabelLine labelLine20;

	public LabelLine labelLine13;

	public LabelLine labelLine19;

	public LabelLine labelLine14;

	public ButtonEditCFL btn_prOw;

	public LabelLine labelLine15;

	public ButtonEditCFL btn_pr;

	public ButtonEditCFL btn_bp;

	public ButtonEditCFL btn_taskCode;

	public LabelLine labelLine17;

	public TextEditEx txt_taskName;

	public LabelLine labelLine22;

	public TextEditEx txt_Id;

	public LabelLine labelLine23;

	public XtraTabControlEx xtraTabControl1;

	public XtraTabPageEx xtraTabPageAddInfo;

	public LabelLine labelLine27;

	public MemoEditEx memo_Summary;

	public LabelLine labelLine11;

	public MemoEditEx txt_note;

	public ucAttachment ucAttachment1;

	public LayoutUDF layout_UDF;

	public LayoutControlGroup Root;

	public XtraTabPageEx xtraTabPageComment;

	public ucComment ucComment1;

	public XtraTabPageEx xtraTabPageHistory;

	public SimpleButtonEx btn_Ok;

	public GridControlEx grd_History;

	public GridViewEx gv_History;

	public GridColumnEx gridColumnEx16;

	public GridColumnEx gridColumnEx17;

	public GridColumnEx gridColumnEx18;

	public GridColumnEx gridColumnEx19;

	public RepositoryItemButtonEditCFL rep_btnUserSign;

	public GridColumnEx gridColumnEx20;

	public GridColumnEx gridColumnEx21;

	public GridColumnEx gridColumnEx22;

	public GridColumnEx gridColumnEx23;

	public GridColumnEx gridColumnEx5;

	public GridViewEx gridViewEx1;

	public DataTable dataTable1;

	public DataTable dataTable2;

	public DataTable dataTable3;

	public DataTable dataTable4;

	public TextEditEx txt_attact;

	public ButtonEditCFL btn_user1;

	private SplitContainerControl splitContainerControl1;

	private SplitContainerControl splitContainerControl2;

	private SplitContainerControl splitContainerControl3;

	public LookUpEditDefineNew luk_Status;

	public LookUpEditDefineNew luk_ChangeStatus;

	public GridViewEx GvTrackingIssue { get; set; }

	public string FormId
	{
		get
		{
			return ucAttachment1.FormId;
		}
		set
		{
			ucAttachment ucAttachment2 = ucAttachment1;
			string formId = (gv_History.FormId = value);
			ucAttachment2.FormId = formId;
		}
	}

	public string AttachmentItemId
	{
		get
		{
			return ucAttachment1.ItemId;
		}
		set
		{
			ucAttachment1.ItemId = value;
		}
	}

	public string HistoryItemId
	{
		get
		{
			return gv_History.ItemId;
		}
		set
		{
			gv_History.ItemId = value;
		}
	}

	public GridControlEx GrdTrackingIssue { get; set; }

	public Enums.FormViewModeType FormView
	{
		get
		{
			return _formView;
		}
		set
		{
			try
			{
				_formView = value;
				switch (value)
				{
				case Enums.FormViewModeType.Add:
					btn_Ok.Text = "Add";
					btn_ChangeStatus.Enabled = false;
					luk_ChangeStatus.Enabled = false;
					ucComment1.Enabled = false;
					break;
				case Enums.FormViewModeType.Update:
					btn_Ok.Text = "Update";
					btn_ChangeStatus.Enabled = true;
					luk_ChangeStatus.Enabled = true;
					ucComment1.Enabled = true;
					break;
				case Enums.FormViewModeType.View:
					btn_Ok.Text = "OK";
					btn_ChangeStatus.Enabled = true;
					luk_ChangeStatus.Enabled = true;
					ucComment1.Enabled = true;
					break;
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
		}
	}

	public DataTable GetDataTable()
	{
		try
		{
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_TGIE");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["Id"] = Function.ParseInt(txt_Id.EditValue);
			if (!Function.ParseDateTime(date_IDate.EditValue).HasValue)
			{
				dataRow["IssueDate"] = DBNull.Value;
			}
			else
			{
				dataRow["IssueDate"] = Function.ParseDateTime(date_IDate.EditValue);
			}
			dataRow["IssueType"] = Function.ParseInt(luk_Type.EditValue);
			dataRow["Priority"] = Function.ParseInt(luk_priority.EditValue);
			dataRow["Status"] = Function.ParseInt(luk_Status.EditValue);
			dataRow["Reporter"] = Function.ParseInt(btn_report.EditValue);
			dataRow["HandleBy"] = Function.ParseInt(btn_handle.EditValue);
			dataRow["SolutionId"] = Function.ParseInt(btn_solution.EditValue);
			if (!Function.ParseDateTime(date_rDate.EditValue).HasValue)
			{
				dataRow["ResponseDate"] = DBNull.Value;
			}
			else
			{
				dataRow["ResponseDate"] = Function.ParseDateTime(date_rDate.EditValue);
			}
			dataRow["BpCode"] = Function.ToString(btn_bp.EditValue);
			dataRow["ProjectCode"] = Function.ParseInt(btn_pr.EditValue);
			dataRow["SolutionBy"] = Function.ParseInt(btn_sBy.EditValue);
			dataRow["WbsCode"] = Function.ToString(btn_taskCode.EditValue);
			dataRow["AttachedFiled"] = Function.ParseInt(txt_attact.EditValue);
			dataRow["Note"] = Function.ToString(txt_note.EditValue);
			dataRow["ProjectOwner"] = Function.ParseInt(btn_prOw.EditValue);
			dataRow["UserSign"] = Function.ParseInt(btn_user1.EditValue);
			dataRow["UserSign2"] = Function.ParseInt(btn_user1.EditValue);
			dataRow["Summary"] = Function.ToString(memo_Summary.EditValue);
			dataRow["ServiceCall"] = Function.ParseInt(txt_ServiceCall.EditValue);
			foreach (Control control in layout_UDF.Controls)
			{
				if (control is BaseEdit && tableStructure.Columns.Contains(control.Name))
				{
					dataRow[control.Name] = ((BaseEdit)control).EditValue;
				}
			}
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
			return tableStructure;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public ucTrackingIssue()
	{
		InitializeComponent();
		LoadUser();
		Load += OnLoad;
		ucAttachment ucAttachment2 = ucAttachment1;
		ucAttachment2.BrowsedFile = (Action<List<DataRow>>)Delegate.Combine(ucAttachment2.BrowsedFile, (Action<List<DataRow>>)((List<DataRow> rowList) =>
		{
			foreach (DataRow row in rowList)
			{
				FileNameAtc(row, 1);
			}
			if (btn_Ok.Text != "Add")
			{
				btn_Ok.Text = "Update";
				FormView = Enums.FormViewModeType.Update;
			}
		}));
		ucAttachment ucAttachment3 = ucAttachment1;
		ucAttachment3.BeforeDelete = (Action<int>)Delegate.Combine(ucAttachment3.BeforeDelete, (Action<int>)((int i) =>
		{
			FileNameAtc(ucAttachment1.gv_File.GetFocusedDataRow(), 2);
		}));
		ucComment1.BeforeSendComment += UcComment1_BeforeSendComment;
		ucAttachment1.gv_File.Columns["FileName"].VisibleIndex = 0;
		ucAttachment1.gv_File.Columns["trgtPath"].VisibleIndex = 1;
	}

	private async Task LoadUser()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TrackingIssue");
			DataTable dataSource = await http.GetAsync("GetAllUser");
			rep_btnUserSign.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void OnLoad(object sender, EventArgs eventArgs)
	{
		_isLoading = true;
		btn_Ok.Text = "OK";
		FormView = Enums.FormViewModeType.View;
		if (await layout_UDF.LoadControls("APZ_TGIE", ApzObjectTypes.oTrackingIssue) <= 0)
		{
			splitContainerControl3.PanelVisibility = SplitPanelVisibility.Panel1;
		}
		else
		{
			splitContainerControl3.PanelVisibility = SplitPanelVisibility.Both;
		}
		InitChangeDataEvent();
		_isLoading = false;
	}

	public async void LoadControl()
	{
		btn_bp.Properties.ValueMember = "CardCode";
		btn_bp.Properties.TypeCFL = Enums.TypeCFL.Customer;
		btn_bp.Properties.LoadDataSourceWhenEnter = true;
		btn_bp.Properties.RefreshDataSource();
		btn_bp.Properties.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
		btn_handle.Properties.ValueMember = "EmpId";
		btn_handle.Properties.DisplayMember = "lastName";
		btn_handle.Properties.DisplayMember2 = "firstName";
		btn_handle.Properties.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
		await btn_handle.Properties.RefreshDataSourceAsync();
		btn_handle.Properties.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
		btn_prOw.Properties.ValueMember = "EmpId";
		btn_prOw.Properties.DisplayMember = "lastName";
		btn_prOw.Properties.DisplayMember2 = "firstName";
		btn_prOw.Properties.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
		await btn_prOw.Properties.RefreshDataSourceAsync();
		btn_prOw.Properties.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
		btn_report.Properties.ValueMember = "EmpId";
		btn_report.Properties.DisplayMember = "lastName";
		btn_report.Properties.DisplayMember2 = "firstName";
		btn_report.Properties.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
		await btn_report.Properties.RefreshDataSourceAsync();
		btn_report.Properties.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
		btn_sBy.Properties.ValueMember = "USERID";
		btn_sBy.Properties.DisplayMember = "U_NAME";
		btn_sBy.Properties.TypeCFL = Enums.TypeCFL.GetUserList;
		btn_sBy.Properties.TypeDrillDown = ApzObjectTypes.oNone;
		btn_sBy.Properties.RefreshDataSource();
		btn_pr.Properties.ValueMember = "AbsEntry";
		btn_pr.Properties.DisplayMember = "Code";
		btn_pr.Properties.TypeCFL = Enums.TypeCFL.ProjectManager;
		btn_pr.Properties.RefreshDataSource();
		btn_pr.Properties.TypeDrillDown = ApzObjectTypes.oProjectManager;
		RepositoryItemButtonEditCFL properties = btn_pr.Properties;
		properties.HandleChooseItem = (Action<DataRow>)Delegate.Combine(properties.HandleChooseItem, new Action<DataRow>(ProjCode_HandleChooseItem));
		btn_taskCode.Properties.ValueMember = "TaskCode";
		btn_taskCode.Properties.FieldNameInDataRow("TaskCode");
		btn_taskCode.Properties.FieldNameInGrid("WbsCode");
		btn_taskCode.Properties.TypeCFL = Enums.TypeCFL.ProjectTask;
		RepositoryItemButtonEditCFL properties2 = btn_taskCode.Properties;
		properties2.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(properties2.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
		{
			frm.projectId = Function.ParseInt(btn_pr.EditValue);
		}));
		btn_taskCode.Properties.RefreshDataSource();
		btn_taskCode.Properties.TypeDrillDown = ApzObjectTypes.oProjectManager;
		RepositoryItemButtonEditCFL properties3 = btn_taskCode.Properties;
		properties3.HandleChooseItem = (Action<DataRow>)Delegate.Combine(properties3.HandleChooseItem, new Action<DataRow>(TaskCode_HandleChooseItem));
		luk_Type.Properties.ValueMember = "Code";
		luk_Type.Properties.DisplayMember = "Name";
		luk_Type.Properties.FormProperties.TableName = "APZ_IETE";
		await luk_Type.Properties.RefreshDataSourceAsync();
		luk_Status.Properties.ValueMember = "Code";
		luk_Status.Properties.DisplayMember = "Name";
		luk_Status.Properties.FormProperties.TableName = "APZ_OTIS";
		await luk_Status.Properties.RefreshDataSourceAsync();
		luk_ChangeStatus.Properties.ValueMember = "Code";
		luk_ChangeStatus.Properties.DisplayMember = "Name";
		luk_ChangeStatus.Properties.FormProperties.TableName = "APZ_OTIS";
		await luk_ChangeStatus.Properties.RefreshDataSourceAsync();
		btn_solution.Properties.ValueMember = "SltCode";
		btn_solution.Properties.DisplayMember = "Subject";
		btn_solution.Properties.TypeCFL = Enums.TypeCFL.KnowledgeBaseSolutions;
		RepositoryItemButtonEditCFL properties4 = btn_solution.Properties;
		properties4.HandleChooseItem = (Action<DataRow>)Delegate.Combine(properties4.HandleChooseItem, new Action<DataRow>(Solution_HandleChooseItem));
		btn_solution.Properties.RefreshDataSource();
		btn_solution.Properties.TypeDrillDown = ApzObjectTypes.oKnowledgeBaseSolutions;
		luk_priority.SetDataSource(_lstPriority);
	}

	public void Solution_HandleChooseItem(DataRow dr)
	{
		try
		{
			btn_solution.EditValue = dr["SltCode"];
			btn_sBy.EditValue = dr["Owner"];
			txt_solutionDesc.EditValue = dr["Descriptio"];
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void ProjCode_HandleChooseItem(DataRow dr)
	{
		try
		{
			btn_pr.EditValue = dr["AbsEntry"];
			btn_prOw.EditValue = dr["Owner"];
			if (string.IsNullOrEmpty(Function.ToString(btn_bp.EditValue)))
			{
				btn_bp.EditValue = dr["CardCode"];
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void TaskCode_HandleChooseItem(DataRow dr)
	{
		try
		{
			btn_taskCode.EditValue = dr["TaskCode"];
			txt_taskName.EditValue = dr["TaskName"];
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void FileNameAtc(DataRow row, int type)
	{
		try
		{
			string text = Function.ToString(row["FileName"]);
			string text2 = Function.ToString(row["FileExt"]);
			string value = text + "." + text2;
			_dtHistory = grd_History.DatasourceTable();
			DataRow dataRow = _dtHistory.NewRow();
			if (btn_Ok.Text != "Add")
			{
				switch (type)
				{
				case 1:
					dataRow["Id"] = Function.ParseInt(txt_Id.EditValue);
					dataRow["LineNum"] = 0;
					dataRow["FieldName"] = "Attach file";
					dataRow["OriginalValue"] = "";
					dataRow["NewValue"] = value;
					dataRow["UserSign"] = GlobalData.UserID;
					dataRow["TrackingDate"] = DateTime.Now;
					dataRow["ActionDetail"] = "Made changes";
					_dtHistory.Rows.Add(dataRow);
					_dtHistory.AcceptChanges();
					break;
				case 2:
					dataRow["Id"] = Function.ParseInt(txt_Id.EditValue);
					dataRow["LineNum"] = 0;
					dataRow["FieldName"] = "Attach file";
					dataRow["OriginalValue"] = value;
					dataRow["NewValue"] = "";
					dataRow["UserSign"] = GlobalData.UserID;
					dataRow["TrackingDate"] = DateTime.Now;
					dataRow["ActionDetail"] = "Delete file";
					_dtHistory.Rows.Add(dataRow);
					_dtHistory.AcceptChanges();
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

	public void UcComment1_BeforeSendComment(object sender, EventArgs e)
	{
		try
		{
			int empID = Function.ParseInt(btn_handle.EditValue);
			int num = Function.ParseInt(_httpClient.User.GetUserIdByEmpId(empID));
			if (num != 0)
			{
				ucComment1.AdditionNotifyUser.Add(num);
			}
			int empID2 = Function.ParseInt(btn_report.EditValue);
			int num2 = Function.ParseInt(_httpClient.User.GetUserIdByEmpId(empID2));
			if (num2 != 0)
			{
				ucComment1.AdditionNotifyUser.Add(num2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void ucAttachment1_ButtonDelete_Click(object sender, EventArgs e)
	{
		if (btn_Ok.Text != "Add")
		{
			btn_Ok.Text = "Update";
			FormView = Enums.FormViewModeType.Update;
		}
	}

	public void DtHistories(string fileName)
	{
		try
		{
			_dtHistory = grd_History.DataSourceTable();
			if (_dtHistory == null)
			{
				return;
			}
			DataRow[] array = _dtHistory.Select($"FieldName = '{fileName}'  and IsDatabase={0}").ToArray();
			if (array.Count() > 0)
			{
				string text = Function.ToString(array[0]["OriginalValue"]);
				for (int i = 0; i < array.Count() - 1; i++)
				{
					array[i].Delete();
				}
				if (string.Equals(Function.ToString(array[array.Count() - 1]["NewValue"]), text))
				{
					array[array.Count() - 1].Delete();
				}
				else
				{
					array[array.Count() - 1]["OriginalValue"] = text;
				}
				_dtHistory.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void Control_EditValueChanging(object sender, ChangingEventArgs e)
	{
		_ = 1;
		try
		{
			if (!_isLoading && !_isSearch && !(btn_Ok.Text == "Add") && !CallFromCal)
			{
				_dtHistory = grd_History.DatasourceTable();
				DataRow row = _dtHistory.NewRow();
				if (((dynamic)sender).Name.Equals(date_IDate.Name))
				{
					string oldValue = Function.ToString(e.OldValue);
					string newValue = Function.ToString(e.NewValue);
					string fileName = "Issue Date";
					DtRowUpdate(row, oldValue, newValue, fileName, "", "");
					_dtHistory.Rows.Add(row);
					DtHistories(fileName);
					_dtHistory.AcceptChanges();
				}
				if (((dynamic)sender).Name.Equals(date_rDate.Name))
				{
					string oldValue2 = Function.ToString(e.OldValue);
					string newValue2 = Function.ToString(e.NewValue);
					string fileName2 = "Response Date";
					DtRowUpdate(row, oldValue2, newValue2, fileName2, "", "");
					_dtHistory.Rows.Add(row);
					DtHistories(fileName2);
					_dtHistory.AcceptChanges();
				}
				if (((dynamic)sender).Name.Equals(luk_Type.Name))
				{
					string old = Function.ToString(e.OldValue);
					string oldValue3 = await DataType(old);
					string newv = Function.ToString(e.NewValue);
					string newValue3 = await DataType(newv);
					string fileName3 = "Issue Type";
					DtRowUpdate(row, oldValue3, newValue3, fileName3, old, newv);
					_dtHistory.Rows.Add(row);
					DtHistories(fileName3);
					_dtHistory.AcceptChanges();
				}
				if (((dynamic)sender).Name.Equals(luk_priority.Name))
				{
					string text = Function.ToString(e.OldValue);
					string oldValue4 = PriorityInHistory(text);
					string text2 = Function.ToString(e.NewValue);
					string newValue4 = PriorityInHistory(text2);
					string fileName4 = "Priority";
					DtRowUpdate(row, oldValue4, newValue4, fileName4, text, text2);
					_dtHistory.Rows.Add(row);
					DtHistories(fileName4);
					_dtHistory.AcceptChanges();
				}
				if (((dynamic)sender).Name.Equals(btn_report.Name))
				{
					Function.ToString(e.OldValue);
					string oldValue5 = (btn_report.Properties.dictionSource.ContainsKey(Function.ToString(e.OldValue)) ? btn_report.Properties.dictionSource[Function.ToString(e.OldValue)] : "");
					string newValue5 = (btn_report.Properties.dictionSource.ContainsKey(Function.ToString(e.NewValue)) ? btn_report.Properties.dictionSource[Function.ToString(e.NewValue)] : "");
					int num = (btn_handle.Properties.dictionSource.ContainsKey(Function.ToString(e.OldValue)) ? Function.ParseInt(e.OldValue) : 0);
					int num2 = (btn_handle.Properties.dictionSource.ContainsKey(Function.ToString(e.NewValue)) ? Function.ParseInt(e.NewValue) : 0);
					string fileName5 = "Report by";
					DtRowUpdate(row, oldValue5, newValue5, fileName5, Function.ToString(num), Function.ToString(num2));
					_dtHistory.Rows.Add(row);
					DtHistories(fileName5);
					_dtHistory.AcceptChanges();
				}
				if (((dynamic)sender).Name.Equals(btn_solution.Name))
				{
					txt_solutionDesc.EditValue = null;
					btn_sBy.EditValue = null;
					string oldValue6 = (btn_solution.Properties.dictionSource.ContainsKey(Function.ToString(e.OldValue)) ? btn_solution.Properties.dictionSource[Function.ToString(e.OldValue)] : "");
					string newValue6 = (btn_solution.Properties.dictionSource.ContainsKey(Function.ToString(e.NewValue)) ? btn_solution.Properties.dictionSource[Function.ToString(e.NewValue)] : "");
					string fileName6 = "Solution ID";
					DtRowUpdate(row, oldValue6, newValue6, fileName6, Function.ToString(e.OldValue), Function.ToString(e.NewValue));
					_dtHistory.Rows.Add(row);
					DtHistories(fileName6);
					_dtHistory.AcceptChanges();
				}
				if (((dynamic)sender).Name.Equals(btn_taskCode.Name))
				{
					string oldValue7 = (btn_taskCode.Properties.dictionSource.ContainsKey(Function.ToString(e.OldValue)) ? btn_taskCode.Properties.dictionSource[Function.ToString(e.OldValue)] : "");
					string newValue7 = (btn_taskCode.Properties.dictionSource.ContainsKey(Function.ToString(e.NewValue)) ? btn_taskCode.Properties.dictionSource[Function.ToString(e.NewValue)] : "");
					string fileName7 = "Wbs Code";
					DtRowUpdate(row, oldValue7, newValue7, fileName7, Function.ToString(e.OldValue), Function.ToString(e.NewValue));
					_dtHistory.Rows.Add(row);
					DtHistories(fileName7);
					_dtHistory.AcceptChanges();
				}
				if (((dynamic)sender).Name.Equals(btn_bp.Name))
				{
					string oldValue8 = (btn_bp.Properties.dictionSource.ContainsKey(Function.ToString(e.OldValue)) ? btn_bp.Properties.dictionSource[Function.ToString(e.OldValue)] : "");
					string newValue8 = (btn_bp.Properties.dictionSource.ContainsKey(Function.ToString(e.NewValue)) ? btn_bp.Properties.dictionSource[Function.ToString(e.NewValue)] : "");
					string fileName8 = "BP Code";
					DtRowUpdate(row, oldValue8, newValue8, fileName8, Function.ToString(e.OldValue), Function.ToString(e.NewValue));
					_dtHistory.Rows.Add(row);
					DtHistories(fileName8);
					_dtHistory.AcceptChanges();
				}
				if (((dynamic)sender).Name.Equals(btn_pr.Name))
				{
					string oldValue9 = (btn_pr.Properties.dictionSource.ContainsKey(Function.ToString(e.OldValue)) ? btn_pr.Properties.dictionSource[Function.ToString(e.OldValue)] : "");
					string newValue9 = (btn_pr.Properties.dictionSource.ContainsKey(Function.ToString(e.NewValue)) ? btn_pr.Properties.dictionSource[Function.ToString(e.NewValue)] : "");
					string fileName9 = "Project Code";
					DtRowUpdate(row, oldValue9, newValue9, fileName9, Function.ToString(e.OldValue), Function.ToString(e.NewValue));
					_dtHistory.Rows.Add(row);
					DtHistories(fileName9);
					_dtHistory.AcceptChanges();
				}
				if (((dynamic)sender).Name.Equals(btn_user1.Name))
				{
					string oldValue10 = (btn_user1.Properties.dictionSource.ContainsKey(Function.ToString(e.OldValue)) ? btn_user1.Properties.dictionSource[Function.ToString(e.OldValue)] : "");
					string newValue10 = (btn_user1.Properties.dictionSource.ContainsKey(Function.ToString(e.NewValue)) ? btn_user1.Properties.dictionSource[Function.ToString(e.NewValue)] : "");
					string fileName10 = "User sign";
					DtRowUpdate(row, oldValue10, newValue10, fileName10, Function.ToString(e.OldValue), Function.ToString(e.NewValue));
					_dtHistory.Rows.Add(row);
					DtHistories(fileName10);
					_dtHistory.AcceptChanges();
				}
				if (btn_Ok.Text == "OK")
				{
					btn_Ok.Text = "Update";
					FormView = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<string> DataType(string Id)
	{
		try
		{
			DataTable dataTable = await _httpClient.TrackingIssue.GetDataStatusById(Id);
			string result = "";
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ToString(dataTable.Rows[0]["Name"]);
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	public string PriorityInHistory(string value)
	{
		try
		{
			string result = "";
			if (value.Equals("4"))
			{
				result = "Critical";
			}
			else if (value.Equals("3"))
			{
				result = "High";
			}
			else if (value.Equals("2"))
			{
				result = "Medium";
			}
			else if (value.Equals("1"))
			{
				result = "Low";
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	public void DtRowAdd(DataRow row)
	{
		try
		{
			row["Id"] = 0;
			row["LineNum"] = 0;
			row["FieldName"] = "";
			row["OriginalValue"] = "";
			row["NewValue"] = "";
			row["UserSign"] = GlobalData.UserID;
			row["TrackingDate"] = DateTime.Now;
			row["ActionDetail"] = "Created issue";
			row["IsDatabase"] = 0;
			row["OldVal"] = "";
			row["NewVal"] = "";
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void DtRowUpdate(DataRow row, string oldValue, string newValue, string fileName, string oldVal, string newVal)
	{
		try
		{
			if (!(btn_Ok.Text == "Add"))
			{
				row["Id"] = Function.ParseInt(txt_Id.EditValue);
				row["LineNum"] = 0;
				row["FieldName"] = fileName;
				row["OriginalValue"] = oldValue;
				row["NewValue"] = newValue;
				row["UserSign"] = GlobalData.UserID;
				row["TrackingDate"] = DateTime.Now;
				row["ActionDetail"] = "Made changes";
				row["IsDatabase"] = 0;
				row["OldVal"] = oldVal;
				row["NewVal"] = newVal;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void btn_Ok_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (btn_Ok.Text)
			{
			case "Add":
				await Save(1);
				break;
			case "Update":
				await Save(2);
				break;
			case "OK":
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task GetDetailById(int Id)
	{
		_isLoading = true;
		try
		{
			WinCommons._OpenProcessing();
			DataSet ds = await _httpClient.TrackingIssue.GetDataById(Id);
			if (ds.IsNotNull())
			{
				SetDataTable(ds.Tables[0]);
				if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
				{
					Function.ParseInt(ds.Tables[0].Rows[0]["AttachedFiled"]);
					ucAttachment1.LoadAttachmentsFile(Id);
				}
				await ucComment1.RefreshComment(Id);
				grd_History.DataSource = ds.Tables[2];
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
		_isLoading = false;
	}

	public void SetDataTable(DataTable dt)
	{
		try
		{
			_isLoading = true;
			if (dt != null && dt.Rows.Count > 0)
			{
				DataRow dataRow = dt.Rows[0];
				if (dataRow != null)
				{
					txt_Id.EditValue = Function.ParseInt(dataRow["Id"]);
					date_IDate.EditValue = Function.ParseDateTime(dataRow["IssueDate"]);
					luk_Type.EditValue = Function.ToString(dataRow["IssueType"]);
					luk_priority.EditValue = Function.ToString(dataRow["Priority"]);
					luk_Status.EditValue = Function.ToString(dataRow["Status"]);
					if (btn_report.Properties.dictionSource.ContainsKey(Function.ToString(dataRow["Reporter"])))
					{
						btn_report.EditValue = ((Function.ParseInt(dataRow["Reporter"]) > 0) ? dataRow["Reporter"] : "");
					}
					else
					{
						btn_report.EditValue = DBNull.Value;
					}
					btn_handle.EditValue = ((Function.ParseInt(dataRow["HandleBy"]) > 0) ? dataRow["HandleBy"] : "");
					btn_solution.EditValue = ((Function.ParseInt(dataRow["SolutionId"]) > 0) ? dataRow["SolutionId"] : "");
					date_rDate.EditValue = Function.ParseDateTime(dataRow["ResponseDate"]);
					btn_bp.EditValue = Function.ToString(dataRow["BpCode"]);
					btn_pr.EditValue = ((Function.ParseInt(dataRow["ProjectCode"]) > 0) ? dataRow["ProjectCode"] : "");
					btn_sBy.EditValue = ((Function.ParseInt(dataRow["SolutionBy"]) > 0) ? dataRow["SolutionBy"] : "");
					btn_taskCode.EditValue = Function.ToString(dataRow["WbsCode"]);
					txt_attact.EditValue = Function.ToString(dataRow["AttachedFiled"]);
					txt_note.Text = Function.ToString(dataRow["Note"]);
					btn_user1.EditValue = Function.ToString(dataRow["UserSign"]);
					memo_Summary.EditValue = Function.ToString(dataRow["Summary"]);
					txt_solutionDesc.EditValue = Function.ToString(dataRow["SolutionDesc"]);
					txt_taskName.EditValue = Function.ToString(dataRow["TaskName"]);
					btn_prOw.EditValue = ((Function.ParseInt(dataRow["ProjectOwner"]) > 0) ? dataRow["ProjectOwner"] : "");
					txt_ServiceCall.EditValue = ((Function.ParseInt(dataRow["ServiceCall"]) > 0) ? dataRow["ServiceCall"] : "");
					foreach (Control control in layout_UDF.Controls)
					{
						if (control is BaseEdit && dt.Columns.Contains(control.Name))
						{
							((BaseEdit)control).EditValue = dataRow[control.Name];
						}
					}
				}
			}
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void ResetFormControl(object projectId)
	{
		try
		{
			employeeId = await EmployeeId(GlobalData.UserID);
			txt_Id.EditValue = null;
			date_IDate.EditValue = DateTime.Now.Date;
			luk_Type.EditValue = null;
			luk_priority.EditValue = "2";
			luk_Status.EditValue = "1";
			btn_report.EditValue = employeeId;
			btn_handle.EditValue = null;
			btn_solution.EditValue = null;
			date_rDate.EditValue = null;
			btn_bp.EditValue = null;
			btn_pr.EditValue = projectId;
			btn_sBy.EditValue = null;
			btn_taskCode.EditValue = null;
			txt_attact.EditValue = null;
			txt_note.EditValue = null;
			memo_Summary.EditValue = null;
			txt_solutionDesc.EditValue = null;
			txt_taskName.EditValue = null;
			luk_ChangeStatus.EditValue = null;
			btn_prOw.EditValue = null;
			txt_solutionDesc.EditValue = null;
			txt_taskName.EditValue = null;
			txt_ServiceCall.EditValue = null;
			DataTable dataTable = new DataTable();
			dataTable.NewRow();
			_ = dataTable.DefaultView;
			foreach (Control control in layout_UDF.Controls)
			{
				if (control is BaseEdit && btn_Ok.Text == "Add")
				{
					(control as BaseEdit).EditValue = control.AccessibleDescription;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void ResetReadOnly(bool value)
	{
		try
		{
			if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_TrackingIssue" && x.authoriza != "R"))
			{
				txt_Id.ReadOnly = value;
				date_IDate.ReadOnly = value;
				luk_Type.ReadOnly = value;
				luk_priority.ReadOnly = value;
				btn_report.ReadOnly = value;
				btn_handle.ReadOnly = value;
				btn_solution.ReadOnly = value;
				date_rDate.ReadOnly = value;
				btn_bp.ReadOnly = value;
				btn_pr.ReadOnly = value;
				btn_taskCode.ReadOnly = value;
				txt_attact.ReadOnly = value;
				txt_note.ReadOnly = value;
				memo_Summary.ReadOnly = value;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<int> Save(int type)
	{
		_ = 2;
		try
		{
			int Id = Function.ParseInt(txt_Id.EditValue);
			if (string.IsNullOrEmpty(Function.ToString(date_IDate.EditValue)))
			{
				date_IDate.Focus();
				WinCommons.Error(Enums.MessageCode.IssueDateIsMissing, "");
				return 1;
			}
			if (string.IsNullOrEmpty(Function.ToString(date_IDate.EditValue)))
			{
				btn_pr.Focus();
				WinCommons.Error(Enums.MessageCode.ProjectCodeIsMissing, "");
				return 1;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DataSet dataSet = new DataSet();
				_dtdoc = GetDataTable();
				_dtHistory = grd_History.DatasourceTable();
				new DataTable();
				if (!ucAttachment1.GetAttachmentNoSAP(out var dt))
				{
					return 1;
				}
				List<DataRow> list = (from t in dt.AsEnumerable()
					where Function.ToString(t["IsDMS"]) == "Y"
					select t).ToList();
				DataTable dataTable = ((list.Count <= 0) ? WinFuntions.GetTableStructure("APZ_D_UUPIM") : list.CopyToDataTable());
				List<DataRow> list2 = (from t in dt.AsEnumerable()
					where Function.ToString(t["IsDMS"]) != "Y"
					select t).ToList();
				dt = ((list2.Count <= 0) ? WinFuntions.GetTableStructure("ATC1") : list2.CopyToDataTable());
				_dtdoc.TableName = "dtTracking";
				dt.TableName = "AttFile";
				dataSet.Tables.Add(_dtdoc.Copy());
				dataSet.Tables.Add(_dtHistory.Copy());
				dataSet.Tables.Add(dt.Copy());
				dataSet.Tables.Add(dataTable.Copy());
				HttpResult httpResult = ((type != 2) ? (await _httpClient.TrackingIssue.Add(dataSet)) : (await _httpClient.TrackingIssue.Update(dataSet)));
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success((type == 1) ? Enums.MessageCode.SaveSuccess : Enums.MessageCode.UpdateSuccess, "");
					CallFromCal = false;
					int num = Function.ParseInt(httpResult.Content);
					txt_Id.EditValue = num;
					FormView = Enums.FormViewModeType.View;
					_isSuccess = true;
					if (SendDataBackToMain != null && !((XtraForm)SendDataBackToMain.Target).IsDisposed)
					{
						await SendDataBackToMain(num);
					}
					DataTable dataTable2 = GrdTrackingIssue.DatasourceTable();
					if (dataTable2 != null && dataTable2.Rows.Count > 0 && type == 2)
					{
						DataRow[] array = dataTable2.Select($"Id={Id}").ToArray();
						if (array != null && array.Length != 0)
						{
							for (int num2 = 0; num2 < array.Count(); num2++)
							{
								if (btn_report.Properties.dictionSource.ContainsKey(Function.ToString(btn_report.EditValue)))
								{
									_dtdoc.Rows[0]["Reporter"] = ((Function.ParseInt(btn_report.EditValue) > 0) ? _dtdoc.Rows[0]["Reporter"] : DBNull.Value);
								}
								else
								{
									_dtdoc.Rows[0]["Reporter"] = DBNull.Value;
								}
								if (btn_pr.Properties.dictionSource.ContainsKey(Function.ToString(btn_pr.EditValue)))
								{
									_dtdoc.Rows[0]["ProjectCode"] = ((Function.ParseInt(btn_report.EditValue) > 0) ? _dtdoc.Rows[0]["ProjectCode"] : DBNull.Value);
								}
								else
								{
									_dtdoc.Rows[0]["ProjectCode"] = DBNull.Value;
								}
								if (btn_handle.Properties.dictionSource.ContainsKey(Function.ToString(btn_handle.EditValue)))
								{
									_dtdoc.Rows[0]["HandleBy"] = ((Function.ParseInt(btn_handle.EditValue) > 0) ? _dtdoc.Rows[0]["HandleBy"] : DBNull.Value);
								}
								else
								{
									_dtdoc.Rows[0]["HandleBy"] = DBNull.Value;
								}
								array[num2].ItemArray = _dtdoc.Rows[0].ItemArray;
							}
							GrdTrackingIssue.DataSource = dataTable2;
							GrdTrackingIssue.DatasourceTable().AcceptChanges();
						}
					}
					return 0;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return 1;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
		WinCommons._CloseProcessing();
		return 1;
	}

	public async Task<int> EmployeeId(int userId)
	{
		try
		{
			DataTable dataTable = await _httpClient.TrackingIssue.GetEmployeeByUserId(userId);
			if (dataTable.IsNotNull())
			{
				return Function.ParseInt(dataTable.Rows[0]["empId"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private void btn_pr_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(btn_taskCode.EditValue)))
			{
				btn_taskCode.EditValue = null;
				txt_taskName.EditValue = null;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void memo_Summary_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			_dtHistory = grd_History.DatasourceTable();
			DataRow row = _dtHistory.NewRow();
			if (!(btn_Ok.Text != "Add"))
			{
				return;
			}
			string text = Function.ToString(e.OldValue);
			string text2 = Function.ToString(e.NewValue);
			string fileName = "Summary";
			if (!string.IsNullOrEmpty(text2) && !string.Equals(text, text2))
			{
				DtRowUpdate(row, text, text2, fileName, "", "");
				_dtHistory.Rows.Add(row);
				DtHistories(fileName);
				_dtHistory.AcceptChanges();
				if (btn_Ok.Text == "OK")
				{
					btn_Ok.Text = "Update";
					FormView = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_handle_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				string oldValue = (btn_handle.Properties.dictionSource.ContainsKey(Function.ToString(e.OldValue)) ? btn_handle.Properties.dictionSource[Function.ToString(e.OldValue)] : "");
				string text = (btn_handle.Properties.dictionSource.ContainsKey(Function.ToString(e.NewValue)) ? btn_handle.Properties.dictionSource[Function.ToString(e.NewValue)] : "");
				_dtHistory = grd_History.DatasourceTable();
				int num = (btn_handle.Properties.dictionSource.ContainsKey(Function.ToString(e.OldValue)) ? Function.ParseInt(e.OldValue) : 0);
				int num2 = (btn_handle.Properties.dictionSource.ContainsKey(Function.ToString(e.NewValue)) ? Function.ParseInt(e.NewValue) : 0);
				if (btn_Ok.Text != "Add")
				{
					string fileName = "Handle by";
					DataRow row = _dtHistory.NewRow();
					DtRowUpdate(row, oldValue, text, fileName, Function.ToString(num), Function.ToString(num2));
					_dtHistory.Rows.Add(row);
					DtHistories(fileName);
					_dtHistory.AcceptChanges();
				}
				if (!string.IsNullOrEmpty(text) && Function.ParseInt(luk_Status.EditValue) == 1)
				{
					luk_Status.EditValue = "2";
					DataRow row2 = _dtHistory.NewRow();
					string oldValue2 = "New";
					string newValue = "Assigned";
					string fileName2 = "Status";
					DtRowUpdate(row2, oldValue2, newValue, fileName2, "1", "2");
					_dtHistory.Rows.Add(row2);
					DtHistories(fileName2);
					_dtHistory.AcceptChanges();
				}
				if (btn_Ok.Text == "OK")
				{
					btn_Ok.Text = "Update";
					FormView = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_ChangeStatus_Click(object sender, EventArgs e)
	{
		try
		{
			frm_ChangeStatus frm_ChangeStatus2 = new frm_ChangeStatus
			{
				status = Function.ToString(luk_ChangeStatus.EditValue),
				issueId = Function.ParseInt(txt_Id.EditValue),
				_oldValue = Function.ToString(luk_Status.EditValue),
				StartPosition = FormStartPosition.CenterScreen
			};
			frm_ChangeStatus2.SendDataBackToMain = (Func<int, string, string, Task>)Delegate.Combine(frm_ChangeStatus2.SendDataBackToMain, new Func<int, string, string, Task>(ReloadStatus));
			frm_ChangeStatus2.ShowDialog(this);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ReloadStatus(int trackingId, string status, string comment)
	{
		_ = 1;
		try
		{
			_isChangStatus = true;
			int focusedRowHandle = GvTrackingIssue.FocusedRowHandle;
			luk_ChangeStatus.EditValue = status;
			luk_Status.EditValue = status;
			GvTrackingIssue.SetRowCellValue(focusedRowHandle, "Status", status);
			await ucComment1.SaveComment(comment);
			await ucComment1.RefreshComment(trackingId);
			_isChangStatus = false;
			btn_Ok.Text = "OK";
			FormView = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_note_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			_dtHistory = grd_History.DatasourceTable();
			DataRow row = _dtHistory.NewRow();
			if (!(btn_Ok.Text != "Add"))
			{
				return;
			}
			string text = Function.ToString(e.OldValue);
			string text2 = Function.ToString(e.NewValue);
			string fileName = "Note";
			if (!string.IsNullOrEmpty(text2) && !string.Equals(text, text2))
			{
				DtRowUpdate(row, text, text2, fileName, "", "");
				_dtHistory.Rows.Add(row);
				DtHistories(fileName);
				_dtHistory.AcceptChanges();
				if (btn_Ok.Text == "OK")
				{
					btn_Ok.Text = "Update";
					FormView = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task RefreshTracking(int projectId)
	{
		_ = 2;
		try
		{
			if (projectId > 0)
			{
				ResetFormControl(projectId);
			}
			else
			{
				ResetFormControl(null);
			}
			ResetReadOnly(value: false);
			await GetDetailById(0);
			ucAttachment ucAttachment2 = ucAttachment1;
			ucAttachment2.DataSource = await _httpClient.TrackingIssue.GetFileByOjectType("1470000130", "0");
			await ucComment1.RefreshComment(0);
			FormView = Enums.FormViewModeType.Add;
			btn_Ok.Text = "Add";
			grd_History.DataSourceTable.Rows.Clear();
			_dtHistory = grd_History.DataSourceTable;
			if (_dtHistory == null || _dtHistory.Columns.Count == 0)
			{
				XtraMessageBoxEx.Show(Enums.MessageCode.InfoCannotLoadData, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK);
			}
			DataRow row = _dtHistory.NewRow();
			DtRowAdd(row);
			_dtHistory.Rows.Add(row);
			gv_History.EndInsertRow();
			grd_History.DataSource = _dtHistory;
			CallFromCal = false;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CheckChangedData()
	{
		try
		{
			if (!_isChangStatus && !_isSuccess && !_isLoading && !_isSearch)
			{
				_isChangStatus = false;
				if (btn_Ok.Text == "OK")
				{
					btn_Ok.Text = "Update";
					FormView = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Control_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!(sender is LookUpEdit { Name: "luk_ChangeStatus" }) && !(sender is MemoEdit { Name: "memoComment" }))
			{
				CheckChangedData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetChangedEvent(Control control)
	{
		try
		{
			if (control is BaseEdit)
			{
				((BaseEdit)control).EditValueChanged += Control_EditValueChanged;
				return;
			}
			foreach (Control control2 in control.Controls)
			{
				SetChangedEvent(control2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void InitChangeDataEvent()
	{
		try
		{
			foreach (Control control in Controls)
			{
				SetChangedEvent(control);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.ucTrackingIssue));
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		this.txt_ServiceCall = new Apzon.Controls.Editor.TextEditEx();
		this.btn_ChangeStatus = new Apzon.Controls.Controls.SimpleButtonEx();
		this.date_IDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_rDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine21 = new Apzon.Controls.Controls.LabelLine();
		this.luk_Type = new Apzon.Forms.UserControls.LookUpEditDefineNew();
		this.btn_sBy = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.luk_priority = new Apzon.Controls.Editor.MyLookUpEdit.LookUpEditSimple();
		this.labelLine16 = new Apzon.Controls.Controls.LabelLine();
		this.txt_solutionDesc = new Apzon.Controls.Editor.TextEditEx();
		this.btn_report = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btn_solution = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btn_handle = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelLine20 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine13 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine19 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine14 = new Apzon.Controls.Controls.LabelLine();
		this.btn_prOw = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelLine15 = new Apzon.Controls.Controls.LabelLine();
		this.btn_pr = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btn_bp = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btn_taskCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelLine17 = new Apzon.Controls.Controls.LabelLine();
		this.txt_taskName = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine22 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Id = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine23 = new Apzon.Controls.Controls.LabelLine();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPageAddInfo = new Apzon.Controls.Controls.XtraTabPageEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.labelLine27 = new Apzon.Controls.Controls.LabelLine();
		this.memo_Summary = new Apzon.Controls.Editor.MemoEditEx();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		this.txt_note = new Apzon.Controls.Editor.MemoEditEx();
		this.splitContainerControl3 = new DevExpress.XtraEditors.SplitContainerControl();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.layout_UDF = new Apzon.Forms.UserControls.LayoutUDF();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.xtraTabPageComment = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucComment1 = new Apzon.Forms.UserControls.ucComment();
		this.xtraTabPageHistory = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_History = new Apzon.Controls.Grid.GridControlEx();
		this.gv_History = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx19 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_btnUserSign = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridViewEx1 = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_user1 = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.txt_attact = new Apzon.Controls.Editor.TextEditEx();
		this.luk_Status = new Apzon.Forms.UserControls.LookUpEditDefineNew();
		this.luk_ChangeStatus = new Apzon.Forms.UserControls.LookUpEditDefineNew();
		((System.ComponentModel.ISupportInitialize)this.txt_ServiceCall.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_IDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_IDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_rDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_rDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Type.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_sBy.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_priority.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_solutionDesc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_report.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_solution.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_handle.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_prOw.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_pr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_bp.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_taskCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_taskName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Id.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPageAddInfo.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.memo_Summary.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_note.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl3).BeginInit();
		this.splitContainerControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layout_UDF).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		this.xtraTabPageComment.SuspendLayout();
		this.xtraTabPageHistory.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_History).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_History).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnUserSign).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_user1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_attact.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ChangeStatus.Properties).BeginInit();
		base.SuspendLayout();
		this.txt_ServiceCall.FieldName = null;
		this.txt_ServiceCall.IsSearchControl = false;
		this.txt_ServiceCall.Location = new System.Drawing.Point(283, 97);
		this.txt_ServiceCall.Name = "txt_ServiceCall";
		this.txt_ServiceCall.Properties.ReadOnly = true;
		this.txt_ServiceCall.Size = new System.Drawing.Size(35, 20);
		this.txt_ServiceCall.TabIndex = 93;
		this.txt_ServiceCall.Visible = false;
		this.btn_ChangeStatus.FieldName = null;
		this.btn_ChangeStatus.IsSearchControl = false;
		this.btn_ChangeStatus.Location = new System.Drawing.Point(5, 171);
		this.btn_ChangeStatus.Name = "btn_ChangeStatus";
		this.btn_ChangeStatus.Size = new System.Drawing.Size(98, 23);
		this.btn_ChangeStatus.TabIndex = 92;
		this.btn_ChangeStatus.Text = "Change Status To";
		this.btn_ChangeStatus.Click += new System.EventHandler(this.btn_ChangeStatus_Click);
		this.date_IDate.EditValue = null;
		this.date_IDate.FieldName = null;
		this.date_IDate.Location = new System.Drawing.Point(106, 3);
		this.date_IDate.Name = "date_IDate";
		this.date_IDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_IDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_IDate.Properties.IsSearchControl = false;
		this.date_IDate.Size = new System.Drawing.Size(182, 20);
		this.date_IDate.TabIndex = 89;
		this.date_IDate.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.date_rDate.EditValue = null;
		this.date_rDate.FieldName = null;
		this.date_rDate.Location = new System.Drawing.Point(106, 142);
		this.date_rDate.Name = "date_rDate";
		this.date_rDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_rDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_rDate.Properties.IsSearchControl = false;
		this.date_rDate.Size = new System.Drawing.Size(181, 20);
		this.date_rDate.TabIndex = 88;
		this.date_rDate.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(6, 10);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(97, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 15;
		this.labelLine4.Utext = "Issue Date";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(6, 32);
		this.labelLine7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(97, 24);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 16;
		this.labelLine7.Utext = "Issue Type";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(6, 55);
		this.labelLine8.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(97, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 17;
		this.labelLine8.Utext = "Priority";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(6, 78);
		this.labelLine9.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(97, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 18;
		this.labelLine9.Utext = "Status";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(6, 102);
		this.labelLine10.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(97, 20);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 19;
		this.labelLine10.Utext = "Report By";
		this.labelLine21.FieldName = null;
		this.labelLine21.HideWhenEmpty = false;
		this.labelLine21.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine21.Location = new System.Drawing.Point(6, 125);
		this.labelLine21.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine21.Name = "labelLine21";
		this.labelLine21.ShowUnderLine = true;
		this.labelLine21.Size = new System.Drawing.Size(97, 20);
		this.labelLine21.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine21.TabIndex = 30;
		this.labelLine21.Utext = "Handle By";
		this.luk_Type.FieldName = null;
		this.luk_Type.IsSearchControl = false;
		this.luk_Type.Location = new System.Drawing.Point(106, 26);
		this.luk_Type.Name = "luk_Type";
		this.luk_Type.Properties.AutoHeight = false;
		this.luk_Type.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Type.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Type.Properties.CheckFields = null;
		this.luk_Type.Properties.CheckTables = null;
		this.luk_Type.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Type.Properties.NullText = "";
		this.luk_Type.Properties.ShowFooter = false;
		this.luk_Type.Properties.ShowHeader = false;
		this.luk_Type.Size = new System.Drawing.Size(181, 20);
		this.luk_Type.TabIndex = 32;
		this.luk_Type.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.btn_sBy.FieldName = null;
		this.btn_sBy.IsSearchControl = false;
		this.btn_sBy.Location = new System.Drawing.Point(414, 166);
		this.btn_sBy.Name = "btn_sBy";
		this.btn_sBy.Properties.AutoLoadDataSource = true;
		this.btn_sBy.Properties.ContainerControl = null;
		this.btn_sBy.Properties.DisplayMember = "U_NAME";
		this.btn_sBy.Properties.DisplayMember2 = "";
		this.btn_sBy.Properties.LoadDataSourceWhenEnter = false;
		this.btn_sBy.Properties.ReadOnly = true;
		this.btn_sBy.Properties.SeparateCharacterForMulti = ';';
		this.btn_sBy.Properties.SeparateString = ", ";
		this.btn_sBy.Properties.UsingAutoCompleteBox = false;
		this.btn_sBy.Properties.UsingDataSource = true;
		this.btn_sBy.Properties.ValidateWhenNoDataSource = false;
		this.btn_sBy.Properties.ValueMember = "USERID";
		this.btn_sBy.Size = new System.Drawing.Size(212, 20);
		this.btn_sBy.TabIndex = 74;
		this.btn_sBy.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.luk_priority.FieldName = null;
		this.luk_priority.IsSearchControl = false;
		this.luk_priority.Location = new System.Drawing.Point(106, 49);
		this.luk_priority.Name = "luk_priority";
		this.luk_priority.Properties.AutoHeight = false;
		this.luk_priority.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_priority.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_priority.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_priority.Properties.IsSearchControl = false;
		this.luk_priority.Properties.NullText = "";
		this.luk_priority.Properties.ShowFooter = false;
		this.luk_priority.Properties.ShowHeader = false;
		this.luk_priority.Size = new System.Drawing.Size(181, 20);
		this.luk_priority.TabIndex = 33;
		this.luk_priority.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.labelLine16.FieldName = null;
		this.labelLine16.HideWhenEmpty = false;
		this.labelLine16.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine16.Location = new System.Drawing.Point(314, 169);
		this.labelLine16.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine16.Name = "labelLine16";
		this.labelLine16.ShowUnderLine = true;
		this.labelLine16.Size = new System.Drawing.Size(100, 20);
		this.labelLine16.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine16.TabIndex = 73;
		this.labelLine16.Utext = "Solution By";
		this.txt_solutionDesc.FieldName = null;
		this.txt_solutionDesc.IsSearchControl = false;
		this.txt_solutionDesc.Location = new System.Drawing.Point(415, 143);
		this.txt_solutionDesc.Name = "txt_solutionDesc";
		this.txt_solutionDesc.Properties.ReadOnly = true;
		this.txt_solutionDesc.Size = new System.Drawing.Size(212, 20);
		this.txt_solutionDesc.TabIndex = 72;
		this.txt_solutionDesc.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.btn_report.FieldName = null;
		this.btn_report.IsSearchControl = false;
		this.btn_report.Location = new System.Drawing.Point(106, 96);
		this.btn_report.Name = "btn_report";
		this.btn_report.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.btn_report.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_report.Properties.ContainerControl = null;
		this.btn_report.Properties.DisplayMember = "lastName";
		this.btn_report.Properties.DisplayMember2 = "firstName";
		this.btn_report.Properties.LoadDataSourceWhenEnter = false;
		this.btn_report.Properties.SeparateCharacterForMulti = ';';
		this.btn_report.Properties.SeparateString = ", ";
		this.btn_report.Properties.UsingAutoCompleteBox = false;
		this.btn_report.Properties.UsingDataSource = true;
		this.btn_report.Properties.ValidateWhenNoDataSource = false;
		this.btn_report.Properties.ValueMember = "EmpId";
		this.btn_report.Size = new System.Drawing.Size(181, 20);
		this.btn_report.TabIndex = 15;
		this.btn_report.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.btn_solution.FieldName = null;
		this.btn_solution.IsSearchControl = false;
		this.btn_solution.Location = new System.Drawing.Point(415, 121);
		this.btn_solution.Name = "btn_solution";
		this.btn_solution.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.btn_solution.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_solution.Properties.ContainerControl = null;
		this.btn_solution.Properties.DisplayMember = "Subject";
		this.btn_solution.Properties.DisplayMember2 = "";
		this.btn_solution.Properties.LoadDataSourceWhenEnter = false;
		this.btn_solution.Properties.SeparateCharacterForMulti = ';';
		this.btn_solution.Properties.SeparateString = ", ";
		this.btn_solution.Properties.UsingAutoCompleteBox = false;
		this.btn_solution.Properties.UsingDataSource = true;
		this.btn_solution.Properties.ValidateWhenNoDataSource = false;
		this.btn_solution.Properties.ValueMember = "SltCode";
		this.btn_solution.Size = new System.Drawing.Size(212, 20);
		this.btn_solution.TabIndex = 71;
		this.btn_solution.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.btn_handle.FieldName = null;
		this.btn_handle.IsSearchControl = false;
		this.btn_handle.Location = new System.Drawing.Point(106, 119);
		this.btn_handle.Name = "btn_handle";
		this.btn_handle.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions3.Image");
		this.btn_handle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_handle.Properties.ContainerControl = null;
		this.btn_handle.Properties.DisplayMember = "lastName";
		this.btn_handle.Properties.DisplayMember2 = "firstName";
		this.btn_handle.Properties.LoadDataSourceWhenEnter = false;
		this.btn_handle.Properties.SeparateCharacterForMulti = ';';
		this.btn_handle.Properties.SeparateString = ", ";
		this.btn_handle.Properties.UsingAutoCompleteBox = false;
		this.btn_handle.Properties.UsingDataSource = true;
		this.btn_handle.Properties.ValidateWhenNoDataSource = false;
		this.btn_handle.Properties.ValueMember = "EmpId";
		this.btn_handle.Size = new System.Drawing.Size(181, 20);
		this.btn_handle.TabIndex = 35;
		this.btn_handle.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.btn_handle_EditValueChanging);
		this.labelLine20.FieldName = null;
		this.labelLine20.HideWhenEmpty = false;
		this.labelLine20.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine20.Location = new System.Drawing.Point(315, 124);
		this.labelLine20.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine20.Name = "labelLine20";
		this.labelLine20.ShowUnderLine = true;
		this.labelLine20.Size = new System.Drawing.Size(100, 20);
		this.labelLine20.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine20.TabIndex = 70;
		this.labelLine20.Utext = "Solution Id";
		this.labelLine13.FieldName = null;
		this.labelLine13.HideWhenEmpty = false;
		this.labelLine13.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine13.Location = new System.Drawing.Point(315, 75);
		this.labelLine13.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine13.Name = "labelLine13";
		this.labelLine13.ShowUnderLine = true;
		this.labelLine13.Size = new System.Drawing.Size(100, 20);
		this.labelLine13.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine13.TabIndex = 46;
		this.labelLine13.Utext = "Wbs Name";
		this.labelLine19.FieldName = null;
		this.labelLine19.HideWhenEmpty = false;
		this.labelLine19.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine19.Location = new System.Drawing.Point(315, 146);
		this.labelLine19.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine19.Name = "labelLine19";
		this.labelLine19.ShowUnderLine = true;
		this.labelLine19.Size = new System.Drawing.Size(100, 20);
		this.labelLine19.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine19.TabIndex = 69;
		this.labelLine19.Utext = "Solution Desc";
		this.labelLine14.FieldName = null;
		this.labelLine14.HideWhenEmpty = false;
		this.labelLine14.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine14.Location = new System.Drawing.Point(315, 53);
		this.labelLine14.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine14.Name = "labelLine14";
		this.labelLine14.ShowUnderLine = true;
		this.labelLine14.Size = new System.Drawing.Size(100, 20);
		this.labelLine14.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine14.TabIndex = 47;
		this.labelLine14.Utext = "Wbs Code";
		this.btn_prOw.FieldName = null;
		this.btn_prOw.IsSearchControl = false;
		this.btn_prOw.Location = new System.Drawing.Point(415, 26);
		this.btn_prOw.Name = "btn_prOw";
		this.btn_prOw.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.btn_prOw.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_prOw.Properties.ContainerControl = null;
		this.btn_prOw.Properties.DisplayMember = "lastName";
		this.btn_prOw.Properties.DisplayMember2 = "firstName";
		this.btn_prOw.Properties.LoadDataSourceWhenEnter = false;
		this.btn_prOw.Properties.ReadOnly = true;
		this.btn_prOw.Properties.SeparateCharacterForMulti = ';';
		this.btn_prOw.Properties.SeparateString = ", ";
		this.btn_prOw.Properties.UsingAutoCompleteBox = false;
		this.btn_prOw.Properties.UsingDataSource = true;
		this.btn_prOw.Properties.ValidateWhenNoDataSource = false;
		this.btn_prOw.Properties.ValueMember = "EmpId";
		this.btn_prOw.Size = new System.Drawing.Size(212, 20);
		this.btn_prOw.TabIndex = 68;
		this.btn_prOw.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.labelLine15.FieldName = null;
		this.labelLine15.HideWhenEmpty = false;
		this.labelLine15.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine15.Location = new System.Drawing.Point(5, 148);
		this.labelLine15.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine15.Name = "labelLine15";
		this.labelLine15.ShowUnderLine = true;
		this.labelLine15.Size = new System.Drawing.Size(97, 20);
		this.labelLine15.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine15.TabIndex = 48;
		this.labelLine15.Utext = "Response Date";
		this.btn_pr.FieldName = null;
		this.btn_pr.IsSearchControl = false;
		this.btn_pr.Location = new System.Drawing.Point(415, 3);
		this.btn_pr.Name = "btn_pr";
		this.btn_pr.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions5.Image");
		this.btn_pr.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_pr.Properties.ContainerControl = null;
		this.btn_pr.Properties.DisplayMember = "Code";
		this.btn_pr.Properties.DisplayMember2 = "";
		this.btn_pr.Properties.LoadDataSourceWhenEnter = false;
		this.btn_pr.Properties.SeparateCharacterForMulti = ';';
		this.btn_pr.Properties.SeparateString = ", ";
		this.btn_pr.Properties.UsingAutoCompleteBox = false;
		this.btn_pr.Properties.UsingDataSource = true;
		this.btn_pr.Properties.ValidateWhenNoDataSource = false;
		this.btn_pr.Properties.ValueMember = "AbsEntry";
		this.btn_pr.Size = new System.Drawing.Size(212, 20);
		this.btn_pr.TabIndex = 67;
		this.btn_pr.EditValueChanged += new System.EventHandler(this.btn_pr_EditValueChanged);
		this.btn_pr.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.btn_bp.FieldName = null;
		this.btn_bp.IsSearchControl = false;
		this.btn_bp.Location = new System.Drawing.Point(415, 96);
		this.btn_bp.Name = "btn_bp";
		this.btn_bp.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.btn_bp.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_bp.Properties.ContainerControl = null;
		this.btn_bp.Properties.DisplayMember = "CardCode";
		this.btn_bp.Properties.DisplayMember2 = "";
		this.btn_bp.Properties.LoadDataSourceWhenEnter = false;
		this.btn_bp.Properties.SeparateCharacterForMulti = ';';
		this.btn_bp.Properties.SeparateString = ", ";
		this.btn_bp.Properties.UsingAutoCompleteBox = false;
		this.btn_bp.Properties.UsingDataSource = true;
		this.btn_bp.Properties.ValidateWhenNoDataSource = false;
		this.btn_bp.Properties.ValueMember = "CardCode";
		this.btn_bp.Size = new System.Drawing.Size(212, 20);
		this.btn_bp.TabIndex = 65;
		this.btn_bp.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.btn_taskCode.FieldName = null;
		this.btn_taskCode.IsSearchControl = false;
		this.btn_taskCode.Location = new System.Drawing.Point(415, 51);
		this.btn_taskCode.Name = "btn_taskCode";
		this.btn_taskCode.Properties.AutoLoadDataSource = true;
		this.btn_taskCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btn_taskCode.Properties.ContainerControl = null;
		this.btn_taskCode.Properties.DisplayMember = "TaskCode";
		this.btn_taskCode.Properties.DisplayMember2 = "";
		this.btn_taskCode.Properties.LoadDataSourceWhenEnter = false;
		this.btn_taskCode.Properties.SeparateCharacterForMulti = ';';
		this.btn_taskCode.Properties.SeparateString = ", ";
		this.btn_taskCode.Properties.UsingAutoCompleteBox = false;
		this.btn_taskCode.Properties.UsingDataSource = true;
		this.btn_taskCode.Properties.ValidateWhenNoDataSource = false;
		this.btn_taskCode.Properties.ValueMember = "TaskCode";
		this.btn_taskCode.Size = new System.Drawing.Size(211, 20);
		this.btn_taskCode.TabIndex = 52;
		this.btn_taskCode.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.labelLine17.FieldName = null;
		this.labelLine17.HideWhenEmpty = false;
		this.labelLine17.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine17.Location = new System.Drawing.Point(315, 98);
		this.labelLine17.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine17.Name = "labelLine17";
		this.labelLine17.ShowUnderLine = true;
		this.labelLine17.Size = new System.Drawing.Size(100, 20);
		this.labelLine17.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine17.TabIndex = 64;
		this.labelLine17.Utext = "Bp Code";
		this.txt_taskName.FieldName = null;
		this.txt_taskName.IsSearchControl = false;
		this.txt_taskName.Location = new System.Drawing.Point(415, 74);
		this.txt_taskName.Name = "txt_taskName";
		this.txt_taskName.Properties.ReadOnly = true;
		this.txt_taskName.Size = new System.Drawing.Size(211, 20);
		this.txt_taskName.TabIndex = 53;
		this.txt_taskName.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.labelLine22.FieldName = null;
		this.labelLine22.HideWhenEmpty = false;
		this.labelLine22.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine22.Location = new System.Drawing.Point(315, 6);
		this.labelLine22.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine22.Name = "labelLine22";
		this.labelLine22.ShowUnderLine = true;
		this.labelLine22.Size = new System.Drawing.Size(100, 20);
		this.labelLine22.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine22.TabIndex = 62;
		this.labelLine22.Utext = "Project Code";
		this.txt_Id.FieldName = null;
		this.txt_Id.IsSearchControl = false;
		this.txt_Id.Location = new System.Drawing.Point(333, 194);
		this.txt_Id.Name = "txt_Id";
		this.txt_Id.Size = new System.Drawing.Size(61, 20);
		this.txt_Id.TabIndex = 58;
		this.txt_Id.Visible = false;
		this.labelLine23.FieldName = null;
		this.labelLine23.HideWhenEmpty = false;
		this.labelLine23.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine23.Location = new System.Drawing.Point(315, 29);
		this.labelLine23.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine23.Name = "labelLine23";
		this.labelLine23.ShowUnderLine = true;
		this.labelLine23.Size = new System.Drawing.Size(100, 20);
		this.labelLine23.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine23.TabIndex = 61;
		this.labelLine23.Utext = "Project Owner";
		this.xtraTabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(3, 228);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPageAddInfo;
		this.xtraTabControl1.Size = new System.Drawing.Size(637, 476);
		this.xtraTabControl1.TabIndex = 81;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.xtraTabPageAddInfo, this.xtraTabPageComment, this.xtraTabPageHistory });
		this.xtraTabPageAddInfo.AutoScroll = true;
		this.xtraTabPageAddInfo.AutoScrollMinSize = new System.Drawing.Size(500, 400);
		this.xtraTabPageAddInfo.Controls.Add(this.splitContainerControl1);
		this.xtraTabPageAddInfo.FieldName = null;
		this.xtraTabPageAddInfo.IsSearchControl = false;
		this.xtraTabPageAddInfo.Name = "xtraTabPageAddInfo";
		this.xtraTabPageAddInfo.Size = new System.Drawing.Size(631, 448);
		this.xtraTabPageAddInfo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageAddInfo.Text = "Additional info";
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.splitContainerControl2);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.splitContainerControl3);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(631, 448);
		this.splitContainerControl1.SplitterPosition = 239;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.labelLine27);
		this.splitContainerControl2.Panel1.Controls.Add(this.memo_Summary);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.labelLine11);
		this.splitContainerControl2.Panel2.Controls.Add(this.txt_note);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(631, 239);
		this.splitContainerControl2.SplitterPosition = 72;
		this.splitContainerControl2.TabIndex = 0;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.labelLine27.FieldName = null;
		this.labelLine27.HideWhenEmpty = false;
		this.labelLine27.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine27.Location = new System.Drawing.Point(10, 2);
		this.labelLine27.Name = "labelLine27";
		this.labelLine27.ShowUnderLine = true;
		this.labelLine27.Size = new System.Drawing.Size(57, 20);
		this.labelLine27.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine27.TabIndex = 80;
		this.labelLine27.Utext = "Summary";
		this.memo_Summary.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.memo_Summary.FieldName = null;
		this.memo_Summary.IsSearchControl = false;
		this.memo_Summary.Location = new System.Drawing.Point(3, 26);
		this.memo_Summary.Name = "memo_Summary";
		this.memo_Summary.Size = new System.Drawing.Size(621, 38);
		this.memo_Summary.TabIndex = 79;
		this.memo_Summary.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.memo_Summary_EditValueChanging);
		this.labelLine11.FieldName = null;
		this.labelLine11.HideWhenEmpty = false;
		this.labelLine11.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine11.Location = new System.Drawing.Point(3, 4);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.ShowUnderLine = true;
		this.labelLine11.Size = new System.Drawing.Size(31, 20);
		this.labelLine11.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine11.TabIndex = 78;
		this.labelLine11.Utext = "Note";
		this.txt_note.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txt_note.FieldName = null;
		this.txt_note.IsSearchControl = false;
		this.txt_note.Location = new System.Drawing.Point(3, 30);
		this.txt_note.Name = "txt_note";
		this.txt_note.Size = new System.Drawing.Size(621, 129);
		this.txt_note.TabIndex = 76;
		this.txt_note.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_note_EditValueChanging);
		this.splitContainerControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl3.Horizontal = false;
		this.splitContainerControl3.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl3.Name = "splitContainerControl3";
		this.splitContainerControl3.Panel1.Controls.Add(this.ucAttachment1);
		this.splitContainerControl3.Panel1.Text = "Panel1";
		this.splitContainerControl3.Panel2.Controls.Add(this.layout_UDF);
		this.splitContainerControl3.Panel2.Text = "Panel2";
		this.splitContainerControl3.Size = new System.Drawing.Size(631, 204);
		this.splitContainerControl3.SplitterPosition = 105;
		this.splitContainerControl3.TabIndex = 78;
		this.splitContainerControl3.Text = "splitContainerControl3";
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucAttachment1.FormId = "";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(0, 0);
		this.ucAttachment1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.Size = new System.Drawing.Size(631, 105);
		this.ucAttachment1.TabIndex = 77;
		this.layout_UDF.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layout_UDF.Location = new System.Drawing.Point(0, 0);
		this.layout_UDF.LookAndFeel.UseDefaultLookAndFeel = false;
		this.layout_UDF.Name = "layout_UDF";
		this.layout_UDF.Root = this.Root;
		this.layout_UDF.Size = new System.Drawing.Size(631, 94);
		this.layout_UDF.TabIndex = 3;
		this.layout_UDF.Text = "layoutControl1";
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(631, 94);
		this.Root.TextVisible = false;
		this.xtraTabPageComment.Controls.Add(this.ucComment1);
		this.xtraTabPageComment.FieldName = null;
		this.xtraTabPageComment.IsSearchControl = false;
		this.xtraTabPageComment.Name = "xtraTabPageComment";
		this.xtraTabPageComment.Size = new System.Drawing.Size(631, 448);
		this.xtraTabPageComment.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageComment.Text = "Comment";
		this.ucComment1.AdditionNotifyUser = (System.Collections.Generic.List<int>)resources.GetObject("ucComment1.AdditionNotifyUser");
		this.ucComment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucComment1.Location = new System.Drawing.Point(0, 0);
		this.ucComment1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.ucComment1.Name = "ucComment1";
		this.ucComment1.ObjectType = Apzon.Commons.ApzObjectTypes.oTrackingIssue;
		this.ucComment1.Size = new System.Drawing.Size(631, 448);
		this.ucComment1.TabIndex = 0;
		this.xtraTabPageHistory.Controls.Add(this.grd_History);
		this.xtraTabPageHistory.FieldName = null;
		this.xtraTabPageHistory.IsSearchControl = false;
		this.xtraTabPageHistory.Name = "xtraTabPageHistory";
		this.xtraTabPageHistory.Size = new System.Drawing.Size(631, 448);
		this.xtraTabPageHistory.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageHistory.Text = "History";
		this.grd_History.AllowPasteExcel = false;
		this.grd_History.ClipboardData = null;
		this.grd_History.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_History.Location = new System.Drawing.Point(0, 0);
		this.grd_History.MainView = this.gv_History;
		this.grd_History.Name = "grd_History";
		this.grd_History.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_btnUserSign });
		this.grd_History.Size = new System.Drawing.Size(631, 448);
		this.grd_History.TabIndex = 1;
		this.grd_History.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gv_History, this.gridViewEx1 });
		this.gv_History.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_History.ColumnKey = null;
		this.gv_History.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumnEx16, this.gridColumnEx17, this.gridColumnEx18, this.gridColumnEx19, this.gridColumnEx20, this.gridColumnEx21, this.gridColumnEx22, this.gridColumnEx23, this.gridColumnEx5 });
		this.gv_History.DisableFormSetting = false;
		this.gv_History.DisplayException = false;
		this.gv_History.EditColumn = null;
		this.gv_History.FieldCurrency = null;
		this.gv_History.FieldRate = null;
		this.gv_History.FormId = "";
		this.gv_History.GridControl = this.grd_History;
		this.gv_History.IndexEditRow = -1;
		this.gv_History.IndicatorWidth = 35;
		this.gv_History.IsApzTable = false;
		this.gv_History.IsInsertNewRow = false;
		this.gv_History.IsLoadingData = false;
		this.gv_History.ItemId = "APZ_TGIE1";
		this.gv_History.LstColumnAllowBatch = null;
		this.gv_History.LstColumnNotAllowBatch = null;
		this.gv_History.LstColumNotSetting = null;
		this.gv_History.Name = "gv_History";
		this.gv_History.OptionsView.BestFitMaxRowCount = 100;
		this.gv_History.OptionsView.ColumnAutoWidth = false;
		this.gv_History.OptionsView.ShowGroupPanel = false;
		this.gv_History.UseBatchMod = false;
		this.gridColumnEx16.Caption = "Id";
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "Id";
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.OptionsColumn.ReadOnly = true;
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx17.Caption = "LineNum";
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.FieldName = "LineNum";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.OptionsColumn.ReadOnly = true;
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx18.Caption = "Action Detail";
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = null;
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = null;
		this.gridColumnEx18.FieldName = "ActionDetail";
		this.gridColumnEx18.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.OptionsColumn.ReadOnly = true;
		this.gridColumnEx18.PrimaryKey = false;
		this.gridColumnEx18.UnitField = null;
		this.gridColumnEx18.Visible = true;
		this.gridColumnEx18.VisibleIndex = 0;
		this.gridColumnEx18.Width = 129;
		this.gridColumnEx19.Caption = "User Sign";
		this.gridColumnEx19.ColumnEdit = this.rep_btnUserSign;
		this.gridColumnEx19.CurrencyDateField = null;
		this.gridColumnEx19.CurrencyField = null;
		this.gridColumnEx19.CurrencyFixed = null;
		this.gridColumnEx19.CurrencyFixedRate = 0.0;
		this.gridColumnEx19.CurrencyRateField = null;
		this.gridColumnEx19.FieldName = "UserSign";
		this.gridColumnEx19.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx19.Name = "gridColumnEx19";
		this.gridColumnEx19.OptionsColumn.ReadOnly = true;
		this.gridColumnEx19.PrimaryKey = false;
		this.gridColumnEx19.UnitField = null;
		this.gridColumnEx19.Visible = true;
		this.gridColumnEx19.VisibleIndex = 1;
		this.gridColumnEx19.Width = 191;
		this.rep_btnUserSign.AutoHeight = false;
		this.rep_btnUserSign.AutoLoadDataSource = true;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions7.Image");
		this.rep_btnUserSign.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_btnUserSign.ContainerControl = this;
		this.rep_btnUserSign.DisplayMember = "U_NAME";
		this.rep_btnUserSign.DisplayMember2 = "";
		this.rep_btnUserSign.LoadDataSourceWhenEnter = false;
		this.rep_btnUserSign.Name = "rep_btnUserSign";
		this.rep_btnUserSign.SeparateCharacterForMulti = ';';
		this.rep_btnUserSign.SeparateString = ", ";
		this.rep_btnUserSign.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oUsers;
		this.rep_btnUserSign.UsingAutoCompleteBox = false;
		this.rep_btnUserSign.UsingDataSource = true;
		this.rep_btnUserSign.ValidateWhenNoDataSource = false;
		this.rep_btnUserSign.ValueMember = "USERID";
		this.gridColumnEx20.Caption = "TrackingDate";
		this.gridColumnEx20.CurrencyDateField = null;
		this.gridColumnEx20.CurrencyField = null;
		this.gridColumnEx20.CurrencyFixed = null;
		this.gridColumnEx20.CurrencyFixedRate = 0.0;
		this.gridColumnEx20.CurrencyRateField = null;
		this.gridColumnEx20.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx20.FieldName = "TrackingDate";
		this.gridColumnEx20.Name = "gridColumnEx20";
		this.gridColumnEx20.OptionsColumn.ReadOnly = true;
		this.gridColumnEx20.PrimaryKey = false;
		this.gridColumnEx20.UnitField = null;
		this.gridColumnEx20.Visible = true;
		this.gridColumnEx20.VisibleIndex = 2;
		this.gridColumnEx20.Width = 87;
		this.gridColumnEx21.Caption = "Field Name";
		this.gridColumnEx21.CurrencyDateField = null;
		this.gridColumnEx21.CurrencyField = null;
		this.gridColumnEx21.CurrencyFixed = null;
		this.gridColumnEx21.CurrencyFixedRate = 0.0;
		this.gridColumnEx21.CurrencyRateField = null;
		this.gridColumnEx21.FieldName = "FieldName";
		this.gridColumnEx21.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx21.Name = "gridColumnEx21";
		this.gridColumnEx21.OptionsColumn.ReadOnly = true;
		this.gridColumnEx21.PrimaryKey = false;
		this.gridColumnEx21.UnitField = null;
		this.gridColumnEx21.Visible = true;
		this.gridColumnEx21.VisibleIndex = 3;
		this.gridColumnEx21.Width = 76;
		this.gridColumnEx22.Caption = "Original Value";
		this.gridColumnEx22.CurrencyDateField = null;
		this.gridColumnEx22.CurrencyField = null;
		this.gridColumnEx22.CurrencyFixed = null;
		this.gridColumnEx22.CurrencyFixedRate = 0.0;
		this.gridColumnEx22.CurrencyRateField = null;
		this.gridColumnEx22.FieldName = "OriginalValue";
		this.gridColumnEx22.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx22.Name = "gridColumnEx22";
		this.gridColumnEx22.OptionsColumn.ReadOnly = true;
		this.gridColumnEx22.PrimaryKey = false;
		this.gridColumnEx22.UnitField = null;
		this.gridColumnEx22.Visible = true;
		this.gridColumnEx22.VisibleIndex = 4;
		this.gridColumnEx22.Width = 92;
		this.gridColumnEx23.Caption = "New Value";
		this.gridColumnEx23.CurrencyDateField = null;
		this.gridColumnEx23.CurrencyField = null;
		this.gridColumnEx23.CurrencyFixed = null;
		this.gridColumnEx23.CurrencyFixedRate = 0.0;
		this.gridColumnEx23.CurrencyRateField = null;
		this.gridColumnEx23.FieldName = "NewValue";
		this.gridColumnEx23.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx23.Name = "gridColumnEx23";
		this.gridColumnEx23.OptionsColumn.ReadOnly = true;
		this.gridColumnEx23.PrimaryKey = false;
		this.gridColumnEx23.UnitField = null;
		this.gridColumnEx23.Visible = true;
		this.gridColumnEx23.VisibleIndex = 5;
		this.gridColumnEx23.Width = 76;
		this.gridColumnEx5.Caption = "gridColumnEx5";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "IsDatabase";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridViewEx1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridViewEx1.ColumnKey = null;
		this.gridViewEx1.DisableFormSetting = false;
		this.gridViewEx1.DisplayException = false;
		this.gridViewEx1.EditColumn = null;
		this.gridViewEx1.FieldCurrency = null;
		this.gridViewEx1.FieldRate = null;
		this.gridViewEx1.FormId = "oTrackingIssue";
		this.gridViewEx1.GridControl = this.grd_History;
		this.gridViewEx1.IndexEditRow = -1;
		this.gridViewEx1.IndicatorWidth = 35;
		this.gridViewEx1.IsApzTable = false;
		this.gridViewEx1.IsInsertNewRow = false;
		this.gridViewEx1.IsLoadingData = false;
		this.gridViewEx1.ItemId = "OGRID1";
		this.gridViewEx1.LstColumnAllowBatch = null;
		this.gridViewEx1.LstColumnNotAllowBatch = null;
		this.gridViewEx1.LstColumNotSetting = null;
		this.gridViewEx1.Name = "gridViewEx1";
		this.gridViewEx1.OptionsView.BestFitMaxRowCount = 100;
		this.gridViewEx1.OptionsView.ColumnAutoWidth = false;
		this.gridViewEx1.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridViewEx1.OptionsView.ShowGroupPanel = false;
		this.gridViewEx1.UseBatchMod = false;
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(3, 710);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 92;
		this.btn_Ok.Text = "Ok";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_user1.FieldName = null;
		this.btn_user1.IsSearchControl = false;
		this.btn_user1.Location = new System.Drawing.Point(81, 194);
		this.btn_user1.Name = "btn_user1";
		this.btn_user1.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions8.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		this.btn_user1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_user1.Properties.ContainerControl = null;
		this.btn_user1.Properties.DisplayMember = "U_NAME";
		this.btn_user1.Properties.DisplayMember2 = "";
		this.btn_user1.Properties.LoadDataSourceWhenEnter = false;
		this.btn_user1.Properties.SeparateCharacterForMulti = ';';
		this.btn_user1.Properties.SeparateString = ", ";
		this.btn_user1.Properties.UsingAutoCompleteBox = false;
		this.btn_user1.Properties.UsingDataSource = true;
		this.btn_user1.Properties.ValidateWhenNoDataSource = false;
		this.btn_user1.Properties.ValueMember = "USERID";
		this.btn_user1.Size = new System.Drawing.Size(182, 20);
		this.btn_user1.TabIndex = 95;
		this.btn_user1.Visible = false;
		this.txt_attact.FieldName = null;
		this.txt_attact.IsSearchControl = false;
		this.txt_attact.Location = new System.Drawing.Point(275, 196);
		this.txt_attact.Name = "txt_attact";
		this.txt_attact.Properties.ReadOnly = true;
		this.txt_attact.Size = new System.Drawing.Size(27, 20);
		this.txt_attact.TabIndex = 79;
		this.txt_attact.Visible = false;
		this.luk_Status.FieldName = null;
		this.luk_Status.IsSearchControl = false;
		this.luk_Status.Location = new System.Drawing.Point(106, 74);
		this.luk_Status.Name = "luk_Status";
		this.luk_Status.Properties.AutoHeight = false;
		this.luk_Status.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Status.Properties.CheckFields = null;
		this.luk_Status.Properties.CheckTables = null;
		this.luk_Status.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Status.Properties.NullText = "";
		this.luk_Status.Properties.ReadOnly = true;
		this.luk_Status.Properties.ShowFooter = false;
		this.luk_Status.Properties.ShowHeader = false;
		this.luk_Status.Size = new System.Drawing.Size(181, 20);
		this.luk_Status.TabIndex = 96;
		this.luk_ChangeStatus.FieldName = null;
		this.luk_ChangeStatus.IsSearchControl = false;
		this.luk_ChangeStatus.Location = new System.Drawing.Point(106, 174);
		this.luk_ChangeStatus.Name = "luk_ChangeStatus";
		this.luk_ChangeStatus.Properties.AutoHeight = false;
		this.luk_ChangeStatus.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_ChangeStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_ChangeStatus.Properties.CheckFields = null;
		this.luk_ChangeStatus.Properties.CheckTables = null;
		this.luk_ChangeStatus.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("VisOrder", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_ChangeStatus.Properties.NullText = "";
		this.luk_ChangeStatus.Properties.ShowFooter = false;
		this.luk_ChangeStatus.Properties.ShowHeader = false;
		this.luk_ChangeStatus.Size = new System.Drawing.Size(181, 20);
		this.luk_ChangeStatus.TabIndex = 97;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.luk_ChangeStatus);
		base.Controls.Add(this.luk_Status);
		base.Controls.Add(this.luk_Type);
		base.Controls.Add(this.labelLine19);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.btn_user1);
		base.Controls.Add(this.date_IDate);
		base.Controls.Add(this.xtraTabControl1);
		base.Controls.Add(this.labelLine17);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine16);
		base.Controls.Add(this.txt_Id);
		base.Controls.Add(this.labelLine15);
		base.Controls.Add(this.labelLine13);
		base.Controls.Add(this.labelLine10);
		base.Controls.Add(this.txt_attact);
		base.Controls.Add(this.btn_handle);
		base.Controls.Add(this.labelLine23);
		base.Controls.Add(this.btn_solution);
		base.Controls.Add(this.labelLine14);
		base.Controls.Add(this.txt_taskName);
		base.Controls.Add(this.txt_solutionDesc);
		base.Controls.Add(this.labelLine8);
		base.Controls.Add(this.btn_sBy);
		base.Controls.Add(this.btn_ChangeStatus);
		base.Controls.Add(this.btn_bp);
		base.Controls.Add(this.btn_prOw);
		base.Controls.Add(this.labelLine22);
		base.Controls.Add(this.btn_pr);
		base.Controls.Add(this.txt_ServiceCall);
		base.Controls.Add(this.luk_priority);
		base.Controls.Add(this.date_rDate);
		base.Controls.Add(this.labelLine9);
		base.Controls.Add(this.labelLine7);
		base.Controls.Add(this.labelLine21);
		base.Controls.Add(this.btn_taskCode);
		base.Controls.Add(this.btn_report);
		base.Controls.Add(this.labelLine20);
		base.Name = "ucTrackingIssue";
		base.Size = new System.Drawing.Size(643, 736);
		((System.ComponentModel.ISupportInitialize)this.txt_ServiceCall.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_IDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_IDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_rDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_rDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Type.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_sBy.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_priority.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_solutionDesc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_report.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_solution.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_handle.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_prOw.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_pr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_bp.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_taskCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_taskName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Id.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPageAddInfo.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.memo_Summary.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_note.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl3).EndInit();
		this.splitContainerControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layout_UDF).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		this.xtraTabPageComment.ResumeLayout(false);
		this.xtraTabPageHistory.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_History).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_History).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnUserSign).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_user1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_attact.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ChangeStatus.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
