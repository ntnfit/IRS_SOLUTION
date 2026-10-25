using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab;

namespace Apzon.Forms.Service;

public class frm_ServiceCallSetting : XtraFormEx
{
	private IContainer components;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx tabSlack;

	private XtraTabPageEx tabWhatsapp;

	private SimpleButtonEx btnOk;

	private SimpleButtonEx btnCancal;

	private LabelLine labName;

	private XtraTabPageEx tabEmail;

	private TextEditEx txtName;

	private CheckEditEx cbSlack;

	private CheckEditEx cbEmail;

	private TextEditEx txtPassword;

	private TextEditEx txtEmail;

	private LabelLine lbPass;

	private LabelLine lbEmail;

	public frm_ServiceCallSetting()
	{
		InitializeComponent();
	}

	private async void btnOk_Click(object sender, EventArgs e)
	{
		int service = 0;
		if (cbEmail.Checked)
		{
			service = 2;
		}
		if (cbSlack.Checked)
		{
			service = 0;
		}
		if (string.IsNullOrEmpty(await Save(txtName.Text, txtEmail.Text, txtPassword.Text, service)))
		{
			WinCommons.Success(Enums.MessageCode.Success, "");
		}
	}

	private async Task<string> Save(string name, string email, string pass, int service)
	{
		try
		{
			using HttpClientHelper<DataTable> helperSetting = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ServiceCallSetting");
			return await helperSetting.GetStringAsync("Save", new
			{
				name,
				email,
				pass,
				service,
				GlobalData.UserName
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}

	private void cbSlack_CheckedChanged(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(txtName.Text))
		{
			WinCommons.Error(Enums.MessageCode.EnterName, "");
			cbSlack.Checked = false;
		}
		else
		{
			cbEmail.Checked = false;
		}
	}

	private void cbEmail_CheckedChanged(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtPassword.Text))
		{
			WinCommons.Error(Enums.MessageCode.EnterEmailAndPassword, "");
			cbEmail.Checked = false;
		}
		else
		{
			cbSlack.Checked = false;
		}
	}

	private async void frm_Setting_Load(object sender, EventArgs e)
	{
		DataTable dataTable = await FormLoad();
		if (dataTable != null && dataTable.Rows.Count > 0)
		{
			txtName.Text = Function.ToString(dataTable.Rows[0]["name"]);
			txtEmail.Text = Function.ToString(dataTable.Rows[0]["email"]);
			txtPassword.Text = Function.ToString(dataTable.Rows[0]["pass"]);
			switch (Function.ParseInt(dataTable.Rows[0]["service"]))
			{
			case 2:
				cbEmail.Checked = true;
				break;
			default:
				cbSlack.Checked = true;
				break;
			case 1:
				break;
			}
		}
	}

	private async Task<DataTable> FormLoad()
	{
		try
		{
			using HttpClientHelper<DataTable> helperSetting = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ServiceCallSetting");
			return await helperSetting.GetAsync("GetServiceCallSetting", new { GlobalData.UserName });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private void btnCancal_Click(object sender, EventArgs e)
	{
		Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Service.frm_ServiceCallSetting));
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabSlack = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txtName = new Apzon.Controls.Editor.TextEditEx();
		this.cbSlack = new Apzon.Controls.Editor.CheckEditEx();
		this.labName = new Apzon.Controls.Controls.LabelLine();
		this.tabEmail = new Apzon.Controls.Controls.XtraTabPageEx();
		this.cbEmail = new Apzon.Controls.Editor.CheckEditEx();
		this.txtPassword = new Apzon.Controls.Editor.TextEditEx();
		this.txtEmail = new Apzon.Controls.Editor.TextEditEx();
		this.lbPass = new Apzon.Controls.Controls.LabelLine();
		this.lbEmail = new Apzon.Controls.Controls.LabelLine();
		this.tabWhatsapp = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btnOk = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancal = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.tabSlack.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbSlack.Properties).BeginInit();
		this.tabEmail.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cbEmail.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPassword.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtEmail.Properties).BeginInit();
		base.SuspendLayout();
		this.xtraTabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(3, 1);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.tabSlack;
		this.xtraTabControl1.Size = new System.Drawing.Size(636, 376);
		this.xtraTabControl1.TabIndex = 0;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.tabSlack, this.tabEmail, this.tabWhatsapp });
		this.tabSlack.Controls.Add(this.txtName);
		this.tabSlack.Controls.Add(this.cbSlack);
		this.tabSlack.Controls.Add(this.labName);
		this.tabSlack.FieldName = null;
		this.tabSlack.IsSearchControl = false;
		this.tabSlack.Name = "tabSlack";
		this.tabSlack.Size = new System.Drawing.Size(634, 347);
		this.tabSlack.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabSlack.Text = "Slack";
		this.txtName.FieldName = null;
		this.txtName.IsSearchControl = false;
		this.txtName.Location = new System.Drawing.Point(243, 93);
		this.txtName.Name = "txtName";
		this.txtName.Size = new System.Drawing.Size(215, 20);
		this.txtName.TabIndex = 2;
		this.cbSlack.FieldName = null;
		this.cbSlack.IsSearchControl = false;
		this.cbSlack.Location = new System.Drawing.Point(228, 131);
		this.cbSlack.Name = "cbSlack";
		this.cbSlack.Properties.Caption = "use to send message";
		this.cbSlack.Size = new System.Drawing.Size(127, 20);
		this.cbSlack.TabIndex = 1;
		this.cbSlack.CheckedChanged += new System.EventHandler(this.cbSlack_CheckedChanged);
		this.labName.FieldName = null;
		this.labName.HideWhenEmpty = false;
		this.labName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labName.Location = new System.Drawing.Point(169, 95);
		this.labName.Name = "labName";
		this.labName.ShowUnderLine = true;
		this.labName.Size = new System.Drawing.Size(80, 20);
		this.labName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labName.TabIndex = 0;
		this.labName.Utext = "Your name:";
		this.tabEmail.Controls.Add(this.cbEmail);
		this.tabEmail.Controls.Add(this.txtPassword);
		this.tabEmail.Controls.Add(this.txtEmail);
		this.tabEmail.Controls.Add(this.lbPass);
		this.tabEmail.Controls.Add(this.lbEmail);
		this.tabEmail.FieldName = null;
		this.tabEmail.IsSearchControl = false;
		this.tabEmail.Name = "tabEmail";
		this.tabEmail.Size = new System.Drawing.Size(634, 347);
		this.tabEmail.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabEmail.Text = "Email";
		this.cbEmail.FieldName = null;
		this.cbEmail.IsSearchControl = false;
		this.cbEmail.Location = new System.Drawing.Point(253, 128);
		this.cbEmail.Name = "cbEmail";
		this.cbEmail.Properties.Caption = "Use to  send message";
		this.cbEmail.Size = new System.Drawing.Size(128, 20);
		this.cbEmail.TabIndex = 4;
		this.cbEmail.CheckedChanged += new System.EventHandler(this.cbEmail_CheckedChanged);
		this.txtPassword.FieldName = null;
		this.txtPassword.IsSearchControl = false;
		this.txtPassword.Location = new System.Drawing.Point(280, 89);
		this.txtPassword.Name = "txtPassword";
		this.txtPassword.Properties.PasswordChar = '*';
		this.txtPassword.Size = new System.Drawing.Size(100, 20);
		this.txtPassword.TabIndex = 3;
		this.txtEmail.FieldName = null;
		this.txtEmail.IsSearchControl = false;
		this.txtEmail.Location = new System.Drawing.Point(281, 47);
		this.txtEmail.Name = "txtEmail";
		this.txtEmail.Size = new System.Drawing.Size(100, 20);
		this.txtEmail.TabIndex = 2;
		this.lbPass.FieldName = null;
		this.lbPass.HideWhenEmpty = false;
		this.lbPass.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbPass.Location = new System.Drawing.Point(209, 91);
		this.lbPass.Name = "lbPass";
		this.lbPass.ShowUnderLine = true;
		this.lbPass.Size = new System.Drawing.Size(80, 20);
		this.lbPass.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbPass.TabIndex = 1;
		this.lbPass.Utext = "Password";
		this.lbEmail.FieldName = null;
		this.lbEmail.HideWhenEmpty = false;
		this.lbEmail.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbEmail.Location = new System.Drawing.Point(209, 49);
		this.lbEmail.Name = "lbEmail";
		this.lbEmail.ShowUnderLine = true;
		this.lbEmail.Size = new System.Drawing.Size(80, 20);
		this.lbEmail.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbEmail.TabIndex = 0;
		this.lbEmail.Utext = "Your email";
		this.tabWhatsapp.FieldName = null;
		this.tabWhatsapp.IsSearchControl = false;
		this.tabWhatsapp.Name = "tabWhatsapp";
		this.tabWhatsapp.Size = new System.Drawing.Size(634, 347);
		this.tabWhatsapp.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabWhatsapp.Text = "Whatsapp";
		this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnOk.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
		this.btnOk.Appearance.Options.UseBackColor = true;
		this.btnOk.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.btnOk.FieldName = null;
		this.btnOk.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOk.ImageOptions.Image");
		this.btnOk.IsSearchControl = false;
		this.btnOk.Location = new System.Drawing.Point(459, 392);
		this.btnOk.Name = "btnOk";
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 1;
		this.btnOk.Text = "OK";
		this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
		this.btnCancal.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancal.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
		this.btnCancal.Appearance.Options.UseBackColor = true;
		this.btnCancal.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.btnCancal.FieldName = null;
		this.btnCancal.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancal.ImageOptions.Image");
		this.btnCancal.IsSearchControl = false;
		this.btnCancal.Location = new System.Drawing.Point(540, 392);
		this.btnCancal.Name = "btnCancal";
		this.btnCancal.Size = new System.Drawing.Size(75, 23);
		this.btnCancal.TabIndex = 2;
		this.btnCancal.Text = "Cancel";
		this.btnCancal.Click += new System.EventHandler(this.btnCancal_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(638, 427);
		base.Controls.Add(this.btnCancal);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.xtraTabControl1);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ServiceCallSetting";
		this.Text = "Setting";
		base.Load += new System.EventHandler(this.frm_Setting_Load);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.tabSlack.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbSlack.Properties).EndInit();
		this.tabEmail.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cbEmail.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPassword.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtEmail.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
