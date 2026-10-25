using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls.Interfaces;
using DevExpress.XtraEditors;

namespace Apzon.Controls.Controls;

[ToolboxItem(true)]
public class LabelLine : XtraUserControl, IControlEx
{
	private bool _isAddControl = true;

	private string _fieldName;

	private const string BaseName = "LabelLine";

	private bool _hideWhenEmpty;

	private bool _showUnderLine = true;

	private IContainer components;

	private Label lblLine;

	private LabelControl lblInfo;

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
			if (SourceType == Enums.DataSourceType.TittleValue && !string.IsNullOrEmpty(_fieldName) && _fieldName.Equals("LabelLine") && !_fieldName.Equals(value) && !DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
				FieldName = Name;
				_isAddControl = this.AddControllProcessTitleSource(FieldName);
			}
		}
	}

	public Enums.DataSourceType SourceType { get; set; }

	public string Utext
	{
		get
		{
			return lblInfo.Text;
		}
		set
		{
			if (!lblInfo.Text.Equals(Function.ToString(value)))
			{
				lblInfo.Text = value;
				if (SourceType == Enums.DataSourceType.SourceValue)
				{
					this.ChangeDataSourceWhenEditValueControl(value);
				}
				if (_hideWhenEmpty)
				{
					Visible = !string.IsNullOrWhiteSpace(value);
				}
			}
		}
	}

	public bool HideWhenEmpty
	{
		get
		{
			return _hideWhenEmpty;
		}
		set
		{
			_hideWhenEmpty = value;
			Invalidate();
		}
	}

	public bool ShowUnderLine
	{
		get
		{
			return _showUnderLine;
		}
		set
		{
			_showUnderLine = value;
			Invalidate();
		}
	}

	public Color LineColor
	{
		get
		{
			return lblLine.BackColor;
		}
		set
		{
			lblLine.BackColor = value;
		}
	}

	public IEditControlEx EditControl { get; set; }

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

	[Browsable(false)]
	public object EditValue
	{
		get
		{
			return Utext;
		}
		set
		{
			Utext = Function.ToString(value);
		}
	}

	[Browsable(false)]
	public override string Text
	{
		get
		{
			return Utext;
		}
		set
		{
			Utext = value;
		}
	}

	public LabelLine()
	{
		InitializeComponent();
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		lblInfo.Width = Width;
		lblLine.Width = Width;
		lblLine.Location = new Point(0, lblInfo.Size.Height + 3);
	}

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

	protected override void OnLoad(EventArgs e)
	{
		try
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
			base.OnLoad(e);
			lblLine.Width = Width;
			lblLine.Location = new Point(0, lblInfo.Size.Height + 3);
			if (_hideWhenEmpty)
			{
				Visible = !string.IsNullOrWhiteSpace(lblInfo.Text);
			}
			lblLine.Visible = _showUnderLine;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.lblLine = new System.Windows.Forms.Label();
		this.lblInfo = new DevExpress.XtraEditors.LabelControl();
		base.SuspendLayout();
		this.lblLine.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lblLine.BackColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblLine.Location = new System.Drawing.Point(0, 16);
		this.lblLine.Name = "lblLine";
		this.lblLine.Size = new System.Drawing.Size(80, 1);
		this.lblLine.TabIndex = 1;
		this.lblLine.Text = "label2";
		this.lblInfo.Location = new System.Drawing.Point(2, 2);
		this.lblInfo.Name = "lblInfo";
		this.lblInfo.Size = new System.Drawing.Size(63, 13);
		this.lblInfo.TabIndex = 1;
		this.lblInfo.Text = "labelControl1";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.lblInfo);
		base.Controls.Add(this.lblLine);
		this.Name = "LabelLine";
		base.Size = new System.Drawing.Size(80, 20);
		base.ResumeLayout(false);
		base.PerformLayout();
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
