using System;
using System.Drawing;
using Apzon.Controls.Editor;

namespace Apzon.Forms.UserControls.POS_UI;

public class CalcEditResizable : CalcEditEx
{
	protected override void OnResize(EventArgs e)
	{
		ScaleFont(this);
		base.OnResize(e);
	}

	private void ScaleFont(CalcEditEx control)
	{
		try
		{
			Graphics graphics = Graphics.FromImage(new Bitmap(1, 1));
			string text = "";
			text = "1000000000000";
			SizeF sizeF = graphics.MeasureString(text, control.Font);
			float num = (float)control.Height / sizeF.Height;
			float num2 = (float)control.Width / sizeF.Width;
			float num3 = ((num < num2) ? num : num2);
			if (num3 != 0f)
			{
				float emSize = control.Font.Size * num3;
				control.Font = new Font(control.Font.FontFamily, emSize, control.Font.Style);
			}
		}
		catch (Exception)
		{
		}
	}
}
