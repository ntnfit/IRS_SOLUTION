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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms;

public class frm_Retail : XtraFormEx
{
	private const string FormController = "Retail";

	private double _invoiceTotal;

	private double _invoiceTax;

	private double _totalAmount;

	private double _invoiceDiscount;

	private bool _cellediting;

	private IContainer components;

	private PanelControl panelControl1;

	private SimpleButtonEx simpleButton1;

	private SimpleButtonEx simpleButton7;

	private SimpleButtonEx simpleButton6;

	private SimpleButtonEx simpleButton5;

	private SimpleButtonEx simpleButton4;

	private SimpleButtonEx simpleButton3;

	private SimpleButtonEx simpleButton2;

	private LabelControl labelControl4;

	private LabelControl lblInvoiceTotal;

	private SimpleButtonEx simpleButton37;

	private SimpleButtonEx simpleButton38;

	private SimpleButtonEx simpleButton39;

	private SimpleButtonEx simpleButton40;

	private SimpleButtonEx simpleButton41;

	private SimpleButtonEx simpleButton42;

	private SimpleButtonEx simpleButton43;

	private SimpleButtonEx simpleButton44;

	private SimpleButtonEx simpleButton45;

	private SimpleButtonEx simpleButton46;

	private SimpleButtonEx simpleButton47;

	private SimpleButtonEx simpleButton48;

	private SimpleButtonEx simpleButton49;

	private SimpleButtonEx simpleButton50;

	private SimpleButtonEx simpleButton51;

	private SplitContainerControl splitContainerControl1;

	private SplitContainerControl splitContainerControl2;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private LabelLine labelLine4;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private TextEditEx txtTotalAmount;

	private TextEditEx txtTax;

	private TextEditEx txtInvoiceTotal;

	private TextEditEx txtPayment;

	private SimpleButtonEx simpleButton21;

	private SimpleButtonEx simpleButton18;

	private SimpleButtonEx simpleButton19;

	private SimpleButtonEx simpleButton20;

	private SimpleButtonEx simpleButton15;

	private SimpleButtonEx simpleButton16;

	private SimpleButtonEx simpleButton17;

	private SimpleButtonEx simpleButton12;

	private SimpleButtonEx simpleButton13;

	private SimpleButtonEx simpleButton14;

	private SimpleButtonEx simpleButton9;

	private SimpleButtonEx simpleButton10;

	private SimpleButtonEx simpleButton11;

	private TextEditEx txtBarCode;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private SimpleButtonEx simpleButton8;

	private GroupControl groupControl1;

	private LabelControl labelControl3;

	private LabelControl labelControl2;

	private GridColumnEx gridColumn8;

	private LabelControl lblCurrency;

	public Size OldSize { get; set; }

	public frm_Retail()
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
					txtInvoiceTotal.EditValue = _invoiceTotal;
					txtTax.EditValue = _invoiceTax;
					txtTotalAmount.EditValue = _totalAmount;
					lblInvoiceTotal.Text = Function.ToString(_invoiceTotal);
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
			txtTotalAmount.EditValue = _totalAmount.ToString("#,##0.00");
			txtInvoiceTotal.EditValue = _invoiceTotal.ToString("#,##0.00");
			txtTax.EditValue = _invoiceTax.ToString("#,##0.00");
			lblInvoiceTotal.Text = _invoiceTotal.ToString("#,##0.00");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_Retail_Load(object sender, EventArgs e)
	{
	}

	private void simpleButton6_Click(object sender, EventArgs e)
	{
	}

	private void simpleButton4_Click(object sender, EventArgs e)
	{
	}

	private void txtBarCode_EditValueChanged(object sender, EventArgs e)
	{
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

	private void gvDoc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
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
			lblInvoiceTotal.Text = _invoiceTotal.ToString("#,##0.00");
			txtTotalAmount.EditValue = _totalAmount.ToString("#,##0.00");
			txtInvoiceTotal.EditValue = _invoiceTotal.ToString("#,##0.00");
			txtTax.EditValue = _invoiceTax.ToString("#,##0.00");
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
		try
		{
			txtPayment.Text = Function.ParseDouble(txtPayment.EditValue).ToString("#,##0.00");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.frm_Retail));
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.lblCurrency = new DevExpress.XtraEditors.LabelControl();
		this.lblInvoiceTotal = new DevExpress.XtraEditors.LabelControl();
		this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
		this.txtBarCode = new Apzon.Controls.Editor.TextEditEx();
		this.simpleButton7 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton6 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton5 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton4 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton3 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton2 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton1 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton37 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton38 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton39 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton40 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton41 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton42 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton43 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton44 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton45 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton46 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton47 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton48 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton49 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton50 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton51 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.simpleButton8 = new Apzon.Controls.Controls.SimpleButtonEx();
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
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txtTotalAmount = new Apzon.Controls.Editor.TextEditEx();
		this.txtTax = new Apzon.Controls.Editor.TextEditEx();
		this.txtInvoiceTotal = new Apzon.Controls.Editor.TextEditEx();
		this.txtPayment = new Apzon.Controls.Editor.TextEditEx();
		this.simpleButton21 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton18 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton19 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton20 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton15 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton16 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton17 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton12 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton13 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton14 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton9 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton10 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton11 = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtBarCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtTotalAmount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtTax.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtInvoiceTotal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPayment.Properties).BeginInit();
		base.SuspendLayout();
		this.panelControl1.Appearance.BackColor = System.Drawing.Color.FromArgb(189, 208, 237);
		this.panelControl1.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.panelControl1.Appearance.Options.UseBackColor = true;
		this.panelControl1.Controls.Add(this.lblCurrency);
		this.panelControl1.Controls.Add(this.lblInvoiceTotal);
		this.panelControl1.Controls.Add(this.labelControl4);
		this.panelControl1.Controls.Add(this.txtBarCode);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.panelControl1.LookAndFeel.UseDefaultLookAndFeel = false;
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(1187, 30);
		this.panelControl1.TabIndex = 0;
		this.lblCurrency.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lblCurrency.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Bold);
		this.lblCurrency.Appearance.Options.UseFont = true;
		this.lblCurrency.Location = new System.Drawing.Point(1124, 1);
		this.lblCurrency.Name = "lblCurrency";
		this.lblCurrency.Size = new System.Drawing.Size(42, 23);
		this.lblCurrency.TabIndex = 93;
		this.lblCurrency.Text = "VND";
		this.lblInvoiceTotal.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lblInvoiceTotal.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Bold);
		this.lblInvoiceTotal.Appearance.Options.UseFont = true;
		this.lblInvoiceTotal.Appearance.Options.UseTextOptions = true;
		this.lblInvoiceTotal.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.lblInvoiceTotal.Location = new System.Drawing.Point(1106, 1);
		this.lblInvoiceTotal.Name = "lblInvoiceTotal";
		this.lblInvoiceTotal.Size = new System.Drawing.Size(12, 23);
		this.lblInvoiceTotal.TabIndex = 5;
		this.lblInvoiceTotal.Text = "0";
		this.lblInvoiceTotal.TextChanged += new System.EventHandler(this.lblInvoiceTotal_TextChanged);
		this.lblInvoiceTotal.Resize += new System.EventHandler(this.lblInvoiceTotal_Resize);
		this.labelControl4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelControl4.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Bold);
		this.labelControl4.Appearance.Options.UseFont = true;
		this.labelControl4.Location = new System.Drawing.Point(824, 1);
		this.labelControl4.Name = "labelControl4";
		this.labelControl4.Size = new System.Drawing.Size(63, 23);
		this.labelControl4.TabIndex = 4;
		this.labelControl4.Text = "TOTAL";
		this.txtBarCode.EditControl = null;
		this.txtBarCode.FieldName = null;
		this.txtBarCode.IsSearchControl = false;
		this.txtBarCode.Location = new System.Drawing.Point(207, 4);
		this.txtBarCode.Name = "txtBarCode";
		this.txtBarCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.txtBarCode.Properties.Appearance.Options.UseFont = true;
		this.txtBarCode.SettingInVisible = false;
		this.txtBarCode.SettingReadonly = false;
		this.txtBarCode.Size = new System.Drawing.Size(442, 28);
		this.txtBarCode.TabIndex = 92;
		this.txtBarCode.EditValueChanged += new System.EventHandler(this.txtBarCode_EditValueChanged);
		this.txtBarCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBarCode_KeyDown);
		this.simpleButton7.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton7.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton7.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton7.Appearance.Options.UseBackColor = true;
		this.simpleButton7.Appearance.Options.UseFont = true;
		this.simpleButton7.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton7.EditControl = null;
		this.simpleButton7.EditValue = "OUTGOING                   F7";
		this.simpleButton7.FieldName = null;
		this.simpleButton7.IsSearchControl = false;
		this.simpleButton7.Location = new System.Drawing.Point(2, 302);
		this.simpleButton7.Name = "simpleButton7";
		this.simpleButton7.ReadOnly = false;
		this.simpleButton7.SettingInVisible = false;
		this.simpleButton7.SettingReadonly = false;
		this.simpleButton7.Size = new System.Drawing.Size(190, 45);
		this.simpleButton7.TabIndex = 30;
		this.simpleButton7.Text = "OUTGOING                   F7";
		this.simpleButton6.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton6.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton6.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton6.Appearance.Options.UseBackColor = true;
		this.simpleButton6.Appearance.Options.UseFont = true;
		this.simpleButton6.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton6.EditControl = null;
		this.simpleButton6.EditValue = "INCOMING                   F6";
		this.simpleButton6.FieldName = null;
		this.simpleButton6.IsSearchControl = false;
		this.simpleButton6.Location = new System.Drawing.Point(2, 252);
		this.simpleButton6.Name = "simpleButton6";
		this.simpleButton6.ReadOnly = false;
		this.simpleButton6.SettingInVisible = false;
		this.simpleButton6.SettingReadonly = false;
		this.simpleButton6.Size = new System.Drawing.Size(190, 45);
		this.simpleButton6.TabIndex = 29;
		this.simpleButton6.Text = "INCOMING                   F6";
		this.simpleButton6.Click += new System.EventHandler(this.simpleButton6_Click);
		this.simpleButton5.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton5.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton5.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton5.Appearance.Options.UseBackColor = true;
		this.simpleButton5.Appearance.Options.UseFont = true;
		this.simpleButton5.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton5.EditControl = null;
		this.simpleButton5.EditValue = "CASH                             F5";
		this.simpleButton5.FieldName = null;
		this.simpleButton5.IsSearchControl = false;
		this.simpleButton5.Location = new System.Drawing.Point(2, 202);
		this.simpleButton5.Name = "simpleButton5";
		this.simpleButton5.ReadOnly = false;
		this.simpleButton5.SettingInVisible = false;
		this.simpleButton5.SettingReadonly = false;
		this.simpleButton5.Size = new System.Drawing.Size(190, 45);
		this.simpleButton5.TabIndex = 28;
		this.simpleButton5.Text = "CASH                             F5";
		this.simpleButton4.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton4.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton4.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton4.Appearance.Options.UseBackColor = true;
		this.simpleButton4.Appearance.Options.UseFont = true;
		this.simpleButton4.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton4.EditControl = null;
		this.simpleButton4.EditValue = "PAYMENT                     F4";
		this.simpleButton4.FieldName = null;
		this.simpleButton4.IsSearchControl = false;
		this.simpleButton4.Location = new System.Drawing.Point(2, 152);
		this.simpleButton4.Name = "simpleButton4";
		this.simpleButton4.ReadOnly = false;
		this.simpleButton4.SettingInVisible = false;
		this.simpleButton4.SettingReadonly = false;
		this.simpleButton4.Size = new System.Drawing.Size(190, 45);
		this.simpleButton4.TabIndex = 27;
		this.simpleButton4.Text = "PAYMENT                     F4";
		this.simpleButton4.Click += new System.EventHandler(this.simpleButton4_Click);
		this.simpleButton3.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton3.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton3.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton3.Appearance.Options.UseBackColor = true;
		this.simpleButton3.Appearance.Options.UseFont = true;
		this.simpleButton3.Appearance.Options.UseTextOptions = true;
		this.simpleButton3.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.simpleButton3.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton3.EditControl = null;
		this.simpleButton3.EditValue = "DISCOUNTS                 F3";
		this.simpleButton3.FieldName = null;
		this.simpleButton3.IsSearchControl = false;
		this.simpleButton3.Location = new System.Drawing.Point(2, 103);
		this.simpleButton3.Name = "simpleButton3";
		this.simpleButton3.ReadOnly = false;
		this.simpleButton3.SettingInVisible = false;
		this.simpleButton3.SettingReadonly = false;
		this.simpleButton3.Size = new System.Drawing.Size(190, 45);
		this.simpleButton3.TabIndex = 26;
		this.simpleButton3.Text = "DISCOUNTS                 F3";
		this.simpleButton2.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton2.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton2.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton2.Appearance.Options.UseBackColor = true;
		this.simpleButton2.Appearance.Options.UseFont = true;
		this.simpleButton2.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton2.EditControl = null;
		this.simpleButton2.EditValue = "ITEMS                           F2";
		this.simpleButton2.FieldName = null;
		this.simpleButton2.IsSearchControl = false;
		this.simpleButton2.Location = new System.Drawing.Point(2, 53);
		this.simpleButton2.Name = "simpleButton2";
		this.simpleButton2.ReadOnly = false;
		this.simpleButton2.SettingInVisible = false;
		this.simpleButton2.SettingReadonly = false;
		this.simpleButton2.Size = new System.Drawing.Size(190, 45);
		this.simpleButton2.TabIndex = 25;
		this.simpleButton2.Text = "ITEMS                           F2";
		this.simpleButton1.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton1.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton1.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton1.Appearance.Options.UseBackColor = true;
		this.simpleButton1.Appearance.Options.UseFont = true;
		this.simpleButton1.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton1.EditControl = null;
		this.simpleButton1.EditValue = "CUSTOMERS                F1";
		this.simpleButton1.FieldName = null;
		this.simpleButton1.IsSearchControl = false;
		this.simpleButton1.Location = new System.Drawing.Point(2, 3);
		this.simpleButton1.Name = "simpleButton1";
		this.simpleButton1.ReadOnly = false;
		this.simpleButton1.SettingInVisible = false;
		this.simpleButton1.SettingReadonly = false;
		this.simpleButton1.Size = new System.Drawing.Size(190, 45);
		this.simpleButton1.TabIndex = 24;
		this.simpleButton1.Text = "CUSTOMERS                F1";
		this.simpleButton37.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton37.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton37.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton37.Appearance.Options.UseBackColor = true;
		this.simpleButton37.Appearance.Options.UseFont = true;
		this.simpleButton37.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton37.EditControl = null;
		this.simpleButton37.EditValue = ".";
		this.simpleButton37.FieldName = null;
		this.simpleButton37.IsSearchControl = false;
		this.simpleButton37.Location = new System.Drawing.Point(127, 184);
		this.simpleButton37.Name = "simpleButton37";
		this.simpleButton37.ReadOnly = false;
		this.simpleButton37.SettingInVisible = false;
		this.simpleButton37.SettingReadonly = false;
		this.simpleButton37.Size = new System.Drawing.Size(60, 60);
		this.simpleButton37.TabIndex = 37;
		this.simpleButton37.Text = ".";
		this.simpleButton38.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton38.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton38.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton38.Appearance.Options.UseBackColor = true;
		this.simpleButton38.Appearance.Options.UseFont = true;
		this.simpleButton38.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton38.EditControl = null;
		this.simpleButton38.EditValue = "Enter";
		this.simpleButton38.FieldName = null;
		this.simpleButton38.IsSearchControl = false;
		this.simpleButton38.Location = new System.Drawing.Point(189, 123);
		this.simpleButton38.Name = "simpleButton38";
		this.simpleButton38.ReadOnly = false;
		this.simpleButton38.SettingInVisible = false;
		this.simpleButton38.SettingReadonly = false;
		this.simpleButton38.Size = new System.Drawing.Size(60, 121);
		this.simpleButton38.TabIndex = 36;
		this.simpleButton38.Text = "Enter";
		this.simpleButton39.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton39.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton39.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton39.Appearance.Options.UseBackColor = true;
		this.simpleButton39.Appearance.Options.UseFont = true;
		this.simpleButton39.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton39.EditControl = null;
		this.simpleButton39.EditValue = "3";
		this.simpleButton39.FieldName = null;
		this.simpleButton39.IsSearchControl = false;
		this.simpleButton39.Location = new System.Drawing.Point(127, 123);
		this.simpleButton39.Name = "simpleButton39";
		this.simpleButton39.ReadOnly = false;
		this.simpleButton39.SettingInVisible = false;
		this.simpleButton39.SettingReadonly = false;
		this.simpleButton39.Size = new System.Drawing.Size(60, 60);
		this.simpleButton39.TabIndex = 36;
		this.simpleButton39.Text = "3";
		this.simpleButton40.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton40.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton40.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton40.Appearance.Options.UseBackColor = true;
		this.simpleButton40.Appearance.Options.UseFont = true;
		this.simpleButton40.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton40.EditControl = null;
		this.simpleButton40.EditValue = "C";
		this.simpleButton40.FieldName = null;
		this.simpleButton40.IsSearchControl = false;
		this.simpleButton40.Location = new System.Drawing.Point(189, 62);
		this.simpleButton40.Name = "simpleButton40";
		this.simpleButton40.ReadOnly = false;
		this.simpleButton40.SettingInVisible = false;
		this.simpleButton40.SettingReadonly = false;
		this.simpleButton40.Size = new System.Drawing.Size(60, 60);
		this.simpleButton40.TabIndex = 35;
		this.simpleButton40.Text = "C";
		this.simpleButton41.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton41.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton41.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton41.Appearance.Options.UseBackColor = true;
		this.simpleButton41.Appearance.Options.UseFont = true;
		this.simpleButton41.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton41.EditControl = null;
		this.simpleButton41.EditValue = "6";
		this.simpleButton41.FieldName = null;
		this.simpleButton41.IsSearchControl = false;
		this.simpleButton41.Location = new System.Drawing.Point(127, 62);
		this.simpleButton41.Name = "simpleButton41";
		this.simpleButton41.ReadOnly = false;
		this.simpleButton41.SettingInVisible = false;
		this.simpleButton41.SettingReadonly = false;
		this.simpleButton41.Size = new System.Drawing.Size(60, 60);
		this.simpleButton41.TabIndex = 35;
		this.simpleButton41.Text = "6";
		this.simpleButton42.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton42.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton42.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton42.Appearance.Options.UseBackColor = true;
		this.simpleButton42.Appearance.Options.UseFont = true;
		this.simpleButton42.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton42.EditControl = null;
		this.simpleButton42.EditValue = "Back";
		this.simpleButton42.FieldName = null;
		this.simpleButton42.IsSearchControl = false;
		this.simpleButton42.Location = new System.Drawing.Point(189, 1);
		this.simpleButton42.Name = "simpleButton42";
		this.simpleButton42.ReadOnly = false;
		this.simpleButton42.SettingInVisible = false;
		this.simpleButton42.SettingReadonly = false;
		this.simpleButton42.Size = new System.Drawing.Size(60, 60);
		this.simpleButton42.TabIndex = 34;
		this.simpleButton42.Text = "Back";
		this.simpleButton43.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton43.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton43.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton43.Appearance.Options.UseBackColor = true;
		this.simpleButton43.Appearance.Options.UseFont = true;
		this.simpleButton43.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton43.EditControl = null;
		this.simpleButton43.EditValue = "9";
		this.simpleButton43.FieldName = null;
		this.simpleButton43.IsSearchControl = false;
		this.simpleButton43.Location = new System.Drawing.Point(127, 1);
		this.simpleButton43.Name = "simpleButton43";
		this.simpleButton43.ReadOnly = false;
		this.simpleButton43.SettingInVisible = false;
		this.simpleButton43.SettingReadonly = false;
		this.simpleButton43.Size = new System.Drawing.Size(60, 60);
		this.simpleButton43.TabIndex = 34;
		this.simpleButton43.Text = "9";
		this.simpleButton44.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton44.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton44.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton44.Appearance.Options.UseBackColor = true;
		this.simpleButton44.Appearance.Options.UseFont = true;
		this.simpleButton44.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton44.EditControl = null;
		this.simpleButton44.EditValue = "*";
		this.simpleButton44.FieldName = null;
		this.simpleButton44.IsSearchControl = false;
		this.simpleButton44.Location = new System.Drawing.Point(65, 184);
		this.simpleButton44.Name = "simpleButton44";
		this.simpleButton44.ReadOnly = false;
		this.simpleButton44.SettingInVisible = false;
		this.simpleButton44.SettingReadonly = false;
		this.simpleButton44.Size = new System.Drawing.Size(60, 60);
		this.simpleButton44.TabIndex = 33;
		this.simpleButton44.Text = "*";
		this.simpleButton45.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton45.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton45.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton45.Appearance.Options.UseBackColor = true;
		this.simpleButton45.Appearance.Options.UseFont = true;
		this.simpleButton45.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton45.EditControl = null;
		this.simpleButton45.EditValue = "2";
		this.simpleButton45.FieldName = null;
		this.simpleButton45.IsSearchControl = false;
		this.simpleButton45.Location = new System.Drawing.Point(65, 123);
		this.simpleButton45.Name = "simpleButton45";
		this.simpleButton45.ReadOnly = false;
		this.simpleButton45.SettingInVisible = false;
		this.simpleButton45.SettingReadonly = false;
		this.simpleButton45.Size = new System.Drawing.Size(60, 60);
		this.simpleButton45.TabIndex = 32;
		this.simpleButton45.Text = "2";
		this.simpleButton46.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton46.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton46.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton46.Appearance.Options.UseBackColor = true;
		this.simpleButton46.Appearance.Options.UseFont = true;
		this.simpleButton46.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton46.EditControl = null;
		this.simpleButton46.EditValue = "5";
		this.simpleButton46.FieldName = null;
		this.simpleButton46.IsSearchControl = false;
		this.simpleButton46.Location = new System.Drawing.Point(65, 62);
		this.simpleButton46.Name = "simpleButton46";
		this.simpleButton46.ReadOnly = false;
		this.simpleButton46.SettingInVisible = false;
		this.simpleButton46.SettingReadonly = false;
		this.simpleButton46.Size = new System.Drawing.Size(60, 60);
		this.simpleButton46.TabIndex = 31;
		this.simpleButton46.Text = "5";
		this.simpleButton47.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton47.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton47.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton47.Appearance.Options.UseBackColor = true;
		this.simpleButton47.Appearance.Options.UseFont = true;
		this.simpleButton47.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton47.EditControl = null;
		this.simpleButton47.EditValue = "8";
		this.simpleButton47.FieldName = null;
		this.simpleButton47.IsSearchControl = false;
		this.simpleButton47.Location = new System.Drawing.Point(65, 1);
		this.simpleButton47.Name = "simpleButton47";
		this.simpleButton47.ReadOnly = false;
		this.simpleButton47.SettingInVisible = false;
		this.simpleButton47.SettingReadonly = false;
		this.simpleButton47.Size = new System.Drawing.Size(60, 60);
		this.simpleButton47.TabIndex = 30;
		this.simpleButton47.Text = "8";
		this.simpleButton48.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton48.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton48.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton48.Appearance.Options.UseBackColor = true;
		this.simpleButton48.Appearance.Options.UseFont = true;
		this.simpleButton48.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton48.EditControl = null;
		this.simpleButton48.EditValue = "0";
		this.simpleButton48.FieldName = null;
		this.simpleButton48.IsSearchControl = false;
		this.simpleButton48.Location = new System.Drawing.Point(3, 184);
		this.simpleButton48.Name = "simpleButton48";
		this.simpleButton48.ReadOnly = false;
		this.simpleButton48.SettingInVisible = false;
		this.simpleButton48.SettingReadonly = false;
		this.simpleButton48.Size = new System.Drawing.Size(60, 60);
		this.simpleButton48.TabIndex = 29;
		this.simpleButton48.Text = "0";
		this.simpleButton49.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton49.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton49.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton49.Appearance.Options.UseBackColor = true;
		this.simpleButton49.Appearance.Options.UseFont = true;
		this.simpleButton49.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton49.EditControl = null;
		this.simpleButton49.EditValue = "1";
		this.simpleButton49.FieldName = null;
		this.simpleButton49.IsSearchControl = false;
		this.simpleButton49.Location = new System.Drawing.Point(3, 123);
		this.simpleButton49.Name = "simpleButton49";
		this.simpleButton49.ReadOnly = false;
		this.simpleButton49.SettingInVisible = false;
		this.simpleButton49.SettingReadonly = false;
		this.simpleButton49.Size = new System.Drawing.Size(60, 60);
		this.simpleButton49.TabIndex = 28;
		this.simpleButton49.Text = "1";
		this.simpleButton50.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton50.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton50.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton50.Appearance.Options.UseBackColor = true;
		this.simpleButton50.Appearance.Options.UseFont = true;
		this.simpleButton50.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton50.EditControl = null;
		this.simpleButton50.EditValue = "4";
		this.simpleButton50.FieldName = null;
		this.simpleButton50.IsSearchControl = false;
		this.simpleButton50.Location = new System.Drawing.Point(3, 62);
		this.simpleButton50.Name = "simpleButton50";
		this.simpleButton50.ReadOnly = false;
		this.simpleButton50.SettingInVisible = false;
		this.simpleButton50.SettingReadonly = false;
		this.simpleButton50.Size = new System.Drawing.Size(60, 60);
		this.simpleButton50.TabIndex = 27;
		this.simpleButton50.Text = "4";
		this.simpleButton51.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton51.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton51.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton51.Appearance.Options.UseBackColor = true;
		this.simpleButton51.Appearance.Options.UseFont = true;
		this.simpleButton51.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton51.EditControl = null;
		this.simpleButton51.EditValue = "7";
		this.simpleButton51.FieldName = null;
		this.simpleButton51.IsSearchControl = false;
		this.simpleButton51.Location = new System.Drawing.Point(3, 1);
		this.simpleButton51.Name = "simpleButton51";
		this.simpleButton51.ReadOnly = false;
		this.simpleButton51.SettingInVisible = false;
		this.simpleButton51.SettingReadonly = false;
		this.simpleButton51.Size = new System.Drawing.Size(60, 60);
		this.simpleButton51.TabIndex = 26;
		this.simpleButton51.Text = "7";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Appearance.BackColor = System.Drawing.Color.FromArgb(189, 208, 237);
		this.splitContainerControl1.Appearance.Options.UseBackColor = true;
		this.splitContainerControl1.IsSplitterFixed = true;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, 30);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.simpleButton8);
		this.splitContainerControl1.Panel1.Controls.Add(this.simpleButton1);
		this.splitContainerControl1.Panel1.Controls.Add(this.simpleButton7);
		this.splitContainerControl1.Panel1.Controls.Add(this.simpleButton2);
		this.splitContainerControl1.Panel1.Controls.Add(this.simpleButton6);
		this.splitContainerControl1.Panel1.Controls.Add(this.simpleButton3);
		this.splitContainerControl1.Panel1.Controls.Add(this.simpleButton5);
		this.splitContainerControl1.Panel1.Controls.Add(this.simpleButton4);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grdDoc);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1186, 469);
		this.splitContainerControl1.SplitterPosition = 197;
		this.splitContainerControl1.TabIndex = 69;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.simpleButton8.Appearance.BackColor = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton8.Appearance.BackColor2 = System.Drawing.Color.FromArgb(175, 205, 242);
		this.simpleButton8.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton8.Appearance.Options.UseBackColor = true;
		this.simpleButton8.Appearance.Options.UseFont = true;
		this.simpleButton8.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton8.EditControl = null;
		this.simpleButton8.EditValue = "SHIFT                            F8";
		this.simpleButton8.FieldName = null;
		this.simpleButton8.IsSearchControl = false;
		this.simpleButton8.Location = new System.Drawing.Point(2, 353);
		this.simpleButton8.Name = "simpleButton8";
		this.simpleButton8.ReadOnly = false;
		this.simpleButton8.SettingInVisible = false;
		this.simpleButton8.SettingReadonly = false;
		this.simpleButton8.Size = new System.Drawing.Size(190, 45);
		this.simpleButton8.TabIndex = 31;
		this.simpleButton8.Text = "SHIFT                            F8";
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Location = new System.Drawing.Point(4, 0);
		this.grdDoc.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
		this.grdDoc.LookAndFeel.UseDefaultLookAndFeel = false;
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.Size = new System.Drawing.Size(980, 466);
		this.grdDoc.TabIndex = 0;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.gvDoc.Appearance.HeaderPanel.BackColor = System.Drawing.Color.FromArgb(189, 208, 237);
		this.gvDoc.Appearance.HeaderPanel.BackColor2 = System.Drawing.Color.FromArgb(189, 208, 237);
		this.gvDoc.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 10.25f, System.Drawing.FontStyle.Bold);
		this.gvDoc.Appearance.HeaderPanel.Options.UseBackColor = true;
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
		this.gvDoc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanging);
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
		this.gridColumn1.PrimaryKey = true;
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
		this.gridColumn6.Caption = "VAT(%)";
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
		this.gridColumn8.Caption = "VAT Amount";
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
		this.splitContainerControl2.Appearance.BackColor = System.Drawing.Color.FromArgb(189, 208, 237);
		this.splitContainerControl2.Appearance.Options.UseBackColor = true;
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.splitContainerControl2.IsSplitterFixed = true;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 502);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.simpleButton37);
		this.splitContainerControl2.Panel1.Controls.Add(this.simpleButton51);
		this.splitContainerControl2.Panel1.Controls.Add(this.simpleButton38);
		this.splitContainerControl2.Panel1.Controls.Add(this.simpleButton50);
		this.splitContainerControl2.Panel1.Controls.Add(this.simpleButton39);
		this.splitContainerControl2.Panel1.Controls.Add(this.simpleButton49);
		this.splitContainerControl2.Panel1.Controls.Add(this.simpleButton40);
		this.splitContainerControl2.Panel1.Controls.Add(this.simpleButton48);
		this.splitContainerControl2.Panel1.Controls.Add(this.simpleButton41);
		this.splitContainerControl2.Panel1.Controls.Add(this.simpleButton47);
		this.splitContainerControl2.Panel1.Controls.Add(this.simpleButton42);
		this.splitContainerControl2.Panel1.Controls.Add(this.simpleButton46);
		this.splitContainerControl2.Panel1.Controls.Add(this.simpleButton43);
		this.splitContainerControl2.Panel1.Controls.Add(this.simpleButton45);
		this.splitContainerControl2.Panel1.Controls.Add(this.simpleButton44);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.groupControl1);
		this.splitContainerControl2.Panel2.Controls.Add(this.labelLine4);
		this.splitContainerControl2.Panel2.Controls.Add(this.labelLine3);
		this.splitContainerControl2.Panel2.Controls.Add(this.labelLine2);
		this.splitContainerControl2.Panel2.Controls.Add(this.labelLine1);
		this.splitContainerControl2.Panel2.Controls.Add(this.txtTotalAmount);
		this.splitContainerControl2.Panel2.Controls.Add(this.txtTax);
		this.splitContainerControl2.Panel2.Controls.Add(this.txtInvoiceTotal);
		this.splitContainerControl2.Panel2.Controls.Add(this.txtPayment);
		this.splitContainerControl2.Panel2.Controls.Add(this.simpleButton21);
		this.splitContainerControl2.Panel2.Controls.Add(this.simpleButton18);
		this.splitContainerControl2.Panel2.Controls.Add(this.simpleButton19);
		this.splitContainerControl2.Panel2.Controls.Add(this.simpleButton20);
		this.splitContainerControl2.Panel2.Controls.Add(this.simpleButton15);
		this.splitContainerControl2.Panel2.Controls.Add(this.simpleButton16);
		this.splitContainerControl2.Panel2.Controls.Add(this.simpleButton17);
		this.splitContainerControl2.Panel2.Controls.Add(this.simpleButton12);
		this.splitContainerControl2.Panel2.Controls.Add(this.simpleButton13);
		this.splitContainerControl2.Panel2.Controls.Add(this.simpleButton14);
		this.splitContainerControl2.Panel2.Controls.Add(this.simpleButton9);
		this.splitContainerControl2.Panel2.Controls.Add(this.simpleButton10);
		this.splitContainerControl2.Panel2.Controls.Add(this.simpleButton11);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(1187, 278);
		this.splitContainerControl2.SplitterPosition = 265;
		this.splitContainerControl2.TabIndex = 69;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.groupControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.groupControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f);
		this.groupControl1.Appearance.Options.UseFont = true;
		this.groupControl1.Controls.Add(this.labelControl3);
		this.groupControl1.Controls.Add(this.labelControl2);
		this.groupControl1.Location = new System.Drawing.Point(2, 3);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(410, 202);
		this.groupControl1.TabIndex = 116;
		this.groupControl1.Text = "Customer Information";
		this.labelControl3.Location = new System.Drawing.Point(140, 32);
		this.labelControl3.Name = "labelControl3";
		this.labelControl3.Size = new System.Drawing.Size(60, 13);
		this.labelControl3.TabIndex = 1;
		this.labelControl3.Text = "1234567890";
		this.labelControl2.Location = new System.Drawing.Point(12, 32);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(40, 13);
		this.labelControl2.TabIndex = 0;
		this.labelControl2.Text = "Khách lẻ";
		this.labelLine4.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine4.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f);
		this.labelLine4.Appearance.Options.UseFont = true;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Tax";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(419, 42);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(122, 24);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 114;
		this.labelLine4.Utext = "Tax";
		this.labelLine3.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine3.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f);
		this.labelLine3.Appearance.Options.UseFont = true;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "TOTAL";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(419, 70);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(122, 24);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 113;
		this.labelLine3.Utext = "TOTAL";
		this.labelLine2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine2.Appearance.Font = new System.Drawing.Font("Tahoma", 10.25f);
		this.labelLine2.Appearance.Options.UseFont = true;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "CUSTOMER PAY";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(419, 98);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(122, 24);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 112;
		this.labelLine2.Utext = "CUSTOMER PAY";
		this.labelLine1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine1.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f, System.Drawing.FontStyle.Bold);
		this.labelLine1.Appearance.Options.UseFont = true;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Total Amount";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(419, 14);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(122, 24);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 111;
		this.labelLine1.Utext = "Total Amount";
		this.txtTotalAmount.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.txtTotalAmount.EditControl = null;
		this.txtTotalAmount.Enabled = false;
		this.txtTotalAmount.FieldName = null;
		this.txtTotalAmount.IsSearchControl = false;
		this.txtTotalAmount.Location = new System.Drawing.Point(554, 11);
		this.txtTotalAmount.Name = "txtTotalAmount";
		this.txtTotalAmount.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.txtTotalAmount.Properties.Appearance.Options.UseFont = true;
		this.txtTotalAmount.Properties.Appearance.Options.UseTextOptions = true;
		this.txtTotalAmount.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.txtTotalAmount.SettingInVisible = false;
		this.txtTotalAmount.SettingReadonly = false;
		this.txtTotalAmount.Size = new System.Drawing.Size(343, 26);
		this.txtTotalAmount.TabIndex = 110;
		this.txtTax.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.txtTax.EditControl = null;
		this.txtTax.Enabled = false;
		this.txtTax.FieldName = null;
		this.txtTax.IsSearchControl = false;
		this.txtTax.Location = new System.Drawing.Point(554, 38);
		this.txtTax.Name = "txtTax";
		this.txtTax.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.txtTax.Properties.Appearance.Options.UseFont = true;
		this.txtTax.Properties.Appearance.Options.UseTextOptions = true;
		this.txtTax.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.txtTax.SettingInVisible = false;
		this.txtTax.SettingReadonly = false;
		this.txtTax.Size = new System.Drawing.Size(343, 26);
		this.txtTax.TabIndex = 109;
		this.txtInvoiceTotal.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.txtInvoiceTotal.EditControl = null;
		this.txtInvoiceTotal.Enabled = false;
		this.txtInvoiceTotal.FieldName = null;
		this.txtInvoiceTotal.IsSearchControl = false;
		this.txtInvoiceTotal.Location = new System.Drawing.Point(554, 65);
		this.txtInvoiceTotal.Name = "txtInvoiceTotal";
		this.txtInvoiceTotal.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.txtInvoiceTotal.Properties.Appearance.Options.UseFont = true;
		this.txtInvoiceTotal.Properties.Appearance.Options.UseTextOptions = true;
		this.txtInvoiceTotal.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.txtInvoiceTotal.SettingInVisible = false;
		this.txtInvoiceTotal.SettingReadonly = false;
		this.txtInvoiceTotal.Size = new System.Drawing.Size(343, 26);
		this.txtInvoiceTotal.TabIndex = 108;
		this.txtPayment.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.txtPayment.EditControl = null;
		this.txtPayment.FieldName = null;
		this.txtPayment.IsSearchControl = false;
		this.txtPayment.Location = new System.Drawing.Point(554, 92);
		this.txtPayment.Name = "txtPayment";
		this.txtPayment.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12.25f);
		this.txtPayment.Properties.Appearance.Options.UseFont = true;
		this.txtPayment.Properties.Appearance.Options.UseTextOptions = true;
		this.txtPayment.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.txtPayment.SettingInVisible = false;
		this.txtPayment.SettingReadonly = false;
		this.txtPayment.Size = new System.Drawing.Size(343, 26);
		this.txtPayment.TabIndex = 107;
		this.txtPayment.EditValueChanged += new System.EventHandler(this.txtPayment_EditValueChanged);
		this.simpleButton21.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton21.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton21.Appearance.Options.UseFont = true;
		this.simpleButton21.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton21.EditControl = null;
		this.simpleButton21.EditValue = "Customer Information";
		this.simpleButton21.FieldName = null;
		this.simpleButton21.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton21.ImageOptions.Image");
		this.simpleButton21.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.simpleButton21.IsSearchControl = false;
		this.simpleButton21.Location = new System.Drawing.Point(832, 211);
		this.simpleButton21.Name = "simpleButton21";
		this.simpleButton21.ReadOnly = false;
		this.simpleButton21.SettingInVisible = false;
		this.simpleButton21.SettingReadonly = false;
		this.simpleButton21.Size = new System.Drawing.Size(65, 65);
		this.simpleButton21.TabIndex = 106;
		this.simpleButton21.Text = "Customer Information";
		this.simpleButton18.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton18.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton18.Appearance.Options.UseFont = true;
		this.simpleButton18.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton18.EditControl = null;
		this.simpleButton18.EditValue = "";
		this.simpleButton18.FieldName = null;
		this.simpleButton18.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton18.ImageOptions.Image");
		this.simpleButton18.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.simpleButton18.IsSearchControl = false;
		this.simpleButton18.Location = new System.Drawing.Point(763, 211);
		this.simpleButton18.Name = "simpleButton18";
		this.simpleButton18.ReadOnly = false;
		this.simpleButton18.SettingInVisible = false;
		this.simpleButton18.SettingReadonly = false;
		this.simpleButton18.Size = new System.Drawing.Size(65, 65);
		this.simpleButton18.TabIndex = 105;
		this.simpleButton19.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton19.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton19.Appearance.Options.UseFont = true;
		this.simpleButton19.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton19.EditControl = null;
		this.simpleButton19.EditValue = "";
		this.simpleButton19.FieldName = null;
		this.simpleButton19.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.simpleButton19.IsSearchControl = false;
		this.simpleButton19.Location = new System.Drawing.Point(694, 211);
		this.simpleButton19.Name = "simpleButton19";
		this.simpleButton19.ReadOnly = false;
		this.simpleButton19.SettingInVisible = false;
		this.simpleButton19.SettingReadonly = false;
		this.simpleButton19.Size = new System.Drawing.Size(65, 65);
		this.simpleButton19.TabIndex = 104;
		this.simpleButton20.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton20.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton20.Appearance.Options.UseFont = true;
		this.simpleButton20.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton20.EditControl = null;
		this.simpleButton20.EditValue = "";
		this.simpleButton20.FieldName = null;
		this.simpleButton20.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.simpleButton20.IsSearchControl = false;
		this.simpleButton20.Location = new System.Drawing.Point(623, 211);
		this.simpleButton20.Name = "simpleButton20";
		this.simpleButton20.ReadOnly = false;
		this.simpleButton20.SettingInVisible = false;
		this.simpleButton20.SettingReadonly = false;
		this.simpleButton20.Size = new System.Drawing.Size(65, 65);
		this.simpleButton20.TabIndex = 103;
		this.simpleButton15.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton15.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton15.Appearance.Options.UseFont = true;
		this.simpleButton15.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton15.EditControl = null;
		this.simpleButton15.EditValue = "";
		this.simpleButton15.FieldName = null;
		this.simpleButton15.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.simpleButton15.IsSearchControl = false;
		this.simpleButton15.Location = new System.Drawing.Point(554, 210);
		this.simpleButton15.Name = "simpleButton15";
		this.simpleButton15.ReadOnly = false;
		this.simpleButton15.SettingInVisible = false;
		this.simpleButton15.SettingReadonly = false;
		this.simpleButton15.Size = new System.Drawing.Size(65, 65);
		this.simpleButton15.TabIndex = 102;
		this.simpleButton16.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton16.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton16.Appearance.Options.UseFont = true;
		this.simpleButton16.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton16.EditControl = null;
		this.simpleButton16.EditValue = "";
		this.simpleButton16.FieldName = null;
		this.simpleButton16.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton16.ImageOptions.Image");
		this.simpleButton16.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.simpleButton16.IsSearchControl = false;
		this.simpleButton16.Location = new System.Drawing.Point(485, 211);
		this.simpleButton16.Name = "simpleButton16";
		this.simpleButton16.ReadOnly = false;
		this.simpleButton16.SettingInVisible = false;
		this.simpleButton16.SettingReadonly = false;
		this.simpleButton16.Size = new System.Drawing.Size(65, 65);
		this.simpleButton16.TabIndex = 101;
		this.simpleButton17.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton17.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton17.Appearance.Options.UseFont = true;
		this.simpleButton17.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton17.EditControl = null;
		this.simpleButton17.EditValue = "";
		this.simpleButton17.FieldName = null;
		this.simpleButton17.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton17.ImageOptions.Image");
		this.simpleButton17.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.simpleButton17.IsSearchControl = false;
		this.simpleButton17.Location = new System.Drawing.Point(416, 210);
		this.simpleButton17.Name = "simpleButton17";
		this.simpleButton17.ReadOnly = false;
		this.simpleButton17.SettingInVisible = false;
		this.simpleButton17.SettingReadonly = false;
		this.simpleButton17.Size = new System.Drawing.Size(65, 65);
		this.simpleButton17.TabIndex = 100;
		this.simpleButton12.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton12.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton12.Appearance.Options.UseFont = true;
		this.simpleButton12.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton12.EditControl = null;
		this.simpleButton12.EditValue = "";
		this.simpleButton12.FieldName = null;
		this.simpleButton12.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton12.ImageOptions.Image");
		this.simpleButton12.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.simpleButton12.IsSearchControl = false;
		this.simpleButton12.Location = new System.Drawing.Point(347, 211);
		this.simpleButton12.Name = "simpleButton12";
		this.simpleButton12.ReadOnly = false;
		this.simpleButton12.SettingInVisible = false;
		this.simpleButton12.SettingReadonly = false;
		this.simpleButton12.Size = new System.Drawing.Size(65, 65);
		this.simpleButton12.TabIndex = 99;
		this.simpleButton13.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton13.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton13.Appearance.Options.UseFont = true;
		this.simpleButton13.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton13.EditControl = null;
		this.simpleButton13.EditValue = "";
		this.simpleButton13.FieldName = null;
		this.simpleButton13.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton13.ImageOptions.Image");
		this.simpleButton13.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.simpleButton13.IsSearchControl = false;
		this.simpleButton13.Location = new System.Drawing.Point(278, 211);
		this.simpleButton13.Name = "simpleButton13";
		this.simpleButton13.ReadOnly = false;
		this.simpleButton13.SettingInVisible = false;
		this.simpleButton13.SettingReadonly = false;
		this.simpleButton13.Size = new System.Drawing.Size(65, 65);
		this.simpleButton13.TabIndex = 98;
		this.simpleButton14.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton14.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton14.Appearance.Options.UseFont = true;
		this.simpleButton14.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton14.EditControl = null;
		this.simpleButton14.EditValue = "";
		this.simpleButton14.FieldName = null;
		this.simpleButton14.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton14.ImageOptions.Image");
		this.simpleButton14.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.simpleButton14.IsSearchControl = false;
		this.simpleButton14.Location = new System.Drawing.Point(209, 211);
		this.simpleButton14.Name = "simpleButton14";
		this.simpleButton14.ReadOnly = false;
		this.simpleButton14.SettingInVisible = false;
		this.simpleButton14.SettingReadonly = false;
		this.simpleButton14.Size = new System.Drawing.Size(65, 65);
		this.simpleButton14.TabIndex = 97;
		this.simpleButton9.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton9.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton9.Appearance.Options.UseFont = true;
		this.simpleButton9.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton9.EditControl = null;
		this.simpleButton9.EditValue = "";
		this.simpleButton9.FieldName = null;
		this.simpleButton9.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton9.ImageOptions.Image");
		this.simpleButton9.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.simpleButton9.IsSearchControl = false;
		this.simpleButton9.Location = new System.Drawing.Point(140, 211);
		this.simpleButton9.Name = "simpleButton9";
		this.simpleButton9.ReadOnly = false;
		this.simpleButton9.SettingInVisible = false;
		this.simpleButton9.SettingReadonly = false;
		this.simpleButton9.Size = new System.Drawing.Size(65, 65);
		this.simpleButton9.TabIndex = 96;
		this.simpleButton10.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton10.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton10.Appearance.Options.UseFont = true;
		this.simpleButton10.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton10.EditControl = null;
		this.simpleButton10.EditValue = "";
		this.simpleButton10.FieldName = null;
		this.simpleButton10.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton10.ImageOptions.Image");
		this.simpleButton10.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.simpleButton10.IsSearchControl = false;
		this.simpleButton10.Location = new System.Drawing.Point(71, 211);
		this.simpleButton10.Name = "simpleButton10";
		this.simpleButton10.ReadOnly = false;
		this.simpleButton10.SettingInVisible = false;
		this.simpleButton10.SettingReadonly = false;
		this.simpleButton10.Size = new System.Drawing.Size(65, 65);
		this.simpleButton10.TabIndex = 95;
		this.simpleButton11.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton11.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.simpleButton11.Appearance.Options.UseFont = true;
		this.simpleButton11.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.simpleButton11.EditControl = null;
		this.simpleButton11.EditValue = "";
		this.simpleButton11.FieldName = null;
		this.simpleButton11.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton11.ImageOptions.Image");
		this.simpleButton11.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.simpleButton11.IsSearchControl = false;
		this.simpleButton11.Location = new System.Drawing.Point(2, 211);
		this.simpleButton11.Name = "simpleButton11";
		this.simpleButton11.ReadOnly = false;
		this.simpleButton11.SettingInVisible = false;
		this.simpleButton11.SettingReadonly = false;
		this.simpleButton11.Size = new System.Drawing.Size(65, 65);
		this.simpleButton11.TabIndex = 94;
		base.Appearance.BackColor = System.Drawing.Color.FromArgb(189, 208, 237);
		base.Appearance.BackColor2 = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1187, 780);
		base.Controls.Add(this.splitContainerControl2);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.panelControl1);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Retail";
		this.Text = "RETAIL";
		base.Load += new System.EventHandler(this.frm_Retail_Load);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		this.panelControl1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.txtBarCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		this.groupControl1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.txtTotalAmount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtTax.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtInvoiceTotal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPayment.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
