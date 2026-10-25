using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls.Interfaces;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Controls.Editor;

[ToolboxItem(true)]
public class LookUpEditEx : LookUpEdit, IEditControlEx, IControlEx
{
	protected bool IsUserChangeValue;

	protected bool IsKeyDown;

	protected object OldValue;

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
				DataTable dataTable = Properties.DataSource as DataTable;
				string valueMember = Properties.ValueMember;
				object editValue;
				if (!dataTable.IsNotNull() || string.IsNullOrEmpty(valueMember) || !dataTable.Columns.Contains(valueMember))
				{
					editValue = value;
				}
				else if (!(dataTable.Columns[valueMember].DataType == typeof(int)) && !(dataTable.Columns[valueMember].DataType == typeof(int)))
				{
					if (!(dataTable.Columns[valueMember].DataType == typeof(short)) && !(dataTable.Columns[valueMember].DataType == typeof(short)))
					{
						editValue = ((!(dataTable.Columns[valueMember].DataType == typeof(long)) && !(dataTable.Columns[valueMember].DataType == typeof(long))) ? value : ((object)Function.ParseLong(value)));
					}
					else
					{
						editValue = Function.ParseShort(value);
					}
				}
				else
				{
					editValue = Function.ParseInt(value);
				}
				base.EditValue = editValue;
				if (!IsKeyDown)
				{
					IsUserChangeValue = false;
					this.ChangeDataSourceWhenEditValueControl(value);
				}
				else
				{
					IsUserChangeValue = true;
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
	public IEditControlEx EditControl { get; set; }

	public LookUpEditEx()
	{
		if ((DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime) && Properties.NullText.Equals("[EditValue is null]"))
		{
			Properties.NullText = "";
		}
		Properties.AllowMouseWheel = false;
		CustomDisplayText += OnCustomDisplayText;
	}

	private void OnCustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		string text = Function.ToString(EditValue);
		if (!string.IsNullOrEmpty(text) && (!Function.IsInt(text) || (text != "0" && text != "-1")) && Properties.DataSource != null && Properties.GetDataSourceRowByKeyValue(EditValue) == null)
		{
			ErrorText = "Value does not exist";
		}
		else
		{
			ErrorText = "";
		}
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
		OldValue = null;
		base.OnValidated(e);
		if (IsUserChangeValue)
		{
			this.ChangeDataSourceWhenEditValueControl(EditValue);
			IsUserChangeValue = false;
		}
		IsKeyDown = false;
	}

	protected override void OnValidating(CancelEventArgs e)
	{
		base.OnValidating(e);
		if (IsUserChangeValue)
		{
			this.ControlValueChanging(EditValue, OldValue, e);
			if (e.Cancel)
			{
				IsKeyDown = false;
				IsUserChangeValue = false;
				base.EditValue = OldValue;
			}
		}
	}

	protected override void OnEditorKeyDown(KeyEventArgs e)
	{
		if (!IsKeyDown)
		{
			IsKeyDown = true;
			OldValue = EditValue;
		}
		base.OnEditorKeyDown(e);
		if (e.KeyCode == Keys.Return)
		{
			this.ChangeDataSourceWhenEditValueControl(EditValue, Enums.EventId.OnKeyEnter);
		}
	}

	protected override void OnPopupShown()
	{
		if (!IsKeyDown)
		{
			IsKeyDown = true;
			OldValue = EditValue;
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
