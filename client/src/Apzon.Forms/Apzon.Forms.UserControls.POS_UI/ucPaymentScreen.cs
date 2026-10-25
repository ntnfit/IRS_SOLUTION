using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucPaymentScreen : XtraUserControl
{
	public Action OnClose;

	public Action<double> NavigateToPayDialog;

	public double InvTotal;

	public ucPaymentDialog PaymentDialog;

	private IContainer components;

	private PanelControl panelControl1;

	private PanelControl panelControl2;

	private PanelControl panelControl5;

	private PanelControl panelControl4;

	private PanelControl panelControl3;

	private TableLayoutPanel tableLayoutPanel1;

	private TableLayoutPanel tableLayoutPanel2;

	private PanelControl panelControl6;

	private SimpleButton btn_Back;

	private SimpleButton btn_ReInput;

	private PanelControl panelControl8;

	private PanelControl panelControl7;

	private LabelControlEx labelControlEx1;

	private LabelControlEx labelControlEx3;

	private LabelControlEx labelControlEx5;

	private LabelControlEx labelControlEx8;

	private LabelControlEx labelControlEx10;

	private LabelControlEx labelControlEx11;

	private LabelControlEx labelControlEx12;

	private LabelControlEx labelControlEx13;

	public CalcEditEx calc_Exchange;

	public CalcEditEx calc_InvTotal;

	public CalcEditEx calc_QtyTotal;

	public CalcEditEx calc_TaxTotal;

	public CalcEditEx calc_ItemCodeQty;

	public ucPaymentScreen()
	{
		InitializeComponent();
		PaymentDialog = new ucPaymentDialog();
	}

	private void btn_Back_Click(object sender, EventArgs e)
	{
		OnClose?.Invoke();
	}

	public void SetValues(double itemTotal, double qtyTotal, double invTotal, double taxTotal)
	{
		try
		{
			calc_ItemCodeQty.EditValue = itemTotal;
			calc_QtyTotal.EditValue = qtyTotal;
			calc_InvTotal.EditValue = invTotal;
			InvTotal = invTotal;
			calc_TaxTotal.EditValue = taxTotal;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void CalcExchange(double cusPay)
	{
		calc_Exchange.EditValue = cusPay - InvTotal;
	}

	private void btn_ReInput_Click(object sender, EventArgs e)
	{
		Action dialogShown = () =>
		{
			PaymentDialog.SetValues(InvTotal);
		};
		RetailDialog.Show(PaymentDialog, dialogShown, null, "Payment");
		_ = 1;
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
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
		this.btn_Back = new DevExpress.XtraEditors.SimpleButton();
		this.btn_ReInput = new DevExpress.XtraEditors.SimpleButton();
		this.panelControl6 = new DevExpress.XtraEditors.PanelControl();
		this.panelControl8 = new DevExpress.XtraEditors.PanelControl();
		this.calc_Exchange = new Apzon.Controls.Editor.CalcEditEx();
		this.labelControlEx13 = new Apzon.Controls.Controls.LabelControlEx();
		this.panelControl7 = new DevExpress.XtraEditors.PanelControl();
		this.labelControlEx10 = new Apzon.Controls.Controls.LabelControlEx();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.calc_InvTotal = new Apzon.Controls.Editor.CalcEditEx();
		this.labelControlEx11 = new Apzon.Controls.Controls.LabelControlEx();
		this.labelControlEx1 = new Apzon.Controls.Controls.LabelControlEx();
		this.panelControl5 = new DevExpress.XtraEditors.PanelControl();
		this.calc_QtyTotal = new Apzon.Controls.Editor.CalcEditEx();
		this.labelControlEx8 = new Apzon.Controls.Controls.LabelControlEx();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		this.calc_TaxTotal = new Apzon.Controls.Editor.CalcEditEx();
		this.labelControlEx12 = new Apzon.Controls.Controls.LabelControlEx();
		this.labelControlEx3 = new Apzon.Controls.Controls.LabelControlEx();
		this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
		this.calc_ItemCodeQty = new Apzon.Controls.Editor.CalcEditEx();
		this.labelControlEx5 = new Apzon.Controls.Controls.LabelControlEx();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		this.tableLayoutPanel2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl6).BeginInit();
		this.panelControl6.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl8).BeginInit();
		this.panelControl8.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_Exchange.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl7).BeginInit();
		this.panelControl7.SuspendLayout();
		this.tableLayoutPanel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_InvTotal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl5).BeginInit();
		this.panelControl5.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_QtyTotal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_TaxTotal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl4).BeginInit();
		this.panelControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_ItemCodeQty.Properties).BeginInit();
		base.SuspendLayout();
		this.panelControl1.Appearance.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.panelControl1.Appearance.Options.UseBackColor = true;
		this.panelControl1.Controls.Add(this.tableLayoutPanel2);
		this.panelControl1.Controls.Add(this.tableLayoutPanel1);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.panelControl1.LookAndFeel.UseDefaultLookAndFeel = false;
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(600, 450);
		this.panelControl1.TabIndex = 0;
		this.tableLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tableLayoutPanel2.ColumnCount = 2;
		this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15f));
		this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85f));
		this.tableLayoutPanel2.Controls.Add(this.btn_Back, 0, 1);
		this.tableLayoutPanel2.Controls.Add(this.btn_ReInput, 0, 0);
		this.tableLayoutPanel2.Controls.Add(this.panelControl6, 1, 0);
		this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 342);
		this.tableLayoutPanel2.Name = "tableLayoutPanel2";
		this.tableLayoutPanel2.RowCount = 2;
		this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60f));
		this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40f));
		this.tableLayoutPanel2.Size = new System.Drawing.Size(600, 108);
		this.tableLayoutPanel2.TabIndex = 5;
		this.btn_Back.Appearance.BackColor = System.Drawing.Color.FromArgb(228, 108, 10);
		this.btn_Back.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_Back.Appearance.ForeColor = System.Drawing.Color.Yellow;
		this.btn_Back.Appearance.Options.UseBackColor = true;
		this.btn_Back.Appearance.Options.UseFont = true;
		this.btn_Back.Appearance.Options.UseForeColor = true;
		this.btn_Back.Appearance.Options.UseTextOptions = true;
		this.btn_Back.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.btn_Back.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_Back.Dock = System.Windows.Forms.DockStyle.Fill;
		this.btn_Back.Location = new System.Drawing.Point(3, 67);
		this.btn_Back.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.btn_Back.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Back.Name = "btn_Back";
		this.btn_Back.Size = new System.Drawing.Size(84, 38);
		this.btn_Back.TabIndex = 10;
		this.btn_Back.Text = "Quay lại";
		this.btn_Back.Click += new System.EventHandler(this.btn_Back_Click);
		this.btn_ReInput.Appearance.BackColor = System.Drawing.Color.FromArgb(0, 176, 80);
		this.btn_ReInput.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_ReInput.Appearance.ForeColor = System.Drawing.Color.Yellow;
		this.btn_ReInput.Appearance.Options.UseBackColor = true;
		this.btn_ReInput.Appearance.Options.UseFont = true;
		this.btn_ReInput.Appearance.Options.UseForeColor = true;
		this.btn_ReInput.Appearance.Options.UseTextOptions = true;
		this.btn_ReInput.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.btn_ReInput.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_ReInput.Dock = System.Windows.Forms.DockStyle.Fill;
		this.btn_ReInput.Location = new System.Drawing.Point(3, 3);
		this.btn_ReInput.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.btn_ReInput.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_ReInput.Name = "btn_ReInput";
		this.btn_ReInput.Size = new System.Drawing.Size(84, 58);
		this.btn_ReInput.TabIndex = 9;
		this.btn_ReInput.Text = "Nhập lại số tiền";
		this.btn_ReInput.Click += new System.EventHandler(this.btn_ReInput_Click);
		this.panelControl6.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 255, 204);
		this.panelControl6.Appearance.Options.UseBackColor = true;
		this.panelControl6.Controls.Add(this.panelControl8);
		this.panelControl6.Controls.Add(this.panelControl7);
		this.panelControl6.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl6.Location = new System.Drawing.Point(93, 3);
		this.panelControl6.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.panelControl6.LookAndFeel.UseDefaultLookAndFeel = false;
		this.panelControl6.Name = "panelControl6";
		this.tableLayoutPanel2.SetRowSpan(this.panelControl6, 2);
		this.panelControl6.Size = new System.Drawing.Size(504, 102);
		this.panelControl6.TabIndex = 0;
		this.panelControl8.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panelControl8.Appearance.BackColor = System.Drawing.Color.Yellow;
		this.panelControl8.Appearance.Options.UseBackColor = true;
		this.panelControl8.Controls.Add(this.calc_Exchange);
		this.panelControl8.Controls.Add(this.labelControlEx13);
		this.panelControl8.Location = new System.Drawing.Point(0, 34);
		this.panelControl8.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.panelControl8.LookAndFeel.UseDefaultLookAndFeel = false;
		this.panelControl8.Name = "panelControl8";
		this.panelControl8.Size = new System.Drawing.Size(501, 68);
		this.panelControl8.TabIndex = 4;
		this.calc_Exchange.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.calc_Exchange.FieldName = null;
		this.calc_Exchange.Location = new System.Drawing.Point(272, 26);
		this.calc_Exchange.Name = "calc_Exchange";
		this.calc_Exchange.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.calc_Exchange.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.calc_Exchange.Properties.Appearance.Options.UseBackColor = true;
		this.calc_Exchange.Properties.Appearance.Options.UseFont = true;
		this.calc_Exchange.Properties.AutoHeight = false;
		this.calc_Exchange.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.calc_Exchange.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.calc_Exchange.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.calc_Exchange.Properties.NullValuePrompt = "0";
		this.calc_Exchange.Properties.NullValuePromptShowForEmptyValue = true;
		this.calc_Exchange.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.calc_Exchange.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.calc_Exchange.Size = new System.Drawing.Size(150, 23);
		this.calc_Exchange.TabIndex = 7;
		this.labelControlEx13.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelControlEx13.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx13.Appearance.Options.UseFont = true;
		this.labelControlEx13.FieldName = "labelControlEx13";
		this.labelControlEx13.Location = new System.Drawing.Point(428, 26);
		this.labelControlEx13.Name = "labelControlEx13";
		this.labelControlEx13.Size = new System.Drawing.Size(42, 23);
		this.labelControlEx13.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx13.TabIndex = 6;
		this.labelControlEx13.Text = "VND";
		this.panelControl7.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panelControl7.Appearance.BackColor = System.Drawing.Color.FromArgb(247, 150, 70);
		this.panelControl7.Appearance.Options.UseBackColor = true;
		this.panelControl7.Controls.Add(this.labelControlEx10);
		this.panelControl7.Location = new System.Drawing.Point(0, 0);
		this.panelControl7.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.panelControl7.LookAndFeel.UseDefaultLookAndFeel = false;
		this.panelControl7.Name = "panelControl7";
		this.panelControl7.Size = new System.Drawing.Size(504, 33);
		this.panelControl7.TabIndex = 3;
		this.labelControlEx10.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx10.Appearance.Options.UseFont = true;
		this.labelControlEx10.FieldName = "labelControlEx10";
		this.labelControlEx10.Location = new System.Drawing.Point(6, 9);
		this.labelControlEx10.Name = "labelControlEx10";
		this.labelControlEx10.Size = new System.Drawing.Size(138, 18);
		this.labelControlEx10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx10.TabIndex = 5;
		this.labelControlEx10.Text = "Tiền trả lại khách :";
		this.tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tableLayoutPanel1.ColumnCount = 2;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.Controls.Add(this.panelControl2, 0, 0);
		this.tableLayoutPanel1.Controls.Add(this.panelControl5, 1, 2);
		this.tableLayoutPanel1.Controls.Add(this.panelControl3, 0, 1);
		this.tableLayoutPanel1.Controls.Add(this.panelControl4, 0, 2);
		this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.RowCount = 3;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30f));
		this.tableLayoutPanel1.Size = new System.Drawing.Size(600, 130);
		this.tableLayoutPanel1.TabIndex = 4;
		this.panelControl2.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 255, 204);
		this.panelControl2.Appearance.Options.UseBackColor = true;
		this.tableLayoutPanel1.SetColumnSpan(this.panelControl2, 2);
		this.panelControl2.Controls.Add(this.calc_InvTotal);
		this.panelControl2.Controls.Add(this.labelControlEx11);
		this.panelControl2.Controls.Add(this.labelControlEx1);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl2.Location = new System.Drawing.Point(3, 3);
		this.panelControl2.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.panelControl2.LookAndFeel.UseDefaultLookAndFeel = false;
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(594, 46);
		this.panelControl2.TabIndex = 0;
		this.calc_InvTotal.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.calc_InvTotal.FieldName = null;
		this.calc_InvTotal.Location = new System.Drawing.Point(300, 10);
		this.calc_InvTotal.Name = "calc_InvTotal";
		this.calc_InvTotal.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.calc_InvTotal.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.calc_InvTotal.Properties.Appearance.Options.UseBackColor = true;
		this.calc_InvTotal.Properties.Appearance.Options.UseFont = true;
		this.calc_InvTotal.Properties.AutoHeight = false;
		this.calc_InvTotal.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.calc_InvTotal.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.calc_InvTotal.Properties.DisplayFormat.FormatString = "n2";
		this.calc_InvTotal.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calc_InvTotal.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.calc_InvTotal.Properties.NullValuePrompt = "0";
		this.calc_InvTotal.Properties.NullValuePromptShowForEmptyValue = true;
		this.calc_InvTotal.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.calc_InvTotal.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.calc_InvTotal.Size = new System.Drawing.Size(212, 30);
		this.calc_InvTotal.TabIndex = 5;
		this.labelControlEx11.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelControlEx11.Appearance.Font = new System.Drawing.Font("Tahoma", 18f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx11.Appearance.Options.UseFont = true;
		this.labelControlEx11.FieldName = "labelControlEx11";
		this.labelControlEx11.Location = new System.Drawing.Point(518, 11);
		this.labelControlEx11.Name = "labelControlEx11";
		this.labelControlEx11.Size = new System.Drawing.Size(53, 29);
		this.labelControlEx11.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx11.TabIndex = 4;
		this.labelControlEx11.Text = "VND";
		this.labelControlEx1.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx1.Appearance.Options.UseFont = true;
		this.labelControlEx1.FieldName = "labelControlEx1";
		this.labelControlEx1.Location = new System.Drawing.Point(59, 8);
		this.labelControlEx1.Name = "labelControlEx1";
		this.labelControlEx1.Size = new System.Drawing.Size(61, 25);
		this.labelControlEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx1.TabIndex = 2;
		this.labelControlEx1.Text = "TỔNG";
		this.panelControl5.Appearance.BackColor = System.Drawing.Color.FromArgb(219, 238, 244);
		this.panelControl5.Appearance.Options.UseBackColor = true;
		this.panelControl5.Controls.Add(this.calc_QtyTotal);
		this.panelControl5.Controls.Add(this.labelControlEx8);
		this.panelControl5.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl5.Location = new System.Drawing.Point(303, 94);
		this.panelControl5.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.panelControl5.LookAndFeel.UseDefaultLookAndFeel = false;
		this.panelControl5.Name = "panelControl5";
		this.panelControl5.Size = new System.Drawing.Size(294, 33);
		this.panelControl5.TabIndex = 3;
		this.calc_QtyTotal.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.calc_QtyTotal.EditValue = new decimal(new int[4] { 12, 0, 0, 0 });
		this.calc_QtyTotal.FieldName = null;
		this.calc_QtyTotal.Location = new System.Drawing.Point(216, 3);
		this.calc_QtyTotal.Name = "calc_QtyTotal";
		this.calc_QtyTotal.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.calc_QtyTotal.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.calc_QtyTotal.Properties.Appearance.Options.UseBackColor = true;
		this.calc_QtyTotal.Properties.Appearance.Options.UseFont = true;
		this.calc_QtyTotal.Properties.AutoHeight = false;
		this.calc_QtyTotal.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.calc_QtyTotal.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.calc_QtyTotal.Properties.DisplayFormat.FormatString = "n0";
		this.calc_QtyTotal.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calc_QtyTotal.Properties.NullValuePrompt = "0";
		this.calc_QtyTotal.Properties.NullValuePromptShowForEmptyValue = true;
		this.calc_QtyTotal.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.calc_QtyTotal.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.calc_QtyTotal.Size = new System.Drawing.Size(72, 23);
		this.calc_QtyTotal.TabIndex = 8;
		this.labelControlEx8.Appearance.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx8.Appearance.Options.UseFont = true;
		this.labelControlEx8.FieldName = "labelControlEx8";
		this.labelControlEx8.Location = new System.Drawing.Point(6, 8);
		this.labelControlEx8.Name = "labelControlEx8";
		this.labelControlEx8.Size = new System.Drawing.Size(126, 19);
		this.labelControlEx8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx8.TabIndex = 6;
		this.labelControlEx8.Text = "Tổng số lượng :";
		this.panelControl3.Appearance.BackColor = System.Drawing.Color.FromArgb(252, 213, 181);
		this.panelControl3.Appearance.Options.UseBackColor = true;
		this.tableLayoutPanel1.SetColumnSpan(this.panelControl3, 2);
		this.panelControl3.Controls.Add(this.calc_TaxTotal);
		this.panelControl3.Controls.Add(this.labelControlEx12);
		this.panelControl3.Controls.Add(this.labelControlEx3);
		this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl3.Location = new System.Drawing.Point(3, 55);
		this.panelControl3.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.panelControl3.LookAndFeel.UseDefaultLookAndFeel = false;
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(594, 33);
		this.panelControl3.TabIndex = 1;
		this.calc_TaxTotal.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.calc_TaxTotal.FieldName = null;
		this.calc_TaxTotal.Location = new System.Drawing.Point(362, 2);
		this.calc_TaxTotal.Name = "calc_TaxTotal";
		this.calc_TaxTotal.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.calc_TaxTotal.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.calc_TaxTotal.Properties.Appearance.Options.UseBackColor = true;
		this.calc_TaxTotal.Properties.Appearance.Options.UseFont = true;
		this.calc_TaxTotal.Properties.AutoHeight = false;
		this.calc_TaxTotal.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.calc_TaxTotal.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.calc_TaxTotal.Properties.DisplayFormat.FormatString = "n2";
		this.calc_TaxTotal.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calc_TaxTotal.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.calc_TaxTotal.Properties.NullValuePrompt = "0";
		this.calc_TaxTotal.Properties.NullValuePromptShowForEmptyValue = true;
		this.calc_TaxTotal.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.calc_TaxTotal.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.calc_TaxTotal.Size = new System.Drawing.Size(150, 23);
		this.calc_TaxTotal.TabIndex = 6;
		this.labelControlEx12.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelControlEx12.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx12.Appearance.Options.UseFont = true;
		this.labelControlEx12.FieldName = "labelControlEx12";
		this.labelControlEx12.Location = new System.Drawing.Point(518, 4);
		this.labelControlEx12.Name = "labelControlEx12";
		this.labelControlEx12.Size = new System.Drawing.Size(42, 23);
		this.labelControlEx12.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx12.TabIndex = 5;
		this.labelControlEx12.Text = "VND";
		this.labelControlEx3.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx3.Appearance.Options.UseFont = true;
		this.labelControlEx3.FieldName = "labelControlEx3";
		this.labelControlEx3.Location = new System.Drawing.Point(59, 4);
		this.labelControlEx3.Name = "labelControlEx3";
		this.labelControlEx3.Size = new System.Drawing.Size(47, 23);
		this.labelControlEx3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx3.TabIndex = 3;
		this.labelControlEx3.Text = "Thuế";
		this.panelControl4.Appearance.BackColor = System.Drawing.Color.FromArgb(204, 193, 218);
		this.panelControl4.Appearance.Options.UseBackColor = true;
		this.panelControl4.Controls.Add(this.calc_ItemCodeQty);
		this.panelControl4.Controls.Add(this.labelControlEx5);
		this.panelControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl4.Location = new System.Drawing.Point(3, 94);
		this.panelControl4.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.panelControl4.LookAndFeel.UseDefaultLookAndFeel = false;
		this.panelControl4.Name = "panelControl4";
		this.panelControl4.Size = new System.Drawing.Size(294, 33);
		this.panelControl4.TabIndex = 2;
		this.calc_ItemCodeQty.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.calc_ItemCodeQty.EditValue = new decimal(new int[4] { 3, 0, 0, 0 });
		this.calc_ItemCodeQty.FieldName = null;
		this.calc_ItemCodeQty.Location = new System.Drawing.Point(216, 3);
		this.calc_ItemCodeQty.Name = "calc_ItemCodeQty";
		this.calc_ItemCodeQty.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.calc_ItemCodeQty.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.calc_ItemCodeQty.Properties.Appearance.Options.UseBackColor = true;
		this.calc_ItemCodeQty.Properties.Appearance.Options.UseFont = true;
		this.calc_ItemCodeQty.Properties.AutoHeight = false;
		this.calc_ItemCodeQty.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.calc_ItemCodeQty.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.calc_ItemCodeQty.Properties.DisplayFormat.FormatString = "n0";
		this.calc_ItemCodeQty.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calc_ItemCodeQty.Properties.NullValuePrompt = "0";
		this.calc_ItemCodeQty.Properties.NullValuePromptShowForEmptyValue = true;
		this.calc_ItemCodeQty.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.calc_ItemCodeQty.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.calc_ItemCodeQty.Size = new System.Drawing.Size(72, 23);
		this.calc_ItemCodeQty.TabIndex = 6;
		this.labelControlEx5.Appearance.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx5.Appearance.Options.UseFont = true;
		this.labelControlEx5.FieldName = "labelControlEx5";
		this.labelControlEx5.Location = new System.Drawing.Point(6, 6);
		this.labelControlEx5.Name = "labelControlEx5";
		this.labelControlEx5.Size = new System.Drawing.Size(150, 19);
		this.labelControlEx5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx5.TabIndex = 4;
		this.labelControlEx5.Text = "Tổng số mã hàng :";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.panelControl1);
		base.Name = "ucPaymentScreen";
		base.Size = new System.Drawing.Size(600, 450);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		this.tableLayoutPanel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl6).EndInit();
		this.panelControl6.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl8).EndInit();
		this.panelControl8.ResumeLayout(false);
		this.panelControl8.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_Exchange.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl7).EndInit();
		this.panelControl7.ResumeLayout(false);
		this.panelControl7.PerformLayout();
		this.tableLayoutPanel1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		this.panelControl2.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_InvTotal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl5).EndInit();
		this.panelControl5.ResumeLayout(false);
		this.panelControl5.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_QtyTotal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		this.panelControl3.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_TaxTotal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl4).EndInit();
		this.panelControl4.ResumeLayout(false);
		this.panelControl4.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_ItemCodeQty.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
