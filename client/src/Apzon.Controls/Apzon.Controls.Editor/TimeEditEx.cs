using System;
using System.ComponentModel;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Controls.Editor;

[ToolboxItem(true)]
public class TimeEditEx : TextEditEx
{
	public override string EditorTypeName => "TimeEditEx";

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public new RepositoryItemTimeEditEx Properties => base.Properties as RepositoryItemTimeEditEx;

	static TimeEditEx()
	{
		RepositoryItemTimeEditEx.RegisterRepositoryItemTimeEditEx();
	}

	public TimeEditEx()
	{
		Properties.DisplayFormat.FormatString = "##:##";
		Properties.DisplayFormat.FormatType = FormatType.Custom;
		Properties.EditFormat.FormatString = "##:##";
		Properties.EditFormat.FormatType = FormatType.Custom;
		CustomDisplayText += TimeEditEx_CustomDisplayText;
		Click += TimeEditEx_Click;
	}

	private void TimeEditEx_Click(object sender, EventArgs e)
	{
		SelectAll();
		Focus();
	}

	private void TimeEditEx_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		try
		{
			string source = Function.ToString(e.Value);
			source = new string(source.Where((char c) => char.IsDigit(c)).ToArray());
			if (source != "")
			{
				int num = Function.ParseInt(source);
				e.DisplayText = (num / 100).ToString("00") + ":" + (num % 100).ToString("00");
			}
		}
		catch (Exception)
		{
		}
	}

	protected override void OnEnter(EventArgs e)
	{
		base.OnEnter(e);
		SelectAll();
	}

	protected override void OnValidating(CancelEventArgs e)
	{
		base.OnValidating(e);
		string text = Function.ToString(EditValue);
		if (!(text != ""))
		{
			return;
		}
		text = new string(text.Where((char c) => char.IsDigit(c)).ToArray());
		if (text != "")
		{
			int num = Function.ParseInt(text);
			if (num < 0 || num > 2400 || num % 100 > 59)
			{
				e.Cancel = true;
				EditValue = OldEditValue;
				WinCommons.Error(Enums.MessageCode.None, "Invalid time value");
			}
			else if (Function.ToString(EditValue) != text)
			{
				EditValue = text;
			}
		}
		else
		{
			e.Cancel = true;
			EditValue = OldEditValue;
			WinCommons.Error(Enums.MessageCode.None, "Invalid time value");
		}
	}
}
