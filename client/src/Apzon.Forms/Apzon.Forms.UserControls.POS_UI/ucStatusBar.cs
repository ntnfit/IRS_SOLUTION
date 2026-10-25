using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons.Enums;
using DevExpress.XtraEditors;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucStatusBar : XtraUserControl
{
	private IContainer components;

	private TableLayoutPanel tableLayoutPanel1;

	public LabelControlResizable lblCustomer;

	public ucStatusBar()
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
		this.lblCustomer = new Apzon.Forms.UserControls.POS_UI.LabelControlResizable();
		this.tableLayoutPanel1.SuspendLayout();
		base.SuspendLayout();
		this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(219, 238, 244);
		this.tableLayoutPanel1.ColumnCount = 4;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25f));
		this.tableLayoutPanel1.Controls.Add(this.lblCustomer, 0, 0);
		this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.RowCount = 1;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 69f));
		this.tableLayoutPanel1.Size = new System.Drawing.Size(961, 69);
		this.tableLayoutPanel1.TabIndex = 5;
		this.lblCustomer.Appearance.Font = new System.Drawing.Font("Tahoma", 24.74675f);
		this.lblCustomer.Appearance.ForeColor = System.Drawing.Color.Black;
		this.lblCustomer.Appearance.Options.UseFont = true;
		this.lblCustomer.Appearance.Options.UseForeColor = true;
		this.tableLayoutPanel1.SetColumnSpan(this.lblCustomer, 4);
		this.lblCustomer.Dock = System.Windows.Forms.DockStyle.Fill;
		this.lblCustomer.FieldName = "lblUser";
		this.lblCustomer.Location = new System.Drawing.Point(3, 3);
		this.lblCustomer.LongestString = "Khách hàng: Công ty cổ phần giải pháp và truyền thông Apzon - Đối tác vàng SAP";
		this.lblCustomer.Name = "lblCustomer";
		this.lblCustomer.Size = new System.Drawing.Size(955, 63);
		this.lblCustomer.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblCustomer.TabIndex = 5;
		this.lblCustomer.Text = "Khách hàng : ";
		base.Appearance.BackColor = System.Drawing.SystemColors.Control;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tableLayoutPanel1);
		base.Name = "ucStatusBar";
		base.Size = new System.Drawing.Size(961, 69);
		this.tableLayoutPanel1.ResumeLayout(false);
		this.tableLayoutPanel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
