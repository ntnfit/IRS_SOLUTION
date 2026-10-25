using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Forms.Commons.CFL;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.UserControls.Buttons;

public class ucButtonCFLImage : System.Windows.Forms.UserControl
{
	private Enums.TypeCFL _cFLType;

	private string _cFLColumnName;

	private Enums.CFLButtonImageDataType _cFLDataType;

	public BaseEdit CFLBaseEditor;

	public bool _isSetCFLText;

	private IContainer components;

	private CustomOnPaintSimpleButton btn_CFL;

	[DefaultValue(Enums.TypeCFL.None)]
	public Enums.TypeCFL CFLType
	{
		get
		{
			return _cFLType;
		}
		set
		{
			_cFLType = value;
		}
	}

	public string CFLColumnName
	{
		get
		{
			return _cFLColumnName;
		}
		set
		{
			_cFLColumnName = value;
		}
	}

	public Enums.CFLButtonImageDataType CFLDataType
	{
		get
		{
			return _cFLDataType;
		}
		set
		{
			_cFLDataType = value;
		}
	}

	public bool IsSearchControl { get; set; }

	public bool IsSetCFLText
	{
		get
		{
			return _isSetCFLText;
		}
		set
		{
			_isSetCFLText = value;
		}
	}

	public ucButtonCFLImage()
	{
		InitializeComponent();
		btn_CFL.CustomOnPaint = ButtonOnPaint;
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		GraphicsPath graphicsPath = new GraphicsPath();
		graphicsPath.AddEllipse(0, 0, Width, Height);
		Region = new Region(graphicsPath);
		SolidBrush solidBrush = new SolidBrush(Color.FromArgb(194, 196, 203));
		Graphics graphics = CreateGraphics();
		graphics.FillEllipse(solidBrush, new Rectangle(0, 0, Width, Height));
		solidBrush.Dispose();
		graphics.Dispose();
	}

	private void ButtonOnPaint(PaintEventArgs pe, Control ctl)
	{
		if (ctl is SimpleButton simpleButton)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddEllipse(0, 0, simpleButton.Width, simpleButton.Height);
			simpleButton.Region = new Region(graphicsPath);
			simpleButton.BorderStyle = BorderStyles.NoBorder;
		}
	}

	private void btn_CFL_Click(object sender, EventArgs e)
	{
		try
		{
			if (CFLBaseEditor == null)
			{
				WinCommons.Error(Enums.MessageCode.NoRelatedControlSet, "");
				return;
			}
			string txtSeach = (_isSetCFLText ? Function.ToString(CFLBaseEditor.Text) : "");
			frm_CFL frm_CFL2 = new frm_CFL(_cFLType, false, txtSeach, "", Enums.ItemObjectType.NoItemType, null, "");
			if (ParentForm == null)
			{
				frm_CFL2.ShowDialog();
			}
			else
			{
				frm_CFL2.ShowDialog(ParentForm);
			}
			if (frm_CFL2.ChooseItem != null)
			{
				switch (_cFLDataType)
				{
				case Enums.CFLButtonImageDataType.StringType:
					CFLBaseEditor.EditValue = Function.ToString(frm_CFL2.ChooseItem[_cFLColumnName]);
					break;
				case Enums.CFLButtonImageDataType.IntegerType:
					CFLBaseEditor.EditValue = Function.ParseInt(frm_CFL2.ChooseItem[_cFLColumnName]);
					break;
				case Enums.CFLButtonImageDataType.ShortType:
					CFLBaseEditor.EditValue = Function.ParseShort(frm_CFL2.ChooseItem[_cFLColumnName]);
					break;
				}
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.Buttons.ucButtonCFLImage));
		this.btn_CFL = new Apzon.Forms.UserControls.Buttons.CustomOnPaintSimpleButton();
		base.SuspendLayout();
		this.btn_CFL.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CFL.ImageOptions.Image");
		this.btn_CFL.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.btn_CFL.Location = new System.Drawing.Point(2, 2);
		this.btn_CFL.Name = "btn_CFL";
		this.btn_CFL.Size = new System.Drawing.Size(14, 14);
		this.btn_CFL.TabIndex = 0;
		this.btn_CFL.Click += new System.EventHandler(this.btn_CFL_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.btn_CFL);
		base.Name = "ucButtonCFLImage";
		base.Size = new System.Drawing.Size(18, 18);
		base.ResumeLayout(false);
	}
}
