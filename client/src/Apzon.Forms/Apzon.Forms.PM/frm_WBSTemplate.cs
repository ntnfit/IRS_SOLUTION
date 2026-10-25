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
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Apzon.Forms.PM;

public class frm_WBSTemplate : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	private DataTable _dtDoc;

	private DataTable _dtTask;

	private DataTable _dtWbs;

	private bool _isLoading;

	private Dictionary<int, string> dictionEmployee = new Dictionary<int, string>();

	private Dictionary<int, string> dictionUser = new Dictionary<int, string>();

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private XtraTabControlEx xtraTabControlEx1;

	private XtraTabPageEx xtraTabPageEx1;

	private TreeEx tv_Task;

	private XtraTabPageEx xtraTabPageEx2;

	private TreeEx tv_Wbs;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private TreeListColumnEx tl_Code;

	private TreeListColumnEx tl_Object;

	private TreeListColumnEx tl_WBSCode;

	private TreeListColumnEx tl_TaskName;

	private TreeListColumnEx tl_Leadtime;

	private TreeListColumnEx tl_Dependence;

	private TreeListColumnEx tl_Note;

	private TreeListColumnEx tb_Code;

	private TreeListColumnEx tb_Object;

	private TreeListColumnEx tb_WBSName;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private TextEditEx txt_Code;

	private DateEditEx date_FromDate;

	private DateEditEx date_ToDate;

	private SimpleButtonEx btn_Search;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private GridColumnEx gridColumnEx6;

	private RepositoryItemDateEditEx repositoryItemDateEditEx1;

	private GridColumnEx gridColumnEx7;

	private RepositoryItemTimeSpanEdit repositoryItemTimeSpanEdit1;

	private GridColumnEx gridColumnEx8;

	private RepositoryItemDateEditEx repositoryItemDateEditEx2;

	private GridColumnEx gridColumnEx9;

	private TreeListColumnEx tl_LineId;

	private TreeListColumnEx tb_LineId;

	private TreeListColumnEx tb_FatherCode;

	private TreeListColumnEx tl_FatherCode;

	private ContextMenuStrip ctm_Doc;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ContextMenuStrip ctm_Task;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ContextMenuStrip ctm_WBS;

	private ToolStripMenuItem addRowToolStripMenuItem1;

	private ToolStripMenuItem deleteRowToolStripMenuItem1;

	private ToolStripMenuItem cancelToolStripMenuItem;

	private ToolStripMenuItem addRowChildToolStripMenuItem;

	private ToolStripMenuItem addRowChildrenInsertToolStripMenuItem;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private RepositoryItemTimeEdit repositoryItemTimeEdit1;

	private RepositoryItemTimeEdit repositoryItemTimeEdit2;

	private RepositoryItemLookUpEditSimple luk_Mode;

	private RepositoryItemLookUpEditSimple luk_Milestone;

	private RepositoryItemButtonEditCFL btn_Predecessors;

	private RepositoryItemMemoExEdit memo_Notes;

	private RepositoryItemButtonEditCFL btn_OcrCode;

	private RepositoryItemButtonEditCFL btn_OcrCode2;

	private RepositoryItemButtonEditCFL btn_OcrCode3;

	private RepositoryItemButtonEditCFL btn_OcrCode4;

	private RepositoryItemButtonEditCFL btn_OcrCode5;

	private RepositoryItemButtonEditCFL btn_Resource;

	private TreeListColumnEx tb_WBSMode;

	private RepositoryItemLookUpEditSimple luk_WbsMode;

	private TreeListColumnEx tb_Duration;

	private TreeListColumnEx tb_Predecessors;

	private TreeListColumnEx tb_Milestone;

	private RepositoryItemLookUpEditSimple lu_Milestone;

	private TreeListColumnEx tb_Notes;

	private RepositoryItemMemoExEdit mem_Notes;

	private RepositoryItemButtonEditCFL b_OcrCode;

	private RepositoryItemButtonEditCFL b_OcrCode2;

	private RepositoryItemButtonEditCFL b_OcrCode3;

	private RepositoryItemButtonEditCFL b_OcrCode4;

	private RepositoryItemButtonEditCFL b_OcrCode5;

	private TreeListColumnEx tb_Resource;

	private RepositoryItemButtonEditCFL b_Resource;

	private TreeListColumnEx tl_Assignee;

	private TreeListColumnEx tl_Employee;

	private TreeListColumnEx tl_User;

	private RepositoryItemLookUpEditSimple luk_Assignee;

	private RepositoryItemButtonEditCFL bts_Employee;

	private RepositoryItemButtonEditCFL bts_User;

	public string Code { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Cancel.Enabled = true;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				btn_Ok.Text = "Add";
				cancelToolStripMenuItem.Visible = false;
				break;
			case Enums.FormViewModeType.Update:
				btn_Ok.Text = "Update";
				cancelToolStripMenuItem.Visible = true;
				break;
			case Enums.FormViewModeType.View:
				btn_Ok.Text = "OK";
				cancelToolStripMenuItem.Visible = true;
				break;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_WBSTemplate()
	{
		InitializeComponent();
		OnLoadAsync += frm_WBSTemplate_Load;
		b_Resource.CustomHandleChooseItem += b_Resource_CustomHandleChooseItem;
		b_Resource.CustomDisplayText += b_Resource_CustomDisplayText;
		bts_Employee.CustomHandleChooseItem += bts_Employee_CustomHandleChooseItem;
		bts_Employee.CustomDisplayText += bts_Employee_CustomDisplayText;
		bts_User.CustomHandleChooseItem += bts_User_CustomHandleChooseItem;
		bts_User.CustomDisplayText += bts_User_CustomDisplayText;
		gv_Doc.ChangeLogType = Enums.ChangeLog.clWBSTemplate;
		gv_Doc.IsApzTable = true;
	}

	private async void b_Resource_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		string displayText = CustomerDisplayEmp(sender, e, dictionEmployee);
		e.DisplayText = displayText;
	}

	private async void bts_Employee_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		string displayText = CustomerDisplayEmp(sender, e, dictionEmployee);
		e.DisplayText = displayText;
	}

	private async void bts_User_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		string displayText = CustomerDisplayEmp(sender, e, dictionUser);
		e.DisplayText = displayText;
	}

	private string CustomerDisplayEmp(object sender, CustomDisplayTextEventArgs e, Dictionary<int, string> diction)
	{
		try
		{
			string text = Function.ToString(e.Value);
			string text2 = "";
			string text3 = "";
			if (!string.IsNullOrEmpty(text))
			{
				foreach (string item in text.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList())
				{
					int key = Function.ParseInt(item);
					if (diction.ContainsKey(key))
					{
						List<string> list = diction[key].Split(new char[1] { '-' }, StringSplitOptions.RemoveEmptyEntries).ToList();
						text2 = text2 + list[0] + "; ";
						text3 = text3 + diction[key] + "; ";
					}
				}
				return text3;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return "";
	}

	private async void bts_Employee_CustomHandleChooseItem(object sender, ButtonEditCFLEventArgs e)
	{
		try
		{
			string text = CustomerHandleChooseEmployee(sender, e);
			TreeListNode focusedNode = tv_Task.FocusedNode;
			tv_Task.SetRowCellValue(focusedNode, tl_Employee, string.Join(";", text));
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void b_Resource_CustomHandleChooseItem(object sender, ButtonEditCFLEventArgs e)
	{
		try
		{
			string text = CustomerHandleChooseEmployee(sender, e);
			TreeListNode focusedNode = tv_Wbs.FocusedNode;
			tv_Wbs.SetRowCellValue(focusedNode, tb_Resource, string.Join(";", text));
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private string CustomerHandleChooseEmployee(object sender, ButtonEditCFLEventArgs e)
	{
		try
		{
			if (sender is ButtonEditCFL && e != null)
			{
				List<DataRow> chooseItems = e.ChooseItems;
				List<string> list = new List<string>();
				foreach (DataRow item2 in chooseItems)
				{
					string item = Function.ToString(item2["EmpId"]);
					list.Add(item);
				}
				return string.Join(";", list);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return "";
	}

	private async void bts_User_CustomHandleChooseItem(object sender, ButtonEditCFLEventArgs e)
	{
		try
		{
			string text = CustomerHandleChooseUser(sender, e);
			TreeListNode focusedNode = tv_Task.FocusedNode;
			tv_Task.SetRowCellValue(focusedNode, tl_User, string.Join(";", text));
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private string CustomerHandleChooseUser(object sender, ButtonEditCFLEventArgs e)
	{
		try
		{
			if (sender is ButtonEditCFL && e != null)
			{
				List<DataRow> chooseItems = e.ChooseItems;
				List<string> list = new List<string>();
				foreach (DataRow item2 in chooseItems)
				{
					string item = Function.ToString(item2["USERID"]);
					list.Add(item);
				}
				return string.Join(";", list);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return "";
	}

	private async Task frm_WBSTemplate_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			_isLoading = true;
			FormMode = Enums.FormViewModeType.View;
			await LoadForm();
			date_FromDate.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			date_ToDate.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
			string startFrom = DateTime.Today.AddDays(-Configuration.SearchDate).ToString("ddMMyyyy");
			string startTo = DateTime.Today.AddDays(Configuration.SearchDate).ToString("ddMMyyyy");
			if (!string.IsNullOrEmpty(Code))
			{
				txt_Code.EditValue = Code;
			}
			string projectCode = Function.ToString(txt_Code.EditValue);
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_Doc, "APZ_OWBS"), tv_Task.LoadUdfColumns("APZ_TASK"), tv_Wbs.LoadUdfColumns("APZ_WBS1"));
			DataTable dataTable = await LoadData(projectCode, startFrom, startTo);
			grd_Doc.DataSource = dataTable;
			grd_Doc.DatasourceTable().AcceptChanges();
			Code = "";
			if (!dataTable.IsNotNull())
			{
				gv_Doc_FocusedRowObjectChanged(null, null);
			}
			gv_Doc.FocusedRowHandle = 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private async Task LoadForm()
	{
		_ = 2;
		try
		{
			b_Resource.ValueMember = "empId";
			b_Resource.DisplayMember = "empId";
			b_Resource.AutoLoadDataSource = false;
			b_Resource.AcceptFalseInput = true;
			b_Resource.FindCFLBy(Enums.TypeCFL.EmployeeListForSalesEmpSetup, true, "", Enums.ItemObjectType.NoItemType, null);
			b_Resource.RefreshDataSource();
			bts_Employee.ValueMember = "empId";
			bts_Employee.DisplayMember = "empId";
			bts_Employee.AutoLoadDataSource = false;
			bts_Employee.AcceptFalseInput = true;
			bts_Employee.FindCFLBy(Enums.TypeCFL.EmployeeListForSalesEmpSetup, true, "", Enums.ItemObjectType.NoItemType, null);
			bts_Employee.RefreshDataSource();
			bts_User.ValueMember = "USERID";
			bts_User.DisplayMember = "USERID";
			bts_User.AutoLoadDataSource = false;
			bts_User.AcceptFalseInput = true;
			bts_User.FindCFLBy(Enums.TypeCFL.GetUserList, true, "", Enums.ItemObjectType.NoItemType, null);
			bts_User.RefreshDataSource();
			luk_Assignee.SetDataSource(new Dictionary<string, string>
			{
				{ "E", "Group Employee" },
				{ "U", "Group User" }
			});
			lu_Milestone.SetDataSource(new Dictionary<string, string>
			{
				{ "Y", "Yes" },
				{ "N", "No" }
			});
			luk_WbsMode.SetDataSource(new Dictionary<string, string>
			{
				{ "A", "Auto" },
				{ "M", "Manual" }
			});
			await CreateTreeDistRule(tv_Wbs);
			await CreateTreeDistRule(tv_Task);
			DataSet dataSet = await _httpClient.Cfl.GetUserList();
			new DataTable();
			DataTable dataTable = ((!dataSet.IsHasStructure()) ? dataSet.Tables[0] : dataSet.Tables[1]);
			if (dataTable.IsNotNull() && dataTable.ExistsColumn("USERID") && dataTable.ExistsColumn("U_NAME"))
			{
				dictionUser = dataTable.AsEnumerable().ToDictionary((DataRow s) => Function.ParseInt(s["USERID"]), (DataRow s) => Function.ToString(s["U_NAME"]));
			}
			DataSet employeeListForSalesEmpSetup = _httpClient.Cfl.GetEmployeeListForSalesEmpSetup();
			new DataTable();
			DataTable dataTable2 = ((!employeeListForSalesEmpSetup.IsHasStructure()) ? employeeListForSalesEmpSetup.Tables[0] : employeeListForSalesEmpSetup.Tables[1]);
			if (dataTable2.IsNotNull() && dataTable2.ExistsColumn("empID") && dataTable2.ExistsColumn("lastName") && dataTable2.ExistsColumn("firstName"))
			{
				dictionEmployee = dataTable2.AsEnumerable().ToDictionary((DataRow s) => Function.ParseInt(s["empID"]), (DataRow s) => Function.ToString(s["firstName"]) + " " + Function.ToString(s["lastName"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static async Task CreateTreeDistRule(TreeList gv)
	{
		try
		{
			if (GlobalData.Dimensions == null || GlobalData.Dimensions.Rows.Count <= 0)
			{
				return;
			}
			foreach (DataRow row in GlobalData.Dimensions.Rows)
			{
				AddDistRuleInTree(gv, row);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private static void AddDistRuleInTree(TreeList gv, DataRow r)
	{
		try
		{
			string code = ((r["DimCode"] == null || r["DimCode"] == DBNull.Value) ? "" : r["DimCode"].ToString());
			if (string.IsNullOrWhiteSpace(code))
			{
				return;
			}
			if (code == "1")
			{
				code = "";
			}
			string fieldName = "OcrCode" + code.ToString();
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit.Name = "rep_line_" + fieldName;
			repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit.Buttons.Add(new EditorButton(ButtonPredefines.Glyph));
			repositoryItemButtonEdit.Buttons[1].ImageOptions.Image = Apzon.Forms.Properties.Resources.right;
			repositoryItemButtonEdit.Buttons[1].IsLeft = true;
			repositoryItemButtonEdit.Buttons[1].Caption = "DrillDown";
			repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					Enums.TypeCFL type = Enums.TypeCFL.OrcCode;
					switch (code)
					{
					case "2":
						type = Enums.TypeCFL.OrcCode2;
						break;
					case "3":
						type = Enums.TypeCFL.OrcCode3;
						break;
					case "4":
						type = Enums.TypeCFL.OrcCode4;
						break;
					case "5":
						type = Enums.TypeCFL.OrcCode5;
						break;
					}
					frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(type, multiSelect: false, "");
					frm_ChooseFromList2.ShowDialog();
					if (frm_ChooseFromList2.ChooseItem != null)
					{
						DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
						gv.SetFocusedRowCellValue(fieldName, row[0]);
					}
				}
				catch (Exception ex2)
				{
					WinCommons.ErrorLineNumber(ex2);
				}
			};
			TreeListColumnEx treeListColumnEx = new TreeListColumnEx();
			treeListColumnEx.FieldName = fieldName;
			treeListColumnEx.Caption = ((r["DimDesc"] == null || r["DimDesc"] == DBNull.Value) ? "" : r["DimDesc"].ToString());
			treeListColumnEx.Visible = true;
			treeListColumnEx.Width = 100;
			treeListColumnEx.ColumnEdit = repositoryItemButtonEdit;
			gv.Columns.Add(treeListColumnEx);
			gv.RepositoryItems.Add(repositoryItemButtonEdit);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			string code = Function.ToString(txt_Code.EditValue);
			string fromDate = (Function.ParseDateTime(date_FromDate.EditValue).HasValue ? ((DateTime)date_FromDate.EditValue).ToString("yyyyMMdd") : "");
			string toDate = (Function.ParseDateTime(date_ToDate.EditValue).HasValue ? ((DateTime)date_ToDate.EditValue).ToString("yyyyMMdd") : "");
			DataTable dataSource = await LoadData(code, fromDate, toDate);
			if (!_isLoading && ((FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes))
			{
				FormMode = Enums.FormViewModeType.View;
				grd_Doc.DataSourceTable.Rows.Clear();
				tv_Task.DataSourceTable.Rows.Clear();
				tv_Wbs.DataSourceTable.Rows.Clear();
				grd_Doc.DataSource = dataSource;
				grd_Doc.DatasourceTable().AcceptChanges();
				string code2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
				await LoadLineById(code2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Add:
				await Save(1);
				break;
			case Enums.FormViewModeType.Update:
				await Save(2);
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			Close();
		}
	}

	private void gv_Doc_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			_dtWbs = tv_Wbs.DataSourceTable;
			_dtTask = tv_Task.DataSourceTable;
			if (e.Column.FieldName == "Code")
			{
				if (_dtTask != null && _dtTask.Rows.Count > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.WbsTemplateConfirmUpdateDataForTask, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, Function.ToString(e.Value)) == DialogResult.Yes)
				{
					for (int i = 0; i < _dtTask.Rows.Count; i++)
					{
						_dtTask.Rows[i]["Code"] = Function.ToString(e.Value);
					}
				}
				if (_dtWbs != null && _dtWbs.Rows.Count > 0)
				{
					for (int j = 0; j < _dtWbs.Rows.Count; j++)
					{
						_dtWbs.Rows[j]["Code"] = Function.ToString(e.Value);
					}
				}
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				string code = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
				await LoadLineById(code);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> LoadData(string code, string fromDate, string toDate)
	{
		try
		{
			return await _httpClient.WBSTemplate.LoadData(code, fromDate, toDate);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private void DataChanged()
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.View)
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

	private async Task<int> Save(int type)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			DataSet dataSet = new DataSet();
			if (type == 2)
			{
				_ = grd_Doc.DataSourceTable;
				DataRow focusedDataRow = gv_Doc.GetFocusedDataRow();
				if (focusedDataRow == null)
				{
					WinCommons.Error(Enums.MessageCode.None, "No data to save");
					return 1;
				}
				focusedDataRow["UpdateDate"] = DateTime.Now.Date;
				focusedDataRow["UpdateTime"] = DateTime.Now.ToString("HHmm");
				focusedDataRow["UserSign2"] = GlobalData.UserID;
				_dtDoc = new DataRow[1] { focusedDataRow }.CopyToDataTable();
			}
			else
			{
				_dtDoc = grd_Doc.DataSourceTable;
				_dtDoc.Rows[gv_Doc.FocusedRowHandle]["UserSign"] = GlobalData.UserID;
			}
			_dtTask = tv_Task.DataSourceTable().Copy();
			if (_dtTask != null && _dtTask.Rows.Count > 0)
			{
				for (int i = 0; i < _dtTask.Rows.Count; i++)
				{
					if (Function.ToString(_dtTask.Rows[i]["TaskName"]) == "")
					{
						_dtTask.Rows.RemoveAt(i--);
					}
				}
				_dtTask.AcceptChanges();
			}
			_dtWbs = tv_Wbs.DataSourceTable().Copy();
			if (_dtWbs != null && _dtWbs.Rows.Count > 0)
			{
				(from DataRow t in _dtWbs.Rows
					where string.IsNullOrEmpty(Function.ToString(t["WBSName"]))
					select t).ToList().ForEach((DataRow r) =>
				{
					r.Delete();
				});
				_dtWbs.AcceptChanges();
			}
			_dtDoc.TableName = "dtDoc";
			_dtTask.TableName = "dtTask";
			_dtWbs.TableName = "dtWbs";
			dataSet.Tables.Add(_dtDoc.Copy());
			dataSet.Tables.Add(_dtTask.Copy());
			dataSet.Tables.Add(_dtWbs.Copy());
			HttpResult httpResult = ((type != 2) ? (await _httpClient.WBSTemplate.Add(dataSet)) : (await _httpClient.WBSTemplate.Update(dataSet)));
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				_isLoading = true;
				FormMode = Enums.FormViewModeType.View;
				if (type == 2)
				{
					grd_Doc.DatasourceTable().AcceptChanges();
					tv_Task.DataSource = _dtTask;
					tv_Wbs.DataSource = _dtWbs;
				}
				else
				{
					btn_Search_Click(null, null);
				}
				_isLoading = false;
				return 0;
			}
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			WinCommons._CloseProcessing();
			return 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
	}

	private async Task LoadLineById(string code)
	{
		try
		{
			_isLoading = true;
			DataSet dataSet = await _httpClient.WBSTemplate.GetDataById(code);
			if (dataSet != null && dataSet.Tables.Count > 0)
			{
				tv_Task.DataSource = dataSet.Tables[0];
				tv_Wbs.DataSource = dataSet.Tables[1];
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || (FormMode != Enums.FormViewModeType.Update && FormMode != Enums.FormViewModeType.Add) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.No)
			{
				FormMode = Enums.FormViewModeType.Add;
				grd_Doc.DatasourceTable().Rows.Clear();
				tv_Task.DataSourceTable.Rows.Clear();
				tv_Wbs.DataSourceTable.Rows.Clear();
				_dtDoc = grd_Doc.DatasourceTable();
				if (_dtDoc == null || _dtDoc.Columns.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				}
				DataRow dataRow = _dtDoc.NewRow();
				dataRow["CreateDate"] = DateTime.Today;
				dataRow["CreateTime"] = Function.ParseTimeSpanToInt(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
				dataRow["UpdateDate"] = DateTime.Today;
				dataRow["UpdateTime"] = Function.ParseTimeSpanToInt(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
				dataRow["Object"] = "APZ_OWBS";
				dataRow["Canceled"] = "N";
				_dtDoc.Rows.Add(dataRow);
				grd_Doc.DataSource = _dtDoc;
				NewTask();
				NewWBS();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewTask()
	{
		try
		{
			_dtTask = tv_Task.DataSourceTable;
			if (_dtTask == null || _dtTask.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				return;
			}
			int num = Function.ParseInt(_dtTask.Compute("MAX(LineId)", ""));
			DataRow dataRow = _dtTask.NewRow();
			dataRow["LineId"] = num + 1;
			dataRow["FatherNum"] = -1;
			dataRow["Object"] = "APZ_OWBS";
			dataRow["Code"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
			_dtTask.Rows.Add(dataRow);
			_dtTask.AcceptChanges();
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewWBS()
	{
		try
		{
			_dtWbs = tv_Wbs.DataSourceTable;
			if (_dtWbs == null || _dtWbs.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				return;
			}
			int num = Function.ParseInt(_dtWbs.Compute("MAX(LineId)", ""));
			DataRow dataRow = _dtWbs.NewRow();
			dataRow["LineId"] = num + 1;
			dataRow["FatherCode"] = -1;
			dataRow["Object"] = "APZ_OWBS";
			dataRow["Code"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
			dataRow["WBSMode"] = "M";
			dataRow["Milestone"] = "N";
			_dtWbs.Rows.Add(dataRow);
			_dtWbs.AcceptChanges();
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewTaskTree(int fatherId = -1, string line = "")
	{
		try
		{
			DataTable dataTable = tv_Task.DataSourceTable();
			if (dataTable == null)
			{
				return;
			}
			if ((from t in dataTable.AsEnumerable()
				where Function.ParseInt(t["FatherNum"]) == fatherId
				select t).ToArray().Length != 0)
			{
				int num = Function.ParseInt(dataTable.Compute("MAX(LineId)", ""));
				if (Function.ParseInt(line) > -1)
				{
					DataRow dataRow = (from t in dataTable.AsEnumerable()
						where Function.ParseInt(t["FatherNum"]) == fatherId
						select t).ToArray().ToList().FirstOrDefault((DataRow t) => Function.ToString(t["TaskName"]) == "");
					if (dataRow != null)
					{
						string cellValue = Function.ToString(dataRow["LineId"]);
						tv_Task.FocusedNode = tv_Task.FindNodeByFieldValue("LineId", cellValue);
					}
					else
					{
						(from t in dataTable.AsEnumerable()
							where Function.ParseInt(t["LineId"]) == Function.ParseInt(line)
							select t).FirstOrDefault();
						(from t in dataTable.AsEnumerable()
							where Function.ParseInt(t["LineId"]) == fatherId
							select t).FirstOrDefault();
						DataRow dataRow2 = dataTable.NewRow();
						dataRow2["FatherNum"] = fatherId;
						dataRow2["LineId"] = num + 1;
						dataRow2["Object"] = "APZ_OWBS";
						dataRow2["Code"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
						dataTable.Rows.Add(dataRow2);
						dataTable.AcceptChanges();
					}
				}
			}
			else
			{
				DataRow dataRow3 = (from t in dataTable.AsEnumerable()
					where Function.ParseInt(t["LineId"]) == Function.ParseInt(line)
					select t).FirstOrDefault();
				int num2 = Function.ParseInt(dataTable.Compute("MAX(LineId)", ""));
				if (dataRow3 != null)
				{
					DataRow dataRow4 = dataTable.NewRow();
					dataRow4["FatherNum"] = fatherId;
					dataRow4["LineId"] = num2 + 1;
					dataRow4["Object"] = "APZ_OWBS";
					dataRow4["Code"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
					dataTable.Rows.Add(dataRow4);
					dataTable.AcceptChanges();
				}
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewWbsTree(int fatherId = -1, string line = "")
	{
		try
		{
			DataTable dataTable = tv_Wbs.DataSourceTable();
			if (dataTable == null)
			{
				return;
			}
			if ((from t in dataTable.AsEnumerable()
				where Function.ParseInt(t["FatherCode"]) == fatherId
				select t).ToArray().Length != 0)
			{
				int num = Function.ParseInt(dataTable.Compute("MAX(LineId)", ""));
				if (Function.ParseInt(line) > -1)
				{
					DataRow dataRow = (from t in dataTable.AsEnumerable()
						where Function.ParseInt(t["FatherCode"]) == fatherId
						select t).ToArray().ToList().FirstOrDefault((DataRow t) => Function.ToString(t["WBSName"]) == "");
					if (dataRow != null)
					{
						string cellValue = Function.ToString(dataRow["LineId"]);
						tv_Task.FocusedNode = tv_Task.FindNodeByFieldValue("LineId", cellValue);
					}
					else
					{
						(from t in dataTable.AsEnumerable()
							where Function.ParseInt(t["LineId"]) == Function.ParseInt(line)
							select t).FirstOrDefault();
						(from t in dataTable.AsEnumerable()
							where Function.ParseInt(t["LineId"]) == fatherId
							select t).FirstOrDefault();
						DataRow dataRow2 = dataTable.NewRow();
						dataRow2["FatherCode"] = fatherId;
						dataRow2["LineId"] = num + 1;
						dataRow2["Object"] = "APZ_OWBS";
						dataRow2["Code"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
						dataRow2["WBSMode"] = "M";
						dataRow2["Milestone"] = "N";
						dataTable.Rows.Add(dataRow2);
						dataTable.AcceptChanges();
					}
				}
			}
			else
			{
				DataRow dataRow3 = (from t in dataTable.AsEnumerable()
					where Function.ParseInt(t["LineId"]) == Function.ParseInt(line)
					select t).FirstOrDefault();
				int num2 = Function.ParseInt(dataTable.Compute("MAX(LineId)", ""));
				if (dataRow3 != null)
				{
					DataRow dataRow4 = dataTable.NewRow();
					dataRow4["FatherCode"] = fatherId;
					dataRow4["LineId"] = num2 + 1;
					dataRow4["Object"] = "APZ_OWBS";
					dataRow4["Code"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
					dataRow4["WBSMode"] = "M";
					dataRow4["Milestone"] = "N";
					dataTable.Rows.Add(dataRow4);
					dataTable.AcceptChanges();
				}
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_FIRST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.First) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Next) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Last) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_PREVIOUS()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Previous) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task<int> FindNextLastFrist(Enums.NavigateType type)
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.ProjectConfirm.FindNextLastFrist(Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry")), (int)type);
			if (dataTable.IsNotNull())
			{
				grd_Doc.DataSource = dataTable;
				grd_Doc.DatasourceTable().AcceptChanges();
				string code = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
				await LoadLineById(code);
				FormMode = Enums.FormViewModeType.View;
				return 1;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private async void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await REFRESH();
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.GetSelectedRows() == null && gv_Doc.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
				return;
			}
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveByCode, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, text) == DialogResult.Yes)
			{
				string code = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
				string text2 = await _httpClient.WBSTemplate.DeleteDocs(code);
				if (string.IsNullOrEmpty(text2))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					btn_Search_Click(null, null);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		NewTask();
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		tv_Task.DeleteSelectedNodes();
		tv_Task.DataSourceTable().AcceptChanges();
		DataChanged();
	}

	private void addRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		NewWBS();
	}

	private void deleteRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		tv_Wbs.DeleteSelectedNodes();
		tv_Wbs.DataSourceTable().AcceptChanges();
		DataChanged();
	}

	private void tv_Task_EditorKeyDown(object sender, KeyEventArgs e)
	{
		KeysTask(sender, e);
	}

	private void tv_Task_KeyDown(object sender, KeyEventArgs e)
	{
		KeysTask(sender, e);
	}

	private void tv_Wbs_EditorKeyDown(object sender, KeyEventArgs e)
	{
		KeysWbs(sender, e);
	}

	private void tv_Wbs_KeyDown(object sender, KeyEventArgs e)
	{
		KeysWbs(sender, e);
	}

	private void KeysTask(object sender, KeyEventArgs e)
	{
		try
		{
			DataTable dataTable = tv_Task.DataSourceTable();
			if (sender is TreeList treeList)
			{
				if (e.KeyCode == Keys.Return)
				{
					tv_Task.SetFocusedRowCellValue(treeList.FocusedColumn.FieldName, treeList.EditingValue);
					Function.ParseInt(tv_Task.FocusedNode.GetValue("FatherNum"));
					Function.ToString(tv_Task.FocusedNode.GetValue("LineNum"));
					tv_Task.FocusedNode.Expand();
				}
				int num = Function.ParseInt(dataTable.Compute("MAX(LineId)", ""));
				if (e.KeyCode == Keys.Insert)
				{
					tv_Task.SetFocusedRowCellValue(treeList.FocusedColumn.FieldName, treeList.EditingValue);
					int num2 = Function.ParseInt(tv_Task.GetFocusedRowCellValue(tl_LineId));
					NewTaskTree(num2, Function.ToString(num2));
					tv_Task.FocusedNode.Expand();
					tv_Task.FocusedNode = tv_Task.FindNodeByFieldValue("LineId", num + 1);
					tv_Task.FocusedColumn = tl_TaskName;
				}
				if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Delete)
				{
					tv_Task.DeleteSelectedNodes();
					tv_Task.DataSourceTable().AcceptChanges();
				}
				DataChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void KeysWbs(object sender, KeyEventArgs e)
	{
		try
		{
			_dtWbs = tv_Wbs.DataSourceTable();
			if (sender is TreeList treeList)
			{
				if (e.KeyCode == Keys.Return)
				{
					tv_Wbs.SetFocusedRowCellValue(treeList.FocusedColumn.FieldName, treeList.EditingValue);
					int fatherId = Function.ParseInt(tv_Wbs.FocusedNode.GetValue("FatherCode"));
					string line = Function.ToString(tv_Wbs.FocusedNode.GetValue("LineNum"));
					NewWbsTree(fatherId, line);
					tv_Wbs.FocusedNode.Expand();
				}
				int num = Function.ParseInt(_dtWbs.Compute("MAX(LineId)", ""));
				if (e.KeyCode == Keys.Insert)
				{
					tv_Wbs.SetFocusedRowCellValue(treeList.FocusedColumn.FieldName, treeList.EditingValue);
					int num2 = Function.ParseInt(tv_Wbs.GetFocusedRowCellValue(tb_LineId));
					tv_Wbs.SetFocusedRowCellValue("WBSMode", "A");
					NewWbsTree(num2, Function.ToString(num2));
					tv_Wbs.FocusedNode.Expand();
					tv_Wbs.FocusedNode = tv_Wbs.FindNodeByFieldValue("LineId", num + 1);
					tv_Wbs.FocusedColumn = tb_WBSName;
				}
				if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Delete)
				{
					tv_Wbs.DeleteSelectedNodes();
					tv_Wbs.DataSourceTable().AcceptChanges();
				}
				DataChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_Task_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		try
		{
			int fatherId = Function.ParseInt(tv_Task.GetRowCellValue(e.Node, tl_FatherCode));
			string line = Function.ToString(tv_Task.GetRowCellValue(e.Node, tl_LineId));
			NewTaskTree(fatherId, line);
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_Wbs_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		try
		{
			tv_Wbs.DataSourceTable();
			int fatherId = Function.ParseInt(tv_Wbs.GetRowCellValue(e.Node, tb_FatherCode));
			string line = Function.ToString(tv_Wbs.GetRowCellValue(e.Node, tb_LineId));
			string a = Function.ToString(tv_Wbs.GetRowCellValue(e.Node, tb_WBSMode));
			if (e.Column.FieldName == "Predecessors" && !string.Equals(a, "A"))
			{
				tv_Wbs.SetRowCellValue(e.Node, tb_WBSMode, "A");
			}
			NewWbsTree(fatherId, line);
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if (e.Column.FieldName == "Code" && FormMode != Enums.FormViewModeType.Add)
		{
			e.Appearance.BackColor = GridColor.GridReadOnly;
		}
		if (e.Column.FieldName == "Object")
		{
			e.Appearance.BackColor = GridColor.GridReadOnly;
		}
		if (Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "Canceled")).Equals("Y"))
		{
			e.Appearance.BackColor = GridColor.GridReadOnly;
		}
	}

	private void tv_Task_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		if (new string[4] { "LineId", "Code", "Object", "FatherNum" }.Contains(e.Column.FieldName))
		{
			e.Appearance.BackColor = GridColor.GridReadOnly;
		}
		int focusedRowHandle = gv_Doc.FocusedRowHandle;
		if (Function.ToString(gv_Doc.GetRowCellValue(focusedRowHandle, "Canceled")).Equals("Y"))
		{
			e.Appearance.BackColor = GridColor.GridReadOnly;
		}
		string text = Function.ToString(tv_Task.GetRowCellValue(e.Node, tl_Assignee));
		if (e.Column.FieldName == "Employee" && text.Equals("U"))
		{
			e.Appearance.BackColor = GridColor.GridReadOnly;
		}
		if (e.Column.FieldName == "User" && text.Equals("E"))
		{
			e.Appearance.BackColor = GridColor.GridReadOnly;
		}
	}

	private void tv_Wbs_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		if (new string[4] { "LineId", "Code", "Object", "FatherCode" }.Contains(e.Column.FieldName))
		{
			e.Appearance.BackColor = GridColor.GridReadOnly;
		}
		int focusedRowHandle = gv_Doc.FocusedRowHandle;
		if (Function.ToString(gv_Doc.GetRowCellValue(focusedRowHandle, "Canceled")).Equals("Y"))
		{
			e.Appearance.BackColor = GridColor.GridReadOnly;
		}
		if (Function.ToString(tv_Wbs.GetRowCellValue(e.Node, tb_Milestone)).Equals("Y"))
		{
			e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);
		}
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		if (gv_Doc.FocusedColumn.FieldName == "Code" && FormMode != Enums.FormViewModeType.Add)
		{
			e.Cancel = true;
		}
		int focusedRowHandle = gv_Doc.FocusedRowHandle;
		if (Function.ToString(gv_Doc.GetRowCellValue(focusedRowHandle, "Canceled")).Equals("Y"))
		{
			e.Cancel = true;
		}
	}

	private async void cancelToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gv_Doc.SetFocusedRowCellValue("Canceled", "Y");
			await Save(2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_Task_ShowingEditor(object sender, CancelEventArgs e)
	{
		int focusedRowHandle = gv_Doc.FocusedRowHandle;
		if (Function.ToString(gv_Doc.GetRowCellValue(focusedRowHandle, "Canceled")).Equals("Y"))
		{
			e.Cancel = true;
		}
		string text = Function.ToString(tv_Task.GetFocusedRowCellValue(tl_Assignee));
		if (tv_Task.FocusedColumn.FieldName == "Employee" && text.Equals("U"))
		{
			e.Cancel = true;
		}
		if (tv_Task.FocusedColumn.FieldName == "User" && text.Equals("E"))
		{
			e.Cancel = true;
		}
	}

	private void tv_Wbs_ShowingEditor(object sender, CancelEventArgs e)
	{
		int focusedRowHandle = gv_Doc.FocusedRowHandle;
		if (Function.ToString(gv_Doc.GetRowCellValue(focusedRowHandle, "Canceled")).Equals("Y"))
		{
			e.Cancel = true;
		}
	}

	private void addRowChildToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(tv_Task.DataSourceTable().Compute("MAX(LineId)", ""));
			int num2 = Function.ParseInt(tv_Task.GetFocusedRowCellValue(tl_LineId));
			NewTaskTree(num2, Function.ToString(num2));
			tv_Task.FocusedNode.Expand();
			tv_Task.FocusedNode = tv_Task.FindNodeByFieldValue("LineId", num + 1);
			tv_Task.FocusedColumn = tl_TaskName;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addRowChildrenInsertToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			_dtWbs = tv_Wbs.DataSourceTable();
			int num = Function.ParseInt(_dtWbs.Compute("MAX(LineId)", ""));
			int num2 = Function.ParseInt(tv_Wbs.GetFocusedRowCellValue(tb_LineId));
			tv_Wbs.SetFocusedRowCellValue("WBSMode", "A");
			NewWbsTree(num2, Function.ToString(num2));
			tv_Wbs.FocusedNode.Expand();
			tv_Wbs.FocusedNode = tv_Wbs.FindNodeByFieldValue("LineId", num + 1);
			tv_Wbs.FocusedColumn = tb_WBSName;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tv_Task_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		if (sender is TreeList)
		{
			e.ExceptionMode = ExceptionMode.DisplayError;
			e.WindowCaption = "Input Error";
		}
	}

	private void tv_Task_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			int result = 0;
			if (!(sender is TreeList treeList) || !(treeList.FocusedColumn.FieldName == "Dependence") || string.IsNullOrEmpty(Function.ToString(e.Value)))
			{
				return;
			}
			int[] array = (from t in Function.ToString(e.Value).Split(',')
				select Function.ParseInt(t)).ToArray();
			int num = Function.ParseInt(tv_Task.GetFocusedRowCellValue(tl_LineId));
			if (array.Length == 0)
			{
				return;
			}
			int[] array2 = array;
			foreach (int num3 in array2)
			{
				if (num3 <= 0)
				{
					e.Valid = false;
					e.ErrorText = "";
					WinCommons.Error(Enums.MessageCode.TypeOnlyPositiveWholeNumber4SimpleRelationshipBetweenDependentInSameProject, "");
					break;
				}
				if (num3 == num)
				{
					e.Valid = false;
					e.ErrorText = "";
					WinCommons.Error(Enums.MessageCode.YouCannotMakeDependentOfItself, "");
					break;
				}
				if (Function.ParseInt(num3) > 0 && !int.TryParse(Function.ToString(num3), out result))
				{
					e.Valid = false;
					e.ErrorText = "";
					WinCommons.Error(Enums.MessageCode.DependentOnlyInputNumberFromColumnLineNum, "");
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_WBSTemplate_FormClosing(object sender, FormClosingEventArgs e)
	{
		if ((btn_Ok.Text == Apzon.Commons.Properties.Resources.ButtonUpdate || btn_Ok.Text == Apzon.Commons.Properties.Resources.ButtonAdd) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			btn_Ok_Click(null, null);
		}
	}

	private void DuplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.FocusedRowHandle != -1)
			{
				_dtTask = tv_Task.DataSourceTable();
				_dtWbs = tv_Wbs.DataSourceTable();
				DataTable dataTable = grd_Doc.DataSourceTable.Copy().Clone();
				DataRow dataRow = dataTable.NewRow();
				dataRow.ItemArray = gv_Doc.GetDataRow(gv_Doc.FocusedRowHandle).ItemArray;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_Doc.DataSource = dataTable;
				grd_Doc.DataSourceTable.AcceptChanges();
				tv_Task.DataSource = _dtTask;
				tv_Task.DataSourceTable().AcceptChanges();
				tv_Wbs.DataSource = _dtWbs;
				tv_Wbs.DataSourceTable().AcceptChanges();
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Tv_Wbs_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			int result = 0;
			if (!(sender is TreeList treeList) || !(treeList.FocusedColumn.FieldName == "Predecessors") || string.IsNullOrEmpty(Function.ToString(e.Value)))
			{
				return;
			}
			int[] array = (from t in Function.ToString(e.Value).Split(',')
				select Function.ParseInt(t)).ToArray();
			int num = Function.ParseInt(tv_Wbs.GetFocusedRowCellValue(tl_LineId));
			if (array.Length == 0)
			{
				return;
			}
			List<int> lst = new List<int>();
			GetAllFatherId(num, ref lst);
			List<int> list = new List<int>();
			GetAllChildId(num, ref lst);
			int[] array2 = array;
			foreach (int num3 in array2)
			{
				if (num3 <= 0)
				{
					e.Valid = false;
					e.ErrorText = "";
					WinCommons.Error(Enums.MessageCode.TypeOnlyPositiveWholeNumber4SimpleRelationshipBetweenWbs, "");
					break;
				}
				if (num3 == num)
				{
					e.Valid = false;
					e.ErrorText = "";
					WinCommons.Error(Enums.MessageCode.YouCannotMakeWbsPredecessorOfItself, "");
					break;
				}
				if (lst.Contains(num3))
				{
					e.Valid = false;
					e.ErrorText = "";
					WinCommons.Error(Enums.MessageCode.ProjectCannotLinkSummaryWbs2ItsSub, "");
					break;
				}
				if (list.Contains(num3))
				{
					e.Valid = false;
					e.ErrorText = "";
					WinCommons.Error(Enums.MessageCode.ProjectCannotLinkSummaryWbs2ItsSub, "");
					break;
				}
				int num4 = Function.ParseInt(num3);
				if (num4 > 0)
				{
					if (!int.TryParse(Function.ToString(num3), out result))
					{
						e.Valid = false;
						e.ErrorText = "";
						WinCommons.Error(Enums.MessageCode.PredecessorOnlyInputFromLineNum, "");
						break;
					}
					if (num4 > 0 && num4 >= num)
					{
						e.Valid = false;
						e.ErrorText = "";
						WinCommons.Error(Enums.MessageCode.PredecessorOnlyGetSmallerSelection, "");
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetAllFatherId(int lineNum, ref List<int> lst)
	{
		try
		{
			DataRow dataRow = tv_Wbs.DataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["LineId"]) == lineNum);
			if (dataRow != null)
			{
				int num = Function.ParseInt(dataRow["FatherCode"]);
				lst.Add(num);
				GetAllFatherId(num, ref lst);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetAllChildId(int lineNum, ref List<int> lst)
	{
		try
		{
			DataRow[] array = (from t in tv_Wbs.DataSourceTable.AsEnumerable()
				where Function.ParseInt(t["FatherCode"]) == lineNum
				select t).ToArray();
			if (array.Length != 0)
			{
				DataRow[] array2 = array;
				for (int num = 0; num < array2.Length; num++)
				{
					int num2 = Function.ParseInt(array2[num]["LineId"]);
					lst.Add(num2);
					GetAllChildId(num2, ref lst);
				}
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.PM.frm_WBSTemplate));
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled17 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled18 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled19 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance20 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered20 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed20 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled20 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions11 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance21 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered21 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed21 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled21 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions11 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance22 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered22 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed22 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled22 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions12 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance23 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered23 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed23 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled23 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions12 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance24 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered24 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed24 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled24 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions13 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance25 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered25 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed25 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled25 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions13 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance26 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered26 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed26 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled26 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions14 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance27 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered27 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed27 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled27 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions14 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance28 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered28 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed28 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled28 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions15 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance29 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered29 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed29 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled29 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions15 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance30 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered30 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed30 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled30 = new DevExpress.Utils.SerializableAppearanceObject();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.cancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx1 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemTimeEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTimeEdit();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx2 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemTimeEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemTimeEdit();
		this.repositoryItemTimeSpanEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTimeSpanEdit();
		this.luk_Mode = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.luk_Milestone = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.memo_Notes = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.btn_OcrCode2 = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.btn_OcrCode3 = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.btn_OcrCode4 = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.btn_OcrCode5 = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.btn_OcrCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.btn_Predecessors = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.btn_Resource = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.xtraTabControlEx1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPageEx2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.tv_Wbs = new Apzon.Controls.Tree.TreeEx();
		this.tb_Code = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_Object = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_WBSName = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_LineId = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_FatherCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_WBSMode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.luk_WbsMode = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.tb_Duration = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_Predecessors = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tb_Milestone = new Apzon.Controls.Tree.TreeListColumnEx();
		this.lu_Milestone = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.tb_Notes = new Apzon.Controls.Tree.TreeListColumnEx();
		this.mem_Notes = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.tb_Resource = new Apzon.Controls.Tree.TreeListColumnEx();
		this.b_Resource = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.ctm_WBS = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.addRowChildrenInsertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.b_OcrCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.b_OcrCode3 = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.b_OcrCode4 = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.b_OcrCode2 = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.b_OcrCode5 = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.xtraTabPageEx1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.tv_Task = new Apzon.Controls.Tree.TreeEx();
		this.tl_Code = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_Object = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_WBSCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_TaskName = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_Leadtime = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_Dependence = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_Note = new Apzon.Controls.Tree.TreeListColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.tl_LineId = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_FatherCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_Assignee = new Apzon.Controls.Tree.TreeListColumnEx();
		this.luk_Assignee = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.tl_Employee = new Apzon.Controls.Tree.TreeListColumnEx();
		this.bts_Employee = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.tl_User = new Apzon.Controls.Tree.TreeListColumnEx();
		this.bts_User = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.ctm_Task = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.addRowChildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		this.date_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctm_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTimeEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTimeEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTimeSpanEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Mode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Milestone).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Notes).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_OcrCode2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_OcrCode3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_OcrCode4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_OcrCode5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_OcrCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Predecessors).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Resource).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).BeginInit();
		this.xtraTabControlEx1.SuspendLayout();
		this.xtraTabPageEx2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tv_Wbs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_WbsMode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lu_Milestone).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mem_Notes).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.b_Resource).BeginInit();
		this.ctm_WBS.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.b_OcrCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.b_OcrCode3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.b_OcrCode4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.b_OcrCode2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.b_OcrCode5).BeginInit();
		this.xtraTabPageEx1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tv_Task).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Assignee).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bts_Employee).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bts_User).BeginInit();
		this.ctm_Task.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 60);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControlEx1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(981, 347);
		this.splitContainerControl1.SplitterPosition = 146;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctm_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[15]
		{
			this.repositoryItemDateEditEx1, this.repositoryItemTimeSpanEdit1, this.repositoryItemDateEditEx2, this.repositoryItemTimeEdit1, this.repositoryItemTimeEdit2, this.luk_Mode, this.luk_Milestone, this.memo_Notes, this.btn_OcrCode2, this.btn_OcrCode3,
			this.btn_OcrCode4, this.btn_OcrCode5, this.btn_OcrCode, this.btn_Predecessors, this.btn_Resource
		});
		this.grd_Doc.Size = new System.Drawing.Size(981, 146);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctm_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.addNewToolStripMenuItem, this.duplicateToolStripMenuItem, this.deleteToolStripMenuItem, this.cancelToolStripMenuItem });
		this.ctm_Doc.Name = "ctm_Doc";
		this.ctm_Doc.Size = new System.Drawing.Size(125, 92);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.duplicateToolStripMenuItem.Text = "Duplicate";
		this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.DuplicateToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.deleteToolStripMenuItem.Text = "Remove";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
		this.cancelToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.cancelToolStripMenuItem.Text = "Cancel";
		this.cancelToolStripMenuItem.Click += new System.EventHandler(this.cancelToolStripMenuItem_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oWBSTemplate";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_OWBS";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Doc.OptionsPrint.AutoWidth = false;
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
		this.gridColumnEx1.Caption = "Code";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "Code";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx2.Caption = "Name";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Name";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx3.Caption = "DocEntry";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "DocEntry";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.AllowEdit = false;
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx4.Caption = "Canceled";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "Canceled";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx5.Caption = "Object";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "Object";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.OptionsColumn.ReadOnly = true;
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx6.Caption = "CreateDate";
		this.gridColumnEx6.ColumnEdit = this.repositoryItemDateEditEx1;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx6.FieldName = "CreateDate";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.OptionsColumn.AllowEdit = false;
		this.gridColumnEx6.OptionsColumn.ReadOnly = true;
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.repositoryItemDateEditEx1.AutoHeight = false;
		this.repositoryItemDateEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx1.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.IsSearchControl = false;
		this.repositoryItemDateEditEx1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx1.Name = "repositoryItemDateEditEx1";
		this.gridColumnEx7.Caption = "CreateTime";
		this.gridColumnEx7.ColumnEdit = this.repositoryItemTimeEdit1;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Time;
		this.gridColumnEx7.FieldName = "CreateTime";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.OptionsColumn.AllowEdit = false;
		this.gridColumnEx7.OptionsColumn.ReadOnly = true;
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.repositoryItemTimeEdit1.AutoHeight = false;
		this.repositoryItemTimeEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemTimeEdit1.Name = "repositoryItemTimeEdit1";
		this.gridColumnEx8.Caption = "UpdateDate";
		this.gridColumnEx8.ColumnEdit = this.repositoryItemDateEditEx2;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx8.FieldName = "UpdateDate";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.OptionsColumn.AllowEdit = false;
		this.gridColumnEx8.OptionsColumn.ReadOnly = true;
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.repositoryItemDateEditEx2.AutoHeight = false;
		this.repositoryItemDateEditEx2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx2.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx2.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx2.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx2.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx2.IsSearchControl = false;
		this.repositoryItemDateEditEx2.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx2.Name = "repositoryItemDateEditEx2";
		this.gridColumnEx9.Caption = "UpdateTime";
		this.gridColumnEx9.ColumnEdit = this.repositoryItemTimeEdit2;
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Time;
		this.gridColumnEx9.FieldName = "UpdateTime";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.OptionsColumn.AllowEdit = false;
		this.gridColumnEx9.OptionsColumn.ReadOnly = true;
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.repositoryItemTimeEdit2.AutoHeight = false;
		this.repositoryItemTimeEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemTimeEdit2.Name = "repositoryItemTimeEdit2";
		this.repositoryItemTimeSpanEdit1.AutoHeight = false;
		this.repositoryItemTimeSpanEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemTimeSpanEdit1.Mask.EditMask = "dd.HH:mm:ss";
		this.repositoryItemTimeSpanEdit1.Name = "repositoryItemTimeSpanEdit1";
		this.luk_Mode.AutoHeight = false;
		this.luk_Mode.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Mode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Mode.IsSearchControl = false;
		this.luk_Mode.Name = "luk_Mode";
		this.luk_Mode.NullText = "";
		this.luk_Mode.ShowFooter = false;
		this.luk_Mode.ShowHeader = false;
		this.luk_Milestone.AutoHeight = false;
		this.luk_Milestone.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Milestone.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Milestone.IsSearchControl = false;
		this.luk_Milestone.Name = "luk_Milestone";
		this.luk_Milestone.NullText = "";
		this.luk_Milestone.ShowFooter = false;
		this.luk_Milestone.ShowHeader = false;
		this.memo_Notes.AutoHeight = false;
		this.memo_Notes.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.memo_Notes.Name = "memo_Notes";
		this.memo_Notes.ShowIcon = false;
		this.btn_OcrCode2.AutoHeight = false;
		this.btn_OcrCode2.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.btn_OcrCode2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_OcrCode2.ContainerControl = this;
		this.btn_OcrCode2.DisplayMember = "";
		this.btn_OcrCode2.DisplayMember2 = "";
		this.btn_OcrCode2.LoadDataSourceWhenEnter = false;
		this.btn_OcrCode2.Name = "btn_OcrCode2";
		this.btn_OcrCode2.SeparateCharacterForMulti = ';';
		this.btn_OcrCode2.SeparateString = ", ";
		this.btn_OcrCode2.UsingAutoCompleteBox = false;
		this.btn_OcrCode2.UsingDataSource = true;
		this.btn_OcrCode2.ValidateWhenNoDataSource = false;
		this.btn_OcrCode2.ValueMember = "";
		this.btn_OcrCode3.AutoHeight = false;
		this.btn_OcrCode3.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.btn_OcrCode3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_OcrCode3.ContainerControl = this;
		this.btn_OcrCode3.DisplayMember = "";
		this.btn_OcrCode3.DisplayMember2 = "";
		this.btn_OcrCode3.LoadDataSourceWhenEnter = false;
		this.btn_OcrCode3.Name = "btn_OcrCode3";
		this.btn_OcrCode3.SeparateCharacterForMulti = ';';
		this.btn_OcrCode3.SeparateString = ", ";
		this.btn_OcrCode3.UsingAutoCompleteBox = false;
		this.btn_OcrCode3.UsingDataSource = true;
		this.btn_OcrCode3.ValidateWhenNoDataSource = false;
		this.btn_OcrCode3.ValueMember = "";
		this.btn_OcrCode4.AutoHeight = false;
		this.btn_OcrCode4.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.btn_OcrCode4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_OcrCode4.ContainerControl = this;
		this.btn_OcrCode4.DisplayMember = "";
		this.btn_OcrCode4.DisplayMember2 = "";
		this.btn_OcrCode4.LoadDataSourceWhenEnter = false;
		this.btn_OcrCode4.Name = "btn_OcrCode4";
		this.btn_OcrCode4.SeparateCharacterForMulti = ';';
		this.btn_OcrCode4.SeparateString = ", ";
		this.btn_OcrCode4.UsingAutoCompleteBox = false;
		this.btn_OcrCode4.UsingDataSource = true;
		this.btn_OcrCode4.ValidateWhenNoDataSource = false;
		this.btn_OcrCode4.ValueMember = "";
		this.btn_OcrCode5.AutoHeight = false;
		this.btn_OcrCode5.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		this.btn_OcrCode5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_OcrCode5.ContainerControl = this;
		this.btn_OcrCode5.DisplayMember = "";
		this.btn_OcrCode5.DisplayMember2 = "";
		this.btn_OcrCode5.LoadDataSourceWhenEnter = false;
		this.btn_OcrCode5.Name = "btn_OcrCode5";
		this.btn_OcrCode5.SeparateCharacterForMulti = ';';
		this.btn_OcrCode5.SeparateString = ", ";
		this.btn_OcrCode5.UsingAutoCompleteBox = false;
		this.btn_OcrCode5.UsingDataSource = true;
		this.btn_OcrCode5.ValidateWhenNoDataSource = false;
		this.btn_OcrCode5.ValueMember = "";
		this.btn_OcrCode.AutoHeight = false;
		this.btn_OcrCode.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions10.Image");
		this.btn_OcrCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_OcrCode.ContainerControl = this;
		this.btn_OcrCode.DisplayMember = "";
		this.btn_OcrCode.DisplayMember2 = "";
		this.btn_OcrCode.LoadDataSourceWhenEnter = false;
		this.btn_OcrCode.Name = "btn_OcrCode";
		this.btn_OcrCode.SeparateCharacterForMulti = ';';
		this.btn_OcrCode.SeparateString = ", ";
		this.btn_OcrCode.UsingAutoCompleteBox = false;
		this.btn_OcrCode.UsingDataSource = true;
		this.btn_OcrCode.ValidateWhenNoDataSource = false;
		this.btn_OcrCode.ValueMember = "";
		this.btn_Predecessors.AutoHeight = false;
		this.btn_Predecessors.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions12.Image");
		this.btn_Predecessors.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance11, appearanceHovered11, appearancePressed11, appearanceDisabled11, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance12, appearanceHovered12, appearancePressed12, appearanceDisabled12, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Predecessors.ContainerControl = this;
		this.btn_Predecessors.DisplayMember = "";
		this.btn_Predecessors.DisplayMember2 = "";
		this.btn_Predecessors.LoadDataSourceWhenEnter = false;
		this.btn_Predecessors.Name = "btn_Predecessors";
		this.btn_Predecessors.SeparateCharacterForMulti = ';';
		this.btn_Predecessors.SeparateString = ", ";
		this.btn_Predecessors.UsingAutoCompleteBox = false;
		this.btn_Predecessors.UsingDataSource = true;
		this.btn_Predecessors.ValidateWhenNoDataSource = false;
		this.btn_Predecessors.ValueMember = "";
		this.btn_Resource.AutoHeight = false;
		this.btn_Resource.AutoLoadDataSource = true;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions14.Image");
		this.btn_Resource.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance13, appearanceHovered13, appearancePressed13, appearanceDisabled13, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance14, appearanceHovered14, appearancePressed14, appearanceDisabled14, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Resource.ContainerControl = this;
		this.btn_Resource.DisplayMember = "";
		this.btn_Resource.DisplayMember2 = "";
		this.btn_Resource.LoadDataSourceWhenEnter = false;
		this.btn_Resource.Name = "btn_Resource";
		this.btn_Resource.SeparateCharacterForMulti = ';';
		this.btn_Resource.SeparateString = ", ";
		this.btn_Resource.UsingAutoCompleteBox = false;
		this.btn_Resource.UsingDataSource = true;
		this.btn_Resource.ValidateWhenNoDataSource = false;
		this.btn_Resource.ValueMember = "EmpId";
		this.xtraTabControlEx1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControlEx1.IsSearchControl = false;
		this.xtraTabControlEx1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControlEx1.Name = "xtraTabControlEx1";
		this.xtraTabControlEx1.SelectedTabPage = this.xtraTabPageEx2;
		this.xtraTabControlEx1.Size = new System.Drawing.Size(981, 191);
		this.xtraTabControlEx1.TabIndex = 0;
		this.xtraTabControlEx1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPageEx2, this.xtraTabPageEx1 });
		this.xtraTabPageEx2.Controls.Add(this.tv_Wbs);
		this.xtraTabPageEx2.EditControl = null;
		this.xtraTabPageEx2.EditValue = "WBS";
		this.xtraTabPageEx2.FieldName = null;
		this.xtraTabPageEx2.IsSearchControl = false;
		this.xtraTabPageEx2.Name = "xtraTabPageEx2";
		this.xtraTabPageEx2.ReadOnly = false;
		this.xtraTabPageEx2.SettingInVisible = false;
		this.xtraTabPageEx2.SettingReadonly = false;
		this.xtraTabPageEx2.Size = new System.Drawing.Size(979, 166);
		this.xtraTabPageEx2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageEx2.Text = "WBS";
		this.tv_Wbs.AllowPasteExcel = true;
		this.tv_Wbs.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tv_Wbs.ColumnKey = "WBSName";
		this.tv_Wbs.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[11]
		{
			this.tb_Code, this.tb_Object, this.tb_WBSName, this.tb_LineId, this.tb_FatherCode, this.tb_WBSMode, this.tb_Duration, this.tb_Predecessors, this.tb_Milestone, this.tb_Notes,
			this.tb_Resource
		});
		this.tv_Wbs.ContextMenuStrip = this.ctm_WBS;
		this.tv_Wbs.Cursor = System.Windows.Forms.Cursors.Default;
		this.tv_Wbs.DisableFormSetting = false;
		this.tv_Wbs.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tv_Wbs.EditColumn = null;
		this.tv_Wbs.FieldCurrency = null;
		this.tv_Wbs.FieldRate = null;
		this.tv_Wbs.FormId = "oWBSTemplate";
		this.tv_Wbs.IndexEditRow = 0;
		this.tv_Wbs.IndicatorWidth = 40;
		this.tv_Wbs.IsApzTable = false;
		this.tv_Wbs.IsLoadingData = false;
		this.tv_Wbs.ItemId = "APZ_WBS1";
		this.tv_Wbs.KeyFieldName = "LineId";
		this.tv_Wbs.Location = new System.Drawing.Point(0, 0);
		this.tv_Wbs.LstColumnNotEditable = null;
		this.tv_Wbs.LstColumNotSetting = null;
		this.tv_Wbs.Name = "tv_Wbs";
		this.tv_Wbs.ParentFieldName = "FatherCode";
		this.tv_Wbs.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[9] { this.luk_WbsMode, this.lu_Milestone, this.mem_Notes, this.b_OcrCode, this.b_OcrCode3, this.b_OcrCode4, this.b_OcrCode2, this.b_OcrCode5, this.b_Resource });
		this.tv_Wbs.Size = new System.Drawing.Size(979, 166);
		this.tv_Wbs.TabIndex = 0;
		this.tv_Wbs.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_Wbs_CellValueChanged);
		this.tv_Wbs.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.tv_Wbs_NodeCellStyle);
		this.tv_Wbs.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.Tv_Wbs_ValidatingEditor);
		this.tv_Wbs.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.tv_Wbs_ShowingEditor);
		this.tv_Wbs.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.tv_Wbs_EditorKeyDown);
		this.tv_Wbs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tv_Wbs_KeyDown);
		this.tb_Code.Caption = "Code";
		this.tb_Code.CurrencyDateField = null;
		this.tb_Code.CurrencyField = null;
		this.tb_Code.CurrencyFixed = null;
		this.tb_Code.CurrencyFixedRate = 0.0;
		this.tb_Code.CurrencyRateField = null;
		this.tb_Code.FieldName = "Code";
		this.tb_Code.Name = "tb_Code";
		this.tb_Code.OptionsColumn.ReadOnly = true;
		this.tb_Code.Visible = true;
		this.tb_Code.VisibleIndex = 0;
		this.tb_Object.Caption = "Object";
		this.tb_Object.CurrencyDateField = null;
		this.tb_Object.CurrencyField = null;
		this.tb_Object.CurrencyFixed = null;
		this.tb_Object.CurrencyFixedRate = 0.0;
		this.tb_Object.CurrencyRateField = null;
		this.tb_Object.FieldName = "Object";
		this.tb_Object.Name = "tb_Object";
		this.tb_Object.OptionsColumn.ReadOnly = true;
		this.tb_WBSName.Caption = "WBSName";
		this.tb_WBSName.CurrencyDateField = null;
		this.tb_WBSName.CurrencyField = null;
		this.tb_WBSName.CurrencyFixed = null;
		this.tb_WBSName.CurrencyFixedRate = 0.0;
		this.tb_WBSName.CurrencyRateField = null;
		this.tb_WBSName.FieldName = "WBSName";
		this.tb_WBSName.Name = "tb_WBSName";
		this.tb_WBSName.Visible = true;
		this.tb_WBSName.VisibleIndex = 1;
		this.tb_LineId.Caption = "LineId";
		this.tb_LineId.CurrencyDateField = null;
		this.tb_LineId.CurrencyField = null;
		this.tb_LineId.CurrencyFixed = null;
		this.tb_LineId.CurrencyFixedRate = 0.0;
		this.tb_LineId.CurrencyRateField = null;
		this.tb_LineId.FieldName = "LineId";
		this.tb_LineId.Name = "tb_LineId";
		this.tb_LineId.OptionsColumn.ReadOnly = true;
		this.tb_LineId.Visible = true;
		this.tb_LineId.VisibleIndex = 8;
		this.tb_FatherCode.Caption = "FatherCode";
		this.tb_FatherCode.CurrencyDateField = null;
		this.tb_FatherCode.CurrencyField = null;
		this.tb_FatherCode.CurrencyFixed = null;
		this.tb_FatherCode.CurrencyFixedRate = 0.0;
		this.tb_FatherCode.CurrencyRateField = null;
		this.tb_FatherCode.FieldName = "FatherCode";
		this.tb_FatherCode.Name = "tb_FatherCode";
		this.tb_FatherCode.OptionsColumn.ReadOnly = true;
		this.tb_WBSMode.Caption = "WBS Mode";
		this.tb_WBSMode.ColumnEdit = this.luk_WbsMode;
		this.tb_WBSMode.CurrencyDateField = null;
		this.tb_WBSMode.CurrencyField = null;
		this.tb_WBSMode.CurrencyFixed = null;
		this.tb_WBSMode.CurrencyFixedRate = 0.0;
		this.tb_WBSMode.CurrencyRateField = null;
		this.tb_WBSMode.FieldName = "WBSMode";
		this.tb_WBSMode.Name = "tb_WBSMode";
		this.tb_WBSMode.Visible = true;
		this.tb_WBSMode.VisibleIndex = 2;
		this.luk_WbsMode.AutoHeight = false;
		this.luk_WbsMode.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_WbsMode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_WbsMode.IsSearchControl = false;
		this.luk_WbsMode.Name = "luk_WbsMode";
		this.luk_WbsMode.NullText = "";
		this.luk_WbsMode.ShowFooter = false;
		this.luk_WbsMode.ShowHeader = false;
		this.tb_Duration.Caption = "Duration";
		this.tb_Duration.CurrencyDateField = null;
		this.tb_Duration.CurrencyField = null;
		this.tb_Duration.CurrencyFixed = null;
		this.tb_Duration.CurrencyFixedRate = 0.0;
		this.tb_Duration.CurrencyRateField = null;
		this.tb_Duration.FieldName = "Duration";
		this.tb_Duration.Name = "tb_Duration";
		this.tb_Duration.Visible = true;
		this.tb_Duration.VisibleIndex = 3;
		this.tb_Predecessors.Caption = "Predecessors";
		this.tb_Predecessors.CurrencyDateField = null;
		this.tb_Predecessors.CurrencyField = null;
		this.tb_Predecessors.CurrencyFixed = null;
		this.tb_Predecessors.CurrencyFixedRate = 0.0;
		this.tb_Predecessors.CurrencyRateField = null;
		this.tb_Predecessors.FieldName = "Predecessors";
		this.tb_Predecessors.Name = "tb_Predecessors";
		this.tb_Predecessors.Visible = true;
		this.tb_Predecessors.VisibleIndex = 4;
		this.tb_Milestone.Caption = "Milestone";
		this.tb_Milestone.ColumnEdit = this.lu_Milestone;
		this.tb_Milestone.CurrencyDateField = null;
		this.tb_Milestone.CurrencyField = null;
		this.tb_Milestone.CurrencyFixed = null;
		this.tb_Milestone.CurrencyFixedRate = 0.0;
		this.tb_Milestone.CurrencyRateField = null;
		this.tb_Milestone.FieldName = "Milestone";
		this.tb_Milestone.Name = "tb_Milestone";
		this.tb_Milestone.Visible = true;
		this.tb_Milestone.VisibleIndex = 5;
		this.lu_Milestone.AutoHeight = false;
		this.lu_Milestone.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.lu_Milestone.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lu_Milestone.IsSearchControl = false;
		this.lu_Milestone.Name = "lu_Milestone";
		this.lu_Milestone.NullText = "";
		this.lu_Milestone.ShowFooter = false;
		this.lu_Milestone.ShowHeader = false;
		this.tb_Notes.Caption = "Notes";
		this.tb_Notes.ColumnEdit = this.mem_Notes;
		this.tb_Notes.CurrencyDateField = null;
		this.tb_Notes.CurrencyField = null;
		this.tb_Notes.CurrencyFixed = null;
		this.tb_Notes.CurrencyFixedRate = 0.0;
		this.tb_Notes.CurrencyRateField = null;
		this.tb_Notes.FieldName = "Notes";
		this.tb_Notes.Name = "tb_Notes";
		this.tb_Notes.Visible = true;
		this.tb_Notes.VisibleIndex = 6;
		this.mem_Notes.AutoHeight = false;
		this.mem_Notes.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.mem_Notes.Name = "mem_Notes";
		this.mem_Notes.ShowIcon = false;
		this.tb_Resource.Caption = "Resource";
		this.tb_Resource.ColumnEdit = this.b_Resource;
		this.tb_Resource.CurrencyDateField = null;
		this.tb_Resource.CurrencyField = null;
		this.tb_Resource.CurrencyFixed = null;
		this.tb_Resource.CurrencyFixedRate = 0.0;
		this.tb_Resource.CurrencyRateField = null;
		this.tb_Resource.FieldName = "Resource";
		this.tb_Resource.Name = "tb_Resource";
		this.tb_Resource.Visible = true;
		this.tb_Resource.VisibleIndex = 7;
		this.b_Resource.AutoHeight = false;
		this.b_Resource.AutoLoadDataSource = true;
		editorButtonImageOptions8.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions16.Image");
		this.b_Resource.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance15, appearanceHovered15, appearancePressed15, appearanceDisabled15, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, editorButtonImageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance16, appearanceHovered16, appearancePressed16, appearanceDisabled16, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.b_Resource.ContainerControl = this;
		this.b_Resource.DisplayMember = "";
		this.b_Resource.DisplayMember2 = "";
		this.b_Resource.LoadDataSourceWhenEnter = false;
		this.b_Resource.Name = "b_Resource";
		this.b_Resource.SeparateCharacterForMulti = ';';
		this.b_Resource.SeparateString = ", ";
		this.b_Resource.UsingAutoCompleteBox = false;
		this.b_Resource.UsingDataSource = true;
		this.b_Resource.ValidateWhenNoDataSource = false;
		this.b_Resource.ValueMember = "";
		this.ctm_WBS.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addRowToolStripMenuItem1, this.deleteRowToolStripMenuItem1, this.addRowChildrenInsertToolStripMenuItem });
		this.ctm_WBS.Name = "ctm_WBS";
		this.ctm_WBS.Size = new System.Drawing.Size(208, 70);
		this.addRowToolStripMenuItem1.Name = "addRowToolStripMenuItem1";
		this.addRowToolStripMenuItem1.Size = new System.Drawing.Size(207, 22);
		this.addRowToolStripMenuItem1.Text = "Add Row(Enter)";
		this.addRowToolStripMenuItem1.Click += new System.EventHandler(this.addRowToolStripMenuItem1_Click);
		this.deleteRowToolStripMenuItem1.Name = "deleteRowToolStripMenuItem1";
		this.deleteRowToolStripMenuItem1.Size = new System.Drawing.Size(207, 22);
		this.deleteRowToolStripMenuItem1.Text = "Delete Row(Shift+Delete)";
		this.deleteRowToolStripMenuItem1.Click += new System.EventHandler(this.deleteRowToolStripMenuItem1_Click);
		this.addRowChildrenInsertToolStripMenuItem.Name = "addRowChildrenInsertToolStripMenuItem";
		this.addRowChildrenInsertToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
		this.addRowChildrenInsertToolStripMenuItem.Text = "Add Row Children(Insert)";
		this.addRowChildrenInsertToolStripMenuItem.Click += new System.EventHandler(this.addRowChildrenInsertToolStripMenuItem_Click);
		this.b_OcrCode.AutoHeight = false;
		this.b_OcrCode.AutoLoadDataSource = true;
		editorButtonImageOptions9.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions18.Image");
		this.b_OcrCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance17, appearanceHovered17, appearancePressed17, appearanceDisabled17, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance18, appearanceHovered18, appearancePressed18, appearanceDisabled18, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.b_OcrCode.ContainerControl = this;
		this.b_OcrCode.DisplayMember = "";
		this.b_OcrCode.DisplayMember2 = "";
		this.b_OcrCode.LoadDataSourceWhenEnter = false;
		this.b_OcrCode.Name = "b_OcrCode";
		this.b_OcrCode.SeparateCharacterForMulti = ';';
		this.b_OcrCode.SeparateString = ", ";
		this.b_OcrCode.UsingAutoCompleteBox = false;
		this.b_OcrCode.UsingDataSource = true;
		this.b_OcrCode.ValidateWhenNoDataSource = false;
		this.b_OcrCode.ValueMember = "";
		this.b_OcrCode3.AutoHeight = false;
		this.b_OcrCode3.AutoLoadDataSource = true;
		editorButtonImageOptions10.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions20.Image");
		this.b_OcrCode3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions10, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance19, appearanceHovered19, appearancePressed19, appearanceDisabled19, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions10, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance20, appearanceHovered20, appearancePressed20, appearanceDisabled20, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.b_OcrCode3.ContainerControl = this;
		this.b_OcrCode3.DisplayMember = "";
		this.b_OcrCode3.DisplayMember2 = "";
		this.b_OcrCode3.LoadDataSourceWhenEnter = false;
		this.b_OcrCode3.Name = "b_OcrCode3";
		this.b_OcrCode3.SeparateCharacterForMulti = ';';
		this.b_OcrCode3.SeparateString = ", ";
		this.b_OcrCode3.UsingAutoCompleteBox = false;
		this.b_OcrCode3.UsingDataSource = true;
		this.b_OcrCode3.ValidateWhenNoDataSource = false;
		this.b_OcrCode3.ValueMember = "";
		this.b_OcrCode4.AutoHeight = false;
		this.b_OcrCode4.AutoLoadDataSource = true;
		editorButtonImageOptions11.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions22.Image");
		this.b_OcrCode4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions11, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance21, appearanceHovered21, appearancePressed21, appearanceDisabled21, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions11, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance22, appearanceHovered22, appearancePressed22, appearanceDisabled22, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.b_OcrCode4.ContainerControl = this;
		this.b_OcrCode4.DisplayMember = "";
		this.b_OcrCode4.DisplayMember2 = "";
		this.b_OcrCode4.LoadDataSourceWhenEnter = false;
		this.b_OcrCode4.Name = "b_OcrCode4";
		this.b_OcrCode4.SeparateCharacterForMulti = ';';
		this.b_OcrCode4.SeparateString = ", ";
		this.b_OcrCode4.UsingAutoCompleteBox = false;
		this.b_OcrCode4.UsingDataSource = true;
		this.b_OcrCode4.ValidateWhenNoDataSource = false;
		this.b_OcrCode4.ValueMember = "";
		this.b_OcrCode2.AutoHeight = false;
		this.b_OcrCode2.AutoLoadDataSource = true;
		editorButtonImageOptions12.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions24.Image");
		this.b_OcrCode2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions12, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance23, appearanceHovered23, appearancePressed23, appearanceDisabled23, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions12, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance24, appearanceHovered24, appearancePressed24, appearanceDisabled24, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.b_OcrCode2.ContainerControl = this;
		this.b_OcrCode2.DisplayMember = "";
		this.b_OcrCode2.DisplayMember2 = "";
		this.b_OcrCode2.LoadDataSourceWhenEnter = false;
		this.b_OcrCode2.Name = "b_OcrCode2";
		this.b_OcrCode2.SeparateCharacterForMulti = ';';
		this.b_OcrCode2.SeparateString = ", ";
		this.b_OcrCode2.UsingAutoCompleteBox = false;
		this.b_OcrCode2.UsingDataSource = true;
		this.b_OcrCode2.ValidateWhenNoDataSource = false;
		this.b_OcrCode2.ValueMember = "";
		this.b_OcrCode5.AutoHeight = false;
		this.b_OcrCode5.AutoLoadDataSource = true;
		editorButtonImageOptions13.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions26.Image");
		this.b_OcrCode5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions13, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance25, appearanceHovered25, appearancePressed25, appearanceDisabled25, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions13, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance26, appearanceHovered26, appearancePressed26, appearanceDisabled26, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.b_OcrCode5.ContainerControl = this;
		this.b_OcrCode5.DisplayMember = "";
		this.b_OcrCode5.DisplayMember2 = "";
		this.b_OcrCode5.LoadDataSourceWhenEnter = false;
		this.b_OcrCode5.Name = "b_OcrCode5";
		this.b_OcrCode5.SeparateCharacterForMulti = ';';
		this.b_OcrCode5.SeparateString = ", ";
		this.b_OcrCode5.UsingAutoCompleteBox = false;
		this.b_OcrCode5.UsingDataSource = true;
		this.b_OcrCode5.ValidateWhenNoDataSource = false;
		this.b_OcrCode5.ValueMember = "";
		this.xtraTabPageEx1.Controls.Add(this.tv_Task);
		this.xtraTabPageEx1.EditControl = null;
		this.xtraTabPageEx1.EditValue = "Task";
		this.xtraTabPageEx1.FieldName = "";
		this.xtraTabPageEx1.IsSearchControl = false;
		this.xtraTabPageEx1.Name = "xtraTabPageEx1";
		this.xtraTabPageEx1.ReadOnly = false;
		this.xtraTabPageEx1.SettingInVisible = false;
		this.xtraTabPageEx1.SettingReadonly = false;
		this.xtraTabPageEx1.Size = new System.Drawing.Size(979, 166);
		this.xtraTabPageEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageEx1.Text = "Task";
		this.tv_Task.AllowPasteExcel = true;
		this.tv_Task.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tv_Task.ColumnKey = "WBSCode";
		this.tv_Task.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[12]
		{
			this.tl_Code, this.tl_Object, this.tl_WBSCode, this.tl_TaskName, this.tl_Leadtime, this.tl_Dependence, this.tl_Note, this.tl_LineId, this.tl_FatherCode, this.tl_Assignee,
			this.tl_Employee, this.tl_User
		});
		this.tv_Task.ContextMenuStrip = this.ctm_Task;
		this.tv_Task.Cursor = System.Windows.Forms.Cursors.Default;
		this.tv_Task.DisableFormSetting = false;
		this.tv_Task.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tv_Task.EditColumn = null;
		this.tv_Task.FieldCurrency = null;
		this.tv_Task.FieldRate = null;
		this.tv_Task.FormId = "oWBSTemplate";
		this.tv_Task.IndexEditRow = 0;
		this.tv_Task.IndicatorWidth = 40;
		this.tv_Task.IsApzTable = false;
		this.tv_Task.IsLoadingData = false;
		this.tv_Task.ItemId = "APZ_TASK";
		this.tv_Task.KeyFieldName = "LineId";
		this.tv_Task.Location = new System.Drawing.Point(0, 0);
		this.tv_Task.LstColumnNotEditable = null;
		this.tv_Task.LstColumNotSetting = null;
		this.tv_Task.Name = "tv_Task";
		this.tv_Task.ParentFieldName = "FatherNum";
		this.tv_Task.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.repositoryItemMemoExEdit1, this.luk_Assignee, this.bts_Employee, this.bts_User });
		this.tv_Task.Size = new System.Drawing.Size(979, 166);
		this.tv_Task.TabIndex = 0;
		this.tv_Task.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tv_Task_CellValueChanged);
		this.tv_Task.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.tv_Task_NodeCellStyle);
		this.tv_Task.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.tv_Task_ValidatingEditor);
		this.tv_Task.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.tv_Task_InvalidValueException);
		this.tv_Task.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.tv_Task_ShowingEditor);
		this.tv_Task.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.tv_Task_EditorKeyDown);
		this.tv_Task.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tv_Task_KeyDown);
		this.tl_Code.Caption = "Code";
		this.tl_Code.CurrencyDateField = null;
		this.tl_Code.CurrencyField = null;
		this.tl_Code.CurrencyFixed = null;
		this.tl_Code.CurrencyFixedRate = 0.0;
		this.tl_Code.CurrencyRateField = null;
		this.tl_Code.FieldName = "Code";
		this.tl_Code.Name = "tl_Code";
		this.tl_Code.OptionsColumn.ReadOnly = true;
		this.tl_Code.Visible = true;
		this.tl_Code.VisibleIndex = 0;
		this.tl_Object.Caption = "Object";
		this.tl_Object.CurrencyDateField = null;
		this.tl_Object.CurrencyField = null;
		this.tl_Object.CurrencyFixed = null;
		this.tl_Object.CurrencyFixedRate = 0.0;
		this.tl_Object.CurrencyRateField = null;
		this.tl_Object.FieldName = "Object";
		this.tl_Object.Name = "tl_Object";
		this.tl_Object.OptionsColumn.ReadOnly = true;
		this.tl_WBSCode.Caption = "WBSCode";
		this.tl_WBSCode.CurrencyDateField = null;
		this.tl_WBSCode.CurrencyField = null;
		this.tl_WBSCode.CurrencyFixed = null;
		this.tl_WBSCode.CurrencyFixedRate = 0.0;
		this.tl_WBSCode.CurrencyRateField = null;
		this.tl_WBSCode.FieldName = "WBSCode";
		this.tl_WBSCode.Name = "tl_WBSCode";
		this.tl_WBSCode.Visible = true;
		this.tl_WBSCode.VisibleIndex = 1;
		this.tl_TaskName.Caption = "TaskName";
		this.tl_TaskName.CurrencyDateField = null;
		this.tl_TaskName.CurrencyField = null;
		this.tl_TaskName.CurrencyFixed = null;
		this.tl_TaskName.CurrencyFixedRate = 0.0;
		this.tl_TaskName.CurrencyRateField = null;
		this.tl_TaskName.FieldName = "TaskName";
		this.tl_TaskName.Name = "tl_TaskName";
		this.tl_TaskName.Visible = true;
		this.tl_TaskName.VisibleIndex = 2;
		this.tl_Leadtime.Caption = "Leadtime";
		this.tl_Leadtime.CurrencyDateField = null;
		this.tl_Leadtime.CurrencyField = null;
		this.tl_Leadtime.CurrencyFixed = null;
		this.tl_Leadtime.CurrencyFixedRate = 0.0;
		this.tl_Leadtime.CurrencyRateField = null;
		this.tl_Leadtime.FieldName = "Leadtime";
		this.tl_Leadtime.Name = "tl_Leadtime";
		this.tl_Leadtime.Visible = true;
		this.tl_Leadtime.VisibleIndex = 3;
		this.tl_Dependence.Caption = "Dependence";
		this.tl_Dependence.CurrencyDateField = null;
		this.tl_Dependence.CurrencyField = null;
		this.tl_Dependence.CurrencyFixed = null;
		this.tl_Dependence.CurrencyFixedRate = 0.0;
		this.tl_Dependence.CurrencyRateField = null;
		this.tl_Dependence.FieldName = "Dependence";
		this.tl_Dependence.Name = "tl_Dependence";
		this.tl_Dependence.Visible = true;
		this.tl_Dependence.VisibleIndex = 4;
		this.tl_Note.Caption = "Note";
		this.tl_Note.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.tl_Note.CurrencyDateField = null;
		this.tl_Note.CurrencyField = null;
		this.tl_Note.CurrencyFixed = null;
		this.tl_Note.CurrencyFixedRate = 0.0;
		this.tl_Note.CurrencyRateField = null;
		this.tl_Note.FieldName = "Note";
		this.tl_Note.Name = "tl_Note";
		this.tl_Note.Visible = true;
		this.tl_Note.VisibleIndex = 5;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.tl_LineId.Caption = "LineId";
		this.tl_LineId.CurrencyDateField = null;
		this.tl_LineId.CurrencyField = null;
		this.tl_LineId.CurrencyFixed = null;
		this.tl_LineId.CurrencyFixedRate = 0.0;
		this.tl_LineId.CurrencyRateField = null;
		this.tl_LineId.FieldName = "LineId";
		this.tl_LineId.Name = "tl_LineId";
		this.tl_LineId.OptionsColumn.ReadOnly = true;
		this.tl_FatherCode.Caption = "FatherCode";
		this.tl_FatherCode.CurrencyDateField = null;
		this.tl_FatherCode.CurrencyField = null;
		this.tl_FatherCode.CurrencyFixed = null;
		this.tl_FatherCode.CurrencyFixedRate = 0.0;
		this.tl_FatherCode.CurrencyRateField = null;
		this.tl_FatherCode.FieldName = "FatherCode";
		this.tl_FatherCode.Name = "tl_FatherCode";
		this.tl_FatherCode.OptionsColumn.ReadOnly = true;
		this.tl_Assignee.Caption = "Assignee";
		this.tl_Assignee.ColumnEdit = this.luk_Assignee;
		this.tl_Assignee.CurrencyDateField = null;
		this.tl_Assignee.CurrencyField = null;
		this.tl_Assignee.CurrencyFixed = null;
		this.tl_Assignee.CurrencyFixedRate = 0.0;
		this.tl_Assignee.CurrencyRateField = null;
		this.tl_Assignee.FieldName = "Assignee";
		this.tl_Assignee.Name = "tl_Assignee";
		this.tl_Assignee.Visible = true;
		this.tl_Assignee.VisibleIndex = 6;
		this.luk_Assignee.AutoHeight = false;
		this.luk_Assignee.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Assignee.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Assignee.IsSearchControl = false;
		this.luk_Assignee.Name = "luk_Assignee";
		this.luk_Assignee.NullText = "";
		this.luk_Assignee.ShowFooter = false;
		this.luk_Assignee.ShowHeader = false;
		this.tl_Employee.Caption = "Employee";
		this.tl_Employee.ColumnEdit = this.bts_Employee;
		this.tl_Employee.CurrencyDateField = null;
		this.tl_Employee.CurrencyField = null;
		this.tl_Employee.CurrencyFixed = null;
		this.tl_Employee.CurrencyFixedRate = 0.0;
		this.tl_Employee.CurrencyRateField = null;
		this.tl_Employee.FieldName = "Employee";
		this.tl_Employee.Name = "tl_Employee";
		this.tl_Employee.Visible = true;
		this.tl_Employee.VisibleIndex = 7;
		this.bts_Employee.AutoHeight = false;
		this.bts_Employee.AutoLoadDataSource = true;
		editorButtonImageOptions14.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions28.Image");
		this.bts_Employee.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions14, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance27, appearanceHovered27, appearancePressed27, appearanceDisabled27, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, editorButtonImageOptions14, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance28, appearanceHovered28, appearancePressed28, appearanceDisabled28, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bts_Employee.ContainerControl = this;
		this.bts_Employee.DisplayMember = "";
		this.bts_Employee.DisplayMember2 = "";
		this.bts_Employee.LoadDataSourceWhenEnter = false;
		this.bts_Employee.Name = "bts_Employee";
		this.bts_Employee.SeparateCharacterForMulti = ';';
		this.bts_Employee.SeparateString = ", ";
		this.bts_Employee.UsingAutoCompleteBox = false;
		this.bts_Employee.UsingDataSource = true;
		this.bts_Employee.ValidateWhenNoDataSource = false;
		this.bts_Employee.ValueMember = "";
		this.tl_User.Caption = "User";
		this.tl_User.ColumnEdit = this.bts_User;
		this.tl_User.CurrencyDateField = null;
		this.tl_User.CurrencyField = null;
		this.tl_User.CurrencyFixed = null;
		this.tl_User.CurrencyFixedRate = 0.0;
		this.tl_User.CurrencyRateField = null;
		this.tl_User.FieldName = "User";
		this.tl_User.Name = "tl_User";
		this.tl_User.Visible = true;
		this.tl_User.VisibleIndex = 8;
		this.bts_User.AutoHeight = false;
		this.bts_User.AutoLoadDataSource = true;
		editorButtonImageOptions15.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions30.Image");
		this.bts_User.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions15, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance29, appearanceHovered29, appearancePressed29, appearanceDisabled29, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, editorButtonImageOptions15, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance30, appearanceHovered30, appearancePressed30, appearanceDisabled30, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bts_User.ContainerControl = this;
		this.bts_User.DisplayMember = "";
		this.bts_User.DisplayMember2 = "";
		this.bts_User.LoadDataSourceWhenEnter = false;
		this.bts_User.Name = "bts_User";
		this.bts_User.SeparateCharacterForMulti = ';';
		this.bts_User.SeparateString = ", ";
		this.bts_User.UsingAutoCompleteBox = false;
		this.bts_User.UsingDataSource = true;
		this.bts_User.ValidateWhenNoDataSource = false;
		this.bts_User.ValueMember = "";
		this.ctm_Task.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addRowToolStripMenuItem, this.addRowChildToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.ctm_Task.Name = "ctm_Task";
		this.ctm_Task.Size = new System.Drawing.Size(210, 70);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
		this.addRowToolStripMenuItem.Text = "Add Row (Enter)";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.addRowChildToolStripMenuItem.Name = "addRowChildToolStripMenuItem";
		this.addRowChildToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
		this.addRowChildToolStripMenuItem.Text = "Add Row Children(Insert)";
		this.addRowChildToolStripMenuItem.Click += new System.EventHandler(this.addRowChildToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row (Shift+Delete)";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 17);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 1;
		this.labelLine1.Utext = "Code";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "From Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(227, 17);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 2;
		this.labelLine2.Utext = "From Date";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "To Date";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(454, 17);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 3;
		this.labelLine3.Utext = "To Date";
		this.txt_Code.EditControl = null;
		this.txt_Code.FieldName = null;
		this.txt_Code.IsSearchControl = true;
		this.txt_Code.Location = new System.Drawing.Point(76, 14);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.SettingInVisible = false;
		this.txt_Code.SettingReadonly = false;
		this.txt_Code.Size = new System.Drawing.Size(100, 20);
		this.txt_Code.TabIndex = 4;
		this.date_FromDate.EditControl = null;
		this.date_FromDate.EditValue = null;
		this.date_FromDate.FieldName = null;
		this.date_FromDate.Location = new System.Drawing.Point(301, 14);
		this.date_FromDate.Name = "date_FromDate";
		this.date_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.IsSearchControl = true;
		this.date_FromDate.SettingInVisible = false;
		this.date_FromDate.SettingReadonly = false;
		this.date_FromDate.Size = new System.Drawing.Size(100, 20);
		this.date_FromDate.TabIndex = 5;
		this.date_ToDate.EditControl = null;
		this.date_ToDate.EditValue = null;
		this.date_ToDate.FieldName = null;
		this.date_ToDate.Location = new System.Drawing.Point(526, 14);
		this.date_ToDate.Name = "date_ToDate";
		this.date_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.IsSearchControl = true;
		this.date_ToDate.SettingInVisible = false;
		this.date_ToDate.SettingReadonly = false;
		this.date_ToDate.Size = new System.Drawing.Size(100, 20);
		this.date_ToDate.TabIndex = 6;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = true;
		this.btn_Search.Location = new System.Drawing.Point(718, 11);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 23);
		this.btn_Search.TabIndex = 7;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(12, 442);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 8;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(90, 442);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 9;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(983, 467);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.date_ToDate);
		base.Controls.Add(this.date_FromDate);
		base.Controls.Add(this.txt_Code);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oWBSTemplate";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_WBSTemplate";
		this.Text = "WBS Template";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_WBSTemplate_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctm_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTimeEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTimeEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTimeSpanEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Mode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Milestone).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Notes).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_OcrCode2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_OcrCode3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_OcrCode4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_OcrCode5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_OcrCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Predecessors).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Resource).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).EndInit();
		this.xtraTabControlEx1.ResumeLayout(false);
		this.xtraTabPageEx2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tv_Wbs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_WbsMode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lu_Milestone).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mem_Notes).EndInit();
		((System.ComponentModel.ISupportInitialize)this.b_Resource).EndInit();
		this.ctm_WBS.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.b_OcrCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.b_OcrCode3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.b_OcrCode4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.b_OcrCode2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.b_OcrCode5).EndInit();
		this.xtraTabPageEx1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tv_Task).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Assignee).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bts_Employee).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bts_User).EndInit();
		this.ctm_Task.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
