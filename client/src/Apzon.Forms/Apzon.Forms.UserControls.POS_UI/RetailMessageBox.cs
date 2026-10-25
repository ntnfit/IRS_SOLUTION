using System;
using System.Diagnostics;
using System.Windows.Forms;
using Apzon.Commons;

namespace Apzon.Forms.UserControls.POS_UI;

public static class RetailMessageBox
{
	public static DialogResult Show(string message = "", string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1, MessageBoxIcon icon = MessageBoxIcon.Asterisk)
	{
		try
		{
			RetailMessageBoxModal retailMessageBoxModal = new RetailMessageBoxModal(message, caption, buttons, defaultButton, icon);
			retailMessageBoxModal.ShowDialog();
			return retailMessageBoxModal.DialogResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return DialogResult.None;
	}
}
