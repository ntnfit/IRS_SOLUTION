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
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using SAPbobsCOM;

namespace Apzon.Forms.Purchase;

public class frm_ProcurementConfirm : XtraFormEx
{
	private string _docEntry = string.Empty;

	private int _CntctPrsnCode;

	private IContainer components;

	private PanelControl panelControl1;

	private PanelControl panelControl2;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Customer;

	private GridViewEx gv_Costomer;

	private GridControlEx grd_SalesOrder;

	private GridViewEx gv_SalesOrder;

	private SimpleButtonEx btn_Save;

	private LabelLine lbl_TargetS;

	private LabelLine lbl_Vendor;

	private CheckEditEx chk_Email;

	private LabelLine lbl_Valid;

	private DateEditEx date_CurrentTime;

	private LabelLine lbl_ReqDate;

	private DateEditEx date_CurrentDate;

	private ComboBoxEditEx cbb_TargetDocument;

	private LabelLine lbl_targetDocument;

	private LabelLine lbl_Email;

	private TextEditEx txt_Email;

	private LabelLine lbl_Name;

	private TextEditEx txt_Name;

	private ComboBoxEditEx cbb_Req;

	private LabelLine lbl_Requester;

	private ComboBoxEditEx cbb_TargetDocSeries;

	private ButtonEditEx txt_Req;

	private ButtonEditEx txt_Vendor;

	private LabelLine lbl_BaseDoc;

	private ComboBoxEditEx cbb_BaseDoc;

	public frm_ProcurementConfirm()
	{
		InitializeComponent();
	}

	private void LoadData()
	{
		try
		{
			LoadDataCombobox();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadDataCombobox()
	{
		try
		{
			List<Common.Table_ID> list = new List<Common.Table_ID>();
			Common.Table_ID table_ID = new Common.Table_ID();
			table_ID.ID = Function.ToString(17);
			table_ID.Name = Apzon.Commons.Properties.Resources.lb_SalesOrder;
			list.Add(table_ID);
			Common.Table_ID table_ID2 = new Common.Table_ID();
			table_ID2.ID = Function.ToString(23);
			table_ID2.Name = Apzon.Commons.Properties.Resources.lb_SalesQuotation;
			list.Add(table_ID2);
			WinFuntions._addComboBox(cbb_BaseDoc, list, _selectFirstItem: true);
			List<Common.Table_ID> list2 = new List<Common.Table_ID>();
			Common.Table_ID table_ID3 = new Common.Table_ID();
			table_ID3.ID = "1";
			table_ID3.Name = "Primary";
			list2.Add(table_ID3);
			WinFuntions._addComboBox(cbb_TargetDocSeries, list2, _selectFirstItem: true);
			List<Common.Table_ID> list3 = new List<Common.Table_ID>();
			Common.Table_ID table_ID4 = new Common.Table_ID();
			table_ID4.ID = "1";
			table_ID4.Name = "User";
			list3.Add(table_ID4);
			Common.Table_ID table_ID5 = new Common.Table_ID();
			table_ID5.ID = "2";
			table_ID5.Name = "Employee";
			list3.Add(table_ID5);
			WinFuntions._addComboBox(cbb_Req, list3, _selectFirstItem: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataSet> LoadPreferredVendor(string baseType, string userName, string branch)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ProcurementConfirm");
			return await helper.GetAsync("LoadPreferredVendor", new { baseType, userName, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataSet> LoadItemLine(string baseType, string docEntry, string userName, string branch)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ProcurementConfirm");
			return await helper.GetAsync("LoadItemLine", new { baseType, docEntry, userName, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	public frm_ChooseFromList ShowChooseFromListBp(string text)
	{
		try
		{
			return new frm_ChooseFromList(Enums.TypeCFL.Vendor, multiSelect: false, text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new frm_ChooseFromList(Enums.TypeCFL.Vendor, multiSelect: false, "");
	}

	public frm_ChooseFromList ShowChooseFromListUser(string text)
	{
		try
		{
			return new frm_ChooseFromList(Enums.TypeCFL.RequesterByUser, multiSelect: false, text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new frm_ChooseFromList(Enums.TypeCFL.Vendor, multiSelect: false, "");
	}

	public frm_ChooseFromList ShowChooseFromListEmployee(string text)
	{
		try
		{
			return new frm_ChooseFromList(Enums.TypeCFL.RequesterByEmployee, multiSelect: false, text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new frm_ChooseFromList(Enums.TypeCFL.Vendor, multiSelect: false, "");
	}

	private bool ValidateRequester()
	{
		if (!Check._text(txt_Req))
		{
			return false;
		}
		if (!Check._date(date_CurrentDate))
		{
			return false;
		}
		if (!Check._date(date_CurrentTime))
		{
			return false;
		}
		return true;
	}

	private bool ValidateOrder()
	{
		if (!Check._date(date_CurrentDate))
		{
			return false;
		}
		if (!Check._date(date_CurrentTime))
		{
			return false;
		}
		DataTable dataTable = grd_SalesOrder.DataSource as DataTable;
		if (dataTable.IsNotNull() && dataTable.ExistsColumn("CardCode"))
		{
			foreach (DataRow row in dataTable.Rows)
			{
				if (string.IsNullOrEmpty(Function.ToString(row["CardCode"])))
				{
					WinCommons.Error(Enums.MessageCode.None, "Vendor trong các item không được để trống!");
					return false;
				}
			}
		}
		return true;
	}

	private bool ValidatePQ()
	{
		DataTable dataTable = grd_SalesOrder.DataSource as DataTable;
		if (dataTable.IsNotNull() && dataTable.ExistsColumn("CardCode"))
		{
			foreach (DataRow row in dataTable.Rows)
			{
				if (string.IsNullOrEmpty(Function.ToString(row["CardCode"])))
				{
					WinCommons.Error(Enums.MessageCode.None, "Vendor trong các item không được để trống!");
					return false;
				}
			}
		}
		return true;
	}

	public async Task<DocumentTable> GetDocCopy(string docEntry, string objType, string controller)
	{
		try
		{
			using HttpClientHelper<DocumentTable> client = new HttpClientHelper<DocumentTable>(Configuration.ApiServer, controller);
			return await client.GetAsync("CopyFrom", new
			{
				type = objType,
				entry = docEntry,
				bplId = GlobalData.DefaultBranch,
				username = GlobalData.UserName
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return null;
	}

	public async Task<DocumentLineTable> GetDocLineCopy(string docEntry, string objType, string controller)
	{
		try
		{
			using HttpClientHelper<DocumentLineTable> client = new HttpClientHelper<DocumentLineTable>(Configuration.ApiServer, controller);
			return await client.GetAsync("CopyFromLine", new
			{
				type = objType,
				entry = docEntry
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return null;
	}

	private async Task<DocumentsImpl> GetDataCopyTo(string docEntry, string controller)
	{
		_ = 4;
		try
		{
			DocumentsImpl docPurReq = new DocumentsImpl();
			new DocumentTable();
			new DocumentLineTable();
			DocumentTable tableDocPR;
			DocumentLineTable documentLineTable;
			if (Function.ParseInt(Function.ToString(((Common.ListItems)cbb_BaseDoc.SelectedItem).Value)) == 17)
			{
				tableDocPR = await GetDocCopy(docEntry, 17.ToString(), controller);
				documentLineTable = await GetDocLineCopy(docEntry, 17.ToString(), controller);
			}
			else
			{
				tableDocPR = await GetDocCopy(docEntry, 23.ToString(), controller);
				documentLineTable = await GetDocLineCopy(docEntry, 23.ToString(), controller);
			}
			List<Document_LinesImpl> lstLineFullPR = ((documentLineTable != null) ? documentLineTable.ToDocumentLine() : new List<Document_LinesImpl>());
			List<Document_LinesImpl> lstLinePurReq = new List<Document_LinesImpl>();
			DataTable dtLine = grd_SalesOrder.DataSource as DataTable;
			if (lstLineFullPR != null && lstLineFullPR.Count > 0 && dtLine.Rows.Count == lstLineFullPR.Count)
			{
				int count = 0;
				for (int i = 0; i < lstLineFullPR.Count; i++)
				{
					DataTable dataTable = await GetItemByCode(lstLineFullPR[i].ItemCode);
					if (dataTable.IsNotNull() && dataTable.ExistsColumn("CardCode"))
					{
						lstLineFullPR[i].VatGroup = Function.ToString(dataTable.Rows[0]["VatGroupPu"]);
					}
					lstLineFullPR[i].CardCode = Function.ToString(dtLine.Rows[count]["CardCode"]);
					lstLineFullPR[i].BaseType = -1;
					lstLinePurReq.Add(lstLineFullPR[i]);
				}
			}
			if (lstLinePurReq.Count > 0)
			{
				if (tableDocPR != null && tableDocPR.Rows.Count > 0)
				{
					docPurReq = tableDocPR[0].ToDocument();
					docPurReq.DocEntry = 0;
					docPurReq.Lines = lstLinePurReq;
					docPurReq.SendNotification = BoYesNoEnum.tNO;
					docPurReq.UseCorrectionVATGroup = BoYesNoEnum.tNO;
				}
				else
				{
					docPurReq = null;
				}
			}
			return docPurReq;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return null;
		}
	}

	public async Task SavePR(string docEntry)
	{
		WinCommons._OpenProcessing();
		try
		{
			if (ValidateRequester())
			{
				DocumentsImpl documentsImpl = await GetDataCopyTo(docEntry, "PurchaseRequest");
				if (documentsImpl != null)
				{
					documentsImpl.CardCode = "";
					documentsImpl.CardName = "";
					documentsImpl.CntctCode = 0;
					documentsImpl.Email = " ";
					documentsImpl.ReqDate = DateTime.Today;
					documentsImpl.Requester = txt_Req.Text;
					documentsImpl.RequesterBranch = GlobalData.DefaultBranch;
					documentsImpl.RequesterDepartment = Function.ParseInt(GlobalData.DeparmentLogin);
					DataTable dataTable = grd_SalesOrder.DataSource as DataTable;
					if (dataTable.IsNotNull() && dataTable.ExistsColumn("DocEntry") && dataTable.ExistsColumn("CardCode"))
					{
						int num = 0;
						foreach (Document_LinesImpl line in documentsImpl.Lines)
						{
							line.LineVendor = Function.ToString(dataTable.Rows[num]["CardCode"]);
							num++;
						}
					}
					HttpResult httpResult = await SavePurchase(documentsImpl, "PurchaseRequest");
					if (httpResult.MessageCode != Enums.MessageCode.Success)
					{
						WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeMedium, httpResult.Message);
					}
					else
					{
						WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task SavePO(string docEntry)
	{
		WinCommons._OpenProcessing();
		try
		{
			if (ValidateOrder())
			{
				DocumentsImpl docPurReq = await GetDataCopyTo(docEntry, "PurchaseOder");
				if (docPurReq != null)
				{
					bool check = false;
					foreach (IGrouping<string, Document_LinesImpl> item in (from x in docPurReq.Lines
						group x by x.CardCode).ToList())
					{
						docPurReq.CardCode = Function.ToString(item.Key);
						docPurReq.CntctCode = 0;
						docPurReq.Email = "";
						docPurReq.Requester = "";
						docPurReq.RequesterName = "";
						docPurReq.Lines = docPurReq.Lines.Where((Document_LinesImpl x) => x.CardCode == item.Key).ToList();
						docPurReq.DocDueDate = date_CurrentDate.DateTime.Date;
						docPurReq.ReqDate = date_CurrentDate.DateTime.Date;
						HttpResult httpResult = await SavePurchase(docPurReq, "PurchaseOder");
						if (httpResult.MessageCode != Enums.MessageCode.Success)
						{
							check = true;
							WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeMedium, httpResult.Message);
							break;
						}
					}
					if (!check)
					{
						WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task SavePQ(string docEntry)
	{
		WinCommons._OpenProcessing();
		try
		{
			if (ValidatePQ())
			{
				DocumentsImpl docPurReq = await GetDataCopyTo(docEntry, "PPurchaseQuotation");
				if (docPurReq != null)
				{
					bool check = false;
					foreach (IGrouping<string, Document_LinesImpl> item in (from x in docPurReq.Lines
						group x by x.CardCode).ToList())
					{
						docPurReq.CardCode = Function.ToString(item.Key);
						docPurReq.CntctCode = 0;
						docPurReq.Email = "";
						docPurReq.Requester = "";
						docPurReq.RequesterName = "";
						docPurReq.Lines = docPurReq.Lines.Where((Document_LinesImpl x) => x.CardCode == item.Key).ToList();
						docPurReq.DocDueDate = date_CurrentDate.DateTime.Date;
						docPurReq.ReqDate = date_CurrentDate.DateTime.Date;
						HttpResult httpResult = await SavePurchase(docPurReq, "PPurchaseQuotation");
						if (httpResult.MessageCode != Enums.MessageCode.Success)
						{
							check = true;
							WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeMedium, httpResult.Message);
							break;
						}
					}
					if (!check)
					{
						WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
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

	public async Task<HttpResult> SavePurchase(DocumentsImpl doc, string controller)
	{
		try
		{
			using HttpClientHelper<HttpResult> client = new HttpClientHelper<HttpResult>(Configuration.ApiServer, controller);
			return await client.PostAsync("SaveDoc", doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_ProcurementConfirm_Load(object sender, EventArgs e)
	{
		try
		{
			LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cbb_TargetDocument_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			if (cbb_TargetDocument.SelectedItem != null)
			{
				txt_Req.Text = string.Empty;
				txt_Name.Text = string.Empty;
				txt_Vendor.Text = string.Empty;
				switch (Function.ParseInt(Function.ToString(((Common.ListItems)cbb_TargetDocument.SelectedItem).Value)))
				{
				case 22:
					txt_Req.Enabled = false;
					txt_Vendor.Enabled = true;
					txt_Name.Enabled = true;
					cbb_Req.Enabled = false;
					chk_Email.Enabled = false;
					txt_Email.Enabled = false;
					lbl_Valid.Utext = Apzon.Commons.Properties.Resources.Doc_gvDoc_gcDeliveyDate;
					break;
				case 540000006:
					txt_Req.Enabled = false;
					txt_Vendor.Enabled = false;
					txt_Name.Enabled = false;
					cbb_Req.Enabled = false;
					chk_Email.Enabled = false;
					txt_Email.Enabled = false;
					lbl_Valid.Utext = Apzon.Commons.Properties.Resources.Doc_gvDoc_gcValidUntil;
					break;
				case 1470000113:
					txt_Req.Enabled = true;
					txt_Vendor.Enabled = false;
					txt_Name.Enabled = true;
					cbb_Req.Enabled = true;
					chk_Email.Enabled = true;
					txt_Email.Enabled = true;
					lbl_Valid.Utext = Apzon.Commons.Properties.Resources.Doc_gvDoc_gcValidUntil;
					break;
				case 202:
					txt_Req.Enabled = false;
					txt_Vendor.Enabled = false;
					txt_Name.Enabled = false;
					cbb_Req.Enabled = false;
					chk_Email.Enabled = false;
					txt_Email.Enabled = false;
					lbl_Valid.Utext = Apzon.Commons.Properties.Resources.Doc_gvDoc_gcDeliveyDate;
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

	private void chk_Email_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (chk_Email.Checked)
			{
				txt_Email.ReadOnly = false;
			}
			else
			{
				txt_Email.ReadOnly = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void cbb_BaseDoc_SelectedIndexChanged(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (cbb_BaseDoc.SelectedItem == null)
			{
				return;
			}
			int num = Function.ParseInt(Function.ToString(((Common.ListItems)cbb_BaseDoc.SelectedItem).Value));
			List<Common.Table_ID> targetDoc = new List<Common.Table_ID>();
			Common.Table_ID table_ID = new Common.Table_ID();
			table_ID.ID = Function.ToString(22);
			table_ID.Name = Apzon.Commons.Properties.Resources.lb_PurchaseOrder;
			targetDoc.Add(table_ID);
			Common.Table_ID table_ID2 = new Common.Table_ID();
			table_ID2.ID = Function.ToString(540000006);
			table_ID2.Name = Apzon.Commons.Properties.Resources.lb_PurchaseQuotation;
			targetDoc.Add(table_ID2);
			Common.Table_ID table_ID3 = new Common.Table_ID();
			table_ID3.ID = Function.ToString(1470000113);
			table_ID3.Name = Apzon.Commons.Properties.Resources.lb_PurchaseRequest;
			targetDoc.Add(table_ID3);
			if (num == 17)
			{
				Common.Table_ID table_ID4 = new Common.Table_ID();
				table_ID4.ID = Function.ToString(202);
				table_ID4.Name = Apzon.Commons.Properties.Resources.Title_frmProductionOder;
				targetDoc.Add(table_ID4);
			}
			DataSet ds = await LoadPreferredVendor(num.ToString(), GlobalData.UserName, GlobalData.DefaultBranch.ToString());
			if (ds.IsHasStructure())
			{
				await GridViewHelper.GenerateGrid(grd_Customer, gv_Costomer, ds.Tables[0], ds.Tables[1]);
				if (gv_Costomer.Columns.Count((GridColumn o) => o.Visible) > 5)
				{
					Size = new Size(800, 600);
				}
			}
			ds.Tables.Clear();
			ds.Dispose();
			cbb_TargetDocument.Enabled = true;
			WinFuntions._addComboBox(cbb_TargetDocument, targetDoc, _selectFirstItem: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_Vendor_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index != 0)
			{
				return;
			}
			frm_ChooseFromList frm_ChooseFromList2 = ShowChooseFromListBp(txt_Vendor.Text);
			frm_ChooseFromList2.ShowDialog(this);
			if (!((frm_ChooseFromList2.ChooseItem != null) ? true : false))
			{
				return;
			}
			txt_Vendor.Text = frm_ChooseFromList2.ChooseItem.CardCode;
			txt_Name.Text = frm_ChooseFromList2.ChooseItem.CardName;
			_CntctPrsnCode = Function.ParseInt(frm_ChooseFromList2.ChooseItem.ContactPersonCode);
			DataTable dataTable = grd_SalesOrder.DataSource as DataTable;
			if (!dataTable.IsNotNull() || !dataTable.ExistsColumn("CardCode") || !dataTable.ExistsColumn("CardName"))
			{
				return;
			}
			foreach (DataRow row in dataTable.Rows)
			{
				row["CardCode"] = txt_Vendor.Text;
				row["CardName"] = txt_Name.Text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_Req_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index != 0 || cbb_Req.SelectedItem == null)
			{
				return;
			}
			if (Function.ToString(((Common.ListItems)cbb_Req.SelectedItem).Value) == "1")
			{
				frm_ChooseFromList frm_ChooseFromList2 = ShowChooseFromListUser(txt_Req.Text);
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					txt_Req.Text = frm_ChooseFromList2.ChooseItem["Requester"];
					txt_Name.Text = frm_ChooseFromList2.ChooseItem["ReqName"];
				}
			}
			else
			{
				frm_ChooseFromList frm_ChooseFromList3 = ShowChooseFromListEmployee(txt_Req.Text);
				frm_ChooseFromList3.ShowDialog(this);
				if (frm_ChooseFromList3.ChooseItem != null)
				{
					txt_Req.Text = Function.ToString(frm_ChooseFromList3.ChooseItem["empID"]);
					txt_Name.Text = frm_ChooseFromList3.ChooseItem["lastName"];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cbb_Req_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			txt_Req.Text = string.Empty;
			txt_Name.Text = string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			try
			{
				if (cbb_TargetDocument.SelectedItem != null)
				{
					switch (Function.ParseInt(Function.ToString(((Common.ListItems)cbb_TargetDocument.SelectedItem).Value)))
					{
					case 22:
						await SavePO(_docEntry);
						break;
					case 1470000113:
						await SavePR(_docEntry);
						break;
					case 540000006:
						await SavePQ(_docEntry);
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
		WinCommons._CloseProcessing();
	}

	private async void gv_Costomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			int[] selectedRows = gv_Costomer.GetSelectedRows();
			List<string> list = new List<string>();
			foreach (int rowHandle in selectedRows)
			{
				string item = Function.ToString(gv_Costomer.GetRowCellValue(rowHandle, "DocEntry"));
				list.Add(item);
			}
			if (list.Count > 0)
			{
				_docEntry = string.Join(",", list.ToArray());
				txt_Vendor.Text = string.Empty;
				DataSet ds = await LoadItemLine(Function.ParseInt(Function.ToString(((Common.ListItems)cbb_BaseDoc.SelectedItem).Value)).ToString(), _docEntry, GlobalData.UserName, GlobalData.DefaultBranch.ToString());
				if (ds.IsHasStructure())
				{
					await GridViewHelper.GenerateGrid(grd_SalesOrder, gv_SalesOrder, ds.Tables[0], ds.Tables[1]);
				}
				ds.Tables.Clear();
				ds.Dispose();
			}
			else
			{
				grd_SalesOrder.DataSource = new DataTable();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
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
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.lbl_BaseDoc = new Apzon.Controls.Controls.LabelLine();
		this.cbb_BaseDoc = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.txt_Vendor = new Apzon.Controls.Editor.ButtonEditEx();
		this.txt_Req = new Apzon.Controls.Editor.ButtonEditEx();
		this.cbb_TargetDocSeries = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.lbl_Email = new Apzon.Controls.Controls.LabelLine();
		this.txt_Email = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_Name = new Apzon.Controls.Controls.LabelLine();
		this.txt_Name = new Apzon.Controls.Editor.TextEditEx();
		this.cbb_Req = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.lbl_Requester = new Apzon.Controls.Controls.LabelLine();
		this.cbb_TargetDocument = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.lbl_targetDocument = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Valid = new Apzon.Controls.Controls.LabelLine();
		this.date_CurrentTime = new Apzon.Controls.Editor.DateEditEx();
		this.lbl_ReqDate = new Apzon.Controls.Controls.LabelLine();
		this.date_CurrentDate = new Apzon.Controls.Editor.DateEditEx();
		this.chk_Email = new Apzon.Controls.Editor.CheckEditEx();
		this.lbl_TargetS = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Vendor = new Apzon.Controls.Controls.LabelLine();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Customer = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Costomer = new Apzon.Controls.Grid.GridViewEx();
		this.grd_SalesOrder = new Apzon.Controls.Grid.GridControlEx();
		this.gv_SalesOrder = new Apzon.Controls.Grid.GridViewEx();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cbb_BaseDoc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Vendor.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Req.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_TargetDocSeries.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Email.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_Req.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_TargetDocument.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_CurrentTime.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_CurrentTime.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_CurrentDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_CurrentDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Email.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Customer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Costomer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_SalesOrder).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SalesOrder).BeginInit();
		base.SuspendLayout();
		this.panelControl1.Controls.Add(this.lbl_BaseDoc);
		this.panelControl1.Controls.Add(this.cbb_BaseDoc);
		this.panelControl1.Controls.Add(this.txt_Vendor);
		this.panelControl1.Controls.Add(this.txt_Req);
		this.panelControl1.Controls.Add(this.cbb_TargetDocSeries);
		this.panelControl1.Controls.Add(this.lbl_Email);
		this.panelControl1.Controls.Add(this.txt_Email);
		this.panelControl1.Controls.Add(this.lbl_Name);
		this.panelControl1.Controls.Add(this.txt_Name);
		this.panelControl1.Controls.Add(this.cbb_Req);
		this.panelControl1.Controls.Add(this.lbl_Requester);
		this.panelControl1.Controls.Add(this.cbb_TargetDocument);
		this.panelControl1.Controls.Add(this.lbl_targetDocument);
		this.panelControl1.Controls.Add(this.lbl_Valid);
		this.panelControl1.Controls.Add(this.date_CurrentTime);
		this.panelControl1.Controls.Add(this.lbl_ReqDate);
		this.panelControl1.Controls.Add(this.date_CurrentDate);
		this.panelControl1.Controls.Add(this.chk_Email);
		this.panelControl1.Controls.Add(this.lbl_TargetS);
		this.panelControl1.Controls.Add(this.lbl_Vendor);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(713, 130);
		this.panelControl1.TabIndex = 1;
		this.lbl_BaseDoc.EditControl = null;
		this.lbl_BaseDoc.EditValue = "Base Doc.";
		this.lbl_BaseDoc.FieldName = null;
		this.lbl_BaseDoc.HideWhenEmpty = false;
		this.lbl_BaseDoc.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_BaseDoc.Location = new System.Drawing.Point(8, 7);
		this.lbl_BaseDoc.Name = "lbl_BaseDoc";
		this.lbl_BaseDoc.ReadOnly = false;
		this.lbl_BaseDoc.SettingInVisible = false;
		this.lbl_BaseDoc.SettingReadonly = false;
		this.lbl_BaseDoc.ShowUnderLine = true;
		this.lbl_BaseDoc.Size = new System.Drawing.Size(97, 20);
		this.lbl_BaseDoc.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_BaseDoc.TabIndex = 27;
		this.lbl_BaseDoc.Utext = "Base Doc.";
		this.cbb_BaseDoc.EditControl = null;
		this.cbb_BaseDoc.FieldName = null;
		this.cbb_BaseDoc.IsSearchControl = false;
		this.cbb_BaseDoc.Location = new System.Drawing.Point(103, 5);
		this.cbb_BaseDoc.Name = "cbb_BaseDoc";
		this.cbb_BaseDoc.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbb_BaseDoc.SettingInVisible = false;
		this.cbb_BaseDoc.SettingReadonly = false;
		this.cbb_BaseDoc.Size = new System.Drawing.Size(159, 20);
		this.cbb_BaseDoc.TabIndex = 26;
		this.cbb_BaseDoc.SelectedIndexChanged += new System.EventHandler(this.cbb_BaseDoc_SelectedIndexChanged);
		this.txt_Vendor.EditControl = null;
		this.txt_Vendor.FieldName = null;
		this.txt_Vendor.IsSearchControl = false;
		this.txt_Vendor.Location = new System.Drawing.Point(103, 30);
		this.txt_Vendor.Name = "txt_Vendor";
		this.txt_Vendor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_Vendor.SettingInVisible = false;
		this.txt_Vendor.SettingReadonly = false;
		this.txt_Vendor.Size = new System.Drawing.Size(159, 20);
		this.txt_Vendor.TabIndex = 25;
		this.txt_Vendor.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_Vendor_ButtonPressed);
		this.txt_Req.EditControl = null;
		this.txt_Req.FieldName = null;
		this.txt_Req.IsSearchControl = false;
		this.txt_Req.Location = new System.Drawing.Point(175, 55);
		this.txt_Req.Name = "txt_Req";
		this.txt_Req.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_Req.SettingInVisible = false;
		this.txt_Req.SettingReadonly = false;
		this.txt_Req.Size = new System.Drawing.Size(87, 20);
		this.txt_Req.TabIndex = 24;
		this.txt_Req.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_Req_ButtonPressed);
		this.cbb_TargetDocSeries.EditControl = null;
		this.cbb_TargetDocSeries.FieldName = null;
		this.cbb_TargetDocSeries.IsSearchControl = false;
		this.cbb_TargetDocSeries.Location = new System.Drawing.Point(103, 105);
		this.cbb_TargetDocSeries.Name = "cbb_TargetDocSeries";
		this.cbb_TargetDocSeries.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbb_TargetDocSeries.SettingInVisible = false;
		this.cbb_TargetDocSeries.SettingReadonly = false;
		this.cbb_TargetDocSeries.Size = new System.Drawing.Size(159, 20);
		this.cbb_TargetDocSeries.TabIndex = 23;
		this.lbl_Email.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_Email.EditControl = null;
		this.lbl_Email.EditValue = "E-mail Address";
		this.lbl_Email.FieldName = null;
		this.lbl_Email.HideWhenEmpty = false;
		this.lbl_Email.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Email.Location = new System.Drawing.Point(460, 57);
		this.lbl_Email.Name = "lbl_Email";
		this.lbl_Email.ReadOnly = false;
		this.lbl_Email.SettingInVisible = false;
		this.lbl_Email.SettingReadonly = false;
		this.lbl_Email.ShowUnderLine = true;
		this.lbl_Email.Size = new System.Drawing.Size(89, 20);
		this.lbl_Email.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Email.TabIndex = 22;
		this.lbl_Email.Utext = "E-mail Address";
		this.txt_Email.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Email.EditControl = null;
		this.txt_Email.FieldName = null;
		this.txt_Email.IsSearchControl = false;
		this.txt_Email.Location = new System.Drawing.Point(548, 55);
		this.txt_Email.Name = "txt_Email";
		this.txt_Email.Properties.ReadOnly = true;
		this.txt_Email.SettingInVisible = false;
		this.txt_Email.SettingReadonly = false;
		this.txt_Email.Size = new System.Drawing.Size(159, 20);
		this.txt_Email.TabIndex = 21;
		this.lbl_Name.EditControl = null;
		this.lbl_Name.EditValue = "Name";
		this.lbl_Name.FieldName = null;
		this.lbl_Name.HideWhenEmpty = false;
		this.lbl_Name.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Name.Location = new System.Drawing.Point(8, 82);
		this.lbl_Name.Name = "lbl_Name";
		this.lbl_Name.ReadOnly = false;
		this.lbl_Name.SettingInVisible = false;
		this.lbl_Name.SettingReadonly = false;
		this.lbl_Name.ShowUnderLine = true;
		this.lbl_Name.Size = new System.Drawing.Size(97, 20);
		this.lbl_Name.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Name.TabIndex = 20;
		this.lbl_Name.Utext = "Name";
		this.txt_Name.EditControl = null;
		this.txt_Name.FieldName = null;
		this.txt_Name.IsSearchControl = false;
		this.txt_Name.Location = new System.Drawing.Point(103, 80);
		this.txt_Name.Name = "txt_Name";
		this.txt_Name.Properties.ReadOnly = true;
		this.txt_Name.SettingInVisible = false;
		this.txt_Name.SettingReadonly = false;
		this.txt_Name.Size = new System.Drawing.Size(159, 20);
		this.txt_Name.TabIndex = 19;
		this.cbb_Req.EditControl = null;
		this.cbb_Req.FieldName = null;
		this.cbb_Req.IsSearchControl = false;
		this.cbb_Req.Location = new System.Drawing.Point(103, 55);
		this.cbb_Req.Name = "cbb_Req";
		this.cbb_Req.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbb_Req.SettingInVisible = false;
		this.cbb_Req.SettingReadonly = false;
		this.cbb_Req.Size = new System.Drawing.Size(66, 20);
		this.cbb_Req.TabIndex = 18;
		this.cbb_Req.SelectedIndexChanged += new System.EventHandler(this.cbb_Req_SelectedIndexChanged);
		this.lbl_Requester.EditControl = null;
		this.lbl_Requester.EditValue = "Requester";
		this.lbl_Requester.FieldName = null;
		this.lbl_Requester.HideWhenEmpty = false;
		this.lbl_Requester.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Requester.Location = new System.Drawing.Point(8, 56);
		this.lbl_Requester.Name = "lbl_Requester";
		this.lbl_Requester.ReadOnly = false;
		this.lbl_Requester.SettingInVisible = false;
		this.lbl_Requester.SettingReadonly = false;
		this.lbl_Requester.ShowUnderLine = true;
		this.lbl_Requester.Size = new System.Drawing.Size(97, 20);
		this.lbl_Requester.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Requester.TabIndex = 16;
		this.lbl_Requester.Utext = "Requester";
		this.cbb_TargetDocument.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.cbb_TargetDocument.EditControl = null;
		this.cbb_TargetDocument.Enabled = false;
		this.cbb_TargetDocument.FieldName = null;
		this.cbb_TargetDocument.IsSearchControl = false;
		this.cbb_TargetDocument.Location = new System.Drawing.Point(548, 5);
		this.cbb_TargetDocument.Name = "cbb_TargetDocument";
		this.cbb_TargetDocument.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbb_TargetDocument.SettingInVisible = false;
		this.cbb_TargetDocument.SettingReadonly = false;
		this.cbb_TargetDocument.Size = new System.Drawing.Size(159, 20);
		this.cbb_TargetDocument.TabIndex = 15;
		this.cbb_TargetDocument.SelectedIndexChanged += new System.EventHandler(this.cbb_TargetDocument_SelectedIndexChanged);
		this.lbl_targetDocument.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_targetDocument.EditControl = null;
		this.lbl_targetDocument.EditValue = "Target Document";
		this.lbl_targetDocument.FieldName = null;
		this.lbl_targetDocument.HideWhenEmpty = false;
		this.lbl_targetDocument.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_targetDocument.Location = new System.Drawing.Point(460, 7);
		this.lbl_targetDocument.Name = "lbl_targetDocument";
		this.lbl_targetDocument.ReadOnly = false;
		this.lbl_targetDocument.SettingInVisible = false;
		this.lbl_targetDocument.SettingReadonly = false;
		this.lbl_targetDocument.ShowUnderLine = true;
		this.lbl_targetDocument.Size = new System.Drawing.Size(89, 20);
		this.lbl_targetDocument.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_targetDocument.TabIndex = 14;
		this.lbl_targetDocument.Utext = "Target Document";
		this.lbl_Valid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_Valid.EditControl = null;
		this.lbl_Valid.EditValue = "Valid Until";
		this.lbl_Valid.FieldName = null;
		this.lbl_Valid.HideWhenEmpty = false;
		this.lbl_Valid.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Valid.Location = new System.Drawing.Point(460, 107);
		this.lbl_Valid.Name = "lbl_Valid";
		this.lbl_Valid.ReadOnly = false;
		this.lbl_Valid.SettingInVisible = false;
		this.lbl_Valid.SettingReadonly = false;
		this.lbl_Valid.ShowUnderLine = true;
		this.lbl_Valid.Size = new System.Drawing.Size(89, 20);
		this.lbl_Valid.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Valid.TabIndex = 13;
		this.lbl_Valid.Utext = "Valid Until";
		this.date_CurrentTime.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_CurrentTime.EditControl = null;
		this.date_CurrentTime.EditValue = null;
		this.date_CurrentTime.FieldName = null;
		this.date_CurrentTime.Location = new System.Drawing.Point(548, 105);
		this.date_CurrentTime.Name = "date_CurrentTime";
		this.date_CurrentTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_CurrentTime.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_CurrentTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.date_CurrentTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_CurrentTime.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.date_CurrentTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_CurrentTime.Properties.IsSearchControl = false;
		this.date_CurrentTime.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.date_CurrentTime.SettingInVisible = false;
		this.date_CurrentTime.SettingReadonly = false;
		this.date_CurrentTime.Size = new System.Drawing.Size(159, 20);
		this.date_CurrentTime.TabIndex = 12;
		this.lbl_ReqDate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_ReqDate.EditControl = null;
		this.lbl_ReqDate.EditValue = "Required Date";
		this.lbl_ReqDate.FieldName = null;
		this.lbl_ReqDate.HideWhenEmpty = false;
		this.lbl_ReqDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ReqDate.Location = new System.Drawing.Point(460, 81);
		this.lbl_ReqDate.Name = "lbl_ReqDate";
		this.lbl_ReqDate.ReadOnly = false;
		this.lbl_ReqDate.SettingInVisible = false;
		this.lbl_ReqDate.SettingReadonly = false;
		this.lbl_ReqDate.ShowUnderLine = true;
		this.lbl_ReqDate.Size = new System.Drawing.Size(89, 20);
		this.lbl_ReqDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ReqDate.TabIndex = 11;
		this.lbl_ReqDate.Utext = "Required Date";
		this.date_CurrentDate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_CurrentDate.EditControl = null;
		this.date_CurrentDate.EditValue = null;
		this.date_CurrentDate.FieldName = null;
		this.date_CurrentDate.Location = new System.Drawing.Point(548, 80);
		this.date_CurrentDate.Name = "date_CurrentDate";
		this.date_CurrentDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_CurrentDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_CurrentDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.date_CurrentDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_CurrentDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.date_CurrentDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_CurrentDate.Properties.IsSearchControl = false;
		this.date_CurrentDate.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.date_CurrentDate.SettingInVisible = false;
		this.date_CurrentDate.SettingReadonly = false;
		this.date_CurrentDate.Size = new System.Drawing.Size(159, 20);
		this.date_CurrentDate.TabIndex = 10;
		this.chk_Email.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.chk_Email.EditControl = null;
		this.chk_Email.FieldName = null;
		this.chk_Email.IsSearchControl = false;
		this.chk_Email.Location = new System.Drawing.Point(460, 30);
		this.chk_Email.Name = "chk_Email";
		this.chk_Email.Properties.Caption = "Send E-Mail if PO or GRPO is Added";
		this.chk_Email.SettingInVisible = false;
		this.chk_Email.SettingReadonly = false;
		this.chk_Email.Size = new System.Drawing.Size(194, 20);
		this.chk_Email.TabIndex = 8;
		this.chk_Email.CheckedChanged += new System.EventHandler(this.chk_Email_CheckedChanged);
		this.lbl_TargetS.EditControl = null;
		this.lbl_TargetS.EditValue = "Target Doc. Series";
		this.lbl_TargetS.FieldName = null;
		this.lbl_TargetS.HideWhenEmpty = false;
		this.lbl_TargetS.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_TargetS.Location = new System.Drawing.Point(8, 107);
		this.lbl_TargetS.Name = "lbl_TargetS";
		this.lbl_TargetS.ReadOnly = false;
		this.lbl_TargetS.SettingInVisible = false;
		this.lbl_TargetS.SettingReadonly = false;
		this.lbl_TargetS.ShowUnderLine = true;
		this.lbl_TargetS.Size = new System.Drawing.Size(97, 20);
		this.lbl_TargetS.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_TargetS.TabIndex = 7;
		this.lbl_TargetS.Utext = "Target Doc. Series";
		this.lbl_Vendor.EditControl = null;
		this.lbl_Vendor.EditValue = "Vendor";
		this.lbl_Vendor.FieldName = null;
		this.lbl_Vendor.HideWhenEmpty = false;
		this.lbl_Vendor.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Vendor.Location = new System.Drawing.Point(8, 32);
		this.lbl_Vendor.Name = "lbl_Vendor";
		this.lbl_Vendor.ReadOnly = false;
		this.lbl_Vendor.SettingInVisible = false;
		this.lbl_Vendor.SettingReadonly = false;
		this.lbl_Vendor.ShowUnderLine = true;
		this.lbl_Vendor.Size = new System.Drawing.Size(97, 20);
		this.lbl_Vendor.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Vendor.TabIndex = 5;
		this.lbl_Vendor.Utext = "Vendor";
		this.panelControl2.Controls.Add(this.btn_Save);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl2.Location = new System.Drawing.Point(0, 447);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(713, 34);
		this.panelControl2.TabIndex = 2;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(5, 6);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(75, 23);
		this.btn_Save.TabIndex = 0;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 130);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Customer);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_SalesOrder);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(713, 317);
		this.splitContainerControl1.SplitterPosition = 140;
		this.splitContainerControl1.TabIndex = 3;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Customer.AllowPasteExcel = true;
		this.grd_Customer.ClipboardData = null;
		this.grd_Customer.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Customer.Location = new System.Drawing.Point(0, 0);
		this.grd_Customer.MainView = this.gv_Costomer;
		this.grd_Customer.Name = "grd_Customer";
		this.grd_Customer.Size = new System.Drawing.Size(713, 140);
		this.grd_Customer.TabIndex = 0;
		this.grd_Customer.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Costomer });
		this.gv_Costomer.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Costomer.ColumnKey = null;
		this.gv_Costomer.DisableFormSetting = false;
		this.gv_Costomer.DisplayException = false;
		this.gv_Costomer.EditColumn = null;
		this.gv_Costomer.FieldCurrency = null;
		this.gv_Costomer.FieldRate = null;
		this.gv_Costomer.FormId = null;
		this.gv_Costomer.GridControl = this.grd_Customer;
		this.gv_Costomer.IgnoreFilter = false;
		this.gv_Costomer.IndexEditRow = -1;
		this.gv_Costomer.IndicatorWidth = 35;
		this.gv_Costomer.IsApzTable = false;
		this.gv_Costomer.IsInsertNewRow = false;
		this.gv_Costomer.IsLoadingData = false;
		this.gv_Costomer.IsMappingContextAction = false;
		this.gv_Costomer.ItemId = null;
		this.gv_Costomer.LstColumnAllowBatch = null;
		this.gv_Costomer.LstColumnNotAllowBatch = null;
		this.gv_Costomer.LstColumnNotEditable = null;
		this.gv_Costomer.LstColumNotSetting = null;
		this.gv_Costomer.Name = "gv_Costomer";
		this.gv_Costomer.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
		this.gv_Costomer.OptionsSelection.MultiSelect = true;
		this.gv_Costomer.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Costomer.OptionsView.ShowGroupPanel = false;
		this.gv_Costomer.UseBatchMod = false;
		this.gv_Costomer.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gv_Costomer_SelectionChanged);
		this.grd_SalesOrder.AllowPasteExcel = true;
		this.grd_SalesOrder.ClipboardData = null;
		this.grd_SalesOrder.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_SalesOrder.Location = new System.Drawing.Point(0, 0);
		this.grd_SalesOrder.MainView = this.gv_SalesOrder;
		this.grd_SalesOrder.Name = "grd_SalesOrder";
		this.grd_SalesOrder.Size = new System.Drawing.Size(713, 167);
		this.grd_SalesOrder.TabIndex = 0;
		this.grd_SalesOrder.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_SalesOrder });
		this.gv_SalesOrder.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_SalesOrder.ColumnKey = null;
		this.gv_SalesOrder.DisableFormSetting = false;
		this.gv_SalesOrder.DisplayException = false;
		this.gv_SalesOrder.EditColumn = null;
		this.gv_SalesOrder.FieldCurrency = null;
		this.gv_SalesOrder.FieldRate = null;
		this.gv_SalesOrder.FormId = null;
		this.gv_SalesOrder.GridControl = this.grd_SalesOrder;
		this.gv_SalesOrder.IgnoreFilter = false;
		this.gv_SalesOrder.IndexEditRow = -1;
		this.gv_SalesOrder.IndicatorWidth = 35;
		this.gv_SalesOrder.IsApzTable = false;
		this.gv_SalesOrder.IsInsertNewRow = false;
		this.gv_SalesOrder.IsLoadingData = false;
		this.gv_SalesOrder.IsMappingContextAction = false;
		this.gv_SalesOrder.ItemId = null;
		this.gv_SalesOrder.LstColumnAllowBatch = null;
		this.gv_SalesOrder.LstColumnNotAllowBatch = null;
		this.gv_SalesOrder.LstColumnNotEditable = null;
		this.gv_SalesOrder.LstColumNotSetting = null;
		this.gv_SalesOrder.Name = "gv_SalesOrder";
		this.gv_SalesOrder.OptionsView.ShowGroupPanel = false;
		this.gv_SalesOrder.UseBatchMod = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(713, 481);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.panelControl2);
		base.Controls.Add(this.panelControl1);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ProcurementConfirm";
		base.ShowInTaskbar = false;
		this.Text = "Procurement Confirmation Wizard";
		base.Load += new System.EventHandler(this.frm_ProcurementConfirm_Load);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cbb_BaseDoc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Vendor.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Req.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_TargetDocSeries.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Email.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_Req.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_TargetDocument.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_CurrentTime.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_CurrentTime.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_CurrentDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_CurrentDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Email.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Customer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Costomer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_SalesOrder).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SalesOrder).EndInit();
		base.ResumeLayout(false);
	}
}
