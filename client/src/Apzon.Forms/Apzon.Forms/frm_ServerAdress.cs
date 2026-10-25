using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms;

public class frm_ServerAdress : XtraForm
{
	public string ApiServer = "";

	private string _serverAdress = "";

	private IContainer components;

	private LabelLine labelLine1;

	private TextEditEx txtIp;

	private TextEditEx txtPort;

	private SimpleButtonEx btnOk;

	private SimpleButtonEx btnCancel;

	private ComboBoxEditEx txtType;

	public frm_ServerAdress()
	{
		InitializeComponent();
		DialogResult = DialogResult.No;
	}

	public frm_ServerAdress(string serverAddress)
	{
		InitializeComponent();
		DialogResult = DialogResult.No;
		_serverAdress = serverAddress;
	}

	private void Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btnOk_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(txtType.Text))
		{
			WinCommons.Error(Enums.MessageCode.YouHaveToEnterType, "");
			txtType.Focus();
			return;
		}
		if (string.IsNullOrEmpty(txtIp.Text))
		{
			WinCommons.Error(Enums.MessageCode.NotEnteredIpServerYet, "");
			txtIp.Focus();
			return;
		}
		WinCommons._OpenProcessing();
		EnableDisableControl(isEnable: false);
		string arg = "";
		if (!string.IsNullOrWhiteSpace(txtPort.Text))
		{
			arg = ":" + txtPort.Text.Trim();
		}
		ApiServer = $"{txtType.Text}://{txtIp.Text}{arg}//api/";
		int num;
		try
		{
			using HttpClientHelper<string> http = new HttpClientHelper<string>(ApiServer, "Commons");
			num = await http.GetIntAsync("CheckConnectServer");
		}
		catch
		{
			num = 0;
		}
		if (num != 1)
		{
			WinCommons.Error(Enums.MessageCode.CannotConnect2ServerPleaseTryAgain, "");
		}
		else
		{
			DialogResult = DialogResult.OK;
			Close();
		}
		EnableDisableControl(isEnable: true);
		WinCommons._CloseProcessing();
	}

	private void frm_ServerAdress_Load(object sender, EventArgs e)
	{
		try
		{
			ComboBoxItemCollection items = txtType.Properties.Items;
			object[] items2 = new string[2] { "http", "https" };
			items.AddRange(items2);
			if (string.IsNullOrEmpty(_serverAdress))
			{
				txtType.EditValue = "http";
				return;
			}
			string[] array = _serverAdress.Split('/');
			if (array.Length < 3)
			{
				return;
			}
			string[] array2 = array[0].Split(':');
			if (array2.Length == 2 && new string[2] { "http", "https" }.Contains(array2[0]))
			{
				txtType.EditValue = array2[0];
				string[] array3 = array[2].Split(':');
				if (array3.Length == 2)
				{
					txtIp.EditValue = array3[0];
					txtPort.EditValue = array3[1];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EnableDisableControl(bool isEnable)
	{
		try
		{
			SimpleButtonEx simpleButtonEx = btnOk;
			bool enabled = (btnCancel.Enabled = isEnable);
			simpleButtonEx.Enabled = enabled;
			ComboBoxEditEx comboBoxEditEx = txtType;
			TextEditEx textEditEx = txtIp;
			bool flag2 = (txtPort.ReadOnly = !isEnable);
			enabled = (textEditEx.ReadOnly = flag2);
			comboBoxEditEx.ReadOnly = enabled;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.frm_ServerAdress));
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txtIp = new Apzon.Controls.Editor.TextEditEx();
		this.txtPort = new Apzon.Controls.Editor.TextEditEx();
		this.btnOk = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtType = new Apzon.Controls.Controls.ComboBoxEditEx();
		((System.ComponentModel.ISupportInitialize)this.txtIp.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPort.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtType.Properties).BeginInit();
		base.SuspendLayout();
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(18, 12);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(87, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Server Address";
		this.txtIp.FieldName = null;
		this.txtIp.Location = new System.Drawing.Point(166, 9);
		this.txtIp.Name = "txtIp";
		this.txtIp.Properties.NullText = "IP address";
		this.txtIp.Size = new System.Drawing.Size(204, 20);
		this.txtIp.TabIndex = 2;
		this.txtIp.ToolTip = "IP Address";
		this.txtPort.EditValue = "8000";
		this.txtPort.FieldName = null;
		this.txtPort.Location = new System.Drawing.Point(371, 9);
		this.txtPort.Name = "txtPort";
		this.txtPort.Size = new System.Drawing.Size(60, 20);
		this.txtPort.TabIndex = 3;
		this.txtPort.ToolTip = "Port";
		this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOk.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOk.ImageOptions.Image");
		this.btnOk.Location = new System.Drawing.Point(154, 37);
		this.btnOk.Name = "btnOk";
		this.btnOk.Size = new System.Drawing.Size(75, 20);
		this.btnOk.TabIndex = 4;
		this.btnOk.Text = "OK";
		this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.Location = new System.Drawing.Point(235, 37);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 20);
		this.btnCancel.TabIndex = 5;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.Cancel_Click);
		this.txtType.Location = new System.Drawing.Point(105, 9);
		this.txtType.Name = "txtType";
		this.txtType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtType.Properties.PopupSizeable = true;
		this.txtType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.txtType.Size = new System.Drawing.Size(60, 20);
		this.txtType.TabIndex = 6;
		this.txtType.ToolTip = "Type";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(449, 70);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.txtPort);
		base.Controls.Add(this.txtIp);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.txtType);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.Name = "frm_ServerAdress";
		base.ShowIcon = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Server config";
		base.Load += new System.EventHandler(this.frm_ServerAdress_Load);
		((System.ComponentModel.ISupportInitialize)this.txtIp.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPort.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtType.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
