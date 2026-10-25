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
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Interfaces.FormDocument;
using Apzon.Forms.PM;
using Apzon.Forms.Purchase;
using Apzon.Forms.Sales;
using Apzon.Forms.SalesOppotunity;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Service;

public class frm_ServiceCalls : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private int currentEmp;

	private bool loadLine;

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gridColumnEx12;

	private GridColumnEx gridColumnEx13;

	private GridColumnEx gridColumnEx14;

	private GridColumnEx gridColumnEx15;

	private GridColumnEx gridColumnEx16;

	private GridColumnEx gridColumnEx17;

	private XtraTabControlEx xtraTabControlEx1;

	private XtraTabPageEx xtraTabPage1;

	private SimpleButton btn_Issue;

	private GridControlEx grd_Issue;

	private GridViewEx gv_Issue;

	private XtraTabPageEx xtraTabPage3;

	private XtraTabPageEx xtraTabPage4;

	private RepositoryItemButtonEditCFL rep_cfl_BPCode;

	private RepositoryItemButtonEditCFL rep_cfl_ItemCode;

	private RepositoryItemCheckedComboBoxEditDefineNew rep_ccb_Tags;

	private RepositoryItemLookUpEditDefineNew rep_luk_CallType;

	private RepositoryItemButtonEditCFL rep_cfl_HandleBy;

	private RepositoryItemLookUpEditSimple rep_luk_Orgin;

	private RepositoryItemLookUpEditSimple rep_luk_Priority;

	public SimpleButtonEx btCancel;

	public SimpleButtonEx btnOK;

	private RepositoryItemLookUpEditSimple rep_luk_Type;

	private RepositoryItemLookUpEditSimple rep_luk_Contact;

	private RepositoryItemMemoExEdit rep_memx_Resolution;

	private GridColumnEx gridColumnEx18;

	private GridColumnEx gridColumnEx19;

	private GridColumnEx gridColumnEx20;

	private GridColumnEx gridColumnEx21;

	private GridColumnEx gridColumnEx22;

	private GridColumnEx gridColumnEx23;

	private GridColumnEx gridColumnEx24;

	private SimpleButton btn_NewSolution;

	private GridControlEx grd_Solution;

	private GridViewEx gv_Solution;

	private GridColumnEx gridColumnEx32;

	private GridColumnEx gridColumnEx33;

	private GridColumnEx gridColumnEx34;

	private GridColumnEx gridColumnEx35;

	private GridColumnEx gridColumnEx36;

	private GridColumnEx gridColumnEx37;

	private XtraTabPage xtraTabPage2;

	private SimpleButton btn_NewTask;

	private GridControlEx grd_Task;

	private GridViewEx gv_Task;

	private GridColumnEx gridColumnEx25;

	private GridColumnEx gridColumnEx26;

	private GridColumnEx gridColumnEx29;

	private GridColumnEx gridColumnEx30;

	private RepositoryItemLookUpEditSimple rep_luk_IssueStatus;

	private RepositoryItemLookUpEditDefineNew rep_luk_IssueType;

	private RepositoryItemButtonEditCFL rep_cfl_Project;

	private RepositoryItemLookUpEditDefineNew rep_luk_SolutionStatus;

	private RepositoryItemButtonEditCFL rep_cfl_Owner;

	private RepositoryItemButtonEditCFL rep_cfl_SltCode;

	private ContextMenuStrip ctx_Solution;

	private ToolStripMenuItem removeToolStripMenuItem;

	private RepositoryItemButtonEditCFL rep_cfl_IssueHandleBy;

	private RepositoryItemButtonEditCFL rep_cfl_IssueId;

	private RepositoryItemButtonEditCFL rep_cfl_TaskDetails;

	private GridColumnEx gridColumnEx27;

	private RepositoryItemHourEdit rep_hour_ENDTime;

	private ContextMenuStrip ctx_ServiceCalls;

	private ToolStripMenuItem addToolStripMenuItem;

	private ToolStripMenuItem removeToolStripMenuItem1;

	private GridControlEx grd_DocLine;

	private GridViewEx gv_DocLine;

	private RepositoryItemMemoExEdit rep_memx_IssueNote;

	private RepositoryItemMemoExEdit rep_memx_SltSubject;

	private GridColumnEx gridColumnEx38;

	private GridColumnEx gridColumnEx40;

	private RepositoryItemButtonEditCFL rep_cfl_DocNum;

	private GridColumnEx gridColumnEx39;

	private GridColumnEx gridColumnEx28;

	private GridColumnEx gridColumnEx31;

	private GridColumnEx gridColumnEx41;

	private GridColumnEx gridColumnEx42;

	private GridColumnEx gridColumnEx43;

	private SimpleButton btn_Return;

	private RepositoryItemLookUpEditSimple rep_luk_ObjType;

	private RepositoryItemLookUpEditSimple rep_luk_DocLineStatus;

	private ContextMenuStrip ctx_DocLine;

	private ToolStripMenuItem removeToolStripMenuItem2;

	private SimpleButton btn_Delivery;

	private SimpleButton btn_ARInvoice;

	private SimpleButton btn_GoodReturn;

	private SimpleButton btn_GoodReceiptPO;

	private SimpleButton btn_APInvoice;

	private RepositoryItemButtonEditCFL rep_cfl_BPCode_Vendor;

	private RepositoryItemButtonEditCFL rep_cfl_ItemCode_CardCode;

	private RepositoryItemButtonEditCFL rep_cfl_SerialNum;

	private SplitContainerControl splitContainerControl1;

	private ButtonEditCFL cfl_BPCode;

	private DateEditEx dte_ToDate;

	private DateEditEx dte_FromDate;

	private LabelLine labelLine4;

	private LabelLine labelLine6;

	private LabelLine labelLine5;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private LabelLine labelLine1;

	private LookUpEditSimple luk_CallType;

	private TextEditEx txt_CallId;

	private LabelLine labelLine7;

	private ButtonEditCFL cfl_HandleBy;

	private GridColumnEx gridColumnEx44;

	private CheckedComboBoxEditDefineNew ccb_Tags;

	private SimpleButtonEx btn_Find;

	public int CallID => Function.ParseInt(gv_Doc.GetFocusedRowCellValue("CallID"));

	public string CardCode => Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				btnOK.Text = Resources.ButtonAdd;
				btn_NewTask.Enabled = false;
				btn_Issue.Enabled = false;
				btn_NewSolution.Enabled = false;
				btn_Return.Enabled = false;
				btn_Delivery.Enabled = false;
				btn_ARInvoice.Enabled = false;
				btn_GoodReturn.Enabled = false;
				btn_GoodReceiptPO.Enabled = false;
				btn_APInvoice.Enabled = false;
				break;
			case Enums.FormViewModeType.Update:
				btnOK.Text = Resources.ButtonUpdate;
				break;
			case Enums.FormViewModeType.View:
				btnOK.Text = Resources.ButtonOK;
				btn_NewTask.Enabled = true;
				btn_Issue.Enabled = true;
				btn_NewSolution.Enabled = true;
				btn_Return.Enabled = true;
				btn_Delivery.Enabled = true;
				btn_ARInvoice.Enabled = true;
				btn_GoodReturn.Enabled = true;
				btn_GoodReceiptPO.Enabled = true;
				btn_APInvoice.Enabled = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_ServiceCalls()
	{
		InitializeComponent();
		FormObjectType = ApzObjectTypes.oServiceCalls;
		gv_Doc.ItemId = "APZ_OSCL";
		gv_Issue.ItemId = "APZ_TGIE";
		gv_Task.ItemId = "APZ_OCLG";
		gv_Solution.ItemId = "SCL1";
		gv_DocLine.ItemId = "APZ_SCL3";
		FormId = "oServiceCallsManagement";
		gv_Doc.FormId = FormId;
		gv_Issue.FormId = FormId;
		gv_Task.FormId = FormId;
		gv_Solution.FormId = FormId;
		gv_DocLine.FormId = FormId;
		OnLoadAsync += frm_ServiceCalls_Load;
		rep_luk_Contact.PopupFilter += Rep_luk_Contact_PopupFilter;
		SetupControls();
	}

	private void SetupControls()
	{
		try
		{
			rep_ccb_Tags.ValueMember = "ID";
			rep_ccb_Tags.DisplayMember = "Name";
			rep_ccb_Tags.SeparatorChar = ';';
			rep_ccb_Tags.FormProperties.DictionRepository.Add("Inactive", new RepositoryItemCheckEdit
			{
				ValueChecked = "Y",
				ValueUnchecked = "N",
				ValueGrayed = "N"
			});
			rep_ccb_Tags.FormProperties.Text = "Tags - Setup";
			rep_ccb_Tags.FormProperties.CheckTables = "APZ_OSCL";
			rep_ccb_Tags.FormProperties.CheckFields = "Tags";
			rep_ccb_Tags.FormProperties.TableName = "APZ_SCL2";
			RepositoryItemCheckedComboBoxEditDefineNew repositoryItemCheckedComboBoxEditDefineNew = rep_ccb_Tags;
			repositoryItemCheckedComboBoxEditDefineNew.DataSourceRefreshed = (Action<DataTable>)Delegate.Combine(repositoryItemCheckedComboBoxEditDefineNew.DataSourceRefreshed, (Action<DataTable>)((DataTable dt) =>
			{
				ccb_Tags.Properties.DataSource = dt;
			}));
			rep_ccb_Tags.Popup += Rep_ccb_Tags_Popup;
			ccb_Tags.Properties.ValueMember = "ID";
			ccb_Tags.Properties.DisplayMember = "Name";
			ccb_Tags.Properties.SeparatorChar = ';';
			ccb_Tags.Properties.FormProperties.DictionRepository.Add("Inactive", new RepositoryItemCheckEdit
			{
				ValueChecked = "Y",
				ValueUnchecked = "N",
				ValueGrayed = "N"
			});
			ccb_Tags.Properties.FormProperties.Text = "Tags - Setup";
			ccb_Tags.Properties.FormProperties.CheckTables = "APZ_OSCL";
			ccb_Tags.Properties.FormProperties.CheckFields = "Tags";
			ccb_Tags.Properties.FormProperties.TableName = "APZ_SCL2";
			rep_luk_CallType.ValueMember = "ID";
			rep_luk_CallType.DisplayMember = "Name";
			rep_luk_CallType.FormProperties.Text = "Call Type - Setup";
			rep_luk_CallType.FormProperties.CheckTables = "APZ_OSCL";
			rep_luk_CallType.FormProperties.CheckFields = "CallType";
			rep_luk_CallType.FormProperties.TableName = "APZ_SCL1";
			RepositoryItemLookUpEditDefineNew repositoryItemLookUpEditDefineNew = rep_luk_CallType;
			repositoryItemLookUpEditDefineNew.DataSourceRefreshed = (Action<DataTable>)Delegate.Combine(repositoryItemLookUpEditDefineNew.DataSourceRefreshed, (Action<DataTable>)((DataTable dt) =>
			{
				DataTable dataTable = dt.Copy();
				dataTable.Rows.InsertAt(dataTable.NewRow(), 0);
				luk_CallType.Properties.DataSource = dataTable;
			}));
			luk_CallType.Properties.ValueMember = "ID";
			luk_CallType.Properties.DisplayMember = "Name";
			luk_CallType.Properties.Columns.Add(new LookUpColumnInfo("Name"));
			rep_cfl_BPCode.ValueMember = "CardCode";
			rep_cfl_BPCode.TypeCFL = Enums.TypeCFL.Customer;
			rep_cfl_BPCode.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rep_cfl_BPCode;
			repositoryItemButtonEditCFL.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL.HandleChooseItem, new Action<DataRow>(BPCode_HandleChooseItem));
			rep_cfl_BPCode_Vendor.ValueMember = "CardCode";
			rep_cfl_BPCode_Vendor.TypeCFL = Enums.TypeCFL.Vendor;
			rep_cfl_BPCode_Vendor.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = rep_cfl_BPCode_Vendor;
			repositoryItemButtonEditCFL2.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL2.HandleChooseItem, new Action<DataRow>(BPCode_HandleChooseItem));
			cfl_BPCode.Properties.ValueMember = "CardCode";
			cfl_BPCode.Properties.TypeCFL = Enums.TypeCFL.Bp;
			cfl_BPCode.Properties.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			cfl_BPCode.Properties.LoadDataSourceWhenEnter = true;
			rep_cfl_ItemCode.ValueMember = "ItemCode";
			rep_cfl_ItemCode.TypeCFL = Enums.TypeCFL.ListItems;
			rep_cfl_ItemCode.TypeDrillDown = ApzObjectTypes.oItems;
			rep_cfl_ItemCode.FieldNameInDataRow("ItemCode", "ItemName");
			rep_cfl_ItemCode.ListMandatoryField = "ItemName";
			gv_Issue.Columns["Reporter"].ColumnEdit = rep_cfl_HandleBy;
			gv_Issue.Columns["HandleBy"].ColumnEdit = rep_cfl_HandleBy;
			rep_cfl_HandleBy.ValueMember = "EmpId";
			rep_cfl_HandleBy.DisplayMember = "firstName";
			rep_cfl_HandleBy.DisplayMember2 = "lastName";
			rep_cfl_HandleBy.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			rep_cfl_HandleBy.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL3 = rep_cfl_HandleBy;
			repositoryItemButtonEditCFL3.DataSourceLoaded = (Action<DataTable>)Delegate.Combine(repositoryItemButtonEditCFL3.DataSourceLoaded, (Action<DataTable>)((DataTable dt) =>
			{
				rep_cfl_IssueHandleBy.DataSource = dt.Copy();
			}));
			rep_cfl_IssueHandleBy.ValueMember = "EmpId";
			rep_cfl_IssueHandleBy.DisplayMember = "firstName";
			rep_cfl_IssueHandleBy.DisplayMember2 = "lastName";
			rep_cfl_IssueHandleBy.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			rep_cfl_IssueHandleBy.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			cfl_HandleBy.Properties.Assign(rep_cfl_HandleBy);
			rep_luk_IssueType.ValueMember = "Code";
			rep_luk_IssueType.DisplayMember = "Name";
			rep_luk_IssueType.FormProperties.TableName = "APZ_IETE";
			rep_luk_SolutionStatus.ValueMember = "Number";
			rep_luk_SolutionStatus.DisplayMember = "Name";
			rep_luk_SolutionStatus.FormProperties.TableName = "OSST";
			rep_luk_Contact.ValueMember = "CntctCode";
			rep_luk_Contact.DisplayMember = "Name";
			rep_luk_Contact.DropDownRows = 4;
			rep_cfl_Owner.ValueMember = "USERID";
			rep_cfl_Owner.DisplayMember = "U_NAME";
			rep_cfl_Owner.DisplayMember2 = "";
			rep_cfl_Owner.TypeCFL = Enums.TypeCFL.GetUserList;
			rep_cfl_Owner.Buttons[1].Visible = false;
			rep_cfl_SltCode.ValueMember = "SltCode";
			rep_cfl_SltCode.TypeCFL = Enums.TypeCFL.KnowledgeBaseSolutions;
			rep_cfl_SltCode.TypeDrillDown = ApzObjectTypes.oKnowledgeBaseSolutions;
			rep_cfl_ItemCode_CardCode.ValueMember = "ItemCode";
			rep_cfl_ItemCode_CardCode.TypeCFL = Enums.TypeCFL.ItemForServiceCall;
			rep_cfl_ItemCode_CardCode.TypeDrillDown = ApzObjectTypes.oItems;
			rep_cfl_ItemCode_CardCode.FieldNameInDataRow("ItemCode", "Dscription");
			rep_cfl_ItemCode_CardCode.FieldNameInGrid("ItemCode", "ItemName");
			rep_cfl_ItemCode_CardCode.ValidateWhenNoDataSource = true;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL4 = rep_cfl_ItemCode_CardCode;
			repositoryItemButtonEditCFL4.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL4.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm.BpCode = CardCode;
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL5 = rep_cfl_ItemCode_CardCode;
			repositoryItemButtonEditCFL5.CustomDataSourceForValidating = (Func<DataTable>)Delegate.Combine(repositoryItemButtonEditCFL5.CustomDataSourceForValidating, (Func<DataTable>)(() =>
			{
				DataSet listItemForServiceCallNoAsync = _httpClient.Cfl.GetListItemForServiceCallNoAsync(CardCode);
				return (listItemForServiceCallNoAsync.Tables.Count > 1) ? listItemForServiceCallNoAsync.Tables[1] : new DataTable();
			}));
			rep_luk_Orgin.SetDataSource(new Dictionary<string, string>
			{
				{ "E", "Email" },
				{ "W", "Web" },
				{ "T", "Tel" },
				{ "S", "Social" }
			}, hideCodeColumn: true);
			rep_luk_Type.SetDataSource(new Dictionary<string, string>
			{
				{ "S", "Sales" },
				{ "P", "Purchase" }
			}, hideCodeColumn: true);
			rep_luk_Priority.SetDataSource(new Dictionary<string, string>
			{
				{ "3", "Critical" },
				{ "2", "High" },
				{ "1", "Normal" },
				{ "0", "Low" }
			}, hideCodeColumn: true);
			rep_luk_IssueStatus.SetDataSource(new Dictionary<string, string>
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
			rep_cfl_IssueId.ValueMember = "Id";
			rep_cfl_IssueId.TypeCFL = Enums.TypeCFL.TrackingIssue;
			rep_cfl_IssueId.TypeDrillDown = ApzObjectTypes.oTrackingIssue;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL6 = rep_cfl_TaskDetails;
			repositoryItemButtonEditCFL6.DrillDownMethod = (Action<string>)Delegate.Combine(repositoryItemButtonEditCFL6.DrillDownMethod, (Action<string>)((string value) =>
			{
				string num = Function.ToString(gv_Task.GetFocusedRowCellValue("ClgCode"));
				this.DrillDown(ApzObjectTypes.oTaskManagement, num);
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL7 = rep_cfl_DocNum;
			repositoryItemButtonEditCFL7.DrillDownMethod = (Action<string>)Delegate.Combine(repositoryItemButtonEditCFL7.DrillDownMethod, (Action<string>)((string s) =>
			{
				Enum.TryParse<ApzObjectTypes>(Function.ToString(gv_DocLine.GetFocusedRowCellValue("ObjType")), out var result);
				this.DrillDown(result, s);
			}));
			rep_luk_ObjType.SetDataSourceObject();
			rep_luk_DocLineStatus.SetDataSource(new Dictionary<string, string>
			{
				{ "O", "Open" },
				{ "C", "Closed" }
			}, hideCodeColumn: true);
			rep_cfl_SerialNum.TypeCFL = Enums.TypeCFL.SerialNum;
			rep_cfl_SerialNum.ValueMember = "DistNumber";
			rep_cfl_SerialNum.TypeDrillDown = ApzObjectTypes.oSerialNumberDetails;
			rep_cfl_SerialNum.ValidateWhenNoDataSource = true;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL8 = rep_cfl_SerialNum;
			repositoryItemButtonEditCFL8.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL8.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm.DataString2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("ItemCode"));
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL9 = rep_cfl_SerialNum;
			repositoryItemButtonEditCFL9.CustomDataSourceForValidating = (Func<DataTable>)Delegate.Combine(repositoryItemButtonEditCFL9.CustomDataSourceForValidating, (Func<DataTable>)(() =>
			{
				DataSet listSerialNumNoAsync = _httpClient.Cfl.GetListSerialNumNoAsync(Function.ToString(gv_Doc.GetFocusedRowCellValue("ItemCode")));
				return (listSerialNumNoAsync.Tables.Count > 1) ? listSerialNumNoAsync.Tables[1] : new DataTable();
			}));
			dte_FromDate.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			dte_ToDate.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Rep_ccb_Tags_Popup(object sender, EventArgs e)
	{
		try
		{
			PopupContainerControl popupContainerControl = (sender as IPopupControl).PopupWindow.Controls.OfType<PopupContainerControl>().FirstOrDefault();
			if (popupContainerControl == null)
			{
				return;
			}
			CheckedListBoxControl checkedListBoxControl = popupContainerControl.Controls[0] as CheckedListBoxControl;
			checkedListBoxControl.BeginUpdate();
			foreach (CheckedListBoxItem item in checkedListBoxControl.Items.Cast<CheckedListBoxItem>())
			{
				int num = Function.ParseInt(item.Value);
				foreach (DataRow item2 in (rep_ccb_Tags.DataSource as DataTable).AsEnumerable())
				{
					if (Function.ParseInt(item2["ID"]) == num && Function.ToString(item2["Inactive"]) == "Y")
					{
						item.Enabled = false;
						break;
					}
				}
			}
			checkedListBoxControl.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_ServiceCalls_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			FormMode = Enums.FormViewModeType.View;
			await LoadData();
			Task task = rep_luk_SolutionStatus.RefreshDataSourceAsync();
			Task currentEmployeeId = GetCurrentEmployeeId();
			Task task2 = LoadRepContactData();
			Task task3 = rep_cfl_Owner.RefreshDataSourceAsync();
			Task task4 = rep_cfl_ItemCode_CardCode.RefreshDataSourceAsync();
			Task task5 = rep_ccb_Tags.RefreshDataAsync();
			Task task6 = rep_luk_CallType.RefreshDataSourceAsync();
			Task task7 = rep_cfl_BPCode.RefreshDataSourceAsync();
			Task task8 = rep_cfl_ItemCode.RefreshDataSourceAsync();
			Task task9 = rep_cfl_HandleBy.RefreshDataSourceAsync();
			Task task10 = rep_luk_IssueType.RefreshDataSourceAsync();
			Task task11 = LoadIssue();
			Task task12 = LoadTask();
			Task task13 = LoadSolution();
			Task task14 = LoadDocumentLine();
			await Task.WhenAll(task5, task6, task7, task8, task9, task10, task11, task12, task, currentEmployeeId, task2, task3, task13, task14, task4);
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_Doc, "APZ_OSCL"));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH()
	{
		AddNewRowDoc();
	}

	private void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		AddNewRowDoc();
	}

	public void AddNewRowDoc()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.No)
			{
				FormMode = Enums.FormViewModeType.Add;
				DataTable dataTable = grd_Doc.DataSourceTable.Clone();
				DataRow dataRow = dataTable.NewRow();
				dataRow["CallID"] = 0;
				dataRow["Type"] = "S";
				dataRow["Priority"] = "1";
				if (currentEmp != 0)
				{
					dataRow["HandleBy"] = currentEmp;
				}
				dataRow["CreateDate"] = DateTime.Today;
				dataTable.Rows.Add(dataRow);
				grd_Doc.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
			{
				await frm_ServiceCalls_Load(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadIssue()
	{
		_ = 1;
		try
		{
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("CallID"));
			if (!string.IsNullOrEmpty(text) && text != "0")
			{
				DataTable dataSource = await _httpClient.TrackingIssue.LoadIssueByCallId(CallID);
				grd_Issue.DataSource = dataSource;
			}
			else
			{
				GridControlEx gridControlEx = grd_Issue;
				gridControlEx.DataSource = await WinFuntions.GetTableStructureAsync("APZ_TGIE");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadTask()
	{
		_ = 1;
		try
		{
			DataTable dataTable = await HttpClient.SubActivities.LoadTaskByObjectType(GlobalData.UserCodeLogin, ApzObjectTypes.oServiceCalls, Function.ToString(CallID));
			if (dataTable.IsNotNull())
			{
				grd_Task.DataSource = dataTable;
				return;
			}
			GridControlEx gridControlEx = grd_Task;
			gridControlEx.DataSource = await WinFuntions.GetTableStructureAsync("APZ_OCLG");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadSolution()
	{
		_ = 1;
		try
		{
			DataTable dataTable = await HttpClient.ServiceCalls.GetSolutionByCallId(CallID);
			if (dataTable.IsNotNull())
			{
				grd_Solution.DataSource = dataTable;
				return;
			}
			DataTable dataTable2 = await WinFuntions.GetTableStructureAsync("SCL1");
			dataTable2.Columns.Add("Subject", typeof(string));
			dataTable2.Columns.Add("StatusNum", typeof(int));
			dataTable2.AcceptChanges();
			grd_Solution.DataSource = dataTable2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadDocumentLine()
	{
		_ = 1;
		try
		{
			DataTable dataTable = await HttpClient.ServiceCalls.GetDocumentByCallId(CallID);
			if (dataTable.IsNotNull())
			{
				grd_DocLine.DataSource = dataTable;
				return;
			}
			DataTable dataSource = await WinFuntions.GetTableStructureAsync("APZ_SCL3");
			grd_DocLine.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void removeToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode != Enums.FormViewModeType.Add && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
			{
				string text = await _httpClient.ServiceCalls.Remove(CallID);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.RemoveSuccess, "");
					FormMode = Enums.FormViewModeType.View;
					await LoadData();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void BPCode_HandleChooseItem(DataRow dr)
	{
		try
		{
			string value = Function.ToString(dr["CardCode"]);
			string value2 = Function.ToString(dr["CardName"]);
			gv_Doc.SetFocusedRowCellValue("CardCode", value);
			gv_Doc.SetFocusedRowCellValue("CardName", value2);
			if (!string.IsNullOrEmpty(Function.ToString(dr["CardCode"])))
			{
				DataTable dataTable = await _httpClient.BusinessMaster.GetADoc(Function.ToString(dr["CardCode"]));
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					gv_Doc.SetFocusedRowCellValue("Tel", Function.ToString(dataTable.Rows[0]["Phone1"]));
					gv_Doc.SetFocusedRowCellValue("Email", Function.ToString(dataTable.Rows[0]["MailAddress"]));
				}
			}
			gv_Doc.SetFocusedRowCellValue("ContactPerson", null);
			DataTable contactPerson = HttpClient.ServiceCalls.GetContactPerson(CardCode);
			if (contactPerson != null && contactPerson.Rows.Count > 0)
			{
				DataRow dataRow = contactPerson.Rows[0];
				gv_Doc.SetFocusedRowCellValue("ContactPerson", dataRow["CntctCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		try
		{
			string cardCode = Function.ToString(cfl_BPCode.EditValue);
			int handleBy = Function.ParseInt(cfl_HandleBy.EditValue);
			int callType = Function.ParseInt(luk_CallType.EditValue);
			int callId = Function.ParseInt(txt_CallId.EditValue);
			string tag = Function.ToString(ccb_Tags.EditValue);
			DateTime dateTime = dte_FromDate.DateTime;
			DateTime dateTime2 = dte_ToDate.DateTime;
			DataTable dataTable = await _httpClient.ServiceCalls.LoadServiceCalls(cardCode, handleBy, callType, callId, tag, dateTime, dateTime2);
			if (dataTable.EmptyData())
			{
				dataTable = WinFuntions.GetTableStructure("APZ_OSCL");
			}
			dataTable.AcceptChanges();
			grd_Doc.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			await LoadLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadLine()
	{
		try
		{
			loadLine = true;
			grd_Issue.DataSourceTable().Rows.Clear();
			grd_Task.DataSourceTable().Rows.Clear();
			grd_Solution.DataSourceTable().Rows.Clear();
			grd_DocLine.DataSourceTable().Rows.Clear();
			Task task = LoadIssue();
			Task task2 = LoadTask();
			Task task3 = LoadSolution();
			Task task4 = LoadDocumentLine();
			await Task.WhenAll(task, task2, task3, task4);
			_ = Function.ToString(gv_Doc.GetFocusedRowCellValue("Type")) == "S";
			loadLine = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadRepContactData()
	{
		try
		{
			DataTable dataTable = await HttpClient.ServiceCalls.GetContactPersonAsync("");
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["CardCode"] = "";
				dataTable.Rows.InsertAt(dataRow, 0);
				dataTable.AcceptChanges();
				rep_luk_Contact.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetCurrentEmployeeId()
	{
		try
		{
			DataTable dataTable = await HttpClient.TrackingIssue.GetEmployeeByUserId(GlobalData.UserID);
			if (dataTable.IsNotNull())
			{
				currentEmp = Function.ParseInt(dataTable.Rows[0]["empId"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Rep_luk_Contact_PopupFilter(object sender, PopupFilterEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
			e.Criteria = new InOperator("CardCode", new string[2] { text, "" });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btnOK_Click(object sender, EventArgs e)
	{
		btnOK.Enabled = false;
		try
		{
			if (FormMode != Enums.FormViewModeType.View)
			{
				await SaveDoc();
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btnOK.Enabled = true;
	}

	private async Task SaveDoc()
	{
		_ = 1;
		try
		{
			DataRow focusedDataRow = gv_Doc.GetFocusedDataRow();
			DataTable dataTable = grd_Doc.DataSourceTable.Clone();
			dataTable.ImportRow(focusedDataRow);
			dataTable.AcceptChanges();
			DataTable table = grd_Solution.DataSourceTable.Copy();
			DataTable table2 = grd_DocLine.DataSourceTable.Copy();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable);
			dataSet.Tables.Add(table);
			dataSet.Tables.Add(table2);
			string text = await HttpClient.ServiceCalls.SaveDoc(dataSet);
			if (Function.ToString(text) == "")
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				FormMode = Enums.FormViewModeType.View;
				await LoadData();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void SetUpdateText()
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Type")
			{
				gv_Doc.SetRowCellValue(e.RowHandle, "CardCode", null);
			}
			if (e.Column.FieldName == "CardCode")
			{
				gv_Doc.SetRowCellValue(e.RowHandle, "ItemCode", null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		SetUpdateText();
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (loadLine)
			{
				e.Allow = false;
			}
			else if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					grd_Doc.DataSourceTable.RejectChanges();
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					e.Allow = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_NewTask_Click(object sender, EventArgs e)
	{
		try
		{
			if (CallID != 0)
			{
				int num = 191;
				string value = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
				frm_TaskManagement frm_TaskManagement2 = new frm_TaskManagement
				{
					ClgCode = 0,
					StartPosition = FormStartPosition.CenterScreen,
					MdiParent = MdiParent,
					CallFromCal = true
				};
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
				DataRow dataRow = tableStructure.NewRow();
				dataRow["GroupUser"] = GlobalData.UserID;
				if (Function.ToString(dataRow["Recontact"]) == "")
				{
					dataRow["Recontact"] = DateTime.Today;
					string text = DateTime.Now.Minute.ToString();
					dataRow["BeginTime"] = DateTime.Now.Hour + ((text.Length == 2) ? text : ("0" + text));
				}
				dataRow["Closed"] = "N";
				dataRow["DocType"] = num;
				dataRow["DocEntry"] = CallID;
				dataRow["DocNum"] = CallID;
				dataRow["CardCode"] = value;
				tableStructure.Rows.Add(dataRow);
				tableStructure.AcceptChanges();
				frm_TaskManagement2.DataRowFromCal = dataRow;
				frm_TaskManagement2.ReloadRelated = (Func<DataRow, Task>)Delegate.Combine(frm_TaskManagement2.ReloadRelated, new Func<DataRow, Task>(ReloadTaskLine));
				frm_TaskManagement2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task ReloadTaskLine(DataRow dr)
	{
		try
		{
			Function.ParseInt(dr["ClgCode"]);
			if (grd_Task.DataSource is DataTable dataTable)
			{
				dataTable.ImportRow(dr);
				dataTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Task_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		try
		{
			if (!(sender is GridView gridView) || e.Column.FieldName != "AssignTo" || !e.IsGetData)
			{
				return;
			}
			int listSourceRowIndex = e.ListSourceRowIndex;
			string text = Function.ToString(gridView.GetListSourceRowCellValue(listSourceRowIndex, "GroupUser"));
			string text2 = Function.ToString(gridView.GetListSourceRowCellValue(listSourceRowIndex, "GroupEmployee"));
			string text3 = "";
			if (!string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2))
			{
				foreach (string item in text.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList())
				{
					if (rep_cfl_Owner.dictionSource.ContainsKey(item))
					{
						text3 = text3 + rep_cfl_Owner.dictionSource[item] + "; ";
					}
				}
			}
			if (!string.IsNullOrEmpty(text2))
			{
				foreach (string item2 in text2.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList())
				{
					if (rep_cfl_HandleBy.dictionSource.ContainsKey(item2))
					{
						text3 = text3 + rep_cfl_HandleBy.dictionSource[item2] + "; ";
					}
				}
			}
			if (text3.Length > 2)
			{
				text3 = text3.Remove(text3.Length - 2);
			}
			e.Value = text3;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Issue_Click(object sender, EventArgs e)
	{
		try
		{
			if (CallID != 0)
			{
				string value = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
				int userID = GlobalData.UserID;
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_TGIE");
				tableStructure.Columns.Add("SolutionDesc");
				tableStructure.Columns.Add("TaskName");
				DataRow dataRow = tableStructure.NewRow();
				dataRow["Priority"] = "2";
				dataRow["Status"] = "1";
				dataRow["IssueDate"] = DateTime.Today;
				dataRow["Reporter"] = userID;
				dataRow["ServiceCall"] = CallID;
				dataRow["BpCode"] = value;
				tableStructure.Rows.Add(dataRow);
				tableStructure.AcceptChanges();
				frm_TrackingIssue frm_TrackingIssue2 = new frm_TrackingIssue
				{
					MdiParent = MdiParent,
					CallFromCal = true,
					DataRowFromCal = dataRow,
					StartPosition = FormStartPosition.CenterScreen
				};
				frm_TrackingIssue2.SendDataBackToMain = (Func<int, Task>)Delegate.Combine(frm_TrackingIssue2.SendDataBackToMain, new Func<int, Task>(ReloadTrackingIssue));
				frm_TrackingIssue2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Frm_FormClosed1(object sender, FormClosedEventArgs e)
	{
		try
		{
			frm_TrackingIssue frm_TrackingIssue2 = sender as frm_TrackingIssue;
			if (frm_TrackingIssue2.OkToClose)
			{
				DataRow focusedDataRow = frm_TrackingIssue2.gv_TrackingIssue.GetFocusedDataRow();
				AddIssueLine(focusedDataRow);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddIssueLine(DataRow sendRow)
	{
		try
		{
			if (sendRow == null)
			{
				return;
			}
			foreach (DataRow item in grd_Issue.DataSourceTable.AsEnumerable())
			{
				int num = Function.ParseInt(item["Id"]);
				if (Function.ParseInt(sendRow["Id"]) == num)
				{
					return;
				}
			}
			grd_Issue.DataSourceTable.ImportRow(sendRow);
			grd_Issue.DataSourceTable.AcceptChanges();
			SetUpdateText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ReloadTrackingIssue(int id)
	{
		try
		{
			DataTable dataSource = await _httpClient.TrackingIssue.LoadIssueByCallId(CallID);
			grd_Issue.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gv_Solution.BeginUpdate();
			gv_Solution.DeleteSelectedRows();
			gv_Solution.EndUpdate();
			SetUpdateText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_NewSolution_Click(object sender, EventArgs e)
	{
		try
		{
			if (CallID != 0)
			{
				frm_Solution frm_Solution2 = new frm_Solution();
				frm_Solution2.newLine = true;
				frm_Solution2.MdiParent = MdiParent;
				frm_Solution2.ItemCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("ItemCode"));
				frm_Solution2.FormClosed += Frm_FormClosed;
				frm_Solution2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Frm_FormClosed(object sender, FormClosedEventArgs e)
	{
		try
		{
			frm_Solution frm_Solution2 = sender as frm_Solution;
			if (frm_Solution2.OkToClose)
			{
				DataRow focusedDataRow = frm_Solution2.gvDoc.GetFocusedDataRow();
				AddSolutionLine(focusedDataRow);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddSolutionLine(DataRow sendRow)
	{
		try
		{
			if (sendRow == null)
			{
				return;
			}
			foreach (DataRow item in grd_Solution.DataSourceTable.AsEnumerable())
			{
				int num = Function.ParseInt(item["solutionID"]);
				if (Function.ParseInt(sendRow["SltCode"]) == num)
				{
					return;
				}
			}
			DataRow dataRow = grd_Solution.DataSourceTable.NewRow();
			dataRow["srvcCallID"] = CallID;
			dataRow["line"] = grd_Solution.DataSourceTable.Rows.Count;
			dataRow["solutionID"] = sendRow["SltCode"];
			dataRow["userSign"] = sendRow["CreatedBy"];
			dataRow["userSign2"] = sendRow["UpdateBy"];
			dataRow["createDate"] = sendRow["DateCreate"];
			dataRow["updateDate"] = sendRow["DateUpdate"];
			dataRow["Subject"] = sendRow["Subject"];
			dataRow["StatusNum"] = sendRow["StatusNum"];
			grd_Solution.DataSourceTable.Rows.Add(dataRow);
			grd_Solution.DataSourceTable.AcceptChanges();
			SetUpdateText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Return_Click(object sender, EventArgs e)
	{
		SelectDoc(new frm_Return());
	}

	private void btn_Delivery_Click(object sender, EventArgs e)
	{
		SelectDoc(new frm_Delivery());
	}

	private void btn_ARInvoice_Click(object sender, EventArgs e)
	{
		SelectDoc(new frm_ARInvoices());
	}

	private void btn_GoodReturn_Click(object sender, EventArgs e)
	{
		SelectDoc(new frm_GoodsReturn());
	}

	private void SelectDoc(VirFormDocument form)
	{
		try
		{
			if (CallID != 0)
			{
				form.MdiParent = MdiParent;
				form.OpenFormViewMode = Enums.FormViewModeType.Add;
				form.Formcopy = this;
				form.FormClosed += Frm_FormClosed2;
				form.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Frm_FormClosed2(object sender, FormClosedEventArgs e)
	{
		try
		{
			VirFormDocument virFormDocument = sender as VirFormDocument;
			if (virFormDocument.OkToClose)
			{
				DataRow focusedDataRow = virFormDocument.gvDoc.GetFocusedDataRow();
				AddDocLine(focusedDataRow, virFormDocument.FormObjectType);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddDocLine(DataRow sendRow, ApzObjectTypes formType)
	{
		try
		{
			if (sendRow == null)
			{
				return;
			}
			foreach (DataRow item in grd_DocLine.DataSourceTable.AsEnumerable())
			{
				int num = Function.ParseInt(item["DocEntry"]);
				string text = Function.ToString(item["ObjType"]);
				if (Function.ParseInt(sendRow["DocEntry"]) == num && Function.ToString(sendRow["ObjType"]) == text)
				{
					return;
				}
			}
			DataRow dataRow = grd_DocLine.DataSourceTable.NewRow();
			dataRow["srvcCallID"] = CallID;
			dataRow["line"] = grd_DocLine.DataSourceTable.Rows.Count;
			dataRow["ObjType"] = Function.ToString((int)formType);
			dataRow["DocEntry"] = sendRow["DocEntry"];
			dataRow["DocNum"] = sendRow["DocNum"];
			dataRow["DocDate"] = sendRow["DocDate"];
			dataRow["DocDueDate"] = sendRow["DocDueDate"];
			dataRow["TaxDate"] = sendRow["TaxDate"];
			dataRow["DocTotal"] = sendRow["DocTotal"];
			dataRow["DocStatus"] = sendRow["DocStatus"];
			dataRow["Comments"] = sendRow["Comments"];
			grd_DocLine.DataSourceTable.Rows.Add(dataRow);
			grd_DocLine.DataSourceTable.AcceptChanges();
			SetUpdateText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void removeToolStripMenuItem2_Click(object sender, EventArgs e)
	{
		try
		{
			gv_DocLine.BeginUpdate();
			gv_DocLine.DeleteSelectedRows();
			gv_DocLine.EndUpdate();
			SetUpdateText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_GoodReceipt_Click(object sender, EventArgs e)
	{
		SelectDoc(new frm_GoodReceiptPO());
	}

	private void btn_APInvoice_Click(object sender, EventArgs e)
	{
		SelectDoc(new frm_APInvoices());
	}

	private void gv_Doc_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "CardCode")
			{
				if (Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "Type")) == "S")
				{
					e.RepositoryItem = rep_cfl_BPCode;
				}
				else
				{
					e.RepositoryItem = rep_cfl_BPCode_Vendor;
				}
			}
			else if (e.Column.FieldName == "ItemCode")
			{
				if (Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "CardCode")) == "")
				{
					e.RepositoryItem = rep_cfl_ItemCode;
				}
				else
				{
					e.RepositoryItem = rep_cfl_ItemCode_CardCode;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (CallID != 0 && gv_Doc.FocusedColumn.FieldName == "Type")
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Find_Click(object sender, EventArgs e)
	{
		if (FormMode != Enums.FormViewModeType.View)
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
			{
				FormMode = Enums.FormViewModeType.View;
				btn_Find.Enabled = false;
				WinCommons._OpenProcessing();
				await LoadData();
				await LoadLine();
				WinCommons._CloseProcessing();
				btn_Find.Enabled = true;
			}
		}
		else
		{
			btn_Find.Enabled = false;
			WinCommons._OpenProcessing();
			await LoadData();
			await LoadLine();
			WinCommons._CloseProcessing();
			btn_Find.Enabled = true;
		}
	}

	private void dte_ToDate_Validating(object sender, CancelEventArgs e)
	{
		DateTime dateTime = dte_ToDate.DateTime;
		DateTime dateTime2 = dte_FromDate.DateTime;
		if (!(dateTime == DateTime.MinValue) && !(dateTime2 == DateTime.MinValue) && dateTime2 > dateTime)
		{
			WinCommons.Error(Enums.MessageCode.FromDateMustBeBefore2Date, "");
			e.Cancel = true;
		}
	}

	private void btn_Journey_Click(object sender, EventArgs e)
	{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Service.frm_ServiceCalls));
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
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_ServiceCalls = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_Type = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_cfl_BPCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_Contact = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_cfl_ItemCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_cfl_SerialNum = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_CallType = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_Orgin = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ccb_Tags = new Apzon.Forms.UserControls.RepositoryItemCheckedComboBoxEditDefineNew();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_cfl_HandleBy = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_Priority = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_memx_Resolution = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridColumnEx44 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_cfl_BPCode_Vendor = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.rep_cfl_ItemCode_CardCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.xtraTabControlEx1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_Issue = new DevExpress.XtraEditors.SimpleButton();
		this.grd_Issue = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Issue = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_cfl_IssueId = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx20 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_IssueType = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gridColumnEx21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx22 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_cfl_IssueHandleBy = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx23 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_IssueStatus = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx24 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_memx_IssueNote = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
		this.btn_NewTask = new DevExpress.XtraEditors.SimpleButton();
		this.grd_Task = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Task = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx26 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_cfl_TaskDetails = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx29 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx30 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx27 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_hour_ENDTime = new Apzon.Controls.Controls.RepositoryItemHourEdit();
		this.rep_cfl_Project = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.xtraTabPage3 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_NewSolution = new DevExpress.XtraEditors.SimpleButton();
		this.grd_Solution = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Solution = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Solution = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx32 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_cfl_SltCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx33 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_memx_SltSubject = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridColumnEx34 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx35 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_cfl_Owner = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx36 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx37 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_SolutionStatus = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.xtraTabPage4 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_APInvoice = new DevExpress.XtraEditors.SimpleButton();
		this.btn_GoodReceiptPO = new DevExpress.XtraEditors.SimpleButton();
		this.btn_GoodReturn = new DevExpress.XtraEditors.SimpleButton();
		this.btn_ARInvoice = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Delivery = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Return = new DevExpress.XtraEditors.SimpleButton();
		this.grd_DocLine = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_DocLine = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_DocLine = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx38 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_ObjType = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx40 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_cfl_DocNum = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx39 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx28 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx31 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx41 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx42 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_DocLineStatus = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx43 = new Apzon.Controls.Grid.GridColumnEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.ccb_Tags = new Apzon.Forms.UserControls.CheckedComboBoxEditDefineNew();
		this.luk_CallType = new Apzon.Controls.Editor.MyLookUpEdit.LookUpEditSimple();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.cfl_HandleBy = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txt_CallId = new Apzon.Controls.Editor.TextEditEx();
		this.cfl_BPCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.dte_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.dte_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.btn_Find = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctx_ServiceCalls.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_BPCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Contact).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_SerialNum).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_CallType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Orgin).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ccb_Tags).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_HandleBy).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Priority).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_memx_Resolution).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_BPCode_Vendor).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_ItemCode_CardCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).BeginInit();
		this.xtraTabControlEx1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Issue).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Issue).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_IssueId).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_IssueType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_IssueHandleBy).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_IssueStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_memx_IssueNote).BeginInit();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Task).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Task).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_TaskDetails).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_hour_ENDTime).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_Project).BeginInit();
		this.xtraTabPage3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Solution).BeginInit();
		this.ctx_Solution.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Solution).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_SltCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_memx_SltSubject).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_Owner).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_SolutionStatus).BeginInit();
		this.xtraTabPage4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_DocLine).BeginInit();
		this.ctx_DocLine.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_DocLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_ObjType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_DocNum).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_DocLineStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ccb_Tags.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CallType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_HandleBy.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CallId.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_BPCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dte_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dte_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dte_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dte_FromDate.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctx_ServiceCalls;
		this.grd_Doc.Location = new System.Drawing.Point(4, 66);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[13]
		{
			this.rep_cfl_BPCode, this.rep_cfl_ItemCode, this.rep_luk_CallType, this.rep_ccb_Tags, this.rep_cfl_HandleBy, this.rep_luk_Orgin, this.rep_luk_Priority, this.rep_luk_Type, this.rep_luk_Contact, this.rep_memx_Resolution,
			this.rep_cfl_BPCode_Vendor, this.rep_cfl_ItemCode_CardCode, this.rep_cfl_SerialNum
		});
		this.grd_Doc.Size = new System.Drawing.Size(1222, 205);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctx_ServiceCalls.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addToolStripMenuItem, this.removeToolStripMenuItem1 });
		this.ctx_ServiceCalls.Name = "ctx_ServiceCalls";
		this.ctx_ServiceCalls.Size = new System.Drawing.Size(118, 48);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.removeToolStripMenuItem1.Name = "removeToolStripMenuItem1";
		this.removeToolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem1.Text = "Remove";
		this.removeToolStripMenuItem1.Click += new System.EventHandler(this.removeToolStripMenuItem1_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[18]
		{
			this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9, this.gridColumnEx10,
			this.gridColumnEx11, this.gridColumnEx12, this.gridColumnEx13, this.gridColumnEx14, this.gridColumnEx15, this.gridColumnEx16, this.gridColumnEx17, this.gridColumnEx44
		});
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oServiceCallsManagement";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OSCL";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Doc_CustomRowCellEdit);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gridColumnEx1.Caption = "No.";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "CallID";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx1.Width = 41;
		this.gridColumnEx2.Caption = "Type";
		this.gridColumnEx2.ColumnEdit = this.rep_luk_Type;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Type";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 77;
		this.rep_luk_Type.AutoHeight = false;
		this.rep_luk_Type.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_luk_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_Type.IsSearchControl = false;
		this.rep_luk_Type.Name = "rep_luk_Type";
		this.rep_luk_Type.NullText = "";
		this.rep_luk_Type.ShowFooter = false;
		this.rep_luk_Type.ShowHeader = false;
		this.gridColumnEx3.Caption = "BP Code";
		this.gridColumnEx3.ColumnEdit = this.rep_cfl_BPCode;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "CardCode";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.gridColumnEx3.Width = 121;
		this.rep_cfl_BPCode.AutoHeight = false;
		this.rep_cfl_BPCode.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.rep_cfl_BPCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_BPCode.ContainerControl = this;
		this.rep_cfl_BPCode.DisplayMember = "";
		this.rep_cfl_BPCode.DisplayMember2 = "";
		this.rep_cfl_BPCode.LoadDataSourceWhenEnter = false;
		this.rep_cfl_BPCode.Name = "rep_cfl_BPCode";
		this.rep_cfl_BPCode.SeparateCharacterForMulti = ';';
		this.rep_cfl_BPCode.SeparateString = ", ";
		this.rep_cfl_BPCode.UsingAutoCompleteBox = false;
		this.rep_cfl_BPCode.UsingDataSource = true;
		this.rep_cfl_BPCode.ValidateWhenNoDataSource = false;
		this.rep_cfl_BPCode.ValueMember = "";
		this.gridColumnEx4.Caption = "BP Name";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "CardName";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.gridColumnEx4.Width = 173;
		this.gridColumnEx5.Caption = "Contact Person";
		this.gridColumnEx5.ColumnEdit = this.rep_luk_Contact;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "ContactPerson";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 4;
		this.gridColumnEx5.Width = 103;
		this.rep_luk_Contact.AutoHeight = false;
		this.rep_luk_Contact.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_luk_Contact.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_Contact.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CntctCode", "", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_luk_Contact.IsSearchControl = false;
		this.rep_luk_Contact.Name = "rep_luk_Contact";
		this.rep_luk_Contact.NullText = "";
		this.rep_luk_Contact.ShowFooter = false;
		this.rep_luk_Contact.ShowHeader = false;
		this.gridColumnEx6.Caption = "Tel";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Tel";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 5;
		this.gridColumnEx6.Width = 124;
		this.gridColumnEx7.Caption = "E-mail";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "Email";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 6;
		this.gridColumnEx7.Width = 176;
		this.gridColumnEx8.Caption = "Item Code";
		this.gridColumnEx8.ColumnEdit = this.rep_cfl_ItemCode;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "ItemCode";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 7;
		this.gridColumnEx8.Width = 117;
		this.rep_cfl_ItemCode.AutoHeight = false;
		this.rep_cfl_ItemCode.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions22.Image");
		this.rep_cfl_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_ItemCode.ContainerControl = this;
		this.rep_cfl_ItemCode.DisplayMember = "";
		this.rep_cfl_ItemCode.DisplayMember2 = "";
		this.rep_cfl_ItemCode.LoadDataSourceWhenEnter = false;
		this.rep_cfl_ItemCode.Name = "rep_cfl_ItemCode";
		this.rep_cfl_ItemCode.SeparateCharacterForMulti = ';';
		this.rep_cfl_ItemCode.SeparateString = ", ";
		this.rep_cfl_ItemCode.UsingAutoCompleteBox = false;
		this.rep_cfl_ItemCode.UsingDataSource = true;
		this.rep_cfl_ItemCode.ValidateWhenNoDataSource = false;
		this.rep_cfl_ItemCode.ValueMember = "";
		this.gridColumnEx9.Caption = "Item Name";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "ItemName";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 8;
		this.gridColumnEx9.Width = 152;
		this.gridColumnEx10.Caption = "Serial";
		this.gridColumnEx10.ColumnEdit = this.rep_cfl_SerialNum;
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "SerialNum";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 9;
		this.gridColumnEx10.Width = 102;
		this.rep_cfl_SerialNum.AutoHeight = false;
		this.rep_cfl_SerialNum.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions24.Image");
		this.rep_cfl_SerialNum.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_SerialNum.ContainerControl = this;
		this.rep_cfl_SerialNum.DisplayMember = "";
		this.rep_cfl_SerialNum.DisplayMember2 = "";
		this.rep_cfl_SerialNum.LoadDataSourceWhenEnter = false;
		this.rep_cfl_SerialNum.Name = "rep_cfl_SerialNum";
		this.rep_cfl_SerialNum.SeparateCharacterForMulti = ';';
		this.rep_cfl_SerialNum.SeparateString = ", ";
		this.rep_cfl_SerialNum.UsingAutoCompleteBox = false;
		this.rep_cfl_SerialNum.UsingDataSource = true;
		this.rep_cfl_SerialNum.ValidateWhenNoDataSource = false;
		this.rep_cfl_SerialNum.ValueMember = "";
		this.gridColumnEx11.Caption = "Call Type";
		this.gridColumnEx11.ColumnEdit = this.rep_luk_CallType;
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "CallType";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 11;
		this.gridColumnEx11.Width = 115;
		this.rep_luk_CallType.AutoHeight = false;
		this.rep_luk_CallType.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.rep_luk_CallType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_CallType.CheckFields = null;
		this.rep_luk_CallType.CheckTables = null;
		this.rep_luk_CallType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_luk_CallType.Name = "rep_luk_CallType";
		this.rep_luk_CallType.NullText = "";
		this.rep_luk_CallType.ShowFooter = false;
		this.rep_luk_CallType.ShowHeader = false;
		this.gridColumnEx12.Caption = "Orgin";
		this.gridColumnEx12.ColumnEdit = this.rep_luk_Orgin;
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "Orgin";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Visible = true;
		this.gridColumnEx12.VisibleIndex = 12;
		this.gridColumnEx12.Width = 54;
		this.rep_luk_Orgin.AutoHeight = false;
		this.rep_luk_Orgin.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_luk_Orgin.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_Orgin.IsSearchControl = false;
		this.rep_luk_Orgin.Name = "rep_luk_Orgin";
		this.rep_luk_Orgin.NullText = "";
		this.rep_luk_Orgin.ShowFooter = false;
		this.rep_luk_Orgin.ShowHeader = false;
		this.gridColumnEx13.Caption = "Tags";
		this.gridColumnEx13.ColumnEdit = this.rep_ccb_Tags;
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "Tags";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx13.Visible = true;
		this.gridColumnEx13.VisibleIndex = 13;
		this.gridColumnEx13.Width = 96;
		this.rep_ccb_Tags.AutoHeight = false;
		this.rep_ccb_Tags.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ccb_Tags.CheckFields = null;
		this.rep_ccb_Tags.CheckTables = null;
		this.rep_ccb_Tags.DisableDefineNew = false;
		this.rep_ccb_Tags.IncrementalSearch = true;
		this.rep_ccb_Tags.Name = "rep_ccb_Tags";
		this.gridColumnEx14.Caption = "Handle By";
		this.gridColumnEx14.ColumnEdit = this.rep_cfl_HandleBy;
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "HandleBy";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx14.Visible = true;
		this.gridColumnEx14.VisibleIndex = 14;
		this.gridColumnEx14.Width = 116;
		this.rep_cfl_HandleBy.AutoHeight = false;
		this.rep_cfl_HandleBy.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions26.Image");
		this.rep_cfl_HandleBy.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_HandleBy.ContainerControl = this;
		this.rep_cfl_HandleBy.DisplayMember = "";
		this.rep_cfl_HandleBy.DisplayMember2 = "";
		this.rep_cfl_HandleBy.LoadDataSourceWhenEnter = false;
		this.rep_cfl_HandleBy.Name = "rep_cfl_HandleBy";
		this.rep_cfl_HandleBy.SeparateCharacterForMulti = ';';
		this.rep_cfl_HandleBy.SeparateString = ", ";
		this.rep_cfl_HandleBy.UsingAutoCompleteBox = false;
		this.rep_cfl_HandleBy.UsingDataSource = true;
		this.rep_cfl_HandleBy.ValidateWhenNoDataSource = false;
		this.rep_cfl_HandleBy.ValueMember = "";
		this.gridColumnEx15.Caption = "Priority";
		this.gridColumnEx15.ColumnEdit = this.rep_luk_Priority;
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "Priority";
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx15.Visible = true;
		this.gridColumnEx15.VisibleIndex = 15;
		this.rep_luk_Priority.AutoHeight = false;
		this.rep_luk_Priority.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_luk_Priority.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_Priority.IsSearchControl = false;
		this.rep_luk_Priority.Name = "rep_luk_Priority";
		this.rep_luk_Priority.NullText = "";
		this.rep_luk_Priority.ShowFooter = false;
		this.rep_luk_Priority.ShowHeader = false;
		this.gridColumnEx16.Caption = "Remark";
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "Remark";
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx16.Visible = true;
		this.gridColumnEx16.VisibleIndex = 16;
		this.gridColumnEx16.Width = 264;
		this.gridColumnEx17.Caption = "Resolution";
		this.gridColumnEx17.ColumnEdit = this.rep_memx_Resolution;
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.FieldName = "Resolution";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx17.Visible = true;
		this.gridColumnEx17.VisibleIndex = 17;
		this.gridColumnEx17.Width = 298;
		this.rep_memx_Resolution.AutoHeight = false;
		this.rep_memx_Resolution.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_memx_Resolution.Name = "rep_memx_Resolution";
		this.rep_memx_Resolution.ShowIcon = false;
		this.gridColumnEx44.Caption = "Create on";
		this.gridColumnEx44.CurrencyDateField = null;
		this.gridColumnEx44.CurrencyField = null;
		this.gridColumnEx44.CurrencyFixed = null;
		this.gridColumnEx44.CurrencyFixedRate = 0.0;
		this.gridColumnEx44.CurrencyRateField = null;
		this.gridColumnEx44.FieldName = "CreateDate";
		this.gridColumnEx44.Name = "gridColumnEx44";
		this.gridColumnEx44.OptionsColumn.ReadOnly = true;
		this.gridColumnEx44.PrimaryKey = false;
		this.gridColumnEx44.UnitField = null;
		this.gridColumnEx44.Visible = true;
		this.gridColumnEx44.VisibleIndex = 10;
		this.gridColumnEx44.Width = 91;
		this.rep_cfl_BPCode_Vendor.AutoHeight = false;
		this.rep_cfl_BPCode_Vendor.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions28.Image");
		this.rep_cfl_BPCode_Vendor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_BPCode_Vendor.ContainerControl = this;
		this.rep_cfl_BPCode_Vendor.DisplayMember = "";
		this.rep_cfl_BPCode_Vendor.DisplayMember2 = "";
		this.rep_cfl_BPCode_Vendor.LoadDataSourceWhenEnter = false;
		this.rep_cfl_BPCode_Vendor.Name = "rep_cfl_BPCode_Vendor";
		this.rep_cfl_BPCode_Vendor.SeparateCharacterForMulti = ';';
		this.rep_cfl_BPCode_Vendor.SeparateString = ", ";
		this.rep_cfl_BPCode_Vendor.UsingAutoCompleteBox = false;
		this.rep_cfl_BPCode_Vendor.UsingDataSource = true;
		this.rep_cfl_BPCode_Vendor.ValidateWhenNoDataSource = false;
		this.rep_cfl_BPCode_Vendor.ValueMember = "";
		this.rep_cfl_ItemCode_CardCode.AutoHeight = false;
		this.rep_cfl_ItemCode_CardCode.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions30.Image");
		this.rep_cfl_ItemCode_CardCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance11, appearanceHovered11, appearancePressed11, appearanceDisabled11, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance12, appearanceHovered12, appearancePressed12, appearanceDisabled12, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_ItemCode_CardCode.ContainerControl = this;
		this.rep_cfl_ItemCode_CardCode.DisplayMember = "";
		this.rep_cfl_ItemCode_CardCode.DisplayMember2 = "";
		this.rep_cfl_ItemCode_CardCode.LoadDataSourceWhenEnter = false;
		this.rep_cfl_ItemCode_CardCode.Name = "rep_cfl_ItemCode_CardCode";
		this.rep_cfl_ItemCode_CardCode.SeparateCharacterForMulti = ';';
		this.rep_cfl_ItemCode_CardCode.SeparateString = ", ";
		this.rep_cfl_ItemCode_CardCode.UsingAutoCompleteBox = false;
		this.rep_cfl_ItemCode_CardCode.UsingDataSource = true;
		this.rep_cfl_ItemCode_CardCode.ValidateWhenNoDataSource = false;
		this.rep_cfl_ItemCode_CardCode.ValueMember = "";
		this.xtraTabControlEx1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControlEx1.IsSearchControl = false;
		this.xtraTabControlEx1.Location = new System.Drawing.Point(2, 6);
		this.xtraTabControlEx1.Name = "xtraTabControlEx1";
		this.xtraTabControlEx1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControlEx1.Size = new System.Drawing.Size(1232, 141);
		this.xtraTabControlEx1.TabIndex = 1;
		this.xtraTabControlEx1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[4] { this.xtraTabPage1, this.xtraTabPage2, this.xtraTabPage3, this.xtraTabPage4 });
		this.xtraTabPage1.Controls.Add(this.btn_Issue);
		this.xtraTabPage1.Controls.Add(this.grd_Issue);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Tracking Issues";
		this.xtraTabPage1.FieldName = null;
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(1230, 116);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Tracking Issues";
		this.btn_Issue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Issue.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Issue.Location = new System.Drawing.Point(1044, 19);
		this.btn_Issue.Name = "btn_Issue";
		this.btn_Issue.Size = new System.Drawing.Size(110, 25);
		this.btn_Issue.TabIndex = 1;
		this.btn_Issue.Text = "New Tracking Issue";
		this.btn_Issue.Click += new System.EventHandler(this.btn_Issue_Click);
		this.grd_Issue.AllowPasteExcel = true;
		this.grd_Issue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Issue.ClipboardData = null;
		this.grd_Issue.Location = new System.Drawing.Point(-1, 0);
		this.grd_Issue.MainView = this.gv_Issue;
		this.grd_Issue.Name = "grd_Issue";
		this.grd_Issue.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rep_luk_IssueStatus, this.rep_luk_IssueType, this.rep_cfl_IssueHandleBy, this.rep_cfl_IssueId, this.rep_memx_IssueNote });
		this.grd_Issue.Size = new System.Drawing.Size(970, 114);
		this.grd_Issue.TabIndex = 0;
		this.grd_Issue.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Issue });
		this.gv_Issue.Appearance.FocusedRow.BackColor = System.Drawing.SystemColors.ActiveBorder;
		this.gv_Issue.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Issue.Appearance.Row.BackColor = System.Drawing.SystemColors.ActiveBorder;
		this.gv_Issue.Appearance.Row.Options.UseBackColor = true;
		this.gv_Issue.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Issue.ColumnKey = null;
		this.gv_Issue.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumnEx18, this.gridColumnEx19, this.gridColumnEx20, this.gridColumnEx21, this.gridColumnEx22, this.gridColumnEx23, this.gridColumnEx24 });
		this.gv_Issue.DisableFormSetting = false;
		this.gv_Issue.DisplayException = false;
		this.gv_Issue.EditColumn = null;
		this.gv_Issue.FieldCurrency = null;
		this.gv_Issue.FieldRate = null;
		this.gv_Issue.FormId = "oServiceCallsManagement";
		this.gv_Issue.GridControl = this.grd_Issue;
		this.gv_Issue.IgnoreFilter = false;
		this.gv_Issue.IndexEditRow = -1;
		this.gv_Issue.IndicatorWidth = 35;
		this.gv_Issue.IsApzTable = false;
		this.gv_Issue.IsInsertNewRow = false;
		this.gv_Issue.IsLoadingData = false;
		this.gv_Issue.IsMappingContextAction = false;
		this.gv_Issue.ItemId = "SCL1";
		this.gv_Issue.LstColumnAllowBatch = null;
		this.gv_Issue.LstColumnNotAllowBatch = null;
		this.gv_Issue.LstColumnNotEditable = null;
		this.gv_Issue.LstColumNotSetting = null;
		this.gv_Issue.Name = "gv_Issue";
		this.gv_Issue.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Issue.OptionsBehavior.ReadOnly = true;
		this.gv_Issue.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gv_Issue.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.gv_Issue.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Issue.OptionsView.ColumnAutoWidth = false;
		this.gv_Issue.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Issue.OptionsView.ShowGroupPanel = false;
		this.gv_Issue.UseBatchMod = false;
		this.gridColumnEx18.ColumnEdit = this.rep_cfl_IssueId;
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = null;
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = null;
		this.gridColumnEx18.FieldName = "Id";
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.PrimaryKey = true;
		this.gridColumnEx18.UnitField = null;
		this.gridColumnEx18.Visible = true;
		this.gridColumnEx18.VisibleIndex = 0;
		this.gridColumnEx18.Width = 55;
		this.rep_cfl_IssueId.AcceptFalseInput = true;
		this.rep_cfl_IssueId.AutoHeight = false;
		this.rep_cfl_IssueId.AutoLoadDataSource = true;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		this.rep_cfl_IssueId.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance13, appearanceHovered13, appearancePressed13, appearanceDisabled13, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance14, appearanceHovered14, appearancePressed14, appearanceDisabled14, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_IssueId.ContainerControl = this;
		this.rep_cfl_IssueId.DisplayMember = "";
		this.rep_cfl_IssueId.DisplayMember2 = "";
		this.rep_cfl_IssueId.LoadDataSourceWhenEnter = false;
		this.rep_cfl_IssueId.Name = "rep_cfl_IssueId";
		this.rep_cfl_IssueId.SeparateCharacterForMulti = ';';
		this.rep_cfl_IssueId.SeparateString = ", ";
		this.rep_cfl_IssueId.UsingAutoCompleteBox = false;
		this.rep_cfl_IssueId.UsingDataSource = true;
		this.rep_cfl_IssueId.ValidateWhenNoDataSource = false;
		this.rep_cfl_IssueId.ValueMember = "";
		this.gridColumnEx19.Caption = "Issue Date";
		this.gridColumnEx19.CurrencyDateField = null;
		this.gridColumnEx19.CurrencyField = null;
		this.gridColumnEx19.CurrencyFixed = null;
		this.gridColumnEx19.CurrencyFixedRate = 0.0;
		this.gridColumnEx19.CurrencyRateField = null;
		this.gridColumnEx19.FieldName = "IssueDate";
		this.gridColumnEx19.Name = "gridColumnEx19";
		this.gridColumnEx19.PrimaryKey = false;
		this.gridColumnEx19.UnitField = null;
		this.gridColumnEx19.Visible = true;
		this.gridColumnEx19.VisibleIndex = 1;
		this.gridColumnEx19.Width = 92;
		this.gridColumnEx20.Caption = "Issue Type";
		this.gridColumnEx20.ColumnEdit = this.rep_luk_IssueType;
		this.gridColumnEx20.CurrencyDateField = null;
		this.gridColumnEx20.CurrencyField = null;
		this.gridColumnEx20.CurrencyFixed = null;
		this.gridColumnEx20.CurrencyFixedRate = 0.0;
		this.gridColumnEx20.CurrencyRateField = null;
		this.gridColumnEx20.FieldName = "IssueType";
		this.gridColumnEx20.Name = "gridColumnEx20";
		this.gridColumnEx20.PrimaryKey = false;
		this.gridColumnEx20.UnitField = null;
		this.gridColumnEx20.Visible = true;
		this.gridColumnEx20.VisibleIndex = 2;
		this.gridColumnEx20.Width = 89;
		this.rep_luk_IssueType.AutoHeight = false;
		this.rep_luk_IssueType.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.rep_luk_IssueType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_IssueType.CheckFields = null;
		this.rep_luk_IssueType.CheckTables = null;
		this.rep_luk_IssueType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name3")
		});
		this.rep_luk_IssueType.Name = "rep_luk_IssueType";
		this.rep_luk_IssueType.NullText = "";
		this.rep_luk_IssueType.ShowFooter = false;
		this.rep_luk_IssueType.ShowHeader = false;
		this.gridColumnEx21.Caption = "Report by";
		this.gridColumnEx21.CurrencyDateField = null;
		this.gridColumnEx21.CurrencyField = null;
		this.gridColumnEx21.CurrencyFixed = null;
		this.gridColumnEx21.CurrencyFixedRate = 0.0;
		this.gridColumnEx21.CurrencyRateField = null;
		this.gridColumnEx21.FieldName = "Reporter";
		this.gridColumnEx21.Name = "gridColumnEx21";
		this.gridColumnEx21.PrimaryKey = false;
		this.gridColumnEx21.UnitField = null;
		this.gridColumnEx21.Visible = true;
		this.gridColumnEx21.VisibleIndex = 3;
		this.gridColumnEx21.Width = 118;
		this.gridColumnEx22.Caption = "Handle by";
		this.gridColumnEx22.ColumnEdit = this.rep_cfl_IssueHandleBy;
		this.gridColumnEx22.CurrencyDateField = null;
		this.gridColumnEx22.CurrencyField = null;
		this.gridColumnEx22.CurrencyFixed = null;
		this.gridColumnEx22.CurrencyFixedRate = 0.0;
		this.gridColumnEx22.CurrencyRateField = null;
		this.gridColumnEx22.FieldName = "HandleBy";
		this.gridColumnEx22.Name = "gridColumnEx22";
		this.gridColumnEx22.PrimaryKey = false;
		this.gridColumnEx22.UnitField = null;
		this.gridColumnEx22.Visible = true;
		this.gridColumnEx22.VisibleIndex = 4;
		this.gridColumnEx22.Width = 124;
		this.rep_cfl_IssueHandleBy.AutoHeight = false;
		this.rep_cfl_IssueHandleBy.AutoLoadDataSource = true;
		editorButtonImageOptions8.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions10.Image");
		this.rep_cfl_IssueHandleBy.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance15, appearanceHovered15, appearancePressed15, appearanceDisabled15, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance16, appearanceHovered16, appearancePressed16, appearanceDisabled16, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_IssueHandleBy.ContainerControl = this;
		this.rep_cfl_IssueHandleBy.DisplayMember = "";
		this.rep_cfl_IssueHandleBy.DisplayMember2 = "";
		this.rep_cfl_IssueHandleBy.LoadDataSourceWhenEnter = false;
		this.rep_cfl_IssueHandleBy.Name = "rep_cfl_IssueHandleBy";
		this.rep_cfl_IssueHandleBy.SeparateCharacterForMulti = ';';
		this.rep_cfl_IssueHandleBy.SeparateString = ", ";
		this.rep_cfl_IssueHandleBy.UsingAutoCompleteBox = false;
		this.rep_cfl_IssueHandleBy.UsingDataSource = true;
		this.rep_cfl_IssueHandleBy.ValidateWhenNoDataSource = false;
		this.rep_cfl_IssueHandleBy.ValueMember = "";
		this.gridColumnEx23.Caption = "Status";
		this.gridColumnEx23.ColumnEdit = this.rep_luk_IssueStatus;
		this.gridColumnEx23.CurrencyDateField = null;
		this.gridColumnEx23.CurrencyField = null;
		this.gridColumnEx23.CurrencyFixed = null;
		this.gridColumnEx23.CurrencyFixedRate = 0.0;
		this.gridColumnEx23.CurrencyRateField = null;
		this.gridColumnEx23.FieldName = "DocStatus";
		this.gridColumnEx23.Name = "gridColumnEx23";
		this.gridColumnEx23.PrimaryKey = false;
		this.gridColumnEx23.UnitField = null;
		this.gridColumnEx23.Visible = true;
		this.gridColumnEx23.VisibleIndex = 5;
		this.gridColumnEx23.Width = 83;
		this.rep_luk_IssueStatus.AutoHeight = false;
		this.rep_luk_IssueStatus.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_luk_IssueStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_IssueStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name2")
		});
		this.rep_luk_IssueStatus.IsSearchControl = false;
		this.rep_luk_IssueStatus.Name = "rep_luk_IssueStatus";
		this.rep_luk_IssueStatus.NullText = "";
		this.rep_luk_IssueStatus.ShowFooter = false;
		this.rep_luk_IssueStatus.ShowHeader = false;
		this.gridColumnEx24.Caption = "Note";
		this.gridColumnEx24.ColumnEdit = this.rep_memx_IssueNote;
		this.gridColumnEx24.CurrencyDateField = null;
		this.gridColumnEx24.CurrencyField = null;
		this.gridColumnEx24.CurrencyFixed = null;
		this.gridColumnEx24.CurrencyFixedRate = 0.0;
		this.gridColumnEx24.CurrencyRateField = null;
		this.gridColumnEx24.FieldName = "Note";
		this.gridColumnEx24.Name = "gridColumnEx24";
		this.gridColumnEx24.PrimaryKey = false;
		this.gridColumnEx24.UnitField = null;
		this.gridColumnEx24.Visible = true;
		this.gridColumnEx24.VisibleIndex = 6;
		this.gridColumnEx24.Width = 309;
		this.rep_memx_IssueNote.AutoHeight = false;
		this.rep_memx_IssueNote.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_memx_IssueNote.Name = "rep_memx_IssueNote";
		this.rep_memx_IssueNote.ShowIcon = false;
		this.xtraTabPage2.Controls.Add(this.btn_NewTask);
		this.xtraTabPage2.Controls.Add(this.grd_Task);
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.Size = new System.Drawing.Size(1230, 116);
		this.xtraTabPage2.Text = "Related Tasks";
		this.btn_NewTask.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_NewTask.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_NewTask.Location = new System.Drawing.Point(1041, 15);
		this.btn_NewTask.Name = "btn_NewTask";
		this.btn_NewTask.Size = new System.Drawing.Size(110, 25);
		this.btn_NewTask.TabIndex = 3;
		this.btn_NewTask.Text = "New Task";
		this.btn_NewTask.Click += new System.EventHandler(this.btn_NewTask_Click);
		this.grd_Task.AllowPasteExcel = false;
		this.grd_Task.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Task.ClipboardData = null;
		this.grd_Task.Location = new System.Drawing.Point(-1, 0);
		this.grd_Task.MainView = this.gv_Task;
		this.grd_Task.Name = "grd_Task";
		this.grd_Task.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_cfl_Project, this.rep_cfl_TaskDetails, this.rep_hour_ENDTime });
		this.grd_Task.Size = new System.Drawing.Size(970, 114);
		this.grd_Task.TabIndex = 2;
		this.grd_Task.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Task });
		this.gv_Task.Appearance.FocusedRow.BackColor = System.Drawing.SystemColors.ActiveBorder;
		this.gv_Task.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Task.Appearance.Row.BackColor = System.Drawing.SystemColors.ActiveBorder;
		this.gv_Task.Appearance.Row.Options.UseBackColor = true;
		this.gv_Task.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Task.ColumnKey = null;
		this.gv_Task.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumnEx25, this.gridColumnEx26, this.gridColumnEx29, this.gridColumnEx30, this.gridColumnEx27 });
		this.gv_Task.DisableFormSetting = false;
		this.gv_Task.DisplayException = false;
		this.gv_Task.EditColumn = null;
		this.gv_Task.FieldCurrency = null;
		this.gv_Task.FieldRate = null;
		this.gv_Task.FormId = "oServiceCallsManagement";
		this.gv_Task.GridControl = this.grd_Task;
		this.gv_Task.IgnoreFilter = false;
		this.gv_Task.IndexEditRow = -1;
		this.gv_Task.IndicatorWidth = 35;
		this.gv_Task.IsApzTable = false;
		this.gv_Task.IsInsertNewRow = false;
		this.gv_Task.IsLoadingData = false;
		this.gv_Task.IsMappingContextAction = false;
		this.gv_Task.ItemId = "OTSK";
		this.gv_Task.LstColumnAllowBatch = null;
		this.gv_Task.LstColumnNotAllowBatch = null;
		this.gv_Task.LstColumnNotEditable = null;
		this.gv_Task.LstColumNotSetting = null;
		this.gv_Task.Name = "gv_Task";
		this.gv_Task.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Task.OptionsBehavior.ReadOnly = true;
		this.gv_Task.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gv_Task.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.gv_Task.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Task.OptionsView.ColumnAutoWidth = false;
		this.gv_Task.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Task.OptionsView.ShowGroupPanel = false;
		this.gv_Task.UseBatchMod = false;
		this.gv_Task.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gv_Task_CustomUnboundColumnData);
		this.gridColumnEx25.CurrencyDateField = null;
		this.gridColumnEx25.CurrencyField = null;
		this.gridColumnEx25.CurrencyFixed = null;
		this.gridColumnEx25.CurrencyFixedRate = 0.0;
		this.gridColumnEx25.CurrencyRateField = null;
		this.gridColumnEx25.FieldName = "ClgCode";
		this.gridColumnEx25.Name = "gridColumnEx25";
		this.gridColumnEx25.PrimaryKey = false;
		this.gridColumnEx25.UnitField = null;
		this.gridColumnEx26.Caption = "Task";
		this.gridColumnEx26.ColumnEdit = this.rep_cfl_TaskDetails;
		this.gridColumnEx26.CurrencyDateField = null;
		this.gridColumnEx26.CurrencyField = null;
		this.gridColumnEx26.CurrencyFixed = null;
		this.gridColumnEx26.CurrencyFixedRate = 0.0;
		this.gridColumnEx26.CurrencyRateField = null;
		this.gridColumnEx26.FieldName = "Details";
		this.gridColumnEx26.Name = "gridColumnEx26";
		this.gridColumnEx26.PrimaryKey = false;
		this.gridColumnEx26.UnitField = null;
		this.gridColumnEx26.Visible = true;
		this.gridColumnEx26.VisibleIndex = 0;
		this.gridColumnEx26.Width = 440;
		this.rep_cfl_TaskDetails.AutoHeight = false;
		this.rep_cfl_TaskDetails.AutoLoadDataSource = true;
		editorButtonImageOptions9.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions12.Image");
		this.rep_cfl_TaskDetails.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance17, appearanceHovered17, appearancePressed17, appearanceDisabled17, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance18, appearanceHovered18, appearancePressed18, appearanceDisabled18, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_TaskDetails.ContainerControl = this;
		this.rep_cfl_TaskDetails.DisplayMember = "";
		this.rep_cfl_TaskDetails.DisplayMember2 = "";
		this.rep_cfl_TaskDetails.LoadDataSourceWhenEnter = false;
		this.rep_cfl_TaskDetails.Name = "rep_cfl_TaskDetails";
		this.rep_cfl_TaskDetails.SeparateCharacterForMulti = ';';
		this.rep_cfl_TaskDetails.SeparateString = ", ";
		this.rep_cfl_TaskDetails.UsingAutoCompleteBox = false;
		this.rep_cfl_TaskDetails.UsingDataSource = true;
		this.rep_cfl_TaskDetails.ValidateWhenNoDataSource = false;
		this.rep_cfl_TaskDetails.ValueMember = "";
		this.gridColumnEx29.Caption = "Handle by";
		this.gridColumnEx29.CurrencyDateField = null;
		this.gridColumnEx29.CurrencyField = null;
		this.gridColumnEx29.CurrencyFixed = null;
		this.gridColumnEx29.CurrencyFixedRate = 0.0;
		this.gridColumnEx29.CurrencyRateField = null;
		this.gridColumnEx29.FieldName = "AssignTo";
		this.gridColumnEx29.Name = "gridColumnEx29";
		this.gridColumnEx29.PrimaryKey = false;
		this.gridColumnEx29.UnboundType = DevExpress.Data.UnboundColumnType.String;
		this.gridColumnEx29.UnitField = null;
		this.gridColumnEx29.Visible = true;
		this.gridColumnEx29.VisibleIndex = 1;
		this.gridColumnEx29.Width = 141;
		this.gridColumnEx30.Caption = "Due Date";
		this.gridColumnEx30.CurrencyDateField = null;
		this.gridColumnEx30.CurrencyField = null;
		this.gridColumnEx30.CurrencyFixed = null;
		this.gridColumnEx30.CurrencyFixedRate = 0.0;
		this.gridColumnEx30.CurrencyRateField = null;
		this.gridColumnEx30.FieldName = "endDate";
		this.gridColumnEx30.Name = "gridColumnEx30";
		this.gridColumnEx30.PrimaryKey = false;
		this.gridColumnEx30.UnitField = null;
		this.gridColumnEx30.Visible = true;
		this.gridColumnEx30.VisibleIndex = 2;
		this.gridColumnEx30.Width = 112;
		this.gridColumnEx27.Caption = "Due Time";
		this.gridColumnEx27.ColumnEdit = this.rep_hour_ENDTime;
		this.gridColumnEx27.CurrencyDateField = null;
		this.gridColumnEx27.CurrencyField = null;
		this.gridColumnEx27.CurrencyFixed = null;
		this.gridColumnEx27.CurrencyFixedRate = 0.0;
		this.gridColumnEx27.CurrencyRateField = null;
		this.gridColumnEx27.FieldName = "ENDTime";
		this.gridColumnEx27.Name = "gridColumnEx27";
		this.gridColumnEx27.PrimaryKey = false;
		this.gridColumnEx27.UnitField = null;
		this.gridColumnEx27.Visible = true;
		this.gridColumnEx27.VisibleIndex = 3;
		this.gridColumnEx27.Width = 56;
		this.rep_hour_ENDTime.AutoHeight = false;
		this.rep_hour_ENDTime.IsSearchControl = false;
		this.rep_hour_ENDTime.MaxLength = 5;
		this.rep_hour_ENDTime.Name = "rep_hour_ENDTime";
		this.rep_cfl_Project.AutoHeight = false;
		this.rep_cfl_Project.AutoLoadDataSource = true;
		editorButtonImageOptions10.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions14.Image");
		this.rep_cfl_Project.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions10, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance19, appearanceHovered19, appearancePressed19, appearanceDisabled19, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions10, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance20, appearanceHovered20, appearancePressed20, appearanceDisabled20, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_Project.ContainerControl = this;
		this.rep_cfl_Project.DisplayMember = "";
		this.rep_cfl_Project.DisplayMember2 = "";
		this.rep_cfl_Project.LoadDataSourceWhenEnter = false;
		this.rep_cfl_Project.Name = "rep_cfl_Project";
		this.rep_cfl_Project.SeparateCharacterForMulti = ';';
		this.rep_cfl_Project.SeparateString = ", ";
		this.rep_cfl_Project.UsingAutoCompleteBox = false;
		this.rep_cfl_Project.UsingDataSource = true;
		this.rep_cfl_Project.ValidateWhenNoDataSource = false;
		this.rep_cfl_Project.ValueMember = "";
		this.xtraTabPage3.Controls.Add(this.btn_NewSolution);
		this.xtraTabPage3.Controls.Add(this.grd_Solution);
		this.xtraTabPage3.EditControl = null;
		this.xtraTabPage3.EditValue = "Related  Solutions";
		this.xtraTabPage3.FieldName = null;
		this.xtraTabPage3.IsSearchControl = false;
		this.xtraTabPage3.Name = "xtraTabPage3";
		this.xtraTabPage3.ReadOnly = false;
		this.xtraTabPage3.SettingInVisible = false;
		this.xtraTabPage3.SettingReadonly = false;
		this.xtraTabPage3.Size = new System.Drawing.Size(1230, 116);
		this.xtraTabPage3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage3.Text = "Related  Solutions";
		this.btn_NewSolution.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_NewSolution.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_NewSolution.Location = new System.Drawing.Point(1043, 22);
		this.btn_NewSolution.Name = "btn_NewSolution";
		this.btn_NewSolution.Size = new System.Drawing.Size(110, 25);
		this.btn_NewSolution.TabIndex = 5;
		this.btn_NewSolution.Text = "New Solution";
		this.btn_NewSolution.Click += new System.EventHandler(this.btn_NewSolution_Click);
		this.grd_Solution.AllowPasteExcel = false;
		this.grd_Solution.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Solution.ClipboardData = null;
		this.grd_Solution.ContextMenuStrip = this.ctx_Solution;
		this.grd_Solution.Location = new System.Drawing.Point(-1, 0);
		this.grd_Solution.MainView = this.gv_Solution;
		this.grd_Solution.Name = "grd_Solution";
		this.grd_Solution.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_cfl_Owner, this.rep_cfl_SltCode, this.rep_memx_SltSubject });
		this.grd_Solution.Size = new System.Drawing.Size(970, 114);
		this.grd_Solution.TabIndex = 4;
		this.grd_Solution.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Solution });
		this.ctx_Solution.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.ctx_Solution.Name = "ctx_Solution";
		this.ctx_Solution.Size = new System.Drawing.Size(118, 26);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_Solution.Appearance.FocusedRow.BackColor = System.Drawing.SystemColors.ActiveBorder;
		this.gv_Solution.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Solution.Appearance.Row.BackColor = System.Drawing.SystemColors.ActiveBorder;
		this.gv_Solution.Appearance.Row.Options.UseBackColor = true;
		this.gv_Solution.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Solution.ColumnKey = null;
		this.gv_Solution.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx32, this.gridColumnEx33, this.gridColumnEx34, this.gridColumnEx35, this.gridColumnEx36, this.gridColumnEx37 });
		this.gv_Solution.DisableFormSetting = false;
		this.gv_Solution.DisplayException = false;
		this.gv_Solution.EditColumn = null;
		this.gv_Solution.FieldCurrency = null;
		this.gv_Solution.FieldRate = null;
		this.gv_Solution.FormId = "oServiceCallsManagement";
		this.gv_Solution.GridControl = this.grd_Solution;
		this.gv_Solution.IgnoreFilter = false;
		this.gv_Solution.IndexEditRow = -1;
		this.gv_Solution.IndicatorWidth = 35;
		this.gv_Solution.IsApzTable = false;
		this.gv_Solution.IsInsertNewRow = false;
		this.gv_Solution.IsLoadingData = false;
		this.gv_Solution.IsMappingContextAction = false;
		this.gv_Solution.ItemId = "OSLT";
		this.gv_Solution.LstColumnAllowBatch = null;
		this.gv_Solution.LstColumnNotAllowBatch = null;
		this.gv_Solution.LstColumnNotEditable = null;
		this.gv_Solution.LstColumNotSetting = null;
		this.gv_Solution.Name = "gv_Solution";
		this.gv_Solution.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Solution.OptionsBehavior.ReadOnly = true;
		this.gv_Solution.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gv_Solution.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.gv_Solution.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Solution.OptionsView.ColumnAutoWidth = false;
		this.gv_Solution.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Solution.OptionsView.ShowGroupPanel = false;
		this.gv_Solution.UseBatchMod = false;
		this.gridColumnEx32.Caption = "No.";
		this.gridColumnEx32.ColumnEdit = this.rep_cfl_SltCode;
		this.gridColumnEx32.CurrencyDateField = null;
		this.gridColumnEx32.CurrencyField = null;
		this.gridColumnEx32.CurrencyFixed = null;
		this.gridColumnEx32.CurrencyFixedRate = 0.0;
		this.gridColumnEx32.CurrencyRateField = null;
		this.gridColumnEx32.FieldName = "solutionID";
		this.gridColumnEx32.Name = "gridColumnEx32";
		this.gridColumnEx32.PrimaryKey = false;
		this.gridColumnEx32.UnitField = null;
		this.gridColumnEx32.Visible = true;
		this.gridColumnEx32.VisibleIndex = 0;
		this.gridColumnEx32.Width = 68;
		this.rep_cfl_SltCode.AutoHeight = false;
		this.rep_cfl_SltCode.AutoLoadDataSource = true;
		editorButtonImageOptions11.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions16.Image");
		this.rep_cfl_SltCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions11, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance21, appearanceHovered21, appearancePressed21, appearanceDisabled21, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions11, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance22, appearanceHovered22, appearancePressed22, appearanceDisabled22, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_SltCode.ContainerControl = this;
		this.rep_cfl_SltCode.DisplayMember = "";
		this.rep_cfl_SltCode.DisplayMember2 = "";
		this.rep_cfl_SltCode.LoadDataSourceWhenEnter = false;
		this.rep_cfl_SltCode.Name = "rep_cfl_SltCode";
		this.rep_cfl_SltCode.SeparateCharacterForMulti = ';';
		this.rep_cfl_SltCode.SeparateString = ", ";
		this.rep_cfl_SltCode.UsingAutoCompleteBox = false;
		this.rep_cfl_SltCode.UsingDataSource = true;
		this.rep_cfl_SltCode.ValidateWhenNoDataSource = false;
		this.rep_cfl_SltCode.ValueMember = "";
		this.gridColumnEx33.Caption = "Solution";
		this.gridColumnEx33.ColumnEdit = this.rep_memx_SltSubject;
		this.gridColumnEx33.CurrencyDateField = null;
		this.gridColumnEx33.CurrencyField = null;
		this.gridColumnEx33.CurrencyFixed = null;
		this.gridColumnEx33.CurrencyFixedRate = 0.0;
		this.gridColumnEx33.CurrencyRateField = null;
		this.gridColumnEx33.FieldName = "Subject";
		this.gridColumnEx33.Name = "gridColumnEx33";
		this.gridColumnEx33.PrimaryKey = false;
		this.gridColumnEx33.UnitField = null;
		this.gridColumnEx33.Visible = true;
		this.gridColumnEx33.VisibleIndex = 1;
		this.gridColumnEx33.Width = 308;
		this.rep_memx_SltSubject.AutoHeight = false;
		this.rep_memx_SltSubject.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_memx_SltSubject.Name = "rep_memx_SltSubject";
		this.rep_memx_SltSubject.ShowIcon = false;
		this.gridColumnEx34.Caption = "Create on";
		this.gridColumnEx34.CurrencyDateField = null;
		this.gridColumnEx34.CurrencyField = null;
		this.gridColumnEx34.CurrencyFixed = null;
		this.gridColumnEx34.CurrencyFixedRate = 0.0;
		this.gridColumnEx34.CurrencyRateField = null;
		this.gridColumnEx34.FieldName = "createDate";
		this.gridColumnEx34.Name = "gridColumnEx34";
		this.gridColumnEx34.PrimaryKey = false;
		this.gridColumnEx34.UnitField = null;
		this.gridColumnEx34.Visible = true;
		this.gridColumnEx34.VisibleIndex = 2;
		this.gridColumnEx34.Width = 92;
		this.gridColumnEx35.Caption = "Owner";
		this.gridColumnEx35.ColumnEdit = this.rep_cfl_Owner;
		this.gridColumnEx35.CurrencyDateField = null;
		this.gridColumnEx35.CurrencyField = null;
		this.gridColumnEx35.CurrencyFixed = null;
		this.gridColumnEx35.CurrencyFixedRate = 0.0;
		this.gridColumnEx35.CurrencyRateField = null;
		this.gridColumnEx35.FieldName = "Owner";
		this.gridColumnEx35.Name = "gridColumnEx35";
		this.gridColumnEx35.PrimaryKey = false;
		this.gridColumnEx35.UnitField = null;
		this.gridColumnEx35.Visible = true;
		this.gridColumnEx35.VisibleIndex = 3;
		this.gridColumnEx35.Width = 107;
		this.rep_cfl_Owner.AutoHeight = false;
		this.rep_cfl_Owner.AutoLoadDataSource = true;
		editorButtonImageOptions12.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions18.Image");
		this.rep_cfl_Owner.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions12, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance23, appearanceHovered23, appearancePressed23, appearanceDisabled23, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions12, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance24, appearanceHovered24, appearancePressed24, appearanceDisabled24, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_Owner.ContainerControl = this;
		this.rep_cfl_Owner.DisplayMember = "";
		this.rep_cfl_Owner.DisplayMember2 = "";
		this.rep_cfl_Owner.LoadDataSourceWhenEnter = false;
		this.rep_cfl_Owner.Name = "rep_cfl_Owner";
		this.rep_cfl_Owner.SeparateCharacterForMulti = ';';
		this.rep_cfl_Owner.SeparateString = ", ";
		this.rep_cfl_Owner.UsingAutoCompleteBox = false;
		this.rep_cfl_Owner.UsingDataSource = true;
		this.rep_cfl_Owner.ValidateWhenNoDataSource = false;
		this.rep_cfl_Owner.ValueMember = "";
		this.gridColumnEx36.Caption = "Handle by";
		this.gridColumnEx36.ColumnEdit = this.rep_cfl_Owner;
		this.gridColumnEx36.CurrencyDateField = null;
		this.gridColumnEx36.CurrencyField = null;
		this.gridColumnEx36.CurrencyFixed = null;
		this.gridColumnEx36.CurrencyFixedRate = 0.0;
		this.gridColumnEx36.CurrencyRateField = null;
		this.gridColumnEx36.FieldName = "userSign2";
		this.gridColumnEx36.Name = "gridColumnEx36";
		this.gridColumnEx36.PrimaryKey = false;
		this.gridColumnEx36.UnitField = null;
		this.gridColumnEx36.Visible = true;
		this.gridColumnEx36.VisibleIndex = 5;
		this.gridColumnEx36.Width = 116;
		this.gridColumnEx37.Caption = "Status";
		this.gridColumnEx37.ColumnEdit = this.rep_luk_SolutionStatus;
		this.gridColumnEx37.CurrencyDateField = null;
		this.gridColumnEx37.CurrencyField = null;
		this.gridColumnEx37.CurrencyFixed = null;
		this.gridColumnEx37.CurrencyFixedRate = 0.0;
		this.gridColumnEx37.CurrencyRateField = null;
		this.gridColumnEx37.FieldName = "StatusNum";
		this.gridColumnEx37.Name = "gridColumnEx37";
		this.gridColumnEx37.PrimaryKey = false;
		this.gridColumnEx37.UnitField = null;
		this.gridColumnEx37.Visible = true;
		this.gridColumnEx37.VisibleIndex = 4;
		this.gridColumnEx37.Width = 76;
		this.rep_luk_SolutionStatus.AutoHeight = false;
		this.rep_luk_SolutionStatus.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.rep_luk_SolutionStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_SolutionStatus.CheckFields = null;
		this.rep_luk_SolutionStatus.CheckTables = null;
		this.rep_luk_SolutionStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name4")
		});
		this.rep_luk_SolutionStatus.Name = "rep_luk_SolutionStatus";
		this.rep_luk_SolutionStatus.NullText = "";
		this.rep_luk_SolutionStatus.ShowFooter = false;
		this.rep_luk_SolutionStatus.ShowHeader = false;
		this.xtraTabPage4.Controls.Add(this.btn_APInvoice);
		this.xtraTabPage4.Controls.Add(this.btn_GoodReceiptPO);
		this.xtraTabPage4.Controls.Add(this.btn_GoodReturn);
		this.xtraTabPage4.Controls.Add(this.btn_ARInvoice);
		this.xtraTabPage4.Controls.Add(this.btn_Delivery);
		this.xtraTabPage4.Controls.Add(this.btn_Return);
		this.xtraTabPage4.Controls.Add(this.grd_DocLine);
		this.xtraTabPage4.EditControl = null;
		this.xtraTabPage4.EditValue = "Related Documents";
		this.xtraTabPage4.FieldName = null;
		this.xtraTabPage4.IsSearchControl = false;
		this.xtraTabPage4.Name = "xtraTabPage4";
		this.xtraTabPage4.ReadOnly = false;
		this.xtraTabPage4.SettingInVisible = false;
		this.xtraTabPage4.SettingReadonly = false;
		this.xtraTabPage4.Size = new System.Drawing.Size(1230, 116);
		this.xtraTabPage4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage4.Text = "Related Documents";
		this.btn_APInvoice.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_APInvoice.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_APInvoice.Location = new System.Drawing.Point(1108, 80);
		this.btn_APInvoice.Name = "btn_APInvoice";
		this.btn_APInvoice.Size = new System.Drawing.Size(110, 24);
		this.btn_APInvoice.TabIndex = 11;
		this.btn_APInvoice.Text = "AP Invoice";
		this.btn_APInvoice.Click += new System.EventHandler(this.btn_APInvoice_Click);
		this.btn_GoodReceiptPO.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_GoodReceiptPO.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_GoodReceiptPO.Location = new System.Drawing.Point(1108, 48);
		this.btn_GoodReceiptPO.Name = "btn_GoodReceiptPO";
		this.btn_GoodReceiptPO.Size = new System.Drawing.Size(110, 24);
		this.btn_GoodReceiptPO.TabIndex = 10;
		this.btn_GoodReceiptPO.Text = "Good Receipt PO";
		this.btn_GoodReceiptPO.Click += new System.EventHandler(this.btn_GoodReceipt_Click);
		this.btn_GoodReturn.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_GoodReturn.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_GoodReturn.Location = new System.Drawing.Point(1108, 17);
		this.btn_GoodReturn.Name = "btn_GoodReturn";
		this.btn_GoodReturn.Size = new System.Drawing.Size(110, 24);
		this.btn_GoodReturn.TabIndex = 9;
		this.btn_GoodReturn.Text = "Good Return";
		this.btn_GoodReturn.Click += new System.EventHandler(this.btn_GoodReturn_Click);
		this.btn_ARInvoice.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ARInvoice.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_ARInvoice.Location = new System.Drawing.Point(991, 80);
		this.btn_ARInvoice.Name = "btn_ARInvoice";
		this.btn_ARInvoice.Size = new System.Drawing.Size(110, 24);
		this.btn_ARInvoice.TabIndex = 8;
		this.btn_ARInvoice.Text = "AR Invoice";
		this.btn_ARInvoice.Click += new System.EventHandler(this.btn_ARInvoice_Click);
		this.btn_Delivery.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Delivery.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Delivery.Location = new System.Drawing.Point(991, 48);
		this.btn_Delivery.Name = "btn_Delivery";
		this.btn_Delivery.Size = new System.Drawing.Size(110, 24);
		this.btn_Delivery.TabIndex = 7;
		this.btn_Delivery.Text = "Delivery";
		this.btn_Delivery.Click += new System.EventHandler(this.btn_Delivery_Click);
		this.btn_Return.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Return.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Return.Location = new System.Drawing.Point(991, 17);
		this.btn_Return.Name = "btn_Return";
		this.btn_Return.Size = new System.Drawing.Size(110, 24);
		this.btn_Return.TabIndex = 6;
		this.btn_Return.Text = "Return";
		this.btn_Return.Click += new System.EventHandler(this.btn_Return_Click);
		this.grd_DocLine.AllowPasteExcel = false;
		this.grd_DocLine.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_DocLine.ClipboardData = null;
		this.grd_DocLine.ContextMenuStrip = this.ctx_DocLine;
		this.grd_DocLine.Location = new System.Drawing.Point(-1, 0);
		this.grd_DocLine.MainView = this.gv_DocLine;
		this.grd_DocLine.Name = "grd_DocLine";
		this.grd_DocLine.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_cfl_DocNum, this.rep_luk_ObjType, this.rep_luk_DocLineStatus });
		this.grd_DocLine.Size = new System.Drawing.Size(971, 115);
		this.grd_DocLine.TabIndex = 5;
		this.grd_DocLine.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_DocLine });
		this.ctx_DocLine.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem2 });
		this.ctx_DocLine.Name = "ctx_DocLine";
		this.ctx_DocLine.Size = new System.Drawing.Size(118, 26);
		this.removeToolStripMenuItem2.Name = "removeToolStripMenuItem2";
		this.removeToolStripMenuItem2.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem2.Text = "Remove";
		this.removeToolStripMenuItem2.Click += new System.EventHandler(this.removeToolStripMenuItem2_Click);
		this.gv_DocLine.Appearance.FocusedRow.BackColor = System.Drawing.SystemColors.ActiveBorder;
		this.gv_DocLine.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_DocLine.Appearance.Row.BackColor = System.Drawing.SystemColors.ActiveBorder;
		this.gv_DocLine.Appearance.Row.Options.UseBackColor = true;
		this.gv_DocLine.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_DocLine.ColumnKey = null;
		this.gv_DocLine.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gridColumnEx38, this.gridColumnEx40, this.gridColumnEx39, this.gridColumnEx28, this.gridColumnEx31, this.gridColumnEx41, this.gridColumnEx42, this.gridColumnEx43 });
		this.gv_DocLine.DisableFormSetting = false;
		this.gv_DocLine.DisplayException = false;
		this.gv_DocLine.EditColumn = null;
		this.gv_DocLine.FieldCurrency = null;
		this.gv_DocLine.FieldRate = null;
		this.gv_DocLine.FormId = "oServiceCallsManagement";
		this.gv_DocLine.GridControl = this.grd_DocLine;
		this.gv_DocLine.IgnoreFilter = false;
		this.gv_DocLine.IndexEditRow = -1;
		this.gv_DocLine.IndicatorWidth = 35;
		this.gv_DocLine.IsApzTable = false;
		this.gv_DocLine.IsInsertNewRow = false;
		this.gv_DocLine.IsLoadingData = false;
		this.gv_DocLine.IsMappingContextAction = false;
		this.gv_DocLine.ItemId = "OLDC";
		this.gv_DocLine.LstColumnAllowBatch = null;
		this.gv_DocLine.LstColumnNotAllowBatch = null;
		this.gv_DocLine.LstColumnNotEditable = null;
		this.gv_DocLine.LstColumNotSetting = null;
		this.gv_DocLine.Name = "gv_DocLine";
		this.gv_DocLine.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_DocLine.OptionsBehavior.ReadOnly = true;
		this.gv_DocLine.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gv_DocLine.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.gv_DocLine.OptionsView.BestFitMaxRowCount = 100;
		this.gv_DocLine.OptionsView.ColumnAutoWidth = false;
		this.gv_DocLine.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_DocLine.OptionsView.ShowGroupPanel = false;
		this.gv_DocLine.UseBatchMod = false;
		this.gridColumnEx38.Caption = "Document Type";
		this.gridColumnEx38.ColumnEdit = this.rep_luk_ObjType;
		this.gridColumnEx38.CurrencyDateField = null;
		this.gridColumnEx38.CurrencyField = null;
		this.gridColumnEx38.CurrencyFixed = null;
		this.gridColumnEx38.CurrencyFixedRate = 0.0;
		this.gridColumnEx38.CurrencyRateField = null;
		this.gridColumnEx38.FieldName = "ObjType";
		this.gridColumnEx38.Name = "gridColumnEx38";
		this.gridColumnEx38.PrimaryKey = false;
		this.gridColumnEx38.UnitField = null;
		this.gridColumnEx38.Visible = true;
		this.gridColumnEx38.VisibleIndex = 0;
		this.gridColumnEx38.Width = 98;
		this.rep_luk_ObjType.AutoHeight = false;
		this.rep_luk_ObjType.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_luk_ObjType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_ObjType.IsSearchControl = false;
		this.rep_luk_ObjType.Name = "rep_luk_ObjType";
		this.rep_luk_ObjType.NullText = "";
		this.rep_luk_ObjType.ShowFooter = false;
		this.rep_luk_ObjType.ShowHeader = false;
		this.gridColumnEx40.Caption = "Document No.";
		this.gridColumnEx40.ColumnEdit = this.rep_cfl_DocNum;
		this.gridColumnEx40.CurrencyDateField = null;
		this.gridColumnEx40.CurrencyField = null;
		this.gridColumnEx40.CurrencyFixed = null;
		this.gridColumnEx40.CurrencyFixedRate = 0.0;
		this.gridColumnEx40.CurrencyRateField = null;
		this.gridColumnEx40.FieldName = "DocNum";
		this.gridColumnEx40.Name = "gridColumnEx40";
		this.gridColumnEx40.PrimaryKey = false;
		this.gridColumnEx40.UnitField = null;
		this.gridColumnEx40.Visible = true;
		this.gridColumnEx40.VisibleIndex = 1;
		this.gridColumnEx40.Width = 82;
		this.rep_cfl_DocNum.AutoHeight = false;
		this.rep_cfl_DocNum.AutoLoadDataSource = true;
		editorButtonImageOptions13.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions20.Image");
		this.rep_cfl_DocNum.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions13, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance25, appearanceHovered25, appearancePressed25, appearanceDisabled25, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions13, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance26, appearanceHovered26, appearancePressed26, appearanceDisabled26, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_DocNum.ContainerControl = this;
		this.rep_cfl_DocNum.DisplayMember = "";
		this.rep_cfl_DocNum.DisplayMember2 = "";
		this.rep_cfl_DocNum.LoadDataSourceWhenEnter = false;
		this.rep_cfl_DocNum.Name = "rep_cfl_DocNum";
		this.rep_cfl_DocNum.SeparateCharacterForMulti = ';';
		this.rep_cfl_DocNum.SeparateString = ", ";
		this.rep_cfl_DocNum.UsingAutoCompleteBox = false;
		this.rep_cfl_DocNum.UsingDataSource = true;
		this.rep_cfl_DocNum.ValidateWhenNoDataSource = false;
		this.rep_cfl_DocNum.ValueMember = "";
		this.gridColumnEx39.Caption = "Posting Date";
		this.gridColumnEx39.CurrencyDateField = null;
		this.gridColumnEx39.CurrencyField = null;
		this.gridColumnEx39.CurrencyFixed = null;
		this.gridColumnEx39.CurrencyFixedRate = 0.0;
		this.gridColumnEx39.CurrencyRateField = null;
		this.gridColumnEx39.FieldName = "DocDate";
		this.gridColumnEx39.Name = "gridColumnEx39";
		this.gridColumnEx39.PrimaryKey = false;
		this.gridColumnEx39.UnitField = null;
		this.gridColumnEx39.Visible = true;
		this.gridColumnEx39.VisibleIndex = 3;
		this.gridColumnEx39.Width = 89;
		this.gridColumnEx28.Caption = "Due Date";
		this.gridColumnEx28.CurrencyDateField = null;
		this.gridColumnEx28.CurrencyField = null;
		this.gridColumnEx28.CurrencyFixed = null;
		this.gridColumnEx28.CurrencyFixedRate = 0.0;
		this.gridColumnEx28.CurrencyRateField = null;
		this.gridColumnEx28.FieldName = "DocDueDate";
		this.gridColumnEx28.Name = "gridColumnEx28";
		this.gridColumnEx28.PrimaryKey = false;
		this.gridColumnEx28.UnitField = null;
		this.gridColumnEx28.Visible = true;
		this.gridColumnEx28.VisibleIndex = 2;
		this.gridColumnEx28.Width = 91;
		this.gridColumnEx31.Caption = "Document Date";
		this.gridColumnEx31.CurrencyDateField = null;
		this.gridColumnEx31.CurrencyField = null;
		this.gridColumnEx31.CurrencyFixed = null;
		this.gridColumnEx31.CurrencyFixedRate = 0.0;
		this.gridColumnEx31.CurrencyRateField = null;
		this.gridColumnEx31.FieldName = "TaxDate";
		this.gridColumnEx31.Name = "gridColumnEx31";
		this.gridColumnEx31.PrimaryKey = false;
		this.gridColumnEx31.UnitField = null;
		this.gridColumnEx31.Visible = true;
		this.gridColumnEx31.VisibleIndex = 4;
		this.gridColumnEx31.Width = 91;
		this.gridColumnEx41.Caption = "Doc Total";
		this.gridColumnEx41.CurrencyDateField = null;
		this.gridColumnEx41.CurrencyField = null;
		this.gridColumnEx41.CurrencyFixed = null;
		this.gridColumnEx41.CurrencyFixedRate = 0.0;
		this.gridColumnEx41.CurrencyRateField = null;
		this.gridColumnEx41.FieldName = "DocTotal";
		this.gridColumnEx41.Name = "gridColumnEx41";
		this.gridColumnEx41.PrimaryKey = false;
		this.gridColumnEx41.UnitField = null;
		this.gridColumnEx41.Visible = true;
		this.gridColumnEx41.VisibleIndex = 5;
		this.gridColumnEx41.Width = 79;
		this.gridColumnEx42.Caption = "Doc Status";
		this.gridColumnEx42.ColumnEdit = this.rep_luk_DocLineStatus;
		this.gridColumnEx42.CurrencyDateField = null;
		this.gridColumnEx42.CurrencyField = null;
		this.gridColumnEx42.CurrencyFixed = null;
		this.gridColumnEx42.CurrencyFixedRate = 0.0;
		this.gridColumnEx42.CurrencyRateField = null;
		this.gridColumnEx42.Name = "gridColumnEx42";
		this.gridColumnEx42.PrimaryKey = false;
		this.gridColumnEx42.UnitField = null;
		this.gridColumnEx42.Visible = true;
		this.gridColumnEx42.VisibleIndex = 6;
		this.rep_luk_DocLineStatus.AutoHeight = false;
		this.rep_luk_DocLineStatus.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_luk_DocLineStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_DocLineStatus.IsSearchControl = false;
		this.rep_luk_DocLineStatus.Name = "rep_luk_DocLineStatus";
		this.rep_luk_DocLineStatus.NullText = "";
		this.rep_luk_DocLineStatus.ShowFooter = false;
		this.rep_luk_DocLineStatus.ShowHeader = false;
		this.gridColumnEx43.Caption = "Remark";
		this.gridColumnEx43.CurrencyDateField = null;
		this.gridColumnEx43.CurrencyField = null;
		this.gridColumnEx43.CurrencyFixed = null;
		this.gridColumnEx43.CurrencyFixedRate = 0.0;
		this.gridColumnEx43.CurrencyRateField = null;
		this.gridColumnEx43.FieldName = "Comments";
		this.gridColumnEx43.Name = "gridColumnEx43";
		this.gridColumnEx43.PrimaryKey = false;
		this.gridColumnEx43.UnitField = null;
		this.gridColumnEx43.Visible = true;
		this.gridColumnEx43.VisibleIndex = 7;
		this.gridColumnEx43.Width = 181;
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(88, 440);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 15;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(1, 440);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(81, 23);
		this.btnOK.TabIndex = 14;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 5);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.ccb_Tags);
		this.splitContainerControl1.Panel1.Controls.Add(this.luk_CallType);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine7);
		this.splitContainerControl1.Panel1.Controls.Add(this.cfl_HandleBy);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine4);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine6);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine5);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine2);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine3);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine1);
		this.splitContainerControl1.Panel1.Controls.Add(this.txt_CallId);
		this.splitContainerControl1.Panel1.Controls.Add(this.cfl_BPCode);
		this.splitContainerControl1.Panel1.Controls.Add(this.dte_ToDate);
		this.splitContainerControl1.Panel1.Controls.Add(this.dte_FromDate);
		this.splitContainerControl1.Panel1.Controls.Add(this.btn_Find);
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControlEx1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1233, 433);
		this.splitContainerControl1.SplitterPosition = 146;
		this.splitContainerControl1.TabIndex = 16;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.ccb_Tags.EditControl = null;
		this.ccb_Tags.FieldName = null;
		this.ccb_Tags.IsSearchControl = true;
		this.ccb_Tags.Location = new System.Drawing.Point(665, 38);
		this.ccb_Tags.Name = "ccb_Tags";
		this.ccb_Tags.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ccb_Tags.Properties.CheckFields = null;
		this.ccb_Tags.Properties.CheckTables = null;
		this.ccb_Tags.Properties.DisableDefineNew = false;
		this.ccb_Tags.Properties.IncrementalSearch = true;
		this.ccb_Tags.SettingInVisible = false;
		this.ccb_Tags.SettingReadonly = false;
		this.ccb_Tags.Size = new System.Drawing.Size(103, 20);
		this.ccb_Tags.TabIndex = 10;
		this.luk_CallType.EditControl = null;
		this.luk_CallType.FieldName = null;
		this.luk_CallType.IsSearchControl = true;
		this.luk_CallType.Location = new System.Drawing.Point(281, 41);
		this.luk_CallType.Name = "luk_CallType";
		this.luk_CallType.Properties.AutoHeight = false;
		this.luk_CallType.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_CallType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_CallType.Properties.IsSearchControl = false;
		this.luk_CallType.Properties.NullText = "";
		this.luk_CallType.Properties.ShowFooter = false;
		this.luk_CallType.Properties.ShowHeader = false;
		this.luk_CallType.SettingInVisible = false;
		this.luk_CallType.SettingReadonly = false;
		this.luk_CallType.Size = new System.Drawing.Size(109, 20);
		this.luk_CallType.TabIndex = 6;
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Handle by";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(405, 11);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(80, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 9;
		this.labelLine7.Utext = "Handle by";
		this.cfl_HandleBy.EditControl = null;
		this.cfl_HandleBy.FieldName = null;
		this.cfl_HandleBy.IsSearchControl = true;
		this.cfl_HandleBy.Location = new System.Drawing.Point(491, 8);
		this.cfl_HandleBy.Name = "cfl_HandleBy";
		this.cfl_HandleBy.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions14.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.cfl_HandleBy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions14, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance27, appearanceHovered27, appearancePressed27, appearanceDisabled27, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions14, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance28, appearanceHovered28, appearancePressed28, appearanceDisabled28, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_HandleBy.Properties.ContainerControl = null;
		this.cfl_HandleBy.Properties.DisplayMember = "";
		this.cfl_HandleBy.Properties.DisplayMember2 = "";
		this.cfl_HandleBy.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_HandleBy.Properties.SeparateCharacterForMulti = ';';
		this.cfl_HandleBy.Properties.SeparateString = ", ";
		this.cfl_HandleBy.Properties.UsingAutoCompleteBox = false;
		this.cfl_HandleBy.Properties.UsingDataSource = true;
		this.cfl_HandleBy.Properties.ValidateWhenNoDataSource = false;
		this.cfl_HandleBy.Properties.ValueMember = "";
		this.cfl_HandleBy.SettingInVisible = false;
		this.cfl_HandleBy.SettingReadonly = false;
		this.cfl_HandleBy.Size = new System.Drawing.Size(109, 20);
		this.cfl_HandleBy.TabIndex = 8;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "No.";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(405, 41);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(52, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 7;
		this.labelLine4.Utext = "No.";
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Type";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(214, 41);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(80, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 7;
		this.labelLine6.Utext = "Type";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Tags";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(605, 38);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 7;
		this.labelLine5.Utext = "Tags";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "To date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(214, 8);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(62, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 7;
		this.labelLine2.Utext = "To date";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Business Partner";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(11, 43);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 7;
		this.labelLine3.Utext = "Business Partner";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "From date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 7);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 7;
		this.labelLine1.Utext = "From date";
		this.txt_CallId.EditControl = null;
		this.txt_CallId.FieldName = null;
		this.txt_CallId.IsSearchControl = false;
		this.txt_CallId.Location = new System.Drawing.Point(490, 38);
		this.txt_CallId.Name = "txt_CallId";
		this.txt_CallId.SettingInVisible = false;
		this.txt_CallId.SettingReadonly = false;
		this.txt_CallId.Size = new System.Drawing.Size(109, 20);
		this.txt_CallId.TabIndex = 4;
		this.cfl_BPCode.EditControl = null;
		this.cfl_BPCode.FieldName = null;
		this.cfl_BPCode.IsSearchControl = true;
		this.cfl_BPCode.Location = new System.Drawing.Point(99, 39);
		this.cfl_BPCode.Name = "cfl_BPCode";
		this.cfl_BPCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions15.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.cfl_BPCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions15, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance29, appearanceHovered29, appearancePressed29, appearanceDisabled29, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions15, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance30, appearanceHovered30, appearancePressed30, appearanceDisabled30, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_BPCode.Properties.ContainerControl = null;
		this.cfl_BPCode.Properties.DisplayMember = "";
		this.cfl_BPCode.Properties.DisplayMember2 = "";
		this.cfl_BPCode.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_BPCode.Properties.SeparateCharacterForMulti = ';';
		this.cfl_BPCode.Properties.SeparateString = ", ";
		this.cfl_BPCode.Properties.UsingAutoCompleteBox = false;
		this.cfl_BPCode.Properties.UsingDataSource = true;
		this.cfl_BPCode.Properties.ValidateWhenNoDataSource = false;
		this.cfl_BPCode.Properties.ValueMember = "";
		this.cfl_BPCode.SettingInVisible = false;
		this.cfl_BPCode.SettingReadonly = false;
		this.cfl_BPCode.Size = new System.Drawing.Size(109, 20);
		this.cfl_BPCode.TabIndex = 3;
		this.dte_ToDate.EditControl = null;
		this.dte_ToDate.EditValue = null;
		this.dte_ToDate.FieldName = null;
		this.dte_ToDate.Location = new System.Drawing.Point(281, 8);
		this.dte_ToDate.Name = "dte_ToDate";
		this.dte_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dte_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dte_ToDate.Properties.IsSearchControl = true;
		this.dte_ToDate.SettingInVisible = false;
		this.dte_ToDate.SettingReadonly = false;
		this.dte_ToDate.Size = new System.Drawing.Size(109, 20);
		this.dte_ToDate.TabIndex = 2;
		this.dte_ToDate.Validating += new System.ComponentModel.CancelEventHandler(this.dte_ToDate_Validating);
		this.dte_FromDate.EditControl = null;
		this.dte_FromDate.EditValue = null;
		this.dte_FromDate.FieldName = null;
		this.dte_FromDate.Location = new System.Drawing.Point(98, 7);
		this.dte_FromDate.Name = "dte_FromDate";
		this.dte_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dte_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dte_FromDate.Properties.IsSearchControl = true;
		this.dte_FromDate.SettingInVisible = false;
		this.dte_FromDate.SettingReadonly = false;
		this.dte_FromDate.Size = new System.Drawing.Size(110, 20);
		this.dte_FromDate.TabIndex = 2;
		this.btn_Find.EditControl = null;
		this.btn_Find.EditValue = "Find";
		this.btn_Find.FieldName = null;
		this.btn_Find.IsSearchControl = true;
		this.btn_Find.Location = new System.Drawing.Point(793, 38);
		this.btn_Find.Name = "btn_Find";
		this.btn_Find.ReadOnly = false;
		this.btn_Find.SettingInVisible = false;
		this.btn_Find.SettingReadonly = false;
		this.btn_Find.Size = new System.Drawing.Size(75, 23);
		this.btn_Find.TabIndex = 1;
		this.btn_Find.Text = "Find";
		this.btn_Find.Click += new System.EventHandler(this.btn_Find_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1233, 473);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btnOK);
		base.FormId = "oServiceCallsManagement";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ServiceCalls";
		base.ShowInTaskbar = false;
		this.Text = "Service Call";
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctx_ServiceCalls.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_BPCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Contact).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_SerialNum).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_CallType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Orgin).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ccb_Tags).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_HandleBy).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Priority).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_memx_Resolution).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_BPCode_Vendor).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_ItemCode_CardCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).EndInit();
		this.xtraTabControlEx1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Issue).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Issue).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_IssueId).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_IssueType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_IssueHandleBy).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_IssueStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_memx_IssueNote).EndInit();
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Task).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Task).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_TaskDetails).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_hour_ENDTime).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_Project).EndInit();
		this.xtraTabPage3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Solution).EndInit();
		this.ctx_Solution.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Solution).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_SltCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_memx_SltSubject).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_Owner).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_SolutionStatus).EndInit();
		this.xtraTabPage4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_DocLine).EndInit();
		this.ctx_DocLine.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_DocLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_ObjType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_DocNum).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_DocLineStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.ccb_Tags.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CallType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_HandleBy.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CallId.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_BPCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dte_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dte_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dte_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dte_FromDate.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
