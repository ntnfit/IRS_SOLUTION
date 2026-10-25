using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons.Enums;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucCustomerPay : XtraUserControl
{
	private IContainer components;

	private LabelControlEx labelScanned;

	private TableLayoutPanel tableLayoutPanel1;

	private LabelControlEx labelControlEx1;

	public CalcEditEx calc_InvTotal;

	public ucCustomerPay()
	{
		InitializeComponent();
	}

	public void SetCusPayValue(double amount)
	{
		calc_InvTotal.EditValue = amount;
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
		this.labelScanned = new Apzon.Controls.Controls.LabelControlEx();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.calc_InvTotal = new Apzon.Controls.Editor.CalcEditEx();
		this.labelControlEx1 = new Apzon.Controls.Controls.LabelControlEx();
		this.tableLayoutPanel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_InvTotal.Properties).BeginInit();
		base.SuspendLayout();
		this.labelScanned.Appearance.Font = new System.Drawing.Font("Tahoma", 11.88764f);
		this.labelScanned.Appearance.ForeColor = System.Drawing.Color.FromArgb(1, 255, 0);
		this.labelScanned.Appearance.Options.UseFont = true;
		this.labelScanned.Appearance.Options.UseForeColor = true;
		this.labelScanned.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelScanned.FieldName = "labelScanned";
		this.labelScanned.Location = new System.Drawing.Point(3, 3);
		this.labelScanned.Name = "labelScanned";
		this.labelScanned.Size = new System.Drawing.Size(219, 88);
		this.labelScanned.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelScanned.TabIndex = 0;
		this.labelScanned.Text = "TỔNG TIỀN KHÁCH TRẢ";
		this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(17, 45, 53);
		this.tableLayoutPanel1.ColumnCount = 3;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15f));
		this.tableLayoutPanel1.Controls.Add(this.calc_InvTotal, 0, 0);
		this.tableLayoutPanel1.Controls.Add(this.labelControlEx1, 2, 0);
		this.tableLayoutPanel1.Controls.Add(this.labelScanned, 0, 0);
		this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.RowCount = 1;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tableLayoutPanel1.Size = new System.Drawing.Size(563, 94);
		this.tableLayoutPanel1.TabIndex = 2;
		this.calc_InvTotal.Dock = System.Windows.Forms.DockStyle.Fill;
		this.calc_InvTotal.FieldName = null;
		this.calc_InvTotal.Location = new System.Drawing.Point(228, 3);
		this.calc_InvTotal.Name = "calc_InvTotal";
		this.calc_InvTotal.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.calc_InvTotal.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 20.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.calc_InvTotal.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(1, 255, 0);
		this.calc_InvTotal.Properties.Appearance.Options.UseBackColor = true;
		this.calc_InvTotal.Properties.Appearance.Options.UseFont = true;
		this.calc_InvTotal.Properties.Appearance.Options.UseForeColor = true;
		this.calc_InvTotal.Properties.AutoHeight = false;
		this.calc_InvTotal.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.calc_InvTotal.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.calc_InvTotal.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.calc_InvTotal.Properties.NullValuePrompt = "0";
		this.calc_InvTotal.Properties.NullValuePromptShowForEmptyValue = true;
		this.calc_InvTotal.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.calc_InvTotal.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.calc_InvTotal.Size = new System.Drawing.Size(247, 88);
		this.calc_InvTotal.TabIndex = 6;
		this.labelControlEx1.Appearance.Font = new System.Drawing.Font("Tahoma", 18.69192f, System.Drawing.FontStyle.Bold);
		this.labelControlEx1.Appearance.ForeColor = System.Drawing.Color.FromArgb(1, 255, 0);
		this.labelControlEx1.Appearance.Options.UseFont = true;
		this.labelControlEx1.Appearance.Options.UseForeColor = true;
		this.labelControlEx1.Appearance.Options.UseTextOptions = true;
		this.labelControlEx1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.labelControlEx1.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.labelControlEx1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelControlEx1.FieldName = "labelControlEx1";
		this.labelControlEx1.Location = new System.Drawing.Point(481, 3);
		this.labelControlEx1.Name = "labelControlEx1";
		this.labelControlEx1.Size = new System.Drawing.Size(79, 88);
		this.labelControlEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx1.TabIndex = 4;
		this.labelControlEx1.Text = "VND";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tableLayoutPanel1);
		base.Name = "ucCustomerPay";
		base.Size = new System.Drawing.Size(563, 94);
		this.tableLayoutPanel1.ResumeLayout(false);
		this.tableLayoutPanel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_InvTotal.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
