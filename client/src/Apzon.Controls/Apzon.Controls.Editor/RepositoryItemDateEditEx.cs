using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Apzon.Commons;
using Apzon.Global;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;

namespace Apzon.Controls.Editor;

[UserRepositoryItem("RegisterDateEditEx")]
public class RepositoryItemDateEditEx : RepositoryItemDateEdit
{
	private readonly string formatString = "";

	private readonly List<string> splitter = new List<string> { "/", "-", ".", "\\" };

	private readonly int defaultYear = 2000;

	private readonly string defaultFormat = "dd/MM/yyyy";

	private bool _showTime;

	private readonly string timeFormat = "HH:mm";

	public const string CustomEditName = "DateEditEx";

	public bool IsSearchControl { get; set; }

	[DefaultValue(false)]
	public bool ShowTime
	{
		get
		{
			return _showTime;
		}
		set
		{
			_showTime = value;
			ApplyDateTimeFormat();
		}
	}

	public override string EditorTypeName => "DateEditEx";

	private void ApplyDateTimeFormat()
	{
		if (LicenseManager.UsageMode != LicenseUsageMode.Designtime && !string.IsNullOrEmpty(formatString))
		{
			string text = (_showTime ? (formatString + " " + timeFormat) : formatString);
			DisplayFormat.FormatString = text;
			EditFormat.FormatString = text;
			if (_showTime)
			{
				CalendarView = CalendarView.Vista;
				CalendarTimeEditing = DefaultBoolean.True;
				CalendarTimeProperties.Mask.MaskType = MaskType.DateTime;
				CalendarTimeProperties.Mask.EditMask = timeFormat;
				CalendarTimeProperties.Mask.UseMaskAsDisplayFormat = true;
				CalendarTimeProperties.DisplayFormat.FormatType = FormatType.DateTime;
				CalendarTimeProperties.DisplayFormat.FormatString = timeFormat;
				CalendarTimeProperties.EditFormat.FormatType = FormatType.DateTime;
				CalendarTimeProperties.EditFormat.FormatString = timeFormat;
			}
			else
			{
				CalendarTimeEditing = DefaultBoolean.Default;
			}
		}
	}

	private bool TryParseTime(string timeText, out int hour, out int minute, out int second)
	{
		hour = (minute = (second = 0));
		if (string.IsNullOrWhiteSpace(timeText))
		{
			return false;
		}
		timeText = timeText.Trim();
		string[] array = timeText.Split(':');
		if (array.Length < 2)
		{
			return false;
		}
		if (!int.TryParse(new string(array[0].Where(char.IsDigit).ToArray()), out hour))
		{
			return false;
		}
		int.TryParse(new string(array[1].Where(char.IsDigit).ToArray()), out minute);
		if (array.Length >= 3)
		{
			int.TryParse(new string(array[2].Where(char.IsDigit).ToArray()), out second);
		}
		string text = timeText.ToLowerInvariant();
		if (text.Contains("pm") && hour < 12)
		{
			hour += 12;
		}
		if (text.Contains("am") && hour == 12)
		{
			hour = 0;
		}
		if (hour >= 0 && hour <= 23 && minute >= 0 && minute <= 59 && second >= 0)
		{
			return second <= 59;
		}
		return false;
	}

	static RepositoryItemDateEditEx()
	{
		RegisterDateEditEx();
	}

	public RepositoryItemDateEditEx()
	{
		if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
		{
			formatString = ((!string.IsNullOrEmpty(GlobalData.DateTimeFormat)) ? GlobalData.DateTimeFormat : defaultFormat);
			try
			{
				if (!string.IsNullOrEmpty(GlobalData.DateSeparatorFormat))
				{
					splitter.Add(GlobalData.DateSeparatorFormat);
				}
			}
			catch
			{
			}
			Mask.MaskType = MaskType.None;
			DisplayFormat.FormatType = FormatType.DateTime;
			EditFormat.FormatType = FormatType.DateTime;
			ApplyDateTimeFormat();
		}
		Validating += RepositoryItemDateEditEx_Validating;
	}

	public void RepositoryItemDateEditEx_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (sender is DateEdit dateEdit && !string.IsNullOrEmpty(Function.ToString(dateEdit.EditValue)))
			{
				if (dateEdit.DateTime == DateTime.MinValue)
				{
					e.Cancel = true;
				}
				if (dateEdit.DateTime.Year < 1753)
				{
					e.Cancel = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void RaiseParseEditValue(ConvertEditValueEventArgs e)
	{
		try
		{
			if (e.Value is DateTime)
			{
				return;
			}
			string text = Function.ToString(e.Value);
			int hour = 0;
			int minute = 0;
			int second = 0;
			bool flag = false;
			string dateText = text;
			int num = text.IndexOf(':');
			if (num >= 0)
			{
				int num2 = text.LastIndexOf(' ', num);
				string timeText;
				if (num2 >= 0)
				{
					dateText = text.Substring(0, num2);
					timeText = text.Substring(num2 + 1);
				}
				else
				{
					dateText = "";
					timeText = text;
				}
				flag = TryParseTime(timeText, out hour, out minute, out second);
			}
			string text2 = new string(dateText.Where((char c) => char.IsDigit(c)).ToArray());
			if (!int.TryParse(text2, out var _))
			{
				DateTime result2;
				if (flag)
				{
					e.Value = DateTime.UtcNow.Date.AddHours(hour).AddMinutes(minute).AddSeconds(second);
				}
				else if (text != "" && !DateTime.TryParse(text, out result2))
				{
					e.Value = DateTime.UtcNow.Date;
				}
				if (e.Value?.ToString() == string.Empty)
				{
					e.Value = null;
				}
				e.Handled = true;
				return;
			}
			bool flag2 = splitter.Any((string s) => dateText.Contains(s));
			string[] array = dateText.Split(splitter.ToArray(), StringSplitOptions.RemoveEmptyEntries);
			string text3 = "";
			string obj = "";
			string text4 = "";
			try
			{
				if (array.Length != 0)
				{
					text3 = array[0];
				}
				if (array.Length > 1)
				{
					obj = array[1];
				}
				if (array.Length > 2)
				{
					text4 = array[2];
				}
			}
			catch
			{
			}
			DateTime date = DateTime.UtcNow.Date;
			int day = date.Day;
			int month = date.Month;
			int year = date.Year;
			int valueDay = day;
			int valueMonth = month;
			int valueYear = year;
			string text5 = new string((from c in formatString.ToLower()
				where char.IsLetter(c)
				select c).ToArray());
			if (flag2)
			{
				switch (text5)
				{
				case "ddmmyy":
				case "ddmmmmyy":
					valueDay = Function.ParseInt(text3);
					valueMonth = Function.ParseInt(obj);
					valueYear = CalculateYear(text4);
					break;
				case "ddmmyyyy":
					valueDay = Function.ParseInt(text3);
					valueMonth = Function.ParseInt(obj);
					valueYear = Function.ParseInt(text4);
					break;
				case "mmddyy":
					valueMonth = Function.ParseInt(text3);
					valueDay = Function.ParseInt(obj);
					valueYear = CalculateYear(text4);
					break;
				case "mmddyyyy":
					valueMonth = Function.ParseInt(text3);
					valueDay = Function.ParseInt(obj);
					valueYear = Function.ParseInt(text4);
					break;
				case "yyyymmdd":
					if (text3.Length >= 4)
					{
						valueYear = Function.ParseInt(text3);
						valueMonth = Function.ParseInt(obj);
						valueDay = Function.ParseInt(text4);
					}
					else if (string.IsNullOrEmpty(text4))
					{
						valueMonth = Function.ParseInt(text3);
						valueDay = Function.ParseInt(obj);
					}
					else
					{
						valueYear = CalculateYear(text3);
						valueMonth = Function.ParseInt(obj);
						valueDay = Function.ParseInt(text4);
					}
					break;
				case "yymmdd":
					if (string.IsNullOrEmpty(text4))
					{
						valueMonth = Function.ParseInt(text3);
						valueDay = Function.ParseInt(obj);
					}
					else
					{
						valueYear = CalculateYear(text3);
						valueMonth = Function.ParseInt(obj);
						valueDay = Function.ParseInt(text4);
					}
					break;
				}
			}
			else
			{
				switch (text5)
				{
				case "ddmmyy":
				case "ddmmmmyy":
					Format_DD_MM_YY(text2, ref valueDay, ref valueMonth, ref valueYear);
					break;
				case "ddmmyyyy":
					Format_DD_MM_YYYY(text2, ref valueDay, ref valueMonth, ref valueYear);
					break;
				case "mmddyy":
					Format_MM_DD_YY(text2, ref valueDay, ref valueMonth, ref valueYear);
					break;
				case "mmddyyyy":
					Format_MM_DD_YYYY(text2, ref valueDay, ref valueMonth, ref valueYear);
					break;
				case "yyyymmdd":
					Format_YYYY_MM_DD(text2, ref valueDay, ref valueMonth, ref valueYear);
					break;
				case "yymmdd":
					Format_YY_MM_DD(text2, ref valueDay, ref valueMonth, ref valueYear);
					break;
				}
			}
			valueDay = ((valueDay == 0) ? day : valueDay);
			valueMonth = ((valueMonth == 0) ? month : valueMonth);
			valueYear = ((valueYear == 0) ? year : valueYear);
			try
			{
				date = new DateTime(valueYear, valueMonth, valueDay);
				if (flag)
				{
					date = date.AddHours(hour).AddMinutes(minute).AddSeconds(second);
				}
				e.Value = ((date == DateTime.MinValue) ? null : ((object)date));
			}
			catch
			{
				if (DateTime.TryParse(text, out var result3))
				{
					e.Value = ((result3 == DateTime.MinValue) ? null : ((object)result3));
				}
			}
			e.Handled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private int CalculateYear(string yearString)
	{
		int year = DateTime.Now.Year;
		if (yearString.Length > 2)
		{
			yearString = yearString.Substring(yearString.Length - 2, 2);
		}
		int num = year;
		int num2 = year % 1000;
		if (num2 > 99)
		{
			num2 %= 100;
		}
		if (Function.ParseInt(yearString) < num2 + 50)
		{
			return string.IsNullOrEmpty(yearString) ? year : (defaultYear + Function.ParseInt(yearString));
		}
		return string.IsNullOrEmpty(yearString) ? year : (defaultYear - 100 + Function.ParseInt(yearString));
	}

	private void Format_YY_MM_DD(string value, ref int valueDay, ref int valueMonth, ref int valueYear)
	{
		switch (value.Length)
		{
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
			Format_YYYY_MM_DD(value, ref valueDay, ref valueMonth, ref valueYear);
			break;
		case 7:
			valueYear = CalculateYear(value.Substring(1, 3));
			valueMonth = Function.ParseInt(value.Substring(3, 2));
			valueDay = Function.ParseInt(value.Substring(5, 2));
			break;
		case 8:
			valueYear = CalculateYear(value.Substring(2, 4));
			valueMonth = Function.ParseInt(value.Substring(4, 2));
			valueDay = Function.ParseInt(value.Substring(6, 2));
			break;
		}
	}

	private void Format_YYYY_MM_DD(string value, ref int valueDay, ref int valueMonth, ref int valueYear)
	{
		switch (value.Length)
		{
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
			Format_MM_DD_YY(value, ref valueDay, ref valueMonth, ref valueYear);
			break;
		case 6:
			valueYear = CalculateYear(value.Substring(0, 2));
			valueMonth = Function.ParseInt(value.Substring(2, 2));
			valueDay = Function.ParseInt(value.Substring(4, 2));
			break;
		case 7:
			valueYear = Function.ParseInt(value.Substring(0, 3));
			valueMonth = Function.ParseInt(value.Substring(3, 2));
			valueDay = Function.ParseInt(value.Substring(5, 2));
			break;
		case 8:
			valueYear = Function.ParseInt(value.Substring(0, 4));
			valueMonth = Function.ParseInt(value.Substring(4, 2));
			valueDay = Function.ParseInt(value.Substring(6, 2));
			break;
		}
	}

	private void Format_MM_DD_YYYY(string value, ref int valueDay, ref int valueMonth, ref int valueYear)
	{
		switch (value.Length)
		{
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
			Format_MM_DD_YY(value, ref valueDay, ref valueMonth, ref valueYear);
			break;
		case 7:
			valueMonth = Function.ParseInt(value.Substring(0, 1));
			valueDay = Function.ParseInt(value.Substring(1, 2));
			valueYear = Function.ParseInt(value.Substring(value.Length - 4, 4));
			break;
		default:
			valueMonth = Function.ParseInt(value.Substring(0, 2));
			valueDay = Function.ParseInt(value.Substring(2, 2));
			valueYear = Function.ParseInt(value.Substring(value.Length - 4, 4));
			break;
		}
	}

	private void Format_DD_MM_YYYY(string value, ref int valueDay, ref int valueMonth, ref int valueYear)
	{
		switch (value.Length)
		{
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
			Format_DD_MM_YY(value, ref valueDay, ref valueMonth, ref valueYear);
			break;
		case 7:
			valueDay = Function.ParseInt(value.Substring(0, 1));
			valueMonth = Function.ParseInt(value.Substring(1, 2));
			valueYear = Function.ParseInt(value.Substring(3, 4));
			break;
		default:
			valueDay = Function.ParseInt(value.Substring(0, 2));
			valueMonth = Function.ParseInt(value.Substring(2, 2));
			valueYear = Function.ParseInt(value.Substring(value.Length - 4, 4));
			break;
		}
	}

	private void Format_DD_MM_YY(string value, ref int valueDay, ref int valueMonth, ref int valueYear)
	{
		switch (value.Length)
		{
		case 1:
		case 2:
			valueDay = Function.ParseInt(value);
			break;
		case 3:
			valueDay = Function.ParseInt(value.Substring(0, 1));
			valueMonth = Function.ParseInt(value.Substring(1, 2));
			break;
		case 4:
			valueDay = Function.ParseInt(value.Substring(0, 2));
			valueMonth = Function.ParseInt(value.Substring(2, 2));
			break;
		case 5:
			valueDay = Function.ParseInt(value.Substring(0, 1));
			valueMonth = Function.ParseInt(value.Substring(1, 2));
			valueYear = CalculateYear(value.Substring(3, 2));
			break;
		case 6:
			valueDay = Function.ParseInt(value.Substring(0, 2));
			valueMonth = Function.ParseInt(value.Substring(2, 2));
			valueYear = CalculateYear(value.Substring(4, 2));
			break;
		case 7:
			valueDay = Function.ParseInt(value.Substring(0, 1));
			valueMonth = Function.ParseInt(value.Substring(1, 2));
			valueYear = CalculateYear(value.Substring(value.Length - 2, 2));
			break;
		default:
			valueDay = Function.ParseInt(value.Substring(0, 2));
			valueMonth = Function.ParseInt(value.Substring(2, 2));
			valueYear = CalculateYear(value.Substring(value.Length - 2, 2));
			break;
		}
	}

	private void Format_MM_DD_YY(string value, ref int valueDay, ref int valueMonth, ref int valueYear)
	{
		switch (value.Length)
		{
		case 1:
		case 2:
			valueDay = Function.ParseInt(value);
			break;
		case 3:
			valueMonth = Function.ParseInt(value.Substring(0, 1));
			valueDay = Function.ParseInt(value.Substring(1, 2));
			break;
		case 4:
			valueMonth = Function.ParseInt(value.Substring(0, 2));
			valueDay = Function.ParseInt(value.Substring(2, 2));
			break;
		case 5:
			valueMonth = Function.ParseInt(value.Substring(0, 1));
			valueDay = Function.ParseInt(value.Substring(1, 2));
			valueYear = CalculateYear(value.Substring(3, 2));
			break;
		case 6:
			valueMonth = Function.ParseInt(value.Substring(0, 2));
			valueDay = Function.ParseInt(value.Substring(2, 2));
			valueYear = CalculateYear(value.Substring(4, 2));
			break;
		case 7:
			valueMonth = Function.ParseInt(value.Substring(0, 1));
			valueDay = Function.ParseInt(value.Substring(1, 2));
			valueYear = CalculateYear(value.Substring(value.Length - 2, 2));
			break;
		default:
			valueMonth = Function.ParseInt(value.Substring(0, 2));
			valueDay = Function.ParseInt(value.Substring(2, 2));
			valueYear = CalculateYear(value.Substring(value.Length - 2, 2));
			break;
		}
	}

	public static void RegisterDateEditEx()
	{
		Image image = null;
		EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("DateEditEx", typeof(DateEditEx), typeof(RepositoryItemDateEditEx), typeof(DateEditViewInfo), new ButtonEditPainter(), designTimeVisible: true, image));
	}

	public override void Assign(RepositoryItem item)
	{
		BeginUpdate();
		try
		{
			base.Assign(item);
			if (item is RepositoryItemDateEditEx repositoryItemDateEditEx)
			{
				ShowTime = repositoryItemDateEditEx.ShowTime;
			}
		}
		finally
		{
			EndUpdate();
		}
	}
}
