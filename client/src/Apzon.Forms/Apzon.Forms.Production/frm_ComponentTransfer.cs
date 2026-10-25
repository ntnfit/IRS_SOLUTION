using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Forms.Helper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Production;

public class frm_ComponentTransfer : XtraForm
{
	private IContainer components;

	private CalcEditEx txtQuantity;

	private SimpleButtonEx btnCancel;

	private SimpleButton btnOk;

	private RadioGroupEx radioGroupQuantity;

	public string TypeGetData { get; set; }

	public double Quantity { get; set; }

	public frm_ComponentTransfer(double defaultQty)
	{
		InitializeComponent();
		txtQuantity.EditValue = defaultQty;
	}

	private void radioGroupQuantity_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			txtQuantity.Enabled = Function.ToString(radioGroupQuantity.EditValue).Equals("Q");
			txtQuantity.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_ComponentTransfer_Load(object sender, EventArgs e)
	{
		txtQuantity.SetFormatControl(Enums.DisplayFormatType.Qty);
		radioGroupQuantity.EditValue = "O";
		txtQuantity.Enabled = false;
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.Cancel;
	}

	private void btnOk_Click(object sender, EventArgs e)
	{
		if (Function.ToString(radioGroupQuantity.EditValue) == "Q" && Function.ParseDouble(txtQuantity.EditValue) <= 0.0)
		{
			WinCommons.Error(Enums.MessageCode.EnterAPositiveValue, "");
			DialogResult = DialogResult.Cancel;
		}
		else
		{
			TypeGetData = Function.ToString(radioGroupQuantity.EditValue);
			Quantity = ((Function.ToString(radioGroupQuantity.EditValue) == "Q") ? Function.ParseDouble(txtQuantity.EditValue) : 0.0);
			DialogResult = DialogResult.OK;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Production.frm_ComponentTransfer));
		this.btnOk = new DevExpress.XtraEditors.SimpleButton();
		this.radioGroupQuantity = new Apzon.Controls.Editor.RadioGroupEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtQuantity = new Apzon.Controls.Editor.CalcEditEx();
		((System.ComponentModel.ISupportInitialize)this.radioGroupQuantity.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtQuantity.Properties).BeginInit();
		base.SuspendLayout();
		this.btnOk.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOk.ImageOptions.Image");
		this.btnOk.Location = new System.Drawing.Point(182, 60);
		this.btnOk.Name = "btnOk";
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 3;
		this.btnOk.Text = "OK";
		this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
		this.radioGroupQuantity.FieldName = null;
		this.radioGroupQuantity.IsSearchControl = false;
		this.radioGroupQuantity.Location = new System.Drawing.Point(12, 8);
		this.radioGroupQuantity.Name = "radioGroupQuantity";
		this.radioGroupQuantity.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.radioGroupQuantity.Properties.Appearance.Options.UseBackColor = true;
		this.radioGroupQuantity.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.radioGroupQuantity.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("O", "Open Quantity of Components"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("Q", "Quantity of Parent Item")
		});
		this.radioGroupQuantity.Size = new System.Drawing.Size(196, 48);
		this.radioGroupQuantity.TabIndex = 1;
		this.radioGroupQuantity.EditValueChanged += new System.EventHandler(this.radioGroupQuantity_EditValueChanged);
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(262, 60);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 4;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.txtQuantity.FieldName = null;
		this.txtQuantity.IsSearchControl = false;
		this.txtQuantity.Location = new System.Drawing.Point(214, 29);
		this.txtQuantity.Name = "txtQuantity";
		this.txtQuantity.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtQuantity.Properties.IsSearchControl = false;
		this.txtQuantity.Size = new System.Drawing.Size(100, 20);
		this.txtQuantity.TabIndex = 2;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(342, 90);
		base.Controls.Add(this.radioGroupQuantity);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.txtQuantity);
		base.Name = "frm_ComponentTransfer";
		base.ShowIcon = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Component Transfer";
		base.Load += new System.EventHandler(this.frm_ComponentTransfer_Load);
		((System.ComponentModel.ISupportInitialize)this.radioGroupQuantity.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtQuantity.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
