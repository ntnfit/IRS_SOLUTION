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
using Apzon.Controls.Grid;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Payments;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using SAPbobsCOM;

namespace Apzon.Forms.Banking;

public class frm_PaymentForInvoice : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataTable dtSearch;

	private int decimals;

	private IContainer components;

	private LabelLine labelLine1;

	private TextEditEx txt_InvoiceNo;

	private GridControlEx grd_Invoice;

	private GridViewEx gv_Invoice;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx tab_Payment;

	private XtraTabPageEx tab_ListOfInvoice;

	private DateEditEx date_to;

	private LabelLine labelLine3;

	private DateEditEx date_from;

	private LabelLine labelLine2;

	private LabelLine labelLine4;

	private GridControlEx grd_Report;

	private GridViewEx gv_Report;

	private TextEditEx txt_InvNo;

	private SimpleButtonEx btn_Search;

	private XtraTabPageEx tab_ListOfPayment;

	private SimpleButtonEx btn_Search_Payment;

	private GridControlEx grd_Payment;

	private GridViewEx gv_Payment;

	private TextEditEx txt_PaymentEntry;

	private LabelLine labelLine5;

	private DateEditEx pdate_To;

	private LabelLine labelLine6;

	private DateEditEx pdate_From;

	private LabelLine labelLine7;

	public string DocEntry { get; set; }

	public frm_PaymentForInvoice()
	{
		InitializeComponent();
	}

	private async void frm_PaymentForInvoice_Load(object sender, EventArgs e)
	{
		try
		{
			date_from.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			date_to.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
			pdate_From.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			pdate_To.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
			decimals = GlobalData.UnitPriceFormat.NumberDecimalDigits;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(GlobalData.MainCurrency.ToUpper()));
				if (currencyCodesImpl != null)
				{
					decimals = ((currencyCodesImpl.Decimals == -1) ? decimals : currencyCodesImpl.Decimals);
				}
			}
			await ResetForm();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void txt_InvoiceNo_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(txt_InvoiceNo.Text))
			{
				return;
			}
			string barcode = Function.ToString(txt_InvoiceNo.EditValue);
			DataTable dataTable = await _httpClient.IncommingPayments.GetByInvoiceNo(barcode);
			int invoiceNo = 0;
			if (dataTable.IsNotNull())
			{
				invoiceNo = Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
			}
			if (invoiceNo != 0)
			{
				DataTable dataSourceTable = grd_Invoice.DataSourceTable;
				if (dataSourceTable.IsNotNull())
				{
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						invoiceNo = Function.ParseInt(dataTable.Rows[i]["DocEntry"]);
						if ((from t in dataSourceTable.AsEnumerable()
							where Function.ParseInt(t["DocEntry"]).Equals(invoiceNo)
							select t).ToList().Count > 0)
						{
							txt_InvoiceNo.EditValue = string.Empty;
							txt_InvoiceNo.Focus();
							return;
						}
					}
				}
				if (dataTable.IsNotNull())
				{
					if (gv_Invoice.Columns.Count > 0)
					{
						if (dataSourceTable.IsNotNull())
						{
							for (int num = 0; num < dataTable.Rows.Count; num++)
							{
								DataRow dataRow = dataSourceTable.NewRow();
								dataRow.ItemArray = dataTable.Rows[num].ItemArray;
								dataSourceTable.Rows.Add(dataRow);
							}
						}
						else
						{
							BindNewData(dataTable);
						}
					}
					else
					{
						BindNewData(dataTable);
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
			txt_InvoiceNo.EditValue = string.Empty;
			txt_InvoiceNo.Enabled = true;
			txt_InvoiceNo.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			DataRow[] array = grd_Invoice.DataSourceTable.Select("Checked = 'True'");
			if (array.Length != 0)
			{
				double num = array.Sum((DataRow t) => Function.ParseDouble(t["TotalPayment"]));
				if (XtraMessageBoxEx.Show(Enums.MessageCode.PaymentForInvoiceConfirmPostAmount, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, string.Format("{0:n" + decimals + "}", num)) != DialogResult.Yes)
				{
					return;
				}
				WinCommons._OpenProcessing();
				DataTable source = array.CopyToDataTable();
				List<IncomingPaymentsImpl> list = new List<IncomingPaymentsImpl>();
				foreach (var item in from t in source.AsEnumerable()
					group t by new
					{
						cardcode = Function.ToString(t["CardCode"])
					} into g
					select new
					{
						Key = g.Key,
						Items = g
					})
				{
					IncomingPaymentsImpl incomingPaymentsImpl = new IncomingPaymentsImpl();
					incomingPaymentsImpl.DocType = "C";
					incomingPaymentsImpl.CardCode = Function.ToString(item.Key.cardcode);
					incomingPaymentsImpl.DocCurrency = GlobalData.MainCurrency;
					incomingPaymentsImpl.DocRate = 1m;
					incomingPaymentsImpl.CashAccount = ((GlobalData.UserDefaultGroup != null) ? GlobalData.UserDefaultGroup.CashAccount : "");
					incomingPaymentsImpl.CashSum = Function.ParseDecimal(item.Items.Sum((DataRow t) => Function.ParseDouble(t["TotalPayment"])));
					incomingPaymentsImpl.BPLId = GlobalData.DefaultBranch;
					incomingPaymentsImpl.Invoices = new List<Payments_InvoicesImpl>();
					foreach (DataRow item2 in item.Items)
					{
						Payments_InvoicesImpl payments_InvoicesImpl = new Payments_InvoicesImpl();
						payments_InvoicesImpl.DiscountPercent = Function.ParseDouble(item2["DiscountPercent"]);
						payments_InvoicesImpl.DocEntry = Function.ParseInt(item2["DocEntry"]);
						payments_InvoicesImpl.SumApplied = Function.ParseDouble(item2["TotalPayment"]);
						payments_InvoicesImpl.InvType = BoRcptInvTypes.it_Invoice.ToString();
						incomingPaymentsImpl.Invoices.Add(payments_InvoicesImpl);
					}
					list.Add(incomingPaymentsImpl);
				}
				HttpResult httpResult = await _httpClient.IncommingPayments.Save(list);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				else
				{
					DocEntry = Function.ToString(httpResult.Content);
					WinCommons.Success(Enums.MessageCode.SuccessWithDocNo, "", DocEntry);
					string[] array2 = Function.ToString(DocEntry).Split(',');
					string[] array3 = array2;
					foreach (string obj in array3)
					{
						await WinFuntions.Print("", Function.ParseInt(obj), "RCT1");
					}
					await ResetForm();
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

	public async void PRINTLAYOUT()
	{
		try
		{
			WinCommons._OpenProcessing();
			if (xtraTabControl1.SelectedTabPage == tab_ListOfPayment)
			{
				object focusedRowCellValue = gv_Payment.GetFocusedRowCellValue("DocEntry");
				await WinFuntions.PrintLayout("", focusedRowCellValue, "RCT1");
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PRINT()
	{
		try
		{
			WinCommons._OpenProcessing();
			if (xtraTabControl1.SelectedTabPage == tab_ListOfPayment)
			{
				int docEntry = Function.ParseInt(gv_Payment.GetFocusedRowCellValue("DocEntry"));
				await WinFuntions.Print("", docEntry, "RCT1");
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ResetForm()
	{
		try
		{
			txt_InvoiceNo.EditValue = string.Empty;
			txt_InvoiceNo.Focus();
			grd_Invoice.DataSource = null;
			grd_Invoice.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void BindNewData(DataTable dt)
	{
		grd_Invoice.DataSource = dt;
		grd_Invoice.RefreshDataSource();
		gv_Invoice.PopulateColumns();
		gv_Invoice.Columns["TotalPayment"].SummaryItem.FieldName = "TotalPayment";
		gv_Invoice.Columns["TotalPayment"].SummaryItem.SummaryType = SummaryItemType.Sum;
		gv_Invoice.Columns["TotalPayment"].SummaryItem.DisplayFormat = "{0:n" + ((GlobalData.TotalsAccuracyFormat != null) ? GlobalData.TotalsAccuracyFormat.CurrencyDecimalDigits.ToString() : "0") + "}";
		gv_Invoice.Columns["TotalPayment"].DisplayFormat.FormatType = FormatType.Numeric;
		gv_Invoice.Columns["TotalPayment"].DisplayFormat.FormatString = "n" + ((GlobalData.TotalsAccuracyFormat != null) ? GlobalData.TotalsAccuracyFormat.CurrencyDecimalDigits.ToString() : "0");
		gv_Invoice.Columns["DocTotal"].SummaryItem.FieldName = "DocTotal";
		gv_Invoice.Columns["DocTotal"].SummaryItem.SummaryType = SummaryItemType.Sum;
		gv_Invoice.Columns["DocTotal"].SummaryItem.DisplayFormat = "{0:n" + ((GlobalData.TotalsAccuracyFormat != null) ? GlobalData.TotalsAccuracyFormat.CurrencyDecimalDigits.ToString() : "0") + "}";
		gv_Invoice.Columns["DocTotal"].DisplayFormat.FormatType = FormatType.Numeric;
		gv_Invoice.Columns["DocTotal"].DisplayFormat.FormatString = "n" + ((GlobalData.TotalsAccuracyFormat != null) ? GlobalData.TotalsAccuracyFormat.CurrencyDecimalDigits.ToString() : "0");
		gv_Invoice.Columns["PaidSum"].SummaryItem.FieldName = "PaidSum";
		gv_Invoice.Columns["PaidSum"].SummaryItem.SummaryType = SummaryItemType.Sum;
		gv_Invoice.Columns["PaidSum"].SummaryItem.DisplayFormat = "{0:n" + ((GlobalData.TotalsAccuracyFormat != null) ? GlobalData.TotalsAccuracyFormat.CurrencyDecimalDigits.ToString() : "0") + "}";
		gv_Invoice.Columns["PaidSum"].DisplayFormat.FormatType = FormatType.Numeric;
		gv_Invoice.Columns["PaidSum"].DisplayFormat.FormatString = "n" + ((GlobalData.TotalsAccuracyFormat != null) ? GlobalData.TotalsAccuracyFormat.CurrencyDecimalDigits.ToString() : "0");
		gv_Invoice.Columns["BalanceDue"].SummaryItem.FieldName = "BalanceDue";
		gv_Invoice.Columns["BalanceDue"].SummaryItem.SummaryType = SummaryItemType.Sum;
		gv_Invoice.Columns["BalanceDue"].SummaryItem.DisplayFormat = "{0:n" + ((GlobalData.TotalsAccuracyFormat != null) ? GlobalData.TotalsAccuracyFormat.CurrencyDecimalDigits.ToString() : "0") + "}";
		gv_Invoice.Columns["BalanceDue"].DisplayFormat.FormatType = FormatType.Numeric;
		gv_Invoice.Columns["BalanceDue"].DisplayFormat.FormatString = "n" + ((GlobalData.TotalsAccuracyFormat != null) ? GlobalData.TotalsAccuracyFormat.CurrencyDecimalDigits.ToString() : "0");
		for (int i = 0; i < gv_Invoice.Columns.Count; i++)
		{
			GridColumn gridColumn = gv_Invoice.Columns[i];
			if (!gridColumn.FieldName.ToLower().Equals("totalpayment") && !gridColumn.FieldName.ToLower().Equals("discountpercent") && !gridColumn.FieldName.ToLower().Equals("checked"))
			{
				gridColumn.OptionsColumn.ReadOnly = true;
			}
		}
	}

	private void gv_Invoice_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
	}

	private void gv_Invoice_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (gv_Invoice.FocusedColumn.FieldName.ToLower().Equals("discountpercent"))
			{
				object oldEditValue = (sender as GridView).ActiveEditor.OldEditValue;
				object value = e.Value;
				double num = Function.ParseDouble(gv_Invoice.GetFocusedRowCellValue("TotalPayment")) / (1.0 - Function.ParseDouble(oldEditValue) / 100.0);
				double value2 = num - num * Function.ParseDouble(value) / 100.0;
				value2 = Math.Round(value2, decimals);
				gv_Invoice.SetFocusedRowCellValue("TotalPayment", value2);
				grd_Invoice.RefreshDataSource();
				grd_Invoice.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		await ResetForm();
	}

	private void txt_InvoiceNo_EditValueChanged(object sender, EventArgs e)
	{
	}

	private async void btn_Search_Click_1(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			DateTime? dateTime = Function.ParseDateTime(date_from.EditValue);
			DateTime? dateTime2 = Function.ParseDateTime(date_to.EditValue);
			string value = Function.ToString(txt_InvNo.EditValue);
			CreateDataSearch();
			DataRow dataRow = dtSearch.NewRow();
			dataRow["docentry"] = value;
			dataRow["fromDate"] = dateTime;
			dataRow["toDate"] = dateTime2;
			dataRow["userName"] = GlobalData.UserName;
			dataRow["branch"] = GlobalData.DefaultBranch;
			dtSearch.Rows.Add(dataRow);
			DataSet ds = await _httpClient.IncommingPayments.GetListInvoice(dtSearch);
			await GridViewHelper.GenerateGrid(grd_Report, gv_Report, ds, null, expand: true);
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Search_Payment_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			DateTime dateTime = (string.IsNullOrWhiteSpace(pdate_From.Text) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(pdate_From.EditValue));
			DateTime dateTime2 = (string.IsNullOrWhiteSpace(pdate_To.Text) ? DateTime.Today : Convert.ToDateTime(pdate_To.EditValue));
			string value = Function.ToString(txt_PaymentEntry.EditValue);
			CreateDataSearch();
			DataRow dataRow = dtSearch.NewRow();
			dataRow["docentry"] = value;
			dataRow["fromDate"] = dateTime;
			dataRow["toDate"] = dateTime2;
			dataRow["userName"] = GlobalData.UserName;
			dataRow["branch"] = GlobalData.DefaultBranch;
			dtSearch.Rows.Add(dataRow);
			DataSet ds = await _httpClient.IncommingPayments.GetListPayment(dtSearch);
			await GridViewHelper.GenerateGrid(grd_Payment, gv_Payment, ds, null, expand: true);
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CreateDataSearch()
	{
		if (!dtSearch.IsNotNull() || dtSearch.Columns.Count == 0)
		{
			dtSearch = new DataTable();
			dtSearch.Columns.Add(new DataColumn("docentry", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("fromDate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("toDate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("userName", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("branch", typeof(int)));
		}
		else
		{
			dtSearch.Rows.Clear();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Banking.frm_PaymentForInvoice));
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txt_InvoiceNo = new Apzon.Controls.Editor.TextEditEx();
		this.grd_Invoice = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Invoice = new Apzon.Controls.Grid.GridViewEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Payment = new Apzon.Controls.Controls.XtraTabPageEx();
		this.tab_ListOfPayment = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_Search_Payment = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Payment = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Payment = new Apzon.Controls.Grid.GridViewEx();
		this.txt_PaymentEntry = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.pdate_To = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.pdate_From = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.tab_ListOfInvoice = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Report = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Report = new Apzon.Controls.Grid.GridViewEx();
		this.txt_InvNo = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.date_to = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.date_from = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.txt_InvoiceNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Invoice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Invoice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.tab_Payment.SuspendLayout();
		this.tab_ListOfPayment.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Payment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Payment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PaymentEntry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pdate_To.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pdate_To.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pdate_From.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pdate_From.Properties).BeginInit();
		this.tab_ListOfInvoice.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Report).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Report).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_InvNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_to.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_to.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_from.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_from.Properties).BeginInit();
		base.SuspendLayout();
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Invoice No.";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(3, 3);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Invoice No.";
		this.txt_InvoiceNo.EditControl = null;
		this.txt_InvoiceNo.FieldName = null;
		this.txt_InvoiceNo.IsSearchControl = false;
		this.txt_InvoiceNo.Location = new System.Drawing.Point(84, 1);
		this.txt_InvoiceNo.Name = "txt_InvoiceNo";
		this.txt_InvoiceNo.SettingInVisible = false;
		this.txt_InvoiceNo.SettingReadonly = false;
		this.txt_InvoiceNo.Size = new System.Drawing.Size(391, 20);
		this.txt_InvoiceNo.TabIndex = 1;
		this.txt_InvoiceNo.EditValueChanged += new System.EventHandler(this.txt_InvoiceNo_EditValueChanged);
		this.txt_InvoiceNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_InvoiceNo_KeyDown);
		this.grd_Invoice.AllowPasteExcel = true;
		this.grd_Invoice.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Invoice.ClipboardData = null;
		this.grd_Invoice.Location = new System.Drawing.Point(3, 27);
		this.grd_Invoice.MainView = this.gv_Invoice;
		this.grd_Invoice.Name = "grd_Invoice";
		this.grd_Invoice.Size = new System.Drawing.Size(797, 306);
		this.grd_Invoice.TabIndex = 2;
		this.grd_Invoice.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Invoice });
		this.gv_Invoice.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Invoice.ColumnKey = null;
		this.gv_Invoice.DisableFormSetting = false;
		this.gv_Invoice.DisplayException = false;
		this.gv_Invoice.EditColumn = null;
		this.gv_Invoice.FieldCurrency = null;
		this.gv_Invoice.FieldRate = null;
		this.gv_Invoice.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
		this.gv_Invoice.FormId = "oPaymentForInvoice";
		this.gv_Invoice.GridControl = this.grd_Invoice;
		this.gv_Invoice.GroupSummary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Sum, "TotalPayment", null, "")
		});
		this.gv_Invoice.IgnoreFilter = false;
		this.gv_Invoice.IndexEditRow = -1;
		this.gv_Invoice.IndicatorWidth = 35;
		this.gv_Invoice.IsApzTable = false;
		this.gv_Invoice.IsInsertNewRow = false;
		this.gv_Invoice.IsLoadingData = false;
		this.gv_Invoice.IsMappingContextAction = false;
		this.gv_Invoice.ItemId = "OINVOICE";
		this.gv_Invoice.LstColumnAllowBatch = null;
		this.gv_Invoice.LstColumnNotAllowBatch = null;
		this.gv_Invoice.LstColumnNotEditable = null;
		this.gv_Invoice.LstColumNotSetting = null;
		this.gv_Invoice.Name = "gv_Invoice";
		this.gv_Invoice.OptionsView.ShowFooter = true;
		this.gv_Invoice.OptionsView.ShowGroupPanel = false;
		this.gv_Invoice.UseBatchMod = false;
		this.gv_Invoice.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Invoice_CellValueChanging);
		this.gv_Invoice.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Invoice_ValidatingEditor);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "Create Incoming Payments";
		this.btn_OK.FieldName = null;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(3, 339);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(164, 23);
		this.btn_OK.TabIndex = 4;
		this.btn_OK.Text = "Create Incoming Payments";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(173, 339);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 5;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.ToolTip = "Thoát";
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.tab_Payment;
		this.xtraTabControl1.Size = new System.Drawing.Size(806, 396);
		this.xtraTabControl1.TabIndex = 6;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.tab_Payment, this.tab_ListOfPayment, this.tab_ListOfInvoice });
		this.tab_Payment.Controls.Add(this.labelLine1);
		this.tab_Payment.Controls.Add(this.btn_Cancel);
		this.tab_Payment.Controls.Add(this.txt_InvoiceNo);
		this.tab_Payment.Controls.Add(this.btn_OK);
		this.tab_Payment.Controls.Add(this.grd_Invoice);
		this.tab_Payment.EditControl = null;
		this.tab_Payment.EditValue = "Payment";
		this.tab_Payment.FieldName = null;
		this.tab_Payment.IsSearchControl = false;
		this.tab_Payment.Name = "tab_Payment";
		this.tab_Payment.ReadOnly = false;
		this.tab_Payment.SettingInVisible = false;
		this.tab_Payment.SettingReadonly = false;
		this.tab_Payment.Size = new System.Drawing.Size(804, 371);
		this.tab_Payment.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Payment.Text = "Payment";
		this.tab_ListOfPayment.Controls.Add(this.btn_Search_Payment);
		this.tab_ListOfPayment.Controls.Add(this.grd_Payment);
		this.tab_ListOfPayment.Controls.Add(this.txt_PaymentEntry);
		this.tab_ListOfPayment.Controls.Add(this.labelLine5);
		this.tab_ListOfPayment.Controls.Add(this.pdate_To);
		this.tab_ListOfPayment.Controls.Add(this.labelLine6);
		this.tab_ListOfPayment.Controls.Add(this.pdate_From);
		this.tab_ListOfPayment.Controls.Add(this.labelLine7);
		this.tab_ListOfPayment.EditControl = null;
		this.tab_ListOfPayment.EditValue = "List of Payment";
		this.tab_ListOfPayment.FieldName = null;
		this.tab_ListOfPayment.IsSearchControl = false;
		this.tab_ListOfPayment.Name = "tab_ListOfPayment";
		this.tab_ListOfPayment.ReadOnly = false;
		this.tab_ListOfPayment.SettingInVisible = false;
		this.tab_ListOfPayment.SettingReadonly = false;
		this.tab_ListOfPayment.Size = new System.Drawing.Size(804, 371);
		this.tab_ListOfPayment.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_ListOfPayment.Text = "List of Payment";
		this.btn_Search_Payment.EditControl = null;
		this.btn_Search_Payment.EditValue = "Search";
		this.btn_Search_Payment.FieldName = null;
		this.btn_Search_Payment.IsSearchControl = false;
		this.btn_Search_Payment.Location = new System.Drawing.Point(566, 2);
		this.btn_Search_Payment.Name = "btn_Search_Payment";
		this.btn_Search_Payment.ReadOnly = false;
		this.btn_Search_Payment.SettingInVisible = false;
		this.btn_Search_Payment.SettingReadonly = false;
		this.btn_Search_Payment.Size = new System.Drawing.Size(80, 20);
		this.btn_Search_Payment.TabIndex = 15;
		this.btn_Search_Payment.Text = "Search";
		this.btn_Search_Payment.Click += new System.EventHandler(this.btn_Search_Payment_Click);
		this.grd_Payment.AllowPasteExcel = false;
		this.grd_Payment.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Payment.ClipboardData = null;
		this.grd_Payment.Location = new System.Drawing.Point(0, 32);
		this.grd_Payment.MainView = this.gv_Payment;
		this.grd_Payment.Name = "grd_Payment";
		this.grd_Payment.Size = new System.Drawing.Size(800, 334);
		this.grd_Payment.TabIndex = 14;
		this.grd_Payment.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Payment });
		this.gv_Payment.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Payment.ColumnKey = null;
		this.gv_Payment.DisableFormSetting = false;
		this.gv_Payment.DisplayException = false;
		this.gv_Payment.EditColumn = null;
		this.gv_Payment.FieldCurrency = null;
		this.gv_Payment.FieldRate = null;
		this.gv_Payment.FormId = "oPaymentForInvoice";
		this.gv_Payment.GridControl = this.grd_Payment;
		this.gv_Payment.IgnoreFilter = false;
		this.gv_Payment.IndexEditRow = -1;
		this.gv_Payment.IndicatorWidth = 35;
		this.gv_Payment.IsApzTable = false;
		this.gv_Payment.IsInsertNewRow = false;
		this.gv_Payment.IsLoadingData = false;
		this.gv_Payment.IsMappingContextAction = false;
		this.gv_Payment.ItemId = "OPAYMENT";
		this.gv_Payment.LstColumnAllowBatch = null;
		this.gv_Payment.LstColumnNotAllowBatch = null;
		this.gv_Payment.LstColumnNotEditable = null;
		this.gv_Payment.LstColumNotSetting = null;
		this.gv_Payment.Name = "gv_Payment";
		this.gv_Payment.UseBatchMod = false;
		this.txt_PaymentEntry.EditControl = null;
		this.txt_PaymentEntry.FieldName = null;
		this.txt_PaymentEntry.IsSearchControl = false;
		this.txt_PaymentEntry.Location = new System.Drawing.Point(460, 2);
		this.txt_PaymentEntry.Name = "txt_PaymentEntry";
		this.txt_PaymentEntry.SettingInVisible = false;
		this.txt_PaymentEntry.SettingReadonly = false;
		this.txt_PaymentEntry.Size = new System.Drawing.Size(100, 20);
		this.txt_PaymentEntry.TabIndex = 13;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Payment No";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(385, 5);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 12;
		this.labelLine5.Utext = "Payment No";
		this.pdate_To.EditControl = null;
		this.pdate_To.EditValue = null;
		this.pdate_To.FieldName = null;
		this.pdate_To.Location = new System.Drawing.Point(279, 2);
		this.pdate_To.Name = "pdate_To";
		this.pdate_To.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.pdate_To.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.pdate_To.Properties.IsSearchControl = false;
		this.pdate_To.SettingInVisible = false;
		this.pdate_To.SettingReadonly = false;
		this.pdate_To.Size = new System.Drawing.Size(100, 20);
		this.pdate_To.TabIndex = 11;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "To Date";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(200, 5);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(80, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 10;
		this.labelLine6.Utext = "To Date";
		this.pdate_From.EditControl = null;
		this.pdate_From.EditValue = null;
		this.pdate_From.FieldName = null;
		this.pdate_From.Location = new System.Drawing.Point(83, 2);
		this.pdate_From.Name = "pdate_From";
		this.pdate_From.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.pdate_From.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.pdate_From.Properties.IsSearchControl = false;
		this.pdate_From.SettingInVisible = false;
		this.pdate_From.SettingReadonly = false;
		this.pdate_From.Size = new System.Drawing.Size(100, 20);
		this.pdate_From.TabIndex = 9;
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "From Date";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(4, 5);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(80, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 8;
		this.labelLine7.Utext = "From Date";
		this.tab_ListOfInvoice.Controls.Add(this.btn_Search);
		this.tab_ListOfInvoice.Controls.Add(this.grd_Report);
		this.tab_ListOfInvoice.Controls.Add(this.txt_InvNo);
		this.tab_ListOfInvoice.Controls.Add(this.labelLine4);
		this.tab_ListOfInvoice.Controls.Add(this.date_to);
		this.tab_ListOfInvoice.Controls.Add(this.labelLine3);
		this.tab_ListOfInvoice.Controls.Add(this.date_from);
		this.tab_ListOfInvoice.Controls.Add(this.labelLine2);
		this.tab_ListOfInvoice.EditControl = null;
		this.tab_ListOfInvoice.EditValue = "List of Invoice";
		this.tab_ListOfInvoice.FieldName = null;
		this.tab_ListOfInvoice.IsSearchControl = false;
		this.tab_ListOfInvoice.Name = "tab_ListOfInvoice";
		this.tab_ListOfInvoice.ReadOnly = false;
		this.tab_ListOfInvoice.SettingInVisible = false;
		this.tab_ListOfInvoice.SettingReadonly = false;
		this.tab_ListOfInvoice.Size = new System.Drawing.Size(804, 371);
		this.tab_ListOfInvoice.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_ListOfInvoice.Text = "List of Invoice";
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(566, 4);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(80, 20);
		this.btn_Search.TabIndex = 7;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click_1);
		this.grd_Report.AllowPasteExcel = false;
		this.grd_Report.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Report.ClipboardData = null;
		this.grd_Report.Location = new System.Drawing.Point(0, 34);
		this.grd_Report.MainView = this.gv_Report;
		this.grd_Report.Name = "grd_Report";
		this.grd_Report.Size = new System.Drawing.Size(800, 334);
		this.grd_Report.TabIndex = 6;
		this.grd_Report.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Report });
		this.gv_Report.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Report.ColumnKey = null;
		this.gv_Report.DisableFormSetting = false;
		this.gv_Report.DisplayException = false;
		this.gv_Report.EditColumn = null;
		this.gv_Report.FieldCurrency = null;
		this.gv_Report.FieldRate = null;
		this.gv_Report.FormId = "oPaymentForInvoice";
		this.gv_Report.GridControl = this.grd_Report;
		this.gv_Report.IgnoreFilter = false;
		this.gv_Report.IndexEditRow = -1;
		this.gv_Report.IndicatorWidth = 35;
		this.gv_Report.IsApzTable = false;
		this.gv_Report.IsInsertNewRow = false;
		this.gv_Report.IsLoadingData = false;
		this.gv_Report.IsMappingContextAction = false;
		this.gv_Report.ItemId = "OREPORT";
		this.gv_Report.LstColumnAllowBatch = null;
		this.gv_Report.LstColumnNotAllowBatch = null;
		this.gv_Report.LstColumnNotEditable = null;
		this.gv_Report.LstColumNotSetting = null;
		this.gv_Report.Name = "gv_Report";
		this.gv_Report.UseBatchMod = false;
		this.txt_InvNo.EditControl = null;
		this.txt_InvNo.FieldName = null;
		this.txt_InvNo.IsSearchControl = false;
		this.txt_InvNo.Location = new System.Drawing.Point(460, 4);
		this.txt_InvNo.Name = "txt_InvNo";
		this.txt_InvNo.SettingInVisible = false;
		this.txt_InvNo.SettingReadonly = false;
		this.txt_InvNo.Size = new System.Drawing.Size(100, 20);
		this.txt_InvNo.TabIndex = 5;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Invoice No";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(385, 7);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 4;
		this.labelLine4.Utext = "Invoice No";
		this.date_to.EditControl = null;
		this.date_to.EditValue = null;
		this.date_to.FieldName = null;
		this.date_to.Location = new System.Drawing.Point(279, 4);
		this.date_to.Name = "date_to";
		this.date_to.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_to.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_to.Properties.IsSearchControl = false;
		this.date_to.SettingInVisible = false;
		this.date_to.SettingReadonly = false;
		this.date_to.Size = new System.Drawing.Size(100, 20);
		this.date_to.TabIndex = 3;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "To Date";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(200, 7);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 2;
		this.labelLine3.Utext = "To Date";
		this.date_from.EditControl = null;
		this.date_from.EditValue = null;
		this.date_from.FieldName = null;
		this.date_from.Location = new System.Drawing.Point(83, 4);
		this.date_from.Name = "date_from";
		this.date_from.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_from.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_from.Properties.IsSearchControl = false;
		this.date_from.SettingInVisible = false;
		this.date_from.SettingReadonly = false;
		this.date_from.Size = new System.Drawing.Size(100, 20);
		this.date_from.TabIndex = 1;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "From Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(4, 7);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 0;
		this.labelLine2.Utext = "From Date";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(806, 396);
		base.Controls.Add(this.xtraTabControl1);
		base.FormId = "oPaymentForInvoice";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_PaymentForInvoice.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PaymentForInvoice";
		base.ShowInTaskbar = false;
		this.Text = "Payment for the Invoice";
		base.Load += new System.EventHandler(this.frm_PaymentForInvoice_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_InvoiceNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Invoice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Invoice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.tab_Payment.ResumeLayout(false);
		this.tab_ListOfPayment.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Payment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Payment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PaymentEntry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pdate_To.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pdate_To.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pdate_From.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pdate_From.Properties).EndInit();
		this.tab_ListOfInvoice.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Report).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Report).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_InvNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_to.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_to.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_from.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_from.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
