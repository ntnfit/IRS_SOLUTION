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
using Apzon.Entities.Models.Payments;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using SAPbobsCOM;

namespace Apzon.Forms.Payment;

public class frm_IncomingPaymentWizard : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public new ApzObjectTypes FormObjectType = ApzObjectTypes.oIncomingPayments;

	private DataTable dtSearch;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Invoice;

	private GridViewEx gv_Invoice;

	private GridControlEx grd_Payment;

	private GridViewEx gv_Payment;

	private XtraTabControl xtraTabControl1;

	private XtraTabPage xtraTabPage1;

	private SplitContainerControl splitContainerControl2;

	private SplitContainerControl splitContainerControl3;

	private XtraTabControl xtraTabControl2;

	private XtraTabPage xtraTabPage2;

	private GridControlEx grd_Details;

	private GridViewEx gv_Details;

	private XtraTabControl xtraTabControl3;

	private XtraTabPage xtraTabPage3;

	private XtraTabControl xtraTabControl4;

	private XtraTabPage xtraTabPage4;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private DateEditEx date_ToDate;

	private DateEditEx date_FromDate;

	private LabelLine labelLine1;

	private LabelLine lb_NoOfTrans;

	private ButtonEdit bte_BPCode;

	private CalcEditEx txt_InvoiceNo;

	private SimpleButtonEx btn_Search;

	private PanelControl panelControl1;

	private SimpleButtonEx btn_Save;

	private SimpleButtonEx btn_Cancel;

	private SplitContainerControl splitContainerControl4;

	private PanelControl panelControl2;

	private SimpleButtonEx btn_PreviousFull;

	private SimpleButtonEx btn_Previous;

	private SimpleButtonEx btn_Next;

	private SimpleButtonEx btn_NextFull;

	private LabelLine labelLine4;

	private ComboBoxEditEx cb_Currency;

	private CalcEditEx cal_BankCharge;

	private LabelLine labelLine7;

	private CalcEditEx cal_BalanceDue;

	private LabelLine labelLine6;

	private CalcEditEx cal_OverallAmt;

	private LabelLine labelLine5;

	private XtraTabControl xtab_PayMean;

	private XtraTabPage tab_Cash;

	private PanelControl panelControl3;

	private XtraTabPage tab_Bank;

	private PanelControl panelControl4;

	private CalcEditEx cal_Total_Cash;

	private LabelLine labelLine9;

	private ButtonEdit bte_GLAccount_Cash;

	private LabelLine labelLine8;

	private LabelControl lbl_GLAccount_Cash;

	private LabelControl lbl_GLAccount_Bank;

	private CalcEditEx cal_Total_Bank;

	private LabelLine labelLine10;

	private ButtonEdit bte_GLAccount_Bank;

	private LabelLine labelLine11;

	private PanelControl panelControl5;

	private CalcEditEx cal_OpenBalanceDue;

	private LabelLine labelLine13;

	private CalcEditEx cal_TotalAmountDue;

	private LabelLine labelLine12;

	private SimpleButtonEx btn_PayCash;

	private SimpleButtonEx btn_PayBank;

	private CalcEditEx cal_Rate;

	private LabelLine labelLine14;

	public frm_IncomingPaymentWizard()
	{
		InitializeComponent();
		OnLoadAsync += frm_IncomingPaymentWizard_Load;
	}

	private async Task frm_IncomingPaymentWizard_Load(object sender, EventArgs e)
	{
		_ = 5;
		try
		{
			LoadCurrency();
			CreateDataSearch();
			bte_GLAccount_Bank.ReadOnly = true;
			cal_Total_Bank.ReadOnly = true;
			bte_GLAccount_Cash.ReadOnly = true;
			cal_Total_Cash.ReadOnly = true;
			DataRow dataRow = dtSearch.NewRow();
			dtSearch.Rows.Add(dataRow);
			dataRow["userName"] = GlobalData.UserName;
			dataRow["branch"] = GlobalData.DefaultBranch;
			DataSet ds = await _httpClient.PaymentWizardClient.GetData(dtSearch);
			if (ds.IsNotNull())
			{
				await GridViewHelper.GenerateGrid(grd_Invoice, gv_Invoice, ds);
			}
			DataSet dsPayment = await _httpClient.PaymentWizardClient.GetPaymentInfo(dtSearch);
			if (dsPayment.IsNotNull())
			{
				await GridViewHelper.GenerateGrid(grd_Payment, gv_Payment, dsPayment);
			}
			DataSet dsDetails = await _httpClient.PaymentWizardClient.GetDetails(dtSearch);
			if (dsDetails.IsNotNull())
			{
				await GridViewHelper.GenerateGrid(grd_Details, gv_Details, dsDetails);
			}
			ds.DisposeDataSet();
			dsPayment.DisposeDataSet();
			dsDetails.DisposeDataSet();
			bte_BPCode.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bte_BPCode_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, true, bte_BPCode.Text.Split(',')[0], "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			List<DataRow> chooseItems = frm_CFL2.ChooseItems;
			string text = "";
			if (chooseItems != null && chooseItems.Count > 0 && chooseItems[0].IsExistsColumn("CardCode"))
			{
				foreach (DataRow item in chooseItems)
				{
					text = text + "," + item[0];
				}
				text = ((!string.IsNullOrEmpty(text) && text.StartsWith(",")) ? text.Substring(1, text.Length - 1) : "");
			}
			bte_BPCode.Text = text;
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
			if (e.KeyCode != Keys.Tab || !string.IsNullOrEmpty(bte_BPCode.Text))
			{
				return;
			}
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, true, bte_BPCode.Text.Split(',')[0], "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			List<DataRow> chooseItems = frm_CFL2.ChooseItems;
			string text = "";
			if (chooseItems != null && chooseItems.Count > 0 && chooseItems[0].IsExistsColumn("CardCode"))
			{
				foreach (DataRow item in chooseItems)
				{
					text = text + "," + item[0];
				}
				text = ((!string.IsNullOrEmpty(text) && text.StartsWith(",")) ? text.Substring(1, text.Length - 1) : "");
			}
			bte_BPCode.Text = text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
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

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		await Search();
	}

	private async Task Search()
	{
		_ = 1;
		try
		{
			btn_Search.Enabled = false;
			btn_Save.Enabled = false;
			WinCommons._OpenProcessing();
			grd_Payment.DataSource = new DataTable();
			grd_Details.DataSource = new DataTable();
			CreateDataSearch();
			DataRow dataRow = dtSearch.NewRow();
			dtSearch.Rows.Add(dataRow);
			dataRow["bpCode"] = Function.ToString(bte_BPCode.Text);
			dataRow["fromdate"] = (string.IsNullOrEmpty(date_FromDate.Text) ? DateTime.Today : Convert.ToDateTime(date_FromDate.EditValue));
			dataRow["todate"] = (string.IsNullOrEmpty(date_ToDate.Text) ? DateTime.Today : Convert.ToDateTime(date_ToDate.EditValue));
			dataRow["invoiceNo"] = Function.ParseInt(txt_InvoiceNo.EditValue);
			dataRow["userName"] = GlobalData.UserName;
			dataRow["branch"] = GlobalData.DefaultBranch;
			DataSet ds = await _httpClient.PaymentWizardClient.GetData(dtSearch);
			if (ds.IsNotNull())
			{
				await GridViewHelper.GenerateGrid(grd_Invoice, gv_Invoice, ds);
			}
			ds.Tables.Clear();
			ds.Dispose();
			if (grd_Invoice.DataSourceTable.IsNotNull())
			{
				btn_NextFull.Enabled = true;
				btn_Next.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_Save.Enabled = true;
		btn_Search.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void CreateDataSearch()
	{
		if (!dtSearch.IsNotNull() || dtSearch.Columns.Count == 0)
		{
			dtSearch = new DataTable();
			dtSearch.Columns.Add(new DataColumn("bpCode", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("fromdate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("todate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("invoiceNo", typeof(int)));
			dtSearch.Columns.Add(new DataColumn("userName", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("branch", typeof(int)));
		}
		else
		{
			dtSearch.Rows.Clear();
		}
	}

	private async void gv_Payment_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Payment.GetRowCellValue(gv_Payment.FocusedRowHandle, "CardCode"));
			if (string.IsNullOrEmpty(text) || gv_Details.Columns.Count <= 0 || !gv_Details.Columns.Any((GridColumn m) => m.FieldName.Equals("CardCode")))
			{
				return;
			}
			gv_Details.ActiveFilterString = $"CardCode='{text}'";
			gv_Details.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
			DataTable dataTable = await _httpClient.BusinessMaster.GetADoc(text);
			if (!dataTable.IsNotNull())
			{
				return;
			}
			if (Function.ToString(dataTable.Rows[0]["Currency"]).Equals("##"))
			{
				cb_Currency.ReadOnly = false;
				string value = Function.ToString(gv_Payment.GetRowCellValue(gv_Payment.FocusedRowHandle, "Currency"));
				if (string.IsNullOrEmpty(value))
				{
					WinFuntions._selectItemComboBoxByValue(cb_Currency, GlobalData.MainCurrency);
				}
				else
				{
					WinFuntions._selectItemComboBoxByValue(cb_Currency, value);
				}
			}
			else
			{
				WinFuntions._selectItemComboBoxByValue(cb_Currency, Function.ToString(dataTable.Rows[0]["Currency"]));
				cb_Currency.ReadOnly = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_NextFull_Click(object sender, EventArgs e)
	{
		try
		{
			if (!grd_Invoice.DatasourceTable().IsNotNull())
			{
				return;
			}
			int[] selectedRows = gv_Invoice.GetSelectedRows();
			if (selectedRows.Length == 0)
			{
				WinCommons.Error(Enums.MessageCode.None, "Bạn cần chọn ít nhất một dòng.");
				return;
			}
			DataTable dataTable = grd_Invoice.DatasourceTable();
			DataTable dataTable2 = grd_Payment.DataSourceTable();
			DataTable dataTable3 = grd_Details.DataSourceTable();
			if (!dataTable2.IsNotNull())
			{
				dataTable2 = dataTable.Clone();
			}
			if (!dataTable3.IsNotNull())
			{
				dataTable3 = dataTable.Clone();
			}
			List<string> list = new List<string>();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				DataRow dataRow = gv_Invoice.GetDataRow(selectedRows[i]);
				string cardCode = Function.ToString(dataRow["CardCode"]);
				if (!dataTable2.AsEnumerable().Any((DataRow m) => m["CardCode"].Equals(cardCode)))
				{
					DataRow dataRow2 = dataTable2.NewRow();
					dataRow2.ItemArray = dataRow.ItemArray;
					dataRow2["DocDate"] = DateTime.Today;
					dataTable2.Rows.Add(dataRow2);
				}
				else
				{
					list.Add(cardCode);
				}
				DataRow dataRow3 = dataTable3.NewRow();
				dataRow3.ItemArray = dataRow.ItemArray;
				dataTable3.Rows.Add(dataRow3);
			}
			gv_Invoice.DeleteSelectedRows();
			foreach (string cardCode2 in list)
			{
				double num = (from m in dataTable3.AsEnumerable()
					where m["CardCode"].Equals(cardCode2)
					select m).Sum((DataRow t) => Function.ParseDouble(t["Total"]));
				double num2 = (from m in dataTable3.AsEnumerable()
					where m["CardCode"].Equals(cardCode2)
					select m).Sum((DataRow t) => Function.ParseDouble(t["TotalPayment"]));
				double num3 = (from m in dataTable3.AsEnumerable()
					where m["CardCode"].Equals(cardCode2)
					select m).Sum((DataRow t) => Function.ParseDouble(t["BalanceDue"]));
				DataRow dataRow4 = dataTable2.AsEnumerable().SingleOrDefault((DataRow m) => m["CardCode"].Equals(cardCode2));
				dataRow4["Total"] = num;
				dataRow4["TotalPayment"] = num2;
				dataRow4["BalanceDue"] = num3;
			}
			grd_Payment.DataSource = dataTable2;
			grd_Payment.RefreshDataSource();
			grd_Details.DataSource = dataTable3;
			grd_Details.RefreshDataSource();
			CalculatorTotalAmount();
			if (grd_Invoice.DataSourceTable.Rows.Count > 0)
			{
				btn_Next.Enabled = true;
				btn_NextFull.Enabled = true;
				btn_Previous.Enabled = true;
				btn_PreviousFull.Enabled = true;
			}
			else
			{
				btn_NextFull.Enabled = false;
				btn_Next.Enabled = false;
				btn_Previous.Enabled = true;
				btn_PreviousFull.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Next_Click(object sender, EventArgs e)
	{
		try
		{
			if (!grd_Invoice.DatasourceTable().IsNotNull())
			{
				return;
			}
			int[] selectedRows = gv_Invoice.GetSelectedRows();
			if (selectedRows.Length == 0)
			{
				WinCommons.Error(Enums.MessageCode.None, "Bạn cần chọn ít nhất một dòng.");
				return;
			}
			DataTable dataTable = grd_Invoice.DatasourceTable();
			DataTable dataTable2 = grd_Payment.DataSourceTable();
			DataTable dataTable3 = grd_Details.DataSourceTable();
			if (!dataTable2.IsNotNull())
			{
				dataTable2 = dataTable.Clone();
			}
			if (!dataTable3.IsNotNull())
			{
				dataTable3 = dataTable.Clone();
			}
			DataRow dataRow = gv_Invoice.GetDataRow(selectedRows[0]);
			DataRow dataRow2 = dataTable3.NewRow();
			dataRow2.ItemArray = dataRow.ItemArray;
			dataTable3.Rows.Add(dataRow2);
			string cardCode = Function.ToString(dataRow["CardCode"]);
			if (!dataTable2.AsEnumerable().Any((DataRow m) => m["CardCode"].Equals(cardCode)))
			{
				DataRow dataRow3 = dataTable2.NewRow();
				dataRow3.ItemArray = dataRow.ItemArray;
				dataTable2.Rows.Add(dataRow3);
			}
			else
			{
				double num = (from m in dataTable3.AsEnumerable()
					where m["CardCode"].Equals(cardCode)
					select m).Sum((DataRow t) => Function.ParseDouble(t["Total"]));
				double num2 = (from m in dataTable3.AsEnumerable()
					where m["CardCode"].Equals(cardCode)
					select m).Sum((DataRow t) => Function.ParseDouble(t["TotalPayment"]));
				double num3 = (from m in dataTable3.AsEnumerable()
					where m["CardCode"].Equals(cardCode)
					select m).Sum((DataRow t) => Function.ParseDouble(t["BalanceDue"]));
				DataRow dataRow4 = dataTable2.AsEnumerable().SingleOrDefault((DataRow m) => m["CardCode"].Equals(cardCode));
				dataRow4["Total"] = num;
				dataRow4["TotalPayment"] = num2;
				dataRow4["BalanceDue"] = num3;
			}
			gv_Invoice.DeleteRow(selectedRows[0]);
			grd_Payment.DataSource = dataTable2;
			grd_Payment.RefreshDataSource();
			grd_Details.DataSource = dataTable3;
			grd_Details.RefreshDataSource();
			CalculatorTotalAmount();
			if (grd_Invoice.DataSourceTable.Rows.Count > 0)
			{
				btn_Next.Enabled = true;
				btn_Previous.Enabled = true;
				btn_PreviousFull.Enabled = true;
			}
			else
			{
				btn_NextFull.Enabled = false;
				btn_Next.Enabled = false;
				btn_Previous.Enabled = true;
				btn_PreviousFull.Enabled = true;
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

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btn_PayCash_Click(object sender, EventArgs e)
	{
		try
		{
			bte_GLAccount_Bank.Text = "";
			lbl_GLAccount_Bank.Text = "";
			cal_Total_Bank.EditValue = 0;
			cal_Total_Cash.EditValue = cal_TotalAmountDue.EditValue;
			xtab_PayMean.SelectedTabPage = tab_Cash;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_PayBank_Click(object sender, EventArgs e)
	{
		try
		{
			bte_GLAccount_Cash.Text = "";
			lbl_GLAccount_Cash.Text = "";
			cal_Total_Cash.EditValue = 0;
			cal_Total_Bank.EditValue = cal_TotalAmountDue.EditValue;
			xtab_PayMean.SelectedTabPage = tab_Bank;
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
			if (!grd_Payment.DataSourceTable.IsNotNull() || !grd_Details.DataSourceTable.IsNotNull() || XtraMessageBox.Show(Resources.xtraMsg_SaveChange, Resources.Msg, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataTable dataSourceTable = grd_Payment.DataSourceTable;
			DataTable dataSourceTable2 = grd_Details.DataSourceTable;
			List<IncomingPaymentsImpl> list = new List<IncomingPaymentsImpl>();
			foreach (DataRow row in dataSourceTable.Rows)
			{
				IncomingPaymentsImpl incomingPaymentsImpl = new IncomingPaymentsImpl();
				incomingPaymentsImpl.DocType = "C";
				incomingPaymentsImpl.CardCode = Function.ToString(row["CardCode"]);
				incomingPaymentsImpl.CardCode = Function.ToString(row["CardCode"]);
				incomingPaymentsImpl.DocDate = Function.ParseDateTimes(row["DocDate"]);
				incomingPaymentsImpl.TaxDate = Function.ParseDateTimes(row["TaxDate"]);
				incomingPaymentsImpl.PayToCode = Function.ToString(row["PayToCode"]);
				incomingPaymentsImpl.DocTotal = Function.ParseDecimal(row["TotalPayment"]);
				incomingPaymentsImpl.DocCurrency = Function.ToString(((Common.ListItems)cb_Currency.SelectedItem).Value);
				if (Function.ParseDecimal(cal_Rate.EditValue) > 0m)
				{
					incomingPaymentsImpl.DocRate = Function.ParseDecimal(cal_Rate.EditValue);
				}
				incomingPaymentsImpl.BPLId = GlobalData.DefaultBranch;
				if (Function.ParseDecimal(cal_Total_Cash.EditValue) > 0m)
				{
					incomingPaymentsImpl.CashAccount = bte_GLAccount_Cash.Text;
					incomingPaymentsImpl.CashSum = Function.ParseDecimal(row["TotalPayment"]);
				}
				else if (Function.ParseDecimal(cal_Total_Bank.EditValue) > 0m)
				{
					incomingPaymentsImpl.TrsfrAcct = bte_GLAccount_Bank.Text;
					incomingPaymentsImpl.TrsfrSum = Function.ParseDecimal(row["TotalPayment"]);
					incomingPaymentsImpl.TrsfrDate = incomingPaymentsImpl.DocDate;
				}
				if (incomingPaymentsImpl.DocCurrency != GlobalData.MainCurrency)
				{
					incomingPaymentsImpl.DocTotalFC = Function.ParseDecimal(row["TotalPayment"]);
					incomingPaymentsImpl.DocTotal = 0m;
				}
				incomingPaymentsImpl.Invoices = new List<Payments_InvoicesImpl>();
				DataRow[] array = dataSourceTable2.Select("CardCode='" + incomingPaymentsImpl.CardCode + "'");
				foreach (DataRow dataRow2 in array)
				{
					Payments_InvoicesImpl payments_InvoicesImpl = new Payments_InvoicesImpl();
					payments_InvoicesImpl.DocEntry = Function.ParseInt(dataRow2["DocEntry"]);
					payments_InvoicesImpl.InvType = BoRcptInvTypes.it_Invoice.ToString();
					if (incomingPaymentsImpl.DocCurrency != GlobalData.MainCurrency)
					{
						payments_InvoicesImpl.AppliedFC = Function.ParseDouble(dataRow2["TotalPayment"]);
					}
					else
					{
						payments_InvoicesImpl.SumApplied = Function.ParseDouble(dataRow2["TotalPayment"]);
					}
					incomingPaymentsImpl.Invoices.Add(payments_InvoicesImpl);
				}
				list.Add(incomingPaymentsImpl);
			}
			HttpResult httpResult = await _httpClient.IncommingPayments.Save(list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return;
			}
			WinCommons.Success(Enums.MessageCode.SuccessWithDocNo, Function.ToString(httpResult.Content));
			ResetForm();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cal_Total_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			decimal num = Function.ParseDecimal(cal_Total_Cash.EditValue);
			decimal num2 = Function.ParseDecimal(cal_Total_Bank.EditValue);
			decimal num3 = Function.ParseDecimal(cal_BankCharge.EditValue);
			cal_OverallAmt.EditValue = num + num2 - num3;
			Function.ParseDecimal(cal_TotalAmountDue.EditValue);
			cal_OpenBalanceDue.EditValue = 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CalculatorTotalAmount()
	{
		try
		{
			DataTable dataTable = grd_Payment.DatasourceTable();
			if (dataTable.IsNotNull())
			{
				double num = dataTable.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["TotalPayment"]));
				cal_TotalAmountDue.EditValue = num;
				if (Function.ParseDouble(cal_OverallAmt.EditValue) > 0.0)
				{
					cal_OverallAmt.EditValue = num;
				}
				if (Function.ParseDouble(cal_Total_Cash.EditValue) > 0.0)
				{
					cal_Total_Cash.EditValue = num;
				}
				if (Function.ParseDouble(cal_Total_Bank.EditValue) > 0.0)
				{
					cal_Total_Bank.EditValue = num;
				}
			}
			else
			{
				cal_TotalAmountDue.EditValue = 0;
				cal_OpenBalanceDue.EditValue = 0;
				cal_BankCharge.EditValue = 0;
				cal_Total_Cash.EditValue = 0;
				cal_Total_Bank.EditValue = 0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Payment_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if ("TotalPayment".Equals(e.Column.FieldName))
			{
				double num = Function.ParseDouble(gv_Payment.GetRowCellValue(e.RowHandle, "TotalPayment"));
				double num2 = Function.ParseDouble(gv_Payment.GetRowCellValue(e.RowHandle, "Total"));
				gv_Payment.SetRowCellValue(e.RowHandle, "BalanceDue", num - num2);
				CalculatorTotalAmount();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Details_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if ("TotalPayment".Equals(e.Column.FieldName))
			{
				double num = Function.ParseDouble(gv_Details.GetRowCellValue(e.RowHandle, "TotalPayment"));
				double num2 = Function.ParseDouble(gv_Details.GetRowCellValue(e.RowHandle, "Total"));
				gv_Details.SetRowCellValue(e.RowHandle, "BalanceDue", num - num2);
				DataTable source = grd_Details.DataSourceTable();
				DataTable source2 = grd_Payment.DataSourceTable();
				string cardCode = Function.ToString(gv_Details.GetRowCellValue(e.RowHandle, "CardCode"));
				double num3 = (from m in source.AsEnumerable()
					where m["CardCode"].Equals(cardCode)
					select m).Sum((DataRow t) => Function.ParseDouble(t["TotalPayment"]));
				DataRow dataRow = source2.AsEnumerable().SingleOrDefault((DataRow m) => m["CardCode"].Equals(cardCode));
				dataRow["TotalPayment"] = num3;
				double num4 = Function.ParseDouble(dataRow["Total"]);
				dataRow["BalanceDue"] = num3 - num4;
				CalculatorTotalAmount();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ResetForm()
	{
		try
		{
			bte_BPCode.Focus();
			grd_Invoice.DataSourceTable.Rows.Clear();
			grd_Payment.DataSourceTable.Rows.Clear();
			grd_Details.DataSourceTable.Rows.Clear();
			btn_NextFull.Enabled = false;
			btn_Next.Enabled = false;
			btn_Previous.Enabled = false;
			btn_PreviousFull.Enabled = false;
			cal_TotalAmountDue.EditValue = 0;
			cal_OpenBalanceDue.EditValue = 0;
			cal_BankCharge.EditValue = 0;
			cal_Total_Cash.EditValue = 0;
			cal_Total_Bank.EditValue = 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_PreviousFull_Click(object sender, EventArgs e)
	{
		try
		{
			try
			{
				btn_NextFull.Enabled = true;
				btn_Next.Enabled = true;
				btn_Previous.Enabled = false;
				btn_PreviousFull.Enabled = false;
				DataTable dataTable = grd_Invoice.DatasourceTable();
				DataTable dataTable2 = grd_Payment.DataSourceTable();
				DataTable dataTable3 = grd_Details.DataSourceTable();
				if (dataTable2.IsNotNull() && dataTable3.IsNotNull())
				{
					for (int i = 0; i < dataTable3.Rows.Count; i++)
					{
						DataRow dataRow = dataTable3.Rows[i];
						DataRow dataRow2 = dataTable.NewRow();
						dataRow2.ItemArray = dataRow.ItemArray;
						dataTable.Rows.Add(dataRow2);
					}
					dataTable3.Rows.Clear();
					dataTable2.Rows.Clear();
					CalculatorTotalAmount();
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private void btn_Previous_Click(object sender, EventArgs e)
	{
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
				DateTime date = Function.ParseDateTimes(gv_Payment.GetFocusedRowCellValue("DocDate"));
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Payment.frm_IncomingPaymentWizard));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_InvoiceNo = new Apzon.Controls.Editor.CalcEditEx();
		this.bte_BPCode = new DevExpress.XtraEditors.ButtonEdit();
		this.date_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.lb_NoOfTrans = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.xtraTabControl4 = new DevExpress.XtraTab.XtraTabControl();
		this.xtraTabPage4 = new DevExpress.XtraTab.XtraTabPage();
		this.grd_Invoice = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Invoice = new Apzon.Controls.Grid.GridViewEx();
		this.grd_Payment = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Payment = new Apzon.Controls.Grid.GridViewEx();
		this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
		this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
		this.panelControl5 = new DevExpress.XtraEditors.PanelControl();
		this.cal_OpenBalanceDue = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine13 = new Apzon.Controls.Controls.LabelLine();
		this.cal_TotalAmountDue = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine12 = new Apzon.Controls.Controls.LabelLine();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.splitContainerControl3 = new DevExpress.XtraEditors.SplitContainerControl();
		this.xtraTabControl2 = new DevExpress.XtraTab.XtraTabControl();
		this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
		this.grd_Details = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Details = new Apzon.Controls.Grid.GridViewEx();
		this.xtraTabControl3 = new DevExpress.XtraTab.XtraTabControl();
		this.xtraTabPage3 = new DevExpress.XtraTab.XtraTabPage();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.cal_Rate = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine14 = new Apzon.Controls.Controls.LabelLine();
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
		this.cal_BankCharge = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.cal_BalanceDue = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.cal_OverallAmt = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.cb_Currency = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl4 = new DevExpress.XtraEditors.SplitContainerControl();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.btn_PreviousFull = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Previous = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Next = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_NextFull = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_InvoiceNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BPCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl4).BeginInit();
		this.xtraTabControl4.SuspendLayout();
		this.xtraTabPage4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Invoice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Invoice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Payment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Payment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl5).BeginInit();
		this.panelControl5.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cal_OpenBalanceDue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cal_TotalAmountDue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl3).BeginInit();
		this.splitContainerControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).BeginInit();
		this.xtraTabControl2.SuspendLayout();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Details).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Details).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl3).BeginInit();
		this.xtraTabControl3.SuspendLayout();
		this.xtraTabPage3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cal_Rate.Properties).BeginInit();
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
		((System.ComponentModel.ISupportInitialize)this.cal_BankCharge.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cal_BalanceDue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cal_OverallAmt.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Currency.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl4).BeginInit();
		this.splitContainerControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		base.SuspendLayout();
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.btn_Search);
		this.splitContainerControl1.Panel1.Controls.Add(this.txt_InvoiceNo);
		this.splitContainerControl1.Panel1.Controls.Add(this.bte_BPCode);
		this.splitContainerControl1.Panel1.Controls.Add(this.date_ToDate);
		this.splitContainerControl1.Panel1.Controls.Add(this.date_FromDate);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine1);
		this.splitContainerControl1.Panel1.Controls.Add(this.lb_NoOfTrans);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine3);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine2);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControl4);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(421, 636);
		this.splitContainerControl1.SplitterPosition = 138;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.btn_Search.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Search.Appearance.Options.UseBackColor = true;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(310, 95);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(99, 23);
		this.btn_Search.TabIndex = 77;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.txt_InvoiceNo.EditControl = null;
		this.txt_InvoiceNo.FieldName = null;
		this.txt_InvoiceNo.IsSearchControl = false;
		this.txt_InvoiceNo.Location = new System.Drawing.Point(94, 96);
		this.txt_InvoiceNo.Name = "txt_InvoiceNo";
		this.txt_InvoiceNo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_InvoiceNo.Properties.IsSearchControl = false;
		this.txt_InvoiceNo.SettingInVisible = false;
		this.txt_InvoiceNo.SettingReadonly = false;
		this.txt_InvoiceNo.Size = new System.Drawing.Size(197, 20);
		this.txt_InvoiceNo.TabIndex = 76;
		this.txt_InvoiceNo.ToolTip = "Số lượng tờ 1 gói";
		this.bte_BPCode.Location = new System.Drawing.Point(94, 20);
		this.bte_BPCode.Name = "bte_BPCode";
		this.bte_BPCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_BPCode.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_BPCode_Properties_ButtonClick);
		this.bte_BPCode.Size = new System.Drawing.Size(315, 20);
		this.bte_BPCode.TabIndex = 74;
		this.bte_BPCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.bte_BPCode_KeyDown);
		this.date_ToDate.EditControl = null;
		this.date_ToDate.EditValue = null;
		this.date_ToDate.FieldName = null;
		this.date_ToDate.Location = new System.Drawing.Point(94, 70);
		this.date_ToDate.Name = "date_ToDate";
		this.date_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.IsSearchControl = false;
		this.date_ToDate.SettingInVisible = false;
		this.date_ToDate.SettingReadonly = false;
		this.date_ToDate.Size = new System.Drawing.Size(197, 20);
		this.date_ToDate.TabIndex = 73;
		this.date_ToDate.EditValueChanged += new System.EventHandler(this.date_ToDate_EditValueChanged);
		this.date_FromDate.EditControl = null;
		this.date_FromDate.EditValue = null;
		this.date_FromDate.FieldName = null;
		this.date_FromDate.Location = new System.Drawing.Point(94, 44);
		this.date_FromDate.Name = "date_FromDate";
		this.date_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.IsSearchControl = false;
		this.date_FromDate.SettingInVisible = false;
		this.date_FromDate.SettingReadonly = false;
		this.date_FromDate.Size = new System.Drawing.Size(197, 20);
		this.date_FromDate.TabIndex = 72;
		this.date_FromDate.EditValueChanged += new System.EventHandler(this.date_FromDate_EditValueChanged);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "To Date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 72);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(79, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 71;
		this.labelLine1.Utext = "To Date";
		this.lb_NoOfTrans.EditControl = null;
		this.lb_NoOfTrans.EditValue = "From Date";
		this.lb_NoOfTrans.FieldName = null;
		this.lb_NoOfTrans.HideWhenEmpty = false;
		this.lb_NoOfTrans.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_NoOfTrans.Location = new System.Drawing.Point(13, 46);
		this.lb_NoOfTrans.Name = "lb_NoOfTrans";
		this.lb_NoOfTrans.ReadOnly = false;
		this.lb_NoOfTrans.SettingInVisible = false;
		this.lb_NoOfTrans.SettingReadonly = false;
		this.lb_NoOfTrans.ShowUnderLine = true;
		this.lb_NoOfTrans.Size = new System.Drawing.Size(79, 20);
		this.lb_NoOfTrans.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_NoOfTrans.TabIndex = 70;
		this.lb_NoOfTrans.Utext = "From Date";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Invoice No.";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(11, 98);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(81, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 69;
		this.labelLine3.Utext = "Invoice No.";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "BP Code";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(13, 20);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(79, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 66;
		this.labelLine2.Utext = "BP Code";
		this.xtraTabControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl4.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl4.Name = "xtraTabControl4";
		this.xtraTabControl4.SelectedTabPage = this.xtraTabPage4;
		this.xtraTabControl4.Size = new System.Drawing.Size(421, 488);
		this.xtraTabControl4.TabIndex = 5;
		this.xtraTabControl4.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtraTabPage4 });
		this.xtraTabPage4.Controls.Add(this.grd_Invoice);
		this.xtraTabPage4.Name = "xtraTabPage4";
		this.xtraTabPage4.Size = new System.Drawing.Size(419, 463);
		this.xtraTabPage4.Text = "List of Invoices";
		this.grd_Invoice.AllowPasteExcel = true;
		this.grd_Invoice.ClipboardData = null;
		this.grd_Invoice.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Invoice.Location = new System.Drawing.Point(0, 0);
		this.grd_Invoice.MainView = this.gv_Invoice;
		this.grd_Invoice.Name = "grd_Invoice";
		this.grd_Invoice.Size = new System.Drawing.Size(419, 463);
		this.grd_Invoice.TabIndex = 0;
		this.grd_Invoice.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Invoice });
		this.gv_Invoice.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Invoice.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Invoice.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Invoice.ColumnKey = null;
		this.gv_Invoice.DisableFormSetting = false;
		this.gv_Invoice.DisplayException = false;
		this.gv_Invoice.EditColumn = null;
		this.gv_Invoice.FieldCurrency = null;
		this.gv_Invoice.FieldRate = null;
		this.gv_Invoice.FormId = "IncomingPaymentWizard";
		this.gv_Invoice.GridControl = this.grd_Invoice;
		this.gv_Invoice.IgnoreFilter = false;
		this.gv_Invoice.IndexEditRow = -1;
		this.gv_Invoice.IndicatorWidth = 35;
		this.gv_Invoice.IsApzTable = false;
		this.gv_Invoice.IsInsertNewRow = false;
		this.gv_Invoice.IsLoadingData = false;
		this.gv_Invoice.IsMappingContextAction = false;
		this.gv_Invoice.ItemId = "gv_Invoice";
		this.gv_Invoice.LstColumnAllowBatch = null;
		this.gv_Invoice.LstColumnNotAllowBatch = null;
		this.gv_Invoice.LstColumnNotEditable = null;
		this.gv_Invoice.LstColumNotSetting = null;
		this.gv_Invoice.Name = "gv_Invoice";
		this.gv_Invoice.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Invoice.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Invoice.OptionsPrint.AutoWidth = false;
		this.gv_Invoice.OptionsPrint.UsePrintStyles = false;
		this.gv_Invoice.OptionsSelection.MultiSelect = true;
		this.gv_Invoice.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Invoice.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Invoice.OptionsView.ColumnAutoWidth = false;
		this.gv_Invoice.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Invoice.OptionsView.ShowGroupedColumns = true;
		this.gv_Invoice.OptionsView.ShowGroupPanel = false;
		this.gv_Invoice.UseBatchMod = false;
		this.grd_Payment.AllowPasteExcel = true;
		this.grd_Payment.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Payment.ClipboardData = null;
		this.grd_Payment.Location = new System.Drawing.Point(0, 0);
		this.grd_Payment.MainView = this.gv_Payment;
		this.grd_Payment.Name = "grd_Payment";
		this.grd_Payment.Size = new System.Drawing.Size(560, 138);
		this.grd_Payment.TabIndex = 1;
		this.grd_Payment.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Payment });
		this.gv_Payment.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Payment.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Payment.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Payment.ColumnKey = null;
		this.gv_Payment.DisableFormSetting = false;
		this.gv_Payment.DisplayException = false;
		this.gv_Payment.EditColumn = null;
		this.gv_Payment.FieldCurrency = null;
		this.gv_Payment.FieldRate = null;
		this.gv_Payment.FormId = "IncomingPaymentWizard";
		this.gv_Payment.GridControl = this.grd_Payment;
		this.gv_Payment.IgnoreFilter = false;
		this.gv_Payment.IndexEditRow = -1;
		this.gv_Payment.IndicatorWidth = 35;
		this.gv_Payment.IsApzTable = false;
		this.gv_Payment.IsInsertNewRow = false;
		this.gv_Payment.IsLoadingData = false;
		this.gv_Payment.IsMappingContextAction = false;
		this.gv_Payment.ItemId = "gv_Payment";
		this.gv_Payment.LstColumnAllowBatch = null;
		this.gv_Payment.LstColumnNotAllowBatch = null;
		this.gv_Payment.LstColumnNotEditable = null;
		this.gv_Payment.LstColumNotSetting = null;
		this.gv_Payment.Name = "gv_Payment";
		this.gv_Payment.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Payment.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Payment.OptionsPrint.AutoWidth = false;
		this.gv_Payment.OptionsPrint.UsePrintStyles = false;
		this.gv_Payment.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Payment.OptionsView.ColumnAutoWidth = false;
		this.gv_Payment.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Payment.OptionsView.ShowGroupedColumns = true;
		this.gv_Payment.OptionsView.ShowGroupPanel = false;
		this.gv_Payment.UseBatchMod = false;
		this.gv_Payment.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Payment_FocusedRowObjectChanged);
		this.gv_Payment.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Payment_CellValueChanged);
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl1.Size = new System.Drawing.Size(566, 196);
		this.xtraTabControl1.TabIndex = 2;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtraTabPage1 });
		this.xtraTabPage1.Controls.Add(this.panelControl5);
		this.xtraTabPage1.Controls.Add(this.grd_Payment);
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.Size = new System.Drawing.Size(564, 171);
		this.xtraTabPage1.Text = "Payment Info";
		this.panelControl5.Controls.Add(this.cal_OpenBalanceDue);
		this.panelControl5.Controls.Add(this.labelLine13);
		this.panelControl5.Controls.Add(this.cal_TotalAmountDue);
		this.panelControl5.Controls.Add(this.labelLine12);
		this.panelControl5.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl5.Location = new System.Drawing.Point(0, 141);
		this.panelControl5.Name = "panelControl5";
		this.panelControl5.Size = new System.Drawing.Size(564, 30);
		this.panelControl5.TabIndex = 2;
		this.cal_OpenBalanceDue.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.cal_OpenBalanceDue.EditControl = null;
		this.cal_OpenBalanceDue.FieldName = null;
		this.cal_OpenBalanceDue.IsSearchControl = false;
		this.cal_OpenBalanceDue.Location = new System.Drawing.Point(388, 5);
		this.cal_OpenBalanceDue.Name = "cal_OpenBalanceDue";
		this.cal_OpenBalanceDue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_OpenBalanceDue.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.cal_OpenBalanceDue.Properties.IsSearchControl = false;
		this.cal_OpenBalanceDue.Properties.ReadOnly = true;
		this.cal_OpenBalanceDue.SettingInVisible = false;
		this.cal_OpenBalanceDue.SettingReadonly = false;
		this.cal_OpenBalanceDue.Size = new System.Drawing.Size(160, 20);
		this.cal_OpenBalanceDue.TabIndex = 44;
		this.labelLine13.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine13.EditControl = null;
		this.labelLine13.EditValue = "Open Balance Due";
		this.labelLine13.FieldName = null;
		this.labelLine13.HideWhenEmpty = false;
		this.labelLine13.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine13.Location = new System.Drawing.Point(281, 5);
		this.labelLine13.Name = "labelLine13";
		this.labelLine13.ReadOnly = false;
		this.labelLine13.SettingInVisible = false;
		this.labelLine13.SettingReadonly = false;
		this.labelLine13.ShowUnderLine = true;
		this.labelLine13.Size = new System.Drawing.Size(101, 20);
		this.labelLine13.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine13.TabIndex = 43;
		this.labelLine13.Utext = "Open Balance Due";
		this.cal_TotalAmountDue.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.cal_TotalAmountDue.EditControl = null;
		this.cal_TotalAmountDue.FieldName = null;
		this.cal_TotalAmountDue.IsSearchControl = false;
		this.cal_TotalAmountDue.Location = new System.Drawing.Point(112, 5);
		this.cal_TotalAmountDue.Name = "cal_TotalAmountDue";
		this.cal_TotalAmountDue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_TotalAmountDue.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.cal_TotalAmountDue.Properties.IsSearchControl = false;
		this.cal_TotalAmountDue.SettingInVisible = false;
		this.cal_TotalAmountDue.SettingReadonly = false;
		this.cal_TotalAmountDue.Size = new System.Drawing.Size(160, 20);
		this.cal_TotalAmountDue.TabIndex = 42;
		this.cal_TotalAmountDue.EditValueChanged += new System.EventHandler(this.cal_Total_EditValueChanged);
		this.labelLine12.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine12.EditControl = null;
		this.labelLine12.EditValue = "Total Amount Due";
		this.labelLine12.FieldName = null;
		this.labelLine12.HideWhenEmpty = false;
		this.labelLine12.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine12.Location = new System.Drawing.Point(15, 8);
		this.labelLine12.Name = "labelLine12";
		this.labelLine12.ReadOnly = false;
		this.labelLine12.SettingInVisible = false;
		this.labelLine12.SettingReadonly = false;
		this.labelLine12.ShowUnderLine = true;
		this.labelLine12.Size = new System.Drawing.Size(97, 20);
		this.labelLine12.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine12.TabIndex = 41;
		this.labelLine12.Utext = "Total Amount Due";
		this.splitContainerControl2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(33, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.xtraTabControl1);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.splitContainerControl3);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(566, 636);
		this.splitContainerControl2.SplitterPosition = 196;
		this.splitContainerControl2.TabIndex = 2;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.splitContainerControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl3.Horizontal = false;
		this.splitContainerControl3.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl3.Name = "splitContainerControl3";
		this.splitContainerControl3.Panel1.Controls.Add(this.xtraTabControl2);
		this.splitContainerControl3.Panel1.Text = "Panel1";
		this.splitContainerControl3.Panel2.Controls.Add(this.xtraTabControl3);
		this.splitContainerControl3.Panel2.Text = "Panel2";
		this.splitContainerControl3.Size = new System.Drawing.Size(566, 430);
		this.splitContainerControl3.SplitterPosition = 170;
		this.splitContainerControl3.TabIndex = 0;
		this.splitContainerControl3.Text = "splitContainerControl3";
		this.xtraTabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl2.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl2.Name = "xtraTabControl2";
		this.xtraTabControl2.SelectedTabPage = this.xtraTabPage2;
		this.xtraTabControl2.Size = new System.Drawing.Size(566, 170);
		this.xtraTabControl2.TabIndex = 3;
		this.xtraTabControl2.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtraTabPage2 });
		this.xtraTabPage2.Controls.Add(this.grd_Details);
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.Size = new System.Drawing.Size(564, 145);
		this.xtraTabPage2.Text = "Details";
		this.grd_Details.AllowPasteExcel = true;
		this.grd_Details.ClipboardData = null;
		this.grd_Details.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Details.Location = new System.Drawing.Point(0, 0);
		this.grd_Details.MainView = this.gv_Details;
		this.grd_Details.Name = "grd_Details";
		this.grd_Details.Size = new System.Drawing.Size(564, 145);
		this.grd_Details.TabIndex = 1;
		this.grd_Details.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Details });
		this.gv_Details.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Details.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Details.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Details.ColumnKey = null;
		this.gv_Details.DisableFormSetting = false;
		this.gv_Details.DisplayException = false;
		this.gv_Details.EditColumn = null;
		this.gv_Details.FieldCurrency = null;
		this.gv_Details.FieldRate = null;
		this.gv_Details.FormId = "IncomingPaymentWizard";
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
		this.gv_Details.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Details_CellValueChanged);
		this.xtraTabControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl3.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl3.Name = "xtraTabControl3";
		this.xtraTabControl3.SelectedTabPage = this.xtraTabPage3;
		this.xtraTabControl3.Size = new System.Drawing.Size(566, 250);
		this.xtraTabControl3.TabIndex = 4;
		this.xtraTabControl3.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtraTabPage3 });
		this.xtraTabPage3.Controls.Add(this.panelControl1);
		this.xtraTabPage3.Name = "xtraTabPage3";
		this.xtraTabPage3.Size = new System.Drawing.Size(564, 225);
		this.xtraTabPage3.Text = "Payment Means";
		this.panelControl1.Controls.Add(this.cal_Rate);
		this.panelControl1.Controls.Add(this.labelLine14);
		this.panelControl1.Controls.Add(this.btn_PayBank);
		this.panelControl1.Controls.Add(this.btn_PayCash);
		this.panelControl1.Controls.Add(this.xtab_PayMean);
		this.panelControl1.Controls.Add(this.cal_BankCharge);
		this.panelControl1.Controls.Add(this.labelLine7);
		this.panelControl1.Controls.Add(this.cal_BalanceDue);
		this.panelControl1.Controls.Add(this.labelLine6);
		this.panelControl1.Controls.Add(this.cal_OverallAmt);
		this.panelControl1.Controls.Add(this.labelLine5);
		this.panelControl1.Controls.Add(this.labelLine4);
		this.panelControl1.Controls.Add(this.cb_Currency);
		this.panelControl1.Controls.Add(this.btn_Cancel);
		this.panelControl1.Controls.Add(this.btn_Save);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(564, 225);
		this.panelControl1.TabIndex = 0;
		this.cal_Rate.EditControl = null;
		this.cal_Rate.FieldName = null;
		this.cal_Rate.IsSearchControl = false;
		this.cal_Rate.Location = new System.Drawing.Point(67, 31);
		this.cal_Rate.Name = "cal_Rate";
		this.cal_Rate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_Rate.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.cal_Rate.Properties.IsSearchControl = false;
		this.cal_Rate.SettingInVisible = false;
		this.cal_Rate.SettingReadonly = false;
		this.cal_Rate.Size = new System.Drawing.Size(80, 20);
		this.cal_Rate.TabIndex = 88;
		this.labelLine14.EditControl = null;
		this.labelLine14.EditValue = "Rate";
		this.labelLine14.FieldName = null;
		this.labelLine14.HideWhenEmpty = false;
		this.labelLine14.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine14.Location = new System.Drawing.Point(8, 31);
		this.labelLine14.Name = "labelLine14";
		this.labelLine14.ReadOnly = false;
		this.labelLine14.SettingInVisible = false;
		this.labelLine14.SettingReadonly = false;
		this.labelLine14.ShowUnderLine = true;
		this.labelLine14.Size = new System.Drawing.Size(59, 20);
		this.labelLine14.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine14.TabIndex = 87;
		this.labelLine14.Utext = "Rate";
		this.btn_PayBank.EditControl = null;
		this.btn_PayBank.EditValue = "Pay all by BANK TRANSFER";
		this.btn_PayBank.FieldName = null;
		this.btn_PayBank.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_PayBank.ImageOptions.Image");
		this.btn_PayBank.IsSearchControl = false;
		this.btn_PayBank.Location = new System.Drawing.Point(399, 37);
		this.btn_PayBank.Name = "btn_PayBank";
		this.btn_PayBank.ReadOnly = false;
		this.btn_PayBank.SettingInVisible = false;
		this.btn_PayBank.SettingReadonly = false;
		this.btn_PayBank.Size = new System.Drawing.Size(158, 23);
		this.btn_PayBank.TabIndex = 47;
		this.btn_PayBank.Text = "Pay all by BANK TRANSFER";
		this.btn_PayBank.Click += new System.EventHandler(this.btn_PayBank_Click);
		this.btn_PayCash.EditControl = null;
		this.btn_PayCash.EditValue = "Pay it all in CASH";
		this.btn_PayCash.FieldName = null;
		this.btn_PayCash.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_PayCash.ImageOptions.Image");
		this.btn_PayCash.IsSearchControl = false;
		this.btn_PayCash.Location = new System.Drawing.Point(399, 8);
		this.btn_PayCash.Name = "btn_PayCash";
		this.btn_PayCash.ReadOnly = false;
		this.btn_PayCash.SettingInVisible = false;
		this.btn_PayCash.SettingReadonly = false;
		this.btn_PayCash.Size = new System.Drawing.Size(158, 23);
		this.btn_PayCash.TabIndex = 46;
		this.btn_PayCash.Text = "Pay it all in CASH";
		this.btn_PayCash.Click += new System.EventHandler(this.btn_PayCash_Click);
		this.xtab_PayMean.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtab_PayMean.Location = new System.Drawing.Point(14, 83);
		this.xtab_PayMean.Name = "xtab_PayMean";
		this.xtab_PayMean.SelectedTabPage = this.tab_Cash;
		this.xtab_PayMean.Size = new System.Drawing.Size(555, 103);
		this.xtab_PayMean.TabIndex = 45;
		this.xtab_PayMean.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.tab_Cash, this.tab_Bank });
		this.tab_Cash.Controls.Add(this.panelControl3);
		this.tab_Cash.Name = "tab_Cash";
		this.tab_Cash.Size = new System.Drawing.Size(553, 78);
		this.tab_Cash.Text = "Cash";
		this.panelControl3.Controls.Add(this.lbl_GLAccount_Cash);
		this.panelControl3.Controls.Add(this.cal_Total_Cash);
		this.panelControl3.Controls.Add(this.labelLine9);
		this.panelControl3.Controls.Add(this.bte_GLAccount_Cash);
		this.panelControl3.Controls.Add(this.labelLine8);
		this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl3.Location = new System.Drawing.Point(0, 0);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(553, 78);
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
		this.tab_Bank.Size = new System.Drawing.Size(553, 78);
		this.tab_Bank.Text = "Bank Transfer";
		this.panelControl4.Controls.Add(this.lbl_GLAccount_Bank);
		this.panelControl4.Controls.Add(this.cal_Total_Bank);
		this.panelControl4.Controls.Add(this.labelLine10);
		this.panelControl4.Controls.Add(this.bte_GLAccount_Bank);
		this.panelControl4.Controls.Add(this.labelLine11);
		this.panelControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl4.Location = new System.Drawing.Point(0, 0);
		this.panelControl4.Name = "panelControl4";
		this.panelControl4.Size = new System.Drawing.Size(553, 78);
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
		this.cal_BankCharge.EditControl = null;
		this.cal_BankCharge.FieldName = null;
		this.cal_BankCharge.IsSearchControl = false;
		this.cal_BankCharge.Location = new System.Drawing.Point(244, 57);
		this.cal_BankCharge.Name = "cal_BankCharge";
		this.cal_BankCharge.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_BankCharge.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.cal_BankCharge.Properties.IsSearchControl = false;
		this.cal_BankCharge.SettingInVisible = false;
		this.cal_BankCharge.SettingReadonly = false;
		this.cal_BankCharge.Size = new System.Drawing.Size(138, 20);
		this.cal_BankCharge.TabIndex = 44;
		this.cal_BankCharge.EditValueChanged += new System.EventHandler(this.cal_Total_EditValueChanged);
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Bank Charge";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(162, 57);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(82, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 43;
		this.labelLine7.Utext = "Bank Charge";
		this.cal_BalanceDue.EditControl = null;
		this.cal_BalanceDue.FieldName = null;
		this.cal_BalanceDue.IsSearchControl = false;
		this.cal_BalanceDue.Location = new System.Drawing.Point(244, 31);
		this.cal_BalanceDue.Name = "cal_BalanceDue";
		this.cal_BalanceDue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_BalanceDue.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.cal_BalanceDue.Properties.IsSearchControl = false;
		this.cal_BalanceDue.Properties.ReadOnly = true;
		this.cal_BalanceDue.SettingInVisible = false;
		this.cal_BalanceDue.SettingReadonly = false;
		this.cal_BalanceDue.Size = new System.Drawing.Size(138, 20);
		this.cal_BalanceDue.TabIndex = 42;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Balance Due";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(162, 31);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(82, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 41;
		this.labelLine6.Utext = "Balance Due";
		this.cal_OverallAmt.EditControl = null;
		this.cal_OverallAmt.FieldName = null;
		this.cal_OverallAmt.IsSearchControl = false;
		this.cal_OverallAmt.Location = new System.Drawing.Point(244, 5);
		this.cal_OverallAmt.Name = "cal_OverallAmt";
		this.cal_OverallAmt.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_OverallAmt.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.cal_OverallAmt.Properties.IsSearchControl = false;
		this.cal_OverallAmt.SettingInVisible = false;
		this.cal_OverallAmt.SettingReadonly = false;
		this.cal_OverallAmt.Size = new System.Drawing.Size(138, 20);
		this.cal_OverallAmt.TabIndex = 40;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Averall Amt";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(162, 5);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(82, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 39;
		this.labelLine5.Utext = "Averall Amt";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Currency";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(8, 6);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(59, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 12;
		this.labelLine4.Utext = "Currency";
		this.cb_Currency.EditControl = null;
		this.cb_Currency.FieldName = null;
		this.cb_Currency.IsSearchControl = false;
		this.cb_Currency.Location = new System.Drawing.Point(67, 5);
		this.cb_Currency.Name = "cb_Currency";
		this.cb_Currency.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_Currency.SettingInVisible = false;
		this.cb_Currency.SettingReadonly = false;
		this.cb_Currency.Size = new System.Drawing.Size(80, 20);
		this.cb_Currency.TabIndex = 11;
		this.cb_Currency.SelectedIndexChanged += new System.EventHandler(this.cb_Currency_SelectedIndexChanged);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(93, 192);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 10;
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
		this.btn_Save.Location = new System.Drawing.Point(12, 192);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(75, 23);
		this.btn_Save.TabIndex = 9;
		this.btn_Save.Text = "Add";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.splitContainerControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl4.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl4.Name = "splitContainerControl4";
		this.splitContainerControl4.Panel1.Controls.Add(this.splitContainerControl1);
		this.splitContainerControl4.Panel1.Text = "Panel1";
		this.splitContainerControl4.Panel2.Controls.Add(this.panelControl2);
		this.splitContainerControl4.Panel2.Controls.Add(this.splitContainerControl2);
		this.splitContainerControl4.Panel2.Text = "Panel2";
		this.splitContainerControl4.Size = new System.Drawing.Size(1025, 636);
		this.splitContainerControl4.SplitterPosition = 421;
		this.splitContainerControl4.TabIndex = 3;
		this.splitContainerControl4.Text = "splitContainerControl4";
		this.panelControl2.Controls.Add(this.btn_PreviousFull);
		this.panelControl2.Controls.Add(this.btn_Previous);
		this.panelControl2.Controls.Add(this.btn_Next);
		this.panelControl2.Controls.Add(this.btn_NextFull);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Left;
		this.panelControl2.Location = new System.Drawing.Point(0, 0);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(27, 636);
		this.panelControl2.TabIndex = 3;
		this.btn_PreviousFull.EditControl = null;
		this.btn_PreviousFull.EditValue = "";
		this.btn_PreviousFull.FieldName = null;
		this.btn_PreviousFull.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_PreviousFull.ImageOptions.Image");
		this.btn_PreviousFull.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.BottomCenter;
		this.btn_PreviousFull.IsSearchControl = false;
		this.btn_PreviousFull.Location = new System.Drawing.Point(1, 315);
		this.btn_PreviousFull.Name = "btn_PreviousFull";
		this.btn_PreviousFull.ReadOnly = false;
		this.btn_PreviousFull.SettingInVisible = false;
		this.btn_PreviousFull.SettingReadonly = false;
		this.btn_PreviousFull.Size = new System.Drawing.Size(26, 23);
		this.btn_PreviousFull.TabIndex = 3;
		this.btn_PreviousFull.Click += new System.EventHandler(this.btn_PreviousFull_Click);
		this.btn_Previous.EditControl = null;
		this.btn_Previous.EditValue = "";
		this.btn_Previous.FieldName = null;
		this.btn_Previous.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Previous.ImageOptions.Image");
		this.btn_Previous.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.BottomCenter;
		this.btn_Previous.IsSearchControl = false;
		this.btn_Previous.Location = new System.Drawing.Point(1, 286);
		this.btn_Previous.Name = "btn_Previous";
		this.btn_Previous.ReadOnly = false;
		this.btn_Previous.SettingInVisible = false;
		this.btn_Previous.SettingReadonly = false;
		this.btn_Previous.Size = new System.Drawing.Size(26, 23);
		this.btn_Previous.TabIndex = 2;
		this.btn_Previous.Click += new System.EventHandler(this.btn_Previous_Click);
		this.btn_Next.EditControl = null;
		this.btn_Next.EditValue = "";
		this.btn_Next.FieldName = null;
		this.btn_Next.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Next.ImageOptions.Image");
		this.btn_Next.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.BottomCenter;
		this.btn_Next.IsSearchControl = false;
		this.btn_Next.Location = new System.Drawing.Point(1, 257);
		this.btn_Next.Name = "btn_Next";
		this.btn_Next.ReadOnly = false;
		this.btn_Next.SettingInVisible = false;
		this.btn_Next.SettingReadonly = false;
		this.btn_Next.Size = new System.Drawing.Size(26, 23);
		this.btn_Next.TabIndex = 1;
		this.btn_Next.Click += new System.EventHandler(this.btn_Next_Click);
		this.btn_NextFull.EditControl = null;
		this.btn_NextFull.EditValue = "";
		this.btn_NextFull.FieldName = null;
		this.btn_NextFull.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_NextFull.ImageOptions.Image");
		this.btn_NextFull.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.BottomCenter;
		this.btn_NextFull.IsSearchControl = false;
		this.btn_NextFull.Location = new System.Drawing.Point(1, 228);
		this.btn_NextFull.Name = "btn_NextFull";
		this.btn_NextFull.ReadOnly = false;
		this.btn_NextFull.SettingInVisible = false;
		this.btn_NextFull.SettingReadonly = false;
		this.btn_NextFull.Size = new System.Drawing.Size(26, 23);
		this.btn_NextFull.TabIndex = 0;
		this.btn_NextFull.Click += new System.EventHandler(this.btn_NextFull_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1025, 636);
		base.Controls.Add(this.splitContainerControl4);
		base.FormId = "IncomingPaymentWizard";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_IncomingPaymentWizard";
		base.ShowInTaskbar = false;
		this.Text = "Incoming Payment Wizard";
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_InvoiceNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BPCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl4).EndInit();
		this.xtraTabControl4.ResumeLayout(false);
		this.xtraTabPage4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Invoice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Invoice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Payment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Payment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl5).EndInit();
		this.panelControl5.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cal_OpenBalanceDue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cal_TotalAmountDue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl3).EndInit();
		this.splitContainerControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).EndInit();
		this.xtraTabControl2.ResumeLayout(false);
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Details).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Details).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl3).EndInit();
		this.xtraTabControl3.ResumeLayout(false);
		this.xtraTabPage3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cal_Rate.Properties).EndInit();
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
		((System.ComponentModel.ISupportInitialize)this.cal_BankCharge.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cal_BalanceDue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cal_OverallAmt.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Currency.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl4).EndInit();
		this.splitContainerControl4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
