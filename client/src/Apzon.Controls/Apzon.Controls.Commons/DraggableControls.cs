using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Apzon.Controls.Commons;

public static class DraggableControls
{
	private static readonly Dictionary<Control, bool> Draggables = new Dictionary<Control, bool>();

	private static Size _mouseOffset;

	public static void Draggable(this Control control, bool Enable)
	{
		if (Enable)
		{
			if (!Draggables.ContainsKey(control))
			{
				Draggables.Add(control, value: false);
				control.MouseDown += control_MouseDown;
				control.MouseUp += control_MouseUp;
				control.MouseMove += control_MouseMove;
			}
		}
		else if (Draggables.ContainsKey(control))
		{
			control.MouseDown -= control_MouseDown;
			control.MouseUp -= control_MouseUp;
			control.MouseMove -= control_MouseMove;
			Draggables.Remove(control);
		}
	}

	private static void control_MouseDown(object sender, MouseEventArgs e)
	{
		_mouseOffset = new Size(e.Location);
		Draggables[(Control)sender] = true;
	}

	private static void control_MouseUp(object sender, MouseEventArgs e)
	{
		Draggables[(Control)sender] = false;
	}

	private static void control_MouseMove(object sender, MouseEventArgs e)
	{
		if (Draggables[(Control)sender])
		{
			Point point = e.Location - _mouseOffset;
			((Control)sender).Left += point.X;
			((Control)sender).Top += point.Y;
		}
	}
}
