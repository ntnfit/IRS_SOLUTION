using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls.Form;
using Apzon.Controls.Interfaces;
using DevExpress.XtraEditors;

namespace Apzon.Controls.Editor;

[ToolboxItem(true)]
public class RadioGroupEx : RadioGroup, IEditControlEx, IControlEx
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
			AddControllRadioProcessTitleSource();
			this.AddControllProcessSource(value);
		}
	}

	public new string Name
	{
		get
		{
			return base.Name;
		}
		set
		{
			base.Name = value;
			this.AddControllProcessTitleSource(FieldName);
		}
	}

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

	public IEditControlEx EditControl { get; set; }

	protected override void InitLayout()
	{
		if (_isAddControl)
		{
			AddControllRadioProcessTitleSource();
			_isAddControl = this.AddControllProcessSource(FieldName);
		}
		base.InitLayout();
	}

	protected override void OnLayout(LayoutEventArgs levent)
	{
		if (_isAddControl)
		{
			AddControllRadioProcessTitleSource();
			_isAddControl = this.AddControllProcessSource(FieldName);
		}
		base.OnLayout(levent);
	}

	protected override void OnLoaded()
	{
		if (_isAddControl)
		{
			AddControllRadioProcessTitleSource();
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

	protected override void OnValidating(CancelEventArgs e)
	{
		base.OnValidating(e);
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

	public bool AddControllRadioProcessTitleSource()
	{
		try
		{
			if (!(FindForm() is XtraFormEx xtraFormEx))
			{
				return true;
			}
			if (Properties.Items == null)
			{
				return false;
			}
			List<IControlEx> lstControlTittleSource = xtraFormEx.LstControlTittleSource;
			int i;
			for (i = 0; i < Properties.Items.Count; i++)
			{
				string text = Function.ToString(Properties.Items[i].Tag);
				if (string.IsNullOrEmpty(text) && !DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
				{
					text = Name + "Item" + i;
				}
				IControlEx controlEx = lstControlTittleSource.FirstOrDefault((IControlEx t) => t is RadioGroupItemEx && (t as RadioGroupItemEx).Item == Properties.Items[i]);
				if (controlEx != null)
				{
					controlEx.FieldName = text;
				}
				else if (!string.IsNullOrEmpty(text))
				{
					new RadioGroupItemEx(Properties.Items[i], this).FieldName = text;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
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
