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
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;

namespace Apzon.Forms.Administrator.ApprovalProcess;

public class frm_ApprovalTemplates : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private int _countTearms;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private XtraTabControlEx tabControl_Main;

	private XtraTabPageEx tabPage_Originator;

	private GridControlEx grd_Originator;

	private GridViewEx gv_Originator;

	private XtraTabPageEx tabPage_Documents;

	private RadioGroupEx rd_Groups;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private XtraTabPageEx tabPage_Stages;

	private GridControlEx grd_Stages;

	private GridViewEx gv_Stages;

	private XtraTabPageEx tabPage_Terms;

	private RadioGroupEx rd_LaunchAP;

	private LabelLine lbl_LaunchAP;

	private PanelControl panelControl1;

	private GroupControl grp_Documents;

	private CheckEditEx chk_APCreditMemo;

	private CheckEditEx chk_ARCreditMemo;

	private CheckEditEx chk_APInvoice;

	private CheckEditEx chk_ARInvoice;

	private CheckEditEx chk_APDownPaymnt;

	private CheckEditEx chk_ARDownPaymnt;

	private CheckEditEx chk_GoodReturns;

	private CheckEditEx chk_Returns;

	private CheckEditEx chk_GoodReturnReqr;

	private CheckEditEx chk_ReturnReqr;

	private CheckEditEx chk_GoodReceiptPO;

	private CheckEditEx chk_Delivery;

	private CheckEditEx chk_PurchaseOrder;

	private CheckEditEx chk_SalesOrder;

	private CheckEditEx chk_PurchaseQuo;

	private CheckEditEx chk_SalesQuotations;

	private LabelLine labelLine5;

	private LabelLine labelLine4;

	private GroupControl grp_BlanketAgreement;

	private CheckEditEx chk_PurchaseBlanketAgrmnt;

	private CheckEditEx chk_SalesBlanketAgrmnt;

	private GroupControl grp_InternalRequis;

	private CheckEditEx chk_PurchaseReqr;

	private GroupControl grp_IVCoutingTrans;

	private GroupControl grp_Payment;

	private CheckEditEx chk_OutgoingPaymnt;

	private GroupControl grp_Inventory;

	private CheckEditEx chk_IVOpenBalance;

	private CheckEditEx chk_IVTransfer;

	private CheckEditEx chk_IVTransReqr;

	private CheckEditEx chk_GoodIssue;

	private CheckEditEx chk_GoodReceipt;

	private CheckEditEx chk_IVPosting;

	private CheckEditEx chk_IVCounting;

	private GridColumnEx gc_WtmCode;

	private GridColumnEx gc_UserId1;

	private GridColumnEx gc_Username;

	private GridColumnEx gc_DepartmntCode;

	private GridColumnEx gc_DepartmntName;

	private RepositoryItemButtonEdit rep_Originator;

	private RepositoryItemButtonEdit rep_Departmnt;

	private SimpleButtonEx btn_MoveUp;

	private GridColumnEx gc_WtmCode1;

	private GridColumnEx gc_WstCode;

	private GridColumnEx gc_WstName;

	private GridColumnEx gc_WstRemarks;

	private SimpleButtonEx btn_MoveDown;

	private RepositoryItemButtonEdit rep_Stages;

	private ContextMenuStrip contextMenuStrip3;

	private ToolStripMenuItem mnu_DeleteLine;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem mnu_AddDoc;

	private ToolStripMenuItem mnu_DeleteDoc;

	private ToolStripMenuItem mnu_DuplicateDoc;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem deleteToolStripMenuItem2;

	private GridColumnEx gc_SortID;

	private PanelControl panelControl2;

	private TextEditEx txt_Remarks;

	private TextEditEx txt_Name;

	private TextEditEx txt_Code;

	private CheckEditEx chk_ActiveUpdateDoc;

	private LabelLine lbl_Remarks;

	private LabelLine lbl_Name;

	private CheckEditEx chk_Active;

	private LabelLine lbl_Code;

	private PanelControl panelControl3;

	private SplitContainerControl splitContainerControl2;

	private GridControlEx grd_Terms;

	private GridViewEx gv_Terms;

	private LabelLine lbl_TotalTerms;

	private GridColumnEx gc_Choose;

	private RepositoryItemCheckEdit rep_Choose;

	private GridColumnEx gc_TermName;

	private GridColumnEx gc_Ratio;

	private RepositoryItemLookUpEdit rep_Ratio;

	private GridColumnEx gc_Value;

	private GridControlEx grd_Query;

	private GridViewEx gv_Query;

	private LabelLine labelLine2;

	private GridColumnEx gc_QueryId;

	private GridColumnEx gc_QueryName;

	private RepositoryItemButtonEdit rep_Query;

	private GridColumnEx gc_TermsId;

	private LabelControlEx lbl_Totals;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btn_OK.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Find:
			btn_OK.Text = Resources.ButtonFind;
			break;
		}
	}

	public frm_ApprovalTemplates()
	{
		InitializeComponent();
	}

	public async Task REFRESH()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			txt_Code.EditValue = _httpClient.ApprovalTemplates.GetNewCode();
			txt_Name.EditValue = string.Empty;
			txt_Remarks.EditValue = string.Empty;
			chk_Active.Checked = true;
			chk_ActiveUpdateDoc.Checked = false;
			txt_Name.Focus();
			AddNewOriginator(isNew: true, Function.ParseInt(txt_Code.EditValue));
			AddNewStages(isNew: true, Function.ParseInt(txt_Code.EditValue));
			AddNewQueries(isNew: true, Function.ParseInt(txt_Code.EditValue));
			rd_LaunchAP.SelectedIndex = 0;
			ClearDocumentControls();
			tabControl_Main.SelectedTabPageIndex = 0;
			FormMode = Enums.FormViewModeType.Add;
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
		try
		{
			WinCommons._OpenProcessing();
			await GetRecord(Enums.NavigateType.First, Function.ParseInt(txt_Code.Text));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			WinCommons._OpenProcessing();
			await GetRecord(Enums.NavigateType.Previous, Function.ParseInt(txt_Code.Text));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			WinCommons._OpenProcessing();
			await GetRecord(Enums.NavigateType.Next, Function.ParseInt(txt_Code.Text));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		try
		{
			WinCommons._OpenProcessing();
			await GetRecord(Enums.NavigateType.Last, Function.ParseInt(txt_Code.Text));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			frm_ApprovalTemplates_Load(null, null);
			rd_LaunchAP.Properties.Items[1].Enabled = true;
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
		try
		{
			if ((FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No)
			{
				ClearDataControls();
				txt_Name.Focus();
				FormMode = Enums.FormViewModeType.Find;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddTermsData(ref DataTable dtSource, int templateCode, Dictionary<int, string> dicData)
	{
		try
		{
			foreach (KeyValuePair<int, string> dicDatum in dicData)
			{
				DataRow dataRow = dtSource.NewRow();
				dataRow["WtmCode"] = templateCode;
				dataRow["Checked"] = "N";
				dataRow["CondId"] = Function.ParseInt(dicDatum.Key);
				dataRow["CondName"] = Function.ToString(dicDatum.Value);
				dataRow["opCode"] = 1;
				dtSource.Rows.Add(dataRow);
			}
			dtSource.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadTermsConditions(int documentIndex)
	{
		try
		{
			DataTable dtSource = WinFuntions.GetTableStructure("WTM4");
			dtSource.Columns.Add("Checked", typeof(string));
			dtSource.Columns.Add("CondName", typeof(string));
			Dictionary<int, string> dataSource = new Dictionary<int, string>
			{
				{ 1, "Greater Than" },
				{ 2, "Greater or Equal" },
				{ 3, "Less Than" },
				{ 4, "Less or Equal" },
				{ 5, "Equal" },
				{ 6, "Does not Equal" },
				{ 7, "In Range" },
				{ 8, "Not in Range" },
				{ 0, "Undefined Type" }
			};
			Dictionary<int, string> dicData = new Dictionary<int, string>
			{
				{ 1, "Deviation from Credit Limit" },
				{ 2, "Deviation from Commitment" },
				{ 3, "Gross Profit %" },
				{ 4, "Discount %" },
				{ 5, "Deviation from Budget" },
				{ 6, "Total Document" }
			};
			Dictionary<int, string> dicData2 = new Dictionary<int, string>
			{
				{ 7, "Quantity" },
				{ 8, "Item Code" },
				{ 9, "Total" },
				{ 6, "Total Document" }
			};
			Dictionary<int, string> dicData3 = new Dictionary<int, string> { { 4, "Total Document" } };
			Dictionary<int, string> dicData4 = new Dictionary<int, string>
			{
				{ 1, "Item Code" },
				{ 2, "Total" },
				{ 3, "Total Document" },
				{ 4, "Counted Quantity" },
				{ 5, "Variance" },
				{ 6, "Variance %" }
			};
			rep_Ratio.DataSource = dataSource;
			switch (documentIndex)
			{
			case 0:
				AddTermsData(ref dtSource, Function.ParseInt(txt_Code.Text), dicData);
				break;
			case 1:
			case 3:
				AddTermsData(ref dtSource, Function.ParseInt(txt_Code.Text), dicData2);
				break;
			case 4:
				AddTermsData(ref dtSource, Function.ParseInt(txt_Code.Text), dicData3);
				break;
			case 5:
				AddTermsData(ref dtSource, Function.ParseInt(txt_Code.Text), dicData4);
				break;
			}
			dtSource.AcceptChanges();
			grd_Terms.DataSource = dtSource;
			grd_Terms.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadData()
	{
		_ = 1;
		try
		{
			DataTable dt = (DataTable)(DataSource = await _httpClient.ApprovalTemplates.GetData());
			chk_ActiveUpdateDoc.Enabled = chk_Active.Checked;
			await LoadDataLines(Function.ParseInt(txt_Code.Text));
			if (!dt.IsNotNull())
			{
				mnu_DeleteDoc.Enabled = false;
				mnu_DuplicateDoc.Enabled = false;
				mnu_DeleteLine.Enabled = false;
				ShowDocuments(0);
				ClearDocumentControls();
				ClearDataControls();
			}
			else
			{
				mnu_DeleteDoc.Enabled = true;
				mnu_DuplicateDoc.Enabled = true;
				mnu_DeleteLine.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataLines(int templateCode)
	{
		try
		{
			DataSet dataSet = await _httpClient.ApprovalTemplates.GetDataLines(templateCode);
			if (dataSet.Tables.Count > 0)
			{
				_countTearms = 0;
				grd_Originator.DataSource = dataSet.Tables[0];
				grd_Originator.RefreshDataSource();
				if (dataSet.Tables[1].IsNotNull())
				{
					string text = Function.ToString(dataSet.Tables[1].Rows[0]["DocType"]);
					ClearDocumentControls();
					switch (text)
					{
					case "Documents":
						rd_Groups.SelectedIndex = 0;
						break;
					case "Inventory":
						rd_Groups.SelectedIndex = 1;
						break;
					case "Blanket Agreement":
						rd_Groups.SelectedIndex = 2;
						break;
					case "Internal Requistion":
						rd_Groups.SelectedIndex = 3;
						break;
					case "Payment":
						rd_Groups.SelectedIndex = 4;
						break;
					case "Inventory Counting Transaction":
						rd_Groups.SelectedIndex = 5;
						break;
					}
				}
				foreach (DataRow row in dataSet.Tables[1].Rows)
				{
					switch (Function.ParseInt(row["TransType"]))
					{
					case 23:
						chk_SalesQuotations.Checked = true;
						break;
					case 17:
						chk_SalesOrder.Checked = true;
						break;
					case 15:
						chk_Delivery.Checked = true;
						break;
					case 16:
						chk_Returns.Checked = true;
						break;
					case 234000031:
						chk_ReturnReqr.Checked = true;
						break;
					case 203:
						chk_ARDownPaymnt.Checked = true;
						break;
					case 13:
						chk_ARInvoice.Checked = true;
						break;
					case 14:
						chk_ARCreditMemo.Checked = true;
						break;
					case 540000006:
						chk_PurchaseQuo.Checked = true;
						break;
					case 22:
						chk_PurchaseOrder.Checked = true;
						break;
					case 20:
						chk_GoodReceiptPO.Checked = true;
						break;
					case 21:
						chk_GoodReturns.Checked = true;
						break;
					case 204:
						chk_APDownPaymnt.Checked = true;
						break;
					case 18:
						chk_APInvoice.Checked = true;
						break;
					case 19:
						chk_APCreditMemo.Checked = true;
						break;
					case 234000032:
						chk_GoodReturnReqr.Checked = true;
						break;
					case 59:
						chk_GoodReceipt.Checked = true;
						break;
					case 60:
						chk_GoodIssue.Checked = true;
						break;
					case 1250000001:
						chk_IVTransReqr.Checked = true;
						break;
					case 67:
						chk_IVTransfer.Checked = true;
						break;
					case 310000001:
						chk_IVOpenBalance.Checked = true;
						break;
					case 1250000026:
						chk_SalesBlanketAgrmnt.Checked = true;
						break;
					case 1250000027:
						chk_PurchaseBlanketAgrmnt.Checked = true;
						break;
					case 1470000113:
						chk_PurchaseReqr.Checked = true;
						break;
					case 46:
						chk_OutgoingPaymnt.Checked = true;
						break;
					case 10000071:
						chk_IVCounting.Checked = true;
						break;
					case 1470000065:
						chk_IVPosting.Checked = true;
						break;
					}
				}
				grd_Stages.DataSource = dataSet.Tables[2];
				grd_Stages.RefreshDataSource();
				if (dataSet.Tables[3].IsNotNull())
				{
					rd_LaunchAP.SelectedIndex = ((!Function.ToString(dataSet.Tables[3].Rows[0]["Conds"]).Equals("N")) ? 1 : 0);
				}
				LoadTermsConditions(rd_Groups.SelectedIndex);
				foreach (DataRow row2 in grd_Terms.DataSourceTable.Rows)
				{
					foreach (DataRow row3 in dataSet.Tables[4].Rows)
					{
						if (Function.ParseInt(row2["CondId"]) == Function.ParseInt(row3["CondId"]))
						{
							if (Function.ToString(row3["Checked"]).Equals("Y"))
							{
								_countTearms++;
							}
							row2["Checked"] = Function.ToString(row3["Checked"]);
							row2["opCode"] = Function.ParseInt(row3["opCode"]);
							row2["opValue"] = Function.ToString(row3["opValue"]);
						}
					}
				}
				grd_Terms.DataSourceTable.AcceptChanges();
				grd_Terms.RefreshDataSource();
				lbl_Totals.Text = Function.ToString(_countTearms);
				grd_Query.DataSource = dataSet.Tables[5];
				grd_Query.RefreshDataSource();
			}
			AddNewOriginator(isNew: false, templateCode);
			AddNewStages(isNew: false, templateCode);
			AddNewQueries(isNew: false, templateCode);
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewOriginator(bool isNew, int templateCode)
	{
		try
		{
			DataTable dataTable = grd_Originator.DataSourceTable;
			if (isNew)
			{
				dataTable = dataTable.Clone();
				dataTable.AcceptChanges();
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["WtmCode"] = templateCode;
			dataRow["UserId"] = DBNull.Value;
			dataRow["Departmnt"] = DBNull.Value;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Originator.DataSource = dataTable;
			grd_Originator.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewStages(bool isNew, int templateCode)
	{
		try
		{
			DataTable dataTable = grd_Stages.DataSourceTable;
			int num = 0;
			if (dataTable.IsNotNull())
			{
				num = dataTable.Copy().AsEnumerable().Max((DataRow r) => Function.ParseInt(r["SortId"]));
			}
			if (isNew)
			{
				dataTable = dataTable.Clone();
				dataTable.AcceptChanges();
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["WtmCode"] = templateCode;
			dataRow["SortId"] = num + 1;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Stages.DataSource = dataTable;
			grd_Stages.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewQueries(bool isNew, int templateCode)
	{
		try
		{
			DataTable dataTable = grd_Query.DataSourceTable;
			if (isNew)
			{
				dataTable = dataTable.Clone();
				dataTable.AcceptChanges();
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["WtmCode"] = templateCode;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Query.DataSource = dataTable;
			grd_Query.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetRecord(Enums.NavigateType type, int templateCode)
	{
		try
		{
			DataTable dataTable = new DataTable();
			switch (type)
			{
			case Enums.NavigateType.First:
				dataTable = _httpClient.ApprovalTemplates.GetRecord("F", Function.ToString(templateCode));
				break;
			case Enums.NavigateType.Last:
				dataTable = _httpClient.ApprovalTemplates.GetRecord("L", Function.ToString(templateCode));
				break;
			case Enums.NavigateType.Previous:
				dataTable = _httpClient.ApprovalTemplates.GetRecord("P", Function.ToString(templateCode));
				break;
			case Enums.NavigateType.Next:
				dataTable = _httpClient.ApprovalTemplates.GetRecord("N", Function.ToString(templateCode));
				break;
			}
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.Rows[0];
				txt_Code.EditValue = Function.ParseInt(dataRow["WtmCode"]);
				txt_Name.EditValue = Function.ToString(dataRow["Name"]);
				txt_Remarks.EditValue = Function.ToString(dataRow["Remarks"]);
				chk_Active.EditValue = Function.ToString(dataRow["Active"]);
				chk_ActiveUpdateDoc.EditValue = Function.ToString(dataRow["AppOnUpd"]);
				await LoadDataLines(Function.ParseInt(dataRow["WtmCode"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChangeViewMode()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
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

	private DataTable GenerateDocuments(int templateCode, int documentIndex)
	{
		try
		{
			DataTable tableStructure = WinFuntions.GetTableStructure("WTM3");
			GroupControl groupControl = new GroupControl();
			switch (documentIndex)
			{
			case 0:
				groupControl = grp_Documents;
				break;
			case 1:
				groupControl = grp_Inventory;
				break;
			case 2:
				groupControl = grp_BlanketAgreement;
				break;
			case 3:
				groupControl = grp_InternalRequis;
				break;
			case 4:
				groupControl = grp_Payment;
				break;
			case 5:
				groupControl = grp_IVCoutingTrans;
				break;
			}
			foreach (Control control in groupControl.Controls)
			{
				if (control.GetType().Name.Equals("CheckEditEx") && control is CheckEditEx { Checked: not false } checkEditEx)
				{
					DataRow dataRow = tableStructure.NewRow();
					dataRow["WtmCode"] = templateCode;
					dataRow["TransType"] = checkEditEx.Tag;
					tableStructure.Rows.Add(dataRow);
				}
			}
			tableStructure.AcceptChanges();
			return tableStructure;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	private void ClearDocumentControls()
	{
		try
		{
			rd_Groups.SelectedIndex = 0;
			foreach (GroupControl control2 in panelControl1.Controls)
			{
				foreach (Control control3 in control2.Controls)
				{
					if (control3.GetType().Name.Equals("CheckEditEx"))
					{
						(control3 as CheckEditEx).Checked = false;
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

	private void ClearTermsData()
	{
		try
		{
			grd_Terms.DataSourceTable.Clear();
			grd_Query.DataSourceTable.Clear();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ClearDataControls()
	{
		try
		{
			txt_Code.Text = "0";
			txt_Name.ResetText();
			txt_Remarks.ResetText();
			chk_Active.Checked = true;
			chk_ActiveUpdateDoc.Checked = true;
			grd_Originator.DataSourceTable.Clear();
			ClearDocumentControls();
			grd_Stages.DataSourceTable.Clear();
			rd_LaunchAP.SelectedIndex = 0;
			rd_LaunchAP.Properties.Items[1].Enabled = false;
			panelControl3.Visible = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ShowDocuments(int documentIndex)
	{
		try
		{
			switch (documentIndex)
			{
			case 0:
				grp_Documents.Dock = DockStyle.Fill;
				grp_Documents.Visible = true;
				grp_Inventory.Visible = false;
				grp_BlanketAgreement.Visible = false;
				grp_InternalRequis.Visible = false;
				grp_Payment.Visible = false;
				grp_IVCoutingTrans.Visible = false;
				rd_LaunchAP.SelectedIndex = 0;
				rd_LaunchAP.Properties.Items[1].Enabled = true;
				break;
			case 1:
				grp_Inventory.Dock = DockStyle.Fill;
				grp_Inventory.Visible = true;
				grp_BlanketAgreement.Visible = false;
				grp_InternalRequis.Visible = false;
				grp_Payment.Visible = false;
				grp_IVCoutingTrans.Visible = false;
				grp_Documents.Visible = false;
				rd_LaunchAP.SelectedIndex = 0;
				rd_LaunchAP.Properties.Items[1].Enabled = true;
				break;
			case 2:
				grp_BlanketAgreement.Dock = DockStyle.Fill;
				grp_Inventory.Visible = false;
				grp_BlanketAgreement.Visible = true;
				grp_InternalRequis.Visible = false;
				grp_Payment.Visible = false;
				grp_IVCoutingTrans.Visible = false;
				grp_Documents.Visible = false;
				rd_LaunchAP.SelectedIndex = 0;
				rd_LaunchAP.SelectedIndex = 0;
				rd_LaunchAP.Properties.Items[1].Enabled = false;
				panelControl3.Visible = false;
				ClearTermsData();
				break;
			case 3:
				grp_InternalRequis.Dock = DockStyle.Fill;
				grp_Inventory.Visible = false;
				grp_BlanketAgreement.Visible = false;
				grp_InternalRequis.Visible = true;
				grp_Payment.Visible = false;
				grp_IVCoutingTrans.Visible = false;
				grp_Documents.Visible = false;
				rd_LaunchAP.SelectedIndex = 0;
				rd_LaunchAP.Properties.Items[1].Enabled = true;
				break;
			case 4:
				grp_Payment.Dock = DockStyle.Fill;
				grp_Inventory.Visible = false;
				grp_BlanketAgreement.Visible = false;
				grp_InternalRequis.Visible = false;
				grp_Payment.Visible = true;
				grp_IVCoutingTrans.Visible = false;
				grp_Documents.Visible = false;
				rd_LaunchAP.SelectedIndex = 0;
				rd_LaunchAP.Properties.Items[1].Enabled = true;
				break;
			case 5:
				grp_IVCoutingTrans.Dock = DockStyle.Fill;
				grp_Inventory.Visible = false;
				grp_BlanketAgreement.Visible = false;
				grp_InternalRequis.Visible = false;
				grp_Payment.Visible = false;
				grp_IVCoutingTrans.Visible = true;
				grp_Documents.Visible = false;
				rd_LaunchAP.SelectedIndex = 0;
				rd_LaunchAP.Properties.Items[1].Enabled = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void UpdateSortId()
	{
		try
		{
			DataTable dataSourceTable = grd_Stages.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				for (int i = 0; i < dataSourceTable.Rows.Count; i++)
				{
					int num = i + 1;
					dataSourceTable.Rows[i]["SortId"] = num;
				}
				dataSourceTable.AcceptChanges();
				grd_Stages.DataSource = dataSourceTable;
				grd_Stages.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void radioGroupEx1_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			ShowDocuments(rd_Groups.SelectedIndex);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void frm_ApprovalTemplates_Load(object sender, EventArgs e)
	{
		try
		{
			await LoadData();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Originator_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			List<Common.ParameterValue> list = new List<Common.ParameterValue>();
			if (string.IsNullOrEmpty(Function.ToString(gv_Originator.GetRowCellValue(gv_Originator.FocusedRowHandle, gc_DepartmntName.FieldName))))
			{
				list.Add(new Common.ParameterValue("department", -1));
			}
			else
			{
				list.Add(new Common.ParameterValue("department", Function.ParseInt(gv_Originator.GetRowCellValue(gv_Originator.FocusedRowHandle, gc_DepartmntCode.FieldName))));
			}
			using frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ApprovalUsers, true, "", "", Enums.ItemObjectType.NoItemType, list, "");
			DataTable dataTable = grd_Originator.DataSourceTable.Copy();
			dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
			dataTable.AcceptChanges();
			frm_CFL2.dtSource = dataTable;
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
			{
				List<DataRow> chooseItems = frm_CFL2.ChooseItems;
				DataTable dataSourceTable = grd_Originator.DataSourceTable;
				int focusedRowHandle = gv_Originator.FocusedRowHandle;
				dataSourceTable.Rows[focusedRowHandle]["WtmCode"] = Function.ParseInt(txt_Code.Text);
				dataSourceTable.Rows[focusedRowHandle]["UserId"] = chooseItems[0]["UserId"];
				dataSourceTable.Rows[focusedRowHandle]["UserName"] = chooseItems[0]["UserName"];
				dataSourceTable.Rows[focusedRowHandle]["DepartmntCode"] = chooseItems[0]["DepartmntCode"];
				dataSourceTable.Rows[focusedRowHandle]["Departmnt"] = chooseItems[0]["Departmnt"];
				for (int i = 1; i < chooseItems.Count; i++)
				{
					DataRow dataRow = dataSourceTable.NewRow();
					dataRow["WtmCode"] = Function.ParseInt(txt_Code.Text);
					dataRow["UserId"] = chooseItems[i]["UserId"];
					dataRow["UserName"] = chooseItems[i]["UserName"];
					dataRow["DepartmntCode"] = chooseItems[i]["DepartmntCode"];
					dataRow["Departmnt"] = chooseItems[i]["Departmnt"];
					dataSourceTable.Rows.Add(dataRow);
				}
				dataSourceTable.AcceptChanges();
				grd_Originator.DataSource = dataSourceTable;
				grd_Originator.RefreshDataSource();
				AddNewOriginator(isNew: false, Function.ParseInt(gv_Originator.GetRowCellValue(gv_Originator.FocusedRowHandle, gc_WtmCode.FieldName)));
				ChangeViewMode();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Departmnt_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			using frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Departments, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gv_Originator.SetRowCellValue(gv_Originator.FocusedRowHandle, gc_DepartmntCode.FieldName, frm_CFL2.ChooseItem["Code"]);
				gv_Originator.SetRowCellValue(gv_Originator.FocusedRowHandle, gc_DepartmntName.FieldName, frm_CFL2.ChooseItem["Name"]);
				ChangeViewMode();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Stages_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			using frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ApprovalTemplateStages, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
			DataTable dataTable = grd_Stages.DataSourceTable.Copy();
			dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
			dataTable.AcceptChanges();
			frm_CFL2.dtSource = dataTable;
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
			{
				List<DataRow> chooseItems = frm_CFL2.ChooseItems;
				DataTable dataSourceTable = grd_Stages.DataSourceTable;
				int focusedRowHandle = gv_Stages.FocusedRowHandle;
				dataSourceTable.Rows[focusedRowHandle]["WtmCode"] = gv_Stages.GetRowCellValue(focusedRowHandle, gc_WtmCode1.FieldName);
				dataSourceTable.Rows[focusedRowHandle]["WstCode"] = chooseItems[0]["WstCode"];
				dataSourceTable.Rows[focusedRowHandle]["WstName"] = chooseItems[0]["Name"];
				dataSourceTable.Rows[focusedRowHandle]["WstRemarks"] = chooseItems[0]["Remarks"];
				dataSourceTable.Rows[focusedRowHandle]["SortId"] = 0;
				for (int i = 1; i < chooseItems.Count; i++)
				{
					DataRow dataRow = dataSourceTable.NewRow();
					dataRow["WtmCode"] = gv_Stages.GetRowCellValue(focusedRowHandle, gc_WtmCode1.FieldName);
					dataRow["WstCode"] = chooseItems[i]["WstCode"];
					dataRow["WstName"] = chooseItems[i]["Name"];
					dataRow["WstRemarks"] = chooseItems[i]["Remarks"];
					dataRow["SortId"] = 0;
					dataSourceTable.Rows.Add(dataRow);
				}
				UpdateSortId();
				dataSourceTable.AcceptChanges();
				grd_Stages.DataSource = dataSourceTable;
				grd_Stages.RefreshDataSource();
				AddNewStages(isNew: false, Function.ParseInt(gv_Stages.GetRowCellValue(gv_Stages.FocusedRowHandle, gc_WtmCode1.FieldName)));
				ChangeViewMode();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Originator_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.Name.Equals(gc_Username.Name))
			{
				DataRow rowFocus = gv_Originator.GetDataRow(e.RowHandle);
				int templateCode = Function.ParseInt(gv_Originator.GetRowCellValue(e.RowHandle, gc_WtmCode.FieldName));
				string userName = Function.ToString(e.Value);
				DataTable dataTable = await _httpClient.ApprovalTemplates.GetUserInfo(userName);
				if (dataTable.IsNotNull())
				{
					DataRow dataRow = dataTable.Rows[0];
					rowFocus["UserID"] = Function.ParseInt(dataRow["USERID"]);
					rowFocus["UserName"] = Function.ToString(dataRow["UserName"]);
					rowFocus["DepartmntCode"] = Function.ToString(dataRow["DepartmntCode"]);
					rowFocus["Departmnt"] = Function.ToString(dataRow["DepartmntName"]);
					rowFocus.AcceptChanges();
					AddNewOriginator(isNew: false, templateCode);
				}
				else
				{
					rep_Originator_ButtonClick(null, null);
				}
			}
			ChangeViewMode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Stages_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.Name.Equals(gc_WstName.Name))
			{
				DataRow rowFocus = gv_Stages.GetDataRow(e.RowHandle);
				int templateCode = Function.ParseInt(gv_Stages.GetRowCellValue(e.RowHandle, gc_WtmCode.FieldName));
				string templateName = Function.ToString(e.Value);
				DataTable dataTable = await _httpClient.ApprovalTemplates.GetApprovalStageInfo(templateName);
				if (dataTable.IsNotNull())
				{
					DataRow dataRow = dataTable.Rows[0];
					rowFocus["WstCode"] = Function.ParseInt(dataRow["WstCode"]);
					rowFocus["WstName"] = Function.ToString(dataRow["Name"]);
					rowFocus["WstRemarks"] = Function.ToString(dataRow["Remarks"]);
					rowFocus["SortId"] = 1;
					rowFocus.AcceptChanges();
					UpdateSortId();
					AddNewStages(isNew: false, templateCode);
				}
				else
				{
					rep_Stages_ButtonClick(null, null);
				}
			}
			ChangeViewMode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void panelControl1_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			if (e.Button == MouseButtons.Right)
			{
				contextMenuStrip2.Show(Control.MousePosition.X, Control.MousePosition.Y);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (FormMode == Enums.FormViewModeType.Find)
			{
				WinCommons._OpenProcessing();
				if (string.IsNullOrEmpty(txt_Name.Text))
				{
					txt_Name.Focus();
					return;
				}
				btn_OK.Enabled = false;
				DataTable dataTable = await _httpClient.ApprovalTemplates.SearchData(txt_Name.Text.Trim(), txt_Remarks.Text.Trim());
				if (!dataTable.IsNotNull())
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
					btn_OK.Enabled = true;
					return;
				}
				DataSource = dataTable;
				await LoadDataLines(Function.ParseInt(txt_Code.Text));
				btn_OK.Enabled = true;
				tabControl_Main.SelectedTabPageIndex = 0;
				rd_LaunchAP.Properties.Items[1].Enabled = true;
				FormMode = Enums.FormViewModeType.View;
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				btn_OK.Enabled = false;
				DataSet dataSet = new DataSet();
				DataTable tableStructure = WinFuntions.GetTableStructure("OWTM");
				DataRow dataRow = tableStructure.NewRow();
				dataRow["WtmCode"] = Function.ParseInt(txt_Code.Text);
				dataRow["Name"] = Function.ToString(txt_Name.Text.Trim());
				dataRow["Remarks"] = Function.ToString(txt_Remarks.Text.Trim());
				dataRow["Active"] = Function.ToString(chk_Active.EditValue);
				dataRow["AppOnUpd"] = Function.ToString(chk_ActiveUpdateDoc.EditValue);
				dataRow["Conds"] = ((rd_LaunchAP.SelectedIndex == 0) ? "N" : "Y");
				tableStructure.Rows.Add(dataRow);
				tableStructure.AcceptChanges();
				tableStructure.TableName = "dt1";
				DataTable dataSourceTable = grd_Originator.DataSourceTable;
				tableStructure.TableName = "dt2";
				DataTable dataTable2 = GenerateDocuments(Function.ParseInt(txt_Code.Text), rd_Groups.SelectedIndex);
				tableStructure.TableName = "dt3";
				DataTable dataSourceTable2 = grd_Stages.DataSourceTable;
				tableStructure.TableName = "dt4";
				DataTable dataTable3 = grd_Terms.DataSourceTable;
				tableStructure.TableName = "dt5";
				DataTable dataSourceTable3 = grd_Query.DataSourceTable;
				tableStructure.TableName = "dt6";
				dataSet.Tables.Add(tableStructure.Copy());
				dataSet.Tables.Add(dataSourceTable.Copy());
				dataSet.Tables.Add(dataTable2.Copy());
				dataSet.Tables.Add(dataSourceTable2.Copy());
				if (rd_LaunchAP.SelectedIndex == 0)
				{
					dataTable3 = dataTable3.Copy().Clone();
				}
				dataSet.Tables.Add(dataTable3.Copy());
				dataSet.Tables.Add(dataSourceTable3.Copy());
				Tuple<DataSet, string> tuple = new Tuple<DataSet, string>(dataSet, (FormMode == Enums.FormViewModeType.Add) ? "A" : "U");
				HttpResult httpResult = await _httpClient.ApprovalTemplates.Save(tuple);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					btn_OK.Enabled = true;
					return;
				}
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				await LoadData();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btn_OK.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void frm_ApprovalTemplates_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
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

	private void btn_MoveUp_Click(object sender, EventArgs e)
	{
		try
		{
			if ((!string.IsNullOrEmpty(Function.ToString(gv_Stages.GetRowCellValue(gv_Stages.FocusedRowHandle, gc_WstCode.FieldName))) || !string.IsNullOrEmpty(Function.ToString(gv_Stages.GetRowCellValue(gv_Stages.FocusedRowHandle, gc_WstName.FieldName)))) && Function.ParseInt(gv_Stages.FocusedRowHandle) != 0)
			{
				DataRow dataRow = gv_Stages.GetDataRow(gv_Stages.FocusedRowHandle);
				DataRow dataRow2 = gv_Stages.GetDataRow(gv_Stages.FocusedRowHandle - 1);
				DataRow dataRow3 = gv_Stages.GetDataRow(gv_Stages.RowCount - 1);
				int num = grd_Stages.DataSourceTable.Copy().AsEnumerable().Max((DataRow row) => Function.ParseInt(row["SortID"]));
				dataRow["SortID"] = Function.ParseInt(dataRow["SortID"]) - 1;
				dataRow2["SortID"] = Function.ParseInt(dataRow2["SortID"]) + 1;
				dataRow3["SortID"] = num + 1;
				grd_Stages.DataSourceTable.DefaultView.Sort = "SortID asc";
				grd_Stages.DataSourceTable.AcceptChanges();
				ChangeViewMode();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void deleteToolStripMenuItem3_Click(object sender, EventArgs e)
	{
		try
		{
			string name = tabControl_Main.SelectedTabPage.Name;
			if (!name.Equals(tabPage_Originator.Name) && !name.Equals(tabPage_Stages.Name) && !name.Equals(tabPage_Terms.Name))
			{
				return;
			}
			if (name.Equals(tabPage_Originator.Name))
			{
				if (gv_Originator.SelectedRowsCount != 1 || gv_Originator.FocusedRowHandle != gv_Originator.RowCount - 1 || !string.IsNullOrEmpty(Function.ToString(gv_Originator.GetRowCellValue(gv_Originator.FocusedRowHandle, gc_Username.FieldName))) || !string.IsNullOrEmpty(Function.ToString(gv_Originator.GetRowCellValue(gv_Originator.FocusedRowHandle, gc_DepartmntName.FieldName))))
				{
					gv_Originator.DeleteDataSourceRowSelected();
					grd_Originator.DataSourceTable.AcceptChanges();
					ChangeViewMode();
				}
			}
			else if (name.Equals(tabPage_Stages.Name))
			{
				if (gv_Stages.SelectedRowsCount != 1 || gv_Stages.FocusedRowHandle != gv_Stages.RowCount - 1 || !string.IsNullOrEmpty(Function.ToString(gv_Stages.GetRowCellValue(gv_Stages.FocusedRowHandle, gc_WstCode.FieldName))) || !string.IsNullOrEmpty(Function.ToString(gv_Stages.GetRowCellValue(gv_Stages.FocusedRowHandle, gc_WstName.FieldName))))
				{
					gv_Stages.DeleteDataSourceRowSelected();
					grd_Stages.DataSourceTable.AcceptChanges();
					ChangeViewMode();
				}
			}
			else if (gv_Query.SelectedRowsCount != 1 || gv_Query.FocusedRowHandle != gv_Query.RowCount - 1 || !string.IsNullOrEmpty(Function.ToString(gv_Query.GetRowCellValue(gv_Query.FocusedRowHandle, gc_QueryName.FieldName))))
			{
				gv_Query.DeleteDataSourceRowSelected();
				grd_Query.DataSourceTable.AcceptChanges();
				ChangeViewMode();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_MoveDown_Click(object sender, EventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(gv_Stages.GetRowCellValue(gv_Stages.FocusedRowHandle, gc_WstCode.FieldName))) || !string.IsNullOrEmpty(Function.ToString(gv_Stages.GetRowCellValue(gv_Stages.FocusedRowHandle, gc_WstName.FieldName))))
			{
				DataRow dataRow = gv_Stages.GetDataRow(gv_Stages.FocusedRowHandle);
				DataRow dataRow2 = gv_Stages.GetDataRow(gv_Stages.FocusedRowHandle + 1);
				DataRow dataRow3 = gv_Stages.GetDataRow(gv_Stages.RowCount - 1);
				int num = grd_Stages.DataSourceTable.Copy().AsEnumerable().Max((DataRow row) => Function.ParseInt(row["SortID"]));
				dataRow["SortID"] = Function.ParseInt(dataRow["SortID"]) + 1;
				dataRow2["SortID"] = Function.ParseInt(dataRow2["SortID"]) - 1;
				dataRow3["SortID"] = num + 1;
				grd_Stages.DataSourceTable.DefaultView.Sort = "SortID asc";
				grd_Stages.DataSourceTable.AcceptChanges();
				ChangeViewMode();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await REFRESH();
	}

	private async void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			btn_OK.Enabled = false;
			HttpResult httpResult = await _httpClient.ApprovalTemplates.Delete(new Tuple<int>(Function.ParseInt(txt_Code.Text)));
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return;
			}
			WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
			await LoadData();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btn_OK.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			string value = Function.ToString(_httpClient.ApprovalTemplates.GetNewCode());
			txt_Code.Text = value;
			txt_Name.ResetText();
			foreach (DataRow row in grd_Originator.DataSourceTable.Rows)
			{
				row["WtmCode"] = value;
			}
			foreach (DataRow row2 in grd_Stages.DataSourceTable.Rows)
			{
				row2["WtmCode"] = value;
			}
			foreach (DataRow row3 in grd_Terms.DataSourceTable.Rows)
			{
				row3["WtmCode"] = value;
			}
			foreach (DataRow row4 in grd_Query.DataSourceTable.Rows)
			{
				row4["WtmCode"] = value;
			}
			txt_Name.Focus();
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void deleteToolStripMenuItem2_Click(object sender, EventArgs e)
	{
		try
		{
			ClearDocumentControls();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rd_LaunchAP_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			if (rd_LaunchAP.SelectedIndex == 0)
			{
				panelControl3.Visible = false;
				return;
			}
			panelControl3.Visible = true;
			LoadTermsConditions(rd_Groups.SelectedIndex);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Terms_ShowingEditor(object sender, CancelEventArgs e)
	{
		if (gv_Terms.FocusedColumn.Name.Equals(gc_TermName.Name))
		{
			e.Cancel = true;
		}
		if (!gv_Terms.FocusedColumn.Name.Equals(gc_Choose.Name) && Function.ToString(gv_Terms.GetRowCellValue(gv_Terms.FocusedRowHandle, gc_Choose.FieldName)).Equals("N"))
		{
			e.Cancel = true;
		}
	}

	private void rep_Query_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			using frm_ManagerQuery frm_ManagerQuery2 = new frm_ManagerQuery();
			frm_ManagerQuery2.Text = "Query Manager";
			frm_ManagerQuery2.Type = "dialog";
			frm_ManagerQuery2.ShowDialog(this);
			if (!string.IsNullOrEmpty(frm_ManagerQuery2.QueryName))
			{
				int queryId = _httpClient.ApprovalTemplates.GetQueryId(frm_ManagerQuery2.QueryName);
				gv_Query.SetRowCellValue(gv_Query.FocusedRowHandle, gc_QueryId, queryId);
				gv_Query.SetRowCellValue(gv_Query.FocusedRowHandle, gc_QueryName, frm_ManagerQuery2.QueryName);
				AddNewQueries(isNew: false, Function.ParseInt(txt_Code.Text));
				ChangeViewMode();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Terms_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (gv_Terms.FocusedColumn.Name.Equals(gc_Choose.Name))
			{
				if (Function.ToString(gv_Terms.GetRowCellValue(e.RowHandle, gc_Choose.FieldName)).Equals("N"))
				{
					DataRow dataRow = gv_Terms.GetDataRow(e.RowHandle);
					dataRow["opValue"] = string.Empty;
					dataRow.AcceptChanges();
					_countTearms--;
				}
				else
				{
					_countTearms++;
				}
				lbl_Totals.Text = Function.ToString(_countTearms);
			}
			ChangeViewMode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Name_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Find && e.KeyCode == Keys.Return)
			{
				btn_OK_Click(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Remarks_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Find && e.KeyCode == Keys.Return)
			{
				btn_OK_Click(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_ApprovalTemplates_OnControlValueChanged(object sender, ControlChangedEventArgs e)
	{
		try
		{
			if (e.FormControl.GetType().Name.Equals("CheckEditEx"))
			{
				ChangeViewMode();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_Active_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			chk_ActiveUpdateDoc.Enabled = chk_Active.Checked;
			chk_ActiveUpdateDoc.Checked = false;
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.ApprovalProcess.frm_ApprovalTemplates));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.mnu_AddDoc = new System.Windows.Forms.ToolStripMenuItem();
		this.mnu_DeleteDoc = new System.Windows.Forms.ToolStripMenuItem();
		this.mnu_DuplicateDoc = new System.Windows.Forms.ToolStripMenuItem();
		this.txt_Remarks = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Name = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		this.chk_ActiveUpdateDoc = new Apzon.Controls.Editor.CheckEditEx();
		this.lbl_Remarks = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Name = new Apzon.Controls.Controls.LabelLine();
		this.chk_Active = new Apzon.Controls.Editor.CheckEditEx();
		this.lbl_Code = new Apzon.Controls.Controls.LabelLine();
		this.tabControl_Main = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabPage_Originator = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Originator = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.mnu_DeleteLine = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Originator = new Apzon.Controls.Grid.GridViewEx();
		this.gc_WtmCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UserId1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Username = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Originator = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_DepartmntCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_DepartmntName = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Departmnt = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tabPage_Documents = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
		this.grp_BlanketAgreement = new DevExpress.XtraEditors.GroupControl();
		this.chk_PurchaseBlanketAgrmnt = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_SalesBlanketAgrmnt = new Apzon.Controls.Editor.CheckEditEx();
		this.grp_InternalRequis = new DevExpress.XtraEditors.GroupControl();
		this.chk_PurchaseReqr = new Apzon.Controls.Editor.CheckEditEx();
		this.grp_Documents = new DevExpress.XtraEditors.GroupControl();
		this.chk_APCreditMemo = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_ARCreditMemo = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_APInvoice = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_ARInvoice = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_APDownPaymnt = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_ARDownPaymnt = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_GoodReturns = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Returns = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_GoodReturnReqr = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_ReturnReqr = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_GoodReceiptPO = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Delivery = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_PurchaseOrder = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_SalesOrder = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_PurchaseQuo = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_SalesQuotations = new Apzon.Controls.Editor.CheckEditEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.grp_Inventory = new DevExpress.XtraEditors.GroupControl();
		this.chk_IVOpenBalance = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_IVTransfer = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_IVTransReqr = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_GoodIssue = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_GoodReceipt = new Apzon.Controls.Editor.CheckEditEx();
		this.grp_Payment = new DevExpress.XtraEditors.GroupControl();
		this.chk_OutgoingPaymnt = new Apzon.Controls.Editor.CheckEditEx();
		this.grp_IVCoutingTrans = new DevExpress.XtraEditors.GroupControl();
		this.chk_IVPosting = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_IVCounting = new Apzon.Controls.Editor.CheckEditEx();
		this.rd_Groups = new Apzon.Controls.Editor.RadioGroupEx();
		this.tabPage_Stages = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_MoveDown = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_MoveUp = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Stages = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Stages = new Apzon.Controls.Grid.GridViewEx();
		this.gc_WtmCode1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_WstCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_WstName = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Stages = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_WstRemarks = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_SortID = new Apzon.Controls.Grid.GridColumnEx();
		this.tabPage_Terms = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		this.lbl_Totals = new Apzon.Controls.Controls.LabelControlEx();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Terms = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Terms = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Choose = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Choose = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gc_TermsId = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_TermName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Ratio = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Ratio = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Value = new Apzon.Controls.Grid.GridColumnEx();
		this.grd_Query = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Query = new Apzon.Controls.Grid.GridViewEx();
		this.gc_QueryId = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_QueryName = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Query = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.lbl_TotalTerms = new Apzon.Controls.Controls.LabelLine();
		this.rd_LaunchAP = new Apzon.Controls.Editor.RadioGroupEx();
		this.lbl_LaunchAP = new Apzon.Controls.Controls.LabelLine();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Remarks.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ActiveUpdateDoc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl_Main).BeginInit();
		this.tabControl_Main.SuspendLayout();
		this.tabPage_Originator.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Originator).BeginInit();
		this.contextMenuStrip3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Originator).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Originator).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Departmnt).BeginInit();
		this.tabPage_Documents.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grp_BlanketAgreement).BeginInit();
		this.grp_BlanketAgreement.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_PurchaseBlanketAgrmnt.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_SalesBlanketAgrmnt.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grp_InternalRequis).BeginInit();
		this.grp_InternalRequis.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_PurchaseReqr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grp_Documents).BeginInit();
		this.grp_Documents.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_APCreditMemo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ARCreditMemo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_APInvoice.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ARInvoice.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_APDownPaymnt.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ARDownPaymnt.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_GoodReturns.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Returns.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_GoodReturnReqr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ReturnReqr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_GoodReceiptPO.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Delivery.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PurchaseOrder.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_SalesOrder.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PurchaseQuo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_SalesQuotations.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grp_Inventory).BeginInit();
		this.grp_Inventory.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_IVOpenBalance.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_IVTransfer.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_IVTransReqr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_GoodIssue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_GoodReceipt.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grp_Payment).BeginInit();
		this.grp_Payment.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_OutgoingPaymnt.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grp_IVCoutingTrans).BeginInit();
		this.grp_IVCoutingTrans.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_IVPosting.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_IVCounting.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rd_Groups.Properties).BeginInit();
		this.tabPage_Stages.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Stages).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Stages).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Stages).BeginInit();
		this.tabPage_Terms.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Terms).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Terms).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Choose).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Ratio).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Query).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Query).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Query).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rd_LaunchAP.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(3, 3);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.panelControl2);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.tabControl_Main);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(918, 596);
		this.splitContainerControl1.SplitterPosition = 126;
		this.splitContainerControl1.TabIndex = 0;
		this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.panelControl2.ContextMenuStrip = this.contextMenuStrip1;
		this.panelControl2.Controls.Add(this.txt_Remarks);
		this.panelControl2.Controls.Add(this.txt_Name);
		this.panelControl2.Controls.Add(this.txt_Code);
		this.panelControl2.Controls.Add(this.chk_ActiveUpdateDoc);
		this.panelControl2.Controls.Add(this.lbl_Remarks);
		this.panelControl2.Controls.Add(this.lbl_Name);
		this.panelControl2.Controls.Add(this.chk_Active);
		this.panelControl2.Controls.Add(this.lbl_Code);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl2.Location = new System.Drawing.Point(0, 0);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(918, 126);
		this.panelControl2.TabIndex = 0;
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.mnu_AddDoc, this.mnu_DeleteDoc, this.mnu_DuplicateDoc });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(125, 70);
		this.mnu_AddDoc.Name = "mnu_AddDoc";
		this.mnu_AddDoc.Size = new System.Drawing.Size(124, 22);
		this.mnu_AddDoc.Text = "Add New";
		this.mnu_AddDoc.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.mnu_DeleteDoc.Name = "mnu_DeleteDoc";
		this.mnu_DeleteDoc.Size = new System.Drawing.Size(124, 22);
		this.mnu_DeleteDoc.Text = "Delete";
		this.mnu_DeleteDoc.Click += new System.EventHandler(this.deleteToolStripMenuItem1_Click);
		this.mnu_DuplicateDoc.Name = "mnu_DuplicateDoc";
		this.mnu_DuplicateDoc.Size = new System.Drawing.Size(124, 22);
		this.mnu_DuplicateDoc.Text = "Duplicate";
		this.mnu_DuplicateDoc.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
		this.txt_Remarks.EditControl = null;
		this.txt_Remarks.FieldName = "Remarks";
		this.txt_Remarks.IsSearchControl = false;
		this.txt_Remarks.Location = new System.Drawing.Point(74, 50);
		this.txt_Remarks.Name = "txt_Remarks";
		this.txt_Remarks.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_Remarks.Properties.Appearance.Options.UseFont = true;
		this.txt_Remarks.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.txt_Remarks.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_Remarks.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_Remarks.Properties.AppearanceFocused.Options.UseFont = true;
		this.txt_Remarks.SettingInVisible = false;
		this.txt_Remarks.SettingReadonly = false;
		this.txt_Remarks.Size = new System.Drawing.Size(238, 20);
		this.txt_Remarks.TabIndex = 11;
		this.txt_Remarks.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_Remarks_KeyDown);
		this.txt_Name.EditControl = null;
		this.txt_Name.FieldName = "Name";
		this.txt_Name.IsSearchControl = false;
		this.txt_Name.Location = new System.Drawing.Point(74, 28);
		this.txt_Name.Name = "txt_Name";
		this.txt_Name.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_Name.Properties.Appearance.Options.UseFont = true;
		this.txt_Name.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.txt_Name.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_Name.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_Name.Properties.AppearanceFocused.Options.UseFont = true;
		this.txt_Name.Properties.MaxLength = 20;
		this.txt_Name.SettingInVisible = false;
		this.txt_Name.SettingReadonly = false;
		this.txt_Name.Size = new System.Drawing.Size(238, 20);
		this.txt_Name.TabIndex = 10;
		this.txt_Name.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_Name_KeyDown);
		this.txt_Code.EditControl = null;
		this.txt_Code.FieldName = "WtmCode";
		this.txt_Code.IsSearchControl = false;
		this.txt_Code.Location = new System.Drawing.Point(74, 6);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(219, 219, 219);
		this.txt_Code.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.txt_Code.Properties.Appearance.Options.UseBackColor = true;
		this.txt_Code.Properties.Appearance.Options.UseFont = true;
		this.txt_Code.Properties.ReadOnly = true;
		this.txt_Code.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
		this.txt_Code.SettingInVisible = false;
		this.txt_Code.SettingReadonly = false;
		this.txt_Code.Size = new System.Drawing.Size(123, 20);
		this.txt_Code.TabIndex = 9;
		this.chk_ActiveUpdateDoc.EditControl = null;
		this.chk_ActiveUpdateDoc.FieldName = "AppOnUpd";
		this.chk_ActiveUpdateDoc.IsSearchControl = false;
		this.chk_ActiveUpdateDoc.Location = new System.Drawing.Point(26, 101);
		this.chk_ActiveUpdateDoc.Name = "chk_ActiveUpdateDoc";
		this.chk_ActiveUpdateDoc.Properties.Caption = "Active when Updating Documents Not Generated By Approval Process";
		this.chk_ActiveUpdateDoc.Properties.DisplayValueChecked = "Y";
		this.chk_ActiveUpdateDoc.Properties.DisplayValueGrayed = "N";
		this.chk_ActiveUpdateDoc.Properties.DisplayValueUnchecked = "N";
		this.chk_ActiveUpdateDoc.Properties.ValueChecked = "Y";
		this.chk_ActiveUpdateDoc.Properties.ValueGrayed = "N";
		this.chk_ActiveUpdateDoc.Properties.ValueUnchecked = "N";
		this.chk_ActiveUpdateDoc.SettingInVisible = false;
		this.chk_ActiveUpdateDoc.SettingReadonly = false;
		this.chk_ActiveUpdateDoc.Size = new System.Drawing.Size(499, 20);
		this.chk_ActiveUpdateDoc.TabIndex = 13;
		this.lbl_Remarks.EditControl = null;
		this.lbl_Remarks.EditValue = "Remarks";
		this.lbl_Remarks.FieldName = null;
		this.lbl_Remarks.HideWhenEmpty = false;
		this.lbl_Remarks.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Remarks.Location = new System.Drawing.Point(9, 53);
		this.lbl_Remarks.Name = "lbl_Remarks";
		this.lbl_Remarks.ReadOnly = false;
		this.lbl_Remarks.SettingInVisible = false;
		this.lbl_Remarks.SettingReadonly = false;
		this.lbl_Remarks.ShowUnderLine = true;
		this.lbl_Remarks.Size = new System.Drawing.Size(80, 20);
		this.lbl_Remarks.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Remarks.TabIndex = 6;
		this.lbl_Remarks.Utext = "Remarks";
		this.lbl_Name.EditControl = null;
		this.lbl_Name.EditValue = "Name";
		this.lbl_Name.FieldName = null;
		this.lbl_Name.HideWhenEmpty = false;
		this.lbl_Name.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Name.Location = new System.Drawing.Point(9, 31);
		this.lbl_Name.Name = "lbl_Name";
		this.lbl_Name.ReadOnly = false;
		this.lbl_Name.SettingInVisible = false;
		this.lbl_Name.SettingReadonly = false;
		this.lbl_Name.ShowUnderLine = true;
		this.lbl_Name.Size = new System.Drawing.Size(80, 20);
		this.lbl_Name.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Name.TabIndex = 7;
		this.lbl_Name.Utext = "Name";
		this.chk_Active.EditControl = null;
		this.chk_Active.FieldName = "Active";
		this.chk_Active.IsSearchControl = false;
		this.chk_Active.Location = new System.Drawing.Point(9, 79);
		this.chk_Active.Name = "chk_Active";
		this.chk_Active.Properties.Caption = "Active";
		this.chk_Active.Properties.DisplayValueChecked = "Y";
		this.chk_Active.Properties.DisplayValueGrayed = "N";
		this.chk_Active.Properties.DisplayValueUnchecked = "N";
		this.chk_Active.Properties.ValueChecked = "Y";
		this.chk_Active.Properties.ValueGrayed = "N";
		this.chk_Active.Properties.ValueUnchecked = "N";
		this.chk_Active.SettingInVisible = false;
		this.chk_Active.SettingReadonly = false;
		this.chk_Active.Size = new System.Drawing.Size(75, 20);
		this.chk_Active.TabIndex = 12;
		this.chk_Active.CheckedChanged += new System.EventHandler(this.chk_Active_CheckedChanged);
		this.lbl_Code.EditControl = null;
		this.lbl_Code.EditValue = "Code";
		this.lbl_Code.FieldName = null;
		this.lbl_Code.HideWhenEmpty = false;
		this.lbl_Code.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Code.Location = new System.Drawing.Point(9, 9);
		this.lbl_Code.Name = "lbl_Code";
		this.lbl_Code.ReadOnly = false;
		this.lbl_Code.SettingInVisible = false;
		this.lbl_Code.SettingReadonly = false;
		this.lbl_Code.ShowUnderLine = true;
		this.lbl_Code.Size = new System.Drawing.Size(80, 20);
		this.lbl_Code.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Code.TabIndex = 8;
		this.lbl_Code.Utext = "Code";
		this.tabControl_Main.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tabControl_Main.IsSearchControl = false;
		this.tabControl_Main.Location = new System.Drawing.Point(0, 0);
		this.tabControl_Main.Name = "tabControl_Main";
		this.tabControl_Main.SelectedTabPage = this.tabPage_Originator;
		this.tabControl_Main.Size = new System.Drawing.Size(918, 460);
		this.tabControl_Main.TabIndex = 6;
		this.tabControl_Main.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[4] { this.tabPage_Originator, this.tabPage_Documents, this.tabPage_Stages, this.tabPage_Terms });
		this.tabPage_Originator.Controls.Add(this.grd_Originator);
		this.tabPage_Originator.EditControl = null;
		this.tabPage_Originator.EditValue = "Originator";
		this.tabPage_Originator.FieldName = "";
		this.tabPage_Originator.IsSearchControl = false;
		this.tabPage_Originator.Name = "tabPage_Originator";
		this.tabPage_Originator.ReadOnly = false;
		this.tabPage_Originator.SettingInVisible = false;
		this.tabPage_Originator.SettingReadonly = false;
		this.tabPage_Originator.Size = new System.Drawing.Size(916, 435);
		this.tabPage_Originator.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPage_Originator.Text = "Originator";
		this.grd_Originator.AllowPasteExcel = true;
		this.grd_Originator.ClipboardData = null;
		this.grd_Originator.ContextMenuStrip = this.contextMenuStrip3;
		this.grd_Originator.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Originator.Location = new System.Drawing.Point(0, 0);
		this.grd_Originator.MainView = this.gv_Originator;
		this.grd_Originator.Name = "grd_Originator";
		this.grd_Originator.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_Originator, this.rep_Departmnt });
		this.grd_Originator.Size = new System.Drawing.Size(916, 435);
		this.grd_Originator.TabIndex = 0;
		this.grd_Originator.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Originator });
		this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.mnu_DeleteLine });
		this.contextMenuStrip3.Name = "contextMenuStrip3";
		this.contextMenuStrip3.Size = new System.Drawing.Size(108, 26);
		this.mnu_DeleteLine.Name = "mnu_DeleteLine";
		this.mnu_DeleteLine.Size = new System.Drawing.Size(107, 22);
		this.mnu_DeleteLine.Text = "Delete";
		this.mnu_DeleteLine.Click += new System.EventHandler(this.deleteToolStripMenuItem3_Click);
		this.gv_Originator.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Originator.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Originator.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Originator.ColumnKey = null;
		this.gv_Originator.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gc_WtmCode, this.gc_UserId1, this.gc_Username, this.gc_DepartmntCode, this.gc_DepartmntName });
		this.gv_Originator.DisableFormSetting = false;
		this.gv_Originator.DisplayException = false;
		this.gv_Originator.EditColumn = null;
		this.gv_Originator.FieldCurrency = null;
		this.gv_Originator.FieldRate = null;
		this.gv_Originator.FormId = "oApprovalTemplates";
		this.gv_Originator.GridControl = this.grd_Originator;
		this.gv_Originator.IgnoreFilter = false;
		this.gv_Originator.IndexEditRow = -1;
		this.gv_Originator.IndicatorWidth = 35;
		this.gv_Originator.IsApzTable = false;
		this.gv_Originator.IsInsertNewRow = false;
		this.gv_Originator.IsLoadingData = false;
		this.gv_Originator.IsMappingContextAction = false;
		this.gv_Originator.ItemId = "WTM1";
		this.gv_Originator.LstColumnAllowBatch = null;
		this.gv_Originator.LstColumnNotAllowBatch = null;
		this.gv_Originator.LstColumnNotEditable = null;
		this.gv_Originator.LstColumNotSetting = null;
		this.gv_Originator.Name = "gv_Originator";
		this.gv_Originator.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Originator.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Originator.OptionsNavigation.AutoFocusNewRow = true;
		this.gv_Originator.OptionsPrint.AutoWidth = false;
		this.gv_Originator.OptionsPrint.UsePrintStyles = false;
		this.gv_Originator.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Originator.OptionsView.ColumnAutoWidth = false;
		this.gv_Originator.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Originator.OptionsView.ShowGroupedColumns = true;
		this.gv_Originator.OptionsView.ShowGroupPanel = false;
		this.gv_Originator.UseBatchMod = false;
		this.gv_Originator.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Originator_CellValueChanged);
		this.gc_WtmCode.Caption = "Template Code";
		this.gc_WtmCode.CurrencyDateField = null;
		this.gc_WtmCode.CurrencyField = null;
		this.gc_WtmCode.CurrencyFixed = null;
		this.gc_WtmCode.CurrencyFixedRate = 0.0;
		this.gc_WtmCode.CurrencyRateField = null;
		this.gc_WtmCode.FieldName = "WtmCode";
		this.gc_WtmCode.Name = "gc_WtmCode";
		this.gc_WtmCode.OptionsColumn.AllowEdit = false;
		this.gc_WtmCode.OptionsColumn.ReadOnly = true;
		this.gc_WtmCode.PrimaryKey = true;
		this.gc_WtmCode.UnitField = null;
		this.gc_UserId1.Caption = "UserId";
		this.gc_UserId1.CurrencyDateField = null;
		this.gc_UserId1.CurrencyField = null;
		this.gc_UserId1.CurrencyFixed = null;
		this.gc_UserId1.CurrencyFixedRate = 0.0;
		this.gc_UserId1.CurrencyRateField = null;
		this.gc_UserId1.FieldName = "UserId";
		this.gc_UserId1.Name = "gc_UserId1";
		this.gc_UserId1.OptionsColumn.AllowEdit = false;
		this.gc_UserId1.OptionsColumn.ReadOnly = true;
		this.gc_UserId1.PrimaryKey = false;
		this.gc_UserId1.UnitField = null;
		this.gc_Username.Caption = "User";
		this.gc_Username.ColumnEdit = this.rep_Originator;
		this.gc_Username.CurrencyDateField = null;
		this.gc_Username.CurrencyField = null;
		this.gc_Username.CurrencyFixed = null;
		this.gc_Username.CurrencyFixedRate = 0.0;
		this.gc_Username.CurrencyRateField = null;
		this.gc_Username.FieldName = "UserName";
		this.gc_Username.Name = "gc_Username";
		this.gc_Username.PrimaryKey = false;
		this.gc_Username.UnitField = null;
		this.gc_Username.Visible = true;
		this.gc_Username.VisibleIndex = 0;
		this.gc_Username.Width = 125;
		this.rep_Originator.AutoHeight = false;
		this.rep_Originator.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Originator.Name = "rep_Originator";
		this.rep_Originator.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Originator_ButtonClick);
		this.gc_DepartmntCode.Caption = "Department Code";
		this.gc_DepartmntCode.CurrencyDateField = null;
		this.gc_DepartmntCode.CurrencyField = null;
		this.gc_DepartmntCode.CurrencyFixed = null;
		this.gc_DepartmntCode.CurrencyFixedRate = 0.0;
		this.gc_DepartmntCode.CurrencyRateField = null;
		this.gc_DepartmntCode.FieldName = "DepartmntCode";
		this.gc_DepartmntCode.Name = "gc_DepartmntCode";
		this.gc_DepartmntCode.OptionsColumn.AllowEdit = false;
		this.gc_DepartmntCode.OptionsColumn.ReadOnly = true;
		this.gc_DepartmntCode.PrimaryKey = false;
		this.gc_DepartmntCode.UnitField = null;
		this.gc_DepartmntName.Caption = "Department";
		this.gc_DepartmntName.ColumnEdit = this.rep_Departmnt;
		this.gc_DepartmntName.CurrencyDateField = null;
		this.gc_DepartmntName.CurrencyField = null;
		this.gc_DepartmntName.CurrencyFixed = null;
		this.gc_DepartmntName.CurrencyFixedRate = 0.0;
		this.gc_DepartmntName.CurrencyRateField = null;
		this.gc_DepartmntName.FieldName = "Departmnt";
		this.gc_DepartmntName.Name = "gc_DepartmntName";
		this.gc_DepartmntName.PrimaryKey = false;
		this.gc_DepartmntName.UnitField = null;
		this.gc_DepartmntName.Visible = true;
		this.gc_DepartmntName.VisibleIndex = 1;
		this.gc_DepartmntName.Width = 125;
		this.rep_Departmnt.AutoHeight = false;
		this.rep_Departmnt.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Departmnt.Name = "rep_Departmnt";
		this.rep_Departmnt.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Departmnt_ButtonClick);
		this.tabPage_Documents.Appearance.PageClient.BackColor = System.Drawing.Color.White;
		this.tabPage_Documents.Appearance.PageClient.Options.UseBackColor = true;
		this.tabPage_Documents.Controls.Add(this.panelControl1);
		this.tabPage_Documents.Controls.Add(this.rd_Groups);
		this.tabPage_Documents.EditControl = null;
		this.tabPage_Documents.EditValue = "Documents";
		this.tabPage_Documents.FieldName = "";
		this.tabPage_Documents.IsSearchControl = false;
		this.tabPage_Documents.Name = "tabPage_Documents";
		this.tabPage_Documents.ReadOnly = false;
		this.tabPage_Documents.SettingInVisible = false;
		this.tabPage_Documents.SettingReadonly = false;
		this.tabPage_Documents.Size = new System.Drawing.Size(916, 435);
		this.tabPage_Documents.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPage_Documents.Text = "Documents";
		this.panelControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panelControl1.Appearance.BackColor = System.Drawing.Color.White;
		this.panelControl1.Appearance.Options.UseBackColor = true;
		this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.panelControl1.ContextMenuStrip = this.contextMenuStrip2;
		this.panelControl1.Controls.Add(this.grp_BlanketAgreement);
		this.panelControl1.Controls.Add(this.grp_InternalRequis);
		this.panelControl1.Controls.Add(this.grp_Documents);
		this.panelControl1.Controls.Add(this.grp_Inventory);
		this.panelControl1.Controls.Add(this.grp_Payment);
		this.panelControl1.Controls.Add(this.grp_IVCoutingTrans);
		this.panelControl1.Location = new System.Drawing.Point(3, 36);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(910, 393);
		this.panelControl1.TabIndex = 2;
		this.panelControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelControl1_MouseDown);
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteToolStripMenuItem2 });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(135, 26);
		this.deleteToolStripMenuItem2.Name = "deleteToolStripMenuItem2";
		this.deleteToolStripMenuItem2.Size = new System.Drawing.Size(134, 22);
		this.deleteToolStripMenuItem2.Text = "Remove All";
		this.deleteToolStripMenuItem2.Click += new System.EventHandler(this.deleteToolStripMenuItem2_Click);
		this.grp_BlanketAgreement.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.grp_BlanketAgreement.Controls.Add(this.chk_PurchaseBlanketAgrmnt);
		this.grp_BlanketAgreement.Controls.Add(this.chk_SalesBlanketAgrmnt);
		this.grp_BlanketAgreement.Location = new System.Drawing.Point(523, 201);
		this.grp_BlanketAgreement.Name = "grp_BlanketAgreement";
		this.grp_BlanketAgreement.ShowCaption = false;
		this.grp_BlanketAgreement.Size = new System.Drawing.Size(97, 83);
		this.grp_BlanketAgreement.TabIndex = 6;
		this.grp_BlanketAgreement.Text = "Inventory";
		this.chk_PurchaseBlanketAgrmnt.EditControl = null;
		this.chk_PurchaseBlanketAgrmnt.FieldName = null;
		this.chk_PurchaseBlanketAgrmnt.IsSearchControl = false;
		this.chk_PurchaseBlanketAgrmnt.Location = new System.Drawing.Point(15, 29);
		this.chk_PurchaseBlanketAgrmnt.Name = "chk_PurchaseBlanketAgrmnt";
		this.chk_PurchaseBlanketAgrmnt.Properties.Caption = "Purchase Blanket Agreement";
		this.chk_PurchaseBlanketAgrmnt.SettingInVisible = false;
		this.chk_PurchaseBlanketAgrmnt.SettingReadonly = false;
		this.chk_PurchaseBlanketAgrmnt.Size = new System.Drawing.Size(187, 20);
		this.chk_PurchaseBlanketAgrmnt.TabIndex = 1;
		this.chk_PurchaseBlanketAgrmnt.Tag = "1250000027";
		this.chk_SalesBlanketAgrmnt.EditControl = null;
		this.chk_SalesBlanketAgrmnt.FieldName = null;
		this.chk_SalesBlanketAgrmnt.IsSearchControl = false;
		this.chk_SalesBlanketAgrmnt.Location = new System.Drawing.Point(15, 3);
		this.chk_SalesBlanketAgrmnt.Name = "chk_SalesBlanketAgrmnt";
		this.chk_SalesBlanketAgrmnt.Properties.Caption = "Sales Blanket Agreement";
		this.chk_SalesBlanketAgrmnt.SettingInVisible = false;
		this.chk_SalesBlanketAgrmnt.SettingReadonly = false;
		this.chk_SalesBlanketAgrmnt.Size = new System.Drawing.Size(164, 20);
		this.chk_SalesBlanketAgrmnt.TabIndex = 1;
		this.chk_SalesBlanketAgrmnt.Tag = "1250000026";
		this.grp_InternalRequis.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.grp_InternalRequis.Controls.Add(this.chk_PurchaseReqr);
		this.grp_InternalRequis.Location = new System.Drawing.Point(35, 135);
		this.grp_InternalRequis.Name = "grp_InternalRequis";
		this.grp_InternalRequis.ShowCaption = false;
		this.grp_InternalRequis.Size = new System.Drawing.Size(109, 78);
		this.grp_InternalRequis.TabIndex = 5;
		this.grp_InternalRequis.Text = "Inventory";
		this.chk_PurchaseReqr.EditControl = null;
		this.chk_PurchaseReqr.FieldName = null;
		this.chk_PurchaseReqr.IsSearchControl = false;
		this.chk_PurchaseReqr.Location = new System.Drawing.Point(15, 2);
		this.chk_PurchaseReqr.Name = "chk_PurchaseReqr";
		this.chk_PurchaseReqr.Properties.Caption = "Purchase Request";
		this.chk_PurchaseReqr.SettingInVisible = false;
		this.chk_PurchaseReqr.SettingReadonly = false;
		this.chk_PurchaseReqr.Size = new System.Drawing.Size(116, 20);
		this.chk_PurchaseReqr.TabIndex = 1;
		this.chk_PurchaseReqr.Tag = "1470000113";
		this.grp_Documents.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.grp_Documents.Controls.Add(this.chk_APCreditMemo);
		this.grp_Documents.Controls.Add(this.chk_ARCreditMemo);
		this.grp_Documents.Controls.Add(this.chk_APInvoice);
		this.grp_Documents.Controls.Add(this.chk_ARInvoice);
		this.grp_Documents.Controls.Add(this.chk_APDownPaymnt);
		this.grp_Documents.Controls.Add(this.chk_ARDownPaymnt);
		this.grp_Documents.Controls.Add(this.chk_GoodReturns);
		this.grp_Documents.Controls.Add(this.chk_Returns);
		this.grp_Documents.Controls.Add(this.chk_GoodReturnReqr);
		this.grp_Documents.Controls.Add(this.chk_ReturnReqr);
		this.grp_Documents.Controls.Add(this.chk_GoodReceiptPO);
		this.grp_Documents.Controls.Add(this.chk_Delivery);
		this.grp_Documents.Controls.Add(this.chk_PurchaseOrder);
		this.grp_Documents.Controls.Add(this.chk_SalesOrder);
		this.grp_Documents.Controls.Add(this.chk_PurchaseQuo);
		this.grp_Documents.Controls.Add(this.chk_SalesQuotations);
		this.grp_Documents.Controls.Add(this.labelLine5);
		this.grp_Documents.Controls.Add(this.labelLine4);
		this.grp_Documents.Location = new System.Drawing.Point(150, 17);
		this.grp_Documents.Name = "grp_Documents";
		this.grp_Documents.ShowCaption = false;
		this.grp_Documents.Size = new System.Drawing.Size(367, 248);
		this.grp_Documents.TabIndex = 3;
		this.grp_Documents.Text = "Documents";
		this.chk_APCreditMemo.EditControl = null;
		this.chk_APCreditMemo.FieldName = null;
		this.chk_APCreditMemo.IsSearchControl = false;
		this.chk_APCreditMemo.Location = new System.Drawing.Point(228, 207);
		this.chk_APCreditMemo.Name = "chk_APCreditMemo";
		this.chk_APCreditMemo.Properties.Caption = "A/P Credit Memo";
		this.chk_APCreditMemo.SettingInVisible = false;
		this.chk_APCreditMemo.SettingReadonly = false;
		this.chk_APCreditMemo.Size = new System.Drawing.Size(116, 20);
		this.chk_APCreditMemo.TabIndex = 1;
		this.chk_APCreditMemo.Tag = "19";
		this.chk_ARCreditMemo.EditControl = null;
		this.chk_ARCreditMemo.FieldName = null;
		this.chk_ARCreditMemo.IsSearchControl = false;
		this.chk_ARCreditMemo.Location = new System.Drawing.Point(29, 207);
		this.chk_ARCreditMemo.Name = "chk_ARCreditMemo";
		this.chk_ARCreditMemo.Properties.Caption = "A/R Credit Memo";
		this.chk_ARCreditMemo.SettingInVisible = false;
		this.chk_ARCreditMemo.SettingReadonly = false;
		this.chk_ARCreditMemo.Size = new System.Drawing.Size(116, 20);
		this.chk_ARCreditMemo.TabIndex = 1;
		this.chk_ARCreditMemo.Tag = "14";
		this.chk_APInvoice.EditControl = null;
		this.chk_APInvoice.FieldName = null;
		this.chk_APInvoice.IsSearchControl = false;
		this.chk_APInvoice.Location = new System.Drawing.Point(228, 181);
		this.chk_APInvoice.Name = "chk_APInvoice";
		this.chk_APInvoice.Properties.Caption = "A/P Invoice";
		this.chk_APInvoice.SettingInVisible = false;
		this.chk_APInvoice.SettingReadonly = false;
		this.chk_APInvoice.Size = new System.Drawing.Size(116, 20);
		this.chk_APInvoice.TabIndex = 1;
		this.chk_APInvoice.Tag = "18";
		this.chk_ARInvoice.EditControl = null;
		this.chk_ARInvoice.FieldName = null;
		this.chk_ARInvoice.IsSearchControl = false;
		this.chk_ARInvoice.Location = new System.Drawing.Point(29, 181);
		this.chk_ARInvoice.Name = "chk_ARInvoice";
		this.chk_ARInvoice.Properties.Caption = "A/R Invoice";
		this.chk_ARInvoice.SettingInVisible = false;
		this.chk_ARInvoice.SettingReadonly = false;
		this.chk_ARInvoice.Size = new System.Drawing.Size(116, 20);
		this.chk_ARInvoice.TabIndex = 1;
		this.chk_ARInvoice.Tag = "13";
		this.chk_APDownPaymnt.EditControl = null;
		this.chk_APDownPaymnt.FieldName = null;
		this.chk_APDownPaymnt.IsSearchControl = false;
		this.chk_APDownPaymnt.Location = new System.Drawing.Point(228, 155);
		this.chk_APDownPaymnt.Name = "chk_APDownPaymnt";
		this.chk_APDownPaymnt.Properties.Caption = "A/P Down Payment";
		this.chk_APDownPaymnt.SettingInVisible = false;
		this.chk_APDownPaymnt.SettingReadonly = false;
		this.chk_APDownPaymnt.Size = new System.Drawing.Size(116, 20);
		this.chk_APDownPaymnt.TabIndex = 1;
		this.chk_APDownPaymnt.Tag = "204";
		this.chk_ARDownPaymnt.EditControl = null;
		this.chk_ARDownPaymnt.FieldName = null;
		this.chk_ARDownPaymnt.IsSearchControl = false;
		this.chk_ARDownPaymnt.Location = new System.Drawing.Point(29, 155);
		this.chk_ARDownPaymnt.Name = "chk_ARDownPaymnt";
		this.chk_ARDownPaymnt.Properties.Caption = "A/R Down Payment";
		this.chk_ARDownPaymnt.SettingInVisible = false;
		this.chk_ARDownPaymnt.SettingReadonly = false;
		this.chk_ARDownPaymnt.Size = new System.Drawing.Size(116, 20);
		this.chk_ARDownPaymnt.TabIndex = 1;
		this.chk_ARDownPaymnt.Tag = "203";
		this.chk_GoodReturns.EditControl = null;
		this.chk_GoodReturns.FieldName = null;
		this.chk_GoodReturns.IsSearchControl = false;
		this.chk_GoodReturns.Location = new System.Drawing.Point(228, 129);
		this.chk_GoodReturns.Name = "chk_GoodReturns";
		this.chk_GoodReturns.Properties.Caption = "Good Returns";
		this.chk_GoodReturns.SettingInVisible = false;
		this.chk_GoodReturns.SettingReadonly = false;
		this.chk_GoodReturns.Size = new System.Drawing.Size(116, 20);
		this.chk_GoodReturns.TabIndex = 1;
		this.chk_GoodReturns.Tag = "21";
		this.chk_Returns.EditControl = null;
		this.chk_Returns.FieldName = null;
		this.chk_Returns.IsSearchControl = false;
		this.chk_Returns.Location = new System.Drawing.Point(29, 129);
		this.chk_Returns.Name = "chk_Returns";
		this.chk_Returns.Properties.Caption = "Returns";
		this.chk_Returns.SettingInVisible = false;
		this.chk_Returns.SettingReadonly = false;
		this.chk_Returns.Size = new System.Drawing.Size(116, 20);
		this.chk_Returns.TabIndex = 1;
		this.chk_Returns.Tag = "16";
		this.chk_GoodReturnReqr.EditControl = null;
		this.chk_GoodReturnReqr.FieldName = null;
		this.chk_GoodReturnReqr.IsSearchControl = false;
		this.chk_GoodReturnReqr.Location = new System.Drawing.Point(228, 103);
		this.chk_GoodReturnReqr.Name = "chk_GoodReturnReqr";
		this.chk_GoodReturnReqr.Properties.Caption = "Good Return Request";
		this.chk_GoodReturnReqr.SettingInVisible = false;
		this.chk_GoodReturnReqr.SettingReadonly = false;
		this.chk_GoodReturnReqr.Size = new System.Drawing.Size(139, 20);
		this.chk_GoodReturnReqr.TabIndex = 1;
		this.chk_GoodReturnReqr.Tag = "234000032";
		this.chk_ReturnReqr.EditControl = null;
		this.chk_ReturnReqr.FieldName = null;
		this.chk_ReturnReqr.IsSearchControl = false;
		this.chk_ReturnReqr.Location = new System.Drawing.Point(29, 103);
		this.chk_ReturnReqr.Name = "chk_ReturnReqr";
		this.chk_ReturnReqr.Properties.Caption = "Return Request";
		this.chk_ReturnReqr.SettingInVisible = false;
		this.chk_ReturnReqr.SettingReadonly = false;
		this.chk_ReturnReqr.Size = new System.Drawing.Size(116, 20);
		this.chk_ReturnReqr.TabIndex = 1;
		this.chk_ReturnReqr.Tag = "234000031";
		this.chk_GoodReceiptPO.EditControl = null;
		this.chk_GoodReceiptPO.FieldName = null;
		this.chk_GoodReceiptPO.IsSearchControl = false;
		this.chk_GoodReceiptPO.Location = new System.Drawing.Point(228, 77);
		this.chk_GoodReceiptPO.Name = "chk_GoodReceiptPO";
		this.chk_GoodReceiptPO.Properties.Caption = "Good Receipt PO";
		this.chk_GoodReceiptPO.SettingInVisible = false;
		this.chk_GoodReceiptPO.SettingReadonly = false;
		this.chk_GoodReceiptPO.Size = new System.Drawing.Size(116, 20);
		this.chk_GoodReceiptPO.TabIndex = 1;
		this.chk_GoodReceiptPO.Tag = "20";
		this.chk_Delivery.EditControl = null;
		this.chk_Delivery.FieldName = null;
		this.chk_Delivery.IsSearchControl = false;
		this.chk_Delivery.Location = new System.Drawing.Point(29, 77);
		this.chk_Delivery.Name = "chk_Delivery";
		this.chk_Delivery.Properties.Caption = "Delivery";
		this.chk_Delivery.SettingInVisible = false;
		this.chk_Delivery.SettingReadonly = false;
		this.chk_Delivery.Size = new System.Drawing.Size(116, 20);
		this.chk_Delivery.TabIndex = 1;
		this.chk_Delivery.Tag = "15";
		this.chk_PurchaseOrder.EditControl = null;
		this.chk_PurchaseOrder.FieldName = null;
		this.chk_PurchaseOrder.IsSearchControl = false;
		this.chk_PurchaseOrder.Location = new System.Drawing.Point(228, 51);
		this.chk_PurchaseOrder.Name = "chk_PurchaseOrder";
		this.chk_PurchaseOrder.Properties.Caption = "Purchase Order";
		this.chk_PurchaseOrder.SettingInVisible = false;
		this.chk_PurchaseOrder.SettingReadonly = false;
		this.chk_PurchaseOrder.Size = new System.Drawing.Size(116, 20);
		this.chk_PurchaseOrder.TabIndex = 1;
		this.chk_PurchaseOrder.Tag = "22";
		this.chk_SalesOrder.EditControl = null;
		this.chk_SalesOrder.FieldName = null;
		this.chk_SalesOrder.IsSearchControl = false;
		this.chk_SalesOrder.Location = new System.Drawing.Point(29, 51);
		this.chk_SalesOrder.Name = "chk_SalesOrder";
		this.chk_SalesOrder.Properties.Caption = "Sales Order";
		this.chk_SalesOrder.SettingInVisible = false;
		this.chk_SalesOrder.SettingReadonly = false;
		this.chk_SalesOrder.Size = new System.Drawing.Size(116, 20);
		this.chk_SalesOrder.TabIndex = 1;
		this.chk_SalesOrder.Tag = "17";
		this.chk_PurchaseQuo.EditControl = null;
		this.chk_PurchaseQuo.FieldName = null;
		this.chk_PurchaseQuo.IsSearchControl = false;
		this.chk_PurchaseQuo.Location = new System.Drawing.Point(228, 25);
		this.chk_PurchaseQuo.Name = "chk_PurchaseQuo";
		this.chk_PurchaseQuo.Properties.Caption = "Purchase Quotation";
		this.chk_PurchaseQuo.SettingInVisible = false;
		this.chk_PurchaseQuo.SettingReadonly = false;
		this.chk_PurchaseQuo.Size = new System.Drawing.Size(139, 20);
		this.chk_PurchaseQuo.TabIndex = 1;
		this.chk_PurchaseQuo.Tag = "540000006";
		this.chk_SalesQuotations.EditControl = null;
		this.chk_SalesQuotations.FieldName = null;
		this.chk_SalesQuotations.IsSearchControl = false;
		this.chk_SalesQuotations.Location = new System.Drawing.Point(29, 25);
		this.chk_SalesQuotations.Name = "chk_SalesQuotations";
		this.chk_SalesQuotations.Properties.Caption = "Sales Quotation";
		this.chk_SalesQuotations.SettingInVisible = false;
		this.chk_SalesQuotations.SettingReadonly = false;
		this.chk_SalesQuotations.Size = new System.Drawing.Size(116, 20);
		this.chk_SalesQuotations.TabIndex = 1;
		this.chk_SalesQuotations.Tag = "23";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Purchasing - A/R";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(215, 3);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(110, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 0;
		this.labelLine5.Utext = "Purchasing - A/R";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Sales - A/R";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(13, 3);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(110, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 0;
		this.labelLine4.Utext = "Sales - A/R";
		this.grp_Inventory.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.grp_Inventory.Controls.Add(this.chk_IVOpenBalance);
		this.grp_Inventory.Controls.Add(this.chk_IVTransfer);
		this.grp_Inventory.Controls.Add(this.chk_IVTransReqr);
		this.grp_Inventory.Controls.Add(this.chk_GoodIssue);
		this.grp_Inventory.Controls.Add(this.chk_GoodReceipt);
		this.grp_Inventory.Location = new System.Drawing.Point(538, 26);
		this.grp_Inventory.Name = "grp_Inventory";
		this.grp_Inventory.ShowCaption = false;
		this.grp_Inventory.Size = new System.Drawing.Size(223, 85);
		this.grp_Inventory.TabIndex = 7;
		this.grp_Inventory.Text = "Inventory";
		this.chk_IVOpenBalance.EditControl = null;
		this.chk_IVOpenBalance.FieldName = null;
		this.chk_IVOpenBalance.IsSearchControl = false;
		this.chk_IVOpenBalance.Location = new System.Drawing.Point(15, 103);
		this.chk_IVOpenBalance.Name = "chk_IVOpenBalance";
		this.chk_IVOpenBalance.Properties.Caption = "Inventory Opening Balance";
		this.chk_IVOpenBalance.SettingInVisible = false;
		this.chk_IVOpenBalance.SettingReadonly = false;
		this.chk_IVOpenBalance.Size = new System.Drawing.Size(198, 20);
		this.chk_IVOpenBalance.TabIndex = 1;
		this.chk_IVOpenBalance.Tag = "310000001";
		this.chk_IVTransfer.EditControl = null;
		this.chk_IVTransfer.FieldName = null;
		this.chk_IVTransfer.IsSearchControl = false;
		this.chk_IVTransfer.Location = new System.Drawing.Point(15, 77);
		this.chk_IVTransfer.Name = "chk_IVTransfer";
		this.chk_IVTransfer.Properties.Caption = "Inventory Tranfer";
		this.chk_IVTransfer.SettingInVisible = false;
		this.chk_IVTransfer.SettingReadonly = false;
		this.chk_IVTransfer.Size = new System.Drawing.Size(116, 20);
		this.chk_IVTransfer.TabIndex = 1;
		this.chk_IVTransfer.Tag = "67";
		this.chk_IVTransReqr.EditControl = null;
		this.chk_IVTransReqr.FieldName = null;
		this.chk_IVTransReqr.IsSearchControl = false;
		this.chk_IVTransReqr.Location = new System.Drawing.Point(15, 51);
		this.chk_IVTransReqr.Name = "chk_IVTransReqr";
		this.chk_IVTransReqr.Properties.Caption = "Inventory Tranfer Request";
		this.chk_IVTransReqr.SettingInVisible = false;
		this.chk_IVTransReqr.SettingReadonly = false;
		this.chk_IVTransReqr.Size = new System.Drawing.Size(175, 20);
		this.chk_IVTransReqr.TabIndex = 1;
		this.chk_IVTransReqr.Tag = "1250000001";
		this.chk_GoodIssue.EditControl = null;
		this.chk_GoodIssue.FieldName = null;
		this.chk_GoodIssue.IsSearchControl = false;
		this.chk_GoodIssue.Location = new System.Drawing.Point(15, 25);
		this.chk_GoodIssue.Name = "chk_GoodIssue";
		this.chk_GoodIssue.Properties.Caption = "Good Issue";
		this.chk_GoodIssue.SettingInVisible = false;
		this.chk_GoodIssue.SettingReadonly = false;
		this.chk_GoodIssue.Size = new System.Drawing.Size(116, 20);
		this.chk_GoodIssue.TabIndex = 1;
		this.chk_GoodIssue.Tag = "60";
		this.chk_GoodReceipt.EditControl = null;
		this.chk_GoodReceipt.FieldName = null;
		this.chk_GoodReceipt.IsSearchControl = false;
		this.chk_GoodReceipt.Location = new System.Drawing.Point(15, 0);
		this.chk_GoodReceipt.Name = "chk_GoodReceipt";
		this.chk_GoodReceipt.Properties.Caption = "Good Receipt";
		this.chk_GoodReceipt.SettingInVisible = false;
		this.chk_GoodReceipt.SettingReadonly = false;
		this.chk_GoodReceipt.Size = new System.Drawing.Size(116, 20);
		this.chk_GoodReceipt.TabIndex = 1;
		this.chk_GoodReceipt.Tag = "59";
		this.grp_Payment.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.grp_Payment.Controls.Add(this.chk_OutgoingPaymnt);
		this.grp_Payment.Location = new System.Drawing.Point(523, 123);
		this.grp_Payment.Name = "grp_Payment";
		this.grp_Payment.ShowCaption = false;
		this.grp_Payment.Size = new System.Drawing.Size(127, 69);
		this.grp_Payment.TabIndex = 2;
		this.grp_Payment.Text = "Inventory";
		this.chk_OutgoingPaymnt.EditControl = null;
		this.chk_OutgoingPaymnt.FieldName = null;
		this.chk_OutgoingPaymnt.IsSearchControl = false;
		this.chk_OutgoingPaymnt.Location = new System.Drawing.Point(15, 2);
		this.chk_OutgoingPaymnt.Name = "chk_OutgoingPaymnt";
		this.chk_OutgoingPaymnt.Properties.Caption = "Outgoing Payment";
		this.chk_OutgoingPaymnt.SettingInVisible = false;
		this.chk_OutgoingPaymnt.SettingReadonly = false;
		this.chk_OutgoingPaymnt.Size = new System.Drawing.Size(169, 20);
		this.chk_OutgoingPaymnt.TabIndex = 1;
		this.chk_OutgoingPaymnt.Tag = "46";
		this.grp_IVCoutingTrans.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.grp_IVCoutingTrans.Controls.Add(this.chk_IVPosting);
		this.grp_IVCoutingTrans.Controls.Add(this.chk_IVCounting);
		this.grp_IVCoutingTrans.Location = new System.Drawing.Point(35, 20);
		this.grp_IVCoutingTrans.Name = "grp_IVCoutingTrans";
		this.grp_IVCoutingTrans.ShowCaption = false;
		this.grp_IVCoutingTrans.Size = new System.Drawing.Size(92, 54);
		this.grp_IVCoutingTrans.TabIndex = 4;
		this.grp_IVCoutingTrans.Text = "Inventory";
		this.chk_IVPosting.EditControl = null;
		this.chk_IVPosting.FieldName = null;
		this.chk_IVPosting.IsSearchControl = false;
		this.chk_IVPosting.Location = new System.Drawing.Point(15, 26);
		this.chk_IVPosting.Name = "chk_IVPosting";
		this.chk_IVPosting.Properties.Caption = "Inventory Posting";
		this.chk_IVPosting.SettingInVisible = false;
		this.chk_IVPosting.SettingReadonly = false;
		this.chk_IVPosting.Size = new System.Drawing.Size(116, 20);
		this.chk_IVPosting.TabIndex = 1;
		this.chk_IVPosting.Tag = "10000071";
		this.chk_IVCounting.EditControl = null;
		this.chk_IVCounting.FieldName = null;
		this.chk_IVCounting.IsSearchControl = false;
		this.chk_IVCounting.Location = new System.Drawing.Point(15, 3);
		this.chk_IVCounting.Name = "chk_IVCounting";
		this.chk_IVCounting.Properties.Caption = "Inventory Couting";
		this.chk_IVCounting.SettingInVisible = false;
		this.chk_IVCounting.SettingReadonly = false;
		this.chk_IVCounting.Size = new System.Drawing.Size(116, 20);
		this.chk_IVCounting.TabIndex = 1;
		this.chk_IVCounting.Tag = "1470000065";
		this.rd_Groups.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.rd_Groups.EditControl = null;
		this.rd_Groups.EditValue = "Documents";
		this.rd_Groups.FieldName = null;
		this.rd_Groups.IsSearchControl = false;
		this.rd_Groups.Location = new System.Drawing.Point(3, 4);
		this.rd_Groups.Name = "rd_Groups";
		this.rd_Groups.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.rd_Groups.Properties.Appearance.Options.UseBackColor = true;
		this.rd_Groups.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.rd_Groups.Properties.ColumnIndent = 3;
		this.rd_Groups.Properties.Columns = 6;
		this.rd_Groups.Properties.ItemHorzAlignment = DevExpress.XtraEditors.RadioItemHorzAlignment.Near;
		this.rd_Groups.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[6]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("Documents", "Documents"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Inventory"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Blanket Agreement"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Internal Requistion"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Payment"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Inventory Counting Transaction")
		});
		this.rd_Groups.Properties.Padding = new System.Windows.Forms.Padding(4, 4, 6, 4);
		this.rd_Groups.SettingInVisible = false;
		this.rd_Groups.SettingReadonly = false;
		this.rd_Groups.Size = new System.Drawing.Size(907, 26);
		this.rd_Groups.TabIndex = 0;
		this.rd_Groups.SelectedIndexChanged += new System.EventHandler(this.radioGroupEx1_SelectedIndexChanged);
		this.tabPage_Stages.Controls.Add(this.btn_MoveDown);
		this.tabPage_Stages.Controls.Add(this.btn_MoveUp);
		this.tabPage_Stages.Controls.Add(this.grd_Stages);
		this.tabPage_Stages.EditControl = null;
		this.tabPage_Stages.EditValue = "Stages";
		this.tabPage_Stages.FieldName = "";
		this.tabPage_Stages.IsSearchControl = false;
		this.tabPage_Stages.Name = "tabPage_Stages";
		this.tabPage_Stages.ReadOnly = false;
		this.tabPage_Stages.SettingInVisible = false;
		this.tabPage_Stages.SettingReadonly = false;
		this.tabPage_Stages.Size = new System.Drawing.Size(916, 435);
		this.tabPage_Stages.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPage_Stages.Text = "Stages";
		this.btn_MoveDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_MoveDown.EditControl = null;
		this.btn_MoveDown.EditValue = "";
		this.btn_MoveDown.FieldName = null;
		this.btn_MoveDown.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_MoveDown.ImageOptions.Image");
		this.btn_MoveDown.IsSearchControl = false;
		this.btn_MoveDown.Location = new System.Drawing.Point(889, 28);
		this.btn_MoveDown.Name = "btn_MoveDown";
		this.btn_MoveDown.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
		this.btn_MoveDown.ReadOnly = false;
		this.btn_MoveDown.SettingInVisible = false;
		this.btn_MoveDown.SettingReadonly = false;
		this.btn_MoveDown.Size = new System.Drawing.Size(22, 22);
		this.btn_MoveDown.TabIndex = 1;
		this.btn_MoveDown.Click += new System.EventHandler(this.btn_MoveDown_Click);
		this.btn_MoveUp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_MoveUp.EditControl = null;
		this.btn_MoveUp.EditValue = "";
		this.btn_MoveUp.FieldName = null;
		this.btn_MoveUp.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_MoveUp.ImageOptions.Image");
		this.btn_MoveUp.IsSearchControl = false;
		this.btn_MoveUp.Location = new System.Drawing.Point(889, 4);
		this.btn_MoveUp.Name = "btn_MoveUp";
		this.btn_MoveUp.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
		this.btn_MoveUp.ReadOnly = false;
		this.btn_MoveUp.SettingInVisible = false;
		this.btn_MoveUp.SettingReadonly = false;
		this.btn_MoveUp.Size = new System.Drawing.Size(22, 20);
		this.btn_MoveUp.TabIndex = 1;
		this.btn_MoveUp.Click += new System.EventHandler(this.btn_MoveUp_Click);
		this.grd_Stages.AllowPasteExcel = false;
		this.grd_Stages.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Stages.ClipboardData = null;
		this.grd_Stages.ContextMenuStrip = this.contextMenuStrip3;
		this.grd_Stages.Location = new System.Drawing.Point(0, 0);
		this.grd_Stages.MainView = this.gv_Stages;
		this.grd_Stages.Name = "grd_Stages";
		this.grd_Stages.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_Stages });
		this.grd_Stages.Size = new System.Drawing.Size(886, 431);
		this.grd_Stages.TabIndex = 0;
		this.grd_Stages.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Stages });
		this.gv_Stages.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Stages.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Stages.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Stages.ColumnKey = null;
		this.gv_Stages.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gc_WtmCode1, this.gc_WstCode, this.gc_WstName, this.gc_WstRemarks, this.gc_SortID });
		this.gv_Stages.DisableFormSetting = false;
		this.gv_Stages.DisplayException = false;
		this.gv_Stages.EditColumn = null;
		this.gv_Stages.FieldCurrency = null;
		this.gv_Stages.FieldRate = null;
		this.gv_Stages.FormId = "oApprovalTemplates";
		this.gv_Stages.GridControl = this.grd_Stages;
		this.gv_Stages.IgnoreFilter = false;
		this.gv_Stages.IndexEditRow = -1;
		this.gv_Stages.IndicatorWidth = 35;
		this.gv_Stages.IsApzTable = false;
		this.gv_Stages.IsInsertNewRow = false;
		this.gv_Stages.IsLoadingData = false;
		this.gv_Stages.IsMappingContextAction = false;
		this.gv_Stages.ItemId = "WTM2";
		this.gv_Stages.LstColumnAllowBatch = null;
		this.gv_Stages.LstColumnNotAllowBatch = null;
		this.gv_Stages.LstColumnNotEditable = null;
		this.gv_Stages.LstColumNotSetting = null;
		this.gv_Stages.Name = "gv_Stages";
		this.gv_Stages.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Stages.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Stages.OptionsNavigation.AutoFocusNewRow = true;
		this.gv_Stages.OptionsPrint.AutoWidth = false;
		this.gv_Stages.OptionsPrint.UsePrintStyles = false;
		this.gv_Stages.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Stages.OptionsView.ColumnAutoWidth = false;
		this.gv_Stages.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Stages.OptionsView.ShowGroupedColumns = true;
		this.gv_Stages.OptionsView.ShowGroupPanel = false;
		this.gv_Stages.UseBatchMod = false;
		this.gv_Stages.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Stages_CellValueChanged);
		this.gc_WtmCode1.Caption = "Template Code";
		this.gc_WtmCode1.CurrencyDateField = null;
		this.gc_WtmCode1.CurrencyField = null;
		this.gc_WtmCode1.CurrencyFixed = null;
		this.gc_WtmCode1.CurrencyFixedRate = 0.0;
		this.gc_WtmCode1.CurrencyRateField = null;
		this.gc_WtmCode1.FieldName = "WtmCode";
		this.gc_WtmCode1.Name = "gc_WtmCode1";
		this.gc_WtmCode1.OptionsColumn.ReadOnly = true;
		this.gc_WtmCode1.PrimaryKey = false;
		this.gc_WtmCode1.UnitField = null;
		this.gc_WstCode.Caption = "Stage Code";
		this.gc_WstCode.CurrencyDateField = null;
		this.gc_WstCode.CurrencyField = null;
		this.gc_WstCode.CurrencyFixed = null;
		this.gc_WstCode.CurrencyFixedRate = 0.0;
		this.gc_WstCode.CurrencyRateField = null;
		this.gc_WstCode.FieldName = "WstCode";
		this.gc_WstCode.Name = "gc_WstCode";
		this.gc_WstCode.OptionsColumn.ReadOnly = true;
		this.gc_WstCode.PrimaryKey = false;
		this.gc_WstCode.UnitField = null;
		this.gc_WstName.Caption = "Stage Name";
		this.gc_WstName.ColumnEdit = this.rep_Stages;
		this.gc_WstName.CurrencyDateField = null;
		this.gc_WstName.CurrencyField = null;
		this.gc_WstName.CurrencyFixed = null;
		this.gc_WstName.CurrencyFixedRate = 0.0;
		this.gc_WstName.CurrencyRateField = null;
		this.gc_WstName.FieldName = "WstName";
		this.gc_WstName.Name = "gc_WstName";
		this.gc_WstName.PrimaryKey = false;
		this.gc_WstName.UnitField = null;
		this.gc_WstName.Visible = true;
		this.gc_WstName.VisibleIndex = 0;
		this.gc_WstName.Width = 125;
		this.rep_Stages.AutoHeight = false;
		this.rep_Stages.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Stages.Name = "rep_Stages";
		this.rep_Stages.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Stages_ButtonClick);
		this.gc_WstRemarks.Caption = "Stage Descriptions";
		this.gc_WstRemarks.CurrencyDateField = null;
		this.gc_WstRemarks.CurrencyField = null;
		this.gc_WstRemarks.CurrencyFixed = null;
		this.gc_WstRemarks.CurrencyFixedRate = 0.0;
		this.gc_WstRemarks.CurrencyRateField = null;
		this.gc_WstRemarks.FieldName = "WstRemarks";
		this.gc_WstRemarks.Name = "gc_WstRemarks";
		this.gc_WstRemarks.PrimaryKey = false;
		this.gc_WstRemarks.UnitField = null;
		this.gc_WstRemarks.Visible = true;
		this.gc_WstRemarks.VisibleIndex = 1;
		this.gc_WstRemarks.Width = 125;
		this.gc_SortID.Caption = "Sort ID";
		this.gc_SortID.CurrencyDateField = null;
		this.gc_SortID.CurrencyField = null;
		this.gc_SortID.CurrencyFixed = null;
		this.gc_SortID.CurrencyFixedRate = 0.0;
		this.gc_SortID.CurrencyRateField = null;
		this.gc_SortID.FieldName = "SortId";
		this.gc_SortID.Name = "gc_SortID";
		this.gc_SortID.OptionsColumn.ReadOnly = true;
		this.gc_SortID.PrimaryKey = false;
		this.gc_SortID.UnitField = null;
		this.tabPage_Terms.Controls.Add(this.panelControl3);
		this.tabPage_Terms.Controls.Add(this.rd_LaunchAP);
		this.tabPage_Terms.Controls.Add(this.lbl_LaunchAP);
		this.tabPage_Terms.EditControl = null;
		this.tabPage_Terms.EditValue = "Terms";
		this.tabPage_Terms.FieldName = "";
		this.tabPage_Terms.IsSearchControl = false;
		this.tabPage_Terms.Name = "tabPage_Terms";
		this.tabPage_Terms.ReadOnly = false;
		this.tabPage_Terms.SettingInVisible = false;
		this.tabPage_Terms.SettingReadonly = false;
		this.tabPage_Terms.Size = new System.Drawing.Size(916, 435);
		this.tabPage_Terms.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPage_Terms.Text = "Terms";
		this.panelControl3.AllowTouchScroll = true;
		this.panelControl3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.panelControl3.Controls.Add(this.lbl_Totals);
		this.panelControl3.Controls.Add(this.splitContainerControl2);
		this.panelControl3.Controls.Add(this.lbl_TotalTerms);
		this.panelControl3.Location = new System.Drawing.Point(1, 77);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(914, 348);
		this.panelControl3.TabIndex = 2;
		this.lbl_Totals.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbl_Totals.EditControl = null;
		this.lbl_Totals.EditValue = "0";
		this.lbl_Totals.FieldName = null;
		this.lbl_Totals.Location = new System.Drawing.Point(144, 327);
		this.lbl_Totals.Name = "lbl_Totals";
		this.lbl_Totals.ReadOnly = false;
		this.lbl_Totals.SettingInVisible = false;
		this.lbl_Totals.SettingReadonly = false;
		this.lbl_Totals.Size = new System.Drawing.Size(6, 13);
		this.lbl_Totals.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Totals.TabIndex = 7;
		this.lbl_Totals.Text = "0";
		this.splitContainerControl2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 1);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.grd_Terms);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.grd_Query);
		this.splitContainerControl2.Panel2.Controls.Add(this.labelLine2);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(915, 318);
		this.splitContainerControl2.SplitterPosition = 147;
		this.splitContainerControl2.TabIndex = 0;
		this.grd_Terms.AllowPasteExcel = false;
		this.grd_Terms.ClipboardData = null;
		this.grd_Terms.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Terms.Location = new System.Drawing.Point(0, 0);
		this.grd_Terms.MainView = this.gv_Terms;
		this.grd_Terms.Name = "grd_Terms";
		this.grd_Terms.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_Choose, this.rep_Ratio });
		this.grd_Terms.Size = new System.Drawing.Size(915, 147);
		this.grd_Terms.TabIndex = 0;
		this.grd_Terms.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Terms });
		this.gv_Terms.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Terms.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Terms.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Terms.ColumnKey = null;
		this.gv_Terms.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gc_Choose, this.gc_TermsId, this.gc_TermName, this.gc_Ratio, this.gc_Value });
		this.gv_Terms.DisableFormSetting = false;
		this.gv_Terms.DisplayException = false;
		this.gv_Terms.EditColumn = null;
		this.gv_Terms.FieldCurrency = null;
		this.gv_Terms.FieldRate = null;
		this.gv_Terms.FormId = "oApprovalTemplates";
		this.gv_Terms.GridControl = this.grd_Terms;
		this.gv_Terms.IgnoreFilter = false;
		this.gv_Terms.IndexEditRow = -1;
		this.gv_Terms.IndicatorWidth = 35;
		this.gv_Terms.IsApzTable = false;
		this.gv_Terms.IsInsertNewRow = false;
		this.gv_Terms.IsLoadingData = false;
		this.gv_Terms.IsMappingContextAction = false;
		this.gv_Terms.ItemId = "WTM4";
		this.gv_Terms.LstColumnAllowBatch = null;
		this.gv_Terms.LstColumnNotAllowBatch = null;
		this.gv_Terms.LstColumnNotEditable = null;
		this.gv_Terms.LstColumNotSetting = null;
		this.gv_Terms.Name = "gv_Terms";
		this.gv_Terms.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Terms.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Terms.OptionsPrint.AutoWidth = false;
		this.gv_Terms.OptionsPrint.UsePrintStyles = false;
		this.gv_Terms.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Terms.OptionsView.ColumnAutoWidth = false;
		this.gv_Terms.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Terms.OptionsView.ShowGroupedColumns = true;
		this.gv_Terms.OptionsView.ShowGroupPanel = false;
		this.gv_Terms.UseBatchMod = false;
		this.gv_Terms.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Terms_ShowingEditor);
		this.gv_Terms.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Terms_CellValueChanged);
		this.gc_Choose.Caption = "Choose";
		this.gc_Choose.ColumnEdit = this.rep_Choose;
		this.gc_Choose.CurrencyDateField = null;
		this.gc_Choose.CurrencyField = null;
		this.gc_Choose.CurrencyFixed = null;
		this.gc_Choose.CurrencyFixedRate = 0.0;
		this.gc_Choose.CurrencyRateField = null;
		this.gc_Choose.FieldName = "Checked";
		this.gc_Choose.Name = "gc_Choose";
		this.gc_Choose.PrimaryKey = false;
		this.gc_Choose.UnitField = null;
		this.gc_Choose.Visible = true;
		this.gc_Choose.VisibleIndex = 0;
		this.gc_Choose.Width = 50;
		this.rep_Choose.AutoHeight = false;
		this.rep_Choose.DisplayValueChecked = "Y";
		this.rep_Choose.DisplayValueGrayed = "N";
		this.rep_Choose.DisplayValueUnchecked = "N";
		this.rep_Choose.Name = "rep_Choose";
		this.rep_Choose.ValueChecked = "Y";
		this.rep_Choose.ValueGrayed = "N";
		this.rep_Choose.ValueUnchecked = "N";
		this.gc_TermsId.Caption = "Term Id";
		this.gc_TermsId.CurrencyDateField = null;
		this.gc_TermsId.CurrencyField = null;
		this.gc_TermsId.CurrencyFixed = null;
		this.gc_TermsId.CurrencyFixedRate = 0.0;
		this.gc_TermsId.CurrencyRateField = null;
		this.gc_TermsId.FieldName = "CondId";
		this.gc_TermsId.Name = "gc_TermsId";
		this.gc_TermsId.OptionsColumn.ReadOnly = true;
		this.gc_TermsId.PrimaryKey = false;
		this.gc_TermsId.UnitField = null;
		this.gc_TermName.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(219, 219, 219);
		this.gc_TermName.AppearanceCell.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.gc_TermName.AppearanceCell.Options.UseBackColor = true;
		this.gc_TermName.AppearanceCell.Options.UseFont = true;
		this.gc_TermName.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.gc_TermName.AppearanceHeader.Options.UseFont = true;
		this.gc_TermName.Caption = "Term";
		this.gc_TermName.CurrencyDateField = null;
		this.gc_TermName.CurrencyField = null;
		this.gc_TermName.CurrencyFixed = null;
		this.gc_TermName.CurrencyFixedRate = 0.0;
		this.gc_TermName.CurrencyRateField = null;
		this.gc_TermName.FieldName = "CondName";
		this.gc_TermName.Name = "gc_TermName";
		this.gc_TermName.OptionsColumn.AllowEdit = false;
		this.gc_TermName.OptionsColumn.ReadOnly = true;
		this.gc_TermName.PrimaryKey = false;
		this.gc_TermName.UnitField = null;
		this.gc_TermName.Visible = true;
		this.gc_TermName.VisibleIndex = 1;
		this.gc_TermName.Width = 170;
		this.gc_Ratio.Caption = "Ratio";
		this.gc_Ratio.ColumnEdit = this.rep_Ratio;
		this.gc_Ratio.CurrencyDateField = null;
		this.gc_Ratio.CurrencyField = null;
		this.gc_Ratio.CurrencyFixed = null;
		this.gc_Ratio.CurrencyFixedRate = 0.0;
		this.gc_Ratio.CurrencyRateField = null;
		this.gc_Ratio.FieldName = "opCode";
		this.gc_Ratio.Name = "gc_Ratio";
		this.gc_Ratio.PrimaryKey = false;
		this.gc_Ratio.UnitField = null;
		this.gc_Ratio.Visible = true;
		this.gc_Ratio.VisibleIndex = 2;
		this.gc_Ratio.Width = 125;
		this.rep_Ratio.AutoHeight = false;
		this.rep_Ratio.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Ratio.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name")
		});
		this.rep_Ratio.DisplayMember = "Value";
		this.rep_Ratio.Name = "rep_Ratio";
		this.rep_Ratio.NullText = "";
		this.rep_Ratio.ShowHeader = false;
		this.rep_Ratio.ValueMember = "Key";
		this.gc_Value.AppearanceCell.Options.UseTextOptions = true;
		this.gc_Value.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.gc_Value.Caption = "Value";
		this.gc_Value.CurrencyDateField = null;
		this.gc_Value.CurrencyField = null;
		this.gc_Value.CurrencyFixed = null;
		this.gc_Value.CurrencyFixedRate = 0.0;
		this.gc_Value.CurrencyRateField = null;
		this.gc_Value.FieldName = "opValue";
		this.gc_Value.Name = "gc_Value";
		this.gc_Value.PrimaryKey = false;
		this.gc_Value.UnitField = null;
		this.gc_Value.Visible = true;
		this.gc_Value.VisibleIndex = 3;
		this.grd_Query.AllowPasteExcel = false;
		this.grd_Query.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Query.ClipboardData = null;
		this.grd_Query.ContextMenuStrip = this.contextMenuStrip3;
		this.grd_Query.Location = new System.Drawing.Point(0, 26);
		this.grd_Query.MainView = this.gv_Query;
		this.grd_Query.Name = "grd_Query";
		this.grd_Query.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_Query });
		this.grd_Query.Size = new System.Drawing.Size(914, 135);
		this.grd_Query.TabIndex = 2;
		this.grd_Query.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Query });
		this.gv_Query.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Query.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Query.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Query.ColumnKey = null;
		this.gv_Query.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gc_QueryId, this.gc_QueryName });
		this.gv_Query.DisableFormSetting = false;
		this.gv_Query.DisplayException = false;
		this.gv_Query.EditColumn = null;
		this.gv_Query.FieldCurrency = null;
		this.gv_Query.FieldRate = null;
		this.gv_Query.FormId = "oApprovalTemplates";
		this.gv_Query.GridControl = this.grd_Query;
		this.gv_Query.IgnoreFilter = false;
		this.gv_Query.IndexEditRow = -1;
		this.gv_Query.IndicatorWidth = 35;
		this.gv_Query.IsApzTable = false;
		this.gv_Query.IsInsertNewRow = false;
		this.gv_Query.IsLoadingData = false;
		this.gv_Query.IsMappingContextAction = false;
		this.gv_Query.ItemId = "WTM5";
		this.gv_Query.LstColumnAllowBatch = null;
		this.gv_Query.LstColumnNotAllowBatch = null;
		this.gv_Query.LstColumnNotEditable = null;
		this.gv_Query.LstColumNotSetting = null;
		this.gv_Query.Name = "gv_Query";
		this.gv_Query.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Query.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Query.OptionsNavigation.AutoFocusNewRow = true;
		this.gv_Query.OptionsPrint.AutoWidth = false;
		this.gv_Query.OptionsPrint.UsePrintStyles = false;
		this.gv_Query.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Query.OptionsView.ColumnAutoWidth = false;
		this.gv_Query.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Query.OptionsView.ShowGroupedColumns = true;
		this.gv_Query.OptionsView.ShowGroupPanel = false;
		this.gv_Query.UseBatchMod = false;
		this.gc_QueryId.Caption = "Query Id";
		this.gc_QueryId.CurrencyDateField = null;
		this.gc_QueryId.CurrencyField = null;
		this.gc_QueryId.CurrencyFixed = null;
		this.gc_QueryId.CurrencyFixedRate = 0.0;
		this.gc_QueryId.CurrencyRateField = null;
		this.gc_QueryId.FieldName = "QueryId";
		this.gc_QueryId.Name = "gc_QueryId";
		this.gc_QueryId.OptionsColumn.ReadOnly = true;
		this.gc_QueryId.PrimaryKey = false;
		this.gc_QueryId.UnitField = null;
		this.gc_QueryName.Caption = "Query Name";
		this.gc_QueryName.ColumnEdit = this.rep_Query;
		this.gc_QueryName.CurrencyDateField = null;
		this.gc_QueryName.CurrencyField = null;
		this.gc_QueryName.CurrencyFixed = null;
		this.gc_QueryName.CurrencyFixedRate = 0.0;
		this.gc_QueryName.CurrencyRateField = null;
		this.gc_QueryName.FieldName = "QueryName";
		this.gc_QueryName.Name = "gc_QueryName";
		this.gc_QueryName.OptionsColumn.ReadOnly = true;
		this.gc_QueryName.PrimaryKey = false;
		this.gc_QueryName.UnitField = null;
		this.gc_QueryName.Visible = true;
		this.gc_QueryName.VisibleIndex = 0;
		this.gc_QueryName.Width = 125;
		this.rep_Query.AutoHeight = false;
		this.rep_Query.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Query.Name = "rep_Query";
		this.rep_Query.ReadOnly = true;
		this.rep_Query.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Query_ButtonClick);
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Term Based on User Queries";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(3, 4);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(159, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 6;
		this.labelLine2.Utext = "Term Based on User Queries";
		this.lbl_TotalTerms.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbl_TotalTerms.EditControl = null;
		this.lbl_TotalTerms.EditValue = "Total Selected Terms:";
		this.lbl_TotalTerms.FieldName = null;
		this.lbl_TotalTerms.HideWhenEmpty = false;
		this.lbl_TotalTerms.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_TotalTerms.Location = new System.Drawing.Point(7, 325);
		this.lbl_TotalTerms.Name = "lbl_TotalTerms";
		this.lbl_TotalTerms.ReadOnly = false;
		this.lbl_TotalTerms.SettingInVisible = false;
		this.lbl_TotalTerms.SettingReadonly = false;
		this.lbl_TotalTerms.ShowUnderLine = true;
		this.lbl_TotalTerms.Size = new System.Drawing.Size(135, 20);
		this.lbl_TotalTerms.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_TotalTerms.TabIndex = 6;
		this.lbl_TotalTerms.Utext = "Total Selected Terms:";
		this.rd_LaunchAP.EditControl = null;
		this.rd_LaunchAP.FieldName = "Conds";
		this.rd_LaunchAP.IsSearchControl = false;
		this.rd_LaunchAP.Location = new System.Drawing.Point(8, 27);
		this.rd_LaunchAP.Name = "rd_LaunchAP";
		this.rd_LaunchAP.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.rd_LaunchAP.Properties.Appearance.Options.UseBackColor = true;
		this.rd_LaunchAP.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.rd_LaunchAP.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Always"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "When the Following applies")
		});
		this.rd_LaunchAP.SettingInVisible = false;
		this.rd_LaunchAP.SettingReadonly = false;
		this.rd_LaunchAP.Size = new System.Drawing.Size(281, 57);
		this.rd_LaunchAP.TabIndex = 1;
		this.rd_LaunchAP.SelectedIndexChanged += new System.EventHandler(this.rd_LaunchAP_SelectedIndexChanged);
		this.lbl_LaunchAP.EditControl = null;
		this.lbl_LaunchAP.EditValue = "Launch Approval Process";
		this.lbl_LaunchAP.FieldName = null;
		this.lbl_LaunchAP.HideWhenEmpty = false;
		this.lbl_LaunchAP.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_LaunchAP.Location = new System.Drawing.Point(8, 8);
		this.lbl_LaunchAP.Name = "lbl_LaunchAP";
		this.lbl_LaunchAP.ReadOnly = false;
		this.lbl_LaunchAP.SettingInVisible = false;
		this.lbl_LaunchAP.SettingReadonly = false;
		this.lbl_LaunchAP.ShowUnderLine = true;
		this.lbl_LaunchAP.Size = new System.Drawing.Size(160, 20);
		this.lbl_LaunchAP.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_LaunchAP.TabIndex = 0;
		this.lbl_LaunchAP.Utext = "Launch Approval Process";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(763, 603);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 20);
		this.btn_OK.TabIndex = 7;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(842, 603);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 8;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(922, 633);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oApprovalTemplates";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ApprovalTemplates";
		this.Text = "Approval Templates";
		base.OnControlValueChanged += new Apzon.Controls.Form.ControlValueChanged(this.frm_ApprovalTemplates_OnControlValueChanged);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_ApprovalTemplates_FormClosing);
		base.Load += new System.EventHandler(this.frm_ApprovalTemplates_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_Remarks.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ActiveUpdateDoc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl_Main).EndInit();
		this.tabControl_Main.ResumeLayout(false);
		this.tabPage_Originator.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Originator).EndInit();
		this.contextMenuStrip3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Originator).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Originator).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Departmnt).EndInit();
		this.tabPage_Documents.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grp_BlanketAgreement).EndInit();
		this.grp_BlanketAgreement.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_PurchaseBlanketAgrmnt.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_SalesBlanketAgrmnt.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grp_InternalRequis).EndInit();
		this.grp_InternalRequis.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_PurchaseReqr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grp_Documents).EndInit();
		this.grp_Documents.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_APCreditMemo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ARCreditMemo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_APInvoice.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ARInvoice.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_APDownPaymnt.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ARDownPaymnt.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_GoodReturns.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Returns.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_GoodReturnReqr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ReturnReqr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_GoodReceiptPO.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Delivery.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PurchaseOrder.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_SalesOrder.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PurchaseQuo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_SalesQuotations.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grp_Inventory).EndInit();
		this.grp_Inventory.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_IVOpenBalance.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_IVTransfer.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_IVTransReqr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_GoodIssue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_GoodReceipt.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grp_Payment).EndInit();
		this.grp_Payment.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_OutgoingPaymnt.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grp_IVCoutingTrans).EndInit();
		this.grp_IVCoutingTrans.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_IVPosting.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_IVCounting.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rd_Groups.Properties).EndInit();
		this.tabPage_Stages.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Stages).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Stages).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Stages).EndInit();
		this.tabPage_Terms.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		this.panelControl3.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Terms).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Terms).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Choose).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Ratio).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Query).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Query).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Query).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rd_LaunchAP.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
