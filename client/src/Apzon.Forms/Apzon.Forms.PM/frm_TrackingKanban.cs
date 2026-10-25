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
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.TableLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;

namespace Apzon.Forms.PM;

public class frm_TrackingKanban : XtraFormEx
{
	public int id;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridControl grd_issue;

	private RepositoryItemLookUpEditDefineNew luk_Type;

	private RepositoryItemLookUpEditSimple luk_StatusGrid;

	private RepositoryItemButtonEditCFL btn_Report;

	private TileView tileView1;

	private TileViewColumn colId;

	private TileViewColumn colIssueDate;

	private TileViewColumn colIssueType;

	private TileViewColumn colPriority;

	private TileViewColumn colStatus;

	private TileViewColumn colReporter;

	private TileViewColumn colHandleBy;

	private TileViewColumn colSolutionId;

	private TileViewColumn colResponseDate;

	private TileViewColumn colBpCode;

	private TileViewColumn colProjectCode;

	private TileViewColumn colSolutionBy;

	private TileViewColumn colWbsCode;

	private TileViewColumn colAttachedFiled;

	private TileViewColumn colNote;

	private TileViewColumn colProjectOwner;

	private TileViewColumn colUserSign;

	private TileViewColumn colUserSign2;

	private RepositoryItemLookUpEditSimple luk_priority;

	private RepositoryItemButtonEditCFL btn_handle;

	private RepositoryItemButtonEditCFL btn_solution;

	private RepositoryItemButtonEditCFL btn_bpCode;

	private RepositoryItemButtonEditCFL btn_projectCode;

	private RepositoryItemButtonEditCFL btn_solotionBy;

	private RepositoryItemButtonEditCFL btn_Wbs;

	private DateEditEx date_searchTD;

	private LabelLine labelLine26;

	private DateEditEx date_searchFD;

	private LabelLine labelLine25;

	private LabelLine labelLine24;

	private LookUpEditSimple luk_statusFind;

	private TextEditEx txt_noSearch;

	private SimpleButtonEx btn_Find;

	private ButtonEditCFL btn_ReportBy;

	private ButtonEditCFL btn_HandleBy;

	private ButtonEditCFL btn_Owner;

	private ButtonEditCFL btn_ProjectSearch;

	private ButtonEditCFL btn_BpSearch;

	private LabelLine labelLine6;

	private LabelLine labelLine5;

	private LabelLine lbl_ReportBy;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private SimpleButtonEx btn_Cancel;

	public frm_TrackingKanban()
	{
		InitializeComponent();
	}

	private async Task LoadForm()
	{
		_ = 8;
		try
		{
			btn_BpSearch.Properties.ValueMember = "CardCode";
			btn_BpSearch.Properties.TypeCFL = Enums.TypeCFL.Customer;
			btn_BpSearch.Properties.LoadDataSourceWhenEnter = true;
			btn_BpSearch.Properties.RefreshDataSource();
			btn_BpSearch.Properties.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			btn_bpCode.ValueMember = "CardCode";
			btn_bpCode.TypeCFL = Enums.TypeCFL.Customer;
			btn_bpCode.LoadDataSourceWhenEnter = true;
			btn_bpCode.RefreshDataSource();
			btn_bpCode.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			luk_Type.ValueMember = "Code";
			luk_Type.DisplayMember = "Name";
			luk_Type.FormProperties.TableName = "APZ_IETE";
			await luk_Type.RefreshDataSourceAsync();
			btn_Report.ValueMember = "EmpId";
			btn_Report.DisplayMember = "lastName";
			btn_Report.DisplayMember2 = "firstName";
			btn_Report.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			btn_Report.MultiSelectMode = true;
			await btn_Report.RefreshDataSourceAsync();
			btn_Report.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_handle.ValueMember = "EmpId";
			btn_handle.DisplayMember = "lastName";
			btn_handle.DisplayMember2 = "firstName";
			btn_handle.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			await btn_handle.RefreshDataSourceAsync();
			btn_handle.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_projectCode.ValueMember = "AbsEntry";
			btn_projectCode.DisplayMember = "Code";
			btn_projectCode.TypeCFL = Enums.TypeCFL.ProjectManager;
			btn_projectCode.LoadDataSourceWhenEnter = true;
			btn_projectCode.RefreshDataSource();
			btn_projectCode.TypeDrillDown = ApzObjectTypes.oProjectManager;
			btn_ProjectSearch.Properties.ValueMember = "AbsEntry";
			btn_ProjectSearch.Properties.DisplayMember = "Code";
			btn_ProjectSearch.Properties.TypeCFL = Enums.TypeCFL.ProjectManager;
			btn_ProjectSearch.Properties.LoadDataSourceWhenEnter = true;
			btn_ProjectSearch.Properties.RefreshDataSource();
			btn_ProjectSearch.Properties.TypeDrillDown = ApzObjectTypes.oProjectManager;
			btn_Wbs.ValueMember = "TaskCode";
			btn_Wbs.FieldNameInDataRow("TaskCode");
			btn_Wbs.FieldNameInGrid("WbsCode");
			btn_Wbs.TypeCFL = Enums.TypeCFL.ProjectTask;
			btn_Wbs.RefreshDataSource();
			btn_Wbs.TypeDrillDown = ApzObjectTypes.oProjectManager;
			btn_HandleBy.Properties.ValueMember = "EmpId";
			btn_HandleBy.Properties.DisplayMember = "lastName";
			btn_HandleBy.Properties.DisplayMember2 = "firstName";
			btn_HandleBy.Properties.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			RepositoryItemButtonEditCFL properties = btn_HandleBy.Properties;
			properties.DataSourceLoaded = (Action<DataTable>)Delegate.Combine(properties.DataSourceLoaded, (Action<DataTable>)((DataTable dt) =>
			{
				btn_Owner.Properties.DataSource = dt.Copy();
			}));
			btn_HandleBy.Properties.MultiSelectMode = true;
			await btn_HandleBy.Properties.RefreshDataSourceAsync();
			btn_HandleBy.Properties.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_Owner.Properties.ValueMember = "EmpId";
			btn_Owner.Properties.DisplayMember = "lastName";
			btn_Owner.Properties.DisplayMember2 = "firstName";
			btn_Owner.Properties.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			await btn_Owner.Properties.RefreshDataSourceAsync();
			btn_Owner.Properties.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_ReportBy.Properties.ValueMember = "EmpId";
			btn_ReportBy.Properties.DisplayMember = "lastName";
			btn_ReportBy.Properties.DisplayMember2 = "firstName";
			btn_ReportBy.Properties.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			btn_ReportBy.Properties.MultiSelectMode = true;
			await btn_ReportBy.Properties.RefreshDataSourceAsync();
			btn_ReportBy.Properties.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			luk_priority.SetDataSource(new Dictionary<string, string>
			{
				{ "4", "Critical" },
				{ "3", "High" },
				{ "2", "Medium" },
				{ "1", "Low" }
			});
			luk_StatusGrid.SetDataSource(new Dictionary<string, string>
			{
				{ "1", "New" },
				{ "2", "Assigned" },
				{ "3", "In Progress" },
				{ "4", "FeedBack" },
				{ "5", "Confirmed" },
				{ "6", "Resolved" },
				{ "7", "Rejected" },
				{ "8", "Closed" }
			});
			luk_statusFind.SetDataSource(new Dictionary<string, string>
			{
				{ "", "None" },
				{ "1", "New" },
				{ "2", "Assigned" },
				{ "3", "In Progress" },
				{ "4", "FeedBack" },
				{ "5", "Confirmed" },
				{ "6", "Resolved" },
				{ "7", "Rejected" },
				{ "8", "Closed" }
			});
			new DataTable();
			DataTable dataTable;
			if (await GetDataSearchByUserId(GlobalData.UserID) > 0)
			{
				string bpCode = Function.ToString(btn_BpSearch.EditValue);
				string projectCode = Function.ToString(btn_ProjectSearch.EditValue);
				int projectOwner = Function.ParseInt(btn_Owner.EditValue);
				string reportBy = Function.ToString(btn_ReportBy.EditValue);
				string handleBy = Function.ToString(btn_HandleBy.EditValue);
				string status = Function.ToString(luk_statusFind.EditValue);
				string fromDate = ((date_searchFD.EditValue != null) ? date_searchFD.DateTime.ToString("ddMMyyyy") : "");
				string toDate = ((date_searchTD.EditValue != null) ? date_searchTD.DateTime.ToString("ddMMyyyy") : "");
				if (id == 0)
				{
					id = Function.ParseInt(txt_noSearch.EditValue);
				}
				dataTable = await Search(bpCode, projectCode, projectOwner, reportBy, handleBy, id, status, fromDate, toDate);
			}
			else
			{
				string fromDate2 = DateTime.Today.AddDays(-Configuration.SearchDate).ToString("ddMMyyyy");
				string toDate2 = DateTime.Today.AddDays(Configuration.SearchDate).ToString("ddMMyyyy");
				dataTable = await Search("", "", 0, "", "", id, "", fromDate2, toDate2);
			}
			id = 0;
			grd_issue.DataSource = dataTable;
			grd_issue.DatasourceTable().AcceptChanges();
			dataTable.Copy();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> Search(string bpCode, string projectCode, int projectOwner, string reportBy, string handleBy, int id, string status, string fromDate, string toDate)
	{
		_ = 1;
		try
		{
			DataTable dt = await _httpClient.TrackingIssue.Search(bpCode, projectCode, projectOwner, reportBy, handleBy, id, status, fromDate, toDate);
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				int num = Function.ParseInt(dt.Rows[i]["ProjectCode"]);
				if (btn_projectCode.dictionSource.ContainsKey(Function.ToString(num)))
				{
					dt.Rows[i]["ProjectCode"] = ((num > 0) ? dt.Rows[i]["ProjectCode"] : DBNull.Value);
				}
				else
				{
					dt.Rows[i]["ProjectCode"] = DBNull.Value;
				}
				int num2 = Function.ParseInt(dt.Rows[i]["Reporter"]);
				if (btn_Report.dictionSource.ContainsKey(Function.ToString(num2)))
				{
					dt.Rows[i]["Reporter"] = ((num2 > 0) ? dt.Rows[i]["Reporter"] : DBNull.Value);
				}
				else
				{
					int num3 = await EmployeeId(num2);
					if (num3 > 0)
					{
						dt.Rows[i]["Reporter"] = num3;
					}
					else
					{
						dt.Rows[i]["Reporter"] = DBNull.Value;
					}
				}
				int num4 = Function.ParseInt(dt.Rows[i]["HandleBy"]);
				if (btn_handle.dictionSource.ContainsKey(Function.ToString(num4)))
				{
					dt.Rows[i]["HandleBy"] = ((num4 > 0) ? dt.Rows[i]["HandleBy"] : DBNull.Value);
				}
				else
				{
					dt.Rows[i]["HandleBy"] = DBNull.Value;
				}
			}
			return dt;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<int> EmployeeId(int userId)
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

	private async void frm_TrackingKanban_Load(object sender, EventArgs e)
	{
		await LoadForm();
	}

	private void tileView1_ItemCustomize_1(object sender, TileViewItemCustomizeEventArgs e)
	{
		try
		{
			if (Function.ParseInt(tileView1.GetRowCellValue(e.RowHandle, colPriority)) == 4)
			{
				e.Item.Elements[0].Appearance.Normal.BackColor = Color.Red;
				e.Item.Elements[0].Width = 3;
				e.Item.Elements[0].Height = 30;
				e.Item.Elements[0].Text = "";
			}
			if (Function.ParseInt(tileView1.GetRowCellValue(e.RowHandle, colPriority)) == 3)
			{
				e.Item.Elements[0].Appearance.Normal.BackColor = Color.Yellow;
				e.Item.Elements[0].Width = 3;
				e.Item.Elements[0].Height = 30;
				e.Item.Elements[0].Text = "";
			}
			if (Function.ParseInt(tileView1.GetRowCellValue(e.RowHandle, colPriority)) == 2)
			{
				e.Item.Elements[0].Appearance.Normal.BackColor = Color.Black;
				e.Item.Elements[0].Width = 3;
				e.Item.Elements[0].Height = 30;
				e.Item.Elements[0].Text = "";
			}
			if (Function.ParseInt(tileView1.GetRowCellValue(e.RowHandle, colPriority)) == 1)
			{
				e.Item.Elements[0].Appearance.Normal.BackColor = Color.Green;
				e.Item.Elements[0].Width = 3;
				e.Item.Elements[0].Height = 30;
				e.Item.Elements[0].Text = "";
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tileView1_ItemDrop(object sender, ItemDropEventArgs e)
	{
	}

	private string ChangeCommentByStatus(string status)
	{
		try
		{
			string result = "";
			if (status.Equals("1"))
			{
				result = "New";
			}
			else if (status.Equals("2"))
			{
				result = "Assigned";
			}
			else if (status.Equals("3"))
			{
				result = "In Progress";
			}
			else if (status.Equals("4"))
			{
				result = "FeedBack";
			}
			else if (status.Equals("5"))
			{
				result = "Confirmed";
			}
			else if (status.Equals("6"))
			{
				result = "Resolved";
			}
			else if (status.Equals("7"))
			{
				result = "Rejected";
			}
			else if (status.Equals("8"))
			{
				result = "Closed";
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	private async void tileView1_BeforeItemDrop(object sender, BeforeItemDropEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(tileView1.GetRowCellValue(e.RowHandle, colId));
			string status = Function.ToString(e.GroupColumnValue);
			string text = Function.ToString(e.NewGroupColumnValue);
			string text2 = ChangeCommentByStatus(text);
			string text3 = ChangeCommentByStatus(status);
			_ = e.NewListSourceRowIndex;
			int num2 = Function.ParseInt(GlobalData.UserID);
			if (!string.Equals(text2, text3))
			{
				if (!string.IsNullOrEmpty(text) || num > 0)
				{
					DataSet dataSet = new DataSet();
					DataTable dataTable = new DataTable();
					string text4 = ChangeCommentByStatus(text);
					_ = "Change status to " + text4;
					dataTable.Columns.Add("Status", typeof(string));
					dataTable.Columns.Add("Id", typeof(int));
					dataTable.Columns.Add("OldValue", typeof(string));
					dataTable.Columns.Add("NewValue", typeof(string));
					dataTable.Columns.Add("UserId", typeof(int));
					dataTable.Columns.Add("FromDate", typeof(string));
					DataRow dataRow = dataTable.NewRow();
					dataRow["Status"] = text;
					dataRow["Id"] = num;
					dataRow["OldValue"] = text3;
					dataRow["NewValue"] = text2;
					dataRow["UserId"] = num2;
					dataRow["FromDate"] = DateTime.Now.ToString("ddMMyyyy HH:mm:ss");
					dataTable.Rows.Add(dataRow);
					dataSet.Tables.Add(dataTable);
					await _httpClient.TrackingIssue.UpdateStatus(dataSet);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.StatusIsNullOrIdIsNull, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Find_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			string bpCode = Function.ToString(btn_BpSearch.EditValue);
			string projectCode = Function.ToString(btn_ProjectSearch.EditValue);
			int projectOwner = Function.ParseInt(btn_Owner.EditValue);
			string reportBy = Function.ToString(btn_ReportBy.EditValue);
			string handleBy = Function.ToString(btn_HandleBy.EditValue);
			string status = Function.ToString(luk_statusFind.EditValue);
			string fromDate = ((date_searchFD.EditValue != null) ? date_searchFD.DateTime.ToString("ddMMyyyy") : "");
			string toDate = ((date_searchTD.EditValue != null) ? date_searchTD.DateTime.ToString("ddMMyyyy") : "");
			if (id == 0)
			{
				id = Function.ParseInt(txt_noSearch.EditValue);
			}
			DataTable dataSource = await Search(bpCode, projectCode, projectOwner, reportBy, handleBy, id, status, fromDate, toDate);
			id = 0;
			grd_issue.DataSource = dataSource;
			grd_issue.DataSourceTable().AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void GetDataTableSearch(ref DataTable dt)
	{
		try
		{
			dt = new DataTable();
			dt.Columns.Add("FormId", typeof(string));
			dt.Columns.Add("ParamName", typeof(string));
			dt.Columns.Add("ParamType", typeof(string));
			dt.Columns.Add("Value", typeof(string));
			dt.Columns.Add("UserCode", typeof(int));
			DataRow dataRow = dt.NewRow();
			dataRow["FormId"] = "oTrackingIssue";
			dataRow["ParamName"] = "Id";
			dataRow["ParamType"] = "int";
			dataRow["Value"] = Function.ParseInt(txt_noSearch.EditValue);
			dataRow["UserCode"] = GlobalData.UserID;
			dt.Rows.Add(dataRow);
			DataRow dataRow2 = dt.NewRow();
			dataRow2["FormId"] = "oTrackingIssue";
			dataRow2["ParamName"] = "Status";
			dataRow2["ParamType"] = "string";
			dataRow2["Value"] = Function.ToString(luk_statusFind.EditValue);
			dataRow2["UserCode"] = GlobalData.UserID;
			dt.Rows.Add(dataRow2);
			DataRow dataRow3 = dt.NewRow();
			dataRow3["FormId"] = "oTrackingIssue";
			dataRow3["ParamName"] = "ReporterBy";
			dataRow3["ParamType"] = "string";
			dataRow3["Value"] = Function.ToString(btn_ReportBy.EditValue);
			dataRow3["UserCode"] = GlobalData.UserID;
			dt.Rows.Add(dataRow3);
			DataRow dataRow4 = dt.NewRow();
			dataRow4["FormId"] = "oTrackingIssue";
			dataRow4["ParamName"] = "HandleBy";
			dataRow4["ParamType"] = "string";
			dataRow4["Value"] = Function.ToString(btn_HandleBy.EditValue);
			dataRow4["UserCode"] = GlobalData.UserID;
			dt.Rows.Add(dataRow4);
			DataRow dataRow5 = dt.NewRow();
			dataRow5["FormId"] = "oTrackingIssue";
			dataRow5["ParamName"] = "ProjectCode";
			dataRow5["ParamType"] = "int";
			dataRow5["Value"] = Function.ParseInt(btn_ProjectSearch.EditValue);
			dataRow5["UserCode"] = GlobalData.UserID;
			dt.Rows.Add(dataRow5);
			DataRow dataRow6 = dt.NewRow();
			dataRow6["FormId"] = "oTrackingIssue";
			dataRow6["ParamName"] = "ProjectOwner";
			dataRow6["ParamType"] = "int";
			dataRow6["Value"] = Function.ParseInt(btn_Owner.EditValue);
			dataRow6["UserCode"] = GlobalData.UserID;
			dt.Rows.Add(dataRow6);
			DataRow dataRow7 = dt.NewRow();
			dataRow7["FormId"] = "oTrackingIssue";
			dataRow7["ParamName"] = "BpCode";
			dataRow7["ParamType"] = "string";
			dataRow7["Value"] = Function.ToString(btn_BpSearch.EditValue);
			dataRow7["UserCode"] = GlobalData.UserID;
			dt.Rows.Add(dataRow7);
			DataRow dataRow8 = dt.NewRow();
			dataRow8["FormId"] = "oTrackingIssue";
			dataRow8["ParamName"] = "FromDate";
			dataRow8["ParamType"] = "DateTime";
			if (!Function.ParseDateTime(date_searchFD.EditValue).HasValue)
			{
				dataRow8["Value"] = DBNull.Value;
			}
			else
			{
				dataRow8["Value"] = Function.ParseDateTime(date_searchFD.EditValue);
			}
			dataRow8["UserCode"] = GlobalData.UserID;
			dt.Rows.Add(dataRow8);
			DataRow dataRow9 = dt.NewRow();
			dataRow9["FormId"] = "oTrackingIssue";
			dataRow9["ParamName"] = "ToDate";
			dataRow9["ParamType"] = "DateTime";
			if (!Function.ParseDateTime(date_searchTD.EditValue).HasValue)
			{
				dataRow9["Value"] = DBNull.Value;
			}
			else
			{
				dataRow9["Value"] = Function.ParseDateTime(date_searchTD.EditValue);
			}
			dataRow9["UserCode"] = GlobalData.UserID;
			dt.Rows.Add(dataRow9);
			dt.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetDataTableSearch(DataTable dt)
	{
		try
		{
			if (dt == null && dt.Rows.Count != 0)
			{
				return;
			}
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				switch (Function.ToString(dt.Rows[i]["ParamName"]))
				{
				case "Id":
					txt_noSearch.EditValue = ((Function.ParseInt(dt.Rows[i]["Value"]) > 0) ? dt.Rows[i]["Value"] : "");
					break;
				case "Status":
					luk_statusFind.EditValue = Function.ToString(dt.Rows[i]["Value"]);
					break;
				case "ReporterBy":
					btn_ReportBy.EditValue = Function.ToString(dt.Rows[i]["Value"]);
					break;
				case "HandleBy":
					btn_HandleBy.EditValue = Function.ToString(dt.Rows[i]["Value"]);
					break;
				case "ProjectCode":
					btn_ProjectSearch.EditValue = ((Function.ParseInt(dt.Rows[i]["Value"]) > 0) ? dt.Rows[i]["Value"] : "");
					break;
				case "ProjectOwner":
					btn_Owner.EditValue = ((Function.ParseInt(dt.Rows[i]["Value"]) > 0) ? dt.Rows[i]["Value"] : "");
					break;
				case "BpCode":
					btn_BpSearch.EditValue = Function.ToString(dt.Rows[i]["Value"]);
					break;
				case "FromDate":
					date_searchFD.EditValue = Function.ParseDateTime(dt.Rows[i]["Value"]);
					break;
				case "ToDate":
					date_searchTD.EditValue = Function.ParseDateTime(dt.Rows[i]["Value"]);
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

	private async Task<int> GetDataSearchByUserId(int userId)
	{
		try
		{
			string formId = "oTrackingIssue";
			DataTable dataTable = await _httpClient.TrackingIssue.GetDataSearchByUserId(userId, formId);
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				return 0;
			}
			SetDataTableSearch(dataTable);
			return 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	public async Task REFRESH_DATA()
	{
		btn_Find_Click(null, null);
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			Close();
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.PM.frm_TrackingKanban));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraGrid.GridFormatRule gridFormatRule = new DevExpress.XtraGrid.GridFormatRule();
		DevExpress.XtraEditors.FormatConditionRuleIconSet formatConditionRuleIconSet = new DevExpress.XtraEditors.FormatConditionRuleIconSet();
		DevExpress.XtraEditors.FormatConditionIconSet formatConditionIconSet = new DevExpress.XtraEditors.FormatConditionIconSet();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon2 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon3 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon4 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition2 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition3 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition4 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj2 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj3 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan2 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan3 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement2 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement3 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement4 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement5 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement6 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions11 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions12 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		this.colPriority = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.luk_priority = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.colHandleBy = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.btn_handle = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.colNote = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.colIssueDate = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.colIssueType = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.luk_Type = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.colReporter = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.btn_Report = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.colProjectCode = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.btn_projectCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.colSolutionId = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.btn_solution = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.colBpCode = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.btn_bpCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.colWbsCode = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.btn_Wbs = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.colSolutionBy = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.btn_solotionBy = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.grd_issue = new DevExpress.XtraGrid.GridControl();
		this.tileView1 = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.colId = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.colStatus = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.luk_StatusGrid = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.colResponseDate = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.colAttachedFiled = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.colProjectOwner = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.colUserSign = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.colUserSign2 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.date_searchTD = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine26 = new Apzon.Controls.Controls.LabelLine();
		this.date_searchFD = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine25 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine24 = new Apzon.Controls.Controls.LabelLine();
		this.luk_statusFind = new Apzon.Controls.Editor.MyLookUpEdit.LookUpEditSimple();
		this.txt_noSearch = new Apzon.Controls.Editor.TextEditEx();
		this.btn_Find = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ReportBy = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btn_HandleBy = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btn_Owner = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btn_ProjectSearch = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btn_BpSearch = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.lbl_ReportBy = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.luk_priority).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_handle).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Report).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_projectCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_solution).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_bpCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Wbs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_solotionBy).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_issue).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tileView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchTD.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchTD.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchFD.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchFD.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_statusFind.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_noSearch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ReportBy.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_HandleBy.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Owner.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ProjectSearch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BpSearch.Properties).BeginInit();
		base.SuspendLayout();
		this.colPriority.Caption = "Priority";
		this.colPriority.ColumnEdit = this.luk_priority;
		this.colPriority.FieldName = "Priority";
		this.colPriority.Name = "colPriority";
		this.colPriority.Visible = true;
		this.colPriority.VisibleIndex = 3;
		this.luk_priority.AutoHeight = false;
		this.luk_priority.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_priority.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_priority.Name = "luk_priority";
		this.luk_priority.NullText = "";
		this.luk_priority.ShowFooter = false;
		this.luk_priority.ShowHeader = false;
		this.colHandleBy.Caption = "HandleBy";
		this.colHandleBy.ColumnEdit = this.btn_handle;
		this.colHandleBy.FieldName = "HandleBy";
		this.colHandleBy.Name = "colHandleBy";
		this.colHandleBy.Visible = true;
		this.colHandleBy.VisibleIndex = 6;
		this.btn_handle.AutoHeight = false;
		this.btn_handle.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.btn_handle.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions)
		});
		this.btn_handle.ContainerControl = this;
		this.btn_handle.DisplayMember = "";
		this.btn_handle.DisplayMember2 = "";
		this.btn_handle.LoadDataSourceWhenEnter = false;
		this.btn_handle.Name = "btn_handle";
		this.btn_handle.SeparateCharacterForMulti = ';';
		this.btn_handle.SeparateString = ", ";
		this.btn_handle.UsingAutoCompleteBox = false;
		this.btn_handle.UsingDataSource = true;
		this.btn_handle.ValueMember = "";
		this.colNote.Caption = "Note";
		this.colNote.FieldName = "Note";
		this.colNote.Name = "colNote";
		this.colNote.Visible = true;
		this.colNote.VisibleIndex = 14;
		this.colIssueDate.Caption = "IssueDate";
		this.colIssueDate.FieldName = "IssueDate";
		this.colIssueDate.Name = "colIssueDate";
		this.colIssueDate.Visible = true;
		this.colIssueDate.VisibleIndex = 1;
		this.colIssueType.Caption = "IssueType";
		this.colIssueType.ColumnEdit = this.luk_Type;
		this.colIssueType.FieldName = "IssueType";
		this.colIssueType.Name = "colIssueType";
		this.colIssueType.Visible = true;
		this.colIssueType.VisibleIndex = 2;
		this.luk_Type.AutoHeight = false;
		this.luk_Type.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Type.CheckFields = null;
		this.luk_Type.CheckTables = null;
		this.luk_Type.Name = "luk_Type";
		this.luk_Type.NullText = "";
		this.luk_Type.ShowFooter = false;
		this.luk_Type.ShowHeader = false;
		this.colReporter.Caption = "Reporter";
		this.colReporter.ColumnEdit = this.btn_Report;
		this.colReporter.FieldName = "Reporter";
		this.colReporter.Name = "colReporter";
		this.colReporter.Visible = true;
		this.colReporter.VisibleIndex = 5;
		this.btn_Report.AutoHeight = false;
		this.btn_Report.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions9.Image");
		this.btn_Report.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2)
		});
		this.btn_Report.ContainerControl = this;
		this.btn_Report.DisplayMember = "";
		this.btn_Report.DisplayMember2 = "";
		this.btn_Report.LoadDataSourceWhenEnter = false;
		this.btn_Report.Name = "btn_Report";
		this.btn_Report.SeparateCharacterForMulti = ';';
		this.btn_Report.SeparateString = ", ";
		this.btn_Report.UsingAutoCompleteBox = false;
		this.btn_Report.UsingDataSource = true;
		this.btn_Report.ValueMember = "";
		this.colProjectCode.Caption = "ProjectCode";
		this.colProjectCode.ColumnEdit = this.btn_projectCode;
		this.colProjectCode.FieldName = "ProjectCode";
		this.colProjectCode.Name = "colProjectCode";
		this.colProjectCode.Visible = true;
		this.colProjectCode.VisibleIndex = 10;
		this.btn_projectCode.AutoHeight = false;
		this.btn_projectCode.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions15.Image");
		this.btn_projectCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions3)
		});
		this.btn_projectCode.ContainerControl = this;
		this.btn_projectCode.DisplayMember = "";
		this.btn_projectCode.DisplayMember2 = "";
		this.btn_projectCode.LoadDataSourceWhenEnter = false;
		this.btn_projectCode.Name = "btn_projectCode";
		this.btn_projectCode.SeparateCharacterForMulti = ';';
		this.btn_projectCode.SeparateString = ", ";
		this.btn_projectCode.UsingAutoCompleteBox = false;
		this.btn_projectCode.UsingDataSource = true;
		this.btn_projectCode.ValueMember = "";
		this.colSolutionId.Caption = "SolutionId";
		this.colSolutionId.ColumnEdit = this.btn_solution;
		this.colSolutionId.FieldName = "SolutionId";
		this.colSolutionId.Name = "colSolutionId";
		this.colSolutionId.Visible = true;
		this.colSolutionId.VisibleIndex = 7;
		this.btn_solution.AutoHeight = false;
		this.btn_solution.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions11.Image");
		this.btn_solution.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions4)
		});
		this.btn_solution.ContainerControl = this;
		this.btn_solution.DisplayMember = "";
		this.btn_solution.DisplayMember2 = "";
		this.btn_solution.LoadDataSourceWhenEnter = false;
		this.btn_solution.Name = "btn_solution";
		this.btn_solution.SeparateCharacterForMulti = ';';
		this.btn_solution.SeparateString = ", ";
		this.btn_solution.UsingAutoCompleteBox = false;
		this.btn_solution.UsingDataSource = true;
		this.btn_solution.ValueMember = "";
		this.colBpCode.Caption = "BpCode";
		this.colBpCode.ColumnEdit = this.btn_bpCode;
		this.colBpCode.FieldName = "BpCode";
		this.colBpCode.Name = "colBpCode";
		this.colBpCode.Visible = true;
		this.colBpCode.VisibleIndex = 9;
		this.btn_bpCode.AutoHeight = false;
		this.btn_bpCode.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions13.Image");
		this.btn_bpCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions5)
		});
		this.btn_bpCode.ContainerControl = this;
		this.btn_bpCode.DisplayMember = "";
		this.btn_bpCode.DisplayMember2 = "";
		this.btn_bpCode.LoadDataSourceWhenEnter = false;
		this.btn_bpCode.Name = "btn_bpCode";
		this.btn_bpCode.SeparateCharacterForMulti = ';';
		this.btn_bpCode.SeparateString = ", ";
		this.btn_bpCode.UsingAutoCompleteBox = false;
		this.btn_bpCode.UsingDataSource = true;
		this.btn_bpCode.ValueMember = "";
		this.colWbsCode.Caption = "WbsCode";
		this.colWbsCode.ColumnEdit = this.btn_Wbs;
		this.colWbsCode.FieldName = "WbsCode";
		this.colWbsCode.Name = "colWbsCode";
		this.colWbsCode.Visible = true;
		this.colWbsCode.VisibleIndex = 12;
		this.btn_Wbs.AutoHeight = false;
		this.btn_Wbs.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions19.Image");
		this.btn_Wbs.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions6)
		});
		this.btn_Wbs.ContainerControl = this;
		this.btn_Wbs.DisplayMember = "";
		this.btn_Wbs.DisplayMember2 = "";
		this.btn_Wbs.LoadDataSourceWhenEnter = false;
		this.btn_Wbs.Name = "btn_Wbs";
		this.btn_Wbs.SeparateCharacterForMulti = ';';
		this.btn_Wbs.SeparateString = ", ";
		this.btn_Wbs.UsingAutoCompleteBox = false;
		this.btn_Wbs.UsingDataSource = true;
		this.btn_Wbs.ValueMember = "";
		this.colSolutionBy.Caption = "SolutionBy";
		this.colSolutionBy.ColumnEdit = this.btn_solotionBy;
		this.colSolutionBy.FieldName = "SolutionBy";
		this.colSolutionBy.Name = "colSolutionBy";
		this.colSolutionBy.Visible = true;
		this.colSolutionBy.VisibleIndex = 11;
		this.btn_solotionBy.AutoHeight = false;
		this.btn_solotionBy.AutoLoadDataSource = true;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions17.Image");
		this.btn_solotionBy.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions7)
		});
		this.btn_solotionBy.ContainerControl = this;
		this.btn_solotionBy.DisplayMember = "";
		this.btn_solotionBy.DisplayMember2 = "";
		this.btn_solotionBy.LoadDataSourceWhenEnter = false;
		this.btn_solotionBy.Name = "btn_solotionBy";
		this.btn_solotionBy.SeparateCharacterForMulti = ';';
		this.btn_solotionBy.SeparateString = ", ";
		this.btn_solotionBy.UsingAutoCompleteBox = false;
		this.btn_solotionBy.UsingDataSource = true;
		this.btn_solotionBy.ValueMember = "";
		this.grd_issue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_issue.Location = new System.Drawing.Point(1, 65);
		this.grd_issue.MainView = this.tileView1;
		this.grd_issue.Name = "grd_issue";
		this.grd_issue.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[10] { this.luk_StatusGrid, this.btn_Report, this.luk_Type, this.luk_priority, this.btn_handle, this.btn_solution, this.btn_bpCode, this.btn_projectCode, this.btn_solotionBy, this.btn_Wbs });
		this.grd_issue.Size = new System.Drawing.Size(1273, 433);
		this.grd_issue.TabIndex = 0;
		this.grd_issue.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tileView1 });
		this.tileView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[18]
		{
			this.colId, this.colIssueDate, this.colIssueType, this.colPriority, this.colStatus, this.colReporter, this.colHandleBy, this.colSolutionId, this.colResponseDate, this.colBpCode,
			this.colProjectCode, this.colSolutionBy, this.colWbsCode, this.colAttachedFiled, this.colNote, this.colProjectOwner, this.colUserSign, this.colUserSign2
		});
		this.tileView1.ColumnSet.GroupColumn = this.colStatus;
		gridFormatRule.Column = this.colPriority;
		gridFormatRule.ColumnApplyTo = this.colPriority;
		gridFormatRule.Name = "Format0";
		formatConditionIconSet.CategoryName = "Shapes";
		formatConditionIconSetIcon.PredefinedName = "TrafficLights4_3.png";
		formatConditionIconSetIcon.Value = new decimal(new int[4] { 4, 0, 0, 0 });
		formatConditionIconSetIcon.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon2.PredefinedName = "TrafficLights4_2.png";
		formatConditionIconSetIcon2.Value = new decimal(new int[4] { 3, 0, 0, 0 });
		formatConditionIconSetIcon2.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon3.PredefinedName = "TrafficLights4_4.png";
		formatConditionIconSetIcon3.Value = new decimal(new int[4] { 2, 0, 0, 0 });
		formatConditionIconSetIcon3.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon4.PredefinedName = "TrafficLights4_1.png";
		formatConditionIconSetIcon4.Value = new decimal(new int[4] { 1, 0, 0, 0 });
		formatConditionIconSetIcon4.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon2);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon3);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon4);
		formatConditionIconSet.Name = "RedToBlack";
		formatConditionIconSet.ValueType = DevExpress.XtraEditors.FormatConditionValueType.Number;
		formatConditionRuleIconSet.IconSet = formatConditionIconSet;
		gridFormatRule.Rule = formatConditionRuleIconSet;
		this.tileView1.FormatRules.Add(gridFormatRule);
		this.tileView1.GridControl = this.grd_issue;
		this.tileView1.Name = "tileView1";
		this.tileView1.OptionsBehavior.EditingMode = DevExpress.XtraGrid.Views.Tile.TileViewEditingMode.EditForm;
		this.tileView1.OptionsDragDrop.AllowDrag = true;
		this.tileView1.OptionsTiles.GroupTextOffset = 4;
		this.tileView1.OptionsTiles.IndentBetweenGroups = 14;
		this.tileView1.OptionsTiles.IndentBetweenItems = 5;
		this.tileView1.OptionsTiles.ItemSize = new System.Drawing.Size(320, 120);
		this.tileView1.OptionsTiles.LayoutMode = DevExpress.XtraGrid.Views.Tile.TileViewLayoutMode.Kanban;
		this.tileView1.OptionsTiles.Padding = new System.Windows.Forms.Padding(4);
		this.tileView1.OptionsTiles.RowCount = 0;
		this.tileView1.OptionsTiles.VerticalContentAlignment = DevExpress.Utils.VertAlignment.Top;
		this.tileView1.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.colStatus, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		tableColumnDefinition.Length.Value = 7.0;
		tableColumnDefinition.PaddingLeft = -19;
		tableColumnDefinition2.Length.Value = 67.0;
		tableColumnDefinition3.Length.Value = 38.0;
		tableColumnDefinition4.Length.Value = 132.0;
		this.tileView1.TileColumns.Add(tableColumnDefinition);
		this.tileView1.TileColumns.Add(tableColumnDefinition2);
		this.tileView1.TileColumns.Add(tableColumnDefinition3);
		this.tileView1.TileColumns.Add(tableColumnDefinition4);
		this.tileView1.TileRows.Add(obj);
		this.tileView1.TileRows.Add(obj2);
		this.tileView1.TileRows.Add(obj3);
		tableSpan.RowSpan = 3;
		tableSpan2.ColumnIndex = 1;
		tableSpan2.ColumnSpan = 3;
		tableSpan2.RowSpan = 2;
		tableSpan3.ColumnIndex = 1;
		tableSpan3.ColumnSpan = 2;
		tableSpan3.RowIndex = 2;
		this.tileView1.TileSpans.Add(tableSpan);
		this.tileView1.TileSpans.Add(tableSpan2);
		this.tileView1.TileSpans.Add(tableSpan3);
		tileViewItemElement.Column = this.colPriority;
		tileViewItemElement.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement.RowIndex = 1;
		tileViewItemElement.Text = "colPriority";
		tileViewItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.AnchorAlignment = DevExpress.Utils.AnchorAlignment.Right;
		tileViewItemElement2.AnchorElementIndex = 4;
		tileViewItemElement2.AnchorIndent = 0;
		tileViewItemElement2.Appearance.Normal.ForeColor = System.Drawing.Color.Lime;
		tileViewItemElement2.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement2.Column = this.colHandleBy;
		tileViewItemElement2.ColumnIndex = 1;
		tileViewItemElement2.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement2.RowIndex = 2;
		tileViewItemElement2.Text = "colHandleBy";
		tileViewItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement3.Column = this.colNote;
		tileViewItemElement3.ColumnIndex = 3;
		tileViewItemElement3.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement3.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement3.Text = "colNote";
		tileViewItemElement3.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement4.AnchorAlignment = DevExpress.Utils.AnchorAlignment.Right;
		tileViewItemElement4.AnchorElementIndex = 5;
		tileViewItemElement4.AnchorIndent = 0;
		tileViewItemElement4.Appearance.Normal.ForeColor = System.Drawing.Color.FromArgb(64, 0, 64);
		tileViewItemElement4.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement4.Column = this.colIssueDate;
		tileViewItemElement4.ColumnIndex = 3;
		tileViewItemElement4.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement4.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement4.RowIndex = 2;
		tileViewItemElement4.Text = "colIssueDate";
		tileViewItemElement4.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement5.Appearance.Normal.ForeColor = System.Drawing.Color.Silver;
		tileViewItemElement5.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement5.ColumnIndex = 1;
		tileViewItemElement5.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement5.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement5.RowIndex = 2;
		tileViewItemElement5.Text = "Handle by: ";
		tileViewItemElement5.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
		tileViewItemElement6.Appearance.Normal.ForeColor = System.Drawing.Color.Silver;
		tileViewItemElement6.Appearance.Normal.Options.UseForeColor = true;
		tileViewItemElement6.ColumnIndex = 3;
		tileViewItemElement6.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement6.RowIndex = 2;
		tileViewItemElement6.Text = "Report issue date :";
		tileViewItemElement6.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.BottomLeft;
		this.tileView1.TileTemplate.Add(tileViewItemElement);
		this.tileView1.TileTemplate.Add(tileViewItemElement2);
		this.tileView1.TileTemplate.Add(tileViewItemElement3);
		this.tileView1.TileTemplate.Add(tileViewItemElement4);
		this.tileView1.TileTemplate.Add(tileViewItemElement5);
		this.tileView1.TileTemplate.Add(tileViewItemElement6);
		this.tileView1.BeforeItemDrop += new DevExpress.XtraGrid.Views.Tile.TileViewBeforeItemDropEventHandler(this.tileView1_BeforeItemDrop);
		this.tileView1.ItemDrop += new DevExpress.XtraGrid.Views.Tile.TileViewItemDropEventHandler(this.tileView1_ItemDrop);
		this.tileView1.ItemCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewItemCustomizeEventHandler(this.tileView1_ItemCustomize_1);
		this.colId.Caption = "Id";
		this.colId.FieldName = "Id";
		this.colId.Name = "colId";
		this.colId.Visible = true;
		this.colId.VisibleIndex = 0;
		this.colStatus.Caption = "Status";
		this.colStatus.ColumnEdit = this.luk_StatusGrid;
		this.colStatus.FieldName = "Status";
		this.colStatus.Name = "colStatus";
		this.colStatus.Visible = true;
		this.colStatus.VisibleIndex = 4;
		this.luk_StatusGrid.AutoHeight = false;
		this.luk_StatusGrid.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_StatusGrid.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_StatusGrid.Name = "luk_StatusGrid";
		this.luk_StatusGrid.NullText = "";
		this.luk_StatusGrid.ShowFooter = false;
		this.luk_StatusGrid.ShowHeader = false;
		this.colResponseDate.Caption = "ResponseDate";
		this.colResponseDate.FieldName = "ResponseDate";
		this.colResponseDate.Name = "colResponseDate";
		this.colResponseDate.Visible = true;
		this.colResponseDate.VisibleIndex = 8;
		this.colAttachedFiled.Caption = "AttachedFiled";
		this.colAttachedFiled.FieldName = "AttachedFiled";
		this.colAttachedFiled.Name = "colAttachedFiled";
		this.colAttachedFiled.Visible = true;
		this.colAttachedFiled.VisibleIndex = 13;
		this.colProjectOwner.Caption = "ProjectOwner";
		this.colProjectOwner.FieldName = "ProjectOwner";
		this.colProjectOwner.Name = "colProjectOwner";
		this.colProjectOwner.Visible = true;
		this.colProjectOwner.VisibleIndex = 15;
		this.colUserSign.Caption = "UserSign";
		this.colUserSign.FieldName = "UserSign";
		this.colUserSign.Name = "colUserSign";
		this.colUserSign.Visible = true;
		this.colUserSign.VisibleIndex = 16;
		this.colUserSign2.Caption = "UserSign2";
		this.colUserSign2.FieldName = "UserSign2";
		this.colUserSign2.Name = "colUserSign2";
		this.colUserSign2.Visible = true;
		this.colUserSign2.VisibleIndex = 17;
		this.date_searchTD.EditValue = null;
		this.date_searchTD.FieldName = null;
		this.date_searchTD.Location = new System.Drawing.Point(848, 31);
		this.date_searchTD.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.date_searchTD.Name = "date_searchTD";
		this.date_searchTD.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_searchTD.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_searchTD.Size = new System.Drawing.Size(100, 20);
		this.date_searchTD.TabIndex = 59;
		this.labelLine26.FieldName = null;
		this.labelLine26.HideWhenEmpty = false;
		this.labelLine26.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine26.Location = new System.Drawing.Point(784, 33);
		this.labelLine26.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine26.Name = "labelLine26";
		this.labelLine26.ShowUnderLine = true;
		this.labelLine26.Size = new System.Drawing.Size(66, 20);
		this.labelLine26.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine26.TabIndex = 58;
		this.labelLine26.Utext = "To Date";
		this.date_searchFD.EditValue = null;
		this.date_searchFD.FieldName = null;
		this.date_searchFD.Location = new System.Drawing.Point(664, 35);
		this.date_searchFD.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.date_searchFD.Name = "date_searchFD";
		this.date_searchFD.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_searchFD.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_searchFD.Size = new System.Drawing.Size(100, 20);
		this.date_searchFD.TabIndex = 57;
		this.labelLine25.FieldName = null;
		this.labelLine25.HideWhenEmpty = false;
		this.labelLine25.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine25.Location = new System.Drawing.Point(586, 37);
		this.labelLine25.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine25.Name = "labelLine25";
		this.labelLine25.ShowUnderLine = true;
		this.labelLine25.Size = new System.Drawing.Size(80, 20);
		this.labelLine25.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine25.TabIndex = 56;
		this.labelLine25.Utext = "From Date";
		this.labelLine24.FieldName = null;
		this.labelLine24.HideWhenEmpty = false;
		this.labelLine24.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine24.Location = new System.Drawing.Point(194, 3);
		this.labelLine24.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine24.Name = "labelLine24";
		this.labelLine24.ShowUnderLine = true;
		this.labelLine24.Size = new System.Drawing.Size(80, 20);
		this.labelLine24.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine24.TabIndex = 55;
		this.labelLine24.Utext = "Hide Status";
		this.luk_statusFind.FieldName = null;
		this.luk_statusFind.Location = new System.Drawing.Point(274, 3);
		this.luk_statusFind.Name = "luk_statusFind";
		this.luk_statusFind.Properties.AutoHeight = false;
		this.luk_statusFind.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_statusFind.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_statusFind.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default)
		});
		this.luk_statusFind.Properties.NullText = "";
		this.luk_statusFind.Properties.ShowFooter = false;
		this.luk_statusFind.Properties.ShowHeader = false;
		this.luk_statusFind.Size = new System.Drawing.Size(100, 22);
		this.luk_statusFind.TabIndex = 54;
		this.txt_noSearch.FieldName = null;
		this.txt_noSearch.Location = new System.Drawing.Point(78, 3);
		this.txt_noSearch.Name = "txt_noSearch";
		this.txt_noSearch.Size = new System.Drawing.Size(100, 20);
		this.txt_noSearch.TabIndex = 53;
		this.btn_Find.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Find.ImageOptions.Image");
		this.btn_Find.Location = new System.Drawing.Point(953, 29);
		this.btn_Find.Name = "btn_Find";
		this.btn_Find.Size = new System.Drawing.Size(75, 23);
		this.btn_Find.TabIndex = 52;
		this.btn_Find.Text = "Search";
		this.btn_Find.Click += new System.EventHandler(this.btn_Find_Click);
		this.btn_ReportBy.FieldName = null;
		this.btn_ReportBy.Location = new System.Drawing.Point(466, 3);
		this.btn_ReportBy.Name = "btn_ReportBy";
		this.btn_ReportBy.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions8.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_ReportBy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions8)
		});
		this.btn_ReportBy.Properties.ContainerControl = null;
		this.btn_ReportBy.Properties.DisplayMember = "lastName";
		this.btn_ReportBy.Properties.DisplayMember2 = "firstName";
		this.btn_ReportBy.Properties.LoadDataSourceWhenEnter = false;
		this.btn_ReportBy.Properties.SeparateCharacterForMulti = ';';
		this.btn_ReportBy.Properties.SeparateString = ", ";
		this.btn_ReportBy.Properties.UsingAutoCompleteBox = false;
		this.btn_ReportBy.Properties.UsingDataSource = true;
		this.btn_ReportBy.Properties.ValueMember = "EmpId";
		this.btn_ReportBy.Size = new System.Drawing.Size(100, 20);
		this.btn_ReportBy.TabIndex = 51;
		this.btn_HandleBy.FieldName = null;
		this.btn_HandleBy.Location = new System.Drawing.Point(665, 3);
		this.btn_HandleBy.Name = "btn_HandleBy";
		this.btn_HandleBy.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions9.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.btn_HandleBy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions9)
		});
		this.btn_HandleBy.Properties.ContainerControl = null;
		this.btn_HandleBy.Properties.DisplayMember = "lastName";
		this.btn_HandleBy.Properties.DisplayMember2 = "firstName";
		this.btn_HandleBy.Properties.LoadDataSourceWhenEnter = false;
		this.btn_HandleBy.Properties.SeparateCharacterForMulti = ';';
		this.btn_HandleBy.Properties.SeparateString = ", ";
		this.btn_HandleBy.Properties.UsingAutoCompleteBox = false;
		this.btn_HandleBy.Properties.UsingDataSource = true;
		this.btn_HandleBy.Properties.ValueMember = "EmpId";
		this.btn_HandleBy.Size = new System.Drawing.Size(100, 20);
		this.btn_HandleBy.TabIndex = 50;
		this.btn_Owner.FieldName = null;
		this.btn_Owner.Location = new System.Drawing.Point(274, 38);
		this.btn_Owner.Name = "btn_Owner";
		this.btn_Owner.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions10.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions5.Image");
		this.btn_Owner.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions10)
		});
		this.btn_Owner.Properties.ContainerControl = null;
		this.btn_Owner.Properties.DisplayMember = "lastName";
		this.btn_Owner.Properties.DisplayMember2 = "firstName";
		this.btn_Owner.Properties.LoadDataSourceWhenEnter = false;
		this.btn_Owner.Properties.SeparateCharacterForMulti = ';';
		this.btn_Owner.Properties.SeparateString = ", ";
		this.btn_Owner.Properties.UsingAutoCompleteBox = false;
		this.btn_Owner.Properties.UsingDataSource = true;
		this.btn_Owner.Properties.ValueMember = "EmpId";
		this.btn_Owner.Size = new System.Drawing.Size(100, 20);
		this.btn_Owner.TabIndex = 49;
		this.btn_ProjectSearch.FieldName = null;
		this.btn_ProjectSearch.Location = new System.Drawing.Point(80, 39);
		this.btn_ProjectSearch.Name = "btn_ProjectSearch";
		this.btn_ProjectSearch.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions11.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.btn_ProjectSearch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions11)
		});
		this.btn_ProjectSearch.Properties.ContainerControl = null;
		this.btn_ProjectSearch.Properties.DisplayMember = "Code";
		this.btn_ProjectSearch.Properties.DisplayMember2 = "";
		this.btn_ProjectSearch.Properties.LoadDataSourceWhenEnter = false;
		this.btn_ProjectSearch.Properties.SeparateCharacterForMulti = ';';
		this.btn_ProjectSearch.Properties.SeparateString = ", ";
		this.btn_ProjectSearch.Properties.UsingAutoCompleteBox = false;
		this.btn_ProjectSearch.Properties.UsingDataSource = true;
		this.btn_ProjectSearch.Properties.ValueMember = "AbsEntry";
		this.btn_ProjectSearch.Size = new System.Drawing.Size(100, 20);
		this.btn_ProjectSearch.TabIndex = 48;
		this.btn_BpSearch.FieldName = null;
		this.btn_BpSearch.Location = new System.Drawing.Point(466, 37);
		this.btn_BpSearch.Name = "btn_BpSearch";
		this.btn_BpSearch.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions12.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions7.Image");
		this.btn_BpSearch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions12)
		});
		this.btn_BpSearch.Properties.ContainerControl = null;
		this.btn_BpSearch.Properties.DisplayMember = "CardCode";
		this.btn_BpSearch.Properties.DisplayMember2 = "";
		this.btn_BpSearch.Properties.LoadDataSourceWhenEnter = false;
		this.btn_BpSearch.Properties.SeparateCharacterForMulti = ';';
		this.btn_BpSearch.Properties.SeparateString = ", ";
		this.btn_BpSearch.Properties.UsingAutoCompleteBox = false;
		this.btn_BpSearch.Properties.UsingDataSource = true;
		this.btn_BpSearch.Properties.ValueMember = "CardCode";
		this.btn_BpSearch.Size = new System.Drawing.Size(100, 20);
		this.btn_BpSearch.TabIndex = 47;
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(2, 3);
		this.labelLine6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(80, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 46;
		this.labelLine6.Utext = "No.";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(586, 3);
		this.labelLine5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 45;
		this.labelLine5.Utext = "Handle By";
		this.lbl_ReportBy.FieldName = null;
		this.lbl_ReportBy.HideWhenEmpty = false;
		this.lbl_ReportBy.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ReportBy.Location = new System.Drawing.Point(393, 4);
		this.lbl_ReportBy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_ReportBy.Name = "lbl_ReportBy";
		this.lbl_ReportBy.ShowUnderLine = true;
		this.lbl_ReportBy.Size = new System.Drawing.Size(80, 19);
		this.lbl_ReportBy.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ReportBy.TabIndex = 44;
		this.lbl_ReportBy.Utext = "Report By";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(194, 39);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 19);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 43;
		this.labelLine3.Utext = "Project Owner";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(2, 41);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 42;
		this.labelLine2.Utext = "Project Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(393, 38);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 19);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 41;
		this.labelLine1.Utext = "BP Code";
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(2, 504);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 60;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1274, 529);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.date_searchTD);
		base.Controls.Add(this.labelLine26);
		base.Controls.Add(this.date_searchFD);
		base.Controls.Add(this.labelLine25);
		base.Controls.Add(this.labelLine24);
		base.Controls.Add(this.luk_statusFind);
		base.Controls.Add(this.txt_noSearch);
		base.Controls.Add(this.btn_Find);
		base.Controls.Add(this.btn_ReportBy);
		base.Controls.Add(this.btn_HandleBy);
		base.Controls.Add(this.btn_Owner);
		base.Controls.Add(this.btn_ProjectSearch);
		base.Controls.Add(this.btn_BpSearch);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.lbl_ReportBy);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.grd_issue);
		base.Name = "frm_TrackingKanban";
		base.ShowIcon = false;
		this.Text = "Tracking Issue";
		base.Load += new System.EventHandler(this.frm_TrackingKanban_Load);
		((System.ComponentModel.ISupportInitialize)this.luk_priority).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_handle).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Report).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_projectCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_solution).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_bpCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Wbs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_solotionBy).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_issue).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tileView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchTD.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchTD.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchFD.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchFD.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_statusFind.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_noSearch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ReportBy.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_HandleBy.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Owner.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ProjectSearch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BpSearch.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
