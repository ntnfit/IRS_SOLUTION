using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Payments;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab;
using Newtonsoft.Json;

namespace Apzon.Forms.Payment;

public class frm_PaymentMeans : Form
{
	public delegate void PassObject(IncomingPaymentsImpl payment, bool isCash, bool isBankTransfer, bool isCredit, bool isCoupon);

	public PassObject passObject;

	private List<Common.Table_Code> _lstCurrency = new List<Common.Table_Code>();

	private List<Common.Table_Code> _lstCreditCard = new List<Common.Table_Code>();

	private decimal _cashsum;

	private decimal _creditSum;

	private decimal _docTotal;

	private decimal _couponSum;

	private decimal rate = 1m;

	private decimal totalPayments;

	private int _trsfrSum;

	private int docEntry;

	private string cardCode = "";

	private string cardName = "";

	private string address = "";

	private string curr = "";

	public bool isView;

	private IncomingPaymentsImpl _objView = new IncomingPaymentsImpl();

	private IContainer components;

	private ComboBoxEditEx cbCurrency;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private SimpleButtonEx btAccept;

	private SimpleButtonEx btCancel;

	private LabelLine labelLine11;

	private LabelLine labelLine12;

	private LabelLine labelLine13;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx xtraTabPage2;

	private XtraTabPageEx xtraTabPage3;

	private XtraTabPageEx xtraTabPage4;

	private XtraTabPageEx xtraTabPage1;

	private LabelLine labelLine5;

	private LabelLine labelLine4;

	private TextEditEx txtCashAccount;

	private LabelLine labelLine3;

	private LabelLine labelLine6;

	private LabelLine labelLine7;

	private TextEditEx txtCreditCardConfirmationNo;

	private TextEditEx txtCreditCardAddress;

	private TextEditEx txtCreditCardTel;

	private TextEditEx txtCreditCardIDNumber;

	private TextEditEx txtCreditCarNo;

	private DateEditEx dateCreditCard;

	private ComboBoxEditEx cbCreditCard;

	private LabelLine labelLine17;

	private LabelLine labelLine14;

	private LabelLine labelLine15;

	private LabelLine labelLine16;

	private LabelLine labelLine8;

	private LabelLine labelLine9;

	private LabelLine labelLine10;

	private TextEditEx txtBankTransferReference;

	private TextEditEx txtBankTransferGLAccount;

	private DateEditEx dateBankTransfer;

	private LabelLine labelLine18;

	private LabelLine labelLine19;

	private LabelLine labelLine20;

	private TextEditEx txtCouponAddress;

	private TextEditEx txtCouponTel;

	private TextEditEx txtCouponIDNumber;

	private TextEditEx txtCouponCardName;

	private DateEditEx dateCoupon;

	private ComboBoxEditEx cbCouponCardNo;

	private LabelLine labelLine21;

	private LabelLine labelLine22;

	private LabelLine labelLine23;

	private LabelLine labelLine24;

	private LabelLine labelLine25;

	private LabelLine labelLine26;

	private CalcEditEx calCashTotal;

	private CalcEditEx calCreditCardTotal;

	private CalcEditEx calBankTransferTotal;

	private CalcEditEx calCouponTotal;

	private CalcEditEx calPaid;

	private CalcEditEx calTotalPayments;

	private CalcEditEx calChange;

	public frm_PaymentMeans(int total, int _docentry, string _cardCode, string _cardName, string _address, decimal _rate, string _curr)
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_PaymentMeans"))
		{
			InitializeComponent();
			docEntry = _docentry;
			cardCode = _cardCode;
			cardName = _cardName;
			address = _address;
			rate = _rate;
			curr = _curr;
			cbCurrency.SelectedItem = _curr;
			_cashsum = total;
			_docTotal = total;
			calCashTotal.Text = total.ToString();
			calTotalPayments.Text = total.ToString();
			isView = false;
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_PaymentMeans" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	public frm_PaymentMeans(IncomingPaymentsImpl objView)
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_PaymentMeans"))
		{
			InitializeComponent();
			isView = true;
			_objView = objView;
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_PaymentMeans" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void btAccept_Click(object sender, EventArgs e)
	{
		try
		{
			if (!isView)
			{
				_cashsum = calCashTotal.Value;
				_trsfrSum = Function.ParseInt(calBankTransferTotal.Value);
				_creditSum = Function.ParseInt(calCreditCardTotal.Value);
				_couponSum = Function.ParseInt(calCouponTotal.Value);
				totalPayments = Function.ParseInt(calTotalPayments.Value);
				_docTotal = _cashsum + _creditSum + (decimal)_trsfrSum + _couponSum;
				bool isCash;
				try
				{
					isCash = Convert.ToInt32(_cashsum) > 0;
				}
				catch
				{
					isCash = false;
				}
				bool isBankTransfer;
				try
				{
					isBankTransfer = Convert.ToInt32(_trsfrSum) > 0;
				}
				catch
				{
					isBankTransfer = false;
				}
				bool isCredit;
				try
				{
					isCredit = Convert.ToInt32(_creditSum) > 0;
				}
				catch
				{
					isCredit = false;
				}
				bool isCoupon;
				try
				{
					isCoupon = Convert.ToInt32(_couponSum) > 0;
				}
				catch
				{
					isCoupon = false;
				}
				IncomingPaymentsImpl incomingPaymentsImpl = new IncomingPaymentsImpl();
				IncomingPaymentCreditCardsImpl incomingPaymentCreditCardsImpl = new IncomingPaymentCreditCardsImpl();
				IncomingPaymentCouponsImpl incomingPaymentCouponsImpl = new IncomingPaymentCouponsImpl();
				incomingPaymentsImpl.DocTotal = _docTotal;
				incomingPaymentsImpl.DocEntry = docEntry;
				incomingPaymentsImpl.DocNum = incomingPaymentsImpl.DocEntry;
				incomingPaymentsImpl.CashSum = _cashsum;
				incomingPaymentsImpl.CashSumSy = _cashsum;
				incomingPaymentsImpl.CreditSum = _creditSum;
				incomingPaymentsImpl.CreditSumSy = _creditSum;
				incomingPaymentsImpl.TrsfrSum = _trsfrSum;
				incomingPaymentsImpl.TrsfrSumSy = _trsfrSum;
				incomingPaymentsImpl.TrsfrDate = dateBankTransfer.DateTime;
				incomingPaymentsImpl.CouponSum = _couponSum;
				incomingPaymentsImpl.DocDate = DateTime.Now.Date;
				incomingPaymentsImpl.DocDueDate = DateTime.Now.Date.ToString();
				incomingPaymentsImpl.DocCurr = _lstCurrency.Single((Common.Table_Code x) => x.Name == cbCurrency.SelectedItem.ToString()).Code;
				incomingPaymentsImpl.DocRate = 1m;
				incomingPaymentsImpl.ObjType = "24";
				incomingPaymentsImpl.CreateDate = DateTime.Now.Date;
				incomingPaymentsImpl.UserSign = 3;
				incomingPaymentsImpl.VatGroup = "";
				incomingPaymentsImpl.Dcount = 0m;
				incomingPaymentsImpl.DcntSum = 0m;
				incomingPaymentsImpl.CashSumFC = 0m;
				incomingPaymentsImpl.CreditSumFC = 0m;
				incomingPaymentsImpl.TrsfrSumFC = 0m;
				incomingPaymentsImpl.DcntSumFC = 0m;
				incomingPaymentsImpl.DocType = null;
				incomingPaymentsImpl.Canceled = null;
				incomingPaymentsImpl.Printed = null;
				incomingPaymentsImpl.VatSum = 0m;
				incomingPaymentsImpl.JrnlMemo = "";
				incomingPaymentsImpl.TransId = 0;
				incomingPaymentsImpl.DocTime = DateTime.Now.TimeOfDay;
				incomingPaymentsImpl.TrsfrAcct = txtBankTransferGLAccount.Text.Trim();
				incomingPaymentsImpl.TrsfrRef = txtBankTransferReference.Text.Trim();
				incomingPaymentsImpl.TrsfrDate = dateBankTransfer.DateTime;
				incomingPaymentsImpl.CardCode = cardCode;
				incomingPaymentsImpl.CardName = cardName;
				incomingPaymentsImpl.Address = address;
				incomingPaymentCreditCardsImpl.DocNum = incomingPaymentsImpl.DocNum;
				incomingPaymentCreditCardsImpl.DocDate = DateTime.Now.Date;
				incomingPaymentCreditCardsImpl.CreditCard = Convert.ToInt32(_lstCreditCard.Single((Common.Table_Code x) => x.Name == cbCreditCard.SelectedItem.ToString()).Code);
				incomingPaymentCreditCardsImpl.CrCardNum = txtCreditCarNo.Text.Trim();
				incomingPaymentCreditCardsImpl.CardName = cbCreditCard.SelectedItem.ToString();
				incomingPaymentCreditCardsImpl.CardValid = dateCreditCard.DateTime;
				incomingPaymentCreditCardsImpl.OwnerIdNum = txtCreditCardIDNumber.Text.Trim();
				incomingPaymentCreditCardsImpl.OwnerPhone = txtCreditCardTel.Text.Trim();
				incomingPaymentCreditCardsImpl.OwnerAdd = txtCreditCardAddress.Text.Trim();
				incomingPaymentCreditCardsImpl.CreditSum = _creditSum;
				incomingPaymentCreditCardsImpl.CreditCur = incomingPaymentsImpl.DocCurr;
				incomingPaymentCreditCardsImpl.ConfNum = txtCreditCardConfirmationNo.Text.Trim();
				incomingPaymentCouponsImpl.DocNum = incomingPaymentsImpl.DocNum;
				incomingPaymentCouponsImpl.DocDate = DateTime.Now.Date;
				incomingPaymentCouponsImpl.CouponNum = txtCouponIDNumber.Text;
				incomingPaymentCouponsImpl.CouponName = txtCouponCardName.Text;
				incomingPaymentCouponsImpl.CardValid = dateCoupon.DateTime;
				incomingPaymentCouponsImpl.OwnerIdNum = txtCouponIDNumber.Text;
				incomingPaymentCouponsImpl.OwnerPhone = txtCouponTel.Text.Trim();
				incomingPaymentCouponsImpl.OwnerAdd = txtCouponAddress.Text.Trim();
				incomingPaymentCouponsImpl.CouponSum = _couponSum;
				incomingPaymentCouponsImpl.CouponCur = incomingPaymentsImpl.DocCurr;
				incomingPaymentsImpl._objCoupons = incomingPaymentCouponsImpl;
				incomingPaymentsImpl._objCreditCard = incomingPaymentCreditCardsImpl;
				passObject(incomingPaymentsImpl, isCash, isBankTransfer, isCredit, isCoupon);
			}
			Close();
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

	private void frm_PaymentMeans_Load(object sender, EventArgs e)
	{
		LoadData();
	}

	private async void LoadData()
	{
		_ = 3;
		try
		{
			dateBankTransfer.Text = DateTime.Now.Date.ToString();
			dateCoupon.Text = DateTime.Now.Date.ToString();
			dateCreditCard.Text = DateTime.Now.Date.ToString();
			await LoadCreditCard();
			await LoadCurency();
			if (isView)
			{
				if (_objView.ObjType == 24.ToString())
				{
					await LoadPaymentMeansByIncoming();
				}
				else if (_objView.ObjType == 46.ToString())
				{
					await LoadPaymentMeansByOutgoing();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadCurency()
	{
		_ = 1;
		try
		{
			using HttpClient client = new HttpClient();
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "PaymentMeans", "ShowCurrency");
			new List<CurrencyCodesImpl>();
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
				_lstCurrency.Add(table_Code);
			}
			WinFuntions._addComboBox(cbCurrency, _lstCurrency, _selectFirstItem: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadCreditCard()
	{
		_ = 1;
		try
		{
			using HttpClient client = new HttpClient();
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "PaymentMeans", "ShowCreditCardType");
			new List<CreditCardTypesImpl>();
			using HttpResponseMessage response = await client.GetAsync(apiUri);
			if (!response.IsSuccessStatusCode)
			{
				return;
			}
			foreach (CreditCardTypesImpl item in JsonConvert.DeserializeObject<List<CreditCardTypesImpl>>(await response.Content.ReadAsStringAsync()).ToList())
			{
				Common.Table_Code table_Code = new Common.Table_Code();
				table_Code.Code = item.CreditCard.ToString();
				table_Code.Name = item.CardName;
				_lstCreditCard.Add(table_Code);
			}
			WinFuntions._addComboBox(cbCreditCard, _lstCreditCard, _selectFirstItem: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadPaymentMeansByIncoming()
	{
		_ = 1;
		try
		{
			using HttpClient client = new HttpClient();
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "PaymentMeans", "LoadPaymentMeansByIncoming", new
			{
				docNum = _objView.DocEntry
			});
			using HttpResponseMessage response = await client.GetAsync(apiUri);
			if (response.IsSuccessStatusCode)
			{
				DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(await response.Content.ReadAsStringAsync());
				if (dataTable.Rows.Count > 0)
				{
					SetControlValue(dataTable.Rows[0]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadPaymentMeansByOutgoing()
	{
		_ = 1;
		try
		{
			using HttpClient client = new HttpClient();
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "PaymentMeans", "LoadPaymentMeansByOutgoing", new
			{
				docNum = _objView.DocEntry
			});
			using HttpResponseMessage response = await client.GetAsync(apiUri);
			if (response.IsSuccessStatusCode)
			{
				DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(await response.Content.ReadAsStringAsync());
				if (dataTable.Rows.Count > 0)
				{
					SetControlValue(dataTable.Rows[0]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetControlValue(DataRow dtr)
	{
		try
		{
			cbCurrency.EditValue = dtr["CreditCur"];
			cbCreditCard.EditValue = dtr["crCreditCard"];
			cbCouponCardNo.EditValue = dtr["CouponNum"];
			calBankTransferTotal.EditValue = _objView.TrsfrSum;
			calCashTotal.EditValue = _objView.CashSum;
			calCouponTotal.EditValue = _objView.CouponSum;
			calCreditCardTotal.EditValue = _objView.CreditSum;
			calPaid.EditValue = _objView.DocTotal;
			calTotalPayments.EditValue = "0";
			txtBankTransferGLAccount.EditValue = _objView.TrsfrAcct;
			txtBankTransferReference.EditValue = _objView.TrsfrRef;
			txtCashAccount.EditValue = _objView.CashAcct;
			txtCouponAddress.EditValue = dtr["coOwnerAdd"];
			txtCouponCardName.EditValue = dtr["CouponName"];
			txtCouponIDNumber.EditValue = dtr["coOwnerIdNum"];
			txtCouponTel.EditValue = dtr["coOwnerPhone"];
			txtCreditCardAddress.EditValue = dtr["crOwnerAdd"];
			txtCreditCardConfirmationNo.EditValue = dtr["ConfNum"];
			txtCreditCardIDNumber.EditValue = dtr["crOwnerIdNum"];
			txtCreditCardTel.EditValue = dtr["crOwnerPhone"];
			txtCreditCarNo.EditValue = dtr["crCardNum"];
			calChange.EditValue = "0";
			dateBankTransfer.EditValue = _objView.TrsfrDate;
			dateCoupon.EditValue = dtr["coCardValid"];
			dateCreditCard.EditValue = dtr["crCardValid"];
			cbCurrency.Enabled = false;
			cbCreditCard.Enabled = false;
			cbCouponCardNo.Enabled = false;
			calBankTransferTotal.Enabled = false;
			calCashTotal.Enabled = false;
			calChange.Enabled = false;
			calCouponTotal.Enabled = false;
			calCreditCardTotal.Enabled = false;
			calPaid.Enabled = false;
			calTotalPayments.Enabled = false;
			txtBankTransferGLAccount.Enabled = false;
			txtBankTransferReference.Enabled = false;
			txtCashAccount.Enabled = false;
			txtCouponAddress.Enabled = false;
			txtCouponCardName.Enabled = false;
			txtCouponIDNumber.Enabled = false;
			txtCouponTel.Enabled = false;
			txtCreditCardAddress.Enabled = false;
			txtCreditCardConfirmationNo.Enabled = false;
			txtCreditCardIDNumber.Enabled = false;
			txtCreditCardTel.Enabled = false;
			txtCreditCarNo.Enabled = false;
			dateBankTransfer.Enabled = false;
			dateCoupon.Enabled = false;
			dateCreditCard.Enabled = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void updateTextValue()
	{
		try
		{
			_cashsum = calCashTotal.Value;
			_trsfrSum = Function.ParseInt(calBankTransferTotal.Value);
			_creditSum = Function.ParseInt(calCreditCardTotal.Value);
			_couponSum = Function.ParseInt(calCouponTotal.Value);
			totalPayments = Function.ParseInt(calTotalPayments.Value);
			_docTotal = _cashsum + _creditSum + (decimal)_trsfrSum + _couponSum;
			decimal num = totalPayments - _docTotal;
			calPaid.Text = _docTotal.ToString();
			calChange.Text = num.ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void calCashTotal_EditValueChanged(object sender, EventArgs e)
	{
		updateTextValue();
	}

	private void calTotalPayments_EditValueChanged(object sender, EventArgs e)
	{
		updateTextValue();
	}

	private void calCreditCardTotal_EditValueChanged(object sender, EventArgs e)
	{
		updateTextValue();
	}

	private void calBankTransferTotal_EditValueChanged(object sender, EventArgs e)
	{
		updateTextValue();
	}

	private void calCouponTotal_EditValueChanged(object sender, EventArgs e)
	{
		updateTextValue();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Payment.frm_PaymentMeans));
		this.cbCurrency = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.btAccept = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine12 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine13 = new Apzon.Controls.Controls.LabelLine();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.calCashTotal = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txtCashAccount = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.calCreditCardTotal = new Apzon.Controls.Editor.CalcEditEx();
		this.txtCreditCardConfirmationNo = new Apzon.Controls.Editor.TextEditEx();
		this.txtCreditCardAddress = new Apzon.Controls.Editor.TextEditEx();
		this.txtCreditCardTel = new Apzon.Controls.Editor.TextEditEx();
		this.txtCreditCardIDNumber = new Apzon.Controls.Editor.TextEditEx();
		this.txtCreditCarNo = new Apzon.Controls.Editor.TextEditEx();
		this.dateCreditCard = new Apzon.Controls.Editor.DateEditEx();
		this.cbCreditCard = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.labelLine17 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine14 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine15 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine16 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.xtraTabPage3 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.calBankTransferTotal = new Apzon.Controls.Editor.CalcEditEx();
		this.txtBankTransferReference = new Apzon.Controls.Editor.TextEditEx();
		this.txtBankTransferGLAccount = new Apzon.Controls.Editor.TextEditEx();
		this.dateBankTransfer = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine18 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine19 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine20 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.xtraTabPage4 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.calCouponTotal = new Apzon.Controls.Editor.CalcEditEx();
		this.txtCouponAddress = new Apzon.Controls.Editor.TextEditEx();
		this.txtCouponTel = new Apzon.Controls.Editor.TextEditEx();
		this.txtCouponIDNumber = new Apzon.Controls.Editor.TextEditEx();
		this.txtCouponCardName = new Apzon.Controls.Editor.TextEditEx();
		this.dateCoupon = new Apzon.Controls.Editor.DateEditEx();
		this.cbCouponCardNo = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.labelLine21 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine22 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine23 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine24 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine25 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine26 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.calPaid = new Apzon.Controls.Editor.CalcEditEx();
		this.calTotalPayments = new Apzon.Controls.Editor.CalcEditEx();
		this.calChange = new Apzon.Controls.Editor.CalcEditEx();
		((System.ComponentModel.ISupportInitialize)this.cbCurrency.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calCashTotal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCashAccount.Properties).BeginInit();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calCreditCardTotal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCreditCardConfirmationNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCreditCardAddress.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCreditCardTel.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCreditCardIDNumber.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCreditCarNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateCreditCard.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateCreditCard.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbCreditCard.Properties).BeginInit();
		this.xtraTabPage3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calBankTransferTotal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBankTransferReference.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBankTransferGLAccount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateBankTransfer.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateBankTransfer.Properties).BeginInit();
		this.xtraTabPage4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calCouponTotal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCouponAddress.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCouponTel.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCouponIDNumber.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCouponCardName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateCoupon.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateCoupon.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbCouponCardNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calPaid.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calTotalPayments.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calChange.Properties).BeginInit();
		base.SuspendLayout();
		this.cbCurrency.Location = new System.Drawing.Point(110, 12);
		this.cbCurrency.Name = "cbCurrency";
		this.cbCurrency.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbCurrency.Size = new System.Drawing.Size(116, 20);
		this.cbCurrency.TabIndex = 0;
		this.labelLine1.Location = new System.Drawing.Point(31, 13);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.TabIndex = 1;
		this.labelLine1.Utext = "Currency";
		this.labelLine2.Location = new System.Drawing.Point(258, 12);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.TabIndex = 2;
		this.labelLine2.Utext = "Rate";
		this.btAccept.Image = (System.Drawing.Image)resources.GetObject("btAccept.Image");
		this.btAccept.Location = new System.Drawing.Point(5, 353);
		this.btAccept.Name = "btAccept";
		this.btAccept.Size = new System.Drawing.Size(75, 23);
		this.btAccept.TabIndex = 3;
		this.btAccept.Text = "Accept";
		this.btAccept.Click += new System.EventHandler(this.btAccept_Click);
		this.btCancel.Image = (System.Drawing.Image)resources.GetObject("btCancel.Image");
		this.btCancel.Location = new System.Drawing.Point(90, 353);
		this.btCancel.Name = "btCancel";
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 4;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.labelLine11.Location = new System.Drawing.Point(209, 326);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.Size = new System.Drawing.Size(82, 20);
		this.labelLine11.TabIndex = 32;
		this.labelLine11.Utext = "Change";
		this.labelLine12.Location = new System.Drawing.Point(209, 303);
		this.labelLine12.Name = "labelLine12";
		this.labelLine12.Size = new System.Drawing.Size(82, 20);
		this.labelLine12.TabIndex = 31;
		this.labelLine12.Utext = "Total payments";
		this.labelLine13.Location = new System.Drawing.Point(209, 281);
		this.labelLine13.Name = "labelLine13";
		this.labelLine13.Size = new System.Drawing.Size(82, 20);
		this.labelLine13.TabIndex = 30;
		this.labelLine13.Utext = "Paid";
		this.xtraTabControl1.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.xtraTabControl1.Appearance.BackColor2 = System.Drawing.Color.Transparent;
		this.xtraTabControl1.Appearance.Options.UseBackColor = true;
		this.xtraTabControl1.AppearancePage.Header.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.xtraTabControl1.AppearancePage.Header.BackColor2 = System.Drawing.Color.FromArgb(235, 236, 239);
		this.xtraTabControl1.AppearancePage.Header.BorderColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.xtraTabControl1.AppearancePage.Header.Options.UseBackColor = true;
		this.xtraTabControl1.AppearancePage.Header.Options.UseBorderColor = true;
		this.xtraTabControl1.AppearancePage.PageClient.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.xtraTabControl1.AppearancePage.PageClient.BackColor2 = System.Drawing.Color.FromArgb(235, 236, 239);
		this.xtraTabControl1.AppearancePage.PageClient.BorderColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.xtraTabControl1.AppearancePage.PageClient.Options.UseBackColor = true;
		this.xtraTabControl1.AppearancePage.PageClient.Options.UseBorderColor = true;
		this.xtraTabControl1.Location = new System.Drawing.Point(1, 43);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.PaintStyleName = "Standard";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl1.Size = new System.Drawing.Size(433, 227);
		this.xtraTabControl1.TabIndex = 36;
		DevExpress.XtraTab.XtraTabPageCollection tabPages = this.xtraTabControl1.TabPages;
		DevExpress.XtraTab.XtraTabPage[] pages = new Apzon.Controls.Controls.XtraTabPageEx[4] { this.xtraTabPage1, this.xtraTabPage2, this.xtraTabPage3, this.xtraTabPage4 };
		tabPages.AddRange(pages);
		this.xtraTabPage1.Appearance.PageClient.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.xtraTabPage1.Appearance.PageClient.Options.UseBackColor = true;
		this.xtraTabPage1.Controls.Add(this.calCashTotal);
		this.xtraTabPage1.Controls.Add(this.labelLine4);
		this.xtraTabPage1.Controls.Add(this.txtCashAccount);
		this.xtraTabPage1.Controls.Add(this.labelLine3);
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.Size = new System.Drawing.Size(429, 203);
		this.xtraTabPage1.Text = "Cash";
		this.calCashTotal.Location = new System.Drawing.Point(288, 179);
		this.calCashTotal.Name = "calCashTotal";
		this.calCashTotal.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calCashTotal.Size = new System.Drawing.Size(138, 20);
		this.calCashTotal.TabIndex = 38;
		this.calCashTotal.EditValueChanged += new System.EventHandler(this.calCashTotal_EditValueChanged);
		this.labelLine4.Location = new System.Drawing.Point(206, 179);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.Size = new System.Drawing.Size(82, 20);
		this.labelLine4.TabIndex = 36;
		this.labelLine4.Utext = "Total";
		this.txtCashAccount.Location = new System.Drawing.Point(86, 3);
		this.txtCashAccount.Name = "txtCashAccount";
		this.txtCashAccount.Properties.ReadOnly = true;
		this.txtCashAccount.Size = new System.Drawing.Size(138, 20);
		this.txtCashAccount.TabIndex = 35;
		this.labelLine3.Location = new System.Drawing.Point(4, 6);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.Size = new System.Drawing.Size(82, 20);
		this.labelLine3.TabIndex = 34;
		this.labelLine3.Utext = "Account";
		this.xtraTabPage2.Controls.Add(this.calCreditCardTotal);
		this.xtraTabPage2.Controls.Add(this.txtCreditCardConfirmationNo);
		this.xtraTabPage2.Controls.Add(this.txtCreditCardAddress);
		this.xtraTabPage2.Controls.Add(this.txtCreditCardTel);
		this.xtraTabPage2.Controls.Add(this.txtCreditCardIDNumber);
		this.xtraTabPage2.Controls.Add(this.txtCreditCarNo);
		this.xtraTabPage2.Controls.Add(this.dateCreditCard);
		this.xtraTabPage2.Controls.Add(this.cbCreditCard);
		this.xtraTabPage2.Controls.Add(this.labelLine17);
		this.xtraTabPage2.Controls.Add(this.labelLine14);
		this.xtraTabPage2.Controls.Add(this.labelLine15);
		this.xtraTabPage2.Controls.Add(this.labelLine16);
		this.xtraTabPage2.Controls.Add(this.labelLine8);
		this.xtraTabPage2.Controls.Add(this.labelLine9);
		this.xtraTabPage2.Controls.Add(this.labelLine10);
		this.xtraTabPage2.Controls.Add(this.labelLine5);
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.Size = new System.Drawing.Size(429, 203);
		this.xtraTabPage2.Text = "Credit card";
		this.calCreditCardTotal.Location = new System.Drawing.Point(288, 176);
		this.calCreditCardTotal.Name = "calCreditCardTotal";
		this.calCreditCardTotal.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calCreditCardTotal.Size = new System.Drawing.Size(138, 20);
		this.calCreditCardTotal.TabIndex = 54;
		this.calCreditCardTotal.EditValueChanged += new System.EventHandler(this.calCreditCardTotal_EditValueChanged);
		this.txtCreditCardConfirmationNo.Location = new System.Drawing.Point(123, 141);
		this.txtCreditCardConfirmationNo.Name = "txtCreditCardConfirmationNo";
		this.txtCreditCardConfirmationNo.Size = new System.Drawing.Size(116, 20);
		this.txtCreditCardConfirmationNo.TabIndex = 53;
		this.txtCreditCardAddress.Location = new System.Drawing.Point(123, 118);
		this.txtCreditCardAddress.Name = "txtCreditCardAddress";
		this.txtCreditCardAddress.Size = new System.Drawing.Size(116, 20);
		this.txtCreditCardAddress.TabIndex = 52;
		this.txtCreditCardTel.Location = new System.Drawing.Point(123, 95);
		this.txtCreditCardTel.Name = "txtCreditCardTel";
		this.txtCreditCardTel.Size = new System.Drawing.Size(116, 20);
		this.txtCreditCardTel.TabIndex = 51;
		this.txtCreditCardIDNumber.Location = new System.Drawing.Point(123, 72);
		this.txtCreditCardIDNumber.Name = "txtCreditCardIDNumber";
		this.txtCreditCardIDNumber.Size = new System.Drawing.Size(116, 20);
		this.txtCreditCardIDNumber.TabIndex = 50;
		this.txtCreditCarNo.Location = new System.Drawing.Point(123, 26);
		this.txtCreditCarNo.Name = "txtCreditCarNo";
		this.txtCreditCarNo.Size = new System.Drawing.Size(116, 20);
		this.txtCreditCarNo.TabIndex = 49;
		this.dateCreditCard.EditValue = null;
		this.dateCreditCard.Location = new System.Drawing.Point(123, 49);
		this.dateCreditCard.Name = "dateCreditCard";
		this.dateCreditCard.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateCreditCard.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateCreditCard.Size = new System.Drawing.Size(116, 20);
		this.dateCreditCard.TabIndex = 48;
		this.cbCreditCard.Location = new System.Drawing.Point(123, 4);
		this.cbCreditCard.Name = "cbCreditCard";
		this.cbCreditCard.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbCreditCard.Size = new System.Drawing.Size(116, 20);
		this.cbCreditCard.TabIndex = 47;
		this.labelLine17.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine17.Appearance.Options.UseBackColor = true;
		this.labelLine17.Location = new System.Drawing.Point(12, 143);
		this.labelLine17.Name = "labelLine17";
		this.labelLine17.Size = new System.Drawing.Size(111, 20);
		this.labelLine17.TabIndex = 46;
		this.labelLine17.Utext = "Confirmation No.";
		this.labelLine14.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine14.Appearance.Options.UseBackColor = true;
		this.labelLine14.Location = new System.Drawing.Point(12, 121);
		this.labelLine14.Name = "labelLine14";
		this.labelLine14.Size = new System.Drawing.Size(111, 20);
		this.labelLine14.TabIndex = 45;
		this.labelLine14.Utext = "Address";
		this.labelLine15.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine15.Appearance.Options.UseBackColor = true;
		this.labelLine15.Location = new System.Drawing.Point(12, 98);
		this.labelLine15.Name = "labelLine15";
		this.labelLine15.Size = new System.Drawing.Size(111, 20);
		this.labelLine15.TabIndex = 44;
		this.labelLine15.Utext = "Telephone";
		this.labelLine16.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine16.Appearance.Options.UseBackColor = true;
		this.labelLine16.Location = new System.Drawing.Point(12, 75);
		this.labelLine16.Name = "labelLine16";
		this.labelLine16.Size = new System.Drawing.Size(111, 20);
		this.labelLine16.TabIndex = 43;
		this.labelLine16.Utext = "ID Number";
		this.labelLine8.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine8.Appearance.Options.UseBackColor = true;
		this.labelLine8.Location = new System.Drawing.Point(12, 51);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.Size = new System.Drawing.Size(111, 20);
		this.labelLine8.TabIndex = 42;
		this.labelLine8.Utext = "Credit Card Valid Until";
		this.labelLine9.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine9.Appearance.Options.UseBackColor = true;
		this.labelLine9.Location = new System.Drawing.Point(12, 29);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.Size = new System.Drawing.Size(111, 20);
		this.labelLine9.TabIndex = 41;
		this.labelLine9.Utext = "Credit Card No.";
		this.labelLine10.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine10.Appearance.Options.UseBackColor = true;
		this.labelLine10.Location = new System.Drawing.Point(12, 7);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.Size = new System.Drawing.Size(111, 20);
		this.labelLine10.TabIndex = 40;
		this.labelLine10.Utext = "Credit Card";
		this.labelLine5.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine5.Appearance.Options.UseBackColor = true;
		this.labelLine5.Location = new System.Drawing.Point(206, 179);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.Size = new System.Drawing.Size(82, 20);
		this.labelLine5.TabIndex = 38;
		this.labelLine5.Utext = "Total";
		this.xtraTabPage3.Controls.Add(this.calBankTransferTotal);
		this.xtraTabPage3.Controls.Add(this.txtBankTransferReference);
		this.xtraTabPage3.Controls.Add(this.txtBankTransferGLAccount);
		this.xtraTabPage3.Controls.Add(this.dateBankTransfer);
		this.xtraTabPage3.Controls.Add(this.labelLine18);
		this.xtraTabPage3.Controls.Add(this.labelLine19);
		this.xtraTabPage3.Controls.Add(this.labelLine20);
		this.xtraTabPage3.Controls.Add(this.labelLine6);
		this.xtraTabPage3.Name = "xtraTabPage3";
		this.xtraTabPage3.Size = new System.Drawing.Size(429, 203);
		this.xtraTabPage3.Text = "Bank transfer";
		this.calBankTransferTotal.Location = new System.Drawing.Point(288, 176);
		this.calBankTransferTotal.Name = "calBankTransferTotal";
		this.calBankTransferTotal.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calBankTransferTotal.Size = new System.Drawing.Size(138, 20);
		this.calBankTransferTotal.TabIndex = 57;
		this.calBankTransferTotal.EditValueChanged += new System.EventHandler(this.calBankTransferTotal_EditValueChanged);
		this.txtBankTransferReference.Location = new System.Drawing.Point(102, 50);
		this.txtBankTransferReference.Name = "txtBankTransferReference";
		this.txtBankTransferReference.Size = new System.Drawing.Size(116, 20);
		this.txtBankTransferReference.TabIndex = 56;
		this.txtBankTransferGLAccount.Location = new System.Drawing.Point(102, 4);
		this.txtBankTransferGLAccount.Name = "txtBankTransferGLAccount";
		this.txtBankTransferGLAccount.Properties.ReadOnly = true;
		this.txtBankTransferGLAccount.Size = new System.Drawing.Size(116, 20);
		this.txtBankTransferGLAccount.TabIndex = 55;
		this.dateBankTransfer.EditValue = null;
		this.dateBankTransfer.Location = new System.Drawing.Point(102, 27);
		this.dateBankTransfer.Name = "dateBankTransfer";
		this.dateBankTransfer.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateBankTransfer.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateBankTransfer.Size = new System.Drawing.Size(116, 20);
		this.dateBankTransfer.TabIndex = 54;
		this.labelLine18.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine18.Appearance.Options.UseBackColor = true;
		this.labelLine18.Location = new System.Drawing.Point(9, 52);
		this.labelLine18.Name = "labelLine18";
		this.labelLine18.Size = new System.Drawing.Size(94, 20);
		this.labelLine18.TabIndex = 53;
		this.labelLine18.Utext = "Reference";
		this.labelLine19.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine19.Appearance.Options.UseBackColor = true;
		this.labelLine19.Location = new System.Drawing.Point(9, 28);
		this.labelLine19.Name = "labelLine19";
		this.labelLine19.Size = new System.Drawing.Size(94, 20);
		this.labelLine19.TabIndex = 52;
		this.labelLine19.Utext = "Transfer Date";
		this.labelLine20.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine20.Appearance.Options.UseBackColor = true;
		this.labelLine20.Location = new System.Drawing.Point(9, 5);
		this.labelLine20.Name = "labelLine20";
		this.labelLine20.Size = new System.Drawing.Size(94, 20);
		this.labelLine20.TabIndex = 51;
		this.labelLine20.Utext = "G/L Account";
		this.labelLine6.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine6.Appearance.Options.UseBackColor = true;
		this.labelLine6.Location = new System.Drawing.Point(206, 179);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.Size = new System.Drawing.Size(82, 20);
		this.labelLine6.TabIndex = 38;
		this.labelLine6.Utext = "Total";
		this.xtraTabPage4.Controls.Add(this.calCouponTotal);
		this.xtraTabPage4.Controls.Add(this.txtCouponAddress);
		this.xtraTabPage4.Controls.Add(this.txtCouponTel);
		this.xtraTabPage4.Controls.Add(this.txtCouponIDNumber);
		this.xtraTabPage4.Controls.Add(this.txtCouponCardName);
		this.xtraTabPage4.Controls.Add(this.dateCoupon);
		this.xtraTabPage4.Controls.Add(this.cbCouponCardNo);
		this.xtraTabPage4.Controls.Add(this.labelLine21);
		this.xtraTabPage4.Controls.Add(this.labelLine22);
		this.xtraTabPage4.Controls.Add(this.labelLine23);
		this.xtraTabPage4.Controls.Add(this.labelLine24);
		this.xtraTabPage4.Controls.Add(this.labelLine25);
		this.xtraTabPage4.Controls.Add(this.labelLine26);
		this.xtraTabPage4.Controls.Add(this.labelLine7);
		this.xtraTabPage4.Name = "xtraTabPage4";
		this.xtraTabPage4.Size = new System.Drawing.Size(429, 203);
		this.xtraTabPage4.Text = "Coupon";
		this.calCouponTotal.Location = new System.Drawing.Point(288, 176);
		this.calCouponTotal.Name = "calCouponTotal";
		this.calCouponTotal.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calCouponTotal.Size = new System.Drawing.Size(138, 20);
		this.calCouponTotal.TabIndex = 65;
		this.calCouponTotal.EditValueChanged += new System.EventHandler(this.calCouponTotal_EditValueChanged);
		this.txtCouponAddress.Location = new System.Drawing.Point(123, 118);
		this.txtCouponAddress.Name = "txtCouponAddress";
		this.txtCouponAddress.Size = new System.Drawing.Size(116, 20);
		this.txtCouponAddress.TabIndex = 64;
		this.txtCouponTel.Location = new System.Drawing.Point(123, 95);
		this.txtCouponTel.Name = "txtCouponTel";
		this.txtCouponTel.Size = new System.Drawing.Size(116, 20);
		this.txtCouponTel.TabIndex = 63;
		this.txtCouponIDNumber.Location = new System.Drawing.Point(123, 72);
		this.txtCouponIDNumber.Name = "txtCouponIDNumber";
		this.txtCouponIDNumber.Size = new System.Drawing.Size(116, 20);
		this.txtCouponIDNumber.TabIndex = 62;
		this.txtCouponCardName.Location = new System.Drawing.Point(123, 26);
		this.txtCouponCardName.Name = "txtCouponCardName";
		this.txtCouponCardName.Size = new System.Drawing.Size(116, 20);
		this.txtCouponCardName.TabIndex = 61;
		this.dateCoupon.EditValue = null;
		this.dateCoupon.Location = new System.Drawing.Point(123, 49);
		this.dateCoupon.Name = "dateCoupon";
		this.dateCoupon.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateCoupon.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateCoupon.Size = new System.Drawing.Size(116, 20);
		this.dateCoupon.TabIndex = 60;
		this.cbCouponCardNo.Location = new System.Drawing.Point(123, 4);
		this.cbCouponCardNo.Name = "cbCouponCardNo";
		this.cbCouponCardNo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbCouponCardNo.Size = new System.Drawing.Size(116, 20);
		this.cbCouponCardNo.TabIndex = 59;
		this.labelLine21.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine21.Appearance.Options.UseBackColor = true;
		this.labelLine21.Location = new System.Drawing.Point(7, 122);
		this.labelLine21.Name = "labelLine21";
		this.labelLine21.Size = new System.Drawing.Size(114, 20);
		this.labelLine21.TabIndex = 58;
		this.labelLine21.Utext = "Address";
		this.labelLine22.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine22.Appearance.Options.UseBackColor = true;
		this.labelLine22.Location = new System.Drawing.Point(7, 99);
		this.labelLine22.Name = "labelLine22";
		this.labelLine22.Size = new System.Drawing.Size(114, 20);
		this.labelLine22.TabIndex = 57;
		this.labelLine22.Utext = "Telephone";
		this.labelLine23.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine23.Appearance.Options.UseBackColor = true;
		this.labelLine23.Location = new System.Drawing.Point(7, 77);
		this.labelLine23.Name = "labelLine23";
		this.labelLine23.Size = new System.Drawing.Size(114, 20);
		this.labelLine23.TabIndex = 56;
		this.labelLine23.Utext = "ID Number";
		this.labelLine24.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine24.Appearance.Options.UseBackColor = true;
		this.labelLine24.Location = new System.Drawing.Point(7, 51);
		this.labelLine24.Name = "labelLine24";
		this.labelLine24.Size = new System.Drawing.Size(114, 20);
		this.labelLine24.TabIndex = 55;
		this.labelLine24.Utext = "Coupon Card Valid Until";
		this.labelLine25.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine25.Appearance.Options.UseBackColor = true;
		this.labelLine25.Location = new System.Drawing.Point(7, 28);
		this.labelLine25.Name = "labelLine25";
		this.labelLine25.Size = new System.Drawing.Size(114, 20);
		this.labelLine25.TabIndex = 54;
		this.labelLine25.Utext = "Coupon Card Name";
		this.labelLine26.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine26.Appearance.Options.UseBackColor = true;
		this.labelLine26.Location = new System.Drawing.Point(7, 6);
		this.labelLine26.Name = "labelLine26";
		this.labelLine26.Size = new System.Drawing.Size(114, 20);
		this.labelLine26.TabIndex = 53;
		this.labelLine26.Utext = "Coupon Card No.";
		this.labelLine7.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.labelLine7.Appearance.Options.UseBackColor = true;
		this.labelLine7.Location = new System.Drawing.Point(206, 179);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.Size = new System.Drawing.Size(82, 20);
		this.labelLine7.TabIndex = 38;
		this.labelLine7.Utext = "Total";
		this.calPaid.Location = new System.Drawing.Point(291, 277);
		this.calPaid.Name = "calPaid";
		this.calPaid.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calPaid.Properties.ReadOnly = true;
		this.calPaid.Size = new System.Drawing.Size(138, 20);
		this.calPaid.TabIndex = 37;
		this.calTotalPayments.Location = new System.Drawing.Point(291, 299);
		this.calTotalPayments.Name = "calTotalPayments";
		this.calTotalPayments.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calTotalPayments.Size = new System.Drawing.Size(138, 20);
		this.calTotalPayments.TabIndex = 38;
		this.calTotalPayments.EditValueChanged += new System.EventHandler(this.calTotalPayments_EditValueChanged);
		this.calChange.Location = new System.Drawing.Point(291, 322);
		this.calChange.Name = "calChange";
		this.calChange.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calChange.Properties.ReadOnly = true;
		this.calChange.Size = new System.Drawing.Size(138, 20);
		this.calChange.TabIndex = 39;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		base.ClientSize = new System.Drawing.Size(433, 379);
		base.Controls.Add(this.calChange);
		base.Controls.Add(this.calTotalPayments);
		base.Controls.Add(this.calPaid);
		base.Controls.Add(this.xtraTabControl1);
		base.Controls.Add(this.labelLine11);
		base.Controls.Add(this.labelLine12);
		base.Controls.Add(this.labelLine13);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btAccept);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.cbCurrency);
		base.Name = "frm_PaymentMeans";
		this.Text = "Payment means";
		base.Load += new System.EventHandler(this.frm_PaymentMeans_Load);
		((System.ComponentModel.ISupportInitialize)this.cbCurrency.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.calCashTotal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCashAccount.Properties).EndInit();
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.calCreditCardTotal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCreditCardConfirmationNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCreditCardAddress.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCreditCardTel.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCreditCardIDNumber.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCreditCarNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateCreditCard.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateCreditCard.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbCreditCard.Properties).EndInit();
		this.xtraTabPage3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.calBankTransferTotal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBankTransferReference.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBankTransferGLAccount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateBankTransfer.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateBankTransfer.Properties).EndInit();
		this.xtraTabPage4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.calCouponTotal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCouponAddress.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCouponTel.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCouponIDNumber.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCouponCardName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateCoupon.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateCoupon.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbCouponCardNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calPaid.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calTotalPayments.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calChange.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
