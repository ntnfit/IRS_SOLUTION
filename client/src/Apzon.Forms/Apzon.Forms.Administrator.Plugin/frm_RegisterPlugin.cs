using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Administrator.Plugin;

public class frm_RegisterPlugin : XtraForm
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private LabelLine lbHeader;

	private LabelLine lbFolder;

	private ButtonEditEx txtFolder;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnOk;

	public frm_RegisterPlugin()
	{
		InitializeComponent();
		DialogResult = DialogResult.None;
	}

	private void txtFolder_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.Description = "Select folder plugin to register";
			if (folderBrowserDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
			{
				txtFolder.EditValue = folderBrowserDialog.SelectedPath;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.Cancel;
		Close();
	}

	private async void btnOk_Click(object sender, EventArgs e)
	{
		SimpleButtonEx simpleButtonEx = btnOk;
		SimpleButtonEx simpleButtonEx2 = btnCancel;
		bool flag = (txtFolder.Enabled = false);
		bool enabled = (simpleButtonEx2.Enabled = flag);
		simpleButtonEx.Enabled = enabled;
		try
		{
			if (string.IsNullOrEmpty(txtFolder.Text))
			{
				WinCommons.Error(Enums.MessageCode.YouMustChooseFolderPluginRegister, "");
				SimpleButtonEx simpleButtonEx3 = btnOk;
				SimpleButtonEx simpleButtonEx4 = btnCancel;
				flag = (txtFolder.Enabled = true);
				enabled = (simpleButtonEx4.Enabled = flag);
				simpleButtonEx3.Enabled = enabled;
				return;
			}
			if (!Directory.Exists(txtFolder.Text))
			{
				WinCommons.Error(Enums.MessageCode.FolderPluginRegisterDoesNotExists, "");
				SimpleButtonEx simpleButtonEx5 = btnOk;
				SimpleButtonEx simpleButtonEx6 = btnCancel;
				flag = (txtFolder.Enabled = true);
				enabled = (simpleButtonEx6.Enabled = flag);
				simpleButtonEx5.Enabled = enabled;
				return;
			}
			if (!File.Exists(string.Format("{0}\\{1}", txtFolder.Text, "PluginConfig.xml")))
			{
				WinCommons.Error(Enums.MessageCode.CannotLoadPlugInFile, "");
				SimpleButtonEx simpleButtonEx7 = btnOk;
				SimpleButtonEx simpleButtonEx8 = btnCancel;
				flag = (txtFolder.Enabled = true);
				enabled = (simpleButtonEx8.Enabled = flag);
				simpleButtonEx7.Enabled = enabled;
				return;
			}
			XElement xElement = XDocument.Load(string.Format("{0}\\{1}", txtFolder.Text, "PluginConfig.xml")).Element("assembly");
			if (xElement == null)
			{
				WinCommons.Error(Enums.MessageCode.CannotLoadPlugInFile, "");
				SimpleButtonEx simpleButtonEx9 = btnOk;
				SimpleButtonEx simpleButtonEx10 = btnCancel;
				flag = (txtFolder.Enabled = true);
				enabled = (simpleButtonEx10.Enabled = flag);
				simpleButtonEx9.Enabled = enabled;
				return;
			}
			XElement xElement2 = xElement.Element("Name");
			if (xElement2 == null)
			{
				WinCommons.Error(Enums.MessageCode.CannotLoadPlugInFile, "");
				SimpleButtonEx simpleButtonEx11 = btnOk;
				SimpleButtonEx simpleButtonEx12 = btnCancel;
				flag = (txtFolder.Enabled = true);
				enabled = (simpleButtonEx12.Enabled = flag);
				simpleButtonEx11.Enabled = enabled;
				return;
			}
			string name = Function.ToString(xElement2.Value);
			if (string.IsNullOrEmpty(name))
			{
				WinCommons.Error(Enums.MessageCode.CannotLoadPlugInFile, "");
				SimpleButtonEx simpleButtonEx13 = btnOk;
				SimpleButtonEx simpleButtonEx14 = btnCancel;
				flag = (txtFolder.Enabled = true);
				enabled = (simpleButtonEx14.Enabled = flag);
				simpleButtonEx13.Enabled = enabled;
				return;
			}
			WinCommons.Info(Enums.MessageCode.UninstallPluginPleaseWait, "", name);
			string folderName = string.Format("{0}\\{3}\\{1}\\{2}", Environment.CurrentDirectory, GlobalData.DatabaseName, name, "Extensions");
			if (Directory.Exists(folderName))
			{
				WinCommons.Error(Enums.MessageCode.DoNotHavePermissionAcessFolder, "");
				SimpleButtonEx simpleButtonEx15 = btnOk;
				SimpleButtonEx simpleButtonEx16 = btnCancel;
				flag = (txtFolder.Enabled = true);
				enabled = (simpleButtonEx16.Enabled = flag);
				simpleButtonEx15.Enabled = enabled;
				return;
			}
			Function.CopyFolder(txtFolder.Text, folderName);
			if (!string.IsNullOrEmpty(await _httpClient.FileNetworkManagerClient.UpLoadFolder(folderName, "Extensions", isOverride: false)))
			{
				try
				{
					Function.DeleteFolder(folderName);
				}
				catch (Exception)
				{
				}
				WinCommons.Error(Enums.MessageCode.CouldNotRegisterPluginUploadFileToServerFailed, "");
				SimpleButtonEx simpleButtonEx17 = btnOk;
				SimpleButtonEx simpleButtonEx18 = btnCancel;
				flag = (txtFolder.Enabled = true);
				enabled = (simpleButtonEx18.Enabled = flag);
				simpleButtonEx17.Enabled = enabled;
				return;
			}
			DataTable dataTable = new DataTable();
			dataTable.Columns.AddRange(new DataColumn[5]
			{
				new DataColumn("PlugId", typeof(int)),
				new DataColumn("Name", typeof(string)),
				new DataColumn("Platform", typeof(string)),
				new DataColumn("Group", typeof(string)),
				new DataColumn("Version", typeof(string))
			});
			DataRow dataRow = dataTable.NewRow();
			dataRow[0] = 0;
			dataRow[1] = name;
			dataRow[2] = Function.GetPlatform();
			dataRow[3] = "M";
			dataRow[4] = "1.0.0";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			string result = await _httpClient.PluginManagerClient.Save(dataTable);
			if (!string.IsNullOrEmpty(result))
			{
				try
				{
					Function.DeleteFolder(folderName);
					await _httpClient.FileNetworkManagerClient.DeleteFolder(string.Format("{0}\\{1}", "Extensions", name));
				}
				catch (Exception)
				{
				}
				WinCommons.Error(Enums.MessageCode.None, result);
				SimpleButtonEx simpleButtonEx19 = btnOk;
				SimpleButtonEx simpleButtonEx20 = btnCancel;
				flag = (txtFolder.Enabled = true);
				enabled = (simpleButtonEx20.Enabled = flag);
				simpleButtonEx19.Enabled = enabled;
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.RegisterPluginSuccess, "");
				DialogResult = DialogResult.OK;
				Close();
			}
		}
		catch (Exception ex3)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex3.Message);
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.Plugin.frm_RegisterPlugin));
		this.lbHeader = new Apzon.Controls.Controls.LabelLine();
		this.lbFolder = new Apzon.Controls.Controls.LabelLine();
		this.txtFolder = new Apzon.Controls.Editor.ButtonEditEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOk = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.txtFolder.Properties).BeginInit();
		base.SuspendLayout();
		this.lbHeader.Location = new System.Drawing.Point(12, 17);
		this.lbHeader.Name = "lbHeader";
		this.lbHeader.Size = new System.Drawing.Size(131, 20);
		this.lbHeader.TabIndex = 0;
		this.lbHeader.Utext = "Installation Information:";
		this.lbFolder.Location = new System.Drawing.Point(42, 44);
		this.lbFolder.Name = "lbFolder";
		this.lbFolder.Size = new System.Drawing.Size(134, 20);
		this.lbFolder.TabIndex = 1;
		this.lbFolder.Utext = "Registration Data Folder";
		this.txtFolder.Location = new System.Drawing.Point(176, 40);
		this.txtFolder.Name = "txtFolder";
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.txtFolder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(editorButtonImageOptions, DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, null)
		});
		this.txtFolder.Size = new System.Drawing.Size(238, 22);
		this.txtFolder.TabIndex = 2;
		this.txtFolder.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtFolder_ButtonClick);
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.Location = new System.Drawing.Point(220, 75);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 3;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnOk.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOk.ImageOptions.Image");
		this.btnOk.Location = new System.Drawing.Point(138, 75);
		this.btnOk.Name = "btnOk";
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 4;
		this.btnOk.Text = "Register";
		this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(431, 115);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.txtFolder);
		base.Controls.Add(this.lbFolder);
		base.Controls.Add(this.lbHeader);
		base.Name = "frm_RegisterPlugin";
		base.ShowIcon = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Register Plugin";
		((System.ComponentModel.ISupportInitialize)this.txtFolder.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
