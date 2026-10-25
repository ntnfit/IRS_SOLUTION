using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucCashPay : XtraUserControl
{
	public Action ForceOk;

	public double ChangeDue;

	public double PaidAmount;

	private readonly List<double> _curDenomin = new List<double> { 1000.0, 2000.0, 5000.0, 10000.0, 20000.0, 50000.0, 100000.0, 200000.0, 500000.0 };

	private IContainer components;

	private SimpleButton btn_500000;

	private LabelControlEx labelControlEx1;

	private SimpleButton btn_200000;

	private SimpleButton btn_100000;

	private SimpleButton btn_50000;

	private SimpleButton btn_20000;

	private SimpleButton btn_10000;

	private LabelControlEx labelControlEx2;

	private SimpleButton btn_Sug1;

	private SimpleButton btn_Sug2;

	private SimpleButton btn_Sug3;

	private SimpleButton btn_Sug4;

	private SimpleButton btn_Sug5;

	private SimpleButton btn_Sug6;

	private LabelControlEx labelControlEx3;

	private CalcEditResizable txt_ChangeDue;

	private ucNumPad ucNumPad1;

	private CalcEditResizable txt_Total;

	private LabelControlEx labelControlEx4;

	private CalcEditResizable txt_Result;

	private LabelControlEx labelControlEx5;

	public ucCashPay()
	{
		InitializeComponent();
		Load += OnLoad;
		SetEventControl(this);
	}

	private void OnLoad(object sender, EventArgs eventArgs)
	{
	}

	private void SetEventControl(Control ctr)
	{
		foreach (Control control in ctr.Controls)
		{
			ucNumPad ucNumPad2 = control as ucNumPad;
			if (control is SimpleButton simpleButton)
			{
				simpleButton.Click += BtnOnClick;
				simpleButton.TextChanged += BtnOnTextChanged;
			}
			else if (ucNumPad2 == null)
			{
				SetEventControl(control);
			}
		}
	}

	private void BtnOnTextChanged(object sender, EventArgs eventArgs)
	{
		try
		{
			if (sender is SimpleButton simpleButton)
			{
				if (Math.Abs(Function.ParseDouble(simpleButton.Text)) < double.Epsilon)
				{
					simpleButton.Visible = false;
				}
				else
				{
					simpleButton.Visible = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void BtnOnClick(object sender, EventArgs eventArgs)
	{
		try
		{
			if (sender is SimpleButton simpleButton && Math.Abs(Function.ParseDouble(simpleButton.Text.Replace(",", ""))) > double.Epsilon)
			{
				txt_Result.EditValue = Function.ParseDouble(simpleButton.Text.Replace(",", ""));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void SetCusPay(double amt)
	{
		txt_Result.EditValue = amt;
	}

	private double GetSpecialSug(double val, double par)
	{
		double num = 0.0;
		try
		{
			num = ((!(Math.Ceiling(val / par) < val / par + 1.0)) ? ((val / par + 1.0) * par) : ((Math.Ceiling(val / par) + 1.0) * par));
			if (_curDenomin.Contains(num) || !(val / par > 5.0) || Math.Ceiling(val / par) > val / par)
			{
				num = 0.0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return num;
	}

	private double GetSuggest(double val, double par)
	{
		double num = 0.0;
		try
		{
			if (Math.Ceiling(val / par) < val / par + 1.0)
			{
				num = Math.Ceiling(val / par) * par;
				if (_curDenomin.Contains(num))
				{
					num = 0.0;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return num;
	}

	public void SetValues(double invTotal)
	{
		try
		{
			double amt = invTotal;
			if (amt <= 0.0)
			{
				amt = 0.0;
			}
			List<double> list = new List<double> { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
			List<double> list2 = _curDenomin.Select((double val) => GetSuggest(amt, val)).ToList();
			if (amt <= 10000.0)
			{
				list2.Add(GetSpecialSug(amt, 1000.0));
			}
			else if (amt > 10000.0 && amt <= 100000.0)
			{
				list2.Add(GetSpecialSug(amt, 1000.0));
				list2.Add(GetSpecialSug(amt, 10000.0));
			}
			else
			{
				list2.Add(GetSpecialSug(amt, 1000.0));
				list2.Add(GetSpecialSug(amt, 10000.0));
				list2.Add(GetSpecialSug(amt, 100000.0));
			}
			List<double> list3 = list2.Distinct().ToList();
			list3.Remove(0.0);
			list3.Sort();
			for (int num = 0; num < list3.Count && num <= list.Count - 1; num++)
			{
				list[num] = list3[num];
			}
			btn_Sug1.Text = string.Format(CultureInfo.InvariantCulture, "{0:0,0}", list[0]);
			btn_Sug2.Text = string.Format(CultureInfo.InvariantCulture, "{0:0,0}", list[1]);
			btn_Sug3.Text = string.Format(CultureInfo.InvariantCulture, "{0:0,0}", list[2]);
			btn_Sug4.Text = string.Format(CultureInfo.InvariantCulture, "{0:0,0}", list[3]);
			btn_Sug5.Text = string.Format(CultureInfo.InvariantCulture, "{0:0,0}", list[4]);
			btn_Sug6.Text = string.Format(CultureInfo.InvariantCulture, "{0:0,0}", list[5]);
			txt_Total.EditValue = invTotal;
			txt_Result.EditValue = ((invTotal >= 0.0) ? invTotal : 0.0);
			double num2 = Function.ParseDouble(txt_Result.EditValue) - Function.ParseDouble(txt_Total.EditValue);
			txt_ChangeDue.EditValue = num2;
			txt_Result.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ucCashPay_Load(object sender, EventArgs e)
	{
	}

	private void txt_Result_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			txt_ChangeDue.EditValue = Function.ParseDouble(e.NewValue) - Function.ParseDouble(txt_Total.EditValue);
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
			double num = Function.ParseDouble(txt_ChangeDue.EditValue);
			if (num >= 0.0)
			{
				ForceOk?.Invoke();
				ChangeDue = num;
				PaidAmount = Function.ParseDouble(txt_Result.EditValue);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Result_Enter(object sender, EventArgs e)
	{
		_ = (ButtonEdit)sender;
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
		this.btn_500000 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_200000 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_100000 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_50000 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_20000 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_10000 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Sug1 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Sug2 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Sug3 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Sug4 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Sug5 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Sug6 = new DevExpress.XtraEditors.SimpleButton();
		this.txt_Result = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.labelControlEx5 = new Apzon.Controls.Controls.LabelControlEx();
		this.txt_Total = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.labelControlEx4 = new Apzon.Controls.Controls.LabelControlEx();
		this.ucNumPad1 = new Apzon.Forms.UserControls.POS_UI.ucNumPad();
		this.txt_ChangeDue = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.labelControlEx3 = new Apzon.Controls.Controls.LabelControlEx();
		this.labelControlEx2 = new Apzon.Controls.Controls.LabelControlEx();
		this.labelControlEx1 = new Apzon.Controls.Controls.LabelControlEx();
		((System.ComponentModel.ISupportInitialize)this.txt_Result.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Total.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ChangeDue.Properties).BeginInit();
		base.SuspendLayout();
		this.btn_500000.Appearance.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.btn_500000.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_500000.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_500000.Appearance.Options.UseBackColor = true;
		this.btn_500000.Appearance.Options.UseFont = true;
		this.btn_500000.Appearance.Options.UseForeColor = true;
		this.btn_500000.Location = new System.Drawing.Point(10, 38);
		this.btn_500000.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_500000.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_500000.Name = "btn_500000";
		this.btn_500000.Size = new System.Drawing.Size(75, 46);
		this.btn_500000.TabIndex = 16;
		this.btn_500000.Text = "500,000";
		this.btn_200000.Appearance.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.btn_200000.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_200000.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_200000.Appearance.Options.UseBackColor = true;
		this.btn_200000.Appearance.Options.UseFont = true;
		this.btn_200000.Appearance.Options.UseForeColor = true;
		this.btn_200000.Location = new System.Drawing.Point(91, 38);
		this.btn_200000.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_200000.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_200000.Name = "btn_200000";
		this.btn_200000.Size = new System.Drawing.Size(75, 46);
		this.btn_200000.TabIndex = 18;
		this.btn_200000.Text = "200,000";
		this.btn_100000.Appearance.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.btn_100000.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_100000.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_100000.Appearance.Options.UseBackColor = true;
		this.btn_100000.Appearance.Options.UseFont = true;
		this.btn_100000.Appearance.Options.UseForeColor = true;
		this.btn_100000.Location = new System.Drawing.Point(172, 38);
		this.btn_100000.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_100000.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_100000.Name = "btn_100000";
		this.btn_100000.Size = new System.Drawing.Size(88, 46);
		this.btn_100000.TabIndex = 19;
		this.btn_100000.Text = "100,000";
		this.btn_50000.Appearance.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.btn_50000.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_50000.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_50000.Appearance.Options.UseBackColor = true;
		this.btn_50000.Appearance.Options.UseFont = true;
		this.btn_50000.Appearance.Options.UseForeColor = true;
		this.btn_50000.Location = new System.Drawing.Point(7, 90);
		this.btn_50000.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_50000.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_50000.Name = "btn_50000";
		this.btn_50000.Size = new System.Drawing.Size(78, 41);
		this.btn_50000.TabIndex = 20;
		this.btn_50000.Text = "50,000";
		this.btn_20000.Appearance.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.btn_20000.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_20000.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_20000.Appearance.Options.UseBackColor = true;
		this.btn_20000.Appearance.Options.UseFont = true;
		this.btn_20000.Appearance.Options.UseForeColor = true;
		this.btn_20000.Location = new System.Drawing.Point(91, 90);
		this.btn_20000.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_20000.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_20000.Name = "btn_20000";
		this.btn_20000.Size = new System.Drawing.Size(75, 41);
		this.btn_20000.TabIndex = 21;
		this.btn_20000.Text = "20,000";
		this.btn_10000.Appearance.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.btn_10000.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_10000.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_10000.Appearance.Options.UseBackColor = true;
		this.btn_10000.Appearance.Options.UseFont = true;
		this.btn_10000.Appearance.Options.UseForeColor = true;
		this.btn_10000.Location = new System.Drawing.Point(172, 90);
		this.btn_10000.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_10000.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_10000.Name = "btn_10000";
		this.btn_10000.Size = new System.Drawing.Size(88, 41);
		this.btn_10000.TabIndex = 22;
		this.btn_10000.Text = "10,000";
		this.btn_Sug1.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.btn_Sug1.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Sug1.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_Sug1.Appearance.Options.UseBackColor = true;
		this.btn_Sug1.Appearance.Options.UseFont = true;
		this.btn_Sug1.Appearance.Options.UseForeColor = true;
		this.btn_Sug1.Location = new System.Drawing.Point(7, 164);
		this.btn_Sug1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Sug1.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Sug1.Name = "btn_Sug1";
		this.btn_Sug1.Size = new System.Drawing.Size(75, 38);
		this.btn_Sug1.TabIndex = 24;
		this.btn_Sug1.Visible = false;
		this.btn_Sug2.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.btn_Sug2.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Sug2.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_Sug2.Appearance.Options.UseBackColor = true;
		this.btn_Sug2.Appearance.Options.UseFont = true;
		this.btn_Sug2.Appearance.Options.UseForeColor = true;
		this.btn_Sug2.Location = new System.Drawing.Point(91, 164);
		this.btn_Sug2.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Sug2.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Sug2.Name = "btn_Sug2";
		this.btn_Sug2.Size = new System.Drawing.Size(83, 38);
		this.btn_Sug2.TabIndex = 25;
		this.btn_Sug2.Visible = false;
		this.btn_Sug3.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.btn_Sug3.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Sug3.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_Sug3.Appearance.Options.UseBackColor = true;
		this.btn_Sug3.Appearance.Options.UseFont = true;
		this.btn_Sug3.Appearance.Options.UseForeColor = true;
		this.btn_Sug3.Location = new System.Drawing.Point(180, 164);
		this.btn_Sug3.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Sug3.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Sug3.Name = "btn_Sug3";
		this.btn_Sug3.Size = new System.Drawing.Size(99, 38);
		this.btn_Sug3.TabIndex = 26;
		this.btn_Sug3.Visible = false;
		this.btn_Sug4.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.btn_Sug4.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Sug4.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_Sug4.Appearance.Options.UseBackColor = true;
		this.btn_Sug4.Appearance.Options.UseFont = true;
		this.btn_Sug4.Appearance.Options.UseForeColor = true;
		this.btn_Sug4.Location = new System.Drawing.Point(7, 208);
		this.btn_Sug4.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Sug4.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Sug4.Name = "btn_Sug4";
		this.btn_Sug4.Size = new System.Drawing.Size(75, 37);
		this.btn_Sug4.TabIndex = 27;
		this.btn_Sug4.Visible = false;
		this.btn_Sug5.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.btn_Sug5.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Sug5.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_Sug5.Appearance.Options.UseBackColor = true;
		this.btn_Sug5.Appearance.Options.UseFont = true;
		this.btn_Sug5.Appearance.Options.UseForeColor = true;
		this.btn_Sug5.Location = new System.Drawing.Point(91, 208);
		this.btn_Sug5.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Sug5.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Sug5.Name = "btn_Sug5";
		this.btn_Sug5.Size = new System.Drawing.Size(83, 37);
		this.btn_Sug5.TabIndex = 28;
		this.btn_Sug5.Visible = false;
		this.btn_Sug6.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.btn_Sug6.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Sug6.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_Sug6.Appearance.Options.UseBackColor = true;
		this.btn_Sug6.Appearance.Options.UseFont = true;
		this.btn_Sug6.Appearance.Options.UseForeColor = true;
		this.btn_Sug6.Location = new System.Drawing.Point(180, 208);
		this.btn_Sug6.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Sug6.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Sug6.Name = "btn_Sug6";
		this.btn_Sug6.Size = new System.Drawing.Size(99, 37);
		this.btn_Sug6.TabIndex = 29;
		this.btn_Sug6.Visible = false;
		this.txt_Result.FieldName = null;
		this.txt_Result.Location = new System.Drawing.Point(425, 42);
		this.txt_Result.Name = "txt_Result";
		this.txt_Result.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 18.0176f);
		this.txt_Result.Properties.Appearance.Options.UseFont = true;
		this.txt_Result.Properties.AutoHeight = false;
		this.txt_Result.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txt_Result.Properties.NullValuePrompt = "0";
		this.txt_Result.Properties.NullValuePromptShowForEmptyValue = true;
		this.txt_Result.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.txt_Result.Size = new System.Drawing.Size(215, 32);
		this.txt_Result.TabIndex = 0;
		this.txt_Result.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_Result_EditValueChanging);
		this.txt_Result.Enter += new System.EventHandler(this.txt_Result_Enter);
		this.labelControlEx5.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx5.Appearance.Options.UseFont = true;
		this.labelControlEx5.FieldName = "labelControlEx1";
		this.labelControlEx5.Location = new System.Drawing.Point(302, 52);
		this.labelControlEx5.Name = "labelControlEx5";
		this.labelControlEx5.Size = new System.Drawing.Size(106, 18);
		this.labelControlEx5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx5.TabIndex = 36;
		this.labelControlEx5.Text = "Tiền khách trả";
		this.txt_Total.FieldName = null;
		this.txt_Total.Location = new System.Drawing.Point(425, 1);
		this.txt_Total.Name = "txt_Total";
		this.txt_Total.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.txt_Total.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 18.0176f);
		this.txt_Total.Properties.Appearance.Options.UseBackColor = true;
		this.txt_Total.Properties.Appearance.Options.UseFont = true;
		this.txt_Total.Properties.AutoHeight = false;
		this.txt_Total.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.txt_Total.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txt_Total.Properties.NullValuePrompt = "0";
		this.txt_Total.Properties.NullValuePromptShowForEmptyValue = true;
		this.txt_Total.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.txt_Total.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.txt_Total.Size = new System.Drawing.Size(215, 32);
		this.txt_Total.TabIndex = 35;
		this.labelControlEx4.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx4.Appearance.Options.UseFont = true;
		this.labelControlEx4.FieldName = "labelControlEx1";
		this.labelControlEx4.Location = new System.Drawing.Point(302, 14);
		this.labelControlEx4.Name = "labelControlEx4";
		this.labelControlEx4.Size = new System.Drawing.Size(69, 18);
		this.labelControlEx4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx4.TabIndex = 34;
		this.labelControlEx4.Text = "Tổng tiền";
		this.ucNumPad1.ClientControl = this.txt_Result;
		this.ucNumPad1.Location = new System.Drawing.Point(305, 129);
		this.ucNumPad1.Name = "ucNumPad1";
		this.ucNumPad1.Size = new System.Drawing.Size(338, 218);
		this.ucNumPad1.TabIndex = 33;
		this.txt_ChangeDue.FieldName = null;
		this.txt_ChangeDue.Location = new System.Drawing.Point(425, 81);
		this.txt_ChangeDue.Name = "txt_ChangeDue";
		this.txt_ChangeDue.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.txt_ChangeDue.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 18.0176f);
		this.txt_ChangeDue.Properties.Appearance.Options.UseBackColor = true;
		this.txt_ChangeDue.Properties.Appearance.Options.UseFont = true;
		this.txt_ChangeDue.Properties.AutoHeight = false;
		this.txt_ChangeDue.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.txt_ChangeDue.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txt_ChangeDue.Properties.NullValuePrompt = "0";
		this.txt_ChangeDue.Properties.NullValuePromptShowForEmptyValue = true;
		this.txt_ChangeDue.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.txt_ChangeDue.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.txt_ChangeDue.Size = new System.Drawing.Size(215, 32);
		this.txt_ChangeDue.TabIndex = 32;
		this.labelControlEx3.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx3.Appearance.Options.UseFont = true;
		this.labelControlEx3.FieldName = "labelControlEx1";
		this.labelControlEx3.Location = new System.Drawing.Point(305, 91);
		this.labelControlEx3.Name = "labelControlEx3";
		this.labelControlEx3.Size = new System.Drawing.Size(96, 18);
		this.labelControlEx3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx3.TabIndex = 30;
		this.labelControlEx3.Text = "Trả lại khách";
		this.labelControlEx2.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx2.Appearance.Options.UseFont = true;
		this.labelControlEx2.FieldName = "labelControlEx1";
		this.labelControlEx2.Location = new System.Drawing.Point(8, 133);
		this.labelControlEx2.Name = "labelControlEx2";
		this.labelControlEx2.Size = new System.Drawing.Size(89, 16);
		this.labelControlEx2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx2.TabIndex = 23;
		this.labelControlEx2.Text = "Gợi ý tiền mặt";
		this.labelControlEx1.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx1.Appearance.Options.UseFont = true;
		this.labelControlEx1.FieldName = "labelControlEx1";
		this.labelControlEx1.Location = new System.Drawing.Point(18, 16);
		this.labelControlEx1.Name = "labelControlEx1";
		this.labelControlEx1.Size = new System.Drawing.Size(93, 16);
		this.labelControlEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx1.TabIndex = 17;
		this.labelControlEx1.Text = "Theo mệnh giá";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.txt_Result);
		base.Controls.Add(this.labelControlEx5);
		base.Controls.Add(this.txt_Total);
		base.Controls.Add(this.labelControlEx4);
		base.Controls.Add(this.ucNumPad1);
		base.Controls.Add(this.txt_ChangeDue);
		base.Controls.Add(this.labelControlEx3);
		base.Controls.Add(this.btn_Sug6);
		base.Controls.Add(this.btn_Sug5);
		base.Controls.Add(this.btn_Sug4);
		base.Controls.Add(this.btn_Sug3);
		base.Controls.Add(this.btn_Sug2);
		base.Controls.Add(this.btn_Sug1);
		base.Controls.Add(this.labelControlEx2);
		base.Controls.Add(this.btn_10000);
		base.Controls.Add(this.btn_20000);
		base.Controls.Add(this.btn_50000);
		base.Controls.Add(this.btn_100000);
		base.Controls.Add(this.btn_200000);
		base.Controls.Add(this.labelControlEx1);
		base.Controls.Add(this.btn_500000);
		base.Name = "ucCashPay";
		base.Size = new System.Drawing.Size(646, 349);
		base.Load += new System.EventHandler(this.ucCashPay_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_Result.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Total.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ChangeDue.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
