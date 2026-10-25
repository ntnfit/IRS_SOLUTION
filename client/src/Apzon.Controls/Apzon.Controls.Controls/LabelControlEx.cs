using System.ComponentModel;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls.Interfaces;
using DevExpress.XtraEditors;

namespace Apzon.Controls.Controls;

[ToolboxItem(true)]
public class LabelControlEx : LabelControl, IControlEx
{
	private bool _isAddControl = true;

	private string _fieldName;

	private const string BaseName = "LabelControlEx";

	public override string Text
	{
		get
		{
			return base.Text;
		}
		set
		{
			if (!base.Text.Equals(Function.ToString(value)))
			{
				base.Text = value;
				if (SourceType == Enums.DataSourceType.SourceValue)
				{
					this.ChangeDataSourceWhenEditValueControl(value);
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
			if (SourceType == Enums.DataSourceType.TittleValue)
			{
				this.AddControllProcessTitleSource(value);
			}
			else
			{
				this.AddControllProcessSource(value);
			}
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
			if (SourceType == Enums.DataSourceType.TittleValue && !string.IsNullOrEmpty(_fieldName) && _fieldName.Equals("LabelControlEx") && !_fieldName.Equals(value) && !DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
				FieldName = Name;
				_isAddControl = this.AddControllProcessTitleSource(FieldName);
			}
		}
	}

	public Enums.DataSourceType SourceType { get; set; }

	[Browsable(false)]
	public bool ReadOnly
	{
		get
		{
			if (EditControl != null)
			{
				return EditControl.ReadOnly;
			}
			return false;
		}
		set
		{
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

	[Browsable(false)]
	public object EditValue
	{
		get
		{
			return Text;
		}
		set
		{
			Text = Function.ToString(value);
		}
	}

	public IEditControlEx EditControl { get; set; }

	protected override void InitLayout()
	{
		if (_isAddControl)
		{
			if (SourceType == Enums.DataSourceType.TittleValue)
			{
				if (string.IsNullOrEmpty(_fieldName) && !DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
				{
					FieldName = Name;
				}
				_isAddControl = this.AddControllProcessTitleSource(FieldName);
			}
			else
			{
				_isAddControl = this.AddControllProcessSource(FieldName);
			}
		}
		base.InitLayout();
	}

	protected override void OnLayout(LayoutEventArgs levent)
	{
		if (_isAddControl)
		{
			if (SourceType == Enums.DataSourceType.TittleValue)
			{
				if (string.IsNullOrEmpty(_fieldName) && !DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
				{
					FieldName = Name;
				}
				_isAddControl = this.AddControllProcessTitleSource(FieldName);
			}
			else
			{
				_isAddControl = this.AddControllProcessSource(FieldName);
			}
		}
		base.OnLayout(levent);
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
