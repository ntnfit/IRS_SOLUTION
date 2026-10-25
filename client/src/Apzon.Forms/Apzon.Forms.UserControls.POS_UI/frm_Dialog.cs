using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.UserControls.POS_UI;

public class frm_Dialog : XtraForm
{
	private Action _ucforceOk;

	public string Caption;

	public const int WM_NCLBUTTONDOWN = 161;

	public const int HT_CAPTION = 2;

	private bool selectAllOnFocus = true;

	private bool selectAllDone;

	private IContainer components;

	private PanelControl pnl_ControlHolder;

	public SimpleButton btnOk;

	public SimpleButton btnCancel;

	private LabelControlEx lbl_Title;

	public dynamic UserCtrl { get; set; }

	[DefaultValue(true)]
	[Category("Behavior")]
	[Description("Set to true to select all contents of the textbox when the box receives focus by clicking it with the mouse")]
	public bool SelectAllOnFocus
	{
		get
		{
			return selectAllOnFocus;
		}
		set
		{
			selectAllOnFocus = value;
		}
	}

	[DllImport("user32.dll")]
	public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

	[DllImport("user32.dll")]
	public static extern bool ReleaseCapture();

	private void Form1_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			ReleaseCapture();
			SendMessage(Handle, 161, 2, 0);
		}
	}

	public frm_Dialog(dynamic userControl)
	{
		InitializeComponent();
		MouseDown += Form1_MouseDown;
		lbl_Title.MouseDown += Form1_MouseDown;
		btnOk.Text = PoinOfSale.msg_DialogOk;
		Load += OnLoad;
		UserCtrl = (object)userControl;
		if (UserCtrl != null)
		{
			int num = Height - pnl_ControlHolder.Height;
			UserCtrl.Dock = DockStyle.Fill;
			Size size = new Size(UserCtrl.Width + 4, UserCtrl.Height + num + 4);
			Size = size;
			pnl_ControlHolder.Controls.Add(UserCtrl);
			dynamic type = UserCtrl.GetType();
			if (type.GetMember("ForceOk").Length > 0)
			{
				_ucforceOk = ForceOk;
				UserCtrl.ForceOk = _ucforceOk;
			}
		}
		else
		{
			Logging.Write("TRACE", "User Control is null!");
		}
	}

	private void BtnBarCodeOnLeave(object sender, EventArgs eventArgs)
	{
		AcceptButton = btnOk;
	}

	private void BtnBarCodeOnEnter(object sender, EventArgs eventArgs)
	{
		AcceptButton = null;
	}

	private void SetEventControl(Control ctr)
	{
		try
		{
			foreach (Control control in ctr.Controls)
			{
				if (control is ButtonEdit { ReadOnly: false } buttonEdit && buttonEdit.Properties.TextEditStyle != TextEditStyles.DisableTextEditor)
				{
					buttonEdit.Leave += tbxValue_Leave;
					buttonEdit.GotFocus += tbxValue_GotFocus;
					buttonEdit.MouseUp += tbxValue_MouseUp;
					if (buttonEdit.Name.ToLower().Contains("barcode"))
					{
						buttonEdit.Enter += BtnBarCodeOnEnter;
						buttonEdit.Leave += BtnBarCodeOnLeave;
					}
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

	private void tbxValue_MouseUp(object sender, MouseEventArgs e)
	{
		try
		{
			if (sender is ButtonEdit buttonEdit && SelectAllOnFocus && !selectAllDone && buttonEdit.SelectionLength == 0)
			{
				selectAllDone = true;
				buttonEdit.SelectAll();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void tbxValue_GotFocus(object sender, EventArgs e)
	{
		try
		{
			ButtonEdit buttonEdit = sender as ButtonEdit;
			if (SelectAllOnFocus && Control.MouseButtons == MouseButtons.None)
			{
				buttonEdit?.SelectAll();
				selectAllDone = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void tbxValue_Leave(object sender, EventArgs e)
	{
		try
		{
			selectAllDone = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void OnLoad(object sender, EventArgs eventArgs)
	{
		try
		{
			lbl_Title.Text = Caption;
			SetEventControl(this);
			btnOk.Click += delegate
			{
				dynamic type = UserCtrl.GetType();
				if (type.GetMethod("SUBMIT") != null)
				{
					UserCtrl.SUBMIT();
				}
				else
				{
					ForceOk();
				}
			};
			btnCancel.Click += delegate
			{
				ForceCancel();
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void ForceOk()
	{
		DialogResult = DialogResult.OK;
	}

	public void ForceCancel()
	{
		DialogResult = DialogResult.Cancel;
	}

	private void frm_Dialog_Load(object sender, EventArgs e)
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
		this.pnl_ControlHolder = new DevExpress.XtraEditors.PanelControl();
		this.btnOk = new DevExpress.XtraEditors.SimpleButton();
		this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
		this.lbl_Title = new Apzon.Controls.Controls.LabelControlEx();
		((System.ComponentModel.ISupportInitialize)this.pnl_ControlHolder).BeginInit();
		base.SuspendLayout();
		this.pnl_ControlHolder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pnl_ControlHolder.Location = new System.Drawing.Point(0, 36);
		this.pnl_ControlHolder.Name = "pnl_ControlHolder";
		this.pnl_ControlHolder.Size = new System.Drawing.Size(650, 287);
		this.pnl_ControlHolder.TabIndex = 0;
		this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
		this.btnOk.Appearance.BackColor = System.Drawing.Color.FromArgb(128, 255, 128);
		this.btnOk.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btnOk.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btnOk.Appearance.Options.UseBackColor = true;
		this.btnOk.Appearance.Options.UseFont = true;
		this.btnOk.Appearance.Options.UseForeColor = true;
		this.btnOk.Location = new System.Drawing.Point(231, 329);
		this.btnOk.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btnOk.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btnOk.Name = "btnOk";
		this.btnOk.Size = new System.Drawing.Size(211, 38);
		this.btnOk.TabIndex = 39;
		this.btnOk.Text = "OK";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 128, 128);
		this.btnCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btnCancel.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.Appearance.Options.UseFont = true;
		this.btnCancel.Appearance.Options.UseForeColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.Location = new System.Drawing.Point(605, 1);
		this.btnCancel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(44, 32);
		this.btnCancel.TabIndex = 40;
		this.btnCancel.Text = "X";
		this.lbl_Title.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_Title.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_Title.Appearance.Options.UseFont = true;
		this.lbl_Title.Appearance.Options.UseTextOptions = true;
		this.lbl_Title.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.lbl_Title.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.lbl_Title.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.lbl_Title.FieldName = "labelControlEx1";
		this.lbl_Title.Location = new System.Drawing.Point(12, 8);
		this.lbl_Title.Name = "lbl_Title";
		this.lbl_Title.Size = new System.Drawing.Size(587, 18);
		this.lbl_Title.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Title.TabIndex = 49;
		this.lbl_Title.Text = "Title";
		base.AcceptButton = this.btnOk;
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(650, 370);
		base.Controls.Add(this.lbl_Title);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.pnl_ControlHolder);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.Name = "frm_Dialog";
		base.ShowIcon = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "frm_PaymentDialog";
		base.Load += new System.EventHandler(this.frm_Dialog_Load);
		((System.ComponentModel.ISupportInitialize)this.pnl_ControlHolder).EndInit();
		base.ResumeLayout(false);
	}
}
