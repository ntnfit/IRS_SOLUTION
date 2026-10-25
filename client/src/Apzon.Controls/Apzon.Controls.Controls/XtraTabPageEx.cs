using System.ComponentModel;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls.Interfaces;
using DevExpress.XtraTab;

namespace Apzon.Controls.Controls;

[ToolboxItem(true)]
public class XtraTabPageEx : XtraTabPage, IControlEx
{
	private bool _isAddControl = true;

	private string _fieldName;

	private const string BaseName = "XtraTabPageEx";

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
			if (SourceType == Enums.DataSourceType.TittleValue && !string.IsNullOrEmpty(_fieldName) && _fieldName.Equals("XtraTabPageEx") && !_fieldName.Equals(value) && !DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
				FieldName = Name;
				_isAddControl = this.AddControllProcessTitleSource(FieldName);
			}
		}
	}

	public Enums.DataSourceType SourceType { get; set; }

	[Browsable(false)]
	public bool ReadOnly { get; set; }

	[Browsable(false)]
	public bool SettingReadonly { get; set; }

	[Browsable(false)]
	public bool SettingInVisible { get; set; }

	public new bool Visible
	{
		get
		{
			return base.PageVisible;
		}
		set
		{
			if (SettingInVisible)
			{
				base.PageVisible = false;
			}
			else
			{
				base.PageVisible = value;
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

	[Browsable(false)]
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
