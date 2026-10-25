using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.ARInvoice;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Payments;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
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
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;

namespace Apzon.Forms.Payment;

public class frm_PaymentIncoming : XtraFormEx
{
	public delegate void ShowForms(XtraFormEx frm);

	private string _privilege = "";

	public ShowForms frmShowChildrenForm;

	private int newDocEntry;

	private int firstEntry;

	private int lastEntry;

	private int currentEntry;

	private IncomingPaymentsImpl _objParent = new IncomingPaymentsImpl();

	private IncomingPaymentsImpl _objView = new IncomingPaymentsImpl();

	private IncomingPaymentInvoicesImpl _objInvoice = new IncomingPaymentInvoicesImpl();

	private IncomingPaymentCreditCardsImpl _objCredit = new IncomingPaymentCreditCardsImpl();

	private IncomingPaymentCouponsImpl _objCoupon = new IncomingPaymentCouponsImpl();

	private bool _isCash;

	private bool _isBankTransfer;

	private bool _isCreditCard;

	private bool _isCoupon;

	private bool isView;

	private List<VatGroupsImpl> lst = new List<VatGroupsImpl>();

	private List<Common.Table_Code> lstCurr = new List<Common.Table_Code>();

	private List<IncomingPaymentInvoicesImpl> lstInvoice = new List<IncomingPaymentInvoicesImpl>();

	private List<Common.Table_ID_Code> _lstUDF = new List<Common.Table_ID_Code>();

	private IContainer components;

	private LabelLine lblCode;

	private LabelLine lblName;

	private LabelLine lblAddress;

	private TextEditEx txtName;

	private MemoEditEx memoAddress;

	private RadioGroupEx radioGroup1;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private LabelLine labelLine6;

	private LabelLine labelLine7;

	private TextEditEx txtNo;

	private DateEditEx datePostingDate;

	private DateEditEx dateDueDate;

	private TextEditEx txtTransactonNo;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private MemoEditEx memoRemarks;

	private LabelLine labelLine8;

	private TextEditEx txtJournalRemark;

	private LabelLine labelLine9;

	private TextEditEx txtTotalAmountDue;

	private TextEditEx txtPaid;

	private LabelLine labelLine10;

	private LabelLine labelLine11;

	private LabelLine labelLine12;

	private LabelLine labelLine13;

	private TextEditEx txtTotalTax;

	private TextEditEx txtNetTotal;

	private SimpleButtonEx btAdd;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btListIncoming;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem toolStripMenuPMMeans;

	private ToolStripMenuItem toolStripMenuItem2;

	private ComboBoxEditEx cbDocCurrency;

	private LabelLine lblDocCurrency;

	private GridColumnEx docentry;

	private GridColumnEx customerName;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn10;

	private GridColumnEx gridColumn11;

	private GridColumnEx gridColumn12;

	private GridColumnEx gridColumn13;

	private GridColumnEx gridColumn14;

	private GridColumnEx gridColumn15;

	private GridColumnEx gridColumn16;

	private GridColumnEx gridColumn17;

	private GridColumnEx gridColumn18;

	private GridColumnEx gridColumn19;

	private GridColumnEx gridColumn20;

	private GridColumnEx gridColumn21;

	private GridColumnEx gridColumn22;

	private GridColumnEx gridColumn23;

	private GridColumnEx gridColumn24;

	private GridColumnEx gridColumn25;

	private GridViewEx gridView1;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn26;

	private GridColumnEx gridColumn27;

	private GridColumnEx gridColumn28;

	private GridColumnEx gridColumn29;

	private GridColumnEx gridColumn30;

	private GridColumnEx gridColumn31;

	private GridColumnEx gridColumn32;

	private GridColumnEx gridColumn33;

	private GridColumnEx gridColumn34;

	private GridColumnEx gridColumn35;

	private GridColumnEx gridColumn36;

	private GridColumnEx gridColumn37;

	private GridColumnEx gridColumn38;

	private GridColumnEx gridColumn39;

	private GridColumnEx gridColumn40;

	private GridColumnEx gridColumn41;

	private GridColumnEx gridColumn42;

	private GridColumnEx gridColumn43;

	private GridColumnEx gridColumn44;

	private GridColumnEx gridColumn45;

	private GridColumnEx gridColumn46;

	private GridColumnEx gcDocNum;

	private GridColumnEx gcLineId;

	private GridColumnEx gcAccount;

	private GridColumnEx gcAcctName;

	private GridColumnEx gcAmount;

	private GridColumnEx gcSumApplied;

	private GridColumnEx gcDescrip;

	private GridColumnEx gcVatGroup;

	private GridColumnEx gcVatPrcnt;

	private GridColumnEx gcObjType;

	private GridColumnEx gcVatAmnt;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private RepositoryItemComboBox repositoryItemComboBox1;

	private TextEditEx txtRate;

	private ButtonEditEx txtCode;

	private SimpleButtonEx btPaymentMeans;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btAdd.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.View:
			btAdd.Text = Resources.ButtonOK;
			break;
		}
	}

	public async Task MODE_FIND()
	{
	}

	public async Task REFRESH()
	{
		FormMode = Enums.FormViewModeType.Add;
		isView = false;
		DisableControls(isDisable: false);
		frm_PaymentIncoming_Load(null, null);
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			if (currentEntry != firstEntry)
			{
				GetFirst();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			if (currentEntry > firstEntry)
			{
				GetPrevious();
				currentEntry--;
			}
			else if (currentEntry == firstEntry)
			{
				GetLast();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			if (currentEntry < lastEntry)
			{
				GetNext();
				currentEntry++;
			}
			else if (currentEntry == lastEntry)
			{
				GetFirst();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	public async Task MOVE_LAST()
	{
		try
		{
			if (currentEntry != lastEntry)
			{
				GetLast();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	public frm_PaymentIncoming()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_PaymentIncoming"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_PaymentIncoming" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
				_privilege = "R";
			}
			else
			{
				_privilege = "F";
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void gvDoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (!isView)
		{
			setBelowTexbox();
		}
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (e.Column == gcAmount || e.Column == gcVatGroup)
		{
			int num = 0;
			double num2 = 0.0;
			try
			{
				num = Convert.ToInt32(gvDoc.GetRowCellValue(e.RowHandle, "Amount"));
			}
			catch
			{
			}
			try
			{
				num2 = getTaxRate(gvDoc.GetRowCellValue(e.RowHandle, "VatGroup").ToString());
			}
			catch
			{
			}
			gvDoc.SetRowCellValue(e.RowHandle, "VatPrcnt", num2);
			gvDoc.SetRowCellValue(e.RowHandle, "VatAmnt", (double)num * num2);
			gvDoc.SetRowCellValue(e.RowHandle, "SumApplied", (double)num * (num2 + 1.0));
			setBelowTexbox();
		}
	}

	private void txtCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				if (radioGroup1.SelectedIndex == 0)
				{
					frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Customer, multiSelect: false, null);
					frm_ChooseFromList2.ShowDialog(this);
					if (frm_ChooseFromList2.ChooseItem != null)
					{
						BusinessPartnersImpl businessPartnersImpl = (BusinessPartnersImpl)frm_ChooseFromList2.ChooseItem;
						LoadCustomerPayment(businessPartnersImpl.CardCode, "O");
						txtCode.Text = businessPartnersImpl.CardCode;
						txtName.Text = businessPartnersImpl.CardName;
						memoAddress.Text = businessPartnersImpl.Address;
						txtCode.ReadOnly = true;
						txtName.ReadOnly = true;
						memoAddress.ReadOnly = true;
					}
				}
				else if (radioGroup1.SelectedIndex == 1)
				{
					frm_ChooseFromList frm_ChooseFromList3 = new frm_ChooseFromList(Enums.TypeCFL.Vendor, multiSelect: false, null);
					frm_ChooseFromList3.ShowDialog(this);
					if (frm_ChooseFromList3.ChooseItem != null)
					{
						BusinessPartnersImpl businessPartnersImpl2 = (BusinessPartnersImpl)frm_ChooseFromList3.ChooseItem;
						LoadVendorPayment(businessPartnersImpl2.CardCode, "O");
						txtCode.Text = businessPartnersImpl2.CardCode;
						txtName.Text = businessPartnersImpl2.CardName;
						memoAddress.Text = businessPartnersImpl2.Address;
						txtCode.ReadOnly = true;
						txtName.ReadOnly = true;
						memoAddress.ReadOnly = true;
					}
				}
			}
			else
			{
				frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
				frm_BusinessMasterData2._copyF_CodeCustomer = txtCode.Text;
				frm_BusinessMasterData2.MdiParent = MdiParent;
				frm_BusinessMasterData2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	protected override void OnMove(EventArgs e)
	{
		try
		{
			base.OnMove(e);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	protected override void OnResize(EventArgs e)
	{
		try
		{
			base.OnMove(e);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	protected override void OnClosed(EventArgs e)
	{
	}

	private void frm_PaymentIncoming_Load(object sender, EventArgs e)
	{
		RefreshData(isGetNewDocEntry: true);
		GetFirstDocEntry();
		GetLastDocEntry();
		currentEntry = firstEntry;
		LoadUDF();
	}

	private void RefreshData(bool isGetNewDocEntry)
	{
		lblDocCurrency.Visible = false;
		cbDocCurrency.Visible = false;
		radioGroup1.SelectedIndex = 0;
		LoadCustomerPayment("", "");
		dateDueDate.EditValue = DateTime.Today;
		datePostingDate.EditValue = DateTime.Today;
		if (isGetNewDocEntry)
		{
			getNewDocEntry();
		}
		loadComBoTax();
		loadCurrency();
		txtCode.Text = "";
		txtName.Text = "";
		memoAddress.Text = "";
		memoRemarks.Text = "";
		txtPaid.Text = "0";
		txtNetTotal.Text = "0";
		txtTotalTax.Text = "0";
		txtTotalAmountDue.Text = "0";
		txtTransactonNo.Text = "0";
	}

	private async void LoadVendorPayment(string iCardCode, string iStatus)
	{
		try
		{
			using HttpClient client = new HttpClient();
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "OutgoingPayment", "GetAPInvoice", new { iCardCode, iStatus });
			using HttpResponseMessage response = await client.GetAsync(apiUri);
			if (!response.IsSuccessStatusCode)
			{
				return;
			}
			List<ARInvoicesImpl> lst = JsonConvert.DeserializeObject<List<ARInvoicesImpl>>(await response.Content.ReadAsStringAsync()).ToList();
			if (grdDoc.InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(() =>
				{
					grdDoc.DataSource = lst;
					grdDoc.RefreshDataSource();
				}));
			}
			else
			{
				grdDoc.DataSource = lst;
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async void LoadCustomerPayment(string iCardCode, string iStatus)
	{
		try
		{
			using HttpClient client = new HttpClient();
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "IncomingPayment", "GetARInvoice", new { iCardCode, iStatus });
			using HttpResponseMessage response = await client.GetAsync(apiUri);
			if (!response.IsSuccessStatusCode)
			{
				return;
			}
			List<ARInvoicesImpl> lst = JsonConvert.DeserializeObject<List<ARInvoicesImpl>>(await response.Content.ReadAsStringAsync()).ToList();
			if (grdDoc.InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(() =>
				{
					grdDoc.DataSource = lst;
					grdDoc.RefreshDataSource();
				}));
			}
			else
			{
				grdDoc.DataSource = lst;
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void LoadPaymentMeans(IncomingPaymentsImpl obj, bool _cash, bool _bank, bool _credit, bool _coupon)
	{
		try
		{
			if (obj != null)
			{
				_objParent = obj;
				txtPaid.Text = _objParent.DocTotal.ToString();
				_objParent.CardCode = txtCode.Text.Trim();
				_objParent.CardName = txtName.Text.Trim();
				_objParent.Address = memoAddress.Text.Trim();
				_objParent.JrnlMemo = txtJournalRemark.Text.Trim();
				_objParent.DocDate = datePostingDate.DateTime;
				_objParent.DocDueDate = dateDueDate.DateTime.ToString();
				_objParent.DocTotalSy = _objParent.DocTotal;
				_objParent.DocTotalFC = _objParent.DocTotal;
				_objParent.VatSum = decimal.Parse(txtTotalTax.Text);
				_objParent.ObjType = 24.ToString();
				try
				{
					_objParent.TransId = Convert.ToInt32(txtTransactonNo.Text.Trim());
				}
				catch
				{
					_objParent.TransId = 0;
				}
				if (radioGroup1.SelectedIndex == 2)
				{
					_objParent.DocType = "A";
				}
				else if (radioGroup1.SelectedIndex == 0)
				{
					_objParent.DocType = "C";
				}
				else
				{
					_objParent.DocType = "S";
				}
			}
			_isCash = _cash;
			_isBankTransfer = _bank;
			_isCreditCard = _credit;
			_isCoupon = _coupon;
			lstInvoice = BillDataIncomingInvoice(_objParent.Dcount, _objParent.DcntSum, 0m);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void LoadAccountPayment(string docNums)
	{
		try
		{
			using HttpClient client = new HttpClient();
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "IncomingPayment", "GetAccountPayment", new { docNums });
			using HttpResponseMessage response = await client.GetAsync(apiUri);
			if (!response.IsSuccessStatusCode)
			{
				return;
			}
			List<Payments_AccountsImpl> lst = JsonConvert.DeserializeObject<List<Payments_AccountsImpl>>(await response.Content.ReadAsStringAsync()).ToList();
			if (grdDoc.InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(() =>
				{
					grdDoc.DataSource = lst;
					grdDoc.RefreshDataSource();
				}));
			}
			else
			{
				grdDoc.DataSource = lst;
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async void getNewDocEntry()
	{
		using HttpClient client = new HttpClient();
		string apiUri = Function.GetApiUri(Configuration.ApiServer, "IncomingPayment", "GetMaxCode");
		using HttpResponseMessage response = await client.GetAsync(apiUri);
		if (response.IsSuccessStatusCode)
		{
			newDocEntry = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync()) + 1;
			txtNo.Text = newDocEntry.ToString();
		}
	}

	private void ShowHideGridColumn(bool type)
	{
		if (type)
		{
			docentry.Visible = true;
			customerName.Visible = true;
			gridColumn4.Visible = true;
			gridColumn5.Visible = true;
			gridColumn6.Visible = true;
			gridColumn7.Visible = true;
			gridColumn8.Visible = true;
			gridColumn9.Visible = true;
			gridColumn10.Visible = true;
			gridColumn11.Visible = true;
			gridColumn12.Visible = true;
			gridColumn13.Visible = true;
			gridColumn14.Visible = true;
			gridColumn15.Visible = true;
			gridColumn16.Visible = true;
			gridColumn17.Visible = true;
			gridColumn18.Visible = true;
			gridColumn19.Visible = true;
			gridColumn20.Visible = true;
			gridColumn21.Visible = true;
			gridColumn22.Visible = true;
			gridColumn23.Visible = true;
			gridColumn24.Visible = true;
			gridColumn25.Visible = true;
			docentry.VisibleIndex = 1;
			customerName.VisibleIndex = 2;
			gridColumn4.VisibleIndex = 3;
			gridColumn5.VisibleIndex = 4;
			gridColumn6.VisibleIndex = 5;
			gridColumn7.VisibleIndex = 6;
			gridColumn8.VisibleIndex = 7;
			gridColumn9.VisibleIndex = 8;
			gridColumn10.VisibleIndex = 9;
			gridColumn11.VisibleIndex = 10;
			gridColumn12.VisibleIndex = 11;
			gridColumn13.VisibleIndex = 12;
			gridColumn14.VisibleIndex = 13;
			gridColumn15.VisibleIndex = 14;
			gridColumn16.VisibleIndex = 15;
			gridColumn17.VisibleIndex = 16;
			gridColumn18.VisibleIndex = 17;
			gridColumn19.VisibleIndex = 18;
			gridColumn20.VisibleIndex = 19;
			gridColumn21.VisibleIndex = 20;
			gridColumn22.VisibleIndex = 21;
			gridColumn23.VisibleIndex = 22;
			gridColumn24.VisibleIndex = 23;
			gridColumn25.VisibleIndex = 24;
			gcAmount.Visible = false;
			gcAccount.Visible = false;
			gcAcctName.Visible = false;
			gcObjType.Visible = false;
			gcSumApplied.Visible = false;
			gcVatAmnt.Visible = false;
			gcVatGroup.Visible = false;
			gcVatPrcnt.Visible = false;
			gcDescrip.Visible = false;
		}
		else
		{
			docentry.Visible = false;
			customerName.Visible = false;
			gridColumn4.Visible = false;
			gridColumn5.Visible = false;
			gridColumn6.Visible = false;
			gridColumn7.Visible = false;
			gridColumn8.Visible = false;
			gridColumn9.Visible = false;
			gridColumn10.Visible = false;
			gridColumn11.Visible = false;
			gridColumn12.Visible = false;
			gridColumn13.Visible = false;
			gridColumn14.Visible = false;
			gridColumn15.Visible = false;
			gridColumn16.Visible = false;
			gridColumn17.Visible = false;
			gridColumn18.Visible = false;
			gridColumn19.Visible = false;
			gridColumn20.Visible = false;
			gridColumn21.Visible = false;
			gridColumn22.Visible = false;
			gridColumn23.Visible = false;
			gridColumn24.Visible = false;
			gridColumn25.Visible = false;
			gcAccount.Visible = true;
			gcAcctName.Visible = true;
			gcAmount.Visible = true;
			gcSumApplied.Visible = true;
			gcDescrip.Visible = true;
			gcVatGroup.Visible = true;
			gcObjType.Visible = true;
			gcVatPrcnt.Visible = true;
			gcVatAmnt.Visible = true;
		}
	}

	private async void loadComBoTax()
	{
		_ = 1;
		try
		{
			using HttpClient client = new HttpClient();
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "IncomingPayment", "GetVatGroupO");
			using HttpResponseMessage response = await client.GetAsync(apiUri);
			if (!response.IsSuccessStatusCode)
			{
				return;
			}
			lst = JsonConvert.DeserializeObject<List<VatGroupsImpl>>(await response.Content.ReadAsStringAsync()).ToList();
			foreach (VatGroupsImpl item in lst)
			{
				repositoryItemComboBox1.Items.Add(item.Name);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async void loadCurrency()
	{
		_ = 1;
		try
		{
			using HttpClient client = new HttpClient();
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "PaymentMeans", "ShowCurrency");
			using HttpResponseMessage response = await client.GetAsync(apiUri);
			if (!response.IsSuccessStatusCode)
			{
				return;
			}
			foreach (CurrencyCodesImpl item in JsonConvert.DeserializeObject<List<CurrencyCodesImpl>>(await response.Content.ReadAsStringAsync()).ToList())
			{
				Common.Table_Code table_Code = new Common.Table_Code();
				table_Code.Code = item.CurrCode;
				table_Code.Name = item.CurrName;
				lstCurr.Add(table_Code);
			}
			WinFuntions._addComboBox(cbDocCurrency, lstCurr, _selectFirstItem: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private double getTaxRate(string name)
	{
		foreach (VatGroupsImpl item in lst)
		{
			if (item.Name == name)
			{
				return Convert.ToDouble(item.Rate) * 0.01;
			}
		}
		return 0.0;
	}

	private string getCurrencyCode(string name)
	{
		foreach (Common.Table_Code item in lstCurr)
		{
			if (item.Name == name)
			{
				return item.Code;
			}
		}
		return "";
	}

	private void setBelowTexbox()
	{
		try
		{
			decimal num = 0m;
			decimal num2 = 0m;
			decimal num3 = 0m;
			int[] selectedRows = gvDoc.GetSelectedRows();
			foreach (int rowHandle in selectedRows)
			{
				if (radioGroup1.SelectedIndex != 2)
				{
					num += (decimal)Convert.ToInt32(gvDoc.GetRowCellValue(rowHandle, "DocTotal"));
					num2 += (decimal)Convert.ToInt32(gvDoc.GetRowCellValue(rowHandle, "VatSum"));
				}
				else
				{
					num += (decimal)Convert.ToInt32(gvDoc.GetRowCellValue(rowHandle, "Amount"));
					num2 += (decimal)Convert.ToInt32(gvDoc.GetRowCellValue(rowHandle, "VatAmnt"));
				}
			}
			num3 = num - num2;
			txtNetTotal.Text = num3.ToString();
			txtTotalTax.Text = num2.ToString();
			txtTotalAmountDue.Text = num.ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btListIncoming_Click(object sender, EventArgs e)
	{
		frm_PaymentIncomingList frm_PaymentIncomingList2 = new frm_PaymentIncomingList();
		frm_PaymentIncomingList2.MaximizeBox = false;
		frm_PaymentIncomingList2.FormBorderStyle = FormBorderStyle.FixedSingle;
		frm_PaymentIncomingList2.reloadParent = LoadForm;
		frm_PaymentIncomingList2.ShowDialog(this);
	}

	private void LoadForm(string docEntry)
	{
		RefreshData(isGetNewDocEntry: false);
		DisableControls(isDisable: true);
		isView = true;
		GetIncomingPayment(docEntry);
	}

	private async void GetIncomingPayment(string docEntry)
	{
		_ = 4;
		try
		{
			using HttpClient client = new HttpClient();
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "IncomingPayment", "GetIncomingPayment", new { docEntry });
			using HttpResponseMessage response = await client.GetAsync(apiUri);
			if (!response.IsSuccessStatusCode)
			{
				return;
			}
			DataTable dt = JsonConvert.DeserializeObject<DataTable>(await response.Content.ReadAsStringAsync());
			if (dt.Rows.Count <= 0)
			{
				return;
			}
			txtNo.Text = docEntry;
			if (dt.Rows[0]["DocType"].ToString() == "A")
			{
				radioGroup1.SelectedIndex = 2;
				await GetPurposeIncomingPayment(docEntry);
				ShowHideGridColumn(type: false);
			}
			else
			{
				if (dt.Rows[0]["CardType"].ToString() == "C")
				{
					radioGroup1.SelectedIndex = 0;
					await GetARInvoiceByIncomingPayment(docEntry);
				}
				else
				{
					radioGroup1.SelectedIndex = 1;
					await GetAPInvoiceByIncomingPayment(docEntry);
				}
				ShowHideGridColumn(type: true);
			}
			txtCode.Text = dt.Rows[0]["CardCode"].ToString();
			txtName.Text = dt.Rows[0]["CardName"].ToString();
			dateDueDate.Text = dt.Rows[0]["DocDueDate"].ToString();
			datePostingDate.Text = dt.Rows[0]["DocDate"].ToString();
			txtTransactonNo.Text = dt.Rows[0]["TransId"].ToString();
			memoAddress.Text = dt.Rows[0]["Address"].ToString();
			txtPaid.Text = dt.Rows[0]["DocTotal"].ToString();
			try
			{
				memoRemarks.Text = dt.Rows[0]["Free_Text"].ToString();
			}
			catch
			{
			}
			txtJournalRemark.Text = dt.Rows[0]["JrnlMemo"].ToString();
			txtTotalTax.Text = dt.Rows[0]["VatSum"].ToString();
			_objView.CashSum = Function.ParseDecimal(dt.Rows[0]["CashSum"].ToString());
			_objView.TrsfrSum = Function.ParseDecimal(dt.Rows[0]["TrsfrSum"].ToString());
			_objView.CreditSum = Function.ParseDecimal(dt.Rows[0]["CreditSum"].ToString());
			_objView.CouponSum = Function.ParseDecimal(dt.Rows[0]["CouponSum"].ToString());
			_objView.DocEntry = Function.ParseInt(docEntry);
			_objView.ObjType = dt.Rows[0]["ObjType"].ToString();
			_objView.DocTotal = Function.ParseDecimal(dt.Rows[0]["DocTotal"].ToString());
			_objView.TrsfrAcct = dt.Rows[0]["TrsfrAcct"].ToString();
			_objView.TrsfrRef = dt.Rows[0]["TrsfrRef"].ToString();
			_objView.CashAcct = dt.Rows[0]["CashAcct"].ToString();
			_objView.TrsfrDate = Convert.ToDateTime(dt.Rows[0]["TrsfrDate"].ToString());
			LoadUDFData(docEntry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async Task GetAPInvoiceByIncomingPayment(string docEntry)
	{
		try
		{
			using HttpClient client = new HttpClient();
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "IncomingPayment", "GetAPInvoiceByIncomingPayment", new { docEntry });
			using HttpResponseMessage response = await client.GetAsync(apiUri);
			if (!response.IsSuccessStatusCode)
			{
				return;
			}
			List<ARInvoicesImpl> lst = JsonConvert.DeserializeObject<List<ARInvoicesImpl>>(await response.Content.ReadAsStringAsync()).ToList();
			decimal num = 0m;
			foreach (ARInvoicesImpl item in lst)
			{
				num += item.DocTotal;
			}
			txtTotalAmountDue.EditValue = num;
			if (grdDoc.InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(() =>
				{
					grdDoc.DataSource = lst;
					grdDoc.RefreshDataSource();
				}));
			}
			else
			{
				grdDoc.DataSource = lst;
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async Task GetARInvoiceByIncomingPayment(string docEntry)
	{
		try
		{
			using HttpClient client = new HttpClient();
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "IncomingPayment", "GetARInvoiceByIncomingPayment", new { docEntry });
			using HttpResponseMessage response = await client.GetAsync(apiUri);
			if (!response.IsSuccessStatusCode)
			{
				return;
			}
			List<ARInvoicesImpl> lst = JsonConvert.DeserializeObject<List<ARInvoicesImpl>>(await response.Content.ReadAsStringAsync()).ToList();
			decimal num = 0m;
			foreach (ARInvoicesImpl item in lst)
			{
				num += item.DocTotal;
			}
			txtTotalAmountDue.EditValue = num;
			if (grdDoc.InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(() =>
				{
					grdDoc.DataSource = lst;
					grdDoc.RefreshDataSource();
				}));
			}
			else
			{
				grdDoc.DataSource = lst;
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async Task GetPurposeIncomingPayment(string docEntry)
	{
		try
		{
			using HttpClient client = new HttpClient();
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "IncomingPayment", "GetPurposeIncomingPayment", new { docEntry });
			using HttpResponseMessage response = await client.GetAsync(apiUri);
			if (!response.IsSuccessStatusCode)
			{
				return;
			}
			List<Payments_AccountsImpl> lst = JsonConvert.DeserializeObject<List<Payments_AccountsImpl>>(await response.Content.ReadAsStringAsync()).ToList();
			if (grdDoc.InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(() =>
				{
					grdDoc.DataSource = lst;
					grdDoc.RefreshDataSource();
				}));
			}
			else
			{
				grdDoc.DataSource = lst;
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void LoadUDF()
	{
	}

	private async void LoadUDFData(string docEntry)
	{
		_ = 1;
		try
		{
			using HttpClient client = new HttpClient();
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "IncomingPayment", "LoadUDFData", new { docEntry });
			using HttpResponseMessage response = await client.GetAsync(apiUri);
			if (response.IsSuccessStatusCode)
			{
				DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(await response.Content.ReadAsStringAsync());
				List<Common.Table_Code> list = new List<Common.Table_Code>();
				for (int i = 0; i < dataTable.Columns.Count; i++)
				{
					Common.Table_Code table_Code = new Common.Table_Code();
					table_Code.Code = dataTable.Columns[i].ColumnName;
					list.Add(table_Code);
				}
				for (int j = 0; j < list.Count; j++)
				{
					list[j].Name = dataTable.Rows[0][j].ToString();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void DisableControls(bool isDisable)
	{
		if (isDisable)
		{
			radioGroup1.Enabled = false;
			cbDocCurrency.Enabled = false;
			dateDueDate.ReadOnly = true;
			datePostingDate.ReadOnly = true;
			memoAddress.ReadOnly = true;
			txtJournalRemark.ReadOnly = true;
			memoRemarks.ReadOnly = true;
			txtTransactonNo.ReadOnly = true;
			gcAmount.OptionsColumn.ReadOnly = true;
			gcDescrip.OptionsColumn.ReadOnly = true;
			gcVatGroup.OptionsColumn.ReadOnly = true;
			txtCode.ReadOnly = true;
			txtName.ReadOnly = true;
			txtNo.ReadOnly = true;
		}
		else
		{
			radioGroup1.Enabled = true;
			cbDocCurrency.Enabled = true;
			dateDueDate.ReadOnly = false;
			datePostingDate.ReadOnly = false;
			memoAddress.ReadOnly = false;
			txtJournalRemark.ReadOnly = false;
			memoRemarks.ReadOnly = false;
			txtTransactonNo.ReadOnly = false;
			gcAmount.OptionsColumn.ReadOnly = false;
			gcDescrip.OptionsColumn.ReadOnly = false;
			gcVatGroup.OptionsColumn.ReadOnly = false;
			txtCode.ReadOnly = false;
			txtName.ReadOnly = false;
			txtNo.ReadOnly = false;
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btAdd_Click(object sender, EventArgs e)
	{
		if (!isView)
		{
			AddData();
			return;
		}
		RefreshData(isGetNewDocEntry: true);
		DisableControls(isDisable: false);
		isView = false;
	}

	private void toolStripMenuPMMeans_Click(object sender, EventArgs e)
	{
		try
		{
			if (!isView)
			{
				frm_PaymentMeans frm_PaymentMeans2 = new frm_PaymentMeans((!string.IsNullOrEmpty(txtTotalAmountDue.Text)) ? Convert.ToInt32(txtTotalAmountDue.Text) : 0, newDocEntry, txtCode.Text.Trim(), txtName.Text.Trim(), memoAddress.Text.Trim(), 1m, getCurrencyCode(cbDocCurrency.SelectedItem.ToString()));
				frm_PaymentMeans2.MaximizeBox = false;
				frm_PaymentMeans2.passObject = LoadPaymentMeans;
				frm_PaymentMeans2.ShowDialog(this);
			}
			else
			{
				frm_PaymentMeans frm_PaymentMeans3 = new frm_PaymentMeans(_objView);
				frm_PaymentMeans3.isView = true;
				frm_PaymentMeans3.MaximizeBox = false;
				frm_PaymentMeans3.passObject = null;
				frm_PaymentMeans3.ShowDialog(this);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void toolStripMenuItem2_Click(object sender, EventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ChartOfAccountPurpose, multiSelect: true, null);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItems == null || frm_ChooseFromList2.ChooseItems.Count <= 0)
			{
				return;
			}
			string text = "";
			foreach (dynamic chooseItem in frm_ChooseFromList2.ChooseItems)
			{
				Payments_AccountsImpl payments_AccountsImpl = (Payments_AccountsImpl)chooseItem;
				text = text + payments_AccountsImpl.AcctCode + ",";
			}
			text = text.Remove(text.Length - 1);
			LoadAccountPayment(text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
	{
		gvDoc.OptionsBehavior.Editable = true;
		RadioGroup radioGroup = sender as RadioGroup;
		if (radioGroup.SelectedIndex == 0 || radioGroup.SelectedIndex == 1)
		{
			lblAddress.Visible = true;
			lblCode.Visible = true;
			lblName.Visible = true;
			txtCode.Visible = true;
			txtName.Visible = true;
			memoAddress.Visible = true;
			lblDocCurrency.Visible = false;
			cbDocCurrency.Visible = false;
			contextMenuStrip1.Items[1].Enabled = false;
			grdDoc.DataSource = null;
			grdDoc.RefreshDataSource();
			ShowHideGridColumn(type: true);
			txtRate.Visible = false;
		}
		else
		{
			lblAddress.Visible = false;
			lblCode.Visible = false;
			lblName.Visible = false;
			txtCode.Visible = false;
			txtName.Visible = false;
			memoAddress.Visible = false;
			lblDocCurrency.Visible = true;
			cbDocCurrency.Visible = true;
			contextMenuStrip1.Items[1].Enabled = true;
			grdDoc.DataSource = null;
			grdDoc.RefreshDataSource();
			ShowHideGridColumn(type: false);
		}
		txtTotalTax.Text = "0";
		txtPaid.Text = "0";
		txtTotalAmountDue.Text = "0";
		txtNetTotal.Text = "0";
		txtCode.Text = "";
		memoAddress.Text = "";
	}

	private void btPaymentMeans_Click(object sender, EventArgs e)
	{
		toolStripMenuPMMeans_Click(null, null);
	}

	private async void AddData()
	{
		_ = 1;
		try
		{
			if (gvDoc.GetSelectedRows().Length == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoRow2Paid, "");
				return;
			}
			int num = 0;
			try
			{
				num = Convert.ToInt32(txtPaid.Text);
			}
			catch
			{
			}
			if (num > 0)
			{
				if (!_isCash && !_isBankTransfer && !_isCreditCard && !_isCoupon)
				{
					return;
				}
				_objParent._isCustomerARInvoice = false;
				_objParent._isPurpose = false;
				_objParent._isVendorAPInvoice = false;
				if (radioGroup1.SelectedIndex != 2)
				{
					_objParent._listInvoice = lstInvoice;
					if (radioGroup1.SelectedIndex == 0)
					{
						_objParent._isCustomerARInvoice = true;
					}
					else
					{
						_objParent._isVendorAPInvoice = true;
					}
				}
				else
				{
					_objParent._isPurpose = true;
					List<Payments_AccountsImpl> list = new List<Payments_AccountsImpl>();
					for (int i = 0; i < gvDoc.DataRowCount; i++)
					{
						Payments_AccountsImpl payments_AccountsImpl = new Payments_AccountsImpl();
						payments_AccountsImpl.DocNum = int.Parse(txtNo.Text);
						payments_AccountsImpl.LineId = i;
						payments_AccountsImpl.AcctCode = gvDoc.GetRowCellValue(i, "AcctCode").ToString();
						payments_AccountsImpl.AcctName = gvDoc.GetRowCellValue(i, "AcctName").ToString();
						payments_AccountsImpl.Amount = decimal.Parse(gvDoc.GetRowCellValue(i, "Amount").ToString());
						payments_AccountsImpl.SumApplied = double.Parse(gvDoc.GetRowCellValue(i, "SumApplied").ToString());
						payments_AccountsImpl.Descrip = gvDoc.GetRowCellValue(i, "Descrip").ToString();
						payments_AccountsImpl.VatGroup = gvDoc.GetRowCellValue(i, "VatGroup").ToString();
						payments_AccountsImpl.VatPrcnt = decimal.Parse(gvDoc.GetRowCellValue(i, "VatPrcnt").ToString());
						payments_AccountsImpl.ObjType = "24";
						payments_AccountsImpl.VatAmnt = decimal.Parse(gvDoc.GetRowCellValue(i, "VatAmnt").ToString());
						list.Add(payments_AccountsImpl);
					}
					_objParent._listPurpose = list;
				}
				await Add(_objParent);
				await SaveToUDF();
				RefreshData(isGetNewDocEntry: true);
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.InvoiceMustPaidOrInvoiceHaveZeroPrice, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Add(IncomingPaymentsImpl doc)
	{
		try
		{
			HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
			new MultipartFormDataContent();
			httpRequestMessage.Content = new ObjectContent<IncomingPaymentsImpl>(doc, new JsonMediaTypeFormatter());
			httpRequestMessage.Method = HttpMethod.Post;
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "IncomingPayment", "SaveDoc");
			httpRequestMessage.RequestUri = new Uri(apiUri);
			await new HttpClient().SendAsync(httpRequestMessage).ContinueWith((Task<HttpResponseMessage> task) =>
			{
				if (task.Result.IsSuccessStatusCode)
				{
					Task<string> task2 = task.Result.Content.ReadAsStringAsync();
					if (task2 != null && string.IsNullOrEmpty(task2.Result))
					{
						return task2.Result;
					}
					WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				}
				return (string)null;
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private List<IncomingPaymentInvoicesImpl> BillDataIncomingInvoice(decimal dcount, decimal dcntSum, decimal bfDcntSum)
	{
		try
		{
			int num = 1;
			List<IncomingPaymentInvoicesImpl> list = new List<IncomingPaymentInvoicesImpl>();
			int[] selectedRows = gvDoc.GetSelectedRows();
			foreach (int rowHandle in selectedRows)
			{
				IncomingPaymentInvoicesImpl incomingPaymentInvoicesImpl = new IncomingPaymentInvoicesImpl();
				int docEntry = Convert.ToInt32(gvDoc.GetRowCellValue(rowHandle, "DocEntry"));
				incomingPaymentInvoicesImpl.DocNum = _objParent.DocNum;
				incomingPaymentInvoicesImpl.InvoiceId = num++;
				incomingPaymentInvoicesImpl.DocEntry = docEntry;
				incomingPaymentInvoicesImpl.SumApplied = decimal.Parse(txtTotalAmountDue.Text);
				incomingPaymentInvoicesImpl.InvType = 13m;
				incomingPaymentInvoicesImpl.DocRate = 1m;
				incomingPaymentInvoicesImpl.DocLine = 0;
				incomingPaymentInvoicesImpl.vatApplied = 0m;
				incomingPaymentInvoicesImpl.ObjType = "24";
				incomingPaymentInvoicesImpl.Dcount = dcount;
				incomingPaymentInvoicesImpl.DcntSum = dcntSum;
				incomingPaymentInvoicesImpl.BfDcntSum = bfDcntSum;
				incomingPaymentInvoicesImpl.BfNetDcnt = 0m;
				list.Add(incomingPaymentInvoicesImpl);
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new List<IncomingPaymentInvoicesImpl>();
		}
	}

	private async Task SaveToUDF()
	{
		try
		{
			Common.Table_ID_Code table_ID_Code = new Common.Table_ID_Code();
			table_ID_Code.ID = txtNo.Text;
			_lstUDF.Add(table_ID_Code);
			await SaveUDF(_lstUDF);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async Task<string> SaveUDF(List<Common.Table_ID_Code> lstUDF)
	{
		try
		{
			HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
			new MultipartFormDataContent();
			httpRequestMessage.Content = new ObjectContent<List<Common.Table_ID_Code>>(lstUDF, new JsonMediaTypeFormatter());
			httpRequestMessage.Method = HttpMethod.Post;
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "IncomingPayment", "SaveUDF");
			httpRequestMessage.RequestUri = new Uri(apiUri);
			await new HttpClient().SendAsync(httpRequestMessage).ContinueWith((Task<HttpResponseMessage> task) =>
			{
				if (task.Result.IsSuccessStatusCode)
				{
					Task<string> task2 = task.Result.Content.ReadAsStringAsync();
					if (task2 != null && string.IsNullOrEmpty(task2.Result))
					{
						return task2.Result;
					}
					WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				}
				return (string)null;
			});
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
			return null;
		}
	}

	private async void GetFirst()
	{
		using HttpClient client = new HttpClient();
		string apiUri = Function.GetApiUri(Configuration.ApiServer, "IncomingPayment", "GetFirst");
		using HttpResponseMessage response = await client.GetAsync(apiUri);
		if (response.IsSuccessStatusCode)
		{
			DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(await response.Content.ReadAsStringAsync());
			if (dataTable.Rows.Count > 0)
			{
				LoadForm(dataTable.Rows[0]["DocEntry"].ToString());
				firstEntry = int.Parse(dataTable.Rows[0]["DocEntry"].ToString());
				currentEntry = firstEntry;
			}
		}
	}

	private async void GetPrevious()
	{
		using HttpClient client = new HttpClient();
		string apiUri = Function.GetApiUri(Configuration.ApiServer, "IncomingPayment", "GetPrevious", new
		{
			docEntry = txtNo.Text
		});
		using HttpResponseMessage response = await client.GetAsync(apiUri);
		if (response.IsSuccessStatusCode)
		{
			DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(await response.Content.ReadAsStringAsync());
			if (dataTable.Rows.Count > 0)
			{
				LoadForm(dataTable.Rows[0]["DocEntry"].ToString());
			}
		}
	}

	private async void GetNext()
	{
		using HttpClient client = new HttpClient();
		string apiUri = Function.GetApiUri(Configuration.ApiServer, "IncomingPayment", "GetNext", new
		{
			docEntry = txtNo.Text
		});
		using HttpResponseMessage response = await client.GetAsync(apiUri);
		if (response.IsSuccessStatusCode)
		{
			DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(await response.Content.ReadAsStringAsync());
			if (dataTable.Rows.Count > 0)
			{
				LoadForm(dataTable.Rows[0]["DocEntry"].ToString());
			}
		}
	}

	private async void GetLast()
	{
		using HttpClient client = new HttpClient();
		string apiUri = Function.GetApiUri(Configuration.ApiServer, "IncomingPayment", "GetLast");
		using HttpResponseMessage response = await client.GetAsync(apiUri);
		if (response.IsSuccessStatusCode)
		{
			DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(await response.Content.ReadAsStringAsync());
			if (dataTable.Rows.Count > 0)
			{
				LoadForm(dataTable.Rows[0]["DocEntry"].ToString());
				lastEntry = int.Parse(dataTable.Rows[0]["DocEntry"].ToString());
				currentEntry = lastEntry;
			}
		}
	}

	private async void GetFirstDocEntry()
	{
		using HttpClient client = new HttpClient();
		string apiUri = Function.GetApiUri(Configuration.ApiServer, "IncomingPayment", "GetFirst");
		using HttpResponseMessage response = await client.GetAsync(apiUri);
		if (response.IsSuccessStatusCode)
		{
			DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(await response.Content.ReadAsStringAsync());
			if (dataTable.Rows.Count > 0)
			{
				firstEntry = int.Parse(dataTable.Rows[0]["DocEntry"].ToString());
			}
		}
	}

	private async void GetLastDocEntry()
	{
		using HttpClient client = new HttpClient();
		string apiUri = Function.GetApiUri(Configuration.ApiServer, "IncomingPayment", "GetLast");
		using HttpResponseMessage response = await client.GetAsync(apiUri);
		if (response.IsSuccessStatusCode)
		{
			DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(await response.Content.ReadAsStringAsync());
			if (dataTable.Rows.Count > 0)
			{
				lastEntry = int.Parse(dataTable.Rows[0]["DocEntry"].ToString());
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Payment.frm_PaymentIncoming));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.lblCode = new Apzon.Controls.Controls.LabelLine();
		this.lblName = new Apzon.Controls.Controls.LabelLine();
		this.lblAddress = new Apzon.Controls.Controls.LabelLine();
		this.txtName = new Apzon.Controls.Editor.TextEditEx();
		this.memoAddress = new Apzon.Controls.Editor.MemoEditEx();
		this.radioGroup1 = new Apzon.Controls.Editor.RadioGroupEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.txtNo = new Apzon.Controls.Editor.TextEditEx();
		this.datePostingDate = new Apzon.Controls.Editor.DateEditEx();
		this.dateDueDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtTransactonNo = new Apzon.Controls.Editor.TextEditEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuPMMeans = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.docentry = new Apzon.Controls.Grid.GridColumnEx();
		this.customerName = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn24 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineId = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAccount = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAcctName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAmount = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcSumApplied = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDescrip = new Apzon.Controls.Grid.GridColumnEx();
		this.gcVatGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.gcVatPrcnt = new Apzon.Controls.Grid.GridColumnEx();
		this.gcObjType = new Apzon.Controls.Grid.GridColumnEx();
		this.gcVatAmnt = new Apzon.Controls.Grid.GridColumnEx();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn26 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn27 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn28 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn29 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn30 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn31 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn32 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn33 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn34 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn35 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn36 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn37 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn38 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn39 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn40 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn41 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn42 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn43 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn44 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn45 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn46 = new Apzon.Controls.Grid.GridColumnEx();
		this.memoRemarks = new Apzon.Controls.Editor.MemoEditEx();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.txtJournalRemark = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.txtTotalAmountDue = new Apzon.Controls.Editor.TextEditEx();
		this.txtPaid = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine12 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine13 = new Apzon.Controls.Controls.LabelLine();
		this.txtTotalTax = new Apzon.Controls.Editor.TextEditEx();
		this.txtNetTotal = new Apzon.Controls.Editor.TextEditEx();
		this.btAdd = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btListIncoming = new Apzon.Controls.Controls.SimpleButtonEx();
		this.cbDocCurrency = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.lblDocCurrency = new Apzon.Controls.Controls.LabelLine();
		this.txtRate = new Apzon.Controls.Editor.TextEditEx();
		this.txtCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.btPaymentMeans = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.txtName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memoAddress.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.radioGroup1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.datePostingDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.datePostingDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateDueDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateDueDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtTransactonNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memoRemarks.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtJournalRemark.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtTotalAmountDue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPaid.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtTotalTax.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtNetTotal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbDocCurrency.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtRate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).BeginInit();
		base.SuspendLayout();
		this.lblCode.EditControl = null;
		this.lblCode.EditValue = "Code";
		this.lblCode.FieldName = null;
		this.lblCode.HideWhenEmpty = false;
		this.lblCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblCode.Location = new System.Drawing.Point(6, 12);
		this.lblCode.Name = "lblCode";
		this.lblCode.ReadOnly = false;
		this.lblCode.SettingInVisible = false;
		this.lblCode.SettingReadonly = false;
		this.lblCode.ShowUnderLine = true;
		this.lblCode.Size = new System.Drawing.Size(80, 20);
		this.lblCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblCode.TabIndex = 0;
		this.lblCode.Utext = "Code";
		this.lblName.EditControl = null;
		this.lblName.EditValue = "Name";
		this.lblName.FieldName = null;
		this.lblName.HideWhenEmpty = false;
		this.lblName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblName.Location = new System.Drawing.Point(6, 35);
		this.lblName.Name = "lblName";
		this.lblName.ReadOnly = false;
		this.lblName.SettingInVisible = false;
		this.lblName.SettingReadonly = false;
		this.lblName.ShowUnderLine = true;
		this.lblName.Size = new System.Drawing.Size(80, 20);
		this.lblName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblName.TabIndex = 1;
		this.lblName.Utext = "Name";
		this.lblAddress.EditControl = null;
		this.lblAddress.EditValue = "Address";
		this.lblAddress.FieldName = null;
		this.lblAddress.HideWhenEmpty = false;
		this.lblAddress.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblAddress.Location = new System.Drawing.Point(6, 60);
		this.lblAddress.Name = "lblAddress";
		this.lblAddress.ReadOnly = false;
		this.lblAddress.SettingInVisible = false;
		this.lblAddress.SettingReadonly = false;
		this.lblAddress.ShowUnderLine = true;
		this.lblAddress.Size = new System.Drawing.Size(80, 20);
		this.lblAddress.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblAddress.TabIndex = 2;
		this.lblAddress.Utext = "Address";
		this.txtName.EditControl = null;
		this.txtName.FieldName = null;
		this.txtName.IsSearchControl = false;
		this.txtName.Location = new System.Drawing.Point(85, 32);
		this.txtName.Name = "txtName";
		this.txtName.SettingInVisible = false;
		this.txtName.SettingReadonly = false;
		this.txtName.Size = new System.Drawing.Size(165, 20);
		this.txtName.TabIndex = 4;
		this.memoAddress.EditControl = null;
		this.memoAddress.FieldName = null;
		this.memoAddress.IsSearchControl = false;
		this.memoAddress.Location = new System.Drawing.Point(85, 55);
		this.memoAddress.Name = "memoAddress";
		this.memoAddress.SettingInVisible = false;
		this.memoAddress.SettingReadonly = false;
		this.memoAddress.Size = new System.Drawing.Size(165, 38);
		this.memoAddress.TabIndex = 5;
		this.radioGroup1.EditControl = null;
		this.radioGroup1.FieldName = null;
		this.radioGroup1.IsSearchControl = false;
		this.radioGroup1.Location = new System.Drawing.Point(265, 0);
		this.radioGroup1.Name = "radioGroup1";
		this.radioGroup1.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.radioGroup1.Properties.Appearance.Options.UseBackColor = true;
		this.radioGroup1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.radioGroup1.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[3]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("<Null>", "Customer"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Vendor"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Account")
		});
		this.radioGroup1.SettingInVisible = false;
		this.radioGroup1.SettingReadonly = false;
		this.radioGroup1.Size = new System.Drawing.Size(74, 96);
		this.radioGroup1.TabIndex = 6;
		this.radioGroup1.SelectedIndexChanged += new System.EventHandler(this.radioGroup1_SelectedIndexChanged);
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Due Date";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(349, 56);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(134, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 9;
		this.labelLine4.Utext = "Due Date";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Posting Date";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(349, 33);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(134, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 8;
		this.labelLine5.Utext = "Posting Date";
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "No.";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(349, 11);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(134, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 7;
		this.labelLine6.Utext = "No.";
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Transaction No";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(349, 80);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(134, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 10;
		this.labelLine7.Utext = "Transaction No";
		this.txtNo.EditControl = null;
		this.txtNo.FieldName = null;
		this.txtNo.IsSearchControl = false;
		this.txtNo.Location = new System.Drawing.Point(481, 7);
		this.txtNo.Name = "txtNo";
		this.txtNo.SettingInVisible = false;
		this.txtNo.SettingReadonly = false;
		this.txtNo.Size = new System.Drawing.Size(165, 20);
		this.txtNo.TabIndex = 11;
		this.datePostingDate.EditControl = null;
		this.datePostingDate.EditValue = null;
		this.datePostingDate.FieldName = null;
		this.datePostingDate.Location = new System.Drawing.Point(481, 30);
		this.datePostingDate.Name = "datePostingDate";
		this.datePostingDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.datePostingDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.datePostingDate.Properties.IsSearchControl = false;
		this.datePostingDate.SettingInVisible = false;
		this.datePostingDate.SettingReadonly = false;
		this.datePostingDate.Size = new System.Drawing.Size(165, 20);
		this.datePostingDate.TabIndex = 12;
		this.dateDueDate.EditControl = null;
		this.dateDueDate.EditValue = null;
		this.dateDueDate.FieldName = null;
		this.dateDueDate.Location = new System.Drawing.Point(481, 53);
		this.dateDueDate.Name = "dateDueDate";
		this.dateDueDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateDueDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateDueDate.Properties.IsSearchControl = false;
		this.dateDueDate.SettingInVisible = false;
		this.dateDueDate.SettingReadonly = false;
		this.dateDueDate.Size = new System.Drawing.Size(165, 20);
		this.dateDueDate.TabIndex = 13;
		this.txtTransactonNo.EditControl = null;
		this.txtTransactonNo.FieldName = null;
		this.txtTransactonNo.IsSearchControl = false;
		this.txtTransactonNo.Location = new System.Drawing.Point(481, 76);
		this.txtTransactonNo.Name = "txtTransactonNo";
		this.txtTransactonNo.SettingInVisible = false;
		this.txtTransactonNo.SettingReadonly = false;
		this.txtTransactonNo.Size = new System.Drawing.Size(165, 20);
		this.txtTransactonNo.TabIndex = 14;
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Location = new System.Drawing.Point(4, 122);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemCalcEdit1, this.repositoryItemComboBox1 });
		this.grdDoc.Size = new System.Drawing.Size(642, 211);
		this.grdDoc.TabIndex = 15;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gvDoc, this.gridView1 });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.toolStripMenuPMMeans, this.toolStripMenuItem2 });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(168, 48);
		this.toolStripMenuPMMeans.Name = "toolStripMenuPMMeans";
		this.toolStripMenuPMMeans.Size = new System.Drawing.Size(167, 22);
		this.toolStripMenuPMMeans.Text = "Payment means";
		this.toolStripMenuPMMeans.Click += new System.EventHandler(this.toolStripMenuPMMeans_Click);
		this.toolStripMenuItem2.Name = "toolStripMenuItem2";
		this.toolStripMenuItem2.Size = new System.Drawing.Size(167, 22);
		this.toolStripMenuItem2.Text = "Payment Purpose";
		this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[35]
		{
			this.docentry, this.customerName, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8, this.gridColumn9, this.gridColumn10, this.gridColumn11,
			this.gridColumn12, this.gridColumn13, this.gridColumn14, this.gridColumn15, this.gridColumn16, this.gridColumn17, this.gridColumn18, this.gridColumn19, this.gridColumn20, this.gridColumn21,
			this.gridColumn22, this.gridColumn23, this.gridColumn24, this.gridColumn25, this.gcDocNum, this.gcLineId, this.gcAccount, this.gcAcctName, this.gcAmount, this.gcSumApplied,
			this.gcDescrip, this.gcVatGroup, this.gcVatPrcnt, this.gcObjType, this.gcVatAmnt
		});
		this.gvDoc.CustomizationFormBounds = new System.Drawing.Rectangle(491, 433, 210, 172);
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = null;
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = null;
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsSelection.CheckBoxSelectorColumnWidth = 50;
		this.gvDoc.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gvDoc.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.gvDoc.OptionsSelection.EnableAppearanceHideSelection = false;
		this.gvDoc.OptionsSelection.MultiSelect = true;
		this.gvDoc.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gvDoc.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.True;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowDetailButtons = false;
		this.gvDoc.OptionsView.ShowFooter = true;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gvDoc_SelectionChanged);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.docentry.Caption = "Internal Number";
		this.docentry.CurrencyDateField = null;
		this.docentry.CurrencyField = null;
		this.docentry.CurrencyFixed = null;
		this.docentry.CurrencyFixedRate = 0.0;
		this.docentry.CurrencyRateField = null;
		this.docentry.FieldName = "DocEntry";
		this.docentry.MinWidth = 80;
		this.docentry.Name = "docentry";
		this.docentry.OptionsColumn.AllowEdit = false;
		this.docentry.OptionsColumn.FixedWidth = true;
		this.docentry.OptionsColumn.ReadOnly = true;
		this.docentry.PrimaryKey = false;
		this.docentry.UnitField = null;
		this.docentry.Visible = true;
		this.docentry.VisibleIndex = 1;
		this.docentry.Width = 99;
		this.customerName.Caption = "Customer/Vendor Name";
		this.customerName.CurrencyDateField = null;
		this.customerName.CurrencyField = null;
		this.customerName.CurrencyFixed = null;
		this.customerName.CurrencyFixedRate = 0.0;
		this.customerName.CurrencyRateField = null;
		this.customerName.FieldName = "CardName";
		this.customerName.MinWidth = 114;
		this.customerName.Name = "customerName";
		this.customerName.OptionsColumn.AllowEdit = false;
		this.customerName.OptionsColumn.FixedWidth = true;
		this.customerName.OptionsColumn.ReadOnly = true;
		this.customerName.PrimaryKey = false;
		this.customerName.UnitField = null;
		this.customerName.Visible = true;
		this.customerName.VisibleIndex = 2;
		this.customerName.Width = 141;
		this.gridColumn4.Caption = "BP Reference No.";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "NumAtCard";
		this.gridColumn4.MinWidth = 114;
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.AllowEdit = false;
		this.gridColumn4.OptionsColumn.FixedWidth = true;
		this.gridColumn4.OptionsColumn.ReadOnly = true;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 3;
		this.gridColumn5.Caption = "Total Tax";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.DisplayFormat.FormatString = "n0";
		this.gridColumn5.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn5.FieldName = "VatSum";
		this.gridColumn5.MinWidth = 114;
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsColumn.AllowEdit = false;
		this.gridColumn5.OptionsColumn.FixedWidth = true;
		this.gridColumn5.OptionsColumn.ReadOnly = true;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "VatSum", "{0:0.##}")
		});
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 4;
		this.gridColumn6.Caption = "Total Tax (FC)";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.DisplayFormat.FormatString = "n0";
		this.gridColumn6.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn6.FieldName = "VatSumFC";
		this.gridColumn6.MinWidth = 114;
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.OptionsColumn.AllowEdit = false;
		this.gridColumn6.OptionsColumn.FixedWidth = true;
		this.gridColumn6.OptionsColumn.ReadOnly = true;
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "VatSumFC", "{0:0.##}")
		});
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 5;
		this.gridColumn7.Caption = "Disc. % for Doc.";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.DisplayFormat.FormatString = "n0";
		this.gridColumn7.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn7.FieldName = "DiscPrcnt";
		this.gridColumn7.MinWidth = 114;
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.OptionsColumn.AllowEdit = false;
		this.gridColumn7.OptionsColumn.FixedWidth = true;
		this.gridColumn7.OptionsColumn.ReadOnly = true;
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 6;
		this.gridColumn8.Caption = "Total Disc.";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.DisplayFormat.FormatString = "n0";
		this.gridColumn8.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn8.FieldName = "DiscSum";
		this.gridColumn8.MinWidth = 114;
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.OptionsColumn.AllowEdit = false;
		this.gridColumn8.OptionsColumn.FixedWidth = true;
		this.gridColumn8.OptionsColumn.ReadOnly = true;
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "DiscSum", "{0:0.##}")
		});
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 7;
		this.gridColumn9.Caption = "Currency";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "DocCur";
		this.gridColumn9.MinWidth = 114;
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.OptionsColumn.AllowEdit = false;
		this.gridColumn9.OptionsColumn.FixedWidth = true;
		this.gridColumn9.OptionsColumn.ReadOnly = true;
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 8;
		this.gridColumn10.Caption = "Rate";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.DisplayFormat.FormatString = "n0";
		this.gridColumn10.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn10.FieldName = "DocRate";
		this.gridColumn10.MinWidth = 114;
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.OptionsColumn.AllowEdit = false;
		this.gridColumn10.OptionsColumn.FixedWidth = true;
		this.gridColumn10.OptionsColumn.ReadOnly = true;
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 9;
		this.gridColumn11.Caption = "Total";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.DisplayFormat.FormatString = "n2";
		this.gridColumn11.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn11.FieldName = "DocTotal";
		this.gridColumn11.MinWidth = 114;
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.OptionsColumn.AllowEdit = false;
		this.gridColumn11.OptionsColumn.FixedWidth = true;
		this.gridColumn11.OptionsColumn.ReadOnly = true;
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "DocTotal", "{0:0.##}")
		});
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 10;
		this.gridColumn12.Caption = "Total (FC)";
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.DisplayFormat.FormatString = "n2";
		this.gridColumn12.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn12.FieldName = "DocTotalFC";
		this.gridColumn12.MinWidth = 114;
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.OptionsColumn.AllowEdit = false;
		this.gridColumn12.OptionsColumn.FixedWidth = true;
		this.gridColumn12.OptionsColumn.ReadOnly = true;
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "DocTotalFC", "{0:0.##}")
		});
		this.gridColumn12.UnitField = null;
		this.gridColumn12.Visible = true;
		this.gridColumn12.VisibleIndex = 11;
		this.gridColumn13.Caption = "Paid to Date";
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.DisplayFormat.FormatString = "n0";
		this.gridColumn13.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn13.FieldName = "PaidToDate";
		this.gridColumn13.MinWidth = 114;
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.OptionsColumn.AllowEdit = false;
		this.gridColumn13.OptionsColumn.FixedWidth = true;
		this.gridColumn13.OptionsColumn.ReadOnly = true;
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn13.Visible = true;
		this.gridColumn13.VisibleIndex = 12;
		this.gridColumn14.Caption = "Paid (FC)";
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.DisplayFormat.FormatString = "n0";
		this.gridColumn14.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn14.FieldName = "PaidFC";
		this.gridColumn14.MinWidth = 114;
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.OptionsColumn.AllowEdit = false;
		this.gridColumn14.OptionsColumn.FixedWidth = true;
		this.gridColumn14.OptionsColumn.ReadOnly = true;
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "PaidFC", "{0:0.##}")
		});
		this.gridColumn14.UnitField = null;
		this.gridColumn14.Visible = true;
		this.gridColumn14.VisibleIndex = 13;
		this.gridColumn15.Caption = "Gross Profit";
		this.gridColumn15.CurrencyDateField = null;
		this.gridColumn15.CurrencyField = null;
		this.gridColumn15.CurrencyFixed = null;
		this.gridColumn15.CurrencyFixedRate = 0.0;
		this.gridColumn15.CurrencyRateField = null;
		this.gridColumn15.DisplayFormat.FormatString = "n0";
		this.gridColumn15.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn15.FieldName = "GrosProfit";
		this.gridColumn15.MinWidth = 114;
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.OptionsColumn.AllowEdit = false;
		this.gridColumn15.OptionsColumn.FixedWidth = true;
		this.gridColumn15.OptionsColumn.ReadOnly = true;
		this.gridColumn15.PrimaryKey = false;
		this.gridColumn15.UnitField = null;
		this.gridColumn15.Visible = true;
		this.gridColumn15.VisibleIndex = 14;
		this.gridColumn16.Caption = "Gross Profit (FC)";
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.DisplayFormat.FormatString = "n0";
		this.gridColumn16.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn16.FieldName = "GrosProfFC";
		this.gridColumn16.MinWidth = 114;
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.OptionsColumn.AllowEdit = false;
		this.gridColumn16.OptionsColumn.FixedWidth = true;
		this.gridColumn16.OptionsColumn.ReadOnly = true;
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.UnitField = null;
		this.gridColumn16.Visible = true;
		this.gridColumn16.VisibleIndex = 15;
		this.gridColumn17.Caption = "Reference 1";
		this.gridColumn17.CurrencyDateField = null;
		this.gridColumn17.CurrencyField = null;
		this.gridColumn17.CurrencyFixed = null;
		this.gridColumn17.CurrencyFixedRate = 0.0;
		this.gridColumn17.CurrencyRateField = null;
		this.gridColumn17.FieldName = "Ref1";
		this.gridColumn17.MinWidth = 114;
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.OptionsColumn.AllowEdit = false;
		this.gridColumn17.OptionsColumn.FixedWidth = true;
		this.gridColumn17.OptionsColumn.ReadOnly = true;
		this.gridColumn17.PrimaryKey = false;
		this.gridColumn17.UnitField = null;
		this.gridColumn17.Visible = true;
		this.gridColumn17.VisibleIndex = 16;
		this.gridColumn18.Caption = "Receipt Number";
		this.gridColumn18.CurrencyDateField = null;
		this.gridColumn18.CurrencyField = null;
		this.gridColumn18.CurrencyFixed = null;
		this.gridColumn18.CurrencyFixedRate = 0.0;
		this.gridColumn18.CurrencyRateField = null;
		this.gridColumn18.FieldName = "ReceiptNum";
		this.gridColumn18.MinWidth = 114;
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.OptionsColumn.AllowEdit = false;
		this.gridColumn18.OptionsColumn.FixedWidth = true;
		this.gridColumn18.OptionsColumn.ReadOnly = true;
		this.gridColumn18.PrimaryKey = false;
		this.gridColumn18.UnitField = null;
		this.gridColumn18.Visible = true;
		this.gridColumn18.VisibleIndex = 17;
		this.gridColumn19.Caption = "Generation Time";
		this.gridColumn19.CurrencyDateField = null;
		this.gridColumn19.CurrencyField = null;
		this.gridColumn19.CurrencyFixed = null;
		this.gridColumn19.CurrencyFixedRate = 0.0;
		this.gridColumn19.CurrencyRateField = null;
		this.gridColumn19.FieldName = "DocTime";
		this.gridColumn19.MinWidth = 114;
		this.gridColumn19.Name = "gridColumn19";
		this.gridColumn19.OptionsColumn.AllowEdit = false;
		this.gridColumn19.OptionsColumn.FixedWidth = true;
		this.gridColumn19.OptionsColumn.ReadOnly = true;
		this.gridColumn19.PrimaryKey = false;
		this.gridColumn19.UnitField = null;
		this.gridColumn19.Visible = true;
		this.gridColumn19.VisibleIndex = 18;
		this.gridColumn20.Caption = "TypeDisc";
		this.gridColumn20.CurrencyDateField = null;
		this.gridColumn20.CurrencyField = null;
		this.gridColumn20.CurrencyFixed = null;
		this.gridColumn20.CurrencyFixedRate = 0.0;
		this.gridColumn20.CurrencyRateField = null;
		this.gridColumn20.FieldName = "TypeDisc";
		this.gridColumn20.MinWidth = 114;
		this.gridColumn20.Name = "gridColumn20";
		this.gridColumn20.OptionsColumn.AllowEdit = false;
		this.gridColumn20.OptionsColumn.FixedWidth = true;
		this.gridColumn20.OptionsColumn.ReadOnly = true;
		this.gridColumn20.PrimaryKey = false;
		this.gridColumn20.UnitField = null;
		this.gridColumn20.Visible = true;
		this.gridColumn20.VisibleIndex = 19;
		this.gridColumn21.Caption = "ShipToCode";
		this.gridColumn21.CurrencyDateField = null;
		this.gridColumn21.CurrencyField = null;
		this.gridColumn21.CurrencyFixed = null;
		this.gridColumn21.CurrencyFixedRate = 0.0;
		this.gridColumn21.CurrencyRateField = null;
		this.gridColumn21.FieldName = "ShipToCode";
		this.gridColumn21.MinWidth = 114;
		this.gridColumn21.Name = "gridColumn21";
		this.gridColumn21.OptionsColumn.AllowEdit = false;
		this.gridColumn21.OptionsColumn.FixedWidth = true;
		this.gridColumn21.OptionsColumn.ReadOnly = true;
		this.gridColumn21.PrimaryKey = false;
		this.gridColumn21.UnitField = null;
		this.gridColumn21.Visible = true;
		this.gridColumn21.VisibleIndex = 20;
		this.gridColumn22.Caption = "PaytoCode";
		this.gridColumn22.CurrencyDateField = null;
		this.gridColumn22.CurrencyField = null;
		this.gridColumn22.CurrencyFixed = null;
		this.gridColumn22.CurrencyFixedRate = 0.0;
		this.gridColumn22.CurrencyRateField = null;
		this.gridColumn22.FieldName = "PaytoCode";
		this.gridColumn22.MinWidth = 114;
		this.gridColumn22.Name = "gridColumn22";
		this.gridColumn22.OptionsColumn.AllowEdit = false;
		this.gridColumn22.OptionsColumn.FixedWidth = true;
		this.gridColumn22.OptionsColumn.ReadOnly = true;
		this.gridColumn22.PrimaryKey = false;
		this.gridColumn22.UnitField = null;
		this.gridColumn22.Visible = true;
		this.gridColumn22.VisibleIndex = 21;
		this.gridColumn23.Caption = "Confirmed";
		this.gridColumn23.CurrencyDateField = null;
		this.gridColumn23.CurrencyField = null;
		this.gridColumn23.CurrencyFixed = null;
		this.gridColumn23.CurrencyFixedRate = 0.0;
		this.gridColumn23.CurrencyRateField = null;
		this.gridColumn23.FieldName = "Confirmed";
		this.gridColumn23.MinWidth = 114;
		this.gridColumn23.Name = "gridColumn23";
		this.gridColumn23.OptionsColumn.AllowEdit = false;
		this.gridColumn23.OptionsColumn.FixedWidth = true;
		this.gridColumn23.OptionsColumn.ReadOnly = true;
		this.gridColumn23.PrimaryKey = false;
		this.gridColumn23.UnitField = null;
		this.gridColumn23.Visible = true;
		this.gridColumn23.VisibleIndex = 22;
		this.gridColumn24.Caption = "Rounding";
		this.gridColumn24.CurrencyDateField = null;
		this.gridColumn24.CurrencyField = null;
		this.gridColumn24.CurrencyFixed = null;
		this.gridColumn24.CurrencyFixedRate = 0.0;
		this.gridColumn24.CurrencyRateField = null;
		this.gridColumn24.FieldName = "Rounding";
		this.gridColumn24.MinWidth = 114;
		this.gridColumn24.Name = "gridColumn24";
		this.gridColumn24.OptionsColumn.AllowEdit = false;
		this.gridColumn24.OptionsColumn.FixedWidth = true;
		this.gridColumn24.OptionsColumn.ReadOnly = true;
		this.gridColumn24.PrimaryKey = false;
		this.gridColumn24.UnitField = null;
		this.gridColumn24.Visible = true;
		this.gridColumn24.VisibleIndex = 23;
		this.gridColumn25.Caption = "Address2";
		this.gridColumn25.CurrencyDateField = null;
		this.gridColumn25.CurrencyField = null;
		this.gridColumn25.CurrencyFixed = null;
		this.gridColumn25.CurrencyFixedRate = 0.0;
		this.gridColumn25.CurrencyRateField = null;
		this.gridColumn25.FieldName = "Address2";
		this.gridColumn25.MinWidth = 114;
		this.gridColumn25.Name = "gridColumn25";
		this.gridColumn25.OptionsColumn.AllowEdit = false;
		this.gridColumn25.OptionsColumn.FixedWidth = true;
		this.gridColumn25.OptionsColumn.ReadOnly = true;
		this.gridColumn25.PrimaryKey = false;
		this.gridColumn25.UnitField = null;
		this.gridColumn25.Visible = true;
		this.gridColumn25.VisibleIndex = 33;
		this.gcDocNum.Caption = "DocNum";
		this.gcDocNum.CurrencyDateField = null;
		this.gcDocNum.CurrencyField = null;
		this.gcDocNum.CurrencyFixed = null;
		this.gcDocNum.CurrencyFixedRate = 0.0;
		this.gcDocNum.CurrencyRateField = null;
		this.gcDocNum.FieldName = "DocNum";
		this.gcDocNum.Name = "gcDocNum";
		this.gcDocNum.PrimaryKey = false;
		this.gcDocNum.UnitField = null;
		this.gcLineId.Caption = "LineId";
		this.gcLineId.CurrencyDateField = null;
		this.gcLineId.CurrencyField = null;
		this.gcLineId.CurrencyFixed = null;
		this.gcLineId.CurrencyFixedRate = 0.0;
		this.gcLineId.CurrencyRateField = null;
		this.gcLineId.FieldName = "LineId";
		this.gcLineId.Name = "gcLineId";
		this.gcLineId.PrimaryKey = false;
		this.gcLineId.UnitField = null;
		this.gcAccount.Caption = "Account";
		this.gcAccount.CurrencyDateField = null;
		this.gcAccount.CurrencyField = null;
		this.gcAccount.CurrencyFixed = null;
		this.gcAccount.CurrencyFixedRate = 0.0;
		this.gcAccount.CurrencyRateField = null;
		this.gcAccount.FieldName = "AcctCode";
		this.gcAccount.MinWidth = 60;
		this.gcAccount.Name = "gcAccount";
		this.gcAccount.OptionsColumn.ReadOnly = true;
		this.gcAccount.PrimaryKey = false;
		this.gcAccount.UnitField = null;
		this.gcAccount.Visible = true;
		this.gcAccount.VisibleIndex = 24;
		this.gcAccount.Width = 60;
		this.gcAcctName.Caption = "Account Name";
		this.gcAcctName.CurrencyDateField = null;
		this.gcAcctName.CurrencyField = null;
		this.gcAcctName.CurrencyFixed = null;
		this.gcAcctName.CurrencyFixedRate = 0.0;
		this.gcAcctName.CurrencyRateField = null;
		this.gcAcctName.FieldName = "AcctName";
		this.gcAcctName.MinWidth = 114;
		this.gcAcctName.Name = "gcAcctName";
		this.gcAcctName.OptionsColumn.AllowEdit = false;
		this.gcAcctName.OptionsColumn.ReadOnly = true;
		this.gcAcctName.PrimaryKey = false;
		this.gcAcctName.UnitField = null;
		this.gcAcctName.Visible = true;
		this.gcAcctName.VisibleIndex = 25;
		this.gcAcctName.Width = 114;
		this.gcAmount.Caption = "Total Amount";
		this.gcAmount.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gcAmount.CurrencyDateField = null;
		this.gcAmount.CurrencyField = null;
		this.gcAmount.CurrencyFixed = null;
		this.gcAmount.CurrencyFixedRate = 0.0;
		this.gcAmount.CurrencyRateField = null;
		this.gcAmount.DisplayFormat.FormatString = "n0";
		this.gcAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcAmount.FieldName = "Amount";
		this.gcAmount.MinWidth = 114;
		this.gcAmount.Name = "gcAmount";
		this.gcAmount.PrimaryKey = false;
		this.gcAmount.UnitField = null;
		this.gcAmount.Visible = true;
		this.gcAmount.VisibleIndex = 26;
		this.gcAmount.Width = 114;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.gcSumApplied.Caption = "Paid";
		this.gcSumApplied.CurrencyDateField = null;
		this.gcSumApplied.CurrencyField = null;
		this.gcSumApplied.CurrencyFixed = null;
		this.gcSumApplied.CurrencyFixedRate = 0.0;
		this.gcSumApplied.CurrencyRateField = null;
		this.gcSumApplied.DisplayFormat.FormatString = "n0";
		this.gcSumApplied.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcSumApplied.FieldName = "SumApplied";
		this.gcSumApplied.MinWidth = 114;
		this.gcSumApplied.Name = "gcSumApplied";
		this.gcSumApplied.OptionsColumn.AllowEdit = false;
		this.gcSumApplied.OptionsColumn.ReadOnly = true;
		this.gcSumApplied.PrimaryKey = false;
		this.gcSumApplied.UnitField = null;
		this.gcSumApplied.Visible = true;
		this.gcSumApplied.VisibleIndex = 27;
		this.gcSumApplied.Width = 114;
		this.gcDescrip.Caption = "Detail";
		this.gcDescrip.CurrencyDateField = null;
		this.gcDescrip.CurrencyField = null;
		this.gcDescrip.CurrencyFixed = null;
		this.gcDescrip.CurrencyFixedRate = 0.0;
		this.gcDescrip.CurrencyRateField = null;
		this.gcDescrip.FieldName = "Descrip";
		this.gcDescrip.MinWidth = 114;
		this.gcDescrip.Name = "gcDescrip";
		this.gcDescrip.OptionsColumn.AllowEdit = false;
		this.gcDescrip.PrimaryKey = false;
		this.gcDescrip.UnitField = null;
		this.gcDescrip.Visible = true;
		this.gcDescrip.VisibleIndex = 28;
		this.gcDescrip.Width = 114;
		this.gcVatGroup.Caption = "Tax Definition";
		this.gcVatGroup.ColumnEdit = this.repositoryItemComboBox1;
		this.gcVatGroup.CurrencyDateField = null;
		this.gcVatGroup.CurrencyField = null;
		this.gcVatGroup.CurrencyFixed = null;
		this.gcVatGroup.CurrencyFixedRate = 0.0;
		this.gcVatGroup.CurrencyRateField = null;
		this.gcVatGroup.FieldName = "VatGroup";
		this.gcVatGroup.MinWidth = 114;
		this.gcVatGroup.Name = "gcVatGroup";
		this.gcVatGroup.PrimaryKey = false;
		this.gcVatGroup.UnitField = null;
		this.gcVatGroup.Visible = true;
		this.gcVatGroup.VisibleIndex = 29;
		this.gcVatGroup.Width = 114;
		this.repositoryItemComboBox1.AutoHeight = false;
		this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
		this.gcVatPrcnt.Caption = "Tax Rate";
		this.gcVatPrcnt.CurrencyDateField = null;
		this.gcVatPrcnt.CurrencyField = null;
		this.gcVatPrcnt.CurrencyFixed = null;
		this.gcVatPrcnt.CurrencyFixedRate = 0.0;
		this.gcVatPrcnt.CurrencyRateField = null;
		this.gcVatPrcnt.FieldName = "VatPrcnt";
		this.gcVatPrcnt.MinWidth = 114;
		this.gcVatPrcnt.Name = "gcVatPrcnt";
		this.gcVatPrcnt.OptionsColumn.AllowEdit = false;
		this.gcVatPrcnt.OptionsColumn.ReadOnly = true;
		this.gcVatPrcnt.PrimaryKey = false;
		this.gcVatPrcnt.UnitField = null;
		this.gcVatPrcnt.Visible = true;
		this.gcVatPrcnt.VisibleIndex = 30;
		this.gcVatPrcnt.Width = 114;
		this.gcObjType.Caption = "Object Type";
		this.gcObjType.CurrencyDateField = null;
		this.gcObjType.CurrencyField = null;
		this.gcObjType.CurrencyFixed = null;
		this.gcObjType.CurrencyFixedRate = 0.0;
		this.gcObjType.CurrencyRateField = null;
		this.gcObjType.FieldName = "ObjType";
		this.gcObjType.MinWidth = 50;
		this.gcObjType.Name = "gcObjType";
		this.gcObjType.OptionsColumn.AllowEdit = false;
		this.gcObjType.OptionsColumn.ReadOnly = true;
		this.gcObjType.PrimaryKey = false;
		this.gcObjType.UnitField = null;
		this.gcObjType.Visible = true;
		this.gcObjType.VisibleIndex = 31;
		this.gcObjType.Width = 50;
		this.gcVatAmnt.Caption = "VAT Amount";
		this.gcVatAmnt.CurrencyDateField = null;
		this.gcVatAmnt.CurrencyField = null;
		this.gcVatAmnt.CurrencyFixed = null;
		this.gcVatAmnt.CurrencyFixedRate = 0.0;
		this.gcVatAmnt.CurrencyRateField = null;
		this.gcVatAmnt.DisplayFormat.FormatString = "n0";
		this.gcVatAmnt.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcVatAmnt.FieldName = "VatAmnt";
		this.gcVatAmnt.MinWidth = 114;
		this.gcVatAmnt.Name = "gcVatAmnt";
		this.gcVatAmnt.OptionsColumn.AllowEdit = false;
		this.gcVatAmnt.OptionsColumn.ReadOnly = true;
		this.gcVatAmnt.PrimaryKey = false;
		this.gcVatAmnt.UnitField = null;
		this.gcVatAmnt.Visible = true;
		this.gcVatAmnt.VisibleIndex = 32;
		this.gcVatAmnt.Width = 114;
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[24]
		{
			this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn26, this.gridColumn27, this.gridColumn28, this.gridColumn29, this.gridColumn30, this.gridColumn31, this.gridColumn32,
			this.gridColumn33, this.gridColumn34, this.gridColumn35, this.gridColumn36, this.gridColumn37, this.gridColumn38, this.gridColumn39, this.gridColumn40, this.gridColumn41, this.gridColumn42,
			this.gridColumn43, this.gridColumn44, this.gridColumn45, this.gridColumn46
		});
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = null;
		this.gridView1.GridControl = this.grdDoc;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 35;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = null;
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.OptionsSelection.CheckBoxSelectorColumnWidth = 20;
		this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gridView1.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.gridView1.OptionsSelection.EnableAppearanceHideSelection = false;
		this.gridView1.OptionsSelection.MultiSelect = true;
		this.gridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gridView1.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.True;
		this.gridView1.OptionsView.ColumnAutoWidth = false;
		this.gridView1.OptionsView.ShowGroupPanel = false;
		this.gridView1.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
		this.gridView1.UseBatchMod = false;
		this.gridColumn1.Caption = "Internal Number";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "DocEntry";
		this.gridColumn1.MinWidth = 80;
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.OptionsColumn.FixedWidth = true;
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 1;
		this.gridColumn1.Width = 99;
		this.gridColumn2.Caption = "Customer/Vendor Name";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "CardName";
		this.gridColumn2.MinWidth = 114;
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.AllowEdit = false;
		this.gridColumn2.OptionsColumn.FixedWidth = true;
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 2;
		this.gridColumn2.Width = 141;
		this.gridColumn3.Caption = "BP Reference No.";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "NumAtCard";
		this.gridColumn3.MinWidth = 114;
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.AllowEdit = false;
		this.gridColumn3.OptionsColumn.FixedWidth = true;
		this.gridColumn3.OptionsColumn.ReadOnly = true;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 3;
		this.gridColumn3.Width = 114;
		this.gridColumn26.Caption = "Total Tax";
		this.gridColumn26.CurrencyDateField = null;
		this.gridColumn26.CurrencyField = null;
		this.gridColumn26.CurrencyFixed = null;
		this.gridColumn26.CurrencyFixedRate = 0.0;
		this.gridColumn26.CurrencyRateField = null;
		this.gridColumn26.FieldName = "VatSum";
		this.gridColumn26.MinWidth = 114;
		this.gridColumn26.Name = "gridColumn26";
		this.gridColumn26.OptionsColumn.AllowEdit = false;
		this.gridColumn26.OptionsColumn.FixedWidth = true;
		this.gridColumn26.OptionsColumn.ReadOnly = true;
		this.gridColumn26.PrimaryKey = false;
		this.gridColumn26.UnitField = null;
		this.gridColumn26.Visible = true;
		this.gridColumn26.VisibleIndex = 4;
		this.gridColumn26.Width = 114;
		this.gridColumn27.Caption = "Total Tax (FC)";
		this.gridColumn27.CurrencyDateField = null;
		this.gridColumn27.CurrencyField = null;
		this.gridColumn27.CurrencyFixed = null;
		this.gridColumn27.CurrencyFixedRate = 0.0;
		this.gridColumn27.CurrencyRateField = null;
		this.gridColumn27.FieldName = "VatSumFC";
		this.gridColumn27.MinWidth = 114;
		this.gridColumn27.Name = "gridColumn27";
		this.gridColumn27.OptionsColumn.AllowEdit = false;
		this.gridColumn27.OptionsColumn.FixedWidth = true;
		this.gridColumn27.OptionsColumn.ReadOnly = true;
		this.gridColumn27.PrimaryKey = false;
		this.gridColumn27.UnitField = null;
		this.gridColumn27.Visible = true;
		this.gridColumn27.VisibleIndex = 5;
		this.gridColumn27.Width = 114;
		this.gridColumn28.Caption = "Disc. % for Doc.";
		this.gridColumn28.CurrencyDateField = null;
		this.gridColumn28.CurrencyField = null;
		this.gridColumn28.CurrencyFixed = null;
		this.gridColumn28.CurrencyFixedRate = 0.0;
		this.gridColumn28.CurrencyRateField = null;
		this.gridColumn28.FieldName = "DiscPrcnt";
		this.gridColumn28.MinWidth = 114;
		this.gridColumn28.Name = "gridColumn28";
		this.gridColumn28.OptionsColumn.AllowEdit = false;
		this.gridColumn28.OptionsColumn.FixedWidth = true;
		this.gridColumn28.OptionsColumn.ReadOnly = true;
		this.gridColumn28.PrimaryKey = false;
		this.gridColumn28.UnitField = null;
		this.gridColumn28.Visible = true;
		this.gridColumn28.VisibleIndex = 6;
		this.gridColumn28.Width = 114;
		this.gridColumn29.Caption = "Total Disc.";
		this.gridColumn29.CurrencyDateField = null;
		this.gridColumn29.CurrencyField = null;
		this.gridColumn29.CurrencyFixed = null;
		this.gridColumn29.CurrencyFixedRate = 0.0;
		this.gridColumn29.CurrencyRateField = null;
		this.gridColumn29.FieldName = "DiscSum";
		this.gridColumn29.MinWidth = 114;
		this.gridColumn29.Name = "gridColumn29";
		this.gridColumn29.OptionsColumn.AllowEdit = false;
		this.gridColumn29.OptionsColumn.FixedWidth = true;
		this.gridColumn29.OptionsColumn.ReadOnly = true;
		this.gridColumn29.PrimaryKey = false;
		this.gridColumn29.UnitField = null;
		this.gridColumn29.Visible = true;
		this.gridColumn29.VisibleIndex = 7;
		this.gridColumn29.Width = 114;
		this.gridColumn30.Caption = "Currency";
		this.gridColumn30.CurrencyDateField = null;
		this.gridColumn30.CurrencyField = null;
		this.gridColumn30.CurrencyFixed = null;
		this.gridColumn30.CurrencyFixedRate = 0.0;
		this.gridColumn30.CurrencyRateField = null;
		this.gridColumn30.FieldName = "DocCur";
		this.gridColumn30.MinWidth = 114;
		this.gridColumn30.Name = "gridColumn30";
		this.gridColumn30.OptionsColumn.AllowEdit = false;
		this.gridColumn30.OptionsColumn.FixedWidth = true;
		this.gridColumn30.OptionsColumn.ReadOnly = true;
		this.gridColumn30.PrimaryKey = false;
		this.gridColumn30.UnitField = null;
		this.gridColumn30.Visible = true;
		this.gridColumn30.VisibleIndex = 8;
		this.gridColumn30.Width = 114;
		this.gridColumn31.Caption = "Rate";
		this.gridColumn31.CurrencyDateField = null;
		this.gridColumn31.CurrencyField = null;
		this.gridColumn31.CurrencyFixed = null;
		this.gridColumn31.CurrencyFixedRate = 0.0;
		this.gridColumn31.CurrencyRateField = null;
		this.gridColumn31.FieldName = "DocRate";
		this.gridColumn31.MinWidth = 114;
		this.gridColumn31.Name = "gridColumn31";
		this.gridColumn31.OptionsColumn.AllowEdit = false;
		this.gridColumn31.OptionsColumn.FixedWidth = true;
		this.gridColumn31.OptionsColumn.ReadOnly = true;
		this.gridColumn31.PrimaryKey = false;
		this.gridColumn31.UnitField = null;
		this.gridColumn31.Visible = true;
		this.gridColumn31.VisibleIndex = 9;
		this.gridColumn31.Width = 114;
		this.gridColumn32.Caption = "Total";
		this.gridColumn32.CurrencyDateField = null;
		this.gridColumn32.CurrencyField = null;
		this.gridColumn32.CurrencyFixed = null;
		this.gridColumn32.CurrencyFixedRate = 0.0;
		this.gridColumn32.CurrencyRateField = null;
		this.gridColumn32.FieldName = "DocTotal";
		this.gridColumn32.MinWidth = 114;
		this.gridColumn32.Name = "gridColumn32";
		this.gridColumn32.OptionsColumn.AllowEdit = false;
		this.gridColumn32.OptionsColumn.FixedWidth = true;
		this.gridColumn32.OptionsColumn.ReadOnly = true;
		this.gridColumn32.PrimaryKey = false;
		this.gridColumn32.UnitField = null;
		this.gridColumn32.Visible = true;
		this.gridColumn32.VisibleIndex = 10;
		this.gridColumn32.Width = 114;
		this.gridColumn33.Caption = "Total (FC)";
		this.gridColumn33.CurrencyDateField = null;
		this.gridColumn33.CurrencyField = null;
		this.gridColumn33.CurrencyFixed = null;
		this.gridColumn33.CurrencyFixedRate = 0.0;
		this.gridColumn33.CurrencyRateField = null;
		this.gridColumn33.FieldName = "DocTotalFC";
		this.gridColumn33.MinWidth = 114;
		this.gridColumn33.Name = "gridColumn33";
		this.gridColumn33.OptionsColumn.AllowEdit = false;
		this.gridColumn33.OptionsColumn.FixedWidth = true;
		this.gridColumn33.OptionsColumn.ReadOnly = true;
		this.gridColumn33.PrimaryKey = false;
		this.gridColumn33.UnitField = null;
		this.gridColumn33.Visible = true;
		this.gridColumn33.VisibleIndex = 11;
		this.gridColumn33.Width = 114;
		this.gridColumn34.Caption = "Paid to Date";
		this.gridColumn34.CurrencyDateField = null;
		this.gridColumn34.CurrencyField = null;
		this.gridColumn34.CurrencyFixed = null;
		this.gridColumn34.CurrencyFixedRate = 0.0;
		this.gridColumn34.CurrencyRateField = null;
		this.gridColumn34.FieldName = "PaidToDate";
		this.gridColumn34.MinWidth = 114;
		this.gridColumn34.Name = "gridColumn34";
		this.gridColumn34.OptionsColumn.AllowEdit = false;
		this.gridColumn34.OptionsColumn.FixedWidth = true;
		this.gridColumn34.OptionsColumn.ReadOnly = true;
		this.gridColumn34.PrimaryKey = false;
		this.gridColumn34.UnitField = null;
		this.gridColumn34.Visible = true;
		this.gridColumn34.VisibleIndex = 12;
		this.gridColumn34.Width = 114;
		this.gridColumn35.Caption = "Paid (FC)";
		this.gridColumn35.CurrencyDateField = null;
		this.gridColumn35.CurrencyField = null;
		this.gridColumn35.CurrencyFixed = null;
		this.gridColumn35.CurrencyFixedRate = 0.0;
		this.gridColumn35.CurrencyRateField = null;
		this.gridColumn35.FieldName = "PaidFC";
		this.gridColumn35.MinWidth = 114;
		this.gridColumn35.Name = "gridColumn35";
		this.gridColumn35.OptionsColumn.AllowEdit = false;
		this.gridColumn35.OptionsColumn.FixedWidth = true;
		this.gridColumn35.OptionsColumn.ReadOnly = true;
		this.gridColumn35.PrimaryKey = false;
		this.gridColumn35.UnitField = null;
		this.gridColumn35.Visible = true;
		this.gridColumn35.VisibleIndex = 13;
		this.gridColumn35.Width = 114;
		this.gridColumn36.Caption = "Gross Profit";
		this.gridColumn36.CurrencyDateField = null;
		this.gridColumn36.CurrencyField = null;
		this.gridColumn36.CurrencyFixed = null;
		this.gridColumn36.CurrencyFixedRate = 0.0;
		this.gridColumn36.CurrencyRateField = null;
		this.gridColumn36.FieldName = "GrosProfit";
		this.gridColumn36.MinWidth = 114;
		this.gridColumn36.Name = "gridColumn36";
		this.gridColumn36.OptionsColumn.AllowEdit = false;
		this.gridColumn36.OptionsColumn.FixedWidth = true;
		this.gridColumn36.OptionsColumn.ReadOnly = true;
		this.gridColumn36.PrimaryKey = false;
		this.gridColumn36.UnitField = null;
		this.gridColumn36.Visible = true;
		this.gridColumn36.VisibleIndex = 14;
		this.gridColumn36.Width = 114;
		this.gridColumn37.Caption = "Gross Profit (FC)";
		this.gridColumn37.CurrencyDateField = null;
		this.gridColumn37.CurrencyField = null;
		this.gridColumn37.CurrencyFixed = null;
		this.gridColumn37.CurrencyFixedRate = 0.0;
		this.gridColumn37.CurrencyRateField = null;
		this.gridColumn37.FieldName = "GrosProfFC";
		this.gridColumn37.MinWidth = 114;
		this.gridColumn37.Name = "gridColumn37";
		this.gridColumn37.OptionsColumn.AllowEdit = false;
		this.gridColumn37.OptionsColumn.FixedWidth = true;
		this.gridColumn37.OptionsColumn.ReadOnly = true;
		this.gridColumn37.PrimaryKey = false;
		this.gridColumn37.UnitField = null;
		this.gridColumn37.Visible = true;
		this.gridColumn37.VisibleIndex = 15;
		this.gridColumn37.Width = 114;
		this.gridColumn38.Caption = "Reference 1";
		this.gridColumn38.CurrencyDateField = null;
		this.gridColumn38.CurrencyField = null;
		this.gridColumn38.CurrencyFixed = null;
		this.gridColumn38.CurrencyFixedRate = 0.0;
		this.gridColumn38.CurrencyRateField = null;
		this.gridColumn38.FieldName = "Ref1";
		this.gridColumn38.MinWidth = 114;
		this.gridColumn38.Name = "gridColumn38";
		this.gridColumn38.OptionsColumn.AllowEdit = false;
		this.gridColumn38.OptionsColumn.FixedWidth = true;
		this.gridColumn38.OptionsColumn.ReadOnly = true;
		this.gridColumn38.PrimaryKey = false;
		this.gridColumn38.UnitField = null;
		this.gridColumn38.Visible = true;
		this.gridColumn38.VisibleIndex = 16;
		this.gridColumn38.Width = 114;
		this.gridColumn39.Caption = "Receipt Number";
		this.gridColumn39.CurrencyDateField = null;
		this.gridColumn39.CurrencyField = null;
		this.gridColumn39.CurrencyFixed = null;
		this.gridColumn39.CurrencyFixedRate = 0.0;
		this.gridColumn39.CurrencyRateField = null;
		this.gridColumn39.FieldName = "ReceiptNum";
		this.gridColumn39.MinWidth = 114;
		this.gridColumn39.Name = "gridColumn39";
		this.gridColumn39.OptionsColumn.AllowEdit = false;
		this.gridColumn39.OptionsColumn.FixedWidth = true;
		this.gridColumn39.OptionsColumn.ReadOnly = true;
		this.gridColumn39.PrimaryKey = false;
		this.gridColumn39.UnitField = null;
		this.gridColumn39.Visible = true;
		this.gridColumn39.VisibleIndex = 17;
		this.gridColumn39.Width = 114;
		this.gridColumn40.Caption = "Generation Time";
		this.gridColumn40.CurrencyDateField = null;
		this.gridColumn40.CurrencyField = null;
		this.gridColumn40.CurrencyFixed = null;
		this.gridColumn40.CurrencyFixedRate = 0.0;
		this.gridColumn40.CurrencyRateField = null;
		this.gridColumn40.FieldName = "DocTime";
		this.gridColumn40.MinWidth = 114;
		this.gridColumn40.Name = "gridColumn40";
		this.gridColumn40.OptionsColumn.AllowEdit = false;
		this.gridColumn40.OptionsColumn.FixedWidth = true;
		this.gridColumn40.OptionsColumn.ReadOnly = true;
		this.gridColumn40.PrimaryKey = false;
		this.gridColumn40.UnitField = null;
		this.gridColumn40.Visible = true;
		this.gridColumn40.VisibleIndex = 18;
		this.gridColumn40.Width = 114;
		this.gridColumn41.Caption = "TypeDisc";
		this.gridColumn41.CurrencyDateField = null;
		this.gridColumn41.CurrencyField = null;
		this.gridColumn41.CurrencyFixed = null;
		this.gridColumn41.CurrencyFixedRate = 0.0;
		this.gridColumn41.CurrencyRateField = null;
		this.gridColumn41.FieldName = "TypeDisc";
		this.gridColumn41.MinWidth = 114;
		this.gridColumn41.Name = "gridColumn41";
		this.gridColumn41.OptionsColumn.AllowEdit = false;
		this.gridColumn41.OptionsColumn.FixedWidth = true;
		this.gridColumn41.OptionsColumn.ReadOnly = true;
		this.gridColumn41.PrimaryKey = false;
		this.gridColumn41.UnitField = null;
		this.gridColumn41.Visible = true;
		this.gridColumn41.VisibleIndex = 19;
		this.gridColumn41.Width = 114;
		this.gridColumn42.Caption = "ShipToCode";
		this.gridColumn42.CurrencyDateField = null;
		this.gridColumn42.CurrencyField = null;
		this.gridColumn42.CurrencyFixed = null;
		this.gridColumn42.CurrencyFixedRate = 0.0;
		this.gridColumn42.CurrencyRateField = null;
		this.gridColumn42.FieldName = "ShipToCode";
		this.gridColumn42.MinWidth = 114;
		this.gridColumn42.Name = "gridColumn42";
		this.gridColumn42.OptionsColumn.AllowEdit = false;
		this.gridColumn42.OptionsColumn.FixedWidth = true;
		this.gridColumn42.OptionsColumn.ReadOnly = true;
		this.gridColumn42.PrimaryKey = false;
		this.gridColumn42.UnitField = null;
		this.gridColumn42.Visible = true;
		this.gridColumn42.VisibleIndex = 20;
		this.gridColumn42.Width = 114;
		this.gridColumn43.Caption = "PaytoCode";
		this.gridColumn43.CurrencyDateField = null;
		this.gridColumn43.CurrencyField = null;
		this.gridColumn43.CurrencyFixed = null;
		this.gridColumn43.CurrencyFixedRate = 0.0;
		this.gridColumn43.CurrencyRateField = null;
		this.gridColumn43.FieldName = "PaytoCode";
		this.gridColumn43.MinWidth = 114;
		this.gridColumn43.Name = "gridColumn43";
		this.gridColumn43.OptionsColumn.AllowEdit = false;
		this.gridColumn43.OptionsColumn.FixedWidth = true;
		this.gridColumn43.OptionsColumn.ReadOnly = true;
		this.gridColumn43.PrimaryKey = false;
		this.gridColumn43.UnitField = null;
		this.gridColumn43.Visible = true;
		this.gridColumn43.VisibleIndex = 21;
		this.gridColumn43.Width = 114;
		this.gridColumn44.Caption = "Confirmed";
		this.gridColumn44.CurrencyDateField = null;
		this.gridColumn44.CurrencyField = null;
		this.gridColumn44.CurrencyFixed = null;
		this.gridColumn44.CurrencyFixedRate = 0.0;
		this.gridColumn44.CurrencyRateField = null;
		this.gridColumn44.FieldName = "Confirmed";
		this.gridColumn44.MinWidth = 114;
		this.gridColumn44.Name = "gridColumn44";
		this.gridColumn44.OptionsColumn.AllowEdit = false;
		this.gridColumn44.OptionsColumn.FixedWidth = true;
		this.gridColumn44.OptionsColumn.ReadOnly = true;
		this.gridColumn44.PrimaryKey = false;
		this.gridColumn44.UnitField = null;
		this.gridColumn44.Visible = true;
		this.gridColumn44.VisibleIndex = 22;
		this.gridColumn44.Width = 114;
		this.gridColumn45.Caption = "Rounding";
		this.gridColumn45.CurrencyDateField = null;
		this.gridColumn45.CurrencyField = null;
		this.gridColumn45.CurrencyFixed = null;
		this.gridColumn45.CurrencyFixedRate = 0.0;
		this.gridColumn45.CurrencyRateField = null;
		this.gridColumn45.FieldName = "Rounding";
		this.gridColumn45.MinWidth = 114;
		this.gridColumn45.Name = "gridColumn45";
		this.gridColumn45.OptionsColumn.AllowEdit = false;
		this.gridColumn45.OptionsColumn.FixedWidth = true;
		this.gridColumn45.OptionsColumn.ReadOnly = true;
		this.gridColumn45.PrimaryKey = false;
		this.gridColumn45.UnitField = null;
		this.gridColumn45.Visible = true;
		this.gridColumn45.VisibleIndex = 23;
		this.gridColumn45.Width = 114;
		this.gridColumn46.Caption = "Address2";
		this.gridColumn46.CurrencyDateField = null;
		this.gridColumn46.CurrencyField = null;
		this.gridColumn46.CurrencyFixed = null;
		this.gridColumn46.CurrencyFixedRate = 0.0;
		this.gridColumn46.CurrencyRateField = null;
		this.gridColumn46.FieldName = "Address2";
		this.gridColumn46.MinWidth = 114;
		this.gridColumn46.Name = "gridColumn46";
		this.gridColumn46.OptionsColumn.AllowEdit = false;
		this.gridColumn46.OptionsColumn.FixedWidth = true;
		this.gridColumn46.OptionsColumn.ReadOnly = true;
		this.gridColumn46.PrimaryKey = false;
		this.gridColumn46.UnitField = null;
		this.gridColumn46.Visible = true;
		this.gridColumn46.VisibleIndex = 24;
		this.gridColumn46.Width = 114;
		this.memoRemarks.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.memoRemarks.EditControl = null;
		this.memoRemarks.FieldName = null;
		this.memoRemarks.IsSearchControl = false;
		this.memoRemarks.Location = new System.Drawing.Point(85, 339);
		this.memoRemarks.Name = "memoRemarks";
		this.memoRemarks.SettingInVisible = false;
		this.memoRemarks.SettingReadonly = false;
		this.memoRemarks.Size = new System.Drawing.Size(165, 44);
		this.memoRemarks.TabIndex = 17;
		this.labelLine8.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "Remarks";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(5, 345);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(80, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 16;
		this.labelLine8.Utext = "Remarks";
		this.txtJournalRemark.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txtJournalRemark.EditControl = null;
		this.txtJournalRemark.FieldName = null;
		this.txtJournalRemark.IsSearchControl = false;
		this.txtJournalRemark.Location = new System.Drawing.Point(85, 385);
		this.txtJournalRemark.Name = "txtJournalRemark";
		this.txtJournalRemark.SettingInVisible = false;
		this.txtJournalRemark.SettingReadonly = false;
		this.txtJournalRemark.Size = new System.Drawing.Size(165, 20);
		this.txtJournalRemark.TabIndex = 19;
		this.labelLine9.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine9.EditControl = null;
		this.labelLine9.EditValue = "Journal Remarks";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(4, 388);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ReadOnly = false;
		this.labelLine9.SettingInVisible = false;
		this.labelLine9.SettingReadonly = false;
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(80, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 18;
		this.labelLine9.Utext = "Journal Remarks";
		this.txtTotalAmountDue.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txtTotalAmountDue.EditControl = null;
		this.txtTotalAmountDue.FieldName = null;
		this.txtTotalAmountDue.IsSearchControl = false;
		this.txtTotalAmountDue.Location = new System.Drawing.Point(481, 409);
		this.txtTotalAmountDue.Name = "txtTotalAmountDue";
		this.txtTotalAmountDue.Properties.DisplayFormat.FormatString = "0.##";
		this.txtTotalAmountDue.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtTotalAmountDue.Properties.EditFormat.FormatString = "0.##";
		this.txtTotalAmountDue.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtTotalAmountDue.Properties.ReadOnly = true;
		this.txtTotalAmountDue.SettingInVisible = false;
		this.txtTotalAmountDue.SettingReadonly = false;
		this.txtTotalAmountDue.Size = new System.Drawing.Size(165, 20);
		this.txtTotalAmountDue.TabIndex = 27;
		this.txtPaid.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txtPaid.EditControl = null;
		this.txtPaid.FieldName = null;
		this.txtPaid.IsSearchControl = false;
		this.txtPaid.Location = new System.Drawing.Point(481, 340);
		this.txtPaid.Name = "txtPaid";
		this.txtPaid.Properties.DisplayFormat.FormatString = "n2";
		this.txtPaid.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtPaid.Properties.ReadOnly = true;
		this.txtPaid.SettingInVisible = false;
		this.txtPaid.SettingReadonly = false;
		this.txtPaid.Size = new System.Drawing.Size(125, 20);
		this.txtPaid.TabIndex = 24;
		this.labelLine10.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine10.EditControl = null;
		this.labelLine10.EditValue = "Total Amount Due";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(345, 413);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ReadOnly = false;
		this.labelLine10.SettingInVisible = false;
		this.labelLine10.SettingReadonly = false;
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(134, 20);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 23;
		this.labelLine10.Utext = "Total Amount Due";
		this.labelLine11.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine11.EditControl = null;
		this.labelLine11.EditValue = "Total Tax";
		this.labelLine11.FieldName = null;
		this.labelLine11.HideWhenEmpty = false;
		this.labelLine11.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine11.Location = new System.Drawing.Point(345, 389);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.ReadOnly = false;
		this.labelLine11.SettingInVisible = false;
		this.labelLine11.SettingReadonly = false;
		this.labelLine11.ShowUnderLine = true;
		this.labelLine11.Size = new System.Drawing.Size(134, 20);
		this.labelLine11.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine11.TabIndex = 22;
		this.labelLine11.Utext = "Total Tax";
		this.labelLine12.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine12.EditControl = null;
		this.labelLine12.EditValue = "Net Total";
		this.labelLine12.FieldName = null;
		this.labelLine12.HideWhenEmpty = false;
		this.labelLine12.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine12.Location = new System.Drawing.Point(345, 366);
		this.labelLine12.Name = "labelLine12";
		this.labelLine12.ReadOnly = false;
		this.labelLine12.SettingInVisible = false;
		this.labelLine12.SettingReadonly = false;
		this.labelLine12.ShowUnderLine = true;
		this.labelLine12.Size = new System.Drawing.Size(134, 20);
		this.labelLine12.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine12.TabIndex = 21;
		this.labelLine12.Utext = "Net Total";
		this.labelLine13.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine13.EditControl = null;
		this.labelLine13.EditValue = "Paid";
		this.labelLine13.FieldName = null;
		this.labelLine13.HideWhenEmpty = false;
		this.labelLine13.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine13.Location = new System.Drawing.Point(345, 344);
		this.labelLine13.Name = "labelLine13";
		this.labelLine13.ReadOnly = false;
		this.labelLine13.SettingInVisible = false;
		this.labelLine13.SettingReadonly = false;
		this.labelLine13.ShowUnderLine = true;
		this.labelLine13.Size = new System.Drawing.Size(134, 20);
		this.labelLine13.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine13.TabIndex = 20;
		this.labelLine13.Utext = "Paid";
		this.txtTotalTax.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txtTotalTax.EditControl = null;
		this.txtTotalTax.FieldName = null;
		this.txtTotalTax.IsSearchControl = false;
		this.txtTotalTax.Location = new System.Drawing.Point(481, 386);
		this.txtTotalTax.Name = "txtTotalTax";
		this.txtTotalTax.Properties.DisplayFormat.FormatString = "n2";
		this.txtTotalTax.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtTotalTax.Properties.ReadOnly = true;
		this.txtTotalTax.SettingInVisible = false;
		this.txtTotalTax.SettingReadonly = false;
		this.txtTotalTax.Size = new System.Drawing.Size(165, 20);
		this.txtTotalTax.TabIndex = 28;
		this.txtNetTotal.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txtNetTotal.EditControl = null;
		this.txtNetTotal.FieldName = null;
		this.txtNetTotal.IsSearchControl = false;
		this.txtNetTotal.Location = new System.Drawing.Point(481, 363);
		this.txtNetTotal.Name = "txtNetTotal";
		this.txtNetTotal.Properties.DisplayFormat.FormatString = "n2";
		this.txtNetTotal.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtNetTotal.Properties.ReadOnly = true;
		this.txtNetTotal.SettingInVisible = false;
		this.txtNetTotal.SettingReadonly = false;
		this.txtNetTotal.Size = new System.Drawing.Size(165, 20);
		this.txtNetTotal.TabIndex = 29;
		this.btAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btAdd.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Add;
		this.btAdd.EditControl = null;
		this.btAdd.EditValue = "Add";
		this.btAdd.FieldName = null;
		this.btAdd.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btAdd.ImageOptions.Image");
		this.btAdd.IsSearchControl = false;
		this.btAdd.Location = new System.Drawing.Point(5, 438);
		this.btAdd.Name = "btAdd";
		this.btAdd.ReadOnly = false;
		this.btAdd.SettingInVisible = false;
		this.btAdd.SettingReadonly = false;
		this.btAdd.Size = new System.Drawing.Size(75, 23);
		this.btAdd.TabIndex = 30;
		this.btAdd.Text = "Add";
		this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(84, 438);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 31;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btListIncoming.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btListIncoming.EditControl = null;
		this.btListIncoming.EditValue = "List Incoming";
		this.btListIncoming.FieldName = null;
		this.btListIncoming.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btListIncoming.ImageOptions.Image");
		this.btListIncoming.IsSearchControl = false;
		this.btListIncoming.Location = new System.Drawing.Point(554, 438);
		this.btListIncoming.Name = "btListIncoming";
		this.btListIncoming.ReadOnly = false;
		this.btListIncoming.SettingInVisible = false;
		this.btListIncoming.SettingReadonly = false;
		this.btListIncoming.Size = new System.Drawing.Size(92, 23);
		this.btListIncoming.TabIndex = 34;
		this.btListIncoming.Text = "List Incoming";
		this.btListIncoming.Click += new System.EventHandler(this.btListIncoming_Click);
		this.cbDocCurrency.EditControl = null;
		this.cbDocCurrency.FieldName = null;
		this.cbDocCurrency.IsSearchControl = false;
		this.cbDocCurrency.Location = new System.Drawing.Point(85, 96);
		this.cbDocCurrency.Name = "cbDocCurrency";
		this.cbDocCurrency.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbDocCurrency.SettingInVisible = false;
		this.cbDocCurrency.SettingReadonly = false;
		this.cbDocCurrency.Size = new System.Drawing.Size(78, 20);
		this.cbDocCurrency.TabIndex = 35;
		this.lblDocCurrency.EditControl = null;
		this.lblDocCurrency.EditValue = "Doc. Currency";
		this.lblDocCurrency.FieldName = null;
		this.lblDocCurrency.HideWhenEmpty = false;
		this.lblDocCurrency.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblDocCurrency.Location = new System.Drawing.Point(6, 99);
		this.lblDocCurrency.Name = "lblDocCurrency";
		this.lblDocCurrency.ReadOnly = false;
		this.lblDocCurrency.SettingInVisible = false;
		this.lblDocCurrency.SettingReadonly = false;
		this.lblDocCurrency.ShowUnderLine = true;
		this.lblDocCurrency.Size = new System.Drawing.Size(80, 20);
		this.lblDocCurrency.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblDocCurrency.TabIndex = 36;
		this.lblDocCurrency.Utext = "Doc. Currency";
		this.txtRate.EditControl = null;
		this.txtRate.FieldName = null;
		this.txtRate.IsSearchControl = false;
		this.txtRate.Location = new System.Drawing.Point(169, 96);
		this.txtRate.Name = "txtRate";
		this.txtRate.Properties.ReadOnly = true;
		this.txtRate.SettingInVisible = false;
		this.txtRate.SettingReadonly = false;
		this.txtRate.Size = new System.Drawing.Size(81, 20);
		this.txtRate.TabIndex = 37;
		this.txtCode.EditControl = null;
		this.txtCode.FieldName = null;
		this.txtCode.IsSearchControl = false;
		this.txtCode.Location = new System.Drawing.Point(85, 9);
		this.txtCode.Name = "txtCode";
		this.txtCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight, "", -1, true, true, true, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtCode.SettingInVisible = false;
		this.txtCode.SettingReadonly = false;
		this.txtCode.Size = new System.Drawing.Size(165, 20);
		this.txtCode.TabIndex = 3;
		this.txtCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtCode_ButtonPressed);
		this.btPaymentMeans.EditControl = null;
		this.btPaymentMeans.EditValue = "";
		this.btPaymentMeans.FieldName = null;
		this.btPaymentMeans.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btPaymentMeans.ImageOptions.Image");
		this.btPaymentMeans.IsSearchControl = false;
		this.btPaymentMeans.Location = new System.Drawing.Point(608, 339);
		this.btPaymentMeans.Name = "btPaymentMeans";
		this.btPaymentMeans.ReadOnly = false;
		this.btPaymentMeans.SettingInVisible = false;
		this.btPaymentMeans.SettingReadonly = false;
		this.btPaymentMeans.Size = new System.Drawing.Size(38, 21);
		this.btPaymentMeans.TabIndex = 38;
		this.btPaymentMeans.Click += new System.EventHandler(this.btPaymentMeans_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(652, 464);
		base.Controls.Add(this.btPaymentMeans);
		base.Controls.Add(this.txtRate);
		base.Controls.Add(this.lblDocCurrency);
		base.Controls.Add(this.cbDocCurrency);
		base.Controls.Add(this.btListIncoming);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btAdd);
		base.Controls.Add(this.txtNetTotal);
		base.Controls.Add(this.txtTotalTax);
		base.Controls.Add(this.txtTotalAmountDue);
		base.Controls.Add(this.txtPaid);
		base.Controls.Add(this.labelLine10);
		base.Controls.Add(this.labelLine11);
		base.Controls.Add(this.labelLine12);
		base.Controls.Add(this.labelLine13);
		base.Controls.Add(this.txtJournalRemark);
		base.Controls.Add(this.labelLine9);
		base.Controls.Add(this.memoRemarks);
		base.Controls.Add(this.labelLine8);
		base.Controls.Add(this.grdDoc);
		base.Controls.Add(this.txtTransactonNo);
		base.Controls.Add(this.dateDueDate);
		base.Controls.Add(this.datePostingDate);
		base.Controls.Add(this.txtNo);
		base.Controls.Add(this.labelLine7);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.radioGroup1);
		base.Controls.Add(this.memoAddress);
		base.Controls.Add(this.txtName);
		base.Controls.Add(this.lblAddress);
		base.Controls.Add(this.lblName);
		base.Controls.Add(this.lblCode);
		base.Controls.Add(this.txtCode);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PaymentIncoming";
		this.Text = "Incoming Payment";
		base.Load += new System.EventHandler(this.frm_PaymentIncoming_Load);
		((System.ComponentModel.ISupportInitialize)this.txtName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memoAddress.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.radioGroup1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.datePostingDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.datePostingDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateDueDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateDueDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtTransactonNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memoRemarks.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtJournalRemark.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtTotalAmountDue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPaid.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtTotalTax.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtNetTotal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbDocCurrency.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtRate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
