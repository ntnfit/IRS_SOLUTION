using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Controls.Grid;
using Apzon.Global;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;

namespace Apzon.Controls.Editor;

[UserRepositoryItem("RegisterMeasureUnitEditEx")]
public class RepositoryItemMeasureUnitEditEx : RepositoryItemTextEdit
{
	public static readonly char[] NumberArray;

	public const string CustomEditName = "MeasureUnitEditEx";

	public override string EditorTypeName => "MeasureUnitEditEx";

	static RepositoryItemMeasureUnitEditEx()
	{
		NumberArray = new char[10] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
		RegisterMeasureUnitEditEx();
	}

	public static void RegisterMeasureUnitEditEx()
	{
		Image image = null;
		EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("MeasureUnitEditEx", typeof(TextEdit), typeof(RepositoryItemMeasureUnitEditEx), typeof(TextEditViewInfo), new TextEditPainter(), designTimeVisible: true, image));
	}

	public override void Assign(RepositoryItem item)
	{
		BeginUpdate();
		try
		{
			base.Assign(item);
		}
		finally
		{
			EndUpdate();
		}
	}

	public RepositoryItemMeasureUnitEditEx()
	{
		try
		{
			KeyPress += KeyDownProcess;
			EditValueChanging += EditValueChangingProcess;
			Enter += EnterProcess;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void EnterProcess(object sender, EventArgs e)
	{
		try
		{
			TextEdit control = sender as TextEdit;
			if (control == null)
			{
				return;
			}
			Control parent = control.Parent;
			if (parent == null)
			{
				return;
			}
			string valueText = "";
			if (!(parent.GetType() == typeof(GridControlEx)))
			{
				return;
			}
			if (parent is GridControlEx gridControlEx)
			{
				dynamic focusedView = gridControlEx.FocusedView;
				if (focusedView == null)
				{
					return;
				}
				valueText = Function.ToString(focusedView.GetFocusedRowCellDisplayText(focusedView.FocusedColumn));
			}
			control.EditValue = valueText;
			control.BeginInvoke((Action)(() =>
			{
				control.SelectionLength = GetIndexUnitSeparation(valueText);
				control.SelectionStart = 0;
			}));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void EditValueChangingProcess(object sender, ChangingEventArgs e)
	{
		try
		{
			TextEdit control = sender as TextEdit;
			if (control == null)
			{
				return;
			}
			int indexCosor = control.SelectionStart;
			string text = e.NewValue as string;
			if (text == null)
			{
				return;
			}
			int num = text.IndexOf(GlobalData.GeneralSetting.DecSep, StringComparison.Ordinal) - 1;
			if (num == -2)
			{
				for (int num2 = text.Length - 1; num2 >= 0; num2--)
				{
					if (NumberArray.Contains(text[num2]))
					{
						num = num2;
						break;
					}
				}
			}
			for (int num3 = num - 1; num3 >= 0; num3--)
			{
				if (Function.ToString(text[num3]) == GlobalData.GeneralSetting.ThousSep)
				{
					text = text.Remove(num3, 1);
					indexCosor--;
					num--;
				}
			}
			int num4 = 1;
			for (int num5 = num - 1; num5 >= 0; num5--)
			{
				if (!NumberArray.Contains(text[num5]))
				{
					num4 = 0;
				}
				else if (num4 == 3)
				{
					text = text.Insert(num5 + 1, GlobalData.GeneralSetting.ThousSep);
					indexCosor++;
					num4 = 1;
				}
				else
				{
					num4++;
				}
			}
			e.NewValue = text;
			control.BeginInvoke((MethodInvoker)(() =>
			{
				control.SelectionLength = 0;
				if (indexCosor >= 0)
				{
					control.SelectionStart = indexCosor;
				}
			}));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void KeyDownProcess(object sender, KeyPressEventArgs e)
	{
		try
		{
			if (e.KeyChar == '.' || e.KeyChar == ',')
			{
				e.KeyChar = Function.ParseChar(GlobalData.GeneralSetting.DecSep) ?? e.KeyChar;
				e.Handled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public int GetIndexUnitSeparation(string value)
	{
		try
		{
			if (string.IsNullOrEmpty(value) || value.Length == 0)
			{
				return 0;
			}
			value = value.Trim().ToLower();
			char[] lstCharactor = value.ToArray();
			int i;
			for (i = 0; i < lstCharactor.Length; i++)
			{
				if (!NumberArray.Any((char t) => t == lstCharactor[i]) && !(GlobalData.GeneralSetting.DecSep == Function.ToString(lstCharactor[i])) && !(GlobalData.GeneralSetting.ThousSep == Function.ToString(lstCharactor[i])))
				{
					return i;
				}
			}
			return value.Length;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}
}
