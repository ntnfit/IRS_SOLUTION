using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Form;
using Apzon.Forms.Properties;
using Commons.Helper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;

namespace Apzon.Forms.UserControls;

[UserRepositoryItem("RegisterRepositoryItemAttachFile")]
public class RepositoryItemAttachFile : RepositoryItemButtonEdit
{
	public const string CustomEditName = "RepositoryItemAttachFile";

	private readonly FtpHelper _ftpHelper = new FtpHelper();

	[DefaultValue(ApzObjectTypes.oNone)]
	public ApzObjectTypes ObjType { get; set; }

	public RepositoryItemAttachFile()
	{
		EditorButton editorButton = new EditorButton();
		editorButton.Kind = ButtonPredefines.Glyph;
		editorButton.ImageOptions.Image = Resources.Show_16x16;
		editorButton.ToolTip = "Open";
		editorButton.ImageLocation = ImageLocation.MiddleCenter;
		editorButton.IsLeft = true;
		EditorButton editorButton2 = new EditorButton();
		editorButton2.Kind = ButtonPredefines.Search;
		editorButton2.ImageOptions.Image = Resources.Open_16x16;
		editorButton2.ToolTip = "Browse";
		editorButton2.ImageLocation = ImageLocation.MiddleCenter;
		base.Buttons.Clear();
		base.Buttons.Add(editorButton2);
		base.Buttons.Add(editorButton);
		ButtonClick += OnButtonClick;
	}

	static RepositoryItemAttachFile()
	{
		RegisterRepositoryItemAttachFile();
	}

	private void OnButtonClick(object sender, ButtonPressedEventArgs e)
	{
		if (e.Button.ToolTip == "Browse")
		{
			btBrowse_Click(sender, e);
		}
		else
		{
			btDisplay_Click(sender, e);
		}
	}

	public static void RegisterRepositoryItemAttachFile()
	{
		Image image = null;
		EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("RepositoryItemAttachFile", typeof(ButtonEditAttach), typeof(RepositoryItemAttachFile), typeof(RepositoryItemButtonEdit), new ButtonEditPainter(), designTimeVisible: true, image));
	}

	private string GetObjectPath()
	{
		string result = "";
		if (ObjType != ApzObjectTypes.oNone)
		{
			int objType = (int)ObjType;
			if (CopyFileSharingCfg.ObjectPath.ContainsKey(objType))
			{
				result = CopyFileSharingCfg.ObjectPath[objType];
			}
		}
		return result;
	}

	private void btBrowse_Click(object sender, EventArgs e)
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			string objectPath = GetObjectPath();
			string text = ((!string.IsNullOrEmpty(objectPath)) ? objectPath : CopyFileSharingCfg.AttachPath);
			if (!Directory.Exists(text))
			{
				WinCommons.Error(Enums.MessageCode.AttachmentPathNotAccess, "", text);
				return;
			}
			openFileDialog.InitialDirectory = text;
			openFileDialog.Multiselect = false;
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string[] fileNames = openFileDialog.FileNames;
			if (fileNames.Length == 0)
			{
				return;
			}
			for (int i = 0; i < fileNames.Length; i++)
			{
				string text2 = Path.GetExtension(fileNames[i]).Remove(0, 1);
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileNames[i]);
				string text3 = ((!string.IsNullOrEmpty(objectPath)) ? objectPath : CopyFileShareHelper.GetUpLoadPath(text2));
				if (!CopyFileSharingCfg.DisableSplitFolder)
				{
					DateTime now = DateTime.Now;
					if (new DirectoryInfo(text3).Exists)
					{
						text3 = Path.Combine(text3, now.ToString("dd") + "-" + now.ToString("MM") + "-" + now.ToString("yyyy"));
						DirectoryInfo directoryInfo = new DirectoryInfo(text3);
						if (!directoryInfo.Exists)
						{
							directoryInfo.Create();
						}
					}
				}
				string fullPath = Path.GetFullPath(fileNames[i]);
				string fullDestPath = "";
				if (UploadFile(fullPath, text3, fileNameWithoutExtension, text2, ref fullDestPath) && sender is ButtonEdit buttonEdit)
				{
					buttonEdit.EditValue = fullDestPath;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btDisplay_Click(object sender, EventArgs e)
	{
		try
		{
			if (sender is ButtonEdit buttonEdit)
			{
				string text = Function.ToString(buttonEdit.EditValue);
				if (!(text == ""))
				{
					Process.Start(text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			if (ex is FileNotFoundException)
			{
				WinCommons.Error(Enums.MessageCode.AttachmentChangedOrDelete, "");
			}
			else
			{
				WinCommons.ErrorLineNumber(ex);
			}
		}
	}

	public bool UploadFile(string fullSourcePath, string trgtPath, string fileName, string extension, ref string fullDestPath)
	{
		try
		{
			if (!string.IsNullOrEmpty(fullSourcePath) && File.Exists(fullSourcePath))
			{
				fullDestPath = Path.Combine(trgtPath, fileName) + "." + extension;
				if (!fullSourcePath.Equals(fullDestPath))
				{
					string retUpload = _ftpHelper.CopyFileSharing(CopyFileSharingCfg.Domain, fullSourcePath, CopyFileSharingCfg.UserName, CopyFileSharingCfg.Password, fullDestPath);
					if (retUpload != fullDestPath)
					{
						if (!retUpload.StartsWith("1000002 -"))
						{
							WinCommons._CloseProcessing();
							WinCommons.Error(Enums.MessageCode.UploadAttachmentFailed, "", retUpload);
							return false;
						}
						if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRenameDestFileName, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, fullDestPath) != DialogResult.Yes)
						{
							WinCommons._CloseProcessing();
							WinCommons.Error(Enums.MessageCode.None, retUpload);
							return false;
						}
						if (!AutoRename(fullSourcePath, ref fileName, extension, trgtPath, ref fullDestPath, ref retUpload))
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return false;
	}

	private bool AutoRename(string sPath, ref string sName, string sExt, string upLoadPath, ref string fullDestPath, ref string retUpload)
	{
		int num = 1;
		string text = sName;
		while (File.Exists(fullDestPath))
		{
			text = $"{sName}({num++})";
			fullDestPath = Path.Combine(upLoadPath, text) + "." + sExt;
		}
		retUpload = _ftpHelper.CopyFileSharing(CopyFileSharingCfg.Domain, sPath, CopyFileSharingCfg.UserName, CopyFileSharingCfg.Password, fullDestPath);
		if (retUpload != fullDestPath)
		{
			WinCommons._CloseProcessing();
			if (retUpload.StartsWith("1000002 -"))
			{
				WinCommons.Error(Enums.MessageCode.None, retUpload);
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.UploadAttachmentFailed, "", retUpload);
			}
			return false;
		}
		return true;
	}

	public override void Assign(RepositoryItem item)
	{
		BeginUpdate();
		try
		{
			base.Assign(item);
			_ = item is RepositoryItemAttachFile;
		}
		finally
		{
			EndUpdate();
		}
	}
}
