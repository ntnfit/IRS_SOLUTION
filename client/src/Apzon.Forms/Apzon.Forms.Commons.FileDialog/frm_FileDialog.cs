using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Commons.FileDialog;

public class frm_FileDialog : XtraForm
{
	private IContainer components;

	private LabelLine lbChooseFile;

	private SimpleButtonEx btnOk;

	private SimpleButtonEx btnCancel;

	private ButtonEditEx txtPathFile;

	public string FileName { get; set; }

	public bool MultiSelect { get; set; }

	public Enums.TypeCFL FormType { get; set; }

	public frm_FileDialog()
	{
		InitializeComponent();
		DialogResult = DialogResult.Cancel;
	}

	public frm_FileDialog(Enums.TypeCFL objectType, bool multiselect)
	{
		InitializeComponent();
		MultiSelect = multiselect;
		FormType = objectType;
		DialogResult = DialogResult.Cancel;
	}

	private void txtPathFile_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			using OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Multiselect = MultiSelect
			};
			if (FormType == Enums.TypeCFL.ImportFileCertificate)
			{
				openFileDialog.Filter = "TXT files|*.txt";
				openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			}
			if (openFileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(openFileDialog.FileName))
			{
				txtPathFile.Text = openFileDialog.FileName;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btnOk_Click(object sender, EventArgs e)
	{
		try
		{
			FileName = txtPathFile.Text;
			DoAction();
			if (DialogResult == DialogResult.OK)
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void DoAction()
	{
		DialogResult = DialogResult.OK;
		if (FormType != Enums.TypeCFL.ImportFileCertificate)
		{
			return;
		}
		if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Certificate")))
		{
			Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Certificate"));
		}
		if (!string.IsNullOrEmpty(FileName))
		{
			if (IsValidCertificate(FileName))
			{
				File.Copy(FileName, Path.Combine(Environment.CurrentDirectory, "Certificate", Path.GetFileName(FileName)));
				WinCommons.Success(Enums.MessageCode.ImportSuccess, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, "Invalid certificate file.");
				DialogResult = DialogResult.None;
			}
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.YouMustChooseCetificateFile2Import, "");
			DialogResult = DialogResult.None;
		}
	}

	private bool IsValidCertificate(string fileName)
	{
		try
		{
			using StreamReader streamReader = new StreamReader(fileName, Encoding.UTF8);
			streamReader.ReadLine();
			streamReader.ReadLine();
			streamReader.ReadLine();
			streamReader.ReadLine();
			streamReader.ReadLine();
			streamReader.ReadLine();
			Function.Decrypt(RSA.PrivateKey, streamReader.ReadLine());
			return true;
		}
		catch
		{
			return false;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.FileDialog.frm_FileDialog));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		this.lbChooseFile = new Apzon.Controls.Controls.LabelLine();
		this.btnOk = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtPathFile = new Apzon.Controls.Editor.ButtonEditEx();
		((System.ComponentModel.ISupportInitialize)this.txtPathFile.Properties).BeginInit();
		base.SuspendLayout();
		this.lbChooseFile.Location = new System.Drawing.Point(29, 33);
		this.lbChooseFile.Name = "lbChooseFile";
		this.lbChooseFile.Size = new System.Drawing.Size(80, 20);
		this.lbChooseFile.TabIndex = 0;
		this.lbChooseFile.Utext = "Choose File";
		this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnOk.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOk.ImageOptions.Image");
		this.btnOk.Location = new System.Drawing.Point(133, 65);
		this.btnOk.Name = "btnOk";
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 3;
		this.btnOk.Text = "Ok";
		this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.Location = new System.Drawing.Point(216, 65);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 4;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.txtPathFile.Location = new System.Drawing.Point(109, 30);
		this.txtPathFile.Name = "txtPathFile";
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.txtPathFile.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(editorButtonImageOptions, DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, null)
		});
		this.txtPathFile.Size = new System.Drawing.Size(281, 22);
		this.txtPathFile.TabIndex = 5;
		this.txtPathFile.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtPathFile_ButtonClick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(426, 106);
		base.Controls.Add(this.txtPathFile);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.lbChooseFile);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.IconOptions.ShowIcon = false;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frm_FileDialog";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Choose File";
		((System.ComponentModel.ISupportInitialize)this.txtPathFile.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
