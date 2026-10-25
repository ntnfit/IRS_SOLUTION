using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Properties;
using DevExpress.Utils;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.UserControls.POS_UI;

public class RetailMessageBoxContent : XtraForm
{
	private readonly MessageBoxButtons _buttons;

	private readonly string _caption;

	private readonly MessageBoxDefaultButton _defaultButton;

	private readonly MessageBoxIcon _icon;

	private readonly string _message;

	private DialogResult _result1;

	private DialogResult _result2;

	private DialogResult _result3;

	private IContainer components;

	private Panel panel_Caption;

	private Panel panel_Button;

	private RichTextBox rtb_Message;

	private TileBar tilebar_Buttons;

	private TileBarItem Button1;

	private TileBarItem Button2;

	private TileBarItem Button3;

	private TileBar tileBar3;

	private TileBarGroup tileBarGroup2;

	private SimpleButton btn_Icon;

	private RichTextBox rtb_Caption;

	private ImageList iml_Icon;

	public RetailMessageBoxContent()
	{
		InitializeComponent();
	}

	public RetailMessageBoxContent(string message = "", string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1, MessageBoxIcon icon = MessageBoxIcon.Asterisk)
	{
		InitializeComponent();
		_caption = caption;
		_message = message;
		_buttons = buttons;
		_defaultButton = defaultButton;
		_icon = icon;
	}

	private void RetailMessageBoxContent_Load(object sender, EventArgs e)
	{
		try
		{
			rtb_Caption.Text = _caption;
			rtb_Message.Text = _message;
			switch (_buttons)
			{
			case MessageBoxButtons.OK:
			{
				Button1.Visible = true;
				Button1.Text = PoinOfSale.msg_DialogOk;
				_result1 = DialogResult.OK;
				TileBarItem button = Button2;
				bool visible = (Button3.Visible = false);
				button.Visible = visible;
				break;
			}
			case MessageBoxButtons.AbortRetryIgnore:
				Button1.Text = PoinOfSale.msg_DialogAbort;
				Button2.Text = PoinOfSale.msg_DialogRetry;
				Button3.Text = PoinOfSale.msg_DialogIgnore;
				_result1 = DialogResult.Abort;
				_result2 = DialogResult.Retry;
				_result3 = DialogResult.Ignore;
				break;
			case MessageBoxButtons.OKCancel:
				Button1.Text = PoinOfSale.msg_DialogOk;
				Button2.Text = PoinOfSale.msg_DialogCancel;
				Button3.Visible = false;
				_result1 = DialogResult.OK;
				_result2 = DialogResult.Cancel;
				break;
			case MessageBoxButtons.RetryCancel:
				Button1.Text = PoinOfSale.msg_DialogRetry;
				Button2.Text = PoinOfSale.msg_DialogCancel;
				Button3.Visible = false;
				_result1 = DialogResult.Retry;
				_result2 = DialogResult.Cancel;
				break;
			case MessageBoxButtons.YesNo:
				Button1.Text = PoinOfSale.msg_DialogYes;
				Button2.Text = PoinOfSale.msg_DialogNo;
				Button3.Visible = false;
				_result1 = DialogResult.Yes;
				_result2 = DialogResult.No;
				break;
			case MessageBoxButtons.YesNoCancel:
				Button1.Text = PoinOfSale.msg_DialogYes;
				Button2.Text = PoinOfSale.msg_DialogNo;
				Button3.Text = PoinOfSale.msg_DialogCancel;
				_result1 = DialogResult.Yes;
				_result2 = DialogResult.No;
				_result3 = DialogResult.Cancel;
				break;
			}
			switch (_icon)
			{
			case MessageBoxIcon.Hand:
				btn_Icon.ImageIndex = 0;
				break;
			case MessageBoxIcon.Question:
				btn_Icon.ImageIndex = 3;
				break;
			case MessageBoxIcon.Exclamation:
				btn_Icon.ImageIndex = 5;
				break;
			case MessageBoxIcon.Asterisk:
				btn_Icon.ImageIndex = 6;
				break;
			}
			switch (_defaultButton)
			{
			case MessageBoxDefaultButton.Button1:
				Button1.AppearanceItem.Normal.BackColor = Color.FromArgb(255, 224, 192);
				break;
			case MessageBoxDefaultButton.Button2:
				Button2.AppearanceItem.Normal.BackColor = Color.FromArgb(255, 224, 192);
				break;
			case MessageBoxDefaultButton.Button3:
				Button3.AppearanceItem.Normal.BackColor = Color.FromArgb(255, 224, 192);
				break;
			}
			Application.AddMessageFilter(new RetailMessageBoxContentFilter(this));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void Button1_ItemClick(object sender, TileItemEventArgs e)
	{
		try
		{
			DialogResult = _result1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void Button2_ItemClick(object sender, TileItemEventArgs e)
	{
		try
		{
			DialogResult = _result2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void Button3_ItemClick(object sender, TileItemEventArgs e)
	{
		try
		{
			DialogResult = _result3;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	internal void EnterKey()
	{
		try
		{
			switch (_defaultButton)
			{
			case MessageBoxDefaultButton.Button1:
				DialogResult = _result1;
				break;
			case MessageBoxDefaultButton.Button2:
				DialogResult = _result2;
				break;
			case MessageBoxDefaultButton.Button3:
				DialogResult = _result3;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.POS_UI.RetailMessageBoxContent));
		DevExpress.XtraEditors.TileItemElement tileItemElement = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement2 = new DevExpress.XtraEditors.TileItemElement();
		DevExpress.XtraEditors.TileItemElement tileItemElement3 = new DevExpress.XtraEditors.TileItemElement();
		this.panel_Caption = new System.Windows.Forms.Panel();
		this.rtb_Caption = new System.Windows.Forms.RichTextBox();
		this.btn_Icon = new DevExpress.XtraEditors.SimpleButton();
		this.iml_Icon = new System.Windows.Forms.ImageList(this.components);
		this.panel_Button = new System.Windows.Forms.Panel();
		this.tilebar_Buttons = new DevExpress.XtraBars.Navigation.TileBar();
		this.tileBarGroup2 = new DevExpress.XtraBars.Navigation.TileBarGroup();
		this.Button1 = new DevExpress.XtraBars.Navigation.TileBarItem();
		this.Button2 = new DevExpress.XtraBars.Navigation.TileBarItem();
		this.Button3 = new DevExpress.XtraBars.Navigation.TileBarItem();
		this.rtb_Message = new System.Windows.Forms.RichTextBox();
		this.tileBar3 = new DevExpress.XtraBars.Navigation.TileBar();
		this.panel_Caption.SuspendLayout();
		this.panel_Button.SuspendLayout();
		base.SuspendLayout();
		this.panel_Caption.BackColor = System.Drawing.Color.FromArgb(35, 65, 114);
		this.panel_Caption.Controls.Add(this.rtb_Caption);
		this.panel_Caption.Controls.Add(this.btn_Icon);
		this.panel_Caption.Dock = System.Windows.Forms.DockStyle.Top;
		this.panel_Caption.Font = new System.Drawing.Font("Microsoft Sans Serif", 20f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.panel_Caption.ForeColor = System.Drawing.Color.White;
		this.panel_Caption.Location = new System.Drawing.Point(0, 0);
		this.panel_Caption.Name = "panel_Caption";
		this.panel_Caption.Size = new System.Drawing.Size(490, 67);
		this.panel_Caption.TabIndex = 0;
		this.rtb_Caption.BackColor = System.Drawing.Color.FromArgb(35, 65, 114);
		this.rtb_Caption.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.rtb_Caption.Font = new System.Drawing.Font("Microsoft Sans Serif", 25f);
		this.rtb_Caption.ForeColor = System.Drawing.Color.White;
		this.rtb_Caption.Location = new System.Drawing.Point(60, 10);
		this.rtb_Caption.Name = "rtb_Caption";
		this.rtb_Caption.ReadOnly = true;
		this.rtb_Caption.Size = new System.Drawing.Size(412, 48);
		this.rtb_Caption.TabIndex = 0;
		this.rtb_Caption.Text = "";
		this.btn_Icon.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.btn_Icon.Appearance.Options.UseBackColor = true;
		this.btn_Icon.AppearanceDisabled.BackColor = System.Drawing.Color.Transparent;
		this.btn_Icon.AppearanceDisabled.Options.UseBackColor = true;
		this.btn_Icon.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.btn_Icon.ImageList = this.iml_Icon;
		this.btn_Icon.Location = new System.Drawing.Point(12, 12);
		this.btn_Icon.Name = "btn_Icon";
		this.btn_Icon.Size = new System.Drawing.Size(42, 45);
		this.btn_Icon.TabIndex = 1;
		this.iml_Icon.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("iml_Icon.ImageStream");
		this.iml_Icon.TransparentColor = System.Drawing.Color.Transparent;
		this.iml_Icon.Images.SetKeyName(0, "error.png");
		this.iml_Icon.Images.SetKeyName(1, "hand.png");
		this.iml_Icon.Images.SetKeyName(2, "stop.png");
		this.iml_Icon.Images.SetKeyName(3, "question.png");
		this.iml_Icon.Images.SetKeyName(4, "exclamation-sign.png");
		this.iml_Icon.Images.SetKeyName(5, "warning.png");
		this.iml_Icon.Images.SetKeyName(6, "info.png");
		this.iml_Icon.Images.SetKeyName(7, "asterisk.png");
		this.panel_Button.BackColor = System.Drawing.Color.FromArgb(35, 65, 114);
		this.panel_Button.Controls.Add(this.tilebar_Buttons);
		this.panel_Button.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel_Button.Location = new System.Drawing.Point(0, 265);
		this.panel_Button.Name = "panel_Button";
		this.panel_Button.Size = new System.Drawing.Size(490, 92);
		this.panel_Button.TabIndex = 1;
		this.tilebar_Buttons.AllowDrag = false;
		this.tilebar_Buttons.AllowDragTilesBetweenGroups = false;
		this.tilebar_Buttons.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tilebar_Buttons.AppearanceItem.Normal.Options.UseFont = true;
		this.tilebar_Buttons.AppearanceText.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tilebar_Buttons.AppearanceText.Options.UseFont = true;
		this.tilebar_Buttons.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tilebar_Buttons.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		this.tilebar_Buttons.Groups.Add(this.tileBarGroup2);
		this.tilebar_Buttons.GroupTextToItemsIndent = 0;
		this.tilebar_Buttons.HorizontalContentAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.tilebar_Buttons.IndentBetweenGroups = 0;
		this.tilebar_Buttons.IndentBetweenItems = 3;
		this.tilebar_Buttons.Location = new System.Drawing.Point(0, 0);
		this.tilebar_Buttons.MaxId = 6;
		this.tilebar_Buttons.Name = "tilebar_Buttons";
		this.tilebar_Buttons.Padding = new System.Windows.Forms.Padding(0);
		this.tilebar_Buttons.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.ScrollButtons;
		this.tilebar_Buttons.ShowGroupText = false;
		this.tilebar_Buttons.Size = new System.Drawing.Size(490, 92);
		this.tilebar_Buttons.TabIndex = 11;
		this.tilebar_Buttons.Text = "tileBar1";
		this.tilebar_Buttons.WideTileWidth = 120;
		this.tileBarGroup2.Items.Add(this.Button1);
		this.tileBarGroup2.Items.Add(this.Button2);
		this.tileBarGroup2.Items.Add(this.Button3);
		this.tileBarGroup2.Name = "tileBarGroup2";
		this.Button1.AppearanceItem.Normal.BackColor = System.Drawing.Color.White;
		this.Button1.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 20f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.Button1.AppearanceItem.Normal.ForeColor = System.Drawing.Color.FromArgb(35, 65, 114);
		this.Button1.AppearanceItem.Normal.Options.UseBackColor = true;
		this.Button1.AppearanceItem.Normal.Options.UseFont = true;
		this.Button1.AppearanceItem.Normal.Options.UseForeColor = true;
		this.Button1.AppearanceItem.Normal.Options.UseTextOptions = true;
		this.Button1.AppearanceItem.Normal.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.Button1.AppearanceItem.Normal.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.Button1.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		tileItemElement.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileItemElement.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
		tileItemElement.Text = "Yes";
		this.Button1.Elements.Add(tileItemElement);
		this.Button1.Id = 3;
		this.Button1.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
		this.Button1.Name = "Button1";
		this.Button1.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.Button1_ItemClick);
		this.Button2.AppearanceItem.Normal.BackColor = System.Drawing.Color.White;
		this.Button2.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 20f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.Button2.AppearanceItem.Normal.ForeColor = System.Drawing.Color.FromArgb(35, 65, 114);
		this.Button2.AppearanceItem.Normal.Options.UseBackColor = true;
		this.Button2.AppearanceItem.Normal.Options.UseFont = true;
		this.Button2.AppearanceItem.Normal.Options.UseForeColor = true;
		this.Button2.AppearanceItem.Normal.Options.UseTextOptions = true;
		this.Button2.AppearanceItem.Normal.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.Button2.AppearanceItem.Normal.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.Button2.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		tileItemElement2.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileItemElement2.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
		tileItemElement2.Text = "No";
		this.Button2.Elements.Add(tileItemElement2);
		this.Button2.Id = 4;
		this.Button2.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
		this.Button2.Name = "Button2";
		this.Button2.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.Button2_ItemClick);
		this.Button3.AppearanceItem.Normal.BackColor = System.Drawing.Color.White;
		this.Button3.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 20f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.Button3.AppearanceItem.Normal.ForeColor = System.Drawing.Color.FromArgb(35, 65, 114);
		this.Button3.AppearanceItem.Normal.Options.UseBackColor = true;
		this.Button3.AppearanceItem.Normal.Options.UseFont = true;
		this.Button3.AppearanceItem.Normal.Options.UseForeColor = true;
		this.Button3.AppearanceItem.Normal.Options.UseTextOptions = true;
		this.Button3.AppearanceItem.Normal.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.Button3.AppearanceItem.Normal.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.Button3.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		tileItemElement3.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileItemElement3.ImageToTextAlignment = DevExpress.XtraEditors.TileControlImageToTextAlignment.Left;
		tileItemElement3.Text = "Cancel";
		this.Button3.Elements.Add(tileItemElement3);
		this.Button3.Id = 5;
		this.Button3.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
		this.Button3.Name = "Button3";
		this.Button3.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.Button3_ItemClick);
		this.rtb_Message.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.rtb_Message.Dock = System.Windows.Forms.DockStyle.Fill;
		this.rtb_Message.Font = new System.Drawing.Font("Microsoft Sans Serif", 30f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.rtb_Message.ForeColor = System.Drawing.Color.FromArgb(35, 65, 114);
		this.rtb_Message.Location = new System.Drawing.Point(0, 67);
		this.rtb_Message.Name = "rtb_Message";
		this.rtb_Message.ReadOnly = true;
		this.rtb_Message.Size = new System.Drawing.Size(490, 198);
		this.rtb_Message.TabIndex = 2;
		this.rtb_Message.Text = "";
		this.tileBar3.AllowDrag = false;
		this.tileBar3.AppearanceText.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.tileBar3.AppearanceText.Options.UseFont = true;
		this.tileBar3.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
		this.tileBar3.Location = new System.Drawing.Point(0, 0);
		this.tileBar3.Name = "tileBar3";
		this.tileBar3.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.ScrollButtons;
		this.tileBar3.Size = new System.Drawing.Size(240, 150);
		this.tileBar3.TabIndex = 0;
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
		base.ClientSize = new System.Drawing.Size(490, 357);
		base.Controls.Add(this.rtb_Message);
		base.Controls.Add(this.panel_Button);
		base.Controls.Add(this.panel_Caption);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.Name = "RetailMessageBoxContent";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "RetailMessageBoxContent";
		base.Load += new System.EventHandler(this.RetailMessageBoxContent_Load);
		this.panel_Caption.ResumeLayout(false);
		this.panel_Button.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
