using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.UserControls.Buttons;

public class ucButtonCustomShape : System.Windows.Forms.UserControl
{
	private Enums.ButtonCustomShapeShape _tShape;

	private Enums.ButtonCustomShapeDirection _tDirect = Enums.ButtonCustomShapeDirection.Down;

	private Color _tColor = Color.Black;

	public Action<object, EventArgs> CustomOnClick;

	private IContainer components;

	private CustomOnPaintSimpleButton btn_CustomShape;

	public Enums.ButtonCustomShapeShape TShape
	{
		get
		{
			return _tShape;
		}
		set
		{
			_tShape = value;
		}
	}

	public Enums.ButtonCustomShapeDirection TDirect
	{
		get
		{
			return _tDirect;
		}
		set
		{
			_tDirect = value;
		}
	}

	public Color TColor
	{
		get
		{
			return _tColor;
		}
		set
		{
			_tColor = value;
		}
	}

	public bool IsSearchControl { get; set; }

	public ucButtonCustomShape()
	{
		InitializeComponent();
		btn_CustomShape.CustomOnPaint = ButtonOnPaint;
	}

	protected override void OnPaint(PaintEventArgs e)
	{
	}

	private void ButtonOnPaint(PaintEventArgs pe, Control ctl)
	{
		if (!(ctl is SimpleButton simpleButton))
		{
			return;
		}
		GraphicsPath graphicsPath = new GraphicsPath();
		List<Point> list = new List<Point>();
		pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
		switch (TShape)
		{
		case Enums.ButtonCustomShapeShape.Triangle:
			switch (TDirect)
			{
			case Enums.ButtonCustomShapeDirection.Up:
				list = new List<Point>
				{
					new Point(0, simpleButton.Height),
					new Point(simpleButton.Width, simpleButton.Height),
					new Point(simpleButton.Width / 2, 0)
				};
				break;
			case Enums.ButtonCustomShapeDirection.Down:
				list = new List<Point>
				{
					new Point(0, 0),
					new Point(simpleButton.Width, 0),
					new Point(simpleButton.Width / 2, simpleButton.Height)
				};
				break;
			}
			graphicsPath.AddPolygon(list.ToArray());
			FillButton(TShape, pe, list.ToArray());
			break;
		case Enums.ButtonCustomShapeShape.Ellipse:
			graphicsPath.AddEllipse(0, 0, simpleButton.Width, simpleButton.Height);
			FillButton(TShape, pe, null, new int[4] { 0, 0, simpleButton.Width, simpleButton.Height });
			break;
		case Enums.ButtonCustomShapeShape.Rectangle:
			graphicsPath.AddRectangle(new Rectangle(0, 0, simpleButton.Width, simpleButton.Height));
			FillButton(TShape, pe, null, new int[4] { 0, 0, simpleButton.Width, simpleButton.Height });
			break;
		}
		simpleButton.Region = new Region(graphicsPath);
		simpleButton.BorderStyle = BorderStyles.NoBorder;
	}

	private void btn_CustomShape_Click(object sender, EventArgs e)
	{
		try
		{
			if (CustomOnClick != null)
			{
				CustomOnClick(sender, e);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ucButtonCustomShape_Resize(object sender, EventArgs e)
	{
		try
		{
			if (Width > 8 && Height > 8)
			{
				btn_CustomShape.Width = Width - 4;
				btn_CustomShape.Height = Height - 4;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void FillButton(Enums.ButtonCustomShapeShape shape, PaintEventArgs pe, Point[] arrPoint = null, int[] LTRB = null)
	{
		try
		{
			using SolidBrush brush = new SolidBrush(TColor);
			switch (shape)
			{
			case Enums.ButtonCustomShapeShape.Triangle:
				pe.Graphics.FillPolygon(brush, arrPoint);
				break;
			case Enums.ButtonCustomShapeShape.Ellipse:
				pe.Graphics.FillEllipse(brush, Rectangle.FromLTRB(LTRB[0], LTRB[1], LTRB[2], LTRB[3]));
				break;
			case Enums.ButtonCustomShapeShape.Rectangle:
				pe.Graphics.FillRectangle(brush, Rectangle.FromLTRB(LTRB[0], LTRB[1], LTRB[2], LTRB[3]));
				break;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.Buttons.ucButtonCustomShape));
		this.btn_CustomShape = new Apzon.Forms.UserControls.Buttons.CustomOnPaintSimpleButton();
		base.SuspendLayout();
		this.btn_CustomShape.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CustomShape.ImageOptions.Image");
		this.btn_CustomShape.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
		this.btn_CustomShape.Location = new System.Drawing.Point(2, 2);
		this.btn_CustomShape.Name = "btn_CustomShape";
		this.btn_CustomShape.Size = new System.Drawing.Size(10, 10);
		this.btn_CustomShape.TabIndex = 0;
		this.btn_CustomShape.Click += new System.EventHandler(this.btn_CustomShape_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.btn_CustomShape);
		base.Name = "ucButtonCustomShape";
		base.Size = new System.Drawing.Size(14, 14);
		base.Resize += new System.EventHandler(this.ucButtonCustomShape_Resize);
		base.ResumeLayout(false);
	}
}
