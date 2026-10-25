using System.Windows.Forms;
using Apzon.Commons.Enums;
using Apzon.Global;
using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace Apzon.Controls.Form;

public static class XtraMessageBoxEx
{
	public static DialogResult Show(Enums.MessageCode mesageCode, Enums.MessageCode captionCode, MessageBoxButtons buttons)
	{
		return XtraMessageBox.Show(GlobalData.MessageDataCenter[mesageCode], GlobalData.MessageDataCenter[captionCode], buttons);
	}

	public static DialogResult Show(Enums.MessageCode mesageCode, Enums.MessageCode captionCode)
	{
		return XtraMessageBox.Show(GlobalData.MessageDataCenter[mesageCode], GlobalData.MessageDataCenter[captionCode]);
	}

	public static DialogResult Show(Enums.MessageCode mesageCode, Enums.MessageCode captionCode, MessageBoxButtons buttons, MessageBoxIcon icon)
	{
		return XtraMessageBox.Show(GlobalData.MessageDataCenter[mesageCode], GlobalData.MessageDataCenter[captionCode], buttons, icon);
	}

	public static DialogResult Show(Enums.MessageCode mesageCode, Enums.MessageCode captionCode, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
	{
		return XtraMessageBox.Show(GlobalData.MessageDataCenter[mesageCode], GlobalData.MessageDataCenter[captionCode], buttons, icon, defaultButton);
	}

	public static DialogResult Show(Enums.MessageCode mesageCode, Enums.MessageCode captionCode, MessageBoxButtons buttons, MessageBoxIcon icon, DefaultBoolean allowHtmlText)
	{
		return XtraMessageBox.Show(GlobalData.MessageDataCenter[mesageCode], GlobalData.MessageDataCenter[captionCode], buttons, icon, allowHtmlText);
	}

	public static DialogResult Show(IWin32Window owner, Enums.MessageCode mesageCode, Enums.MessageCode captionCode, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
	{
		return XtraMessageBox.Show(owner, GlobalData.MessageDataCenter[mesageCode], GlobalData.MessageDataCenter[captionCode], buttons, icon, defaultButton);
	}

	public static DialogResult Show(Enums.MessageCode mesageCode, Enums.MessageCode captionCode, MessageBoxButtons buttons, MessageBoxIcon icon, params string[] additionalInfo)
	{
		string text = GlobalData.MessageDataCenter[mesageCode];
		if (additionalInfo != null)
		{
			for (int i = 0; i < additionalInfo.Length; i++)
			{
				text = text.Replace($"{{{i}}}", additionalInfo[i]);
			}
		}
		return XtraMessageBox.Show(text, GlobalData.MessageDataCenter[captionCode], buttons, icon);
	}

	public static DialogResult Show(IWin32Window owner, Enums.MessageCode mesageCode, Enums.MessageCode captionCode, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, params string[] additionalInfo)
	{
		string text = GlobalData.MessageDataCenter[mesageCode];
		if (additionalInfo != null)
		{
			for (int i = 0; i < additionalInfo.Length; i++)
			{
				text = text.Replace($"{{{i}}}", additionalInfo[i]);
			}
		}
		return XtraMessageBox.Show(owner, text, GlobalData.MessageDataCenter[captionCode], buttons, icon, defaultButton);
	}

	public static DialogResult Show(Enums.MessageCode mesageCode, Enums.MessageCode captionCode, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, params string[] additionalInfo)
	{
		string text = GlobalData.MessageDataCenter[mesageCode];
		if (additionalInfo != null)
		{
			for (int i = 0; i < additionalInfo.Length; i++)
			{
				text = text.Replace($"{{{i}}}", additionalInfo[i]);
			}
		}
		return XtraMessageBox.Show(text, GlobalData.MessageDataCenter[captionCode], buttons, icon, defaultButton);
	}

	public static DialogResult Show(Enums.MessageCode mesageCode, Enums.MessageCode captionCode, params string[] additionalInfo)
	{
		string text = GlobalData.MessageDataCenter[mesageCode];
		if (additionalInfo != null)
		{
			for (int i = 0; i < additionalInfo.Length; i++)
			{
				text = text.Replace($"{{{i}}}", additionalInfo[i]);
			}
		}
		return XtraMessageBox.Show(text, GlobalData.MessageDataCenter[captionCode]);
	}

	public static DialogResult Show(Enums.MessageCode mesageCode, Enums.MessageCode captionCode, MessageBoxButtons buttons, params string[] additionalInfo)
	{
		string text = GlobalData.MessageDataCenter[mesageCode];
		if (additionalInfo != null)
		{
			for (int i = 0; i < additionalInfo.Length; i++)
			{
				text = text.Replace($"{{{i}}}", additionalInfo[i]);
			}
		}
		return XtraMessageBox.Show(text, GlobalData.MessageDataCenter[captionCode], buttons);
	}
}
