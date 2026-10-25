using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Controls.Interfaces;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Controls.Editor;

public class RadioGroupItemEx : IControlEx
{
	private string _fieldName;

	public string Text
	{
		get
		{
			return Item.Description;
		}
		set
		{
			Item.Description = value;
		}
	}

	public bool Visible
	{
		get
		{
			return true;
		}
		set
		{
		}
	}

	public bool ReadOnly { get; set; }

	public string FieldName
	{
		get
		{
			return _fieldName;
		}
		set
		{
			_fieldName = value;
			this.AddControllProcessTitleSource(value);
		}
	}

	public string Name { get; set; }

	[Browsable(false)]
	public bool SettingReadonly { get; set; }

	[Browsable(false)]
	public bool SettingInVisible { get; set; }

	[Browsable(false)]
	public object EditValue
	{
		get
		{
			return Item.Description;
		}
		set
		{
			Item.Description = Function.ToString(value);
		}
	}

	[Browsable(false)]
	public bool Enabled { get; set; }

	[Browsable(false)]
	public Color BackColor { get; set; }

	[Browsable(false)]
	public object Tag
	{
		get
		{
			return Item.Tag;
		}
		set
		{
			Item.Tag = value;
		}
	}

	[Browsable(false)]
	public IEditControlEx EditControl { get; set; }

	public RadioGroupItem Item { get; }

	public RadioGroupEx Group { get; }

	public RadioGroupItemEx(RadioGroupItem item, RadioGroupEx radioGroup)
	{
		if (item == null || radioGroup == null)
		{
			throw new Exception("Item Cannot be null");
		}
		Item = item;
		Group = radioGroup;
	}

	public System.Windows.Forms.Form FindForm()
	{
		return Group.FindForm();
	}

	public bool Focus()
	{
		return false;
	}
}
