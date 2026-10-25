using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using MessagingToolkit.QRCode.Codec;

namespace Apzon.Forms.Administrator;

public class frm_POS : XtraFormEx
{
	private const string FormController = "Retail";

	private double _invoiceTotal;

	private double _invoiceTax;

	private double _totalAmount;

	private double _invoiceDiscount;

	private bool _cellediting;

	private IContainer components;

	private TileGroup tileGroup1;

	private LayoutControl layoutControl1;

	private LayoutControlGroup layoutControlGroup1;

	private TextEditEx txtBarCode;

	private TabPane tabPane1;

	private TabNavigationPage MainPage;

	private TabNavigationPage DiscountPage;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem5;

	private LayoutControlItemEx layoutControlItem1;

	private LayoutControlItemEx layoutControlItem2;

	private TileControl tileControl1;

	private TileGroup tileGroup4;

	private TileItem tileItem3;

	private TileItem tileItem4;

	private TileItem tileItem5;

	private TileItem tileItem6;

	private TileItem tileItem7;

	private TileItem tileItem8;

	private TileControl tileControl2;

	private TileGroup tileGroup5;

	private TileItem tileItem9;

	private TileItem tileItem10;

	private TileItem tileItem11;

	private TileItem tileItem12;

	private TileItem tileItem13;

	private TileItem tileItem14;

	private TileGroup tileGroup3;

	private TileItem tileItem2;

	private TileItem tileItem1;

	private PictureEdit pictureEdit1;

	private LayoutControlItemEx layoutControlItem4;

	private LabelControl lblEmail;

	private LabelControl labelControl6;

	private LabelControl labelControl5;

	private LabelControl lblLoyaltyP;

	private LabelControl labelControl2;

	private LabelControl labelControl3;

	private LabelControl labelControl1;

	private LabelControl labelControl36;

	private LayoutControl layoutControl4;

	private LabelControl labelControl21;

	private LabelControl lblUserLogin;

	private LabelControl labelControl24;

	private LabelControl lblStoreId;

	private LabelControl labelControl15;

	private LabelControl lblDate;

	private LabelControl labelControl17;

	private LabelControl lblBussinessDate;

	private LayoutControlGroup layoutControlGroup3;

	private LayoutControlGroup layoutControlGroup5;

	private LayoutControlItemEx layoutControlItem23;

	private LayoutControlItemEx layoutControlItem24;

	private LayoutControlItemEx layoutControlItem25;

	private LayoutControlItemEx layoutControlItem26;

	private LayoutControlItemEx layoutControlItem29;

	private LayoutControlItemEx layoutControlItem30;

	private LayoutControlItemEx layoutControlItem31;

	private LayoutControlItemEx layoutControlItem32;

	private EmptySpaceItem emptySpaceItem3;

	private EmptySpaceItem emptySpaceItem6;

	private LayoutControlItemEx layoutControlItem22;

	private EmptySpaceItem emptySpaceItem2;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn7;

	private LayoutControlItemEx layoutControlItem27;

	private TableLayoutPanel tableLayoutPanel1;

	private LabelControl lblBalance;

	private LabelControl lblTaxAmount;

	private LabelControl lblInvoiceTotal;

	private LabelControl lblDiscountAmount;

	private LabelControl labelControl4;

	private LabelControl labelControl8;

	private LabelControl labelControl10;

	private LabelControl labelControl9;

	private LabelControl labelControl7;

	private EmptySpaceItem emptySpaceItem9;

	private LayoutControlItemEx layoutControlItem3;

	private LabelControl lblSubTotal;

	private LabelControl lblItemQuantity;

	private TableLayoutPanel tableLayoutPanel3;

	private LayoutControlItemEx layoutControlItem6;

	private EmptySpaceItem emptySpaceItem4;

	private LabelControl labelControl19;

	private LabelControl labelControl18;

	private LabelControl labelControl16;

	private LabelControl labelControl14;

	private LabelControl labelControl13;

	private LabelControl labelControl12;

	private LabelControl labelControl11;

	private TileItem tileItem15;

	private TileItem tileItem16;

	private TileItem tileItem17;

	private TileItem tileItem18;

	private TableLayoutPanel tableLayoutPanel2;

	private LabelControl labelControl20;

	private LayoutControlItemEx layoutControlItem5;

	public Size OldSize { get; set; }

	public frm_POS()
	{
		InitializeComponent();
	}

	private void LoadData()
	{
		try
		{
			int num = 0;
			_invoiceTotal = 0.0;
			_invoiceTax = 0.0;
			_totalAmount = 0.0;
			DataTable dataTable = grdDoc.DataSource as DataTable;
			string barCode = Function.ToString(txtBarCode.Text);
			DataTable dataTable2;
			using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Retail"))
			{
				dataTable2 = httpClientHelper.Get("GetDocByCode", new
				{
					barCode = barCode,
					userName = GlobalData.UserNameLogin,
					branch = GlobalData.BranchLogin
				});
			}
			if (dataTable2.IsNotNull() && dataTable.IsNotNull())
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					if (Function.ToString(dataTable.Rows[i]["ItemCode"]).Equals(Function.ToString(dataTable2.Rows[0]["ItemCode"])))
					{
						double num2 = Function.ParseDouble(dataTable.Rows[i]["Price"]);
						double num3 = Function.ParseDouble(dataTable.Rows[i]["TaxRate"]);
						double num4 = Function.ParseDouble(dataTable.Rows[i]["Discount"]);
						dataTable.Rows[i]["Qty"] = Function.ParseInt(dataTable.Rows[i]["Qty"]) + 1;
						dataTable.Rows[i]["Total"] = Function.ParseDouble(dataTable.Rows[i]["Total"]) + num2 - num2 * num4 / 100.0 + (num2 - num2 * num4 / 100.0) * num3 / 100.0;
						dataTable.Rows[i]["TaxAmount"] = Function.ParseDouble(dataTable.Rows[i]["TaxAmount"]) + (num2 - num2 * num4 / 100.0) * num3 / 100.0;
						num++;
						break;
					}
				}
				if (num > 0)
				{
					grdDoc.DataSource = dataTable;
					grdDoc.RefreshDataSource();
					lblInvoiceTotal.Text = Function.ToString(_invoiceTotal);
					lblTaxAmount.Text = Function.ToString(_invoiceTax);
					lblSubTotal.Text = Function.ToString(_totalAmount);
				}
				else
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["ItemCode"] = dataTable2.Rows[0]["ItemCode"];
					dataRow["ItemName"] = dataTable2.Rows[0]["ItemName"];
					dataRow["Qty"] = dataTable2.Rows[0]["Qty"];
					dataRow["Price"] = dataTable2.Rows[0]["Price"];
					dataRow["Discount"] = dataTable2.Rows[0]["Discount"];
					dataRow["TaxRate"] = dataTable2.Rows[0]["TaxRate"];
					dataRow["TaxAmount"] = dataTable2.Rows[0]["TaxAmount"];
					dataRow["Total"] = dataTable2.Rows[0]["Total"];
					dataTable.Rows.Add(dataRow);
					grdDoc.DataSource = dataTable;
					grdDoc.RefreshDataSource();
				}
				for (int j = 0; j < dataTable.Rows.Count; j++)
				{
					_invoiceTotal += Function.ParseDouble(dataTable.Rows[j]["Total"]);
					_invoiceTax += Function.ParseDouble(dataTable.Rows[j]["TaxAmount"]);
					_totalAmount = _invoiceTotal - _invoiceTax;
				}
			}
			else if (dataTable2.IsNotNull() && !dataTable.IsNotNull())
			{
				for (int k = 0; k < dataTable2.Rows.Count; k++)
				{
					_invoiceTotal += Function.ParseDouble(dataTable2.Rows[k]["Total"]);
					_invoiceTax += Function.ParseDouble(dataTable2.Rows[k]["TaxAmount"]);
					_totalAmount = _invoiceTotal - _invoiceTax;
				}
				grdDoc.DataSource = dataTable2;
				grdDoc.RefreshDataSource();
			}
			lblSubTotal.Text = Function.ToString(_totalAmount.ToString("#,##0.00"));
			lblInvoiceTotal.Text = Function.ToString(_invoiceTotal.ToString("#,##0.00"));
			lblTaxAmount.Text = Function.ToString(_invoiceTax.ToString("#,##0.00"));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_POS_Load(object sender, EventArgs e)
	{
		new QRCodeEncoder().Encode("CC15-001\nCC15-001CC15-001CC15-001CC15-001CC15-001CC15-001CC15-001CC15-001");
	}

	private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return && !string.IsNullOrEmpty(txtBarCode.Text))
			{
				LoadData();
				txtBarCode.Text = string.Empty;
				txtBarCode.Enabled = true;
				txtBarCode.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			Function.ToString(gvDoc.GetFocusedRowCellValue("ItemCode"));
			Function.ToString(gvDoc.GetFocusedRowCellValue("ItemName"));
			double num = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("Price"));
			double num2 = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("TaxRate"));
			double num3 = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("Discount"));
			int num4 = Function.ParseInt(gvDoc.GetFocusedRowCellValue("Qty"));
			Function.ParseDouble(gvDoc.GetFocusedRowCellValue("TaxAmount"));
			Function.ParseDouble(gvDoc.GetFocusedRowCellValue("Total"));
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			if (!_cellediting && (e.Column.FieldName == "Qty" || e.Column.FieldName == "TaxRate" || e.Column.FieldName == "Discount" || e.Column.FieldName == "Price"))
			{
				_invoiceTotal = 0.0;
				_invoiceTax = 0.0;
				_totalAmount = 0.0;
				_invoiceDiscount = 0.0;
				_cellediting = true;
				gvDoc.SetFocusedRowCellValue("Total", (num - num * num3 / 100.0 + (num - num * num3 / 100.0) * num2 / 100.0) * (double)num4);
				gvDoc.SetFocusedRowCellValue("TaxAmount", (num - num * num3 / 100.0) * num2 / 100.0 * (double)num4);
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					_invoiceTotal += Function.ParseDouble(dataTable.Rows[i]["Total"]);
					_invoiceTax += Function.ParseDouble(dataTable.Rows[i]["TaxAmount"]);
					_totalAmount = _invoiceTotal - _invoiceTax;
				}
				_cellediting = false;
			}
			lblSubTotal.Text = Function.ToString(_totalAmount.ToString("#,##0.00"));
			lblInvoiceTotal.Text = Function.ToString(_invoiceTotal.ToString("#,##0.00"));
			lblTaxAmount.Text = Function.ToString(_invoiceTax.ToString("#,##0.00"));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void lblInvoiceTotal_Resize(object sender, EventArgs e)
	{
		try
		{
			if (sender is LabelControl { Size: { Width: var num } } labelControl)
			{
				int num2 = num - OldSize.Width;
				labelControl.Left -= num2;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void lblInvoiceTotal_TextChanged(object sender, EventArgs e)
	{
		try
		{
			if (sender is LabelControl labelControl)
			{
				OldSize = labelControl.Size;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txtPayment_EditValueChanged(object sender, EventArgs e)
	{
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
		DevExpress.XtraEditors.TileItemElement tileItemElement = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement2 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement3 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement4 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement5 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement6 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement7 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement8 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement9 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement10 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement11 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement12 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement13 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement14 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement15 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement16 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement17 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement18 = new DevExpress.XtraEditors.TileItemElement();
		this.tileGroup1 = new DevExpress.XtraEditors.TileGroup();
		this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
		this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
		this.labelControl20 = new DevExpress.XtraEditors.LabelControl();
		this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
		this.labelControl19 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl18 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl16 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl14 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl13 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl12 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
		this.lblEmail = new DevExpress.XtraEditors.LabelControl();
		this.lblLoyaltyP = new DevExpress.XtraEditors.LabelControl();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl11 = new DevExpress.XtraEditors.LabelControl();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.lblSubTotal = new DevExpress.XtraEditors.LabelControl();
		this.lblBalance = new DevExpress.XtraEditors.LabelControl();
		this.lblTaxAmount = new DevExpress.XtraEditors.LabelControl();
		this.lblItemQuantity = new DevExpress.XtraEditors.LabelControl();
		this.lblInvoiceTotal = new DevExpress.XtraEditors.LabelControl();
		this.lblDiscountAmount = new DevExpress.XtraEditors.LabelControl();
		this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl10 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl36 = new DevExpress.XtraEditors.LabelControl();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.layoutControl4 = new DevExpress.XtraLayout.LayoutControl();
		this.labelControl21 = new DevExpress.XtraEditors.LabelControl();
		this.lblUserLogin = new DevExpress.XtraEditors.LabelControl();
		this.labelControl24 = new DevExpress.XtraEditors.LabelControl();
		this.lblStoreId = new DevExpress.XtraEditors.LabelControl();
		this.labelControl15 = new DevExpress.XtraEditors.LabelControl();
		this.lblDate = new DevExpress.XtraEditors.LabelControl();
		this.labelControl17 = new DevExpress.XtraEditors.LabelControl();
		this.lblBussinessDate = new DevExpress.XtraEditors.LabelControl();
		this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlGroup5 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem23 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem24 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem25 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem26 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem29 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem30 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem31 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem32 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
		this.txtBarCode = new Apzon.Controls.Editor.TextEditEx();
		this.tabPane1 = new DevExpress.XtraBars.Navigation.TabPane();
		this.MainPage = new DevExpress.XtraBars.Navigation.TabNavigationPage();
		this.tileControl1 = new DevExpress.XtraEditors.TileControl();
		this.tileGroup4 = new DevExpress.XtraEditors.TileGroup();
		this.tileItem3 = new DevExpress.XtraEditors.TileItem();
		this.tileItem4 = new DevExpress.XtraEditors.TileItem();
		this.tileItem5 = new DevExpress.XtraEditors.TileItem();
		this.tileItem6 = new DevExpress.XtraEditors.TileItem();
		this.tileItem7 = new DevExpress.XtraEditors.TileItem();
		this.tileItem8 = new DevExpress.XtraEditors.TileItem();
		this.tileItem15 = new DevExpress.XtraEditors.TileItem();
		this.tileItem16 = new DevExpress.XtraEditors.TileItem();
		this.DiscountPage = new DevExpress.XtraBars.Navigation.TabNavigationPage();
		this.tileControl2 = new DevExpress.XtraEditors.TileControl();
		this.tileGroup5 = new DevExpress.XtraEditors.TileGroup();
		this.tileItem9 = new DevExpress.XtraEditors.TileItem();
		this.tileItem10 = new DevExpress.XtraEditors.TileItem();
		this.tileItem11 = new DevExpress.XtraEditors.TileItem();
		this.tileItem12 = new DevExpress.XtraEditors.TileItem();
		this.tileItem13 = new DevExpress.XtraEditors.TileItem();
		this.tileItem14 = new DevExpress.XtraEditors.TileItem();
		this.tileItem17 = new DevExpress.XtraEditors.TileItem();
		this.tileItem18 = new DevExpress.XtraEditors.TileItem();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem2 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem4 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem22 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem27 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem9 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem3 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem6 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem5 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.tileGroup3 = new DevExpress.XtraEditors.TileGroup();
		this.tileItem2 = new DevExpress.XtraEditors.TileItem();
		this.tileItem1 = new DevExpress.XtraEditors.TileItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		this.tableLayoutPanel2.SuspendLayout();
		this.tableLayoutPanel3.SuspendLayout();
		this.tableLayoutPanel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl4).BeginInit();
		this.layoutControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem23).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem24).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem25).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem26).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem29).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem30).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem31).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem32).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pictureEdit1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBarCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tabPane1).BeginInit();
		this.tabPane1.SuspendLayout();
		this.MainPage.SuspendLayout();
		this.DiscountPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem22).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem27).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		base.SuspendLayout();
		this.tileGroup1.Name = "tileGroup1";
		this.layoutControl1.Controls.Add(this.tableLayoutPanel2);
		this.layoutControl1.Controls.Add(this.tableLayoutPanel3);
		this.layoutControl1.Controls.Add(this.tableLayoutPanel1);
		this.layoutControl1.Controls.Add(this.grdDoc);
		this.layoutControl1.Controls.Add(this.layoutControl4);
		this.layoutControl1.Controls.Add(this.pictureEdit1);
		this.layoutControl1.Controls.Add(this.txtBarCode);
		this.layoutControl1.Controls.Add(this.tabPane1);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Margin = new System.Windows.Forms.Padding(0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(294, 317, 250, 350);
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(1286, 710);
		this.layoutControl1.TabIndex = 0;
		this.layoutControl1.Text = "layoutControl1";
		this.tableLayoutPanel2.ColumnCount = 1;
		this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20f));
		this.tableLayoutPanel2.Controls.Add(this.labelControl20, 0, 0);
		this.tableLayoutPanel2.Location = new System.Drawing.Point(919, 13);
		this.tableLayoutPanel2.Name = "tableLayoutPanel2";
		this.tableLayoutPanel2.RowCount = 1;
		this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20f));
		this.tableLayoutPanel2.Size = new System.Drawing.Size(364, 27);
		this.tableLayoutPanel2.TabIndex = 33;
		this.labelControl20.Appearance.Font = new System.Drawing.Font("Tahoma", 13.25f);
		this.labelControl20.Appearance.Options.UseFont = true;
		this.labelControl20.Location = new System.Drawing.Point(3, 3);
		this.labelControl20.Name = "labelControl20";
		this.labelControl20.Size = new System.Drawing.Size(174, 22);
		this.labelControl20.TabIndex = 0;
		this.labelControl20.Text = "Customer Information";
		this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
		this.tableLayoutPanel3.ColumnCount = 2;
		this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.37465f));
		this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 71.62534f));
		this.tableLayoutPanel3.Controls.Add(this.labelControl19, 1, 6);
		this.tableLayoutPanel3.Controls.Add(this.labelControl18, 1, 5);
		this.tableLayoutPanel3.Controls.Add(this.labelControl16, 1, 4);
		this.tableLayoutPanel3.Controls.Add(this.labelControl14, 1, 3);
		this.tableLayoutPanel3.Controls.Add(this.labelControl13, 1, 2);
		this.tableLayoutPanel3.Controls.Add(this.labelControl12, 1, 1);
		this.tableLayoutPanel3.Controls.Add(this.labelControl5, 0, 4);
		this.tableLayoutPanel3.Controls.Add(this.labelControl6, 0, 5);
		this.tableLayoutPanel3.Controls.Add(this.lblEmail, 0, 6);
		this.tableLayoutPanel3.Controls.Add(this.lblLoyaltyP, 0, 3);
		this.tableLayoutPanel3.Controls.Add(this.labelControl2, 0, 2);
		this.tableLayoutPanel3.Controls.Add(this.labelControl3, 0, 1);
		this.tableLayoutPanel3.Controls.Add(this.labelControl1, 0, 0);
		this.tableLayoutPanel3.Controls.Add(this.labelControl11, 1, 0);
		this.tableLayoutPanel3.Location = new System.Drawing.Point(919, 44);
		this.tableLayoutPanel3.Name = "tableLayoutPanel3";
		this.tableLayoutPanel3.RowCount = 7;
		this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571f));
		this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571f));
		this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571f));
		this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571f));
		this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571f));
		this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571f));
		this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571f));
		this.tableLayoutPanel3.Size = new System.Drawing.Size(364, 326);
		this.tableLayoutPanel3.TabIndex = 32;
		this.labelControl19.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.labelControl19.Appearance.Options.UseFont = true;
		this.labelControl19.Location = new System.Drawing.Point(107, 280);
		this.labelControl19.Name = "labelControl19";
		this.labelControl19.Size = new System.Drawing.Size(113, 19);
		this.labelControl19.TabIndex = 18;
		this.labelControl19.Text = "abc@gmail.com";
		this.labelControl18.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.labelControl18.Appearance.Options.UseFont = true;
		this.labelControl18.Location = new System.Drawing.Point(107, 234);
		this.labelControl18.Name = "labelControl18";
		this.labelControl18.Size = new System.Drawing.Size(81, 19);
		this.labelControl18.TabIndex = 17;
		this.labelControl18.Text = "123456789";
		this.labelControl16.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.labelControl16.Appearance.Options.UseFont = true;
		this.labelControl16.Location = new System.Drawing.Point(107, 188);
		this.labelControl16.Name = "labelControl16";
		this.labelControl16.Size = new System.Drawing.Size(9, 19);
		this.labelControl16.TabIndex = 16;
		this.labelControl16.Text = "0";
		this.labelControl14.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.labelControl14.Appearance.Options.UseFont = true;
		this.labelControl14.Location = new System.Drawing.Point(107, 142);
		this.labelControl14.Name = "labelControl14";
		this.labelControl14.Size = new System.Drawing.Size(36, 19);
		this.labelControl14.TabIndex = 15;
		this.labelControl14.Text = "2000";
		this.labelControl13.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.labelControl13.Appearance.Options.UseFont = true;
		this.labelControl13.Location = new System.Drawing.Point(107, 96);
		this.labelControl13.Name = "labelControl13";
		this.labelControl13.Size = new System.Drawing.Size(9, 19);
		this.labelControl13.TabIndex = 14;
		this.labelControl13.Text = "1";
		this.labelControl12.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.labelControl12.Appearance.Options.UseFont = true;
		this.labelControl12.Location = new System.Drawing.Point(107, 50);
		this.labelControl12.Name = "labelControl12";
		this.labelControl12.Size = new System.Drawing.Size(34, 19);
		this.labelControl12.TabIndex = 13;
		this.labelControl12.Text = "Cash";
		this.labelControl5.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.labelControl5.Appearance.Options.UseFont = true;
		this.labelControl5.Location = new System.Drawing.Point(4, 188);
		this.labelControl5.Name = "labelControl5";
		this.labelControl5.Size = new System.Drawing.Size(53, 19);
		this.labelControl5.TabIndex = 9;
		this.labelControl5.Text = "Balance";
		this.labelControl6.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.labelControl6.Appearance.Options.UseFont = true;
		this.labelControl6.Location = new System.Drawing.Point(4, 234);
		this.labelControl6.Name = "labelControl6";
		this.labelControl6.Size = new System.Drawing.Size(100, 19);
		this.labelControl6.TabIndex = 10;
		this.labelControl6.Text = "Mobile  Phone";
		this.lblEmail.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.lblEmail.Appearance.Options.UseFont = true;
		this.lblEmail.Location = new System.Drawing.Point(4, 280);
		this.lblEmail.Name = "lblEmail";
		this.lblEmail.Size = new System.Drawing.Size(39, 19);
		this.lblEmail.TabIndex = 11;
		this.lblEmail.Text = "Email";
		this.lblLoyaltyP.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.lblLoyaltyP.Appearance.Options.UseFont = true;
		this.lblLoyaltyP.Location = new System.Drawing.Point(4, 142);
		this.lblLoyaltyP.Name = "lblLoyaltyP";
		this.lblLoyaltyP.Size = new System.Drawing.Size(98, 19);
		this.lblLoyaltyP.TabIndex = 8;
		this.lblLoyaltyP.Text = "Loyalty Points";
		this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.labelControl2.Appearance.Options.UseFont = true;
		this.labelControl2.Location = new System.Drawing.Point(4, 96);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(91, 19);
		this.labelControl2.TabIndex = 7;
		this.labelControl2.Text = "Loyalty Level";
		this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.labelControl3.Appearance.Options.UseFont = true;
		this.labelControl3.Location = new System.Drawing.Point(4, 50);
		this.labelControl3.Name = "labelControl3";
		this.labelControl3.Size = new System.Drawing.Size(41, 19);
		this.labelControl3.TabIndex = 6;
		this.labelControl3.Text = "Name";
		this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.labelControl1.Appearance.Options.UseFont = true;
		this.labelControl1.Location = new System.Drawing.Point(4, 4);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(17, 19);
		this.labelControl1.TabIndex = 4;
		this.labelControl1.Text = "ID";
		this.labelControl11.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.labelControl11.Appearance.Options.UseFont = true;
		this.labelControl11.Location = new System.Drawing.Point(107, 4);
		this.labelControl11.Name = "labelControl11";
		this.labelControl11.Size = new System.Drawing.Size(27, 19);
		this.labelControl11.TabIndex = 12;
		this.labelControl11.Text = "123";
		this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
		this.tableLayoutPanel1.ColumnCount = 6;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17.5f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.5f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20f));
		this.tableLayoutPanel1.Controls.Add(this.lblSubTotal, 1, 0);
		this.tableLayoutPanel1.Controls.Add(this.lblBalance, 5, 1);
		this.tableLayoutPanel1.Controls.Add(this.lblTaxAmount, 3, 1);
		this.tableLayoutPanel1.Controls.Add(this.lblItemQuantity, 1, 1);
		this.tableLayoutPanel1.Controls.Add(this.lblInvoiceTotal, 5, 0);
		this.tableLayoutPanel1.Controls.Add(this.lblDiscountAmount, 3, 0);
		this.tableLayoutPanel1.Controls.Add(this.labelControl4, 0, 0);
		this.tableLayoutPanel1.Controls.Add(this.labelControl8, 2, 0);
		this.tableLayoutPanel1.Controls.Add(this.labelControl10, 4, 0);
		this.tableLayoutPanel1.Controls.Add(this.labelControl9, 2, 1);
		this.tableLayoutPanel1.Controls.Add(this.labelControl7, 0, 1);
		this.tableLayoutPanel1.Controls.Add(this.labelControl36, 4, 1);
		this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 592);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.RowCount = 2;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.Size = new System.Drawing.Size(725, 53);
		this.tableLayoutPanel1.TabIndex = 30;
		this.lblSubTotal.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.lblSubTotal.Appearance.Options.UseFont = true;
		this.lblSubTotal.Location = new System.Drawing.Point(112, 4);
		this.lblSubTotal.Name = "lblSubTotal";
		this.lblSubTotal.Size = new System.Drawing.Size(0, 19);
		this.lblSubTotal.TabIndex = 17;
		this.lblBalance.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.lblBalance.Appearance.Options.UseFont = true;
		this.lblBalance.Location = new System.Drawing.Point(580, 30);
		this.lblBalance.Name = "lblBalance";
		this.lblBalance.Size = new System.Drawing.Size(0, 19);
		this.lblBalance.TabIndex = 24;
		this.lblTaxAmount.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.lblTaxAmount.Appearance.Options.UseFont = true;
		this.lblTaxAmount.Location = new System.Drawing.Point(382, 30);
		this.lblTaxAmount.Name = "lblTaxAmount";
		this.lblTaxAmount.Size = new System.Drawing.Size(0, 19);
		this.lblTaxAmount.TabIndex = 23;
		this.lblItemQuantity.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.lblItemQuantity.Appearance.Options.UseFont = true;
		this.lblItemQuantity.Location = new System.Drawing.Point(112, 30);
		this.lblItemQuantity.Name = "lblItemQuantity";
		this.lblItemQuantity.Size = new System.Drawing.Size(0, 19);
		this.lblItemQuantity.TabIndex = 22;
		this.lblInvoiceTotal.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.lblInvoiceTotal.Appearance.Options.UseFont = true;
		this.lblInvoiceTotal.Location = new System.Drawing.Point(580, 4);
		this.lblInvoiceTotal.Name = "lblInvoiceTotal";
		this.lblInvoiceTotal.Size = new System.Drawing.Size(0, 19);
		this.lblInvoiceTotal.TabIndex = 21;
		this.lblDiscountAmount.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.lblDiscountAmount.Appearance.Options.UseFont = true;
		this.lblDiscountAmount.Location = new System.Drawing.Point(382, 4);
		this.lblDiscountAmount.Name = "lblDiscountAmount";
		this.lblDiscountAmount.Size = new System.Drawing.Size(0, 19);
		this.lblDiscountAmount.TabIndex = 20;
		this.labelControl4.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.labelControl4.Appearance.Options.UseFont = true;
		this.labelControl4.Location = new System.Drawing.Point(4, 4);
		this.labelControl4.Name = "labelControl4";
		this.labelControl4.Size = new System.Drawing.Size(73, 19);
		this.labelControl4.TabIndex = 5;
		this.labelControl4.Text = "Sub. Total";
		this.labelControl8.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.labelControl8.Appearance.Options.UseFont = true;
		this.labelControl8.Location = new System.Drawing.Point(256, 4);
		this.labelControl8.Name = "labelControl8";
		this.labelControl8.Size = new System.Drawing.Size(123, 19);
		this.labelControl8.TabIndex = 8;
		this.labelControl8.Text = "Discount Amount";
		this.labelControl10.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.labelControl10.Appearance.Options.UseFont = true;
		this.labelControl10.Location = new System.Drawing.Point(526, 4);
		this.labelControl10.Name = "labelControl10";
		this.labelControl10.Size = new System.Drawing.Size(36, 19);
		this.labelControl10.TabIndex = 18;
		this.labelControl10.Text = "Total";
		this.labelControl9.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.labelControl9.Appearance.Options.UseFont = true;
		this.labelControl9.Location = new System.Drawing.Point(256, 30);
		this.labelControl9.Name = "labelControl9";
		this.labelControl9.Size = new System.Drawing.Size(88, 19);
		this.labelControl9.TabIndex = 17;
		this.labelControl9.Text = "Tax Amount";
		this.labelControl7.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.labelControl7.Appearance.Options.UseFont = true;
		this.labelControl7.Location = new System.Drawing.Point(4, 30);
		this.labelControl7.Name = "labelControl7";
		this.labelControl7.Size = new System.Drawing.Size(105, 19);
		this.labelControl7.TabIndex = 6;
		this.labelControl7.Text = "Items Quantity";
		this.labelControl36.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.labelControl36.Appearance.Options.UseFont = true;
		this.labelControl36.Location = new System.Drawing.Point(526, 30);
		this.labelControl36.Name = "labelControl36";
		this.labelControl36.Size = new System.Drawing.Size(53, 19);
		this.labelControl36.TabIndex = 13;
		this.labelControl36.Text = "Balance";
		this.grdDoc.AllowPasteExcel = false;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Location = new System.Drawing.Point(3, 37);
		this.grdDoc.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.grdDoc.LookAndFeel.UseDefaultLookAndFeel = false;
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.Size = new System.Drawing.Size(725, 541);
		this.grdDoc.TabIndex = 29;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.gvDoc.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.gvDoc.Appearance.HeaderPanel.Options.UseFont = true;
		this.gvDoc.Appearance.Row.Font = new System.Drawing.Font("Tahoma", 10.25f);
		this.gvDoc.Appearance.Row.Options.UseFont = true;
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn8, this.gridColumn7 });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = null;
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 40;
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
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gridColumn1.Caption = "Item Code";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "ItemCode";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn2.Caption = "Item Name";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "ItemName";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn3.Caption = "Quantity";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "Qty";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 2;
		this.gridColumn4.Caption = "Price";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.DisplayFormat.FormatString = "n0";
		this.gridColumn4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn4.FieldName = "Price";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 3;
		this.gridColumn5.Caption = "Discounts (%)";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "Discount";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 4;
		this.gridColumn6.Caption = "Tax Rate(%)";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "TaxRate";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 5;
		this.gridColumn8.Caption = "Tax Amount";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.DisplayFormat.FormatString = "n0";
		this.gridColumn8.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn8.FieldName = "TaxAmount";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 6;
		this.gridColumn7.Caption = "Amount";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.DisplayFormat.FormatString = "n0";
		this.gridColumn7.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn7.FieldName = "Total";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.OptionsColumn.AllowEdit = false;
		this.gridColumn7.OptionsColumn.ReadOnly = true;
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 7;
		this.layoutControl4.Controls.Add(this.labelControl21);
		this.layoutControl4.Controls.Add(this.lblUserLogin);
		this.layoutControl4.Controls.Add(this.labelControl24);
		this.layoutControl4.Controls.Add(this.lblStoreId);
		this.layoutControl4.Controls.Add(this.labelControl15);
		this.layoutControl4.Controls.Add(this.lblDate);
		this.layoutControl4.Controls.Add(this.labelControl17);
		this.layoutControl4.Controls.Add(this.lblBussinessDate);
		this.layoutControl4.Location = new System.Drawing.Point(3, 659);
		this.layoutControl4.Name = "layoutControl4";
		this.layoutControl4.Root = this.layoutControlGroup3;
		this.layoutControl4.Size = new System.Drawing.Size(725, 48);
		this.layoutControl4.TabIndex = 27;
		this.layoutControl4.Text = "layoutControl4";
		this.labelControl21.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f);
		this.labelControl21.Appearance.Options.UseFont = true;
		this.labelControl21.Location = new System.Drawing.Point(3, 3);
		this.labelControl21.Name = "labelControl21";
		this.labelControl21.Size = new System.Drawing.Size(43, 17);
		this.labelControl21.StyleController = this.layoutControl4;
		this.labelControl21.TabIndex = 4;
		this.labelControl21.Text = "User Id";
		this.lblUserLogin.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f);
		this.lblUserLogin.Appearance.Options.UseFont = true;
		this.lblUserLogin.Location = new System.Drawing.Point(50, 3);
		this.lblUserLogin.Name = "lblUserLogin";
		this.lblUserLogin.Size = new System.Drawing.Size(52, 17);
		this.lblUserLogin.StyleController = this.layoutControl4;
		this.lblUserLogin.TabIndex = 6;
		this.lblUserLogin.Text = "Manager";
		this.labelControl24.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f);
		this.labelControl24.Appearance.Options.UseFont = true;
		this.labelControl24.Location = new System.Drawing.Point(581, 3);
		this.labelControl24.Name = "labelControl24";
		this.labelControl24.Size = new System.Drawing.Size(49, 17);
		this.labelControl24.StyleController = this.layoutControl4;
		this.labelControl24.TabIndex = 7;
		this.labelControl24.Text = "Store Id";
		this.lblStoreId.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f);
		this.lblStoreId.Appearance.Options.UseFont = true;
		this.lblStoreId.Location = new System.Drawing.Point(634, 3);
		this.lblStoreId.Name = "lblStoreId";
		this.lblStoreId.Size = new System.Drawing.Size(88, 17);
		this.lblStoreId.StyleController = this.layoutControl4;
		this.lblStoreId.TabIndex = 8;
		this.lblStoreId.Text = "12345645677";
		this.labelControl15.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f);
		this.labelControl15.Appearance.Options.UseFont = true;
		this.labelControl15.Location = new System.Drawing.Point(3, 24);
		this.labelControl15.Name = "labelControl15";
		this.labelControl15.Size = new System.Drawing.Size(29, 17);
		this.labelControl15.StyleController = this.layoutControl4;
		this.labelControl15.TabIndex = 5;
		this.labelControl15.Text = "Date";
		this.lblDate.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f);
		this.lblDate.Appearance.Options.UseFont = true;
		this.lblDate.Location = new System.Drawing.Point(50, 24);
		this.lblDate.Name = "lblDate";
		this.lblDate.Size = new System.Drawing.Size(74, 17);
		this.lblDate.StyleController = this.layoutControl4;
		this.lblDate.TabIndex = 11;
		this.lblDate.Text = "13/01/2017";
		this.labelControl17.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f);
		this.labelControl17.Appearance.Options.UseFont = true;
		this.labelControl17.Location = new System.Drawing.Point(540, 24);
		this.labelControl17.Name = "labelControl17";
		this.labelControl17.Size = new System.Drawing.Size(90, 17);
		this.labelControl17.StyleController = this.layoutControl4;
		this.labelControl17.TabIndex = 16;
		this.labelControl17.Text = "Bussiness Date";
		this.lblBussinessDate.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f);
		this.lblBussinessDate.Appearance.Options.UseFont = true;
		this.lblBussinessDate.Location = new System.Drawing.Point(634, 24);
		this.lblBussinessDate.Name = "lblBussinessDate";
		this.lblBussinessDate.Size = new System.Drawing.Size(74, 17);
		this.lblBussinessDate.StyleController = this.layoutControl4;
		this.lblBussinessDate.TabIndex = 12;
		this.lblBussinessDate.Text = "13/01/2017";
		this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup3.GroupBordersVisible = false;
		this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlGroup5 });
		this.layoutControlGroup3.Name = "layoutControlGroup3";
		this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(1, 1, 1, 1);
		this.layoutControlGroup3.Size = new System.Drawing.Size(725, 48);
		this.layoutControlGroup3.TextVisible = false;
		this.layoutControlGroup5.CustomizationFormText = "Root";
		this.layoutControlGroup5.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup5.GroupBordersVisible = false;
		this.layoutControlGroup5.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[10] { this.layoutControlItem23, this.layoutControlItem24, this.layoutControlItem25, this.layoutControlItem26, this.layoutControlItem29, this.layoutControlItem30, this.layoutControlItem31, this.layoutControlItem32, this.emptySpaceItem3, this.emptySpaceItem6 });
		this.layoutControlGroup5.Location = new System.Drawing.Point(0, 0);
		this.layoutControlGroup5.Name = "layoutControlGroup5";
		this.layoutControlGroup5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup5.Size = new System.Drawing.Size(723, 46);
		this.layoutControlGroup5.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup5.Text = "Root";
		this.layoutControlGroup5.TextVisible = false;
		this.layoutControlItem23.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem23.Control = this.labelControl21;
		this.layoutControlItem23.CustomizationFormText = "layoutControlItem34";
		this.layoutControlItem23.EditControl = null;
		this.layoutControlItem23.EditValue = "layoutControlItem34";
		this.layoutControlItem23.FieldName = null;
		this.layoutControlItem23.IsSearchControl = false;
		this.layoutControlItem23.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem23.Name = "layoutControlItem23";
		this.layoutControlItem23.ReadOnly = false;
		this.layoutControlItem23.SettingInVisible = false;
		this.layoutControlItem23.SettingReadonly = false;
		this.layoutControlItem23.Size = new System.Drawing.Size(47, 21);
		this.layoutControlItem23.Text = "layoutControlItem34";
		this.layoutControlItem23.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem23.TextVisible = false;
		this.layoutControlItem24.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem24.Control = this.lblUserLogin;
		this.layoutControlItem24.CustomizationFormText = "layoutControlItem36";
		this.layoutControlItem24.EditControl = null;
		this.layoutControlItem24.EditValue = "layoutControlItem36";
		this.layoutControlItem24.FieldName = null;
		this.layoutControlItem24.IsSearchControl = false;
		this.layoutControlItem24.Location = new System.Drawing.Point(47, 0);
		this.layoutControlItem24.Name = "layoutControlItem24";
		this.layoutControlItem24.OptionsTableLayoutItem.ColumnIndex = 1;
		this.layoutControlItem24.ReadOnly = false;
		this.layoutControlItem24.SettingInVisible = false;
		this.layoutControlItem24.SettingReadonly = false;
		this.layoutControlItem24.Size = new System.Drawing.Size(78, 21);
		this.layoutControlItem24.Text = "layoutControlItem36";
		this.layoutControlItem24.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem24.TextVisible = false;
		this.layoutControlItem25.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem25.Control = this.labelControl24;
		this.layoutControlItem25.CustomizationFormText = "layoutControlItem37";
		this.layoutControlItem25.EditControl = null;
		this.layoutControlItem25.EditValue = "layoutControlItem37";
		this.layoutControlItem25.FieldName = null;
		this.layoutControlItem25.IsSearchControl = false;
		this.layoutControlItem25.Location = new System.Drawing.Point(578, 0);
		this.layoutControlItem25.Name = "layoutControlItem25";
		this.layoutControlItem25.OptionsTableLayoutItem.ColumnIndex = 2;
		this.layoutControlItem25.ReadOnly = false;
		this.layoutControlItem25.SettingInVisible = false;
		this.layoutControlItem25.SettingReadonly = false;
		this.layoutControlItem25.Size = new System.Drawing.Size(53, 21);
		this.layoutControlItem25.Text = "layoutControlItem37";
		this.layoutControlItem25.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem25.TextVisible = false;
		this.layoutControlItem26.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem26.Control = this.lblStoreId;
		this.layoutControlItem26.CustomizationFormText = "layoutControlItem38";
		this.layoutControlItem26.EditControl = null;
		this.layoutControlItem26.EditValue = "layoutControlItem38";
		this.layoutControlItem26.FieldName = null;
		this.layoutControlItem26.IsSearchControl = false;
		this.layoutControlItem26.Location = new System.Drawing.Point(631, 0);
		this.layoutControlItem26.Name = "layoutControlItem26";
		this.layoutControlItem26.OptionsTableLayoutItem.ColumnIndex = 3;
		this.layoutControlItem26.ReadOnly = false;
		this.layoutControlItem26.SettingInVisible = false;
		this.layoutControlItem26.SettingReadonly = false;
		this.layoutControlItem26.Size = new System.Drawing.Size(92, 21);
		this.layoutControlItem26.Text = "layoutControlItem38";
		this.layoutControlItem26.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem26.TextVisible = false;
		this.layoutControlItem29.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem29.Control = this.labelControl15;
		this.layoutControlItem29.CustomizationFormText = "layoutControlItem35";
		this.layoutControlItem29.EditControl = null;
		this.layoutControlItem29.EditValue = "layoutControlItem35";
		this.layoutControlItem29.FieldName = null;
		this.layoutControlItem29.IsSearchControl = false;
		this.layoutControlItem29.Location = new System.Drawing.Point(0, 21);
		this.layoutControlItem29.Name = "layoutControlItem29";
		this.layoutControlItem29.OptionsTableLayoutItem.RowIndex = 1;
		this.layoutControlItem29.ReadOnly = false;
		this.layoutControlItem29.SettingInVisible = false;
		this.layoutControlItem29.SettingReadonly = false;
		this.layoutControlItem29.Size = new System.Drawing.Size(47, 25);
		this.layoutControlItem29.Text = "layoutControlItem35";
		this.layoutControlItem29.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem29.TextVisible = false;
		this.layoutControlItem30.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem30.Control = this.lblDate;
		this.layoutControlItem30.CustomizationFormText = "layoutControlItem41";
		this.layoutControlItem30.EditControl = null;
		this.layoutControlItem30.EditValue = "layoutControlItem41";
		this.layoutControlItem30.FieldName = null;
		this.layoutControlItem30.IsSearchControl = false;
		this.layoutControlItem30.Location = new System.Drawing.Point(47, 21);
		this.layoutControlItem30.Name = "layoutControlItem30";
		this.layoutControlItem30.OptionsTableLayoutItem.ColumnIndex = 1;
		this.layoutControlItem30.OptionsTableLayoutItem.RowIndex = 1;
		this.layoutControlItem30.ReadOnly = false;
		this.layoutControlItem30.SettingInVisible = false;
		this.layoutControlItem30.SettingReadonly = false;
		this.layoutControlItem30.Size = new System.Drawing.Size(78, 25);
		this.layoutControlItem30.Text = "layoutControlItem41";
		this.layoutControlItem30.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem30.TextVisible = false;
		this.layoutControlItem31.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem31.Control = this.labelControl17;
		this.layoutControlItem31.CustomizationFormText = "layoutControlItem46";
		this.layoutControlItem31.EditControl = null;
		this.layoutControlItem31.EditValue = "layoutControlItem46";
		this.layoutControlItem31.FieldName = null;
		this.layoutControlItem31.IsSearchControl = false;
		this.layoutControlItem31.Location = new System.Drawing.Point(537, 21);
		this.layoutControlItem31.Name = "layoutControlItem31";
		this.layoutControlItem31.OptionsTableLayoutItem.ColumnIndex = 2;
		this.layoutControlItem31.OptionsTableLayoutItem.RowIndex = 1;
		this.layoutControlItem31.ReadOnly = false;
		this.layoutControlItem31.SettingInVisible = false;
		this.layoutControlItem31.SettingReadonly = false;
		this.layoutControlItem31.Size = new System.Drawing.Size(94, 25);
		this.layoutControlItem31.Text = "layoutControlItem46";
		this.layoutControlItem31.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem31.TextVisible = false;
		this.layoutControlItem32.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem32.Control = this.lblBussinessDate;
		this.layoutControlItem32.CustomizationFormText = "layoutControlItem42";
		this.layoutControlItem32.EditControl = null;
		this.layoutControlItem32.EditValue = "layoutControlItem42";
		this.layoutControlItem32.FieldName = null;
		this.layoutControlItem32.IsSearchControl = false;
		this.layoutControlItem32.Location = new System.Drawing.Point(631, 21);
		this.layoutControlItem32.Name = "layoutControlItem32";
		this.layoutControlItem32.OptionsTableLayoutItem.ColumnIndex = 3;
		this.layoutControlItem32.OptionsTableLayoutItem.RowIndex = 1;
		this.layoutControlItem32.ReadOnly = false;
		this.layoutControlItem32.SettingInVisible = false;
		this.layoutControlItem32.SettingReadonly = false;
		this.layoutControlItem32.Size = new System.Drawing.Size(92, 25);
		this.layoutControlItem32.Text = "layoutControlItem42";
		this.layoutControlItem32.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem32.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(125, 0);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(453, 21);
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem6.AllowHotTrack = false;
		this.emptySpaceItem6.Location = new System.Drawing.Point(125, 21);
		this.emptySpaceItem6.Name = "emptySpaceItem6";
		this.emptySpaceItem6.Size = new System.Drawing.Size(412, 25);
		this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
		this.pictureEdit1.Location = new System.Drawing.Point(732, 160);
		this.pictureEdit1.Name = "pictureEdit1";
		this.pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
		this.pictureEdit1.Size = new System.Drawing.Size(183, 210);
		this.pictureEdit1.StyleController = this.layoutControl1;
		this.pictureEdit1.TabIndex = 9;
		this.txtBarCode.EditControl = null;
		this.txtBarCode.FieldName = null;
		this.txtBarCode.IsSearchControl = false;
		this.txtBarCode.Location = new System.Drawing.Point(54, 3);
		this.txtBarCode.Name = "txtBarCode";
		this.txtBarCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25f);
		this.txtBarCode.Properties.Appearance.Options.UseFont = true;
		this.txtBarCode.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
		this.txtBarCode.SettingInVisible = false;
		this.txtBarCode.SettingReadonly = false;
		this.txtBarCode.Size = new System.Drawing.Size(674, 30);
		this.txtBarCode.StyleController = this.layoutControl1;
		this.txtBarCode.TabIndex = 5;
		this.txtBarCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBarCode_KeyDown);
		this.tabPane1.AllowGlyphSkinning = true;
		this.tabPane1.AllowTransitionAnimation = DevExpress.Utils.DefaultBoolean.False;
		this.tabPane1.AppearanceButton.Hovered.BackColor = System.Drawing.Color.FromArgb(0, 192, 0);
		this.tabPane1.AppearanceButton.Hovered.Font = new System.Drawing.Font("Tahoma", 14.25f);
		this.tabPane1.AppearanceButton.Hovered.Options.UseBackColor = true;
		this.tabPane1.AppearanceButton.Hovered.Options.UseFont = true;
		this.tabPane1.AppearanceButton.Normal.Font = new System.Drawing.Font("Tahoma", 14.25f);
		this.tabPane1.AppearanceButton.Normal.Options.UseFont = true;
		this.tabPane1.AppearanceButton.Pressed.ForeColor = System.Drawing.Color.White;
		this.tabPane1.AppearanceButton.Pressed.Options.UseForeColor = true;
		this.tabPane1.Controls.Add(this.MainPage);
		this.tabPane1.Controls.Add(this.DiscountPage);
		this.tabPane1.DisableLiveResize = true;
		this.tabPane1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
		this.tabPane1.Location = new System.Drawing.Point(732, 384);
		this.tabPane1.Margin = new System.Windows.Forms.Padding(0);
		this.tabPane1.Name = "tabPane1";
		this.tabPane1.PageProperties.ShowMode = DevExpress.XtraBars.Navigation.ItemShowMode.ImageAndText;
		this.tabPane1.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[2] { this.MainPage, this.DiscountPage });
		this.tabPane1.RegularSize = new System.Drawing.Size(551, 323);
		this.tabPane1.SelectedPage = this.MainPage;
		this.tabPane1.Size = new System.Drawing.Size(551, 323);
		this.tabPane1.TabIndex = 4;
		this.tabPane1.Text = "Main";
		this.MainPage.BackgroundPadding = new System.Windows.Forms.Padding(0);
		this.MainPage.Caption = "Main";
		this.MainPage.Controls.Add(this.tileControl1);
		this.MainPage.Margin = new System.Windows.Forms.Padding(0);
		this.MainPage.Name = "MainPage";
		this.MainPage.Size = new System.Drawing.Size(551, 293);
		this.tileControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tileControl1.Groups.Add(this.tileGroup4);
		this.tileControl1.LayoutMode = DevExpress.XtraEditors.TileControlLayoutMode.Adaptive;
		this.tileControl1.Location = new System.Drawing.Point(0, 0);
		this.tileControl1.MaxId = 22;
		this.tileControl1.Name = "tileControl1";
		this.tileControl1.Size = new System.Drawing.Size(551, 293);
		this.tileControl1.TabIndex = 0;
		this.tileControl1.Text = "tileControl1";
		this.tileGroup4.Items.Add(this.tileItem3);
		this.tileGroup4.Items.Add(this.tileItem4);
		this.tileGroup4.Items.Add(this.tileItem5);
		this.tileGroup4.Items.Add(this.tileItem6);
		this.tileGroup4.Items.Add(this.tileItem7);
		this.tileGroup4.Items.Add(this.tileItem8);
		this.tileGroup4.Items.Add(this.tileItem15);
		this.tileGroup4.Items.Add(this.tileItem16);
		this.tileGroup4.Name = "tileGroup4";
		tileItemElement.Text = "Item Search";
		this.tileItem3.Elements.Add(tileItemElement);
		this.tileItem3.Id = 11;
		this.tileItem3.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem3.Name = "tileItem3";
		tileItemElement2.Text = "Edit Item";
		this.tileItem4.Elements.Add(tileItemElement2);
		this.tileItem4.Id = 12;
		this.tileItem4.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem4.Name = "tileItem4";
		tileItemElement3.Text = "Customer Search";
		this.tileItem5.Elements.Add(tileItemElement3);
		this.tileItem5.Id = 13;
		this.tileItem5.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem5.Name = "tileItem5";
		tileItemElement4.Text = "tileItem6";
		this.tileItem6.Elements.Add(tileItemElement4);
		this.tileItem6.Id = 14;
		this.tileItem6.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem6.Name = "tileItem6";
		tileItemElement5.Text = "Quantity";
		this.tileItem7.Elements.Add(tileItemElement5);
		this.tileItem7.Id = 15;
		this.tileItem7.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem7.Name = "tileItem7";
		tileItemElement6.Text = "tileItem8";
		this.tileItem8.Elements.Add(tileItemElement6);
		this.tileItem8.Id = 16;
		this.tileItem8.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem8.Name = "tileItem8";
		tileItemElement7.Text = "tileItem15";
		this.tileItem15.Elements.Add(tileItemElement7);
		this.tileItem15.Id = 20;
		this.tileItem15.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
		this.tileItem15.Name = "tileItem15";
		tileItemElement8.Text = "tileItem16";
		this.tileItem16.Elements.Add(tileItemElement8);
		this.tileItem16.Id = 21;
		this.tileItem16.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
		this.tileItem16.Name = "tileItem16";
		this.DiscountPage.BackgroundPadding = new System.Windows.Forms.Padding(0);
		this.DiscountPage.Caption = "Discount&Returns";
		this.DiscountPage.Controls.Add(this.tileControl2);
		this.DiscountPage.Margin = new System.Windows.Forms.Padding(0);
		this.DiscountPage.Name = "DiscountPage";
		this.DiscountPage.Size = new System.Drawing.Size(549, 324);
		this.tileControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tileControl2.Groups.Add(this.tileGroup5);
		this.tileControl2.LayoutMode = DevExpress.XtraEditors.TileControlLayoutMode.Adaptive;
		this.tileControl2.Location = new System.Drawing.Point(0, 0);
		this.tileControl2.Margin = new System.Windows.Forms.Padding(1);
		this.tileControl2.MaxId = 8;
		this.tileControl2.Name = "tileControl2";
		this.tileControl2.Size = new System.Drawing.Size(549, 324);
		this.tileControl2.TabIndex = 0;
		this.tileControl2.Text = "tileControl2";
		this.tileGroup5.Items.Add(this.tileItem9);
		this.tileGroup5.Items.Add(this.tileItem10);
		this.tileGroup5.Items.Add(this.tileItem11);
		this.tileGroup5.Items.Add(this.tileItem12);
		this.tileGroup5.Items.Add(this.tileItem13);
		this.tileGroup5.Items.Add(this.tileItem14);
		this.tileGroup5.Items.Add(this.tileItem17);
		this.tileGroup5.Items.Add(this.tileItem18);
		this.tileGroup5.Name = "tileGroup5";
		tileItemElement9.Text = "Discount";
		this.tileItem9.Elements.Add(tileItemElement9);
		this.tileItem9.Id = 0;
		this.tileItem9.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem9.Name = "tileItem9";
		tileItemElement10.Text = "tileItem10";
		this.tileItem10.Elements.Add(tileItemElement10);
		this.tileItem10.Id = 1;
		this.tileItem10.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem10.Name = "tileItem10";
		tileItemElement11.Text = "tileItem11";
		this.tileItem11.Elements.Add(tileItemElement11);
		this.tileItem11.Id = 2;
		this.tileItem11.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem11.Name = "tileItem11";
		tileItemElement12.Text = "tileItem12";
		this.tileItem12.Elements.Add(tileItemElement12);
		this.tileItem12.Id = 3;
		this.tileItem12.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem12.Name = "tileItem12";
		tileItemElement13.Text = "tileItem13";
		this.tileItem13.Elements.Add(tileItemElement13);
		this.tileItem13.Id = 4;
		this.tileItem13.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem13.Name = "tileItem13";
		tileItemElement14.Text = "tileItem14";
		this.tileItem14.Elements.Add(tileItemElement14);
		this.tileItem14.Id = 5;
		this.tileItem14.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem14.Name = "tileItem14";
		tileItemElement15.Text = "tileItem17";
		this.tileItem17.Elements.Add(tileItemElement15);
		this.tileItem17.Id = 6;
		this.tileItem17.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
		this.tileItem17.Name = "tileItem17";
		tileItemElement16.Text = "tileItem18";
		this.tileItem18.Elements.Add(tileItemElement16);
		this.tileItem18.Id = 7;
		this.tileItem18.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
		this.tileItem18.Name = "tileItem18";
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[13]
		{
			this.layoutControlItem1, this.layoutControlItem2, this.layoutControlItem4, this.layoutControlItem22, this.emptySpaceItem2, this.layoutControlItem27, this.emptySpaceItem9, this.layoutControlItem3, this.layoutControlItem6, this.emptySpaceItem1,
			this.emptySpaceItem4, this.emptySpaceItem5, this.layoutControlItem5
		});
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(1, 1, 1, 1);
		this.layoutControlGroup1.Size = new System.Drawing.Size(1286, 710);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem1.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem1.Control = this.tabPane1;
		this.layoutControlItem1.EditControl = null;
		this.layoutControlItem1.EditValue = "layoutControlItem1";
		this.layoutControlItem1.FieldName = null;
		this.layoutControlItem1.IsSearchControl = false;
		this.layoutControlItem1.Location = new System.Drawing.Point(729, 381);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.ReadOnly = false;
		this.layoutControlItem1.SettingInVisible = false;
		this.layoutControlItem1.SettingReadonly = false;
		this.layoutControlItem1.Size = new System.Drawing.Size(555, 327);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem2.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem2.Control = this.txtBarCode;
		this.layoutControlItem2.EditControl = null;
		this.layoutControlItem2.EditValue = "BARCODE";
		this.layoutControlItem2.FieldName = null;
		this.layoutControlItem2.IsSearchControl = false;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.ReadOnly = false;
		this.layoutControlItem2.SettingInVisible = false;
		this.layoutControlItem2.SettingReadonly = false;
		this.layoutControlItem2.Size = new System.Drawing.Size(729, 34);
		this.layoutControlItem2.Text = "BARCODE";
		this.layoutControlItem2.TextSize = new System.Drawing.Size(48, 13);
		this.layoutControlItem4.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem4.Control = this.pictureEdit1;
		this.layoutControlItem4.EditControl = null;
		this.layoutControlItem4.EditValue = "layoutControlItem4";
		this.layoutControlItem4.FieldName = null;
		this.layoutControlItem4.IsSearchControl = false;
		this.layoutControlItem4.Location = new System.Drawing.Point(729, 157);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.ReadOnly = false;
		this.layoutControlItem4.SettingInVisible = false;
		this.layoutControlItem4.SettingReadonly = false;
		this.layoutControlItem4.Size = new System.Drawing.Size(187, 214);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.layoutControlItem22.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem22.Control = this.layoutControl4;
		this.layoutControlItem22.EditControl = null;
		this.layoutControlItem22.EditValue = "layoutControlItem22";
		this.layoutControlItem22.FieldName = null;
		this.layoutControlItem22.IsSearchControl = false;
		this.layoutControlItem22.Location = new System.Drawing.Point(0, 656);
		this.layoutControlItem22.Name = "layoutControlItem22";
		this.layoutControlItem22.ReadOnly = false;
		this.layoutControlItem22.SettingInVisible = false;
		this.layoutControlItem22.SettingReadonly = false;
		this.layoutControlItem22.Size = new System.Drawing.Size(729, 52);
		this.layoutControlItem22.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem22.TextVisible = false;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(0, 579);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(729, 10);
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem27.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem27.Control = this.grdDoc;
		this.layoutControlItem27.EditControl = null;
		this.layoutControlItem27.EditValue = "layoutControlItem27";
		this.layoutControlItem27.FieldName = null;
		this.layoutControlItem27.IsSearchControl = false;
		this.layoutControlItem27.Location = new System.Drawing.Point(0, 34);
		this.layoutControlItem27.Name = "layoutControlItem27";
		this.layoutControlItem27.ReadOnly = false;
		this.layoutControlItem27.SettingInVisible = false;
		this.layoutControlItem27.SettingReadonly = false;
		this.layoutControlItem27.Size = new System.Drawing.Size(729, 545);
		this.layoutControlItem27.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem27.TextVisible = false;
		this.emptySpaceItem9.AllowHotTrack = false;
		this.emptySpaceItem9.Location = new System.Drawing.Point(0, 646);
		this.emptySpaceItem9.Name = "emptySpaceItem9";
		this.emptySpaceItem9.Size = new System.Drawing.Size(729, 10);
		this.emptySpaceItem9.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem3.Control = this.tableLayoutPanel1;
		this.layoutControlItem3.EditControl = null;
		this.layoutControlItem3.EditValue = "layoutControlItem3";
		this.layoutControlItem3.FieldName = null;
		this.layoutControlItem3.IsSearchControl = false;
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 589);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.ReadOnly = false;
		this.layoutControlItem3.SettingInVisible = false;
		this.layoutControlItem3.SettingReadonly = false;
		this.layoutControlItem3.Size = new System.Drawing.Size(729, 57);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.layoutControlItem6.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem6.Control = this.tableLayoutPanel3;
		this.layoutControlItem6.EditControl = null;
		this.layoutControlItem6.EditValue = "layoutControlItem6";
		this.layoutControlItem6.FieldName = null;
		this.layoutControlItem6.IsSearchControl = false;
		this.layoutControlItem6.Location = new System.Drawing.Point(916, 41);
		this.layoutControlItem6.Name = "layoutControlItem6";
		this.layoutControlItem6.ReadOnly = false;
		this.layoutControlItem6.SettingInVisible = false;
		this.layoutControlItem6.SettingReadonly = false;
		this.layoutControlItem6.Size = new System.Drawing.Size(368, 330);
		this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(729, 371);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(555, 10);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(729, 0);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(187, 157);
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem5.AllowHotTrack = false;
		this.emptySpaceItem5.Location = new System.Drawing.Point(916, 0);
		this.emptySpaceItem5.Name = "emptySpaceItem5";
		this.emptySpaceItem5.Size = new System.Drawing.Size(368, 10);
		this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem5.Control = this.tableLayoutPanel2;
		this.layoutControlItem5.EditControl = null;
		this.layoutControlItem5.EditValue = "layoutControlItem5";
		this.layoutControlItem5.FieldName = null;
		this.layoutControlItem5.IsSearchControl = false;
		this.layoutControlItem5.Location = new System.Drawing.Point(916, 10);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.ReadOnly = false;
		this.layoutControlItem5.SettingInVisible = false;
		this.layoutControlItem5.SettingReadonly = false;
		this.layoutControlItem5.Size = new System.Drawing.Size(368, 31);
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.tileGroup3.Name = "tileGroup3";
		tileItemElement17.Text = "tileItem2";
		this.tileItem2.Elements.Add(tileItemElement17);
		this.tileItem2.Id = 1;
		this.tileItem2.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
		this.tileItem2.Name = "tileItem2";
		tileItemElement18.Text = "tileItem1";
		this.tileItem1.Elements.Add(tileItemElement18);
		this.tileItem1.Id = 0;
		this.tileItem1.ItemSize = DevExpress.XtraEditors.TileItemSize.Wide;
		this.tileItem1.Name = "tileItem1";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1286, 710);
		base.Controls.Add(this.layoutControl1);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_POS";
		this.Text = "frm_POS";
		base.Load += new System.EventHandler(this.frm_POS_Load);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		this.tableLayoutPanel2.ResumeLayout(false);
		this.tableLayoutPanel2.PerformLayout();
		this.tableLayoutPanel3.ResumeLayout(false);
		this.tableLayoutPanel3.PerformLayout();
		this.tableLayoutPanel1.ResumeLayout(false);
		this.tableLayoutPanel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl4).EndInit();
		this.layoutControl4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem23).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem24).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem25).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem26).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem29).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem30).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem31).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem32).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pictureEdit1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBarCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tabPane1).EndInit();
		this.tabPane1.ResumeLayout(false);
		this.MainPage.ResumeLayout(false);
		this.DiscountPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem22).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem27).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		base.ResumeLayout(false);
	}
}
