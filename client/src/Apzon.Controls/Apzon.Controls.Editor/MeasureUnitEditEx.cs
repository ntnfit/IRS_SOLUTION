using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Administrator;
using Apzon.Global;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Controls.Editor;

[ToolboxItem(true)]
public class MeasureUnitEditEx : TextEditEx
{
	private short? _unitCode;

	private string _unitDisplay;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public short? UnitCode
	{
		get
		{
			return _unitCode;
		}
		set
		{
			if (LicenseManager.UsageMode == LicenseUsageMode.Designtime || DesignMode)
			{
				return;
			}
			_unitDisplay = "";
			_unitCode = null;
			if (!value.HasValue)
			{
				_unitCode = GetDefaultUnitCode();
			}
			else
			{
				if (!ValidateUnitByCode(value))
				{
					WinCommons.Error(Enums.MessageCode.InvalidUnit, "");
					return;
				}
				_unitCode = value;
			}
			_unitDisplay = _unitCode.GetDisplayNameUnitByCode(MeasureUnitType);
			Refresh();
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string UnitDisplay
	{
		get
		{
			return _unitDisplay;
		}
		set
		{
			if (LicenseManager.UsageMode == LicenseUsageMode.Designtime || DesignMode)
			{
				return;
			}
			_unitDisplay = "";
			_unitCode = null;
			if (string.IsNullOrEmpty(value))
			{
				_unitCode = GetDefaultUnitCode();
				_unitDisplay = _unitCode.GetDisplayNameUnitByCode(MeasureUnitType);
			}
			else
			{
				string text = value.ToLower();
				if (!ValidateUnitByDisplayName(text))
				{
					WinCommons.Error(Enums.MessageCode.InvalidUnit, "");
					return;
				}
				_unitDisplay = text;
				_unitCode = _unitDisplay.GetUnitCodeByDisplayName(MeasureUnitType);
			}
			Refresh();
		}
	}

	private string UnitField { get; set; }

	[DefaultValue(Enums.MeasureType.Weight)]
	public Enums.MeasureType MeasureUnitType { get; set; }

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public new RepositoryItemMeasureUnitEditEx Properties => base.Properties as RepositoryItemMeasureUnitEditEx;

	public override string EditorTypeName => "MeasureUnitEditEx";

	static MeasureUnitEditEx()
	{
		RepositoryItemMeasureUnitEditEx.RegisterMeasureUnitEditEx();
	}

	public MeasureUnitEditEx()
	{
		Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
		CustomDisplayText += CustomDisplayTextProcess;
		KeyPress += KeyDownProcess;
	}

	protected override void OnValidating(CancelEventArgs e)
	{
		if (IsUserChangeValue)
		{
			ValueValidate(this, e);
		}
	}

	protected void ValueValidate(object sender, CancelEventArgs e)
	{
		try
		{
			ErrorText = "";
			string[] dataUnit = GetDataUnit(Function.ToString(EditValue));
			if (dataUnit != null && dataUnit.Length == 2)
			{
				decimal d = Function.ParseDecimal(dataUnit[0]);
				UnitDisplay = Function.ToString(dataUnit[1]);
				if (string.IsNullOrEmpty(UnitDisplay))
				{
					e.Cancel = true;
					ErrorText = "Invalid Unit";
				}
				else
				{
					d = Math.Round(d, GlobalData.GeneralSetting.MeasureDec.GetValueOrDefault());
					EditValue = d;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			e.Cancel = true;
			ErrorText = ex.Message;
		}
	}

	private void CustomDisplayTextProcess(object sender, CustomDisplayTextEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(_unitDisplay))
			{
				UnitCode = null;
			}
			if (Math.Abs(Function.ParseDouble(e.Value)) < 1E-07)
			{
				e.DisplayText = "";
				return;
			}
			string text = new string('#', GlobalData.GeneralSetting.MeasureDec.GetValueOrDefault());
			e.DisplayText = string.Format("{0:#,0." + text + "} {1}", e.Value, UnitDisplay);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void OnEditorEnter(EventArgs e)
	{
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

	private bool ValidateUnitByCode(short? unitCode)
	{
		if (unitCode.HasValue)
		{
			switch (MeasureUnitType)
			{
			case Enums.MeasureType.LengthAndWidth:
				return GlobalData.LengthWidthUnits.Exists((LengthAndWidthImpl t) => t.UnitCode == unitCode);
			case Enums.MeasureType.Weight:
				return GlobalData.WeightUnits.Exists((WeightImpl t) => t.UnitCode == unitCode);
			}
		}
		return false;
	}

	private bool ValidateUnitByDisplayName(string displayName)
	{
		return MeasureUnitType switch
		{
			Enums.MeasureType.LengthAndWidth => GlobalData.LengthWidthUnits.Exists((LengthAndWidthImpl t) => t.UnitDisply.Equals(displayName)), 
			Enums.MeasureType.Weight => GlobalData.WeightUnits.Exists((WeightImpl t) => t.UnitDisply.Equals(displayName)), 
			_ => false, 
		};
	}

	public static string[] GetDataUnit(string value)
	{
		try
		{
			if (string.IsNullOrEmpty(value) || value.Length == 0)
			{
				return null;
			}
			value = value.Trim().ToLower();
			char[] lstCharactor = value.ToArray();
			int num = -1;
			int i;
			for (i = 0; i < lstCharactor.Length; i++)
			{
				if (!RepositoryItemMeasureUnitEditEx.NumberArray.Any((char t) => t == lstCharactor[i]) && !(GlobalData.GeneralSetting.DecSep == Function.ToString(lstCharactor[i])) && !(GlobalData.GeneralSetting.ThousSep == Function.ToString(lstCharactor[i])))
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				return new string[2] { value, "" };
			}
			return new string[2]
			{
				value.Substring(0, num),
				value.Substring(num)
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	private short? GetDefaultUnitCode()
	{
		return MeasureUnitType switch
		{
			Enums.MeasureType.LengthAndWidth => GlobalData.GeneralSetting.DefLengthU, 
			Enums.MeasureType.Weight => GlobalData.GeneralSetting.DefWeightU, 
			_ => null, 
		};
	}
}
