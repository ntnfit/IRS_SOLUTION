using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Rows;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucMemberDetail : XtraUserControl
{
	public Action ChooseMember;

	private IContainer components;

	public TableLayoutPanel tableLayoutPanel1;

	public VGridControl vGrid_Content;

	private EditorRow row;

	private EditorRow row1;

	private EditorRow row2;

	private EditorRow row3;

	private EditorRow row4;

	private SimpleButton btnOk;

	private SimpleButton btnCancel;

	private LabelControlEx labelControlEx1;

	public ucMemberDetail()
	{
		InitializeComponent();
		Load += UcMemberDetail_Load;
	}

	private void UcMemberDetail_Load(object sender, EventArgs e)
	{
	}

	public void AddCustomerToTable(DataRow dr)
	{
		try
		{
			DataTable dataTable = dr.Table.Clone();
			DataRow dataRow = dataTable.NewRow();
			dataRow.ItemArray = dr.ItemArray.Clone() as object[];
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			vGrid_Content.DataSource = dataTable;
			vGrid_Content.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnOk_Click(object sender, EventArgs e)
	{
		try
		{
			ChooseMember?.Invoke();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		try
		{
			ChooseMember?.Invoke();
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
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.vGrid_Content = new DevExpress.XtraVerticalGrid.VGridControl();
		this.row = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row1 = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row2 = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row3 = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.row4 = new DevExpress.XtraVerticalGrid.Rows.EditorRow();
		this.btnOk = new DevExpress.XtraEditors.SimpleButton();
		this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
		this.labelControlEx1 = new Apzon.Controls.Controls.LabelControlEx();
		this.tableLayoutPanel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.vGrid_Content).BeginInit();
		base.SuspendLayout();
		this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(215, 228, 189);
		this.tableLayoutPanel1.ColumnCount = 2;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.Controls.Add(this.vGrid_Content, 0, 1);
		this.tableLayoutPanel1.Controls.Add(this.btnOk, 0, 2);
		this.tableLayoutPanel1.Controls.Add(this.btnCancel, 1, 2);
		this.tableLayoutPanel1.Controls.Add(this.labelControlEx1, 0, 0);
		this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.RowCount = 3;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20f));
		this.tableLayoutPanel1.Size = new System.Drawing.Size(600, 450);
		this.tableLayoutPanel1.TabIndex = 15;
		this.vGrid_Content.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.vGrid_Content.Appearance.Empty.BackColor = System.Drawing.Color.FromArgb(215, 228, 189);
		this.vGrid_Content.Appearance.Empty.Options.UseBackColor = true;
		this.vGrid_Content.Appearance.ModifiedRecordValue.Font = new System.Drawing.Font("Tahoma", 25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.vGrid_Content.Appearance.ModifiedRecordValue.Options.UseFont = true;
		this.vGrid_Content.Appearance.RecordValue.BackColor = System.Drawing.Color.FromArgb(255, 255, 204);
		this.vGrid_Content.Appearance.RecordValue.Font = new System.Drawing.Font("Tahoma", 20f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.vGrid_Content.Appearance.RecordValue.Options.UseBackColor = true;
		this.vGrid_Content.Appearance.RecordValue.Options.UseFont = true;
		this.vGrid_Content.Appearance.RowHeaderPanel.BackColor = System.Drawing.Color.FromArgb(215, 228, 189);
		this.vGrid_Content.Appearance.RowHeaderPanel.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.vGrid_Content.Appearance.RowHeaderPanel.Options.UseBackColor = true;
		this.vGrid_Content.Appearance.RowHeaderPanel.Options.UseFont = true;
		this.vGrid_Content.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.tableLayoutPanel1.SetColumnSpan(this.vGrid_Content, 2);
		this.vGrid_Content.Cursor = System.Windows.Forms.Cursors.Default;
		this.vGrid_Content.Font = new System.Drawing.Font("Tahoma", 25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.vGrid_Content.LayoutStyle = DevExpress.XtraVerticalGrid.LayoutViewStyle.SingleRecordView;
		this.vGrid_Content.Location = new System.Drawing.Point(3, 93);
		this.vGrid_Content.Name = "vGrid_Content";
		this.vGrid_Content.OptionsBehavior.ResizeHeaderPanel = false;
		this.vGrid_Content.OptionsView.MaxRowAutoHeight = 30;
		this.vGrid_Content.OptionsView.ShowRootLevelIndent = false;
		this.vGrid_Content.RecordWidth = 161;
		this.vGrid_Content.RowHeaderWidth = 39;
		this.vGrid_Content.Rows.AddRange(new DevExpress.XtraVerticalGrid.Rows.BaseRow[5] { this.row, this.row1, this.row2, this.row3, this.row4 });
		this.vGrid_Content.Size = new System.Drawing.Size(594, 167);
		this.vGrid_Content.TabIndex = 11;
		this.row.Height = 40;
		this.row.Name = "row";
		this.row.Properties.Caption = "Contact Code";
		this.row.Properties.FieldName = "CntctCode";
		this.row.Visible = false;
		this.row1.Height = 40;
		this.row1.Name = "row1";
		this.row1.Properties.Caption = "Mã thành viên";
		this.row1.Properties.FieldName = "CardCode";
		this.row1.Properties.ReadOnly = true;
		this.row2.Height = 40;
		this.row2.Name = "row2";
		this.row2.Properties.Caption = "Tên thành viên";
		this.row2.Properties.FieldName = "CardName";
		this.row3.Height = 40;
		this.row3.Name = "row3";
		this.row3.Properties.Caption = "Số CMND";
		this.row3.Properties.FieldName = "AddID";
		this.row4.Height = 40;
		this.row4.Name = "row4";
		this.row4.Properties.Caption = "Số điện thoại";
		this.row4.Properties.FieldName = "Cellular";
		this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOk.Appearance.BackColor = System.Drawing.Color.FromArgb(0, 176, 80);
		this.btnOk.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btnOk.Appearance.ForeColor = System.Drawing.Color.Yellow;
		this.btnOk.Appearance.Options.UseBackColor = true;
		this.btnOk.Appearance.Options.UseFont = true;
		this.btnOk.Appearance.Options.UseForeColor = true;
		this.btnOk.Location = new System.Drawing.Point(3, 389);
		this.btnOk.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btnOk.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btnOk.Name = "btnOk";
		this.btnOk.Size = new System.Drawing.Size(115, 58);
		this.btnOk.TabIndex = 12;
		this.btnOk.Text = "Đồng ý";
		this.btnOk.Visible = false;
		this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(228, 108, 10);
		this.btnCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btnCancel.Appearance.ForeColor = System.Drawing.Color.Yellow;
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.Appearance.Options.UseFont = true;
		this.btnCancel.Appearance.Options.UseForeColor = true;
		this.btnCancel.Location = new System.Drawing.Point(482, 389);
		this.btnCancel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(115, 58);
		this.btnCancel.TabIndex = 13;
		this.btnCancel.Text = "Hủy bỏ";
		this.btnCancel.Visible = false;
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.labelControlEx1.Appearance.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx1.Appearance.Options.UseFont = true;
		this.labelControlEx1.Appearance.Options.UseTextOptions = true;
		this.labelControlEx1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.labelControlEx1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.tableLayoutPanel1.SetColumnSpan(this.labelControlEx1, 2);
		this.labelControlEx1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.labelControlEx1.FieldName = "labelControlEx1";
		this.labelControlEx1.Location = new System.Drawing.Point(3, 3);
		this.labelControlEx1.Name = "labelControlEx1";
		this.labelControlEx1.Size = new System.Drawing.Size(594, 84);
		this.labelControlEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx1.TabIndex = 1;
		this.labelControlEx1.Text = "";
		base.Appearance.BackColor = System.Drawing.SystemColors.Control;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tableLayoutPanel1);
		base.Name = "ucMemberDetail";
		base.Size = new System.Drawing.Size(600, 450);
		this.tableLayoutPanel1.ResumeLayout(false);
		this.tableLayoutPanel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.vGrid_Content).EndInit();
		base.ResumeLayout(false);
	}
}
