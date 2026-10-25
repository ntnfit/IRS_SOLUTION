using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Controls.Editor;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.UserControls;

[ToolboxItem(true)]
public class ButtonEditCFL : ButtonEditEx
{
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public new RepositoryItemButtonEditCFL Properties => base.Properties as RepositoryItemButtonEditCFL;

	public override string EditorTypeName => "ButtonEditCFL";

	static ButtonEditCFL()
	{
		RepositoryItemButtonEditCFL.RegisterButtonEditCFL();
	}

	public ButtonEditCFL()
	{
		Validating += ButtonEditCFL_Validating;
		KeyDown += ButtonEditCFL_KeyDown;
		PreviewKeyDown += ButtonEditCFL_PreviewKeyDown;
		Enter += ButtonEditCFL_Enter;
		InvalidValue += ButtonEditCFL_InvalidValue;
	}

	private void ButtonEditCFL_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
	{
		e.ExceptionMode = ExceptionMode.NoAction;
	}

	private void ButtonEditCFL_Enter(object sender, EventArgs e)
	{
		Properties.ButtonEditCFL_Enter(sender, e);
	}

	private void ButtonEditCFL_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		try
		{
			if (!Properties.TabWhenEmpty || e.KeyCode != Keys.Tab)
			{
				return;
			}
			if (e.Shift)
			{
				e.IsInputKey = true;
				return;
			}
			ButtonEditCFL obj = sender as ButtonEditCFL;
			string value = Function.ToString(obj.EditValue);
			string text = obj.Text;
			if (string.IsNullOrEmpty(text) && text.Equals(value))
			{
				e.IsInputKey = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void ButtonEditCFL_KeyDown(object sender, KeyEventArgs e)
	{
		Properties.RepositoryItemButtonEditCFL_KeyDown(sender, e);
	}

	private void ButtonEditCFL_Validating(object sender, CancelEventArgs e)
	{
		Properties.ButtonEditCFL_Validating(sender, e);
	}
}
