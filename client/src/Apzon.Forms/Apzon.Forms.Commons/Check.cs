using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using DevExpress.XtraEditors;

namespace Apzon.Forms.Commons;

internal class Check
{
	internal static bool _text(TextEdit _txt)
	{
		try
		{
			_txt.Text.Replace("'", "");
			if (_txt.Text.Trim().Length == 0)
			{
				_txt.ErrorText = "Can not empty !";
				_txt.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_txt.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_txt.Focus();
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
			return false;
		}
	}

	internal static bool _text(TextEdit _txt, string objName)
	{
		try
		{
			_txt.Text.Replace("'", "");
			if (_txt.Text.Trim().Length == 0)
			{
				_txt.ErrorText = string.Format("{0} {1}", objName, "Can not empty !");
				_txt.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_txt.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_txt.Focus();
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
			return false;
		}
	}

	internal static bool _file_exist(TextEdit _txt)
	{
		try
		{
			if (!FileIO.FileExist(_txt.Text.Trim()))
			{
				_txt.ErrorText = "File not exist!";
				_txt.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_txt.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_txt.Focus();
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
			return false;
		}
	}

	internal static bool _combobox(ComboBoxEdit _cb)
	{
		try
		{
			if (_cb.SelectedIndex == -1)
			{
				_cb.ErrorText = "Not select value yet!";
				_cb.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_cb.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_cb.Focus();
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
			return false;
		}
	}

	internal static bool _CheckedListBoxControl(CheckedListBoxControl clb)
	{
		try
		{
			if (clb.CheckedItemsCount > 0)
			{
				return true;
			}
			clb.Focus();
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
			return false;
		}
	}

	internal static bool _combobox(ComboBoxEdit _cb, string _text)
	{
		try
		{
			if (_cb.SelectedText.Equals(_text))
			{
				_cb.ErrorText = "Not select value yet!";
				_cb.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_cb.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_cb.Focus();
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
			return false;
		}
	}

	internal static bool _checkedComboBoxEdit(CheckedComboBoxEdit _cb)
	{
		try
		{
			bool flag = false;
			for (int i = 0; i < _cb.Properties.Items.Count; i++)
			{
				if (_cb.Properties.Items[i].CheckState == CheckState.Checked)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				return true;
			}
			_cb.ErrorText = "Not select value yet!";
			_cb.ErrorIcon = BaseEdit.DefaultErrorIcon;
			_cb.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
			_cb.Focus();
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
			return false;
		}
	}

	internal static bool _calc(CalcEdit _cal)
	{
		try
		{
			if (_cal.Text.Trim().Length == 0)
			{
				_cal.ErrorText = "Not value";
				_cal.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_cal.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_cal.Focus();
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
			return false;
		}
	}

	internal static bool _checkIsNumber(TextEdit txt)
	{
		try
		{
			if (!int.TryParse(txt.Text, out var _))
			{
				txt.ErrorText = "Not number";
				txt.ErrorIcon = BaseEdit.DefaultErrorIcon;
				txt.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				txt.Focus();
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
			return false;
		}
	}

	internal static bool _spin(SpinEdit _spin)
	{
		try
		{
			if (_spin.Text.Equals(""))
			{
				_spin.ErrorText = "Not value";
				_spin.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_spin.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_spin.Focus();
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
			return false;
		}
	}

	internal static bool _time(TimeEdit _time)
	{
		try
		{
			if (_time.Text.Trim().Equals("00:00"))
			{
				_time.ErrorText = "Incorrect Fomat";
				_time.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_time.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_time.Focus();
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
			return false;
		}
	}

	internal static bool _date(DateEdit _date)
	{
		try
		{
			if (_date.Text.Trim().Length == 0)
			{
				_date.ErrorText = "Can not empty !";
				_date.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_date.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_date.Focus();
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
			return false;
		}
	}

	internal static bool _email(TextEdit _txt)
	{
		try
		{
			if (_txt.Text.Trim().Length == 0)
			{
				_txt.ErrorText = "Can not empty !";
				_txt.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_txt.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_txt.Focus();
				return false;
			}
			if (new Regex("^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\\-+)|([A-Za-z0-9]+\\.+)|([A-Za-z0-9]+\\++))*[A-Za-z0-9]+@((\\w+\\-+)|(\\w+\\.))*\\w{1,63}\\.[a-zA-Z]{2,6}$").IsMatch(_txt.Text))
			{
				return true;
			}
			_txt.ErrorText = "Incorrect Fomat";
			_txt.ErrorIcon = BaseEdit.DefaultErrorIcon;
			_txt.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
			_txt.Focus();
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
			return false;
		}
	}

	internal static bool _phone(TextEdit _txt)
	{
		try
		{
			if (_txt.Text.Trim().Length == 0)
			{
				_txt.ErrorText = "Can not empty !";
				_txt.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_txt.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_txt.Focus();
				return false;
			}
			if (new Regex("(^[+][0-9]\\d{6,12}$)|(^\\d{6,12}$)").IsMatch(_txt.Text))
			{
				return true;
			}
			_txt.ErrorText = "Incorrect Fomat";
			_txt.ErrorIcon = BaseEdit.DefaultErrorIcon;
			_txt.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
			_txt.Focus();
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
			return false;
		}
	}

	internal static bool _isemail(TextEdit _txt)
	{
		try
		{
			if (_txt.Text.Trim().Length != 0)
			{
				if (new Regex("^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\\-+)|([A-Za-z0-9]+\\.+)|([A-Za-z0-9]+\\++))*[A-Za-z0-9]+@((\\w+\\-+)|(\\w+\\.))*\\w{1,63}\\.[a-zA-Z]{2,6}$").IsMatch(_txt.Text))
				{
					return true;
				}
				_txt.ErrorText = "Incorrect Fomat";
				_txt.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_txt.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_txt.Focus();
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
			return false;
		}
	}

	internal static bool _isphone(TextEdit _txt)
	{
		try
		{
			if (_txt.Text.Trim().Length != 0)
			{
				if (new Regex("(^[+][0-9]\\d{6,12}$)|(^\\d{6,12}$)").IsMatch(_txt.Text))
				{
					return true;
				}
				_txt.ErrorText = "Incorrect Fomat";
				_txt.ErrorIcon = BaseEdit.DefaultErrorIcon;
				_txt.ErrorIconAlignment = ErrorIconAlignment.MiddleRight;
				_txt.Focus();
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
			return false;
		}
	}

	public static DateTime ToDate(string x, string kieu)
	{
		try
		{
			int num = x.IndexOf('/');
			int num2 = x.LastIndexOf('/');
			int day;
			int month;
			int year;
			if (kieu.Equals("MM/dd/yyyy"))
			{
				day = int.Parse(x.Substring(num + 1, num2 - num - 1));
				month = int.Parse(x.Substring(0, num));
				year = int.Parse(x.Substring(num2 + 1, 4));
			}
			else if (kieu.Equals("yyyyMMdd"))
			{
				day = int.Parse(x.Substring(6, 2));
				month = int.Parse(x.Substring(4, 2));
				year = int.Parse(x.Substring(0, 4));
			}
			else
			{
				day = int.Parse(x.Substring(0, num));
				month = int.Parse(x.Substring(num + 1, num2 - num - 1));
				year = int.Parse(x.Substring(num2 + 1, 4));
			}
			return new DateTime(year, month, day);
		}
		catch (Exception ex)
		{
			throw new Exception("Sai kiểu ngày tháng" + ex.Message);
		}
	}

	internal static void _onlyNumber(KeyPressEventArgs e)
	{
		if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
		{
			e.Handled = true;
		}
	}
}
