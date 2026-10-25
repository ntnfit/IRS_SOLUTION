using System;
using System.ComponentModel;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls.Interfaces;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;

namespace Apzon.Controls.Editor;

[ToolboxItem(true)]
public class SpinEditEx : SpinEdit, IEditControlEx, IControlEx
{
	private string _fieldName;

	private bool _isUserChangeValue;

	private bool _isKeyDown;

	private object _oldValue;

	private bool _isAddControl = true;

	public bool IsSearchControl { get; set; }

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

	[Browsable(false)]
	public IEditControlEx EditControl { get; set; }

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

	protected override void OnPressButton(EditorButtonObjectInfoArgs buttonInfo)
	{
		if (!_isKeyDown)
		{
			_isKeyDown = true;
			_oldValue = EditValue;
		}
		base.OnPressButton(buttonInfo);
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
