using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucPaymentTotal : XtraUserControl
{
	private IContainer components;

	public TableLayoutPanel tableLayoutPanel1;

	public TableLayoutPanel tableLayoutPanel2;

	public LabelControlResizable labelControlEx1;

	public LabelControlResizable labelControlEx3;

	public LabelControlResizable labelControlEx2;

	public CalcEditResizable lblSubTotal;

	public CalcEditResizable lblTaxAmount;

	public CalcEditResizable lblDiscountAmount;

	public CalcEditResizable lblInvoiceTotal;

	public void SetText(Tuple<double, double, double, double> value)
	{
		try
		{
			lblSubTotal.EditValue = Function.ParseDouble(value.Item1);
			lblInvoiceTotal.EditValue = Function.ParseDouble(value.Item2);
			lblTaxAmount.EditValue = Function.ParseDouble(value.Item3);
			lblDiscountAmount.EditValue = Function.ParseDouble(value.Item4);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public ucPaymentTotal()
	{
		InitializeComponent();
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
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.lblInvoiceTotal = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
		this.labelControlEx1 = new Apzon.Forms.UserControls.POS_UI.LabelControlResizable();
		this.labelControlEx3 = new Apzon.Forms.UserControls.POS_UI.LabelControlResizable();
		this.labelControlEx2 = new Apzon.Forms.UserControls.POS_UI.LabelControlResizable();
		this.lblSubTotal = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.lblTaxAmount = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.lblDiscountAmount = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.tableLayoutPanel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lblInvoiceTotal.Properties).BeginInit();
		this.tableLayoutPanel2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lblSubTotal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lblTaxAmount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lblDiscountAmount.Properties).BeginInit();
		base.SuspendLayout();
		this.tableLayoutPanel1.ColumnCount = 2;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.Controls.Add(this.lblInvoiceTotal, 0, 0);
		this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
		this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.RowCount = 1;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 379f));
		this.tableLayoutPanel1.Size = new System.Drawing.Size(511, 112);
		this.tableLayoutPanel1.TabIndex = 2;
		this.lblInvoiceTotal.Dock = System.Windows.Forms.DockStyle.Fill;
		this.lblInvoiceTotal.FieldName = null;
		this.lblInvoiceTotal.Location = new System.Drawing.Point(258, 3);
		this.lblInvoiceTotal.Name = "lblInvoiceTotal";
		this.lblInvoiceTotal.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(31, 73, 125);
		this.lblInvoiceTotal.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 22.85896f, System.Drawing.FontStyle.Bold);
		this.lblInvoiceTotal.Properties.Appearance.ForeColor = System.Drawing.Color.Yellow;
		this.lblInvoiceTotal.Properties.Appearance.Options.UseBackColor = true;
		this.lblInvoiceTotal.Properties.Appearance.Options.UseFont = true;
		this.lblInvoiceTotal.Properties.Appearance.Options.UseForeColor = true;
		this.lblInvoiceTotal.Properties.AutoHeight = false;
		this.lblInvoiceTotal.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblInvoiceTotal.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblInvoiceTotal.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.lblInvoiceTotal.Properties.NullValuePrompt = "0";
		this.lblInvoiceTotal.Properties.NullValuePromptShowForEmptyValue = true;
		this.lblInvoiceTotal.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.lblInvoiceTotal.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.lblInvoiceTotal.Size = new System.Drawing.Size(250, 106);
		this.lblInvoiceTotal.TabIndex = 4;
		this.tableLayoutPanel2.BackColor = System.Drawing.Color.FromArgb(250, 192, 144);
		this.tableLayoutPanel2.ColumnCount = 2;
		this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel2.Controls.Add(this.labelControlEx1, 0, 0);
		this.tableLayoutPanel2.Controls.Add(this.labelControlEx3, 0, 2);
		this.tableLayoutPanel2.Controls.Add(this.labelControlEx2, 0, 1);
		this.tableLayoutPanel2.Controls.Add(this.lblSubTotal, 1, 0);
		this.tableLayoutPanel2.Controls.Add(this.lblTaxAmount, 1, 1);
		this.tableLayoutPanel2.Controls.Add(this.lblDiscountAmount, 1, 2);
		this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
		this.tableLayoutPanel2.Name = "tableLayoutPanel2";
		this.tableLayoutPanel2.RowCount = 3;
		this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333f));
		this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333f));
		this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333f));
		this.tableLayoutPanel2.Size = new System.Drawing.Size(249, 106);
		this.tableLayoutPanel2.TabIndex = 2;
		this.labelControlEx1.Appearance.Font = new System.Drawing.Font("Tahoma", 12.49412f);
		this.labelControlEx1.Appearance.Options.UseFont = true;
		this.labelControlEx1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelControlEx1.FieldName = "labelControlEx1";
		this.labelControlEx1.Location = new System.Drawing.Point(3, 3);
		this.labelControlEx1.LongestString = "100000000000";
		this.labelControlEx1.Name = "labelControlEx1";
		this.labelControlEx1.Size = new System.Drawing.Size(118, 29);
		this.labelControlEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx1.TabIndex = 0;
		this.labelControlEx1.Text = "Tiền hàng";
		this.labelControlEx3.Appearance.Font = new System.Drawing.Font("Tahoma", 12.49412f);
		this.labelControlEx3.Appearance.Options.UseFont = true;
		this.labelControlEx3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelControlEx3.FieldName = "labelControlEx3";
		this.labelControlEx3.Location = new System.Drawing.Point(3, 73);
		this.labelControlEx3.LongestString = "100000000000";
		this.labelControlEx3.Name = "labelControlEx3";
		this.labelControlEx3.Size = new System.Drawing.Size(118, 30);
		this.labelControlEx3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx3.TabIndex = 0;
		this.labelControlEx3.Text = "Chiết khấu";
		this.labelControlEx2.Appearance.Font = new System.Drawing.Font("Tahoma", 12.49412f);
		this.labelControlEx2.Appearance.Options.UseFont = true;
		this.labelControlEx2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelControlEx2.FieldName = "labelControlEx2";
		this.labelControlEx2.Location = new System.Drawing.Point(3, 38);
		this.labelControlEx2.LongestString = "100000000000";
		this.labelControlEx2.Name = "labelControlEx2";
		this.labelControlEx2.Size = new System.Drawing.Size(118, 29);
		this.labelControlEx2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx2.TabIndex = 0;
		this.labelControlEx2.Text = "Thuế";
		this.lblSubTotal.Dock = System.Windows.Forms.DockStyle.Fill;
		this.lblSubTotal.FieldName = null;
		this.lblSubTotal.Location = new System.Drawing.Point(127, 3);
		this.lblSubTotal.Name = "lblSubTotal";
		this.lblSubTotal.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.lblSubTotal.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12.6f);
		this.lblSubTotal.Properties.Appearance.Options.UseBackColor = true;
		this.lblSubTotal.Properties.Appearance.Options.UseFont = true;
		this.lblSubTotal.Properties.AutoHeight = false;
		this.lblSubTotal.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblSubTotal.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblSubTotal.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.lblSubTotal.Properties.NullValuePrompt = "0";
		this.lblSubTotal.Properties.NullValuePromptShowForEmptyValue = true;
		this.lblSubTotal.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.lblSubTotal.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.lblSubTotal.Size = new System.Drawing.Size(119, 29);
		this.lblSubTotal.TabIndex = 1;
		this.lblTaxAmount.Dock = System.Windows.Forms.DockStyle.Fill;
		this.lblTaxAmount.FieldName = null;
		this.lblTaxAmount.Location = new System.Drawing.Point(127, 38);
		this.lblTaxAmount.Name = "lblTaxAmount";
		this.lblTaxAmount.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.lblTaxAmount.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12.6f);
		this.lblTaxAmount.Properties.Appearance.Options.UseBackColor = true;
		this.lblTaxAmount.Properties.Appearance.Options.UseFont = true;
		this.lblTaxAmount.Properties.AutoHeight = false;
		this.lblTaxAmount.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblTaxAmount.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblTaxAmount.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.lblTaxAmount.Properties.NullValuePrompt = "0";
		this.lblTaxAmount.Properties.NullValuePromptShowForEmptyValue = true;
		this.lblTaxAmount.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.lblTaxAmount.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.lblTaxAmount.Size = new System.Drawing.Size(119, 29);
		this.lblTaxAmount.TabIndex = 2;
		this.lblDiscountAmount.Dock = System.Windows.Forms.DockStyle.Fill;
		this.lblDiscountAmount.FieldName = null;
		this.lblDiscountAmount.Location = new System.Drawing.Point(127, 73);
		this.lblDiscountAmount.Name = "lblDiscountAmount";
		this.lblDiscountAmount.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.lblDiscountAmount.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12.6f);
		this.lblDiscountAmount.Properties.Appearance.Options.UseBackColor = true;
		this.lblDiscountAmount.Properties.Appearance.Options.UseFont = true;
		this.lblDiscountAmount.Properties.AutoHeight = false;
		this.lblDiscountAmount.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblDiscountAmount.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lblDiscountAmount.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.lblDiscountAmount.Properties.NullValuePrompt = "0";
		this.lblDiscountAmount.Properties.NullValuePromptShowForEmptyValue = true;
		this.lblDiscountAmount.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.lblDiscountAmount.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.lblDiscountAmount.Size = new System.Drawing.Size(119, 30);
		this.lblDiscountAmount.TabIndex = 3;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tableLayoutPanel1);
		base.Name = "ucPaymentTotal";
		base.Size = new System.Drawing.Size(511, 112);
		this.tableLayoutPanel1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.lblInvoiceTotal.Properties).EndInit();
		this.tableLayoutPanel2.ResumeLayout(false);
		this.tableLayoutPanel2.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.lblSubTotal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lblTaxAmount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lblDiscountAmount.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
