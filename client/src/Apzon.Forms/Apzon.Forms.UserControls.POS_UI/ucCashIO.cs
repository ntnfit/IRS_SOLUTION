using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucCashIO : XtraUserControl
{
	public bool CashInType;

	public bool IgnoreEmptyAmt;

	public DataTable DtCashIo;

	public Action ForceOk;

	private IContainer components;

	private ucNumPad ucNumPad1;

	private LabelControlEx labelControlEx4;

	private CalcEditResizable calc_Amount;

	private LabelControlEx labelControlEx1;

	private MemoEditEx memo_Remarks;

	public ucCashIO()
	{
		InitializeComponent();
		SetEventControl(this);
		Load += OnLoad;
	}

	private void OnLoad(object sender, EventArgs eventArgs)
	{
		DtCashIo = new DataTable();
		DtCashIo.Columns.Add("Amount", typeof(double));
		DtCashIo.Columns.Add("Remarks", typeof(string));
		DtCashIo.Columns.Add("Type", typeof(string));
	}

	private void SetEventControl(Control ctr)
	{
		try
		{
			foreach (Control control in ctr.Controls)
			{
				if (control is ButtonEdit { ReadOnly: false } buttonEdit && buttonEdit.Properties.TextEditStyle != TextEditStyles.DisableTextEditor)
				{
					buttonEdit.GotFocus += BtnOnGotFocus;
				}
				else
				{
					SetEventControl(control);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void ResetValue()
	{
		try
		{
			calc_Amount.EditValue = 0;
			memo_Remarks.EditValue = "";
			calc_Amount.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void BtnOnGotFocus(object sender, EventArgs eventArgs1)
	{
		try
		{
			ButtonEdit clientControl = sender as ButtonEdit;
			ucNumPad1.ClientControl = clientControl;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void SUBMIT()
	{
		try
		{
			double num = Function.ParseDouble(calc_Amount.EditValue);
			string text = Function.ToString(memo_Remarks.EditValue);
			if (num > 0.0)
			{
				DtCashIo.Rows.Clear();
				DtCashIo.Rows.Add(num, text, CashInType ? "I" : "O");
				ForceOk?.Invoke();
			}
			else if (Math.Abs(num) < double.Epsilon)
			{
				if (!IgnoreEmptyAmt)
				{
					WinCommons.Warning(Enums.MessageCode.None, "Vui lòng nhập số tiền!");
					return;
				}
				DtCashIo.Rows.Clear();
				DtCashIo.Rows.Add(num, text, CashInType ? "I" : "O");
				ForceOk?.Invoke();
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.None, "Vui lòng nhập số tiền!");
			}
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
		this.ucNumPad1 = new Apzon.Forms.UserControls.POS_UI.ucNumPad();
		this.calc_Amount = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.labelControlEx4 = new Apzon.Controls.Controls.LabelControlEx();
		this.labelControlEx1 = new Apzon.Controls.Controls.LabelControlEx();
		this.memo_Remarks = new Apzon.Controls.Editor.MemoEditEx();
		((System.ComponentModel.ISupportInitialize)this.calc_Amount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Remarks.Properties).BeginInit();
		base.SuspendLayout();
		this.ucNumPad1.ClientControl = null;
		this.ucNumPad1.Location = new System.Drawing.Point(305, 17);
		this.ucNumPad1.Name = "ucNumPad1";
		this.ucNumPad1.Size = new System.Drawing.Size(280, 260);
		this.ucNumPad1.TabIndex = 33;
		this.calc_Amount.FieldName = null;
		this.calc_Amount.Location = new System.Drawing.Point(88, 14);
		this.calc_Amount.Name = "calc_Amount";
		this.calc_Amount.Properties.Appearance.BackColor = System.Drawing.Color.White;
		this.calc_Amount.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 18.0176f);
		this.calc_Amount.Properties.Appearance.Options.UseBackColor = true;
		this.calc_Amount.Properties.Appearance.Options.UseFont = true;
		this.calc_Amount.Properties.AutoHeight = false;
		this.calc_Amount.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.calc_Amount.Properties.NullValuePrompt = "0";
		this.calc_Amount.Properties.NullValuePromptShowForEmptyValue = true;
		this.calc_Amount.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.calc_Amount.Size = new System.Drawing.Size(211, 32);
		this.calc_Amount.TabIndex = 35;
		this.labelControlEx4.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx4.Appearance.Options.UseFont = true;
		this.labelControlEx4.FieldName = "labelControlEx1";
		this.labelControlEx4.Location = new System.Drawing.Point(11, 26);
		this.labelControlEx4.Name = "labelControlEx4";
		this.labelControlEx4.Size = new System.Drawing.Size(52, 18);
		this.labelControlEx4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx4.TabIndex = 34;
		this.labelControlEx4.Text = "Số tiền";
		this.labelControlEx1.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx1.Appearance.Options.UseFont = true;
		this.labelControlEx1.FieldName = "labelControlEx1";
		this.labelControlEx1.Location = new System.Drawing.Point(11, 53);
		this.labelControlEx1.Name = "labelControlEx1";
		this.labelControlEx1.Size = new System.Drawing.Size(55, 18);
		this.labelControlEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx1.TabIndex = 36;
		this.labelControlEx1.Text = "Ghi chú";
		this.memo_Remarks.FieldName = null;
		this.memo_Remarks.Location = new System.Drawing.Point(88, 52);
		this.memo_Remarks.Name = "memo_Remarks";
		this.memo_Remarks.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.memo_Remarks.Properties.Appearance.Options.UseFont = true;
		this.memo_Remarks.Size = new System.Drawing.Size(211, 225);
		this.memo_Remarks.TabIndex = 37;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.memo_Remarks);
		base.Controls.Add(this.labelControlEx1);
		base.Controls.Add(this.calc_Amount);
		base.Controls.Add(this.labelControlEx4);
		base.Controls.Add(this.ucNumPad1);
		base.Name = "ucCashIO";
		base.Size = new System.Drawing.Size(592, 287);
		((System.ComponentModel.ISupportInitialize)this.calc_Amount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Remarks.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
