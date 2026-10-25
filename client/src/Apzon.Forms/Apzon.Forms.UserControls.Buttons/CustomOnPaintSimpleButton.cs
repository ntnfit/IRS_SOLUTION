using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Apzon.Forms.UserControls.Buttons;

public class CustomOnPaintSimpleButton : SimpleButton
{
	public Action<PaintEventArgs, Control> CustomOnPaint;

	private IContainer components;

	public bool IsSearchControl { get; set; }

	public CustomOnPaintSimpleButton()
	{
		InitializeComponent();
	}

	protected override void OnPaint(PaintEventArgs pe)
	{
		base.OnPaint(pe);
		if (CustomOnPaint != null)
		{
			CustomOnPaint(pe, this);
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
		this.components = new System.ComponentModel.Container();
	}
}
