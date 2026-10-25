using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Editor;
using Apzon.Forms.Commons.CFL;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.UserControls;

[ToolboxItem(true)]
public class ButtonEditDistrRule : ButtonEditEx
{
	private bool fromCFL;

	private bool fromDB;

	[DefaultValue(false)]
	public bool FromCFL => fromCFL;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public new RepositoryItemButtonEditDistrRule Properties => base.Properties as RepositoryItemButtonEditDistrRule;

	public override string EditorTypeName => "ButtonEditDistrRule";

	static ButtonEditDistrRule()
	{
		RepositoryItemButtonEditDistrRule.RegisterButtonEditDistrRule();
	}

	public ButtonEditDistrRule()
	{
		new SerializableAppearanceObject();
		Properties.Buttons.Clear();
		Properties.Buttons.Add(new EditorButton(ButtonPredefines.Search, "Search"));
		ButtonClick += ButtonEditDistrRule_ButtonPressed;
		Validating += ButtonEditDistrRule_Validating;
		ParseEditValue += ButtonEditDistrRule_ParseEditValue;
	}

	private void ButtonEditDistrRule_ParseEditValue(object sender, ConvertEditValueEventArgs e)
	{
		try
		{
			e.Value = RemoveLastSeparator(Function.ToString(e.Value));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ButtonEditDistrRule_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (fromCFL || fromDB)
			{
				return;
			}
			ButtonEditDistrRule buttonEditDistrRule = sender as ButtonEditDistrRule;
			if (!buttonEditDistrRule.IsModified)
			{
				return;
			}
			string text = Function.ToString(buttonEditDistrRule.EditValue);
			if (text == "")
			{
				return;
			}
			string text2 = ValidateDistrRule(text);
			fromDB = true;
			buttonEditDistrRule.EditValue = text2;
			fromDB = false;
			if (text2 != text)
			{
				BeginInvoke((Action)(() =>
				{
					ShowCFL();
				}));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			fromDB = false;
		}
	}

	private static string RemoveLastSeparator(string value)
	{
		while (value.Length > 0 && value[value.Length - 1].Equals(';'))
		{
			value = value.Remove(value.Length - 1);
		}
		return value;
	}

	private void ButtonEditDistrRule_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Caption == "Search")
			{
				ShowCFL();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ShowCFL()
	{
		try
		{
			string value = Function.ToString(EditValue);
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.DistributeRule, multiSelect: false, "");
			frm_ChooseFromList2._strOcrDistRule = Function.ToString(EditValue);
			frm_ChooseFromList2.ShowDialog(FindForm());
			DataTable dtSource = frm_ChooseFromList2._dtSource;
			string text = "";
			if (dtSource != null && dtSource.Rows.Count > 0)
			{
				for (int i = 0; i < dtSource.Rows.Count; i++)
				{
					text = text + Function.ToString(dtSource.Rows[i]["OcrCode"]) + ";";
				}
			}
			string value2 = text.Replace(";", "");
			if (string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(value2))
			{
				fromCFL = true;
				EditValue = text;
				fromCFL = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private string ValidateDistrRule(string value)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "CFL");
			string text = httpClientHelper.Get("ValidateDistrRule", new { value });
			if (!text.ToLower().Contains("error"))
			{
				return text;
			}
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text);
			WinCommons.Error(Enums.MessageCode.None, text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "";
	}

	public void SetEditValue(DataRow dr)
	{
		try
		{
			if (dr != null)
			{
				List<string> list = new List<string>();
				if (dr["OcrCode"] != null)
				{
					list.Add(Function.ToString(dr["OcrCode"]));
				}
				if (dr["OcrCode2"] != null)
				{
					list.Add(Function.ToString(dr["OcrCode2"]));
				}
				if (dr["OcrCode3"] != null)
				{
					list.Add(Function.ToString(dr["OcrCode3"]));
				}
				if (dr["OcrCode4"] != null)
				{
					list.Add(Function.ToString(dr["OcrCode4"]));
				}
				if (dr["OcrCode5"] != null)
				{
					list.Add(Function.ToString(dr["OcrCode5"]));
				}
				string editValue = string.Join(";", list);
				EditValue = editValue;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void AddValueToRow(DataRow dr)
	{
		try
		{
			if (dr == null)
			{
				return;
			}
			string text = Function.ToString(EditValue);
			List<string> list = text.Split(';').ToList();
			while (list.Count < 5)
			{
				list.Add("");
			}
			for (int i = 0; i < list.Count && i <= 5; i++)
			{
				switch (i)
				{
				case 0:
					if (dr["OcrCode"] != null)
					{
						dr["OcrCode"] = list[i];
					}
					break;
				case 1:
					if (dr["OcrCode2"] != null)
					{
						dr["OcrCode2"] = list[i];
					}
					break;
				case 2:
					if (dr["OcrCode3"] != null)
					{
						dr["OcrCode3"] = list[i];
					}
					break;
				case 3:
					if (dr["OcrCode4"] != null)
					{
						dr["OcrCode4"] = list[i];
					}
					break;
				case 4:
					if (dr["OcrCode5"] != null)
					{
						dr["OcrCode5"] = list[i];
					}
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}
}
