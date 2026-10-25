using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Controls.Editor;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.UserControls;

[ToolboxItem(true)]
public class ChooseFormListUDF : ButtonEditEx
{
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public new RepositoryItemButtonEditCFL Properties => base.Properties as RepositoryItemButtonEditCFL;

	public override string EditorTypeName => "ButtonEditCFL";

	static ChooseFormListUDF()
	{
		RepositoryItemButtonEditCFL.RegisterButtonEditCFL();
	}

	public ChooseFormListUDF()
	{
		Validating += ChooseFormListUDF_Validating;
		KeyDown += ChooseFormListUDF_KeyDown;
		PreviewKeyDown += ChooseFormListUDF_PreviewKeyDown;
		Enter += ChooseFormListUDF_Enter;
		InvalidValue += ChooseFormListUDF_InvalidValue;
	}

	private void ChooseFormListUDF_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
	{
		e.ExceptionMode = ExceptionMode.NoAction;
	}

	private void ChooseFormListUDF_Enter(object sender, EventArgs e)
	{
		Properties.ButtonEditCFL_Enter(sender, e);
	}

	private void ChooseFormListUDF_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

	private void ChooseFormListUDF_KeyDown(object sender, KeyEventArgs e)
	{
		Properties.RepositoryItemButtonEditCFL_KeyDown(sender, e);
	}

	private void ChooseFormListUDF_Validating(object sender, CancelEventArgs e)
	{
		Properties.ButtonEditCFL_Validating(sender, e);
	}
}
