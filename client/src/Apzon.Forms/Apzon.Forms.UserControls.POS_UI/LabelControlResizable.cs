using System;
using System.Drawing;
using Apzon.Controls.Controls;

namespace Apzon.Forms.UserControls.POS_UI;

public class LabelControlResizable : LabelControlEx
{
	private string _longestString = "1000000000000";

	public string LongestString
	{
		get
		{
			return _longestString;
		}
		set
		{
			_longestString = value;
		}
	}

	protected override void OnResize(EventArgs e)
	{
		ScaleFont(this);
		base.OnResize(e);
	}

	private void ScaleFont(LabelControlEx lbl)
	{
		try
		{
			Graphics graphics = Graphics.FromImage(new Bitmap(1, 1));
			string text = "";
			text = _longestString;
			SizeF sizeF = graphics.MeasureString(text, lbl.Font);
			float num = (float)lbl.Height / sizeF.Height;
			float num2 = (float)lbl.Width / sizeF.Width;
			float num3 = ((num < num2) ? num : num2);
			if (num3 != 0f)
			{
				float emSize = lbl.Font.Size * num3;
				lbl.Font = new Font(lbl.Font.FontFamily, emSize, lbl.Font.Style);
			}
		}
		catch (Exception)
		{
		}
	}
}
