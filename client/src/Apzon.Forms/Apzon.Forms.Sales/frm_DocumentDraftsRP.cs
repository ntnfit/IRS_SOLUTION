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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Sales;

public class frm_DocumentDraftsRP : XtraFormEx
{
	public delegate void ShowForms(XtraFormEx frm);

	private DataTable dt = new DataTable();

	private DataTable dtObject = new DataTable();

	public ShowForms frmShowChildrenForm;

	private bool isReadOnly;

	private IContainer components;

	private LabelLine labelLine1;

	private ComboBoxEditEx cbDate;

	private CheckEditEx chkOpen;

	private LabelLine labelLine3;

	private DateEditEx fDate;

	private LabelLine labelLine4;

	private DateEditEx tDate;

	private CheckEditEx chkDate;

	private CheckEditEx chkSales;

	private CheckEditEx chkSQ;

	private CheckEditEx chkSOrder;

	private CheckEditEx chkDeliveries;

	private CheckEditEx chkReturn;

	private CheckEditEx chkARI;

	private CheckEditEx chkARCreditM;

	private CheckEditEx chkPurchase;

	private CheckEditEx chkP_Request;

	private CheckEditEx chkPQ;

	private CheckEditEx chkPO;

	private CheckEditEx chkGoodsReceiptPO;

	private CheckEditEx chkGoodReturn;

	private CheckEditEx chkAPI;

	private CheckEditEx chkInventory;

	private CheckEditEx chkICT;

	private LabelLine labelLine2;

	private CheckEditEx chkAPCreditM;

	private CheckEditEx chkIC;

	private CheckEditEx chkIOB;

	private CheckEditEx chkIP;

	private CheckEditEx chkGoodsReceipt;

	private CheckEditEx chkGoodsIssue;

	private CheckEditEx chkITR;

	private CheckEditEx chkInventoryTransfers;

	private Panel panel_Sales;

	private Panel panel_Purchase;

	private Panel panel_Inventory;

	private Panel panel_IventoryTransaction;

	private ComboBoxEditEx cbUser;

	private Panel panel_Date;

	private SimpleButtonEx btOK;

	private SimpleButtonEx btCancel;

	private CheckEditEx chkARDownP;

	private CheckEditEx chkAPDownP;

	private CheckEditEx chkReturnRequest;

	public frm_DocumentDraftsRP()
	{
		InitializeComponent();
		WinFuntions.SetFormatControlValue(this);
	}

	private DataTable GetLogForm()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DocumentDraft");
			return httpClientHelper.Get("GetLogForm", new
			{
				userCode = GlobalData.UserName
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return null;
		}
	}

	private void LoadCheckBox(string listCheck, bool open)
	{
		string[] source = listCheck.Split(',');
		if (source.Contains("23"))
		{
			chkSQ.Checked = true;
		}
		if (source.Contains("17"))
		{
			chkSOrder.Checked = true;
		}
		if (source.Contains("15"))
		{
			chkDeliveries.Checked = true;
		}
		if (source.Contains("234000031"))
		{
			chkReturnRequest.Checked = true;
		}
		if (source.Contains("16"))
		{
			chkReturn.Checked = true;
		}
		if (source.Contains("13"))
		{
			chkARI.Checked = true;
		}
		if (source.Contains("14"))
		{
			chkARCreditM.Checked = true;
		}
		if (source.Contains("203"))
		{
			chkARDownP.Checked = true;
		}
		if (source.Contains("59"))
		{
			chkGoodsReceipt.Checked = true;
		}
		if (source.Contains("60"))
		{
			chkGoodsIssue.Checked = true;
		}
		if (source.Contains("1250000001"))
		{
			chkITR.Checked = true;
		}
		if (source.Contains("67"))
		{
			chkInventoryTransfers.Checked = true;
		}
		if (source.Contains("1470000113"))
		{
			chkP_Request.Checked = true;
		}
		if (source.Contains("540000006"))
		{
			chkPQ.Checked = true;
		}
		if (source.Contains("22"))
		{
			chkPO.Checked = true;
		}
		if (source.Contains("20"))
		{
			chkGoodsReceiptPO.Checked = true;
		}
		if (source.Contains("21"))
		{
			chkGoodReturn.Checked = true;
		}
		if (source.Contains("18"))
		{
			chkAPI.Checked = true;
		}
		if (source.Contains("19"))
		{
			chkAPCreditM.Checked = true;
		}
		if (source.Contains("204"))
		{
			chkAPDownP.Checked = true;
		}
		if (source.Contains("1470000065"))
		{
			chkIC.Checked = true;
		}
		if (source.Contains("10000071"))
		{
			chkIP.Checked = true;
		}
		chkOpen.Checked = open;
	}

	private void frm_DocumentDraftsRP_Load(object sender, EventArgs e)
	{
		LoadData();
	}

	private async void LoadData()
	{
		await GetDefaultTablbe();
		btOK.Enabled = false;
		panel_Date.Visible = false;
		string menuCode = Function.ToString(GetType().Name);
		if (GlobalData.ListAccessForm.FirstOrDefault((AuthsImpl t) => t.authoriza == "R" && t.menucode == menuCode) != null)
		{
			isReadOnly = true;
		}
		new DataTable();
		DataTable result = GetLogForm();
		DataRow dataRow = result.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["LogType"]) == "ObjType");
		string listCheck = ((dataRow != null) ? Function.ToString(dataRow["Value"]) : "");
		dataRow = result.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["LogType"]) == "Open");
		LoadCheckBox(listCheck, dataRow != null && Function.ParseBool(dataRow["Value"]));
		await GetListUser();
		dataRow = result.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["LogType"]) == "User");
		string text = ((dataRow != null && Function.ToString(dataRow["Value"]) != "") ? Function.ToString(dataRow["Value"]) : GlobalData.UserID.ToString());
		for (int num = 0; num < cbUser.Properties.Items.Count; num++)
		{
			if (Function.ToString(((Common.ListItems)cbUser.Properties.Items[num]).Value) == text)
			{
				cbUser.SelectedIndex = num;
				break;
			}
		}
	}

	private async Task GetDefaultTablbe()
	{
		try
		{
			using HttpClientHelper<DataTable> client = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DocumentDraft");
			dtObject = (await client.GetAsync("GetTableDefault")).Clone();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetListUser()
	{
		try
		{
			using HttpClientHelper<RequesterImpl> helper = new HttpClientHelper<RequesterImpl>(Configuration.ApiServer, "Requester");
			DataTable dataTable = await helper.GetDataTableAsync("GetAllUser_tableOUSR");
			List<Common.Table_Code> list = new List<Common.Table_Code>();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				Common.Table_Code table_Code = new Common.Table_Code();
				table_Code.Code = dataTable.Rows[i]["USERID"].ToString();
				table_Code.Name = dataTable.Rows[i]["U_NAME"].ToString();
				list.Add(table_Code);
			}
			WinFuntions._addComboBox(cbUser, list, _selectFirstItem: false);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btOK_Click(object sender, EventArgs e)
	{
		try
		{
			dt = new DataTable();
			dt.Columns.Add("ObjType", typeof(string));
			dt.Columns.Add("UserSign", typeof(string));
			dt.Columns.Add("DocStatus", typeof(string));
			dt.Columns.Add("Date", typeof(string));
			dt.Columns.Add("TypeDate", typeof(string));
			dt.Columns.Add("From", typeof(string));
			dt.Columns.Add("To", typeof(string));
			DataRow dataRow = dt.NewRow();
			List<string> list = new List<string>();
			if (chkSQ.Checked)
			{
				list.Add("23");
			}
			if (chkSOrder.Checked)
			{
				list.Add("17");
			}
			if (chkDeliveries.Checked)
			{
				list.Add("15");
			}
			if (chkReturn.Checked)
			{
				list.Add("16");
			}
			if (chkARDownP.Checked)
			{
				list.Add("203");
			}
			if (chkARI.Checked)
			{
				list.Add("13");
			}
			if (chkARCreditM.Checked)
			{
				list.Add("14");
			}
			if (chkP_Request.Checked)
			{
				list.Add("1470000113");
			}
			if (chkPQ.Checked)
			{
				list.Add("540000006");
			}
			if (chkPO.Checked)
			{
				list.Add("22");
			}
			if (chkGoodsReceiptPO.Checked)
			{
				list.Add("20");
			}
			if (chkGoodReturn.Checked)
			{
				list.Add("21");
			}
			if (chkAPDownP.Checked)
			{
				list.Add("204");
			}
			if (chkAPI.Checked)
			{
				list.Add("18");
			}
			if (chkAPCreditM.Checked)
			{
				list.Add("19");
			}
			if (chkGoodsReceipt.Checked)
			{
				list.Add("59");
			}
			if (chkGoodsIssue.Checked)
			{
				list.Add("60");
			}
			if (chkITR.Checked)
			{
				list.Add("1250000001");
			}
			if (chkInventoryTransfers.Checked)
			{
				list.Add("67");
			}
			if (chkReturnRequest.Checked)
			{
				list.Add("234000031");
			}
			if (list.Count > 0)
			{
				dataRow["ObjType"] = string.Join(",", list.ToArray());
			}
			dataRow["UserSign"] = Function.ToString(((Common.ListItems)cbUser.SelectedItem).Value);
			if (chkOpen.Checked)
			{
				dataRow["DocStatus"] = "1";
			}
			else
			{
				dataRow["DocStatus"] = "0";
			}
			if (Function.ParseBool(chkDate.EditValue))
			{
				dataRow["TypeDate"] = cbDate.SelectedIndex;
				dataRow["From"] = (Function.ParseDateTime(fDate.EditValue).HasValue ? ((DateTime)fDate.EditValue).ToString("yyyy-MM-dd") : "");
				dataRow["To"] = (Function.ParseDateTime(tDate.EditValue).HasValue ? ((DateTime)tDate.EditValue).ToString("yyyy-MM-dd") : "");
				dataRow["Date"] = 1;
			}
			else
			{
				dataRow["TypeDate"] = cbDate.SelectedIndex;
				dataRow["From"] = (Function.ParseDateTime(fDate.EditValue).HasValue ? ((DateTime)fDate.EditValue).ToString("yyyy-MM-dd") : "");
				dataRow["To"] = (Function.ParseDateTime(tDate.EditValue).HasValue ? ((DateTime)tDate.EditValue).ToString("yyyy-MM-dd") : "");
				dataRow["Date"] = 0;
			}
			dt.Rows.Add(dataRow);
			if (CheckSearch())
			{
				if (!isReadOnly)
				{
					SaveLogAccessform(string.Join(",", list), chkOpen.Checked);
				}
				await Search(dt);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private bool CheckSearch()
	{
		try
		{
			if (!chkDate.Checked)
			{
				return true;
			}
			if (cbDate.SelectedIndex == -1)
			{
				WinCommons.Error(Enums.MessageCode.InvalidDate, "");
				return false;
			}
			if (string.IsNullOrWhiteSpace(Function.ToString(fDate.EditValue)))
			{
				WinCommons.Error(Enums.MessageCode.YouDoNotEnterFromDate, "");
				return false;
			}
			if (string.IsNullOrWhiteSpace(Function.ToString(tDate.EditValue)))
			{
				WinCommons.Error(Enums.MessageCode.YouDoNotEnterToDate, "");
				return false;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return true;
	}

	private async Task SaveLogAccessform(string lstObj, bool open)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("FormId", typeof(string));
			dataTable.Columns["FormId"].DefaultValue = FormId;
			dataTable.Columns.Add("LogType", typeof(string));
			dataTable.Columns.Add("UserCode", typeof(string));
			dataTable.Columns["UserCode"].DefaultValue = GlobalData.UserCodeLogin;
			dataTable.Columns.Add("Value", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["LogType"] = "ObjType";
			dataRow["Value"] = lstObj;
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["LogType"] = "Open";
			dataRow2["Value"] = open.ToString();
			dataTable.Rows.Add(dataRow2);
			if (cbUser.SelectedItem != null)
			{
				Common.ListItems listItems = (Common.ListItems)cbUser.SelectedItem;
				DataRow dataRow3 = dataTable.NewRow();
				dataRow3["LogType"] = "User";
				dataRow3["Value"] = listItems.Value;
				dataTable.Rows.Add(dataRow3);
			}
			dataTable.AcceptChanges();
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "DocumentDraft");
			await helper.PostAsync("SaveLogForm", dataTable);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Search(DataTable dt)
	{
		try
		{
			WinCommons._OpenProcessing();
			using (HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DocumentDraft"))
			{
				dtObject = await _helper.PostAsync("SearchListDocEntry", dt);
				if (dtObject != null && dtObject.Rows.Count > 0)
				{
					frm_ListDocumentDraft frm_ListDocumentDraft2 = new frm_ListDocumentDraft(dt);
					frm_ListDocumentDraft2.MdiParent = MdiParent;
					frm_ListDocumentDraft2.Show();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void SetVisibleChk()
	{
		chkSales.Checked = false;
		chkPurchase.Checked = false;
		chkInventory.Checked = false;
		chkICT.Checked = false;
		chkDate.Checked = false;
		panel_Sales.Visible = false;
		panel_Purchase.Visible = false;
		panel_Inventory.Visible = false;
		panel_IventoryTransaction.Visible = false;
		panel_Date.Visible = false;
	}

	private void chkSales_CheckedChanged(object sender, EventArgs e)
	{
		if (chkSales.Checked)
		{
			chkSQ.Checked = true;
			chkSOrder.Checked = true;
			chkDeliveries.Checked = true;
			chkReturn.Checked = true;
			chkARDownP.Checked = true;
			chkARI.Checked = true;
			chkARCreditM.Checked = true;
			chkReturnRequest.Checked = true;
		}
		else
		{
			chkSQ.Checked = false;
			chkSOrder.Checked = false;
			chkDeliveries.Checked = false;
			chkReturn.Checked = false;
			chkARDownP.Checked = false;
			chkARI.Checked = false;
			chkARCreditM.Checked = false;
			chkReturnRequest.Checked = false;
		}
		SetEnable_btOk();
	}

	private void chkPurchase_CheckedChanged(object sender, EventArgs e)
	{
		if (chkPurchase.Checked)
		{
			chkP_Request.Checked = true;
			chkPQ.Checked = true;
			chkPO.Checked = true;
			chkGoodsReceiptPO.Checked = true;
			chkGoodReturn.Checked = true;
			chkAPDownP.Checked = true;
			chkAPI.Checked = true;
			chkAPCreditM.Checked = true;
		}
		else
		{
			chkP_Request.Checked = false;
			chkPQ.Checked = false;
			chkPO.Checked = false;
			chkGoodsReceiptPO.Checked = false;
			chkGoodReturn.Checked = false;
			chkAPDownP.Checked = false;
			chkAPI.Checked = false;
			chkAPCreditM.Checked = false;
		}
		SetEnable_btOk();
	}

	private void chkInventory_CheckedChanged(object sender, EventArgs e)
	{
		if (chkInventory.Checked)
		{
			chkGoodsReceipt.Checked = true;
			chkGoodsIssue.Checked = true;
			chkITR.Checked = true;
			chkInventoryTransfers.Checked = true;
		}
		else
		{
			chkGoodsReceipt.Checked = false;
			chkGoodsIssue.Checked = false;
			chkITR.Checked = false;
			chkInventoryTransfers.Checked = false;
		}
		SetEnable_btOk();
	}

	private void chkICT_CheckedChanged(object sender, EventArgs e)
	{
		if (chkICT.Checked)
		{
			chkIC.Checked = true;
			chkIOB.Checked = true;
			chkIP.Checked = true;
		}
		else
		{
			chkIC.Checked = false;
			chkIOB.Checked = false;
			chkIP.Checked = false;
		}
		SetEnable_btOk();
	}

	private void chkDate_CheckedChanged(object sender, EventArgs e)
	{
		if (chkDate.Checked)
		{
			panel_Date.Visible = true;
		}
		else
		{
			panel_Date.Visible = false;
		}
	}

	private void SetEnable_btOk()
	{
		if (chkSQ.Checked || chkSOrder.Checked || chkDeliveries.Checked || chkReturn.Checked || chkARDownP.Checked || chkARI.Checked || chkARCreditM.Checked || chkP_Request.Checked || chkPQ.Checked || chkPO.Checked || chkGoodsReceiptPO.Checked || chkGoodReturn.Checked || chkAPDownP.Checked || chkAPI.Checked || chkARCreditM.Checked || chkGoodsReceipt.Checked || chkGoodsIssue.Checked || chkITR.Checked || chkInventoryTransfers.Checked || chkIC.Checked || chkIOB.Checked || chkIP.Checked || chkAPCreditM.Checked || chkReturnRequest.Checked)
		{
			btOK.Enabled = true;
		}
		else if (!chkSQ.Checked && !chkSOrder.Checked && !chkDeliveries.Checked && !chkReturn.Checked && !chkARDownP.Checked && !chkARI.Checked && !chkARCreditM.Checked && !chkP_Request.Checked && !chkPQ.Checked && !chkPO.Checked && !chkGoodsReceiptPO.Checked && !chkGoodReturn.Checked && !chkAPDownP.Checked && !chkAPI.Checked && !chkARCreditM.Checked && !chkGoodsReceipt.Checked && !chkGoodsIssue.Checked && !chkITR.Checked && !chkInventoryTransfers.Checked && !chkIC.Checked && !chkIOB.Checked && !chkIP.Checked && !chkAPCreditM.Checked && !chkReturnRequest.Checked)
		{
			btOK.Enabled = false;
		}
	}

	private void chkSQ_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkP_Request_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkSOrder_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkDeliveries_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkReturn_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkARDownP_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkARI_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkARCreditM_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkGoodsReceipt_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkGoodsIssue_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkITR_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkInventoryTransfers_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkPQ_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkPO_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkGoodsReceiptPO_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkGoodReturn_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkAPDowmPayment_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkAPI_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkAPCreditM_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkIC_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkIOB_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkIP_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkARDownP_CheckedChanged_1(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkAPDownP_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
	}

	private void chkReturnRequest_CheckedChanged(object sender, EventArgs e)
	{
		SetEnable_btOk();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Sales.frm_DocumentDraftsRP));
		this.cbDate = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.chkOpen = new Apzon.Controls.Editor.CheckEditEx();
		this.fDate = new Apzon.Controls.Editor.DateEditEx();
		this.tDate = new Apzon.Controls.Editor.DateEditEx();
		this.chkDate = new Apzon.Controls.Editor.CheckEditEx();
		this.chkSales = new Apzon.Controls.Editor.CheckEditEx();
		this.chkSQ = new Apzon.Controls.Editor.CheckEditEx();
		this.chkSOrder = new Apzon.Controls.Editor.CheckEditEx();
		this.chkDeliveries = new Apzon.Controls.Editor.CheckEditEx();
		this.chkReturn = new Apzon.Controls.Editor.CheckEditEx();
		this.chkARI = new Apzon.Controls.Editor.CheckEditEx();
		this.chkARCreditM = new Apzon.Controls.Editor.CheckEditEx();
		this.chkPurchase = new Apzon.Controls.Editor.CheckEditEx();
		this.chkP_Request = new Apzon.Controls.Editor.CheckEditEx();
		this.chkPQ = new Apzon.Controls.Editor.CheckEditEx();
		this.chkPO = new Apzon.Controls.Editor.CheckEditEx();
		this.chkGoodsReceiptPO = new Apzon.Controls.Editor.CheckEditEx();
		this.chkGoodReturn = new Apzon.Controls.Editor.CheckEditEx();
		this.chkAPI = new Apzon.Controls.Editor.CheckEditEx();
		this.chkInventory = new Apzon.Controls.Editor.CheckEditEx();
		this.chkICT = new Apzon.Controls.Editor.CheckEditEx();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.chkAPCreditM = new Apzon.Controls.Editor.CheckEditEx();
		this.chkIC = new Apzon.Controls.Editor.CheckEditEx();
		this.chkIOB = new Apzon.Controls.Editor.CheckEditEx();
		this.chkIP = new Apzon.Controls.Editor.CheckEditEx();
		this.chkGoodsReceipt = new Apzon.Controls.Editor.CheckEditEx();
		this.chkGoodsIssue = new Apzon.Controls.Editor.CheckEditEx();
		this.chkITR = new Apzon.Controls.Editor.CheckEditEx();
		this.chkInventoryTransfers = new Apzon.Controls.Editor.CheckEditEx();
		this.panel_Sales = new System.Windows.Forms.Panel();
		this.chkReturnRequest = new Apzon.Controls.Editor.CheckEditEx();
		this.chkARDownP = new Apzon.Controls.Editor.CheckEditEx();
		this.panel_Purchase = new System.Windows.Forms.Panel();
		this.chkAPDownP = new Apzon.Controls.Editor.CheckEditEx();
		this.panel_Inventory = new System.Windows.Forms.Panel();
		this.panel_IventoryTransaction = new System.Windows.Forms.Panel();
		this.cbUser = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.panel_Date = new System.Windows.Forms.Panel();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.cbDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkOpen.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkSales.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkSQ.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkSOrder.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkDeliveries.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkReturn.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkARI.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkARCreditM.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkPurchase.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkP_Request.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkPQ.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkPO.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkGoodsReceiptPO.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkGoodReturn.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkAPI.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkInventory.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkICT.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkAPCreditM.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkIC.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkIOB.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkIP.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkGoodsReceipt.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkGoodsIssue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkITR.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkInventoryTransfers.Properties).BeginInit();
		this.panel_Sales.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chkReturnRequest.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkARDownP.Properties).BeginInit();
		this.panel_Purchase.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chkAPDownP.Properties).BeginInit();
		this.panel_Inventory.SuspendLayout();
		this.panel_IventoryTransaction.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cbUser.Properties).BeginInit();
		this.panel_Date.SuspendLayout();
		base.SuspendLayout();
		this.cbDate.EditControl = null;
		this.cbDate.FieldName = null;
		this.cbDate.IsSearchControl = false;
		this.cbDate.Location = new System.Drawing.Point(2, 12);
		this.cbDate.Name = "cbDate";
		this.cbDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbDate.Properties.Items.AddRange(new object[5] { "Creation Date", "Date of Update", "Posting Date", "Value Date", "Document Date" });
		this.cbDate.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cbDate.SettingInVisible = false;
		this.cbDate.SettingReadonly = false;
		this.cbDate.Size = new System.Drawing.Size(161, 20);
		this.cbDate.TabIndex = 1;
		this.chkOpen.EditControl = null;
		this.chkOpen.FieldName = null;
		this.chkOpen.IsSearchControl = false;
		this.chkOpen.Location = new System.Drawing.Point(270, 10);
		this.chkOpen.Name = "chkOpen";
		this.chkOpen.Properties.Caption = "Open Only";
		this.chkOpen.SettingInVisible = false;
		this.chkOpen.SettingReadonly = false;
		this.chkOpen.Size = new System.Drawing.Size(75, 20);
		this.chkOpen.TabIndex = 2;
		this.fDate.EditControl = null;
		this.fDate.EditValue = null;
		this.fDate.FieldName = null;
		this.fDate.Location = new System.Drawing.Point(244, 12);
		this.fDate.Name = "fDate";
		this.fDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.fDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.fDate.Properties.IsSearchControl = false;
		this.fDate.SettingInVisible = false;
		this.fDate.SettingReadonly = false;
		this.fDate.Size = new System.Drawing.Size(85, 20);
		this.fDate.TabIndex = 4;
		this.tDate.EditControl = null;
		this.tDate.EditValue = null;
		this.tDate.FieldName = null;
		this.tDate.Location = new System.Drawing.Point(444, 12);
		this.tDate.Name = "tDate";
		this.tDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.tDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.tDate.Properties.IsSearchControl = false;
		this.tDate.SettingInVisible = false;
		this.tDate.SettingReadonly = false;
		this.tDate.Size = new System.Drawing.Size(88, 20);
		this.tDate.TabIndex = 4;
		this.chkDate.EditControl = null;
		this.chkDate.FieldName = null;
		this.chkDate.IsSearchControl = false;
		this.chkDate.Location = new System.Drawing.Point(11, 45);
		this.chkDate.Name = "chkDate";
		this.chkDate.Properties.Caption = "";
		this.chkDate.SettingInVisible = false;
		this.chkDate.SettingReadonly = false;
		this.chkDate.Size = new System.Drawing.Size(16, 20);
		this.chkDate.TabIndex = 2;
		this.chkDate.CheckedChanged += new System.EventHandler(this.chkDate_CheckedChanged);
		this.chkSales.EditControl = null;
		this.chkSales.FieldName = null;
		this.chkSales.IsSearchControl = false;
		this.chkSales.Location = new System.Drawing.Point(11, 75);
		this.chkSales.Name = "chkSales";
		this.chkSales.Properties.Caption = "Sales - A/R";
		this.chkSales.SettingInVisible = false;
		this.chkSales.SettingReadonly = false;
		this.chkSales.Size = new System.Drawing.Size(75, 20);
		this.chkSales.TabIndex = 2;
		this.chkSales.CheckedChanged += new System.EventHandler(this.chkSales_CheckedChanged);
		this.chkSQ.EditControl = null;
		this.chkSQ.FieldName = null;
		this.chkSQ.IsSearchControl = false;
		this.chkSQ.Location = new System.Drawing.Point(3, 3);
		this.chkSQ.Name = "chkSQ";
		this.chkSQ.Properties.Caption = "Sales Quotations";
		this.chkSQ.SettingInVisible = false;
		this.chkSQ.SettingReadonly = false;
		this.chkSQ.Size = new System.Drawing.Size(101, 20);
		this.chkSQ.TabIndex = 2;
		this.chkSQ.CheckedChanged += new System.EventHandler(this.chkSQ_CheckedChanged);
		this.chkSOrder.EditControl = null;
		this.chkSOrder.FieldName = null;
		this.chkSOrder.IsSearchControl = false;
		this.chkSOrder.Location = new System.Drawing.Point(3, 28);
		this.chkSOrder.Name = "chkSOrder";
		this.chkSOrder.Properties.Caption = "Sales Orders";
		this.chkSOrder.SettingInVisible = false;
		this.chkSOrder.SettingReadonly = false;
		this.chkSOrder.Size = new System.Drawing.Size(101, 20);
		this.chkSOrder.TabIndex = 2;
		this.chkSOrder.CheckedChanged += new System.EventHandler(this.chkSOrder_CheckedChanged);
		this.chkDeliveries.EditControl = null;
		this.chkDeliveries.FieldName = null;
		this.chkDeliveries.IsSearchControl = false;
		this.chkDeliveries.Location = new System.Drawing.Point(3, 53);
		this.chkDeliveries.Name = "chkDeliveries";
		this.chkDeliveries.Properties.Caption = "Deliveries";
		this.chkDeliveries.SettingInVisible = false;
		this.chkDeliveries.SettingReadonly = false;
		this.chkDeliveries.Size = new System.Drawing.Size(101, 20);
		this.chkDeliveries.TabIndex = 2;
		this.chkDeliveries.CheckedChanged += new System.EventHandler(this.chkDeliveries_CheckedChanged);
		this.chkReturn.EditControl = null;
		this.chkReturn.FieldName = null;
		this.chkReturn.IsSearchControl = false;
		this.chkReturn.Location = new System.Drawing.Point(3, 103);
		this.chkReturn.Name = "chkReturn";
		this.chkReturn.Properties.Caption = "Returns";
		this.chkReturn.SettingInVisible = false;
		this.chkReturn.SettingReadonly = false;
		this.chkReturn.Size = new System.Drawing.Size(101, 20);
		this.chkReturn.TabIndex = 2;
		this.chkReturn.CheckedChanged += new System.EventHandler(this.chkReturn_CheckedChanged);
		this.chkARI.EditControl = null;
		this.chkARI.FieldName = null;
		this.chkARI.IsSearchControl = false;
		this.chkARI.Location = new System.Drawing.Point(3, 128);
		this.chkARI.Name = "chkARI";
		this.chkARI.Properties.Caption = "A/R Invoices";
		this.chkARI.SettingInVisible = false;
		this.chkARI.SettingReadonly = false;
		this.chkARI.Size = new System.Drawing.Size(101, 20);
		this.chkARI.TabIndex = 2;
		this.chkARI.CheckedChanged += new System.EventHandler(this.chkARI_CheckedChanged);
		this.chkARCreditM.EditControl = null;
		this.chkARCreditM.FieldName = null;
		this.chkARCreditM.IsSearchControl = false;
		this.chkARCreditM.Location = new System.Drawing.Point(3, 153);
		this.chkARCreditM.Name = "chkARCreditM";
		this.chkARCreditM.Properties.Caption = "A/R Credit Memos";
		this.chkARCreditM.SettingInVisible = false;
		this.chkARCreditM.SettingReadonly = false;
		this.chkARCreditM.Size = new System.Drawing.Size(101, 20);
		this.chkARCreditM.TabIndex = 2;
		this.chkARCreditM.CheckedChanged += new System.EventHandler(this.chkARCreditM_CheckedChanged);
		this.chkPurchase.EditControl = null;
		this.chkPurchase.FieldName = null;
		this.chkPurchase.IsSearchControl = false;
		this.chkPurchase.Location = new System.Drawing.Point(298, 75);
		this.chkPurchase.Name = "chkPurchase";
		this.chkPurchase.Properties.Caption = "Purchasing - A/P";
		this.chkPurchase.SettingInVisible = false;
		this.chkPurchase.SettingReadonly = false;
		this.chkPurchase.Size = new System.Drawing.Size(102, 20);
		this.chkPurchase.TabIndex = 2;
		this.chkPurchase.CheckedChanged += new System.EventHandler(this.chkPurchase_CheckedChanged);
		this.chkP_Request.EditControl = null;
		this.chkP_Request.FieldName = null;
		this.chkP_Request.IsSearchControl = false;
		this.chkP_Request.Location = new System.Drawing.Point(3, 3);
		this.chkP_Request.Name = "chkP_Request";
		this.chkP_Request.Properties.Caption = "Purchase Request";
		this.chkP_Request.SettingInVisible = false;
		this.chkP_Request.SettingReadonly = false;
		this.chkP_Request.Size = new System.Drawing.Size(126, 20);
		this.chkP_Request.TabIndex = 2;
		this.chkP_Request.CheckedChanged += new System.EventHandler(this.chkP_Request_CheckedChanged);
		this.chkPQ.EditControl = null;
		this.chkPQ.FieldName = null;
		this.chkPQ.IsSearchControl = false;
		this.chkPQ.Location = new System.Drawing.Point(3, 28);
		this.chkPQ.Name = "chkPQ";
		this.chkPQ.Properties.Caption = "Purchase Quotation";
		this.chkPQ.SettingInVisible = false;
		this.chkPQ.SettingReadonly = false;
		this.chkPQ.Size = new System.Drawing.Size(126, 20);
		this.chkPQ.TabIndex = 2;
		this.chkPQ.CheckedChanged += new System.EventHandler(this.chkPQ_CheckedChanged);
		this.chkPO.EditControl = null;
		this.chkPO.FieldName = null;
		this.chkPO.IsSearchControl = false;
		this.chkPO.Location = new System.Drawing.Point(3, 53);
		this.chkPO.Name = "chkPO";
		this.chkPO.Properties.Caption = "Purchase Orders";
		this.chkPO.SettingInVisible = false;
		this.chkPO.SettingReadonly = false;
		this.chkPO.Size = new System.Drawing.Size(126, 20);
		this.chkPO.TabIndex = 2;
		this.chkPO.CheckedChanged += new System.EventHandler(this.chkPO_CheckedChanged);
		this.chkGoodsReceiptPO.EditControl = null;
		this.chkGoodsReceiptPO.FieldName = null;
		this.chkGoodsReceiptPO.IsSearchControl = false;
		this.chkGoodsReceiptPO.Location = new System.Drawing.Point(3, 78);
		this.chkGoodsReceiptPO.Name = "chkGoodsReceiptPO";
		this.chkGoodsReceiptPO.Properties.Caption = "Goods Receipt PO";
		this.chkGoodsReceiptPO.SettingInVisible = false;
		this.chkGoodsReceiptPO.SettingReadonly = false;
		this.chkGoodsReceiptPO.Size = new System.Drawing.Size(126, 20);
		this.chkGoodsReceiptPO.TabIndex = 2;
		this.chkGoodsReceiptPO.CheckedChanged += new System.EventHandler(this.chkGoodsReceiptPO_CheckedChanged);
		this.chkGoodReturn.EditControl = null;
		this.chkGoodReturn.FieldName = null;
		this.chkGoodReturn.IsSearchControl = false;
		this.chkGoodReturn.Location = new System.Drawing.Point(3, 103);
		this.chkGoodReturn.Name = "chkGoodReturn";
		this.chkGoodReturn.Properties.Caption = "Goods Return";
		this.chkGoodReturn.SettingInVisible = false;
		this.chkGoodReturn.SettingReadonly = false;
		this.chkGoodReturn.Size = new System.Drawing.Size(126, 20);
		this.chkGoodReturn.TabIndex = 2;
		this.chkGoodReturn.CheckedChanged += new System.EventHandler(this.chkGoodReturn_CheckedChanged);
		this.chkAPI.EditControl = null;
		this.chkAPI.FieldName = null;
		this.chkAPI.IsSearchControl = false;
		this.chkAPI.Location = new System.Drawing.Point(3, 128);
		this.chkAPI.Name = "chkAPI";
		this.chkAPI.Properties.Caption = "A/P Invoices";
		this.chkAPI.SettingInVisible = false;
		this.chkAPI.SettingReadonly = false;
		this.chkAPI.Size = new System.Drawing.Size(126, 20);
		this.chkAPI.TabIndex = 2;
		this.chkAPI.CheckedChanged += new System.EventHandler(this.chkAPI_CheckedChanged);
		this.chkInventory.EditControl = null;
		this.chkInventory.FieldName = null;
		this.chkInventory.IsSearchControl = false;
		this.chkInventory.Location = new System.Drawing.Point(10, 319);
		this.chkInventory.Name = "chkInventory";
		this.chkInventory.Properties.Caption = "Inventory";
		this.chkInventory.SettingInVisible = false;
		this.chkInventory.SettingReadonly = false;
		this.chkInventory.Size = new System.Drawing.Size(75, 20);
		this.chkInventory.TabIndex = 2;
		this.chkInventory.CheckedChanged += new System.EventHandler(this.chkInventory_CheckedChanged);
		this.chkICT.EditControl = null;
		this.chkICT.FieldName = null;
		this.chkICT.IsSearchControl = false;
		this.chkICT.Location = new System.Drawing.Point(296, 319);
		this.chkICT.Name = "chkICT";
		this.chkICT.Properties.Caption = "Inventory Counting Transactions";
		this.chkICT.SettingInVisible = false;
		this.chkICT.SettingReadonly = false;
		this.chkICT.Size = new System.Drawing.Size(191, 20);
		this.chkICT.TabIndex = 2;
		this.chkICT.CheckedChanged += new System.EventHandler(this.chkICT_CheckedChanged);
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = true;
		this.btOK.Location = new System.Drawing.Point(10, 464);
		this.btOK.Name = "btOK";
		this.btOK.ReadOnly = false;
		this.btOK.SettingInVisible = false;
		this.btOK.SettingReadonly = false;
		this.btOK.Size = new System.Drawing.Size(75, 23);
		this.btOK.TabIndex = 5;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = true;
		this.btCancel.Location = new System.Drawing.Point(91, 464);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 5;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.chkAPCreditM.EditControl = null;
		this.chkAPCreditM.FieldName = null;
		this.chkAPCreditM.IsSearchControl = false;
		this.chkAPCreditM.Location = new System.Drawing.Point(3, 153);
		this.chkAPCreditM.Name = "chkAPCreditM";
		this.chkAPCreditM.Properties.Caption = "A/P Credit Memos";
		this.chkAPCreditM.SettingInVisible = false;
		this.chkAPCreditM.SettingReadonly = false;
		this.chkAPCreditM.Size = new System.Drawing.Size(126, 20);
		this.chkAPCreditM.TabIndex = 2;
		this.chkAPCreditM.CheckedChanged += new System.EventHandler(this.chkAPCreditM_CheckedChanged);
		this.chkIC.EditControl = null;
		this.chkIC.FieldName = null;
		this.chkIC.IsSearchControl = false;
		this.chkIC.Location = new System.Drawing.Point(3, 3);
		this.chkIC.Name = "chkIC";
		this.chkIC.Properties.Caption = "Inventory Counting ";
		this.chkIC.SettingInVisible = false;
		this.chkIC.SettingReadonly = false;
		this.chkIC.Size = new System.Drawing.Size(162, 20);
		this.chkIC.TabIndex = 2;
		this.chkIC.CheckedChanged += new System.EventHandler(this.chkIC_CheckedChanged);
		this.chkIOB.EditControl = null;
		this.chkIOB.FieldName = null;
		this.chkIOB.IsSearchControl = false;
		this.chkIOB.Location = new System.Drawing.Point(3, 28);
		this.chkIOB.Name = "chkIOB";
		this.chkIOB.Properties.Caption = "Inventory Opening Balances";
		this.chkIOB.SettingInVisible = false;
		this.chkIOB.SettingReadonly = false;
		this.chkIOB.Size = new System.Drawing.Size(162, 20);
		this.chkIOB.TabIndex = 2;
		this.chkIOB.CheckedChanged += new System.EventHandler(this.chkIOB_CheckedChanged);
		this.chkIP.EditControl = null;
		this.chkIP.FieldName = null;
		this.chkIP.IsSearchControl = false;
		this.chkIP.Location = new System.Drawing.Point(3, 53);
		this.chkIP.Name = "chkIP";
		this.chkIP.Properties.Caption = "Inventory Posting";
		this.chkIP.SettingInVisible = false;
		this.chkIP.SettingReadonly = false;
		this.chkIP.Size = new System.Drawing.Size(162, 20);
		this.chkIP.TabIndex = 2;
		this.chkIP.CheckedChanged += new System.EventHandler(this.chkIP_CheckedChanged);
		this.chkGoodsReceipt.EditControl = null;
		this.chkGoodsReceipt.FieldName = null;
		this.chkGoodsReceipt.IsSearchControl = false;
		this.chkGoodsReceipt.Location = new System.Drawing.Point(3, 3);
		this.chkGoodsReceipt.Name = "chkGoodsReceipt";
		this.chkGoodsReceipt.Properties.Caption = "Goods Receipt";
		this.chkGoodsReceipt.SettingInVisible = false;
		this.chkGoodsReceipt.SettingReadonly = false;
		this.chkGoodsReceipt.Size = new System.Drawing.Size(163, 20);
		this.chkGoodsReceipt.TabIndex = 2;
		this.chkGoodsReceipt.CheckedChanged += new System.EventHandler(this.chkGoodsReceipt_CheckedChanged);
		this.chkGoodsIssue.EditControl = null;
		this.chkGoodsIssue.FieldName = null;
		this.chkGoodsIssue.IsSearchControl = false;
		this.chkGoodsIssue.Location = new System.Drawing.Point(3, 28);
		this.chkGoodsIssue.Name = "chkGoodsIssue";
		this.chkGoodsIssue.Properties.Caption = "Goods Issue";
		this.chkGoodsIssue.SettingInVisible = false;
		this.chkGoodsIssue.SettingReadonly = false;
		this.chkGoodsIssue.Size = new System.Drawing.Size(163, 20);
		this.chkGoodsIssue.TabIndex = 2;
		this.chkGoodsIssue.CheckedChanged += new System.EventHandler(this.chkGoodsIssue_CheckedChanged);
		this.chkITR.EditControl = null;
		this.chkITR.FieldName = null;
		this.chkITR.IsSearchControl = false;
		this.chkITR.Location = new System.Drawing.Point(3, 53);
		this.chkITR.Name = "chkITR";
		this.chkITR.Properties.Caption = "Inventory Transfer Request";
		this.chkITR.SettingInVisible = false;
		this.chkITR.SettingReadonly = false;
		this.chkITR.Size = new System.Drawing.Size(163, 20);
		this.chkITR.TabIndex = 2;
		this.chkITR.CheckedChanged += new System.EventHandler(this.chkITR_CheckedChanged);
		this.chkInventoryTransfers.EditControl = null;
		this.chkInventoryTransfers.FieldName = null;
		this.chkInventoryTransfers.IsSearchControl = false;
		this.chkInventoryTransfers.Location = new System.Drawing.Point(3, 78);
		this.chkInventoryTransfers.Name = "chkInventoryTransfers";
		this.chkInventoryTransfers.Properties.Caption = "Inventory Transfers";
		this.chkInventoryTransfers.SettingInVisible = false;
		this.chkInventoryTransfers.SettingReadonly = false;
		this.chkInventoryTransfers.Size = new System.Drawing.Size(163, 20);
		this.chkInventoryTransfers.TabIndex = 2;
		this.chkInventoryTransfers.CheckedChanged += new System.EventHandler(this.chkInventoryTransfers_CheckedChanged);
		this.panel_Sales.Controls.Add(this.chkReturnRequest);
		this.panel_Sales.Controls.Add(this.chkARDownP);
		this.panel_Sales.Controls.Add(this.chkSQ);
		this.panel_Sales.Controls.Add(this.chkSOrder);
		this.panel_Sales.Controls.Add(this.chkDeliveries);
		this.panel_Sales.Controls.Add(this.chkReturn);
		this.panel_Sales.Controls.Add(this.chkARI);
		this.panel_Sales.Controls.Add(this.chkARCreditM);
		this.panel_Sales.Location = new System.Drawing.Point(23, 100);
		this.panel_Sales.Name = "panel_Sales";
		this.panel_Sales.Size = new System.Drawing.Size(180, 201);
		this.panel_Sales.TabIndex = 6;
		this.chkReturnRequest.EditControl = null;
		this.chkReturnRequest.FieldName = null;
		this.chkReturnRequest.IsSearchControl = false;
		this.chkReturnRequest.Location = new System.Drawing.Point(3, 78);
		this.chkReturnRequest.Name = "chkReturnRequest";
		this.chkReturnRequest.Properties.Caption = "Return Requests";
		this.chkReturnRequest.SettingInVisible = false;
		this.chkReturnRequest.SettingReadonly = false;
		this.chkReturnRequest.Size = new System.Drawing.Size(101, 20);
		this.chkReturnRequest.TabIndex = 4;
		this.chkReturnRequest.CheckedChanged += new System.EventHandler(this.chkReturnRequest_CheckedChanged);
		this.chkARDownP.EditControl = null;
		this.chkARDownP.FieldName = null;
		this.chkARDownP.IsSearchControl = false;
		this.chkARDownP.Location = new System.Drawing.Point(3, 178);
		this.chkARDownP.Name = "chkARDownP";
		this.chkARDownP.Properties.Caption = "A/R Down Payment";
		this.chkARDownP.SettingInVisible = false;
		this.chkARDownP.SettingReadonly = false;
		this.chkARDownP.Size = new System.Drawing.Size(120, 20);
		this.chkARDownP.TabIndex = 3;
		this.chkARDownP.CheckedChanged += new System.EventHandler(this.chkARDownP_CheckedChanged_1);
		this.panel_Purchase.Controls.Add(this.chkAPDownP);
		this.panel_Purchase.Controls.Add(this.chkP_Request);
		this.panel_Purchase.Controls.Add(this.chkPQ);
		this.panel_Purchase.Controls.Add(this.chkPO);
		this.panel_Purchase.Controls.Add(this.chkGoodsReceiptPO);
		this.panel_Purchase.Controls.Add(this.chkGoodReturn);
		this.panel_Purchase.Controls.Add(this.chkAPI);
		this.panel_Purchase.Controls.Add(this.chkAPCreditM);
		this.panel_Purchase.Location = new System.Drawing.Point(313, 100);
		this.panel_Purchase.Name = "panel_Purchase";
		this.panel_Purchase.Size = new System.Drawing.Size(174, 201);
		this.panel_Purchase.TabIndex = 7;
		this.chkAPDownP.EditControl = null;
		this.chkAPDownP.FieldName = null;
		this.chkAPDownP.IsSearchControl = false;
		this.chkAPDownP.Location = new System.Drawing.Point(3, 178);
		this.chkAPDownP.Name = "chkAPDownP";
		this.chkAPDownP.Properties.Caption = "A/P Down Payment";
		this.chkAPDownP.SettingInVisible = false;
		this.chkAPDownP.SettingReadonly = false;
		this.chkAPDownP.Size = new System.Drawing.Size(126, 20);
		this.chkAPDownP.TabIndex = 3;
		this.chkAPDownP.CheckedChanged += new System.EventHandler(this.chkAPDownP_CheckedChanged);
		this.panel_Inventory.Controls.Add(this.chkGoodsReceipt);
		this.panel_Inventory.Controls.Add(this.chkGoodsIssue);
		this.panel_Inventory.Controls.Add(this.chkITR);
		this.panel_Inventory.Controls.Add(this.chkInventoryTransfers);
		this.panel_Inventory.Location = new System.Drawing.Point(26, 344);
		this.panel_Inventory.Name = "panel_Inventory";
		this.panel_Inventory.Size = new System.Drawing.Size(177, 101);
		this.panel_Inventory.TabIndex = 8;
		this.panel_IventoryTransaction.Controls.Add(this.chkIC);
		this.panel_IventoryTransaction.Controls.Add(this.chkIOB);
		this.panel_IventoryTransaction.Controls.Add(this.chkIP);
		this.panel_IventoryTransaction.Location = new System.Drawing.Point(313, 341);
		this.panel_IventoryTransaction.Name = "panel_IventoryTransaction";
		this.panel_IventoryTransaction.Size = new System.Drawing.Size(174, 75);
		this.panel_IventoryTransaction.TabIndex = 9;
		this.cbUser.EditControl = null;
		this.cbUser.FieldName = null;
		this.cbUser.IsSearchControl = false;
		this.cbUser.Location = new System.Drawing.Point(91, 9);
		this.cbUser.Name = "cbUser";
		this.cbUser.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbUser.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cbUser.SettingInVisible = false;
		this.cbUser.SettingReadonly = false;
		this.cbUser.Size = new System.Drawing.Size(159, 20);
		this.cbUser.TabIndex = 10;
		this.panel_Date.Controls.Add(this.labelLine3);
		this.panel_Date.Controls.Add(this.cbDate);
		this.panel_Date.Controls.Add(this.fDate);
		this.panel_Date.Controls.Add(this.labelLine4);
		this.panel_Date.Controls.Add(this.tDate);
		this.panel_Date.Location = new System.Drawing.Point(92, 30);
		this.panel_Date.Name = "panel_Date";
		this.panel_Date.Size = new System.Drawing.Size(540, 39);
		this.panel_Date.TabIndex = 11;
		this.panel_Date.Visible = false;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "From";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(169, 15);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(75, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 3;
		this.labelLine3.Utext = "From";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "To";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(382, 15);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(62, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 3;
		this.labelLine4.Utext = "To";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(33, 44);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(59, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 0;
		this.labelLine2.Utext = "Date";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "User";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 12);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "User";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(636, 493);
		base.Controls.Add(this.panel_Date);
		base.Controls.Add(this.cbUser);
		base.Controls.Add(this.panel_IventoryTransaction);
		base.Controls.Add(this.panel_Inventory);
		base.Controls.Add(this.panel_Purchase);
		base.Controls.Add(this.panel_Sales);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btOK);
		base.Controls.Add(this.chkICT);
		base.Controls.Add(this.chkInventory);
		base.Controls.Add(this.chkPurchase);
		base.Controls.Add(this.chkSales);
		base.Controls.Add(this.chkDate);
		base.Controls.Add(this.chkOpen);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.FormId = "oDocumentDraftRP";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_DocumentDraftsRP.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_DocumentDraftsRP";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Document Drafts Report  - Selection Criteria";
		base.Load += new System.EventHandler(this.frm_DocumentDraftsRP_Load);
		((System.ComponentModel.ISupportInitialize)this.cbDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkOpen.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkSales.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkSQ.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkSOrder.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkDeliveries.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkReturn.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkARI.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkARCreditM.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkPurchase.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkP_Request.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkPQ.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkPO.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkGoodsReceiptPO.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkGoodReturn.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkAPI.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkInventory.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkICT.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkAPCreditM.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkIC.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkIOB.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkIP.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkGoodsReceipt.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkGoodsIssue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkITR.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkInventoryTransfers.Properties).EndInit();
		this.panel_Sales.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chkReturnRequest.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkARDownP.Properties).EndInit();
		this.panel_Purchase.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chkAPDownP.Properties).EndInit();
		this.panel_Inventory.ResumeLayout(false);
		this.panel_IventoryTransaction.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cbUser.Properties).EndInit();
		this.panel_Date.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
