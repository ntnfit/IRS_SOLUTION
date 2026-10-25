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
using Apzon.Controls.Tree;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using SAPbobsCOM;

namespace Apzon.Forms.Sales;

public class frm_SalesControl : XtraFormEx
{
	private const string FormController = "SalesControl";

	private List<UserFieldsImpl> _lstUDFLine;

	private List<UserFieldsImpl> _lstUDFHeader;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private HttpClientHelper<DataTable> _dtClientHelper;

	private DataTable dtSearch;

	private IContainer components;

	private PanelControl panelControl1;

	private PanelControl panelControl2;

	private SimpleButtonEx btn_Search;

	private LabelLine lb_BusinessPartner;

	private LabelLine lb_NoOfTrans;

	private LabelLine lb_OwnerName;

	private ButtonEdit bte_BPCode;

	private LabelLine labelLine1;

	private LabelLine lb_SalesEmp;

	private XtraTabControlEx xtraTab_SalesControl;

	private XtraTabPageEx tab_SalesQuotation;

	private TreeEx tre_SalesQuotation;

	private XtraTabPageEx tab_SalesOrder;

	private TreeEx tre_SalesOrder;

	private XtraTabPageEx tab_SalesReturn;

	private TreeEx tre_SalesReturn;

	private XtraTabPageEx tab_SalesContract;

	private TreeEx tre_SalesContract;

	private LookUpEdit lue_SalesEmp;

	private SimpleButton btn_AddSalesOrder;

	private LookUpEdit lue_OwnerName;

	private SimpleButton btn_CancelSO;

	private PanelControl panelControl3;

	private PanelControl panelControl4;

	private CheckedComboBoxEdit chb_Status;

	private LabelLine labelLine2;

	private XtraTabPageEx tab_SalesProduction;

	private TreeEx tre_SalesProduction;

	private SimpleButton btn_CloseSO;

	private DateEditEx date_FromDate;

	private DateEditEx date_ToDate;

	private SimpleButton btn_Approval;

	private SimpleButton btn_Reject;

	public frm_SalesControl()
	{
		InitializeComponent();
	}

	private async void frm_SalesControl_Load(object sender, EventArgs e)
	{
		_ = 4;
		try
		{
			btn_AddSalesOrder.Visible = true;
			tab_SalesContract.PageVisible = false;
			date_FromDate.EditValue = DateTime.Today;
			date_ToDate.EditValue = DateTime.Today;
			chb_Status.Properties.Items.Clear();
			chb_Status.Properties.Items.Add("O", "Open");
			chb_Status.Properties.Items.Add("C", "Closed");
			chb_Status.Properties.Items.Add("N", "Canceled");
			chb_Status.SetEditValue("O");
			_lstUDFHeader = await _httpClient.Udf.GetListField("ORDR");
			_lstUDFLine = await _httpClient.Udf.GetListField("RDR1");
			await LoadEmployees();
			await LoadComboSalesGroup();
			await Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		await Search();
	}

	private async void xtraTab_SalesControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		await Search();
	}

	private async void btn_AddSalesOrder_Click(object sender, EventArgs e)
	{
		_ = 12;
		try
		{
			DataRow row = ((DataRowView)tre_SalesQuotation.GetDataRecordByNode(tre_SalesQuotation.FocusedNode)).Row;
			if (Function.ToString(row["Approval"]).Equals("W"))
			{
				XtraMessageBoxEx.Show(Enums.MessageCode.InfoWaitApproval, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				return;
			}
			if (Function.ToString(row["Approval"]).Equals("Y"))
			{
				int draftKey = Function.ParseInt(row["DraftKey"]);
				if (draftKey <= 0)
				{
					return;
				}
				btn_AddSalesOrder.Enabled = false;
				btn_Search.Enabled = false;
				WinCommons._OpenProcessing();
				DocumentTable tableDocDraft = await ExtenseionMethods.GetDocCopy(draftKey, 112.ToString(), 17.ToString());
				DocumentLineTable documentLineTable = await ExtenseionMethods.GetDocLineCopy(draftKey, 112.ToString(), 17.ToString());
				new DocumentsImpl();
				DocumentsImpl documentsImpl;
				if (tableDocDraft != null && tableDocDraft.Rows.Count > 0)
				{
					documentsImpl = tableDocDraft[0].ToDocument(_lstUDFHeader);
					documentsImpl.DocEntry = 0;
				}
				else
				{
					documentsImpl = null;
				}
				List<Document_LinesImpl> lines = new List<Document_LinesImpl>();
				if (documentLineTable != null && documentLineTable.Rows.Count > 0)
				{
					lines = documentLineTable.ToDocumentLine(_lstUDFLine);
					lines = lines.Where((Document_LinesImpl x) => !string.IsNullOrEmpty(x.ItemCode)).ToList();
				}
				if (documentsImpl != null)
				{
					documentsImpl.Lines = lines;
					documentsImpl.DocObjectCode = ApzObjectTypes.oOrders;
					string text = await ExtenseionMethods.SaveDocument(new Tuple<DocumentsImpl, ApzObjectTypes>(documentsImpl, ApzObjectTypes.oOrders));
					WinCommons._CloseProcessing();
					if (string.IsNullOrEmpty(text))
					{
						await Search();
						WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
					}
					else
					{
						WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeMedium, text);
					}
				}
				btn_AddSalesOrder.Enabled = true;
				btn_Search.Enabled = true;
				return;
			}
			int docEntry = (row.IsExistsColumn("DocEntry") ? Function.ParseInt(row["DocEntry"]) : (-1));
			if (docEntry <= 0 || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			btn_AddSalesOrder.Enabled = false;
			btn_Search.Enabled = false;
			WinCommons._OpenProcessing();
			DocumentTable tableDoc = await ExtenseionMethods.GetDocCopy(docEntry, 23.ToString(), 17.ToString());
			DocumentLineTable documentLineTable2 = await ExtenseionMethods.GetDocLineCopy(docEntry, 23.ToString(), 17.ToString());
			new DocumentsImpl();
			DocumentsImpl document;
			if (tableDoc != null && tableDoc.Rows.Count > 0)
			{
				document = tableDoc[0].ToDocument(_lstUDFHeader);
				document.DocEntry = 0;
			}
			else
			{
				document = null;
			}
			List<Document_LinesImpl> lines2 = new List<Document_LinesImpl>();
			if (documentLineTable2 != null && documentLineTable2.Rows.Count > 0)
			{
				lines2 = documentLineTable2.ToDocumentLine(_lstUDFLine);
				lines2 = lines2.Where((Document_LinesImpl x) => !string.IsNullOrEmpty(x.ItemCode)).ToList();
			}
			if (document != null)
			{
				document.Lines = lines2;
				_dtClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Sales");
				if (!(await document.GetApprovalTemplate(_dtClientHelper, "3", this)))
				{
					WinCommons._CloseProcessing();
					btn_AddSalesOrder.Enabled = true;
					btn_Search.Enabled = true;
					return;
				}
				_dtClientHelper.Dispose();
				string text2 = await ExtenseionMethods.SaveDocument(new Tuple<DocumentsImpl, ApzObjectTypes>(document, ApzObjectTypes.oOrders));
				if (string.IsNullOrEmpty(text2))
				{
					await Search();
					DocumentTable tableDocDraft = await ExtenseionMethods.GetDocCopy(docEntry, 23.ToString(), 1470000113.ToString());
					DocumentTable documentTable = await ExtenseionMethods.GetDocCopy(docEntry, 23.ToString(), 1470000113.ToString());
					List<Document_LinesImpl> list = ((documentTable != null) ? documentTable.ToDocumentLine() : new List<Document_LinesImpl>());
					List<Document_LinesImpl> lstLinePurReq = new List<Document_LinesImpl>();
					if (list != null && list.Count > 0)
					{
						foreach (Document_LinesImpl line in list)
						{
							UserFieldsImpl userFieldsImpl = line.UserFields.FirstOrDefault((UserFieldsImpl o) => o.Name.Equals("U_SaleType"));
							if (userFieldsImpl != null && userFieldsImpl.Value.Equals("03"))
							{
								DataTable dataTable = await GetItemByCode(line.ItemCode);
								if (dataTable.IsNotNull() && dataTable.ExistsColumn("CardCode"))
								{
									line.LineVendor = Function.ToString(dataTable.Rows[0]["CardCode"]);
									line.CardCode = Function.ToString(dataTable.Rows[0]["CardCode"]);
									line.VatGroup = Function.ToString(dataTable.Rows[0]["VatGroupPu"]);
									lstLinePurReq.Add(line);
								}
							}
						}
					}
					DocumentsImpl documentsImpl2 = new DocumentsImpl();
					if (lstLinePurReq.Count > 0)
					{
						if (tableDocDraft != null && tableDocDraft.Rows.Count > 0)
						{
							documentsImpl2 = tableDocDraft[0].ToDocument();
							documentsImpl2.DocEntry = 0;
						}
					}
					else
					{
						documentsImpl2 = null;
					}
					if (documentsImpl2 != null)
					{
						documentsImpl2.Lines = lstLinePurReq;
						documentsImpl2.CardCode = "";
						documentsImpl2.CardName = "";
						documentsImpl2.Requester = GlobalData.UserName;
						documentsImpl2.RequesterBranch = GlobalData.DefaultBranch;
						documentsImpl2.RequesterDepartment = Function.ParseInt(GlobalData.DeparmentLogin);
						documentsImpl2.SendNotification = BoYesNoEnum.tNO;
						documentsImpl2.Email = " ";
						documentsImpl2.ReqDate = DateTime.Today;
						documentsImpl2.UseCorrectionVATGroup = BoYesNoEnum.tNO;
						string text3 = await ExtenseionMethods.SaveDocument(new Tuple<DocumentsImpl, ApzObjectTypes>(documentsImpl2, ApzObjectTypes.oPurchaseRequest));
						if (!string.IsNullOrEmpty(text3))
						{
							WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeMedium, text3);
						}
					}
					WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				}
				else
				{
					WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeMedium, text2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_AddSalesOrder.Enabled = true;
		btn_Search.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void bte_BPCode_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, false, bte_BPCode.Text, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null && chooseItem.IsExistsColumn("CardCode"))
			{
				bte_BPCode.Text = Function.ToString(chooseItem["CardCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bte_BPCode_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Tab && string.IsNullOrEmpty(bte_BPCode.Text))
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, false, bte_BPCode.Text, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				DataRow chooseItem = frm_CFL2.ChooseItem;
				if (chooseItem != null && chooseItem.IsExistsColumn("CardCode"))
				{
					bte_BPCode.Text = Function.ToString(chooseItem["CardCode"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_CancelSO_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			DataRow row = ((DataRowView)tre_SalesOrder.GetDataRecordByNode(tre_SalesOrder.FocusedNode)).Row;
			int docEntry = (row.IsExistsColumn("DocEntry") ? Function.ParseInt(row["DocEntry"]) : (-1));
			if (docEntry <= 0 || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			string text = await ExtenseionMethods.Cancel(docEntry, ApzObjectTypes.oOrders);
			if (string.IsNullOrEmpty(text) || docEntry.ToString().Equals(text))
			{
				WinCommons.writeStatus(Enums.MessageCode.CancelSoSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "", text);
				await Search();
			}
			else
			{
				WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeMedium, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async void btn_CloseSO_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			DataRow row = ((DataRowView)tre_SalesOrder.GetDataRecordByNode(tre_SalesOrder.FocusedNode)).Row;
			int docEntry = (row.IsExistsColumn("DocEntry") ? Function.ParseInt(row["DocEntry"]) : (-1));
			if (docEntry <= 0 || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			string text = await ExtenseionMethods.Close(docEntry, ApzObjectTypes.oOrders);
			if (string.IsNullOrEmpty(text) || docEntry.ToString().Equals(text))
			{
				WinCommons.writeStatus(Enums.MessageCode.CloseSoSuccess, Enums.MessageType.Error, Msg_Time._TimeMedium, "", text);
				await Search();
			}
			else
			{
				WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeMedium, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async Task LoadEmployees()
	{
		try
		{
			DataTable dataTable = await _httpClient.Commons.GetAllEmployees();
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.NewRow();
				if (dataRow.IsExistsColumn("empID") && dataRow.IsExistsColumn("FullName"))
				{
					dataRow["empID"] = "-1";
					dataRow["FullName"] = "All";
					dataTable.Rows.InsertAt(dataRow, 0);
					lue_OwnerName.Properties.DataSource = dataTable;
					lue_OwnerName.Properties.DisplayMember = "FullName";
					lue_OwnerName.Properties.ValueMember = "empID";
					lue_OwnerName.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task LoadComboSalesGroup()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SaleEmployee");
			DataTable dataTable = await _helper.GetDataTableAsync("ShowSaleEmployee");
			if (dataTable.IsNotNull())
			{
				lue_SalesEmp.Properties.DataSource = dataTable;
				lue_SalesEmp.Properties.DisplayMember = "SlpName";
				lue_SalesEmp.Properties.ValueMember = "SlpCode";
				lue_SalesEmp.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataSet> LoadData()
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "SalesControl");
			return await helper.PostAsync("LoadSalesControl", dtSearch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task Search()
	{
		_ = 9;
		try
		{
			btn_Search.Enabled = false;
			btn_AddSalesOrder.Enabled = false;
			WinCommons._OpenProcessing();
			CreateDataSearch();
			DataRow dataRow = dtSearch.NewRow();
			dtSearch.Rows.Add(dataRow);
			dataRow["userCode"] = "";
			dataRow["ownerName"] = Function.ToString(lue_OwnerName.EditValue);
			dataRow["salesEmp"] = Function.ToString(lue_SalesEmp.EditValue);
			dataRow["fromdate"] = (string.IsNullOrEmpty(date_FromDate.Text) ? DateTime.Today : Convert.ToDateTime(date_FromDate.EditValue));
			dataRow["todate"] = (string.IsNullOrEmpty(date_ToDate.Text) ? DateTime.Today : Convert.ToDateTime(date_ToDate.EditValue));
			dataRow["bpCode"] = Function.ToString(bte_BPCode.Text);
			dataRow["userName"] = GlobalData.UserName;
			dataRow["branch"] = GlobalData.DefaultBranch;
			Function.ToString(lue_OwnerName.EditValue);
			Function.ToString(lue_SalesEmp.EditValue);
			if (!string.IsNullOrEmpty(date_FromDate.Text))
			{
				Convert.ToDateTime(date_FromDate.EditValue);
			}
			else
			{
				_ = DateTime.Today;
			}
			if (!string.IsNullOrEmpty(date_ToDate.Text))
			{
				Convert.ToDateTime(date_ToDate.EditValue);
			}
			else
			{
				_ = DateTime.Today;
			}
			Function.ToString(bte_BPCode.Text);
			string text = "";
			for (int i = 0; i < chb_Status.Properties.Items.Count; i++)
			{
				if (chb_Status.Properties.Items[i].CheckState == CheckState.Checked)
				{
					text = text + "," + chb_Status.Properties.Items[i].Value;
				}
			}
			text = ((!string.IsNullOrEmpty(text) && text.StartsWith(",")) ? text.Substring(1, text.Length - 1) : "");
			dataRow["docStatus"] = text;
			DataSet ds = new DataSet();
			switch (xtraTab_SalesControl.SelectedTabPage.Name)
			{
			case "tab_SalesQuotation":
				dataRow["saleType"] = "sq";
				ds = await LoadData();
				if (ds.IsNotNull())
				{
					await TreeListHelper.GenerateTreeList(tre_SalesQuotation, ds, _lstUDFLine, expand: true);
				}
				break;
			case "tab_SalesOrder":
				dataRow["saleType"] = "so";
				ds = await LoadData();
				if (ds.IsNotNull())
				{
					await TreeListHelper.GenerateTreeList(tre_SalesOrder, ds, _lstUDFLine, expand: true);
				}
				break;
			case "tab_SalesReturn":
				dataRow["saleType"] = "sr";
				ds = await LoadData();
				if (ds.IsNotNull())
				{
					await TreeListHelper.GenerateTreeList(tre_SalesReturn, ds, _lstUDFLine, expand: true);
				}
				break;
			case "tab_SalesContract":
				dataRow["saleType"] = "sc";
				ds = await LoadData();
				if (ds.IsNotNull())
				{
					await TreeListHelper.GenerateTreeList(tre_SalesContract, ds, _lstUDFLine, expand: true);
				}
				break;
			case "tab_SalesProduction":
				dataRow["saleType"] = "sp";
				ds = await LoadData();
				if (ds.IsNotNull())
				{
					await TreeListHelper.GenerateTreeList(tre_SalesProduction, ds, _lstUDFLine, expand: true);
				}
				break;
			}
			ds.Tables.Clear();
			ds.Dispose();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_AddSalesOrder.Enabled = true;
		btn_Search.Enabled = true;
		WinCommons._CloseProcessing();
	}

	public async Task<DataTable> GetItemByCode(string itemCode)
	{
		try
		{
			using HttpClientHelper<DataTable> client = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			return await client.GetAsync("GetItemByItemCode", new
			{
				ItemCode = itemCode
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return null;
	}

	private void CreateDataSearch()
	{
		if (!dtSearch.IsNotNull() || dtSearch.Columns.Count == 0)
		{
			dtSearch = new DataTable();
			dtSearch.Columns.Add(new DataColumn("userCode", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("ownerName", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("salesEmp", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("fromdate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("todate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("docStatus", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("bpCode", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("saleType", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("userName", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("branch", typeof(int)));
		}
		else
		{
			dtSearch.Rows.Clear();
		}
	}

	protected override void NoAuthSetControlSearch()
	{
		btn_Search.Enabled = true;
		btn_AddSalesOrder.Visible = false;
		date_FromDate.Enabled = true;
		date_ToDate.Enabled = true;
		lue_SalesEmp.Enabled = true;
		lue_OwnerName.Enabled = true;
		bte_BPCode.Enabled = true;
		chb_Status.Enabled = true;
		date_FromDate.ReadOnly = false;
		date_ToDate.ReadOnly = false;
		lue_SalesEmp.ReadOnly = false;
		lue_OwnerName.ReadOnly = false;
		bte_BPCode.ReadOnly = false;
		chb_Status.ReadOnly = false;
	}

	public async void PRINTLAYOUT()
	{
		_ = 1;
		try
		{
			string name = xtraTab_SalesControl.SelectedTabPage.Name;
			if (!(name == "tab_SalesQuotation"))
			{
				if (name == "tab_SalesOrder")
				{
					DataRow row = ((DataRowView)tre_SalesOrder.GetDataRecordByNode(tre_SalesOrder.FocusedNode)).Row;
					int num = (row.IsExistsColumn("DocEntry") ? Function.ParseInt(row["DocEntry"]) : (-1));
					await WinFuntions.PrintLayout("", num, "RDR2");
				}
			}
			else
			{
				DataRow row2 = ((DataRowView)tre_SalesQuotation.GetDataRecordByNode(tre_SalesQuotation.FocusedNode)).Row;
				int num = (row2.IsExistsColumn("DocEntry") ? Function.ParseInt(row2["DocEntry"]) : (-1));
				await WinFuntions.PrintLayout("", num, "QUT2");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PREVIEWLAYOUTS()
	{
		_ = 1;
		try
		{
			string name = xtraTab_SalesControl.SelectedTabPage.Name;
			if (!(name == "tab_SalesQuotation"))
			{
				if (name == "tab_SalesOrder")
				{
					DataRow row = ((DataRowView)tre_SalesOrder.GetDataRecordByNode(tre_SalesOrder.FocusedNode)).Row;
					int num = (row.IsExistsColumn("DocEntry") ? Function.ParseInt(row["DocEntry"]) : (-1));
					await WinFuntions.PreviewLayout("", num, "RDR2", this);
				}
			}
			else
			{
				DataRow row2 = ((DataRowView)tre_SalesQuotation.GetDataRecordByNode(tre_SalesOrder.FocusedNode)).Row;
				int num = (row2.IsExistsColumn("DocEntry") ? Function.ParseInt(row2["DocEntry"]) : (-1));
				await WinFuntions.PreviewLayout("", num, "QUT2", this);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tre_SalesQuotation_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		try
		{
			if (Function.ParseBool(e.Node.GetDisplayText("Approval")))
			{
				e.Appearance.BackColor = Color.Gray;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async void tre_SalesProduction_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			string type = "";
			string fieldName = e.Column.FieldName;
			if (!fieldName.Equals("SXDelTimeConfirm") && !fieldName.Equals("CSDelTimeConfirm") && !fieldName.Equals("DEDelTimeConfirm") && !fieldName.Equals("SXRemark") && !fieldName.Equals("CSRemark") && !fieldName.Equals("DERemark"))
			{
				return;
			}
			TreeListNode focusNode = e.Node;
			int docentry = Function.ParseInt(focusNode.GetValue("DocEntry"));
			int lineid = Function.ParseInt(focusNode.GetValue("LineNum"));
			string remark = "";
			DateTime deltime = default;
			if (fieldName.Equals("SXDelTimeConfirm") || fieldName.Equals("SXRemark"))
			{
				type = "PR";
				deltime = Function.ParseDateTimes(focusNode.GetValue("SXDelTimeConfirm"));
				remark = Function.ToString(focusNode.GetValue("SXRemark"));
			}
			else if (fieldName.Equals("CSDelTimeConfirm") || fieldName.Equals("CSRemark"))
			{
				type = "CS";
				deltime = Function.ParseDateTimes(focusNode.GetValue("CSDelTimeConfirm"));
				remark = Function.ToString(focusNode.GetValue("CSRemark"));
			}
			else if (fieldName.Equals("DEDelTimeConfirm") || fieldName.Equals("DERemark"))
			{
				type = "DE";
				deltime = Function.ParseDateTimes(focusNode.GetValue("DEDelTimeConfirm"));
				remark = Function.ToString(focusNode.GetValue("DERemark"));
			}
			WinCommons._OpenProcessing();
			string text = await UpdateDeltimeConfirm(docentry, lineid, deltime, type, remark, GlobalData.UserName);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
			}
			else
			{
				WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeMedium, text);
				switch (type)
				{
				case "PR":
					focusNode.SetValue("SXDeltimeConfirm", null);
					break;
				case "CS":
					focusNode.SetValue("CSDeltimeConfirm", null);
					break;
				case "DE":
					focusNode.SetValue("DEDeltimeConfirm", null);
					break;
				}
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> UpdateDeltimeConfirm(int docentry, int lineid, DateTime deltime, string type, string remark, string username)
	{
		try
		{
			using HttpClientHelper<string> _help = new HttpClientHelper<string>(Configuration.ApiServer, "SalesControl");
			return await _help.GetAsync("UpdateDeltimeConfirm", new { docentry, lineid, deltime, type, remark, username });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private void date_FromDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (date_FromDate.EditValue != null)
			{
				date_ToDate.Properties.MinValue = (DateTime)date_FromDate.EditValue;
			}
			else if (date_FromDate.EditValue == null || string.IsNullOrEmpty(date_FromDate.Text))
			{
				date_ToDate.Properties.MinValue = default;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void date_ToDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (date_ToDate.EditValue != null)
			{
				date_FromDate.Properties.MaxValue = (DateTime)date_ToDate.EditValue;
			}
			else if (date_ToDate.EditValue == null || string.IsNullOrEmpty(date_ToDate.Text))
			{
				date_FromDate.Properties.MaxValue = DateTime.Today.AddMonths(1);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async void btn_Approval_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			WinCommons._OpenProcessing();
			DataRow row = ((DataRowView)tre_SalesOrder.GetDataRecordByNode(tre_SalesOrder.FocusedNode)).Row;
			string text = (row.IsExistsColumn("Stage1") ? Function.ToString(row["Stage1"]) : "");
			if (text.Equals("Y"))
			{
				return;
			}
			int num = (row.IsExistsColumn("PunEntry") ? Function.ParseInt(row["PunEntry"]) : (-1));
			if (num <= 0)
			{
				return;
			}
			int soNo = (row.IsExistsColumn("DocEntry") ? Function.ParseInt(row["DocEntry"]) : (-1));
			if (string.IsNullOrEmpty(text) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			DataTable dataTable = await _httpClient.Udo.GetUdoByCode(new Common.Table_Code
			{
				Name = "OUDO",
				Code = "DPUN"
			});
			if (!dataTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.UdoTableIsInvalid, "");
				return;
			}
			DataSet ds = new DataSet();
			string tableName = Function.ToString(dataTable.Rows[0]["TableName"]);
			ds.Tables.Add(dataTable.Copy());
			DataTable dtDoc = new DataTable(tableName)
			{
				Columns = 
				{
					{
						"DocEntry",
						typeof(int)
					},
					{
						"U_Stage1",
						typeof(string)
					},
					{
						"U_Stage2",
						typeof(string)
					},
					{
						"U_SoNo",
						typeof(string)
					},
					{
						"U_SoLine",
						typeof(string)
					},
					{
						"U_Remark1",
						typeof(string)
					},
					{
						"U_Remark2",
						typeof(string)
					},
					{
						"U_UserSign2",
						typeof(string)
					}
				}
			};
			DataRow[] array = tre_SalesOrder.DataSourceTable().Select("DocEntry = " + soNo);
			DataRow[] array2 = array;
			foreach (DataRow dataRow in array2)
			{
				dtDoc.Rows.Clear();
				string value = (dataRow.IsExistsColumn("U_Remark1") ? Function.ToString(dataRow["U_Remark1"]) : "");
				string value2 = (dataRow.IsExistsColumn("U_Remark2") ? Function.ToString(dataRow["U_Remark2"]) : "");
				int num2 = Function.ParseInt(dataRow["LineNum"]);
				num = Function.ParseInt(dataRow["PunEntry"]);
				if (num != 0)
				{
					DataRow dataRow2 = dtDoc.NewRow();
					dataRow2["DocEntry"] = num;
					dataRow2["U_Stage1"] = "Y";
					dataRow2["U_Stage2"] = "Y";
					dataRow2["U_SoNo"] = soNo;
					dataRow2["U_SoLine"] = num2;
					dataRow2["U_Remark1"] = value;
					dataRow2["U_Remark2"] = value2;
					dtDoc.Rows.Add(dataRow2);
					ds.Tables.Add(dtDoc.Copy());
					HttpResult httpResult = await _httpClient.Udo.Save(ds);
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
						return;
					}
					ds.Tables.RemoveAt(1);
				}
			}
			WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			await Search();
			ds.DisposeDataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private async void btn_Reject_Click(object sender, EventArgs e)
	{
		_ = 5;
		try
		{
			WinCommons._OpenProcessing();
			DataRow row = ((DataRowView)tre_SalesOrder.GetDataRecordByNode(tre_SalesOrder.FocusedNode)).Row;
			string text = (row.IsExistsColumn("Stage1") ? Function.ToString(row["Stage1"]) : "");
			if (text.Equals("N") || (row.IsExistsColumn("PunEntry") ? Function.ParseInt(row["PunEntry"]) : (-1)) <= 0)
			{
				return;
			}
			int soNo = (row.IsExistsColumn("DocEntry") ? Function.ParseInt(row["DocEntry"]) : (-1));
			if (string.IsNullOrEmpty(text) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			new List<DocumentsImpl>();
			DocumentsImpl documentsImpl = new DocumentsImpl();
			documentsImpl.DocEntry = soNo;
			documentsImpl.UserName = "manager";
			DataTable tableDoc = await LoadOrder(documentsImpl);
			if (!tableDoc.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DocEntryNotFound, "", Function.ToString(soNo));
				return;
			}
			DocumentLineTable dt = await LoadOrderLines(soNo);
			new DocumentsImpl();
			DocumentsImpl documentsImpl2 = tableDoc.Rows[0].ToDocument();
			documentsImpl2.Lines = new List<Document_LinesImpl>();
			documentsImpl2.Lines = dt.ToDocumentLine(_lstUDFLine);
			foreach (Document_LinesImpl line in documentsImpl2.Lines)
			{
				line.WhsCode = ((GlobalData.DefaultBranch == 1) ? "SW05" : "HW05");
				line.OcrCode4 = "SD05";
			}
			string text2 = await ExtenseionMethods.SaveDocument(new Tuple<DocumentsImpl, ApzObjectTypes>(documentsImpl2, ApzObjectTypes.oOrders));
			if (!string.IsNullOrEmpty(text2))
			{
				WinCommons.Error(Enums.MessageCode.None, text2);
				return;
			}
			DataTable dataTable = await _httpClient.Udo.GetUdoByCode(new Common.Table_Code
			{
				Name = "OUDO",
				Code = "DPUN"
			});
			if (!dataTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.UdoTableIsInvalid, "");
				return;
			}
			DataSet ds = new DataSet();
			string tableName = Function.ToString(dataTable.Rows[0]["TableName"]);
			ds.Tables.Add(dataTable.Copy());
			DataTable dtDoc = new DataTable(tableName)
			{
				Columns = 
				{
					{
						"DocEntry",
						typeof(int)
					},
					{
						"U_Stage1",
						typeof(string)
					},
					{
						"U_SoNo",
						typeof(string)
					},
					{
						"U_SoLine",
						typeof(string)
					},
					{
						"U_Remark1",
						typeof(string)
					},
					{
						"U_Remark2",
						typeof(string)
					},
					{
						"U_UserSign2",
						typeof(string)
					}
				}
			};
			DataRow[] array = tre_SalesOrder.DataSourceTable().Select("DocEntry = " + soNo);
			DataRow[] array2 = array;
			foreach (DataRow dataRow in array2)
			{
				dtDoc.Rows.Clear();
				string value = (dataRow.IsExistsColumn("U_Remark1") ? Function.ToString(dataRow["U_Remark1"]) : "");
				string value2 = (dataRow.IsExistsColumn("U_Remark2") ? Function.ToString(dataRow["U_Remark2"]) : "");
				int num = Function.ParseInt(dataRow["LineNum"]);
				int num2 = Function.ParseInt(dataRow["PunEntry"]);
				if (num2 != 0)
				{
					DataRow dataRow2 = dtDoc.NewRow();
					dataRow2["DocEntry"] = num2;
					dataRow2["U_Stage1"] = "N";
					dataRow2["U_SoNo"] = soNo;
					dataRow2["U_SoLine"] = num;
					dataRow2["U_Remark1"] = value;
					dataRow2["U_Remark2"] = value2;
					dtDoc.Rows.Add(dataRow2);
					ds.Tables.Add(dtDoc.Copy());
					HttpResult httpResult = await _httpClient.Udo.Save(ds);
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
						return;
					}
					ds.Tables.RemoveAt(1);
				}
			}
			WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			await Search();
			ds.DisposeDataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private async Task<DocumentLineTable> LoadOrderLines(int docEntry)
	{
		DocumentLineTable lst = new DocumentLineTable();
		try
		{
			using HttpClientHelper<DocumentLineTable> docLineTableHelper = new HttpClientHelper<DocumentLineTable>(Configuration.ApiServer, "Sales");
			lst = await docLineTableHelper.GetAsync("GetLine", new
			{
				id = docEntry
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return lst;
	}

	private async Task<DataTable> LoadOrder(DocumentsImpl value)
	{
		DataTable dt = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> docLineTableHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Sales");
			dt = await docLineTableHelper.PostAsync("Search", value);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return dt;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Sales.frm_SalesControl));
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.date_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.chb_Status = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.lue_OwnerName = new DevExpress.XtraEditors.LookUpEdit();
		this.lue_SalesEmp = new DevExpress.XtraEditors.LookUpEdit();
		this.bte_BPCode = new DevExpress.XtraEditors.ButtonEdit();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.lb_SalesEmp = new Apzon.Controls.Controls.LabelLine();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lb_BusinessPartner = new Apzon.Controls.Controls.LabelLine();
		this.lb_NoOfTrans = new Apzon.Controls.Controls.LabelLine();
		this.lb_OwnerName = new Apzon.Controls.Controls.LabelLine();
		this.btn_AddSalesOrder = new DevExpress.XtraEditors.SimpleButton();
		this.xtraTab_SalesControl = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_SalesQuotation = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		this.tre_SalesQuotation = new Apzon.Controls.Tree.TreeEx();
		this.tab_SalesOrder = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
		this.btn_Reject = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Approval = new DevExpress.XtraEditors.SimpleButton();
		this.btn_CloseSO = new DevExpress.XtraEditors.SimpleButton();
		this.tre_SalesOrder = new Apzon.Controls.Tree.TreeEx();
		this.btn_CancelSO = new DevExpress.XtraEditors.SimpleButton();
		this.tab_SalesReturn = new Apzon.Controls.Controls.XtraTabPageEx();
		this.tre_SalesReturn = new Apzon.Controls.Tree.TreeEx();
		this.tab_SalesContract = new Apzon.Controls.Controls.XtraTabPageEx();
		this.tre_SalesContract = new Apzon.Controls.Tree.TreeEx();
		this.tab_SalesProduction = new Apzon.Controls.Controls.XtraTabPageEx();
		this.tre_SalesProduction = new Apzon.Controls.Tree.TreeEx();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chb_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lue_OwnerName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lue_SalesEmp.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BPCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTab_SalesControl).BeginInit();
		this.xtraTab_SalesControl.SuspendLayout();
		this.tab_SalesQuotation.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tre_SalesQuotation).BeginInit();
		this.tab_SalesOrder.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl4).BeginInit();
		this.panelControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tre_SalesOrder).BeginInit();
		this.tab_SalesReturn.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tre_SalesReturn).BeginInit();
		this.tab_SalesContract.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tre_SalesContract).BeginInit();
		this.tab_SalesProduction.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tre_SalesProduction).BeginInit();
		base.SuspendLayout();
		this.panelControl1.Location = new System.Drawing.Point(372, -83);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(200, 100);
		this.panelControl1.TabIndex = 4;
		this.panelControl2.Controls.Add(this.date_ToDate);
		this.panelControl2.Controls.Add(this.date_FromDate);
		this.panelControl2.Controls.Add(this.labelLine2);
		this.panelControl2.Controls.Add(this.chb_Status);
		this.panelControl2.Controls.Add(this.lue_OwnerName);
		this.panelControl2.Controls.Add(this.lue_SalesEmp);
		this.panelControl2.Controls.Add(this.bte_BPCode);
		this.panelControl2.Controls.Add(this.labelLine1);
		this.panelControl2.Controls.Add(this.lb_SalesEmp);
		this.panelControl2.Controls.Add(this.btn_Search);
		this.panelControl2.Controls.Add(this.lb_BusinessPartner);
		this.panelControl2.Controls.Add(this.lb_NoOfTrans);
		this.panelControl2.Controls.Add(this.lb_OwnerName);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl2.Location = new System.Drawing.Point(0, 0);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(804, 90);
		this.panelControl2.TabIndex = 2;
		this.date_ToDate.EditValue = null;
		this.date_ToDate.Location = new System.Drawing.Point(374, 10);
		this.date_ToDate.Name = "date_ToDate";
		this.date_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Size = new System.Drawing.Size(172, 20);
		this.date_ToDate.TabIndex = 63;
		this.date_ToDate.EditValueChanged += new System.EventHandler(this.date_ToDate_EditValueChanged);
		this.date_FromDate.EditValue = null;
		this.date_FromDate.Location = new System.Drawing.Point(92, 10);
		this.date_FromDate.Name = "date_FromDate";
		this.date_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Size = new System.Drawing.Size(172, 20);
		this.date_FromDate.TabIndex = 62;
		this.date_FromDate.EditValueChanged += new System.EventHandler(this.date_FromDate_EditValueChanged);
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(270, 11);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(105, 20);
		this.labelLine2.TabIndex = 60;
		this.labelLine2.Utext = "To Date";
		this.chb_Status.Location = new System.Drawing.Point(374, 34);
		this.chb_Status.Name = "chb_Status";
		this.chb_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.chb_Status.Size = new System.Drawing.Size(172, 20);
		this.chb_Status.TabIndex = 59;
		this.lue_OwnerName.Location = new System.Drawing.Point(92, 34);
		this.lue_OwnerName.Name = "lue_OwnerName";
		this.lue_OwnerName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lue_OwnerName.Properties.NullText = "";
		this.lue_OwnerName.Size = new System.Drawing.Size(172, 20);
		this.lue_OwnerName.TabIndex = 34;
		this.lue_SalesEmp.Location = new System.Drawing.Point(92, 59);
		this.lue_SalesEmp.Name = "lue_SalesEmp";
		this.lue_SalesEmp.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lue_SalesEmp.Properties.NullText = "";
		this.lue_SalesEmp.Size = new System.Drawing.Size(172, 20);
		this.lue_SalesEmp.TabIndex = 33;
		this.bte_BPCode.Location = new System.Drawing.Point(374, 59);
		this.bte_BPCode.Name = "bte_BPCode";
		this.bte_BPCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_BPCode.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_BPCode_Properties_ButtonClick);
		this.bte_BPCode.Size = new System.Drawing.Size(172, 20);
		this.bte_BPCode.TabIndex = 27;
		this.bte_BPCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.bte_BPCode_KeyDown);
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(270, 59);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(107, 20);
		this.labelLine1.TabIndex = 26;
		this.labelLine1.Utext = "BP Code";
		this.lb_SalesEmp.HideWhenEmpty = false;
		this.lb_SalesEmp.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_SalesEmp.Location = new System.Drawing.Point(13, 62);
		this.lb_SalesEmp.Name = "lb_SalesEmp";
		this.lb_SalesEmp.ShowUnderLine = true;
		this.lb_SalesEmp.Size = new System.Drawing.Size(80, 20);
		this.lb_SalesEmp.TabIndex = 24;
		this.lb_SalesEmp.Utext = "Sales Emp";
		this.btn_Search.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Search.Appearance.Options.UseBackColor = true;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.Location = new System.Drawing.Point(590, 8);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.Size = new System.Drawing.Size(99, 23);
		this.btn_Search.TabIndex = 21;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.lb_BusinessPartner.HideWhenEmpty = false;
		this.lb_BusinessPartner.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BusinessPartner.Location = new System.Drawing.Point(270, 37);
		this.lb_BusinessPartner.Name = "lb_BusinessPartner";
		this.lb_BusinessPartner.ShowUnderLine = true;
		this.lb_BusinessPartner.Size = new System.Drawing.Size(107, 20);
		this.lb_BusinessPartner.TabIndex = 17;
		this.lb_BusinessPartner.Utext = "Document status";
		this.lb_NoOfTrans.HideWhenEmpty = false;
		this.lb_NoOfTrans.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_NoOfTrans.Location = new System.Drawing.Point(7, 11);
		this.lb_NoOfTrans.Name = "lb_NoOfTrans";
		this.lb_NoOfTrans.ShowUnderLine = true;
		this.lb_NoOfTrans.Size = new System.Drawing.Size(86, 20);
		this.lb_NoOfTrans.TabIndex = 15;
		this.lb_NoOfTrans.Utext = "From Date";
		this.lb_OwnerName.HideWhenEmpty = false;
		this.lb_OwnerName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_OwnerName.Location = new System.Drawing.Point(13, 37);
		this.lb_OwnerName.Name = "lb_OwnerName";
		this.lb_OwnerName.ShowUnderLine = true;
		this.lb_OwnerName.Size = new System.Drawing.Size(80, 20);
		this.lb_OwnerName.TabIndex = 13;
		this.lb_OwnerName.Utext = "Owner Name";
		this.btn_AddSalesOrder.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_AddSalesOrder.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_AddSalesOrder.ImageOptions.Image");
		this.btn_AddSalesOrder.Location = new System.Drawing.Point(671, 291);
		this.btn_AddSalesOrder.Name = "btn_AddSalesOrder";
		this.btn_AddSalesOrder.Size = new System.Drawing.Size(120, 23);
		this.btn_AddSalesOrder.TabIndex = 34;
		this.btn_AddSalesOrder.Text = "Add Sales Order";
		this.btn_AddSalesOrder.Click += new System.EventHandler(this.btn_AddSalesOrder_Click);
		this.xtraTab_SalesControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTab_SalesControl.Location = new System.Drawing.Point(0, 90);
		this.xtraTab_SalesControl.Name = "xtraTab_SalesControl";
		this.xtraTab_SalesControl.SelectedTabPage = this.tab_SalesQuotation;
		this.xtraTab_SalesControl.Size = new System.Drawing.Size(804, 347);
		this.xtraTab_SalesControl.TabIndex = 5;
		DevExpress.XtraTab.XtraTabPageCollection tabPages = this.xtraTab_SalesControl.TabPages;
		DevExpress.XtraTab.XtraTabPage[] pages = new Apzon.Controls.Controls.XtraTabPageEx[5] { this.tab_SalesQuotation, this.tab_SalesOrder, this.tab_SalesReturn, this.tab_SalesContract, this.tab_SalesProduction };
		tabPages.AddRange(pages);
		this.xtraTab_SalesControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTab_SalesControl_SelectedPageChanged);
		this.tab_SalesQuotation.Controls.Add(this.panelControl3);
		this.tab_SalesQuotation.Name = "tab_SalesQuotation";
		this.tab_SalesQuotation.Size = new System.Drawing.Size(798, 319);
		this.tab_SalesQuotation.Text = "Sales Quotation";
		this.panelControl3.Controls.Add(this.tre_SalesQuotation);
		this.panelControl3.Controls.Add(this.btn_AddSalesOrder);
		this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl3.Location = new System.Drawing.Point(0, 0);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(798, 319);
		this.panelControl3.TabIndex = 35;
		this.tre_SalesQuotation.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tre_SalesQuotation.KeyFieldName = "LineNum";
		this.tre_SalesQuotation.Location = new System.Drawing.Point(-1, 0);
		this.tre_SalesQuotation.Name = "tre_SalesQuotation";
		this.tre_SalesQuotation.OptionsFind.FindNullPrompt = "Nhập để tìm kiếm...";
		this.tre_SalesQuotation.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.RowFullFocus;
		this.tre_SalesQuotation.ParentFieldName = "DocEntry";
		this.tre_SalesQuotation.Size = new System.Drawing.Size(799, 285);
		this.tre_SalesQuotation.TabIndex = 0;
		this.tre_SalesQuotation.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.tre_SalesQuotation_NodeCellStyle);
		this.tab_SalesOrder.Controls.Add(this.panelControl4);
		this.tab_SalesOrder.Name = "tab_SalesOrder";
		this.tab_SalesOrder.Size = new System.Drawing.Size(798, 319);
		this.tab_SalesOrder.Text = "Sales Order";
		this.panelControl4.Controls.Add(this.btn_Reject);
		this.panelControl4.Controls.Add(this.btn_Approval);
		this.panelControl4.Controls.Add(this.btn_CloseSO);
		this.panelControl4.Controls.Add(this.tre_SalesOrder);
		this.panelControl4.Controls.Add(this.btn_CancelSO);
		this.panelControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl4.Location = new System.Drawing.Point(0, 0);
		this.panelControl4.Name = "panelControl4";
		this.panelControl4.Size = new System.Drawing.Size(798, 319);
		this.panelControl4.TabIndex = 36;
		this.btn_Reject.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Reject.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Reject.ImageOptions.Image");
		this.btn_Reject.Location = new System.Drawing.Point(641, 291);
		this.btn_Reject.Name = "btn_Reject";
		this.btn_Reject.Size = new System.Drawing.Size(154, 23);
		this.btn_Reject.TabIndex = 38;
		this.btn_Reject.Text = "Reject Fast Response";
		this.btn_Reject.Click += new System.EventHandler(this.btn_Reject_Click);
		this.btn_Approval.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Approval.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Approval.ImageOptions.Image");
		this.btn_Approval.Location = new System.Drawing.Point(471, 291);
		this.btn_Approval.Name = "btn_Approval";
		this.btn_Approval.Size = new System.Drawing.Size(154, 23);
		this.btn_Approval.TabIndex = 37;
		this.btn_Approval.Text = "Approval Fast Response";
		this.btn_Approval.Click += new System.EventHandler(this.btn_Approval_Click);
		this.btn_CloseSO.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_CloseSO.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CloseSO.ImageOptions.Image");
		this.btn_CloseSO.Location = new System.Drawing.Point(11, 291);
		this.btn_CloseSO.Name = "btn_CloseSO";
		this.btn_CloseSO.Size = new System.Drawing.Size(120, 23);
		this.btn_CloseSO.TabIndex = 36;
		this.btn_CloseSO.Text = "Close Sales Order";
		this.btn_CloseSO.Visible = false;
		this.btn_CloseSO.Click += new System.EventHandler(this.btn_CloseSO_Click);
		this.tre_SalesOrder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tre_SalesOrder.Location = new System.Drawing.Point(-1, 0);
		this.tre_SalesOrder.Name = "tre_SalesOrder";
		this.tre_SalesOrder.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.RowFocus;
		this.tre_SalesOrder.OptionsView.HeaderFilterButtonShowMode = DevExpress.XtraEditors.Controls.FilterButtonShowMode.SmartTag;
		this.tre_SalesOrder.Size = new System.Drawing.Size(799, 285);
		this.tre_SalesOrder.TabIndex = 0;
		this.btn_CancelSO.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_CancelSO.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CancelSO.ImageOptions.Image");
		this.btn_CancelSO.Location = new System.Drawing.Point(137, 291);
		this.btn_CancelSO.Name = "btn_CancelSO";
		this.btn_CancelSO.Size = new System.Drawing.Size(120, 23);
		this.btn_CancelSO.TabIndex = 35;
		this.btn_CancelSO.Text = "Cancel Sales Order";
		this.btn_CancelSO.Visible = false;
		this.btn_CancelSO.Click += new System.EventHandler(this.btn_CancelSO_Click);
		this.tab_SalesReturn.Controls.Add(this.tre_SalesReturn);
		this.tab_SalesReturn.Name = "tab_SalesReturn";
		this.tab_SalesReturn.PageVisible = false;
		this.tab_SalesReturn.Size = new System.Drawing.Size(798, 319);
		this.tab_SalesReturn.Text = "Sales Return";
		this.tre_SalesReturn.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tre_SalesReturn.Location = new System.Drawing.Point(0, 0);
		this.tre_SalesReturn.Name = "tre_SalesReturn";
		this.tre_SalesReturn.Size = new System.Drawing.Size(798, 319);
		this.tre_SalesReturn.TabIndex = 1;
		this.tab_SalesContract.Controls.Add(this.tre_SalesContract);
		this.tab_SalesContract.Name = "tab_SalesContract";
		this.tab_SalesContract.Size = new System.Drawing.Size(798, 319);
		this.tab_SalesContract.Text = "Sales Contract";
		this.tre_SalesContract.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tre_SalesContract.Location = new System.Drawing.Point(0, 0);
		this.tre_SalesContract.Name = "tre_SalesContract";
		this.tre_SalesContract.Size = new System.Drawing.Size(798, 319);
		this.tre_SalesContract.TabIndex = 1;
		this.tab_SalesProduction.Controls.Add(this.tre_SalesProduction);
		this.tab_SalesProduction.Name = "tab_SalesProduction";
		this.tab_SalesProduction.Size = new System.Drawing.Size(798, 319);
		this.tab_SalesProduction.Text = "Production for Sales";
		this.tre_SalesProduction.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tre_SalesProduction.Location = new System.Drawing.Point(0, 0);
		this.tre_SalesProduction.Name = "tre_SalesProduction";
		this.tre_SalesProduction.Size = new System.Drawing.Size(798, 319);
		this.tre_SalesProduction.TabIndex = 2;
		this.tre_SalesProduction.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tre_SalesProduction_CellValueChanged);
		base.AcceptButton = this.btn_Search;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(804, 437);
		base.Controls.Add(this.xtraTab_SalesControl);
		base.Controls.Add(this.panelControl2);
		base.Name = "frm_SalesControl";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Sales Control";
		base.Load += new System.EventHandler(this.frm_SalesControl_Load);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chb_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lue_OwnerName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lue_SalesEmp.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BPCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTab_SalesControl).EndInit();
		this.xtraTab_SalesControl.ResumeLayout(false);
		this.tab_SalesQuotation.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tre_SalesQuotation).EndInit();
		this.tab_SalesOrder.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl4).EndInit();
		this.panelControl4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tre_SalesOrder).EndInit();
		this.tab_SalesReturn.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tre_SalesReturn).EndInit();
		this.tab_SalesContract.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tre_SalesContract).EndInit();
		this.tab_SalesProduction.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tre_SalesProduction).EndInit();
		base.ResumeLayout(false);
	}
}
