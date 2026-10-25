using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Controls.Form;
using Apzon.Controls.Interfaces;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Apzon.Controls.Controls;

public class LayoutControlItemEx : LayoutControlItem, IControlEx
{
	private bool _isAddControl = true;

	private string _fieldName;

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
			AddControllRadioProcessTitleSource();
		}
	}

	public new bool Visible
	{
		get
		{
			return base.Visible;
		}
		set
		{
		}
	}

	[Browsable(false)]
	public bool ReadOnly { get; set; }

	[Browsable(false)]
	public bool SettingReadonly { get; set; }

	[Browsable(false)]
	public bool SettingInVisible { get; set; }

	public new LayoutVisibility Visibility
	{
		get
		{
			return base.Visibility;
		}
		set
		{
			if (SettingInVisible)
			{
				base.Visibility = LayoutVisibility.Never;
			}
			else
			{
				base.Visibility = value;
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
	public Color BackColor { get; set; }

	public IEditControlEx EditControl { get; set; }

	public override void EndInit()
	{
		if (string.IsNullOrEmpty(_fieldName) && !DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
		{
			FieldName = Name;
		}
		if (_isAddControl)
		{
			_isAddControl = AddControllRadioProcessTitleSource();
		}
		base.EndInit();
	}

	public bool AddControllRadioProcessTitleSource()
	{
		try
		{
			if (!TextVisible || string.IsNullOrEmpty(Text))
			{
				return false;
			}
			XtraFormEx xtraFormEx = null;
			if (Control != null)
			{
				xtraFormEx = Control.FindForm() as XtraFormEx;
			}
			if (xtraFormEx == null)
			{
				return true;
			}
			List<IControlEx> lstControlTittleSource = xtraFormEx.LstControlTittleSource;
			if (lstControlTittleSource.Contains(this))
			{
				if (string.IsNullOrEmpty(FieldName))
				{
					lstControlTittleSource.Remove(this);
					return false;
				}
			}
			else if (!string.IsNullOrEmpty(FieldName))
			{
				lstControlTittleSource.Add(this);
			}
			if (xtraFormEx.DataSourceTittle != null)
			{
				SetValueControlFromDataSourceTitle(xtraFormEx.DataSourceTittle);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public void SetValueControlFromDataSourceTitle(DataTable sourceTitle)
	{
		try
		{
			if (sourceTitle.IsNotNull() && !string.IsNullOrEmpty(FieldName) && sourceTitle.Columns.Contains("ColumnId") && sourceTitle.Columns.Contains("ColumnName"))
			{
				DataRow dataRow = sourceTitle.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ColumnId"]).Equals(FieldName));
				if (dataRow != null)
				{
					Text = Function.ToString(dataRow["ColumnName"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public bool Focus()
	{
		return false;
	}

	public System.Windows.Forms.Form FindForm()
	{
		return null;
	}
}
