using System;
using System.Diagnostics;
using System.Windows.Forms;
using Apzon.Commons;
using DevExpress.XtraEditors;

namespace Apzon.Forms.UserControls.POS_UI;

public static class RetailDialog
{
	public static DialogResult Show(XtraUserControl control, Action dialogShown = null, string message = "", string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1, MessageBoxIcon icon = MessageBoxIcon.Asterisk)
	{
		try
		{
			MaskForm maskForm = new MaskForm(control, message, caption);
			maskForm.FrmShown = dialogShown;
			maskForm.ShowDialog();
			return maskForm.DialogResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return DialogResult.None;
	}

	public static DialogResult ShowForm(XtraForm form, string message = "", string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1, MessageBoxIcon icon = MessageBoxIcon.Asterisk)
	{
		try
		{
			MaskForm maskForm = new MaskForm(form);
			maskForm.ShowDialog();
			return maskForm.DialogResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return DialogResult.None;
	}
}
