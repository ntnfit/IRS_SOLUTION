using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Controls;
using Apzon.Controls.Controls;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;

namespace Apzon.Forms.Retail;

public class frm_OSKeyboard : Form
{
	private const int WM_MOUSEACTIVATE = 33;

	private const int MA_NOACTIVATE = 3;

	private IContainer components;

	private LayoutControl layoutControl1;

	private LayoutControlGroup layoutControlGroup1;

	private Panel panel1;

	private SimpleButtonEx buttonBack;

	private SimpleButtonEx buttonSpace;

	private SimpleButtonEx btn_Number_RSlash;

	private SimpleButtonEx buttonEnter;

	private SimpleButtonEx btn_Number_Equal;

	private SimpleButtonEx btn_Number_Slash;

	private SimpleButtonEx btn_Number_Dot;

	private SimpleButtonEx btn_Number_Colon;

	private SimpleButtonEx btn_Char_M;

	private SimpleButtonEx btn_Char_N;

	private SimpleButtonEx btn_Number_Apostrophe;

	private SimpleButtonEx btn_Number_Semicolon;

	private SimpleButtonEx btn_Char_L;

	private SimpleButtonEx btn_Char_K;

	private SimpleButtonEx btn_Char_J;

	private SimpleButtonEx btn_Char_H;

	private SimpleButtonEx btn_Number_RBracket;

	private SimpleButtonEx btn_Number_6LBracket;

	private SimpleButtonEx btn_Char_P;

	private SimpleButtonEx btn_Char_O;

	private SimpleButtonEx btn_Char_I;

	private SimpleButtonEx btn_Char_U;

	private SimpleButtonEx btn_Number_Minus;

	private SimpleButtonEx btn_Number_0;

	private SimpleButtonEx btn_Number_9;

	private SimpleButtonEx btn_Number_8;

	private SimpleButtonEx btn_Number_7;

	private SimpleButtonEx btn_Number_6;

	private SimpleButtonEx btn_Char_B;

	private SimpleButtonEx btn_Char_V;

	private SimpleButtonEx btn_Char_C;

	private SimpleButtonEx btn_Char_X;

	private SimpleButtonEx btn_Char_Z;

	private SimpleButtonEx btn_Char_G;

	private SimpleButtonEx btn_Char_F;

	private SimpleButtonEx btn_Char_D;

	private SimpleButtonEx btn_Char_S;

	private SimpleButtonEx btn_Char_A;

	private SimpleButtonEx btn_Char_Y;

	private SimpleButtonEx btn_Char_T;

	private SimpleButtonEx btn_Char_R;

	private SimpleButtonEx btn_Char_E;

	private SimpleButtonEx btn_Char_W;

	private SimpleButtonEx btn_Char_Q;

	private SimpleButtonEx btn_Number_5;

	private SimpleButtonEx btn_Number_4;

	private SimpleButtonEx btn_Number_3;

	private SimpleButtonEx btn_Number_2;

	private SimpleButtonEx btn_Number_1;

	private SimpleButtonEx btn_Number_99;

	private LayoutControlItemEx layoutControlItem1;

	private CheckButton checkButtonCaps;

	private CheckButton checkButtonRShift;

	private CheckButton checkButtonShift;

	private CheckButton checkButtonCtrl;

	private CheckButton checkButtonAlt;

	private SimpleButtonEx buttonDel;

	private SimpleButtonEx buttonTab;

	private SimpleButtonEx buttonUp;

	private SimpleButtonEx buttonLeft;

	private SimpleButtonEx buttonRight;

	private SimpleButtonEx buttonDown;

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams createParams = base.CreateParams;
			createParams.ExStyle |= 134217728;
			return createParams;
		}
	}

	public frm_OSKeyboard()
	{
		InitializeComponent();
	}

	protected override void WndProc(ref Message m)
	{
		if (m.Msg == 33)
		{
			m.Result = (IntPtr)3;
		}
		else
		{
			base.WndProc(ref m);
		}
	}

	private void SendAlphaKey(string key)
	{
		try
		{
			if (checkButtonShift.Checked || checkButtonRShift.Checked)
			{
				if (checkButtonCaps.Checked)
				{
					SendKeys.Send(key);
				}
				else
				{
					SendKeys.Send("+" + key);
				}
				checkButtonShift.Checked = false;
				checkButtonRShift.Checked = false;
				checkButtonCtrl.Checked = false;
				checkButtonAlt.Checked = false;
			}
			else if (checkButtonCaps.Checked)
			{
				if (checkButtonShift.Checked || checkButtonRShift.Checked)
				{
					SendKeys.Send(key);
				}
				else
				{
					SendKeys.Send("+" + key);
				}
			}
			else if (checkButtonCtrl.Checked)
			{
				SendKeys.Send("^" + key);
				checkButtonAlt.Checked = false;
				checkButtonCtrl.Checked = false;
				checkButtonShift.Checked = false;
				checkButtonRShift.Checked = false;
			}
			else if (checkButtonAlt.Checked)
			{
				SendKeys.Send("%" + key);
				checkButtonAlt.Checked = false;
				checkButtonCtrl.Checked = false;
				checkButtonShift.Checked = false;
				checkButtonRShift.Checked = false;
			}
			else
			{
				SendKeys.Send(key);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SendSymbolKey(string key)
	{
		try
		{
			if (checkButtonShift.Checked || checkButtonRShift.Checked)
			{
				SendKeys.Send("+" + key);
				checkButtonShift.Checked = false;
				checkButtonRShift.Checked = false;
			}
			else if (checkButtonCtrl.Checked)
			{
				SendKeys.Send("^" + key);
				checkButtonCtrl.Checked = false;
			}
			else if (checkButtonAlt.Checked)
			{
				SendKeys.Send("%" + key);
				checkButtonAlt.Checked = false;
			}
			else
			{
				SendKeys.Send(key);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SendOtherKey(string key)
	{
		try
		{
			SendKeys.Send("{" + key + "}");
			checkButtonShift.Checked = false;
			checkButtonRShift.Checked = false;
			checkButtonCtrl.Checked = false;
			checkButtonAlt.Checked = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void buttonAlphakey_Click(object sender, EventArgs e)
	{
		try
		{
			string toolTip = ((SimpleButton)sender).ToolTip;
			SendAlphaKey(toolTip);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void buttonSymbolkey_Click(object sender, EventArgs e)
	{
		try
		{
			string toolTip = ((SimpleButton)sender).ToolTip;
			SendSymbolKey(toolTip);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void buttonOtherkey_Click(object sender, EventArgs e)
	{
		try
		{
			string key = ((SimpleButton)sender).Text;
			SendOtherKey(key);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void buttonSpace_Click(object sender, EventArgs e)
	{
		try
		{
			SendKeys.Send(" ");
			checkButtonShift.Checked = false;
			checkButtonRShift.Checked = false;
			checkButtonCtrl.Checked = false;
			checkButtonAlt.Checked = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void checkButtonShift_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			Control.ControlCollection controls = panel1.Controls;
			if (checkButtonShift.Checked)
			{
				checkButtonRShift.Checked = false;
				for (int i = 0; i < controls.Count; i++)
				{
					Control control = controls[i];
					if (control.GetType().Name.Equals("SimpleButton"))
					{
						SimpleButton simpleButton = (SimpleButton)control;
						if (simpleButton.Name.StartsWith("btn_Char_"))
						{
							simpleButton.Text = (checkButtonCaps.Checked ? simpleButton.Text.ToLower() : simpleButton.Text.ToUpper());
						}
						else if (simpleButton.Name.StartsWith("btn_Number_"))
						{
							simpleButton.Text = Function.ToString(simpleButton.Tag);
						}
					}
				}
				return;
			}
			for (int j = 0; j < controls.Count; j++)
			{
				Control control2 = controls[j];
				if (control2.GetType().Name.Equals("SimpleButton"))
				{
					SimpleButton simpleButton2 = (SimpleButton)control2;
					if (simpleButton2.Name.StartsWith("btn_Char_"))
					{
						simpleButton2.Text = simpleButton2.Text.ToLower();
					}
					else if (simpleButton2.Name.StartsWith("btn_Number_"))
					{
						simpleButton2.Text = Function.ToString(simpleButton2.ToolTip);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void checkButtonRShift_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			Control.ControlCollection controls = panel1.Controls;
			if (checkButtonRShift.Checked)
			{
				checkButtonShift.Checked = false;
				for (int i = 0; i < controls.Count; i++)
				{
					Control control = controls[i];
					if (control.GetType().Name.Equals("SimpleButton"))
					{
						SimpleButton simpleButton = (SimpleButton)control;
						if (simpleButton.Name.StartsWith("btn_Char_"))
						{
							simpleButton.Text = (checkButtonCaps.Checked ? simpleButton.Text.ToLower() : simpleButton.Text.ToUpper());
						}
						else if (simpleButton.Name.StartsWith("btn_Number_"))
						{
							simpleButton.Text = Function.ToString(simpleButton.Tag);
						}
					}
				}
				return;
			}
			for (int j = 0; j < controls.Count; j++)
			{
				Control control2 = controls[j];
				if (control2.GetType().Name.Equals("SimpleButton"))
				{
					SimpleButton simpleButton2 = (SimpleButton)control2;
					if (simpleButton2.Name.StartsWith("btn_Char_"))
					{
						simpleButton2.Text = simpleButton2.Text.ToLower();
					}
					else if (simpleButton2.Name.StartsWith("btn_Number_"))
					{
						simpleButton2.Text = Function.ToString(simpleButton2.ToolTip);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void checkButtonCaps_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			Control.ControlCollection controls = panel1.Controls;
			if (checkButtonCaps.Checked)
			{
				for (int i = 0; i < controls.Count; i++)
				{
					Control control = controls[i];
					if (control.GetType().Name.Equals("SimpleButton"))
					{
						SimpleButton simpleButton = (SimpleButton)control;
						if (simpleButton.Name.StartsWith("btn_Char_"))
						{
							simpleButton.Text = ((checkButtonRShift.Checked || checkButtonShift.Checked) ? simpleButton.Text.ToLower() : simpleButton.Text.ToUpper());
						}
					}
				}
				return;
			}
			for (int j = 0; j < controls.Count; j++)
			{
				Control control2 = controls[j];
				if (control2.GetType().Name.Equals("SimpleButton"))
				{
					SimpleButton simpleButton2 = (SimpleButton)control2;
					if (simpleButton2.Name.StartsWith("btn_Char_"))
					{
						simpleButton2.Text = ((checkButtonRShift.Checked || checkButtonShift.Checked) ? simpleButton2.Text.ToUpper() : simpleButton2.Text.ToLower());
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void checkButtonCtrl_CheckedChanged(object sender, EventArgs e)
	{
	}

	private void checkButtonAlt_CheckedChanged(object sender, EventArgs e)
	{
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
		this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
		this.panel1 = new System.Windows.Forms.Panel();
		this.buttonUp = new Apzon.Controls.Controls.SimpleButtonEx();
		this.buttonLeft = new Apzon.Controls.Controls.SimpleButtonEx();
		this.buttonRight = new Apzon.Controls.Controls.SimpleButtonEx();
		this.buttonDown = new Apzon.Controls.Controls.SimpleButtonEx();
		this.buttonDel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.buttonTab = new Apzon.Controls.Controls.SimpleButtonEx();
		this.checkButtonAlt = new DevExpress.XtraEditors.CheckButton();
		this.checkButtonCtrl = new DevExpress.XtraEditors.CheckButton();
		this.checkButtonRShift = new DevExpress.XtraEditors.CheckButton();
		this.checkButtonShift = new DevExpress.XtraEditors.CheckButton();
		this.checkButtonCaps = new DevExpress.XtraEditors.CheckButton();
		this.buttonBack = new Apzon.Controls.Controls.SimpleButtonEx();
		this.buttonSpace = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_RSlash = new Apzon.Controls.Controls.SimpleButtonEx();
		this.buttonEnter = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_Equal = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_Slash = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_Dot = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_Colon = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_M = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_N = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_Apostrophe = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_Semicolon = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_L = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_K = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_J = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_H = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_RBracket = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_6LBracket = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_P = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_O = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_I = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_U = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_Minus = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_0 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_9 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_8 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_7 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_6 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_B = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_V = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_C = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_X = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_Z = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_G = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_F = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_D = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_S = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_A = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_Y = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_T = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_R = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_E = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_W = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Char_Q = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_5 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_4 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_3 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_2 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_1 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Number_99 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new Apzon.Controls.Controls.LayoutControlItemEx();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		base.SuspendLayout();
		this.layoutControl1.Controls.Add(this.panel1);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(1019, 356);
		this.layoutControl1.TabIndex = 0;
		this.layoutControl1.Text = "layoutControl1";
		this.panel1.BackColor = System.Drawing.SystemColors.Control;
		this.panel1.Controls.Add(this.buttonUp);
		this.panel1.Controls.Add(this.buttonLeft);
		this.panel1.Controls.Add(this.buttonRight);
		this.panel1.Controls.Add(this.buttonDown);
		this.panel1.Controls.Add(this.buttonDel);
		this.panel1.Controls.Add(this.buttonTab);
		this.panel1.Controls.Add(this.checkButtonAlt);
		this.panel1.Controls.Add(this.checkButtonCtrl);
		this.panel1.Controls.Add(this.checkButtonRShift);
		this.panel1.Controls.Add(this.checkButtonShift);
		this.panel1.Controls.Add(this.checkButtonCaps);
		this.panel1.Controls.Add(this.buttonBack);
		this.panel1.Controls.Add(this.buttonSpace);
		this.panel1.Controls.Add(this.btn_Number_RSlash);
		this.panel1.Controls.Add(this.buttonEnter);
		this.panel1.Controls.Add(this.btn_Number_Equal);
		this.panel1.Controls.Add(this.btn_Number_Slash);
		this.panel1.Controls.Add(this.btn_Number_Dot);
		this.panel1.Controls.Add(this.btn_Number_Colon);
		this.panel1.Controls.Add(this.btn_Char_M);
		this.panel1.Controls.Add(this.btn_Char_N);
		this.panel1.Controls.Add(this.btn_Number_Apostrophe);
		this.panel1.Controls.Add(this.btn_Number_Semicolon);
		this.panel1.Controls.Add(this.btn_Char_L);
		this.panel1.Controls.Add(this.btn_Char_K);
		this.panel1.Controls.Add(this.btn_Char_J);
		this.panel1.Controls.Add(this.btn_Char_H);
		this.panel1.Controls.Add(this.btn_Number_RBracket);
		this.panel1.Controls.Add(this.btn_Number_6LBracket);
		this.panel1.Controls.Add(this.btn_Char_P);
		this.panel1.Controls.Add(this.btn_Char_O);
		this.panel1.Controls.Add(this.btn_Char_I);
		this.panel1.Controls.Add(this.btn_Char_U);
		this.panel1.Controls.Add(this.btn_Number_Minus);
		this.panel1.Controls.Add(this.btn_Number_0);
		this.panel1.Controls.Add(this.btn_Number_9);
		this.panel1.Controls.Add(this.btn_Number_8);
		this.panel1.Controls.Add(this.btn_Number_7);
		this.panel1.Controls.Add(this.btn_Number_6);
		this.panel1.Controls.Add(this.btn_Char_B);
		this.panel1.Controls.Add(this.btn_Char_V);
		this.panel1.Controls.Add(this.btn_Char_C);
		this.panel1.Controls.Add(this.btn_Char_X);
		this.panel1.Controls.Add(this.btn_Char_Z);
		this.panel1.Controls.Add(this.btn_Char_G);
		this.panel1.Controls.Add(this.btn_Char_F);
		this.panel1.Controls.Add(this.btn_Char_D);
		this.panel1.Controls.Add(this.btn_Char_S);
		this.panel1.Controls.Add(this.btn_Char_A);
		this.panel1.Controls.Add(this.btn_Char_Y);
		this.panel1.Controls.Add(this.btn_Char_T);
		this.panel1.Controls.Add(this.btn_Char_R);
		this.panel1.Controls.Add(this.btn_Char_E);
		this.panel1.Controls.Add(this.btn_Char_W);
		this.panel1.Controls.Add(this.btn_Char_Q);
		this.panel1.Controls.Add(this.btn_Number_5);
		this.panel1.Controls.Add(this.btn_Number_4);
		this.panel1.Controls.Add(this.btn_Number_3);
		this.panel1.Controls.Add(this.btn_Number_2);
		this.panel1.Controls.Add(this.btn_Number_1);
		this.panel1.Controls.Add(this.btn_Number_99);
		this.panel1.Location = new System.Drawing.Point(12, 12);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(995, 332);
		this.panel1.TabIndex = 4;
		this.buttonUp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.buttonUp.Appearance.BackColor = System.Drawing.Color.White;
		this.buttonUp.Appearance.Font = new System.Drawing.Font("Tahoma", 20f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.buttonUp.Appearance.ForeColor = System.Drawing.Color.Black;
		this.buttonUp.Appearance.Options.UseBackColor = true;
		this.buttonUp.Appearance.Options.UseFont = true;
		this.buttonUp.Appearance.Options.UseForeColor = true;
		this.buttonUp.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.buttonUp.AppearanceHovered.Options.UseBackColor = true;
		this.buttonUp.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.buttonUp.AppearancePressed.Options.UseBackColor = true;
		this.buttonUp.AutoWidthInLayoutControl = true;
		this.buttonUp.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.buttonUp.Location = new System.Drawing.Point(798, 201);
		this.buttonUp.Name = "buttonUp";
		this.buttonUp.Size = new System.Drawing.Size(60, 60);
		this.buttonUp.TabIndex = 63;
		this.buttonUp.Text = "Up";
		this.buttonUp.Click += new System.EventHandler(this.buttonOtherkey_Click);
		this.buttonLeft.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.buttonLeft.Appearance.BackColor = System.Drawing.Color.White;
		this.buttonLeft.Appearance.Font = new System.Drawing.Font("Tahoma", 20f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.buttonLeft.Appearance.ForeColor = System.Drawing.Color.Black;
		this.buttonLeft.Appearance.Options.UseBackColor = true;
		this.buttonLeft.Appearance.Options.UseFont = true;
		this.buttonLeft.Appearance.Options.UseForeColor = true;
		this.buttonLeft.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.buttonLeft.AppearanceHovered.Options.UseBackColor = true;
		this.buttonLeft.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.buttonLeft.AppearancePressed.Options.UseBackColor = true;
		this.buttonLeft.AutoWidthInLayoutControl = true;
		this.buttonLeft.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.buttonLeft.Location = new System.Drawing.Point(732, 267);
		this.buttonLeft.Name = "buttonLeft";
		this.buttonLeft.Size = new System.Drawing.Size(60, 60);
		this.buttonLeft.TabIndex = 62;
		this.buttonLeft.Text = "Left";
		this.buttonLeft.Click += new System.EventHandler(this.buttonOtherkey_Click);
		this.buttonRight.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.buttonRight.Appearance.BackColor = System.Drawing.Color.White;
		this.buttonRight.Appearance.Font = new System.Drawing.Font("Tahoma", 20f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.buttonRight.Appearance.ForeColor = System.Drawing.Color.Black;
		this.buttonRight.Appearance.Options.UseBackColor = true;
		this.buttonRight.Appearance.Options.UseFont = true;
		this.buttonRight.Appearance.Options.UseForeColor = true;
		this.buttonRight.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.buttonRight.AppearanceHovered.Options.UseBackColor = true;
		this.buttonRight.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.buttonRight.AppearancePressed.Options.UseBackColor = true;
		this.buttonRight.AutoWidthInLayoutControl = true;
		this.buttonRight.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.buttonRight.Location = new System.Drawing.Point(864, 267);
		this.buttonRight.Name = "buttonRight";
		this.buttonRight.Size = new System.Drawing.Size(60, 60);
		this.buttonRight.TabIndex = 61;
		this.buttonRight.Text = "Right";
		this.buttonRight.Click += new System.EventHandler(this.buttonOtherkey_Click);
		this.buttonDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.buttonDown.Appearance.BackColor = System.Drawing.Color.White;
		this.buttonDown.Appearance.Font = new System.Drawing.Font("Tahoma", 20f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.buttonDown.Appearance.ForeColor = System.Drawing.Color.Black;
		this.buttonDown.Appearance.Options.UseBackColor = true;
		this.buttonDown.Appearance.Options.UseFont = true;
		this.buttonDown.Appearance.Options.UseForeColor = true;
		this.buttonDown.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.buttonDown.AppearanceHovered.Options.UseBackColor = true;
		this.buttonDown.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.buttonDown.AppearancePressed.Options.UseBackColor = true;
		this.buttonDown.AutoWidthInLayoutControl = true;
		this.buttonDown.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.buttonDown.Location = new System.Drawing.Point(798, 267);
		this.buttonDown.Name = "buttonDown";
		this.buttonDown.Size = new System.Drawing.Size(60, 60);
		this.buttonDown.TabIndex = 60;
		this.buttonDown.Text = "Down";
		this.buttonDown.Click += new System.EventHandler(this.buttonOtherkey_Click);
		this.buttonDel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.buttonDel.Appearance.BackColor = System.Drawing.Color.White;
		this.buttonDel.Appearance.Font = new System.Drawing.Font("Tahoma", 25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.buttonDel.Appearance.ForeColor = System.Drawing.Color.Black;
		this.buttonDel.Appearance.Options.UseBackColor = true;
		this.buttonDel.Appearance.Options.UseFont = true;
		this.buttonDel.Appearance.Options.UseForeColor = true;
		this.buttonDel.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.buttonDel.AppearanceHovered.Options.UseBackColor = true;
		this.buttonDel.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.buttonDel.AppearancePressed.Options.UseBackColor = true;
		this.buttonDel.AutoWidthInLayoutControl = true;
		this.buttonDel.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.buttonDel.Location = new System.Drawing.Point(928, 267);
		this.buttonDel.Name = "buttonDel";
		this.buttonDel.Size = new System.Drawing.Size(62, 60);
		this.buttonDel.TabIndex = 59;
		this.buttonDel.Text = "Del";
		this.buttonDel.Click += new System.EventHandler(this.buttonOtherkey_Click);
		this.buttonTab.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.buttonTab.Appearance.BackColor = System.Drawing.Color.White;
		this.buttonTab.Appearance.Font = new System.Drawing.Font("Tahoma", 25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.buttonTab.Appearance.ForeColor = System.Drawing.Color.Black;
		this.buttonTab.Appearance.Options.UseBackColor = true;
		this.buttonTab.Appearance.Options.UseFont = true;
		this.buttonTab.Appearance.Options.UseForeColor = true;
		this.buttonTab.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.buttonTab.AppearanceHovered.Options.UseBackColor = true;
		this.buttonTab.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.buttonTab.AppearancePressed.Options.UseBackColor = true;
		this.buttonTab.AutoWidthInLayoutControl = true;
		this.buttonTab.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.buttonTab.Location = new System.Drawing.Point(3, 69);
		this.buttonTab.Name = "buttonTab";
		this.buttonTab.Size = new System.Drawing.Size(126, 60);
		this.buttonTab.TabIndex = 58;
		this.buttonTab.Text = "Tab";
		this.buttonTab.Click += new System.EventHandler(this.buttonOtherkey_Click);
		this.checkButtonAlt.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.checkButtonAlt.Appearance.BackColor = System.Drawing.Color.White;
		this.checkButtonAlt.Appearance.Font = new System.Drawing.Font("Tahoma", 25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.checkButtonAlt.Appearance.ForeColor = System.Drawing.Color.Black;
		this.checkButtonAlt.Appearance.Options.UseBackColor = true;
		this.checkButtonAlt.Appearance.Options.UseFont = true;
		this.checkButtonAlt.Appearance.Options.UseForeColor = true;
		this.checkButtonAlt.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.checkButtonAlt.AppearanceHovered.Options.UseBackColor = true;
		this.checkButtonAlt.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.checkButtonAlt.AppearancePressed.Options.UseBackColor = true;
		this.checkButtonAlt.AutoWidthInLayoutControl = true;
		this.checkButtonAlt.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.checkButtonAlt.Location = new System.Drawing.Point(135, 267);
		this.checkButtonAlt.Name = "checkButtonAlt";
		this.checkButtonAlt.Size = new System.Drawing.Size(60, 60);
		this.checkButtonAlt.TabIndex = 57;
		this.checkButtonAlt.Text = "Alt";
		this.checkButtonAlt.CheckedChanged += new System.EventHandler(this.checkButtonAlt_CheckedChanged);
		this.checkButtonCtrl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.checkButtonCtrl.Appearance.BackColor = System.Drawing.Color.White;
		this.checkButtonCtrl.Appearance.Font = new System.Drawing.Font("Tahoma", 25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.checkButtonCtrl.Appearance.ForeColor = System.Drawing.Color.Black;
		this.checkButtonCtrl.Appearance.Options.UseBackColor = true;
		this.checkButtonCtrl.Appearance.Options.UseFont = true;
		this.checkButtonCtrl.Appearance.Options.UseForeColor = true;
		this.checkButtonCtrl.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.checkButtonCtrl.AppearanceHovered.Options.UseBackColor = true;
		this.checkButtonCtrl.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.checkButtonCtrl.AppearancePressed.Options.UseBackColor = true;
		this.checkButtonCtrl.AutoWidthInLayoutControl = true;
		this.checkButtonCtrl.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.checkButtonCtrl.Location = new System.Drawing.Point(3, 267);
		this.checkButtonCtrl.Name = "checkButtonCtrl";
		this.checkButtonCtrl.Size = new System.Drawing.Size(126, 60);
		this.checkButtonCtrl.TabIndex = 56;
		this.checkButtonCtrl.Text = "Ctrl";
		this.checkButtonCtrl.CheckedChanged += new System.EventHandler(this.checkButtonCtrl_CheckedChanged);
		this.checkButtonRShift.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.checkButtonRShift.Appearance.BackColor = System.Drawing.Color.White;
		this.checkButtonRShift.Appearance.Font = new System.Drawing.Font("Tahoma", 25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.checkButtonRShift.Appearance.ForeColor = System.Drawing.Color.Black;
		this.checkButtonRShift.Appearance.Options.UseBackColor = true;
		this.checkButtonRShift.Appearance.Options.UseFont = true;
		this.checkButtonRShift.Appearance.Options.UseForeColor = true;
		this.checkButtonRShift.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.checkButtonRShift.AppearanceHovered.Options.UseBackColor = true;
		this.checkButtonRShift.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.checkButtonRShift.AppearancePressed.Options.UseBackColor = true;
		this.checkButtonRShift.AutoWidthInLayoutControl = true;
		this.checkButtonRShift.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.checkButtonRShift.Location = new System.Drawing.Point(863, 201);
		this.checkButtonRShift.Name = "checkButtonRShift";
		this.checkButtonRShift.Size = new System.Drawing.Size(127, 60);
		this.checkButtonRShift.TabIndex = 55;
		this.checkButtonRShift.Text = "Shift";
		this.checkButtonRShift.CheckedChanged += new System.EventHandler(this.checkButtonRShift_CheckedChanged);
		this.checkButtonShift.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.checkButtonShift.Appearance.BackColor = System.Drawing.Color.White;
		this.checkButtonShift.Appearance.Font = new System.Drawing.Font("Tahoma", 25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.checkButtonShift.Appearance.ForeColor = System.Drawing.Color.Black;
		this.checkButtonShift.Appearance.Options.UseBackColor = true;
		this.checkButtonShift.Appearance.Options.UseFont = true;
		this.checkButtonShift.Appearance.Options.UseForeColor = true;
		this.checkButtonShift.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.checkButtonShift.AppearanceHovered.Options.UseBackColor = true;
		this.checkButtonShift.AppearancePressed.BackColor = System.Drawing.Color.Gray;
		this.checkButtonShift.AppearancePressed.Font = new System.Drawing.Font("Tahoma", 25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.checkButtonShift.AppearancePressed.ForeColor = System.Drawing.Color.White;
		this.checkButtonShift.AppearancePressed.Options.UseBackColor = true;
		this.checkButtonShift.AppearancePressed.Options.UseFont = true;
		this.checkButtonShift.AppearancePressed.Options.UseForeColor = true;
		this.checkButtonShift.AutoWidthInLayoutControl = true;
		this.checkButtonShift.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.checkButtonShift.Location = new System.Drawing.Point(3, 201);
		this.checkButtonShift.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.checkButtonShift.LookAndFeel.UseDefaultLookAndFeel = false;
		this.checkButtonShift.Name = "checkButtonShift";
		this.checkButtonShift.Size = new System.Drawing.Size(126, 60);
		this.checkButtonShift.TabIndex = 54;
		this.checkButtonShift.Text = "Shift";
		this.checkButtonShift.CheckedChanged += new System.EventHandler(this.checkButtonShift_CheckedChanged);
		this.checkButtonCaps.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.checkButtonCaps.Appearance.BackColor = System.Drawing.Color.White;
		this.checkButtonCaps.Appearance.Font = new System.Drawing.Font("Tahoma", 25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.checkButtonCaps.Appearance.ForeColor = System.Drawing.Color.Black;
		this.checkButtonCaps.Appearance.Options.UseBackColor = true;
		this.checkButtonCaps.Appearance.Options.UseFont = true;
		this.checkButtonCaps.Appearance.Options.UseForeColor = true;
		this.checkButtonCaps.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.checkButtonCaps.AppearanceHovered.Options.UseBackColor = true;
		this.checkButtonCaps.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.checkButtonCaps.AppearancePressed.Options.UseBackColor = true;
		this.checkButtonCaps.AutoWidthInLayoutControl = true;
		this.checkButtonCaps.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.checkButtonCaps.Location = new System.Drawing.Point(3, 135);
		this.checkButtonCaps.Name = "checkButtonCaps";
		this.checkButtonCaps.Size = new System.Drawing.Size(126, 60);
		this.checkButtonCaps.TabIndex = 53;
		this.checkButtonCaps.Text = "Caps Lock";
		this.checkButtonCaps.CheckedChanged += new System.EventHandler(this.checkButtonCaps_CheckedChanged);
		this.buttonBack.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.buttonBack.Appearance.BackColor = System.Drawing.Color.White;
		this.buttonBack.Appearance.Font = new System.Drawing.Font("Tahoma", 25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.buttonBack.Appearance.ForeColor = System.Drawing.Color.Black;
		this.buttonBack.Appearance.Options.UseBackColor = true;
		this.buttonBack.Appearance.Options.UseFont = true;
		this.buttonBack.Appearance.Options.UseForeColor = true;
		this.buttonBack.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.buttonBack.AppearanceHovered.Options.UseBackColor = true;
		this.buttonBack.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.buttonBack.AppearancePressed.Options.UseBackColor = true;
		this.buttonBack.AutoWidthInLayoutControl = true;
		this.buttonBack.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.buttonBack.Location = new System.Drawing.Point(863, 3);
		this.buttonBack.Name = "buttonBack";
		this.buttonBack.Size = new System.Drawing.Size(126, 60);
		this.buttonBack.TabIndex = 52;
		this.buttonBack.Text = "Backspace";
		this.buttonBack.Click += new System.EventHandler(this.buttonOtherkey_Click);
		this.buttonSpace.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.buttonSpace.Appearance.BackColor = System.Drawing.Color.White;
		this.buttonSpace.Appearance.Font = new System.Drawing.Font("Tahoma", 25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.buttonSpace.Appearance.ForeColor = System.Drawing.Color.Black;
		this.buttonSpace.Appearance.Options.UseBackColor = true;
		this.buttonSpace.Appearance.Options.UseFont = true;
		this.buttonSpace.Appearance.Options.UseForeColor = true;
		this.buttonSpace.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.buttonSpace.AppearanceHovered.Options.UseBackColor = true;
		this.buttonSpace.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.buttonSpace.AppearancePressed.Options.UseBackColor = true;
		this.buttonSpace.AutoWidthInLayoutControl = true;
		this.buttonSpace.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.buttonSpace.Location = new System.Drawing.Point(202, 267);
		this.buttonSpace.Name = "buttonSpace";
		this.buttonSpace.Size = new System.Drawing.Size(523, 60);
		this.buttonSpace.TabIndex = 51;
		this.buttonSpace.Text = "Space";
		this.buttonSpace.Click += new System.EventHandler(this.buttonSpace_Click);
		this.btn_Number_RSlash.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_RSlash.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_RSlash.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_RSlash.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_RSlash.Appearance.Options.UseBackColor = true;
		this.btn_Number_RSlash.Appearance.Options.UseFont = true;
		this.btn_Number_RSlash.Appearance.Options.UseForeColor = true;
		this.btn_Number_RSlash.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_RSlash.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_RSlash.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_RSlash.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_RSlash.AutoWidthInLayoutControl = true;
		this.btn_Number_RSlash.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_RSlash.Location = new System.Drawing.Point(928, 69);
		this.btn_Number_RSlash.Name = "btn_Number_RSlash";
		this.btn_Number_RSlash.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_RSlash.TabIndex = 50;
		this.btn_Number_RSlash.Tag = "|";
		this.btn_Number_RSlash.Text = "\\";
		this.btn_Number_RSlash.ToolTip = "\\";
		this.btn_Number_RSlash.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.buttonEnter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.buttonEnter.Appearance.BackColor = System.Drawing.Color.White;
		this.buttonEnter.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.buttonEnter.Appearance.ForeColor = System.Drawing.Color.Black;
		this.buttonEnter.Appearance.Options.UseBackColor = true;
		this.buttonEnter.Appearance.Options.UseFont = true;
		this.buttonEnter.Appearance.Options.UseForeColor = true;
		this.buttonEnter.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.buttonEnter.AppearanceHovered.Options.UseBackColor = true;
		this.buttonEnter.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.buttonEnter.AppearancePressed.Options.UseBackColor = true;
		this.buttonEnter.AutoWidthInLayoutControl = true;
		this.buttonEnter.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.buttonEnter.Location = new System.Drawing.Point(864, 135);
		this.buttonEnter.Name = "buttonEnter";
		this.buttonEnter.Size = new System.Drawing.Size(125, 60);
		this.buttonEnter.TabIndex = 49;
		this.buttonEnter.Text = "Enter";
		this.buttonEnter.Click += new System.EventHandler(this.buttonOtherkey_Click);
		this.btn_Number_Equal.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_Equal.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_Equal.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_Equal.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_Equal.Appearance.Options.UseBackColor = true;
		this.btn_Number_Equal.Appearance.Options.UseFont = true;
		this.btn_Number_Equal.Appearance.Options.UseForeColor = true;
		this.btn_Number_Equal.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_Equal.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_Equal.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_Equal.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_Equal.AutoWidthInLayoutControl = true;
		this.btn_Number_Equal.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_Equal.Location = new System.Drawing.Point(797, 3);
		this.btn_Number_Equal.Name = "btn_Number_Equal";
		this.btn_Number_Equal.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_Equal.TabIndex = 48;
		this.btn_Number_Equal.Tag = "+";
		this.btn_Number_Equal.Text = "=";
		this.btn_Number_Equal.ToolTip = "=";
		this.btn_Number_Equal.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.btn_Number_Slash.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_Slash.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_Slash.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_Slash.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_Slash.Appearance.Options.UseBackColor = true;
		this.btn_Number_Slash.Appearance.Options.UseFont = true;
		this.btn_Number_Slash.Appearance.Options.UseForeColor = true;
		this.btn_Number_Slash.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_Slash.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_Slash.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_Slash.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_Slash.AutoWidthInLayoutControl = true;
		this.btn_Number_Slash.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_Slash.Location = new System.Drawing.Point(731, 201);
		this.btn_Number_Slash.Name = "btn_Number_Slash";
		this.btn_Number_Slash.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_Slash.TabIndex = 46;
		this.btn_Number_Slash.Tag = "?";
		this.btn_Number_Slash.Text = "/";
		this.btn_Number_Slash.ToolTip = "/";
		this.btn_Number_Slash.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.btn_Number_Dot.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_Dot.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_Dot.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_Dot.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_Dot.Appearance.Options.UseBackColor = true;
		this.btn_Number_Dot.Appearance.Options.UseFont = true;
		this.btn_Number_Dot.Appearance.Options.UseForeColor = true;
		this.btn_Number_Dot.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_Dot.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_Dot.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_Dot.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_Dot.AutoWidthInLayoutControl = true;
		this.btn_Number_Dot.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_Dot.Location = new System.Drawing.Point(665, 201);
		this.btn_Number_Dot.Name = "btn_Number_Dot";
		this.btn_Number_Dot.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_Dot.TabIndex = 45;
		this.btn_Number_Dot.Tag = ">";
		this.btn_Number_Dot.Text = ".";
		this.btn_Number_Dot.ToolTip = ".";
		this.btn_Number_Dot.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.btn_Number_Colon.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_Colon.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_Colon.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_Colon.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_Colon.Appearance.Options.UseBackColor = true;
		this.btn_Number_Colon.Appearance.Options.UseFont = true;
		this.btn_Number_Colon.Appearance.Options.UseForeColor = true;
		this.btn_Number_Colon.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_Colon.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_Colon.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_Colon.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_Colon.AutoWidthInLayoutControl = true;
		this.btn_Number_Colon.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_Colon.Location = new System.Drawing.Point(598, 201);
		this.btn_Number_Colon.Name = "btn_Number_Colon";
		this.btn_Number_Colon.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_Colon.TabIndex = 44;
		this.btn_Number_Colon.Tag = "<";
		this.btn_Number_Colon.Text = ",";
		this.btn_Number_Colon.ToolTip = ",";
		this.btn_Number_Colon.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.btn_Char_M.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_M.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_M.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_M.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_M.Appearance.Options.UseBackColor = true;
		this.btn_Char_M.Appearance.Options.UseFont = true;
		this.btn_Char_M.Appearance.Options.UseForeColor = true;
		this.btn_Char_M.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_M.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_M.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_M.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_M.AutoWidthInLayoutControl = true;
		this.btn_Char_M.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_M.Location = new System.Drawing.Point(532, 201);
		this.btn_Char_M.Name = "btn_Char_M";
		this.btn_Char_M.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_M.TabIndex = 43;
		this.btn_Char_M.Tag = "M";
		this.btn_Char_M.Text = "m";
		this.btn_Char_M.ToolTip = "m";
		this.btn_Char_M.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_N.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_N.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_N.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_N.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_N.Appearance.Options.UseBackColor = true;
		this.btn_Char_N.Appearance.Options.UseFont = true;
		this.btn_Char_N.Appearance.Options.UseForeColor = true;
		this.btn_Char_N.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_N.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_N.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_N.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_N.AutoWidthInLayoutControl = true;
		this.btn_Char_N.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_N.Location = new System.Drawing.Point(466, 201);
		this.btn_Char_N.Name = "btn_Char_N";
		this.btn_Char_N.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_N.TabIndex = 42;
		this.btn_Char_N.Tag = "N";
		this.btn_Char_N.Text = "n";
		this.btn_Char_N.ToolTip = "n";
		this.btn_Char_N.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Number_Apostrophe.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_Apostrophe.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_Apostrophe.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_Apostrophe.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_Apostrophe.Appearance.Options.UseBackColor = true;
		this.btn_Number_Apostrophe.Appearance.Options.UseFont = true;
		this.btn_Number_Apostrophe.Appearance.Options.UseForeColor = true;
		this.btn_Number_Apostrophe.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_Apostrophe.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_Apostrophe.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_Apostrophe.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_Apostrophe.AutoWidthInLayoutControl = true;
		this.btn_Number_Apostrophe.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_Apostrophe.Location = new System.Drawing.Point(797, 135);
		this.btn_Number_Apostrophe.Name = "btn_Number_Apostrophe";
		this.btn_Number_Apostrophe.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_Apostrophe.TabIndex = 41;
		this.btn_Number_Apostrophe.Tag = "\"";
		this.btn_Number_Apostrophe.Text = "'";
		this.btn_Number_Apostrophe.ToolTip = "'";
		this.btn_Number_Apostrophe.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.btn_Number_Semicolon.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_Semicolon.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_Semicolon.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_Semicolon.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_Semicolon.Appearance.Options.UseBackColor = true;
		this.btn_Number_Semicolon.Appearance.Options.UseFont = true;
		this.btn_Number_Semicolon.Appearance.Options.UseForeColor = true;
		this.btn_Number_Semicolon.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_Semicolon.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_Semicolon.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_Semicolon.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_Semicolon.AutoWidthInLayoutControl = true;
		this.btn_Number_Semicolon.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_Semicolon.Location = new System.Drawing.Point(731, 135);
		this.btn_Number_Semicolon.Name = "btn_Number_Semicolon";
		this.btn_Number_Semicolon.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_Semicolon.TabIndex = 40;
		this.btn_Number_Semicolon.Tag = ":";
		this.btn_Number_Semicolon.Text = ";";
		this.btn_Number_Semicolon.ToolTip = ";";
		this.btn_Number_Semicolon.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.btn_Char_L.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_L.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_L.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_L.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_L.Appearance.Options.UseBackColor = true;
		this.btn_Char_L.Appearance.Options.UseFont = true;
		this.btn_Char_L.Appearance.Options.UseForeColor = true;
		this.btn_Char_L.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_L.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_L.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_L.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_L.AutoWidthInLayoutControl = true;
		this.btn_Char_L.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_L.Location = new System.Drawing.Point(665, 135);
		this.btn_Char_L.Name = "btn_Char_L";
		this.btn_Char_L.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_L.TabIndex = 39;
		this.btn_Char_L.Tag = "L";
		this.btn_Char_L.Text = "l";
		this.btn_Char_L.ToolTip = "l";
		this.btn_Char_L.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_K.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_K.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_K.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_K.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_K.Appearance.Options.UseBackColor = true;
		this.btn_Char_K.Appearance.Options.UseFont = true;
		this.btn_Char_K.Appearance.Options.UseForeColor = true;
		this.btn_Char_K.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_K.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_K.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_K.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_K.AutoWidthInLayoutControl = true;
		this.btn_Char_K.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_K.Location = new System.Drawing.Point(598, 135);
		this.btn_Char_K.Name = "btn_Char_K";
		this.btn_Char_K.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_K.TabIndex = 38;
		this.btn_Char_K.Tag = "K";
		this.btn_Char_K.Text = "k";
		this.btn_Char_K.ToolTip = "k";
		this.btn_Char_K.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_J.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_J.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_J.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_J.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_J.Appearance.Options.UseBackColor = true;
		this.btn_Char_J.Appearance.Options.UseFont = true;
		this.btn_Char_J.Appearance.Options.UseForeColor = true;
		this.btn_Char_J.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_J.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_J.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_J.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_J.AutoWidthInLayoutControl = true;
		this.btn_Char_J.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_J.Location = new System.Drawing.Point(532, 135);
		this.btn_Char_J.Name = "btn_Char_J";
		this.btn_Char_J.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_J.TabIndex = 37;
		this.btn_Char_J.Tag = "J";
		this.btn_Char_J.Text = "j";
		this.btn_Char_J.ToolTip = "j";
		this.btn_Char_J.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_H.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_H.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_H.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_H.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_H.Appearance.Options.UseBackColor = true;
		this.btn_Char_H.Appearance.Options.UseFont = true;
		this.btn_Char_H.Appearance.Options.UseForeColor = true;
		this.btn_Char_H.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_H.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_H.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_H.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_H.AutoWidthInLayoutControl = true;
		this.btn_Char_H.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_H.Location = new System.Drawing.Point(466, 135);
		this.btn_Char_H.Name = "btn_Char_H";
		this.btn_Char_H.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_H.TabIndex = 36;
		this.btn_Char_H.Tag = "H";
		this.btn_Char_H.Text = "h";
		this.btn_Char_H.ToolTip = "h";
		this.btn_Char_H.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Number_RBracket.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_RBracket.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_RBracket.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_RBracket.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_RBracket.Appearance.Options.UseBackColor = true;
		this.btn_Number_RBracket.Appearance.Options.UseFont = true;
		this.btn_Number_RBracket.Appearance.Options.UseForeColor = true;
		this.btn_Number_RBracket.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_RBracket.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_RBracket.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_RBracket.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_RBracket.AutoWidthInLayoutControl = true;
		this.btn_Number_RBracket.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_RBracket.Location = new System.Drawing.Point(864, 69);
		this.btn_Number_RBracket.Name = "btn_Number_RBracket";
		this.btn_Number_RBracket.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_RBracket.TabIndex = 35;
		this.btn_Number_RBracket.Tag = "}";
		this.btn_Number_RBracket.Text = "]";
		this.btn_Number_RBracket.ToolTip = "]";
		this.btn_Number_RBracket.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.btn_Number_6LBracket.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_6LBracket.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_6LBracket.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_6LBracket.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_6LBracket.Appearance.Options.UseBackColor = true;
		this.btn_Number_6LBracket.Appearance.Options.UseFont = true;
		this.btn_Number_6LBracket.Appearance.Options.UseForeColor = true;
		this.btn_Number_6LBracket.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_6LBracket.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_6LBracket.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_6LBracket.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_6LBracket.AutoWidthInLayoutControl = true;
		this.btn_Number_6LBracket.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_6LBracket.Location = new System.Drawing.Point(798, 69);
		this.btn_Number_6LBracket.Name = "btn_Number_6LBracket";
		this.btn_Number_6LBracket.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_6LBracket.TabIndex = 34;
		this.btn_Number_6LBracket.Tag = "{";
		this.btn_Number_6LBracket.Text = "[";
		this.btn_Number_6LBracket.ToolTip = "[";
		this.btn_Number_6LBracket.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.btn_Char_P.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_P.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_P.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_P.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_P.Appearance.Options.UseBackColor = true;
		this.btn_Char_P.Appearance.Options.UseFont = true;
		this.btn_Char_P.Appearance.Options.UseForeColor = true;
		this.btn_Char_P.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_P.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_P.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_P.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_P.AutoWidthInLayoutControl = true;
		this.btn_Char_P.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_P.Location = new System.Drawing.Point(732, 69);
		this.btn_Char_P.Name = "btn_Char_P";
		this.btn_Char_P.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_P.TabIndex = 33;
		this.btn_Char_P.Tag = "P";
		this.btn_Char_P.Text = "p";
		this.btn_Char_P.ToolTip = "p";
		this.btn_Char_P.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_O.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_O.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_O.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_O.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_O.Appearance.Options.UseBackColor = true;
		this.btn_Char_O.Appearance.Options.UseFont = true;
		this.btn_Char_O.Appearance.Options.UseForeColor = true;
		this.btn_Char_O.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_O.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_O.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_O.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_O.AutoWidthInLayoutControl = true;
		this.btn_Char_O.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_O.Location = new System.Drawing.Point(665, 69);
		this.btn_Char_O.Name = "btn_Char_O";
		this.btn_Char_O.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_O.TabIndex = 32;
		this.btn_Char_O.Tag = "O";
		this.btn_Char_O.Text = "o";
		this.btn_Char_O.ToolTip = "o";
		this.btn_Char_O.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_I.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_I.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_I.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_I.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_I.Appearance.Options.UseBackColor = true;
		this.btn_Char_I.Appearance.Options.UseFont = true;
		this.btn_Char_I.Appearance.Options.UseForeColor = true;
		this.btn_Char_I.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_I.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_I.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_I.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_I.AutoWidthInLayoutControl = true;
		this.btn_Char_I.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_I.Location = new System.Drawing.Point(599, 69);
		this.btn_Char_I.Name = "btn_Char_I";
		this.btn_Char_I.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_I.TabIndex = 31;
		this.btn_Char_I.Tag = "I";
		this.btn_Char_I.Text = "i";
		this.btn_Char_I.ToolTip = "i";
		this.btn_Char_I.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_U.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_U.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_U.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_U.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_U.Appearance.Options.UseBackColor = true;
		this.btn_Char_U.Appearance.Options.UseFont = true;
		this.btn_Char_U.Appearance.Options.UseForeColor = true;
		this.btn_Char_U.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_U.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_U.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_U.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_U.AutoWidthInLayoutControl = true;
		this.btn_Char_U.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_U.Location = new System.Drawing.Point(533, 69);
		this.btn_Char_U.Name = "btn_Char_U";
		this.btn_Char_U.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_U.TabIndex = 30;
		this.btn_Char_U.Tag = "U";
		this.btn_Char_U.Text = "u";
		this.btn_Char_U.ToolTip = "u";
		this.btn_Char_U.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Number_Minus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_Minus.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_Minus.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_Minus.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_Minus.Appearance.Options.UseBackColor = true;
		this.btn_Number_Minus.Appearance.Options.UseFont = true;
		this.btn_Number_Minus.Appearance.Options.UseForeColor = true;
		this.btn_Number_Minus.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_Minus.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_Minus.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_Minus.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_Minus.AutoWidthInLayoutControl = true;
		this.btn_Number_Minus.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_Minus.Location = new System.Drawing.Point(731, 3);
		this.btn_Number_Minus.Name = "btn_Number_Minus";
		this.btn_Number_Minus.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_Minus.TabIndex = 29;
		this.btn_Number_Minus.Tag = "_";
		this.btn_Number_Minus.Text = "-";
		this.btn_Number_Minus.ToolTip = "-";
		this.btn_Number_Minus.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.btn_Number_0.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_0.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_0.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_0.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_0.Appearance.Options.UseBackColor = true;
		this.btn_Number_0.Appearance.Options.UseFont = true;
		this.btn_Number_0.Appearance.Options.UseForeColor = true;
		this.btn_Number_0.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_0.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_0.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_0.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_0.AutoWidthInLayoutControl = true;
		this.btn_Number_0.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_0.Location = new System.Drawing.Point(665, 3);
		this.btn_Number_0.Name = "btn_Number_0";
		this.btn_Number_0.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_0.TabIndex = 28;
		this.btn_Number_0.Tag = ")";
		this.btn_Number_0.Text = "0";
		this.btn_Number_0.ToolTip = "0";
		this.btn_Number_0.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.btn_Number_9.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_9.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_9.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_9.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_9.Appearance.Options.UseBackColor = true;
		this.btn_Number_9.Appearance.Options.UseFont = true;
		this.btn_Number_9.Appearance.Options.UseForeColor = true;
		this.btn_Number_9.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_9.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_9.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_9.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_9.AutoWidthInLayoutControl = true;
		this.btn_Number_9.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_9.Location = new System.Drawing.Point(599, 3);
		this.btn_Number_9.Name = "btn_Number_9";
		this.btn_Number_9.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_9.TabIndex = 27;
		this.btn_Number_9.Tag = "(";
		this.btn_Number_9.Text = "9";
		this.btn_Number_9.ToolTip = "9";
		this.btn_Number_9.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.btn_Number_8.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_8.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_8.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_8.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_8.Appearance.Options.UseBackColor = true;
		this.btn_Number_8.Appearance.Options.UseFont = true;
		this.btn_Number_8.Appearance.Options.UseForeColor = true;
		this.btn_Number_8.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_8.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_8.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_8.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_8.AutoWidthInLayoutControl = true;
		this.btn_Number_8.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_8.Location = new System.Drawing.Point(532, 3);
		this.btn_Number_8.Name = "btn_Number_8";
		this.btn_Number_8.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_8.TabIndex = 26;
		this.btn_Number_8.Tag = "*";
		this.btn_Number_8.Text = "8";
		this.btn_Number_8.ToolTip = "8";
		this.btn_Number_8.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.btn_Number_7.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_7.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_7.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_7.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_7.Appearance.Options.UseBackColor = true;
		this.btn_Number_7.Appearance.Options.UseFont = true;
		this.btn_Number_7.Appearance.Options.UseForeColor = true;
		this.btn_Number_7.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_7.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_7.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_7.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_7.AutoWidthInLayoutControl = true;
		this.btn_Number_7.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_7.Location = new System.Drawing.Point(466, 3);
		this.btn_Number_7.Name = "btn_Number_7";
		this.btn_Number_7.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_7.TabIndex = 25;
		this.btn_Number_7.Tag = "&&";
		this.btn_Number_7.Text = "7";
		this.btn_Number_7.ToolTip = "7";
		this.btn_Number_7.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.btn_Number_6.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_6.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_6.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_6.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_6.Appearance.Options.UseBackColor = true;
		this.btn_Number_6.Appearance.Options.UseFont = true;
		this.btn_Number_6.Appearance.Options.UseForeColor = true;
		this.btn_Number_6.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_6.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_6.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_6.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_6.AutoWidthInLayoutControl = true;
		this.btn_Number_6.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_6.Location = new System.Drawing.Point(400, 3);
		this.btn_Number_6.Name = "btn_Number_6";
		this.btn_Number_6.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_6.TabIndex = 24;
		this.btn_Number_6.Tag = "^";
		this.btn_Number_6.Text = "6";
		this.btn_Number_6.ToolTip = "6";
		this.btn_Number_6.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.btn_Char_B.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_B.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_B.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_B.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_B.Appearance.Options.UseBackColor = true;
		this.btn_Char_B.Appearance.Options.UseFont = true;
		this.btn_Char_B.Appearance.Options.UseForeColor = true;
		this.btn_Char_B.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_B.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_B.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_B.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_B.AutoWidthInLayoutControl = true;
		this.btn_Char_B.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_B.Location = new System.Drawing.Point(400, 201);
		this.btn_Char_B.Name = "btn_Char_B";
		this.btn_Char_B.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_B.TabIndex = 23;
		this.btn_Char_B.Tag = "B";
		this.btn_Char_B.Text = "b";
		this.btn_Char_B.ToolTip = "b";
		this.btn_Char_B.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_V.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_V.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_V.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_V.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_V.Appearance.Options.UseBackColor = true;
		this.btn_Char_V.Appearance.Options.UseFont = true;
		this.btn_Char_V.Appearance.Options.UseForeColor = true;
		this.btn_Char_V.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_V.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_V.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_V.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_V.AutoWidthInLayoutControl = true;
		this.btn_Char_V.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_V.Location = new System.Drawing.Point(334, 201);
		this.btn_Char_V.Name = "btn_Char_V";
		this.btn_Char_V.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_V.TabIndex = 22;
		this.btn_Char_V.Tag = "V";
		this.btn_Char_V.Text = "v";
		this.btn_Char_V.ToolTip = "v";
		this.btn_Char_V.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_C.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_C.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_C.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_C.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_C.Appearance.Options.UseBackColor = true;
		this.btn_Char_C.Appearance.Options.UseFont = true;
		this.btn_Char_C.Appearance.Options.UseForeColor = true;
		this.btn_Char_C.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_C.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_C.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_C.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_C.AutoWidthInLayoutControl = true;
		this.btn_Char_C.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_C.Location = new System.Drawing.Point(268, 201);
		this.btn_Char_C.Name = "btn_Char_C";
		this.btn_Char_C.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_C.TabIndex = 21;
		this.btn_Char_C.Tag = "C";
		this.btn_Char_C.Text = "c";
		this.btn_Char_C.ToolTip = "c";
		this.btn_Char_C.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_X.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_X.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_X.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_X.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_X.Appearance.Options.UseBackColor = true;
		this.btn_Char_X.Appearance.Options.UseFont = true;
		this.btn_Char_X.Appearance.Options.UseForeColor = true;
		this.btn_Char_X.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_X.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_X.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_X.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_X.AutoWidthInLayoutControl = true;
		this.btn_Char_X.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_X.Location = new System.Drawing.Point(201, 201);
		this.btn_Char_X.Name = "btn_Char_X";
		this.btn_Char_X.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_X.TabIndex = 20;
		this.btn_Char_X.Tag = "X";
		this.btn_Char_X.Text = "x";
		this.btn_Char_X.ToolTip = "x";
		this.btn_Char_X.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_Z.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_Z.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_Z.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_Z.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_Z.Appearance.Options.UseBackColor = true;
		this.btn_Char_Z.Appearance.Options.UseFont = true;
		this.btn_Char_Z.Appearance.Options.UseForeColor = true;
		this.btn_Char_Z.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_Z.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_Z.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_Z.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_Z.AutoWidthInLayoutControl = true;
		this.btn_Char_Z.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_Z.Location = new System.Drawing.Point(135, 201);
		this.btn_Char_Z.Name = "btn_Char_Z";
		this.btn_Char_Z.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_Z.TabIndex = 19;
		this.btn_Char_Z.Tag = "Z";
		this.btn_Char_Z.Text = "z";
		this.btn_Char_Z.ToolTip = "z";
		this.btn_Char_Z.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_G.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_G.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_G.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_G.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_G.Appearance.Options.UseBackColor = true;
		this.btn_Char_G.Appearance.Options.UseFont = true;
		this.btn_Char_G.Appearance.Options.UseForeColor = true;
		this.btn_Char_G.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_G.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_G.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_G.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_G.AutoWidthInLayoutControl = true;
		this.btn_Char_G.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_G.Location = new System.Drawing.Point(400, 135);
		this.btn_Char_G.Name = "btn_Char_G";
		this.btn_Char_G.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_G.TabIndex = 17;
		this.btn_Char_G.Tag = "G";
		this.btn_Char_G.Text = "g";
		this.btn_Char_G.ToolTip = "g";
		this.btn_Char_G.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_F.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_F.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_F.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_F.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_F.Appearance.Options.UseBackColor = true;
		this.btn_Char_F.Appearance.Options.UseFont = true;
		this.btn_Char_F.Appearance.Options.UseForeColor = true;
		this.btn_Char_F.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_F.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_F.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_F.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_F.AutoWidthInLayoutControl = true;
		this.btn_Char_F.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_F.Location = new System.Drawing.Point(334, 135);
		this.btn_Char_F.Name = "btn_Char_F";
		this.btn_Char_F.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_F.TabIndex = 16;
		this.btn_Char_F.Tag = "F";
		this.btn_Char_F.Text = "f";
		this.btn_Char_F.ToolTip = "f";
		this.btn_Char_F.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_D.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_D.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_D.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_D.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_D.Appearance.Options.UseBackColor = true;
		this.btn_Char_D.Appearance.Options.UseFont = true;
		this.btn_Char_D.Appearance.Options.UseForeColor = true;
		this.btn_Char_D.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_D.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_D.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_D.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_D.AutoWidthInLayoutControl = true;
		this.btn_Char_D.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_D.Location = new System.Drawing.Point(268, 135);
		this.btn_Char_D.Name = "btn_Char_D";
		this.btn_Char_D.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_D.TabIndex = 15;
		this.btn_Char_D.Tag = "D";
		this.btn_Char_D.Text = "d";
		this.btn_Char_D.ToolTip = "d";
		this.btn_Char_D.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_S.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_S.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_S.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_S.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_S.Appearance.Options.UseBackColor = true;
		this.btn_Char_S.Appearance.Options.UseFont = true;
		this.btn_Char_S.Appearance.Options.UseForeColor = true;
		this.btn_Char_S.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_S.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_S.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_S.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_S.AutoWidthInLayoutControl = true;
		this.btn_Char_S.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_S.Location = new System.Drawing.Point(201, 135);
		this.btn_Char_S.Name = "btn_Char_S";
		this.btn_Char_S.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_S.TabIndex = 14;
		this.btn_Char_S.Tag = "S";
		this.btn_Char_S.Text = "s";
		this.btn_Char_S.ToolTip = "s";
		this.btn_Char_S.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_A.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_A.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_A.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_A.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_A.Appearance.Options.UseBackColor = true;
		this.btn_Char_A.Appearance.Options.UseFont = true;
		this.btn_Char_A.Appearance.Options.UseForeColor = true;
		this.btn_Char_A.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_A.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_A.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_A.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_A.AutoWidthInLayoutControl = true;
		this.btn_Char_A.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_A.Location = new System.Drawing.Point(135, 135);
		this.btn_Char_A.Name = "btn_Char_A";
		this.btn_Char_A.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_A.TabIndex = 13;
		this.btn_Char_A.Tag = "A";
		this.btn_Char_A.Text = "a";
		this.btn_Char_A.ToolTip = "a";
		this.btn_Char_A.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_Y.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_Y.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_Y.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_Y.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_Y.Appearance.Options.UseBackColor = true;
		this.btn_Char_Y.Appearance.Options.UseFont = true;
		this.btn_Char_Y.Appearance.Options.UseForeColor = true;
		this.btn_Char_Y.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_Y.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_Y.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_Y.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_Y.AutoWidthInLayoutControl = true;
		this.btn_Char_Y.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_Y.Location = new System.Drawing.Point(467, 69);
		this.btn_Char_Y.Name = "btn_Char_Y";
		this.btn_Char_Y.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_Y.TabIndex = 11;
		this.btn_Char_Y.Tag = "Y";
		this.btn_Char_Y.Text = "y";
		this.btn_Char_Y.ToolTip = "y";
		this.btn_Char_Y.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_T.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_T.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_T.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_T.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_T.Appearance.Options.UseBackColor = true;
		this.btn_Char_T.Appearance.Options.UseFont = true;
		this.btn_Char_T.Appearance.Options.UseForeColor = true;
		this.btn_Char_T.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_T.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_T.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_T.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_T.AutoWidthInLayoutControl = true;
		this.btn_Char_T.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_T.Location = new System.Drawing.Point(401, 69);
		this.btn_Char_T.Name = "btn_Char_T";
		this.btn_Char_T.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_T.TabIndex = 10;
		this.btn_Char_T.Tag = "T";
		this.btn_Char_T.Text = "t";
		this.btn_Char_T.ToolTip = "t";
		this.btn_Char_T.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_R.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_R.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_R.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_R.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_R.Appearance.Options.UseBackColor = true;
		this.btn_Char_R.Appearance.Options.UseFont = true;
		this.btn_Char_R.Appearance.Options.UseForeColor = true;
		this.btn_Char_R.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_R.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_R.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_R.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_R.AutoWidthInLayoutControl = true;
		this.btn_Char_R.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_R.Location = new System.Drawing.Point(335, 69);
		this.btn_Char_R.Name = "btn_Char_R";
		this.btn_Char_R.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_R.TabIndex = 9;
		this.btn_Char_R.Tag = "R";
		this.btn_Char_R.Text = "r";
		this.btn_Char_R.ToolTip = "r";
		this.btn_Char_R.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_E.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_E.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_E.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_E.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_E.Appearance.Options.UseBackColor = true;
		this.btn_Char_E.Appearance.Options.UseFont = true;
		this.btn_Char_E.Appearance.Options.UseForeColor = true;
		this.btn_Char_E.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_E.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_E.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_E.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_E.AutoWidthInLayoutControl = true;
		this.btn_Char_E.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_E.Location = new System.Drawing.Point(268, 69);
		this.btn_Char_E.Name = "btn_Char_E";
		this.btn_Char_E.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_E.TabIndex = 8;
		this.btn_Char_E.Tag = "E";
		this.btn_Char_E.Text = "e";
		this.btn_Char_E.ToolTip = "e";
		this.btn_Char_E.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_W.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_W.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_W.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_W.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_W.Appearance.Options.UseBackColor = true;
		this.btn_Char_W.Appearance.Options.UseFont = true;
		this.btn_Char_W.Appearance.Options.UseForeColor = true;
		this.btn_Char_W.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_W.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_W.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_W.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_W.AutoWidthInLayoutControl = true;
		this.btn_Char_W.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_W.Location = new System.Drawing.Point(202, 69);
		this.btn_Char_W.Name = "btn_Char_W";
		this.btn_Char_W.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_W.TabIndex = 7;
		this.btn_Char_W.Tag = "W";
		this.btn_Char_W.Text = "w";
		this.btn_Char_W.ToolTip = "w";
		this.btn_Char_W.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Char_Q.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Char_Q.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Char_Q.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Char_Q.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Char_Q.Appearance.Options.UseBackColor = true;
		this.btn_Char_Q.Appearance.Options.UseFont = true;
		this.btn_Char_Q.Appearance.Options.UseForeColor = true;
		this.btn_Char_Q.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Char_Q.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Char_Q.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Char_Q.AppearancePressed.Options.UseBackColor = true;
		this.btn_Char_Q.AutoWidthInLayoutControl = true;
		this.btn_Char_Q.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Char_Q.Location = new System.Drawing.Point(135, 69);
		this.btn_Char_Q.Name = "btn_Char_Q";
		this.btn_Char_Q.Size = new System.Drawing.Size(60, 60);
		this.btn_Char_Q.TabIndex = 6;
		this.btn_Char_Q.Tag = "Q";
		this.btn_Char_Q.Text = "q";
		this.btn_Char_Q.ToolTip = "q";
		this.btn_Char_Q.Click += new System.EventHandler(this.buttonAlphakey_Click);
		this.btn_Number_5.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_5.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_5.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_5.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_5.Appearance.Options.UseBackColor = true;
		this.btn_Number_5.Appearance.Options.UseFont = true;
		this.btn_Number_5.Appearance.Options.UseForeColor = true;
		this.btn_Number_5.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_5.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_5.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_5.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_5.AutoWidthInLayoutControl = true;
		this.btn_Number_5.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_5.Location = new System.Drawing.Point(334, 3);
		this.btn_Number_5.Name = "btn_Number_5";
		this.btn_Number_5.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_5.TabIndex = 5;
		this.btn_Number_5.Tag = "%";
		this.btn_Number_5.Text = "5";
		this.btn_Number_5.ToolTip = "5";
		this.btn_Number_5.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.btn_Number_4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_4.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_4.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_4.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_4.Appearance.Options.UseBackColor = true;
		this.btn_Number_4.Appearance.Options.UseFont = true;
		this.btn_Number_4.Appearance.Options.UseForeColor = true;
		this.btn_Number_4.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_4.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_4.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_4.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_4.AutoWidthInLayoutControl = true;
		this.btn_Number_4.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_4.Location = new System.Drawing.Point(268, 3);
		this.btn_Number_4.Name = "btn_Number_4";
		this.btn_Number_4.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_4.TabIndex = 4;
		this.btn_Number_4.Tag = "$";
		this.btn_Number_4.Text = "4";
		this.btn_Number_4.ToolTip = "4";
		this.btn_Number_4.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.btn_Number_3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_3.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_3.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_3.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_3.Appearance.Options.UseBackColor = true;
		this.btn_Number_3.Appearance.Options.UseFont = true;
		this.btn_Number_3.Appearance.Options.UseForeColor = true;
		this.btn_Number_3.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_3.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_3.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_3.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_3.AutoWidthInLayoutControl = true;
		this.btn_Number_3.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_3.Location = new System.Drawing.Point(202, 3);
		this.btn_Number_3.Name = "btn_Number_3";
		this.btn_Number_3.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_3.TabIndex = 3;
		this.btn_Number_3.Tag = "#";
		this.btn_Number_3.Text = "3";
		this.btn_Number_3.ToolTip = "3";
		this.btn_Number_3.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.btn_Number_2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_2.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_2.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_2.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_2.Appearance.Options.UseBackColor = true;
		this.btn_Number_2.Appearance.Options.UseFont = true;
		this.btn_Number_2.Appearance.Options.UseForeColor = true;
		this.btn_Number_2.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_2.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_2.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_2.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_2.AutoWidthInLayoutControl = true;
		this.btn_Number_2.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_2.Location = new System.Drawing.Point(135, 3);
		this.btn_Number_2.Name = "btn_Number_2";
		this.btn_Number_2.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_2.TabIndex = 2;
		this.btn_Number_2.Tag = "@";
		this.btn_Number_2.Text = "2";
		this.btn_Number_2.ToolTip = "2";
		this.btn_Number_2.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.btn_Number_1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_1.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_1.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_1.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_1.Appearance.Options.UseBackColor = true;
		this.btn_Number_1.Appearance.Options.UseFont = true;
		this.btn_Number_1.Appearance.Options.UseForeColor = true;
		this.btn_Number_1.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_1.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_1.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
		this.btn_Number_1.AppearancePressed.Options.UseBackColor = true;
		this.btn_Number_1.AutoWidthInLayoutControl = true;
		this.btn_Number_1.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_1.Location = new System.Drawing.Point(69, 3);
		this.btn_Number_1.Name = "btn_Number_1";
		this.btn_Number_1.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_1.TabIndex = 1;
		this.btn_Number_1.Tag = "!";
		this.btn_Number_1.Text = "1";
		this.btn_Number_1.ToolTip = "1";
		this.btn_Number_1.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.btn_Number_99.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Number_99.Appearance.BackColor = System.Drawing.Color.White;
		this.btn_Number_99.Appearance.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Number_99.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Number_99.Appearance.Options.UseBackColor = true;
		this.btn_Number_99.Appearance.Options.UseFont = true;
		this.btn_Number_99.Appearance.Options.UseForeColor = true;
		this.btn_Number_99.AppearanceHovered.BackColor = System.Drawing.Color.Silver;
		this.btn_Number_99.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Number_99.AutoWidthInLayoutControl = true;
		this.btn_Number_99.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Number_99.Location = new System.Drawing.Point(3, 3);
		this.btn_Number_99.Name = "btn_Number_99";
		this.btn_Number_99.Size = new System.Drawing.Size(60, 60);
		this.btn_Number_99.TabIndex = 0;
		this.btn_Number_99.Tag = "~";
		this.btn_Number_99.Text = "`";
		this.btn_Number_99.ToolTip = "`";
		this.btn_Number_99.Click += new System.EventHandler(this.buttonSymbolkey_Click);
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem1 });
		this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlGroup1.Name = "layoutControlGroup1";
		this.layoutControlGroup1.Size = new System.Drawing.Size(1019, 356);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem1.Control = this.panel1;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(999, 336);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1019, 356);
		base.Controls.Add(this.layoutControl1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.Name = "frm_OSKeyboard";
		base.ShowIcon = false;
		this.Text = "On-Screen Keyboard";
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		this.panel1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		base.ResumeLayout(false);
	}
}
