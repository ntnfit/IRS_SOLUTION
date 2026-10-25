using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls.Controls;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Payments;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using Newtonsoft.Json;

namespace Apzon.Forms.Payment;

public class frm_PaymentIncomingList : Form
{
	public delegate void CloseEvent(string str);

	public CloseEvent reloadParent;

	private IContainer components;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx DocEntry;

	private GridColumnEx DocNum;

	private GridColumnEx DocType;

	private RepositoryItemComboBox repositoryItemComboBox1;

	private GridColumnEx Canceled;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private GridColumnEx Printed;

	private SimpleButtonEx btCancel;

	private GridColumnEx DocDate;

	private GridColumnEx DocDueDate;

	private GridColumnEx CardCode;

	private GridColumnEx CardName;

	private GridColumnEx Address;

	private GridColumnEx CashAcct;

	private GridColumnEx CashSum;

	private GridColumnEx CreditSum;

	private GridColumnEx CouponSum;

	private GridColumnEx TrsfrAcct;

	private GridColumnEx TrsfrSum;

	private GridColumnEx TrsfrDate;

	private GridColumnEx TrsfrRef;

	private GridColumnEx DocCurr;

	private GridColumnEx DocRate;

	private GridColumnEx DocTotal;

	private GridColumnEx JrnlMemo;

	private GridColumnEx TransId;

	private GridColumnEx DocTime;

	private GridColumnEx ObjType;

	private GridColumnEx UpdateDate;

	private GridColumnEx CreateDate;

	private GridColumnEx UserSign;

	private GridColumnEx VatGroup;

	private GridColumnEx VatSum;

	private GridColumnEx Dcount;

	private GridColumnEx DcntSum;

	private GridColumnEx CashSumFC;

	private GridColumnEx CreditSumFC;

	private GridColumnEx TrsfrSumFC;

	private GridColumnEx CashSumSy;

	private GridColumnEx CreditSumSy;

	private GridColumnEx TrsfrSumSy;

	private GridColumnEx VatSumFC;

	private GridColumnEx VatSumSy;

	private GridColumnEx DcntSumFC;

	private GridColumnEx DocTotalFC;

	private GridColumnEx DocTotalSy;

	public frm_PaymentIncomingList()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_PaymentIncomingList"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_PaymentIncomingList" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void frm_PaymentIncomingList_Load(object sender, EventArgs e)
	{
		loadData();
	}

	private async void loadData()
	{
		using HttpClient client = new HttpClient();
		string apiUri = Function.GetApiUri(Configuration.ApiServer, "IncomingPayment", "ShowIncomingPayment");
		List<IncomingPaymentsImpl> lst = new List<IncomingPaymentsImpl>();
		using HttpResponseMessage response = await client.GetAsync(apiUri);
		if (!response.IsSuccessStatusCode)
		{
			return;
		}
		lst = JsonConvert.DeserializeObject<List<IncomingPaymentsImpl>>(await response.Content.ReadAsStringAsync()).ToList();
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

	private void gvDoc_DoubleClick(object sender, EventArgs e)
	{
		string rowCellDisplayText = gvDoc.GetRowCellDisplayText(gvDoc.FocusedRowHandle, "DocEntry");
		reloadParent(rowCellDisplayText);
		Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Payment.frm_PaymentIncomingList));
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.DocEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.DocNum = new Apzon.Controls.Grid.GridColumnEx();
		this.DocType = new Apzon.Controls.Grid.GridColumnEx();
		this.Canceled = new Apzon.Controls.Grid.GridColumnEx();
		this.Printed = new Apzon.Controls.Grid.GridColumnEx();
		this.DocDate = new Apzon.Controls.Grid.GridColumnEx();
		this.DocDueDate = new Apzon.Controls.Grid.GridColumnEx();
		this.CardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.CardName = new Apzon.Controls.Grid.GridColumnEx();
		this.Address = new Apzon.Controls.Grid.GridColumnEx();
		this.CashAcct = new Apzon.Controls.Grid.GridColumnEx();
		this.CashSum = new Apzon.Controls.Grid.GridColumnEx();
		this.CreditSum = new Apzon.Controls.Grid.GridColumnEx();
		this.CouponSum = new Apzon.Controls.Grid.GridColumnEx();
		this.TrsfrAcct = new Apzon.Controls.Grid.GridColumnEx();
		this.TrsfrSum = new Apzon.Controls.Grid.GridColumnEx();
		this.TrsfrDate = new Apzon.Controls.Grid.GridColumnEx();
		this.TrsfrRef = new Apzon.Controls.Grid.GridColumnEx();
		this.DocCurr = new Apzon.Controls.Grid.GridColumnEx();
		this.DocRate = new Apzon.Controls.Grid.GridColumnEx();
		this.DocTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.JrnlMemo = new Apzon.Controls.Grid.GridColumnEx();
		this.TransId = new Apzon.Controls.Grid.GridColumnEx();
		this.DocTime = new Apzon.Controls.Grid.GridColumnEx();
		this.ObjType = new Apzon.Controls.Grid.GridColumnEx();
		this.UpdateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.CreateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.UserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.VatGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.VatSum = new Apzon.Controls.Grid.GridColumnEx();
		this.Dcount = new Apzon.Controls.Grid.GridColumnEx();
		this.DcntSum = new Apzon.Controls.Grid.GridColumnEx();
		this.CashSumFC = new Apzon.Controls.Grid.GridColumnEx();
		this.CreditSumFC = new Apzon.Controls.Grid.GridColumnEx();
		this.TrsfrSumFC = new Apzon.Controls.Grid.GridColumnEx();
		this.CashSumSy = new Apzon.Controls.Grid.GridColumnEx();
		this.CreditSumSy = new Apzon.Controls.Grid.GridColumnEx();
		this.TrsfrSumSy = new Apzon.Controls.Grid.GridColumnEx();
		this.VatSumFC = new Apzon.Controls.Grid.GridColumnEx();
		this.VatSumSy = new Apzon.Controls.Grid.GridColumnEx();
		this.DcntSumFC = new Apzon.Controls.Grid.GridColumnEx();
		this.DocTotalFC = new Apzon.Controls.Grid.GridColumnEx();
		this.DocTotalSy = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox1).BeginInit();
		base.SuspendLayout();
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Location = new System.Drawing.Point(0, -1);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemCheckEdit1, this.repositoryItemComboBox1 });
		this.grdDoc.Size = new System.Drawing.Size(742, 352);
		this.grdDoc.TabIndex = 4;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[43]
		{
			this.DocEntry, this.DocNum, this.DocType, this.Canceled, this.Printed, this.DocDate, this.DocDueDate, this.CardCode, this.CardName, this.Address,
			this.CashAcct, this.CashSum, this.CreditSum, this.CouponSum, this.TrsfrAcct, this.TrsfrSum, this.TrsfrDate, this.TrsfrRef, this.DocCurr, this.DocRate,
			this.DocTotal, this.JrnlMemo, this.TransId, this.DocTime, this.ObjType, this.UpdateDate, this.CreateDate, this.UserSign, this.VatGroup, this.VatSum,
			this.Dcount, this.DcntSum, this.CashSumFC, this.CreditSumFC, this.TrsfrSumFC, this.CashSumSy, this.CreditSumSy, this.TrsfrSumSy, this.VatSumFC, this.VatSumSy,
			this.DcntSumFC, this.DocTotalFC, this.DocTotalSy
		});
		this.gvDoc.CustomizationFormBounds = new System.Drawing.Rectangle(433, 310, 216, 180);
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = null;
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 30;
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
		this.gvDoc.OptionsFind.AlwaysVisible = true;
		this.gvDoc.OptionsView.ShowDetailButtons = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.DoubleClick += new System.EventHandler(this.gvDoc_DoubleClick);
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
		this.DocEntry.PrimaryKey = true;
		this.DocEntry.UnitField = null;
		this.DocEntry.Visible = true;
		this.DocEntry.VisibleIndex = 0;
		this.DocEntry.Width = 100;
		this.DocNum.Caption = "Doc. Number";
		this.DocNum.CurrencyDateField = null;
		this.DocNum.CurrencyField = null;
		this.DocNum.CurrencyFixed = null;
		this.DocNum.CurrencyFixedRate = 0.0;
		this.DocNum.CurrencyRateField = null;
		this.DocNum.FieldName = "DocNum";
		this.DocNum.MinWidth = 100;
		this.DocNum.Name = "DocNum";
		this.DocNum.OptionsColumn.AllowEdit = false;
		this.DocNum.OptionsColumn.ReadOnly = true;
		this.DocNum.PrimaryKey = false;
		this.DocNum.UnitField = null;
		this.DocNum.Visible = true;
		this.DocNum.VisibleIndex = 1;
		this.DocNum.Width = 100;
		this.DocType.Caption = "Doc. Type";
		this.DocType.CurrencyDateField = null;
		this.DocType.CurrencyField = null;
		this.DocType.CurrencyFixed = null;
		this.DocType.CurrencyFixedRate = 0.0;
		this.DocType.CurrencyRateField = null;
		this.DocType.FieldName = "DocType";
		this.DocType.MinWidth = 100;
		this.DocType.Name = "DocType";
		this.DocType.OptionsColumn.AllowEdit = false;
		this.DocType.OptionsColumn.ReadOnly = true;
		this.DocType.PrimaryKey = false;
		this.DocType.UnitField = null;
		this.DocType.Visible = true;
		this.DocType.VisibleIndex = 2;
		this.DocType.Width = 100;
		this.Canceled.Caption = "Cancelled";
		this.Canceled.CurrencyDateField = null;
		this.Canceled.CurrencyField = null;
		this.Canceled.CurrencyFixed = null;
		this.Canceled.CurrencyFixedRate = 0.0;
		this.Canceled.CurrencyRateField = null;
		this.Canceled.FieldName = "Canceled";
		this.Canceled.MinWidth = 100;
		this.Canceled.Name = "Canceled";
		this.Canceled.OptionsColumn.AllowEdit = false;
		this.Canceled.OptionsColumn.ReadOnly = true;
		this.Canceled.PrimaryKey = false;
		this.Canceled.UnitField = null;
		this.Canceled.Visible = true;
		this.Canceled.VisibleIndex = 3;
		this.Canceled.Width = 1000;
		this.Printed.Caption = "Printed";
		this.Printed.CurrencyDateField = null;
		this.Printed.CurrencyField = null;
		this.Printed.CurrencyFixed = null;
		this.Printed.CurrencyFixedRate = 0.0;
		this.Printed.CurrencyRateField = null;
		this.Printed.FieldName = "Printed";
		this.Printed.MinWidth = 100;
		this.Printed.Name = "Printed";
		this.Printed.OptionsColumn.AllowEdit = false;
		this.Printed.OptionsColumn.ReadOnly = true;
		this.Printed.PrimaryKey = false;
		this.Printed.UnitField = null;
		this.Printed.Visible = true;
		this.Printed.VisibleIndex = 4;
		this.Printed.Width = 100;
		this.DocDate.Caption = "Posting Date";
		this.DocDate.CurrencyDateField = null;
		this.DocDate.CurrencyField = null;
		this.DocDate.CurrencyFixed = null;
		this.DocDate.CurrencyFixedRate = 0.0;
		this.DocDate.CurrencyRateField = null;
		this.DocDate.FieldName = "DocDate";
		this.DocDate.MinWidth = 100;
		this.DocDate.Name = "DocDate";
		this.DocDate.OptionsColumn.AllowEdit = false;
		this.DocDate.OptionsColumn.ReadOnly = true;
		this.DocDate.PrimaryKey = false;
		this.DocDate.UnitField = null;
		this.DocDate.Visible = true;
		this.DocDate.VisibleIndex = 5;
		this.DocDate.Width = 100;
		this.DocDueDate.Caption = "Due Date";
		this.DocDueDate.CurrencyDateField = null;
		this.DocDueDate.CurrencyField = null;
		this.DocDueDate.CurrencyFixed = null;
		this.DocDueDate.CurrencyFixedRate = 0.0;
		this.DocDueDate.CurrencyRateField = null;
		this.DocDueDate.FieldName = "DocDueDate";
		this.DocDueDate.MinWidth = 100;
		this.DocDueDate.Name = "DocDueDate";
		this.DocDueDate.OptionsColumn.AllowEdit = false;
		this.DocDueDate.OptionsColumn.ReadOnly = true;
		this.DocDueDate.PrimaryKey = false;
		this.DocDueDate.UnitField = null;
		this.DocDueDate.Visible = true;
		this.DocDueDate.VisibleIndex = 6;
		this.DocDueDate.Width = 100;
		this.CardCode.Caption = "Customer Code";
		this.CardCode.CurrencyDateField = null;
		this.CardCode.CurrencyField = null;
		this.CardCode.CurrencyFixed = null;
		this.CardCode.CurrencyFixedRate = 0.0;
		this.CardCode.CurrencyRateField = null;
		this.CardCode.FieldName = "CardCode";
		this.CardCode.MinWidth = 100;
		this.CardCode.Name = "CardCode";
		this.CardCode.OptionsColumn.AllowEdit = false;
		this.CardCode.OptionsColumn.ReadOnly = true;
		this.CardCode.PrimaryKey = false;
		this.CardCode.UnitField = null;
		this.CardCode.Visible = true;
		this.CardCode.VisibleIndex = 7;
		this.CardCode.Width = 100;
		this.CardName.Caption = "Customer Name";
		this.CardName.CurrencyDateField = null;
		this.CardName.CurrencyField = null;
		this.CardName.CurrencyFixed = null;
		this.CardName.CurrencyFixedRate = 0.0;
		this.CardName.CurrencyRateField = null;
		this.CardName.FieldName = "CardName";
		this.CardName.MinWidth = 100;
		this.CardName.Name = "CardName";
		this.CardName.OptionsColumn.AllowEdit = false;
		this.CardName.OptionsColumn.ReadOnly = true;
		this.CardName.PrimaryKey = false;
		this.CardName.UnitField = null;
		this.CardName.Visible = true;
		this.CardName.VisibleIndex = 8;
		this.CardName.Width = 100;
		this.Address.Caption = "Address";
		this.Address.CurrencyDateField = null;
		this.Address.CurrencyField = null;
		this.Address.CurrencyFixed = null;
		this.Address.CurrencyFixedRate = 0.0;
		this.Address.CurrencyRateField = null;
		this.Address.FieldName = "Address";
		this.Address.MinWidth = 100;
		this.Address.Name = "Address";
		this.Address.OptionsColumn.AllowEdit = false;
		this.Address.OptionsColumn.ReadOnly = true;
		this.Address.PrimaryKey = false;
		this.Address.UnitField = null;
		this.Address.Visible = true;
		this.Address.VisibleIndex = 9;
		this.Address.Width = 100;
		this.CashAcct.Caption = "Cash Account";
		this.CashAcct.CurrencyDateField = null;
		this.CashAcct.CurrencyField = null;
		this.CashAcct.CurrencyFixed = null;
		this.CashAcct.CurrencyFixedRate = 0.0;
		this.CashAcct.CurrencyRateField = null;
		this.CashAcct.FieldName = "CashAcct";
		this.CashAcct.MinWidth = 100;
		this.CashAcct.Name = "CashAcct";
		this.CashAcct.OptionsColumn.AllowEdit = false;
		this.CashAcct.OptionsColumn.ReadOnly = true;
		this.CashAcct.PrimaryKey = false;
		this.CashAcct.UnitField = null;
		this.CashAcct.Visible = true;
		this.CashAcct.VisibleIndex = 10;
		this.CashAcct.Width = 100;
		this.CashSum.Caption = "Cash Amount";
		this.CashSum.CurrencyDateField = null;
		this.CashSum.CurrencyField = null;
		this.CashSum.CurrencyFixed = null;
		this.CashSum.CurrencyFixedRate = 0.0;
		this.CashSum.CurrencyRateField = null;
		this.CashSum.FieldName = "CashSum";
		this.CashSum.MinWidth = 100;
		this.CashSum.Name = "CashSum";
		this.CashSum.OptionsColumn.AllowEdit = false;
		this.CashSum.OptionsColumn.ReadOnly = true;
		this.CashSum.PrimaryKey = false;
		this.CashSum.UnitField = null;
		this.CashSum.Visible = true;
		this.CashSum.VisibleIndex = 11;
		this.CashSum.Width = 100;
		this.CreditSum.Caption = "Credit Amount";
		this.CreditSum.CurrencyDateField = null;
		this.CreditSum.CurrencyField = null;
		this.CreditSum.CurrencyFixed = null;
		this.CreditSum.CurrencyFixedRate = 0.0;
		this.CreditSum.CurrencyRateField = null;
		this.CreditSum.FieldName = "CreditSum";
		this.CreditSum.MinWidth = 100;
		this.CreditSum.Name = "CreditSum";
		this.CreditSum.OptionsColumn.AllowEdit = false;
		this.CreditSum.OptionsColumn.ReadOnly = true;
		this.CreditSum.PrimaryKey = false;
		this.CreditSum.UnitField = null;
		this.CreditSum.Visible = true;
		this.CreditSum.VisibleIndex = 12;
		this.CreditSum.Width = 100;
		this.CouponSum.Caption = "Coupon Amount";
		this.CouponSum.CurrencyDateField = null;
		this.CouponSum.CurrencyField = null;
		this.CouponSum.CurrencyFixed = null;
		this.CouponSum.CurrencyFixedRate = 0.0;
		this.CouponSum.CurrencyRateField = null;
		this.CouponSum.FieldName = "CouponSum";
		this.CouponSum.MinWidth = 100;
		this.CouponSum.Name = "CouponSum";
		this.CouponSum.OptionsColumn.AllowEdit = false;
		this.CouponSum.OptionsColumn.ReadOnly = true;
		this.CouponSum.PrimaryKey = false;
		this.CouponSum.UnitField = null;
		this.CouponSum.Visible = true;
		this.CouponSum.VisibleIndex = 13;
		this.CouponSum.Width = 100;
		this.TrsfrAcct.Caption = "Transfer Account";
		this.TrsfrAcct.CurrencyDateField = null;
		this.TrsfrAcct.CurrencyField = null;
		this.TrsfrAcct.CurrencyFixed = null;
		this.TrsfrAcct.CurrencyFixedRate = 0.0;
		this.TrsfrAcct.CurrencyRateField = null;
		this.TrsfrAcct.FieldName = "TrsfrAcct";
		this.TrsfrAcct.MinWidth = 100;
		this.TrsfrAcct.Name = "TrsfrAcct";
		this.TrsfrAcct.OptionsColumn.AllowEdit = false;
		this.TrsfrAcct.OptionsColumn.ReadOnly = true;
		this.TrsfrAcct.PrimaryKey = false;
		this.TrsfrAcct.UnitField = null;
		this.TrsfrAcct.Visible = true;
		this.TrsfrAcct.VisibleIndex = 14;
		this.TrsfrAcct.Width = 100;
		this.TrsfrSum.Caption = "Transfer Amount";
		this.TrsfrSum.CurrencyDateField = null;
		this.TrsfrSum.CurrencyField = null;
		this.TrsfrSum.CurrencyFixed = null;
		this.TrsfrSum.CurrencyFixedRate = 0.0;
		this.TrsfrSum.CurrencyRateField = null;
		this.TrsfrSum.FieldName = "TrsfrSum";
		this.TrsfrSum.MinWidth = 100;
		this.TrsfrSum.Name = "TrsfrSum";
		this.TrsfrSum.OptionsColumn.AllowEdit = false;
		this.TrsfrSum.OptionsColumn.ReadOnly = true;
		this.TrsfrSum.PrimaryKey = false;
		this.TrsfrSum.UnitField = null;
		this.TrsfrSum.Visible = true;
		this.TrsfrSum.VisibleIndex = 15;
		this.TrsfrSum.Width = 100;
		this.TrsfrDate.Caption = "Transfer Date";
		this.TrsfrDate.CurrencyDateField = null;
		this.TrsfrDate.CurrencyField = null;
		this.TrsfrDate.CurrencyFixed = null;
		this.TrsfrDate.CurrencyFixedRate = 0.0;
		this.TrsfrDate.CurrencyRateField = null;
		this.TrsfrDate.FieldName = "TrsfrDate";
		this.TrsfrDate.MinWidth = 100;
		this.TrsfrDate.Name = "TrsfrDate";
		this.TrsfrDate.OptionsColumn.AllowEdit = false;
		this.TrsfrDate.OptionsColumn.ReadOnly = true;
		this.TrsfrDate.PrimaryKey = false;
		this.TrsfrDate.UnitField = null;
		this.TrsfrDate.Visible = true;
		this.TrsfrDate.VisibleIndex = 16;
		this.TrsfrDate.Width = 100;
		this.TrsfrRef.Caption = "Intended Purpose";
		this.TrsfrRef.CurrencyDateField = null;
		this.TrsfrRef.CurrencyField = null;
		this.TrsfrRef.CurrencyFixed = null;
		this.TrsfrRef.CurrencyFixedRate = 0.0;
		this.TrsfrRef.CurrencyRateField = null;
		this.TrsfrRef.FieldName = "TrsfrRef";
		this.TrsfrRef.MinWidth = 100;
		this.TrsfrRef.Name = "TrsfrRef";
		this.TrsfrRef.OptionsColumn.AllowEdit = false;
		this.TrsfrRef.OptionsColumn.ReadOnly = true;
		this.TrsfrRef.PrimaryKey = false;
		this.TrsfrRef.UnitField = null;
		this.TrsfrRef.Visible = true;
		this.TrsfrRef.VisibleIndex = 17;
		this.TrsfrRef.Width = 100;
		this.DocCurr.Caption = "Currency";
		this.DocCurr.CurrencyDateField = null;
		this.DocCurr.CurrencyField = null;
		this.DocCurr.CurrencyFixed = null;
		this.DocCurr.CurrencyFixedRate = 0.0;
		this.DocCurr.CurrencyRateField = null;
		this.DocCurr.FieldName = "DocCurr";
		this.DocCurr.MinWidth = 100;
		this.DocCurr.Name = "DocCurr";
		this.DocCurr.OptionsColumn.AllowEdit = false;
		this.DocCurr.OptionsColumn.ReadOnly = true;
		this.DocCurr.PrimaryKey = false;
		this.DocCurr.UnitField = null;
		this.DocCurr.Visible = true;
		this.DocCurr.VisibleIndex = 18;
		this.DocCurr.Width = 100;
		this.DocRate.Caption = "Rate";
		this.DocRate.CurrencyDateField = null;
		this.DocRate.CurrencyField = null;
		this.DocRate.CurrencyFixed = null;
		this.DocRate.CurrencyFixedRate = 0.0;
		this.DocRate.CurrencyRateField = null;
		this.DocRate.FieldName = "DocRate";
		this.DocRate.MinWidth = 100;
		this.DocRate.Name = "DocRate";
		this.DocRate.OptionsColumn.AllowEdit = false;
		this.DocRate.OptionsColumn.ReadOnly = true;
		this.DocRate.PrimaryKey = false;
		this.DocRate.UnitField = null;
		this.DocRate.Visible = true;
		this.DocRate.VisibleIndex = 19;
		this.DocRate.Width = 100;
		this.DocTotal.Caption = "Total";
		this.DocTotal.CurrencyDateField = null;
		this.DocTotal.CurrencyField = null;
		this.DocTotal.CurrencyFixed = null;
		this.DocTotal.CurrencyFixedRate = 0.0;
		this.DocTotal.CurrencyRateField = null;
		this.DocTotal.FieldName = "DocTotal";
		this.DocTotal.MinWidth = 100;
		this.DocTotal.Name = "DocTotal";
		this.DocTotal.OptionsColumn.AllowEdit = false;
		this.DocTotal.OptionsColumn.ReadOnly = true;
		this.DocTotal.PrimaryKey = false;
		this.DocTotal.UnitField = null;
		this.DocTotal.Visible = true;
		this.DocTotal.VisibleIndex = 20;
		this.DocTotal.Width = 100;
		this.JrnlMemo.Caption = "Journal Remarks";
		this.JrnlMemo.CurrencyDateField = null;
		this.JrnlMemo.CurrencyField = null;
		this.JrnlMemo.CurrencyFixed = null;
		this.JrnlMemo.CurrencyFixedRate = 0.0;
		this.JrnlMemo.CurrencyRateField = null;
		this.JrnlMemo.FieldName = "JrnlMemo";
		this.JrnlMemo.MinWidth = 100;
		this.JrnlMemo.Name = "JrnlMemo";
		this.JrnlMemo.OptionsColumn.AllowEdit = false;
		this.JrnlMemo.OptionsColumn.ReadOnly = true;
		this.JrnlMemo.PrimaryKey = false;
		this.JrnlMemo.UnitField = null;
		this.JrnlMemo.Visible = true;
		this.JrnlMemo.VisibleIndex = 21;
		this.JrnlMemo.Width = 100;
		this.TransId.Caption = "Transaction Number";
		this.TransId.CurrencyDateField = null;
		this.TransId.CurrencyField = null;
		this.TransId.CurrencyFixed = null;
		this.TransId.CurrencyFixedRate = 0.0;
		this.TransId.CurrencyRateField = null;
		this.TransId.FieldName = "TransId";
		this.TransId.MinWidth = 100;
		this.TransId.Name = "TransId";
		this.TransId.OptionsColumn.AllowEdit = false;
		this.TransId.OptionsColumn.ReadOnly = true;
		this.TransId.PrimaryKey = false;
		this.TransId.UnitField = null;
		this.TransId.Visible = true;
		this.TransId.VisibleIndex = 22;
		this.TransId.Width = 106;
		this.DocTime.Caption = "Generation Time";
		this.DocTime.CurrencyDateField = null;
		this.DocTime.CurrencyField = null;
		this.DocTime.CurrencyFixed = null;
		this.DocTime.CurrencyFixedRate = 0.0;
		this.DocTime.CurrencyRateField = null;
		this.DocTime.FieldName = "DocTime";
		this.DocTime.MinWidth = 100;
		this.DocTime.Name = "DocTime";
		this.DocTime.OptionsColumn.AllowEdit = false;
		this.DocTime.OptionsColumn.ReadOnly = true;
		this.DocTime.PrimaryKey = false;
		this.DocTime.UnitField = null;
		this.DocTime.Visible = true;
		this.DocTime.VisibleIndex = 23;
		this.DocTime.Width = 100;
		this.ObjType.Caption = "Object Type";
		this.ObjType.CurrencyDateField = null;
		this.ObjType.CurrencyField = null;
		this.ObjType.CurrencyFixed = null;
		this.ObjType.CurrencyFixedRate = 0.0;
		this.ObjType.CurrencyRateField = null;
		this.ObjType.FieldName = "ObjType";
		this.ObjType.MinWidth = 100;
		this.ObjType.Name = "ObjType";
		this.ObjType.OptionsColumn.AllowEdit = false;
		this.ObjType.OptionsColumn.ReadOnly = true;
		this.ObjType.PrimaryKey = false;
		this.ObjType.UnitField = null;
		this.ObjType.Visible = true;
		this.ObjType.VisibleIndex = 24;
		this.ObjType.Width = 100;
		this.UpdateDate.Caption = "Date of Update";
		this.UpdateDate.CurrencyDateField = null;
		this.UpdateDate.CurrencyField = null;
		this.UpdateDate.CurrencyFixed = null;
		this.UpdateDate.CurrencyFixedRate = 0.0;
		this.UpdateDate.CurrencyRateField = null;
		this.UpdateDate.FieldName = "UpdateDate";
		this.UpdateDate.MinWidth = 100;
		this.UpdateDate.Name = "UpdateDate";
		this.UpdateDate.OptionsColumn.AllowEdit = false;
		this.UpdateDate.OptionsColumn.ReadOnly = true;
		this.UpdateDate.PrimaryKey = false;
		this.UpdateDate.UnitField = null;
		this.UpdateDate.Visible = true;
		this.UpdateDate.VisibleIndex = 25;
		this.UpdateDate.Width = 100;
		this.CreateDate.Caption = "Creation Date";
		this.CreateDate.CurrencyDateField = null;
		this.CreateDate.CurrencyField = null;
		this.CreateDate.CurrencyFixed = null;
		this.CreateDate.CurrencyFixedRate = 0.0;
		this.CreateDate.CurrencyRateField = null;
		this.CreateDate.FieldName = "CreateDate";
		this.CreateDate.MinWidth = 100;
		this.CreateDate.Name = "CreateDate";
		this.CreateDate.OptionsColumn.AllowEdit = false;
		this.CreateDate.OptionsColumn.ReadOnly = true;
		this.CreateDate.PrimaryKey = false;
		this.CreateDate.UnitField = null;
		this.CreateDate.Visible = true;
		this.CreateDate.VisibleIndex = 26;
		this.CreateDate.Width = 100;
		this.UserSign.Caption = "User Signature";
		this.UserSign.CurrencyDateField = null;
		this.UserSign.CurrencyField = null;
		this.UserSign.CurrencyFixed = null;
		this.UserSign.CurrencyFixedRate = 0.0;
		this.UserSign.CurrencyRateField = null;
		this.UserSign.FieldName = "UserSign";
		this.UserSign.MinWidth = 100;
		this.UserSign.Name = "UserSign";
		this.UserSign.OptionsColumn.AllowEdit = false;
		this.UserSign.OptionsColumn.ReadOnly = true;
		this.UserSign.PrimaryKey = false;
		this.UserSign.UnitField = null;
		this.UserSign.Visible = true;
		this.UserSign.VisibleIndex = 27;
		this.UserSign.Width = 100;
		this.VatGroup.Caption = "Tax Group";
		this.VatGroup.CurrencyDateField = null;
		this.VatGroup.CurrencyField = null;
		this.VatGroup.CurrencyFixed = null;
		this.VatGroup.CurrencyFixedRate = 0.0;
		this.VatGroup.CurrencyRateField = null;
		this.VatGroup.FieldName = "VatGroup";
		this.VatGroup.MinWidth = 100;
		this.VatGroup.Name = "VatGroup";
		this.VatGroup.OptionsColumn.AllowEdit = false;
		this.VatGroup.OptionsColumn.ReadOnly = true;
		this.VatGroup.PrimaryKey = false;
		this.VatGroup.UnitField = null;
		this.VatGroup.Visible = true;
		this.VatGroup.VisibleIndex = 28;
		this.VatGroup.Width = 100;
		this.VatSum.Caption = "Tax Amount";
		this.VatSum.CurrencyDateField = null;
		this.VatSum.CurrencyField = null;
		this.VatSum.CurrencyFixed = null;
		this.VatSum.CurrencyFixedRate = 0.0;
		this.VatSum.CurrencyRateField = null;
		this.VatSum.FieldName = "VatSum";
		this.VatSum.MinWidth = 100;
		this.VatSum.Name = "VatSum";
		this.VatSum.OptionsColumn.AllowEdit = false;
		this.VatSum.OptionsColumn.ReadOnly = true;
		this.VatSum.PrimaryKey = false;
		this.VatSum.UnitField = null;
		this.VatSum.Visible = true;
		this.VatSum.VisibleIndex = 29;
		this.VatSum.Width = 100;
		this.Dcount.Caption = "Disc. %";
		this.Dcount.CurrencyDateField = null;
		this.Dcount.CurrencyField = null;
		this.Dcount.CurrencyFixed = null;
		this.Dcount.CurrencyFixedRate = 0.0;
		this.Dcount.CurrencyRateField = null;
		this.Dcount.MinWidth = 100;
		this.Dcount.Name = "Dcount";
		this.Dcount.OptionsColumn.AllowEdit = false;
		this.Dcount.OptionsColumn.ReadOnly = true;
		this.Dcount.PrimaryKey = false;
		this.Dcount.UnitField = null;
		this.Dcount.Visible = true;
		this.Dcount.VisibleIndex = 30;
		this.Dcount.Width = 100;
		this.DcntSum.Caption = "Disc. Amount";
		this.DcntSum.CurrencyDateField = null;
		this.DcntSum.CurrencyField = null;
		this.DcntSum.CurrencyFixed = null;
		this.DcntSum.CurrencyFixedRate = 0.0;
		this.DcntSum.CurrencyRateField = null;
		this.DcntSum.FieldName = "DcntSum";
		this.DcntSum.MinWidth = 100;
		this.DcntSum.Name = "DcntSum";
		this.DcntSum.OptionsColumn.AllowEdit = false;
		this.DcntSum.OptionsColumn.ReadOnly = true;
		this.DcntSum.PrimaryKey = false;
		this.DcntSum.UnitField = null;
		this.DcntSum.Visible = true;
		this.DcntSum.VisibleIndex = 31;
		this.DcntSum.Width = 100;
		this.CashSumFC.Caption = "Cash Amount FC";
		this.CashSumFC.CurrencyDateField = null;
		this.CashSumFC.CurrencyField = null;
		this.CashSumFC.CurrencyFixed = null;
		this.CashSumFC.CurrencyFixedRate = 0.0;
		this.CashSumFC.CurrencyRateField = null;
		this.CashSumFC.FieldName = "CashSumFC";
		this.CashSumFC.MinWidth = 100;
		this.CashSumFC.Name = "CashSumFC";
		this.CashSumFC.OptionsColumn.AllowEdit = false;
		this.CashSumFC.OptionsColumn.ReadOnly = true;
		this.CashSumFC.PrimaryKey = false;
		this.CashSumFC.UnitField = null;
		this.CashSumFC.Visible = true;
		this.CashSumFC.VisibleIndex = 32;
		this.CashSumFC.Width = 100;
		this.CreditSumFC.Caption = "Credit Amount FC";
		this.CreditSumFC.CurrencyDateField = null;
		this.CreditSumFC.CurrencyField = null;
		this.CreditSumFC.CurrencyFixed = null;
		this.CreditSumFC.CurrencyFixedRate = 0.0;
		this.CreditSumFC.CurrencyRateField = null;
		this.CreditSumFC.FieldName = "CreditSumFC";
		this.CreditSumFC.MinWidth = 100;
		this.CreditSumFC.Name = "CreditSumFC";
		this.CreditSumFC.OptionsColumn.AllowEdit = false;
		this.CreditSumFC.OptionsColumn.ReadOnly = true;
		this.CreditSumFC.PrimaryKey = false;
		this.CreditSumFC.UnitField = null;
		this.CreditSumFC.Visible = true;
		this.CreditSumFC.VisibleIndex = 33;
		this.CreditSumFC.Width = 100;
		this.TrsfrSumFC.Caption = "Transfer Amount FC";
		this.TrsfrSumFC.CurrencyDateField = null;
		this.TrsfrSumFC.CurrencyField = null;
		this.TrsfrSumFC.CurrencyFixed = null;
		this.TrsfrSumFC.CurrencyFixedRate = 0.0;
		this.TrsfrSumFC.CurrencyRateField = null;
		this.TrsfrSumFC.FieldName = "TrsfrSumFC";
		this.TrsfrSumFC.MinWidth = 100;
		this.TrsfrSumFC.Name = "TrsfrSumFC";
		this.TrsfrSumFC.OptionsColumn.AllowEdit = false;
		this.TrsfrSumFC.OptionsColumn.Printable = DevExpress.Utils.DefaultBoolean.False;
		this.TrsfrSumFC.OptionsColumn.ReadOnly = true;
		this.TrsfrSumFC.PrimaryKey = false;
		this.TrsfrSumFC.UnitField = null;
		this.TrsfrSumFC.Visible = true;
		this.TrsfrSumFC.VisibleIndex = 34;
		this.TrsfrSumFC.Width = 107;
		this.CashSumSy.Caption = "Cash Amount Sy";
		this.CashSumSy.CurrencyDateField = null;
		this.CashSumSy.CurrencyField = null;
		this.CashSumSy.CurrencyFixed = null;
		this.CashSumSy.CurrencyFixedRate = 0.0;
		this.CashSumSy.CurrencyRateField = null;
		this.CashSumSy.FieldName = "CashSumSy";
		this.CashSumSy.MinWidth = 100;
		this.CashSumSy.Name = "CashSumSy";
		this.CashSumSy.OptionsColumn.AllowEdit = false;
		this.CashSumSy.OptionsColumn.ReadOnly = true;
		this.CashSumSy.PrimaryKey = false;
		this.CashSumSy.UnitField = null;
		this.CashSumSy.Visible = true;
		this.CashSumSy.VisibleIndex = 35;
		this.CashSumSy.Width = 100;
		this.CreditSumSy.Caption = "Credit Amount Sy";
		this.CreditSumSy.CurrencyDateField = null;
		this.CreditSumSy.CurrencyField = null;
		this.CreditSumSy.CurrencyFixed = null;
		this.CreditSumSy.CurrencyFixedRate = 0.0;
		this.CreditSumSy.CurrencyRateField = null;
		this.CreditSumSy.FieldName = "CreditSumSy";
		this.CreditSumSy.MinWidth = 100;
		this.CreditSumSy.Name = "CreditSumSy";
		this.CreditSumSy.OptionsColumn.AllowEdit = false;
		this.CreditSumSy.OptionsColumn.ReadOnly = true;
		this.CreditSumSy.PrimaryKey = false;
		this.CreditSumSy.UnitField = null;
		this.CreditSumSy.Visible = true;
		this.CreditSumSy.VisibleIndex = 36;
		this.CreditSumSy.Width = 100;
		this.TrsfrSumSy.Caption = "Transfer Amount Sy";
		this.TrsfrSumSy.CurrencyDateField = null;
		this.TrsfrSumSy.CurrencyField = null;
		this.TrsfrSumSy.CurrencyFixed = null;
		this.TrsfrSumSy.CurrencyFixedRate = 0.0;
		this.TrsfrSumSy.CurrencyRateField = null;
		this.TrsfrSumSy.FieldName = "TrsfrSumSy";
		this.TrsfrSumSy.MinWidth = 100;
		this.TrsfrSumSy.Name = "TrsfrSumSy";
		this.TrsfrSumSy.OptionsColumn.AllowEdit = false;
		this.TrsfrSumSy.OptionsColumn.ReadOnly = true;
		this.TrsfrSumSy.PrimaryKey = false;
		this.TrsfrSumSy.UnitField = null;
		this.TrsfrSumSy.Visible = true;
		this.TrsfrSumSy.VisibleIndex = 37;
		this.TrsfrSumSy.Width = 106;
		this.VatSumFC.Caption = "Vat Amount FC";
		this.VatSumFC.CurrencyDateField = null;
		this.VatSumFC.CurrencyField = null;
		this.VatSumFC.CurrencyFixed = null;
		this.VatSumFC.CurrencyFixedRate = 0.0;
		this.VatSumFC.CurrencyRateField = null;
		this.VatSumFC.FieldName = "VatSumFC";
		this.VatSumFC.MinWidth = 100;
		this.VatSumFC.Name = "VatSumFC";
		this.VatSumFC.OptionsColumn.AllowEdit = false;
		this.VatSumFC.OptionsColumn.ReadOnly = true;
		this.VatSumFC.PrimaryKey = false;
		this.VatSumFC.UnitField = null;
		this.VatSumFC.Visible = true;
		this.VatSumFC.VisibleIndex = 38;
		this.VatSumFC.Width = 100;
		this.VatSumSy.Caption = "Vat Amount Sy";
		this.VatSumSy.CurrencyDateField = null;
		this.VatSumSy.CurrencyField = null;
		this.VatSumSy.CurrencyFixed = null;
		this.VatSumSy.CurrencyFixedRate = 0.0;
		this.VatSumSy.CurrencyRateField = null;
		this.VatSumSy.FieldName = "VatSumSy";
		this.VatSumSy.MinWidth = 100;
		this.VatSumSy.Name = "VatSumSy";
		this.VatSumSy.OptionsColumn.AllowEdit = false;
		this.VatSumSy.OptionsColumn.ReadOnly = true;
		this.VatSumSy.PrimaryKey = false;
		this.VatSumSy.UnitField = null;
		this.VatSumSy.Visible = true;
		this.VatSumSy.VisibleIndex = 39;
		this.VatSumSy.Width = 100;
		this.DcntSumFC.Caption = "Disc. Amount FC";
		this.DcntSumFC.CurrencyDateField = null;
		this.DcntSumFC.CurrencyField = null;
		this.DcntSumFC.CurrencyFixed = null;
		this.DcntSumFC.CurrencyFixedRate = 0.0;
		this.DcntSumFC.CurrencyRateField = null;
		this.DcntSumFC.FieldName = "DcntSumFC";
		this.DcntSumFC.MinWidth = 100;
		this.DcntSumFC.Name = "DcntSumFC";
		this.DcntSumFC.OptionsColumn.AllowEdit = false;
		this.DcntSumFC.OptionsColumn.ReadOnly = true;
		this.DcntSumFC.PrimaryKey = false;
		this.DcntSumFC.UnitField = null;
		this.DcntSumFC.Visible = true;
		this.DcntSumFC.VisibleIndex = 40;
		this.DcntSumFC.Width = 100;
		this.DocTotalFC.Caption = "Doc. Total FC";
		this.DocTotalFC.CurrencyDateField = null;
		this.DocTotalFC.CurrencyField = null;
		this.DocTotalFC.CurrencyFixed = null;
		this.DocTotalFC.CurrencyFixedRate = 0.0;
		this.DocTotalFC.CurrencyRateField = null;
		this.DocTotalFC.FieldName = "DocTotalFC";
		this.DocTotalFC.MinWidth = 100;
		this.DocTotalFC.Name = "DocTotalFC";
		this.DocTotalFC.OptionsColumn.AllowEdit = false;
		this.DocTotalFC.OptionsColumn.ReadOnly = true;
		this.DocTotalFC.PrimaryKey = false;
		this.DocTotalFC.UnitField = null;
		this.DocTotalFC.Visible = true;
		this.DocTotalFC.VisibleIndex = 41;
		this.DocTotalFC.Width = 100;
		this.DocTotalSy.Caption = "Doc. Total Sy";
		this.DocTotalSy.CurrencyDateField = null;
		this.DocTotalSy.CurrencyField = null;
		this.DocTotalSy.CurrencyFixed = null;
		this.DocTotalSy.CurrencyFixedRate = 0.0;
		this.DocTotalSy.CurrencyRateField = null;
		this.DocTotalSy.FieldName = "DocTotalSy";
		this.DocTotalSy.MinWidth = 100;
		this.DocTotalSy.Name = "DocTotalSy";
		this.DocTotalSy.OptionsColumn.AllowEdit = false;
		this.DocTotalSy.OptionsColumn.ReadOnly = true;
		this.DocTotalSy.PrimaryKey = false;
		this.DocTotalSy.UnitField = null;
		this.DocTotalSy.Visible = true;
		this.DocTotalSy.VisibleIndex = 42;
		this.DocTotalSy.Width = 100;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.DisplayValueChecked = "Y";
		this.repositoryItemCheckEdit1.DisplayValueUnchecked = "N";
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.repositoryItemComboBox1.AutoHeight = false;
		this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemComboBox1.Items.AddRange(new object[3] { "S", "C", "V" });
		this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(639, 356);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(99, 20);
		this.btCancel.TabIndex = 38;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		base.ClientSize = new System.Drawing.Size(745, 378);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.grdDoc);
		base.Name = "frm_PaymentIncomingList";
		this.Text = "Payment Incoming List";
		base.Load += new System.EventHandler(this.frm_PaymentIncomingList_Load);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox1).EndInit();
		base.ResumeLayout(false);
	}
}
