using System;
using System.Drawing;
using DevExpress.XtraEditors;

namespace Apzon.Forms.UserControls.POS_UI;

public class KeyboardCheckButton : CheckButton
{
	protected override void OnResize(EventArgs e)
	{
		ScaleFont(this);
		base.OnResize(e);
	}

	private void ScaleFont(CheckButton btn)
	{
		try
		{
			SizeF sizeF = Graphics.FromImage(new Bitmap(1, 1)).MeasureString(btn.Text, btn.Font);
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
