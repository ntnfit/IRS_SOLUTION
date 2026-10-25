using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Apzon.Commons;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;

namespace Apzon.Controls.Controls;

[UserRepositoryItem("RegisterHourEdit")]
public class RepositoryItemHourEdit : RepositoryItemTextEdit
{
	private List<string> splitter = new List<string> { ":" };

	public const string CustomEditName = "HourEdit";

	public bool IsSearchControl { get; set; }

	public override string EditorTypeName => "HourEdit";

	static RepositoryItemHourEdit()
	{
		RegisterHourEdit();
	}

	public RepositoryItemHourEdit()
	{
		MaxLength = 5;
		ParseEditValue += RepositoryItemHourEdit_ParseEditValue;
		Validating += RepositoryItemHourEdit_Validating;
		CustomDisplayText += RepositoryItemHourEdit_CustomDisplayText;
		FormatEditValue += RepositoryItemHourEdit_FormatEditValue;
	}

	private void RepositoryItemHourEdit_FormatEditValue(object sender, ConvertEditValueEventArgs e)
	{
		try
		{
			string text = ((e.Value == null) ? "" : e.Value.ToString());
			switch (text.Length)
			{
			case 4:
				e.Value = text.Insert(2, ":");
				break;
			case 3:
				e.Value = text.Insert(0, "0").Insert(2, ":");
				break;
			case 2:
				e.Value = "00:" + text;
				break;
			case 1:
				e.Value = "00:0" + text;
				break;
			case 0:
				break;
			}
		}
		catch
		{
		}
	}

	private void RepositoryItemHourEdit_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		try
		{
			string text = Function.ToString(e.Value);
			switch (text.Length)
			{
			case 4:
				e.DisplayText = text.Insert(2, ":");
				break;
			case 3:
				e.DisplayText = text.Insert(0, "0").Insert(2, ":");
				break;
			case 2:
				e.DisplayText = "00:" + text;
				break;
			case 1:
				e.DisplayText = "00:0" + text;
				break;
			case 0:
				break;
			}
		}
		catch
		{
		}
	}

	protected internal void RepositoryItemHourEdit_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			string text = Convert.ToString((sender as TextEdit).EditValue);
			if (!(text != ""))
			{
				return;
			}
			try
			{
				int valueMinutes = 0;
				int valueHours = 0;
				FormatValue(text, ref valueMinutes, ref valueHours);
				if (valueHours >= 24 || valueMinutes >= 60)
				{
					e.Cancel = true;
				}
			}
			catch
			{
				e.Cancel = true;
			}
		}
		catch (Exception)
		{
		}
	}

	private void RepositoryItemHourEdit_ParseEditValue(object sender, ConvertEditValueEventArgs e)
	{
		try
		{
			string eValue = Convert.ToString(e.Value);
			string text = new string(eValue.Where((char c) => char.IsDigit(c)).ToArray());
			if (!int.TryParse(text, out var _))
			{
				if (eValue == "")
				{
					e.Value = null;
				}
				return;
			}
			bool flag = splitter.Any((string s) => eValue.Contains(s));
			string[] array = eValue.Split(splitter.ToArray(), StringSplitOptions.RemoveEmptyEntries);
			string value = "";
			string value2 = "";
			try
			{
				if (array.Length != 0)
				{
					value = array[0];
				}
				if (array.Length > 1)
				{
					value2 = array[1];
				}
			}
			catch
			{
			}
			int valueMinutes = 0;
			int valueHours = 0;
			if (flag)
			{
				valueHours = Convert.ToInt32(value);
				valueMinutes = Convert.ToInt32(value2);
			}
			else
			{
				FormatValue(text, ref valueMinutes, ref valueHours);
			}
			try
			{
				if (valueHours < 24 && valueMinutes < 60)
				{
					e.Value = ((valueHours == 0) ? "" : valueHours.ToString()) + ((valueMinutes.ToString().Length == 1) ? ("0" + valueMinutes) : valueMinutes.ToString());
				}
			}
			catch
			{
			}
			e.Handled = true;
		}
		catch
		{
		}
	}

	private static void FormatValue(string value, ref int valueMinutes, ref int valueHours)
	{
		switch (value.Length)
		{
		case 1:
			valueMinutes = int.Parse(value);
			break;
		case 2:
			valueMinutes = int.Parse(value);
			break;
		case 3:
			valueHours = int.Parse(value.Substring(0, 1));
			valueMinutes = int.Parse(value.Substring(1, 2));
			break;
		case 4:
			valueHours = int.Parse(value.Substring(0, 2));
			valueMinutes = int.Parse(value.Substring(2, 2));
			break;
		case 5:
			valueHours = int.Parse(value.Substring(1, 2));
			valueMinutes = int.Parse(value.Substring(3, 2));
			break;
		}
	}

	public static void RegisterHourEdit()
	{
		Image image = null;
		EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("HourEdit", typeof(HourEdit), typeof(RepositoryItemHourEdit), typeof(TextEditViewInfo), new TextEditPainter(), designTimeVisible: true, image));
	}

	public override void Assign(RepositoryItem item)
	{
		BeginUpdate();
		try
		{
			base.Assign(item);
			_ = item is RepositoryItemHourEdit;
		}
		finally
		{
			EndUpdate();
		}
	}
}
