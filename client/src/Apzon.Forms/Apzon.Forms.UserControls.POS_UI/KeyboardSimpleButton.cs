using System;
using System.Drawing;
using Apzon.Controls.Controls;

namespace Apzon.Forms.UserControls.POS_UI;

public class KeyboardSimpleButton : SimpleButtonEx
{
	protected override void OnResize(EventArgs e)
	{
		ScaleFont(this);
		base.OnResize(e);
	}

	private void ScaleFont(SimpleButtonEx btn)
	{
		try
		{
			SizeF sizeF = Graphics.FromImage(new Bitmap(1, 1)).MeasureString("000", btn.Font);
			float num = (float)btn.Height / sizeF.Height;
			float num2 = (float)btn.Width / sizeF.Width;
			float num3 = ((num < num2) ? num : num2);
			if (num3 != 0f)
			{
				float emSize = (float)((double)btn.Font.Size * ((double)num3 - 0.2));
				btn.Font = new Font(btn.Font.FontFamily, emSize, btn.Font.Style);
			}
		}
		catch (Exception)
		{
		}
	}
}
