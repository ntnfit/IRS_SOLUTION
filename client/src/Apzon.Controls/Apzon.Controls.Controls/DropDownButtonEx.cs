using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls.Commons;
using Apzon.Controls.Compiler;
using Apzon.Controls.Form;
using Apzon.Controls.Interfaces;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;

namespace Apzon.Controls.Controls;

public class DropDownButtonEx : DropDownButton, IControlEx
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
			this.AddControllProcessSource(value);
		}
	}

	public string ItemId { get; set; }

	[Browsable(false)]
	public bool ReadOnly
	{
		get
		{
			return !Enabled;
		}
		set
		{
			Enabled = !value;
		}
	}

	[Browsable(false)]
	public bool SettingReadonly { get; set; }

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

	public new bool Enabled
	{
		get
		{
			return base.Enabled;
		}
		set
		{
			if (SettingReadonly)
			{
				base.Enabled = false;
			}
			else
			{
				base.Enabled = value;
			}
		}
	}

	[Browsable(false)]
	public IEditControlEx EditControl { get; set; }

	[Browsable(true)]
	public event OnClickAsync OnClickAsync;

	protected override void InitLayout()
	{
		if (_isAddControl)
		{
			if (string.IsNullOrEmpty(_fieldName) && !DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
				FieldName = Name;
			}
			_isAddControl = this.AddControllProcessSource(FieldName);
		}
		base.InitLayout();
	}

	protected override void OnLayout(LayoutEventArgs levent)
	{
		if (_isAddControl)
		{
			if (string.IsNullOrEmpty(_fieldName) && !DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
				FieldName = Name;
			}
			_isAddControl = this.AddControllProcessSource(FieldName);
		}
		base.OnLayout(levent);
	}

	protected override async void OnClick(EventArgs e)
	{
		try
		{
			BeforeClick(out var cancelEvent);
			if (!cancelEvent)
			{
				if (OnClickAsync != null)
				{
					await OnClickAsync(this, e);
				}
				base.OnClick(e);
				AfterClick();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void BeforeClick(out bool cancelEvent)
	{
		cancelEvent = false;
		if (!(FindForm() is XtraFormEx xtraFormEx) || string.IsNullOrEmpty(GlobalData.SessionId) || string.IsNullOrEmpty(xtraFormEx.FormId))
		{
			return;
		}
		if (xtraFormEx.SyntaxBeforeClick == null)
		{
			string text = xtraFormEx.FormId;
			if (text.Equals("oUserDefinedReports"))
			{
				try
				{
					text += ((dynamic)xtraFormEx).ItemId;
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				}
			}
			string formulaText = xtraFormEx.HttpClient.Formula.GetFormulaText(xtraFormEx.FormId, Enums.EventId.OnBeforeClick, text);
			xtraFormEx.SyntaxBeforeClick = ((!string.IsNullOrEmpty(formulaText)) ? ControlFunction.GetToken(formulaText) : new List<Token>());
		}
		if (xtraFormEx.SyntaxBeforeClick.Count != 0)
		{
			CtdMessage ctdMessage = new Apzon.Controls.Compiler.Compiler(Configuration.ApiServer, GlobalData.SessionId, -1, FieldName).UserdefinedFomular(xtraFormEx, xtraFormEx.SyntaxBeforeClick, out cancelEvent);
			if (!string.IsNullOrEmpty(ctdMessage.Message))
			{
				WinCommons.Warning(Enums.MessageCode.None, ctdMessage.Message);
			}
		}
	}

	public void AfterClick()
	{
		if (!(FindForm() is XtraFormEx xtraFormEx) || string.IsNullOrEmpty(GlobalData.SessionId) || string.IsNullOrEmpty(xtraFormEx.FormId))
		{
			return;
		}
		if (xtraFormEx.SyntaxAfterClick == null)
		{
			string text = xtraFormEx.FormId;
			if (text.Equals("oUserDefinedReports"))
			{
				try
				{
					text += ((dynamic)xtraFormEx).ItemId;
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				}
			}
			string formulaText = xtraFormEx.HttpClient.Formula.GetFormulaText(xtraFormEx.FormId, Enums.EventId.OnAfterClick, text);
			xtraFormEx.SyntaxAfterClick = ((!string.IsNullOrEmpty(formulaText)) ? ControlFunction.GetToken(formulaText) : new List<Token>());
		}
		if (xtraFormEx.SyntaxAfterClick.Count != 0)
		{
			CtdMessage ctdMessage = new Apzon.Controls.Compiler.Compiler(Configuration.ApiServer, GlobalData.SessionId, -1, FieldName).UserdefinedFomular(xtraFormEx, xtraFormEx.SyntaxAfterClick, out var _);
			if (!string.IsNullOrEmpty(ctdMessage.Message))
			{
				WinCommons.Warning(Enums.MessageCode.None, ctdMessage.Message);
			}
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
