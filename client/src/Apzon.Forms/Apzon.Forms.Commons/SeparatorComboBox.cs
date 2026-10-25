using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Apzon.Forms.Commons;

public class SeparatorComboBox : ComboBox
{
	private ArrayList _separators;

	private DashStyle _separatorStyle;

	private Color _separatorColor;

	private int _separatorWidth;

	private int _separatorMargin;

	private bool _autoAdjustItemHeight;

	public bool IsSearchControl { get; set; }

	[Description("Gets or sets the Separator Style")]
	[Category("Separator")]
	public DashStyle SeparatorStyle
	{
		get
		{
			return _separatorStyle;
		}
		set
		{
			_separatorStyle = value;
		}
	}

	[Description("Gets or sets the Separator Color")]
	[Category("Separator")]
	public Color SeparatorColor
	{
		get
		{
			return _separatorColor;
		}
		set
		{
			_separatorColor = value;
		}
	}

	[Description("Gets or sets the Separator Width")]
	[Category("Separator")]
	public int SeparatorWidth
	{
		get
		{
			return _separatorWidth;
		}
		set
		{
			_separatorWidth = value;
		}
	}

	[Description("Gets or sets the Separator Margin")]
	[Category("Separator")]
	public int SeparatorMargin
	{
		get
		{
			return _separatorMargin;
		}
		set
		{
			_separatorMargin = value;
		}
	}

	[Description("Gets or sets Auto Adjust Item Height")]
	[Category("Separator")]
	public bool AutoAdjustItemHeight
	{
		get
		{
			return _autoAdjustItemHeight;
		}
		set
		{
			_autoAdjustItemHeight = value;
		}
	}

	public SeparatorComboBox()
	{
		DrawMode = DrawMode.OwnerDrawVariable;
		_separatorStyle = DashStyle.Solid;
		_separators = new ArrayList();
		_separatorColor = Color.Black;
		_separatorMargin = 1;
		_separatorWidth = 1;
		_autoAdjustItemHeight = false;
	}

	public void AddString(string s)
	{
		Items.Add(s);
	}

	public void AddStringWithSeparator(string s)
	{
		Items.Add(s);
		_separators.Add(s);
	}

	public void SetSeparator(int pos)
	{
		_separators.Add(pos);
	}

	protected override void OnMeasureItem(MeasureItemEventArgs e)
	{
		if (_autoAdjustItemHeight)
		{
			e.ItemHeight += _separatorWidth;
		}
		base.OnMeasureItem(e);
	}

	protected override void OnDrawItem(DrawItemEventArgs e)
	{
		if (-1 == e.Index)
		{
			return;
		}
		bool flag = false;
		int num = 0;
		while (!flag && num < _separators.Count)
		{
			object obj = _separators[num];
			if (obj is string)
			{
				if ((string)Items[e.Index] == obj as string)
				{
					flag = true;
				}
			}
			else
			{
				int num2 = (int)obj;
				if (num2 < 0)
				{
					num2 += Items.Count;
				}
				if (e.Index == num2)
				{
					flag = true;
				}
			}
			num++;
		}
		e.DrawBackground();
		Graphics graphics = e.Graphics;
		int num3 = e.Bounds.Location.Y + _separatorWidth - 1;
		if (flag)
		{
			Pen pen = new Pen(_separatorColor, _separatorWidth);
			pen.DashStyle = _separatorStyle;
			graphics.DrawLine(pen, e.Bounds.Location.X + _separatorMargin, num3, e.Bounds.Location.X + e.Bounds.Width - _separatorMargin, num3);
			num3++;
		}
		Brush brush = ((DrawItemState.Selected == (DrawItemState.Selected & e.State)) ? SystemBrushes.HighlightText : new SolidBrush(e.ForeColor));
		graphics.DrawString((string)Items[e.Index], e.Font, brush, e.Bounds.Left, num3 + 1);
		base.OnDrawItem(e);
	}
}
