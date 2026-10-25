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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Form.ExchangeRateAndIndex;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Purchase;

public class frm_PurchaseLandedCost : XtraFormEx
{
	private readonly List<Common.Table_Code> lstShippingType = new List<Common.Table_Code>();

	private readonly List<Document_LinesImpl> lstDocItem = new List<Document_LinesImpl>();

	private readonly object[] arrAllocation = new object[6] { "F \t\t - \t\t Cash Value Before Custom", "C \t\t - \t\t Cash Value After Custom", "Q \t\t - \t\t Quantity", "V \t\t - \t\t Weight", "A \t\t - \t\t Volume", "W \t\t - \t\t Equal" };

	private readonly object[] arrUnitVolume = new object[6] { "1 \t\t - \t\t a", "2 \t\t - \t\t b", "3 \t\t - \t\t c", "4 \t\t - \t\t d", "5 \t\t - \t\t e", "6 \t\t - \t\t f" };

	private int newDocEntry;

	private int currentEntry;

	private int firstEntry;

	private int lastEntry;

	private string vendorCode;

	private string brokerCode;

	private string lastFocus;

	private readonly HttpClientHelper<DocumentsImpl> _httpClientHelper = new HttpClientHelper<DocumentsImpl>(Configuration.ApiServer, "LandedCost");

	public bool isFinding;

	private readonly HttpClientHelper<ExchangeRateAndIndexImpl> _httpHelperCurrRate = new HttpClientHelper<ExchangeRateAndIndexImpl>(Configuration.ApiServer, "ExRateAndIndex");

	private IContainer components;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private TextEditEx txtVendorDetail;

	private TextEditEx txtBrokerDetail;

	private TextEditEx txtFileNo;

	private TextEditEx txtReference;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private TextEditEx txtNumber;

	private LabelLine labelLine5;

	private LabelLine labelLine6;

	private LabelLine labelLine7;

	private DateEditEx datePostingDate;

	private DateEditEx dateDueDate;

	private ComboBoxEditEx cbSeries;

	private LabelLine labelLine8;

	private ComboBoxEditEx cbCurrency;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx xtraTabPage1;

	private MemoEditEx memoRemark;

	private LabelLine labelLine19;

	private TextEditEx txtBillLaddingNo;

	private LabelLine labelLine18;

	private ComboBoxEditEx cbTransportType;

	private LabelLine labelLine17;

	private CheckEditEx chkCusAffectInvent;

	private LabelLine labelLine15;

	private CalcEditEx calTotal;

	private LabelLine labelLine16;

	private CalcEditEx calTax2;

	private LabelLine labelLine13;

	private CalcEditEx calTax1;

	private LabelLine labelLine14;

	private CalcEditEx calBeforeTax;

	private LabelLine labelLine11;

	private CalcEditEx calAmountTBalance;

	private LabelLine labelLine12;

	private CalcEditEx calTotalFCharges;

	private LabelLine labelLine10;

	private CalcEditEx calActualCustoms;

	private LabelLine labelLine9;

	private CalcEditEx calcProjectedCustom;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx LineNum;

	private GridColumnEx TrgetEntry;

	private GridColumnEx DocEntry;

	private GridColumnEx ItemCode;

	private GridColumnEx Dscription;

	private GridColumnEx Quantity;

	private GridColumnEx PriceFOB;

	private GridColumnEx Currency;

	private RepositoryItemComboBox repositoryItemComboBox5;

	private GridColumnEx Rate;

	private GridColumnEx Custom;

	private RepositoryItemCalcEdit repositoryItemCalcEdit2;

	private GridColumnEx Cost;

	private RepositoryItemCalcEdit repositoryItemCalcEdit3;

	private GridColumnEx PriceAtWH;

	private GridColumnEx LineTotal;

	private GridColumnEx Volume;

	private RepositoryItemCalcEdit repositoryItemCalcEdit4;

	private GridColumnEx UnitCode;

	private RepositoryItemComboBox gridcomboUnitCode;

	private GridColumnEx Reference;

	private GridColumnEx OrigLine;

	private GridColumnEx FactNoCust;

	private GridColumnEx PriceList;

	private RepositoryItemComboBox repositoryItemComboBox3;

	private GridColumnEx CostOH;

	private GridColumnEx StockEval;

	private GridColumnEx UseBaseUn;

	private GridColumnEx BlockNum;

	private GridColumnEx ImportLog;

	private GridColumnEx WhsCode;

	private GridColumnEx OrigRow;

	private GridColumnEx Locked;

	private GridColumnEx OrigWhs;

	private GridColumnEx ReleaseNum;

	private GridColumnEx VarCosts;

	private GridColumnEx ConstCosts;

	private GridColumnEx UserCustom;

	private GridColumnEx FobValue;

	private GridColumnEx TtlExpndLC;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private GridColumnEx TtlCustLC;

	private RepositoryItemCalcEdit repositoryItemCalcEdit5;

	private GridColumnEx TtlCostLC;

	private RepositoryItemCalcEdit repositoryItemCalcEdit6;

	private GridColumnEx NumPerMsr;

	private XtraTabPageEx xtraTabPage2;

	private GridControlEx grdCost;

	private GridViewEx gvCost;

	private GridColumnEx AlcCode;

	private GridColumnEx OhType;

	private RepositoryItemComboBox repositoryItemComboBox1;

	private GridColumnEx CostSum;

	private RepositoryItemCalcEdit repositoryItemCalcEdit7;

	private GridColumnEx Factor;

	private RepositoryItemCalcEdit repositoryItemCalcEdit8;

	private GridColumnEx LaCAllcAcc;

	private RepositoryItemComboBox repositoryItemComboBox2;

	private XtraTabPageEx xtraTabPage3;

	private TextEditEx txtJournalRemark;

	private TextEditEx txtTransNo;

	private LabelLine labelLine20;

	private LabelLine labelLine21;

	private SimpleButtonEx btNewLandCost;

	private SimpleButtonEx btCostClear;

	private ButtonEditEx txtVendor;

	private ButtonEditEx txtBroker;

	private PopupMenu popupMenuCopyFrom;

	private BarManager barManager1;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private BarButtonItem barButtonItem1;

	private BarButtonItem barButtonItem2;

	private BarButtonItem barButtonItem3;

	private GridColumnEx AlcName;

	private CalcEditEx txtRate;

	private SimpleButtonEx btAdd;

	private SimpleButtonEx btCancel;

	private DropDownButtonEx btCopy;

	public frm_PurchaseLandedCost()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_PurchaseLandedCost"))
		{
			InitializeComponent();
			WinFuntions.SetFormatControlValue(this);
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_PurchaseLandedCost" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Return)
		{
			btAdd_Click(null, null);
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void ShowBtCombo()
	{
		if (FormMode == Enums.FormViewModeType.Find)
		{
			txtVendor.Properties.Buttons[0].Visible = false;
			txtBroker.Properties.Buttons[0].Visible = false;
		}
		else
		{
			txtVendor.Properties.Buttons[0].Visible = true;
			txtBroker.Properties.Buttons[0].Visible = true;
		}
	}

	private void memoRemark_TextChanged(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && isFinding)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void txtJournalRemark_TextChanged(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && isFinding)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btAdd.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btAdd.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btAdd.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btAdd.Text = Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Find:
			btAdd.Text = Resources.ButtonFind;
			break;
		}
	}

	public async Task MODE_FIND()
	{
		FormMode = Enums.FormViewModeType.Find;
		await LoadInitData(isGetNewDocEntry: false);
		txtVendor.ReadOnly = false;
		txtVendorDetail.ReadOnly = false;
		txtNumber.ReadOnly = false;
		txtNumber.Text = "";
	}

	public async Task REFRESH()
	{
		FormMode = Enums.FormViewModeType.Add;
		gvDoc.OptionsBehavior.Editable = true;
		gvCost.OptionsBehavior.Editable = true;
		ShowBtCombo();
		txtNumber.ReadOnly = true;
		await LoadInitData(isGetNewDocEntry: true);
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			if (currentEntry != firstEntry)
			{
				await GetFirst();
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
				await GetNext();
				currentEntry++;
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
				await GetLast();
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
				await GetPrevious();
				currentEntry--;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btCopy_Click_1(object sender, EventArgs e)
	{
	}

	private async void btAdd_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			Close();
		}
		if (FormMode == Enums.FormViewModeType.Find)
		{
			FormMode = Enums.FormViewModeType.Find;
		}
		if (FormMode == Enums.FormViewModeType.View)
		{
			Close();
		}
		else if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
		{
			if (!(await ExtenseionMethods.CheckExistsCarCodetxt(txtVendor.Text)))
			{
				WinCommons.Error(Enums.MessageCode.VendorDoesNotExistsChooseOtherVendor, "");
				txtVendor.Focus();
			}
			else if (checkTexbox())
			{
				SaveAll();
			}
		}
		else if (FormMode == Enums.FormViewModeType.Find)
		{
			DocumentsImpl documentsImpl = new DocumentsImpl();
			try
			{
				documentsImpl.DocEntry = Convert.ToInt32(txtNumber.Text.Trim());
			}
			catch
			{
				documentsImpl.DocEntry = -1;
			}
			try
			{
				documentsImpl.CardCode = txtVendor.Text.Trim();
			}
			catch
			{
				documentsImpl.CardCode = "";
			}
			try
			{
				documentsImpl.CardName = txtVendorDetail.Text.Trim();
			}
			catch
			{
				documentsImpl.CardName = "";
			}
			await Search(documentsImpl);
		}
	}

	private void btCostClear_Click(object sender, EventArgs e)
	{
	}

	private void btNewLandCost_Click(object sender, EventArgs e)
	{
		try
		{
			frm_PLandedCostAddNew frm_PLandedCostAddNew2 = new frm_PLandedCostAddNew();
			frm_PLandedCostAddNew2.MaximizeBox = false;
			frm_PLandedCostAddNew2.FormBorderStyle = FormBorderStyle.FixedSingle;
			frm_PLandedCostAddNew2.reloadParent = LoadWaitCost;
			frm_PLandedCostAddNew2.ShowDialog(this);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txtVendor_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Vendor, multiSelect: false, null);
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					BusinessPartnersImpl businessPartnersImpl = (BusinessPartnersImpl)frm_ChooseFromList2.ChooseItem;
					txtVendor.Text = businessPartnersImpl.CardCode;
					txtVendorDetail.Text = businessPartnersImpl.CardName;
					btCopy.Enabled = true;
					vendorCode = businessPartnersImpl.CardCode;
					string currency = businessPartnersImpl.Currency;
					if (currency == "##")
					{
						WinFuntions._selectItemComboBoxByValue(cbCurrency, GlobalData.MainCurrency);
					}
					else
					{
						WinFuntions._selectItemComboBoxByValue(cbCurrency, currency);
					}
				}
			}
			else
			{
				frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
				frm_BusinessMasterData2._copyF_CodeCustomer = txtVendor.Text;
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

	private void txtBroker_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Vendor, multiSelect: false, null);
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					BusinessPartnersImpl businessPartnersImpl = (BusinessPartnersImpl)frm_ChooseFromList2.ChooseItem;
					txtBroker.Text = businessPartnersImpl.CardCode;
					txtBrokerDetail.Text = businessPartnersImpl.CardName;
					brokerCode = businessPartnersImpl.CardCode;
					btCopy.Enabled = true;
				}
			}
			else
			{
				frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
				frm_BusinessMasterData2._copyF_CodeCustomer = txtBroker.Text;
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

	private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			string txtText = txtVendor.Text.Trim();
			frm_List_Document frm_List_Document2 = new frm_List_Document(ApzObjectTypes.oPurchaseDeliveryNotes, multiSelect: true, txtText);
			frm_List_Document2.ShowDialog(this);
			if (frm_List_Document2.ChooseItems == null || frm_List_Document2.ChooseItems.Count <= 0)
			{
				return;
			}
			string text = "";
			foreach (DocumentsImpl chooseItem in frm_List_Document2.ChooseItems)
			{
				DocumentsImpl documentsImpl = chooseItem;
				text = text + documentsImpl.DocEntry + ",";
				memoRemark.EditValue = "Based On Goods Receipt PO " + chooseItem.DocEntry;
			}
			text = text.Remove(text.Length - 1);
			LoadLandedCostItem("PDN1", text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			string txtText = txtVendor.Text.Trim();
			frm_List_Document frm_List_Document2 = new frm_List_Document(ApzObjectTypes.oPurchaseInvoices, multiSelect: true, txtText);
			frm_List_Document2.ShowDialog(this);
			if (frm_List_Document2.ChooseItems == null || frm_List_Document2.ChooseItems.Count <= 0)
			{
				return;
			}
			string text = "";
			foreach (DocumentsImpl chooseItem in frm_List_Document2.ChooseItems)
			{
				DocumentsImpl documentsImpl = chooseItem;
				text = text + documentsImpl.DocEntry + ",";
				memoRemark.EditValue = "Based On A/P Invoice " + chooseItem.DocEntry;
			}
			text = text.Remove(text.Length - 1);
			LoadLandedCostItem("PCH1", text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			string txtText = txtVendor.Text.Trim();
			frm_List_Document frm_List_Document2 = new frm_List_Document(ApzObjectTypes.oPurchaseLandedCost, multiSelect: true, txtText);
			frm_List_Document2.ShowDialog(this);
			if (frm_List_Document2.ChooseItems == null || frm_List_Document2.ChooseItems.Count <= 0)
			{
				return;
			}
			string text = "";
			foreach (DocumentsImpl chooseItem in frm_List_Document2.ChooseItems)
			{
				DocumentsImpl documentsImpl = chooseItem;
				text = text + documentsImpl.DocEntry + ",";
				memoRemark.EditValue = "Based On Landed Costs  " + chooseItem.DocEntry;
			}
			text = text.Remove(text.Length - 1);
			LoadLandedCostItem("IPF1", text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async Task getNewDocEntry()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LandedCost");
			newDocEntry = await _help.GetIntAsync("GetMaxDocEntry") + 1;
			txtNumber.Text = newDocEntry.ToString();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_PurchaseLandedCost_Load(object sender, EventArgs e)
	{
		isFinding = false;
		await LoadInitData(isGetNewDocEntry: true);
		foreach (GridColumn column in gvDoc.Columns)
		{
			column.MinWidth = 100;
		}
		await GetFirstDocEntry();
		await GetLastDocEntry();
		LoadUDF();
	}

	private async Task LoadInitData(bool isGetNewDocEntry)
	{
		_ = 2;
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			grdDoc.DataSource = null;
			grdDoc.RefreshDataSource();
			await LoadCurrency();
			LoadShippingType();
			await LoadPriceList();
			if (repositoryItemComboBox1.Items.Count == 0)
			{
				repositoryItemComboBox1.Items.AddRange(arrAllocation);
			}
			if (gridcomboUnitCode.Items.Count == 0)
			{
				gridcomboUnitCode.Items.AddRange(arrUnitVolume);
			}
			grdCost.DataSource = null;
			grdDoc.RefreshDataSource();
			LoadWaitCost();
			calActualCustoms.Text = "0";
			calAmountTBalance.Text = "0";
			calcProjectedCustom.Text = "0";
			calTotalFCharges.Text = "0";
			calTax1.Text = "0";
			calTax2.Text = "0";
			calBeforeTax.EditValue = "0";
			calTotal.Text = "0";
			datePostingDate.EditValue = DateTime.Today;
			dateDueDate.EditValue = DateTime.Today;
			txtVendor.Text = "";
			txtBroker.Text = "";
			txtVendorDetail.Text = "";
			txtBrokerDetail.Text = "";
			txtReference.Text = "";
			txtFileNo.Text = "";
			txtBillLaddingNo.Text = "";
			memoRemark.Text = "";
			cbTransportType.SelectedIndex = -1;
			if (isGetNewDocEntry)
			{
				await getNewDocEntry();
				txtVendor.ReadOnly = false;
				txtBroker.ReadOnly = false;
				txtVendorDetail.ReadOnly = false;
				txtBrokerDetail.ReadOnly = false;
				txtReference.ReadOnly = false;
				txtFileNo.ReadOnly = false;
				calActualCustoms.ReadOnly = false;
				calAmountTBalance.ReadOnly = false;
				calTax1.ReadOnly = false;
				calTax2.ReadOnly = false;
				dateDueDate.ReadOnly = false;
				datePostingDate.ReadOnly = false;
				txtBillLaddingNo.ReadOnly = false;
				memoRemark.ReadOnly = false;
				cbTransportType.ReadOnly = false;
				chkCusAffectInvent.ReadOnly = false;
				txtJournalRemark.EditValue = "Landed Costs " + newDocEntry;
			}
			else
			{
				txtVendor.ReadOnly = true;
				txtBroker.ReadOnly = true;
				txtVendorDetail.ReadOnly = true;
				txtBrokerDetail.ReadOnly = true;
				txtReference.ReadOnly = true;
				txtFileNo.ReadOnly = true;
				calActualCustoms.ReadOnly = true;
				calAmountTBalance.ReadOnly = true;
				calTax1.ReadOnly = true;
				calTax2.ReadOnly = true;
				dateDueDate.ReadOnly = true;
				datePostingDate.ReadOnly = true;
				txtBillLaddingNo.ReadOnly = true;
				memoRemark.ReadOnly = false;
				cbTransportType.ReadOnly = true;
				chkCusAffectInvent.ReadOnly = true;
			}
			cbSeries.Properties.Items.Add("Primary");
			cbSeries.SelectedItem = "Primary";
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadCurrency()
	{
		_ = 1;
		try
		{
			using HttpClientHelper<CurrencyCodesImpl> _help = new HttpClientHelper<CurrencyCodesImpl>(Configuration.ApiServer, "PaymentMeans");
			List<CurrencyCodesImpl> list = await _help.GetListAsync("ShowCurrency");
			List<string> list2 = new List<string>();
			List<Common.Table_Code> list3 = new List<Common.Table_Code>();
			foreach (CurrencyCodesImpl item in list)
			{
				Common.Table_Code table_Code = new Common.Table_Code();
				table_Code.Code = item.CurrCode;
				table_Code.Name = item.CurrCode;
				list2.Add(table_Code.Code);
				list3.Add(table_Code);
			}
			WinFuntions._addComboBox(cbCurrency, list3);
			ComboBoxItemCollection items = repositoryItemComboBox5.Items;
			object[] items2 = list2.ToArray();
			items.AddRange(items2);
			await getNewDocEntry();
			if (Function.ParseInt(txtNumber.EditValue) == newDocEntry)
			{
				WinFuntions._selectItemComboBoxByValue(cbCurrency, GlobalData.MainCurrency);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async void LoadShippingType()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ShippingTypes");
			DataTable dataTable = await _help.GetDataTableAsync("ShowShippingTypes");
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				Common.Table_Code table_Code = new Common.Table_Code();
				table_Code.Code = dataTable.Rows[i]["TrnspCode"].ToString();
				table_Code.Name = dataTable.Rows[i]["TrnspName"].ToString();
				lstShippingType.Add(table_Code);
			}
			WinFuntions._addComboBox(cbTransportType, lstShippingType, _selectFirstItem: false);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadWaitCost()
	{
		await LoadLandedCostCode();
	}

	private async Task LoadLandedCostCode()
	{
		try
		{
			using HttpClientHelper<Document_LinesImpl> _help = new HttpClientHelper<Document_LinesImpl>(Configuration.ApiServer, "LandedCost");
			List<Document_LinesImpl> list = await _help.GetListAsync("GetLandedCostCodeFormMain");
			if (list != null && list.Count > 0)
			{
				grdCost.DataSource = list;
				grdCost.RefreshDataSource();
			}
			else
			{
				list = new List<Document_LinesImpl>();
				grdCost.DataSource = list;
				grdCost.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadLandedCostItem(string tblLine, string codes)
	{
		try
		{
			using HttpClientHelper<Document_LinesImpl> _help = new HttpClientHelper<Document_LinesImpl>(Configuration.ApiServer, "LandedCost");
			List<Document_LinesImpl> list = await _help.GetListAsyncByPr("GetLandedCostItem", new { tblLine, codes });
			grdDoc.DataSource = list;
			grdDoc.RefreshDataSource();
			decimal textTotal = 0m;
			decimal num = 0m;
			decimal num2 = 0m;
			int num3 = 0;
			foreach (Document_LinesImpl item in list)
			{
				_ = item;
				try
				{
					num = decimal.Parse(gvDoc.GetRowCellValue(num3, TtlCostLC).ToString());
				}
				catch
				{
				}
				try
				{
					num2 = decimal.Parse(gvDoc.GetRowCellValue(num3, FobValue).ToString());
				}
				catch
				{
				}
				textTotal += num + num2;
				gvDoc.SetRowCellValue(num3, UnitCode, "1 \t\t - \t\t a");
				num3++;
			}
			if (FormMode == Enums.FormViewModeType.Add)
			{
				calBeforeTax.EditValue = textTotal.ToString();
				setTextTotal(textTotal);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadPriceList()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "PriceList");
			DataTable dataTable = await _help.GetDataTableAsync("ShowPriceList");
			List<string> list = new List<string>();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				list.Add(dataTable.Rows[i]["PriceListNo"].ToString());
			}
			ComboBoxItemCollection items = repositoryItemComboBox3.Items;
			object[] items2 = list.ToArray();
			items.AddRange(items2);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadForm(string docEntry)
	{
		FormMode = Enums.FormViewModeType.View;
		await LoadInitData(isGetNewDocEntry: false);
		await GetLandedCosts(docEntry);
	}

	private async Task GetLandedCosts(string docEntry)
	{
		_ = 2;
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LandedCost");
			DataTable dt = await _help.GetDataTableAsync("GetLandedCosts", new { docEntry });
			if (dt != null && dt.Rows.Count > 0)
			{
				FormMode = Enums.FormViewModeType.View;
				txtVendor.Text = dt.Rows[0]["CardCode"].ToString();
				txtVendorDetail.Text = dt.Rows[0]["SuppName"].ToString();
				txtBroker.Text = dt.Rows[0]["AgentCode"].ToString();
				txtBrokerDetail.Text = dt.Rows[0]["AgentName"].ToString();
				dateDueDate.Text = dt.Rows[0]["DocDueDate"].ToString();
				datePostingDate.Text = dt.Rows[0]["DocDate"].ToString();
				txtNumber.Text = dt.Rows[0]["DocEntry"].ToString();
				calActualCustoms.Text = dt.Rows[0]["ActCustom"].ToString();
				calcProjectedCustom.Text = dt.Rows[0]["ExpCustom"].ToString();
				calBeforeTax.EditValue = dt.Rows[0]["BeforeVat"].ToString();
				calTotal.Text = dt.Rows[0]["DocTotal"].ToString();
				calTax1.Text = dt.Rows[0]["vat1"].ToString();
				calTax2.Text = dt.Rows[0]["vat2"].ToString();
				calTotalFCharges.Text = dt.Rows[0]["CostSum"].ToString();
				txtBillLaddingNo.Text = dt.Rows[0]["BillOfLad"].ToString();
				chkCusAffectInvent.Checked = dt.Rows[0]["incCustom"].ToString() == "Y";
				calAmountTBalance.Text = dt.Rows[0]["Cost_Match"].ToString();
				memoRemark.Text = dt.Rows[0]["Descr"].ToString();
				cbTransportType.SelectedIndex = lstShippingType.FindIndex((Common.Table_Code x) => x.Code == dt.Rows[0]["TrnspCode"].ToString()) - 1;
				txtRate.EditValue = dt.Rows[0]["DocRate"].ToString();
				cbCurrency.EditValue = dt.Rows[0]["DocCur"].ToString();
				txtJournalRemark.EditValue = dt.Rows[0]["JdtMemo"].ToString();
				await GetLandedCostItemByDocEntry(docEntry);
				await LoadLandedCostlineByDocEntry(docEntry);
				LoadUDFData(docEntry);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetLandedCostItemByDocEntry(string docEntry)
	{
		try
		{
			using HttpClientHelper<Document_LinesImpl> _help = new HttpClientHelper<Document_LinesImpl>(Configuration.ApiServer, "LandedCost");
			List<Document_LinesImpl> list = await _help.GetListAsync("GetLandedCostItemByDocEntry", new { docEntry });
			if (list != null)
			{
				grdDoc.DataSource = list;
				grdDoc.RefreshDataSource();
			}
			gvCost.OptionsBehavior.Editable = false;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadLandedCostlineByDocEntry(string docEntry)
	{
		try
		{
			using HttpClientHelper<Document_LinesImpl> _help = new HttpClientHelper<Document_LinesImpl>(Configuration.ApiServer, "LandedCost");
			List<Document_LinesImpl> dataSource = await _help.GetListAsync("GetLandedCostLineByDocEntry", new { docEntry });
			grdCost.DataSource = dataSource;
			grdCost.RefreshDataSource();
			gvDoc.OptionsBehavior.Editable = false;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadUDF()
	{
	}

	private async void LoadUDFData(string docEntry)
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LandedCost");
			DataTable dataTable = await _help.GetDataTableAsync("LoadUDFData", new { docEntry });
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
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void txtVendor_TextChanged(object sender, EventArgs e)
	{
		if (txtVendor.Text.Trim() != vendorCode)
		{
			btCopy.Enabled = false;
		}
	}

	private void txtBroker_TextChanged(object sender, EventArgs e)
	{
		if (txtBroker.Text.Trim() != brokerCode)
		{
			btCopy.Enabled = false;
		}
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			switch (e.Column.Name)
			{
			case "TtlExpndLC":
			{
				decimal num9 = decimal.Parse(gvDoc.GetRowCellValue(e.RowHandle, TtlExpndLC).ToString());
				calcProjectedCustom.EditValue = num9;
				if (Convert.ToInt32(calActualCustoms.EditValue) <= 0)
				{
					calActualCustoms.EditValue = num9;
				}
				decimal num10 = decimal.Parse(gvDoc.GetRowCellValue(e.RowHandle, TtlCostLC).ToString());
				gvDoc.SetRowCellValue(e.RowHandle, TtlCostLC, num10 + num9);
				break;
			}
			case "Cost":
			{
				decimal num6 = decimal.Parse(gvDoc.GetRowCellValue(e.RowHandle, Cost).ToString());
				decimal num7 = decimal.Parse(gvDoc.GetRowCellValue(e.RowHandle, PriceAtWH).ToString());
				decimal num8 = decimal.Parse(gvDoc.GetRowCellValue(e.RowHandle, LineTotal).ToString());
				gvDoc.SetRowCellValue(e.RowHandle, Custom, num6);
				gvDoc.SetRowCellValue(e.RowHandle, PriceAtWH, num7 + num6);
				gvDoc.SetRowCellValue(e.RowHandle, LineTotal, num8 + num6);
				break;
			}
			case "Volume":
			{
				decimal num3 = decimal.Parse(gvDoc.GetRowCellValue(e.RowHandle, Volume).ToString());
				decimal num4 = decimal.Parse(gvDoc.GetRowCellValue(e.RowHandle, PriceAtWH).ToString());
				decimal num5 = decimal.Parse(gvDoc.GetRowCellValue(e.RowHandle, LineTotal).ToString());
				gvDoc.SetRowCellValue(e.RowHandle, PriceAtWH, num4 + num3);
				gvDoc.SetRowCellValue(e.RowHandle, LineTotal, num5 + num3);
				break;
			}
			case "TtlCustLC":
			{
				decimal num = decimal.Parse(gvDoc.GetRowCellValue(e.RowHandle, TtlCustLC).ToString());
				decimal num2 = decimal.Parse(gvDoc.GetRowCellValue(e.RowHandle, TtlCostLC).ToString());
				gvDoc.SetRowCellValue(e.RowHandle, TtlCostLC, num2 + num);
				break;
			}
			}
			decimal textTotal = 0m;
			decimal num11 = 0m;
			decimal num12 = 0m;
			int num13 = 0;
			foreach (Document_LinesImpl item in lstDocItem)
			{
				_ = item;
				try
				{
					num11 = decimal.Parse(gvDoc.GetRowCellValue(num13, TtlCostLC).ToString());
				}
				catch
				{
				}
				try
				{
					num12 = decimal.Parse(gvDoc.GetRowCellValue(num13, FobValue).ToString());
				}
				catch
				{
				}
				textTotal += num11 + num12;
				num13++;
			}
			if (FormMode != Enums.FormViewModeType.View)
			{
				calBeforeTax.EditValue = textTotal.ToString();
				setTextTotal(textTotal);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void setTextTotal(decimal total)
	{
		if (FormMode != Enums.FormViewModeType.View && total > 0m)
		{
			decimal num = 0m;
			decimal num2 = 0m;
			try
			{
				num = decimal.Parse(calTax1.EditValue.ToString());
			}
			catch
			{
			}
			try
			{
				num2 = decimal.Parse(calTax2.EditValue.ToString());
			}
			catch
			{
			}
			calTotal.EditValue = (total + num + num2).ToString();
		}
	}

	private void txtBroker_MouseDown(object sender, MouseEventArgs e)
	{
	}

	private void txtVendor_Leave(object sender, EventArgs e)
	{
		lastFocus = "";
	}

	private void gvCost_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (e.Column == CostSum)
		{
			SetTotalFreight();
		}
	}

	private void SetTotalFreight()
	{
		decimal num = 0m;
		for (int i = 0; i < gvCost.RowCount; i++)
		{
			num += decimal.Parse(gvCost.GetRowCellValue(i, "CostSum").ToString());
		}
		calTotalFCharges.EditValue = num.ToString();
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

	public void ShowForm(XtraFormEx frm)
	{
		WinCommons._OpenProcessing();
		frm.MdiParent = this;
		frm.Show();
		WinCommons._CloseProcessing();
	}

	private async void SaveAll()
	{
		_ = 7;
		try
		{
			DocumentsImpl _objLC = new DocumentsImpl();
			Document_LinesImpl _objLCGRPO = new Document_LinesImpl();
			DocumentsImpl _objLCIJ = new DocumentsImpl();
			WinCommons._OpenProcessing();
			_objLC.DocEntry = newDocEntry;
			_objLC.DocNum = newDocEntry;
			_objLC.DocDate = Function.ParseDateTimes(datePostingDate);
			_objLC.DocDueDate = Function.ParseDateTimes(dateDueDate);
			_objLC.CardCode = txtVendor.Text.Trim();
			_objLC.SuppName = txtVendorDetail.Text.Trim();
			_objLC.AgentCode = txtBroker.Text.Trim();
			_objLC.AgentName = txtBrokerDetail.Text.Trim();
			_objLC.DocStatus = "O";
			_objLC.AgentNum = txtFileNo.Text.Trim();
			_objLC.Descr = memoRemark.Text;
			_objLC.Ref1 = txtReference.Text.Trim();
			_objLC.Ref2 = null;
			_objLC.DocCur = cbCurrency.SelectedItem.ToString().Trim();
			try
			{
				_objLC.DocRate = (txtRate.Visible ? double.Parse(txtRate.Text) : 0.0);
			}
			catch
			{
				_objLC.DocRate = 1.0;
			}
			_objLC.ExpCustom = decimal.Parse(calcProjectedCustom.Text.Trim());
			_objLC.ActCustom = decimal.Parse(calActualCustoms.Text.Trim());
			_objLC.Vat1 = decimal.Parse(string.IsNullOrEmpty(calTax1.Text) ? "0" : calTax1.Text.Trim());
			_objLC.Vat2 = decimal.Parse(string.IsNullOrEmpty(calTax1.Text) ? "0" : calTax1.Text.Trim());
			_objLC.BeforeVat = decimal.Parse(calBeforeTax.Text);
			_objLC.DocTotal = double.Parse(calTotal.Text.Trim());
			_objLC.CloseDate = DateTime.Now.Date;
			_objLC.Cost_Match = decimal.Parse(calAmountTBalance.Text);
			_objLC.BillOfLad = txtBillLaddingNo.Text.Trim();
			try
			{
				_objLC.TrnspCode = Function.ParseInt(lstShippingType.Find((Common.Table_Code x) => x.Name == cbTransportType.SelectedItem.ToString()).Code);
			}
			catch (Exception)
			{
				_objLC.TrnspCode = -1;
			}
			_objLC.CostFactor = decimal.Parse(string.IsNullOrEmpty(calAmountTBalance.Text) ? "0" : calAmountTBalance.Text);
			_objLC.UpdateDate = DateTime.Now.Date;
			_objLC.CreateDate = DateTime.Now.Date;
			_objLC.TaxDate = DateTime.Now.Date;
			if (cbSeries.SelectedItem == null)
			{
				_objLC.Series = -1;
			}
			else
			{
				_objLC.Series = cbSeries.SelectedIndex;
			}
			_objLC.JdtNum = int.Parse(string.IsNullOrEmpty(txtTransNo.Text) ? "0" : txtTransNo.Text);
			_objLC.JdtMemo = txtJournalRemark.Text.Trim();
			_objLC.UserSign = 3;
			_objLC.ObjType = 69;
			_objLC.OpenForLaC = "N";
			_objLC.incCustom = (chkCusAffectInvent.Checked ? "Y" : "N");
			if (gvCost.RowCount > 0)
			{
				decimal costSum = 0m;
				for (int num = 0; num < gvCost.RowCount; num++)
				{
					costSum += (string.IsNullOrEmpty(gvCost.GetRowCellValue(num, "CostSum").ToString()) ? 0m : decimal.Parse(gvCost.GetRowCellValue(num, "CostSum").ToString()));
				}
				_objLC.CostSum = costSum;
			}
			else
			{
				_objLC.CostSum = 0m;
			}
			_objLC.ExCustomFC = _objLC.ExpCustom;
			_objLC.AcCustomFC = _objLC.ActCustom;
			_objLC.Vat1FC = _objLC.Vat1;
			_objLC.Vat2FC = _objLC.Vat2;
			_objLC.BeforVatFC = _objLC.BeforeVat;
			_objLC.DocTotalFC = decimal.Parse(_objLC.DocTotal.ToString());
			_objLC.CostSumFC = _objLC.CostSum;
			_objLC.C_Match_FC = _objLC.Cost_Match;
			_objLC.ExCustomSC = _objLC.ExpCustom;
			_objLC.ActCustSC = _objLC.ActCustom;
			_objLC.TtlCostSC = _objLC.CostSum;
			_objLC.BillOfLad = txtBillLaddingNo.Text;
			_objLC.Descr = memoRemark.Text;
			if (await saveLandedCost(_objLC) != "ERROR")
			{
				int lcLCCount = gvCost.RowCount;
				if (lcLCCount > 0)
				{
					for (int i = 0; i < lcLCCount; i++)
					{
						Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
						document_LinesImpl.DocEntry = newDocEntry;
						document_LinesImpl.AlcCode = gvCost.GetRowCellValue(i, "AlcCode").ToString();
						document_LinesImpl.OhType = ((gvCost.GetRowCellValue(i, "OhType").ToString().Length > 1) ? gvCost.GetRowCellValue(i, "OhType").ToString().Remove(1) : gvCost.GetRowCellValue(i, "OhType").ToString());
						document_LinesImpl.CostSum = decimal.Parse(gvCost.GetRowCellValue(i, "CostSum").ToString());
						document_LinesImpl.CostSumFC = document_LinesImpl.CostSum;
						try
						{
							document_LinesImpl.Factor = (string.IsNullOrEmpty(gvDoc.GetRowCellValue(i, "Factor").ToString()) ? 1m : decimal.Parse(gvDoc.GetRowCellValue(i, "Factor").ToString()));
						}
						catch
						{
							document_LinesImpl.Factor = 1m;
						}
						document_LinesImpl.CostType = "";
						document_LinesImpl.InCustCalc = "";
						document_LinesImpl.CostSumSC = document_LinesImpl.CostSum;
						document_LinesImpl.Locked = "N";
						document_LinesImpl.LaCAllcAcc = "N";
						await saveLandedCostLine(document_LinesImpl);
					}
				}
				int lcICount = gvDoc.RowCount;
				if (lcICount > 0)
				{
					for (int i = 0; i < lcICount; i++)
					{
						Document_LinesImpl document_LinesImpl2 = new Document_LinesImpl();
						document_LinesImpl2.DocEntry = newDocEntry;
						document_LinesImpl2.LineNum = i + 1;
						document_LinesImpl2.BaseType = 20;
						document_LinesImpl2.BaseEntry = int.Parse(gvDoc.GetRowCellValue(i, "DocEntry").ToString());
						document_LinesImpl2.ItemCode = gvDoc.GetRowCellValue(i, "ItemCode").ToString();
						document_LinesImpl2.Dscription = gvDoc.GetRowCellValue(i, "Dscription").ToString();
						document_LinesImpl2.Quantity = double.Parse(gvDoc.GetRowCellValue(i, "Quantity").ToString());
						document_LinesImpl2.PriceFOB = gvDoc.GetRowCellValue(i, "Price").ToString();
						document_LinesImpl2.Currency = gvDoc.GetRowCellValue(i, "Currency").ToString();
						document_LinesImpl2.Rate = double.Parse(gvDoc.GetRowCellValue(i, "Rate").ToString());
						document_LinesImpl2.Custom = decimal.Parse(gvDoc.GetRowCellValue(i, "Custom").ToString());
						document_LinesImpl2.CustomFC = document_LinesImpl2.Custom;
						document_LinesImpl2.Cost = decimal.Parse(gvDoc.GetRowCellValue(i, "Cost").ToString());
						document_LinesImpl2.CostFC = document_LinesImpl2.Cost;
						document_LinesImpl2.PriceAtWH = decimal.Parse(gvDoc.GetRowCellValue(i, "PriceAtWH").ToString());
						document_LinesImpl2.PricAtWHFC = document_LinesImpl2.PriceAtWH;
						document_LinesImpl2.LineTotal = double.Parse(gvDoc.GetRowCellValue(i, "LineTotal").ToString());
						document_LinesImpl2.TotalFrgn = decimal.Parse(document_LinesImpl2.LineTotal.ToString());
						document_LinesImpl2.Volume = double.Parse(gvDoc.GetRowCellValue(i, "Volume").ToString());
						document_LinesImpl2.UnitCode = Function.ParseInt(gvDoc.GetFocusedRowCellValue("UnitCode"));
						document_LinesImpl2.Weight1 = document_LinesImpl2.Weight1;
						document_LinesImpl2.UnitCode1 = document_LinesImpl2.UnitCode1;
						document_LinesImpl2.Weight2 = document_LinesImpl2.Weight2;
						document_LinesImpl2.UnitCode2 = document_LinesImpl2.UnitCode2;
						document_LinesImpl2.CardCode = txtVendor.Text.Trim();
						document_LinesImpl2.Reference = txtReference.Text.Trim();
						document_LinesImpl2.OrigLine = document_LinesImpl2.LineNum;
						document_LinesImpl2.FactNoCust = 0m;
						document_LinesImpl2.FacWthCust = 0m;
						document_LinesImpl2.PriceList = int.Parse(gvDoc.GetRowCellValue(i, "PriceList").ToString());
						document_LinesImpl2.CostOH = "Y";
						document_LinesImpl2.StockEval = "Y";
						document_LinesImpl2.UseBaseUn = gvDoc.GetRowCellValue(i, "UseBaseUn").ToString();
						document_LinesImpl2.BlockNum = "";
						document_LinesImpl2.ImportLog = "";
						document_LinesImpl2.WhsCode = gvDoc.GetRowCellValue(i, "WhsCode").ToString();
						document_LinesImpl2.OrigRow = -1;
						document_LinesImpl2.Locked = "N";
						document_LinesImpl2.OrigWhs = "";
						document_LinesImpl2.ReleaseNum = -1;
						document_LinesImpl2.VarCosts = 0m;
						document_LinesImpl2.ConstCosts = 0m;
						document_LinesImpl2.VarCostsFR = 0m;
						document_LinesImpl2.CnstCostFR = 0m;
						document_LinesImpl2.UserCustom = decimal.Parse(gvDoc.GetRowCellValue(i, "UserCustom").ToString());
						document_LinesImpl2.UsrCusomFC = document_LinesImpl2.UserCustom;
						document_LinesImpl2.FobValue = decimal.Parse(gvDoc.GetRowCellValue(i, "LineTotal").ToString());
						document_LinesImpl2.FobValueFC = document_LinesImpl2.FobValue;
						document_LinesImpl2.TtlExpndLC = decimal.Parse(document_LinesImpl2.Quantity.ToString()) * document_LinesImpl2.Cost;
						document_LinesImpl2.TtlExpndFC = 0m;
						document_LinesImpl2.TtlCustLC = 0m;
						document_LinesImpl2.TtlCustFC = 0m;
						document_LinesImpl2.TtlCostLC = 0m;
						document_LinesImpl2.TtlCostFC = 0m;
						document_LinesImpl2.TtlVolume = 0m;
						document_LinesImpl2.TtlWeight = 0m;
						document_LinesImpl2.BaseRowNum = 0;
						document_LinesImpl2.TtlCustSC = 0m;
						document_LinesImpl2.TtlExpndSC = 0m;
						document_LinesImpl2.GRPOAbsEnt = 0;
						document_LinesImpl2.GRPOLineNu = 0;
						document_LinesImpl2.TargetDoc = 0;
						document_LinesImpl2.FobValCurr = "";
						document_LinesImpl2.FobnLaC = 0m;
						document_LinesImpl2.FobnLaCFC = 0m;
						document_LinesImpl2.NumPerMsr = 0.0;
						document_LinesImpl2.CostSum = decimal.Parse(calTotalFCharges.Text);
						await saveLandedCostItemLine(document_LinesImpl2);
					}
				}
				if (lcICount > 0)
				{
					for (int i = 0; i < lcICount; i++)
					{
						Document_LinesImpl _objLCGRPOD = new Document_LinesImpl
						{
							DocEntry = int.Parse(gvDoc.GetRowCellValue(i, "DocEntry").ToString()),
							LineNum = int.Parse(gvDoc.GetRowCellValue(i, "LineNum").ToString()),
							TrgetEntry = int.Parse(gvDoc.GetRowCellValue(i, "TrgetEntry").ToString())
						};
						if (FormMode != Enums.FormViewModeType.Add)
						{
							return;
						}
						await saveLandedCostRecPODetail(_objLCGRPOD);
						_objLCGRPO.DocEntry = _objLCGRPOD.DocEntry;
						if (FormMode != Enums.FormViewModeType.Add)
						{
							return;
						}
						await saveLandedCostRecPO(_objLCGRPO);
					}
				}
				_objLCIJ.DocNum = _objLC.DocEntry;
				_objLCIJ.ObjType = 69;
				await saveLandedCostsp_InsertWhseJournal(_objLCIJ);
				await SaveToUDF();
			}
			WinCommons._CloseProcessing();
			await LoadInitData(isGetNewDocEntry: true);
		}
		catch (Exception ex2)
		{
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private bool checkTexbox()
	{
		try
		{
			if (string.IsNullOrEmpty(txtVendor.Text))
			{
				WinCommons.Error(Enums.MessageCode.YouDoNotChooseVendor, "");
				return false;
			}
			int num = 0;
			try
			{
				num = int.Parse(calcProjectedCustom.Text);
			}
			catch
			{
			}
			if (num < 0)
			{
				return false;
			}
			if (gvDoc.RowCount <= 0)
			{
				WinCommons.Error(Enums.MessageCode.None, "Bạn chưa copy from để lấy itemCode");
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return true;
		}
	}

	private async Task<string> saveLandedCost(DocumentsImpl doc)
	{
		string result = string.Empty;
		try
		{
			using HttpClientHelper<DocumentsImpl> _help = new HttpClientHelper<DocumentsImpl>(Configuration.ApiServer, "LandedCost");
			result = await _help.PostAsJsonAsync("SaveDoc", doc);
			if (string.IsNullOrEmpty(result))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, result);
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return result;
		}
	}

	private async Task saveLandedCostItemLine(Document_LinesImpl doc)
	{
		_ = string.Empty;
		try
		{
			using HttpClientHelper<Document_LinesImpl> _help = new HttpClientHelper<Document_LinesImpl>(Configuration.ApiServer, "LandedCost");
			string text = await _help.PostAsJsonAsync("SaveDocItemLine", doc);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task saveLandedCostLine(Document_LinesImpl doc)
	{
		_ = string.Empty;
		try
		{
			using HttpClientHelper<Document_LinesImpl> _help = new HttpClientHelper<Document_LinesImpl>(Configuration.ApiServer, "LandedCost");
			string text = await _help.PostAsJsonAsync("SaveDocCostLine", doc);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task saveLandedCostRecPODetail(Document_LinesImpl doc)
	{
		_ = string.Empty;
		try
		{
			using HttpClientHelper<Document_LinesImpl> _help = new HttpClientHelper<Document_LinesImpl>(Configuration.ApiServer, "LandedCost");
			string text = await _help.PostAsJsonAsync("SaveDocCostGRPODetail", doc);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task saveLandedCostRecPO(Document_LinesImpl doc)
	{
		_ = string.Empty;
		try
		{
			using HttpClientHelper<Document_LinesImpl> _help = new HttpClientHelper<Document_LinesImpl>(Configuration.ApiServer, "LandedCost");
			string text = await _help.PostAsJsonAsync("SaveDocCostGRPO", doc);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task saveLandedCostsp_InsertWhseJournal(DocumentsImpl doc)
	{
		_ = string.Empty;
		try
		{
			using HttpClientHelper<DocumentsImpl> _help = new HttpClientHelper<DocumentsImpl>(Configuration.ApiServer, "LandedCost");
			string text = await _help.PostAsJsonAsync("Savesp_InsertWhseJournal", doc);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveToUDF()
	{
	}

	private async Task Search(DocumentsImpl doc)
	{
		_ = 1;
		try
		{
			isFinding = true;
			DataTable dataTable = await _httpClientHelper.GetDataTableAsJsonAsync("Search", doc);
			if (dataTable != null && dataTable.Rows.Count == 1)
			{
				string docEntry = Function.ToString(dataTable.Rows[0]["DocEntry"]);
				await GetLandedCosts(docEntry);
				ShowBtCombo();
				FormMode = Enums.FormViewModeType.Update;
				txtNumber.ReadOnly = true;
			}
			else if (dataTable != null && dataTable.Rows.Count > 1)
			{
				frm_PLandedCostsList frm_PLandedCostsList2 = new frm_PLandedCostsList(doc);
				frm_PLandedCostsList2.reloadParentComboBox = reloadFromChild;
				frm_PLandedCostsList2.ShowDialog(this);
				ShowBtCombo();
				FormMode = Enums.FormViewModeType.Update;
				txtNumber.ReadOnly = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void reloadFromChild(string docEntry)
	{
		try
		{
			LoadForm(docEntry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetFirst()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LandedCost");
			DataTable dataTable = await _help.GetDataTableAsync("GetFirst", new
			{
				docEntry = txtNumber.Text
			});
			if (dataTable.Rows.Count > 0)
			{
				LoadForm(dataTable.Rows[0]["DocEntry"].ToString());
				firstEntry = int.Parse(dataTable.Rows[0]["DocEntry"].ToString());
				currentEntry = firstEntry;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async Task GetPrevious()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LandedCost");
			DataTable dataTable = await _help.GetDataTableAsync("GetPrevious", new
			{
				docEntry = txtNumber.Text
			});
			if (dataTable.Rows.Count > 0)
			{
				LoadForm(dataTable.Rows[0]["DocEntry"].ToString());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async Task GetNext()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LandedCost");
			DataTable dataTable = await _help.GetDataTableAsync("GetNext", new
			{
				docEntry = txtNumber.Text
			});
			if (dataTable.Rows.Count > 0)
			{
				LoadForm(dataTable.Rows[0]["DocEntry"].ToString());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async Task GetLast()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LandedCost");
			DataTable dataTable = await _help.GetDataTableAsync("GetLast");
			if (dataTable.Rows.Count > 0)
			{
				LoadForm(dataTable.Rows[0]["DocEntry"].ToString());
				lastEntry = int.Parse(dataTable.Rows[0]["DocEntry"].ToString());
				currentEntry = lastEntry;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async Task GetFirstDocEntry()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LandedCost");
			DataTable dataTable = await _help.GetDataTableAsync("GetFirst");
			if (dataTable.Rows.Count > 0)
			{
				firstEntry = int.Parse(dataTable.Rows[0]["DocEntry"].ToString());
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetLastDocEntry()
	{
		try
		{
			using HttpClientHelper<DataTable> _hlep = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LandedCost");
			DataTable dataTable = await _hlep.GetDataTableAsync("GetLast");
			if (dataTable.Rows.Count > 0)
			{
				lastEntry = int.Parse(dataTable.Rows[0]["DocEntry"].ToString());
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void cbCurrency_SelectedIndexChanged(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			string curr = cbCurrency.SelectedItem.ToString();
			DateTime date = Convert.ToDateTime(datePostingDate.EditValue);
			date.ToString("MM/dd/yyyy");
			decimal rateC = 0m;
			rateC = await GetExchangeRate(curr, date);
			txtRate.Value = rateC;
			if (rateC == 0m)
			{
				frm_ExchangeRatesAndIndexs frm = new frm_ExchangeRatesAndIndexs();
				frm.Curr = curr;
				frm._date = date;
				frm.FormClosing += (object s, FormClosingEventArgs e2) =>
				{
					rateC = (decimal)frm.rate;
					ExtenseionMethods.isBindingData = false;
				};
				frm.ShowDialog(this);
				rateC = await GetExchangeRate(curr, date);
				txtRate.Value = rateC;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<decimal> GetExchangeRate(string curr, DateTime date)
	{
		try
		{
			return (decimal)HttpClientHelperFactory.GetHttpClient().ExRateAndIndex.GetExchangeRate(curr, date);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 1m;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Purchase.frm_PurchaseLandedCost));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txtVendorDetail = new Apzon.Controls.Editor.TextEditEx();
		this.txtBrokerDetail = new Apzon.Controls.Editor.TextEditEx();
		this.txtFileNo = new Apzon.Controls.Editor.TextEditEx();
		this.txtReference = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txtNumber = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.datePostingDate = new Apzon.Controls.Editor.DateEditEx();
		this.dateDueDate = new Apzon.Controls.Editor.DateEditEx();
		this.cbSeries = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.cbCurrency = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.btAdd = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCopy = new Apzon.Controls.Controls.DropDownButtonEx();
		this.popupMenuCopyFrom = new DevExpress.XtraBars.PopupMenu(this.components);
		this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
		this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.memoRemark = new Apzon.Controls.Editor.MemoEditEx();
		this.labelLine19 = new Apzon.Controls.Controls.LabelLine();
		this.txtBillLaddingNo = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine18 = new Apzon.Controls.Controls.LabelLine();
		this.cbTransportType = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.labelLine17 = new Apzon.Controls.Controls.LabelLine();
		this.chkCusAffectInvent = new Apzon.Controls.Editor.CheckEditEx();
		this.labelLine15 = new Apzon.Controls.Controls.LabelLine();
		this.calTotal = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine16 = new Apzon.Controls.Controls.LabelLine();
		this.calTax2 = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine13 = new Apzon.Controls.Controls.LabelLine();
		this.calTax1 = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine14 = new Apzon.Controls.Controls.LabelLine();
		this.calBeforeTax = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		this.calAmountTBalance = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine12 = new Apzon.Controls.Controls.LabelLine();
		this.calTotalFCharges = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.calActualCustoms = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.calcProjectedCustom = new Apzon.Controls.Editor.CalcEditEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.LineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.TrgetEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.DocEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.ItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.Dscription = new Apzon.Controls.Grid.GridColumnEx();
		this.Quantity = new Apzon.Controls.Grid.GridColumnEx();
		this.PriceFOB = new Apzon.Controls.Grid.GridColumnEx();
		this.Currency = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemComboBox5 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.Rate = new Apzon.Controls.Grid.GridColumnEx();
		this.Custom = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.Cost = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.PriceAtWH = new Apzon.Controls.Grid.GridColumnEx();
		this.LineTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.Volume = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.UnitCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gridcomboUnitCode = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.Reference = new Apzon.Controls.Grid.GridColumnEx();
		this.OrigLine = new Apzon.Controls.Grid.GridColumnEx();
		this.FactNoCust = new Apzon.Controls.Grid.GridColumnEx();
		this.PriceList = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemComboBox3 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.CostOH = new Apzon.Controls.Grid.GridColumnEx();
		this.StockEval = new Apzon.Controls.Grid.GridColumnEx();
		this.UseBaseUn = new Apzon.Controls.Grid.GridColumnEx();
		this.BlockNum = new Apzon.Controls.Grid.GridColumnEx();
		this.ImportLog = new Apzon.Controls.Grid.GridColumnEx();
		this.WhsCode = new Apzon.Controls.Grid.GridColumnEx();
		this.OrigRow = new Apzon.Controls.Grid.GridColumnEx();
		this.Locked = new Apzon.Controls.Grid.GridColumnEx();
		this.OrigWhs = new Apzon.Controls.Grid.GridColumnEx();
		this.ReleaseNum = new Apzon.Controls.Grid.GridColumnEx();
		this.VarCosts = new Apzon.Controls.Grid.GridColumnEx();
		this.ConstCosts = new Apzon.Controls.Grid.GridColumnEx();
		this.UserCustom = new Apzon.Controls.Grid.GridColumnEx();
		this.FobValue = new Apzon.Controls.Grid.GridColumnEx();
		this.TtlExpndLC = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.TtlCustLC = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit5 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.TtlCostLC = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit6 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.NumPerMsr = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btNewLandCost = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCostClear = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdCost = new Apzon.Controls.Grid.GridControlEx();
		this.gvCost = new Apzon.Controls.Grid.GridViewEx();
		this.AlcCode = new Apzon.Controls.Grid.GridColumnEx();
		this.AlcName = new Apzon.Controls.Grid.GridColumnEx();
		this.OhType = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.CostSum = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit7 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.Factor = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit8 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.LaCAllcAcc = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemComboBox2 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.xtraTabPage3 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txtJournalRemark = new Apzon.Controls.Editor.TextEditEx();
		this.txtTransNo = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine20 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine21 = new Apzon.Controls.Controls.LabelLine();
		this.txtVendor = new Apzon.Controls.Editor.ButtonEditEx();
		this.txtBroker = new Apzon.Controls.Editor.ButtonEditEx();
		this.txtRate = new Apzon.Controls.Editor.CalcEditEx();
		((System.ComponentModel.ISupportInitialize)this.txtVendorDetail.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBrokerDetail.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFileNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtReference.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtNumber.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.datePostingDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.datePostingDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateDueDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateDueDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbSeries.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbCurrency.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.popupMenuCopyFrom).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.memoRemark.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBillLaddingNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbTransportType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkCusAffectInvent.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calTotal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calTax2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calTax1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calBeforeTax.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calAmountTBalance.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calTotalFCharges.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calActualCustoms.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcProjectedCustom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridcomboUnitCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit6).BeginInit();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdCost).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvCost).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox2).BeginInit();
		this.xtraTabPage3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtJournalRemark.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtTransNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtVendor.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBroker.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtRate.Properties).BeginInit();
		base.SuspendLayout();
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Vendor";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 6);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(87, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Vendor";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Broker";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(12, 31);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(87, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 1;
		this.labelLine2.Utext = "Broker";
		this.txtVendorDetail.EditControl = null;
		this.txtVendorDetail.FieldName = null;
		this.txtVendorDetail.IsSearchControl = false;
		this.txtVendorDetail.Location = new System.Drawing.Point(217, 3);
		this.txtVendorDetail.Name = "txtVendorDetail";
		this.txtVendorDetail.SettingInVisible = false;
		this.txtVendorDetail.SettingReadonly = false;
		this.txtVendorDetail.Size = new System.Drawing.Size(177, 20);
		this.txtVendorDetail.TabIndex = 6;
		this.txtBrokerDetail.EditControl = null;
		this.txtBrokerDetail.FieldName = null;
		this.txtBrokerDetail.IsSearchControl = false;
		this.txtBrokerDetail.Location = new System.Drawing.Point(217, 28);
		this.txtBrokerDetail.Name = "txtBrokerDetail";
		this.txtBrokerDetail.SettingInVisible = false;
		this.txtBrokerDetail.SettingReadonly = false;
		this.txtBrokerDetail.Size = new System.Drawing.Size(177, 20);
		this.txtBrokerDetail.TabIndex = 7;
		this.txtFileNo.EditControl = null;
		this.txtFileNo.FieldName = null;
		this.txtFileNo.IsSearchControl = false;
		this.txtFileNo.Location = new System.Drawing.Point(510, 119);
		this.txtFileNo.Name = "txtFileNo";
		this.txtFileNo.SettingInVisible = false;
		this.txtFileNo.SettingReadonly = false;
		this.txtFileNo.Size = new System.Drawing.Size(113, 20);
		this.txtFileNo.TabIndex = 11;
		this.txtReference.EditControl = null;
		this.txtReference.FieldName = null;
		this.txtReference.IsSearchControl = false;
		this.txtReference.Location = new System.Drawing.Point(510, 96);
		this.txtReference.Name = "txtReference";
		this.txtReference.SettingInVisible = false;
		this.txtReference.SettingReadonly = false;
		this.txtReference.Size = new System.Drawing.Size(113, 20);
		this.txtReference.TabIndex = 10;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "File No.";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(417, 121);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(95, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 9;
		this.labelLine3.Utext = "File No.";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Reference";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(417, 98);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(95, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 8;
		this.labelLine4.Utext = "Reference";
		this.txtNumber.EditControl = null;
		this.txtNumber.FieldName = null;
		this.txtNumber.IsSearchControl = false;
		this.txtNumber.Location = new System.Drawing.Point(510, 4);
		this.txtNumber.Name = "txtNumber";
		this.txtNumber.Properties.ReadOnly = true;
		this.txtNumber.SettingInVisible = false;
		this.txtNumber.SettingReadonly = false;
		this.txtNumber.Size = new System.Drawing.Size(113, 20);
		this.txtNumber.TabIndex = 13;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Number";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(417, 6);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(95, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 12;
		this.labelLine5.Utext = "Number";
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Due Date";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(417, 76);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(95, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 15;
		this.labelLine6.Utext = "Due Date";
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Posting Date";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(417, 53);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(95, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 14;
		this.labelLine7.Utext = "Posting Date";
		this.datePostingDate.EditControl = null;
		this.datePostingDate.EditValue = null;
		this.datePostingDate.FieldName = null;
		this.datePostingDate.Location = new System.Drawing.Point(510, 50);
		this.datePostingDate.Name = "datePostingDate";
		this.datePostingDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.datePostingDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.datePostingDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.datePostingDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.datePostingDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.datePostingDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.datePostingDate.Properties.IsSearchControl = false;
		this.datePostingDate.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.datePostingDate.SettingInVisible = false;
		this.datePostingDate.SettingReadonly = false;
		this.datePostingDate.Size = new System.Drawing.Size(113, 20);
		this.datePostingDate.TabIndex = 16;
		this.dateDueDate.EditControl = null;
		this.dateDueDate.EditValue = null;
		this.dateDueDate.FieldName = null;
		this.dateDueDate.Location = new System.Drawing.Point(510, 73);
		this.dateDueDate.Name = "dateDueDate";
		this.dateDueDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateDueDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateDueDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.dateDueDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.dateDueDate.Properties.IsSearchControl = false;
		this.dateDueDate.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.dateDueDate.SettingInVisible = false;
		this.dateDueDate.SettingReadonly = false;
		this.dateDueDate.Size = new System.Drawing.Size(113, 20);
		this.dateDueDate.TabIndex = 17;
		this.cbSeries.EditControl = null;
		this.cbSeries.FieldName = null;
		this.cbSeries.IsSearchControl = false;
		this.cbSeries.Location = new System.Drawing.Point(510, 27);
		this.cbSeries.Name = "cbSeries";
		this.cbSeries.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbSeries.Properties.ReadOnly = true;
		this.cbSeries.SettingInVisible = false;
		this.cbSeries.SettingReadonly = false;
		this.cbSeries.Size = new System.Drawing.Size(113, 20);
		this.cbSeries.TabIndex = 18;
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "Series";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(417, 29);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(95, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 19;
		this.labelLine8.Utext = "Series";
		this.cbCurrency.EditControl = null;
		this.cbCurrency.FieldName = null;
		this.cbCurrency.IsSearchControl = false;
		this.cbCurrency.Location = new System.Drawing.Point(12, 58);
		this.cbCurrency.Name = "cbCurrency";
		this.cbCurrency.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbCurrency.SettingInVisible = false;
		this.cbCurrency.SettingReadonly = false;
		this.cbCurrency.Size = new System.Drawing.Size(103, 20);
		this.cbCurrency.TabIndex = 20;
		this.cbCurrency.SelectedIndexChanged += new System.EventHandler(this.cbCurrency_SelectedIndexChanged);
		this.btAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btAdd.Appearance.BackColor = System.Drawing.Color.FromArgb(247, 215, 107);
		this.btAdd.Appearance.Options.UseBackColor = true;
		this.btAdd.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Add;
		this.btAdd.EditControl = null;
		this.btAdd.EditValue = "Add";
		this.btAdd.FieldName = null;
		this.btAdd.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btAdd.ImageOptions.Image");
		this.btAdd.IsSearchControl = false;
		this.btAdd.Location = new System.Drawing.Point(3, 525);
		this.btAdd.Name = "btAdd";
		this.btAdd.ReadOnly = false;
		this.btAdd.SettingInVisible = false;
		this.btAdd.SettingReadonly = false;
		this.btAdd.Size = new System.Drawing.Size(75, 23);
		this.btAdd.TabIndex = 45;
		this.btAdd.Text = "Add";
		this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(247, 215, 107);
		this.btCancel.Appearance.Options.UseBackColor = true;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(82, 525);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 46;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btCopy.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCopy.DropDownControl = this.popupMenuCopyFrom;
		this.btCopy.EditControl = null;
		this.btCopy.EditValue = "Copy From";
		this.btCopy.Enabled = false;
		this.btCopy.FieldName = null;
		this.btCopy.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCopy.ImageOptions.Image");
		this.btCopy.IsSearchControl = false;
		this.btCopy.ItemId = null;
		this.btCopy.Location = new System.Drawing.Point(516, 525);
		this.btCopy.Name = "btCopy";
		this.btCopy.ReadOnly = false;
		this.btCopy.SettingInVisible = false;
		this.btCopy.SettingReadonly = false;
		this.btCopy.Size = new System.Drawing.Size(107, 23);
		this.btCopy.TabIndex = 48;
		this.btCopy.Text = "Copy From";
		this.btCopy.Click += new System.EventHandler(this.btCopy_Click_1);
		this.popupMenuCopyFrom.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem2),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem3)
		});
		this.popupMenuCopyFrom.Manager = this.barManager1;
		this.popupMenuCopyFrom.Name = "popupMenuCopyFrom";
		this.barButtonItem1.Caption = "Good Receipt PO";
		this.barButtonItem1.Id = 0;
		this.barButtonItem1.Name = "barButtonItem1";
		this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
		this.barButtonItem2.Caption = "A/P Invoice";
		this.barButtonItem2.Id = 1;
		this.barButtonItem2.Name = "barButtonItem2";
		this.barButtonItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem2_ItemClick);
		this.barButtonItem3.Caption = "Landed Cost";
		this.barButtonItem3.Id = 2;
		this.barButtonItem3.Name = "barButtonItem3";
		this.barButtonItem3.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem3_ItemClick);
		this.barManager1.DockControls.Add(this.barDockControlTop);
		this.barManager1.DockControls.Add(this.barDockControlBottom);
		this.barManager1.DockControls.Add(this.barDockControlLeft);
		this.barManager1.DockControls.Add(this.barDockControlRight);
		this.barManager1.Form = this;
		this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[3] { this.barButtonItem1, this.barButtonItem2, this.barButtonItem3 });
		this.barManager1.MaxItemId = 3;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager1;
		this.barDockControlTop.Size = new System.Drawing.Size(625, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 551);
		this.barDockControlBottom.Manager = this.barManager1;
		this.barDockControlBottom.Size = new System.Drawing.Size(625, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager1;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 551);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(625, 0);
		this.barDockControlRight.Manager = this.barManager1;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 551);
		this.xtraTabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControl1.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
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
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(3, 137);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.PaintStyleName = "Standard";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl1.Size = new System.Drawing.Size(620, 382);
		this.xtraTabControl1.TabIndex = 54;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.xtraTabPage1, this.xtraTabPage2, this.xtraTabPage3 });
		this.xtraTabPage1.Appearance.PageClient.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.xtraTabPage1.Appearance.PageClient.Options.UseBackColor = true;
		this.xtraTabPage1.Controls.Add(this.memoRemark);
		this.xtraTabPage1.Controls.Add(this.labelLine19);
		this.xtraTabPage1.Controls.Add(this.txtBillLaddingNo);
		this.xtraTabPage1.Controls.Add(this.labelLine18);
		this.xtraTabPage1.Controls.Add(this.cbTransportType);
		this.xtraTabPage1.Controls.Add(this.labelLine17);
		this.xtraTabPage1.Controls.Add(this.chkCusAffectInvent);
		this.xtraTabPage1.Controls.Add(this.labelLine15);
		this.xtraTabPage1.Controls.Add(this.calTotal);
		this.xtraTabPage1.Controls.Add(this.labelLine16);
		this.xtraTabPage1.Controls.Add(this.calTax2);
		this.xtraTabPage1.Controls.Add(this.labelLine13);
		this.xtraTabPage1.Controls.Add(this.calTax1);
		this.xtraTabPage1.Controls.Add(this.labelLine14);
		this.xtraTabPage1.Controls.Add(this.calBeforeTax);
		this.xtraTabPage1.Controls.Add(this.labelLine11);
		this.xtraTabPage1.Controls.Add(this.calAmountTBalance);
		this.xtraTabPage1.Controls.Add(this.labelLine12);
		this.xtraTabPage1.Controls.Add(this.calTotalFCharges);
		this.xtraTabPage1.Controls.Add(this.labelLine10);
		this.xtraTabPage1.Controls.Add(this.calActualCustoms);
		this.xtraTabPage1.Controls.Add(this.labelLine9);
		this.xtraTabPage1.Controls.Add(this.calcProjectedCustom);
		this.xtraTabPage1.Controls.Add(this.grdDoc);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Item";
		this.xtraTabPage1.FieldName = null;
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(616, 358);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Item";
		this.memoRemark.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.memoRemark.EditControl = null;
		this.memoRemark.FieldName = null;
		this.memoRemark.IsSearchControl = false;
		this.memoRemark.Location = new System.Drawing.Point(113, 315);
		this.memoRemark.Name = "memoRemark";
		this.memoRemark.SettingInVisible = false;
		this.memoRemark.SettingReadonly = false;
		this.memoRemark.Size = new System.Drawing.Size(212, 35);
		this.memoRemark.TabIndex = 90;
		this.memoRemark.TextChanged += new System.EventHandler(this.memoRemark_TextChanged);
		this.labelLine19.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine19.EditControl = null;
		this.labelLine19.EditValue = "Remark";
		this.labelLine19.FieldName = null;
		this.labelLine19.HideWhenEmpty = false;
		this.labelLine19.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine19.Location = new System.Drawing.Point(9, 326);
		this.labelLine19.Name = "labelLine19";
		this.labelLine19.ReadOnly = false;
		this.labelLine19.SettingInVisible = false;
		this.labelLine19.SettingReadonly = false;
		this.labelLine19.ShowUnderLine = true;
		this.labelLine19.Size = new System.Drawing.Size(101, 20);
		this.labelLine19.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine19.TabIndex = 89;
		this.labelLine19.Utext = "Remark";
		this.txtBillLaddingNo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txtBillLaddingNo.EditControl = null;
		this.txtBillLaddingNo.FieldName = null;
		this.txtBillLaddingNo.IsSearchControl = false;
		this.txtBillLaddingNo.Location = new System.Drawing.Point(113, 270);
		this.txtBillLaddingNo.Name = "txtBillLaddingNo";
		this.txtBillLaddingNo.SettingInVisible = false;
		this.txtBillLaddingNo.SettingReadonly = false;
		this.txtBillLaddingNo.Size = new System.Drawing.Size(212, 20);
		this.txtBillLaddingNo.TabIndex = 88;
		this.labelLine18.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine18.EditControl = null;
		this.labelLine18.EditValue = "Bill of Ladding No.";
		this.labelLine18.FieldName = null;
		this.labelLine18.HideWhenEmpty = false;
		this.labelLine18.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine18.Location = new System.Drawing.Point(9, 273);
		this.labelLine18.Name = "labelLine18";
		this.labelLine18.ReadOnly = false;
		this.labelLine18.SettingInVisible = false;
		this.labelLine18.SettingReadonly = false;
		this.labelLine18.ShowUnderLine = true;
		this.labelLine18.Size = new System.Drawing.Size(101, 20);
		this.labelLine18.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine18.TabIndex = 87;
		this.labelLine18.Utext = "Bill of Ladding No.";
		this.cbTransportType.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.cbTransportType.EditControl = null;
		this.cbTransportType.FieldName = null;
		this.cbTransportType.IsSearchControl = false;
		this.cbTransportType.Location = new System.Drawing.Point(113, 292);
		this.cbTransportType.Name = "cbTransportType";
		this.cbTransportType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbTransportType.SettingInVisible = false;
		this.cbTransportType.SettingReadonly = false;
		this.cbTransportType.Size = new System.Drawing.Size(212, 20);
		this.cbTransportType.TabIndex = 86;
		this.labelLine17.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine17.EditControl = null;
		this.labelLine17.EditValue = "Transport Type";
		this.labelLine17.FieldName = null;
		this.labelLine17.HideWhenEmpty = false;
		this.labelLine17.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine17.Location = new System.Drawing.Point(9, 293);
		this.labelLine17.Name = "labelLine17";
		this.labelLine17.ReadOnly = false;
		this.labelLine17.SettingInVisible = false;
		this.labelLine17.SettingReadonly = false;
		this.labelLine17.ShowUnderLine = true;
		this.labelLine17.Size = new System.Drawing.Size(101, 20);
		this.labelLine17.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine17.TabIndex = 85;
		this.labelLine17.Utext = "Transport Type";
		this.chkCusAffectInvent.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.chkCusAffectInvent.EditControl = null;
		this.chkCusAffectInvent.FieldName = null;
		this.chkCusAffectInvent.IsSearchControl = false;
		this.chkCusAffectInvent.Location = new System.Drawing.Point(9, 246);
		this.chkCusAffectInvent.Name = "chkCusAffectInvent";
		this.chkCusAffectInvent.Properties.Caption = "Customs Affect Inventory";
		this.chkCusAffectInvent.SettingInVisible = false;
		this.chkCusAffectInvent.SettingReadonly = false;
		this.chkCusAffectInvent.Size = new System.Drawing.Size(149, 20);
		this.chkCusAffectInvent.TabIndex = 84;
		this.labelLine15.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine15.EditControl = null;
		this.labelLine15.EditValue = "Total";
		this.labelLine15.FieldName = null;
		this.labelLine15.HideWhenEmpty = false;
		this.labelLine15.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine15.Location = new System.Drawing.Point(435, 267);
		this.labelLine15.Name = "labelLine15";
		this.labelLine15.ReadOnly = false;
		this.labelLine15.SettingInVisible = false;
		this.labelLine15.SettingReadonly = false;
		this.labelLine15.ShowUnderLine = true;
		this.labelLine15.Size = new System.Drawing.Size(70, 20);
		this.labelLine15.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine15.TabIndex = 83;
		this.labelLine15.Utext = "Total";
		this.calTotal.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.calTotal.EditControl = null;
		this.calTotal.FieldName = null;
		this.calTotal.IsSearchControl = false;
		this.calTotal.Location = new System.Drawing.Point(505, 263);
		this.calTotal.Name = "calTotal";
		this.calTotal.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calTotal.Properties.DisplayFormat.FormatString = "#.00";
		this.calTotal.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calTotal.Properties.IsSearchControl = false;
		this.calTotal.Properties.ReadOnly = true;
		this.calTotal.SettingInVisible = false;
		this.calTotal.SettingReadonly = false;
		this.calTotal.Size = new System.Drawing.Size(103, 20);
		this.calTotal.TabIndex = 82;
		this.labelLine16.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine16.EditControl = null;
		this.labelLine16.EditValue = "Tax 2";
		this.labelLine16.FieldName = null;
		this.labelLine16.HideWhenEmpty = false;
		this.labelLine16.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine16.Location = new System.Drawing.Point(435, 245);
		this.labelLine16.Name = "labelLine16";
		this.labelLine16.ReadOnly = false;
		this.labelLine16.SettingInVisible = false;
		this.labelLine16.SettingReadonly = false;
		this.labelLine16.ShowUnderLine = true;
		this.labelLine16.Size = new System.Drawing.Size(70, 20);
		this.labelLine16.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine16.TabIndex = 81;
		this.labelLine16.Utext = "Tax 2";
		this.calTax2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.calTax2.EditControl = null;
		this.calTax2.FieldName = null;
		this.calTax2.IsSearchControl = false;
		this.calTax2.Location = new System.Drawing.Point(505, 241);
		this.calTax2.Name = "calTax2";
		this.calTax2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calTax2.Properties.DisplayFormat.FormatString = "#.00";
		this.calTax2.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calTax2.Properties.IsSearchControl = false;
		this.calTax2.SettingInVisible = false;
		this.calTax2.SettingReadonly = false;
		this.calTax2.Size = new System.Drawing.Size(103, 20);
		this.calTax2.TabIndex = 80;
		this.labelLine13.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine13.EditControl = null;
		this.labelLine13.EditValue = "Tax 1";
		this.labelLine13.FieldName = null;
		this.labelLine13.HideWhenEmpty = false;
		this.labelLine13.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine13.Location = new System.Drawing.Point(435, 223);
		this.labelLine13.Name = "labelLine13";
		this.labelLine13.ReadOnly = false;
		this.labelLine13.SettingInVisible = false;
		this.labelLine13.SettingReadonly = false;
		this.labelLine13.ShowUnderLine = true;
		this.labelLine13.Size = new System.Drawing.Size(70, 20);
		this.labelLine13.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine13.TabIndex = 79;
		this.labelLine13.Utext = "Tax 1";
		this.calTax1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.calTax1.EditControl = null;
		this.calTax1.FieldName = null;
		this.calTax1.IsSearchControl = false;
		this.calTax1.Location = new System.Drawing.Point(505, 219);
		this.calTax1.Name = "calTax1";
		this.calTax1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calTax1.Properties.DisplayFormat.FormatString = "#.00";
		this.calTax1.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calTax1.Properties.IsSearchControl = false;
		this.calTax1.SettingInVisible = false;
		this.calTax1.SettingReadonly = false;
		this.calTax1.Size = new System.Drawing.Size(103, 20);
		this.calTax1.TabIndex = 78;
		this.labelLine14.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine14.EditControl = null;
		this.labelLine14.EditValue = "Before Tax";
		this.labelLine14.FieldName = null;
		this.labelLine14.HideWhenEmpty = false;
		this.labelLine14.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine14.Location = new System.Drawing.Point(435, 201);
		this.labelLine14.Name = "labelLine14";
		this.labelLine14.ReadOnly = false;
		this.labelLine14.SettingInVisible = false;
		this.labelLine14.SettingReadonly = false;
		this.labelLine14.ShowUnderLine = true;
		this.labelLine14.Size = new System.Drawing.Size(70, 20);
		this.labelLine14.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine14.TabIndex = 77;
		this.labelLine14.Utext = "Before Tax";
		this.calBeforeTax.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.calBeforeTax.EditControl = null;
		this.calBeforeTax.FieldName = null;
		this.calBeforeTax.IsSearchControl = false;
		this.calBeforeTax.Location = new System.Drawing.Point(505, 197);
		this.calBeforeTax.Name = "calBeforeTax";
		this.calBeforeTax.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calBeforeTax.Properties.DisplayFormat.FormatString = "#.00";
		this.calBeforeTax.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calBeforeTax.Properties.IsSearchControl = false;
		this.calBeforeTax.Properties.ReadOnly = true;
		this.calBeforeTax.SettingInVisible = false;
		this.calBeforeTax.SettingReadonly = false;
		this.calBeforeTax.Size = new System.Drawing.Size(103, 20);
		this.calBeforeTax.TabIndex = 76;
		this.labelLine11.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine11.EditControl = null;
		this.labelLine11.EditValue = "Amount To Balance";
		this.labelLine11.FieldName = null;
		this.labelLine11.HideWhenEmpty = false;
		this.labelLine11.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine11.Location = new System.Drawing.Point(209, 222);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.ReadOnly = false;
		this.labelLine11.SettingInVisible = false;
		this.labelLine11.SettingReadonly = false;
		this.labelLine11.ShowUnderLine = true;
		this.labelLine11.Size = new System.Drawing.Size(126, 20);
		this.labelLine11.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine11.TabIndex = 75;
		this.labelLine11.Utext = "Amount To Balance";
		this.calAmountTBalance.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.calAmountTBalance.EditControl = null;
		this.calAmountTBalance.FieldName = null;
		this.calAmountTBalance.IsSearchControl = false;
		this.calAmountTBalance.Location = new System.Drawing.Point(335, 219);
		this.calAmountTBalance.Name = "calAmountTBalance";
		this.calAmountTBalance.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calAmountTBalance.Properties.DisplayFormat.FormatString = "#.00";
		this.calAmountTBalance.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calAmountTBalance.Properties.IsSearchControl = false;
		this.calAmountTBalance.Properties.ReadOnly = true;
		this.calAmountTBalance.SettingInVisible = false;
		this.calAmountTBalance.SettingReadonly = false;
		this.calAmountTBalance.Size = new System.Drawing.Size(90, 20);
		this.calAmountTBalance.TabIndex = 74;
		this.labelLine12.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine12.EditControl = null;
		this.labelLine12.EditValue = "Total Freight Charges";
		this.labelLine12.FieldName = null;
		this.labelLine12.HideWhenEmpty = false;
		this.labelLine12.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine12.Location = new System.Drawing.Point(209, 200);
		this.labelLine12.Name = "labelLine12";
		this.labelLine12.ReadOnly = false;
		this.labelLine12.SettingInVisible = false;
		this.labelLine12.SettingReadonly = false;
		this.labelLine12.ShowUnderLine = true;
		this.labelLine12.Size = new System.Drawing.Size(126, 20);
		this.labelLine12.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine12.TabIndex = 73;
		this.labelLine12.Utext = "Total Freight Charges";
		this.calTotalFCharges.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.calTotalFCharges.EditControl = null;
		this.calTotalFCharges.FieldName = null;
		this.calTotalFCharges.IsSearchControl = false;
		this.calTotalFCharges.Location = new System.Drawing.Point(335, 197);
		this.calTotalFCharges.Name = "calTotalFCharges";
		this.calTotalFCharges.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calTotalFCharges.Properties.DisplayFormat.FormatString = "0.00";
		this.calTotalFCharges.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calTotalFCharges.Properties.EditFormat.FormatString = "0.00";
		this.calTotalFCharges.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calTotalFCharges.Properties.IsSearchControl = false;
		this.calTotalFCharges.Properties.ReadOnly = true;
		this.calTotalFCharges.SettingInVisible = false;
		this.calTotalFCharges.SettingReadonly = false;
		this.calTotalFCharges.Size = new System.Drawing.Size(90, 20);
		this.calTotalFCharges.TabIndex = 72;
		this.labelLine10.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine10.EditControl = null;
		this.labelLine10.EditValue = "Actual Customs";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(9, 221);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ReadOnly = false;
		this.labelLine10.SettingInVisible = false;
		this.labelLine10.SettingReadonly = false;
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(103, 20);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 71;
		this.labelLine10.Utext = "Actual Customs";
		this.calActualCustoms.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.calActualCustoms.EditControl = null;
		this.calActualCustoms.FieldName = null;
		this.calActualCustoms.IsSearchControl = false;
		this.calActualCustoms.Location = new System.Drawing.Point(112, 219);
		this.calActualCustoms.Name = "calActualCustoms";
		this.calActualCustoms.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calActualCustoms.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calActualCustoms.Properties.IsSearchControl = false;
		this.calActualCustoms.SettingInVisible = false;
		this.calActualCustoms.SettingReadonly = false;
		this.calActualCustoms.Size = new System.Drawing.Size(90, 20);
		this.calActualCustoms.TabIndex = 70;
		this.labelLine9.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine9.EditControl = null;
		this.labelLine9.EditValue = "Projected Customs";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(9, 200);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ReadOnly = false;
		this.labelLine9.SettingInVisible = false;
		this.labelLine9.SettingReadonly = false;
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(103, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 69;
		this.labelLine9.Utext = "Projected Customs";
		this.calcProjectedCustom.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.calcProjectedCustom.EditControl = null;
		this.calcProjectedCustom.FieldName = null;
		this.calcProjectedCustom.IsSearchControl = false;
		this.calcProjectedCustom.Location = new System.Drawing.Point(113, 197);
		this.calcProjectedCustom.Name = "calcProjectedCustom";
		this.calcProjectedCustom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calcProjectedCustom.Properties.DisplayFormat.FormatString = "#.00";
		this.calcProjectedCustom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calcProjectedCustom.Properties.IsSearchControl = false;
		this.calcProjectedCustom.Properties.ReadOnly = true;
		this.calcProjectedCustom.SettingInVisible = false;
		this.calcProjectedCustom.SettingReadonly = false;
		this.calcProjectedCustom.Size = new System.Drawing.Size(90, 20);
		this.calcProjectedCustom.TabIndex = 68;
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Location = new System.Drawing.Point(3, 3);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[9] { this.repositoryItemComboBox3, this.gridcomboUnitCode, this.repositoryItemComboBox5, this.repositoryItemCalcEdit1, this.repositoryItemCalcEdit2, this.repositoryItemCalcEdit3, this.repositoryItemCalcEdit4, this.repositoryItemCalcEdit5, this.repositoryItemCalcEdit6 });
		this.grdDoc.Size = new System.Drawing.Size(607, 192);
		this.grdDoc.TabIndex = 2;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[37]
		{
			this.LineNum, this.TrgetEntry, this.DocEntry, this.ItemCode, this.Dscription, this.Quantity, this.PriceFOB, this.Currency, this.Rate, this.Custom,
			this.Cost, this.PriceAtWH, this.LineTotal, this.Volume, this.UnitCode, this.Reference, this.OrigLine, this.FactNoCust, this.PriceList, this.CostOH,
			this.StockEval, this.UseBaseUn, this.BlockNum, this.ImportLog, this.WhsCode, this.OrigRow, this.Locked, this.OrigWhs, this.ReleaseNum, this.VarCosts,
			this.ConstCosts, this.UserCustom, this.FobValue, this.TtlExpndLC, this.TtlCustLC, this.TtlCostLC, this.NumPerMsr
		});
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
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowDetailButtons = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.LineNum.Caption = "GridColumnEx1";
		this.LineNum.CurrencyDateField = null;
		this.LineNum.CurrencyField = null;
		this.LineNum.CurrencyFixed = null;
		this.LineNum.CurrencyFixedRate = 0.0;
		this.LineNum.CurrencyRateField = null;
		this.LineNum.FieldName = "LineNum";
		this.LineNum.MinWidth = 100;
		this.LineNum.Name = "LineNum";
		this.LineNum.PrimaryKey = false;
		this.LineNum.UnitField = null;
		this.TrgetEntry.Caption = "TrgetEntry";
		this.TrgetEntry.CurrencyDateField = null;
		this.TrgetEntry.CurrencyField = null;
		this.TrgetEntry.CurrencyFixed = null;
		this.TrgetEntry.CurrencyFixedRate = 0.0;
		this.TrgetEntry.CurrencyRateField = null;
		this.TrgetEntry.FieldName = "TrgetEntry";
		this.TrgetEntry.MinWidth = 100;
		this.TrgetEntry.Name = "TrgetEntry";
		this.TrgetEntry.PrimaryKey = false;
		this.TrgetEntry.UnitField = null;
		this.DocEntry.Caption = "DocEntry";
		this.DocEntry.CurrencyDateField = null;
		this.DocEntry.CurrencyField = null;
		this.DocEntry.CurrencyFixed = null;
		this.DocEntry.CurrencyFixedRate = 0.0;
		this.DocEntry.CurrencyRateField = null;
		this.DocEntry.FieldName = "DocEntry";
		this.DocEntry.MinWidth = 100;
		this.DocEntry.Name = "DocEntry";
		this.DocEntry.OptionsColumn.AllowEdit = false;
		this.DocEntry.OptionsColumn.ReadOnly = true;
		this.DocEntry.PrimaryKey = false;
		this.DocEntry.UnitField = null;
		this.ItemCode.Caption = "Item Code";
		this.ItemCode.CurrencyDateField = null;
		this.ItemCode.CurrencyField = null;
		this.ItemCode.CurrencyFixed = null;
		this.ItemCode.CurrencyFixedRate = 0.0;
		this.ItemCode.CurrencyRateField = null;
		this.ItemCode.FieldName = "ItemCode";
		this.ItemCode.MinWidth = 100;
		this.ItemCode.Name = "ItemCode";
		this.ItemCode.OptionsColumn.AllowEdit = false;
		this.ItemCode.OptionsColumn.ReadOnly = true;
		this.ItemCode.PrimaryKey = false;
		this.ItemCode.UnitField = null;
		this.ItemCode.Visible = true;
		this.ItemCode.VisibleIndex = 0;
		this.ItemCode.Width = 126;
		this.Dscription.Caption = "Description";
		this.Dscription.CurrencyDateField = null;
		this.Dscription.CurrencyField = null;
		this.Dscription.CurrencyFixed = null;
		this.Dscription.CurrencyFixedRate = 0.0;
		this.Dscription.CurrencyRateField = null;
		this.Dscription.FieldName = "Dscription";
		this.Dscription.MinWidth = 100;
		this.Dscription.Name = "Dscription";
		this.Dscription.OptionsColumn.AllowEdit = false;
		this.Dscription.OptionsColumn.ReadOnly = true;
		this.Dscription.PrimaryKey = false;
		this.Dscription.UnitField = null;
		this.Dscription.Visible = true;
		this.Dscription.VisibleIndex = 1;
		this.Dscription.Width = 100;
		this.Quantity.Caption = "Qty";
		this.Quantity.CurrencyDateField = null;
		this.Quantity.CurrencyField = null;
		this.Quantity.CurrencyFixed = null;
		this.Quantity.CurrencyFixedRate = 0.0;
		this.Quantity.CurrencyRateField = null;
		this.Quantity.FieldName = "Quantity";
		this.Quantity.MinWidth = 100;
		this.Quantity.Name = "Quantity";
		this.Quantity.OptionsColumn.AllowEdit = false;
		this.Quantity.OptionsColumn.ReadOnly = true;
		this.Quantity.PrimaryKey = false;
		this.Quantity.UnitField = null;
		this.Quantity.Visible = true;
		this.Quantity.VisibleIndex = 2;
		this.Quantity.Width = 100;
		this.PriceFOB.Caption = "Base Doc. Price";
		this.PriceFOB.CurrencyDateField = null;
		this.PriceFOB.CurrencyField = null;
		this.PriceFOB.CurrencyFixed = null;
		this.PriceFOB.CurrencyFixedRate = 0.0;
		this.PriceFOB.CurrencyRateField = null;
		this.PriceFOB.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.PriceFOB.FieldName = "Price";
		this.PriceFOB.MinWidth = 100;
		this.PriceFOB.Name = "PriceFOB";
		this.PriceFOB.OptionsColumn.AllowEdit = false;
		this.PriceFOB.OptionsColumn.ReadOnly = true;
		this.PriceFOB.PrimaryKey = false;
		this.PriceFOB.UnitField = null;
		this.PriceFOB.Visible = true;
		this.PriceFOB.VisibleIndex = 3;
		this.PriceFOB.Width = 100;
		this.Currency.Caption = "Currency";
		this.Currency.ColumnEdit = this.repositoryItemComboBox5;
		this.Currency.CurrencyDateField = null;
		this.Currency.CurrencyField = null;
		this.Currency.CurrencyFixed = null;
		this.Currency.CurrencyFixedRate = 0.0;
		this.Currency.CurrencyRateField = null;
		this.Currency.FieldName = "Currency";
		this.Currency.MinWidth = 100;
		this.Currency.Name = "Currency";
		this.Currency.OptionsColumn.AllowEdit = false;
		this.Currency.OptionsColumn.ReadOnly = true;
		this.Currency.PrimaryKey = false;
		this.Currency.UnitField = null;
		this.Currency.Visible = true;
		this.Currency.VisibleIndex = 4;
		this.Currency.Width = 100;
		this.repositoryItemComboBox5.AutoHeight = false;
		this.repositoryItemComboBox5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemComboBox5.Name = "repositoryItemComboBox5";
		this.Rate.Caption = "Rate";
		this.Rate.CurrencyDateField = null;
		this.Rate.CurrencyField = null;
		this.Rate.CurrencyFixed = null;
		this.Rate.CurrencyFixedRate = 0.0;
		this.Rate.CurrencyRateField = null;
		this.Rate.FieldName = "Rate";
		this.Rate.MinWidth = 100;
		this.Rate.Name = "Rate";
		this.Rate.OptionsColumn.AllowEdit = false;
		this.Rate.OptionsColumn.ReadOnly = true;
		this.Rate.PrimaryKey = false;
		this.Rate.UnitField = null;
		this.Rate.Visible = true;
		this.Rate.VisibleIndex = 5;
		this.Rate.Width = 100;
		this.Custom.Caption = "Proj. Cust.";
		this.Custom.ColumnEdit = this.repositoryItemCalcEdit2;
		this.Custom.CurrencyDateField = null;
		this.Custom.CurrencyField = null;
		this.Custom.CurrencyFixed = null;
		this.Custom.CurrencyFixedRate = 0.0;
		this.Custom.CurrencyRateField = null;
		this.Custom.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.Custom.FieldName = "Custom";
		this.Custom.MinWidth = 100;
		this.Custom.Name = "Custom";
		this.Custom.PrimaryKey = false;
		this.Custom.UnitField = null;
		this.Custom.Visible = true;
		this.Custom.VisibleIndex = 6;
		this.Custom.Width = 100;
		this.repositoryItemCalcEdit2.AutoHeight = false;
		this.repositoryItemCalcEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit2.Name = "repositoryItemCalcEdit2";
		this.Cost.Caption = "Expenditure";
		this.Cost.ColumnEdit = this.repositoryItemCalcEdit3;
		this.Cost.CurrencyDateField = null;
		this.Cost.CurrencyField = null;
		this.Cost.CurrencyFixed = null;
		this.Cost.CurrencyFixedRate = 0.0;
		this.Cost.CurrencyRateField = null;
		this.Cost.DisplayFormat.FormatString = "#.00";
		this.Cost.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.Cost.FieldName = "Cost";
		this.Cost.MinWidth = 100;
		this.Cost.Name = "Cost";
		this.Cost.PrimaryKey = false;
		this.Cost.UnitField = null;
		this.Cost.Visible = true;
		this.Cost.VisibleIndex = 7;
		this.Cost.Width = 100;
		this.repositoryItemCalcEdit3.AutoHeight = false;
		this.repositoryItemCalcEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit3.Name = "repositoryItemCalcEdit3";
		this.PriceAtWH.Caption = "Whse Price";
		this.PriceAtWH.CurrencyDateField = null;
		this.PriceAtWH.CurrencyField = null;
		this.PriceAtWH.CurrencyFixed = null;
		this.PriceAtWH.CurrencyFixedRate = 0.0;
		this.PriceAtWH.CurrencyRateField = null;
		this.PriceAtWH.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.PriceAtWH.FieldName = "PriceAtWH";
		this.PriceAtWH.MinWidth = 100;
		this.PriceAtWH.Name = "PriceAtWH";
		this.PriceAtWH.OptionsColumn.AllowEdit = false;
		this.PriceAtWH.OptionsColumn.ReadOnly = true;
		this.PriceAtWH.PrimaryKey = false;
		this.PriceAtWH.UnitField = null;
		this.PriceAtWH.Visible = true;
		this.PriceAtWH.VisibleIndex = 8;
		this.PriceAtWH.Width = 100;
		this.LineTotal.Caption = "Total";
		this.LineTotal.CurrencyDateField = null;
		this.LineTotal.CurrencyField = null;
		this.LineTotal.CurrencyFixed = null;
		this.LineTotal.CurrencyFixedRate = 0.0;
		this.LineTotal.CurrencyRateField = null;
		this.LineTotal.DisplayFormat.FormatString = "#.00";
		this.LineTotal.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.LineTotal.FieldName = "LineTotal";
		this.LineTotal.MinWidth = 100;
		this.LineTotal.Name = "LineTotal";
		this.LineTotal.OptionsColumn.AllowEdit = false;
		this.LineTotal.OptionsColumn.ReadOnly = true;
		this.LineTotal.PrimaryKey = false;
		this.LineTotal.UnitField = null;
		this.LineTotal.Visible = true;
		this.LineTotal.VisibleIndex = 9;
		this.LineTotal.Width = 100;
		this.Volume.Caption = "Volume";
		this.Volume.ColumnEdit = this.repositoryItemCalcEdit4;
		this.Volume.CurrencyDateField = null;
		this.Volume.CurrencyField = null;
		this.Volume.CurrencyFixed = null;
		this.Volume.CurrencyFixedRate = 0.0;
		this.Volume.CurrencyRateField = null;
		this.Volume.FieldName = "Volume";
		this.Volume.MinWidth = 100;
		this.Volume.Name = "Volume";
		this.Volume.PrimaryKey = false;
		this.Volume.UnitField = null;
		this.Volume.Visible = true;
		this.Volume.VisibleIndex = 10;
		this.Volume.Width = 100;
		this.repositoryItemCalcEdit4.AutoHeight = false;
		this.repositoryItemCalcEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit4.Name = "repositoryItemCalcEdit4";
		this.UnitCode.Caption = "Unit of Volume";
		this.UnitCode.ColumnEdit = this.gridcomboUnitCode;
		this.UnitCode.CurrencyDateField = null;
		this.UnitCode.CurrencyField = null;
		this.UnitCode.CurrencyFixed = null;
		this.UnitCode.CurrencyFixedRate = 0.0;
		this.UnitCode.CurrencyRateField = null;
		this.UnitCode.FieldName = "UnitCode";
		this.UnitCode.MinWidth = 100;
		this.UnitCode.Name = "UnitCode";
		this.UnitCode.PrimaryKey = false;
		this.UnitCode.UnitField = null;
		this.UnitCode.Visible = true;
		this.UnitCode.VisibleIndex = 11;
		this.UnitCode.Width = 100;
		this.gridcomboUnitCode.AutoHeight = false;
		this.gridcomboUnitCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.gridcomboUnitCode.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.gridcomboUnitCode.Name = "gridcomboUnitCode";
		this.Reference.Caption = "Reference";
		this.Reference.CurrencyDateField = null;
		this.Reference.CurrencyField = null;
		this.Reference.CurrencyFixed = null;
		this.Reference.CurrencyFixedRate = 0.0;
		this.Reference.CurrencyRateField = null;
		this.Reference.FieldName = "Reference";
		this.Reference.MinWidth = 100;
		this.Reference.Name = "Reference";
		this.Reference.OptionsColumn.AllowEdit = false;
		this.Reference.OptionsColumn.ReadOnly = true;
		this.Reference.PrimaryKey = false;
		this.Reference.UnitField = null;
		this.Reference.Visible = true;
		this.Reference.VisibleIndex = 12;
		this.Reference.Width = 100;
		this.OrigLine.Caption = "Orig. Line";
		this.OrigLine.CurrencyDateField = null;
		this.OrigLine.CurrencyField = null;
		this.OrigLine.CurrencyFixed = null;
		this.OrigLine.CurrencyFixedRate = 0.0;
		this.OrigLine.CurrencyRateField = null;
		this.OrigLine.FieldName = "OrigLine";
		this.OrigLine.MinWidth = 100;
		this.OrigLine.Name = "OrigLine";
		this.OrigLine.OptionsColumn.AllowEdit = false;
		this.OrigLine.OptionsColumn.ReadOnly = true;
		this.OrigLine.PrimaryKey = false;
		this.OrigLine.UnitField = null;
		this.OrigLine.Visible = true;
		this.OrigLine.VisibleIndex = 13;
		this.OrigLine.Width = 100;
		this.FactNoCust.Caption = "Fact. No Cust.";
		this.FactNoCust.CurrencyDateField = null;
		this.FactNoCust.CurrencyField = null;
		this.FactNoCust.CurrencyFixed = null;
		this.FactNoCust.CurrencyFixedRate = 0.0;
		this.FactNoCust.CurrencyRateField = null;
		this.FactNoCust.FieldName = "FactNoCust";
		this.FactNoCust.MinWidth = 100;
		this.FactNoCust.Name = "FactNoCust";
		this.FactNoCust.OptionsColumn.AllowEdit = false;
		this.FactNoCust.OptionsColumn.ReadOnly = true;
		this.FactNoCust.PrimaryKey = false;
		this.FactNoCust.UnitField = null;
		this.FactNoCust.Visible = true;
		this.FactNoCust.VisibleIndex = 14;
		this.FactNoCust.Width = 100;
		this.PriceList.Caption = "Price List";
		this.PriceList.ColumnEdit = this.repositoryItemComboBox3;
		this.PriceList.CurrencyDateField = null;
		this.PriceList.CurrencyField = null;
		this.PriceList.CurrencyFixed = null;
		this.PriceList.CurrencyFixedRate = 0.0;
		this.PriceList.CurrencyRateField = null;
		this.PriceList.FieldName = "PriceList";
		this.PriceList.MinWidth = 100;
		this.PriceList.Name = "PriceList";
		this.PriceList.PrimaryKey = false;
		this.PriceList.UnitField = null;
		this.PriceList.Visible = true;
		this.PriceList.VisibleIndex = 15;
		this.PriceList.Width = 100;
		this.repositoryItemComboBox3.AutoHeight = false;
		this.repositoryItemComboBox3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemComboBox3.Name = "repositoryItemComboBox3";
		this.CostOH.Caption = "Cost OH";
		this.CostOH.CurrencyDateField = null;
		this.CostOH.CurrencyField = null;
		this.CostOH.CurrencyFixed = null;
		this.CostOH.CurrencyFixedRate = 0.0;
		this.CostOH.CurrencyRateField = null;
		this.CostOH.FieldName = "CostOH";
		this.CostOH.MinWidth = 100;
		this.CostOH.Name = "CostOH";
		this.CostOH.OptionsColumn.AllowEdit = false;
		this.CostOH.OptionsColumn.ReadOnly = true;
		this.CostOH.PrimaryKey = false;
		this.CostOH.UnitField = null;
		this.CostOH.Visible = true;
		this.CostOH.VisibleIndex = 16;
		this.CostOH.Width = 100;
		this.StockEval.Caption = "Stock Eval";
		this.StockEval.CurrencyDateField = null;
		this.StockEval.CurrencyField = null;
		this.StockEval.CurrencyFixed = null;
		this.StockEval.CurrencyFixedRate = 0.0;
		this.StockEval.CurrencyRateField = null;
		this.StockEval.FieldName = "StockEval";
		this.StockEval.MinWidth = 100;
		this.StockEval.Name = "StockEval";
		this.StockEval.PrimaryKey = false;
		this.StockEval.UnitField = null;
		this.StockEval.Visible = true;
		this.StockEval.VisibleIndex = 17;
		this.StockEval.Width = 100;
		this.UseBaseUn.Caption = "Inventory UOM";
		this.UseBaseUn.CurrencyDateField = null;
		this.UseBaseUn.CurrencyField = null;
		this.UseBaseUn.CurrencyFixed = null;
		this.UseBaseUn.CurrencyFixedRate = 0.0;
		this.UseBaseUn.CurrencyRateField = null;
		this.UseBaseUn.FieldName = "UseBaseUn";
		this.UseBaseUn.MinWidth = 100;
		this.UseBaseUn.Name = "UseBaseUn";
		this.UseBaseUn.OptionsColumn.AllowEdit = false;
		this.UseBaseUn.OptionsColumn.ReadOnly = true;
		this.UseBaseUn.PrimaryKey = false;
		this.UseBaseUn.UnitField = null;
		this.UseBaseUn.Visible = true;
		this.UseBaseUn.VisibleIndex = 18;
		this.UseBaseUn.Width = 100;
		this.BlockNum.Caption = "Block No";
		this.BlockNum.CurrencyDateField = null;
		this.BlockNum.CurrencyField = null;
		this.BlockNum.CurrencyFixed = null;
		this.BlockNum.CurrencyFixedRate = 0.0;
		this.BlockNum.CurrencyRateField = null;
		this.BlockNum.FieldName = "BlockNum";
		this.BlockNum.MinWidth = 100;
		this.BlockNum.Name = "BlockNum";
		this.BlockNum.PrimaryKey = false;
		this.BlockNum.UnitField = null;
		this.BlockNum.Visible = true;
		this.BlockNum.VisibleIndex = 19;
		this.BlockNum.Width = 100;
		this.ImportLog.Caption = "Import Log";
		this.ImportLog.CurrencyDateField = null;
		this.ImportLog.CurrencyField = null;
		this.ImportLog.CurrencyFixed = null;
		this.ImportLog.CurrencyFixedRate = 0.0;
		this.ImportLog.CurrencyRateField = null;
		this.ImportLog.FieldName = "ImportLog";
		this.ImportLog.MinWidth = 100;
		this.ImportLog.Name = "ImportLog";
		this.ImportLog.PrimaryKey = false;
		this.ImportLog.UnitField = null;
		this.ImportLog.Visible = true;
		this.ImportLog.VisibleIndex = 20;
		this.ImportLog.Width = 100;
		this.WhsCode.Caption = "Warehourse";
		this.WhsCode.CurrencyDateField = null;
		this.WhsCode.CurrencyField = null;
		this.WhsCode.CurrencyFixed = null;
		this.WhsCode.CurrencyFixedRate = 0.0;
		this.WhsCode.CurrencyRateField = null;
		this.WhsCode.FieldName = "WhsCode";
		this.WhsCode.MinWidth = 100;
		this.WhsCode.Name = "WhsCode";
		this.WhsCode.OptionsColumn.ReadOnly = true;
		this.WhsCode.PrimaryKey = false;
		this.WhsCode.UnitField = null;
		this.WhsCode.Visible = true;
		this.WhsCode.VisibleIndex = 21;
		this.WhsCode.Width = 100;
		this.OrigRow.Caption = "Orig. Row";
		this.OrigRow.CurrencyDateField = null;
		this.OrigRow.CurrencyField = null;
		this.OrigRow.CurrencyFixed = null;
		this.OrigRow.CurrencyFixedRate = 0.0;
		this.OrigRow.CurrencyRateField = null;
		this.OrigRow.FieldName = "OrigRow";
		this.OrigRow.MinWidth = 100;
		this.OrigRow.Name = "OrigRow";
		this.OrigRow.OptionsColumn.ReadOnly = true;
		this.OrigRow.PrimaryKey = false;
		this.OrigRow.UnitField = null;
		this.OrigRow.Visible = true;
		this.OrigRow.VisibleIndex = 22;
		this.OrigRow.Width = 100;
		this.Locked.Caption = "Locked";
		this.Locked.CurrencyDateField = null;
		this.Locked.CurrencyField = null;
		this.Locked.CurrencyFixed = null;
		this.Locked.CurrencyFixedRate = 0.0;
		this.Locked.CurrencyRateField = null;
		this.Locked.FieldName = "Locked";
		this.Locked.MinWidth = 100;
		this.Locked.Name = "Locked";
		this.Locked.OptionsColumn.ReadOnly = true;
		this.Locked.PrimaryKey = false;
		this.Locked.UnitField = null;
		this.Locked.Visible = true;
		this.Locked.VisibleIndex = 23;
		this.Locked.Width = 100;
		this.OrigWhs.Caption = "Orig. Whse";
		this.OrigWhs.CurrencyDateField = null;
		this.OrigWhs.CurrencyField = null;
		this.OrigWhs.CurrencyFixed = null;
		this.OrigWhs.CurrencyFixedRate = 0.0;
		this.OrigWhs.CurrencyRateField = null;
		this.OrigWhs.FieldName = "OrigWhs";
		this.OrigWhs.MinWidth = 100;
		this.OrigWhs.Name = "OrigWhs";
		this.OrigWhs.OptionsColumn.ReadOnly = true;
		this.OrigWhs.PrimaryKey = false;
		this.OrigWhs.UnitField = null;
		this.OrigWhs.Visible = true;
		this.OrigWhs.VisibleIndex = 24;
		this.OrigWhs.Width = 100;
		this.ReleaseNum.Caption = "Release Number";
		this.ReleaseNum.CurrencyDateField = null;
		this.ReleaseNum.CurrencyField = null;
		this.ReleaseNum.CurrencyFixed = null;
		this.ReleaseNum.CurrencyFixedRate = 0.0;
		this.ReleaseNum.CurrencyRateField = null;
		this.ReleaseNum.FieldName = "ReleaseNum";
		this.ReleaseNum.MinWidth = 100;
		this.ReleaseNum.Name = "ReleaseNum";
		this.ReleaseNum.OptionsColumn.ReadOnly = true;
		this.ReleaseNum.PrimaryKey = false;
		this.ReleaseNum.UnitField = null;
		this.ReleaseNum.Visible = true;
		this.ReleaseNum.VisibleIndex = 25;
		this.ReleaseNum.Width = 100;
		this.VarCosts.Caption = "Var. Costs";
		this.VarCosts.CurrencyDateField = null;
		this.VarCosts.CurrencyField = null;
		this.VarCosts.CurrencyFixed = null;
		this.VarCosts.CurrencyFixedRate = 0.0;
		this.VarCosts.CurrencyRateField = null;
		this.VarCosts.FieldName = "VarCosts";
		this.VarCosts.MinWidth = 100;
		this.VarCosts.Name = "VarCosts";
		this.VarCosts.OptionsColumn.ReadOnly = true;
		this.VarCosts.PrimaryKey = false;
		this.VarCosts.UnitField = null;
		this.VarCosts.Visible = true;
		this.VarCosts.VisibleIndex = 26;
		this.VarCosts.Width = 100;
		this.ConstCosts.Caption = "Fix Costs";
		this.ConstCosts.CurrencyDateField = null;
		this.ConstCosts.CurrencyField = null;
		this.ConstCosts.CurrencyFixed = null;
		this.ConstCosts.CurrencyFixedRate = 0.0;
		this.ConstCosts.CurrencyRateField = null;
		this.ConstCosts.FieldName = "ConstCosts";
		this.ConstCosts.MinWidth = 100;
		this.ConstCosts.Name = "ConstCosts";
		this.ConstCosts.OptionsColumn.ReadOnly = true;
		this.ConstCosts.PrimaryKey = false;
		this.ConstCosts.UnitField = null;
		this.ConstCosts.Visible = true;
		this.ConstCosts.VisibleIndex = 27;
		this.ConstCosts.Width = 100;
		this.UserCustom.Caption = "Customs";
		this.UserCustom.CurrencyDateField = null;
		this.UserCustom.CurrencyField = null;
		this.UserCustom.CurrencyFixed = null;
		this.UserCustom.CurrencyFixedRate = 0.0;
		this.UserCustom.CurrencyRateField = null;
		this.UserCustom.FieldName = "UserCustom";
		this.UserCustom.MinWidth = 100;
		this.UserCustom.Name = "UserCustom";
		this.UserCustom.OptionsColumn.ReadOnly = true;
		this.UserCustom.PrimaryKey = false;
		this.UserCustom.UnitField = null;
		this.UserCustom.Visible = true;
		this.UserCustom.VisibleIndex = 28;
		this.UserCustom.Width = 100;
		this.FobValue.Caption = "Base Doc. Value";
		this.FobValue.CurrencyDateField = null;
		this.FobValue.CurrencyField = null;
		this.FobValue.CurrencyFixed = null;
		this.FobValue.CurrencyFixedRate = 0.0;
		this.FobValue.CurrencyRateField = null;
		this.FobValue.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.FobValue.FieldName = "LineTotal";
		this.FobValue.MinWidth = 100;
		this.FobValue.Name = "FobValue";
		this.FobValue.OptionsColumn.ReadOnly = true;
		this.FobValue.PrimaryKey = false;
		this.FobValue.UnitField = null;
		this.FobValue.Visible = true;
		this.FobValue.VisibleIndex = 29;
		this.FobValue.Width = 100;
		this.TtlExpndLC.Caption = "Alloc. Costs";
		this.TtlExpndLC.ColumnEdit = this.repositoryItemCalcEdit1;
		this.TtlExpndLC.CurrencyDateField = null;
		this.TtlExpndLC.CurrencyField = null;
		this.TtlExpndLC.CurrencyFixed = null;
		this.TtlExpndLC.CurrencyFixedRate = 0.0;
		this.TtlExpndLC.CurrencyRateField = null;
		this.TtlExpndLC.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.TtlExpndLC.FieldName = "TtlExpndLC";
		this.TtlExpndLC.MinWidth = 100;
		this.TtlExpndLC.Name = "TtlExpndLC";
		this.TtlExpndLC.PrimaryKey = false;
		this.TtlExpndLC.UnitField = null;
		this.TtlExpndLC.Visible = true;
		this.TtlExpndLC.VisibleIndex = 30;
		this.TtlExpndLC.Width = 100;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.TtlCustLC.Caption = "Cust. Value";
		this.TtlCustLC.ColumnEdit = this.repositoryItemCalcEdit5;
		this.TtlCustLC.CurrencyDateField = null;
		this.TtlCustLC.CurrencyField = null;
		this.TtlCustLC.CurrencyFixed = null;
		this.TtlCustLC.CurrencyFixedRate = 0.0;
		this.TtlCustLC.CurrencyRateField = null;
		this.TtlCustLC.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.TtlCustLC.FieldName = "TtlCustLC";
		this.TtlCustLC.MinWidth = 100;
		this.TtlCustLC.Name = "TtlCustLC";
		this.TtlCustLC.PrimaryKey = false;
		this.TtlCustLC.UnitField = null;
		this.TtlCustLC.Visible = true;
		this.TtlCustLC.VisibleIndex = 31;
		this.TtlCustLC.Width = 100;
		this.repositoryItemCalcEdit5.AutoHeight = false;
		this.repositoryItemCalcEdit5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit5.Name = "repositoryItemCalcEdit5";
		this.TtlCostLC.Caption = "Total Costs";
		this.TtlCostLC.ColumnEdit = this.repositoryItemCalcEdit6;
		this.TtlCostLC.CurrencyDateField = null;
		this.TtlCostLC.CurrencyField = null;
		this.TtlCostLC.CurrencyFixed = null;
		this.TtlCostLC.CurrencyFixedRate = 0.0;
		this.TtlCostLC.CurrencyRateField = null;
		this.TtlCostLC.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.TtlCostLC.FieldName = "TtlCostLC";
		this.TtlCostLC.MinWidth = 100;
		this.TtlCostLC.Name = "TtlCostLC";
		this.TtlCostLC.PrimaryKey = false;
		this.TtlCostLC.UnitField = null;
		this.TtlCostLC.Visible = true;
		this.TtlCostLC.VisibleIndex = 32;
		this.TtlCostLC.Width = 100;
		this.repositoryItemCalcEdit6.AutoHeight = false;
		this.repositoryItemCalcEdit6.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit6.Name = "repositoryItemCalcEdit6";
		this.NumPerMsr.Caption = "Unit Per Row";
		this.NumPerMsr.CurrencyDateField = null;
		this.NumPerMsr.CurrencyField = null;
		this.NumPerMsr.CurrencyFixed = null;
		this.NumPerMsr.CurrencyFixedRate = 0.0;
		this.NumPerMsr.CurrencyRateField = null;
		this.NumPerMsr.DisplayFormat.FormatString = "#";
		this.NumPerMsr.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.NumPerMsr.FieldName = "NumPerMsr";
		this.NumPerMsr.MinWidth = 100;
		this.NumPerMsr.Name = "NumPerMsr";
		this.NumPerMsr.OptionsColumn.AllowEdit = false;
		this.NumPerMsr.OptionsColumn.ReadOnly = true;
		this.NumPerMsr.PrimaryKey = false;
		this.NumPerMsr.UnitField = null;
		this.NumPerMsr.Visible = true;
		this.NumPerMsr.VisibleIndex = 33;
		this.NumPerMsr.Width = 100;
		this.xtraTabPage2.Controls.Add(this.btNewLandCost);
		this.xtraTabPage2.Controls.Add(this.btCostClear);
		this.xtraTabPage2.Controls.Add(this.grdCost);
		this.xtraTabPage2.EditControl = null;
		this.xtraTabPage2.EditValue = "Costs";
		this.xtraTabPage2.FieldName = null;
		this.xtraTabPage2.IsSearchControl = false;
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.ReadOnly = false;
		this.xtraTabPage2.SettingInVisible = false;
		this.xtraTabPage2.SettingReadonly = false;
		this.xtraTabPage2.Size = new System.Drawing.Size(616, 358);
		this.xtraTabPage2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage2.Text = "Costs";
		this.btNewLandCost.EditControl = null;
		this.btNewLandCost.EditValue = "New Landed costs";
		this.btNewLandCost.FieldName = null;
		this.btNewLandCost.IsSearchControl = false;
		this.btNewLandCost.Location = new System.Drawing.Point(491, 282);
		this.btNewLandCost.Name = "btNewLandCost";
		this.btNewLandCost.ReadOnly = false;
		this.btNewLandCost.SettingInVisible = false;
		this.btNewLandCost.SettingReadonly = false;
		this.btNewLandCost.Size = new System.Drawing.Size(113, 23);
		this.btNewLandCost.TabIndex = 5;
		this.btNewLandCost.Text = "New Landed costs";
		this.btNewLandCost.Click += new System.EventHandler(this.btNewLandCost_Click);
		this.btCostClear.EditControl = null;
		this.btCostClear.EditValue = "Clear table";
		this.btCostClear.FieldName = null;
		this.btCostClear.IsSearchControl = false;
		this.btCostClear.Location = new System.Drawing.Point(491, 253);
		this.btCostClear.Name = "btCostClear";
		this.btCostClear.ReadOnly = false;
		this.btCostClear.SettingInVisible = false;
		this.btCostClear.SettingReadonly = false;
		this.btCostClear.Size = new System.Drawing.Size(113, 23);
		this.btCostClear.TabIndex = 4;
		this.btCostClear.Text = "Clear table";
		this.btCostClear.Click += new System.EventHandler(this.btCostClear_Click);
		this.grdCost.AllowPasteExcel = false;
		this.grdCost.ClipboardData = null;
		this.grdCost.Location = new System.Drawing.Point(3, 3);
		this.grdCost.MainView = this.gvCost;
		this.grdCost.Name = "grdCost";
		this.grdCost.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.repositoryItemComboBox1, this.repositoryItemComboBox2, this.repositoryItemCalcEdit7, this.repositoryItemCalcEdit8 });
		this.grdCost.Size = new System.Drawing.Size(477, 345);
		this.grdCost.TabIndex = 3;
		this.grdCost.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvCost });
		this.gvCost.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvCost.ColumnKey = null;
		this.gvCost.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.AlcCode, this.AlcName, this.OhType, this.CostSum, this.Factor, this.LaCAllcAcc });
		this.gvCost.DisableFormSetting = false;
		this.gvCost.DisplayException = false;
		this.gvCost.EditColumn = null;
		this.gvCost.FieldCurrency = null;
		this.gvCost.FieldRate = null;
		this.gvCost.FormId = null;
		this.gvCost.GridControl = this.grdCost;
		this.gvCost.IgnoreFilter = false;
		this.gvCost.IndexEditRow = -1;
		this.gvCost.IndicatorWidth = 35;
		this.gvCost.IsApzTable = false;
		this.gvCost.IsInsertNewRow = false;
		this.gvCost.IsLoadingData = false;
		this.gvCost.IsMappingContextAction = false;
		this.gvCost.ItemId = null;
		this.gvCost.LstColumnAllowBatch = null;
		this.gvCost.LstColumnNotAllowBatch = null;
		this.gvCost.LstColumnNotEditable = null;
		this.gvCost.LstColumNotSetting = null;
		this.gvCost.Name = "gvCost";
		this.gvCost.OptionsView.ShowDetailButtons = false;
		this.gvCost.OptionsView.ShowGroupPanel = false;
		this.gvCost.UseBatchMod = false;
		this.gvCost.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvCost.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvCost_CellValueChanged);
		this.AlcCode.Caption = "Landed Code";
		this.AlcCode.CurrencyDateField = null;
		this.AlcCode.CurrencyField = null;
		this.AlcCode.CurrencyFixed = null;
		this.AlcCode.CurrencyFixedRate = 0.0;
		this.AlcCode.CurrencyRateField = null;
		this.AlcCode.FieldName = "AlcCode";
		this.AlcCode.Name = "AlcCode";
		this.AlcCode.OptionsColumn.AllowEdit = false;
		this.AlcCode.OptionsColumn.ReadOnly = true;
		this.AlcCode.PrimaryKey = false;
		this.AlcCode.UnitField = null;
		this.AlcName.Caption = "Landed Cost Name";
		this.AlcName.CurrencyDateField = null;
		this.AlcName.CurrencyField = null;
		this.AlcName.CurrencyFixed = null;
		this.AlcName.CurrencyFixedRate = 0.0;
		this.AlcName.CurrencyRateField = null;
		this.AlcName.FieldName = "AlcName";
		this.AlcName.Name = "AlcName";
		this.AlcName.OptionsColumn.AllowEdit = false;
		this.AlcName.OptionsColumn.ReadOnly = true;
		this.AlcName.PrimaryKey = false;
		this.AlcName.UnitField = null;
		this.AlcName.Visible = true;
		this.AlcName.VisibleIndex = 0;
		this.AlcName.Width = 121;
		this.OhType.Caption = "Allocation By";
		this.OhType.ColumnEdit = this.repositoryItemComboBox1;
		this.OhType.CurrencyDateField = null;
		this.OhType.CurrencyField = null;
		this.OhType.CurrencyFixed = null;
		this.OhType.CurrencyFixedRate = 0.0;
		this.OhType.CurrencyRateField = null;
		this.OhType.FieldName = "OhType";
		this.OhType.Name = "OhType";
		this.OhType.PrimaryKey = false;
		this.OhType.UnitField = null;
		this.OhType.Visible = true;
		this.OhType.VisibleIndex = 1;
		this.OhType.Width = 83;
		this.repositoryItemComboBox1.AutoHeight = false;
		this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
		this.CostSum.Caption = "Amount";
		this.CostSum.ColumnEdit = this.repositoryItemCalcEdit7;
		this.CostSum.CurrencyDateField = null;
		this.CostSum.CurrencyField = null;
		this.CostSum.CurrencyFixed = null;
		this.CostSum.CurrencyFixedRate = 0.0;
		this.CostSum.CurrencyRateField = null;
		this.CostSum.DisplayFormat.FormatString = "#.00";
		this.CostSum.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.CostSum.FieldName = "CostSum";
		this.CostSum.Name = "CostSum";
		this.CostSum.PrimaryKey = false;
		this.CostSum.UnitField = null;
		this.CostSum.Visible = true;
		this.CostSum.VisibleIndex = 2;
		this.CostSum.Width = 83;
		this.repositoryItemCalcEdit7.AutoHeight = false;
		this.repositoryItemCalcEdit7.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit7.Name = "repositoryItemCalcEdit7";
		this.Factor.Caption = "Factor";
		this.Factor.ColumnEdit = this.repositoryItemCalcEdit8;
		this.Factor.CurrencyDateField = null;
		this.Factor.CurrencyField = null;
		this.Factor.CurrencyFixed = null;
		this.Factor.CurrencyFixedRate = 0.0;
		this.Factor.CurrencyRateField = null;
		this.Factor.DisplayFormat.FormatString = "#.00";
		this.Factor.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.Factor.FieldName = "Factor";
		this.Factor.Name = "Factor";
		this.Factor.PrimaryKey = false;
		this.Factor.UnitField = null;
		this.Factor.Visible = true;
		this.Factor.VisibleIndex = 3;
		this.Factor.Width = 83;
		this.repositoryItemCalcEdit8.AutoHeight = false;
		this.repositoryItemCalcEdit8.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit8.Name = "repositoryItemCalcEdit8";
		this.LaCAllcAcc.Caption = "Landed Costs Alloc. Account";
		this.LaCAllcAcc.ColumnEdit = this.repositoryItemComboBox2;
		this.LaCAllcAcc.CurrencyDateField = null;
		this.LaCAllcAcc.CurrencyField = null;
		this.LaCAllcAcc.CurrencyFixed = null;
		this.LaCAllcAcc.CurrencyFixedRate = 0.0;
		this.LaCAllcAcc.CurrencyRateField = null;
		this.LaCAllcAcc.FieldName = "LaCAllcAcc";
		this.LaCAllcAcc.Name = "LaCAllcAcc";
		this.LaCAllcAcc.PrimaryKey = false;
		this.LaCAllcAcc.UnitField = null;
		this.LaCAllcAcc.Visible = true;
		this.LaCAllcAcc.VisibleIndex = 4;
		this.LaCAllcAcc.Width = 89;
		this.repositoryItemComboBox2.AutoHeight = false;
		this.repositoryItemComboBox2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemComboBox2.Items.AddRange(new object[2] { "Yes", "No" });
		this.repositoryItemComboBox2.Name = "repositoryItemComboBox2";
		this.xtraTabPage3.Appearance.PageClient.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.xtraTabPage3.Appearance.PageClient.Options.UseBackColor = true;
		this.xtraTabPage3.Controls.Add(this.txtJournalRemark);
		this.xtraTabPage3.Controls.Add(this.txtTransNo);
		this.xtraTabPage3.Controls.Add(this.labelLine20);
		this.xtraTabPage3.Controls.Add(this.labelLine21);
		this.xtraTabPage3.EditControl = null;
		this.xtraTabPage3.EditValue = "General";
		this.xtraTabPage3.FieldName = null;
		this.xtraTabPage3.IsSearchControl = false;
		this.xtraTabPage3.Name = "xtraTabPage3";
		this.xtraTabPage3.ReadOnly = false;
		this.xtraTabPage3.SettingInVisible = false;
		this.xtraTabPage3.SettingReadonly = false;
		this.xtraTabPage3.Size = new System.Drawing.Size(616, 358);
		this.xtraTabPage3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage3.Text = "General";
		this.txtJournalRemark.EditControl = null;
		this.txtJournalRemark.EditValue = "Landed Costs";
		this.txtJournalRemark.FieldName = null;
		this.txtJournalRemark.IsSearchControl = false;
		this.txtJournalRemark.Location = new System.Drawing.Point(117, 42);
		this.txtJournalRemark.Name = "txtJournalRemark";
		this.txtJournalRemark.SettingInVisible = false;
		this.txtJournalRemark.SettingReadonly = false;
		this.txtJournalRemark.Size = new System.Drawing.Size(279, 20);
		this.txtJournalRemark.TabIndex = 15;
		this.txtJournalRemark.TextChanged += new System.EventHandler(this.txtJournalRemark_TextChanged);
		this.txtTransNo.EditControl = null;
		this.txtTransNo.EditValue = "123456";
		this.txtTransNo.FieldName = null;
		this.txtTransNo.IsSearchControl = false;
		this.txtTransNo.Location = new System.Drawing.Point(117, 17);
		this.txtTransNo.Name = "txtTransNo";
		this.txtTransNo.Properties.ReadOnly = true;
		this.txtTransNo.SettingInVisible = false;
		this.txtTransNo.SettingReadonly = false;
		this.txtTransNo.Size = new System.Drawing.Size(279, 20);
		this.txtTransNo.TabIndex = 14;
		this.labelLine20.EditControl = null;
		this.labelLine20.EditValue = "Journal Remarks";
		this.labelLine20.FieldName = null;
		this.labelLine20.HideWhenEmpty = false;
		this.labelLine20.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine20.Location = new System.Drawing.Point(31, 42);
		this.labelLine20.Name = "labelLine20";
		this.labelLine20.ReadOnly = false;
		this.labelLine20.SettingInVisible = false;
		this.labelLine20.SettingReadonly = false;
		this.labelLine20.ShowUnderLine = true;
		this.labelLine20.Size = new System.Drawing.Size(80, 20);
		this.labelLine20.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine20.TabIndex = 13;
		this.labelLine20.Utext = "Journal Remarks";
		this.labelLine21.EditControl = null;
		this.labelLine21.EditValue = "Trans. No.";
		this.labelLine21.FieldName = null;
		this.labelLine21.HideWhenEmpty = false;
		this.labelLine21.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine21.Location = new System.Drawing.Point(31, 17);
		this.labelLine21.Name = "labelLine21";
		this.labelLine21.ReadOnly = false;
		this.labelLine21.SettingInVisible = false;
		this.labelLine21.SettingReadonly = false;
		this.labelLine21.ShowUnderLine = true;
		this.labelLine21.Size = new System.Drawing.Size(80, 20);
		this.labelLine21.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine21.TabIndex = 12;
		this.labelLine21.Utext = "Trans. No.";
		this.txtVendor.EditControl = null;
		this.txtVendor.FieldName = null;
		this.txtVendor.IsSearchControl = false;
		this.txtVendor.Location = new System.Drawing.Point(98, 3);
		this.txtVendor.Name = "txtVendor";
		this.txtVendor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight, "", -1, true, true, true, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtVendor.SettingInVisible = false;
		this.txtVendor.SettingReadonly = false;
		this.txtVendor.Size = new System.Drawing.Size(113, 20);
		this.txtVendor.TabIndex = 2;
		this.txtVendor.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtVendor_ButtonPressed);
		this.txtVendor.TextChanged += new System.EventHandler(this.txtVendor_TextChanged);
		this.txtVendor.Leave += new System.EventHandler(this.txtVendor_Leave);
		this.txtBroker.EditControl = null;
		this.txtBroker.FieldName = null;
		this.txtBroker.IsSearchControl = false;
		this.txtBroker.Location = new System.Drawing.Point(98, 28);
		this.txtBroker.Name = "txtBroker";
		this.txtBroker.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight, "", -1, true, true, true, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtBroker.SettingInVisible = false;
		this.txtBroker.SettingReadonly = false;
		this.txtBroker.Size = new System.Drawing.Size(113, 20);
		this.txtBroker.TabIndex = 4;
		this.txtBroker.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtBroker_ButtonPressed);
		this.txtBroker.TextChanged += new System.EventHandler(this.txtBroker_TextChanged);
		this.txtBroker.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtBroker_MouseDown);
		this.txtRate.EditControl = null;
		this.txtRate.FieldName = null;
		this.txtRate.IsSearchControl = false;
		this.txtRate.Location = new System.Drawing.Point(121, 58);
		this.txtRate.Name = "txtRate";
		this.txtRate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtRate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtRate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txtRate.Properties.IsSearchControl = false;
		this.txtRate.SettingInVisible = false;
		this.txtRate.SettingReadonly = false;
		this.txtRate.Size = new System.Drawing.Size(100, 20);
		this.txtRate.TabIndex = 49;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(625, 551);
		base.Controls.Add(this.xtraTabControl1);
		base.Controls.Add(this.btCopy);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btAdd);
		base.Controls.Add(this.cbCurrency);
		base.Controls.Add(this.labelLine8);
		base.Controls.Add(this.cbSeries);
		base.Controls.Add(this.dateDueDate);
		base.Controls.Add(this.datePostingDate);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.labelLine7);
		base.Controls.Add(this.txtNumber);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.txtFileNo);
		base.Controls.Add(this.txtReference);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.txtBrokerDetail);
		base.Controls.Add(this.txtVendorDetail);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.txtVendor);
		base.Controls.Add(this.txtBroker);
		base.Controls.Add(this.txtRate);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PurchaseLandedCost";
		this.Text = "Landed Costs";
		base.Load += new System.EventHandler(this.frm_PurchaseLandedCost_Load);
		((System.ComponentModel.ISupportInitialize)this.txtVendorDetail.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBrokerDetail.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFileNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtReference.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtNumber.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.datePostingDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.datePostingDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateDueDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateDueDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbSeries.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbCurrency.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.popupMenuCopyFrom).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.memoRemark.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBillLaddingNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbTransportType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkCusAffectInvent.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calTotal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calTax2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calTax1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calBeforeTax.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calAmountTBalance.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calTotalFCharges.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calActualCustoms.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcProjectedCustom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridcomboUnitCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit6).EndInit();
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdCost).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvCost).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox2).EndInit();
		this.xtraTabPage3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtJournalRemark.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtTransNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtVendor.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBroker.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtRate.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
