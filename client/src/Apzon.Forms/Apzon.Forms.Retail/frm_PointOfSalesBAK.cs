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

namespace Apzon.Forms.Retail;

public class frm_PointOfSalesBAK : XtraFormEx
{
	private const string FormController = "Retail";

	private double _invoiceTotal;

	private double _invoiceTax;

	private double _subTotal;

	private double _invoiceDiscount;

	private bool _cellediting;

	private IContainer components;

	private NavigationFrame nframe_Main;

	private NavigationPage npage_Main;

	private NavigationPage navigationPage1;

	private LayoutControl layoutControl1;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControl layoutControl2;

	private NavigationFrame nframe_Menu;

	private NavigationPage navigationPage2;

	private NavigationPage npage_Discount;

	private TileBar tileBar2;

	private TileBarGroup tileBarGroup3;

	private TileBarItem tileBarItem5;

	private TileBarItem tileBarItem6;

	private TileBarItem tileBarItem7;

	private TileBar tileBar1;

	private ButtonEditEx btnBarCode;

	private GridControlEx grd_Contents;

	private GridViewEx gv_Contents;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private LayoutControlItemEx layoutControlItem2;

	private LayoutControlItemEx layoutControlItem3;

	private TableLayoutPanel tableLayoutPanel1;

	private LayoutControlItemEx layoutControlItem1;

	private Panel panel4;

	private Panel panel3;

	private Panel panel2;

	private Panel panel1;

	private LabelControl labelControl1;

	private LabelControl labelControl4;

	private LabelControl labelControl3;

	private LabelControl labelControl5;

	private CalcEditEx lblSubTotal;

	private CalcEditEx lblInvoiceTotal;

	private CalcEditEx lblTaxAmount;

	private CalcEditEx lblDiscountAmount;

	private TileBar tileBar3;

	private TileBarGroup tileBarGroup2;

	private TileBarItem btn_Cash;

	private TileBarItem tileBarItem2;

	private TileBarItem tileBarItem3;

	private TileBarItem tileBarItem4;

	private TileBarItem tileBarItem8;

	private LayoutControlItemEx layoutControlItem4;

	private TableLayoutPanel tableLayoutPanel2;

	private LayoutControlItemEx layoutControlItem5;

	private EmptySpaceItem emptySpaceItem3;

	private LabelControl labelControl2;

	private LayoutControlItemEx layoutControlItem6;

	private Panel panel10;

	private Panel panel9;

	private Panel panel8;

	private Panel panel7;

	private Panel panel6;

	private Panel panel5;

	private LabelControl labelControl11;

	private LabelControl labelControl10;

	private LabelControl labelControl9;

	private LabelControl labelControl8;

	private LabelControl labelControl7;

	private LabelControl labelControl6;

	private PictureEdit pictureEdit1;

	private LayoutControlItemEx layoutControlItem7;

	private TileBar tileBar4;

	private TileBarGroup tileBarGroup1;

	private TileBarItem tbi_Main;

	private TileBarItem tbi_DiscountReturnMenu;

	private TileBarItem tbi_MenuOperation;

	private LayoutControlItemEx layoutControlItem8;

	private NavigationFrame nframe_MainMenu;

	private NavigationPage npage_MenuMain;

	private NavigationPage npage_DiscountReturnMenu;

	private LayoutControlItemEx layoutControlItem9;

	private TileControl tileControl1;

	private TileGroup tileGroup2;

	private TileItem tileItem_CustomerSearch;

	private TileItem tileItem2;

	private TileItem tileItem3;

	private TileItem tileItem_ItemSearch;

	private TileItem tileItem5;

	private TileItem tileItem6;

	private TileItem tileItem7;

	private TileItem tileItem_VoidItem;

	private TileItem tileItem11;

	private TileBarItem tbi_MenuMiscellaneous;

	private TileBarItem tileBarItem12;

	private TileControl tileControl2;

	private TileGroup tileGroup1;

	private TileItem tileItem9;

	private TileItem tileItem10;

	private TileItem tileItem12;

	private TileItem tileItem13;

	private TileItem tileItem14;

	private TileItem tileItem15;

	private TileItem tileItem16;

	private TileItem tileItem17;

	private NavigationPage npage_MenuMiscellaneous;

	private TileControl tileControl3;

	private TileGroup tileGroup3;

	private TileItem tileItem1;

	private TileItem tileItem18;

	private TileItem tileItem20;

	private TileItem tileItem21;

	private TileItem tileItem22;

	private TileItem tileItem23;

	private TileItem tileItem24;

	private TileItem tileItem25;

	private TileItem tileItem26;

	private TileItem tileItem19;

	private NavigationPage npage_MenuOperation;

	private TileControl tileControl4;

	private TileGroup tileGroup4;

	private TileItem tileItem27;

	private TileItem tileItem28;

	private TileItem tileItem29;

	private TileItem tileItem30;

	private TileItem tileItem31;

	private TileItem tileItem32;

	private TileItem tileItem33;

	private TileItem tileItem34;

	private TileItem tileItem35;

	private Panel panel11;

	private LayoutControlItemEx layoutControlItem10;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	private CalcEditEx lblBalance;

	private CalcEditEx lblCardCode;

	private CalcEditEx lblEmail;

	private CalcEditEx lblTel;

	private CalcEditEx lblAddress;

	private CalcEditEx lblCardName;

	public Size OldSize { get; set; }

	public frm_PointOfSalesBAK()
	{
		InitializeComponent();
	}

	private void Main_Load(object sender, EventArgs e)
	{
	}

	private void tileItem_CustomerSearch_Click(object sender, TileItemEventArgs e)
	{
		try
		{
			nframe_Main.SelectedPage = navigationPage1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void tileItem_ItemSearch_ItemClick(object sender, TileItemEventArgs e)
	{
		try
		{
			nframe_Main.SelectedPage = navigationPage1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void tileItem_VoidItem_ItemClick(object sender, TileItemEventArgs e)
	{
		try
		{
			if ((grd_Contents.DataSource as DataTable).IsNotNull() && XtraMessageBoxEx.Show(Enums.MessageCode.PointOfSalesConfirmDeleteItem, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
			{
				gv_Contents.DeleteSelectedRows();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void tbi_Main_ItemClick(object sender, TileItemEventArgs e)
	{
		nframe_MainMenu.SelectedPage = npage_MenuMain;
	}

	private void tbi_DiscountReturnMenu_ItemClick(object sender, TileItemEventArgs e)
	{
		nframe_MainMenu.SelectedPage = npage_DiscountReturnMenu;
	}

	private void tbi_MenuMiscellaneous_ItemClick(object sender, TileItemEventArgs e)
	{
		nframe_MainMenu.SelectedPage = npage_MenuMiscellaneous;
	}

	private void tbi_MenuOperation_ItemClick(object sender, TileItemEventArgs e)
	{
		nframe_MainMenu.SelectedPage = npage_MenuOperation;
	}

	private void buttonEdit1_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return && !string.IsNullOrEmpty(btnBarCode.Text))
			{
				string itemCode = Function.ToString(btnBarCode.Text);
				LoadData(itemCode);
				btnBarCode.Text = string.Empty;
				btnBarCode.Enabled = true;
				btnBarCode.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Contents_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			Function.ToString(gv_Contents.GetFocusedRowCellValue("ItemCode"));
			Function.ToString(gv_Contents.GetFocusedRowCellValue("ItemName"));
			double num = Function.ParseDouble(gv_Contents.GetFocusedRowCellValue("Price"));
			double num2 = Function.ParseDouble(gv_Contents.GetFocusedRowCellValue("TaxRate"));
			double num3 = Function.ParseDouble(gv_Contents.GetFocusedRowCellValue("Discount"));
			int num4 = Function.ParseInt(gv_Contents.GetFocusedRowCellValue("Qty"));
			Function.ParseDouble(gv_Contents.GetFocusedRowCellValue("TaxAmount"));
			Function.ParseDouble(gv_Contents.GetFocusedRowCellValue("LineTotal"));
			DataTable dataTable = grd_Contents.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			if (!_cellediting && (e.Column.FieldName == "Qty" || e.Column.FieldName == "TaxRate" || e.Column.FieldName == "Discount" || e.Column.FieldName == "Price"))
			{
				_invoiceTotal = 0.0;
				_invoiceTax = 0.0;
				_subTotal = 0.0;
				_invoiceDiscount = 0.0;
				_cellediting = true;
				gv_Contents.SetFocusedRowCellValue("LineTotal", (num - num * num3 / 100.0) * (double)num4);
				gv_Contents.SetFocusedRowCellValue("TaxAmount", (num - num * num3 / 100.0) * num2 / 100.0 * (double)num4);
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					_invoiceTax += Function.ParseDouble(dataTable.Rows[i]["TaxAmount"]);
					_invoiceDiscount += Function.ParseDouble(dataTable.Rows[i]["Price"]) * Function.ParseDouble(dataTable.Rows[i]["Discount"]) / 100.0;
					_subTotal += Function.ParseDouble(dataTable.Rows[i]["LineTotal"]);
				}
				_invoiceTotal = _subTotal - _invoiceDiscount + _invoiceTax;
				_cellediting = false;
			}
			lblSubTotal.EditValue = Function.ParseDecimal(_subTotal);
			lblInvoiceTotal.EditValue = Function.ParseDecimal(_invoiceTotal);
			lblTaxAmount.EditValue = Function.ParseDecimal(_invoiceTax);
			lblDiscountAmount.EditValue = Function.ParseDecimal(_invoiceDiscount);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cash_ItemClick(object sender, TileItemEventArgs e)
	{
		try
		{
			nframe_Main.SelectedPage = navigationPage1;
			new DataTable
			{
				Columns = 
				{
					{
						"Total",
						typeof(double)
					},
					{
						"AmountPaid",
						typeof(double)
					},
					{
						"BalanceDue",
						typeof(double)
					},
					{
						"Surcharge",
						typeof(double)
					},
					{
						"PaymentDiscount",
						typeof(double)
					}
				},
				Rows = { new object[5]
				{
					_invoiceTotal,
					0.0,
					0.0 - _invoiceTotal,
					0.0,
					0.0
				} }
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tileBarItem2_ItemClick(object sender, TileItemEventArgs e)
	{
	}

	private void tileBarItem3_ItemClick(object sender, TileItemEventArgs e)
	{
	}

	private void tileBarItem4_ItemClick(object sender, TileItemEventArgs e)
	{
	}

	private void tileBarItem8_ItemClick(object sender, TileItemEventArgs e)
	{
	}

	private async void btnBarCode_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "RetailStore");
			DataTable dataTable = await helper.GetAsync("LoadDoc");
			if (!dataTable.IsNotNull() || !Function.ToString(dataTable.Rows[0]["OSKeyBoardEnable"]).Equals("Y"))
			{
				return;
			}
			WinCommons._OpenProcessing();
			frm_OSKeyboard frm_OSKeyboard2 = new frm_OSKeyboard();
			frm_OSKeyboard2.TopMost = true;
			frm_OSKeyboard2.Show();
			btnBarCode.Focus();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadData(string itemCode)
	{
		try
		{
			int num = 0;
			_invoiceTotal = 0.0;
			_invoiceTax = 0.0;
			_subTotal = 0.0;
			DataTable dataTable = grd_Contents.DataSource as DataTable;
			DataTable dataTable2;
			using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Retail"))
			{
				dataTable2 = httpClientHelper.Get("GetDocByCode", new
				{
					barCode = itemCode,
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
						dataTable.Rows[i]["LineTotal"] = Function.ParseDouble(dataTable.Rows[i]["LineTotal"]) + num2 - num2 * num4 / 100.0;
						dataTable.Rows[i]["TaxAmount"] = Function.ParseDouble(dataTable.Rows[i]["TaxAmount"]) + (num2 - num2 * num4 / 100.0) * num3 / 100.0;
						num++;
						break;
					}
				}
				if (num > 0)
				{
					grd_Contents.DataSource = dataTable;
					grd_Contents.RefreshDataSource();
					lblInvoiceTotal.Text = Function.ToString(_invoiceTotal);
					lblTaxAmount.Text = Function.ToString(_invoiceTax);
					lblSubTotal.Text = Function.ToString(_subTotal);
				}
				else
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["TranType"] = dataTable2.Rows[0]["TranType"];
					dataRow["ItemCode"] = dataTable2.Rows[0]["ItemCode"];
					dataRow["ItemName"] = dataTable2.Rows[0]["ItemName"];
					dataRow["Qty"] = dataTable2.Rows[0]["Qty"];
					dataRow["Price"] = dataTable2.Rows[0]["Price"];
					dataRow["Discount"] = dataTable2.Rows[0]["Discount"];
					dataRow["TaxRate"] = dataTable2.Rows[0]["TaxRate"];
					dataRow["TaxAmount"] = dataTable2.Rows[0]["TaxAmount"];
					dataRow["LineTotal"] = dataTable2.Rows[0]["LineTotal"];
					dataTable.Rows.Add(dataRow);
					grd_Contents.DataSource = dataTable;
					grd_Contents.RefreshDataSource();
				}
				for (int j = 0; j < dataTable.Rows.Count; j++)
				{
					_invoiceTax += Function.ParseDouble(dataTable.Rows[j]["TaxAmount"]);
					_invoiceDiscount += Function.ParseDouble(dataTable.Rows[j]["Price"]) * Function.ParseDouble(dataTable.Rows[j]["Discount"]) / 100.0;
					_subTotal += Function.ParseDouble(dataTable.Rows[j]["LineTotal"]);
				}
				_invoiceTotal = _subTotal - _invoiceDiscount + _invoiceTax;
			}
			else if (dataTable2.IsNotNull() && !dataTable.IsNotNull())
			{
				for (int k = 0; k < dataTable2.Rows.Count; k++)
				{
					_invoiceTax += Function.ParseDouble(dataTable2.Rows[k]["TaxAmount"]);
					_invoiceDiscount += Function.ParseDouble(dataTable2.Rows[k]["Price"]) * Function.ParseDouble(dataTable2.Rows[k]["Discount"]) / 100.0;
					_subTotal += Function.ParseDouble(dataTable2.Rows[k]["LineTotal"]);
				}
				_invoiceTotal = _subTotal - _invoiceDiscount + _invoiceTax;
				grd_Contents.DataSource = dataTable2;
				grd_Contents.RefreshDataSource();
			}
			lblSubTotal.EditValue = Function.ParseDecimal(_subTotal);
			lblInvoiceTotal.EditValue = Function.ParseDecimal(_invoiceTotal);
			lblTaxAmount.EditValue = Function.ParseDecimal(_invoiceTax);
			lblDiscountAmount.EditValue = Function.ParseDecimal(_invoiceDiscount);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void AddItemToGrid(DataRow row)
	{
		try
		{
			string itemCode = Function.ToString(row["ItemCode"]);
			LoadData(itemCode);
			nframe_Main.SelectedPage = npage_Main;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void AddCustomerToTable(DataRow row)
	{
		try
		{
			string text = Function.ToString(row["CardCode"]);
			string text2 = Function.ToString(row["CardName"]);
			string text3 = Function.ToString(row["Address"]);
			string text4 = Function.ToString(row["Cellular"]);
			string text5 = Function.ToString(row["MailAddress"]);
			double num = Function.ParseDouble(row["Balance"]);
			lblCardCode.Text = text;
			lblCardName.Text = text2;
			lblAddress.Text = text3;
			lblTel.Text = text4;
			lblEmail.Text = text5;
			lblBalance.Value = Function.ParseDecimal(num);
			nframe_Main.SelectedPage = npage_Main;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void BackToMain()
	{
		try
		{
			nframe_Main.SelectedPage = npage_Main;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
		DevExpress.XtraEditors.TileItemElement tileItemElement19 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement20 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement21 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement22 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement23 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement24 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement25 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement26 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement27 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement28 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement29 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement30 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement31 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement32 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement33 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement34 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement35 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement36 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement37 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement38 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement39 = new DevExpress.XtraEditors.TileItemElement();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Retail.frm_PointOfSalesBAK));
		DevExpress.XtraEditors.TileItemElement tileItemElement40 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement41 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement42 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement43 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement44 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.TileItemElement tileItemElement45 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement46 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement47 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement48 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement49 = new DevExpress.XtraEditors.TileItemElement();
		this.nframe_Main = new DevExpress.XtraBars.Navigation.NavigationFrame();
		this.npage_Main = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
		this.panel11 = new System.Windows.Forms.Panel();
		this.nframe_MainMenu = new DevExpress.XtraBars.Navigation.NavigationFrame();
		this.npage_MenuMain = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.tileControl1 = new DevExpress.XtraEditors.TileControl();
		this.tileGroup2 = new DevExpress.XtraEditors.TileGroup();
		this.tileItem_ItemSearch = new DevExpress.XtraEditors.TileItem();
		this.tileItem2 = new DevExpress.XtraEditors.TileItem();
		this.tileItem_CustomerSearch = new DevExpress.XtraEditors.TileItem();
		this.tileItem3 = new DevExpress.XtraEditors.TileItem();
		this.tileItem5 = new DevExpress.XtraEditors.TileItem();
		this.tileItem6 = new DevExpress.XtraEditors.TileItem();
		this.tileItem7 = new DevExpress.XtraEditors.TileItem();
		this.tileItem_VoidItem = new DevExpress.XtraEditors.TileItem();
		this.tileItem11 = new DevExpress.XtraEditors.TileItem();
		this.npage_DiscountReturnMenu = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.tileControl2 = new DevExpress.XtraEditors.TileControl();
		this.tileGroup1 = new DevExpress.XtraEditors.TileGroup();
		this.tileItem9 = new DevExpress.XtraEditors.TileItem();
		this.tileItem10 = new DevExpress.XtraEditors.TileItem();
		this.tileItem12 = new DevExpress.XtraEditors.TileItem();
		this.tileItem13 = new DevExpress.XtraEditors.TileItem();
		this.tileItem14 = new DevExpress.XtraEditors.TileItem();
		this.tileItem15 = new DevExpress.XtraEditors.TileItem();
		this.tileItem16 = new DevExpress.XtraEditors.TileItem();
		this.tileItem17 = new DevExpress.XtraEditors.TileItem();
		this.npage_MenuMiscellaneous = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.tileControl3 = new DevExpress.XtraEditors.TileControl();
		this.tileGroup3 = new DevExpress.XtraEditors.TileGroup();
		this.tileItem1 = new DevExpress.XtraEditors.TileItem();
		this.tileItem18 = new DevExpress.XtraEditors.TileItem();
		this.tileItem20 = new DevExpress.XtraEditors.TileItem();
		this.tileItem21 = new DevExpress.XtraEditors.TileItem();
		this.tileItem22 = new DevExpress.XtraEditors.TileItem();
		this.tileItem23 = new DevExpress.XtraEditors.TileItem();
		this.tileItem24 = new DevExpress.XtraEditors.TileItem();
		this.tileItem25 = new DevExpress.XtraEditors.TileItem();
		this.tileItem26 = new DevExpress.XtraEditors.TileItem();
		this.npage_MenuOperation = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.tileControl4 = new DevExpress.XtraEditors.TileControl();
		this.tileGroup4 = new DevExpress.XtraEditors.TileGroup();
		this.tileItem27 = new DevExpress.XtraEditors.TileItem();
		this.tileItem28 = new DevExpress.XtraEditors.TileItem();
		this.tileItem29 = new DevExpress.XtraEditors.TileItem();
		this.tileItem30 = new DevExpress.XtraEditors.TileItem();
		this.tileItem31 = new DevExpress.XtraEditors.TileItem();
		this.tileItem32 = new DevExpress.XtraEditors.TileItem();
		this.tileItem33 = new DevExpress.XtraEditors.TileItem();
		this.tileItem34 = new DevExpress.XtraEditors.TileItem();
		this.tileItem35 = new DevExpress.XtraEditors.TileItem();
		this.tileBar4 = new DevExpress.XtraBars.Navigation.TileBar();
		this.tileBarGroup1 = new DevExpress.XtraBars.Navigation.TileBarGroup();
		this.tbi_Main = new DevExpress.XtraBars.Navigation.TileBarItem();
		this.tbi_DiscountReturnMenu = new DevExpress.XtraBars.Navigation.TileBarItem();
		this.tbi_MenuMiscellaneous = new DevExpress.XtraBars.Navigation.TileBarItem();
		this.tbi_MenuOperation = new DevExpress.XtraBars.Navigation.TileBarItem();
		this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
		this.panel10 = new System.Windows.Forms.Panel();
		this.lblEmail = new Apzon.Controls.Editor.CalcEditEx();
		this.labelControl11 = new DevExpress.XtraEditors.LabelControl();
		this.panel9 = new System.Windows.Forms.Panel();
		this.lblTel = new Apzon.Controls.Editor.CalcEditEx();
		this.labelControl10 = new DevExpress.XtraEditors.LabelControl();
		this.panel8 = new System.Windows.Forms.Panel();
		this.lblBalance = new Apzon.Controls.Editor.CalcEditEx();
		this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
		this.panel7 = new System.Windows.Forms.Panel();
		this.lblAddress = new Apzon.Controls.Editor.CalcEditEx();
		this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
		this.panel6 = new System.Windows.Forms.Panel();
		this.lblCardName = new Apzon.Controls.Editor.CalcEditEx();
		this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
		this.panel5 = new System.Windows.Forms.Panel();
		this.lblCardCode = new Apzon.Controls.Editor.CalcEditEx();
		this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
		this.tileBar3 = new DevExpress.XtraBars.Navigation.TileBar();
		this.tileBarGroup2 = new DevExpress.XtraBars.Navigation.TileBarGroup();
		this.btn_Cash = new DevExpress.XtraBars.Navigation.TileBarItem();
		this.tileBarItem2 = new DevExpress.XtraBars.Navigation.TileBarItem();
		this.tileBarItem3 = new DevExpress.XtraBars.Navigation.TileBarItem();
		this.tileBarItem4 = new DevExpress.XtraBars.Navigation.TileBarItem();
		this.tileBarItem8 = new DevExpress.XtraBars.Navigation.TileBarItem();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.panel4 = new System.Windows.Forms.Panel();
		this.lblInvoiceTotal = new Apzon.Controls.Editor.CalcEditEx();
		this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
		this.panel3 = new System.Windows.Forms.Panel();
		this.lblTaxAmount = new Apzon.Controls.Editor.CalcEditEx();
		this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
		this.panel2 = new System.Windows.Forms.Panel();
		this.lblDiscountAmount = new Apzon.Controls.Editor.CalcEditEx();
		this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
		this.panel1 = new System.Windows.Forms.Panel();
		this.lblSubTotal = new Apzon.Controls.Editor.CalcEditEx();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.grd_Contents = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Contents = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.btnBarCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem2 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem3 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem1 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem4 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem5 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem6 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem7 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem8 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem9 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem10 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.navigationPage1 = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
		this.nframe_Menu = new DevExpress.XtraBars.Navigation.NavigationFrame();
		this.navigationPage2 = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.npage_Discount = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.tileBar2 = new DevExpress.XtraBars.Navigation.TileBar();
		this.tileBarGroup3 = new DevExpress.XtraBars.Navigation.TileBarGroup();
		this.tileBarItem5 = new DevExpress.XtraBars.Navigation.TileBarItem();
		this.tileBarItem6 = new DevExpress.XtraBars.Navigation.TileBarItem();
		this.tileBarItem7 = new DevExpress.XtraBars.Navigation.TileBarItem();
		this.tileBar1 = new DevExpress.XtraBars.Navigation.TileBar();
		this.tileBarItem12 = new DevExpress.XtraBars.Navigation.TileBarItem();
		this.tileItem19 = new DevExpress.XtraEditors.TileItem();
		((System.ComponentModel.ISupportInitialize)this.nframe_Main).BeginInit();
		this.nframe_Main.SuspendLayout();
		this.npage_Main.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nframe_MainMenu).BeginInit();
		this.nframe_MainMenu.SuspendLayout();
		this.npage_MenuMain.SuspendLayout();
		this.npage_DiscountReturnMenu.SuspendLayout();
		this.npage_MenuMiscellaneous.SuspendLayout();
		this.npage_MenuOperation.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureEdit1.Properties).BeginInit();
		this.tableLayoutPanel2.SuspendLayout();
		this.panel10.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lblEmail.Properties).BeginInit();
		this.panel9.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lblTel.Properties).BeginInit();
		this.panel8.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lblBalance.Properties).BeginInit();
		this.panel7.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lblAddress.Properties).BeginInit();
		this.panel6.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lblCardName.Properties).BeginInit();
		this.panel5.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lblCardCode.Properties).BeginInit();
		this.tableLayoutPanel1.SuspendLayout();
		this.panel4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lblInvoiceTotal.Properties).BeginInit();
		this.panel3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lblTaxAmount.Properties).BeginInit();
		this.panel2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lblDiscountAmount.Properties).BeginInit();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lblSubTotal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Contents).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Contents).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btnBarCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl2).BeginInit();
		this.layoutControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nframe_Menu).BeginInit();
		this.nframe_Menu.SuspendLayout();
		base.SuspendLayout();
		this.nframe_Main.Controls.Add(this.npage_Main);
		this.nframe_Main.Controls.Add(this.navigationPage1);
		this.nframe_Main.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nframe_Main.Location = new System.Drawing.Point(0, 0);
		this.nframe_Main.Name = "nframe_Main";
		this.nframe_Main.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[2] { this.npage_Main, this.navigationPage1 });
		this.nframe_Main.SelectedPage = this.npage_Main;
		this.nframe_Main.Size = new System.Drawing.Size(1405, 799);
		this.nframe_Main.TabIndex = 0;
		this.nframe_Main.Text = "navigationFrame1";
		this.nframe_Main.TransitionAnimationProperties.FrameInterval = 5000;
		this.npage_Main.Caption = "npage_Main";
		this.npage_Main.Controls.Add(this.layoutControl1);
		this.npage_Main.Name = "npage_Main";
		this.npage_Main.Size = new System.Drawing.Size(1405, 799);
		this.layoutControl1.Controls.Add(this.panel11);
		this.layoutControl1.Controls.Add(this.nframe_MainMenu);
		this.layoutControl1.Controls.Add(this.tileBar4);
		this.layoutControl1.Controls.Add(this.pictureEdit1);
		this.layoutControl1.Controls.Add(this.labelControl2);
		this.layoutControl1.Controls.Add(this.tableLayoutPanel2);
		this.layoutControl1.Controls.Add(this.tileBar3);
		this.layoutControl1.Controls.Add(this.tableLayoutPanel1);
		this.layoutControl1.Controls.Add(this.grd_Contents);
		this.layoutControl1.Controls.Add(this.btnBarCode);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(1405, 799);
		this.layoutControl1.TabIndex = 0;
		this.layoutControl1.Text = "layoutControl1";
		this.panel11.BackColor = System.Drawing.SystemColors.Info;
		this.panel11.Location = new System.Drawing.Point(2, 777);
		this.panel11.Name = "panel11";
		this.panel11.Size = new System.Drawing.Size(1401, 20);
		this.panel11.TabIndex = 16;
		this.nframe_MainMenu.Controls.Add(this.npage_MenuMain);
		this.nframe_MainMenu.Controls.Add(this.npage_DiscountReturnMenu);
		this.nframe_MainMenu.Controls.Add(this.npage_MenuMiscellaneous);
		this.nframe_MainMenu.Controls.Add(this.npage_MenuOperation);
		this.nframe_MainMenu.Location = new System.Drawing.Point(796, 529);
		this.nframe_MainMenu.Name = "nframe_MainMenu";
		this.nframe_MainMenu.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[4] { this.npage_MenuMain, this.npage_DiscountReturnMenu, this.npage_MenuMiscellaneous, this.npage_MenuOperation });
		this.nframe_MainMenu.SelectedPage = this.npage_MenuMain;
		this.nframe_MainMenu.Size = new System.Drawing.Size(607, 244);
		this.nframe_MainMenu.TabIndex = 15;
		this.nframe_MainMenu.TransitionAnimationProperties.FrameInterval = 5000;
		this.npage_MenuMain.Caption = "npage_MenuMain";
		this.npage_MenuMain.Controls.Add(this.tileControl1);
		this.npage_MenuMain.Name = "npage_MenuMain";
		this.npage_MenuMain.Size = new System.Drawing.Size(607, 244);
		this.tileControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tileControl1.Groups.Add(this.tileGroup2);
		this.tileControl1.IndentBetweenGroups = 0;
		this.tileControl1.IndentBetweenItems = 1;
		this.tileControl1.ItemPadding = new System.Windows.Forms.Padding(0);
		this.tileControl1.LayoutMode = DevExpress.XtraEditors.TileControlLayoutMode.Adaptive;
		this.tileControl1.Location = new System.Drawing.Point(0, 0);
		this.tileControl1.Margin = new System.Windows.Forms.Padding(0);
		this.tileControl1.MaxId = 18;
		this.tileControl1.Name = "tileControl1";
		this.tileControl1.Padding = new System.Windows.Forms.Padding(0);
		this.tileControl1.Size = new System.Drawing.Size(607, 244);
		this.tileControl1.TabIndex = 1;
		this.tileControl1.Text = "tileControl1";
		this.tileGroup2.Items.Add(this.tileItem_ItemSearch);
		this.tileGroup2.Items.Add(this.tileItem2);
		this.tileGroup2.Items.Add(this.tileItem_CustomerSearch);
		this.tileGroup2.Items.Add(this.tileItem3);
		this.tileGroup2.Items.Add(this.tileItem5);
		this.tileGroup2.Items.Add(this.tileItem6);
		this.tileGroup2.Items.Add(this.tileItem7);
		this.tileGroup2.Items.Add(this.tileItem_VoidItem);
		this.tileGroup2.Items.Add(this.tileItem11);
		this.tileGroup2.Name = "tileGroup2";
		this.tileItem_ItemSearch.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_ItemSearch.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem_ItemSearch.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(192, 64, 0);
		this.tileItem_ItemSearch.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_ItemSearch.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_ItemSearch.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_ItemSearch.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_ItemSearch.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem_ItemSearch.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_ItemSearch.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement.Text = "Item Search";
		this.tileItem_ItemSearch.Elements.Add(tileItemElement);
		this.tileItem_ItemSearch.Id = 8;
		this.tileItem_ItemSearch.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem_ItemSearch.Name = "tileItem_ItemSearch";
		this.tileItem_ItemSearch.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileItem_ItemSearch_ItemClick);
		this.tileItem2.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem2.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem2.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem2.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem2.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem2.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem2.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem2.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem2.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem2.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement2.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement2.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement2.Text = "Item Edit";
		this.tileItem2.Elements.Add(tileItemElement2);
		this.tileItem2.Id = 6;
		this.tileItem2.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem2.Name = "tileItem2";
		this.tileItem_CustomerSearch.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(0, 192, 0);
		this.tileItem_CustomerSearch.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_CustomerSearch.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_CustomerSearch.AppearanceItem.Normal.Options.UseFont = true;
		tileItemElement3.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement3.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement3.Text = "Customer Search";
		this.tileItem_CustomerSearch.Elements.Add(tileItemElement3);
		this.tileItem_CustomerSearch.Id = 5;
		this.tileItem_CustomerSearch.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem_CustomerSearch.Name = "tileItem_CustomerSearch";
		this.tileItem_CustomerSearch.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileItem_CustomerSearch_Click);
		this.tileItem3.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem3.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem3.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem3.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem3.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem3.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem3.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem3.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem3.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem3.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement4.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement4.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement4.Text = "Suspend/Recall";
		this.tileItem3.Elements.Add(tileItemElement4);
		this.tileItem3.Id = 7;
		this.tileItem3.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem3.Name = "tileItem3";
		this.tileItem5.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem5.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem5.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem5.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem5.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem5.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem5.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem5.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem5.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem5.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement5.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement5.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement5.Text = "Sale Edit";
		this.tileItem5.Elements.Add(tileItemElement5);
		this.tileItem5.Id = 9;
		this.tileItem5.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem5.Name = "tileItem5";
		this.tileItem6.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem6.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem6.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem6.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem6.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem6.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem6.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem6.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem6.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem6.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement6.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement6.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement6.Text = "Payment";
		this.tileItem6.Elements.Add(tileItemElement6);
		this.tileItem6.Id = 10;
		this.tileItem6.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem6.Name = "tileItem6";
		this.tileItem7.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem7.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem7.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem7.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem7.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem7.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem7.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem7.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem7.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem7.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement7.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement7.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement7.Text = "Quantity";
		this.tileItem7.Elements.Add(tileItemElement7);
		this.tileItem7.Id = 11;
		this.tileItem7.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem7.Name = "tileItem7";
		this.tileItem_VoidItem.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_VoidItem.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem_VoidItem.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem_VoidItem.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_VoidItem.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem_VoidItem.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem_VoidItem.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_VoidItem.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem_VoidItem.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem_VoidItem.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement8.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement8.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement8.Text = "Void Item";
		this.tileItem_VoidItem.Elements.Add(tileItemElement8);
		this.tileItem_VoidItem.Id = 12;
		this.tileItem_VoidItem.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem_VoidItem.Name = "tileItem_VoidItem";
		this.tileItem_VoidItem.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileItem_VoidItem_ItemClick);
		this.tileItem11.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem11.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem11.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem11.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem11.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem11.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem11.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem11.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem11.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem11.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement9.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement9.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement9.Text = "Void Current Transaction";
		this.tileItem11.Elements.Add(tileItemElement9);
		this.tileItem11.Id = 15;
		this.tileItem11.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem11.Name = "tileItem11";
		this.npage_DiscountReturnMenu.Caption = "npage_DiscountReturnMenu";
		this.npage_DiscountReturnMenu.Controls.Add(this.tileControl2);
		this.npage_DiscountReturnMenu.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.npage_DiscountReturnMenu.Name = "npage_DiscountReturnMenu";
		this.npage_DiscountReturnMenu.Size = new System.Drawing.Size(607, 244);
		this.tileControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tileControl2.Groups.Add(this.tileGroup1);
		this.tileControl2.IndentBetweenGroups = 0;
		this.tileControl2.IndentBetweenItems = 1;
		this.tileControl2.ItemPadding = new System.Windows.Forms.Padding(0);
		this.tileControl2.LayoutMode = DevExpress.XtraEditors.TileControlLayoutMode.Adaptive;
		this.tileControl2.Location = new System.Drawing.Point(0, 0);
		this.tileControl2.Margin = new System.Windows.Forms.Padding(0);
		this.tileControl2.MaxId = 20;
		this.tileControl2.Name = "tileControl2";
		this.tileControl2.Padding = new System.Windows.Forms.Padding(0);
		this.tileControl2.Size = new System.Drawing.Size(607, 244);
		this.tileControl2.TabIndex = 2;
		this.tileControl2.Text = "tileControl2";
		this.tileGroup1.Items.Add(this.tileItem9);
		this.tileGroup1.Items.Add(this.tileItem10);
		this.tileGroup1.Items.Add(this.tileItem12);
		this.tileGroup1.Items.Add(this.tileItem13);
		this.tileGroup1.Items.Add(this.tileItem14);
		this.tileGroup1.Items.Add(this.tileItem15);
		this.tileGroup1.Items.Add(this.tileItem16);
		this.tileGroup1.Items.Add(this.tileItem17);
		this.tileGroup1.Name = "tileGroup1";
		this.tileItem9.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem9.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem9.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem9.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem9.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem9.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem9.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem9.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem9.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem9.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement10.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement10.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement10.Text = "Change Price";
		this.tileItem9.Elements.Add(tileItemElement10);
		this.tileItem9.Id = 6;
		this.tileItem9.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem9.Name = "tileItem9";
		this.tileItem10.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem10.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem10.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem10.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem10.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem10.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem10.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem10.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem10.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem10.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement11.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement11.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement11.Text = "";
		this.tileItem10.Elements.Add(tileItemElement11);
		this.tileItem10.Id = 7;
		this.tileItem10.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem10.Name = "tileItem10";
		this.tileItem12.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem12.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem12.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem12.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem12.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem12.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem12.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem12.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem12.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem12.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement12.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement12.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement12.Text = "Line Discount";
		this.tileItem12.Elements.Add(tileItemElement12);
		this.tileItem12.Id = 8;
		this.tileItem12.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem12.Name = "tileItem12";
		this.tileItem13.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem13.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem13.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem13.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem13.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem13.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem13.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem13.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem13.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem13.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement13.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement13.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement13.Text = "";
		this.tileItem13.Elements.Add(tileItemElement13);
		this.tileItem13.Id = 9;
		this.tileItem13.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem13.Name = "tileItem13";
		this.tileItem14.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem14.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem14.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem14.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem14.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem14.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem14.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem14.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem14.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem14.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement14.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement14.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement14.Text = "Sales Discount";
		this.tileItem14.Elements.Add(tileItemElement14);
		this.tileItem14.Id = 10;
		this.tileItem14.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem14.Name = "tileItem14";
		this.tileItem15.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem15.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem15.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem15.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem15.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem15.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem15.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem15.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem15.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem15.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement15.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement15.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement15.Text = "";
		this.tileItem15.Elements.Add(tileItemElement15);
		this.tileItem15.Id = 11;
		this.tileItem15.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem15.Name = "tileItem15";
		this.tileItem16.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem16.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem16.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem16.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem16.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem16.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem16.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem16.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem16.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem16.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement16.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement16.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement16.Text = "";
		this.tileItem16.Elements.Add(tileItemElement16);
		this.tileItem16.Id = 12;
		this.tileItem16.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem16.Name = "tileItem16";
		this.tileItem17.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem17.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem17.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem17.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem17.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem17.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem17.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem17.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem17.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem17.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement17.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement17.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement17.Text = "";
		this.tileItem17.Elements.Add(tileItemElement17);
		this.tileItem17.Id = 15;
		this.tileItem17.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem17.Name = "tileItem17";
		this.npage_MenuMiscellaneous.Caption = "npage_MenuMiscellaneous";
		this.npage_MenuMiscellaneous.Controls.Add(this.tileControl3);
		this.npage_MenuMiscellaneous.Name = "npage_MenuMiscellaneous";
		this.npage_MenuMiscellaneous.Size = new System.Drawing.Size(607, 244);
		this.tileControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tileControl3.Groups.Add(this.tileGroup3);
		this.tileControl3.IndentBetweenGroups = 0;
		this.tileControl3.IndentBetweenItems = 1;
		this.tileControl3.ItemPadding = new System.Windows.Forms.Padding(0);
		this.tileControl3.LayoutMode = DevExpress.XtraEditors.TileControlLayoutMode.Adaptive;
		this.tileControl3.Location = new System.Drawing.Point(0, 0);
		this.tileControl3.Margin = new System.Windows.Forms.Padding(0);
		this.tileControl3.MaxId = 18;
		this.tileControl3.Name = "tileControl3";
		this.tileControl3.Padding = new System.Windows.Forms.Padding(0);
		this.tileControl3.Size = new System.Drawing.Size(607, 244);
		this.tileControl3.TabIndex = 2;
		this.tileControl3.Text = "tileControl3";
		this.tileGroup3.Items.Add(this.tileItem1);
		this.tileGroup3.Items.Add(this.tileItem18);
		this.tileGroup3.Items.Add(this.tileItem20);
		this.tileGroup3.Items.Add(this.tileItem21);
		this.tileGroup3.Items.Add(this.tileItem22);
		this.tileGroup3.Items.Add(this.tileItem23);
		this.tileGroup3.Items.Add(this.tileItem24);
		this.tileGroup3.Items.Add(this.tileItem25);
		this.tileGroup3.Items.Add(this.tileItem26);
		this.tileGroup3.Name = "tileGroup3";
		this.tileItem1.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem1.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem1.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem1.AppearanceItem.Normal.Options.UseFont = true;
		tileItemElement18.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement18.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement18.Text = "Add Customer";
		this.tileItem1.Elements.Add(tileItemElement18);
		this.tileItem1.Id = 5;
		this.tileItem1.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem1.Name = "tileItem1";
		this.tileItem18.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem18.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem18.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem18.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem18.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem18.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem18.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem18.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem18.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem18.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement19.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement19.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement19.Text = "";
		this.tileItem18.Elements.Add(tileItemElement19);
		this.tileItem18.Id = 6;
		this.tileItem18.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem18.Name = "tileItem18";
		this.tileItem20.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem20.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem20.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem20.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem20.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem20.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem20.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem20.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem20.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem20.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement20.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement20.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement20.Text = "Edit Customer";
		this.tileItem20.Elements.Add(tileItemElement20);
		this.tileItem20.Id = 7;
		this.tileItem20.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem20.Name = "tileItem20";
		this.tileItem21.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem21.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem21.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem21.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem21.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem21.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem21.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem21.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem21.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem21.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement21.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement21.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement21.Text = "";
		this.tileItem21.Elements.Add(tileItemElement21);
		this.tileItem21.Id = 8;
		this.tileItem21.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem21.Name = "tileItem21";
		this.tileItem22.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem22.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem22.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem22.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem22.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem22.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem22.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem22.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem22.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem22.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement22.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement22.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement22.Text = "Trasaction Search";
		this.tileItem22.Elements.Add(tileItemElement22);
		this.tileItem22.Id = 9;
		this.tileItem22.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem22.Name = "tileItem22";
		this.tileItem23.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem23.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem23.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem23.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem23.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem23.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem23.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem23.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem23.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem23.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement23.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement23.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement23.Text = "";
		this.tileItem23.Elements.Add(tileItemElement23);
		this.tileItem23.Id = 10;
		this.tileItem23.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem23.Name = "tileItem23";
		this.tileItem24.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem24.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem24.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem24.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem24.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem24.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem24.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem24.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem24.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem24.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement24.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement24.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement24.Text = "Customer View";
		this.tileItem24.Elements.Add(tileItemElement24);
		this.tileItem24.Id = 11;
		this.tileItem24.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem24.Name = "tileItem24";
		this.tileItem25.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem25.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem25.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem25.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem25.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem25.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem25.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem25.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem25.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem25.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement25.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement25.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement25.Text = "";
		this.tileItem25.Elements.Add(tileItemElement25);
		this.tileItem25.Id = 12;
		this.tileItem25.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem25.Name = "tileItem25";
		this.tileItem26.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem26.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem26.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem26.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem26.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem26.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem26.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem26.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem26.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem26.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement26.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement26.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement26.Text = "";
		this.tileItem26.Elements.Add(tileItemElement26);
		this.tileItem26.Id = 15;
		this.tileItem26.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem26.Name = "tileItem26";
		this.npage_MenuOperation.Caption = "npage_MenuOperation";
		this.npage_MenuOperation.Controls.Add(this.tileControl4);
		this.npage_MenuOperation.Name = "npage_MenuOperation";
		this.npage_MenuOperation.Size = new System.Drawing.Size(607, 244);
		this.tileControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tileControl4.Groups.Add(this.tileGroup4);
		this.tileControl4.IndentBetweenGroups = 0;
		this.tileControl4.IndentBetweenItems = 1;
		this.tileControl4.ItemPadding = new System.Windows.Forms.Padding(0);
		this.tileControl4.LayoutMode = DevExpress.XtraEditors.TileControlLayoutMode.Adaptive;
		this.tileControl4.Location = new System.Drawing.Point(0, 0);
		this.tileControl4.Margin = new System.Windows.Forms.Padding(0);
		this.tileControl4.MaxId = 18;
		this.tileControl4.Name = "tileControl4";
		this.tileControl4.Padding = new System.Windows.Forms.Padding(0);
		this.tileControl4.Size = new System.Drawing.Size(607, 244);
		this.tileControl4.TabIndex = 3;
		this.tileControl4.Text = "tileControl4";
		this.tileGroup4.Items.Add(this.tileItem27);
		this.tileGroup4.Items.Add(this.tileItem28);
		this.tileGroup4.Items.Add(this.tileItem29);
		this.tileGroup4.Items.Add(this.tileItem30);
		this.tileGroup4.Items.Add(this.tileItem31);
		this.tileGroup4.Items.Add(this.tileItem32);
		this.tileGroup4.Items.Add(this.tileItem33);
		this.tileGroup4.Items.Add(this.tileItem34);
		this.tileGroup4.Items.Add(this.tileItem35);
		this.tileGroup4.Name = "tileGroup4";
		this.tileItem27.AppearanceItem.Normal.BackColor = System.Drawing.Color.Green;
		this.tileItem27.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem27.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem27.AppearanceItem.Normal.Options.UseFont = true;
		tileItemElement27.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement27.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement27.Text = "Cash In";
		this.tileItem27.Elements.Add(tileItemElement27);
		this.tileItem27.Id = 5;
		this.tileItem27.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem27.Name = "tileItem27";
		this.tileItem28.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem28.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem28.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem28.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem28.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem28.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem28.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem28.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem28.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem28.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement28.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement28.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement28.Text = "Open Drawer";
		this.tileItem28.Elements.Add(tileItemElement28);
		this.tileItem28.Id = 6;
		this.tileItem28.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem28.Name = "tileItem28";
		this.tileItem29.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem29.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem29.AppearanceItem.Normal.BackColor = System.Drawing.Color.Green;
		this.tileItem29.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem29.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem29.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem29.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem29.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem29.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem29.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement29.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement29.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement29.Text = "Cash Out";
		this.tileItem29.Elements.Add(tileItemElement29);
		this.tileItem29.Id = 7;
		this.tileItem29.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem29.Name = "tileItem29";
		this.tileItem30.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem30.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem30.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem30.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem30.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem30.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem30.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem30.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem30.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem30.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement30.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement30.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement30.Text = "Lock Screen";
		this.tileItem30.Elements.Add(tileItemElement30);
		this.tileItem30.Id = 8;
		this.tileItem30.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem30.Name = "tileItem30";
		this.tileItem31.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem31.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem31.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem31.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem31.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem31.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem31.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem31.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem31.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem31.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement31.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement31.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement31.Text = "Expense";
		this.tileItem31.Elements.Add(tileItemElement31);
		this.tileItem31.Id = 9;
		this.tileItem31.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem31.Name = "tileItem31";
		this.tileItem32.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem32.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem32.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(128, 64, 0);
		this.tileItem32.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem32.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem32.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem32.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem32.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem32.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem32.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement32.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement32.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement32.Text = "Log Off";
		this.tileItem32.Elements.Add(tileItemElement32);
		this.tileItem32.Id = 10;
		this.tileItem32.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem32.Name = "tileItem32";
		this.tileItem33.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem33.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem33.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem33.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem33.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem33.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem33.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem33.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem33.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem33.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement33.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement33.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement33.Text = "";
		this.tileItem33.Elements.Add(tileItemElement33);
		this.tileItem33.Id = 11;
		this.tileItem33.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem33.Name = "tileItem33";
		this.tileItem34.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem34.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem34.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.tileItem34.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem34.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem34.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem34.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem34.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem34.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem34.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement34.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement34.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement34.Text = "Shutdown";
		this.tileItem34.Elements.Add(tileItemElement34);
		this.tileItem34.Id = 12;
		this.tileItem34.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem34.Name = "tileItem34";
		this.tileItem35.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem35.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem35.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem35.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem35.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem35.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem35.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem35.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem35.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem35.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement35.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement35.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement35.Text = "";
		this.tileItem35.Elements.Add(tileItemElement35);
		this.tileItem35.Id = 15;
		this.tileItem35.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem35.Name = "tileItem35";
		this.tileBar4.DropDownButtonWidth = 0;
		this.tileBar4.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		this.tileBar4.Groups.Add(this.tileBarGroup1);
		this.tileBar4.GroupTextToItemsIndent = 0;
		this.tileBar4.HorizontalContentAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.tileBar4.IndentBetweenGroups = 0;
		this.tileBar4.IndentBetweenItems = 1;
		this.tileBar4.ItemBorderVisibility = DevExpress.XtraEditors.TileItemBorderVisibility.Always;
		this.tileBar4.ItemImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleRight;
		this.tileBar4.ItemPadding = new System.Windows.Forms.Padding(0);
		this.tileBar4.ItemSize = 80;
		this.tileBar4.ItemTextShowMode = DevExpress.XtraEditors.TileItemContentShowMode.Always;
		this.tileBar4.Location = new System.Drawing.Point(796, 459);
		this.tileBar4.Margin = new System.Windows.Forms.Padding(0);
		this.tileBar4.MaxId = 4;
		this.tileBar4.Name = "tileBar4";
		this.tileBar4.Padding = new System.Windows.Forms.Padding(0);
		this.tileBar4.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.ScrollButtons;
		this.tileBar4.ShowGroupText = false;
		this.tileBar4.Size = new System.Drawing.Size(607, 66);
		this.tileBar4.TabIndex = 14;
		this.tileBar4.Text = "tileBar4";
		this.tileBar4.WideTileWidth = 175;
		this.tileBarGroup1.Items.Add(this.tbi_Main);
		this.tileBarGroup1.Items.Add(this.tbi_DiscountReturnMenu);
		this.tileBarGroup1.Items.Add(this.tbi_MenuMiscellaneous);
		this.tileBarGroup1.Items.Add(this.tbi_MenuOperation);
		this.tileBarGroup1.Name = "tileBarGroup1";
		this.tbi_Main.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tbi_Main.AppearanceItem.Hovered.Options.UseFont = true;
		this.tbi_Main.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tbi_Main.AppearanceItem.Normal.Options.UseFont = true;
		this.tbi_Main.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tbi_Main.AppearanceItem.Pressed.Options.UseFont = true;
		this.tbi_Main.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tbi_Main.AppearanceItem.Selected.Options.UseFont = true;
		this.tbi_Main.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		tileItemElement36.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement36.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement36.Text = "Main";
		this.tbi_Main.Elements.Add(tileItemElement36);
		this.tbi_Main.Id = 0;
		this.tbi_Main.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
		this.tbi_Main.Name = "tbi_Main";
		this.tbi_Main.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tbi_Main_ItemClick);
		this.tbi_DiscountReturnMenu.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tbi_DiscountReturnMenu.AppearanceItem.Hovered.Options.UseFont = true;
		this.tbi_DiscountReturnMenu.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tbi_DiscountReturnMenu.AppearanceItem.Normal.Options.UseFont = true;
		this.tbi_DiscountReturnMenu.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tbi_DiscountReturnMenu.AppearanceItem.Pressed.Options.UseFont = true;
		this.tbi_DiscountReturnMenu.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tbi_DiscountReturnMenu.AppearanceItem.Selected.Options.UseFont = true;
		this.tbi_DiscountReturnMenu.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		tileItemElement37.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement37.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement37.Text = "Discount&Returns";
		this.tbi_DiscountReturnMenu.Elements.Add(tileItemElement37);
		this.tbi_DiscountReturnMenu.Id = 1;
		this.tbi_DiscountReturnMenu.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
		this.tbi_DiscountReturnMenu.Name = "tbi_DiscountReturnMenu";
		this.tbi_DiscountReturnMenu.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tbi_DiscountReturnMenu_ItemClick);
		this.tbi_MenuMiscellaneous.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tbi_MenuMiscellaneous.AppearanceItem.Hovered.Options.UseFont = true;
		this.tbi_MenuMiscellaneous.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tbi_MenuMiscellaneous.AppearanceItem.Normal.Options.UseFont = true;
		this.tbi_MenuMiscellaneous.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tbi_MenuMiscellaneous.AppearanceItem.Pressed.Options.UseFont = true;
		this.tbi_MenuMiscellaneous.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tbi_MenuMiscellaneous.AppearanceItem.Selected.Options.UseFont = true;
		this.tbi_MenuMiscellaneous.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		tileItemElement38.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement38.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement38.Text = "Miscellaneous";
		this.tbi_MenuMiscellaneous.Elements.Add(tileItemElement38);
		this.tbi_MenuMiscellaneous.Id = 3;
		this.tbi_MenuMiscellaneous.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
		this.tbi_MenuMiscellaneous.Name = "tbi_MenuMiscellaneous";
		this.tbi_MenuMiscellaneous.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tbi_MenuMiscellaneous_ItemClick);
		this.tbi_MenuOperation.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tbi_MenuOperation.AppearanceItem.Hovered.Options.UseFont = true;
		this.tbi_MenuOperation.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tbi_MenuOperation.AppearanceItem.Normal.Options.UseFont = true;
		this.tbi_MenuOperation.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tbi_MenuOperation.AppearanceItem.Pressed.Options.UseFont = true;
		this.tbi_MenuOperation.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tbi_MenuOperation.AppearanceItem.Selected.Options.UseFont = true;
		this.tbi_MenuOperation.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		tileItemElement39.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement39.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement39.Text = "POS Operation";
		this.tbi_MenuOperation.Elements.Add(tileItemElement39);
		this.tbi_MenuOperation.Id = 2;
		this.tbi_MenuOperation.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
		this.tbi_MenuOperation.Name = "tbi_MenuOperation";
		this.tbi_MenuOperation.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tbi_MenuOperation_ItemClick);
		this.pictureEdit1.EditValue = resources.GetObject("pictureEdit1.EditValue");
		this.pictureEdit1.Location = new System.Drawing.Point(796, 2);
		this.pictureEdit1.Name = "pictureEdit1";
		this.pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
		this.pictureEdit1.Size = new System.Drawing.Size(276, 453);
		this.pictureEdit1.StyleController = this.layoutControl1;
		this.pictureEdit1.TabIndex = 13;
		this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.labelControl2.Appearance.ForeColor = System.Drawing.Color.RoyalBlue;
		this.labelControl2.Appearance.Options.UseFont = true;
		this.labelControl2.Appearance.Options.UseForeColor = true;
		this.labelControl2.Location = new System.Drawing.Point(1076, 2);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(155, 18);
		this.labelControl2.StyleController = this.layoutControl1;
		this.labelControl2.TabIndex = 12;
		this.labelControl2.Text = "Customer infomation";
		this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
		this.tableLayoutPanel2.ColumnCount = 1;
		this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tableLayoutPanel2.Controls.Add(this.panel10, 0, 5);
		this.tableLayoutPanel2.Controls.Add(this.panel9, 0, 4);
		this.tableLayoutPanel2.Controls.Add(this.panel8, 0, 3);
		this.tableLayoutPanel2.Controls.Add(this.panel7, 0, 2);
		this.tableLayoutPanel2.Controls.Add(this.panel6, 0, 1);
		this.tableLayoutPanel2.Controls.Add(this.panel5, 0, 0);
		this.tableLayoutPanel2.Location = new System.Drawing.Point(1076, 24);
		this.tableLayoutPanel2.Name = "tableLayoutPanel2";
		this.tableLayoutPanel2.RowCount = 6;
		this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.15762f));
		this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15.38821f));
		this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.11354f));
		this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.11354f));
		this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.11354f));
		this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.11354f));
		this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20f));
		this.tableLayoutPanel2.Size = new System.Drawing.Size(327, 431);
		this.tableLayoutPanel2.TabIndex = 11;
		this.panel10.Controls.Add(this.lblEmail);
		this.panel10.Controls.Add(this.labelControl11);
		this.panel10.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel10.Location = new System.Drawing.Point(4, 358);
		this.panel10.Name = "panel10";
		this.panel10.Size = new System.Drawing.Size(319, 69);
		this.panel10.TabIndex = 5;
		this.lblEmail.Anchor = System.Windows.Forms.AnchorStyles.Right;
		this.lblEmail.EditControl = null;
		this.lblEmail.FieldName = null;
		this.lblEmail.IsSearchControl = false;
		this.lblEmail.Location = new System.Drawing.Point(56, 20);
		this.lblEmail.Name = "lblEmail";
		this.lblEmail.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.lblEmail.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblEmail.Properties.Appearance.Options.UseBackColor = true;
		this.lblEmail.Properties.Appearance.Options.UseFont = true;
		this.lblEmail.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblEmail.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblEmail.Properties.DisplayFormat.FormatString = "n2";
		this.lblEmail.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.lblEmail.Properties.IsSearchControl = false;
		this.lblEmail.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.lblEmail.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.lblEmail.SettingInVisible = false;
		this.lblEmail.SettingReadonly = false;
		this.lblEmail.Size = new System.Drawing.Size(257, 22);
		this.lblEmail.TabIndex = 6;
		this.labelControl11.Anchor = System.Windows.Forms.AnchorStyles.Left;
		this.labelControl11.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.labelControl11.Appearance.Options.UseFont = true;
		this.labelControl11.Location = new System.Drawing.Point(5, 24);
		this.labelControl11.Name = "labelControl11";
		this.labelControl11.Size = new System.Drawing.Size(33, 18);
		this.labelControl11.TabIndex = 5;
		this.labelControl11.Text = "Email";
		this.panel9.Controls.Add(this.lblTel);
		this.panel9.Controls.Add(this.labelControl10);
		this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel9.Location = new System.Drawing.Point(4, 285);
		this.panel9.Name = "panel9";
		this.panel9.Size = new System.Drawing.Size(319, 66);
		this.panel9.TabIndex = 4;
		this.lblTel.Anchor = System.Windows.Forms.AnchorStyles.Right;
		this.lblTel.EditControl = null;
		this.lblTel.FieldName = null;
		this.lblTel.IsSearchControl = false;
		this.lblTel.Location = new System.Drawing.Point(56, 24);
		this.lblTel.Name = "lblTel";
		this.lblTel.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.lblTel.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblTel.Properties.Appearance.Options.UseBackColor = true;
		this.lblTel.Properties.Appearance.Options.UseFont = true;
		this.lblTel.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblTel.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblTel.Properties.DisplayFormat.FormatString = "n2";
		this.lblTel.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.lblTel.Properties.IsSearchControl = false;
		this.lblTel.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.lblTel.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.lblTel.SettingInVisible = false;
		this.lblTel.SettingReadonly = false;
		this.lblTel.Size = new System.Drawing.Size(257, 22);
		this.lblTel.TabIndex = 6;
		this.labelControl10.Anchor = System.Windows.Forms.AnchorStyles.Left;
		this.labelControl10.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.labelControl10.Appearance.Options.UseFont = true;
		this.labelControl10.Location = new System.Drawing.Point(5, 26);
		this.labelControl10.Name = "labelControl10";
		this.labelControl10.Size = new System.Drawing.Size(25, 18);
		this.labelControl10.TabIndex = 5;
		this.labelControl10.Text = "Tel.";
		this.panel8.Controls.Add(this.lblBalance);
		this.panel8.Controls.Add(this.labelControl9);
		this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel8.Location = new System.Drawing.Point(4, 212);
		this.panel8.Name = "panel8";
		this.panel8.Size = new System.Drawing.Size(319, 66);
		this.panel8.TabIndex = 3;
		this.lblBalance.Anchor = System.Windows.Forms.AnchorStyles.Right;
		this.lblBalance.EditControl = null;
		this.lblBalance.FieldName = null;
		this.lblBalance.IsSearchControl = false;
		this.lblBalance.Location = new System.Drawing.Point(56, 20);
		this.lblBalance.Name = "lblBalance";
		this.lblBalance.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.lblBalance.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblBalance.Properties.Appearance.Options.UseBackColor = true;
		this.lblBalance.Properties.Appearance.Options.UseFont = true;
		this.lblBalance.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblBalance.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblBalance.Properties.DisplayFormat.FormatString = "n2";
		this.lblBalance.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.lblBalance.Properties.IsSearchControl = false;
		this.lblBalance.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.lblBalance.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.lblBalance.SettingInVisible = false;
		this.lblBalance.SettingReadonly = false;
		this.lblBalance.Size = new System.Drawing.Size(257, 22);
		this.lblBalance.TabIndex = 5;
		this.labelControl9.Anchor = System.Windows.Forms.AnchorStyles.Left;
		this.labelControl9.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.labelControl9.Appearance.Options.UseFont = true;
		this.labelControl9.Location = new System.Drawing.Point(3, 24);
		this.labelControl9.Name = "labelControl9";
		this.labelControl9.Size = new System.Drawing.Size(50, 18);
		this.labelControl9.TabIndex = 4;
		this.labelControl9.Text = "Balance";
		this.panel7.Controls.Add(this.lblAddress);
		this.panel7.Controls.Add(this.labelControl8);
		this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel7.Location = new System.Drawing.Point(4, 139);
		this.panel7.Name = "panel7";
		this.panel7.Size = new System.Drawing.Size(319, 66);
		this.panel7.TabIndex = 2;
		this.lblAddress.Anchor = System.Windows.Forms.AnchorStyles.Right;
		this.lblAddress.EditControl = null;
		this.lblAddress.FieldName = null;
		this.lblAddress.IsSearchControl = false;
		this.lblAddress.Location = new System.Drawing.Point(56, 22);
		this.lblAddress.Name = "lblAddress";
		this.lblAddress.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.lblAddress.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblAddress.Properties.Appearance.Options.UseBackColor = true;
		this.lblAddress.Properties.Appearance.Options.UseFont = true;
		this.lblAddress.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblAddress.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblAddress.Properties.DisplayFormat.FormatString = "n2";
		this.lblAddress.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.lblAddress.Properties.IsSearchControl = false;
		this.lblAddress.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.lblAddress.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.lblAddress.SettingInVisible = false;
		this.lblAddress.SettingReadonly = false;
		this.lblAddress.Size = new System.Drawing.Size(257, 22);
		this.lblAddress.TabIndex = 6;
		this.labelControl8.Anchor = System.Windows.Forms.AnchorStyles.Left;
		this.labelControl8.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.labelControl8.Appearance.Options.UseFont = true;
		this.labelControl8.Location = new System.Drawing.Point(3, 24);
		this.labelControl8.Name = "labelControl8";
		this.labelControl8.Size = new System.Drawing.Size(52, 18);
		this.labelControl8.TabIndex = 3;
		this.labelControl8.Text = "Address";
		this.panel6.Controls.Add(this.lblCardName);
		this.panel6.Controls.Add(this.labelControl7);
		this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel6.Location = new System.Drawing.Point(4, 73);
		this.panel6.Name = "panel6";
		this.panel6.Size = new System.Drawing.Size(319, 59);
		this.panel6.TabIndex = 1;
		this.lblCardName.Anchor = System.Windows.Forms.AnchorStyles.Right;
		this.lblCardName.EditControl = null;
		this.lblCardName.FieldName = null;
		this.lblCardName.IsSearchControl = false;
		this.lblCardName.Location = new System.Drawing.Point(56, 19);
		this.lblCardName.Name = "lblCardName";
		this.lblCardName.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.lblCardName.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblCardName.Properties.Appearance.Options.UseBackColor = true;
		this.lblCardName.Properties.Appearance.Options.UseFont = true;
		this.lblCardName.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblCardName.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblCardName.Properties.DisplayFormat.FormatString = "n2";
		this.lblCardName.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.lblCardName.Properties.IsSearchControl = false;
		this.lblCardName.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.lblCardName.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.lblCardName.SettingInVisible = false;
		this.lblCardName.SettingReadonly = false;
		this.lblCardName.Size = new System.Drawing.Size(257, 22);
		this.lblCardName.TabIndex = 6;
		this.labelControl7.Anchor = System.Windows.Forms.AnchorStyles.Left;
		this.labelControl7.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.labelControl7.Appearance.Options.UseFont = true;
		this.labelControl7.Location = new System.Drawing.Point(3, 21);
		this.labelControl7.Name = "labelControl7";
		this.labelControl7.Size = new System.Drawing.Size(39, 18);
		this.labelControl7.TabIndex = 2;
		this.labelControl7.Text = "Name";
		this.panel5.Controls.Add(this.lblCardCode);
		this.panel5.Controls.Add(this.labelControl6);
		this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel5.Location = new System.Drawing.Point(4, 4);
		this.panel5.Name = "panel5";
		this.panel5.Size = new System.Drawing.Size(319, 62);
		this.panel5.TabIndex = 0;
		this.lblCardCode.Anchor = System.Windows.Forms.AnchorStyles.Right;
		this.lblCardCode.EditControl = null;
		this.lblCardCode.FieldName = null;
		this.lblCardCode.IsSearchControl = false;
		this.lblCardCode.Location = new System.Drawing.Point(56, 22);
		this.lblCardCode.Name = "lblCardCode";
		this.lblCardCode.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.lblCardCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.lblCardCode.Properties.Appearance.Options.UseBackColor = true;
		this.lblCardCode.Properties.Appearance.Options.UseFont = true;
		this.lblCardCode.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblCardCode.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblCardCode.Properties.DisplayFormat.FormatString = "n2";
		this.lblCardCode.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.lblCardCode.Properties.IsSearchControl = false;
		this.lblCardCode.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.lblCardCode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.lblCardCode.SettingInVisible = false;
		this.lblCardCode.SettingReadonly = false;
		this.lblCardCode.Size = new System.Drawing.Size(260, 22);
		this.lblCardCode.TabIndex = 6;
		this.labelControl6.Anchor = System.Windows.Forms.AnchorStyles.Left;
		this.labelControl6.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.labelControl6.Appearance.Options.UseFont = true;
		this.labelControl6.Location = new System.Drawing.Point(3, 24);
		this.labelControl6.Name = "labelControl6";
		this.labelControl6.Size = new System.Drawing.Size(33, 18);
		this.labelControl6.TabIndex = 1;
		this.labelControl6.Text = "Code";
		this.tileBar3.AppearanceText.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBar3.AppearanceText.Options.UseFont = true;
		this.tileBar3.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		this.tileBar3.Groups.Add(this.tileBarGroup2);
		this.tileBar3.GroupTextToItemsIndent = 0;
		this.tileBar3.IndentBetweenGroups = 0;
		this.tileBar3.IndentBetweenItems = 1;
		this.tileBar3.ItemSize = 100;
		this.tileBar3.Location = new System.Drawing.Point(2, 659);
		this.tileBar3.MaxId = 6;
		this.tileBar3.Name = "tileBar3";
		this.tileBar3.Padding = new System.Windows.Forms.Padding(0);
		this.tileBar3.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.ScrollButtons;
		this.tileBar3.ShowGroupText = false;
		this.tileBar3.Size = new System.Drawing.Size(790, 114);
		this.tileBar3.TabIndex = 10;
		this.tileBar3.Text = "tileBar3";
		this.tileBar3.WideTileWidth = 200;
		this.tileBarGroup2.Items.Add(this.btn_Cash);
		this.tileBarGroup2.Items.Add(this.tileBarItem2);
		this.tileBarGroup2.Items.Add(this.tileBarItem3);
		this.tileBarGroup2.Items.Add(this.tileBarItem4);
		this.tileBarGroup2.Items.Add(this.tileBarItem8);
		this.tileBarGroup2.Name = "tileBarGroup2";
		this.btn_Cash.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.False;
		this.btn_Cash.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(64, 0, 0);
		this.btn_Cash.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Cash.AppearanceItem.Normal.Options.UseBackColor = true;
		this.btn_Cash.AppearanceItem.Normal.Options.UseFont = true;
		this.btn_Cash.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		tileItemElement40.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("resource.Image");
		tileItemElement40.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileItemElement40.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
		tileItemElement40.Text = "Cash";
		this.btn_Cash.Elements.Add(tileItemElement40);
		this.btn_Cash.Id = 1;
		this.btn_Cash.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
		this.btn_Cash.Name = "btn_Cash";
		this.btn_Cash.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.btn_Cash_ItemClick);
		this.tileBarItem2.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(64, 64, 0);
		this.tileBarItem2.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem2.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileBarItem2.AppearanceItem.Normal.Options.UseFont = true;
		this.tileBarItem2.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		tileItemElement41.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("resource.Image1");
		tileItemElement41.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileItemElement41.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
		tileItemElement41.Text = "Payment";
		this.tileBarItem2.Elements.Add(tileItemElement41);
		this.tileBarItem2.Id = 2;
		this.tileBarItem2.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
		this.tileBarItem2.Name = "tileBarItem2";
		this.tileBarItem2.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileBarItem2_ItemClick);
		this.tileBarItem3.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(0, 64, 0);
		this.tileBarItem3.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem3.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileBarItem3.AppearanceItem.Normal.Options.UseFont = true;
		this.tileBarItem3.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		tileItemElement42.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("resource.Image2");
		tileItemElement42.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileItemElement42.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
		tileItemElement42.Text = "Credit Card";
		this.tileBarItem3.Elements.Add(tileItemElement42);
		this.tileBarItem3.Id = 3;
		this.tileBarItem3.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
		this.tileBarItem3.Name = "tileBarItem3";
		this.tileBarItem3.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileBarItem3_ItemClick);
		this.tileBarItem4.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(0, 64, 64);
		this.tileBarItem4.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem4.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileBarItem4.AppearanceItem.Normal.Options.UseFont = true;
		this.tileBarItem4.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		tileItemElement43.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("resource.Image3");
		tileItemElement43.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileItemElement43.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
		tileItemElement43.Text = "Foreign Currency";
		this.tileBarItem4.Elements.Add(tileItemElement43);
		this.tileBarItem4.Id = 4;
		this.tileBarItem4.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
		this.tileBarItem4.Name = "tileBarItem4";
		this.tileBarItem4.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileBarItem4_ItemClick);
		this.tileBarItem8.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(64, 0, 64);
		this.tileBarItem8.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem8.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileBarItem8.AppearanceItem.Normal.Options.UseFont = true;
		this.tileBarItem8.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		tileItemElement44.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("resource.Image4");
		tileItemElement44.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileItemElement44.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement44.Text = "On Account";
		this.tileBarItem8.Elements.Add(tileItemElement44);
		this.tileBarItem8.Id = 5;
		this.tileBarItem8.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
		this.tileBarItem8.Name = "tileBarItem8";
		this.tileBarItem8.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileBarItem8_ItemClick);
		this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
		this.tableLayoutPanel1.ColumnCount = 2;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.Controls.Add(this.panel4, 1, 1);
		this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 1);
		this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
		this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
		this.tableLayoutPanel1.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 566);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.RowCount = 2;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.Size = new System.Drawing.Size(790, 89);
		this.tableLayoutPanel1.TabIndex = 9;
		this.panel4.Controls.Add(this.lblInvoiceTotal);
		this.panel4.Controls.Add(this.labelControl5);
		this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel4.Location = new System.Drawing.Point(398, 48);
		this.panel4.Name = "panel4";
		this.panel4.Size = new System.Drawing.Size(388, 37);
		this.panel4.TabIndex = 3;
		this.lblInvoiceTotal.Anchor = System.Windows.Forms.AnchorStyles.Right;
		this.lblInvoiceTotal.EditControl = null;
		this.lblInvoiceTotal.EditValue = new decimal(new int[4]);
		this.lblInvoiceTotal.FieldName = null;
		this.lblInvoiceTotal.IsSearchControl = false;
		this.lblInvoiceTotal.Location = new System.Drawing.Point(185, 6);
		this.lblInvoiceTotal.Name = "lblInvoiceTotal";
		this.lblInvoiceTotal.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.lblInvoiceTotal.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.lblInvoiceTotal.Properties.Appearance.ForeColor = System.Drawing.Color.Green;
		this.lblInvoiceTotal.Properties.Appearance.Options.UseBackColor = true;
		this.lblInvoiceTotal.Properties.Appearance.Options.UseFont = true;
		this.lblInvoiceTotal.Properties.Appearance.Options.UseForeColor = true;
		this.lblInvoiceTotal.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblInvoiceTotal.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblInvoiceTotal.Properties.DisplayFormat.FormatString = "n2";
		this.lblInvoiceTotal.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.lblInvoiceTotal.Properties.IsSearchControl = false;
		this.lblInvoiceTotal.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.lblInvoiceTotal.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.lblInvoiceTotal.SettingInVisible = false;
		this.lblInvoiceTotal.SettingReadonly = false;
		this.lblInvoiceTotal.Size = new System.Drawing.Size(200, 22);
		this.lblInvoiceTotal.TabIndex = 2;
		this.labelControl5.Anchor = System.Windows.Forms.AnchorStyles.Left;
		this.labelControl5.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.labelControl5.Appearance.Options.UseFont = true;
		this.labelControl5.Location = new System.Drawing.Point(3, 8);
		this.labelControl5.Name = "labelControl5";
		this.labelControl5.Size = new System.Drawing.Size(33, 18);
		this.labelControl5.TabIndex = 1;
		this.labelControl5.Text = "Total";
		this.panel3.Controls.Add(this.lblTaxAmount);
		this.panel3.Controls.Add(this.labelControl4);
		this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel3.Location = new System.Drawing.Point(4, 48);
		this.panel3.Name = "panel3";
		this.panel3.Size = new System.Drawing.Size(387, 37);
		this.panel3.TabIndex = 2;
		this.lblTaxAmount.Anchor = System.Windows.Forms.AnchorStyles.Right;
		this.lblTaxAmount.EditControl = null;
		this.lblTaxAmount.EditValue = new decimal(new int[4]);
		this.lblTaxAmount.FieldName = null;
		this.lblTaxAmount.IsSearchControl = false;
		this.lblTaxAmount.Location = new System.Drawing.Point(184, 6);
		this.lblTaxAmount.Name = "lblTaxAmount";
		this.lblTaxAmount.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.lblTaxAmount.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.lblTaxAmount.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(32, 31, 53);
		this.lblTaxAmount.Properties.Appearance.Options.UseBackColor = true;
		this.lblTaxAmount.Properties.Appearance.Options.UseFont = true;
		this.lblTaxAmount.Properties.Appearance.Options.UseForeColor = true;
		this.lblTaxAmount.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblTaxAmount.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblTaxAmount.Properties.DisplayFormat.FormatString = "n2";
		this.lblTaxAmount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.lblTaxAmount.Properties.IsSearchControl = false;
		this.lblTaxAmount.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.lblTaxAmount.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.lblTaxAmount.SettingInVisible = false;
		this.lblTaxAmount.SettingReadonly = false;
		this.lblTaxAmount.Size = new System.Drawing.Size(200, 22);
		this.lblTaxAmount.TabIndex = 2;
		this.labelControl4.Anchor = System.Windows.Forms.AnchorStyles.Left;
		this.labelControl4.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.labelControl4.Appearance.Options.UseFont = true;
		this.labelControl4.Location = new System.Drawing.Point(3, 8);
		this.labelControl4.Name = "labelControl4";
		this.labelControl4.Size = new System.Drawing.Size(82, 18);
		this.labelControl4.TabIndex = 1;
		this.labelControl4.Text = "Tax Amount";
		this.panel2.Controls.Add(this.lblDiscountAmount);
		this.panel2.Controls.Add(this.labelControl3);
		this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel2.Location = new System.Drawing.Point(398, 4);
		this.panel2.Name = "panel2";
		this.panel2.Size = new System.Drawing.Size(388, 37);
		this.panel2.TabIndex = 1;
		this.lblDiscountAmount.Anchor = System.Windows.Forms.AnchorStyles.Right;
		this.lblDiscountAmount.EditControl = null;
		this.lblDiscountAmount.EditValue = new decimal(new int[4]);
		this.lblDiscountAmount.FieldName = null;
		this.lblDiscountAmount.IsSearchControl = false;
		this.lblDiscountAmount.Location = new System.Drawing.Point(185, 9);
		this.lblDiscountAmount.Name = "lblDiscountAmount";
		this.lblDiscountAmount.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.lblDiscountAmount.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.lblDiscountAmount.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(32, 31, 53);
		this.lblDiscountAmount.Properties.Appearance.Options.UseBackColor = true;
		this.lblDiscountAmount.Properties.Appearance.Options.UseFont = true;
		this.lblDiscountAmount.Properties.Appearance.Options.UseForeColor = true;
		this.lblDiscountAmount.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblDiscountAmount.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblDiscountAmount.Properties.DisplayFormat.FormatString = "n2";
		this.lblDiscountAmount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.lblDiscountAmount.Properties.IsSearchControl = false;
		this.lblDiscountAmount.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.lblDiscountAmount.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.lblDiscountAmount.SettingInVisible = false;
		this.lblDiscountAmount.SettingReadonly = false;
		this.lblDiscountAmount.Size = new System.Drawing.Size(200, 22);
		this.lblDiscountAmount.TabIndex = 2;
		this.labelControl3.Anchor = System.Windows.Forms.AnchorStyles.Left;
		this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.labelControl3.Appearance.Options.UseFont = true;
		this.labelControl3.Location = new System.Drawing.Point(3, 11);
		this.labelControl3.Name = "labelControl3";
		this.labelControl3.Size = new System.Drawing.Size(111, 18);
		this.labelControl3.TabIndex = 1;
		this.labelControl3.Text = "Discount Amount";
		this.panel1.Controls.Add(this.lblSubTotal);
		this.panel1.Controls.Add(this.labelControl1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel1.Location = new System.Drawing.Point(4, 4);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(387, 37);
		this.panel1.TabIndex = 0;
		this.lblSubTotal.Anchor = System.Windows.Forms.AnchorStyles.Right;
		this.lblSubTotal.EditControl = null;
		this.lblSubTotal.EditValue = new decimal(new int[4]);
		this.lblSubTotal.FieldName = null;
		this.lblSubTotal.IsSearchControl = false;
		this.lblSubTotal.Location = new System.Drawing.Point(184, 9);
		this.lblSubTotal.Name = "lblSubTotal";
		this.lblSubTotal.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.lblSubTotal.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.lblSubTotal.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(32, 31, 53);
		this.lblSubTotal.Properties.Appearance.Options.UseBackColor = true;
		this.lblSubTotal.Properties.Appearance.Options.UseFont = true;
		this.lblSubTotal.Properties.Appearance.Options.UseForeColor = true;
		this.lblSubTotal.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblSubTotal.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblSubTotal.Properties.DisplayFormat.FormatString = "n2";
		this.lblSubTotal.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.lblSubTotal.Properties.IsSearchControl = false;
		this.lblSubTotal.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.lblSubTotal.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.lblSubTotal.SettingInVisible = false;
		this.lblSubTotal.SettingReadonly = false;
		this.lblSubTotal.Size = new System.Drawing.Size(200, 22);
		this.lblSubTotal.TabIndex = 1;
		this.labelControl1.Anchor = System.Windows.Forms.AnchorStyles.Left;
		this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.labelControl1.Appearance.Options.UseFont = true;
		this.labelControl1.Location = new System.Drawing.Point(3, 11);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(67, 18);
		this.labelControl1.TabIndex = 0;
		this.labelControl1.Text = "Sub. Total";
		this.grd_Contents.AllowPasteExcel = true;
		this.grd_Contents.ClipboardData = null;
		this.grd_Contents.Location = new System.Drawing.Point(2, 48);
		this.grd_Contents.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.grd_Contents.LookAndFeel.UseDefaultLookAndFeel = false;
		this.grd_Contents.MainView = this.gv_Contents;
		this.grd_Contents.Name = "grd_Contents";
		this.grd_Contents.Size = new System.Drawing.Size(790, 514);
		this.grd_Contents.TabIndex = 8;
		this.grd_Contents.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Contents });
		this.gv_Contents.Appearance.HeaderPanel.BackColor = System.Drawing.Color.SkyBlue;
		this.gv_Contents.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.gv_Contents.Appearance.HeaderPanel.Options.UseBackColor = true;
		this.gv_Contents.Appearance.HeaderPanel.Options.UseFont = true;
		this.gv_Contents.Appearance.Row.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.gv_Contents.Appearance.Row.Options.UseFont = true;
		this.gv_Contents.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Contents.ColumnKey = null;
		this.gv_Contents.ColumnPanelRowHeight = 50;
		this.gv_Contents.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn8, this.gridColumn9, this.gridColumn5, this.gridColumn6, this.gridColumn7 });
		this.gv_Contents.DisableFormSetting = false;
		this.gv_Contents.DisplayException = false;
		this.gv_Contents.EditColumn = null;
		this.gv_Contents.FieldCurrency = null;
		this.gv_Contents.FieldRate = null;
		this.gv_Contents.FormId = null;
		this.gv_Contents.GridControl = this.grd_Contents;
		this.gv_Contents.IgnoreFilter = false;
		this.gv_Contents.IndexEditRow = -1;
		this.gv_Contents.IndicatorWidth = 50;
		this.gv_Contents.IsApzTable = false;
		this.gv_Contents.IsInsertNewRow = false;
		this.gv_Contents.IsLoadingData = false;
		this.gv_Contents.IsMappingContextAction = false;
		this.gv_Contents.ItemId = null;
		this.gv_Contents.LstColumnAllowBatch = null;
		this.gv_Contents.LstColumnNotAllowBatch = null;
		this.gv_Contents.LstColumnNotEditable = null;
		this.gv_Contents.LstColumNotSetting = null;
		this.gv_Contents.Name = "gv_Contents";
		this.gv_Contents.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Contents.OptionsView.ShowGroupPanel = false;
		this.gv_Contents.RowHeight = 50;
		this.gv_Contents.UseBatchMod = false;
		this.gv_Contents.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Contents_CellValueChanged);
		this.gridColumn1.Caption = "Type";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "TranType";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 68;
		this.gridColumn2.Caption = "Product Code";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "ItemCode";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.AllowEdit = false;
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 112;
		this.gridColumn3.Caption = "Description";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "ItemName";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 2;
		this.gridColumn3.Width = 94;
		this.gridColumn4.Caption = "Discount (%)";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "Discount";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 5;
		this.gridColumn4.Width = 89;
		this.gridColumn8.Caption = "Tax (%)";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "TaxRate";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 6;
		this.gridColumn8.Width = 86;
		this.gridColumn9.Caption = "Tax Amount";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "TaxAmount";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn5.Caption = "Qty";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "Qty";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 3;
		this.gridColumn5.Width = 83;
		this.gridColumn6.Caption = "Price";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.DisplayFormat.FormatString = "n0";
		this.gridColumn6.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn6.FieldName = "Price";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 4;
		this.gridColumn6.Width = 83;
		this.gridColumn7.Caption = "Total";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.DisplayFormat.FormatString = "n0";
		this.gridColumn7.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn7.FieldName = "LineTotal";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.OptionsColumn.AllowEdit = false;
		this.gridColumn7.OptionsColumn.ReadOnly = true;
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 7;
		this.gridColumn7.Width = 101;
		this.btnBarCode.EditControl = null;
		this.btnBarCode.FieldName = null;
		this.btnBarCode.IsSearchControl = false;
		this.btnBarCode.Location = new System.Drawing.Point(2, 2);
		this.btnBarCode.Name = "btnBarCode";
		this.btnBarCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btnBarCode.Properties.Appearance.Options.UseFont = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.btnBarCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btnBarCode.SettingInVisible = false;
		this.btnBarCode.SettingReadonly = false;
		this.btnBarCode.Size = new System.Drawing.Size(790, 42);
		this.btnBarCode.StyleController = this.layoutControl1;
		this.btnBarCode.TabIndex = 7;
		this.btnBarCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.buttonEdit1_KeyDown);
		this.btnBarCode.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnBarCode_MouseDown);
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[11]
		{
			this.layoutControlItem2, this.layoutControlItem3, this.layoutControlItem1, this.layoutControlItem4, this.layoutControlItem5, this.emptySpaceItem3, this.layoutControlItem6, this.layoutControlItem7, this.layoutControlItem8, this.layoutControlItem9,
			this.layoutControlItem10
		});
		this.layoutControlGroup1.Name = "layoutControlGroup1";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(1405, 799);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem2.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem2.Control = this.btnBarCode;
		this.layoutControlItem2.EditControl = null;
		this.layoutControlItem2.EditValue = "layoutControlItem2";
		this.layoutControlItem2.FieldName = null;
		this.layoutControlItem2.IsSearchControl = false;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.ReadOnly = false;
		this.layoutControlItem2.SettingInVisible = false;
		this.layoutControlItem2.SettingReadonly = false;
		this.layoutControlItem2.Size = new System.Drawing.Size(794, 46);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.layoutControlItem3.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem3.Control = this.grd_Contents;
		this.layoutControlItem3.EditControl = null;
		this.layoutControlItem3.EditValue = "layoutControlItem3";
		this.layoutControlItem3.FieldName = null;
		this.layoutControlItem3.IsSearchControl = false;
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 46);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.ReadOnly = false;
		this.layoutControlItem3.SettingInVisible = false;
		this.layoutControlItem3.SettingReadonly = false;
		this.layoutControlItem3.Size = new System.Drawing.Size(794, 518);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.layoutControlItem1.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem1.Control = this.tableLayoutPanel1;
		this.layoutControlItem1.EditControl = null;
		this.layoutControlItem1.EditValue = "layoutControlItem1";
		this.layoutControlItem1.FieldName = null;
		this.layoutControlItem1.IsSearchControl = false;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 564);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.ReadOnly = false;
		this.layoutControlItem1.SettingInVisible = false;
		this.layoutControlItem1.SettingReadonly = false;
		this.layoutControlItem1.Size = new System.Drawing.Size(794, 93);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem4.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem4.Control = this.tileBar3;
		this.layoutControlItem4.EditControl = null;
		this.layoutControlItem4.EditValue = "layoutControlItem4";
		this.layoutControlItem4.FieldName = null;
		this.layoutControlItem4.IsSearchControl = false;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 657);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.ReadOnly = false;
		this.layoutControlItem4.SettingInVisible = false;
		this.layoutControlItem4.SettingReadonly = false;
		this.layoutControlItem4.Size = new System.Drawing.Size(794, 118);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.layoutControlItem5.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem5.Control = this.tableLayoutPanel2;
		this.layoutControlItem5.EditControl = null;
		this.layoutControlItem5.EditValue = "layoutControlItem5";
		this.layoutControlItem5.FieldName = null;
		this.layoutControlItem5.IsSearchControl = false;
		this.layoutControlItem5.Location = new System.Drawing.Point(1074, 22);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.ReadOnly = false;
		this.layoutControlItem5.SettingInVisible = false;
		this.layoutControlItem5.SettingReadonly = false;
		this.layoutControlItem5.Size = new System.Drawing.Size(331, 435);
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(1233, 0);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(172, 22);
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem6.Control = this.labelControl2;
		this.layoutControlItem6.EditControl = null;
		this.layoutControlItem6.EditValue = "layoutControlItem6";
		this.layoutControlItem6.FieldName = null;
		this.layoutControlItem6.IsSearchControl = false;
		this.layoutControlItem6.Location = new System.Drawing.Point(1074, 0);
		this.layoutControlItem6.Name = "layoutControlItem6";
		this.layoutControlItem6.ReadOnly = false;
		this.layoutControlItem6.SettingInVisible = false;
		this.layoutControlItem6.SettingReadonly = false;
		this.layoutControlItem6.Size = new System.Drawing.Size(159, 22);
		this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.TextVisible = false;
		this.layoutControlItem7.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem7.Control = this.pictureEdit1;
		this.layoutControlItem7.EditControl = null;
		this.layoutControlItem7.EditValue = "layoutControlItem7";
		this.layoutControlItem7.FieldName = null;
		this.layoutControlItem7.IsSearchControl = false;
		this.layoutControlItem7.Location = new System.Drawing.Point(794, 0);
		this.layoutControlItem7.Name = "layoutControlItem7";
		this.layoutControlItem7.ReadOnly = false;
		this.layoutControlItem7.SettingInVisible = false;
		this.layoutControlItem7.SettingReadonly = false;
		this.layoutControlItem7.Size = new System.Drawing.Size(280, 457);
		this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem7.TextVisible = false;
		this.layoutControlItem8.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem8.Control = this.tileBar4;
		this.layoutControlItem8.EditControl = null;
		this.layoutControlItem8.EditValue = "layoutControlItem8";
		this.layoutControlItem8.FieldName = null;
		this.layoutControlItem8.IsSearchControl = false;
		this.layoutControlItem8.Location = new System.Drawing.Point(794, 457);
		this.layoutControlItem8.Name = "layoutControlItem8";
		this.layoutControlItem8.ReadOnly = false;
		this.layoutControlItem8.SettingInVisible = false;
		this.layoutControlItem8.SettingReadonly = false;
		this.layoutControlItem8.Size = new System.Drawing.Size(611, 70);
		this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem8.TextVisible = false;
		this.layoutControlItem9.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem9.Control = this.nframe_MainMenu;
		this.layoutControlItem9.EditControl = null;
		this.layoutControlItem9.EditValue = "layoutControlItem9";
		this.layoutControlItem9.FieldName = null;
		this.layoutControlItem9.IsSearchControl = false;
		this.layoutControlItem9.Location = new System.Drawing.Point(794, 527);
		this.layoutControlItem9.Name = "layoutControlItem9";
		this.layoutControlItem9.ReadOnly = false;
		this.layoutControlItem9.SettingInVisible = false;
		this.layoutControlItem9.SettingReadonly = false;
		this.layoutControlItem9.Size = new System.Drawing.Size(611, 248);
		this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem9.TextVisible = false;
		this.layoutControlItem10.BackColor = System.Drawing.Color.Empty;
		this.layoutControlItem10.Control = this.panel11;
		this.layoutControlItem10.EditControl = null;
		this.layoutControlItem10.EditValue = "layoutControlItem10";
		this.layoutControlItem10.FieldName = null;
		this.layoutControlItem10.IsSearchControl = false;
		this.layoutControlItem10.Location = new System.Drawing.Point(0, 775);
		this.layoutControlItem10.Name = "layoutControlItem10";
		this.layoutControlItem10.ReadOnly = false;
		this.layoutControlItem10.SettingInVisible = false;
		this.layoutControlItem10.SettingReadonly = false;
		this.layoutControlItem10.Size = new System.Drawing.Size(1405, 24);
		this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem10.TextVisible = false;
		this.navigationPage1.Caption = "navigationPage1";
		this.navigationPage1.Name = "navigationPage1";
		this.navigationPage1.Size = new System.Drawing.Size(1405, 799);
		this.layoutControl2.Controls.Add(this.nframe_Menu);
		this.layoutControl2.Controls.Add(this.tileBar2);
		this.layoutControl2.Location = new System.Drawing.Point(0, 0);
		this.layoutControl2.Name = "layoutControl2";
		this.layoutControl2.Root = null;
		this.layoutControl2.Size = new System.Drawing.Size(180, 120);
		this.layoutControl2.TabIndex = 0;
		this.nframe_Menu.Controls.Add(this.navigationPage2);
		this.nframe_Menu.Controls.Add(this.npage_Discount);
		this.nframe_Menu.Location = new System.Drawing.Point(802, 551);
		this.nframe_Menu.Margin = new System.Windows.Forms.Padding(0);
		this.nframe_Menu.Name = "nframe_Menu";
		this.nframe_Menu.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[2] { this.navigationPage2, this.npage_Discount });
		this.nframe_Menu.SelectedPage = this.navigationPage2;
		this.nframe_Menu.Size = new System.Drawing.Size(560, 295);
		this.nframe_Menu.TabIndex = 10;
		this.nframe_Menu.TransitionAnimationProperties.FrameInterval = 3000;
		this.navigationPage2.Caption = "navigationPage2";
		this.navigationPage2.Name = "navigationPage2";
		this.navigationPage2.Size = new System.Drawing.Size(560, 295);
		this.npage_Discount.Caption = "npage_Discount";
		this.npage_Discount.Name = "npage_Discount";
		this.npage_Discount.Size = new System.Drawing.Size(560, 295);
		this.tileBar2.DropDownButtonWidth = 0;
		this.tileBar2.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		this.tileBar2.Groups.Add(this.tileBarGroup3);
		this.tileBar2.GroupTextToItemsIndent = 0;
		this.tileBar2.HorizontalContentAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.tileBar2.IndentBetweenGroups = 0;
		this.tileBar2.IndentBetweenItems = 1;
		this.tileBar2.ItemBorderVisibility = DevExpress.XtraEditors.TileItemBorderVisibility.Always;
		this.tileBar2.ItemImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleRight;
		this.tileBar2.ItemPadding = new System.Windows.Forms.Padding(0);
		this.tileBar2.ItemSize = 80;
		this.tileBar2.ItemTextShowMode = DevExpress.XtraEditors.TileItemContentShowMode.Always;
		this.tileBar2.Location = new System.Drawing.Point(802, 12);
		this.tileBar2.Margin = new System.Windows.Forms.Padding(0);
		this.tileBar2.MaxId = 3;
		this.tileBar2.Name = "tileBar2";
		this.tileBar2.Padding = new System.Windows.Forms.Padding(0);
		this.tileBar2.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.ScrollButtons;
		this.tileBar2.ShowGroupText = false;
		this.tileBar2.ShowItemShadow = true;
		this.tileBar2.Size = new System.Drawing.Size(560, 535);
		this.tileBar2.TabIndex = 9;
		this.tileBar2.Text = "tileBar2";
		this.tileBar2.WideTileWidth = 175;
		this.tileBarGroup3.Items.Add(this.tileBarItem5);
		this.tileBarGroup3.Items.Add(this.tileBarItem6);
		this.tileBarGroup3.Items.Add(this.tileBarItem7);
		this.tileBarGroup3.Name = "tileBarGroup3";
		this.tileBarItem5.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem5.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileBarItem5.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem5.AppearanceItem.Normal.Options.UseFont = true;
		this.tileBarItem5.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem5.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileBarItem5.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem5.AppearanceItem.Selected.Options.UseFont = true;
		this.tileBarItem5.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		tileItemElement45.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement45.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement45.Text = "Main";
		this.tileBarItem5.Elements.Add(tileItemElement45);
		this.tileBarItem5.Id = 0;
		this.tileBarItem5.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
		this.tileBarItem5.Name = "tileBarItem5";
		this.tileBarItem6.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem6.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileBarItem6.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem6.AppearanceItem.Normal.Options.UseFont = true;
		this.tileBarItem6.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem6.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileBarItem6.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem6.AppearanceItem.Selected.Options.UseFont = true;
		this.tileBarItem6.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		tileItemElement46.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement46.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement46.Text = "Discount";
		this.tileBarItem6.Elements.Add(tileItemElement46);
		this.tileBarItem6.Id = 1;
		this.tileBarItem6.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
		this.tileBarItem6.Name = "tileBarItem6";
		this.tileBarItem7.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem7.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileBarItem7.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem7.AppearanceItem.Normal.Options.UseFont = true;
		this.tileBarItem7.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem7.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileBarItem7.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem7.AppearanceItem.Selected.Options.UseFont = true;
		this.tileBarItem7.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		tileItemElement47.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement47.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement47.Text = "POS Operation";
		this.tileBarItem7.Elements.Add(tileItemElement47);
		this.tileBarItem7.Id = 2;
		this.tileBarItem7.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
		this.tileBarItem7.Name = "tileBarItem7";
		this.tileBar1.AppearanceText.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBar1.AppearanceText.Options.UseFont = true;
		this.tileBar1.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		this.tileBar1.Location = new System.Drawing.Point(0, 0);
		this.tileBar1.Name = "tileBar1";
		this.tileBar1.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.ScrollButtons;
		this.tileBar1.Size = new System.Drawing.Size(240, 150);
		this.tileBar1.TabIndex = 0;
		this.tileBarItem12.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem12.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileBarItem12.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem12.AppearanceItem.Normal.Options.UseFont = true;
		this.tileBarItem12.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem12.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileBarItem12.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBarItem12.AppearanceItem.Selected.Options.UseFont = true;
		this.tileBarItem12.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		tileItemElement48.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement48.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement48.Text = "POS Operation";
		this.tileBarItem12.Elements.Add(tileItemElement48);
		this.tileBarItem12.Id = 2;
		this.tileBarItem12.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
		this.tileBarItem12.Name = "tileBarItem12";
		this.tileItem19.AppearanceItem.Hovered.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem19.AppearanceItem.Hovered.Options.UseFont = true;
		this.tileItem19.AppearanceItem.Normal.BackColor = System.Drawing.Color.Teal;
		this.tileItem19.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem19.AppearanceItem.Normal.Options.UseBackColor = true;
		this.tileItem19.AppearanceItem.Normal.Options.UseFont = true;
		this.tileItem19.AppearanceItem.Pressed.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem19.AppearanceItem.Pressed.Options.UseFont = true;
		this.tileItem19.AppearanceItem.Selected.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileItem19.AppearanceItem.Selected.Options.UseFont = true;
		tileItemElement49.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileItemElement49.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Right;
		tileItemElement49.Text = "";
		this.tileItem19.Elements.Add(tileItemElement49);
		this.tileItem19.Id = 12;
		this.tileItem19.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
		this.tileItem19.Name = "tileItem19";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1405, 799);
		base.Controls.Add(this.nframe_Main);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_PointOfSalesBAK.IconOptions.Icon");
		base.Name = "frm_PointOfSalesBAK";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Apzon Retail - Point of Sale";
		base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
		base.Load += new System.EventHandler(this.Main_Load);
		((System.ComponentModel.ISupportInitialize)this.nframe_Main).EndInit();
		this.nframe_Main.ResumeLayout(false);
		this.npage_Main.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.nframe_MainMenu).EndInit();
		this.nframe_MainMenu.ResumeLayout(false);
		this.npage_MenuMain.ResumeLayout(false);
		this.npage_DiscountReturnMenu.ResumeLayout(false);
		this.npage_MenuMiscellaneous.ResumeLayout(false);
		this.npage_MenuOperation.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pictureEdit1.Properties).EndInit();
		this.tableLayoutPanel2.ResumeLayout(false);
		this.panel10.ResumeLayout(false);
		this.panel10.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.lblEmail.Properties).EndInit();
		this.panel9.ResumeLayout(false);
		this.panel9.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.lblTel.Properties).EndInit();
		this.panel8.ResumeLayout(false);
		this.panel8.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.lblBalance.Properties).EndInit();
		this.panel7.ResumeLayout(false);
		this.panel7.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.lblAddress.Properties).EndInit();
		this.panel6.ResumeLayout(false);
		this.panel6.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.lblCardName.Properties).EndInit();
		this.panel5.ResumeLayout(false);
		this.panel5.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.lblCardCode.Properties).EndInit();
		this.tableLayoutPanel1.ResumeLayout(false);
		this.panel4.ResumeLayout(false);
		this.panel4.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.lblInvoiceTotal.Properties).EndInit();
		this.panel3.ResumeLayout(false);
		this.panel3.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.lblTaxAmount.Properties).EndInit();
		this.panel2.ResumeLayout(false);
		this.panel2.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.lblDiscountAmount.Properties).EndInit();
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.lblSubTotal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Contents).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Contents).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btnBarCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl2).EndInit();
		this.layoutControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.nframe_Menu).EndInit();
		this.nframe_Menu.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
