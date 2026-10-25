using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
using Apzon.Entities.Models.Payments;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;

namespace Apzon.Forms.Payment;

public class frm_AdvancePaymentWizard : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataTable _dtContact = new DataTable();

	private bool isCaculate;

	private IContainer components;

	private ButtonEdit bte_BPCode;

	private LabelLine labelLine2;

	private CalcEditEx cal_Amount;

	private LabelLine labelLine12;

	private CalcEditEx cal_AmountDue;

	private LabelLine labelLine1;

	private CalcEditEx cal_OpenBalance;

	private LabelLine labelLine3;

	private CalcEditEx cal_Rate;

	private LabelLine labelLine5;

	private LabelLine labelLine4;

	private ComboBoxEditEx cb_Currency;

	private SimpleButtonEx btn_PayBank;

	private SimpleButtonEx btn_PayCash;

	private XtraTabControl xtab_PayMean;

	private XtraTabPage tab_Cash;

	private PanelControl panelControl3;

	private LabelControl lbl_GLAccount_Cash;

	private CalcEditEx cal_Total_Cash;

	private LabelLine labelLine9;

	private ButtonEdit bte_GLAccount_Cash;

	private LabelLine labelLine8;

	private XtraTabPage tab_Bank;

	private PanelControl panelControl4;

	private LabelControl lbl_GLAccount_Bank;

	private CalcEditEx cal_Total_Bank;

	private LabelLine labelLine10;

	private ButtonEdit bte_GLAccount_Bank;

	private LabelLine labelLine11;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Save;

	private GridControlEx grd_Details;

	private GridViewEx gv_Details;

	private LabelLine labelLine6;

	private TextEditEx txt_CardName;

	private GridColumnEx gc_DocDate;

	private GridColumnEx gc_TaxDate;

	private GridColumnEx gc_ContactPerson;

	private GridColumnEx gc_BillTo;

	private RepositoryItemDateEditEx repositoryItemDateEditEx1;

	public frm_AdvancePaymentWizard()
	{
		InitializeComponent();
	}

	private void frm_AdvancePaymentWizard_Load(object sender, EventArgs e)
	{
		try
		{
			LoadCurrency();
			ResetForm();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bte_GLAccount_Cash_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, false, bte_GLAccount_Cash.Text, "APZ_CFL_GetGLAccount", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null && chooseItem.IsExistsColumn("AcctCode"))
			{
				bte_GLAccount_Cash.Text = Function.ToString(chooseItem["AcctCode"]);
				lbl_GLAccount_Cash.Text = Function.ToString(chooseItem["AcctName"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bte_GLAccount_Cash_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Tab && string.IsNullOrEmpty(bte_GLAccount_Cash.Text))
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, false, bte_GLAccount_Cash.Text, "APZ_CFL_GetGLAccount", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				DataRow chooseItem = frm_CFL2.ChooseItem;
				if (chooseItem != null && chooseItem.IsExistsColumn("AcctCode"))
				{
					bte_GLAccount_Cash.Text = Function.ToString(chooseItem["AcctCode"]);
					lbl_GLAccount_Cash.Text = Function.ToString(chooseItem["AcctName"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bte_GLAccount_Bank_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, false, bte_GLAccount_Bank.Text, "APZ_CFL_GetGLAccount", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null && chooseItem.IsExistsColumn("AcctCode"))
			{
				bte_GLAccount_Bank.Text = Function.ToString(chooseItem["AcctCode"]);
				lbl_GLAccount_Bank.Text = Function.ToString(chooseItem["AcctName"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bte_GLAccount_Bank_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Tab && string.IsNullOrEmpty(bte_GLAccount_Bank.Text))
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, false, bte_GLAccount_Bank.Text, "APZ_CFL_GetGLAccount", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				DataRow chooseItem = frm_CFL2.ChooseItem;
				if (chooseItem != null && chooseItem.IsExistsColumn("AcctCode"))
				{
					bte_GLAccount_Bank.Text = Function.ToString(chooseItem["AcctCode"]);
					lbl_GLAccount_Bank.Text = Function.ToString(chooseItem["AcctName"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void bte_BPCode_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, false, bte_BPCode.Text.Split(',')[0], "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			string text = "";
			string text2 = "";
			if (chooseItem != null && chooseItem.IsExistsColumn("CardCode"))
			{
				text = Function.ToString(chooseItem["CardCode"]);
				text2 = Function.ToString(chooseItem["CardName"]);
			}
			bte_BPCode.Text = text;
			txt_CardName.Text = text2;
			if (Function.ToString(chooseItem["Currency"]).Equals("##"))
			{
				WinFuntions._selectItemComboBoxByValue(cb_Currency, GlobalData.MainCurrency);
				cb_Currency.ReadOnly = false;
			}
			else
			{
				WinFuntions._selectItemComboBoxByValue(cb_Currency, Function.ToString(chooseItem["Currency"]));
				cb_Currency.ReadOnly = true;
			}
			DataTable dataTable = _httpClient.BusinessMaster.ShowContactPerson(text);
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit
			{
				NullText = "",
				ValueMember = "CntctCode",
				DisplayMember = "Name",
				BestFitMode = BestFitMode.BestFitResizePopup
			};
			if (dataTable.IsNotNull())
			{
				repositoryItemLookUpEdit.DataSource = dataTable.Copy();
				repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo("CntctCode", "Code"));
				repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo("Name", "Name"));
			}
			gc_ContactPerson.ColumnEdit = repositoryItemLookUpEdit;
			DataTable addressByCardCode = _httpClient.BusinessMaster.GetAddressByCardCode(text);
			RepositoryItemLookUpEdit repositoryItemLookUpEdit2 = new RepositoryItemLookUpEdit
			{
				NullText = "",
				ValueMember = "Address",
				DisplayMember = "Street",
				BestFitMode = BestFitMode.BestFitResizePopup
			};
			if (addressByCardCode.IsNotNull())
			{
				repositoryItemLookUpEdit2.DataSource = addressByCardCode.Copy();
				repositoryItemLookUpEdit2.Columns.Add(new LookUpColumnInfo("Address", "Code"));
				repositoryItemLookUpEdit2.Columns.Add(new LookUpColumnInfo("Street", "Name"));
			}
			gc_BillTo.ColumnEdit = repositoryItemLookUpEdit2;
			grd_Details.DataSource = new DataTable();
			DataTable dataTable2 = await _httpClient.BusinessMaster.GetADoc(text);
			if (dataTable2.IsNotNull())
			{
				DataTable dataTable3 = CreateTableDoc();
				DataRow dataRow = dataTable3.NewRow();
				dataRow["DocDate"] = DateTime.Today;
				dataRow["TaxDate"] = DateTime.Today;
				dataRow["ContactPerson"] = dataTable2.Rows[0]["CntctCode"];
				dataRow["BillTo"] = dataTable2.Rows[0]["BillToDef"];
				dataTable3.Rows.Add(dataRow);
				grd_Details.DataSource = dataTable3;
				grd_Details.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void bte_BPCode_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Tab && string.IsNullOrEmpty(bte_BPCode.Text))
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, false, bte_BPCode.Text.Split(',')[0], "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				DataRow chooseItem = frm_CFL2.ChooseItem;
				string text = "";
				string text2 = "";
				if (chooseItem != null && chooseItem.IsExistsColumn("CardCode"))
				{
					text = Function.ToString(chooseItem["CardCode"]);
					text2 = Function.ToString(chooseItem["CardName"]);
				}
				bte_BPCode.Text = text;
				txt_CardName.Text = text2;
				if (Function.ToString(chooseItem["Currency"]).Equals("##"))
				{
					WinFuntions._selectItemComboBoxByValue(cb_Currency, GlobalData.MainCurrency);
					cb_Currency.ReadOnly = false;
				}
				else
				{
					WinFuntions._selectItemComboBoxByValue(cb_Currency, Function.ToString(chooseItem["Currency"]));
					cb_Currency.ReadOnly = true;
				}
				_dtContact = _httpClient.BusinessMaster.ShowContactPerson(text);
				RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit
				{
					NullText = "",
					DisplayMember = "Name",
					ValueMember = "CntctCode",
					BestFitMode = BestFitMode.BestFitResizePopup
				};
				if (_dtContact.IsNotNull())
				{
					repositoryItemLookUpEdit.DataSource = _dtContact.Copy();
					repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo("CntctCode", 20, "Code"));
					repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo("Name", 20, "Name"));
				}
				gc_ContactPerson.ColumnEdit = repositoryItemLookUpEdit;
				DataTable dataTable = _httpClient.BusinessMaster.ShowContactPerson(text);
				RepositoryItemLookUpEdit repositoryItemLookUpEdit2 = new RepositoryItemLookUpEdit
				{
					NullText = "",
					DisplayMember = "Street",
					ValueMember = "Address",
					BestFitMode = BestFitMode.BestFitResizePopup
				};
				if (dataTable.IsNotNull())
				{
					repositoryItemLookUpEdit2.DataSource = dataTable.Copy();
					repositoryItemLookUpEdit2.Columns.Add(new LookUpColumnInfo("Address", 20, "Code"));
					repositoryItemLookUpEdit2.Columns.Add(new LookUpColumnInfo("Street", 20, "Name"));
				}
				gc_BillTo.ColumnEdit = repositoryItemLookUpEdit2;
				grd_Details.DataSource = new DataTable();
				DataTable dataTable2 = await _httpClient.BusinessMaster.GetADoc(text);
				if (dataTable2.IsNotNull())
				{
					DataTable dataTable3 = CreateTableDoc();
					DataRow dataRow = dataTable3.NewRow();
					dataRow["DocDate"] = DateTime.Today;
					dataRow["TaxDate"] = DateTime.Today;
					dataRow["ContactPerson"] = dataTable2.Rows[0]["CntctCode"];
					dataRow["BillTo"] = dataTable2.Rows[0]["BillToDef"];
					dataTable3.Rows.Add(dataRow);
					grd_Details.DataSource = dataTable3;
					grd_Details.RefreshDataSource();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void LoadCurrency()
	{
		try
		{
			WinFuntions._addComboBox(cb_Currency, GlobalData.Currencies, _selectFirstItem: false);
			WinFuntions._selectItemComboBoxByValue(cb_Currency, GlobalData.MainCurrency);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cb_Currency_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			if (cb_Currency.SelectedIndex >= 0)
			{
				string text = Function.ToString(((Common.ListItems)cb_Currency.SelectedItem).Value);
				if (text == GlobalData.MainCurrency)
				{
					cal_Rate.EditValue = 0;
					cal_Rate.ReadOnly = true;
					return;
				}
				DateTime date = Function.ParseDateTimes(gv_Details.GetFocusedRowCellValue("DocDate"));
				double exchangeRate = _httpClient.ExRateAndIndex.GetExchangeRate(text, date);
				cal_Rate.EditValue = exchangeRate;
				cal_Rate.ReadOnly = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btn_PayCash_Click(object sender, EventArgs e)
	{
		isCaculate = true;
		try
		{
			cal_Total_Bank.EditValue = 0;
			cal_Total_Cash.EditValue = cal_Amount.EditValue;
			xtab_PayMean.SelectedTabPage = tab_Cash;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		isCaculate = false;
	}

	private void btn_PayBank_Click(object sender, EventArgs e)
	{
		isCaculate = true;
		try
		{
			cal_Total_Cash.EditValue = 0;
			cal_Total_Bank.EditValue = cal_Amount.EditValue;
			xtab_PayMean.SelectedTabPage = tab_Bank;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		isCaculate = false;
	}

	private void cal_Total_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!isCaculate)
			{
				decimal num = Function.ParseDecimal(cal_Total_Cash.EditValue);
				decimal num2 = Function.ParseDecimal(cal_Total_Bank.EditValue);
				cal_Amount.EditValue = num + num2;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (grd_Details.DataSourceTable.IsNotNull() && XtraMessageBox.Show(Resources.xtraMsg_SaveChange, Resources.Msg, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No)
			{
				WinCommons._OpenProcessing();
				DataTable dataSourceTable = grd_Details.DataSourceTable;
				List<IncomingPaymentsImpl> list = new List<IncomingPaymentsImpl>();
				IncomingPaymentsImpl incomingPaymentsImpl = new IncomingPaymentsImpl();
				incomingPaymentsImpl.DocType = "C";
				incomingPaymentsImpl.CardCode = bte_BPCode.Text;
				incomingPaymentsImpl.CardName = txt_CardName.Text;
				incomingPaymentsImpl.ContactPersonCode = Function.ParseInt(dataSourceTable.Rows[0]["ContactPerson"]);
				incomingPaymentsImpl.DocDate = Function.ParseDateTimes(dataSourceTable.Rows[0]["DocDate"]);
				incomingPaymentsImpl.TaxDate = Function.ParseDateTimes(dataSourceTable.Rows[0]["TaxDate"]);
				incomingPaymentsImpl.PayToCode = Function.ToString(dataSourceTable.Rows[0]["BillTo"]);
				if (!string.IsNullOrEmpty(incomingPaymentsImpl.PayToCode))
				{
					incomingPaymentsImpl.Address = Function.ToString(gv_Details.GetRowCellDisplayText(0, gc_BillTo));
				}
				incomingPaymentsImpl.DocCurrency = Function.ToString(((Common.ListItems)cb_Currency.SelectedItem).Value);
				if (Function.ParseDecimal(cal_Rate.EditValue) > 0m)
				{
					incomingPaymentsImpl.DocRate = Function.ParseDecimal(cal_Rate.EditValue);
				}
				incomingPaymentsImpl.BPLId = GlobalData.DefaultBranch;
				if (Function.ParseDecimal(cal_Total_Cash.EditValue) > 0m)
				{
					incomingPaymentsImpl.CashAccount = bte_GLAccount_Cash.Text;
					incomingPaymentsImpl.CashSum = Function.ParseDecimal(cal_Total_Cash.EditValue);
				}
				else if (Function.ParseDecimal(cal_Total_Bank.EditValue) > 0m)
				{
					incomingPaymentsImpl.TrsfrAcct = bte_GLAccount_Bank.Text;
					incomingPaymentsImpl.TrsfrSum = Function.ParseDecimal(cal_Total_Bank.EditValue);
					incomingPaymentsImpl.TrsfrDate = incomingPaymentsImpl.DocDate;
				}
				list.Add(incomingPaymentsImpl);
				HttpResult httpResult = await _httpClient.IncommingPayments.Save(list);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					return;
				}
				WinCommons.Success(Enums.MessageCode.SuccessWithDocNo, Function.ToString(httpResult.Content));
				ResetForm();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable CreateTableDoc()
	{
		DataTable dataTable = new DataTable();
		try
		{
			dataTable.Columns.Add("DocDate", typeof(DateTime));
			dataTable.Columns.Add("TaxDate", typeof(DateTime));
			dataTable.Columns.Add("ContactPerson", typeof(int));
			dataTable.Columns.Add("BillTo", typeof(string));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return dataTable;
	}

	private void ResetForm()
	{
		try
		{
			txt_CardName.Text = "";
			bte_BPCode.Text = "";
			bte_BPCode.Focus();
			grd_Details.DataSourceTable.Rows.Clear();
			cal_Amount.EditValue = 0;
			cal_OpenBalance.EditValue = 0;
			cal_AmountDue.EditValue = 0;
			cal_Total_Cash.EditValue = 0;
			cal_Total_Bank.EditValue = 0;
			bte_GLAccount_Cash.Text = "";
			bte_GLAccount_Bank.Text = "";
			xtab_PayMean.SelectedTabPage = tab_Cash;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cal_Amount_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			cal_AmountDue.EditValue = cal_Amount.EditValue;
			cal_OpenBalance.EditValue = cal_Amount.EditValue;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Payment.frm_AdvancePaymentWizard));
		this.bte_BPCode = new DevExpress.XtraEditors.ButtonEdit();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.cal_Amount = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine12 = new Apzon.Controls.Controls.LabelLine();
		this.cal_AmountDue = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.cal_OpenBalance = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.cal_Rate = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.cb_Currency = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.btn_PayBank = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_PayCash = new Apzon.Controls.Controls.SimpleButtonEx();
		this.xtab_PayMean = new DevExpress.XtraTab.XtraTabControl();
		this.tab_Cash = new DevExpress.XtraTab.XtraTabPage();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		this.lbl_GLAccount_Cash = new DevExpress.XtraEditors.LabelControl();
		this.cal_Total_Cash = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.bte_GLAccount_Cash = new DevExpress.XtraEditors.ButtonEdit();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.tab_Bank = new DevExpress.XtraTab.XtraTabPage();
		this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
		this.lbl_GLAccount_Bank = new DevExpress.XtraEditors.LabelControl();
		this.cal_Total_Bank = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.bte_GLAccount_Bank = new DevExpress.XtraEditors.ButtonEdit();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Details = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Details = new Apzon.Controls.Grid.GridViewEx();
		this.gc_DocDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx1 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gc_TaxDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ContactPerson = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_BillTo = new Apzon.Controls.Grid.GridColumnEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.txt_CardName = new Apzon.Controls.Editor.TextEditEx();
		((System.ComponentModel.ISupportInitialize)this.bte_BPCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cal_Amount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cal_AmountDue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cal_OpenBalance.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cal_Rate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Currency.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtab_PayMean).BeginInit();
		this.xtab_PayMean.SuspendLayout();
		this.tab_Cash.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cal_Total_Cash.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_GLAccount_Cash.Properties).BeginInit();
		this.tab_Bank.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl4).BeginInit();
		this.panelControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cal_Total_Bank.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_GLAccount_Bank.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Details).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Details).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CardName.Properties).BeginInit();
		base.SuspendLayout();
		this.bte_BPCode.Location = new System.Drawing.Point(93, 12);
		this.bte_BPCode.Name = "bte_BPCode";
		this.bte_BPCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_BPCode.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_BPCode_Properties_ButtonClick);
		this.bte_BPCode.Size = new System.Drawing.Size(160, 20);
		this.bte_BPCode.TabIndex = 76;
		this.bte_BPCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.bte_BPCode_KeyDown);
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "BP Code";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(12, 12);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(79, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 75;
		this.labelLine2.Utext = "BP Code";
		this.cal_Amount.EditControl = null;
		this.cal_Amount.FieldName = null;
		this.cal_Amount.IsSearchControl = false;
		this.cal_Amount.Location = new System.Drawing.Point(93, 64);
		this.cal_Amount.Name = "cal_Amount";
		this.cal_Amount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_Amount.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.cal_Amount.Properties.IsSearchControl = false;
		this.cal_Amount.SettingInVisible = false;
		this.cal_Amount.SettingReadonly = false;
		this.cal_Amount.Size = new System.Drawing.Size(160, 20);
		this.cal_Amount.TabIndex = 78;
		this.cal_Amount.EditValueChanged += new System.EventHandler(this.cal_Amount_EditValueChanged);
		this.labelLine12.EditControl = null;
		this.labelLine12.EditValue = "Amount";
		this.labelLine12.FieldName = null;
		this.labelLine12.HideWhenEmpty = false;
		this.labelLine12.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine12.Location = new System.Drawing.Point(12, 64);
		this.labelLine12.Name = "labelLine12";
		this.labelLine12.ReadOnly = false;
		this.labelLine12.SettingInVisible = false;
		this.labelLine12.SettingReadonly = false;
		this.labelLine12.ShowUnderLine = true;
		this.labelLine12.Size = new System.Drawing.Size(79, 20);
		this.labelLine12.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine12.TabIndex = 77;
		this.labelLine12.Utext = "Amount";
		this.cal_AmountDue.EditControl = null;
		this.cal_AmountDue.FieldName = null;
		this.cal_AmountDue.IsSearchControl = false;
		this.cal_AmountDue.Location = new System.Drawing.Point(333, 64);
		this.cal_AmountDue.Name = "cal_AmountDue";
		this.cal_AmountDue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_AmountDue.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.cal_AmountDue.Properties.IsSearchControl = false;
		this.cal_AmountDue.Properties.ReadOnly = true;
		this.cal_AmountDue.SettingInVisible = false;
		this.cal_AmountDue.SettingReadonly = false;
		this.cal_AmountDue.Size = new System.Drawing.Size(160, 20);
		this.cal_AmountDue.TabIndex = 80;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Amount Due";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(259, 64);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(81, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 79;
		this.labelLine1.Utext = "Amount Due";
		this.cal_OpenBalance.EditControl = null;
		this.cal_OpenBalance.FieldName = null;
		this.cal_OpenBalance.IsSearchControl = false;
		this.cal_OpenBalance.Location = new System.Drawing.Point(596, 61);
		this.cal_OpenBalance.Name = "cal_OpenBalance";
		this.cal_OpenBalance.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_OpenBalance.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.cal_OpenBalance.Properties.IsSearchControl = false;
		this.cal_OpenBalance.Properties.ReadOnly = true;
		this.cal_OpenBalance.SettingInVisible = false;
		this.cal_OpenBalance.SettingReadonly = false;
		this.cal_OpenBalance.Size = new System.Drawing.Size(156, 20);
		this.cal_OpenBalance.TabIndex = 82;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Open Balance";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(512, 64);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(78, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 81;
		this.labelLine3.Utext = "Open Balance";
		this.cal_Rate.EditControl = null;
		this.cal_Rate.FieldName = null;
		this.cal_Rate.IsSearchControl = false;
		this.cal_Rate.Location = new System.Drawing.Point(93, 130);
		this.cal_Rate.Name = "cal_Rate";
		this.cal_Rate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_Rate.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.cal_Rate.Properties.IsSearchControl = false;
		this.cal_Rate.SettingInVisible = false;
		this.cal_Rate.SettingReadonly = false;
		this.cal_Rate.Size = new System.Drawing.Size(160, 20);
		this.cal_Rate.TabIndex = 86;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Rate";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(12, 130);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(79, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 85;
		this.labelLine5.Utext = "Rate";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Currency";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(12, 103);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(79, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 84;
		this.labelLine4.Utext = "Currency";
		this.cb_Currency.EditControl = null;
		this.cb_Currency.FieldName = null;
		this.cb_Currency.IsSearchControl = false;
		this.cb_Currency.Location = new System.Drawing.Point(93, 103);
		this.cb_Currency.Name = "cb_Currency";
		this.cb_Currency.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_Currency.SettingInVisible = false;
		this.cb_Currency.SettingReadonly = false;
		this.cb_Currency.Size = new System.Drawing.Size(160, 20);
		this.cb_Currency.TabIndex = 83;
		this.cb_Currency.SelectedIndexChanged += new System.EventHandler(this.cb_Currency_SelectedIndexChanged);
		this.btn_PayBank.EditControl = null;
		this.btn_PayBank.EditValue = "Pay all by BANK TRANSFER";
		this.btn_PayBank.FieldName = null;
		this.btn_PayBank.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_PayBank.ImageOptions.Image");
		this.btn_PayBank.IsSearchControl = false;
		this.btn_PayBank.Location = new System.Drawing.Point(333, 129);
		this.btn_PayBank.Name = "btn_PayBank";
		this.btn_PayBank.ReadOnly = false;
		this.btn_PayBank.SettingInVisible = false;
		this.btn_PayBank.SettingReadonly = false;
		this.btn_PayBank.Size = new System.Drawing.Size(158, 23);
		this.btn_PayBank.TabIndex = 88;
		this.btn_PayBank.Text = "Pay all by BANK TRANSFER";
		this.btn_PayBank.Click += new System.EventHandler(this.btn_PayBank_Click);
		this.btn_PayCash.EditControl = null;
		this.btn_PayCash.EditValue = "Pay it all in CASH";
		this.btn_PayCash.FieldName = null;
		this.btn_PayCash.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_PayCash.ImageOptions.Image");
		this.btn_PayCash.IsSearchControl = false;
		this.btn_PayCash.Location = new System.Drawing.Point(333, 100);
		this.btn_PayCash.Name = "btn_PayCash";
		this.btn_PayCash.ReadOnly = false;
		this.btn_PayCash.SettingInVisible = false;
		this.btn_PayCash.SettingReadonly = false;
		this.btn_PayCash.Size = new System.Drawing.Size(158, 23);
		this.btn_PayCash.TabIndex = 87;
		this.btn_PayCash.Text = "Pay it all in CASH";
		this.btn_PayCash.Click += new System.EventHandler(this.btn_PayCash_Click);
		this.xtab_PayMean.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtab_PayMean.Location = new System.Drawing.Point(8, 169);
		this.xtab_PayMean.Name = "xtab_PayMean";
		this.xtab_PayMean.SelectedTabPage = this.tab_Cash;
		this.xtab_PayMean.Size = new System.Drawing.Size(749, 110);
		this.xtab_PayMean.TabIndex = 89;
		this.xtab_PayMean.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.tab_Cash, this.tab_Bank });
		this.tab_Cash.Controls.Add(this.panelControl3);
		this.tab_Cash.Name = "tab_Cash";
		this.tab_Cash.Size = new System.Drawing.Size(747, 85);
		this.tab_Cash.Text = "Cash";
		this.panelControl3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panelControl3.Controls.Add(this.lbl_GLAccount_Cash);
		this.panelControl3.Controls.Add(this.cal_Total_Cash);
		this.panelControl3.Controls.Add(this.labelLine9);
		this.panelControl3.Controls.Add(this.bte_GLAccount_Cash);
		this.panelControl3.Controls.Add(this.labelLine8);
		this.panelControl3.Location = new System.Drawing.Point(0, 0);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(743, 82);
		this.panelControl3.TabIndex = 0;
		this.lbl_GLAccount_Cash.Location = new System.Drawing.Point(269, 19);
		this.lbl_GLAccount_Cash.Name = "lbl_GLAccount_Cash";
		this.lbl_GLAccount_Cash.Size = new System.Drawing.Size(0, 13);
		this.lbl_GLAccount_Cash.TabIndex = 78;
		this.cal_Total_Cash.EditControl = null;
		this.cal_Total_Cash.FieldName = null;
		this.cal_Total_Cash.IsSearchControl = false;
		this.cal_Total_Cash.Location = new System.Drawing.Point(125, 42);
		this.cal_Total_Cash.Name = "cal_Total_Cash";
		this.cal_Total_Cash.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_Total_Cash.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.cal_Total_Cash.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.cal_Total_Cash.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.cal_Total_Cash.Properties.IsSearchControl = false;
		this.cal_Total_Cash.SettingInVisible = false;
		this.cal_Total_Cash.SettingReadonly = false;
		this.cal_Total_Cash.Size = new System.Drawing.Size(138, 20);
		this.cal_Total_Cash.TabIndex = 77;
		this.cal_Total_Cash.EditValueChanged += new System.EventHandler(this.cal_Total_EditValueChanged);
		this.labelLine9.EditControl = null;
		this.labelLine9.EditValue = "Total Cash";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(15, 42);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ReadOnly = false;
		this.labelLine9.SettingInVisible = false;
		this.labelLine9.SettingReadonly = false;
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(110, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 76;
		this.labelLine9.Utext = "Total Cash";
		this.bte_GLAccount_Cash.Location = new System.Drawing.Point(125, 16);
		this.bte_GLAccount_Cash.Name = "bte_GLAccount_Cash";
		this.bte_GLAccount_Cash.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_GLAccount_Cash.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_GLAccount_Cash_Properties_ButtonClick);
		this.bte_GLAccount_Cash.Size = new System.Drawing.Size(138, 20);
		this.bte_GLAccount_Cash.TabIndex = 75;
		this.bte_GLAccount_Cash.KeyDown += new System.Windows.Forms.KeyEventHandler(this.bte_GLAccount_Cash_KeyDown);
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "G/L Account";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(15, 16);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(110, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 67;
		this.labelLine8.Utext = "G/L Account";
		this.tab_Bank.Controls.Add(this.panelControl4);
		this.tab_Bank.Name = "tab_Bank";
		this.tab_Bank.Size = new System.Drawing.Size(747, 85);
		this.tab_Bank.Text = "Bank Transfer";
		this.panelControl4.Controls.Add(this.lbl_GLAccount_Bank);
		this.panelControl4.Controls.Add(this.cal_Total_Bank);
		this.panelControl4.Controls.Add(this.labelLine10);
		this.panelControl4.Controls.Add(this.bte_GLAccount_Bank);
		this.panelControl4.Controls.Add(this.labelLine11);
		this.panelControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl4.Location = new System.Drawing.Point(0, 0);
		this.panelControl4.Name = "panelControl4";
		this.panelControl4.Size = new System.Drawing.Size(747, 85);
		this.panelControl4.TabIndex = 1;
		this.lbl_GLAccount_Bank.Location = new System.Drawing.Point(272, 19);
		this.lbl_GLAccount_Bank.Name = "lbl_GLAccount_Bank";
		this.lbl_GLAccount_Bank.Size = new System.Drawing.Size(0, 13);
		this.lbl_GLAccount_Bank.TabIndex = 83;
		this.cal_Total_Bank.EditControl = null;
		this.cal_Total_Bank.FieldName = null;
		this.cal_Total_Bank.IsSearchControl = false;
		this.cal_Total_Bank.Location = new System.Drawing.Point(125, 42);
		this.cal_Total_Bank.Name = "cal_Total_Bank";
		this.cal_Total_Bank.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_Total_Bank.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.cal_Total_Bank.Properties.IsSearchControl = false;
		this.cal_Total_Bank.SettingInVisible = false;
		this.cal_Total_Bank.SettingReadonly = false;
		this.cal_Total_Bank.Size = new System.Drawing.Size(138, 20);
		this.cal_Total_Bank.TabIndex = 82;
		this.cal_Total_Bank.EditValueChanged += new System.EventHandler(this.cal_Total_EditValueChanged);
		this.labelLine10.EditControl = null;
		this.labelLine10.EditValue = "Total Bank Transfer";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(15, 42);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ReadOnly = false;
		this.labelLine10.SettingInVisible = false;
		this.labelLine10.SettingReadonly = false;
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(110, 20);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 81;
		this.labelLine10.Utext = "Total Bank Transfer";
		this.bte_GLAccount_Bank.Location = new System.Drawing.Point(125, 16);
		this.bte_GLAccount_Bank.Name = "bte_GLAccount_Bank";
		this.bte_GLAccount_Bank.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_GLAccount_Bank.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_GLAccount_Bank_Properties_ButtonClick);
		this.bte_GLAccount_Bank.Size = new System.Drawing.Size(138, 20);
		this.bte_GLAccount_Bank.TabIndex = 80;
		this.bte_GLAccount_Bank.KeyDown += new System.Windows.Forms.KeyEventHandler(this.bte_GLAccount_Bank_KeyDown);
		this.labelLine11.EditControl = null;
		this.labelLine11.EditValue = "G/L Account";
		this.labelLine11.FieldName = null;
		this.labelLine11.HideWhenEmpty = false;
		this.labelLine11.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine11.Location = new System.Drawing.Point(15, 16);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.ReadOnly = false;
		this.labelLine11.SettingInVisible = false;
		this.labelLine11.SettingReadonly = false;
		this.labelLine11.ShowUnderLine = true;
		this.labelLine11.Size = new System.Drawing.Size(110, 20);
		this.labelLine11.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine11.TabIndex = 79;
		this.labelLine11.Utext = "G/L Account";
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(93, 519);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 91;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.ToolTip = "Thoát";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Add;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Add";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(12, 519);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(75, 23);
		this.btn_Save.TabIndex = 90;
		this.btn_Save.Text = "Add";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.grd_Details.AllowPasteExcel = true;
		this.grd_Details.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Details.ClipboardData = null;
		this.grd_Details.Location = new System.Drawing.Point(8, 285);
		this.grd_Details.MainView = this.gv_Details;
		this.grd_Details.Name = "grd_Details";
		this.grd_Details.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemDateEditEx1 });
		this.grd_Details.Size = new System.Drawing.Size(744, 228);
		this.grd_Details.TabIndex = 92;
		this.grd_Details.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Details });
		this.gv_Details.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Details.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Details.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Details.ColumnKey = null;
		this.gv_Details.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gc_DocDate, this.gc_TaxDate, this.gc_ContactPerson, this.gc_BillTo });
		this.gv_Details.DisableFormSetting = false;
		this.gv_Details.DisplayException = false;
		this.gv_Details.EditColumn = null;
		this.gv_Details.FieldCurrency = null;
		this.gv_Details.FieldRate = null;
		this.gv_Details.FormId = "AdvancePaymentWizard";
		this.gv_Details.GridControl = this.grd_Details;
		this.gv_Details.IgnoreFilter = false;
		this.gv_Details.IndexEditRow = -1;
		this.gv_Details.IndicatorWidth = 35;
		this.gv_Details.IsApzTable = false;
		this.gv_Details.IsInsertNewRow = false;
		this.gv_Details.IsLoadingData = false;
		this.gv_Details.IsMappingContextAction = false;
		this.gv_Details.ItemId = "gv_Details";
		this.gv_Details.LstColumnAllowBatch = null;
		this.gv_Details.LstColumnNotAllowBatch = null;
		this.gv_Details.LstColumnNotEditable = null;
		this.gv_Details.LstColumNotSetting = null;
		this.gv_Details.Name = "gv_Details";
		this.gv_Details.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Details.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Details.OptionsPrint.AutoWidth = false;
		this.gv_Details.OptionsPrint.UsePrintStyles = false;
		this.gv_Details.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Details.OptionsView.ColumnAutoWidth = false;
		this.gv_Details.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Details.OptionsView.ShowGroupedColumns = true;
		this.gv_Details.OptionsView.ShowGroupPanel = false;
		this.gv_Details.UseBatchMod = false;
		this.gc_DocDate.Caption = "Posting Date";
		this.gc_DocDate.ColumnEdit = this.repositoryItemDateEditEx1;
		this.gc_DocDate.CurrencyDateField = null;
		this.gc_DocDate.CurrencyField = null;
		this.gc_DocDate.CurrencyFixed = null;
		this.gc_DocDate.CurrencyFixedRate = 0.0;
		this.gc_DocDate.CurrencyRateField = null;
		this.gc_DocDate.FieldName = "DocDate";
		this.gc_DocDate.Name = "gc_DocDate";
		this.gc_DocDate.PrimaryKey = false;
		this.gc_DocDate.UnitField = null;
		this.gc_DocDate.Visible = true;
		this.gc_DocDate.VisibleIndex = 0;
		this.gc_DocDate.Width = 100;
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
		this.gc_TaxDate.Caption = "Document Date";
		this.gc_TaxDate.ColumnEdit = this.repositoryItemDateEditEx1;
		this.gc_TaxDate.CurrencyDateField = null;
		this.gc_TaxDate.CurrencyField = null;
		this.gc_TaxDate.CurrencyFixed = null;
		this.gc_TaxDate.CurrencyFixedRate = 0.0;
		this.gc_TaxDate.CurrencyRateField = null;
		this.gc_TaxDate.FieldName = "TaxDate";
		this.gc_TaxDate.Name = "gc_TaxDate";
		this.gc_TaxDate.PrimaryKey = false;
		this.gc_TaxDate.UnitField = null;
		this.gc_TaxDate.Visible = true;
		this.gc_TaxDate.VisibleIndex = 1;
		this.gc_TaxDate.Width = 100;
		this.gc_ContactPerson.Caption = "Contact Person";
		this.gc_ContactPerson.CurrencyDateField = null;
		this.gc_ContactPerson.CurrencyField = null;
		this.gc_ContactPerson.CurrencyFixed = null;
		this.gc_ContactPerson.CurrencyFixedRate = 0.0;
		this.gc_ContactPerson.CurrencyRateField = null;
		this.gc_ContactPerson.DisplayFormat.FormatString = "n0";
		this.gc_ContactPerson.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gc_ContactPerson.FieldName = "ContactPerson";
		this.gc_ContactPerson.Name = "gc_ContactPerson";
		this.gc_ContactPerson.PrimaryKey = false;
		this.gc_ContactPerson.UnitField = null;
		this.gc_ContactPerson.Visible = true;
		this.gc_ContactPerson.VisibleIndex = 2;
		this.gc_ContactPerson.Width = 110;
		this.gc_BillTo.Caption = "Bill To";
		this.gc_BillTo.CurrencyDateField = null;
		this.gc_BillTo.CurrencyField = null;
		this.gc_BillTo.CurrencyFixed = null;
		this.gc_BillTo.CurrencyFixedRate = 0.0;
		this.gc_BillTo.CurrencyRateField = null;
		this.gc_BillTo.FieldName = "BillTo";
		this.gc_BillTo.Name = "gc_BillTo";
		this.gc_BillTo.PrimaryKey = false;
		this.gc_BillTo.UnitField = null;
		this.gc_BillTo.Visible = true;
		this.gc_BillTo.VisibleIndex = 3;
		this.gc_BillTo.Width = 250;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "BP Name";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(12, 38);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(79, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 93;
		this.labelLine6.Utext = "BP Name";
		this.txt_CardName.EditControl = null;
		this.txt_CardName.FieldName = null;
		this.txt_CardName.IsSearchControl = false;
		this.txt_CardName.Location = new System.Drawing.Point(93, 38);
		this.txt_CardName.Name = "txt_CardName";
		this.txt_CardName.SettingInVisible = false;
		this.txt_CardName.SettingReadonly = false;
		this.txt_CardName.Size = new System.Drawing.Size(423, 20);
		this.txt_CardName.TabIndex = 95;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(760, 554);
		base.Controls.Add(this.txt_CardName);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.grd_Details);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.xtab_PayMean);
		base.Controls.Add(this.btn_PayBank);
		base.Controls.Add(this.btn_PayCash);
		base.Controls.Add(this.cal_Rate);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.cb_Currency);
		base.Controls.Add(this.cal_OpenBalance);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.cal_AmountDue);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.cal_Amount);
		base.Controls.Add(this.labelLine12);
		base.Controls.Add(this.bte_BPCode);
		base.Controls.Add(this.labelLine2);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AdvancePaymentWizard";
		base.ShowInTaskbar = false;
		this.Text = "Advance Payment Wizard";
		base.Load += new System.EventHandler(this.frm_AdvancePaymentWizard_Load);
		((System.ComponentModel.ISupportInitialize)this.bte_BPCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cal_Amount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cal_AmountDue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cal_OpenBalance.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cal_Rate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Currency.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtab_PayMean).EndInit();
		this.xtab_PayMean.ResumeLayout(false);
		this.tab_Cash.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		this.panelControl3.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.cal_Total_Cash.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_GLAccount_Cash.Properties).EndInit();
		this.tab_Bank.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl4).EndInit();
		this.panelControl4.ResumeLayout(false);
		this.panelControl4.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.cal_Total_Bank.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_GLAccount_Bank.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Details).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Details).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CardName.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
