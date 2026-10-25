using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons;
using Apzon.Global;
using Commons.Helper;
using DevExpress.XtraEditors;

namespace Apzon.Forms.Administrator;

public class frm_FileCopySharingSettings : XtraFormEx
{
	private CopyFileShareStruct _cFStruct;

	private IContainer components;

	private PanelControl panelControl2;

	private TextEditEx txt_Domain;

	private SimpleButtonEx btnFtpCancel;

	private LabelLine labelLine5;

	private SimpleButtonEx btnFtpRefresh;

	private LabelLine labelLine7;

	private SimpleButtonEx btnFtpUpdate;

	private LabelLine labelLine8;

	private TextEditEx txt_Pass;

	private TextEditEx txt_User;

	public frm_FileCopySharingSettings()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_FileCopySharingSettings"))
		{
			InitializeComponent();
			_cFStruct = default;
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_FileCopySharingSettings" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btnFtpCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void frm_FileCopySharingSettings_Load(object sender, EventArgs e)
	{
		GetConfig();
	}

	private void GetConfig()
	{
		try
		{
			_cFStruct = CopyFileShareHelper.GetConfigData();
			txt_Domain.Text = _cFStruct.Domain;
			txt_User.Text = _cFStruct.User;
			txt_Pass.Text = _cFStruct.Pass;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnFtpUpdate_Click(object sender, EventArgs e)
	{
		try
		{
			_cFStruct.Domain = txt_Domain.Text;
			_cFStruct.User = txt_User.Text;
			_cFStruct.Pass = txt_Pass.Text;
			CopyFileShareHelper.SetConfigData(_cFStruct);
			CopyFileSharingCfg.Domain = _cFStruct.Domain;
			CopyFileSharingCfg.UserName = _cFStruct.User;
			CopyFileSharingCfg.Password = _cFStruct.Pass;
			WinCommons.Success(Enums.MessageCode.UpdateSuccess, "");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnFtpCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btnFtpRefresh_Click(object sender, EventArgs e)
	{
		GetConfig();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_FileCopySharingSettings));
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.txt_Domain = new Apzon.Controls.Editor.TextEditEx();
		this.btnFtpCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.btnFtpRefresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.btnFtpUpdate = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Pass = new Apzon.Controls.Editor.TextEditEx();
		this.txt_User = new Apzon.Controls.Editor.TextEditEx();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Domain.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Pass.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_User.Properties).BeginInit();
		base.SuspendLayout();
		this.panelControl2.Controls.Add(this.txt_Domain);
		this.panelControl2.Controls.Add(this.btnFtpCancel);
		this.panelControl2.Controls.Add(this.labelLine5);
		this.panelControl2.Controls.Add(this.btnFtpRefresh);
		this.panelControl2.Controls.Add(this.labelLine7);
		this.panelControl2.Controls.Add(this.btnFtpUpdate);
		this.panelControl2.Controls.Add(this.labelLine8);
		this.panelControl2.Controls.Add(this.txt_Pass);
		this.panelControl2.Controls.Add(this.txt_User);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl2.Location = new System.Drawing.Point(0, 0);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(278, 135);
		this.panelControl2.TabIndex = 0;
		this.txt_Domain.Location = new System.Drawing.Point(92, 8);
		this.txt_Domain.Name = "txt_Domain";
		this.txt_Domain.Size = new System.Drawing.Size(174, 20);
		this.txt_Domain.TabIndex = 0;
		this.btnFtpCancel.Image = (System.Drawing.Image)resources.GetObject("btnFtpCancel.Image");
		this.btnFtpCancel.Location = new System.Drawing.Point(191, 99);
		this.btnFtpCancel.Name = "btnFtpCancel";
		this.btnFtpCancel.Size = new System.Drawing.Size(75, 23);
		this.btnFtpCancel.TabIndex = 7;
		this.btnFtpCancel.Text = "Exit";
		this.btnFtpCancel.Click += new System.EventHandler(this.btnFtpCancel_Click);
		this.labelLine5.Location = new System.Drawing.Point(12, 10);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.TabIndex = 8;
		this.labelLine5.Utext = "Domain";
		this.btnFtpRefresh.Image = (System.Drawing.Image)resources.GetObject("btnFtpRefresh.Image");
		this.btnFtpRefresh.Location = new System.Drawing.Point(93, 99);
		this.btnFtpRefresh.Name = "btnFtpRefresh";
		this.btnFtpRefresh.Size = new System.Drawing.Size(75, 23);
		this.btnFtpRefresh.TabIndex = 6;
		this.btnFtpRefresh.Text = "Refresh";
		this.btnFtpRefresh.Click += new System.EventHandler(this.btnFtpRefresh_Click);
		this.labelLine7.Location = new System.Drawing.Point(12, 37);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.Size = new System.Drawing.Size(80, 20);
		this.labelLine7.TabIndex = 10;
		this.labelLine7.Utext = "User Name";
		this.btnFtpUpdate.Image = (System.Drawing.Image)resources.GetObject("btnFtpUpdate.Image");
		this.btnFtpUpdate.Location = new System.Drawing.Point(12, 99);
		this.btnFtpUpdate.Name = "btnFtpUpdate";
		this.btnFtpUpdate.Size = new System.Drawing.Size(75, 23);
		this.btnFtpUpdate.TabIndex = 5;
		this.btnFtpUpdate.Text = "Update";
		this.btnFtpUpdate.Click += new System.EventHandler(this.btnFtpUpdate_Click);
		this.labelLine8.Location = new System.Drawing.Point(12, 63);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.Size = new System.Drawing.Size(80, 20);
		this.labelLine8.TabIndex = 11;
		this.labelLine8.Utext = "Password";
		this.txt_Pass.Location = new System.Drawing.Point(92, 60);
		this.txt_Pass.Name = "txt_Pass";
		this.txt_Pass.Properties.PasswordChar = '*';
		this.txt_Pass.Size = new System.Drawing.Size(174, 20);
		this.txt_Pass.TabIndex = 3;
		this.txt_User.Location = new System.Drawing.Point(92, 34);
		this.txt_User.Name = "txt_User";
		this.txt_User.Size = new System.Drawing.Size(174, 20);
		this.txt_User.TabIndex = 2;
		base.AcceptButton = this.btnFtpUpdate;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(278, 135);
		base.Controls.Add(this.panelControl2);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frm_FileCopySharingSettings";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "File Copy Sharing Settings";
		base.Load += new System.EventHandler(this.frm_FileCopySharingSettings_Load);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_Domain.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Pass.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_User.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
