using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls.Interfaces;
using Apzon.Global;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;

namespace Apzon.Controls.Editor;

[ToolboxItem(true)]
public class CalcEditEx : CalcEdit, IEditControlEx, IControlEx
{
	private bool _isUserChangeValue;

	private bool _isKeyDown;

	private object _oldValue;

	private bool _isAddControl = true;

	private string _fieldName;

	public bool IsSearchControl { get; set; }

	public override object EditValue
	{
		get
		{
			return base.EditValue;
		}
		set
		{
			if (!Function.ToString(base.EditValue).Equals(Function.ToString(value)))
			{
				base.EditValue = value;
				if (!_isKeyDown)
				{
					_isUserChangeValue = false;
					this.ChangeDataSourceWhenEditValueControl(value);
				}
				else
				{
					_isUserChangeValue = true;
				}
			}
		}
	}

	public string FieldName
	{
		get
		{
			return _fieldName;
		}
		set
		{
			_fieldName = value;
			this.AddControllProcessSource(value);
		}
	}

	[Browsable(false)]
	public IEditControlEx EditControl { get; set; }

	[Browsable(false)]
	public bool SettingReadonly { get; set; }

	[Browsable(false)]
	public bool SettingInVisible { get; set; }

	public new bool Visible
	{
		get
		{
			return base.Visible;
		}
		set
		{
			if (SettingInVisible)
			{
				base.Visible = false;
			}
			else
			{
				base.Visible = value;
			}
		}
	}

	public new bool ReadOnly
	{
		get
		{
			return base.ReadOnly;
		}
		set
		{
			if (SettingReadonly)
			{
				base.ReadOnly = true;
			}
			else
			{
				base.ReadOnly = value;
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public new RepositoryItemCalcEditEx Properties => base.Properties as RepositoryItemCalcEditEx;

	public override string EditorTypeName => "AmountEditEx";

	static CalcEditEx()
	{
		RepositoryItemCalcEditEx.RegisterAmountEditEx();
	}

	public CalcEditEx()
	{
		CustomDisplayText += CalcEditEx_CustomDisplayText;
	}

	private void CalcEditEx_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		try
		{
			if (Math.Round(Function.ParseDecimal(EditValue), Properties.NumOfDecs) == 0m)
			{
				e.DisplayText = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void OnValidating(CancelEventArgs e)
	{
		try
		{
			short? num = null;
			switch (Properties.DisplayFormatType)
			{
			case Enums.DisplayFormatType.Qty:
				num = GlobalData.GeneralSetting.QtyDec;
				break;
			case Enums.DisplayFormatType.Price:
				num = GlobalData.GeneralSetting.PriceDec;
				break;
			case Enums.DisplayFormatType.Amount:
				num = GlobalData.GeneralSetting.SumDec;
				break;
			case Enums.DisplayFormatType.Rate:
				num = GlobalData.GeneralSetting.RateDec;
				break;
			case Enums.DisplayFormatType.Percent:
				num = GlobalData.GeneralSetting.PercentDec;
				break;
			case Enums.DisplayFormatType.Unit:
				num = GlobalData.GeneralSetting.QtyDec;
				break;
			}
			Properties.IsRounding = true;
			EditValue = ((!num.HasValue) ? Function.ParseDecimal(EditValue) : Math.Round(Function.ParseDecimal(EditValue), num.Value));
			Properties.IsRounding = false;
			if (_isUserChangeValue)
			{
				this.ControlValueChanging(EditValue, _oldValue, e);
				if (e.Cancel)
				{
					_isKeyDown = false;
					_isUserChangeValue = false;
					base.EditValue = _oldValue;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override PopupBaseForm CreatePopupForm()
	{
		return new AmountEditExPopupForm(this);
	}

	protected override void InitLayout()
	{
		if (_isAddControl)
		{
			_isAddControl = this.AddControllProcessSource(FieldName);
		}
		base.InitLayout();
	}

	protected override void OnLayout(LayoutEventArgs levent)
	{
		if (_isAddControl)
		{
			_isAddControl = this.AddControllProcessSource(FieldName);
		}
		base.OnLayout(levent);
	}

	protected override void OnLoaded()
	{
		if (_isAddControl)
		{
			_isAddControl = this.AddControllProcessSource(FieldName);
		}
		base.OnLoaded();
	}

	protected override void OnValidated(EventArgs e)
	{
		_oldValue = null;
		base.OnValidated(e);
		if (_isUserChangeValue)
		{
			this.ChangeDataSourceWhenEditValueControl(EditValue);
			_isUserChangeValue = false;
		}
		_isKeyDown = false;
	}

	protected override void OnEditorKeyDown(KeyEventArgs e)
	{
		if (!_isKeyDown)
		{
			_isKeyDown = true;
			_oldValue = EditValue;
		}
		base.OnEditorKeyDown(e);
		if (e.KeyCode == Keys.Return)
		{
			this.ChangeDataSourceWhenEditValueControl(EditValue, Enums.EventId.OnKeyEnter);
		}
	}

	protected override void OnPopupShown()
	{
		if (!_isKeyDown)
		{
			_isKeyDown = true;
			_oldValue = EditValue;
		}
		base.OnPopupShown();
	}

	bool IControlEx.Focus()
	{
		return Focus();
	}

	System.Windows.Forms.Form IControlEx.FindForm()
	{
		return FindForm();
	}
}
