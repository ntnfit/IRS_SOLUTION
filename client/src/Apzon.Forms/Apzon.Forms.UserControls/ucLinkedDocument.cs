using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Documents;
using Apzon.Forms.Administrator.SetUp.Purchasing;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Inventory.InventoryTransaction;
using Apzon.Forms.ItemsMaster;
using Apzon.Forms.PM;
using Apzon.Forms.Payment;
using Apzon.Forms.Production;
using Apzon.Forms.Properties;
using Apzon.Forms.Purchase;
using Apzon.Forms.Sales;
using Apzon.HttpClientHelper;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.UserControls;

public class ucLinkedDocument : XtraUserControl
{
	private int _linkeDocType = -1;

	private int _linkDocNum = -1;

	public bool checkLinkDraft;

	public bool setDocEntry;

	public bool setDocType;

	public bool setDocNum;

	public bool setLinkDraft;

	public bool isValidating;

	private List<int> listDocType = new List<int>();

	private IContainer components;

	public LookUpEditSimple luk_DocType;

	public CheckEditEx chkLinkDraft;

	public ButtonEditEx cbDocNum;

	public TextEditEx txtDocEntry;

	public string IsLinkDraftFieldName
	{
		get
		{
			return chkLinkDraft.FieldName;
		}
		set
		{
			chkLinkDraft.FieldName = value;
		}
	}

	public string DocEntryFieldName
	{
		get
		{
			return txtDocEntry.FieldName;
		}
		set
		{
			txtDocEntry.FieldName = value;
		}
	}

	public string DocTypeFieldName
	{
		get
		{
			return luk_DocType.FieldName;
		}
		set
		{
			luk_DocType.FieldName = value;
		}
	}

	public string DocNumFieldName
	{
		get
		{
			return cbDocNum.FieldName;
		}
		set
		{
			cbDocNum.FieldName = value;
		}
	}

	public object EditValue
	{
		get
		{
			return cbDocNum.EditValue;
		}
		set
		{
			try
			{
				setDocNum = true;
				cbDocNum.EditValue = value;
				setDocNum = false;
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
				WinCommons.ErrorLineNumber(ex);
			}
		}
	}

	public int DocNum
	{
		get
		{
			return Function.ParseInt(cbDocNum.EditValue);
		}
		set
		{
			EditValue = value;
		}
	}

	public object DocEntry
	{
		get
		{
			if (Function.ParseInt(cbDocNum.EditValue) != 0)
			{
				return txtDocEntry.EditValue;
			}
			return null;
		}
		set
		{
			setDocEntry = true;
			txtDocEntry.EditValue = value;
			setDocEntry = false;
		}
	}

	public int DocType
	{
		get
		{
			try
			{
				if (!(bool)chkLinkDraft.EditValue)
				{
					int num = Function.ParseInt(luk_DocType.EditValue);
					if (num == 0)
					{
						return -1;
					}
					return num;
				}
				return 112;
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
				WinCommons.ErrorLineNumber(ex);
			}
			return -1;
		}
		set
		{
			try
			{
				setDocType = true;
				object docEntry = DocEntry;
				int num = Function.ParseInt(value);
				if (num != 112)
				{
					chkLinkDraft.EditValue = false;
					luk_DocType.EditValue = num;
				}
				else
				{
					chkLinkDraft.EditValue = true;
					int num2 = Function.ParseInt(GetDocTypeFromDocEntryForDraft(Function.ParseInt(docEntry)));
					luk_DocType.EditValue = ((num2 == 0) ? (-1) : num2);
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
				WinCommons.ErrorLineNumber(ex);
			}
			finally
			{
				setDocType = false;
			}
		}
	}

	public string DocName => luk_DocType.Text;

	public bool IsLinked
	{
		get
		{
			try
			{
				return (bool)chkLinkDraft.EditValue;
			}
			catch
			{
			}
			return false;
		}
		set
		{
			setLinkDraft = true;
			chkLinkDraft.EditValue = value;
			setLinkDraft = false;
		}
	}

	public DataTable ValidateInfo { get; set; }

	public string CardCode { get; set; }

	public string FindCardCode { get; set; }

	[Description("Occur after an item is selected from CFL.")]
	public event EventHandler ItemSelected;

	[Description("Occur after validating event finished and before validated event raises.")]
	public event EventHandler AfterValidating;

	public event EventHandler BeforeButtonPressed;

	public ucLinkedDocument()
	{
		InitializeComponent();
		SetupControls();
	}

	private void SetupControls()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ID", typeof(int));
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Rows.Add(-1, "");
			dataTable.Rows.Add(23, "Sales Quotations");
			dataTable.Rows.Add(17, "Sales Orders");
			dataTable.Rows.Add(15, "Deliveries");
			dataTable.Rows.Add(16, "Returns");
			dataTable.Rows.Add(13, "A/R Invoices");
			dataTable.Rows.Add(14, "A/R Credit Memos");
			dataTable.Rows.Add(203, "A/R Down Payment");
			dataTable.Rows.Add(1470000113, "Purchase Request");
			dataTable.Rows.Add(540000006, "Purchase Quotation");
			dataTable.Rows.Add(22, "Purchase Orders");
			dataTable.Rows.Add(20, "Goods Receipt PO");
			dataTable.Rows.Add(21, "Goods Return");
			dataTable.Rows.Add(18, "A/P Invoices");
			dataTable.Rows.Add(19, "A/P Credit Memos");
			dataTable.Rows.Add(48, "Landed Costs");
			dataTable.Rows.Add(204, "A/P Down Payment");
			dataTable.Rows.Add(24, "Incoming Payments");
			dataTable.Rows.Add(46, "Outgoing Payments");
			dataTable.Rows.Add(1250000001, "Inventory Transfer Request");
			dataTable.Rows.Add(67, "Inventory Transfer");
			dataTable.Rows.Add(60, "Goods Issue");
			dataTable.Rows.Add(59, "Goods Receipt");
			dataTable.Rows.Add(162, "Inventory Revaluation");
			dataTable.Rows.Add(202, "Production Orders");
			dataTable.Rows.Add(4, "Items");
			dataTable.Rows.Add(1250000025, "Blanket Agreements");
			dataTable.Rows.Add(ApzObjectTypes.oTrackingIssue, "Tracking Issue");
			dataTable.Rows.Add(ApzObjectTypes.oServiceCalls, "Service Call");
			dataTable.Rows.Add(ApzObjectTypes.oMarketingCampaign, "Marketing Campaign");
			dataTable.AcceptChanges();
			foreach (DataRow item in dataTable.AsEnumerable())
			{
				listDocType.Add(Function.ParseInt(item["ID"]));
			}
			luk_DocType.Properties.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cbDocNum_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (BeforeButtonPressed != null)
			{
				BeforeButtonPressed(sender, EventArgs.Empty);
			}
			ButtonEdit editor = sender as ButtonEdit;
			if (editor == null)
			{
				return;
			}
			if (e.Button.Index == 0)
			{
				BeginInvoke((Action)(() =>
				{
					ShowCFL(editor, FindCardCode);
				}));
			}
			else
			{
				if (e.Button.Index != 1 || (editor.IsModified && !isValidating && !editor.DoValidate()) || string.IsNullOrWhiteSpace(Function.ToString(editor.EditValue)))
				{
					return;
				}
				bool flag = (bool)chkLinkDraft.EditValue;
				ApzObjectTypes apzObjectTypes = (ApzObjectTypes)Function.ParseInt(luk_DocType.EditValue);
				if (!flag)
				{
					switch (apzObjectTypes)
					{
					case ApzObjectTypes.oItems:
					{
						string itemCode = Function.ToString(editor.EditValue);
						frm_ItemMasterData frm2 = new frm_ItemMasterData(flag: false, flagSearchFrm: true, itemCode);
						BeginInvoke((Action)(() =>
						{
							frm2.ShowDialog();
						}));
						break;
					}
					case ApzObjectTypes.oLandedCostsCodes:
					{
						frm_LandedCostsSetup frm = new frm_LandedCostsSetup();
						frm.Code = Function.ToString(editor.EditValue);
						BeginInvoke((Action)(() =>
						{
							frm.ShowDialog();
						}));
						break;
					}
					default:
						Function.ToString(txtDocEntry.EditValue);
						if (Function.ToString(DocEntry) != "" && FindForm() is XtraFormEx)
						{
							(FindForm() as XtraFormEx).DrillDown(apzObjectTypes, apzObjectTypes, Function.ToString(DocEntry));
						}
						break;
					}
				}
				if (flag && !string.IsNullOrEmpty(Function.ToString(txtDocEntry.EditValue)) && FindForm() is XtraFormEx)
				{
					(FindForm() as XtraFormEx).DrillDown(ApzObjectTypes.oDrafts, apzObjectTypes, Function.ToString(DocEntry));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ShowCFL(ButtonEdit editor, string CardCode = "")
	{
		bool flag = false;
		ApzObjectTypes apzObjectTypes = (ApzObjectTypes)Function.ParseInt(luk_DocType.EditValue);
		if (apzObjectTypes == ApzObjectTypes.oNone)
		{
			return;
		}
		if (!(bool)chkLinkDraft.EditValue)
		{
			switch (apzObjectTypes)
			{
			case ApzObjectTypes.oInvoices:
			case ApzObjectTypes.oCreditNotes:
			case ApzObjectTypes.oDeliveryNotes:
			case ApzObjectTypes.oReturns:
			case ApzObjectTypes.oOrders:
			case ApzObjectTypes.oPurchaseInvoices:
			case ApzObjectTypes.oPurchaseCreditNotes:
			case ApzObjectTypes.oPurchaseDeliveryNotes:
			case ApzObjectTypes.oPurchaseReturns:
			case ApzObjectTypes.oPurchaseOrders:
			case ApzObjectTypes.oQuotations:
			case ApzObjectTypes.oInventoryGenEntry:
			case ApzObjectTypes.oInventoryGenExit:
			case ApzObjectTypes.oStockTransfer:
			case ApzObjectTypes.oDownPayments:
			case ApzObjectTypes.oPurchaseDownPayments:
			case ApzObjectTypes.oPurchaseQuotations:
			case ApzObjectTypes.oInventoryTransferRequest:
			{
				frm_List_Document frm_List_Document2 = new frm_List_Document(apzObjectTypes, multiSelect: false, CardCode);
				frm_List_Document2.ShowDialog(FindForm());
				if (frm_List_Document2.ChooseItem != null)
				{
					flag = true;
					DocumentsImpl chooseItem = frm_List_Document2.ChooseItem;
					EditValue = chooseItem.DocNum;
					DocEntry = chooseItem.DocEntry;
					this.CardCode = chooseItem.CardCode;
					if (!string.IsNullOrEmpty(Function.ToString(cbDocNum.EditValue)))
					{
						editor.Properties.Buttons[1].Visible = true;
					}
					else
					{
						editor.Properties.Buttons[1].Visible = false;
					}
				}
				break;
			}
			case ApzObjectTypes.oItems:
			{
				frm_ChooseFromList frm_ChooseFromList7 = new frm_ChooseFromList(Enums.TypeCFL.Items, multiSelect: false, string.Empty);
				frm_ChooseFromList7.ShowDialog(FindForm());
				if (frm_ChooseFromList7.ChooseItem != null)
				{
					flag = true;
					dynamic chooseItem2 = frm_ChooseFromList7.ChooseItem;
					EditValue = (object)chooseItem2.ItemCode;
					DocEntry = (object)chooseItem2.ItemCode;
					this.CardCode = chooseItem2.CardCode;
					if (!string.IsNullOrEmpty(Function.ToString(cbDocNum.EditValue)))
					{
						editor.Properties.Buttons[1].Visible = true;
					}
					else
					{
						editor.Properties.Buttons[1].Visible = false;
					}
				}
				break;
			}
			case ApzObjectTypes.oPurchaseRequest:
			{
				frm_ChooseFromList frm_ChooseFromList8 = new frm_ChooseFromList(Enums.TypeCFL.ListPurchaseRequest, multiSelect: false, string.Empty);
				frm_ChooseFromList8.ShowDialog(FindForm());
				if (frm_ChooseFromList8.ChooseItem != null)
				{
					flag = true;
					dynamic chooseItem3 = frm_ChooseFromList8.ChooseItem;
					EditValue = (object)chooseItem3.DocNum;
					DocEntry = (object)chooseItem3.DocEntry;
					this.CardCode = chooseItem3.CardCode;
					if (!string.IsNullOrEmpty(Function.ToString(cbDocNum.EditValue)))
					{
						editor.Properties.Buttons[1].Visible = true;
					}
					else
					{
						editor.Properties.Buttons[1].Visible = false;
					}
				}
				break;
			}
			case ApzObjectTypes.oMarketingCampaign:
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.MarketingCampaign, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(FindForm());
				if (frm_CFL2.ChooseItem != null)
				{
					EditValue = frm_CFL2.ChooseItem["DocEntry"];
					DocEntry = frm_CFL2.ChooseItem["DocEntry"];
					this.CardCode = "";
				}
				break;
			}
			case ApzObjectTypes.oTrackingIssue:
			{
				frm_ChooseFromList frm_ChooseFromList5 = new frm_ChooseFromList(Enums.TypeCFL.TrackingIssue, multiSelect: false, "", FindCardCode);
				frm_ChooseFromList5.ShowDialog(FindForm());
				if (frm_ChooseFromList5.ChooseItem != null)
				{
					EditValue = (object)frm_ChooseFromList5.ChooseItem["Id"];
					DocEntry = (object)frm_ChooseFromList5.ChooseItem["Id"];
					this.CardCode = "";
				}
				break;
			}
			case ApzObjectTypes.oProductionOrders:
			{
				frm_ChooseFromList frm_ChooseFromList9 = new frm_ChooseFromList(Enums.TypeCFL.ProductionOrder, multiSelect: false, "");
				frm_ChooseFromList9.ShowDialog(FindForm());
				if (frm_ChooseFromList9.ChooseItem != null)
				{
					EditValue = (object)frm_ChooseFromList9.ChooseItem["DocNum"];
					DocEntry = (object)frm_ChooseFromList9.ChooseItem["DocEntry"];
					this.CardCode = "";
				}
				break;
			}
			case ApzObjectTypes.oMaterialRevaluation:
			{
				frm_ChooseFromList frm_ChooseFromList3 = new frm_ChooseFromList(Enums.TypeCFL.MaterialRevaluation, multiSelect: false, "");
				frm_ChooseFromList3.ShowDialog(FindForm());
				if (frm_ChooseFromList3.ChooseItem != null)
				{
					EditValue = (object)frm_ChooseFromList3.ChooseItem["DocNum"];
					DocEntry = (object)frm_ChooseFromList3.ChooseItem["DocEntry"];
				}
				break;
			}
			case ApzObjectTypes.oLandedCostsCodes:
			{
				frm_CFL frm_CFL3 = new frm_CFL(Enums.TypeCFL.GetLanedCostsList, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL3.ShowDialog(FindForm());
				if (frm_CFL3.ChooseItem != null)
				{
					object editValue = (DocEntry = frm_CFL3.ChooseItem["AlcCode"]);
					EditValue = editValue;
				}
				break;
			}
			case ApzObjectTypes.oVendorPayments:
			{
				frm_ChooseFromList frm_ChooseFromList4 = new frm_ChooseFromList(Enums.TypeCFL.OutgoingPayment, multiSelect: false, "");
				frm_ChooseFromList4.ShowDialog(FindForm());
				if (frm_ChooseFromList4.ChooseItem != null)
				{
					EditValue = (object)frm_ChooseFromList4.ChooseItem["DocNum"];
					DocEntry = (object)frm_ChooseFromList4.ChooseItem["DocEntry"];
				}
				break;
			}
			case ApzObjectTypes.oIncomingPayments:
			{
				frm_ChooseFromList frm_ChooseFromList6 = new frm_ChooseFromList(Enums.TypeCFL.IncomingPayment, multiSelect: false, "");
				frm_ChooseFromList6.ShowDialog(FindForm());
				if (frm_ChooseFromList6.ChooseItem != null)
				{
					EditValue = (object)frm_ChooseFromList6.ChooseItem["DocNum"];
					DocEntry = (object)frm_ChooseFromList6.ChooseItem["DocEntry"];
				}
				break;
			}
			case ApzObjectTypes.oBlanketAgreement:
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.BlanketAgreement, multiSelect: false, "", FindCardCode);
				frm_ChooseFromList2.ShowDialog(FindForm());
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					EditValue = (object)frm_ChooseFromList2.ChooseItem["Number"];
					DocEntry = (object)frm_ChooseFromList2.ChooseItem["AbsID"];
				}
				break;
			}
			}
		}
		else
		{
			frm_List_Document frm_List_Document3 = new frm_List_Document(apzObjectTypes, multiSelect: false, CardCode, isNoStatus: true, isDraft: true);
			frm_List_Document3.ShowDialog();
			if (frm_List_Document3.ChooseItem != null)
			{
				flag = true;
				DocumentsImpl chooseItem4 = frm_List_Document3.ChooseItem;
				EditValue = chooseItem4.DocNum;
				DocEntry = chooseItem4.DocEntry;
				this.CardCode = chooseItem4.CardCode;
				if (!string.IsNullOrEmpty(Function.ToString(cbDocNum.EditValue)))
				{
					editor.Properties.Buttons[1].Visible = true;
				}
				else
				{
					editor.Properties.Buttons[1].Visible = false;
				}
			}
		}
		if (flag && ItemSelected != null)
		{
			ItemSelected(editor, EventArgs.Empty);
		}
	}

	private void OpenDocForm(ApzObjectTypes docType)
	{
		try
		{
			dynamic val = new XtraFormEx();
			int num = Function.ParseInt(cbDocNum.EditValue);
			int num2 = Function.ParseInt(txtDocEntry.EditValue);
			switch (docType)
			{
			default:
				return;
			case ApzObjectTypes.oQuotations:
				val = new frm_SalesQuotationForBatch(num);
				break;
			case ApzObjectTypes.oOrders:
				val = new frm_SalesOrder(num);
				break;
			case ApzObjectTypes.oDeliveryNotes:
				val = new frm_Delivery(num);
				break;
			case ApzObjectTypes.oReturns:
				val = new frm_Return(num);
				break;
			case ApzObjectTypes.oInvoices:
				val = new frm_ARInvoices(num);
				break;
			case ApzObjectTypes.oCreditNotes:
				val = new frm_APCreditMemos(num);
				break;
			case ApzObjectTypes.oPurchaseRequest:
				val = new frm_PurchaseRequest(num);
				break;
			case ApzObjectTypes.oPurchaseQuotations:
				val = new frm_PurchaseQuotation(num);
				break;
			case ApzObjectTypes.oPurchaseOrders:
				val = new frm_PurchaseOder(num);
				break;
			case ApzObjectTypes.oPurchaseDeliveryNotes:
				val = new frm_GoodReceiptPO(num);
				break;
			case ApzObjectTypes.oPurchaseReturns:
				val = new frm_GoodsReturn(num);
				break;
			case ApzObjectTypes.oPurchaseInvoices:
				val = new frm_APInvoices(num);
				break;
			case ApzObjectTypes.oPurchaseCreditNotes:
				val = new frm_APCreditMemos(num);
				break;
			case ApzObjectTypes.oLandedCostsCodes:
				val = new frm_PurchaseLandedCost();
				break;
			case ApzObjectTypes.oIncomingPayments:
				val = new frm_PaymentIncoming();
				break;
			case ApzObjectTypes.oVendorPayments:
				val = new frm_PaymentOutgoing();
				break;
			case ApzObjectTypes.oInventoryTransferRequest:
				val = new frm_InventoryTranfer();
				break;
			case ApzObjectTypes.oStockTransfer:
				val = new frm_PaymentOutgoing();
				break;
			case ApzObjectTypes.oInventoryGenExit:
				val = new frm_GoodsIssue();
				break;
			case ApzObjectTypes.oInventoryGenEntry:
				val = new frm_InventoryTranferRequest(Function.ToString(num2), ApzObjectTypes.oInventoryTransferRequest);
				break;
			case ApzObjectTypes.oMaterialRevaluation:
				val = new frm_InventoryRevaluation();
				val.SearchDocEntry = num2;
				val.MODE_FIND();
				val.Search();
				break;
			case ApzObjectTypes.oProductionOrders:
				val = new frm_ProductionOrder(num);
				break;
			case ApzObjectTypes.oTrackingIssue:
				val = new frm_TrackingIssue();
				val.id = Function.ParseInt(num);
				break;
			}
			val.MdiParent = FindForm().MdiParent;
			val.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_DocType_EditValueChanged(object sender, EventArgs e)
	{
		if (!setDocType)
		{
			EditValue = null;
			DocEntry = null;
		}
	}

	private void cbDocNum_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (setDocNum && Function.ToString(cbDocNum.EditValue) == "")
			{
				DocEntry = null;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chkLinkDraft_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (checkLinkDraft || setDocType || setLinkDraft)
			{
				return;
			}
			checkLinkDraft = true;
			if (Function.ParseInt(cbDocNum.EditValue) == 0)
			{
				luk_DocType.EditValue = -1;
				EditValue = null;
				DocEntry = null;
			}
			else if ((bool)chkLinkDraft.EditValue)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveLinkDocument, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					luk_DocType.EditValue = -1;
					EditValue = null;
					DocEntry = null;
				}
				else
				{
					chkLinkDraft.EditValue = false;
				}
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveLinkDocuemntDraft, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				luk_DocType.EditValue = -1;
				EditValue = null;
				DocEntry = null;
			}
			else
			{
				chkLinkDraft.EditValue = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			checkLinkDraft = false;
		}
	}

	private void luk_DocType_Properties_PopupFilter(object sender, PopupFilterEventArgs e)
	{
		try
		{
			List<int> list = listDocType.ToList();
			if ((bool)chkLinkDraft.EditValue)
			{
				int[] array = new int[7] { 202, 4, 1250000025, 162, 1470000130, 191, 1470000145 };
				foreach (int item in array)
				{
					list.Remove(item);
				}
			}
			e.Criteria = new InOperator("ID", list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cbDocNum_Validating(object sender, CancelEventArgs e)
	{
		ButtonEdit buttonEdit = sender as ButtonEdit;
		try
		{
			isValidating = true;
			string docNum = Function.ToString(buttonEdit.EditValue);
			int num = Function.ParseInt(luk_DocType.EditValue);
			if ((num == -1 || num == 0) && !string.IsNullOrEmpty(docNum))
			{
				e.Cancel = true;
				return;
			}
			string text = Function.ToString(txtDocEntry.EditValue);
			bool flag = (bool)chkLinkDraft.EditValue;
			if (string.IsNullOrEmpty(docNum))
			{
				return;
			}
			if (!flag)
			{
				DataTable dataTable = ValidateDocNumByDocType(docNum, (ApzObjectTypes)num);
				if (dataTable.Rows.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.InvalidDocNumber, "");
					e.Cancel = true;
					ShowCFL(buttonEdit);
					return;
				}
				string editValue = Function.ToString(dataTable.Rows[0]["DocNum"]);
				if (dataTable.Rows.Count == 1)
				{
					EditValue = editValue;
					string docEntry = Function.ToString(dataTable.Rows[0]["DocEntry"]);
					DocEntry = docEntry;
					CardCode = Function.ToString(dataTable.Rows[0]["CardCode"]);
				}
				if (dataTable.Rows.Count > 1)
				{
					DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow s) => Function.ToString(s["DocNum"]).Equals(docNum));
					if (dataRow != null)
					{
						string text2 = Function.ToString(dataRow["DocEntry"]);
						if (text2 != "0" && text2 != "" && text2.Equals(text))
						{
							return;
						}
					}
					e.Cancel = true;
					ShowCFL(buttonEdit);
				}
			}
			if (!flag)
			{
				return;
			}
			DataTable dataTable2 = ValidateDocNumDraft(docNum, num);
			if (dataTable2.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InvalidDocNumber, "");
				e.Cancel = true;
				ShowCFL(buttonEdit);
				return;
			}
			string text3 = Function.ToString(dataTable2.Rows[0]["DocNum"]);
			if (dataTable2.Rows.Count == 1)
			{
				EditValue = text3;
				int num2 = Function.ParseInt(dataTable2.Rows[0]["DocEntry"]);
				DocEntry = num2;
				CardCode = Function.ToString(dataTable2.Rows[0]["CardCode"]);
			}
			if (dataTable2.Rows.Count > 1)
			{
				List<string> list = (from s in dataTable2.AsEnumerable()
					select Function.ToString(s["DocEntry"])).ToList();
				if (!text3.Equals(docNum) || !list.Contains(text))
				{
					e.Cancel = true;
					ShowCFL(buttonEdit);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			isValidating = false;
			if (AfterValidating != null)
			{
				AfterValidating(this, EventArgs.Empty);
			}
		}
	}

	private void cbDocNum_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
	{
		e.ExceptionMode = ExceptionMode.NoAction;
	}

	private string GetDocTypeFromDocEntryForDraft(int docEntry)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "Commons");
			return httpClientHelper.Get("GetDocTypeFromDocEntryForDraft", new { docEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "";
	}

	private DataTable ValidateDocNumDraft(string docNum, int docType)
	{
		DataTable result = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			result = httpClientHelper.Get("ValidateDocNumDraft", new { docNum, docType });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return result;
	}

	private DataTable ValidateDocNumByDocType(string docNum, ApzObjectTypes docType)
	{
		DataTable result = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			result = httpClientHelper.Get("ValidateDocNumByDocType", new { docNum, docType });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return result;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		this.txtDocEntry = new Apzon.Controls.Editor.TextEditEx();
		this.cbDocNum = new Apzon.Controls.Editor.ButtonEditEx();
		this.luk_DocType = new Apzon.Controls.Editor.MyLookUpEdit.LookUpEditSimple();
		this.chkLinkDraft = new Apzon.Controls.Editor.CheckEditEx();
		((System.ComponentModel.ISupportInitialize)this.txtDocEntry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbDocNum.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DocType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkLinkDraft.Properties).BeginInit();
		base.SuspendLayout();
		this.txtDocEntry.FieldName = null;
		this.txtDocEntry.Location = new System.Drawing.Point(72, 1);
		this.txtDocEntry.Name = "txtDocEntry";
		this.txtDocEntry.Size = new System.Drawing.Size(75, 20);
		this.txtDocEntry.TabIndex = 86;
		this.txtDocEntry.TabStop = false;
		this.txtDocEntry.Visible = false;
		this.cbDocNum.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.cbDocNum.FieldName = null;
		this.cbDocNum.Location = new System.Drawing.Point(0, 51);
		this.cbDocNum.Name = "cbDocNum";
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.cbDocNum.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions)
		});
		this.cbDocNum.Size = new System.Drawing.Size(147, 20);
		this.cbDocNum.TabIndex = 2;
		this.cbDocNum.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cbDocNum_ButtonPressed);
		this.cbDocNum.InvalidValue += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.cbDocNum_InvalidValue);
		this.cbDocNum.EditValueChanged += new System.EventHandler(this.cbDocNum_EditValueChanged);
		this.cbDocNum.Validating += new System.ComponentModel.CancelEventHandler(this.cbDocNum_Validating);
		this.luk_DocType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.luk_DocType.FieldName = null;
		this.luk_DocType.Location = new System.Drawing.Point(0, 23);
		this.luk_DocType.Name = "luk_DocType";
		this.luk_DocType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_DocType.Properties.Appearance.Options.UseFont = true;
		this.luk_DocType.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_DocType.Properties.AppearanceDropDown.Options.UseFont = true;
		this.luk_DocType.Properties.AutoHeight = false;
		this.luk_DocType.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_DocType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_DocType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_DocType.Properties.DisplayMember = "Name";
		this.luk_DocType.Properties.NullText = "";
		this.luk_DocType.Properties.ShowFooter = false;
		this.luk_DocType.Properties.ShowHeader = false;
		this.luk_DocType.Properties.UseDropDownRowsAsMaxCount = true;
		this.luk_DocType.Properties.ValueMember = "ID";
		this.luk_DocType.Properties.PopupFilter += new DevExpress.XtraEditors.Controls.PopupFilterEventHandler(this.luk_DocType_Properties_PopupFilter);
		this.luk_DocType.Size = new System.Drawing.Size(147, 20);
		this.luk_DocType.TabIndex = 1;
		this.luk_DocType.EditValueChanged += new System.EventHandler(this.luk_DocType_EditValueChanged);
		this.chkLinkDraft.FieldName = null;
		this.chkLinkDraft.Location = new System.Drawing.Point(-1, 1);
		this.chkLinkDraft.Name = "chkLinkDraft";
		this.chkLinkDraft.Properties.Caption = "Link Draft";
		this.chkLinkDraft.Size = new System.Drawing.Size(66, 19);
		this.chkLinkDraft.TabIndex = 0;
		this.chkLinkDraft.EditValueChanged += new System.EventHandler(this.chkLinkDraft_EditValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.txtDocEntry);
		base.Controls.Add(this.cbDocNum);
		base.Controls.Add(this.luk_DocType);
		base.Controls.Add(this.chkLinkDraft);
		base.Name = "ucLinkedDocument";
		base.Size = new System.Drawing.Size(147, 77);
		((System.ComponentModel.ISupportInitialize)this.txtDocEntry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbDocNum.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DocType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkLinkDraft.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
