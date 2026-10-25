using System;
using System.ComponentModel;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls.Interfaces;
using DevExpress.XtraRichEdit;

namespace Apzon.Controls.Editor;

[ToolboxItem(true)]
public class RichEditControlEx : RichEditControl, IEditControlEx, IControlEx
{
	private bool _isUserChangeValue;

	private bool _isKeyDown;

	private object _oldValue;

	private bool _isAddControl = true;

	private string _fieldName;

	public bool IsSearchControl { get; set; }

	public new string HtmlText
	{
		get
		{
			return base.HtmlText;
		}
		set
		{
			if (!Function.ToString(base.HtmlText).Equals(Function.ToString(value)))
			{
				base.HtmlText = value;
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

	public new string WordMLText
	{
		get
		{
			return base.WordMLText;
		}
		set
		{
			if (!Function.ToString(base.WordMLText).Equals(Function.ToString(value)))
			{
				base.WordMLText = value;
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
	public object EditValue
	{
		get
		{
			return WordMLText;
		}
		set
		{
			string text = Function.ToString(value);
			if (text.Contains("w:wordDocument"))
			{
				WordMLText = text;
			}
			else
			{
				HtmlText = text;
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

	protected override void OnDocumentLoaded(object sender, EventArgs e)
	{
		if (_isAddControl)
		{
			_isAddControl = this.AddControllProcessSource(FieldName);
		}
		base.OnDocumentLoaded(sender, e);
	}

	protected override void OnValidated(EventArgs e)
	{
		_oldValue = null;
		base.OnValidated(e);
		if (_isUserChangeValue)
		{
			this.ChangeDataSourceWhenEditValueControl(WordMLText);
			_isUserChangeValue = false;
		}
		_isKeyDown = false;
	}

	protected override void OnValidating(CancelEventArgs e)
	{
		base.OnValidating(e);
		if (_isUserChangeValue)
		{
			this.ControlValueChanging(WordMLText, _oldValue, e);
			if (e.Cancel)
			{
				_isKeyDown = false;
				_isUserChangeValue = false;
				base.Text = Function.ToString(_oldValue);
			}
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (!_isKeyDown)
		{
			_isKeyDown = true;
			_oldValue = WordMLText;
		}
		base.OnKeyDown(e);
		if (e.KeyCode == Keys.Return)
		{
			this.ChangeDataSourceWhenEditValueControl(WordMLText, Enums.EventId.OnKeyEnter);
		}
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
