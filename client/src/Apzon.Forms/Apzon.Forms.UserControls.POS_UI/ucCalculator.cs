using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Global;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucCalculator : XtraUserControl
{
	private double _resultValue;

	private string _operationPerformed = "";

	private bool _isOperationPerformed;

	private bool _isEqualClicked;

	private bool _isDecimalFraction;

	private bool _isClearAll = true;

	private bool _isFirstNo;

	private bool _isFirstOpe;

	public Action<double> EnterButtonClick;

	private IContainer components;

	private LabelControlEx labelCurrentOperation;

	private KeyboardSimpleButton btn_Equal;

	private KeyboardSimpleButton btn_Plus;

	private KeyboardSimpleButton btn_Dot;

	private KeyboardSimpleButton btn_0;

	private KeyboardSimpleButton btn_Minus;

	private KeyboardSimpleButton btn_3;

	private KeyboardSimpleButton btn_2;

	private KeyboardSimpleButton btn_1;

	private KeyboardSimpleButton btn_Cancel;

	private KeyboardSimpleButton btn_multiple;

	private KeyboardSimpleButton btn_6;

	private KeyboardSimpleButton btn_5;

	private KeyboardSimpleButton btn_4;

	private KeyboardSimpleButton btn_CancelEntry;

	private KeyboardSimpleButton btn_Devide;

	private KeyboardSimpleButton btn_9;

	private KeyboardSimpleButton btn_8;

	private KeyboardSimpleButton btn_7;

	private TableLayoutPanel tableLayoutPanel1;

	private KeyboardSimpleButton btn_00;

	private KeyboardSimpleButton btn_Submit;

	private CalcEditResizable txt_Result;

	private bool IsClearAll
	{
		get
		{
			return _isClearAll;
		}
		set
		{
			_isClearAll = value;
			if (value)
			{
				txt_Result.EditValue = 0;
				_resultValue = 0.0;
				_isDecimalFraction = false;
				_isOperationPerformed = false;
				_isEqualClicked = false;
				_operationPerformed = "";
				_isFirstNo = false;
				_isFirstOpe = false;
			}
		}
	}

	public ucCalculator()
	{
		InitializeComponent();
	}

	private void uc_Calculator_Load(object sender, EventArgs e)
	{
		IsClearAll = true;
		txt_Result.CustomDisplayText += TxtResultOnCustomDisplayText;
	}

	private void TxtResultOnCustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		if (Math.Abs(Function.ParseDouble(e.Value)) < double.Epsilon)
		{
			e.DisplayText = "0";
		}
	}

	private void button_click(object sender, EventArgs e)
	{
		try
		{
			if (Math.Abs(Function.ParseDouble(txt_Result.EditValue)) < double.Epsilon || _isOperationPerformed)
			{
				txt_Result.EditValue = 0;
			}
			if (_isEqualClicked)
			{
				btn_Cancel.PerformClick();
			}
			KeyboardSimpleButton keyboardSimpleButton = (KeyboardSimpleButton)sender;
			if (keyboardSimpleButton.Text.Equals("."))
			{
				_isDecimalFraction = true;
			}
			else
			{
				_isFirstNo = IsClearAll;
				string text = Function.ToString(txt_Result.EditValue);
				string text2;
				if (_isDecimalFraction)
				{
					text2 = ((!text.Contains(GlobalData.GeneralSetting.DecSep)) ? (text + GlobalData.GeneralSetting.DecSep + keyboardSimpleButton.Text) : (text + keyboardSimpleButton.Text));
				}
				else
				{
					text2 = text + keyboardSimpleButton.Text;
				}
				if (_operationPerformed == "-" && _isFirstOpe && !text2.Contains("-"))
				{
					text2 = "-" + text2;
				}
				txt_Result.EditValue = Function.ParseDouble(text2);
			}
			_isOperationPerformed = false;
			_isEqualClicked = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void operator_click(object sender, EventArgs e)
	{
		try
		{
			_isFirstOpe = !_isFirstNo && IsClearAll;
			KeyboardSimpleButton keyboardSimpleButton = (KeyboardSimpleButton)sender;
			if (Math.Abs(_resultValue) > 0.0 && !_isEqualClicked && !_isOperationPerformed)
			{
				btn_Equal.PerformClick();
			}
			else
			{
				_resultValue = Function.ParseDouble(txt_Result.Text);
			}
			_operationPerformed = keyboardSimpleButton.Text;
			_isOperationPerformed = true;
			labelCurrentOperation.Text = _resultValue + " " + _operationPerformed;
			_isEqualClicked = false;
			_isDecimalFraction = false;
			IsClearAll = false;
			_isFirstNo = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_CancelEntry_Click(object sender, EventArgs e)
	{
		try
		{
			if (_isEqualClicked)
			{
				btn_Cancel.PerformClick();
				return;
			}
			string text = Function.ToString(txt_Result.EditValue);
			int num = text.Length - 1;
			if (num > 0)
			{
				string text2 = text.Substring(0, num);
				if (text2.EndsWith(GlobalData.GeneralSetting.DecSep))
				{
					_isDecimalFraction = false;
				}
				txt_Result.EditValue = Function.ParseDouble(text2);
				if (Math.Abs(Function.ParseDouble(txt_Result.EditValue)) < double.Epsilon && (_isFirstNo || _isFirstOpe))
				{
					IsClearAll = true;
				}
			}
			else
			{
				txt_Result.EditValue = 0;
				if (_isFirstNo || _isFirstOpe)
				{
					IsClearAll = true;
				}
			}
			_isOperationPerformed = false;
			_isEqualClicked = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			IsClearAll = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Equal_Click(object sender, EventArgs e)
	{
		try
		{
			if (_isEqualClicked)
			{
				return;
			}
			if (!_isFirstNo && !_isFirstOpe)
			{
				switch (_operationPerformed)
				{
				case "+":
					txt_Result.EditValue = _resultValue + Function.ParseDouble(txt_Result.Text);
					break;
				case "-":
					txt_Result.EditValue = _resultValue - Function.ParseDouble(txt_Result.Text);
					break;
				case "*":
					txt_Result.EditValue = _resultValue * Function.ParseDouble(txt_Result.Text);
					break;
				case "/":
					txt_Result.EditValue = _resultValue / Function.ParseDouble(txt_Result.Text);
					break;
				}
			}
			_resultValue = Function.ParseDouble(txt_Result.Text);
			_isEqualClicked = true;
			_isDecimalFraction = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Submit_Click(object sender, EventArgs e)
	{
		try
		{
			btn_Equal.PerformClick();
			EnterButtonClick?.Invoke(_resultValue);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.POS_UI.ucCalculator));
		this.labelCurrentOperation = new Apzon.Controls.Controls.LabelControlEx();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.txt_Result = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.btn_0 = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_1 = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_4 = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_7 = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_00 = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_2 = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_5 = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_8 = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_Dot = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_3 = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_6 = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_9 = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_Minus = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_Plus = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_multiple = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_Devide = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_Equal = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_Submit = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_Cancel = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_CancelEntry = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.tableLayoutPanel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Result.Properties).BeginInit();
		base.SuspendLayout();
		this.labelCurrentOperation.Appearance.BackColor = System.Drawing.SystemColors.Window;
		this.labelCurrentOperation.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelCurrentOperation.Appearance.ForeColor = System.Drawing.SystemColors.ControlDark;
		this.labelCurrentOperation.Appearance.Options.UseBackColor = true;
		this.labelCurrentOperation.Appearance.Options.UseFont = true;
		this.labelCurrentOperation.Appearance.Options.UseForeColor = true;
		this.labelCurrentOperation.FieldName = "labelCurrentOperation";
		this.labelCurrentOperation.Location = new System.Drawing.Point(10, 8);
		this.labelCurrentOperation.Name = "labelCurrentOperation";
		this.labelCurrentOperation.Size = new System.Drawing.Size(0, 24);
		this.labelCurrentOperation.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelCurrentOperation.TabIndex = 41;
		this.labelCurrentOperation.Visible = false;
		this.tableLayoutPanel1.ColumnCount = 5;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20f));
		this.tableLayoutPanel1.Controls.Add(this.txt_Result, 0, 0);
		this.tableLayoutPanel1.Controls.Add(this.btn_0, 0, 4);
		this.tableLayoutPanel1.Controls.Add(this.btn_1, 0, 3);
		this.tableLayoutPanel1.Controls.Add(this.btn_4, 0, 2);
		this.tableLayoutPanel1.Controls.Add(this.btn_7, 0, 1);
		this.tableLayoutPanel1.Controls.Add(this.btn_00, 1, 4);
		this.tableLayoutPanel1.Controls.Add(this.btn_2, 1, 3);
		this.tableLayoutPanel1.Controls.Add(this.btn_5, 1, 2);
		this.tableLayoutPanel1.Controls.Add(this.btn_8, 1, 1);
		this.tableLayoutPanel1.Controls.Add(this.btn_Dot, 2, 4);
		this.tableLayoutPanel1.Controls.Add(this.btn_3, 2, 3);
		this.tableLayoutPanel1.Controls.Add(this.btn_6, 2, 2);
		this.tableLayoutPanel1.Controls.Add(this.btn_9, 2, 1);
		this.tableLayoutPanel1.Controls.Add(this.btn_Minus, 3, 4);
		this.tableLayoutPanel1.Controls.Add(this.btn_Plus, 3, 3);
		this.tableLayoutPanel1.Controls.Add(this.btn_multiple, 3, 2);
		this.tableLayoutPanel1.Controls.Add(this.btn_Devide, 3, 1);
		this.tableLayoutPanel1.Controls.Add(this.btn_Equal, 4, 4);
		this.tableLayoutPanel1.Controls.Add(this.btn_Submit, 4, 3);
		this.tableLayoutPanel1.Controls.Add(this.btn_Cancel, 4, 2);
		this.tableLayoutPanel1.Controls.Add(this.btn_CancelEntry, 4, 1);
		this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.RowCount = 5;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20f));
		this.tableLayoutPanel1.Size = new System.Drawing.Size(361, 325);
		this.tableLayoutPanel1.TabIndex = 42;
		this.tableLayoutPanel1.SetColumnSpan(this.txt_Result, 5);
		this.txt_Result.Dock = System.Windows.Forms.DockStyle.Fill;
		this.txt_Result.FieldName = null;
		this.txt_Result.Location = new System.Drawing.Point(3, 3);
		this.txt_Result.Name = "txt_Result";
		this.txt_Result.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 33.21995f);
		this.txt_Result.Properties.Appearance.Options.UseFont = true;
		this.txt_Result.Properties.AutoHeight = false;
		this.txt_Result.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txt_Result.Properties.NullValuePrompt = "0";
		this.txt_Result.Properties.NullValuePromptShowForEmptyValue = true;
		this.txt_Result.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.txt_Result.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.txt_Result.Size = new System.Drawing.Size(355, 59);
		this.txt_Result.TabIndex = 21;
		this.btn_0.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_0.Appearance.BackColor = System.Drawing.Color.SkyBlue;
		this.btn_0.Appearance.Font = new System.Drawing.Font("Arial", 29.73871f);
		this.btn_0.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_0.Appearance.Options.UseBackColor = true;
		this.btn_0.Appearance.Options.UseFont = true;
		this.btn_0.Appearance.Options.UseForeColor = true;
		this.btn_0.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_0.Location = new System.Drawing.Point(3, 263);
		this.btn_0.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_0.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_0.Name = "btn_0";
		this.btn_0.Size = new System.Drawing.Size(66, 59);
		this.btn_0.TabIndex = 36;
		this.btn_0.Text = "0";
		this.btn_0.Click += new System.EventHandler(this.button_click);
		this.btn_1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_1.Appearance.BackColor = System.Drawing.Color.SkyBlue;
		this.btn_1.Appearance.Font = new System.Drawing.Font("Arial", 29.76987f);
		this.btn_1.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_1.Appearance.Options.UseBackColor = true;
		this.btn_1.Appearance.Options.UseFont = true;
		this.btn_1.Appearance.Options.UseForeColor = true;
		this.btn_1.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_1.Location = new System.Drawing.Point(3, 198);
		this.btn_1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_1.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_1.Name = "btn_1";
		this.btn_1.Size = new System.Drawing.Size(66, 59);
		this.btn_1.TabIndex = 32;
		this.btn_1.Text = "1";
		this.btn_1.Click += new System.EventHandler(this.button_click);
		this.btn_4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_4.Appearance.BackColor = System.Drawing.Color.SkyBlue;
		this.btn_4.Appearance.Font = new System.Drawing.Font("Arial", 29.76987f);
		this.btn_4.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_4.Appearance.Options.UseBackColor = true;
		this.btn_4.Appearance.Options.UseFont = true;
		this.btn_4.Appearance.Options.UseForeColor = true;
		this.btn_4.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_4.Location = new System.Drawing.Point(3, 133);
		this.btn_4.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_4.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_4.Name = "btn_4";
		this.btn_4.Size = new System.Drawing.Size(66, 59);
		this.btn_4.TabIndex = 27;
		this.btn_4.Text = "4";
		this.btn_4.Click += new System.EventHandler(this.button_click);
		this.btn_7.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_7.Appearance.BackColor = System.Drawing.Color.SkyBlue;
		this.btn_7.Appearance.Font = new System.Drawing.Font("Arial", 29.76987f);
		this.btn_7.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_7.Appearance.Options.UseBackColor = true;
		this.btn_7.Appearance.Options.UseFont = true;
		this.btn_7.Appearance.Options.UseForeColor = true;
		this.btn_7.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_7.Location = new System.Drawing.Point(3, 68);
		this.btn_7.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_7.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_7.Name = "btn_7";
		this.btn_7.Size = new System.Drawing.Size(66, 59);
		this.btn_7.TabIndex = 22;
		this.btn_7.Text = "7";
		this.btn_7.Click += new System.EventHandler(this.button_click);
		this.btn_00.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_00.Appearance.BackColor = System.Drawing.Color.SkyBlue;
		this.btn_00.Appearance.Font = new System.Drawing.Font("Arial", 27.97793f);
		this.btn_00.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_00.Appearance.Options.UseBackColor = true;
		this.btn_00.Appearance.Options.UseFont = true;
		this.btn_00.Appearance.Options.UseForeColor = true;
		this.btn_00.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_00.Location = new System.Drawing.Point(75, 263);
		this.btn_00.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_00.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_00.Name = "btn_00";
		this.btn_00.Size = new System.Drawing.Size(66, 59);
		this.btn_00.TabIndex = 41;
		this.btn_00.Text = "00";
		this.btn_00.Click += new System.EventHandler(this.button_click);
		this.btn_2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_2.Appearance.BackColor = System.Drawing.Color.SkyBlue;
		this.btn_2.Appearance.Font = new System.Drawing.Font("Arial", 29.76987f);
		this.btn_2.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_2.Appearance.Options.UseBackColor = true;
		this.btn_2.Appearance.Options.UseFont = true;
		this.btn_2.Appearance.Options.UseForeColor = true;
		this.btn_2.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_2.Location = new System.Drawing.Point(75, 198);
		this.btn_2.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_2.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_2.Name = "btn_2";
		this.btn_2.Size = new System.Drawing.Size(66, 59);
		this.btn_2.TabIndex = 33;
		this.btn_2.Text = "2";
		this.btn_2.Click += new System.EventHandler(this.button_click);
		this.btn_5.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_5.Appearance.BackColor = System.Drawing.Color.SkyBlue;
		this.btn_5.Appearance.Font = new System.Drawing.Font("Arial", 29.76987f);
		this.btn_5.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_5.Appearance.Options.UseBackColor = true;
		this.btn_5.Appearance.Options.UseFont = true;
		this.btn_5.Appearance.Options.UseForeColor = true;
		this.btn_5.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_5.Location = new System.Drawing.Point(75, 133);
		this.btn_5.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_5.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_5.Name = "btn_5";
		this.btn_5.Size = new System.Drawing.Size(66, 59);
		this.btn_5.TabIndex = 28;
		this.btn_5.Text = "5";
		this.btn_5.Click += new System.EventHandler(this.button_click);
		this.btn_8.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_8.Appearance.BackColor = System.Drawing.Color.SkyBlue;
		this.btn_8.Appearance.Font = new System.Drawing.Font("Arial", 29.76987f);
		this.btn_8.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_8.Appearance.Options.UseBackColor = true;
		this.btn_8.Appearance.Options.UseFont = true;
		this.btn_8.Appearance.Options.UseForeColor = true;
		this.btn_8.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_8.Location = new System.Drawing.Point(75, 68);
		this.btn_8.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_8.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_8.Name = "btn_8";
		this.btn_8.Size = new System.Drawing.Size(66, 59);
		this.btn_8.TabIndex = 23;
		this.btn_8.Text = "8";
		this.btn_8.Click += new System.EventHandler(this.button_click);
		this.btn_Dot.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Dot.Appearance.BackColor = System.Drawing.Color.SkyBlue;
		this.btn_Dot.Appearance.Font = new System.Drawing.Font("Arial", 29.73871f);
		this.btn_Dot.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Dot.Appearance.Options.UseBackColor = true;
		this.btn_Dot.Appearance.Options.UseFont = true;
		this.btn_Dot.Appearance.Options.UseForeColor = true;
		this.btn_Dot.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_Dot.Location = new System.Drawing.Point(147, 263);
		this.btn_Dot.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Dot.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Dot.Name = "btn_Dot";
		this.btn_Dot.Size = new System.Drawing.Size(66, 59);
		this.btn_Dot.TabIndex = 37;
		this.btn_Dot.Text = ".";
		this.btn_Dot.Click += new System.EventHandler(this.button_click);
		this.btn_3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_3.Appearance.BackColor = System.Drawing.Color.SkyBlue;
		this.btn_3.Appearance.Font = new System.Drawing.Font("Arial", 29.76987f);
		this.btn_3.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_3.Appearance.Options.UseBackColor = true;
		this.btn_3.Appearance.Options.UseFont = true;
		this.btn_3.Appearance.Options.UseForeColor = true;
		this.btn_3.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_3.Location = new System.Drawing.Point(147, 198);
		this.btn_3.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_3.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_3.Name = "btn_3";
		this.btn_3.Size = new System.Drawing.Size(66, 59);
		this.btn_3.TabIndex = 34;
		this.btn_3.Text = "3";
		this.btn_3.Click += new System.EventHandler(this.button_click);
		this.btn_6.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_6.Appearance.BackColor = System.Drawing.Color.SkyBlue;
		this.btn_6.Appearance.Font = new System.Drawing.Font("Arial", 29.76987f);
		this.btn_6.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_6.Appearance.Options.UseBackColor = true;
		this.btn_6.Appearance.Options.UseFont = true;
		this.btn_6.Appearance.Options.UseForeColor = true;
		this.btn_6.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_6.Location = new System.Drawing.Point(147, 133);
		this.btn_6.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_6.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_6.Name = "btn_6";
		this.btn_6.Size = new System.Drawing.Size(66, 59);
		this.btn_6.TabIndex = 29;
		this.btn_6.Text = "6";
		this.btn_6.Click += new System.EventHandler(this.button_click);
		this.btn_9.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_9.Appearance.BackColor = System.Drawing.Color.SkyBlue;
		this.btn_9.Appearance.Font = new System.Drawing.Font("Arial", 29.76987f);
		this.btn_9.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_9.Appearance.Options.UseBackColor = true;
		this.btn_9.Appearance.Options.UseFont = true;
		this.btn_9.Appearance.Options.UseForeColor = true;
		this.btn_9.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_9.Location = new System.Drawing.Point(147, 68);
		this.btn_9.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_9.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_9.Name = "btn_9";
		this.btn_9.Size = new System.Drawing.Size(66, 59);
		this.btn_9.TabIndex = 24;
		this.btn_9.Text = "9";
		this.btn_9.Click += new System.EventHandler(this.button_click);
		this.btn_Minus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Minus.Appearance.BackColor = System.Drawing.Color.SkyBlue;
		this.btn_Minus.Appearance.Font = new System.Drawing.Font("Arial", 29.73871f);
		this.btn_Minus.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Minus.Appearance.Options.UseBackColor = true;
		this.btn_Minus.Appearance.Options.UseFont = true;
		this.btn_Minus.Appearance.Options.UseForeColor = true;
		this.btn_Minus.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_Minus.Location = new System.Drawing.Point(219, 263);
		this.btn_Minus.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Minus.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Minus.Name = "btn_Minus";
		this.btn_Minus.Size = new System.Drawing.Size(66, 59);
		this.btn_Minus.TabIndex = 35;
		this.btn_Minus.Text = "-";
		this.btn_Minus.Click += new System.EventHandler(this.operator_click);
		this.btn_Plus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Plus.Appearance.BackColor = System.Drawing.Color.SkyBlue;
		this.btn_Plus.Appearance.Font = new System.Drawing.Font("Arial", 29.76987f);
		this.btn_Plus.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Plus.Appearance.Options.UseBackColor = true;
		this.btn_Plus.Appearance.Options.UseFont = true;
		this.btn_Plus.Appearance.Options.UseForeColor = true;
		this.btn_Plus.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_Plus.Location = new System.Drawing.Point(219, 198);
		this.btn_Plus.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Plus.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Plus.Name = "btn_Plus";
		this.btn_Plus.Size = new System.Drawing.Size(66, 59);
		this.btn_Plus.TabIndex = 38;
		this.btn_Plus.Text = "+";
		this.btn_Plus.Click += new System.EventHandler(this.operator_click);
		this.btn_multiple.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_multiple.Appearance.BackColor = System.Drawing.Color.SkyBlue;
		this.btn_multiple.Appearance.Font = new System.Drawing.Font("Arial", 29.76987f);
		this.btn_multiple.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_multiple.Appearance.Options.UseBackColor = true;
		this.btn_multiple.Appearance.Options.UseFont = true;
		this.btn_multiple.Appearance.Options.UseForeColor = true;
		this.btn_multiple.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_multiple.Location = new System.Drawing.Point(219, 133);
		this.btn_multiple.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_multiple.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_multiple.Name = "btn_multiple";
		this.btn_multiple.Size = new System.Drawing.Size(66, 59);
		this.btn_multiple.TabIndex = 30;
		this.btn_multiple.Text = "*";
		this.btn_multiple.Click += new System.EventHandler(this.operator_click);
		this.btn_Devide.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Devide.Appearance.BackColor = System.Drawing.Color.SkyBlue;
		this.btn_Devide.Appearance.Font = new System.Drawing.Font("Arial", 29.76987f);
		this.btn_Devide.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Devide.Appearance.Options.UseBackColor = true;
		this.btn_Devide.Appearance.Options.UseFont = true;
		this.btn_Devide.Appearance.Options.UseForeColor = true;
		this.btn_Devide.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_Devide.Location = new System.Drawing.Point(219, 68);
		this.btn_Devide.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Devide.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Devide.Name = "btn_Devide";
		this.btn_Devide.Size = new System.Drawing.Size(66, 59);
		this.btn_Devide.TabIndex = 25;
		this.btn_Devide.Text = "/";
		this.btn_Devide.Click += new System.EventHandler(this.operator_click);
		this.btn_Equal.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Equal.Appearance.BackColor = System.Drawing.Color.SkyBlue;
		this.btn_Equal.Appearance.Font = new System.Drawing.Font("Arial", 29.73867f);
		this.btn_Equal.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Equal.Appearance.Options.UseBackColor = true;
		this.btn_Equal.Appearance.Options.UseFont = true;
		this.btn_Equal.Appearance.Options.UseForeColor = true;
		this.btn_Equal.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_Equal.Location = new System.Drawing.Point(291, 263);
		this.btn_Equal.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Equal.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Equal.Name = "btn_Equal";
		this.btn_Equal.Size = new System.Drawing.Size(67, 59);
		this.btn_Equal.TabIndex = 39;
		this.btn_Equal.Text = "=";
		this.btn_Equal.Click += new System.EventHandler(this.btn_Equal_Click);
		this.btn_Submit.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Submit.Appearance.BackColor = System.Drawing.Color.FromArgb(128, 255, 128);
		this.btn_Submit.Appearance.Font = new System.Drawing.Font("Arial", 14.88341f);
		this.btn_Submit.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Submit.Appearance.Options.UseBackColor = true;
		this.btn_Submit.Appearance.Options.UseFont = true;
		this.btn_Submit.Appearance.Options.UseForeColor = true;
		this.btn_Submit.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_Submit.Location = new System.Drawing.Point(291, 198);
		this.btn_Submit.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Submit.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Submit.Name = "btn_Submit";
		this.btn_Submit.Size = new System.Drawing.Size(67, 59);
		this.btn_Submit.TabIndex = 20;
		this.btn_Submit.Text = "Enter";
		this.btn_Submit.Click += new System.EventHandler(this.btn_Submit_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 192, 192);
		this.btn_Cancel.Appearance.Font = new System.Drawing.Font("Arial", 29.76524f);
		this.btn_Cancel.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Cancel.Appearance.Options.UseBackColor = true;
		this.btn_Cancel.Appearance.Options.UseFont = true;
		this.btn_Cancel.Appearance.Options.UseForeColor = true;
		this.btn_Cancel.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_Cancel.Location = new System.Drawing.Point(291, 133);
		this.btn_Cancel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Cancel.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(67, 59);
		this.btn_Cancel.TabIndex = 31;
		this.btn_Cancel.Text = "C";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_CancelEntry.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_CancelEntry.Appearance.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.btn_CancelEntry.Appearance.Font = new System.Drawing.Font("Arial", 25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_CancelEntry.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_CancelEntry.Appearance.Options.UseBackColor = true;
		this.btn_CancelEntry.Appearance.Options.UseFont = true;
		this.btn_CancelEntry.Appearance.Options.UseForeColor = true;
		this.btn_CancelEntry.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.btn_CancelEntry.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CancelEntry.ImageOptions.Image");
		this.btn_CancelEntry.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.btn_CancelEntry.Location = new System.Drawing.Point(291, 68);
		this.btn_CancelEntry.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_CancelEntry.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_CancelEntry.Name = "btn_CancelEntry";
		this.btn_CancelEntry.Size = new System.Drawing.Size(67, 59);
		this.btn_CancelEntry.TabIndex = 26;
		this.btn_CancelEntry.Click += new System.EventHandler(this.btn_CancelEntry_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tableLayoutPanel1);
		base.Controls.Add(this.labelCurrentOperation);
		base.Name = "ucCalculator";
		base.Size = new System.Drawing.Size(361, 325);
		base.Load += new System.EventHandler(this.uc_Calculator_Load);
		this.tableLayoutPanel1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_Result.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
