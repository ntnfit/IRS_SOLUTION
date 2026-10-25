using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using DevExpress.XtraEditors;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucScanItem : XtraUserControl
{
	private IContainer components;

	private LabelControlEx lblScannedVal;

	private LabelControlEx lblQtyVal;

	private LabelControlEx labelPow;

	private TableLayoutPanel tableLayoutPanel1;

	public ucScanItem()
	{
		InitializeComponent();
	}

	public void SetText(string par1, double par2)
	{
		try
		{
			lblScannedVal.Text = Function.ToString(par1);
			lblQtyVal.Text = Function.ToString(par2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
		this.lblQtyVal = new Apzon.Controls.Controls.LabelControlEx();
		this.labelPow = new Apzon.Controls.Controls.LabelControlEx();
		this.lblScannedVal = new Apzon.Controls.Controls.LabelControlEx();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.tableLayoutPanel1.SuspendLayout();
		base.SuspendLayout();
		this.lblQtyVal.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.lblQtyVal.Appearance.ForeColor = System.Drawing.Color.White;
		this.lblQtyVal.Appearance.Options.UseFont = true;
		this.lblQtyVal.Appearance.Options.UseForeColor = true;
		this.lblQtyVal.Dock = System.Windows.Forms.DockStyle.Fill;
		this.lblQtyVal.FieldName = "lblQtyVal";
		this.lblQtyVal.Location = new System.Drawing.Point(228, 50);
		this.lblQtyVal.Name = "lblQtyVal";
		this.lblQtyVal.Size = new System.Drawing.Size(332, 41);
		this.lblQtyVal.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblQtyVal.TabIndex = 3;
		this.lblQtyVal.Text = "1";
		this.labelPow.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelPow.Appearance.ForeColor = System.Drawing.Color.White;
		this.labelPow.Appearance.Options.UseFont = true;
		this.labelPow.Appearance.Options.UseForeColor = true;
		this.labelPow.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelPow.FieldName = "labelPow";
		this.labelPow.Location = new System.Drawing.Point(3, 50);
		this.labelPow.Name = "labelPow";
		this.labelPow.Size = new System.Drawing.Size(219, 41);
		this.labelPow.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelPow.TabIndex = 2;
		this.labelPow.Text = "1X";
		this.lblScannedVal.Appearance.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.lblScannedVal.Appearance.ForeColor = System.Drawing.Color.FromArgb(1, 255, 0);
		this.lblScannedVal.Appearance.Options.UseFont = true;
		this.lblScannedVal.Appearance.Options.UseForeColor = true;
		this.tableLayoutPanel1.SetColumnSpan(this.lblScannedVal, 2);
		this.lblScannedVal.Dock = System.Windows.Forms.DockStyle.Fill;
		this.lblScannedVal.FieldName = "lblScannedVal";
		this.lblScannedVal.Location = new System.Drawing.Point(3, 3);
		this.lblScannedVal.Name = "lblScannedVal";
		this.lblScannedVal.Size = new System.Drawing.Size(557, 41);
		this.lblScannedVal.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblScannedVal.TabIndex = 1;
		this.lblScannedVal.Text = "No Items";
		this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(17, 45, 53);
		this.tableLayoutPanel1.ColumnCount = 2;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60f));
		this.tableLayoutPanel1.Controls.Add(this.lblQtyVal, 1, 1);
		this.tableLayoutPanel1.Controls.Add(this.labelPow, 0, 1);
		this.tableLayoutPanel1.Controls.Add(this.lblScannedVal, 0, 0);
		this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.RowCount = 2;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.Size = new System.Drawing.Size(563, 94);
		this.tableLayoutPanel1.TabIndex = 1;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tableLayoutPanel1);
		base.Name = "ucScanItem";
		base.Size = new System.Drawing.Size(563, 94);
		this.tableLayoutPanel1.ResumeLayout(false);
		this.tableLayoutPanel1.PerformLayout();
		base.ResumeLayout(false);
	}
}
